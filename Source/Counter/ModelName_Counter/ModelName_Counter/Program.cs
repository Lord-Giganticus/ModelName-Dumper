using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ModelName_Counter
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length is 0) throw new ArgumentException();
            var filelist = new List<FileInfo>();
            foreach (var arg in args.Where(s => new FileInfo(s).Exists && new FileInfo(s).Extension is ".txt"))
            {
                filelist.Add(new FileInfo(arg));
            }
            foreach (var file in filelist)
            {
                var dict = new Dictionary<string, int>();
                foreach (var line in File.ReadAllLines(file.FullName))
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (!dict.ContainsKey(line))
                            dict.Add(line, 1);
                        else
                            dict[line]++;
                    }
                var writer = new StreamWriter($"{file.Directory.FullName}\\{Path.GetFileNameWithoutExtension(file.FullName)}.count.txt");
                foreach (var d in dict.Take(dict.Count - 1))
                    writer.WriteLine($"The number of times {d.Key} appeared: {d.Value}");
                var l = dict.Last();
                writer.Write($"The number of times {l.Key} appeared: {l.Value}");
                writer.Close();
            }
        }
    }
}