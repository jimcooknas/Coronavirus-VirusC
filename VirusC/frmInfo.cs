using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirusC
{
    public partial class frmInfo : Form
    {
        public frmInfo()
        {
            InitializeComponent();
            richTextBox1.LoadFile("The_model.rtf");
        }
    }
}
