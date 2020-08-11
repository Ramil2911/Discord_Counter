using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using CsvHelper;

namespace discordstats
{
    public static class Program
    {
        public static void Main(params string[] args)
        {
            var messages = new List<Message>();
            using (var streamReader = new StreamReader(
                    @"C:\Users\Ramil\Desktop\discord\DTF Аниме - Общение - голосовой [677619287871062056].csv"))
            {
                using (var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture))
                {
                    csvReader.Configuration.Delimiter = ",";
                    messages = csvReader.GetRecords<Message>().ToList();
                }
            }
            var authors = messages.Select(message => message.Author).ToList();
            var authorsPop = authors
                .GroupBy(s => s)
                .Where(g=>g.Count()>1)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key);
            var i = 0;
            foreach (var author in authorsPop)
            {
                Console.WriteLine(author + " - " + messages.Count(x => x.Author == author));
                i++;
                if(i>15) break;
            }
            Console.WriteLine("-------------");
            var words = messages.SelectMany(message => GetWords(message.Content)).ToList();
            var wordsPop = words
                .GroupBy(s => s)
                .Where(g=>g.Count()>1)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key);
            i = 0;
            foreach (var word in wordsPop)
            {
                if (word.Length <= 4) continue;
                if (word == "https") continue;
                var continuer = false;
                foreach (var author in authorsPop)
                {
                    continuer = author.Contains(word);
                }
                if (continuer) continue;
                Console.WriteLine(word + " - " + messages.Count(x => x.Content.Contains(word)));
                i++;
                if (i > 15) break;
            }
            Console.WriteLine("-------------");
            Console.WriteLine($"Пидарасин: {messages.Count(message => message.Content.ToLower().Contains("пидарас"))}");
        }
        
        static string[] GetWords(string input)
        {
            MatchCollection matches = Regex.Matches(input, @"\b[\w']*\b");

            var words = from m in matches.Cast<Match>()
                where !string.IsNullOrEmpty(m.Value)
                select TrimSuffix(m.Value);

            return words.ToArray();
        }

        static string TrimSuffix(string word)
        {
            int apostropheLocation = word.IndexOf('\'');
            if (apostropheLocation != -1)
            {
                word = word.Substring(0, apostropheLocation);
            }

            return word;
        }
        
    }

    public class Message
    {
        public string AuthorID { get; set; }
        public string Author { get; set; }
        public string Date { get; set; }
        public string Content { get; set; }
        public string Attachments { get; set; }
        public string Reactions { get; set; }
    }
    
}