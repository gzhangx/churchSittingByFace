using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfFaceApp
{
    public class Util
    {
        public static BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        public static BitmapImage getImgSrc(RecoInfo info)
        {
            using (Bitmap bmp = (Bitmap)Bitmap.FromFile(info.imageName))
            {
                return Convert(bmp);
            }
        }
    }
}
