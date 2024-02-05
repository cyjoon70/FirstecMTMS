#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매요청확정
// 작 성 자 : 권순철
// 작 성 일 : 2013-03-27
// 작성내용 : 구매요청확정 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using WNDW;

namespace IBF.IBFB07U
{
    public partial class IBFB07P : UIForm.FPCOMM1
    {
        #region 변수선언
        private string strDel_flag = "N";
        #endregion

        public IBFB07P()
        {
            InitializeComponent();
        }
        public IBFB07P(string strTR, string strBASED_NO)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            //

            txtTR_NO.Value = strTR;
            txtBASED_NO.Value = strBASED_NO;

        }

        #region Form Load 시
        private void IBFB07P_Load(object sender, System.EventArgs e)
        {
            //UIForm.Buttons.ReButton("001111100001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnInsert, BtnExcel, BtnPrint, BtnDelete, BtnHelp, BtnClose);
            UIForm.Buttons.ReButton("001111010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            this.Name = "반출부가정보등록";
            SearchExec();
        }
        #endregion

        protected override void RowInsExe()
        {
            int row = fpSpread1.ActiveSheet.ActiveRow.Index;
            fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량단위")].Text = "EA";
            fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "중량단위")].Text = "KG";
            fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Text = "0";
            fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "중량")].Text = "0";
        }

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_IBFB07P  'S1',";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTR_NO.Text + "',  ";
                    strQuery = strQuery + " @pUSE_CREATE_NO  = '" + txtBASED_NO.Text + "'";         
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            //Major 코드 필수항목 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))// 그리드 필수항목 체크 
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    int seq = 0;
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";
                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;
                                case "D": strGbn = "D1"; break;
                                case "I": strGbn = "I1"; break;
                                default: strGbn = ""; break;
                            }

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SEQ")].Text.ToString().Trim() == "") seq = 0;
                            else seq = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SEQ")].Value);
                            string strQuery = " usp_IBFB07P '" + strGbn + "'";
                            strQuery = strQuery + ", @pTRACKING_NO = '" + txtTR_NO.Text + "'";
                            strQuery = strQuery + ", @pUSE_CREATE_NO     = '" + txtBASED_NO.Text + "'";

                            if (strGbn == "I1") strQuery = strQuery + ", @pSEQ  = 0 ";
                            else strQuery = strQuery + ", @pSEQ  = " + seq;

                            strQuery = strQuery + ", @pPACKING_LIST = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "포장재")].Text + "'";
                            strQuery = strQuery + ", @pQTY = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value;
                            strQuery = strQuery + ", @pQTY_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량단위")].Text + "'";
                            strQuery = strQuery + ", @pWEIGHT = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "중량")].Value;
                            strQuery = strQuery + ", @pWEIGHT_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "중량단위")].Text + "'";
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
                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                    SearchExec();
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        #endregion

        #region 그리드 이벤트 로직
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {

                if (Column == 5)
                {
                    string strQuery = " Nusp_BF_Comm 'BF15' ";
                    string[] strWhere = new string[] { "@pValue" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량단위")].Text };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "단위 팝업");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량단위")].Text = Msgs[0].ToString();
                    }
                }

                if (Column == 8)
                {
                    string strQuery = " Nusp_BF_Comm 'BF05' ";
                    string[] strWhere = new string[] { "@pValue" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "중량단위")].Text };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "단위 팝업");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "중량단위")].Text = Msgs[0].ToString();
                    }
                }
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
