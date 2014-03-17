using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace LargeFileSplitter
{
    class Program
    {
        const int DefaultNumFilesToCreate = 2;

        static void Main(string[] args)
        {
            HandleHelp(args);
            ParseJob job = BuildParseJob(args);
            SplitFile(job);
            ShowMsgAndQuit("Done. Press Enter.");
        }
     
        static ParseJob BuildParseJob(string[] args)
        {
            ParseJob job = new ParseJob();

            if (args.FirstOrDefault() != null) { job.FileToSplit = args.First(); }
            if (args.LastOrDefault() != null) { job.NumFilesToCreate = int.Parse(args[1]); }

            if (string.IsNullOrEmpty(job.FileToSplit))
            {
                Console.WriteLine("Which file would you like to split?");
                job.FileToSplit = Console.ReadLine();
                job.FileToSplit = job.FileToSplit.Trim(new[] { '"' });
                if (File.Exists(job.FileToSplit) == false)
                {
                    ShowMsgAndQuit("Couldn't find that file. We're done here.");
                }
            }

            if (job.NumFilesToCreate == 0)
            {
                Console.WriteLine("How many files would you like this split into?");
                string numFiles = Console.ReadLine();
                int num;
                job.NumFilesToCreate = (int.TryParse(numFiles, out num)) ? num : 2;
                Console.WriteLine(string.Format("Splitting into {0} files.", job.NumFilesToCreate));
            }
            return job;
        }

        static void SplitFile(ParseJob job)
        {
            string[] allLines = File.ReadAllLines(job.FileToSplit);
            string csvHeader = allLines.First();
            job.LinesPerFile = (allLines.Length / job.NumFilesToCreate);

            for (int i = 0; i < job.NumFilesToCreate; i++)
            {
                int startAt = Convert.ToInt32(i * job.LinesPerFile) + 1;
                string newFile = Path.Combine(job.WriteDir, Path.GetFileNameWithoutExtension(job.FileToSplit) + i.ToString() + Path.GetExtension(job.FileToSplit));

                List<string> newFileContents = new List<string> { csvHeader };
                newFileContents.AddRange(allLines.Skip(startAt).Take(job.LinesPerFile));
                File.WriteAllLines(newFile, newFileContents.ToArray());
            }
        }

        private static void ShowMsgAndQuit(string msg)
        {
            Console.WriteLine(msg);
            Console.ReadLine();
            Environment.Exit(0);
        }

        private static void HandleHelp(string[] args)
        {
            if (args.FirstOrDefault() == "/?")
            {
                string msg = string.Format(@"Arg 1 is the file you want to split.{0}Arg 2 is the number of files to create when splitting the content of the target.{0}If a non-int value is taken, the app defaults to {1}{0}e.g.{0}this.exe ""C:\path\to\file.txt"" 5", Environment.NewLine, DefaultNumFilesToCreate);
                ShowMsgAndQuit(msg);
            }
        }
    }
}
