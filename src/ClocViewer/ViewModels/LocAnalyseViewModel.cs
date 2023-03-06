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

        public string ClocOptionsText
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

        public ICommand MenuOpenResult { get; }
        public ICommand MenuSaveResult { get; }

        public ICommand SourceBrowseCommand { get; }
        public ICommand ClocBrowseCommand { get; }
        public ICommand OptionsFileBrowseCommand { get; }
        public ICommand IgnoredFileBrowseCommand { get; }

        public ICommand AnalyzeCommand { get; }
        public ICommand MouseDoubleClickCommand { get; }
        public ICommand SelectionChangedCommand { get; }
        public ICommand CopyPathStatsCommand { get; }
        public ICommand CopyLanguageStatsCommand { get; }

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
                    ClocOptionsText = string.Empty;
                    // Read and load the settings
                    var fileContent = File.ReadAllLines(openFileDialog.FileName);
                    var lastConfigLine = -1;
                    for (int i = 0; i < fileContent.Length; i++)
                    {
                        string line = fileContent[i];
                        if (line.StartsWith("#SourcePath"))
                        {
                            SourcePath = line.Substring(line.IndexOf('=') + 1);
                            lastConfigLine = i;
                        }
                        else if (line.StartsWith("#ClocPath"))
                        {
                            ClocPath = line.Substring(line.IndexOf('=') + 1);
                            lastConfigLine = i;
                        }
                    }
                    ClocOptionsText = string.Join(Environment.NewLine, fileContent[(lastConfigLine + 1)..]);
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
                    fileContent.AppendLine($"#SourcePath={SourcePath}");
                    fileContent.AppendLine($"#ClocPath={ClocPath}");
                    fileContent.Append($"{ClocOptionsText}");
                    File.WriteAllText(saveFileDialog.FileName, fileContent.ToString());
                }
            });

            MenuSaveResult = new RelayCommand(o =>
            {
                // TODO
            });

            MenuOpenResult = new RelayCommand(o =>
            {
                // TODO
            });

            MenuExitCommand = new RelayCommand(o =>
            {
                Application.Current.Shutdown();
            });

            SourceBrowseCommand = new RelayCommand(o =>
            {
                using var dialog = new System.Windows.Forms.FolderBrowserDialog
                {
                    Description = $"Select a source folder",
                    UseDescriptionForTitle = true,
                    SelectedPath = SourcePath,
                    ShowNewFolderButton = true
                };

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SourcePath = dialog.SelectedPath;
                }
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

            AnalyzeCommand = new RelayCommand(o =>
            {
                var src = SourcePath;
                try
                {
                    var rootFolder = LocAnalyzer.Analyze(new LocAnalyzerSettings
                    {
                        RootPath = src,
                        ClocExePath = ClocPath,
                        ClocOptions = ClocOptionsText,
                    });
                    Root = new LocAnalyseEntryViewModel(rootFolder, true);
                    DisplayedEntry = Root;
                    CurrentPath = rootFolder.FullPath;
                    SelectionText = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to get data: " + ex.Message);
                }
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
                long fileCount = 0;
                var languageDict = new Dictionary<string, LocStats>();
                foreach (var item in collection)
                {
                    // Total counts
                    codeCount += item.CodeCount;
                    commentCount += item.CommentCount;
                    blankCount += item.BlankCount;
                    fileCount += item.FileCount;

                    // Language statistics
                    if (item.IsFolder)
                    {
                        // Skip special folders like ".."
                        if (item.ModelFolder == null) continue;
                        // Add all languages
                        foreach (var (key, value) in item.ModelFolder.Languages)
                        {
                            ExtensionMethods.AddStatsToDict(languageDict, value);
                        }
                    }
                    else
                    {
                        // Add a single file
                        ExtensionMethods.AddStatsToDict(languageDict, item.ModelFile);
                    }
                }
                // Taskbar Text for total counts
                SelectionText = $"Files: {fileCount:n0}, Code: {codeCount:n0}, Comment: {commentCount:n0}, Blank: {blankCount:n0}";
                // Update Language statistics
                SelectedLanguages.Clear();
                foreach (var (key, value) in languageDict)
                {
                    SelectedLanguages.Add(new LanguageEntryViewModel(value));
                }
            });

            CopyPathStatsCommand = new RelayCommand(o =>
            {
                IList items = (IList)o;
                var collection = items.Cast<LocAnalyseEntryViewModel>();
                var sb = new StringBuilder();
                sb.AppendLine("Name;Files;Code;Comment;Blanks");
                foreach (var item in collection.OrderBy(x => !x.IsFolder).ThenBy(x => x.IsIgnored).ThenBy(x => x.Name))
                {
                    sb.AppendLine($"{item.Name};{item.FileCount};{item.CodeCount};{item.CommentCount};{item.BlankCount}");
                }
                Clipboard.SetText(sb.ToString());
            });

            CopyLanguageStatsCommand = new RelayCommand(o =>
            {
                IList items = (IList)o;
                var collection = items.Cast<LanguageEntryViewModel>();
                var sb = new StringBuilder();
                sb.AppendLine("Language;Files;Code;Comment;Blanks");
                foreach (var item in collection.OrderByDescending(x => x.CodeCount).ThenByDescending(x => x.FileCount))
                {
                    sb.AppendLine($"{item.Language};{item.FileCount};{item.CodeCount};{item.CommentCount};{item.BlankCount}");
                }
                Clipboard.SetText(sb.ToString());
            });
        }
    }
}
