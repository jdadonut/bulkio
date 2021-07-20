using System;
using System.Collections.Generic;
using CommandLine;

namespace bulkio.Entities
{
    public class ConsoleOptions
    {
        [Option('s', "safe", Required = false, HelpText = "Allow safe content. Default: true")]
        public bool Safe { get; set; } = true;
        [Option('q', "questionable", Required = false, HelpText = "Allow questionable content. Default: true")]
        public bool Questionable { get; set; } = true;
        [Option('e', "explicit", Required = false, HelpText = "Allow explicit content. Default: true")]
        public bool Explicit { get; set; } = false;
        [Option("service", Required = true, HelpText = "Service to download from")]
        public string Service { get; set; }

        [Option("tags", Required = true, HelpText = "Tags to use when downloading")]
        public IEnumerable<string> Tags { get; set; }
        [Option("out", Required = true, HelpText = "Out directory for images")]
        public string Out { get; set; }
        [Option('c', "count", Required = true, HelpText = "How many images to download")]
        public int Count { get; set; }
        [Option("test-tag", Required = false, HelpText = "Tag to use when testing site selectors (default: 1girl)")]
        public string TestTag { get; set; } = "1girl";

    }
}