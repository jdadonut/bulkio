using bulkio.Entities;
using HtmlAgilityPack;
namespace bulkio.Detection
{
    class CommonTitle
    {
        public static Ratings GetRating(HtmlNode Element)
        {
            string Title = Element.Attributes["title"].Value;
            if (Title.Contains("Rating: Safe"))
            {
                return Ratings.SAFE;
            }
            else if (Title.Contains("Rating: Questionable"))
            {
                return Ratings.QUESTIONABLE;
            }
            else if (Title.Contains("Rating: Explicit"))
            {
                return Ratings.EXPLICIT;
            }
            return Ratings.UNKNOWN;
        }
    }
}