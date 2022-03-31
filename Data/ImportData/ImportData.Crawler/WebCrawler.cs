using HtmlAgilityPack;
using System;
using System.Linq.Expressions;

namespace ImportData.Crawler
{
    public class WebCrawler
    {
        private static ICrawlerProvider _Provider;
        private static ICrawlerProvider CreateProvider<TProvider>() where TProvider:ICrawlerProvider,new()
        {
            return _Provider??(_Provider=new TProvider());
        }

        public static dynamic Crawl<TWebCrawler>(string source, string path="//html") where TWebCrawler : WebCrawler
        {
            return Crawl<TWebCrawler, CrawlerProvider>(source, path);
        }

        public static dynamic Crawl<TWebCrawler,TProvider>(string source, string path) where TWebCrawler : WebCrawler where TProvider : ICrawlerProvider, new()
        {
            var provider = CreateProvider<TProvider>();
            var crawl = (TWebCrawler)Activator.CreateInstance(typeof(TWebCrawler),source,path);
            crawl.Init(provider);
            return crawl.Value;
        }


        protected WebCrawler( string source,string path)
        {
            Source = source;
            Path = path;
            
        }
        
        public string Source { get; protected set; }
        public string Name { get; }
        public string Path { get; protected set; }


        public virtual dynamic Value
        {
            get
            {
                return null;
            }
        }

        internal protected virtual void Init(ICrawlerProvider provider)
        {

        }



        public override string ToString()
        {
            return Value.ToString();
        }

        protected TWebCrawler Create<TWebCrawler>(string source, string path) where TWebCrawler : WebCrawler
        {
            return (TWebCrawler)Activator.CreateInstance(typeof(TWebCrawler), source, path);
        }

        protected WebCrawler GetString(string source, string path)
        {
            return Create<StringWebCrawler>(source, path);
        }
    }
    public class PrimitiveWebCrawler<T> : WebCrawler
    {
        protected PrimitiveWebCrawler(string source, string path) : base(source, path)
        {
        }

        protected T value;

        protected HtmlNode node;
        protected virtual HtmlNode GetNode(HtmlNode document)
        {
            return document.SelectSingleNode(Path);
        }
        protected virtual T ConvertValue(HtmlNode document)
        {
            return (T)Convert.ChangeType(document.InnerText, typeof(T));
        }

        internal protected override sealed void Init(ICrawlerProvider provider)
        {
            node = GetNode(provider.GetDocument(Source));
            value = node==null?default(T):ConvertValue(node);
        }
        public override dynamic Value
        {
            get
            {
               
                return value;
            }
        }
    }
    public class StringWebCrawler : PrimitiveWebCrawler<string>
    {
        public StringWebCrawler(string source, string path) : base(source, path)
        {
        }
        public override dynamic Value => base.value?.Replace("&auml;","ae")?.Replace("&uuml;","ue")?.Replace("&ouml;","oe");
    }
}
