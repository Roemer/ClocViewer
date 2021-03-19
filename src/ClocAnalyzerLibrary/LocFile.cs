namespace ClocAnalyzerLibrary
{
    public class LocFile
    {
        public string Name { get; set; }
        public LocStats Stats { get; set; }
        public bool IsIgnored => Stats.IsIgnored;
        public string FullPath { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Stats}";
        }
    }
}
