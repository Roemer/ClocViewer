namespace ClocAnalyzerLibrary
{
    public class LocStats
    {
        public string Type { get; set; }
        public long Blank { get; set; }
        public long Comment { get; set; }
        public long Code { get; set; }
        public bool IsIgnored { get; set; }
        public string IgnoreReason { get; set; }

        public LocStats(string type, long blank, long comment, long code)
        {
            Type = type;
            Blank = blank;
            Comment = comment;
            Code = code;
        }

        public LocStats(string ignoreReason)
        {
            IsIgnored = true;
            IgnoreReason = ignoreReason;
        }

        public override string ToString()
        {
            if (IsIgnored)
            {
                return $"Ignored: {IgnoreReason}";
            }
            return $"Type: {Type}, Blank: {Blank}, Comment: {Comment}, Code: {Code}";
        }

        public static LocStats operator +(LocStats a, LocStats b)
        {
            a.Blank += b.Blank;
            a.Comment += b.Comment;
            a.Code += b.Code;
            return a;
        }
    }
}
