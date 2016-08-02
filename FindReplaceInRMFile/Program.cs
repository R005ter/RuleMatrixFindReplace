using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindReplaceInRMFile
{
    class Program
    {

        public string SourceFile { get; set; } = @"c:\users\jtoth\desktop\COA.rm";
        public string MapFile { get; set; } = @"c:\users\jtoth\desktop\ReplacementMap.csv";
        public string OutputDir { get; set; } = @"c:\users\jtoth\Desktop";
        public string Delimiter { get; set; } = @"|";

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            var p = new Program();
            string outputFile = string.Format(@"{0}\{1}.rm", p.OutputDir, Guid.NewGuid().ToString());
            int replacements = 0;

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile))
            {
                log.Info("Application Start");
                log.Info("SourceFile: " + p.SourceFile);
                log.Info("MapFile: " + p.MapFile);
                log.Info("Delimiter: " + p.Delimiter);
                log.Info("OutputFile: " + outputFile);
                log.Info("---------------------------");

                // Load in Map
                var map = new Dictionary<string, string>();

                log.Info("Loading Map...");
                foreach (string line in System.IO.File.ReadLines(p.MapFile))
                {
                    // Skip Blank
                    if (string.IsNullOrEmpty(line))
                        continue;

                    // split on delimiter
                    string[] entry = line.Split(p.Delimiter.ToCharArray());

                    if (entry.Count() == 2)
                    {
                        map.Add(entry[0], entry[1]);
                    }
                }
                log.Info(String.Format("Loaded {0} Replacements", map.Count.ToString()));

                // Load Source File into Memory
                log.Info("Loading Source...");
                string[] source = System.IO.File.ReadAllLines(p.SourceFile);

                // Loop through every line of the Source and process any replacements for lines starting with <Value
                log.Info("Processing Each Value Line of Source...");
                int i = 1;
                foreach (string srcLine in source)
                {
                    string outLine = srcLine;

                    // we are only changing lines for values.  This protects us from some potential evil
                    if (srcLine.Trim().StartsWith("<Value"))
                    {
                        // Loop through Map Entries and do Replacements
                        foreach (KeyValuePair<string, string> entry in map)
                        {
                            if (outLine.Contains(entry.Key))
                            {
                                outLine = outLine.Replace(entry.Key, entry.Value);
                                log.Info(String.Format(@"Replaced Old Value: {0} with New Value: {1} on Line {2}", entry.Key.ToString(), entry.Value.ToString(), i.ToString()));
                                replacements++;
                            }
                        }
                    }
                    sw.WriteLine(outLine);
                    i++;
                }
                
            } // end using sw

            // Summary
            log.Info("---------------------------------");
            log.Info("Summary:");
            log.Info(string.Format("Replaced {0} Old Values with their mapped counterparts", replacements.ToString()));
            log.Info(string.Format("New File:"));
            log.Info(outputFile.ToString());

            Console.Read();
        }

        private void Test()
        {

            
        }
    }
}
