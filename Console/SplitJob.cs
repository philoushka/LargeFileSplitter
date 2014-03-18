using System;
namespace LargeFileSplitter
{
    public class SplitJob
    {
        /// <summary>
        /// The user's choice for the max number of files to create.
        /// </summary>
        public int NumFilesToCreate { get; set; }
        /// <summary>
        /// The large file on disk to be split.
        /// </summary>
        public string FileToSplit { get; set; }
        
        /// <summary>
        /// A calculated value determining how many lines per file. It's approximately TotalLines/NumFilesToCreate with rounding up to avoid situations where lines are missed.
        /// </summary>
        public int LinesPerFile { get { return Convert.ToInt32(Math.Ceiling((this.TotalLines / this.NumFilesToCreate))); } } //Ceiling to avoid missing fractions of lines
        
        /// <summary>
        /// The number of lines that the large file contains.
        /// </summary>
        public decimal TotalLines { get; set; }
    }
}
