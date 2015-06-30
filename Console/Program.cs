using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
namespace LargeFileSplitter
{
    class Program
    {
        const int DefaultNumFilesToCreate = 2;
        const string HelpArg = "/?";
        static char[] FilePathDelimiters = new[] { '"' };

        static void Main(string[] args)
        {
            HandleHelp(args);
            SplitJob job = BuildSplitJob(args);
            var sw = Stopwatch.StartNew();

            var fileOps = new FileOps(job);
            try
            {
                fileOps.SplitSourceFileToMultiples();
                sw.Stop();

                Console.WriteLine(UserMessages.TimeSpent.FormatWith(sw.Elapsed.TotalSeconds));
            }
            catch (IOException ioe)
            {
                ShowMsgAndQuit(ioe.ToString());
            }
            ShowMsgAndQuit(UserMessages.CompletedFileCreation.FormatWith(fileOps.NumFilesCreated));
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
            var job = new SplitJob();

            if (args.FirstOrDefault() != null)
            {
                job.FileToSplit = args.First();

            }
            if (args.LastOrDefault() != null)
            {
                job.NumFilesToCreate = NumberOrAppDefault(args.LastOrDefault());
            }

            EnsureGoodFileArg(job);
            EnsureGoodNumFilesArg(job);
            return job;
        }

        /// <summary>
        /// If the file to split is missing, prompt the user. Clean the arg of delimiters. Ensure that the file exists.
        /// </summary>
        /// <param name="job">The job to analyze</param>
        private static void EnsureGoodFileArg(SplitJob job)
        {
            if (job.FileToSplit.IsNullOrEmpty())
            {
                job.FileToSplit = PromptUserForAnswer(UserMessages.AskWhichFile);
            }
            job.FileToSplit = job.FileToSplit.Trim(FilePathDelimiters);

            if (File.Exists(job.FileToSplit) == false)
            {
                ShowMsgAndQuit(UserMessages.CannotFindFile);
            }
        }

        /// <summary>
        /// Prompt the user if no number of files exists.
        /// </summary>
        private static void EnsureGoodNumFilesArg(SplitJob job)
        {
            if (job.NumFilesToCreate == 0)
            {
                string numFiles = PromptUserForAnswer(UserMessages.AskHowManyFiles);
                job.NumFilesToCreate = NumberOrAppDefault(numFiles);
                Console.WriteLine(UserMessages.MsgAttemptingToSplit.FormatWith(job.NumFilesToCreate));
            }
        }

        /// <summary>
        /// Show a message to the user on the console, and take in a response.
        /// </summary>
        /// <param name="question">The question/statment to show the user.</param>
        /// <returns>An answer given by the user.</returns>
        private static string PromptUserForAnswer(string question)
        {
            Console.WriteLine(question);
            return Console.ReadLine();
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
            if (args.FirstOrDefault() == HelpArg)
            {
                ShowMsgAndQuit(UserMessages.Help.FormatWith(Environment.NewLine, DefaultNumFilesToCreate));
            }
        }
    }
}
