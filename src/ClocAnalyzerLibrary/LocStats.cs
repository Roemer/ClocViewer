namespace ClocAnalyzerLibrary
{
    public class LocStats
    {
        public string Type { get; set; }
        public long FileCount { get; set; }
        public long CodeCount { get; set; }
        public long CommentCount { get; set; }
        public long BlankCount { get; set; }

        public override string ToString()
        {
            return $"Type: {Type}, Code: {CodeCount}, Comments: {CommentCount}, Blanks: {BlankCount}";
        }

        public void AddStats(LocStats otherStats)
        {
            FileCount += otherStats.FileCount;
            CodeCount += otherStats.CodeCount;
            CommentCount += otherStats.CommentCount;
            BlankCount += otherStats.BlankCount;
        }
    }
}
