using System.Reflection;
using System.Windows.Media.Imaging;

namespace RandomizedSteamPick.Images
{
    public class ImageService
    {
        public static BitmapImage GetLogoPlaceholder()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("SteamPickRandomizer.Images.Logo-Placeholder.png");
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            return image;
        }

        public static BitmapImage GetAvatarPlaceholder()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("SteamPickRandomizer.Images.Avatar-Placeholder.png");
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            return image;
        }

        public static BitmapImage GetUserPlaceholder()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("SteamPickRandomizer.Images.User-Placeholder.png");
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            return image;
        }
    }
}
