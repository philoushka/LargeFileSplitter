using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LargeFileSplitter
{
    public class FileOps
    {
        public int NumFilesCreated { get; set; }
        private IEnumerable<string> AllLinesToWrite { get; set; }

        SplitJob Job { get; set; }
        
        /// <summary>
        /// Read the source file and split into a files.
        /// </summary>
        /// <param name="job"></param>
        public FileOps(SplitJob job)
        {
            this.Job = job;
            AllLinesToWrite = ReadFile(Job.FileToSplit);
            Job.TotalLines = AllLinesToWrite.Count();
        }

        /// <summary>
        /// Read the contents of the file from disk.
        /// </summary>
        public IEnumerable<string> ReadFile(string filePath)
        {
            try
            {
                return File.ReadLines(filePath).ToList();
            }
            catch (System.OutOfMemoryException)
            {
                return File.ReadLines(filePath);
            }
        }

        /// <summary>
        /// Given the large collection of strings read from disk, split into the appropriate number of files. If the source file is empty, tilt immediately.
        /// </summary>                
        public void SplitSourceFileToMultiples()
        {
            if (AllLinesToWrite.Any() == false) return;
            string header = AllLinesToWrite.First();
            foreach (int fileNumber in Enumerable.Range(1, Job.NumFilesToCreate))
            {
                WriteItemsToNewFile(header, fileNumber);
            }
        }

        private void WriteItemsToNewFile(string header, int fileNumber)
        {
            FileInfo baseFile = new FileInfo(Job.FileToSplit);
            string newFileName = Path.Combine(baseFile.DirectoryName, "{0}-{1}".FormatWith(fileNumber, baseFile.Name));

            //always skip the header, the batches previously taken + 1 for the header.
            //First iteration, skip none.
            int skip = ((fileNumber - 1) * Job.LinesPerFile + 1);

            List<string> newFileContents = AllLinesToWrite.Skip(skip).Take(Job.LinesPerFile).ToList();

            //write the file when there are new contents to be written. we may have the case where we don't need to create empty files if the user
            //has specified more files than we need.
            if (newFileContents.Any())
            {
                newFileContents.Insert(0, header);
                //remove the newline on the last item to ensure that an empty line isn't in the new file.
                newFileContents[newFileContents.Count - 1] = newFileContents.Last().Replace(Environment.NewLine, "");
                File.WriteAllLines(newFileName, newFileContents);
                NumFilesCreated++;
            }
        }
    }
}
