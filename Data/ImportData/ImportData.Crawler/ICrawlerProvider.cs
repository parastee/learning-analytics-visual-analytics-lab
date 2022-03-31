using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ImportData.Crawler
{
    public interface ICrawlerProvider
    {
        HtmlNode GetDocument(string source);
    }

    public class CrawlerProvider : ICrawlerProvider
    {
        private readonly WebClient channel;
        SortedDictionary<string, string> cachePage = LoadCache();
        public CrawlerProvider()
        {
            channel = new WebClient();
        }

        dynamic ParsSource(string source)
        {
            dynamic v = new System.Dynamic.ExpandoObject();
            var startText = source.Substring(0, 10).ToUpper();
            if (startText.StartsWith("HTTP") || startText.StartsWith("GET ") || startText.StartsWith("POST ") || startText.StartsWith("PUT "))
            {
                
                var method = "";
                if (startText.StartsWith("GET ") || startText.StartsWith("PUT "))
                {
                    method = startText.Substring(0, 3);
                    source = source.Substring(4);
                }
                else if (startText.StartsWith("POST "))
                {
                    method = startText.Substring(0, 5);
                    source = source.Substring(5);
                }
                else
                {
                    method = "GET";
                }

                v.method = method;
                var splited = source.Split('\n');
                v.url = splited[0].Trim();
                if (splited.Length > 1 && string.IsNullOrWhiteSpace(splited[1]))
                {
                    v.data = splited[1].Trim();
                    if (splited.Length > 2 && string.IsNullOrWhiteSpace(splited[2]))
                    {
                        v.type = splited[1].Trim();
                        v.data = splited[2].Trim();
                    }

                }
            }
            else
            {
                v.method = "";
                v.type = "html";
                v.text = source;
            }
            return v;

        }
        private  static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
        const string seperateCach= "<----------------->";
        private static void saveInFile(string source,string content)
        {
            string cachpage = GetCacheFolder();
            var filename = GetHashString(source);
            File.WriteAllText(Path.Combine(cachpage, filename + ".cach"), string.Concat(source, seperateCach, content));
        }
        private static SortedDictionary<string, string> LoadCache()
        {
            string cachpage = GetCacheFolder();
            var c= new SortedDictionary<string, string>();
            foreach (var a in Directory.GetFiles(cachpage,"*.cach"))
            {
                var data = File.ReadAllText(a).Split(new[] { seperateCach },StringSplitOptions.None);
                c.Add(data[0], data[1]);
            }
            return c;
        }
        private static string GetCacheFolder()
        {
            var cachpage = Path.Combine(Directory.GetCurrentDirectory(), "cach");
            if (!Directory.Exists(cachpage))
                Directory.CreateDirectory(cachpage);
            return cachpage;
        }

        public HtmlNode GetDocument(string source)
        {
            var info = ParsSource(source);
            if (!string.IsNullOrWhiteSpace(info.method))
            {
                if (cachePage.TryGetValue(source, out string t))
                    source = t;
                else
                {
                    var n= channel.DownloadString(new Uri(info.url));
                    cachePage.Add(source, n);
                    saveInFile(source, n);
                    source = n;

                }
            }
            else if(info.type=="html")
            {
                source = info.text;
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(source);

            return doc.DocumentNode;
        }
    }
}
