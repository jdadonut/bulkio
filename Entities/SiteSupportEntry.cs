using System;
using System.Collections.Generic;
namespace bulkio.Entities
{
    public class SiteSupportEntry
    {
        public List<string> SiteAliases;
        public string BaseURI;
        public string PostUriTemplate;
        public bool UseBaseUriInSearch;
        public SelectorArray Selectors;
        public PaginationSetup Pagination;
        public CensorshipOptions Censorship;

    }
}
