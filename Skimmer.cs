using System;
using bulkio.Entities;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Net.Http;
using Fizzler.Systems.HtmlAgilityPack;
using System.Globalization;
using System.Web;
using System.Linq;
using bulkio.Extensions;
namespace bulkio
{
    public class Skimmer
    {
        SiteSupportEntry SiteOptions;
        ConsoleOptions Options;
        HttpClient client = new HttpClient();
        List<string> Tags = new List<string>() { };
        public Skimmer(SiteDB siteSupportEntries, ConsoleOptions opts)
        {

            Options = opts;
            Console.Write("\r[Warning] Censorship is not implemented yet, so exit if you don't know what you'll get.");
            if (
                siteSupportEntries.FindAll(x =>
                {
                    return x.SiteAliases.Contains(opts.Service);
                }).Count == 0
            )
                throw new Exception("Service " + opts.Service + " not implemented, exiting. (default skimmer coming soon)");
            SiteOptions = siteSupportEntries.Find(x =>
            {
                return x.SiteAliases.Contains(opts.Service);
            });
            foreach (string tag in opts.Tags)
            {
                if (TestTag(tag))
                {
                    Tags.Add(tag);
                }
                else
                {
                    Console.Write("\rNo images found for tag " + tag + ", discarding tag.");
                }
            }
        }
        public IEnumerable<string> IteratePageUri(List<string> tags)
        {
            int count = SiteOptions.Pagination.PaginationStart;
            if (count == SiteOptions.Pagination.PaginationStart && !SiteOptions.Pagination.IncludeParameterOnFirstPage)
            {
                count += SiteOptions.Pagination.PaginationIncrease;
                yield return SiteOptions.BaseURI + SiteOptions.PostUriTemplate.Replace("{tags}", String.Join('+', tags)).Replace("{pagination}", "");
            }
            else
            {
                count += SiteOptions.Pagination.PaginationIncrease;
                yield return SiteOptions.BaseURI + SiteOptions.PostUriTemplate.Replace("{tags}", String.Join('+', tags)).Replace("{pagination}",
                    SiteOptions.Pagination.UriParameter + (count - SiteOptions.Pagination.PaginationIncrease).ToString()
                );
            }
        }
        public bool TestTag(string tag)
        {
            foreach (string page in IteratePageUri(new List<string> { tag }))
            {
                foreach (HtmlNode element in IterateSearchPageElements(page))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        public IEnumerable<HtmlNode> IterateSearchPageElements(string uri)
        {
            string content = client.GetAsync(uri).GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult();
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(content);
            HtmlNode document = html.DocumentNode;
            IEnumerable<HtmlNode> enu = document.QuerySelectorAll(SiteOptions.Selectors.PostLink);
            return enu;
        }
        public string GetImageUriFromPageUri(string uri)
        {

            uri = uri.Replace("&amp;", "&"); // why do i need to do this? no idea.
            string content = client.GetAsync(uri).GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult();
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(content);
            HtmlNode document = html.DocumentNode;
            if (SiteOptions.Selectors.HasDownsizing && document.QuerySelector(SiteOptions.Selectors.MaximumResImage) != null)
                return document.QuerySelector(SiteOptions.Selectors.MaximumResImage).GetAttributeValue("href", "");
            return document.QuerySelector(SiteOptions.Selectors.DefaultImage).GetAttributeValue("src", "");
        }
        public List<string> Run()
        {
            List<string> final = new List<string>() { };
            foreach (string uri in IteratePageUri(Tags))
            {
                bool good_page = false;
                foreach (HtmlNode node in IterateSearchPageElements(uri))
                {
                    good_page = true;

                    final.Add(GetImageUriFromPageUri($"{(SiteOptions.UseBaseUriInSearch ? SiteOptions.BaseURI : "")}{node.GetAttributeValue("href", "")}"));
                    Console.Write(
                        "\r" +
                        "[" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(SiteOptions.SiteAliases[0]) + "] " +
                        "Found image " + final.Count.ToString() +
                        " out of " + Options.Count.ToString() +
                        ". " +
                        $"{(Options.Count == -1 ? "" : $"{Options.Count - final.Count} images remaining.")}"
                    );
                    if (final.Count >= Options.Count)
                        return final;
                }
                if (!good_page)
                {
                    Console.Write("\rNo remaining images.");
                    return final;
                }
            }
            return new List<string>() { };
        }
    }
#nullable enable
    class SuperSkimmer
    {
        SiteDB siteSupportEntries;

        SiteSupportEntry SiteOptions;
        ConsoleOptions Options;

        HttpClient client = new HttpClient();
        List<string> Tags = new List<string>() { };
        public SuperSkimmer(SiteDB siteSupportEntries, ConsoleOptions opts)
        {

            this.siteSupportEntries = siteSupportEntries;
            this.Options = opts;
            if (siteSupportEntries.FindAll((x) => { return x.SiteAliases.Contains(Options.Service); }).Count == 0)
                SiteOptions = this.DetectSupport(Options.Service);
            else
                SiteOptions = siteSupportEntries.FindAll((x) => { return x.SiteAliases.Contains(Options.Service); })[0];
            foreach (string tag in opts.Tags)
            {
                if (TestTag(tag))
                {
                    Tags.Add(tag);
                }
                else
                {
                    Console.Write("\rNo images found for tag " + tag + ", discarding tag.");
                }
            }
        }
        #region Skimmer code, nothing new here.
        public IEnumerable<string> IteratePageUri(List<string> tags)
        {
            int count = SiteOptions.Pagination.PaginationStart;
            if (count == SiteOptions.Pagination.PaginationStart && !SiteOptions.Pagination.IncludeParameterOnFirstPage)
            {
                count += SiteOptions.Pagination.PaginationIncrease;
                yield return SiteOptions.BaseURI + SiteOptions.PostUriTemplate.Replace("{tags}", String.Join('+', tags)).Replace("{pagination}", "");
            }
            else
            {
                count += SiteOptions.Pagination.PaginationIncrease;
                yield return SiteOptions.BaseURI + SiteOptions.PostUriTemplate.Replace("{tags}", String.Join('+', tags)).Replace("{pagination}",
                    SiteOptions.Pagination.UriParameter + (count - SiteOptions.Pagination.PaginationIncrease).ToString()
                );
            }
        }
        public bool TestTag(string tag)
        {
            foreach (string page in IteratePageUri(new List<string> { tag }))
            {
                foreach (HtmlNode element in IterateSearchPageElements(page))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        public IEnumerable<HtmlNode> IterateSearchPageElements(string uri)
        {
            string content = client.GetAsync(uri).GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult();
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(content);
            HtmlNode document = html.DocumentNode;
            IEnumerable<HtmlNode> enu = document.QuerySelectorAll(SiteOptions.Selectors.PostLink);
            return enu;
        }
        public string GetImageUriFromPageUri(string uri)
        {

            uri = uri.Replace("&amp;", "&"); // why do i need to do this? no idea.
            string content = client.GetAsync(uri).GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult();
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(content);
            HtmlNode document = html.DocumentNode;
            if (SiteOptions.Selectors.HasDownsizing && document.QuerySelector(SiteOptions.Selectors.MaximumResImage) != null)
                return document.QuerySelector(SiteOptions.Selectors.MaximumResImage).GetAttributeValue("href", "");
            return document.QuerySelector(SiteOptions.Selectors.DefaultImage).GetAttributeValue("src", "");
        }
        public List<string> Run()
        {
            List<string> final = new List<string>() { };
            foreach (string uri in IteratePageUri(Tags))
            {
                bool good_page = false;
                foreach (HtmlNode node in IterateSearchPageElements(uri))
                {
                    good_page = true;

                    final.Add(GetImageUriFromPageUri($"{(SiteOptions.UseBaseUriInSearch ? SiteOptions.BaseURI : "")}{node.GetAttributeValue("href", "")}"));
                    Console.Write(
                        "\r" +
                        "[" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(SiteOptions.SiteAliases[0]) + "] " +
                        "Found image " + final.Count.ToString() +
                        " out of " + Options.Count.ToString() +
                        ". " +
                        $"{(Options.Count == -1 ? "" : $"{Options.Count - final.Count} images remaining.")}"
                    );
                    if (final.Count >= Options.Count)
                        return final;
                }
                if (!good_page)
                {
                    Console.Write("\rNo remaining images.");
                    return final;
                }
            }
            return new List<string>() { };
        }
        #endregion
        #region Autoskimmer code, not working yet.
        private SiteSupportEntry DetectSupport(string service)
        {
            var d = new HtmlDocument();
            d.LoadHtml(client.PostAsync("https://html.duckduckgo.com/html/",
            new FormUrlEncodedContent(new List<KeyValuePair<string?, string?>>() {
                new KeyValuePair<string?, string?>("q", service),
                new KeyValuePair<string?, string?>("b", null)
            })).GetAwaiter().GetResult().Content
            .ReadAsStringAsync().GetAwaiter().GetResult());
            HtmlNode document = d.DocumentNode;
            string site = this.SelectHref(document.QuerySelectorAll("a.result__snippet"));
            d.LoadHtml(client.Get(site).Content.ReadAsString());
            HtmlNode form = document.QuerySelector("form");
            if (form.GetAttributeValue("action", "a").Contains("post"))
            {
                string BaseURI = new Uri(site).Host;
                HtmlNode tagInput = document.QuerySelector("form input[type=text]");
                string PostUriTemplate = "/" +
                form.GetAttributeValue("action", "a") + "?" +
                tagInput.GetAttributeValue("name", "tags") + "{tags}{pagination}";
                bool UseBaseUriInSearch = true;
                d.LoadHtml(client.Get(BaseURI + PostUriTemplate
                .Replace("{tags}", Options.TestTag).Replace("{pagination}", ""))
                .Content.ReadAsString());
                IEnumerable<HtmlNode> nodes = d.DocumentNode.QuerySelectorAll("a[href*=post] > img");
                if (nodes.ToList<HtmlNode>().Count == 0)
                {
                    throw new Exception("Search image links not found, throwing.");
                }
                List<HtmlNode> cleared = new List<HtmlNode>();
                foreach (HtmlNode node in nodes)
                {
                    if (!node.ParentNode.GetAttributeValue("href", "").Contains("list")
                    )
                    {
                        cleared.Add(node.ParentNode);
                        string PostLink = string.Join(" ", node.ParentNode.GetClasses());
                        Console.Write("\rThumb Selector: " + PostLink);
                    }
                    else
                        Console.Write("\rUri might be default page, ignoring. (uri = " + node.ParentNode.GetAttributeValue("href", "") + " )");

                }
                d.LoadHtml(client.Get(cleared[0].GetAttributeValue("href", "")).ReadContentAsString());
                document = d.DocumentNode;
                if (document.QuerySelector("img[id][src][alt*=" + Options.TestTag + "]") != null)
                {
                    HtmlNode image = document.QuerySelector("img[id][src][alt*=" + Options.TestTag + "]");
                    string href = image.GetAttributeValue("href", "?????");
                    document.QuerySelector("a[href*="+href.Split(".").Last().Split("?")[0] + "]").GetAttributeValue("href", "");
                    
                }




            }
            else
            {
                throw new Exception("Tags form not found, throwing.");
            }

            // for now
            return default(SiteSupportEntry);
        }
        #endregion
        private string SelectHref(IEnumerable<HtmlNode> nodes)
        {
            foreach (HtmlNode node in nodes)
            {
                Console.Write("\r[Detector] is the site url " + node.GetAttributeValue("href", "https://google.com/") + " (Y/n) ");
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.Y || key.Key == ConsoleKey.Enter)
                    return node.GetAttributeValue("href", "https://google.com/");
            }
            throw new Exception("Site not found.");
        }
    }
}