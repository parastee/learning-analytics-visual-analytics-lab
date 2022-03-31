using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ImportData.Crawler
{
    public class ListWebCrawler<TCrawler> : WebCrawler where TCrawler : WebCrawler
    {
        dynamic value = null;

        protected IEnumerable<TCrawler> Crawlers { get; private set; }
        protected IEnumerable<HtmlNode> Nodes { get; private set; }



        protected ListWebCrawler(string source, string path) : base(source, path)
        {

        }

        protected virtual List<HtmlNode> GetNods(HtmlNode document)
        {
            var paths = Path.Split(new[] { "[]" },  StringSplitOptions.None);
            var enumerator = paths.GetEnumerator();
            enumerator.Reset();
            enumerator.MoveNext();
            var nodes = document.SelectNodes((string)enumerator.Current)?.AsEnumerable()??new HtmlNode[0];
            while (enumerator.MoveNext())
                nodes = nodes.SelectMany(node => node.SelectNodes("/" + (string)enumerator.Current));
            return nodes?.ToList() ?? new List<HtmlNode>();
        }

        internal protected override sealed void Init(ICrawlerProvider provider)
        {
            Nodes = GetNods(provider.GetDocument(Source));//.Take(40);

            Crawlers = ParseNodes(provider);
        }

        private IEnumerable<TCrawler> ParseNodes(ICrawlerProvider provider)
        {
            foreach (var node in Nodes)
            {
                var obj = ParseModel(node);
                if (obj != null)
                {
                    obj.Init(provider);
                    yield return obj;

                }
            }
        }

        protected virtual TCrawler ParseModel(HtmlNode node)
        {
            return (TCrawler)Activator.CreateInstance(typeof(TCrawler), node.OuterHtml,$"/{node.OriginalName}");
        }

        class EnumerableDynamic : IEnumerable<object>
        {
            class IEnumeratorDynamic : IEnumerator<object>
            {
                private readonly IEnumerator<object> enumerator;
                private IList<object> cach;
                private IEnumerable<object> enumerable;
                Func<object, object> AddCach = null;
                Func<bool> Next = null;

                private object AddList(object o) {
                    this.cach.Add(o);
                    AddCach = NoAddList;
                    return o;
                }

                private object NoAddList(object o) => o;
                public IEnumeratorDynamic(ref IEnumerable<object> cach, Func<IEnumerable<object>> IEnumeratorFunc )
                {
                    if (cach is null)
                    {
                        enumerator = IEnumeratorFunc().GetEnumerator();
                        AddCach = AddList;
                         this.cach = new List<object>();
                        cach = this.cach;
                        Next = () => {
                            AddCach = AddList;
                            return enumerator.MoveNext();
                        };
                    }
                    else
                    {
                        AddCach = NoAddList;
                        enumerator = cach.GetEnumerator();
                        Next = enumerator.MoveNext;
                    }

                }

                public object Current=> AddCach( enumerator.Current);


                public void Dispose() => enumerator.Dispose();

                public bool MoveNext() => Next();

                public void Reset() => enumerator.Reset();

            }

            private readonly Func<IEnumerable<object>> iEnumeratorFunc;
            IEnumerable<object> cach = null;
            public EnumerableDynamic(Func<IEnumerable<object>> IEnumeratorFunc )
            {
                iEnumeratorFunc = IEnumeratorFunc;
            }

            public IEnumerator<dynamic> GetEnumerator()
            {
                return new IEnumeratorDynamic(ref cach,iEnumeratorFunc);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public override string ToString()
            {
                using (var e = GetEnumerator())
                {
                    var s = "";
                    string sep = null;
                    bool isNext = true;
                    
                    while (e.MoveNext()) 
                    {
                        s += $"{sep}{e.Current}";
                        sep = ",";
                    } 
                    
                    return $"[{s}]";
                }
            }
        }

        private IEnumerable<dynamic> GetListValue()
        {
            foreach (var crawlObj in Crawlers)
                yield return crawlObj.Value;
        }

        public override dynamic Value
        {
            get
            {
                if (value is null)
                {

                    value = new EnumerableDynamic(GetListValue);
                    
                }
                return value;
            }
        }
        public override string ToString()
        {
            return $"[{string.Join(",", Value)}]";
        }
    }
}
