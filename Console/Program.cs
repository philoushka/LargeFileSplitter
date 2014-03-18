using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace LargeFileSplitter
{
    class Program
    {
        const int DefaultNumFilesToCreate = 2;

        static void Main(string[] args)
        {
            HandleHelp(args);
            SplitJob job = BuildSplitJob(args);
            var fileOps = new FileOps(job);
            try
            {
                fileOps.SplitFile();
            }
            catch (System.IO.IOException ioe)
            {
                ShowMsgAndQuit(ioe.ToString());
            }
            ShowMsgAndQuit(string.Format("Done. Created {0} files. Press Enter to quit.",fileOps.NumFilesCreated));
        }

        /// <summary>
        /// Take a user's argument that's supposed to be an integer. If it parses as an int, return it as an int. If not, use the application's default. If it's a decimal, that'll use the app default.
        /// </summary>
        /// <param name="input">An argument from the user that may or may not hold an int,</param>
        /// <returns>An integer with the value supplied by the user</returns>
        static int NumberOrAppDefault(string input)
        {
            int num;
            return (int.TryParse(input, out num)) ? num : DefaultNumFilesToCreate;
        }

        /// <summary>
        /// Analyze the input from the user. Ensure all args are valid to continue. If any args are invalid, then show a message to the user and cause the app to exit.
        /// </summary>
        /// <param name="args">All the args given by the user</param>
        /// <returns>An object populated with the file to split, and the number of files to split to.</returns>
        static SplitJob BuildSplitJob(string[] args)
        {
            SplitJob job = new SplitJob();

            if (args.FirstOrDefault() != null) { job.FileToSplit = args.First(); }
            if (args.LastOrDefault() != null) { job.NumFilesToCreate = NumberOrAppDefault(args.LastOrDefault()); }

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
                job.NumFilesToCreate = NumberOrAppDefault(numFiles);
                Console.WriteLine(string.Format("Attempting to split into {0} files.", job.NumFilesToCreate));
            }
            return job;
        }

        /// <summary>
        /// Show a message to the console and quit the program.
        /// </summary>
        /// <param name="msg">A message to show the user.</param>
        private static void ShowMsgAndQuit(string msg)
        {
            Console.WriteLine(msg);
            Console.ReadLine();
            Environment.Exit(0);
        }

        /// <summary>
        /// Detect whether the 'help' param was supplied. Detects for /? being the first argument. If found, show the help text for the user.
        /// </summary>
        /// <param name="args">All args passed to the app.</param>
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
