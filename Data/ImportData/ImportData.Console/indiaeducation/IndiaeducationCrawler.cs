using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using ImportData.Crawler;

namespace ImportData.Console.indiaeducation
{
    public class IndiaeducationCrawler : ListWebCrawler<UniCrawler>
    {

        public IndiaeducationCrawler( string source, string path) : base( source, path)
        {
        }
        //protected override List<HtmlNode> GetNods(HtmlNode document)
        //{
        //    return base.GetNods(document);// internalGetNods(document).ToList();
        //}
        // IEnumerable<HtmlNode> internalGetNods(HtmlNode document)
        //{
        //    document = document.SelectSingleNode(Path);
        //    HtmlNode cityNode = null;
        //    foreach (var node in document.ChildNodes)
        //    {
        //        if(node.OriginalName=="h3")
        //        {
        //            cityNode = node;
        //        }
        //        else if(node.OriginalName == "ul")
        //        {
        //            foreach(var aTagNode in node.SelectNodes("/li/a"))
        //            {
        //                aTagNode.ParentNode.ChildNodes.Add(cityNode.CloneNode(true));
        //                yield return aTagNode.ParentNode;
        //            }
        //        }
        //    }
        //}
        protected override UniCrawler ParseModel(HtmlNode node)
        {
            if (string.IsNullOrWhiteSpace(node.InnerText))
                return null;
            return base.ParseModel(node);
        }
    }
    public class UniCrawler : ObjWebCrawler
    {
        public UniCrawler(string source, string path)
            : base(source, path
                  , (config) => config
                  .AddPath("UniId", node => new IndiaeducationIdWebCrawler(node.InnerHtml, "/a"))
                  .AddPath("UniName", node => new StringWebCrawler(node.InnerHtml, "/a"))
                  .AddPath("Founded", node => new StringWebCrawler(node.GetAttributeValue("href", ""), "//*[@id=\"campus_div\"]/div[2]/div[1]/div[2]/span"))
                  .AddPath("City", node => new StringWebCrawler(node.GetAttributeValue("href", ""), "//*[@id=\"colg_addr\"]"))
                  .AddPath("Rank", node => new StringWebCrawler("<i>1</i>", "/")))
        {

        }
    }
    public class IndiaeducationIdWebCrawler : StringWebCrawler
    {
        public IndiaeducationIdWebCrawler(string source, string path) : base(source, path)
        {
        }
        protected override string ConvertValue(HtmlNode document)
        {
            var a = document.GetAttributeValue("href", "").Split("/");
            return a[a.Length-1].Split(".")[0].Replace("co-","");
        }
    }
}
