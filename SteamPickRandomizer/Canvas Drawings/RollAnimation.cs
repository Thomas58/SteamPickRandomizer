using RandomizedSteamPick.CanvasDrawings;
using RandomizedSteamPick.Images;
using RandomizedSteamPick.Model;
using RandomizedSteamPick.Tools;
using Steam.SteamObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RandomizedSteamPick.Canvas_Drawings
{
    public class RollAnimation
    {
        private SteamList steamlist;
        private List<Game> gameList => steamlist.GameList;
        private List<Game> ignoreList => steamlist.IgnoreList;
        private Canvas canvas;
        private Image mainImage;
        private TextBlock mainText;
        private Task<ImageSource> logoTask;

        public RollAnimation(SteamList list, Canvas canvas, Image image, TextBlock text)
        {
            this.steamlist = list;
            this.canvas = canvas;
            this.mainImage = image;
            this.mainText = text;
        }

        public void Start(int finalIndex)
        {
            this.mainImage.Source = ImageService.GetLogoPlaceholder();
            logoTask = SteamServiceExt.GetLogoAsync(gameList[finalIndex]);
        }

        public void Update(int index)
        {
            mainText.Dispatcher.Invoke(() => mainText.Text = gameList[index].Name);
        }

        public void Finish(int index)
        {
            canvas.Dispatcher.Invoke(() => {
                var selected = gameList[index];
                mainText.Text = selected.Name;
                mainImage.Source = logoTask.Result;
                CommandManager.InvalidateRequerySuggested();
                new SpreadingCircles(canvas).Run();
                int ignoreindex = ignoreList.InsertSorted(selected, new AlphabeticComparer());
                gameList.Remove(selected);
                FileService.SaveFile(steamlist);
            });
        }
    }
}
