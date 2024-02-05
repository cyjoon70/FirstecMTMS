#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 결재정보조회
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-06
// 작성내용 : 결재정보조회
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

namespace WNDW
{
    public partial class WNDW004 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strAssign_NO = "";
        string strForm_id = "";
        string strUser_id = "";
        #endregion

        #region 생성자
        public WNDW004(string assign_NO, string form_id, string user_id)
        {
            InitializeComponent();

            strAssign_NO = assign_NO;
            strForm_id = form_id;
            strUser_id = user_id;
        }

        public WNDW004()
        {
            InitializeComponent();
        }
        #endregion

        #region WNW004_Load() 화면 로드
        private void WNDW004_Load(object sender, EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            string Query = "usp_B_COMMON 'COMM' ,@pLANG_CD ='" + SystemBase.Base.gstrLangCd + "',@pCODE = 'B026'";//결재방법
            G1Etc[5] = SystemBase.ComboMake.ComboOnGrid(Query);
            string Query2 = "usp_B_COMMON 'COMM' ,@pLANG_CD ='" + SystemBase.Base.gstrLangCd + "',@pCODE = 'B027'";//결재상태
            G1Etc[6] = SystemBase.ComboMake.ComboOnGrid(Query2);

            Grid_search(false);
        }
        #endregion

        #region fpButtonClick() 그리드 버튼클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "ID_2"))
                {
                    string strQuery = " usp_B_COMMON 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, 2].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "결재자 조회");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, 2].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, 4].Text = Msgs[1].ToString();
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "결재자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec(bool Msg)
        { Grid_search(true); }

        private void Grid_search(bool Msg)
        {
            string strQuery = " usp_WNDW004 @pType='S1' ";
            strQuery = strQuery + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "'";
            strQuery = strQuery + ", @pFORM_ID = '" + strForm_id + "' ";
            strQuery = strQuery + ", @pASSIGN_NO = '" + strAssign_NO + "' ";
            strQuery = strQuery + ", @pUP_ID='" + strUser_id + "'";

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0);
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
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
                            case "I": strGbn = "I2"; break;
                            default: strGbn = ""; break;
                        }

                        string strSql = "    usp_WNDW004 ";
                        strSql = strSql + "  @pType = '" + strGbn + "'";
                        strSql = strSql + ", @pFORM_ID = '" + strForm_id + "'";
                        strSql = strSql + ", @pUP_ID = '" + strUser_id + "'";
                        strSql = strSql + ", @pASSIGN_NO = '" + strAssign_NO + "'";
                        strSql = strSql + ", @pASSIGN_SEQ = '" + fpSpread1.Sheets[0].Cells[i, 1].Text.ToString() + "'";
                        strSql = strSql + ", @pASSIGN_ID = '" + fpSpread1.Sheets[0].Cells[i, 2].Text.ToString() + "'";
                        strSql = strSql + ", @pASSIGN_TYPE = '" + fpSpread1.Sheets[0].Cells[i, 5].Value.ToString() + "'";
                        strSql = strSql + ", @pETC = '" + fpSpread1.Sheets[0].Cells[i, 8].Text.ToString() + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }
                }
                Trans.Commit();
            }
            catch(Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                MSGCode = "P0001";
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                SearchExec();
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
        }
        #endregion
    }
}
