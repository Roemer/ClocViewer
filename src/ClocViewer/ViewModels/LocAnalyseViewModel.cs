using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ClocAnalyzerLibrary;
using ClocViewer.Core;
using Microsoft.Win32;

namespace ClocViewer.ViewModels
{
    public class LocAnalyseViewModel : ObservableObject
    {
        public LocAnalyseEntryViewModel Root { get; set; }

        public string SourcePath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string ClocPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string OptionsFilePath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string IgnoredFilePath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string CurrentPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public LocAnalyseEntryViewModel DisplayedEntry
        {
            get => GetValue<LocAnalyseEntryViewModel>();
            set => SetValue(value);
        }

        public string SelectionText
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public ObservableCollection<LanguageEntryViewModel> SelectedLanguages { get; set; }

        public bool IsGridFocused
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public ICommand MenuOpenCommand { get; }
        public ICommand MenuSaveCommand { get; }
        public ICommand MenuExitCommand { get; }

        public ICommand SourceBrowseCommand { get; }
        public ICommand ClocBrowseCommand { get; }
        public ICommand OptionsFileBrowseCommand { get; }
        public ICommand IgnoredFileBrowseCommand { get; }

        public ICommand AnalyzeCommand { get; }
        public ICommand MouseDoubleClickCommand { get; }
        public ICommand SelectionChangedCommand { get; }
        public ICommand CopyCommand { get; }

        public LocAnalyseViewModel()
        {
            SelectedLanguages = new ObservableCollection<LanguageEntryViewModel>();

            MenuOpenCommand = new RelayCommand(o =>
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Select the settings file",
                    Filter = "ClocViewer Settings (*.cloc)|*.cloc|All files (*.*)|*.*"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    // Reset
                    SourcePath = string.Empty;
                    OptionsFilePath = string.Empty;
                    IgnoredFilePath = string.Empty;
                    // Read and load the settings
                    var fileContent = File.ReadAllLines(openFileDialog.FileName);
                    foreach (var line in fileContent)
                    {
                        if (line.StartsWith("SourcePath"))
                        {
                            SourcePath = line.Substring(line.IndexOf('=') + 1);
                        }
                        else if (line.StartsWith("OptionsFilePath"))
                        {
                            OptionsFilePath = line.Substring(line.IndexOf('=') + 1);
                        }
                        else if (line.StartsWith("IgnoredFilePath"))
                        {
                            IgnoredFilePath = line.Substring(line.IndexOf('=') + 1);
                        }
                        else if (line.StartsWith("ClocPath"))
                        {
                            ClocPath = line.Substring(line.IndexOf('=') + 1);
                        }
                    }
                }
            });

            MenuSaveCommand = new RelayCommand(o =>
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Save the settings file",
                    Filter = "ClocViewer Settings (*.cloc)|*.cloc|All files (*.*)|*.*"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    var fileContent = new StringBuilder();
                    fileContent.AppendLine($"SourcePath={SourcePath}");
                    fileContent.AppendLine($"OptionsFilePath={OptionsFilePath}");
                    fileContent.AppendLine($"IgnoredFilePath={IgnoredFilePath}");
                    fileContent.AppendLine($"ClocPath={ClocPath}");
                    File.WriteAllText(saveFileDialog.FileName, fileContent.ToString());
                }
            });

            MenuExitCommand = new RelayCommand(o =>
            {
                Application.Current.Shutdown();
            });

            SourceBrowseCommand = new RelayCommand(o =>
            {
                FolderBrowser.BrowseForFolder("source", (s) => SourcePath = s);
            });

            ClocBrowseCommand = new RelayCommand(o =>
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Select the cloc.exe",
                    Filter = "Cloc Exe (*.exe)|*.exe"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    ClocPath = openFileDialog.FileName;
                }
            });

            OptionsFileBrowseCommand = new RelayCommand(o =>
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Select the options file",
                    Filter = "Text (*.txt)|*.txt|All files (*.*)|*.*"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    OptionsFilePath = openFileDialog.FileName;
                }
            });

            IgnoredFileBrowseCommand = new RelayCommand(o =>
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Select the ignored file",
                    Filter = "Text (*.txt)|*.txt|All files (*.*)|*.*"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    IgnoredFilePath = openFileDialog.FileName;
                }
            });

            AnalyzeCommand = new RelayCommand(o =>
            {
                var src = SourcePath;
                var rootFolder = LocAnalyzer.Analyze(new LocAnalyzerSettings
                {
                    RootPath = src,
                    ClocExePath = ClocPath,
                    IgnoredFile = IgnoredFilePath,
                    OptionsFile = OptionsFilePath
                });
                Root = new LocAnalyseEntryViewModel(rootFolder, true);
                DisplayedEntry = Root;
                CurrentPath = rootFolder.FullPath;
                SelectionText = "";
            });

            MouseDoubleClickCommand = new RelayCommand(o =>
            {
                var clickedEntry = (LocAnalyseEntryViewModel)o;
                if (clickedEntry.Name == "..")
                {
                    DisplayedEntry = clickedEntry.Parent.Parent;
                }
                else
                {
                    if (clickedEntry.IsFolder)
                    {
                        DisplayedEntry = clickedEntry;
                    }
                    else
                    {
                        // It is a file, so open it
                        Process.Start(@"C:\Program Files\Notepad++\notepad++.exe", @$"""{clickedEntry.FullPath}""");
                    }
                }

                CurrentPath = DisplayedEntry.FullPath;

                IsGridFocused = true;
            });

            SelectionChangedCommand = new RelayCommand(o =>
            {
                IList items = (IList)o;
                var collection = items.Cast<LocAnalyseEntryViewModel>();
                long codeCount = 0;
                long commentCount = 0;
                long blankCount = 0;
                var languageDictCount = new Dictionary<string, long>();
                foreach (var item in collection)
                {
                    codeCount += item.CodeCount;
                    commentCount += item.CommentCount;
                    blankCount += item.BlankCount;
                    if (item.IsFolder)
                    {
                        if (item.ModelFolder == null) continue;
                        foreach (var (key, value) in item.ModelFolder.LanguageCount)
                        {
                            languageDictCount.IncrementBy(key, value);
                        }
                    }
                    else
                    {
                        if (item.IsIgnored) continue;
                        languageDictCount.IncrementBy(item.FileType, item.CodeCount);
                    }
                }
                SelectionText = $"Code: {codeCount:n0}, Comment: {commentCount:n0}, Blank: {blankCount:n0}";
                SelectedLanguages.Clear();
                foreach (var keyValuePair in languageDictCount)
                {
                    SelectedLanguages.Add(new LanguageEntryViewModel { Language = keyValuePair.Key, LineCount = keyValuePair.Value });
                }
            });

            CopyCommand = new RelayCommand(o =>
            {
                IList items = (IList)o;
                var collection = items.Cast<LocAnalyseEntryViewModel>();
                var sb = new StringBuilder();
                sb.AppendLine("Name;Code;Comment;Blanks");
                foreach (var item in collection.OrderBy(x => !x.IsFolder).ThenBy(x => x.IsIgnored).ThenBy(x => x.Name))
                {
                    sb.AppendLine($"{item.Name};{item.CodeCount};{item.CommentCount};{item.BlankCount}");
                }
                Clipboard.SetText(sb.ToString());
            });
        }
    }
}
