using IronSoftware.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Core.Core
{
    public class ImageServiceCore
    {
        public (byte[] Buffer, int Width, int Height) ResizeBitmap(byte[] sourceBuffer, int sourceWidth, int sourceHeight, double shrinkRatio)
        {
            if (shrinkRatio >= 1)
                throw new InvalidDataException("Shrink ratio must be less than 1");


            int targetWidth = (int)(shrinkRatio * sourceWidth);
            int targetHeight = (int)(shrinkRatio * sourceHeight);


            if ((targetWidth <= 0) || (targetHeight <= 0))
                throw new InvalidDataException("Target size too small");


            double deltaX = (double)sourceWidth / targetWidth;
            double deltaY = (double)sourceHeight / targetHeight;
            byte[] buffer = new byte[targetWidth * targetHeight * 3];


            for (int y = 0; y < targetHeight; y++)
            {
                for (int x = 0; x < targetWidth; x++)
                {
                    int targetPixel = ((targetWidth * y) + x) * 3;
                    int sourcePixel = ((int)(deltaY * y) * sourceWidth + (int)(deltaX * x)) * 3;

                    buffer[targetPixel] = sourceBuffer[sourcePixel];
                    buffer[targetPixel + 1] = sourceBuffer[sourcePixel + 1];
                    buffer[targetPixel + 2] = sourceBuffer[sourcePixel + 2];

                }
            }

            return (buffer, targetWidth, targetHeight);
        }

    }
}
