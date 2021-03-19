using System.Windows;
using ClocViewer.ViewModels;

namespace ClocViewer.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var vm = new LocAnalyseViewModel();
            DataContext = vm;
        }
    }
}
