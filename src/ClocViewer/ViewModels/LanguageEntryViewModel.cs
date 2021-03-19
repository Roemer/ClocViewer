using ClocViewer.Core;

namespace ClocViewer.ViewModels
{
    public class LanguageEntryViewModel : ObservableObject
    {
        public string Language
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public long LineCount
        {
            get => GetValue<long>();
            set => SetValue(value);
        }
    }
}
