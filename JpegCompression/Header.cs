using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
    /// <summary>
    /// Header object to hold the header data to save to the file.
    /// </summary>
    /// <remarks>
    /// This is the class that defines the header information of the image
    /// </remarks>
    public class Header
    {
        /// <summary>
        /// Height and width of the original image. Saved as a 2 byte short.
        /// </summary>
        short height,
              width;

        /// <summary>
        /// Y, Cb and Cr data length saved to the file.
        /// </summary>
        /// <remarks>
        /// Because these are
        /// run in RLE, it is "necessary" (definately another way to read)
        /// the data into the final data object.
        /// </remarks>
        int ylen,
            cblen,
            crlen;

        /// <summary>
        /// Qulity can be set by multiplying this number into the chroma table
        /// to produce either better quality, worst compression, or the
        /// inverse.
        /// </summary>
        byte quality = 0;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Header()
        {

        }

        /// <summary>
        /// Getters for the header information.
        /// </summary>
        public short getHeight() { return this.height; }
        public short getWidth() { return this.width; }
        public int getYlen() { return this.ylen; }
        public int getCblen() { return this.cblen; }
        public int getCrlen() { return this.crlen; }
        public byte getQuality() { return this.quality; }

        /// <summary>
        /// Setters for the header information
        /// </summary>
        public void setHeight(short h) { this.height = h; }
        public void setWidth(short w) { this.width = w; }
        public void setYlen(int y) { this.ylen = y; }
        public void setCblen(int cb) { this.cblen = cb; }
        public void setCrlen(int cr) { this.crlen = cr; }
        public void setQuality(byte q) { this.quality = q; }
    }
}
