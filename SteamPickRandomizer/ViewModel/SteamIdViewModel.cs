using GalaSoft.MvvmLight.CommandWpf;
using RandomizedSteamPick.Images;
using RandomizedSteamPick.Model;
using RandomizedSteamPick.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Steam.SteamObjects;
using System.Windows;

namespace RandomizedSteamPick.ViewModel
{
    public class SteamIdViewModel : ViewModel
    {
        private ListViewModel OwnerViewModel;
        private SteamList steamlist;
        public string Username { get { return steamlist.Username; } set { steamlist.Username = value; OnPropertyChanged(); } }
        public ulong SteamID { get { return steamlist.SteamID; } set { steamlist.SteamID = value; OnPropertyChanged(); } }
        private ImageSource _avatar = ImageService.GetAvatarPlaceholder();
        public ImageSource Avatar { get { return _avatar; } set { _avatar = value; OnPropertyChanged(); } }

        public RelayCommand<string> SteamIdCommand => new RelayCommand<string>(SteamId, task == null || task.Status != TaskStatus.Running);
        public Task<PlayerSummaries> task;
        private void SteamId(string steamIdString)
        {
            ulong steamid;
            try
            {
                steamid = ulong.Parse(steamIdString);
            }
            catch (FormatException e)
            {
                MessageBox.Show(e.Message);
                return;
            }
            task = SteamServiceExt.GetPlayerSummaryAsync(steamid);
            task.ContinueWith((result) =>
            {
                Player user = result.Result.Players[0];
                Username = user.PersonaName;
                SteamID = user.SteamId;

                ImageSource image = SteamServiceExt.GetAvatar(user);
                image.Freeze();
                Avatar = image;

                steamlist.Username = Username;
                steamlist.SteamID = SteamID;
                steamlist.GameList = new List<Game>();
                steamlist.IgnoreList = new List<Game>();

                OwnerViewModel.GameList = new ObservableCollection<Game>(steamlist.GameList);
                OwnerViewModel.IgnoreList = new ObservableCollection<Game>(steamlist.IgnoreList);
                FileService.SaveFile(steamlist);
            });
        }

        public SteamIdViewModel(SteamList steamlist, ListViewModel owner)
        {
            this.steamlist = steamlist;
            this.OwnerViewModel = owner;
            if (File.Exists(steamlist.SteamID + ".jpg"))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(steamlist.SteamID + ".jpg", UriKind.RelativeOrAbsolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                Avatar = image;
            }
            else
            {
                Avatar = ImageService.GetUserPlaceholder();
            }
        }

    }
}
