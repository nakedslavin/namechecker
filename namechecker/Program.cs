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
        static int Main(string[] args)
        {
            string p = RequestWhois("hned.org");
            Console.WriteLine(p);

            /*
            var names = GenerateQueueFor5().Where(x => !x.StartsWith("y")).ToList();
            foreach (var domain in names)
                Console.WriteLine(domain);

            Console.WriteLine(names.Count);
            */
            Console.WriteLine(p);


            return p == null ? 500 : 200;
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
                using (var client = new TcpClient(internic, 43))
                using (var stream = client.GetStream())
                {
                    var bytes = ASCIIEncoding.ASCII.GetBytes(domainName);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                    using (var sr = new StreamReader(stream))
                        result = sr.ReadToEnd();
                }
            }
            catch (Exception ex) { return ex.Message; }
            return result;
        }
    }
}
