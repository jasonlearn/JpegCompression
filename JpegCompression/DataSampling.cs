using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
    /// <summary>
    /// This class is used to up/sub sample data.
    /// </summary>
    class DataSampling
    {
        /// <summary>
        /// Upsamples the 2D byte data to the original size.
        /// </summary>
        /// <remarks>
        /// Upsamples the data to double the size of what it originally is
        /// using a 4:2:0 ideaology. (makes a 2x2 block equal to the top 
        /// right data).
        /// </remarks>
        /// <param name="org">Original 2D byte data to be upsampled</param>
        /// <param name="dataObj">Data object for the padded height and width of the image</param>
        /// <returns>2D byte array upsampled (doubled in size)</returns>
        public static byte[,] upsample(byte[,] org, ref Data dataObj)
        {
            int height = dataObj.paddedHeight,
                width = dataObj.paddedWidth;
            byte[,] output = new byte[width, height];

            for (int yy = 0, y = 0; y < height; y += 2)
            {
                for (int xx = 0, x = 0; x < width; x += 2)
                {
                    output[x, y] = org[xx, yy]; // always runs
                    if (x + 1 != width)
                    {
                        output[x + 1, y] = org[xx, yy];
                    }
                    if (y + 1 != height)
                    {
                        output[x, y + 1] = org[xx, yy];
                    }
                    if (x + 1 != width && y + 1 != height)
                    {
                        output[x + 1, y + 1] = org[xx, yy];
                    }
                    xx++;
                }
                yy++;
            }
            return output;
        }

        /// <summary>
        /// Upsamples the 2D double data to the original size.
        /// </summary>
        /// <remarks>
        /// Upsample
        /// Upsamples the data to double the size of what it originally is
        /// using a 4:2:0 idealogoy (makes a 2x2 block equal to the top
        /// right data).
        /// </remarks>
        /// <param name="org">Original 2D double array to be upsampled</param>
        /// <param name="dataObj">Data object for the padded height and width of the image</param>
        /// <returns>2D double array upsampled (doubled in size)</returns>
        public static double[,] upsample(double[,] org, ref Data dataObj)
        {
            int height = dataObj.paddedHeight,
                width = dataObj.paddedWidth;
            double[,] output = new double[width, height];

            for (int yy = 0, y = 0; y < height; y += 2)
            {
                for (int xx = 0, x = 0; x < width; x += 2)
                {
                    output[x, y] = org[xx, yy]; // always runs
                    if (x + 1 != width)
                    {
                        output[x + 1, y] = org[xx, yy];
                    }
                    if (y + 1 != height)
                    {
                        output[x, y + 1] = org[xx, yy];
                    }
                    if (x + 1 != width && y + 1 != height)
                    {
                        output[x + 1, y + 1] = org[xx, yy];
                    }
                    xx++;
                }
                yy++;
            }
            return output;
        }

        /// <summary>
        /// Upsamples the 2D double data to the original size.
        /// </summary>
        /// <remarks>
        /// Upsample
        /// Upsamples the data to double the size of what it originally is
        /// using a 4:2:0 idealogoy (makes a 2x2 block equal to the top
        /// right data).
        /// </remarks>
        /// <param name="org">Original 2D double array to be upsampled</param>
        /// <param name="dataObj">Data object for the padded height and width of the image</param>
        /// <returns>2D double array upsampled (doubled in size)</returns>
        public static MotionVector[] MVupsample(MotionVector[] org, ref Data dataObj)
        {
            int height = dataObj.paddedHeight,
                width = dataObj.paddedWidth;
            MotionVector[] output = new MotionVector[width * height];

            for (int yy = 0, y = 0; y < width * height; y += 4)
            {
                if (y + 3 != 0)
                {
                    output[y] = org[yy]; // always runs
                    output[y + 1] = org[yy];
                    output[y + 2] = org[yy];
                    output[y + 3] = org[yy];
                }
            }
            return output;
        }

        /// <summary>
        /// Subsamples the 2D byte array to be 1/2 the size.
        /// </summary>
        /// <remarks>
        /// Subsamples the data to literally 1/2 the size of what it originally
        /// was.
        /// </remarks>
        /// <param name="org">Original 2D byte data array</param>
        /// <param name="dataObj">Data object for the padded height and width of the image</param>
        /// <returns>2D byte array (1/2 the size of the original)</returns>
        public static byte[,] subsample(byte[,] org, ref Data dataObj)
        {
            int height = dataObj.paddedHeight,
                width = dataObj.paddedWidth,
                hHeight = height / 2,
                hWidth = width / 2;
            byte[,] output = new byte[hWidth, hHeight];
            for (int y = 0, yy = 0; y < height; y += 2, yy++)
            {
                for (int x = 0, xx = 0; x < width; x += 2, xx++)
                {
                    output[xx, yy] = org[x, y];
                }
            }
            return output;
        }

        /// <summary>
        /// Subsamples the 2D byte array to be 1/2 the size.
        /// </summary>
        /// <remarks>
        /// Subsamples the data to literally 1/2 the size of what it originally
        /// was.
        /// </remarks>
        /// <param name="org">Original 2D byte data array</param>
        /// <param name="dataObj">Data object for the padded height and width of the image</param>
        /// <returns>2D byte array (1/2 the size of the original)</returns>
        public static double[,] subsample(double[,] org, ref Data dataObj)
        {
            int height = dataObj.paddedHeight,
                width = dataObj.paddedWidth,
                hHeight = height / 2,
                hWidth = width / 2;
            double[,] output = new double[hWidth, hHeight];
            for (int y = 0, yy = 0; y < height; y += 2, yy++)
            {
                for (int x = 0, xx = 0; x < width; x += 2, xx++)
                {
                    output[xx, yy] = org[x, y];
                }
            }
            return output;
        }

        /// <summary>
        /// Subsamples the 2D sbyte array to be 1/2 the size.
        /// </summary>
        /// <remarks>
        /// Subsamples the data to literally 1/2 the size of what it originally
        /// was.
        /// </remarks>
        /// <param name="org">Original 2D sbyte data array</param>
        /// <param name="dataObj">Data object for the padded height and width of the image</param>
        /// <returns>2D sbyte array (1/2 the size of the orignal)</returns>
        public static sbyte[,] supsample(sbyte[,] org, ref Data dataObj)
        {
            int height = dataObj.paddedHeight,
                width = dataObj.paddedWidth;
            sbyte[,] output = new sbyte[width, height];
            for (int y = 0, yy = 0; y < height; y += 2, yy++)
            {
                for (int x = 0, xx = 0; x < width; x += 2, xx++)
                {
                    if ((y + 1) < height)
                        output[x + 1, y] = output[x + 1, y + 1] = output[x, y + 1] = output[x, y] = org[xx, yy];
                    else
                        output[x + 1, y] = org[xx, yy];
                }
            }
            return output;
        }

        /// <summary>
        /// Subsamples the 2D sbyte data to 1/2 the size.
        /// </summary>
        /// <remarks>
        /// Subsamples the data to literally 1/2 the size of what it originally
        /// was.
        /// </remarks>
        /// <param name="org">Original 2D sbyte data array</param>
        /// <param name="dataObj">Data object for the padded height and widht of the image</param>
        /// <returns>2D sbyte array (1/2 the size of the original)</returns>
        public static sbyte[,] ssubsample(sbyte[,] org, ref Data dataObj)
        {
            int height = dataObj.paddedHeight,
                width = dataObj.paddedWidth,
                hHeight = height / 2,
                hWidth = width / 2;
            sbyte[,] output = new sbyte[hWidth, hHeight];
            for (int y = 0, yy = 0; y < height; y += 2, yy++)
            {
                for (int x = 0, xx = 0; x < width; x += 2, xx++)
                {
                    output[xx, yy] = org[x, y];
                }
            }
            return output;
        }
    }
}
