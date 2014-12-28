using AzureKienThao.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzureKienThao.Background
{
    public class PrepareSeeds
    {
        private string KanjiFilePath = @"..\..\..\Resources\1945Kanji.txt";
        private string TangorinPrefix = @"http://tangorin.com/kanji/";
        private string HtmlsDirPath = @"..\..\..\Resources\Htmls\";
        public void Execute()
        {
            /////// Create context
            DictionaryContext context = new DictionaryContext("DictionaryJaConnection");
            context.Database.CreateIfNotExists();

            /////// Prepare kanjis
            Dictionary<string, KanjiModel> kanjis = new Dictionary<string, KanjiModel>();
            Dictionary<string, WordModel> words = new Dictionary<string, WordModel>();
            Scrape(out kanjis);

            ///////// Preprocess data
            Preprocess(ref kanjis);

            //////////////
            /// Push to database

            foreach (var kv in kanjis)
            {
                try
                {
                    if (context.Kanjis.SingleOrDefault(x => x.Name == kv.Value.Name) == null)
                    {
                        context.Kanjis.Add(kv.Value);
                        context.SaveChanges();
                        
                    }
                    else
                    {
                        Console.WriteLine("Allready in: " + kv.Value.Name);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("-------------: " + kv.Value.SoundJa);
                    Console.WriteLine(ex);
                    var errors = context.GetValidationErrors();
                    foreach (var error in errors)
                    {
                        if (error.IsValid)
                            continue;
                        foreach (var e in error.ValidationErrors)
                        {
                            Console.WriteLine(e.PropertyName + " " + e.ErrorMessage);
                        }
                    }
                    context.Kanjis.Remove(kv.Value);
                    Console.Read();
                }
            }
        }

        public void Scrape(out Dictionary<string, KanjiModel> kanjis)
        {
            kanjis = new Dictionary<string, KanjiModel>();
            Scrape scrape = new Scrape();
            using (StreamReader sr = new StreamReader(KanjiFilePath))
            {
                try
                {
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] splits = line.Split('\t');
                        if (splits.Length != 5)
                        {
                            Console.WriteLine("Line is wrong: " + line);
                        }
                        else
                        {
                            KanjiModel kanji = scrape.ScrapeKanji(TangorinPrefix + splits[1], HtmlsDirPath + splits[1] + ".html");
                            if (kanji == null)
                            {
                                Console.WriteLine("Null kanji returned");
                                continue;
                            }
                            kanji.SoundVn = splits[2];
                            kanji.MeaningVi = splits[3];
                            kanjis.Add(splits[1], kanji);
                            Console.WriteLine("Done: " + line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cannot process:\n" + ex);
                }
            }
        }

        public void Preprocess(ref Dictionary<string, KanjiModel> kanjis)
        {
            var keys = kanjis.Keys;
            Regex reg = new Regex("[- .。]");
            foreach (var key in keys)
            {
                kanjis[key].SoundJa = string.Join(", ", kanjis[key].SoundJa.Split(',').Select(x => reg.Replace(x, "")).Distinct());
                kanjis[key].SoundRo = string.Join(", ", kanjis[key].SoundRo.Split(',').Select(x => reg.Replace(x, "")).Distinct());
            }
        }

        public void PostProcess()
        {

        }
    }
}