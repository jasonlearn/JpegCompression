using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
    /// <summary>
    /// Runs through an array in a zigzag pattern to read the data.
    /// </summary>
    /// <remarks>
    /// This class uses my own zigzag implementation for the class.
    /// This will run through a 8x8 2D array in a zigzag pattern, starting
    /// in the top left, and finishing in the bottom right.
    /// Took 4 hours to get working.
    /// 
    /// *Should be easy to change into any n*n block array, so long as the
    /// array is squared. Just needs to take in an extra parameter.*
    /// </remarks>
    class ZigZag
    {
        /// <summary>
        /// Runs zigzag on a 2D array and returns a 1D array.
        /// </summary>
        /// <remarks>
        /// This is the zigzag method, used to run through the array from
        /// top-left to bottom-right.
        /// </remarks>
        /// <param name="data">sbyte 2D array to zigzag through</param>
        /// <returns>A zigzagged single array</returns>
        public sbyte[] zigzag(sbyte[,] data) // or take in n for n * n array
        {
            // testing
            /*
            byte[,] data = new byte[,]
            {
                {0,1,5,6,14,15,27,28 },
                {2,4,7,13,16,26,29,42 },
                {3,8,12,17,25,30,41,43 },
                {9,11,18,24,31,40,44,53 },
                {10,19,23,32,39,45,52,54 },
                {20,22,33,38,46,51,55,60 },
                {21,34,37,47,50,56,59,61 },
                {35,36,48,49,57,58,62,63 }
            };
            */
            sbyte[] result = new sbyte[8 * 8]; // (n * n array)
            int i = 0,
                x = 0,
                y = 0,
                d = 0; // d is to start decrementing on the way back
            bool flag = false; // flag is to move over once we've hit the top side of the array
            bool reverseFlag = false; // once we get through half of the array,
                                      // we need to start going down /x as opposed to x/ side ('/' being the longest line down the middle)
                                      // draw an 5x5 array and trace from the top-right to the bottom-left if this doesn't make sense
            do
            {
                flag = false;
                if (x > 7 || reverseFlag) // start going down (7 = n - 1 for n*n array)
                {
                    d++;
                    y = d;
                    reverseFlag = true; // keep doing this the rest of the do/while
                    if (x > 7) x--; // 7 should be n - 1 for a n*n array
                    result[i] = data[x, y]; // move along the bottom of the array (right)
                    while (x != d)
                    {
                        result[++i] = data[--x, ++y]; // go diagonal upwards
                    }
                    if (++d > 7) break; // if we have gone outside of the bounds ( 7 = n - 1 )
                    x = d; // set x to be just 1 bigger
                    result[++i] = data[x, y]; // move along the right side of the array (down)
                    while (y != d)
                    {
                        result[++i] = data[++x, --y]; // go diagonal downwards
                    }
                    i++;
                }
                else // go up
                {
                    result[i] = data[x, y]; // intially all 0
                    if (i == 0) x++; // for the first time increment by 1 to go down first 
                                     // (then we go up for the very first number to grab [0,1])
                    while (x != 0)
                    {
                        result[++i] = data[--x, ++y]; // go diagonal upwards
                    }
                    while (y != 0)
                    {
                        if (flag || y == 1) x++; // if y is 1, or the flag is set we need to go down 1
                        else y += 2; // else go over 2 to then go diagonal down
                        result[++i] = data[x, --y]; // go diagonal downwards
                        if (!flag) flag = true; // go down 1
                    }
                    x++; y = 0; i++; // increment x by 1 to save the next [x+1, 0] (ie, going down 1)
                }
            } while (i < 64); // this is where it should be n*n for any n^2 array
            result[63] = data[7, 7]; // again [n*n - 1] = [n - 1, n - 1] for the last number
            return result;
        }

        /// <summary>
        /// Unzigzags the 1D array of data and returns a 2D array.
        /// </summary>
        /// <remarks>
        /// This is the un-zigzag method, used to take in a single array
        /// and change it into a sbyte 2D array, un-zigzagged!
        /// </remarks>
        /// <param name="data">1D array of zigzagged data</param>
        /// <returns>An un-zigzagged 2D array</returns>
        public sbyte[,] unzigzag(sbyte[] data)
        {
            // testing
            /*
            byte[,] data = new byte[,]
            {
                {0,1,5,6,14,15,27,28 },
                {2,4,7,13,16,26,29,42 },
                {3,8,12,17,25,30,41,43 },
                {9,11,18,24,31,40,44,53 },
                {10,19,23,32,39,45,52,54 },
                {20,22,33,38,46,51,55,60 },
                {21,34,37,47,50,56,59,61 },
                {35,36,48,49,57,58,62,63 }
            };
            */
            sbyte[,] result = new sbyte[8, 8];
            int i = 0,
                x = 0,
                y = 0,
                d = 0; // d is to start decrementing on the way back
            bool flag = false;
            bool reverseFlag = false;
            do
            {
                flag = false;
                if (x >= 8 || reverseFlag)
                {
                    d++;
                    y = d;
                    reverseFlag = true;
                    if (x >= 8)
                        x--;
                    result[x, y] = data[i];
                    while (x != d)
                    {
                        result[--x, ++y] = data[++i];
                    }
                    if (++d > 7) break;
                    x = d;
                    result[x, y] = data[++i];
                    while (y != d)
                    {
                        result[++x, --y] = data[++i];
                    }
                    i++;
                }
                else
                {
                    result[x, y] = data[i];
                    if (i == 0) x++;
                    while (x != 0)
                    {
                        result[--x, ++y] = data[++i];
                    }
                    while (y != 0)
                    {
                        if (flag || y == 1) x++;
                        else y += 2;
                        result[x, --y] = data[++i];
                        if (!flag) flag = true;
                    }
                    x++; y = 0; i++;
                }
            } while (i < 64);
            result[7, 7] = data[63];
            return result;
        }
    }
}
