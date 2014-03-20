using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeFileSplitter
{
    public static class UserMessages
    {
        public const string Help = @"Arg 1 is the file you want to split.
Arg 2 is the number of files to create when splitting the content of the target.
this.exe ""C:\path\to\file.txt"" 5
Press Enter to quit.";

        public const string AskWhichFile = "Which file would you like to split?";

        public const string CannotFindFile = "Couldn't find that file. We're done here.";

        public const string AskHowManyFiles = "How many files would you like this split into?";

        public const string MsgAttemptingToSplit = "Attempting to split into {0} files.";

        public const string TimeSpent = "Time Spent (s): {0}";
            public const string CompletedFileCreation="Done. Created {0} files. Press Enter to quit.";
    }
}
