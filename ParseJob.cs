
namespace LargeFileSplitter
{
    public class ParseJob
    {
        public int NumFilesToCreate { get; set; }
        public string FileToSplit { get; set; }
        public int LinesPerFile { get; set; }

        public string WriteDir { get { return   System.IO.Path.GetDirectoryName(this.FileToSplit); } }

    }


}
