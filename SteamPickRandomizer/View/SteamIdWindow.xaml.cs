using System.Windows;

namespace RandomizedSteamPick.View
{
    /// <summary>
    /// Interaction logic for SteamIdWindow.xaml
    /// </summary>
    public partial class SteamIdWindow : Window
    {
        public SteamIdWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Owner.Activate();
        }
    }
}
