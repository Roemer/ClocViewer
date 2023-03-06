using ClocAnalyzerLibrary;
using ClocViewer.Core;

namespace ClocViewer.ViewModels
{
    public class LanguageEntryViewModel : ObservableObject
    {
        public LanguageEntryViewModel(LocStats stats)
        {
            Language = stats.Type;
            CodeCount = stats.CodeCount;
            CommentCount = stats.CommentCount;
            BlankCount = stats.BlankCount;
            FileCount = stats.FileCount;
        }
        
        public string Language
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

        public long FileCount
        {
            get => GetValue<long>();
            set => SetValue(value);
        }
    }
}
