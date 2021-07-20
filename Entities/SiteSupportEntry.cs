using System;
using System.Collections.Generic;
namespace bulkio.Entities
{
    class SiteSupportEntry
    {
        public List<string> SiteAliases;
        public string BaseURI;
        public SelectorArray Selectors;
        public PaginationSetup Pagination;
        public CensorshipOptions CensorshipOptions;

    }
}
