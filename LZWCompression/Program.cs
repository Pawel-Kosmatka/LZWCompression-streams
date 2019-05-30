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

            Compress(entryPath, compressedPath);
            Decompress(compressedPath, decompressedPath);

        }

        public static void Compress(string entryPath, string compressedPath)
        {
            var dictionary = new Dictionary<string, int>();
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                dictionary.Add(((char)i).ToString(), i);
            }

            var source = new StreamReader(entryPath, Encoding.UTF8);

            var c = ((char)source.Read()).ToString();
            char s;
            var result = new FileStream(compressedPath, FileMode.OpenOrCreate, FileAccess.Write);
            //var result = new StreamWriter(compressedPath, false, Encoding.UTF8);
            var x = BitConverter.GetBytes(dictionary[c]);
            while (!source.EndOfStream)
            {
                s = (char)source.Read();
                string cs = c + s;
                if (dictionary.ContainsKey(cs))
                {
                    c = cs;
                }
                else
                {
                    result.Write(BitConverter.GetBytes(dictionary[c]), 0, 4);
                    //result.Write(dictionary[c] + ",");
                    dictionary.Add(cs, dictionary.Count);
                    c = s.ToString();
                }
            }

            if (!string.IsNullOrEmpty(c))
            {
                //result.Write(dictionary[c] + ",");
            }

            source.Dispose();
            result.Dispose();
        }

        public static void Decompress(string compressedPath, string decompressedPath)
        {
            var dictionary = new Dictionary<int, string>();
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                dictionary.Add(i, ((char)i).ToString());
            }

            var source = new StreamReader(compressedPath, Encoding.UTF8);

            var fc = getCode();

            var result = new StreamWriter(decompressedPath, false, Encoding.UTF8);
            result.Write(dictionary[fc]);

            while (!source.EndOfStream)
            {
                var c = getCode();
                var pc = dictionary[fc];
                if (dictionary.ContainsKey(c))
                {
                    dictionary.Add(dictionary.Count, pc + dictionary[c][0]);
                    result.Write(dictionary[c]);
                }
                else
                {
                    dictionary.Add(dictionary.Count, pc + pc[0]);
                    result.Write(pc + pc[0]);
                }

                fc = c;

            }
            source.Dispose();
            result.Dispose();

            int getCode()
            {
                var entry = source.Read();
                string code = string.Empty;
                while ((char)entry != ',')
                {
                    code += (char)entry;
                    entry = source.Read();
                }
                return int.Parse(code);
            }
        }

    }
}
