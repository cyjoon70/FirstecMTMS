#region 작성정보
/*********************************************************************/
// 단위업무명 : 전표마감일등록
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-06
// 작성내용 : 전표마감일등록
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

namespace AA.ACA003
{
    public partial class ACA003 : UIForm.FPCOMM1 
    {
        public ACA003()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACA003_Load(object sender, System.EventArgs e)
        {
            NewExec();
            SearchExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            string YYMMDD = SystemBase.Base.ServerTime("YYMMDD");
            txtCloseYYMM.Text = YYMMDD.Substring(0, 4) + YYMMDD.Substring(5, 2);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strCloseYYMM = txtCloseYYMM.Text.Replace("/", "");
                    if (Convert.ToInt32(strCloseYYMM) < 0 || Convert.ToInt16(strCloseYYMM.Substring(4, 2)) == 0 || Convert.ToInt16(strCloseYYMM.Substring(4, 2)) > 12)
                    {
                        MessageBox.Show("마감년월을 확인하세요.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Cursor = Cursors.Default;
                        return;
                    }
                    string strQuery = " usp_ACA003  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pCLOSE_YYMM = '" + strCloseYYMM + "' ";
                    strQuery += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                    strQuery += ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true))// 그리드 필수항목 체크 
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                string strCOST_CENTER = "";

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
                                case "I": strGbn = "I1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            string strSql = " usp_ACA003 '" + strGbn + "'";
                            strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pCLOSE_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "일자")].Text.Replace("-","")+ "' ";
                            strSql = strSql + ", @pDAY_OF_WEEK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요일코드")].Text + "' ";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결의전표마감여부")].Text == "True")
                            {
                                strSql = strSql + ", @pT_CLOSE_YN = 'Y' ";
                            }
                            else
                            {
                                strSql = strSql + ", @pT_CLOSE_YN = 'N' ";
                            }
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "회계전표마감여부")].Text == "True")
                            {
                                strSql = strSql + ", @pG_CLOSE_YN = 'Y' ";
                            }
                            else
                            {
                                strSql = strSql + ", @pG_CLOSE_YN = 'N' ";
                            }

                            strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
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
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strCOST_CENTER);
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

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region fpSpread1_CellClick
        private void fpSpread1_CellClick_1(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    if (fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
                    {
                        if (e.ColumnHeader == true)
                        {

                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결의전표마감여부")].Text == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결의전표마감여부기존")].Text
                                    && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "회계전표마감여부")].Text == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "회계전표마감여부기존")].Text)
                                {
                                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                                    fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Org;
                                }
                                else
                                {
                                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                                    fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Update;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
