using RandomizedSteamPick.ViewModel;
using System.Windows;

namespace RandomizedSteamPick.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(DrawingCanvas, MainText, MainImage);
        }
    }
}
