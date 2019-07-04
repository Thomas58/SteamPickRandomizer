using RandomizedSteamPick.Images;
using System;
using System.IO;
using System.Net;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Steam;
using Steam.SteamObjects;
using System.Threading.Tasks;

namespace RandomizedSteamPick.Model
{
    public class SteamServiceExt : SteamService
    {
        public static ImageSource GetLogo(Game game)
        {
            byte[] bytes;
            using (WebClient client = new WebClient())
            {
                try
                {
                    bytes = client.DownloadData(game.LogoUri);
                }
                catch (Exception e) when (e is WebException || e is ArgumentException)
                {
                    return ImageService.GetLogoPlaceholder();
                }
            }
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(bytes);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();
            return image;
        }
        public static Task<ImageSource> GetLogoAsync(Game game)
        {
            return Task.Run(()=>GetLogo(game));
        }

        public static ImageSource GetAvatar(Player user)
        {
            if (!File.Exists(user.SteamId + ".jpg"))
            {
                using (WebClient webclient = new WebClient())
                {
                    try
                    {
                        webclient.DownloadFile(user.AvatarFull, user.SteamId + ".jpg");
                    }
                    catch (WebException)
                    {
                        return ImageService.GetAvatarPlaceholder();
                    }
                }
            }
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(user.SteamId + ".jpg", UriKind.RelativeOrAbsolute);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();
            return image;
        }
    }
}
