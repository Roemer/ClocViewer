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

        public long BlankCount
        {
            get => GetValue<long>();
            set => SetValue(value);
        }

        public long CommentCount
        {
            get => GetValue<long>();
            set => SetValue(value);
        }

        public long CodeCount
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

        public LocFolder ModelFolder { get; }

        public LocAnalyseEntryViewModel(LocFolder folder, bool isRoot = false)
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
            FileCount = folder.TotalFilesCount;
            FillFromStats(folder.Stats);
        }

        public LocAnalyseEntryViewModel(string name)
        {
            Name = name;
        }

        public LocAnalyseEntryViewModel(LocFile file)
        {
            Name = file.Name;
            FullPath = file.FullPath;
            FillFromStats(file.Stats);
        }

        private void FillFromStats(LocStats stats)
        {
            FileType = stats.Type;
            BlankCount = stats.Blank;
            CommentCount = stats.Comment;
            CodeCount = stats.Code;
            IgnoreReason = stats.IgnoreReason;
            IsIgnored = stats.IsIgnored;
        }
    }
}
