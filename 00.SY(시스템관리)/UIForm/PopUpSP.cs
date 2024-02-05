using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

namespace UIForm
{
    public partial class PopUpSP : Form
    {
        string returnVal = "";
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox2;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        //private System.ComponentModel.Container components = null;
        string strQuery = "";
        string[] strWhere = new string[] { };
        string[] strSearch = new string[] { };
        string[] strPHeadText = new string[] { };
        string[] strPTxtAlign = new string[] { };
        string[] strPCellType = new string[] { };
        int[] strHeadWidth = new int[] { };

        public PopUpSP(string Query, string[] Where, string[] Search, string[] PHeadText, string[] PTxtAlign, string[] PCellType, int[] PHeadWidth, int[] PSearchLabel)
        {
            InitializeComponent();

            if (PSearchLabel.Length > 0)
                lblCode.Text = PHeadText[PSearchLabel[0]].ToString();
            if (PSearchLabel.Length > 1)
                lblCodeName.Text = PHeadText[PSearchLabel[1]].ToString();

            if (Search.Length > 0)
                txtCode.Text = Search[0].ToString();
            if (Search.Length > 1)
                txtCodeName.Text = Search[1].ToString();

            strQuery = Query;
            strWhere = Where;
            strSearch = Search;
            strPHeadText = PHeadText;
            strPTxtAlign = PTxtAlign;
            strPCellType = PCellType;
            strHeadWidth = PHeadWidth;

            string Tmp = strQuery;

            if (strWhere.Length > 0 && strWhere[0].ToString().Length > 0)
                Tmp = Tmp + "," + strWhere[0].ToString() + "='" + txtCode.Text + "'";
            if (strWhere.Length > 1 && strWhere[1].ToString().Length > 0)
                Tmp = Tmp + "," + strWhere[1].ToString() + "='" + txtCodeName.Text + "'";
            if (strWhere.Length > 2 && strWhere[2].ToString().Length > 0)
                Tmp = Tmp + "," + strWhere[2].ToString() + "='" + strSearch[2].ToString() + "'";
            if (strWhere.Length > 3 && strWhere[3].ToString().Length > 0)
                Tmp = Tmp + "," + strWhere[3].ToString() + "='" + strSearch[3].ToString() + "'";
            if (strWhere.Length > 4 && strWhere[4].ToString().Length > 0)
                Tmp = Tmp + "," + strWhere[4].ToString() + "='" + strSearch[4].ToString() + "'";
            if (strWhere.Length > 5 && strWhere[5].ToString().Length > 0)
                Tmp = Tmp + "," + strWhere[5].ToString() + "='" + strSearch[5].ToString() + "'";

            FPMake.grdMakeSheet(fpSpread1, Tmp, strPHeadText, strPTxtAlign, strPCellType, strHeadWidth);
            RtnStr(0);
        }

        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }

        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            RtnStr(e.Row);
            this.DialogResult = DialogResult.OK;
            //this.Close();
        }

        private void fpSpread1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.DialogResult = DialogResult.OK;
                //this.Close();
            }
        }

        private void fpSpread1_EnterCell(object sender, FarPoint.Win.Spread.EnterCellEventArgs e)
        {
            try
            {
                RtnStr(e.Row);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show("Row를 선택하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void RtnStr(int R)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                returnVal = "";
                for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                {
                    if (returnVal.Length > 0)
                    {
                        if (fpSpread1.Sheets[0].Cells[R, i].Value == null)
                        {
                            returnVal = returnVal + "|" + "";
                        }
                        else
                        {
                            returnVal = returnVal + "|" + fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
                        }
                    }
                    else
                        returnVal = fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
                }
            }
        }

        private void Code_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string Tmp = strQuery;

                if (strWhere.Length > 0 && strWhere[0].ToString().Length > 0)
                    Tmp = Tmp + "," + strWhere[0].ToString() + "='" + txtCode.Text + "'";
                if (strWhere.Length > 1 && strWhere[1].ToString().Length > 0)
                    Tmp = Tmp + "," + strWhere[1].ToString() + "='" + txtCodeName.Text + "'";
                if (strWhere.Length > 2 && strWhere[2].ToString().Length > 0)
                    Tmp = Tmp + "," + strWhere[2].ToString() + "='" + strSearch[2].ToString() + "'";
                if (strWhere.Length > 3 && strWhere[3].ToString().Length > 0)
                    Tmp = Tmp + "," + strWhere[3].ToString() + "='" + strSearch[3].ToString() + "'";
                if (strWhere.Length > 4 && strWhere[4].ToString().Length > 0)
                    Tmp = Tmp + "," + strWhere[4].ToString() + "='" + strSearch[4].ToString() + "'";
                if (strWhere.Length > 5 && strWhere[5].ToString().Length > 0)
                    Tmp = Tmp + "," + strWhere[5].ToString() + "='" + strSearch[5].ToString() + "'";

                FPMake.grdMakeSheet(fpSpread1, Tmp, strPHeadText, strPTxtAlign, strPCellType, strHeadWidth);

                fpSpread1.Focus();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                txtCode.Text = "";
                txtCodeName.Text = "";
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            string Tmp = strQuery;

            if (strWhere.Length > 0 && strWhere[0].ToString().Length > 0)
                Tmp = Tmp + "," + strWhere[0].ToString() + "='" + txtCode.Text + "'";
            if (strWhere.Length > 1 && strWhere[1].ToString().Length > 0)
                Tmp = Tmp + "," + strWhere[1].ToString() + "='" + txtCodeName.Text + "'";
            if (strWhere.Length > 2 && strWhere[2].ToString().Length > 0)
                Tmp = Tmp + "," + strWhere[2].ToString() + "='" + strSearch[2].ToString() + "'";
            if (strWhere.Length > 3 && strWhere[3].ToString().Length > 0)
                Tmp = Tmp + "," + strWhere[3].ToString() + "='" + strSearch[3].ToString() + "'";
            if (strWhere.Length > 4 && strWhere[4].ToString().Length > 0)
                Tmp = Tmp + "," + strWhere[4].ToString() + "='" + strSearch[4].ToString() + "'";
            if (strWhere.Length > 5 && strWhere[5].ToString().Length > 0)
                Tmp = Tmp + "," + strWhere[5].ToString() + "='" + strSearch[5].ToString() + "'";

            FPMake.grdMakeSheet(fpSpread1, Tmp, strPHeadText, strPTxtAlign, strPCellType, strHeadWidth);

        }

        private void PopUpSP_Load(object sender, System.EventArgs e)
        {
            //Thread.Sleep(1000);
            fpSpread1.Focus();
        }

        private void fpSpread1_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "Down" || e.KeyCode.ToString() == "Up")
            {
                if (fpSpread1.ActiveSheet.GetSelection(0).Row.ToString() == null)
                {
                    fpSpread1.ActiveSheet.SetActiveCell(0, 0);
                }
                else
                {
                    RtnStr(fpSpread1.ActiveSheet.GetSelection(0).Row);
                }
            }
        }


    }
}
