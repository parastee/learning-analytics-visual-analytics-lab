using HtmlAgilityPack;
using System;

namespace ImportData.Crawler
{
    public interface IMemberConfig
    {
        IMemberConfig AddPath(string name, Func<HtmlNode, WebCrawler> path);
    }
}
