using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using Microsoft.WindowsAzure;

namespace NuSurvey.Web.Services
{
    public interface IPictureService
    {
        //byte[] Crop(byte[] img, int x, int y, int width, int height);
        byte[] Resize(byte[] img, int width, int height, bool watermark = false);
        byte[] Resize(byte[] img, int width, bool watermark = false);

        byte[] MakeThumbnail(byte[] img);
        byte[] MakeDirectorThumbnail(byte[] img);

        //460x290
        byte[] MakeDisplayImage(byte[] img, bool watermark = false);

    }

    public class PictureService : IPictureService
    {
        public byte[] Resize(byte[] img, int width, int height, bool watermark = false)
        {
            var s = new MemoryStream(img);
            var origImg = Image.FromStream(s);

            var newImg = new Bitmap(width, height);

            var rectangle = new Rectangle(0, 0, width, height);

            var graphic = Graphics.FromImage(newImg);
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;


            
            graphic.DrawImage(origImg, rectangle);

            if (watermark)
            {
                // Create font and brush.
                Font drawFont = new Font("Arial", 16, FontStyle.Bold);
                int opacity = 128; // 50% opaque (0 = invisible, 255 = fully opaque)

                SolidBrush drawBrush = new SolidBrush(Color.FromArgb(opacity, Color.Black));

                // Create point for upper-left corner of drawing.
                PointF drawPoint = new PointF(0, height - 90);

                graphic.DrawString(CloudConfigurationManager.GetSetting("WatermarkText"), drawFont, drawBrush, drawPoint);
            }


            var ms = new MemoryStream();
            newImg.Save(ms, origImg.RawFormat);

            graphic.Dispose();
            newImg.Dispose();
            origImg.Dispose();

            return ms.GetBuffer();
        }

        public byte[] Resize(byte[] img, int width, bool watermark = false)
        {
            var s = new MemoryStream(img);
            var origImg = Image.FromStream(s);

            var height = (origImg.Height * width) / origImg.Width;

            var newImg = new Bitmap(width, height);

            var rectangle = new Rectangle(0, 0, width, height);

            var graphic = Graphics.FromImage(newImg);
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.DrawImage(origImg, rectangle);

            var fontSize = 16;
            var heightAdjust = 30;
            if (width < 460)
            {
                fontSize = 12;
                if (width < 300)
                {
                    fontSize = 6;
                    heightAdjust = 10;
                }
            }
            if (watermark)
            {
                // Create font and brush.
                Font drawFont = new Font("Arial", fontSize, FontStyle.Bold);
                int opacity = 128; // 50% opaque (0 = invisible, 255 = fully opaque)

                SolidBrush drawBrush = new SolidBrush(Color.FromArgb(opacity, Color.Black));

                // Create point for upper-left corner of drawing.
                PointF drawPoint = new PointF(0, height - heightAdjust);

                graphic.DrawString(CloudConfigurationManager.GetSetting("WatermarkText"), drawFont, drawBrush, drawPoint);
            }

            var ms = new MemoryStream();
            newImg.Save(ms, origImg.RawFormat);

            graphic.Dispose();
            newImg.Dispose();
            origImg.Dispose();

            return ms.GetBuffer();
        }

        public byte[] MakeThumbnail(byte[] img)
        {
            return Resize(img, 120, true);
        }

        public byte[] MakeDirectorThumbnail(byte[] img)
        {
            return Resize(img, 300, true);
        }

        /// <summary>
        /// 460 by 
        /// </summary>
        /// <param name="img"></param>
        /// <param name="watermark"> </param>
        /// <returns></returns>
        public byte[] MakeDisplayImage(byte[] img, bool watermark = false)
        {
            return Resize(img, 460, 290, watermark);
        }
    }
}

