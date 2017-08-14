using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
    /// <summary>
    /// Motion Vector class
    /// </summary>
    /// <remarks>
    /// This is the class used to calculate the motion vectors for MPEG
    /// </remarks>
    class MotionCompesation
    {
        /// <summary>
        /// Mean Absolute Difference between two macroblocks
        /// </summary>
        /// <remarks>
        /// Calculates the mean absolute difference between two macroblocks
        /// where the goal of the search is to find a vector (i, j) as the 
        /// motion vector MV = (u, v), such that MAD(i, j) is the minimum.
        /// </remarks>
        /// <param name="N">Size of the macroblock</param>
        /// <param name="p">Size of the search area (2 * p + 1)</param>
        /// <param name="C">Target (current) frame</param>
        /// <param name="R">Reference frame</param>
        /// <param name="x">Origin of the macroblock</param>
        /// <param name="y">Origin of the macroblock</param>
        /// <param name="i">Where to move in the macroblock</param>
        /// <param name="j">Where to move in the macroblock</param>
        /// <returns>The mean absolute difference as a double</returns>
        public static double MAD(int N, int p, byte[,] C, byte[,] R, int x, int y, int i, int j, ref double[,] diffBlock)
        {
            // (x, y) is the upper left corner of the macroblock
            double diff; // difference calculated
            double curDiff = 0;
            // N is the size of the MACROBLOCK
            diff = 0;
            for (int k = 0; k < N; k++)
            {
                for (int l = 0; l < N; l++)
                {
                    // C is the Target Frame, R is the Reference Frame
                    // also check if we have gone out of bounds, don't add 
                    curDiff = C[x + k, y + l] - R[i + k, j + l];
                    diffBlock[k, l] = curDiff;
                    diff += curDiff;
                }
            }
            diff = (1 / Math.Pow(N, 2)) * diff;
            return diff;
        }

        /// <summary>
        /// Sequential Motion Vector Search
        /// </summary>
        /// <remarks>
        /// This will sequentiall search the whole (2p + 1) * (2p + 1) window
        /// in the Reference frame.
        /// </remarks>
        /// <param name="N">Size of the macroblock</param>
        /// <param name="p">Size of the search area (2 * p + 1)</param>
        /// <param name="C">Target (current) frame</param>
        /// <param name="R">Reference frame</param>
        /// <param name="x">Origin of the macroblock</param>
        /// <param name="y">Origin of the macroblock</param>
        /// <param name="dataObj">Data object to get the width and height from</param>
        /// <param name="minDiff">minDifference to save for use in the outside object.</param>
        /// <returns>MotionVector with the coords of the (x,y) origin and where to (u,v) difference is</returns>
        public static MotionVector seqMVSearch(int N, int p, byte[,] C, byte[,] R, int x, int y, Data dataObj, ref double minDiff, ref double[,] outBlock)
        {
            int u = x, v = y; // Vector (x-u, y-v), set to origin point initially
            MotionVector mv;
            double[,] diffBlock = new double[N, N], bestBlock = new double[N, N];
            minDiff = MAD(N, p, C, R, x, y, x, y, ref bestBlock); // Init
            for (int i = x - p; i < x + p; i++)
            {
                if (i < 0 || i + N > dataObj.paddedWidth) continue;
                for (int j = y - p; j < y + p; j++)
                {
                    if (j < 0 || j + N > dataObj.paddedHeight) continue;
                    double curDiff = MAD(N, p, C, R, x, y, i, j, ref diffBlock);
                    if (Math.Abs(curDiff) < Math.Abs(minDiff))
                    {
                        minDiff = curDiff;
                        u = i; // get the coords for MV
                        v = j;
                        bestBlock = diffBlock;
                    }
                }
            }
            mv = new MotionVector(x, y, u, v);
            outBlock = bestBlock;
            return mv;
        }

        /// <summary>
        /// Sequential Motion Vector Search
        /// </summary>
        /// <remarks>
        /// This will sequentiall search the whole (2p + 1) * (2p + 1) window
        /// in the Reference frame.
        /// </remarks>
        /// <param name="N">Size of the macroblock.</param>
        /// <param name="p">Size of the search area (2 * p + 1).</param>
        /// <param name="C">Target (current) frame.</param>
        /// <param name="R">Reference frame.</param>
        /// <param name="x">Origin of the macroblock.</param>
        /// <param name="y">Origin of the macroblock.</param>
        /// <param name="dataObj">Data object to get the width and height from.</param>
        /// <param name="minDiff">minDifference to save for use in the outside object.</param>
        /// <returns>MotionVector with the coords of the (x,y) origin and where to (u,v) difference is</returns>
        public static MotionVector chromaSeqMVSearch(int N, int p, byte[,] C, byte[,] R, int x, int y, Data dataObj, ref double minDiff, ref double[,] outBlock)
        {
            int u = x, v = y; // Vector (x-u, y-v), set to origin point initially
            MotionVector mv;
            double[,] diffBlock = new double[N, N], bestBlock = new double[N, N];
            minDiff = MAD(N, p, C, R, x, y, x, y, ref diffBlock); // Init
            for (int i = x - p; i < x + p; i++)
            {
                if (i < 0 || i + N > dataObj.paddedWidth / 2) continue;
                for (int j = y - p; j < y + p; j++)
                {
                    if (j < 0 || j + N > dataObj.paddedHeight / 2) continue;
                    double curDiff = MAD(N, p, C, R, x, y, i, j, ref diffBlock);
                    if (Math.Abs(curDiff) < Math.Abs(minDiff))
                    {
                        minDiff = curDiff;
                        u = i; // get the coords for MV
                        v = j;
                        bestBlock = diffBlock;
                    }
                }
            }
            mv = new MotionVector(x, y, u, v);
            outBlock = bestBlock;
            return mv;
        }
    }
}
