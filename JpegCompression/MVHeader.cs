using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
    public class MVHeader
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
            crlen,
            diffYlen,
            diffCblen,
            diffCrlen,
            MVYlen,
            MVCblen,
            MVCrlen;

        /// <summary>
        /// Qulity can be set by multiplying this number into the chroma table
        /// to produce either better quality, worst compression, or the
        /// inverse.
        /// </summary>
        byte quality = 0;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MVHeader()
        {

        }

        /// <summary>
        /// Getters for the header information.
        /// </summary>
        public short getHeight() { return this.height; }
        public short getWidth() { return this.width; }
        public int getYlen() { return this.ylen; }
        public int getCrlen() { return this.crlen; }
        public int getCblen() { return this.cblen; }
        public int getDiffYlen() { return this.diffYlen; }
        public int getDiffCrlen() { return this.diffCrlen; }
        public int getDiffCblen() { return this.diffCblen; }
        public int getMVYlen() { return this.MVYlen; }
        public int getMVCrlen() { return this.MVCrlen; }
        public int getMVCblen() { return this.MVCblen; }
        public byte getQuality() { return this.quality; }

        /// <summary>
        /// Setters for the header information
        /// </summary>
        public void setHeight(short h) { this.height = h; }
        public void setWidth(short w) { this.width = w; }
        public void setYlen(int y) { this.ylen = y; }
        public void setCrlen(int cr) { this.crlen = cr; }
        public void setCblen(int cb) { this.cblen = cb; }
        public void setDiffYlen(int y) { this.diffYlen = y; }
        public void setDiffCrlen(int cr) { this.diffCrlen = cr; }
        public void setDiffCblen(int cb) { this.diffCblen = cb; }
        public void setMVYlen(int y) { this.MVYlen = y; }
        public void setMVCrlen(int cr) { this.MVCrlen = cr; }
        public void setMVCblen(int cb) { this.MVCblen = cb; }
        public void setQuality(byte q) { this.quality = q; }
    }
}
