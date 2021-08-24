using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SARCExt;
using Hack.io.YAZ0;

namespace Sarc.Dumper
{
    static class Program
    {
        static void Main(string[] args)
        {
            var home_dir = Directory.GetCurrentDirectory();
            if (args.Length is 0) throw new ArgumentException();
            foreach (var arg in args)
            {
                if (new FileInfo(arg).Exists && new FileInfo(arg).Extension is ".szs")
                {
                    var SarcData = SARC.UnpackRamN(YAZ0.Decompress(File.ReadAllBytes(arg)));
                    foreach (var f in SarcData.Files)
                    {
                        if (f.Key.EndsWith(".byml"))
                        {
                            if (f.Key.ReverseSubstring(f.Key.IndexOf(".")).EndsWith("Map"))
                            {
                                Directory.SetCurrentDirectory(new FileInfo(arg).Directory.FullName);
                                File.WriteAllBytes(f.Key, f.Value);
                            }
                        }
                        Directory.SetCurrentDirectory(home_dir);
                    }
                }
            }
        }

        static string ReverseSubstring(this string Str, int index)
        {
            var arr = new List<string>();
            for (int i = 0; i < index; i++)
            {
                arr.Add(Str[i].ToString());
            }
            return string.Join(string.Empty, arr.ToArray());
        }
    }
}
