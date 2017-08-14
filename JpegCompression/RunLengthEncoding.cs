using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
    /// <summary>
    /// Run Length Encoding class, compresses and uncompresses data read in 
    /// from the file.
    /// </summary>
    /// <remarks>
    /// Run Length Encoding class
    /// Used to run RLE of the sbyte data passeed in, and to un rle the data
    /// afterwards.
    /// </remarks>
    class RunLengthEncoding
    {
        /// <summary>
        /// Run Length Encoding saved as {count, data}.
        /// </summary>
        /// <remarks>
        /// Data is saved as
        /// {count, data}
        /// This will run through the data and generate a count of data that
        /// is the same in a run, until it hits a number that is different.
        /// Simple function really.
        /// 
        /// * If the number hits 127, it is saved automatically and the run
        /// is started agian, as we are saving sbytes *
        /// </remarks>
        /// <param name="data">Data to be RLE'ed</param>
        /// <returns>RLE'ed data</returns>
        public static sbyte[] rle(sbyte[] data)
        {
            sbyte[] output = new sbyte[0];
            sbyte count = 1; // because we start at 1
            int pos = 0;
            sbyte s = data[0];
            for (int i = 1; i < data.Length; i++)
            {
                if (count == 127) // maximum size of sbyte
                {
                    // save to array
                    Array.Resize<sbyte>(ref output, output.Length + 2);
                    output[pos] = count;
                    output[pos + 1] = s;
                    pos += 2;
                    count = 1;
                    s = data[i];
                }
                if (s == data[i])
                {
                    count++;
                    if (i + 1 == data.Length)
                    {
                        Array.Resize<sbyte>(ref output, output.Length + 2);
                        output[pos] = count;
                        output[pos + 1] = s;
                    }
                }
                else
                {
                    Array.Resize<sbyte>(ref output, output.Length + 2);
                    output[pos] = count;
                    output[pos + 1] = s;
                    s = data[i];
                    count = 1;
                    pos += 2;
                }
            }

            return output;
        }

        /// <summary>
        /// Un-RLE's the data read in from the file.
        /// </summary>
        /// <remarks>
        /// Data is read in as
        /// {count, data}
        /// This will run through the data saved and generate an array
        /// based off this data.
        /// </remarks>
        /// <param name="data">Data that has been RLE'ed</param>
        /// <returns>Un-RLE'ed data</returns>
        public static sbyte[] unrle(sbyte[] data)
        {
            sbyte[] output = new sbyte[0];
            sbyte count = 0; // because we start at 1
            sbyte s;
            int len;
            for (int i = 0; i < data.Length; i += 2)
            {
                count = data[i];
                s = data[i + 1];
                len = output.Length;
                Array.Resize<sbyte>(ref output, output.Length + count);
                for (; len < output.Length; len++)
                {
                    output[len] = s;
                }
            }
            return output;
        }
    }
}
