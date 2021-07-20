using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using bulkio.Entities;
namespace bulkio
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<ConsoleOptions>(args)
                .WithParsed<ConsoleOptions>(Run);
        }
        static void Run(ConsoleOptions opts)
        {
            // string site_opts_str = await File.ReadAllTextAsync("./site_options.json");
            SiteDB siteSupportEntries = new SiteDB() 
            {
                new SiteSupportEntry() 
                {
                    SiteAliases = new List<string> { "safebooru" },
                    BaseURI = "https://safebooru.org/",
                    PostUriTemplate = "index.php?page=post&s=list&tags={tags}{pagination}",
                    UseBaseUriInSearch = true,
                    Selectors = new SelectorArray 
                    {
                        PostLink = "span.thumb[id] a[id][href*=\"s=view\"]",
                        UsePostLinkForImageUri = false,
                        DefaultImage = "#image",
                        HasDownsizing = true,
                        MaximumResImage = "[href*=image]",
                        UseNoImagesSelector = true,
                        NoImagesSelector = "#content div h1"
                    },
                    Pagination = new PaginationSetup() 
                    {
                        PaginationIncrease = 40,
                        PaginationStart = 0,
                        UriParameter = "&pid=",
                        IncludeParameterOnFirstPage = false
                    },
                    Censorship = new CensorshipOptions()
                    {
                        Use = false
                    }
                }
            };
            new Downloader(new Skimmer(siteSupportEntries, opts).Run()).DownloadAsync(opts.Out).GetAwaiter().GetResult();
        }
    }
}
