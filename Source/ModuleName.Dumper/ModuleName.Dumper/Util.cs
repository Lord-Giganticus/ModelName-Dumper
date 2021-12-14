using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LordG.IO;
using SM3DW_Level_Porter.Ext;

namespace Byml.Dumper
{
    public static class Util
    {
        public static Dictionary<string, EndianStream> GetBymls(this SarcData sarc)
        {
            var bymls = new Dictionary<string, EndianStream>();
            foreach (var pair in sarc.Files)
                if (pair.Key.EndsWith(".byml"))
                    if (pair.Key.ReverseSubstring(pair.Key.IndexOf(".")).EndsWith("Map"))
                        bymls.Add(pair.Key, pair.Value);
            return bymls;
        }

        public static KeyValuePair<string, EndianStream> GetByml(this FileInfo src)
        {
            return new KeyValuePair<string, EndianStream>(src.Name, new EndianStream(src));
        }

        public static Dictionary<string, string[]> GetYmls(this Dictionary<string, EndianStream> bymls)
        {
            return bymls.Change(GetYml);
        }

        public static KeyValuePair<string, string[]> GetYml(this KeyValuePair<string, EndianStream> pair)
        {
            var byml = pair.Value.GetByml();
            var data = byml.ToYaml();
            return new KeyValuePair<string, string[]>(pair.Key, data.Split(new string[] { Environment.NewLine }, 0));
        }

        public static KeyValuePair<string, string[]> GetYml(this FileInfo src)
        {
            return GetYml(new KeyValuePair<string, EndianStream>(src.Name, new EndianStream(src)));
        }

        public static Dictionary<string, List<string>> GetObjects(this Dictionary<string, string[]> ymls)
        {
            return ymls.Change(GetObject);
        }

        public static KeyValuePair<string, List<string>> GetObject(this KeyValuePair<string, string[]> pair)
        {
            string endian;
            switch (pair.Value[1].Substring(pair.Value[1].IndexOf(":") + 2))
            {
                case "True": endian = "WiiU"; break;
                case "False": endian = "Switch"; break;
                default: endian = null; break;
            }
            var names = new List<string>();
            foreach (var line in pair.Value)
                if (line.Contains("ModelName"))
                    if (line.Substring(line.IndexOf(':') + 2) != "null")
                        names.Add(line.Substring(line.IndexOf(':') + 2));
            var name = pair.Key;
            name = $"{name.ReverseSubstring(name.IndexOf('.'))}-{endian ?? "unknown"}.txt";
            return new KeyValuePair<string, List<string>>(name, names);
        }

        public static KeyValuePair<string, List<string>> GetObject(this FileInfo src)
        {
            return GetObject(new KeyValuePair<string, string[]>(src.Name, File.ReadAllLines(src.FullName)));
        }

        public static Dictionary<string, Dictionary<string, int>> GetCounts(this Dictionary<string, List<string>> objects)
        {
            return objects.Change(GetCount);
        }

        public static KeyValuePair<string, Dictionary<string, int>> GetCount(this KeyValuePair<string, List<string>> pair)
        {
            var name = pair.Key.ReverseSubstring(pair.Key.IndexOf('.'));
            name += ".count.txt";
            var dict = new Dictionary<string, int>();
            foreach (var line in pair.Value)
                if (!string.IsNullOrEmpty(line))
                {
                    if (!dict.ContainsKey(line))
                        dict.Add(line, 1);
                    else
                        dict[line]++;
                }
            return new KeyValuePair<string, Dictionary<string, int>>(name, dict);
        }

        public static Dictionary<FileInfo, byte[]> GetDatas(this Dictionary<string, List<string>> objects)
        {
            return objects.Change(GetData);
        }

        public static KeyValuePair<FileInfo, byte[]> GetData(this KeyValuePair<string, List<string>> pair)
        {
            var info = new FileInfo(pair.Key);
            using (var endian = new EndianStream())
            {
                var encoding = Encoding.UTF8;
                foreach (var value in pair.Value)
                {
                    endian.WriteString(value, encoding);
                    endian.WriteString(Environment.NewLine, encoding);
                }
                return new KeyValuePair<FileInfo, byte[]>(info, (byte[])endian);
            }
        }

        public static Dictionary<FileInfo, byte[]> GetDatas(this Dictionary<string, Dictionary<string, int>> counts)
        {
            return counts.Change(GetData);
        }

        public static KeyValuePair<FileInfo, byte[]> GetData(this KeyValuePair<string, Dictionary<string, int>> pair)
        {
            var file = new FileInfo(pair.Key);
            using (var es = new EndianStream())
            {
                var encoding = Encoding.UTF8;
                foreach (var d in pair.Value)
                {
                    es.WriteString($"The number of times {d.Key} appeared: {d.Value}", Encoding.UTF8);
                    es.WriteString(Environment.NewLine, encoding);
                }
                return new KeyValuePair<FileInfo, byte[]>(file, (byte[])es);
            }
        }

        public static void WriteToFileInfo(this KeyValuePair<FileInfo, byte[]> pair)
        {
            if (!pair.Key.Name.Contains("unknown"))
            {
                using (var file = pair.Key.Create())
                {
                    using (var writer = new BinaryWriter(file))
                    {
                        writer.Write(pair.Value);
                        writer.Flush();
                    }
                }
                var str = File.ReadAllText(pair.Key.Name);
                str = new string(str.Take(str.Length - Environment.NewLine.Length).ToArray());
                File.WriteAllText(pair.Key.Name, str);
            }
        }

        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<KeyValuePair<TKey, TValue>> action)
        {
            foreach (var pair in dict)
                action.Invoke(pair);
        }

        public static Dictionary<TKey, TValue> Change<TKey, TValue, TKeyO, TValueO>(this Dictionary<TKeyO, TValueO> dict, Func<KeyValuePair<TKeyO, TValueO>, KeyValuePair<TKey, TValue>> func)
        {
            var res = new Dictionary<TKey, TValue>();
            foreach (var pair in dict)
            {
                var value = func.Invoke(pair);
                res.Add(value.Key, value.Value);
            }
            return res;
        }
    }
}
