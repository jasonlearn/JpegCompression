using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compression
{
    public partial class Loading : Form
    {
        MainWindow parent = null;

        public Loading(MainWindow parent, String msg)
        {
            InitializeComponent();
            this.parent = parent;
        }
    }
}
