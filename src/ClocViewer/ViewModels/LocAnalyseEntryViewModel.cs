using System.Collections.ObjectModel;
using ClocAnalyzerLibrary;
using ClocViewer.Core;

namespace ClocViewer.ViewModels
{
    public class LocAnalyseEntryViewModel : ObservableObject
    {
        public ObservableCollection<LocAnalyseEntryViewModel> Entries { get; set; }

        public LocAnalyseEntryViewModel Parent { get; set; }

        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public long FileCount
        {
            get => GetValue<long>();
            set => SetValue(value);
        }

        public string FileType
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public long CodeCount
        {
            get => GetValue<long>();
            set => SetValue(value);
        }

        public long CommentCount
        {
            get => GetValue<long>();
            set => SetValue(value);
        }

        public long BlankCount
        {
            get => GetValue<long>();
            set => SetValue(value);
        }

        public bool IsFolder
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public bool IsIgnored
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public string IgnoreReason
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string FullPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public FileStats ModelFile { get; }
        public FolderStats ModelFolder { get; }

        public LocAnalyseEntryViewModel(FileStats file)
        {
            ModelFile = file;
            Name = file.Name;
            FullPath = file.FullPath;
            FileCount = file.FileCount;
            FillFromStats(file);
        }

        public LocAnalyseEntryViewModel(FolderStats folder, bool isRoot = false)
        {
            Entries = new ObservableCollection<LocAnalyseEntryViewModel>();
            IsFolder = true;
            ModelFolder = folder;

            if (!isRoot)
            {
                Entries.Add(new LocAnalyseEntryViewModel("..") { IsFolder = true, Parent = this });
            }

            foreach (var dir in folder.Folders)
            {
                Entries.Add(new LocAnalyseEntryViewModel(dir) { Parent = this });
            }

            foreach (var file in folder.Files)
            {
                Entries.Add(new LocAnalyseEntryViewModel(file));
            }

            Name = folder.Name;
            FullPath = folder.FullPath;
            FileCount = folder.FileCount;
            FillFromStats(folder);
        }

        public LocAnalyseEntryViewModel(string name)
        {
            Name = name;
        }

        private void FillFromStats(LocStats stats)
        {
            CodeCount = stats.CodeCount;
            CommentCount = stats.CommentCount;
            BlankCount = stats.BlankCount;
        }

        private void FillFromStats(FileStats stats)
        {
            FillFromStats((LocStats)stats);
            FileType = stats.Type;
            IgnoreReason = stats.IgnoreReason;
            IsIgnored = stats.IsIgnored;
        }
    }
}
