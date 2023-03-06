using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClocAnalyzerLibrary
{
    public class FolderStats : LocStats
    {
        public string Name { get => Path.GetFileName(FullPath); }
        public string FullPath { get; set; }
        public List<FolderStats> Folders { get; set; }
        public List<FileStats> Files { get; set; }

        public Dictionary<string, LocStats> Languages { get; set; }

        public FolderStats()
        {
            Folders = new List<FolderStats>();
            Files = new List<FileStats>();
            Languages = new Dictionary<string, LocStats>();
        }

        internal void AddFile(FileStats fileStats)
        {
            var relativePathToFolder = Path.GetRelativePath(FullPath, fileStats.FullPath);
            var pathParts = relativePathToFolder.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            if (pathParts.Length > 1)
            {
                // The file is in another subfolder
                var subfolder = GetOrCreateFolder(Path.Combine(FullPath, pathParts[0]));
                subfolder.AddFile(fileStats);
            }
            else
            {
                // Seems to be a file in the current folder
                Files.Add(fileStats);
            }
            // Add the stats to the current folder
            this.AddStats(fileStats);
            // Language stats
            ExtensionMethods.AddStatsToDict(Languages, fileStats);
        }

        internal FolderStats GetOrCreateFolder(string folderPath)
        {
            var desiredFolderName = Path.GetFileName(folderPath);
            var desiredFolder = Folders.Find(x => x.Name == desiredFolderName);
            if (desiredFolder == null)
            {
                desiredFolder = new FolderStats { FullPath = folderPath };
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
