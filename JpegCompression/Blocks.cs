using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
    /// <summary>
    /// class Blocks to generate 8x8 based off offset position
    /// </summary>
    class Blocks
    {
        /// <summary>
        /// Method to generate 2D block of data in bytes
        /// </summary>
        /// <param name="data">Initial data to work off</param>
        /// <param name="offsetX">offset for X</param>
        /// <param name="offsetY">offset of Y</param>
        /// <returns>8x8 result of the generated data in bytes</returns>
        public byte[,] generate2DBlocks(byte[,] data, int offsetX, int offsetY)
        {
            byte[,] result = new byte[8, 8]; //initiate array for 8 by 8
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    result[x, y] = data[offsetX + x, offsetY + y];
                }
            }
            return result;
        }

        /// <summary>
        /// Method to generate 2D block of data in bytes
        /// </summary>
        /// <param name="data">Initial data to work off</param>
        /// <param name="offsetX">offset for X</param>
        /// <param name="offsetY">offset of Y</param>
        /// <returns>8x8 result of the generated data as a double</returns>
        public double[,] generate2DBlocks(double[,] data, int offsetX, int offsetY)
        {
            double[,] result = new double[8, 8]; //initialize array for 8 by 8 
            for(int y = 0; y < 8; y++)
            {
                for(int x = 0; x < 8; x++)
                {
                    result[x, y] = data[offsetX + x, offsetY + y];
                }
            }
            return result;
        }

        /// <summary>
        /// Method to generate 1D block of data in sbyte
        /// </summary>
        /// <param name="data">Initial data in sbyte</param>
        /// <param name="offsetX">offset of X</param>
        /// <param name="offsetY">offset of Y</param>
        /// <returns>8x8 block of data in sbyte</returns>
        public sbyte[] generate1DBlocks(sbyte[] data, int offsetX, int offsetY)
        {
            sbyte[] result = new sbyte[8 * 8]; //initiate array for 8 x 8 sbyte
            for (int i = 0; i < 64; i++)
            {
                result[i] = data[i + offsetX * offsetY];
            }
            return result;
        }

        /// <summary>
        /// Method to generate 1D block of data in sbyte
        /// </summary>
        /// <param name="data">Initial data in sbyte</param>
        /// <param name="offset">1D array offset</param>
        /// <returns></returns>
        public sbyte[] generate1DBlocks(sbyte[] data, int offset)
        {
            sbyte[] result = new sbyte[8 * 8]; //initiate array for 8x8 sbyte
            for(int i = 0; i < 64; i++)
            {
                result[i] = data[i + offset];
            }
            return result;
        }

        /// <summary>
        /// Method to restore the 8x8 data of the 2D byte array to the original position in the 2D array.
        /// </summary>
        /// <param name="dataArray">array of the original data</param>
        /// <param name="data">array of the 2D byte data</param>
        /// <param name="offsetX">position of X where data is restored</param>
        /// <param name="offsetY">position of Y where data is restored</param>
        public void restoreByte(byte[,] dataArray, byte[,] data, int offsetX, int offsetY)
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    dataArray[offsetX + x, offsetY + y] = data[x, y];
                }
            }
        }

        /// <summary>
        /// Method to restore the 8x8 data of the 2D byte array to the original position in the 2D array.
        /// </summary>
        /// <param name="dataArray">array of the original data</param>
        /// <param name="data">array of the 2D byte data</param>
        /// <param name="offsetX">position of X where data is restored</param>
        /// <param name="offsetY">position of Y where data is restored</param>
        public void restoreSbyte(byte[,] dataArray, byte[,] data, int offsetX, int offsetY)
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    dataArray[offsetX + x, offsetY + y] = data[x, y];
                }
            }
        }

        /// <summary>
        /// Method to restore the 8x8 data of the 2D double array to the original position in the 2D array.
        /// </summary>
        /// <param name="dataArray">array of the original data</param>
        /// <param name="data">array of double data</param>
        /// <param name="offsetX">position of X where data is restored</param>
        /// <param name="offsetY">position of Y where data is restored</param>
        public void restoreDouble(double[,] dataArray, double[,] data, int offsetX, int offsetY)
        {
            for (int y = 0; y < 8; y++)
            {
                {
                    for (int x = 0; x < 8; x++)
                    {
                        dataArray[offsetX + x, offsetY + y] = data[x, y];
                    }
                }
            }
        }

        /// <summary>
        /// Method to restore the 8x8 data of the 2D double array to the original position in the 2D array.
        /// </summary>
        /// <param name="dataArray">array of the original data</param>
        /// <param name="data">array of byte data</param>
        /// <param name="offsetX">position of X where data is restored</param>
        /// <param name="offsetY">position of Y where data is restored</param>
        public void restoreDouble(double[,] dataArray, byte[,] data, int offsetX, int offsetY)
        {
            for (int y = 0; y < 8; y++)
            {
                {
                    for (int x = 0; x < 8; x++)
                    {
                        dataArray[offsetX + x, offsetY + y] = data[x, y];
                    }
                }
            }
        }
    }
}
