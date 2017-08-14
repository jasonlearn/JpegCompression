using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
    /// <summary>
    /// class Data use to hold data for project.
    /// Data are passed by reference.
    /// </summary>
    class Data
    {
        private Bitmap initial; // initial bitmap
        private Bitmap RGBToYCrCb; // RGB to YCrCb bitmap
        private Bitmap YCrCbToRGB; // YCrCb to RGB bitmap

        // public data of YCrCb in byte and double
        public byte[,] yData;
        public byte[,] CrData;
        public byte[,] CbData;

        public byte[,] yData2;
        public byte[,] CbData2;
        public byte[,] CrData2;

        public double[,] yDiff;
        public double[,] cbDiff;
        public double[,] crDiff;

        public double[,] yDiffBlock;
        public double[,] cbDiffBlock;
        public double[,] crDiffBlock;

        public double[,] dyData;
        public double[,] dCbData;
        public double[,] dCrData;

        // private Data for RGB in bytes
        private byte[,] rData;
        private byte[,] gData;
        private byte[,] bData;

        // private Data for YCrCb
        private Color[,] YCrCbData;
        private Color[,] YCrCbData2;

        private double[,] DCTData; // private DCT data in double
        private byte[,] iDCTData; // private iDCT data in bytes

        // To store initial specification of size 
        public int width;
        public int height;
        public int paddedWidth;
        public int paddedHeight;

        /// <summary>
        /// Public arrays for final data, YEncoding data, CbEncoding data,
        /// and CrEncoding data. as sbytes
        /// </summary>
        public sbyte[] finalData, finalDiffData;
        public sbyte[] yEncoded,
                       cbEncoded,
                       crEncoded,
                       yDiffEncoded,
                       cbDiffEncoded,
                       crDiffEncoded;

        public MotionVector[] finalMVData,
                              yMVEncoded,
                              cbMVEncoded,
                              crMVEncoded;

        /// <summary>
        /// Public header for the file, both for reading and saving.
        /// </summary>
        public Header gHead = new Header();

        /// <summary>
        /// Header for the motion vector image 1
        /// </summary>
        public Header mv1Head = new Header();

        /// <summary>
        /// Header for the motion vector image 2
        /// </summary>
        public Header mv2Head = new Header();

        /// <summary>
        /// Public header for the motion vector compressed file.
        /// </summary>
        public MVHeader gMHead = new MVHeader();

        /// <summary>
        /// Gets Y Bitmap
        /// </summary>
        /// <remarks>
        /// Gets the Y bitmap for the image based on the header
        /// </remarks>
        /// <param name="header">Header for the image size</param>
        /// <returns></returns>
        public Bitmap getYBitmap(Header header)
        {
            Bitmap outBmp = new Bitmap(header.getWidth(), header.getHeight());
            for (int y = 0; y < header.getHeight(); y++)
            {
                for (int x = 0; x < header.getWidth(); x++)
                {
                    //outBmp.SetPixel(x, y, Color.FromArgb(yData[x,y]/3, yData[x, y] / 3, yData[x, y] / 3));
                    outBmp.SetPixel(x, y, Color.FromArgb(yData[x, y], yData[x, y], yData[x, y]));
                }
            }

            return outBmp;
        }

        /// <summary>
        /// Gets Cr Bitmap
        /// </summary>
        /// <remarks>
        /// Gets the Cr bitmap for the image based on the header
        /// </remarks>
        /// <param name="header">Header for the image size</param>
        /// <returns></returns>
        public Bitmap getCrBitmap(Header header)
        {
            Bitmap outBmp = new Bitmap(header.getWidth(), header.getHeight());
            for (int y = 0; y < header.getHeight(); y++)
            {
                for (int x = 0; x < header.getWidth(); x++)
                {
                    //outBmp.SetPixel(x, y, Color.FromArgb(CrData[x, y] / 3, CrData[x, y] / 3, CrData[x, y] / 3));
                    outBmp.SetPixel(x, y, Color.FromArgb(CrData[x, y], CrData[x, y], CrData[x, y]));
                }
            }

            return outBmp;
        }

        /// <summary>
        /// Gets Cb Bitmap
        /// </summary>
        /// <remarks>
        /// Gets the Cb bitmap for the image based on the header
        /// </remarks>
        /// <param name="header">Header for the image</param>
        /// <returns></returns>
        public Bitmap getCbBitmap(Header header)
        {
            Bitmap outBmp = new Bitmap(header.getWidth(), header.getHeight());
            for (int y = 0; y < header.getHeight(); y++)
            {
                for (int x = 0; x < header.getWidth(); x++)
                {
                    //outBmp.SetPixel(x, y, Color.FromArgb(CbData[x, y] / 3, CbData[x, y] / 3, CbData[x, y] / 3));
                    outBmp.SetPixel(x, y, Color.FromArgb(CbData[x, y], CbData[x, y], CbData[x, y]));
                }
            }

            return outBmp;
        }

        /// <summary>
        /// Gets YCbCr Bitmap
        /// </summary>
        /// <remarks>
        /// Gets the YCbCr bitmap for the image based on the header
        /// </remarks>
        /// <param name="header">Header for the image</param>
        /// <returns></returns>
        public Bitmap getYCbCrBitmap(Header header)
        {
            Bitmap outBmp = new Bitmap(header.getWidth(), header.getHeight());
            for (int y = 0; y < header.getHeight(); y++)
            {
                for (int x = 0; x < header.getWidth(); x++)
                {
                    outBmp.SetPixel(x, y, YCrCbData[x, y]);
                }
            }

            return outBmp;
        }

        /// <summary>
        /// Generates the Bitmap for the image data
        /// </summary>
        /// <remarks>
        /// Generates the bitmap for the image
        /// </remarks>
        /// <param name="head">Header of the bitmap.</param>
        /// <returns>Bitmap</returns>
        public Bitmap generateBitmap(Header head)
        {
            Bitmap outBmp = new Bitmap(head.getWidth(), head.getHeight());
            for (int y = 0; y < head.getHeight(); y++)
            {
                for (int x = 0; x < head.getWidth(); x++)
                {
                    outBmp.SetPixel(x, y, Color.FromArgb(rData[x, y], gData[x, y], bData[x, y]));
                }
            }
            return outBmp;
        }

        /// <summary>
        /// Generates the Bitmap for the image data
        /// </summary>
        /// <remarks>
        /// Generates the bitmap for the image
        /// </remarks>
        /// <param name="head">MHeader of the bitmap.</param>
        /// <returns>Bitmap</returns>
        public Bitmap generateBitmap(MVHeader head)
        {
            Bitmap outBmp = new Bitmap(head.getWidth(), head.getHeight());
            for (int y = 0; y < head.getHeight(); y++)
            {
                for (int x = 0; x < head.getWidth(); x++)
                {
                    outBmp.SetPixel(x, y, Color.FromArgb(rData[x, y], gData[x, y], bData[x, y]));
                }
            }
            return outBmp;
        }

        /// <summary>
        /// Generates the Bitmap for the image data
        /// </summary>
        /// <remarks>
        /// Generates the bitmap for the image
        /// </remarks>
        /// <returns></returns>
        public Bitmap generateBitmap(Color[,] data)
        {
            Bitmap outBmp = new Bitmap(gHead.getWidth(), gHead.getHeight());
            for (int y = 0; y < gHead.getHeight(); y++)
            {
                for (int x = 0; x < gHead.getWidth(); x++)
                {
                    outBmp.SetPixel(x, y, data[x, y]);
                }
            }
            return outBmp;
        }

        /// <summary>
        /// Initializes the YCbCr double byte array.
        /// </summary>
        public Color[,] createYCbCrArray(Header head, byte[,] luma, byte[,] cb, byte[,] cr)
        {
            Color[,] output = new Color[head.getWidth(), head.getHeight()];
            for (int x = 0; x < head.getWidth(); x++)
            {
                for (int y = 0; y < head.getHeight(); y++)
                {
                    output[x, y] = Color.FromArgb(luma[x, y], cb[x, y], cr[x, y]);
                }
            }
            return output;
        }

        /// <summary>
        /// Getters for all the data variables
        /// </summary>
        public Bitmap getInitial() { return this.initial; }
        public Bitmap getRGBToYCrCb() { return this.RGBToYCrCb; }
        public Bitmap getYCrCbToRGB() { return this.YCrCbToRGB; }
        public double[,] getForwardDCTData() { return this.DCTData; }

        //public double[,] getInverseDCTData() { return this.inverseDCTData; }
        //testing
        public byte[,] getInverseDCTData() { return this.iDCTData; }
        public byte[,] getyData() { return this.yData; }
        public byte[,] getCbData() { return this.CbData; }
        public byte[,] getCrData() { return this.CrData; }
        public byte[,] getyData2() { return this.yData2; }
        public byte[,] getCbData2() { return this.CbData2; }
        public byte[,] getCrData2() { return this.CrData2; }
        public double[,] getdyData() { return this.dyData; }
        public double[,] getdCbData() { return this.dCbData; }
        public double[,] getdCrData() { return this.dCrData; }
        public byte[,] getrData() { return this.rData; }
        public byte[,] getgData() { return this.gData; }
        public byte[,] getbData() { return this.bData; }
        public Color[,] getYCrCbData() { return this.YCrCbData; }
        public Color[,] getYCrCbData2() { return this.YCrCbData2; }

        /// <summary>
        /// Setters for all the data variables
        /// </summary>
        public void setInitial(Bitmap bmp) { this.initial = bmp; }
        public void setRGBToYCrCb(Bitmap bmp) { this.RGBToYCrCb = bmp; }
        public void setYCrCbToRGB(Bitmap bmp) { this.YCrCbToRGB = bmp; }
        public void setDCTData(double[,] data) { this.DCTData = data; }

        //public void setInverseDCTData(double[,] data) { this.inverseDCTData = data; }
        //testing
        public void setInverseDCTData(byte[,] data) { this.iDCTData = data; }
        public void setyData(byte[,] data) { this.yData = data; }
        public void setCrData(byte[,] data) { this.CrData = data; }
        public void setCbData(byte[,] data) { this.CbData = data; }
        public void setyData2(byte[,] data) { this.yData2 = data; }
        public void setCbData2(byte[,] data) { this.CbData2 = data; }
        public void setCrData2(byte[,] data) { this.CrData2 = data; }
        public void setdyData(double[,] data) { this.dyData = data; }
        public void setdCbData(double[,] data) { this.dCbData = data; }
        public void setdCrData(double[,] data) { this.dCrData = data; }
        public void setrData(byte[,] data) { this.rData = data; }
        public void setgData(byte[,] data) { this.gData = data; }
        public void setbData(byte[,] data) { this.bData = data; }
        public void setYCrCbData(Color[,] data) { this.YCrCbData = data; }
        public void setYCrCbData2(Color[,] data) { this.YCrCbData2 = data; }
    }
}
