using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;
using System.Data.SqlClient;

namespace IBF.IBFB03U
{
    public partial class IBFB03P : UIForm.FPCOMM1
    {
        #region 변수선언
        string returnVal = null;
        private string strDel_flag = "N";
        #endregion

        #region 생성자
        public IBFB03P()
        {
            InitializeComponent();
        }

        public IBFB03P(string strTRNo, string strITEM_CD)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            //

            txtTRNo.Value = strTRNo;
            txtITEM.Value = strITEM_CD;
            txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", strITEM_CD, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 폼로드 이벤트
        private void IBFB03P_Load(object sender, EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.Buttons.ReButton("000001010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            SearchExec();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {

                    string strQuery = " usp_IBFB03U  'S2',";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "',  ";
                    strQuery = strQuery + " @pCHILD_ITEM_CD = '" + txtITEM.Text + "',";
                    strQuery = strQuery + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
                    //					fpSpread1.Sheets[0].OperationMode =  FarPoint.Win.Spread.OperationMode.SingleSelect;

                    if (fpSpread1.Sheets[0].Rows.Count > 0) Compute_Rest_Qty();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            //			if(fpSpread1.Sheets[0].Rows.Count > 0)	Spread_Compute();
            this.Cursor = Cursors.Default;
        }
        #endregion

        private void Compute_Rest_Qty()
        {
            decimal sum_qty = 0;
            try
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (i == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value = 0;
                    else fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value = sum_qty;

                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Value) - Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value) - sum_qty;

                    sum_qty += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Value);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true)
            {
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";
                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                //								case "U": strGbn = "U1"; break;   //수정
                                case "D": strGbn = "D1"; break;   //삭제
                                //								case "I": strGbn = "I1"; break;   //입력
                                default: strGbn = ""; break;
                            }

                            string strQuery = " usp_IBFB03U '" + strGbn + "'";
                            strQuery = strQuery + ", @pTRACKING_NO = '" + txtTRNo.Text + "'";
                            strQuery = strQuery + ", @pCHILD_ITEM_CD = '" + txtITEM.Text + "'";
                            strQuery = strQuery + ", @pNOTIFY_DT= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고일자")].Text + "'";
                            strQuery = strQuery + ", @pNOTIFY_NO= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고번호")].Text + "'";
                            strQuery = strQuery + ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            strDel_flag = "Y";
                        }
                    }
                    Trans.Commit();
                    this.Cursor = Cursors.Default;
                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "P0019";
                }
            Exit:
                this.Cursor = Cursors.Default;
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                if (txtTRNo.Text.Trim() != "") SearchExec();

            }
        }
        #endregion

        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            if (strDel_flag == "Y") returnVal = "Y";
            else returnVal = "N";
            this.Close();

        }

        private void BtnClose_Click(object sender, System.EventArgs e)
        {
            btnOK_Click(sender, e);
        }

    }
}
