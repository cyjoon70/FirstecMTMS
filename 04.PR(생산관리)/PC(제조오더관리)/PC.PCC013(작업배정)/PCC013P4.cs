using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PC.PCC013
{
    public partial class PCC013P4 : UIForm.FPCOMM1
    {
        string strWoNoRs = "";

        public PCC013P4()
        {
            InitializeComponent();
        }

        public PCC013P4(string WoNo)
        {
            strWoNoRs = WoNo;
            InitializeComponent();
        }

        #region 폼로드 이벤트
        private void PCC013P4_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtUnityOrderNo.Text = strWoNoRs;

            Search(strWoNoRs);
        }
        #endregion

        private void Search(string WoNoRs)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strMQuery = " usp_PCC013 'S6'";
                strMQuery += ", @pWORKORDER_NO_RS = '" + WoNoRs + "' ";
                strMQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
    }
}
