using GalaSoft.MvvmLight.CommandWpf;
using RandomizedSteamPick.Canvas_Drawings;
using RandomizedSteamPick.Commands;
using RandomizedSteamPick.Images;
using RandomizedSteamPick.Model;
using RandomizedSteamPick.Tools;
using RandomizedSteamPick.View;
using Steam.SteamObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RandomizedSteamPick.ViewModel
{
    public class MainViewModel : ViewModel
    {
        private SteamList steamlist = new SteamList();
        private List<Game> list => steamlist.GameList;

        public ImageBrush Avatar { get { return new ImageBrush(GetAvatar()); } }
        private ImageSource GetAvatar()
        {
            if (File.Exists(steamlist.SteamID + ".jpg"))
            {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(steamlist.SteamID + ".jpg", UriKind.RelativeOrAbsolute);
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    return image;
            }
            else { return ImageService.GetUserPlaceholder(); }
        }

        public static AlphabeticComparer Alphabetic = new AlphabeticComparer();

        public int TimePlayedIndex { get; set; } = 0;
        public int GameTypeIndex { get; set; } = 0;

        public static Random Random = new Random();
        private RollAnimation rollAnimation;
        public ICommand RollCommand => new RollCommand(rollAnimation, steamlist);
        public RelayCommand<Window> ShowListCommand => new RelayCommand<Window>(ShowList);

        public ListWindow ListWindow;
        public void ShowList(Window owner)
        {
            if (ListWindow == null || !ListWindow.IsVisible)
            {
                ListWindow = new ListWindow() { DataContext = new ListViewModel(steamlist), Owner = owner };
                ListWindow.Closing += (sender,o) => { OnPropertyChanged("RollCommand"); OnPropertyChanged("Avatar"); };
                ListWindow.Show();
            }
        }

        public MainViewModel(Canvas canvas, TextBlock label, Image image) { Initialize(canvas, label, image); }
        public void Initialize(Canvas canvas, TextBlock label, Image image)
        {
            try {
                this.steamlist = FileService.LoadFile<SteamList>();
            } catch (IOException) {
                this.steamlist = new SteamList();
            }
            image.Source = ImageService.GetLogoPlaceholder();
            
            this.rollAnimation = new RollAnimation(steamlist, canvas, image, label);
            hotkey = new HotKey(Key.N, KeyModifier.Shift | KeyModifier.Ctrl, OnHotKeyHandler);
        }

        HotKey hotkey;
        private void OnHotKeyHandler(HotKey hotKey)
        {
            var command = RollCommand;
            if (command.CanExecute(TimePlayedIndex))
                command.Execute(TimePlayedIndex);
        }
    }
}