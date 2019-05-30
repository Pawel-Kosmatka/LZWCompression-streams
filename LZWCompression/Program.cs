using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZWCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            var entryPath = AppDomain.CurrentDomain.BaseDirectory + "entry.txt";
            var compressedPath = AppDomain.CurrentDomain.BaseDirectory + "compressed.txt";
            var decompressedPath = AppDomain.CurrentDomain.BaseDirectory + "decompressed.txt";

            var entry = File.OpenRead(entryPath);

        }

        public static List<int> Compress(string source)
        {
            var dictionary = new Dictionary<string, int>();
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                dictionary.Add(((char)i).ToString(), i);
            }

            var c = string.Empty;
            var result = new List<int>();

            foreach (var s in source)
            {
                string cs = c + s;
                if (dictionary.ContainsKey(cs))
                {
                    c = cs;
                }
                else
                {
                    result.Add(dictionary[c]);
                    dictionary.Add(cs, dictionary.Count);
                    c = s.ToString();
                }
            }

            if (!string.IsNullOrEmpty(c))
            {
                result.Add(dictionary[c]);
            }

            return result;
        }

        public static string Decompress(List<int> source)
        {
            var dictionary = new Dictionary<int, string>();
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                dictionary.Add(i, ((char)i).ToString());
            }

            var fc = source[0];
            source.RemoveAt(0);

            var result = dictionary[fc];

            foreach (var c in source)
            {
                var pc = dictionary[fc];
                if (dictionary.ContainsKey(c))
                {
                    dictionary.Add(dictionary.Count, pc + dictionary[c][0]);
                    result += dictionary[c];
                }
                else
                {
                    dictionary.Add(dictionary.Count, pc + pc[0]);
                    result += pc + pc[0];
                }

                fc = c;
            }
            return result.ToString();
        }
    }
}
