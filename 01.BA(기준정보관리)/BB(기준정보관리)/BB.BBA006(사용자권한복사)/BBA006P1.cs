using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data.SqlClient;

namespace BB.BBA006
{
    public partial class BBA006P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        #endregion

        #region 생성자
        public BBA006P1()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드시
        private void BBA006P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "부서별 권한복사";

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

            SearchExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
            txtDeptCd.Focus();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BBA006  'S3'";
                strQuery = strQuery + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                strQuery = strQuery + ", @pREORG_ID ='" + SystemBase.Base.gstrREORG_ID + "' ";
                strQuery = strQuery + ", @pDEPT_CD ='" + txtDeptCd.Text + "' ";
                strQuery = strQuery + ", @pDEPT_NM ='" + txtDeptNm.Text + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 리턴값 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }
        public void RtnStr()
        {
            int realRow = 0;
            int intRow = fpSpread1.ActiveSheet.Rows.Count;

            if (intRow > 0)
            {
                for (int i = 0; i < intRow; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                    {
                        realRow = realRow + 1;
                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = Convert.ToString(i + 1);
                    }
                }
                if (realRow > 0)
                {
                    returnVal = new string[realRow];
                    realRow = 0;

                    for (int i = 0; i < intRow; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                        {
                            returnVal[realRow] = "";
                            for (int j = 2; j < fpSpread1.ActiveSheet.Columns.Count; j++)
                            {
                                if (returnVal[realRow].ToString() != "")
                                    returnVal[realRow] = returnVal[realRow] + "!!" + fpSpread1.Sheets[0].Cells[i, j].Text.ToString();
                                else
                                    returnVal[realRow] = fpSpread1.Sheets[0].Cells[i, j].Value.ToString();
                            }
                            realRow = realRow + 1;
                        }
                    }
                    this.DialogResult = DialogResult.OK;
                }
            }
            this.Close();
        }
        #endregion

        #region 버튼 클릭 이벤트
        //확인
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            RtnStr();
        }
        //취소
        private void btnCancel1_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = Convert.ToString(i + 1);
            }
            this.Close();
        }
        #endregion

        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }
    }
}
