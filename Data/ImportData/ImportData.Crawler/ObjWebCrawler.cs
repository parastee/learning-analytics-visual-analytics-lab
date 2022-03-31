using HtmlAgilityPack;
using ImportData.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace ImportData.Crawler
{
    public class ObjWebCrawler : WebCrawler, IMemberConfig
    {
        protected ObjWebCrawler(string source, string path,Action<IMemberConfig> config) : base( source, path)
        {
            this.config = config;
        }
        protected HtmlNode node;
        protected Dictionary<string, Func<dynamic>> Crawlers { get; private set; }
        Dictionary<string, Func<HtmlNode, WebCrawler>> members = new Dictionary<string, Func<HtmlNode, WebCrawler>>();
        IMemberConfig IMemberConfig.AddPath(string name,Func<HtmlNode, WebCrawler> path)
        {
            members.Add(name, path);
            return this;
        }
        dynamic value = null;
        private readonly Action<IMemberConfig> config;
        class MemberConfigForRep : IMemberConfig
        {
            private readonly IRepositoryConfig repConfig;

            public MemberConfigForRep(IRepositoryConfig repConfig)
            {
                this.repConfig = repConfig;
            }
            public IMemberConfig AddPath(string name, Func<HtmlNode, WebCrawler> path)
            {
                repConfig.TryAddCol(name, DataType.TEXT_NUll);
                return this;
            }
        }
        public void ConfigRepository(IRepositoryConfig repConfig)
        {
            config(new MemberConfigForRep(repConfig));
        }

        protected virtual HtmlNode GetNode(HtmlNode document)
        {
            return document.SelectSingleNode(Path);
        }

        internal protected override sealed void Init(ICrawlerProvider provider)
        {
            config(this);
            node = GetNode(provider.GetDocument(Source));
            var list = new Dictionary<string,Func<dynamic>>();
            foreach (var member in members)
            {
                var mm = member;
                var p = provider;
                var n = node;
                list.Add(mm.Key, () => {
                    var obj = mm.Value(n);
                    obj.Init(p);
                    return obj.Value;
                });
            }
            Crawlers = list;
        }
        public override dynamic Value {
            get {
                if(value is null)
                {
                    dynamic v = new ExpandoObjectOnDemand(Crawlers);
                    value = v;
                }
                return value;
            }
        }

        private class ExpandoObjectOnDemand : DynamicObject,IDynamicMetaObjectProvider, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable, IDictionary<string, object>, INotifyPropertyChanged
        {
            
            ExpandoObject parent;
            private readonly Dictionary<string, Lazy<dynamic>> crawlers;

            public ExpandoObjectOnDemand(Dictionary<string, Func<dynamic>> crawlers)
            {
                parent  = new ExpandoObject();
                this.crawlers = crawlers.ToDictionary(a=>a.Key,a=>new Lazy<dynamic>(()=>a.Value()));
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return Keys;
            }
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                return TryGetValue(binder.Name , out result);
            }
            public override bool TrySetMember(SetMemberBinder binder, object value)
            { 
                var dic = ((IDictionary<string, object>)parent);
                if (!dic.ContainsKey(binder.Name))
                    dic.Add(binder.Name, value);
                else
                    this[binder.Name] = value;
                return true;
            }

            public int Count => ((ICollection<KeyValuePair<string, object>>)parent).Count < crawlers.Count?
                crawlers.Count:
                ((ICollection<KeyValuePair<string, object>>)parent).Count;

            public bool IsReadOnly => ((ICollection<KeyValuePair<string, object>>)parent).IsReadOnly;

            public ICollection<string> Keys =>new Collection<string>(((IDictionary<string, object>)parent).Keys.Concat(crawlers.Keys).Distinct().ToList());

            public ICollection<object> Values => new Collection<object>(GetBothList().Select(a=>a.Value).ToList());

            public object this[string key] {
                get {
                    if (((IDictionary<string, object>)parent).TryGetValue(key, out object value))
                        return value;
                    if (crawlers.TryGetValue(key, out Lazy<dynamic> lvalue))
                    {
                        Add(key, lvalue.Value);
                        return lvalue.Value;
                    }
                    return ((IDictionary<string, object>)parent)[key];
                }
                set => ((IDictionary<string, object>)parent)[key] = value; }

            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
            {
                add
                {
                    ((INotifyPropertyChanged)parent).PropertyChanged += value;
                }

                remove
                {
                    ((INotifyPropertyChanged)parent).PropertyChanged -= value;
                }
            }

            public void Add(KeyValuePair<string, object> item)
            {
                ((ICollection<KeyValuePair<string, object>>)parent).Add(item);
            }

            public void Clear()
            {
                ((ICollection<KeyValuePair<string, object>>)parent).Clear();
            }

            public bool Contains(KeyValuePair<string, object> item)
            {
                return ((ICollection<KeyValuePair<string, object>>)parent).Contains(item) || crawlers.ContainsKey(item.Key);
            }

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                 
                ((ICollection<KeyValuePair<string, object>>)parent).CopyTo(array, arrayIndex);
            }
            IEnumerable<KeyValuePair<string, object>> GetBothList()
            {
                foreach(var item in ((ICollection<KeyValuePair<string, object>>)parent))
                {
                    yield return item;
                }
                foreach (var item in crawlers)
                {

                    if (!((IDictionary<string, object>)parent).ContainsKey(item.Key)) {
                        var newItem = new KeyValuePair<string, object>(item.Key, item.Value.Value);
                        Add(newItem.Key, newItem.Value);
                        yield return newItem;
                    }
                }
            }
            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                return GetBothList().GetEnumerator();
            }

            public bool Remove(KeyValuePair<string, object> item)
            {
                return ((ICollection<KeyValuePair<string, object>>)parent).Remove(item);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetBothList().GetEnumerator();
            }

            public void Add(string key, object value)
            {
                ((IDictionary<string, object>)parent).Add(key, value);
            }

            public bool ContainsKey(string key)
            {
                return ((IDictionary<string, object>)parent).ContainsKey(key) || crawlers.ContainsKey(key);
            }

            public bool Remove(string key)
            {
                return ((IDictionary<string, object>)parent).Remove(key);
            }

            public bool TryGetValue(string key, out object value)
            {
                if (((IDictionary<string, object>)parent).TryGetValue(key, out value))
                    return true;
                if (crawlers.TryGetValue(key, out Lazy<dynamic> lvalue))
                {
                    value = lvalue.Value;
                    Add(key, value);
                    return true;
                }
                return false;
            }

            public override string ToString()
            {
                return $"{{{string.Join(",", GetBothList().Select(a=>$"\"{a.Key}\":\"{a.Value}\""))}}}";
            }
        }
    }
}
