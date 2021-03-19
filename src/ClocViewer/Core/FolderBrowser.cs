using System;
using System.IO;

namespace ClocViewer.Core
{
    public static class FolderBrowser
    {
        public static bool BrowseForFolder(string name, Action<string> okAction)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = $"Select a {name} folder",
                UseDescriptionForTitle = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + Path.DirectorySeparatorChar,
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                okAction?.Invoke(dialog.SelectedPath);
                return true;
            }
            return false;
        }
    }
}
