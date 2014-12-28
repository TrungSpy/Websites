using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using AzureKienThao.Domain.Models;
using CsQuery;

namespace AzureKienThao.Background
{
    public class Scrape
    {
        public KanjiModel ScrapeKanji(string url, string htmlOutputFilePath)
        {
            if (!File.Exists(htmlOutputFilePath))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(url, htmlOutputFilePath);
                }
            }

            HtmlDocument doc = new HtmlDocument();
            doc.Load(new StreamReader(htmlOutputFilePath));

            var dictEntries = doc.GetElementbyId("dictEntries");
            var nodes = dictEntries != null ? dictEntries.DescendantNodes() : null;
            if (nodes == null)
                return null;
            KanjiModel ret = new KanjiModel()
            {
                Name = nodes.Where(x=>x.Name=="h2" 
                    && x.ParentNode.Name=="dt" && x.ParentNode.Attributes["class"].Value=="k-dt").SingleOrDefault().InnerText,
                SoundJa = string.Join(", ",nodes.Where(x=>x.Name=="rb" && x.HasAttributes && x.Attributes["title"]!=null              
                && x.ParentNode.Name=="ruby" && x.ParentNode.ParentNode.Attributes["class"].Value=="kana").Select(x=>x.InnerText)),
                SoundRo = string.Join(", ", nodes.Where(x => x.Name == "rb" && x.HasAttributes && x.Attributes["title"] != null
                && x.ParentNode.Name == "ruby" && x.ParentNode.ParentNode.Attributes["class"].Value == "romaji").Select(x => x.InnerText.ToLower())),
                MeaningEn= string.Join(", ",nodes.Where(x=>x.HasAttributes && x.Attributes["class"]!=null &&
                    x.Attributes["class"].Value=="k-lng-en").First().Descendants("b").Select(x=>x.InnerText))

            };

            var words = nodes.Where(x => x.Name == "td" && x.Descendants("a").Count() > 0).SelectMany(x => x.Descendants("a")).Select(
                 x => new WordModel()
                 {
                     Name = x.InnerText,
                     SoundJa = x.NextSibling.NextSibling.InnerText,
                     MeaningEn = x.NextSibling.NextSibling.NextSibling.NextSibling.InnerText.Replace("】", "")
                 }
               );
            foreach (var word in words)
            {
                ret.Words.Add(word);
            }

            return ret;           
        }
        public IEnumerable<KanjiModel> ScrapeKanjis(List<string> urls, List<string> htmlOutputFilePaths)
        {
            List<KanjiModel> rets = new List<KanjiModel>();
            for(int i=0; i<urls.Count; i++){
                rets.Add(ScrapeKanji(urls[i],htmlOutputFilePaths[i]));
            }
            return rets;
        }
    }
}
