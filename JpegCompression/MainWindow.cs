using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compression
{
    public partial class MainWindow : Form
    {
        Data dataObj;
        DataConvertor dataConvertor;
        DCT dctObj;
        ZigZag zz;
        Blocks block;
        Quantize q;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// This function will be called on the startup of the window. It 
        /// initializes the objects for the data, dct, zigzag, block, and 
        /// quantize class. It also turns off all the buttons that should 
        /// not be pressed right away.
        /// </remarks>
        public MainWindow()
        {
            InitializeComponent();
            dataObj = new Data(); // sets up the data object to us methods
            dctObj = new DCT();
            dataConvertor = new DataConvertor();
            zz = new ZigZag();
            block = new Blocks();
            q = new Quantize();
            convertDataToolStripMenuItem.Enabled = false;
            yCBCrToolStripMenuItem.Enabled = false;
            yToolStripMenuItem.Enabled = false;
            cbToolStripMenuItem.Enabled = false;
            crToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            calculateMotionVectorToolStripMenuItem.Enabled = false;
            clearPicturesToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Load in the files specified by the user. 
        /// </summary>
        /// <remarks>
        /// It will then turn on all buttons allowed when opening that
        /// type of file.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BMP Files|*.bmp|JPG Files|*.jpg|PNG Files|*.png|CPEG Files|*.cpeg|MCPEG Files|*.mcpeg|All Files|*.*";
            DialogResult result = openFileDialog.ShowDialog(); // I want to open this to the child window in the file
            if (result == DialogResult.OK) // checks if the result returned true
            {
                string ext = Path.GetExtension(openFileDialog.FileName); // includes the period
                if (ext == ".cpeg")
                {
                    pictureBox2.Image = null;
                    openFileCPG(openFileDialog.FileName);
                }
                else if (ext == ".mcpeg")
                {
                    pictureBox2.Image = null;
                    openFileMCPG(openFileDialog.FileName, pictureBox1);
                }
                else
                {
                    pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                    dataObj.setInitial(new Bitmap(openFileDialog.FileName)); // sets the original bitmap to the loaded
                    dataObj.gHead.setHeight((short)dataObj.getInitial().Height);
                    dataObj.gHead.setWidth((short)dataObj.getInitial().Width);
                    dataObj.gHead.setQuality(1);
                    pictureBox2.Image = null;
                    convertDataToolStripMenuItem.Enabled = true;
                    clearPicturesToolStripMenuItem.Enabled = true;
                    this.label1.Text = "Original Image";
                }
                yCBCrToolStripMenuItem.Enabled = false;
                yToolStripMenuItem.Enabled = false;
                cbToolStripMenuItem.Enabled = false;
                crToolStripMenuItem.Enabled = false;


            }
        }

        /// <summary>
        /// This function opens a file save dialog.
        /// </summary>
        /// <remarks>
        /// Calls the saveFile method.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CPEG Files|*.cpeg|MotionCPEG Files|*.mcpeg|All Files|*.*";
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string ext = Path.GetExtension(saveFileDialog.FileName); // includes the period
                if (ext == ".cpeg")
                {
                    this.saveFileCPG(saveFileDialog.FileName);
                }
                else if (ext == ".mcpeg")
                {
                    this.saveFileMCPG(saveFileDialog.FileName);
                }
            }
        }

        /// <summary>
        /// Removes the pictures from all the picture boxes
        /// </summary>
        /// <remarks>
        /// Turns off all buttons that should not be allowed.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearPicturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            pictureBox3.Image = null;
            convertDataToolStripMenuItem.Enabled = false;
            yCBCrToolStripMenuItem.Enabled = false;
            yToolStripMenuItem.Enabled = false;
            cbToolStripMenuItem.Enabled = false;
            crToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            clearPicturesToolStripMenuItem.Enabled = false;
            calculateMotionVectorToolStripMenuItem.Enabled = false;
            dataObj = new Data();
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
        }

        /// <summary>
        /// Sets final data array with Y + Cb + Cr data (in order) after being RLE'ed
        /// </summary>
        /// <remarks>
        /// This function sets the final data to be outputted to a file into
        /// a single array. This will make it easier to save the data by
        /// only calling on one array, instead of 3 each of different
        /// sizes.
        /// </remarks>
        private void setFinalData()
        {
            int fd = 0;
            dataObj.finalData = new sbyte[dataObj.yEncoded.Length + dataObj.cbEncoded.Length + dataObj.crEncoded.Length];
            for (int i = 0; i < dataObj.yEncoded.Length; i++)
            {
                dataObj.finalData[fd++] = dataObj.yEncoded[i];
            }
            for (int jj = 0; jj < dataObj.cbEncoded.Length; jj++)
            {
                dataObj.finalData[fd++] = dataObj.cbEncoded[jj];
            }
            for (int kk = 0; kk < dataObj.crEncoded.Length; kk++)
            {
                dataObj.finalData[fd++] = dataObj.crEncoded[kk];
            }
        }


        private void setFinalDiffData()
        {
            int fd = 0;
            dataObj.finalDiffData = new sbyte[dataObj.yDiffEncoded.Length + dataObj.cbDiffEncoded.Length + dataObj.crDiffEncoded.Length];
            for (int i = 0; i < dataObj.yDiffEncoded.Length; i++)
            {
                dataObj.finalDiffData[fd++] = dataObj.yDiffEncoded[i];
            }
            for (int jj = 0; jj < dataObj.cbDiffEncoded.Length; jj++)
            {
                dataObj.finalDiffData[fd++] = dataObj.cbDiffEncoded[jj];
            }
            for (int kk = 0; kk < dataObj.crDiffEncoded.Length; kk++)
            {
                dataObj.finalDiffData[fd++] = dataObj.crDiffEncoded[kk];
            }
        }

        /// <summary>
        /// Saves the MotionVector data to the finalMVData array. Saved as ints!
        /// </summary>
        private void setFinalMVData()
        {
            int fd = 0;
            dataObj.finalMVData = new MotionVector[dataObj.yMVEncoded.Length + dataObj.cbMVEncoded.Length + dataObj.crMVEncoded.Length];
            for (int i = 0; i < dataObj.yMVEncoded.Length; i++)
            {
                dataObj.finalMVData[fd++] = dataObj.yMVEncoded[i];
            }
            for (int jj = 0; jj < dataObj.cbMVEncoded.Length; jj++)
            {
                dataObj.finalMVData[fd++] = dataObj.cbMVEncoded[jj];
            }
            for (int kk = 0; kk < dataObj.crMVEncoded.Length; kk++)
            {
                dataObj.finalMVData[fd++] = dataObj.crMVEncoded[kk];
            }
        }

        /// <summary>
        /// Splits data from a single array to Y, Cb & Cr depending on the size
        /// specified in header.
        /// </summary>
        /// <remarks>
        /// This splits the data up that has been read into the program
        /// from a file. It splits the data up based on the size of the
        /// arrays read in from the header.
        /// </remarks>
        private void splitFinalData()
        {
            int fd = 0;

            for (int i = 0; i < dataObj.yEncoded.Length; i++)
            {
                dataObj.yEncoded[i] = dataObj.finalData[fd++];
            }
            for (int jj = 0; jj < dataObj.cbEncoded.Length; jj++)
            {
                dataObj.cbEncoded[jj] = dataObj.finalData[fd++];
            }
            for (int kk = 0; kk < dataObj.crEncoded.Length; kk++)
            {
                dataObj.crEncoded[kk] = dataObj.finalData[fd++];
            }
        }


        private void splitFinalDiffData()
        {
            int fd = 0;

            for (int i = 0; i < dataObj.yDiffEncoded.Length; i++)
            {
                dataObj.yDiffEncoded[i] = dataObj.finalDiffData[fd++];
            }
            for (int jj = 0; jj < dataObj.cbDiffEncoded.Length; jj++)
            {
                dataObj.cbDiffEncoded[jj] = dataObj.finalDiffData[fd++];
            }
            for (int kk = 0; kk < dataObj.crDiffEncoded.Length; kk++)
            {
                dataObj.crDiffEncoded[kk] = dataObj.finalDiffData[fd++];
            }
        }


        private void splitFinalMVData()
        {
            int fd = 0;

            for (int i = 0; i < dataObj.yMVEncoded.Length; i++)
            {
                dataObj.yMVEncoded[i] = dataObj.finalMVData[fd++];
            }
            for (int jj = 0; jj < dataObj.cbMVEncoded.Length; jj++)
            {
                dataObj.cbMVEncoded[jj] = dataObj.finalMVData[fd++];
            }
            for (int kk = 0; kk < dataObj.crMVEncoded.Length; kk++)
            {
                dataObj.crMVEncoded[kk] = dataObj.finalMVData[fd++];
            }
        }

        /// <summary>
        /// Saves the YCbCr RLE'ed data to the file name with the gHead in the
        /// data object.
        /// </summary>
        /// <remarks>
        /// Called when we want to save the file, to the specified file name.
        /// This is used after the image has been changed to YCbCr and back,
        /// so that we can have a better compression when saving the data and
        /// using RLE.
        /// </remarks>
        /// <param name="fileName">File name to the data to</param>
        public void saveFileCPG(string fileName)
        {
            if (pictureBox2.Image == null) return;
            this.Text = fileName; // sets the text of the form to the file name
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter wr = new BinaryWriter(fs);
            // setup the header information
            // height, width, ylen, cblen, crlen, quality
            writeData(wr, dataObj.gHead);
            writeData(wr, dataObj.gHead, dataObj.finalData);
            wr.Close();
            fs.Close();
        }

        /// <summary>
        /// Saves the YCbCr RLE'ed data to the file name with the gHead in the
        /// data object.
        /// </summary>
        /// <remarks>
        /// Called when we want to save the file, to the specified file name.
        /// This is used after the image has been changed to YCbCr and back,
        /// so that we can have a better compression when saving the data and
        /// using RLE.
        /// </remarks>
        /// <param name="fileName">File name to the data to</param>
        public void saveFileMCPG(string fileName)
        {
            if (pictureBox3.Image == null) return;
            this.Text = fileName;
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter wr = new BinaryWriter(fs);
            // setup the header information
            // height, width, ylen, cblen, crlen, quality
            writeData(wr, dataObj.gMHead);
            writeData(wr, dataObj.gMHead, dataObj.finalData, dataObj.finalDiffData, dataObj.finalMVData);
            wr.Close();
            fs.Close();
        }

        /// <summary>
        /// Opens the file for format CPG, which only works for only this application
        /// </summary>
        /// <remarks>
        /// Defaults to pictureBox2
        /// </remarks>
        /// <param name="fileName">File name to load data from</param>
        public void openFileCPG(string fileName)
        {
            this.Text = fileName; // sets the text of the form to the file name
            BinaryReader re = new BinaryReader(File.OpenRead(fileName));
            // setup the header information
            readData(re, dataObj.gHead);

            Padding padding = new Padding(ref dataObj);

            dataObj.finalData = new sbyte[dataObj.gHead.getYlen() + dataObj.gHead.getCblen() + dataObj.gHead.getCrlen()];
            dataObj.yEncoded = new sbyte[dataObj.gHead.getYlen()];
            dataObj.cbEncoded = new sbyte[dataObj.gHead.getCblen()];
            dataObj.crEncoded = new sbyte[dataObj.gHead.getCrlen()];
            dataObj.setyData(new byte[dataObj.paddedWidth, dataObj.paddedHeight]);
            dataObj.setCbData(new byte[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2]);
            dataObj.setCrData(new byte[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2]);
            dataObj.setdyData(new double[dataObj.paddedWidth, dataObj.paddedHeight]);
            dataObj.setdCbData(new double[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2]);
            dataObj.setdCrData(new double[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2]);
            // read the data
            readData(re, dataObj.gHead, dataObj.finalData);
            // split the data
            splitFinalData();
            // unrle the data
            dataObj.yEncoded = RunLengthEncoding.unrle(dataObj.yEncoded);
            dataObj.cbEncoded = RunLengthEncoding.unrle(dataObj.cbEncoded);
            dataObj.crEncoded = RunLengthEncoding.unrle(dataObj.crEncoded);

            sbyte[] tempY, tempCb, tempCr;
            sbyte[,] stempY, stempCb, stempCr;
            double[,] tempDY, tempDCb, tempDCr;
            int pos = 0;
            for (int y = 0; y < dataObj.paddedHeight; y += 8)
            {
                for (int x = 0; x < dataObj.paddedWidth; x += 8)
                {
                    // DCT, Quantize, ZigZag and RLE
                    // Y
                    // block
                    tempY = block.generate1DBlocks(dataObj.yEncoded, pos); // put in x, y here for cool spirals
                    // unzigzag
                    stempY = zz.unzigzag(tempY);
                    // inverse quantize
                    tempDY = q.inverseQuantizeLuma(stempY);
                    tempDY = dctObj.dinverseDCT(tempDY);
                    block.restoreDouble(dataObj.getdyData(), tempDY, x, y);
                    pos += 64;
                }
            }
            pos = 0;
            for (int y = 0; y < dataObj.paddedHeight / 2; y += 8)
            {
                for (int x = 0; x < dataObj.paddedWidth / 2; x += 8)
                {
                    // Cb
                    // block
                    tempCb = block.generate1DBlocks(dataObj.cbEncoded, pos);
                    // unzigzag
                    stempCb = zz.unzigzag(tempCb);
                    // inverse quantize
                    tempDCb = q.inverseQuantizeData(stempCb);
                    tempDCb = dctObj.dinverseDCT(tempDCb);
                    block.restoreDouble(dataObj.getdCbData(), tempDCb, x, y);

                    // Cr
                    // block
                    tempCr = block.generate1DBlocks(dataObj.crEncoded, pos);
                    // unzigzag
                    stempCr = zz.unzigzag(tempCr);
                    // inverse quantize
                    tempDCr = q.inverseQuantizeData(stempCr);
                    tempDCr = dctObj.dinverseDCT(tempDCr);
                    block.restoreDouble(dataObj.getdCrData(), tempDCr, x, y);
                    pos += 64;
                }
            }
            re.Close();
            // set pixels
            dataObj.setdCbData(DataSampling.upsample(dataObj.dCbData, ref dataObj));
            dataObj.setdCrData(DataSampling.upsample(dataObj.dCrData, ref dataObj));
            dataConvertor.sYCbCrtoRGB(ref dataObj);
            dataConvertor = new DataConvertor();
            pictureBox2.Image = dataObj.generateBitmap(dataObj.gHead);
        }

        /// <summary>
        /// Opens the file into specific picture box, in CPG format
        /// </summary>
        /// <remarks>
        /// Saves it to the specified picture box.
        /// </remarks>
        /// <param name="fileName">File name to load data from</param>
        public void openFileCPG(string fileName, PictureBox pb)
        {
            this.Text = fileName; // sets the text of the form to the file name
            BinaryReader re = new BinaryReader(File.OpenRead(fileName));
            // setup the header information
            readData(re, dataObj.gHead);

            Padding padding = new Padding(ref dataObj);

            dataObj.finalData = new sbyte[dataObj.gHead.getYlen() + dataObj.gHead.getCblen() + dataObj.gHead.getCrlen()];
            dataObj.yEncoded = new sbyte[dataObj.gHead.getYlen()];
            dataObj.cbEncoded = new sbyte[dataObj.gHead.getCblen()];
            dataObj.crEncoded = new sbyte[dataObj.gHead.getCrlen()];
            dataObj.setyData(new byte[dataObj.paddedWidth, dataObj.paddedHeight]);
            dataObj.setCbData(new byte[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2]);
            dataObj.setCrData(new byte[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2]);
            dataObj.setdyData(new double[dataObj.paddedWidth, dataObj.paddedHeight]);
            dataObj.setdCbData(new double[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2]);
            dataObj.setdCrData(new double[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2]);
            // read the data
            readData(re, dataObj.gHead, dataObj.finalData);
            // split the data
            splitFinalData();
            // unrle the data
            dataObj.yEncoded = RunLengthEncoding.unrle(dataObj.yEncoded);
            dataObj.cbEncoded = RunLengthEncoding.unrle(dataObj.cbEncoded);
            dataObj.crEncoded = RunLengthEncoding.unrle(dataObj.crEncoded);

            sbyte[] tempY, tempCb, tempCr;
            sbyte[,] stempY, stempCb, stempCr;
            double[,] tempDY, tempDCb, tempDCr;
            int pos = 0;
            for (int y = 0; y < dataObj.paddedHeight; y += 8)
            {
                for (int x = 0; x < dataObj.paddedWidth; x += 8)
                {
                    // DCT, Quantize, ZigZag and RLE
                    // Y
                    // block
                    tempY = block.generate1DBlocks(dataObj.yEncoded, pos); // put in x, y here for cool spirals
                    // unzigzag
                    stempY = zz.unzigzag(tempY);
                    // inverse quantize
                    tempDY = q.inverseQuantizeLuma(stempY);
                    tempDY = dctObj.dinverseDCT(tempDY);
                    block.restoreDouble(dataObj.getdyData(), tempDY, x, y);
                    pos += 64;
                }
            }
            pos = 0;
            for (int y = 0; y < dataObj.paddedHeight / 2; y += 8)
            {
                for (int x = 0; x < dataObj.paddedWidth / 2; x += 8)
                {
                    // Cb
                    // block
                    tempCb = block.generate1DBlocks(dataObj.cbEncoded, pos);
                    // unzigzag
                    stempCb = zz.unzigzag(tempCb);
                    // inverse quantize
                    tempDCb = q.inverseQuantizeData(stempCb);
                    tempDCb = dctObj.dinverseDCT(tempDCb);
                    block.restoreDouble(dataObj.getdCbData(), tempDCb, x, y);

                    // Cr
                    // block
                    tempCr = block.generate1DBlocks(dataObj.crEncoded, pos);
                    // unzigzag
                    stempCr = zz.unzigzag(tempCr);
                    // inverse quantize
                    tempDCr = q.inverseQuantizeData(stempCr);
                    tempDCr = dctObj.dinverseDCT(tempDCr);
                    block.restoreDouble(dataObj.getdCrData(), tempDCr, x, y);
                    pos += 64;
                }
            }
            re.Close();
            // set pixels
            dataObj.setdCbData(DataSampling.upsample(dataObj.dCbData, ref dataObj));
            dataObj.setdCrData(DataSampling.upsample(dataObj.dCrData, ref dataObj));
            dataConvertor.sYCbCrtoRGB(ref dataObj);
            dataConvertor = new DataConvertor();
            pb.Image = dataObj.generateBitmap(dataObj.gHead);
        }

        /// <summary>
        /// Method to open file format CPEG, special format which only works for this application
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="pb"></param>
        public void openFileMCPG(string fileName, PictureBox pb)
        {

            this.Text = fileName; // sets the text of the form to the file name
            BinaryReader re = new BinaryReader(File.OpenRead(fileName));
            // setup the header information
            readData(re, dataObj.gMHead);

            Padding padding = new Padding(ref dataObj, dataObj.gMHead);

            dataObj.finalData = new sbyte[dataObj.gMHead.getYlen() + dataObj.gMHead.getCblen() + dataObj.gMHead.getCrlen()];
            dataObj.finalDiffData = new sbyte[dataObj.gMHead.getDiffYlen() + dataObj.gMHead.getDiffCblen() + dataObj.gMHead.getDiffCrlen()];
            dataObj.finalMVData = new MotionVector[dataObj.gMHead.getMVYlen() + dataObj.gMHead.getMVCblen() + dataObj.gMHead.getMVCrlen()];
            for (int kek = 0; kek < dataObj.finalMVData.Length; kek++) dataObj.finalMVData[kek] = new MotionVector();

            dataObj.setyData(new byte[dataObj.paddedWidth, dataObj.paddedHeight]);
            dataObj.setCbData(new byte[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2]);
            dataObj.setCrData(new byte[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2]);
            dataObj.yEncoded = new sbyte[dataObj.gMHead.getYlen()];
            dataObj.cbEncoded = new sbyte[dataObj.gMHead.getCblen()];
            dataObj.crEncoded = new sbyte[dataObj.gMHead.getCrlen()];
            dataObj.setdyData(new double[dataObj.paddedWidth, dataObj.paddedHeight]);
            dataObj.setdCbData(new double[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2]);
            dataObj.setdCrData(new double[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2]);
            dataObj.yMVEncoded = new MotionVector[dataObj.gMHead.getMVYlen()];
            dataObj.cbMVEncoded = new MotionVector[dataObj.gMHead.getMVCblen()];
            dataObj.crMVEncoded = new MotionVector[dataObj.gMHead.getMVCrlen()];

            dataObj.yDiff = new double[(dataObj.paddedWidth / 8), (dataObj.paddedHeight / 8)];
            dataObj.cbDiff = new double[((dataObj.paddedWidth / 8) / 2), ((dataObj.paddedHeight / 8) / 2)];
            dataObj.crDiff = new double[((dataObj.paddedWidth / 8) / 2), ((dataObj.paddedHeight / 8) / 2)];
            dataObj.yDiffEncoded = new sbyte[dataObj.gMHead.getDiffYlen()];
            dataObj.cbDiffEncoded = new sbyte[dataObj.gMHead.getDiffCblen()];
            dataObj.crDiffEncoded = new sbyte[dataObj.gMHead.getDiffCrlen()];

            dataObj.yDiffBlock = new double[dataObj.paddedWidth, dataObj.paddedHeight];
            dataObj.cbDiffBlock = new double[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2];
            dataObj.crDiffBlock = new double[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2];

            readData(re, dataObj.gMHead, dataObj.finalData, dataObj.finalDiffData, dataObj.finalMVData);

            splitFinalData();
            splitFinalDiffData();
            splitFinalMVData();

            sbyte[] tempY, tempCb, tempCr;
            sbyte[,] stempY, stempCb, stempCr;
            double[,] tempDY, tempDCb, tempDCr;
            sbyte[] szztempY, szztempB, szztempR;
            MotionVector[] mvLArr = new MotionVector[(dataObj.paddedWidth / 8) * (dataObj.paddedHeight / 8)];
            MotionVector[] mvCbArr = new MotionVector[((dataObj.paddedWidth / 8) / 2) * ((dataObj.paddedHeight / 8) / 2)];
            MotionVector[] mvCrArr = new MotionVector[((dataObj.paddedWidth / 8) / 2) * ((dataObj.paddedHeight / 8) / 2)];

            dataObj.yEncoded = RunLengthEncoding.unrle(dataObj.yEncoded);
            dataObj.cbEncoded = RunLengthEncoding.unrle(dataObj.cbEncoded);
            dataObj.crEncoded = RunLengthEncoding.unrle(dataObj.crEncoded);
            dataObj.yDiffEncoded = RunLengthEncoding.unrle(dataObj.yDiffEncoded);
            dataObj.cbDiffEncoded = RunLengthEncoding.unrle(dataObj.cbDiffEncoded);
            dataObj.crDiffEncoded = RunLengthEncoding.unrle(dataObj.crDiffEncoded);

            // I-Frame setup opening
            int pos = 0;
            for (int y = 0; y < dataObj.paddedHeight; y += 8)
            {
                for (int x = 0; x < dataObj.paddedWidth; x += 8)
                {
                    // DCT, Quantize, ZigZag and RLE
                    // Y
                    // block
                    tempY = block.generate1DBlocks(dataObj.yEncoded, pos); // put in x, y here for cool spirals
                    // unzigzag
                    stempY = zz.unzigzag(tempY);
                    // inverse quantize
                    tempDY = q.inverseQuantizeLuma(stempY);
                    tempDY = dctObj.dinverseDCT(tempDY);
                    block.restoreDouble(dataObj.getdyData(), tempDY, x, y);
                    pos += 64;
                }
            }
            pos = 0;
            for (int j = 0; j < dataObj.paddedHeight / 2; j += 8)
            {
                for (int i = 0; i < dataObj.paddedWidth / 2; i += 8)
                {
                    // Cb
                    // block
                    tempCb = block.generate1DBlocks(dataObj.cbEncoded, pos);
                    // unzigzag
                    stempCb = zz.unzigzag(tempCb);
                    // inverse quantize
                    tempDCb = q.inverseQuantizeData(stempCb);
                    tempDCb = dctObj.dinverseDCT(tempDCb);
                    block.restoreDouble(dataObj.getdCbData(), tempDCb, i, j);

                    // Cr
                    // block
                    tempCr = block.generate1DBlocks(dataObj.crEncoded, pos);
                    // unzigzag
                    stempCr = zz.unzigzag(tempCr);
                    // inverse quantize
                    tempDCr = q.inverseQuantizeData(stempCr);
                    tempDCr = dctObj.dinverseDCT(tempDCr);
                    block.restoreDouble(dataObj.getdCrData(), tempDCr, i, j);
                    pos += 64;
                }
            }
            Bitmap iframe = new Bitmap(dataObj.gMHead.getWidth(), dataObj.gMHead.getHeight());
            dataObj.setdCbData(DataSampling.upsample(dataObj.dCbData, ref dataObj));
            dataObj.setdCrData(DataSampling.upsample(dataObj.dCrData, ref dataObj));
            dataConvertor.sYCbCrtoRGB(ref dataObj, dataObj.gMHead);

            iframe = dataObj.generateBitmap(dataObj.gMHead);
            pictureBox2.Image = iframe;

            pos = 0;
            for (int y = 0; y < dataObj.yDiffBlock.GetLength(1); y += 8)
            {
                for (int x = 0; x < dataObj.yDiffBlock.GetLength(0); x += 8)
                {
                    // Y
                    // block
                    szztempY = block.generate1DBlocks(dataObj.yDiffEncoded, x, y);
                    // unzigzag
                    stempY = zz.unzigzag(szztempY);
                    // inverse quantize
                    tempDY = q.inverseQuantizeLuma(stempY);
                    tempDY = dctObj.dinverseDCT(tempDY);
                    block.restoreDouble(dataObj.yDiffBlock, tempDY, x, y);

                    pos += 64;
                }
            }
            pos = 0;
            for (int j = 0; j < dataObj.cbDiffBlock.GetLength(1) / 2; j += 8)
            {
                for (int i = 0; i < dataObj.cbDiffBlock.GetLength(0) / 2; i += 8)
                {
                    // Cb
                    // block
                    szztempB = block.generate1DBlocks(dataObj.cbDiffEncoded, i, j);
                    // unzigzag
                    stempCb = zz.unzigzag(szztempB);
                    // inverse quantize
                    tempDCb = q.inverseQuantizeData(stempCb);
                    tempDCb = dctObj.dinverseDCT(tempDCb);
                    block.restoreDouble(dataObj.cbDiffBlock, tempDCb, i, j);

                    // Cr
                    // block
                    szztempR = block.generate1DBlocks(dataObj.cbDiffEncoded, i, j);
                    // unzigzag
                    stempCr = zz.unzigzag(szztempR);
                    // inverse quantize
                    tempDCr = q.inverseQuantizeData(stempCr);
                    tempDCr = dctObj.dinverseDCT(tempDCr);
                    block.restoreDouble(dataObj.crDiffBlock, tempDCr, i, j);
                    pos += 64;
                }
            }

            byte[,] PLtemp = new byte[dataObj.paddedWidth, dataObj.paddedHeight];
            byte[,] PCbtemp = new byte[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2];
            byte[,] PCrtemp = new byte[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2];

            Bitmap test = new Bitmap(dataObj.gMHead.getWidth(), dataObj.gMHead.getHeight());

            foreach (MotionVector mv in dataObj.yMVEncoded)
            {
                for (int iii = 0; iii < 8; iii++)
                {
                    for (int jjj = 0; jjj < 8; jjj++)
                    {
                        PLtemp[iii + mv.x, jjj + mv.y] = Convert.ToByte(Math.Max(0, Math.Min(255, (dataObj.dyData[iii + mv.u, jjj + mv.v] + dataObj.yDiffBlock[iii + mv.x, jjj + mv.y]))));
                    }
                }
            }

            foreach (MotionVector mv in dataObj.cbMVEncoded)
            {
                for (int iii = 0; iii < 8; iii++)
                {
                    for (int jjj = 0; jjj < 8; jjj++)
                    {
                        PCbtemp[iii + mv.x, jjj + mv.y] = Convert.ToByte(Math.Max(0, Math.Min(255, (dataObj.dCbData[iii + mv.u, jjj + mv.v] + dataObj.cbDiffBlock[iii + mv.x, jjj + mv.y]))));
                    }
                }
            }

            foreach (MotionVector mv in dataObj.crMVEncoded)
            {
                for (int iii = 0; iii < 8; iii++)
                {
                    for (int jjj = 0; jjj < 8; jjj++)
                    {
                        PCrtemp[iii + mv.x, jjj + mv.y] = Convert.ToByte(Math.Max(0, Math.Min(255, (dataObj.dCrData[iii + mv.u, jjj + mv.v] + dataObj.crDiffBlock[iii + mv.x, jjj + mv.y]))));
                    }
                }
            }

            // upsample cb & cr after to then convert to RGB

            PCbtemp = DataSampling.upsample(PCbtemp, ref dataObj);
            PCrtemp = DataSampling.upsample(PCrtemp, ref dataObj);

            dataObj.setyData(PLtemp);
            dataObj.setCbData(PCbtemp);
            dataObj.setCrData(PCrtemp);

            dataConvertor.YCbCrtoRGB(ref dataObj, dataObj.gMHead);

            test = dataObj.generateBitmap(dataObj.gMHead);

            saveToolStripMenuItem.Enabled = true;

            re.Close();
            // Set bitmap for picturebox3
            pictureBox3.Image = test;
        }

        /// <summary>
        /// Writes the header data to the file.
        /// </summary>
        /// <remarks>
        /// Writes the header data. This data comes from after we have changed
        /// the data from RGB to YCrCb.
        /// </remarks>
        /// <param name="file">File to write to.</param>
        /// <param name="header">Header information.</param>
        private void writeData(BinaryWriter file, Header header)
        {
            file.Write(header.getHeight());
            file.Write(header.getWidth());
            file.Write(header.getQuality());
            file.Write(header.getYlen());
            file.Write(header.getCblen());
            file.Write(header.getCrlen());
        }

        /// <summary>
        /// Writes the motion header data to the file.
        /// </summary>
        /// <param name="file">File to write to.</param>
        /// <param name="header">MHeader information.</param>
        private void writeData(BinaryWriter file, MVHeader header)
        {
            file.Write(header.getHeight());
            file.Write(header.getWidth());
            file.Write(header.getQuality());
            file.Write(header.getYlen());
            file.Write(header.getCblen());
            file.Write(header.getCrlen());
            file.Write(header.getDiffYlen());
            file.Write(header.getDiffCblen());
            file.Write(header.getDiffCrlen());
            file.Write(header.getMVYlen());
            file.Write(header.getMVCblen());
            file.Write(header.getMVCrlen());
        }

        /// <summary>
        /// Writes the final data to the file
        /// </summary>
        /// <remarks>
        /// Writes the final data, loaded earlier into the Data object, and
        /// writes the data as sbytes to the file after being RLE'ed.
        /// </remarks>
        /// <param name="file">File to write to.</param>
        /// <param name="header">Header information for data size.</param>
        /// <param name="data">Data to write.</param>
        private void writeData(BinaryWriter file, Header header, sbyte[] data)
        {
            int c = 0;
            for (int i = 0; i < header.getYlen(); i++)
                file.Write(data[c++]);
            for (int i = 0; i < header.getCblen(); i++)
                file.Write(data[c++]);
            for (int i = 0; i < header.getCrlen(); i++)
                file.Write(data[c++]);
        }

        /// <summary>
        /// Writes the I frame, Difference frame and motion vector data to the 
        /// file.
        /// </summary>
        /// <param name="file">File to write to.</param>
        /// <param name="header">MHeader information for the data size.</param>
        /// <param name="data">I frame data.</param>
        /// <param name="diffdata">Difference frame data.</param>
        /// <param name="mvData">Motion Vector data.</param>
        private void writeData(BinaryWriter file, MVHeader header, sbyte[] data, sbyte[] diffdata, MotionVector[] mvdata)
        {
            int c = 0; // I frame
            for (int i = 0; i < header.getYlen(); i++)
                file.Write(data[c++]);
            for (int i = 0; i < header.getCblen(); i++)
                file.Write(data[c++]);
            for (int i = 0; i < header.getCrlen(); i++)
                file.Write(data[c++]);

            c = 0; // Diff data
            for (int i = 0; i < header.getDiffYlen(); i++)
                file.Write(diffdata[c++]);
            for (int i = 0; i < header.getDiffCblen(); i++)
                file.Write(diffdata[c++]);
            for (int i = 0; i < header.getDiffCrlen(); i++)
                file.Write(diffdata[c++]);

            c = 0; // MotionVectors
            for (int i = 0; i < header.getMVYlen(); i++)
            {
                file.Write(mvdata[c].x);
                file.Write(mvdata[c].y);
                file.Write(mvdata[c].u);
                file.Write(mvdata[c++].v);
            }
            for (int i = 0; i < header.getMVCblen(); i++)
            {
                file.Write(mvdata[c].x);
                file.Write(mvdata[c].y);
                file.Write(mvdata[c].u);
                file.Write(mvdata[c++].v);
            }
            for (int i = 0; i < header.getMVCrlen(); i++)
            {
                file.Write(mvdata[c].x);
                file.Write(mvdata[c].y);
                file.Write(mvdata[c].u);
                file.Write(mvdata[c++].v);
            }
        }

        /// <summary>
        /// Reads the header data into the Data object.
        /// </summary>
        /// <remarks>
        /// Reads data into the header object frome the specifed file.
        /// Necessary to read the data in properly.
        /// </remarks>
        /// <param name="file">File to read data in from.</param>
        /// <param name="header">Header to read the data into.</param>
        private void readData(BinaryReader file, Header header)
        {
            header.setHeight(file.ReadInt16());
            header.setWidth(file.ReadInt16());
            header.setQuality(file.ReadByte());
            header.setYlen(file.ReadInt32());
            header.setCblen(file.ReadInt32());
            header.setCrlen(file.ReadInt32());
        }

        /// <summary>
        /// Reads the header data of the motion file into the Data object.
        /// </summary>
        /// <param name="file">File to read the data in from.</param>
        /// <param name="header">Header to read the data into.</param>
        private void readData(BinaryReader file, MVHeader header)
        {
            header.setHeight(file.ReadInt16());
            header.setWidth(file.ReadInt16());
            header.setQuality(file.ReadByte());
            header.setYlen(file.ReadInt32());
            header.setCblen(file.ReadInt32());
            header.setCrlen(file.ReadInt32());
            header.setDiffYlen(file.ReadInt32());
            header.setDiffCblen(file.ReadInt32());
            header.setDiffCrlen(file.ReadInt32());
            header.setMVYlen(file.ReadInt32());
            header.setMVCblen(file.ReadInt32());
            header.setMVCrlen(file.ReadInt32());
        }

        /// <summary>
        /// Reads the rest of the data into the final data array to be split.
        /// </summary>
        /// <remarks>
        /// Reads data from the file into the data array. This will read in
        /// the data based on the 3 array sizes we have saved specified in
        /// the header which we also pass in.
        /// </remarks>
        /// <param name="file">File to read data in from</param>
        /// <param name="header">Header for the array sizes</param>
        /// <param name="data">Data array to read into</param>
        private void readData(BinaryReader file, Header header, sbyte[] data)
        {

            Padding p = new Padding(ref dataObj);

            int c = 0;
            for (int i = 0; i < header.getYlen(); i++)
            {
                data[c++] = file.ReadSByte();
            }
            for (int j = 0; j < header.getCblen(); j++)
            {
                data[c++] = file.ReadSByte();
            }
            for (int k = 0; k < header.getCrlen(); k++)
            {
                data[c++] = file.ReadSByte();
            }
        }

        /// <summary>
        /// Reads the rest of the data into the data, diffdata and mvdata 
        /// arrays.
        /// </summary>
        /// <param name="file">File to read the data in from.</param>
        /// <param name="header">MHeader for the array sizes.</param>
        /// <param name="data">I frame data array.</param>
        /// <param name="diffdata">Difference frame data array.</param>
        /// <param name="mvdata">Motion Vector data array.</param>
        private void readData(BinaryReader file, MVHeader header, sbyte[] data, sbyte[] diffdata, MotionVector[] mvdata)
        {

            Padding p = new Padding(ref dataObj, header);

            // I frame data
            int c = 0;
            for (int i = 0; i < header.getYlen(); i++)
            {
                data[c++] = file.ReadSByte();
            }
            for (int j = 0; j < header.getCblen(); j++)
            {
                data[c++] = file.ReadSByte();
            }
            for (int k = 0; k < header.getCrlen(); k++)
            {
                data[c++] = file.ReadSByte();
            }
            c = 0; // Diff data
            for (int i = 0; i < header.getDiffYlen(); i++)
                diffdata[c++] = file.ReadSByte();
            for (int i = 0; i < header.getDiffCblen(); i++)
                diffdata[c++] = file.ReadSByte();
            for (int i = 0; i < header.getDiffCrlen(); i++)
                diffdata[c++] = file.ReadSByte();

            c = 0; // MotionVectors
            for (int i = 0; i < header.getMVYlen(); i++)
            {
                mvdata[c].x = file.ReadInt32();
                mvdata[c].y = file.ReadInt32();
                mvdata[c].u = file.ReadInt32();
                mvdata[c++].v = file.ReadInt32();
            }
            for (int i = 0; i < header.getMVCblen(); i++)
            {
                mvdata[c].x = file.ReadInt32();
                mvdata[c].y = file.ReadInt32();
                mvdata[c].u = file.ReadInt32();
                mvdata[c++].v = file.ReadInt32();
            }
            for (int i = 0; i < header.getMVCrlen(); i++)
            {
                mvdata[c].x = file.ReadInt32();
                mvdata[c].y = file.ReadInt32();
                mvdata[c].u = file.ReadInt32();
                mvdata[c++].v = file.ReadInt32();
            }

        }

        /// <summary>
        /// Method to hand data conversion when convertDataToolStripMenuItem is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void convertDataToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Loading box = new Loading(this, "Converting RGB to YCbCr\nPlease Wait...");
            box.Show();

            int sz = 0;
            byte[,] tempY, tempCb, tempCr;
            sbyte[,] stempY, stempCb, stempCr;
            double[,] tempDY, tempDCb, tempDCr;
            sbyte[] szztempY, szztempB, szztempR;
            /* This data needs to be saved for the header information */
            Padding padding = new Padding(ref dataObj);
            dataConvertor.RGBtoYCbCr(dataObj.getInitial(), ref dataObj); // This will set the data changed bitmap to that of the returned bitmap from the data changer
            // pad data
            dataObj.setyData(padding.padData(dataObj.getyData(), padding.padW, padding.padH, dataObj));
            dataObj.setCbData(padding.padData(dataObj.getCbData(), padding.padW, padding.padH, dataObj));
            dataObj.setCrData(padding.padData(dataObj.getCrData(), padding.padW, padding.padH, dataObj));
            // setup arrays
            dataObj.finalData = new sbyte[dataObj.paddedHeight * dataObj.paddedWidth * 3];
            dataObj.yEncoded = new sbyte[dataObj.paddedHeight * dataObj.paddedWidth];
            dataObj.cbEncoded = new sbyte[(dataObj.paddedHeight / 2) * (dataObj.paddedWidth / 2)];
            dataObj.crEncoded = new sbyte[(dataObj.paddedHeight / 2) * (dataObj.paddedWidth / 2)];
            int pos = 0;
            for (int y = 0; y < dataObj.paddedHeight; y += 8)
            {
                for (int x = 0; x < dataObj.paddedWidth; x += 8)
                {
                    sz += 64;
                    // (add 128 before)DCT, Quantize, ZigZag and RLE
                    // Y
                    tempY = block.generate2DBlocks(dataObj.getyData(), x, y);
                    tempDY = dctObj.forwardDCT(tempY);
                    // quantize
                    stempY = q.quantizeLuma(tempDY);
                    // zigzag
                    szztempY = zz.zigzag(stempY);

                    // put the data into the final array here with an offset of i+=64 for each array
                    Array.Resize<sbyte>(ref dataObj.yEncoded, sz);
                    Buffer.BlockCopy(szztempY, 0, dataObj.yEncoded, pos, 64);
                    // unzigzag
                    stempY = zz.unzigzag(szztempY);
                    // inverse quantize
                    tempDY = q.inverseQuantizeLuma(stempY);
                    tempY = dctObj.inverseDCTByte(tempDY);
                    block.restoreByte(dataObj.getyData(), tempY, x, y);
                    pos += 64;
                }
            }
            dataObj.setCbData(DataSampling.subsample(dataObj.CbData, ref dataObj));
            dataObj.setCrData(DataSampling.subsample(dataObj.CrData, ref dataObj));
            pos = 0;
            sz = 0;
            for (int j = 0; j < dataObj.paddedHeight / 2; j += 8)
            {
                for (int i = 0; i < dataObj.paddedWidth / 2; i += 8)
                {
                    sz += 64;
                    // Cb (data is subsampled)
                    tempCb = block.generate2DBlocks(dataObj.getCbData(), i, j);
                    tempDCb = dctObj.forwardDCT(tempCb);
                    // quantize
                    stempCb = q.quantizeData(tempDCb);
                    // zigzag
                    szztempB = zz.zigzag(stempCb);

                    // put the data into the final array here with an offset of i+=64 for each array
                    Array.Resize<sbyte>(ref dataObj.cbEncoded, sz);
                    Buffer.BlockCopy(szztempB, 0, dataObj.cbEncoded, pos, 64);
                    // unzigzag
                    stempCb = zz.unzigzag(szztempB);
                    // inverse quantize
                    tempDCb = q.inverseQuantizeData(stempCb);
                    tempCb = dctObj.inverseDCTByte(tempDCb);
                    block.restoreByte(dataObj.getCbData(), tempCb, i, j);

                    // Cr (data is subsampled)
                    tempCr = block.generate2DBlocks(dataObj.getCrData(), i, j);
                    tempDCr = dctObj.forwardDCT(tempCr);
                    // quantize
                    stempCr = q.quantizeData(tempDCr);
                    // zigzag
                    szztempR = zz.zigzag(stempCr);

                    // put the data into the final array here with an offset of i+=64 for each array
                    Array.Resize<sbyte>(ref dataObj.crEncoded, sz);
                    Buffer.BlockCopy(szztempR, 0, dataObj.crEncoded, pos, 64);
                    // unzigzag
                    stempCr = zz.unzigzag(szztempR);
                    // inverse quantize
                    tempDCr = q.inverseQuantizeData(stempCr);
                    tempCr = dctObj.inverseDCTByte(tempDCr);
                    block.restoreByte(dataObj.getCrData(), tempCr, i, j);
                    pos += 64;
                }
            }
            // rle the data
            dataObj.yEncoded = RunLengthEncoding.rle(dataObj.yEncoded);
            dataObj.cbEncoded = RunLengthEncoding.rle(dataObj.cbEncoded);
            dataObj.crEncoded = RunLengthEncoding.rle(dataObj.crEncoded);
            // set the header information
            dataObj.gHead.setYlen(dataObj.yEncoded.Length);
            dataObj.gHead.setCblen(dataObj.cbEncoded.Length);
            dataObj.gHead.setCrlen(dataObj.crEncoded.Length);
            // update the RGBChanger data to what we have in the dataObj
            setFinalData();
            // upsample data
            dataObj.setCbData(DataSampling.upsample(dataObj.getCbData(), ref dataObj));
            dataObj.setCrData(DataSampling.upsample(dataObj.getCrData(), ref dataObj));
            dataConvertor.YCbCrtoRGB(ref dataObj, dataObj.gHead);
            dataConvertor = new DataConvertor();
            yCBCrToolStripMenuItem.Enabled = true;
            yToolStripMenuItem.Enabled = true;
            cbToolStripMenuItem.Enabled = true;
            crToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = true;

            box.Close();

            pictureBox2.Image = dataObj.generateBitmap(dataObj.gHead);

            label2.Text = "Compressed Image";

        }

        private void yCbCrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox3.Image = dataObj.getYCbCrBitmap(dataObj.gHead);
            label3.Text = "Showing YCbCr Channel";
        }

        private void yToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox3.Image = dataObj.getYBitmap(dataObj.gHead);
            label3.Text = "Showing Y Channel";
        }

        private void cbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox3.Image = dataObj.getCbBitmap(dataObj.gHead);
            label3.Text = "Showing Cb Channel";
        }

        private void crToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox3.Image = dataObj.getCrBitmap(dataObj.gHead);
            label3.Text = "Showing Cr Channel";
        }

        /// <summary>
        /// Method to load sample project for easy demo purpose
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadSampleProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile("D:\\4932\\JpegCompression\\JpegCompression\\nomadLeft.jpg");
            pictureBox2.Image = Image.FromFile("D:\\4932\\JpegCompression\\JpegCompression\\nomadRight.jpg");
            dataObj.setInitial((Bitmap)pictureBox1.Image);
            dataObj.mv1Head.setHeight((short)pictureBox1.Image.Height);
            dataObj.mv1Head.setWidth((short)pictureBox1.Image.Width);
            dataObj.mv2Head.setHeight((short)pictureBox2.Image.Height);
            dataObj.mv2Head.setWidth((short)pictureBox2.Image.Width);
            calculateMotionVectorToolStripMenuItem.Enabled = true;
            this.label1.Text = "Motion Vector Image 1";
            this.label2.Text = "Motion Vector Image 2";
            clearPicturesToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// Method to calulate motion vectors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calculateMotionVectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[,] tempY, tempCb, tempCr;
            sbyte[,] stempY, stempCb, stempCr;
            double[,] tempDY, tempDCb, tempDCr;
            sbyte[] szztempY, szztempB, szztempR;
            // need to get YCbCr data of img 1 and 2
            // Image 1
            // data saved into the Data.yData, cbData, crData
            // Image 2
            // data saved into the Data.yData2, cbData2, crData2
            dataConvertor.RGBtoYCbCr((Bitmap)pictureBox1.Image, ref dataObj, 1);
            dataConvertor.RGBtoYCbCr((Bitmap)pictureBox2.Image, ref dataObj, 2);
            // Create bitmap for picturebox 3
            //Bitmap bmp = new Bitmap(dataObj.mv1Head.getWidth(), dataObj.mv1Head.getHeight());
            Bitmap bmp = (Bitmap)pictureBox2.Image;

            // Current is picturebox2 (C Frame) (right image)
            // Reference is picturebox1 (R Frame) (left image)
            // data is YCbCr data
            // how big are the macroblocks? Are these my 8x8 blocks? Ask Austin
            // Where does the MAD data go?
            // How do I decide on N, or p?
            Padding padding = new Padding(ref dataObj, 1);
            dataObj.yData = padding.padData(dataObj.yData, padding.padW, padding.padH, dataObj, 1);
            dataObj.yData2 = padding.padData(dataObj.yData2, padding.padW, padding.padH, dataObj, 2);
            MotionVector[] mvLArr = new MotionVector[(dataObj.paddedWidth / 8) * (dataObj.paddedHeight / 8)];
            dataObj.yDiff = new double[(dataObj.paddedWidth / 8), (dataObj.paddedHeight / 8)];
            dataObj.CbData = padding.padData(dataObj.CbData, padding.padW, padding.padH, dataObj, 1);
            dataObj.CbData2 = padding.padData(dataObj.CbData2, padding.padW, padding.padH, dataObj, 2);
            MotionVector[] mvCbArr = new MotionVector[((dataObj.paddedWidth / 8) / 2) * ((dataObj.paddedHeight / 8) / 2)];
            dataObj.cbDiff = new double[((dataObj.paddedWidth / 8) / 2), ((dataObj.paddedHeight / 8) / 2)];
            dataObj.CrData = padding.padData(dataObj.CrData, padding.padW, padding.padH, dataObj, 1);
            dataObj.CrData2 = padding.padData(dataObj.CrData2, padding.padW, padding.padH, dataObj, 2);
            MotionVector[] mvCrArr = new MotionVector[((dataObj.paddedWidth / 8) / 2) * ((dataObj.paddedHeight / 8) / 2)];
            dataObj.crDiff = new double[((dataObj.paddedWidth / 8) / 2), ((dataObj.paddedHeight / 8) / 2)];

            dataObj.CbData = DataSampling.subsample(dataObj.CbData, ref dataObj);
            dataObj.CbData2 = DataSampling.subsample(dataObj.CbData2, ref dataObj);
            dataObj.CrData = DataSampling.subsample(dataObj.CrData, ref dataObj);
            dataObj.CrData2 = DataSampling.subsample(dataObj.CrData2, ref dataObj);

            dataObj.yDiffBlock = new double[dataObj.paddedWidth, dataObj.paddedHeight];
            dataObj.cbDiffBlock = new double[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2];
            dataObj.crDiffBlock = new double[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2];

            dataObj.yDiffEncoded = new sbyte[dataObj.paddedHeight * dataObj.paddedWidth];
            dataObj.cbDiffEncoded = new sbyte[(dataObj.paddedHeight / 2) * (dataObj.paddedWidth / 2)];
            dataObj.crDiffEncoded = new sbyte[(dataObj.paddedHeight / 2) * (dataObj.paddedWidth / 2)];

            double[,] outBlock = new double[8, 8];

            int z = 0;
            int xx = 0, yy = 0;
            double minDiff = 0, minDiff2 = 0;
            for (int x = 0; x < dataObj.paddedWidth; x += 8)
            {
                for (int y = 0; y < dataObj.paddedHeight; y += 8)
                {
                    minDiff = 0;
                    // luma first
                    mvLArr[z++] = MotionCompesation.seqMVSearch(8, 8, dataObj.yData, dataObj.yData2, x, y, dataObj, ref minDiff, ref outBlock);
                    block.restoreDouble(dataObj.yDiffBlock, outBlock, x, y);
                    dataObj.yDiff[xx, yy++] = minDiff;
                }
                xx++; yy = 0;
            }
            z = 0;
            xx = 0;
            yy = 0;
            minDiff = 0;
            for (int x = 0; x < dataObj.paddedWidth / 2; x += 8)
            {
                for (int y = 0; y < dataObj.paddedHeight / 2; y += 8)
                {
                    minDiff = 0;
                    minDiff2 = 0;
                    mvCbArr[z] = MotionCompesation.chromaSeqMVSearch(8, 8, dataObj.CbData, dataObj.CbData2, x, y, dataObj, ref minDiff, ref outBlock);
                    block.restoreDouble(dataObj.cbDiffBlock, outBlock, x, y);
                    mvCrArr[z++] = MotionCompesation.chromaSeqMVSearch(8, 8, dataObj.CrData, dataObj.CrData2, x, y, dataObj, ref minDiff2, ref outBlock);
                    block.restoreDouble(dataObj.crDiffBlock, outBlock, x, y);
                    dataObj.cbDiff[xx, yy] = minDiff;
                    dataObj.crDiff[xx, yy++] = minDiff2;
                }
                xx++; yy = 0;
            }

            // draw lines where the changes are
            using (var graphics = Graphics.FromImage(bmp))
            {
                Pen redPen = new Pen(Color.Red, 1);
                // just draw the motion vector(x,y)(x1,y1) coords
                foreach (MotionVector vec in mvLArr)
                    graphics.DrawLine(redPen, vec.x + 4, vec.y + 4, vec.u + 5, vec.v + 4);
                
            }

            // I Frame
            // DCT, Quantize, ZigZag, RLE, output
            // Difference Frame
            // DCT, Quantize, ZigZag, RLE, output
            // Save with MHeader

            // Save the original size
            dataObj.gMHead.setHeight((short)pictureBox1.Image.Height);
            dataObj.gMHead.setWidth((short)pictureBox1.Image.Width);

            // I-Frame setup saving
            int pos = 0, sz = 0;
            for (int y = 0; y < dataObj.paddedHeight; y += 8)
            {
                for (int x = 0; x < dataObj.paddedWidth; x += 8)
                {
                    sz += 64;
                    // (add 128 before)DCT, Quantize, ZigZag and RLE
                    // Y
                    tempY = block.generate2DBlocks(dataObj.getyData(), x, y);
                    tempDY = dctObj.forwardDCT(tempY);
                    // quantize
                    stempY = q.quantizeLuma(tempDY);
                    // zigzag
                    szztempY = zz.zigzag(stempY);

                    // put the data into the final array here with an offset of i+=64 for each array
                    Array.Resize<sbyte>(ref dataObj.yEncoded, sz);
                    Buffer.BlockCopy(szztempY, 0, dataObj.yEncoded, pos, 64);
                    pos += 64;
                }
            }
            pos = 0;
            sz = 0;
            for (int j = 0; j < dataObj.paddedHeight / 2; j += 8)
            {
                for (int i = 0; i < dataObj.paddedWidth / 2; i += 8)
                {
                    sz += 64;
                    // Cb (data is subsampled)
                    tempCb = block.generate2DBlocks(dataObj.getCbData(), i, j);
                    tempDCb = dctObj.forwardDCT(tempCb);
                    // quantize
                    stempCb = q.quantizeData(tempDCb);
                    // zigzag
                    szztempB = zz.zigzag(stempCb);

                    // put the data into the final array here with an offset of i+=64 for each array
                    Array.Resize<sbyte>(ref dataObj.cbEncoded, sz);
                    Buffer.BlockCopy(szztempB, 0, dataObj.cbEncoded, pos, 64);

                    // Cr (data is subsampled)
                    tempCr = block.generate2DBlocks(dataObj.getCrData(), i, j);
                    tempDCr = dctObj.forwardDCT(tempCr);
                    // quantize
                    stempCr = q.quantizeData(tempDCr);
                    // zigzag
                    szztempR = zz.zigzag(stempCr);

                    // put the data into the final array here with an offset of i+=64 for each array
                    Array.Resize<sbyte>(ref dataObj.crEncoded, sz);
                    Buffer.BlockCopy(szztempR, 0, dataObj.crEncoded, pos, 64);
                    pos += 64;
                }
            }
            // rle the data
            dataObj.yEncoded = RunLengthEncoding.rle(dataObj.yEncoded);
            dataObj.cbEncoded = RunLengthEncoding.rle(dataObj.cbEncoded);
            dataObj.crEncoded = RunLengthEncoding.rle(dataObj.crEncoded);
            // set the header information
            dataObj.gMHead.setYlen(dataObj.yEncoded.Length);
            dataObj.gMHead.setCblen(dataObj.cbEncoded.Length);
            dataObj.gMHead.setCrlen(dataObj.crEncoded.Length);
            // update the RGBChanger data to what we have in the dataObj
            setFinalData(); // GOOD

            // Save the differences and the size of the array to gMHead
            // DCT, Quantize, ZigZag, RLE, output

            // DIFFERENCES NEED TO BE UPSAMPLED BEFORE WE USE THEM

            // Pad the data

            pos = 0;
            sz = 0;
            for (int y = 0; y < dataObj.yDiffBlock.GetLength(1); y += 8)
            {
                for (int x = 0; x < dataObj.yDiffBlock.GetLength(0); x += 8)
                {
                    sz += 64;
                    // (add 128 before)DCT, Quantize, ZigZag and RLE
                    // Y
                    tempDY = block.generate2DBlocks(dataObj.yDiffBlock, x, y);
                    tempDY = dctObj.forwardDCT(tempDY);
                    // quantize
                    stempY = q.quantizeLuma(tempDY);
                    // zigzag
                    szztempY = zz.zigzag(stempY);

                    // put the data into the final array here with an offset of i+=64 for each array
                    Array.Resize<sbyte>(ref dataObj.yDiffEncoded, sz);
                    Buffer.BlockCopy(szztempY, 0, dataObj.yDiffEncoded, pos, 64);

                    // unzigzag
                    stempY = zz.unzigzag(szztempY);
                    // inverse quantize
                    tempDY = q.inverseQuantizeLuma(stempY);
                    tempDY = dctObj.dinverseDCT(tempDY);
                    block.restoreDouble(dataObj.yDiffBlock, tempDY, x, y);

                    pos += 64;
                }
            }
            pos = 0;
            sz = 0;
            for (int j = 0; j < dataObj.cbDiffBlock.GetLength(1) / 2; j += 8)
            {
                for (int i = 0; i < dataObj.cbDiffBlock.GetLength(0) / 2; i += 8)
                {
                    sz += 64;
                    // Cb (data is subsampled)
                    tempDCb = block.generate2DBlocks(dataObj.cbDiffBlock, i, j);
                    tempDCb = dctObj.forwardDCT(tempDCb);
                    // quantize
                    stempCb = q.quantizeData(tempDCb);
                    // zigzag
                    szztempB = zz.zigzag(stempCb);

                    // put the data into the final array here with an offset of i+=64 for each array
                    Array.Resize<sbyte>(ref dataObj.cbDiffEncoded, sz);
                    Buffer.BlockCopy(szztempB, 0, dataObj.cbDiffEncoded, pos, 64);

                    // unzigzag
                    stempCb = zz.unzigzag(szztempB);
                    // inverse quantize
                    tempDCb = q.inverseQuantizeData(stempCb);
                    tempDCb = dctObj.dinverseDCT(tempDCb);
                    block.restoreDouble(dataObj.cbDiffBlock, tempDCb, i, j);

                    // Cr (data is subsampled)
                    tempDCr = block.generate2DBlocks(dataObj.crDiffBlock, i, j);
                    tempDCr = dctObj.forwardDCT(tempDCr);
                    // quantize
                    stempCr = q.quantizeData(tempDCr);
                    // zigzag
                    szztempR = zz.zigzag(stempCr);

                    // put the data into the final array here with an offset of i+=64 for each array
                    Array.Resize<sbyte>(ref dataObj.crDiffEncoded, sz);
                    Buffer.BlockCopy(szztempR, 0, dataObj.crDiffEncoded, pos, 64);

                    // unzigzag
                    stempCr = zz.unzigzag(szztempR);
                    // inverse quantize
                    tempDCr = q.inverseQuantizeData(stempCr);
                    tempDCr = dctObj.dinverseDCT(tempDCr);
                    block.restoreDouble(dataObj.crDiffBlock, tempDCr, i, j);

                    pos += 64;
                }
            }
            dataObj.yDiffEncoded = RunLengthEncoding.rle(dataObj.yDiffEncoded);
            dataObj.cbDiffEncoded = RunLengthEncoding.rle(dataObj.cbDiffEncoded);
            dataObj.crDiffEncoded = RunLengthEncoding.rle(dataObj.crDiffEncoded);

            setFinalDiffData(); // GOOD

            dataObj.gMHead.setDiffYlen(dataObj.yDiffEncoded.Length);
            dataObj.gMHead.setDiffCblen(dataObj.cbDiffEncoded.Length);
            dataObj.gMHead.setDiffCrlen(dataObj.crDiffEncoded.Length);


            // Save the motion vectors and the size of the array to gMHea



            //* TESTING *//

            byte[,] PLtemp = new byte[dataObj.paddedWidth, dataObj.paddedHeight];
            byte[,] PCbtemp = new byte[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2];
            byte[,] PCrtemp = new byte[dataObj.paddedWidth / 2, dataObj.paddedHeight / 2];

            Bitmap test = new Bitmap(dataObj.mv1Head.getWidth(), dataObj.mv1Head.getHeight());

            foreach (MotionVector mv in mvLArr)
            {
                for (int iii = 0; iii < 8; iii++)
                {
                    for (int jjj = 0; jjj < 8; jjj++)
                    {
                        PLtemp[iii + mv.x, jjj + mv.y] = Convert.ToByte(Math.Max(0, Math.Min(255, (dataObj.yData[iii + mv.u, jjj + mv.v] + dataObj.yDiffBlock[iii + mv.x, jjj + mv.y]))));
                    }
                }
            }

            foreach (MotionVector mv in mvCbArr)
            {
                for (int iii = 0; iii < 8; iii++)
                {
                    for (int jjj = 0; jjj < 8; jjj++)
                    {
                        PCbtemp[iii + mv.x, jjj + mv.y] = Convert.ToByte(Math.Max(0, Math.Min(255, (dataObj.CbData[iii + mv.u, jjj + mv.v] + dataObj.cbDiffBlock[iii + mv.x, jjj + mv.y]))));
                    }
                }
            }

            foreach (MotionVector mv in mvCrArr)
            {
                for (int iii = 0; iii < 8; iii++)
                {
                    for (int jjj = 0; jjj < 8; jjj++)
                    {
                        PCrtemp[iii + mv.x, jjj + mv.y] = Convert.ToByte(Math.Max(0, Math.Min(255, (dataObj.CrData[iii + mv.u, jjj + mv.v] + dataObj.crDiffBlock[iii + mv.x, jjj + mv.y]))));
                    }
                }
            }

            PCbtemp = DataSampling.upsample(PCbtemp, ref dataObj);
            PCrtemp = DataSampling.upsample(PCrtemp, ref dataObj);

            dataObj.setyData(PLtemp);
            dataObj.setCbData(PCbtemp);
            dataObj.setCrData(PCrtemp);

            // upsample cb & cr after to then convert to RGB

            dataConvertor.YCbCrtoRGB(ref dataObj, dataObj.mv1Head);

            test = dataObj.generateBitmap(dataObj.mv1Head);

            //* TESTING *//

            // Save the motion vector data

            // Not really encoded
            // SAVED AS INTS!!!

            dataObj.yMVEncoded = mvLArr;
            dataObj.cbMVEncoded = mvCbArr;
            dataObj.crMVEncoded = mvCrArr;

            dataObj.gMHead.setMVYlen(dataObj.yMVEncoded.Length);
            dataObj.gMHead.setMVCblen(dataObj.cbMVEncoded.Length);
            dataObj.gMHead.setMVCrlen(dataObj.crMVEncoded.Length);

            setFinalMVData();

            saveToolStripMenuItem.Enabled = true;

            // Set bitmap for picturebox3
            pictureBox3.Image = test;

            this.label3.Text = "Motion Vector Result";

            pictureBox2.Invalidate();

        }

        /// <summary>
        /// Method to handle loadImage1ToolStripMenuItem1_Click loads image 1 for motion vector calculation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadImage1ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPG Files|*.jpg|BMP Files|*.bmp|PNG Files|*.png|CPEG Files|*.cpeg|All Files|*.*";
            DialogResult result = openFileDialog.ShowDialog(); // I want to open this to the child window in the file
            if (result == DialogResult.OK) // checks if the result returned true
            {
                string ext = Path.GetExtension(openFileDialog.FileName); // includes the period
                if (ext == ".cpeg")
                {
                    openFileCPG(openFileDialog.FileName, pictureBox1);
                    dataObj.mv1Head.setHeight((short)pictureBox1.Image.Height);
                    dataObj.mv1Head.setWidth((short)pictureBox1.Image.Width);
                }
                else
                {
                    pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                    dataObj.mv1Head.setHeight((short)pictureBox1.Image.Height);
                    dataObj.mv1Head.setWidth((short)pictureBox1.Image.Width);
                }
                if (pictureBox1.Image != null && pictureBox2.Image != null)
                    calculateMotionVectorToolStripMenuItem.Enabled = true;
                this.label1.Text = "Motion Vector Image 1";
                clearPicturesToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// Method to handle loadImage2ToolStripMenuItem1_Click loads image 2 for motion vector calculation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadImage2ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPG Files|*.jpg|BMP Files|*.bmp|PNG Files|*.png|CPEG Files|*.cpeg|All Files|*.*";
            DialogResult result = openFileDialog.ShowDialog(); // I want to open this to the child window in the file
            if (result == DialogResult.OK) // checks if the result returned true
            {
                string ext = Path.GetExtension(openFileDialog.FileName); // includes the period
                if (ext == ".cpeg")
                {
                    openFileCPG(openFileDialog.FileName, pictureBox2);
                    dataObj.mv2Head.setHeight((short)pictureBox2.Image.Height);
                    dataObj.mv2Head.setWidth((short)pictureBox2.Image.Width);
                }
                else
                {
                    pictureBox2.Image = Image.FromFile(openFileDialog.FileName);
                    dataObj.mv2Head.setHeight((short)pictureBox2.Image.Height);
                    dataObj.mv2Head.setWidth((short)pictureBox2.Image.Width);
                }
                if (pictureBox1.Image != null && pictureBox2.Image != null)
                    calculateMotionVectorToolStripMenuItem.Enabled = true;
                this.label2.Text = "Motion Vector Image 2";
                clearPicturesToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// Message box to display project information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String message = "Jason Chan\nA00698160\nComp 4932 Assignment 2\n";
            String caption = "H261 Compression";
            MessageBoxButtons ok = MessageBoxButtons.OK;
            DialogResult about;

            about = MessageBox.Show(message, caption, ok);
        }

        /// <summary>
        /// Method to close application through menustrip item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
