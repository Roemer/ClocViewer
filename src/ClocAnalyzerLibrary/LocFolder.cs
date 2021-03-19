using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClocAnalyzerLibrary
{
    public class LocFolder
    {
        public string Name { get; set; }
        public LocStats Stats { get; set; }
        public List<LocFolder> Folders { get; set; }
        public List<LocFile> Files { get; set; }
        public long TotalFilesCount => Files.Count + Folders.Sum(x => x.TotalFilesCount);
        public string FullPath { get; set; }

        public IDictionary<string, long> LanguageCount
        {
            get
            {
                var ret = new Dictionary<string, long>();
                foreach (var file in Files.Where(file => !file.IsIgnored))
                {
                    ret.IncrementBy(file.Stats.Type, file.Stats.Code);
                }
                foreach (var (key, value) in Folders.SelectMany(folder => folder.LanguageCount))
                {
                    ret.IncrementBy(key, value);
                }
                return ret;
            }
        }

        public LocFolder()
        {
            Folders = new List<LocFolder>();
            Files = new List<LocFile>();
            Stats = new LocStats("Folder", 0, 0, 0);
        }

        public void AddPath(string path, LocStats fileStats)
        {
            //Console.WriteLine(path);
            var parts = path.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                // The file is in another subfolder
                var subfolder = GetOrCreateFolder(parts[0]);
                subfolder.AddPath(string.Join(Path.DirectorySeparatorChar, parts.Skip(1)), fileStats);
            }
            else
            {
                // Seems to be a file
                var fileName = parts[0];
                Files.Add(new LocFile { Name = fileName, Stats = fileStats, FullPath = Path.Combine(FullPath, fileName) });
            }
            // Add the stats to the current folder
            Stats += fileStats;
        }

        public LocFolder GetOrCreateFolder(string folderName)
        {
            var desiredFolder = Folders.Find(x => x.Name == folderName);
            if (desiredFolder == null)
            {
                desiredFolder = new LocFolder { Name = folderName, FullPath = Path.Combine(FullPath, folderName) };
                Folders.Add(desiredFolder);
            }
            return desiredFolder;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
