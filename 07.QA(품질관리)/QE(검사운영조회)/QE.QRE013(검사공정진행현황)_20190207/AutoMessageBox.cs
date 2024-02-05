using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QE.QRE013
{
    public partial class AutoMessageBox : Form
    {
        string msg = "";

        public AutoMessageBox()
        {
            InitializeComponent();
        }
        public AutoMessageBox(string Message)
        {
            msg = Message;
            InitializeComponent();
        }

        private void AutoMessageBox_Load(object sender, EventArgs e)
        {
            label1.Text = msg;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            this.Close();
        }
    }
}
