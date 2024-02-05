using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Reflection;
using FarPoint.Win.Spread;


namespace UIForm
{
    public partial class ExcelWaiting : Form
    {
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        FarPoint.Win.Spread.FpSpread baseGrid;
        public System.Windows.Forms.ProgressBar progressBar_temp;
        public System.Windows.Forms.Label label_temp;
        public string prg_nm = "";

        public ExcelWaiting()
        {
            InitializeComponent();
            progressBar_temp = progressBar1;
            label_temp = label1;
        }

        public ExcelWaiting(string prg_title)
        {
            InitializeComponent();
            progressBar_temp = progressBar1;
            label_temp = label1;
            prg_nm = prg_title;
        }

        #region ExcelWaiting_Load
        private void ExcelWaiting_Load(object sender, System.EventArgs e)
        {
            if (prg_nm != "") this.Text = prg_nm;
        }
        #endregion
    }
}
