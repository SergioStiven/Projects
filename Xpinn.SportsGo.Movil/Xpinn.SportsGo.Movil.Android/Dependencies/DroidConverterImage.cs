using Android.Graphics;
using Java.Util;
using System;
using System.IO;
using System.Threading.Tasks;
using Xpinn.SportsGo.Movil.Abstract;
using Xpinn.SportsGo.Util.Portable.Enums;
using BaseAndroid = Android;

namespace Xpinn.SportsGo.Movil.Android.Dependencies
{
    [Xamarin.Forms.Internals.Preserve(true, true)]
    class DroidConverterImage : IHelperImagen
    {
        public async Task<string> ConvertImage(string imagenFilePath, int quality, FormatoImagen formatoImagen)
        {
            if (string.IsNullOrWhiteSpace(imagenFilePath)) throw new ArgumentNullException("File path de la imagen a convertir esta vacio!.");
            if (formatoImagen == FormatoImagen.SinTipoFormato) throw new ArgumentNullException("Formato de imagen invalido!.");

            string uniqueID = UUID.RandomUUID().ToString();

            using (Java.IO.File path = BaseAndroid.OS.Environment.GetExternalStoragePublicDirectory(BaseAndroid.OS.Environment.DirectoryPictures))
            using (Java.IO.File file = new Java.IO.File(path, uniqueID + ".jpg"))
            using (FileStream fileOutputStream = new FileStream(file.AbsolutePath, FileMode.CreateNew))
            {
                Bitmap imageToConvert = await BitmapFactory.DecodeFileAsync(imagenFilePath);

                Bitmap.CompressFormat compressFormat = null;
                if (formatoImagen == FormatoImagen.Jpeg)
                {
                    compressFormat = Bitmap.CompressFormat.Jpeg;
                }
                else if (formatoImagen == FormatoImagen.Png)
                {
                    compressFormat = Bitmap.CompressFormat.Png;
                }

                await imageToConvert.CompressAsync(compressFormat, quality, fileOutputStream);

                await fileOutputStream.FlushAsync();
                fileOutputStream.Close();
                fileOutputStream.Dispose();

                return file.AbsolutePath;
            }
        }

        public async Task DeleteTransparencyFromAnImage(string imagenFilePath, int quality, FormatoImagen formatoImagen)
        {
            if (string.IsNullOrWhiteSpace(imagenFilePath)) throw new ArgumentNullException("File path de la imagen a convertir esta vacio!.");
            if (formatoImagen == FormatoImagen.SinTipoFormato) throw new ArgumentNullException("Formato de imagen invalido!.");

            Bitmap sourceBitmap = await BitmapFactory.DecodeFileAsync(imagenFilePath);

            int minX = sourceBitmap.Width;
            int minY = sourceBitmap.Height;
            int maxX = -1;
            int maxY = -1;
            for (int y = 0; y < sourceBitmap.Height; y++)
            {
                for (int x = 0; x < sourceBitmap.Width; x++)
                {
                    int alpha = (sourceBitmap.GetPixel(x, y) >> 24) & 255;
                    if (alpha > 0)   // pixel is not 100% transparent
                    {
                        if (x < minX)
                            minX = x;
                        if (x > maxX)
                            maxX = x;
                        if (y < minY)
                            minY = y;
                        if (y > maxY)
                            maxY = y;
                    }
                }
            }
            if ((maxX < minX) || (maxY < minY))
                return; // Bitmap is entirely transparent

            // crop bitmap to non-transparent area and return:
            Bitmap bitmapWithoutTransparency = Bitmap.CreateBitmap(sourceBitmap, minX, minY, (maxX - minX) + 1, (maxY - minY) + 1);

            using (var stream = new FileStream(imagenFilePath, FileMode.Create))
            {
                Bitmap.CompressFormat compressFormat = null;
                if (formatoImagen == FormatoImagen.Jpeg)
                {
                    compressFormat = Bitmap.CompressFormat.Jpeg;
                }
                else if (formatoImagen == FormatoImagen.Png)
                {
                    compressFormat = Bitmap.CompressFormat.Png;
                }

                await bitmapWithoutTransparency.CompressAsync(compressFormat, quality, stream);
                stream.Close();
            }
        }
    }
}