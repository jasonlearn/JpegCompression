using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
    /// <summary>
    /// Quantizes the data in the Data object.
    /// </summary>
    class Quantize
    {
        /// <summary>
        /// Luminance Quantization data table
        /// </summary>
        private readonly int[,] luminance = {
            { 16, 11, 10, 16, 24, 40, 51, 61 },
            { 12, 12, 14, 19, 26, 58, 60, 55 },
            { 14, 13, 16, 24, 40, 57, 69, 56 },
            { 14, 17, 22, 29, 51, 87, 80, 62 },
            { 18, 22, 37, 56, 68, 109, 103, 77 },
            { 24, 35, 55, 64, 81, 104, 113, 92 },
            { 49, 64, 78, 87, 103, 121, 120, 101 },
            { 72, 92, 95, 98, 112, 100, 103, 99 }
        };

        /// <summary>
        /// Chroma Quantization data table
        /// </summary>
        private readonly int[,] chrominance = {
            { 17, 18, 24, 27, 47, 99, 99, 99 },
            { 18, 21, 26, 66, 99, 99, 99, 99 },
            { 24, 26, 56, 99, 99, 99, 99, 99 },
            { 47, 66, 99, 99, 99, 99, 99, 99 },
            { 99, 99, 99, 99, 99, 99, 99, 99 },
            { 99, 99, 99, 99, 99, 99, 99, 99 },
            { 99, 99, 99, 99, 99, 99, 99, 99 },
            { 99, 99, 99, 99, 99, 99, 99, 99 }
        };

        /// <summary>
        /// Quantize the chroma data.
        /// </summary>
        /// <param name="data">Chroma data to quantize</param>
        /// <returns>sbyte quantized data in 2D array</returns>
        public sbyte[,] quantizeData(double[,] data)
        {
            sbyte[,] output = new sbyte[8, 8];
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    output[x, y] = Convert.ToSByte(Math.Round(data[x, y] / chrominance[x, y]));
                }
            }
            return output;
        }

        /// <summary>
        /// Inverse quantize the chroma data.
        /// </summary>
        /// <param name="data">sbyte data to inverse quantize</param>
        /// <returns>2D array of doubles</returns>
        public double[,] inverseQuantizeData(sbyte[,] data)
        {
            double[,] output = new double[8, 8];
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    output[x, y] = (data[x, y] * chrominance[x, y]);
                }
            }
            return output;
        }

        /// <summary>
        /// Quantize the luma channel data.
        /// </summary>
        /// <param name="data">2D double array of data</param>
        /// <returns>2D array of sbytes</returns>
        public sbyte[,] quantizeLuma(double[,] data)
        {
            sbyte[,] output = new sbyte[8, 8];
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    output[x, y] = Convert.ToSByte(Math.Round((double)(data[x, y] / luminance[x, y])));
                }
            }
            return output;
        }

        /// <summary>
        /// Inverse quantize the luma data
        /// </summary>
        /// <param name="data">2D sbyte array of data</param>
        /// <returns>2D array of doubles</returns>
        public double[,] inverseQuantizeLuma(sbyte[,] data)
        {
            double[,] output = new double[8, 8];
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    output[x, y] = data[x, y] * luminance[x, y];
                }
            }
            return output;
        }
    }
}
