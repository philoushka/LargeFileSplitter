using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeFileSplitter
{
    public class FileOps
    {
        public int NumFilesCreated { get; set; }
        private IEnumerable<string> AllLinesToWrite { get; set; }
        SplitJob Job { get; set; }
        /// <summary>
        /// Read the source file and split into 
        /// </summary>
        /// <param name="job"></param>

        public FileOps(SplitJob job)
        {
            this.Job = job;
            AllLinesToWrite = ReadFile(Job.FileToSplit);
            Job.TotalLines = AllLinesToWrite.Count();
        }
        public void SplitFile()
        {
            var sw = Stopwatch.StartNew();

            if (AllLinesToWrite.Any() == false) return;

            WriteFiles();
            sw.Stop();
            Console.WriteLine("Time Spent (s): " + sw.Elapsed.TotalSeconds.ToString());
        }

        /// <summary>
        /// Read the contents of the file from disk.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public IEnumerable<string> ReadFile(string filePath)
        {
            try
            {
                return File.ReadLines(filePath).ToList();
            }
            catch (System.OutOfMemoryException)
            {
                Console.WriteLine("We encountered an OutOfMemoryException. Switching to a slower method of splitting.");
                return File.ReadLines(filePath);
            }
        }


        /// <summary>
        /// Given the large collection of strings read from disk, split into the appropriate number of files. If the source file is empty, tilt immediately.
        /// </summary>                
        public void WriteFiles()
        {
            if (AllLinesToWrite.Any() == false) return;
            string header = AllLinesToWrite.First();
            foreach (int i in Enumerable.Range(1, Job.NumFilesToCreate))
            {
                FileInfo baseFile = new FileInfo(Job.FileToSplit);
                string newFile = Path.Combine(baseFile.DirectoryName, string.Format("{0}-{1}", i, baseFile.Name));

                //always skip the header, the batches previously taken + 1 for the header.
                //First iteration, skip none.
                int skip = ((i-1) * Job.LinesPerFile + 1);

                List<string> newFileContents = AllLinesToWrite.Skip(skip).Take(Job.LinesPerFile).ToList();

                //write the file when there are new contents to be written. we may have the case where we don't need to create empty files if the user
                //has specified more files than we need.
                if (newFileContents.Any())
                {
                    newFileContents.Insert(0, header);
                    File.WriteAllLines(newFile, newFileContents);
                    NumFilesCreated++;
                }
            }
        }

    }
}
