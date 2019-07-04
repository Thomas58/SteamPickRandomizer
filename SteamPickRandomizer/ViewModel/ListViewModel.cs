using GalaSoft.MvvmLight.CommandWpf;
using RandomizedSteamPick.Model;
using RandomizedSteamPick.Tools;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading;
using RandomizedSteamPick.View;
using Steam.SteamObjects;

namespace RandomizedSteamPick.ViewModel
{
    public class ListViewModel : ViewModel
    {
        private SteamList steamList;
        private List<Game> gameList => steamList.GameList;
        private List<Game> ignoreList => steamList.IgnoreList;

        private ObservableCollection<Game> _gameList;
        public ObservableCollection<Game> GameList { get { return _gameList; } set { _gameList = value; OnPropertyChanged(); } }
        private Game _selectedGame;
        public Game SelectedGame { get { return _selectedGame; } set { _selectedGame = value; OnPropertyChanged(); } }
        private ObservableCollection<Game> _ignoreList;
        public ObservableCollection<Game> IgnoreList { get { return _ignoreList; } set { _ignoreList = value; OnPropertyChanged(); } }
        private Game _selectedIgnore;
        public Game SelectedIgnore { get { return _selectedIgnore; } set { _selectedIgnore = value; OnPropertyChanged(); } }

        public RelayCommand<Window> ShowSteamIdWindowCommand => new RelayCommand<Window>(ShowSteamIdWindow);
        public SteamIdWindow SteamIdWindow;
        private void ShowSteamIdWindow(Window owner)
        {
            if (SteamIdWindow == null || !SteamIdWindow.IsVisible)
            {
                SteamIdWindow = new SteamIdWindow() { DataContext = new SteamIdViewModel(steamList, this), Owner = owner };
                SteamIdWindow.Show();
            }
        }

        private bool updateGameListWorking = false;
        public RelayCommand UpdateGameListCommand => new RelayCommand(UpdateGameListAsync, !updateGameListWorking);
        private async void UpdateGameListAsync()
        {
            updateGameListWorking = true;
            var task = SteamServiceExt.GetOwnedGamesAsync(steamList.SteamID);
            await task.ContinueWith((result) =>
            {
                //After web response is received, continue here
                var newList = result.Result.Games;
                var itemsTasks = 0;
                var itemsAdded = 0;
                object itemsLock = new object();
                var comparator = new AlphabeticComparer();
                //Start adding games to the game list
                lock (itemsLock)
                {
                    foreach (Game item in newList)
                    {
                        // If item is in another list already, don't add it
                        if (ignoreList.BinarySearch(item, comparator) < 0)
                        {
                            itemsTasks++;
                            Application.Current.Dispatcher.BeginInvoke((ThreadStart)delegate
                            {
                                bool added = false;
                                lock (gameList)
                                {
                                    if (gameList.Count == 0)
                                    {
                                        gameList.Add(item);
                                        GameList.Add(item);
                                        OnPropertyChanged("GameList");
                                        added = true;
                                    }
                                    if (comparator.Compare(gameList[gameList.Count - 1], item) < 0)
                                    {
                                        gameList.Add(item);
                                        GameList.Add(item);
                                        OnPropertyChanged("GameList");
                                        added = true;
                                    }
                                    if (comparator.Compare(gameList[0], item) > 0)
                                    {
                                        gameList.Insert(0, item);
                                        GameList.Insert(0, item);
                                        OnPropertyChanged("GameList");
                                        added = true;
                                    }
                                    int index = gameList.BinarySearch(item, comparator);
                                    // If index is negative, item is not present in list. Positive index means item is present in the list.
                                    if (index < 0)
                                    {
                                        index = ~index;
                                        gameList.Insert(index, item);
                                        GameList.Insert(index, item);
                                        OnPropertyChanged("GameList");
                                        added = true;
                                    }
                                }
                                lock (itemsLock)
                                {
                                    itemsTasks--;
                                    if (added)
                                        itemsAdded++;
                                }
                            });
                        }
                    }
                    //Create thread to show message when all tasks are done
                    new Thread(() =>
                    {
                        bool finished = false;
                        while (!finished)
                        {
                            Thread.Sleep(100);
                            lock (itemsLock)
                            {
                                if (itemsTasks == 0)
                                {
                                    FileService.SaveFile(steamList);
                                    MessageBox.Show(itemsAdded + " games added to game list");
                                    updateGameListWorking = false;
                                    finished = true;
                                }
                            }
                        }
                    }).Start();
                }
            });
            return;
        }

        public RelayCommand MoveGameToIgnoreListCommand => new RelayCommand(MoveGameToIgnoreList, () => _selectedGame != null && !updateGameListWorking);
        private void MoveGameToIgnoreList()
        {
            var selected = _selectedGame;
            int index = ignoreList.InsertSorted(selected, new AlphabeticComparer());
            IgnoreList.Insert(index, selected);
            gameList.Remove(selected);
            GameList.Remove(selected);
            OnPropertyChanged("GameList");
            OnPropertyChanged("IgnoreList");
            FileService.SaveFile(steamList);
        }

        public RelayCommand MoveIgnoreToGameListCommand => new RelayCommand(MoveIgnoreToGameList, () => _selectedIgnore != null && !updateGameListWorking);
        private void MoveIgnoreToGameList()
        {
            var selected = _selectedIgnore;
            int index = gameList.InsertSorted(selected, new AlphabeticComparer());
            GameList.Insert(index, selected);
            ignoreList.Remove(selected);
            IgnoreList.Remove(selected);
            OnPropertyChanged("GameList");
            OnPropertyChanged("IgnoreList");
            FileService.SaveFile(steamList);
        }

        public ListViewModel(SteamList list)
        {
            this.steamList = list;
            this._gameList = new ObservableCollection<Game>(gameList);
            this._ignoreList = new ObservableCollection<Game>(ignoreList);
        }
    }
}