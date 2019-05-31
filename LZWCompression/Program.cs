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
            var compressedPath = AppDomain.CurrentDomain.BaseDirectory + "compressed";
            var decompressedPath = AppDomain.CurrentDomain.BaseDirectory + "decompressed.txt";

            Compress(entryPath, compressedPath);
            Decompress(compressedPath, decompressedPath);

            var entryFileSize = new FileInfo(entryPath).Length;
            var compressedFileSize = new FileInfo(compressedPath).Length;
            var decompressedFileSize = new FileInfo(decompressedPath).Length;

            Console.WriteLine("File sizes [B]:");
            Console.WriteLine($"Source: \t{entryFileSize}");
            Console.WriteLine($"Compressed: \t{compressedFileSize}");
            Console.WriteLine($"Decompressed: \t{decompressedFileSize}");
            Console.WriteLine("\nCompression ratio:");
            Console.WriteLine(((double)entryFileSize / compressedFileSize).ToString("F"));
            
            Console.ReadLine();
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
            var result = new FileStream(compressedPath, FileMode.Create, FileAccess.Write);
            Action<int> writeByte = (n) =>
            {
                result.WriteByte((byte)(n >> 8));
                result.WriteByte((byte)n);
            };

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
                    writeByte(dictionary[c]);
                    dictionary.Add(cs, dictionary.Count);
                    c = s.ToString();
                }
            }

            if (!string.IsNullOrEmpty(c))
            {
                writeByte(dictionary[c]);
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

            var source = new FileStream(compressedPath, FileMode.Open, FileAccess.Read);
            Func<int> readCode = () =>
            {
                var buffer = source.ReadByte();
                return (source.ReadByte()) | (buffer << 8);
            };

            var result = new StreamWriter(decompressedPath, false);

            var fc = readCode();
            result.Write(dictionary[fc]);

            while (source.Position != source.Length)
            {
                var c = readCode();
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
        }

    }
}
