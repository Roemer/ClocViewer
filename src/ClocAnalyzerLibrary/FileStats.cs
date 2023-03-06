using System.IO;

namespace ClocAnalyzerLibrary
{
    public class FileStats : LocStats
    {
        public string Name { get => Path.GetFileName(FullPath); }
        public string FullPath { get; set; }
        public bool IsIgnored { get; set; }
        public string IgnoreReason { get; set; }

        public FileStats(string type, long codeCount, long commentCount, long blankCount)
        {
            FileCount = 1;
            Type = type;
            CodeCount = codeCount;
            CommentCount = commentCount;
            BlankCount = blankCount;
        }

        public FileStats(string ignoreReason)
        {
            Type = "Ignored";
            IsIgnored = true;
            IgnoreReason = ignoreReason;
        }

        public override string ToString()
        {
            if (IsIgnored)
            {
                return $"Ignored: {IgnoreReason}";
            }
            return $"{Name}: {base.ToString()}";
        }
    }
}
