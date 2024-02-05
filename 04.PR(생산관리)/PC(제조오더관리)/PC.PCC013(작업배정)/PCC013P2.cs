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
    public partial class PCC013P2 : UIForm.FPCOMM1
    {
        string strWo_No = "", strProcSeq = "";

        public PCC013P2()
        {
            InitializeComponent();
        }

        public PCC013P2(string Wo_No, string ProcSeq)
        {
            strWo_No = Wo_No;
            strProcSeq = ProcSeq;

            InitializeComponent();
        }

        #region 폼로드 이벤트
        private void PCC013P2_Load(object sender, System.EventArgs e)
        {
            this.Text = "부품내역";

            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtWorkOrderNo.Value = strWo_No;
            txtProcSeq.Value = strProcSeq;

            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            string Query = " usp_PCC013 @pTYPE = 'S4'";
            Query += ", @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "' ";
            Query += ", @pWORKORDER_NO = '" + strWo_No + "' ";
            Query += ", @pPROC_SEQ = '" + strProcSeq + "' ";
            Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

            UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
        }
        #endregion
    }
}
