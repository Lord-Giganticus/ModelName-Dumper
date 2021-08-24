using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace ModelName.Dumper
{
    static class Program
    {
        static string AppLocation => new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;

        static void Main(string[] args)
        {
            if (args.Length is 0) throw new ArgumentException();
            var files = new List<FileInfo>();
            foreach (var arg in args)
            {
                files.Add(new FileInfo(arg));
            }
            foreach (var file in files)
            {
                string endian;
                switch (File.ReadAllLines(file.FullName)[1].Substring(File.ReadAllLines(file.FullName)[1].IndexOf(":") + 2))
                {
                    case "True":
                        endian = "WiiU";
                        break;
                    case "False":
                        endian = "Switch";
                        break;
                    default:
                        endian = null;
                        break;
                }
                var names = new List<string>();
                if (file.Extension is ".yml")
                {
                    foreach (var line in File.ReadAllLines(file.FullName))
                    {
                        if (line.Contains("ModelName"))
                        {
                            if (line.Substring(line.IndexOf(':') + 2) != "null")
                            {
                                names.Add(line.Substring(line.IndexOf(':') + 2));
                            }
                        }
                    }
                }
                Directory.SetCurrentDirectory(file.Directory.FullName);
                File.WriteAllLines($"{file.Name.ReverseSubstring(file.Name.IndexOf('.'))}-{endian ?? "unknown"}.txt", names.ToArray());
                Directory.SetCurrentDirectory(AppLocation);
            }
        }

        static string ReverseSubstring(this string Str, int index)
        {
            List<string> arr()
            {
                var r = new List<string>();
                for (int i = 0; i < index; i++)
                {
                    r.Add(Str[i].ToString());
                }
                return r;
            };
            return string.Join(string.Empty, arr().ToArray());
        }
    }
}
