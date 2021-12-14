using LordG.IO;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ByamlExt.Byaml;
using SM3DW_Level_Porter.Ext;
using System.Text;
using System;

namespace Byml.Dumper
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                FileInfo[] files = args.Select(x => new FileInfo(x)).Where(x => x.Exists).ToArray();
                FileInfo[] szsfiles = files.Where(x => x.Extension is ".szs").ToArray();
                FileInfo[] bymlfiles = files.Where(x => x.Extension is ".byml").ToArray();
                FileInfo[] ymlfiles = files.Where(x => x.Extension is ".yml").ToArray();
                FileInfo[] txtfiles = files.Where(x => x.Extension is ".txt").ToArray();
                FileInfo[] objectfiles = files.Where(x => !x.Name.Contains(".count")).ToArray();
                if (szsfiles.Length > 0)
                    foreach (var file in szsfiles)
                    {
                        var es = new EndianStream(file);
                        if (!YAZ0Util.CheckMagic((byte[])es))
                            continue;
                        es = YAZ0Util.Decompress(es);
                        var sarc = SARC.UnpackRamN((byte[])es);
                        es.Dispose();
                        var bymls = sarc.GetBymls();
                        var ymls = bymls.GetYmls();
                        var objects = ymls.GetObjects();
                        var counts = objects.GetCounts();
                        objects.GetDatas().ForEach(Util.WriteToFileInfo);
                        counts.GetDatas().ForEach(Util.WriteToFileInfo);
                    }
                if (bymlfiles.Length > 0)
                    foreach (var file in bymlfiles)
                    {
                        var byml = file.GetByml();
                        var yml = byml.GetYml();
                        var obj = yml.GetObject();
                        var count = obj.GetCount();
                        obj.GetData().WriteToFileInfo();
                        count.GetData().WriteToFileInfo();
                    }
                if (ymlfiles.Length > 0)
                    foreach (var file in ymlfiles)
                    {
                        var yml = file.GetYml();
                        var obj = yml.GetObject();
                        var count = obj.GetCount();
                        obj.GetData().WriteToFileInfo();
                        count.GetData().WriteToFileInfo();
                    }
                if (objectfiles.Length > 0)
                    foreach (var file in objectfiles)
                    {
                        var obj = file.GetObject();
                        var count = obj.GetCount();
                        obj.GetData().WriteToFileInfo();
                        count.GetData().WriteToFileInfo();
                    }
            }
#if DEBUG
            else
            {
                var szs = new FileInfo("Test-WiiU.szs");
                var es = new EndianStream(szs);
                es = YAZ0Util.Decompress(es);
                var sarc = SARC.UnpackRamN((byte[])es);
                var bymls = sarc.GetBymls();
                var ymls = bymls.GetYmls();
                var objects = ymls.GetObjects();
                var count = objects.GetCounts();
                objects.GetDatas().ForEach(Util.WriteToFileInfo);
                count.GetDatas().ForEach(Util.WriteToFileInfo);
            }
#endif
        }

        internal static string ReverseSubstring(this string str, int index)
        {
            IEnumerable<char> arr()
            {
                for (int i = 0; i < index; i++)
                    yield return str[i];
            }
            return new string(arr().ToArray());
        }
    }
}
