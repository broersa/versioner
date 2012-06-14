using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;
using System.IO;

namespace com.bekijkhet.Versioner
{
    class Program
    {
        static void Main(string[] args)
        {
            string inpath = null;
            string major = null;
            string minor = null;
            string build = null;
            string revision = null;
            bool help = false;
            bool verbose = false;
            var p = new OptionSet() {
   	            { "inpath=",    v => inpath = v },
   	            { "major=",    v => major = v },
   	            { "minor=",    v => minor = v },
   	            { "build=",    v => build = v },
   	            { "revision=",    v => revision = v },
   	            { "v|verbose",  v => verbose = v != null },
   	            { "h|?|help",   v => help = v != null },
            };
            List<string> extra = p.Parse(args);
            if (inpath == null || major == null || minor == null || build == null || revision == null || help)
            {
                Console.WriteLine("Versioner.exe --inpath <path> --major <version> --minor <version> --build <version> --revision <version>");
            }
            else
            {
                foreach (string sourceFilePath in Directory.GetFiles(inpath, "AssemblyInfo.cs", SearchOption.AllDirectories))
                {
                    File.Delete(sourceFilePath + ".orig");
                    File.Move(sourceFilePath, sourceFilePath + ".orig");
                    using (var fsout = new FileStream(sourceFilePath, FileMode.Create))
                    {
                        using (var w = new StreamWriter(fsout, Encoding.UTF8))
                        {
                            using (var fsin = new FileStream(sourceFilePath + ".orig", FileMode.Open))
                            {
                                using (StreamReader r = new StreamReader(fsin, Encoding.UTF8))
                                {
                                    String line;
                                    while ((line = r.ReadLine()) != null)
                                    {
                                        if (line.StartsWith("[assembly: AssemblyVersion(")) line = "[assembly: AssemblyVersion(\""+major+"."+minor+"."+build+"."+revision+"\")]";
                                        if (line.StartsWith("[assembly: AssemblyFileVersion(")) line = "[assembly: AssemblyFileVersion(\""+major+"."+minor+"."+build+"."+revision+"\")]";
                                        w.WriteLine(line);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
