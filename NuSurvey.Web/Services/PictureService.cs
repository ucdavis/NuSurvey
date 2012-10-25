using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace NuSurvey.Web.Services
{
    public interface IPictureService
    {
        //byte[] Crop(byte[] img, int x, int y, int width, int height);
        byte[] Resize(byte[] img, int width, int height);
        byte[] Resize(byte[] img, int width);

        byte[] MakeThumbnail(byte[] img);
    }

    public class PictureService : IPictureService
    {
        public byte[] Resize(byte[] img, int width, int height)
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

            var ms = new MemoryStream();
            newImg.Save(ms, origImg.RawFormat);

            graphic.Dispose();
            newImg.Dispose();
            origImg.Dispose();

            return ms.GetBuffer();
        }

        public byte[] Resize(byte[] img, int width)
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

            var ms = new MemoryStream();
            newImg.Save(ms, origImg.RawFormat);

            graphic.Dispose();
            newImg.Dispose();
            origImg.Dispose();

            return ms.GetBuffer();
        }

        public byte[] MakeThumbnail(byte[] img)
        {
            return Resize(img, 80);
        }
    }
}

