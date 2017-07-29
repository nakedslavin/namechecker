using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace namechecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var names = GenerateQueueFor5().ToList();
            Console.WriteLine(names.Count);
            var text = new StringBuilder();
            foreach (var name in names){
                var domainName = name + ".com";
                var res = RequestWhois(domainName);
				if (res.Contains("No match for"))
				{
                    Console.WriteLine($"{domainName} is FREE");
                    text.AppendLine(domainName);
					continue;
				}
			}
            File.WriteAllText("domains.txt",text.ToString());
			Console.WriteLine("Finish");
        }

        private static void REPL()
        {
            while (true)
            {
                string domainName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(domainName))
                {
                    Console.WriteLine("Provide a domain name");
                    continue;
                }
                var res = RequestWhois(domainName);
                if (res.Contains("No match for"))
                {
                    Console.WriteLine("FREE");
                    continue;
                }

                var pattern = @"Expiry\sDate:\s*(?<date>\d{4}-\d{2}-\d{2})";
                var date = System.Text.RegularExpressions.Regex.Match(res, pattern).Groups["date"].Value;

                Console.WriteLine($"TAKEN until {date}");
            }
        }

        private static List<string> GenerateQueueFor5() {
            var vowels = "aeijoquy".ToCharArray();
            var consonants = "bcdfghklmnprstvwxz".ToCharArray();
            var patterns = new List<string> {
                "cvccv","ccvcv","cvcvv","cvvcv","cvccv", "vcvcv", "vccvv"
            };
            var words = new List<string>();

            foreach (var pattern in patterns) {
                //cvccv
                var collections = new List<List<char>>();
                var pwords = new List<string>();

                foreach (char letter in pattern) {
                    if (letter == 'c')
                        collections.Add(consonants.ToList());
                    if (letter == 'v')
                        collections.Add(vowels.ToList());
                }

                foreach (var a in collections[0])
                    foreach (var b in collections[1])
                        foreach (var c in collections[2])
                            foreach (var d in collections[3])
                                foreach (var e in collections[4]) {
                                    pwords.Add($"{a}{b}{c}{d}{e}");
                                }

                words.AddRange(pwords);
            }
            return words;
        }

        private static string RequestWhois(string domainName)
        {
            const string internic = "whois.internic.net";
            string result = null;
            try
            {
                ServicePointManager.Expect100Continue = false;
                var client = new TcpClient(internic, 43);

                var sreader = client.GetStream();
                var swriter = client.GetStream();


                var streamwriter = new StreamWriter(swriter);
                streamwriter.WriteLine(domainName);
                streamwriter.Flush();
                var streamreader = new StreamReader(sreader);
                result = streamreader.ReadToEnd();

                streamwriter.Close();
                streamreader.Close();
            }
            catch (Exception ex) { return ex.Message; }
            return result;
        }
    }
}
