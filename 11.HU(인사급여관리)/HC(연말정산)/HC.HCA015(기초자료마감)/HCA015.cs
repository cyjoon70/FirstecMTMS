#region 작성정보
/*********************************************************************/
// 단위업무명 :기초자료마감
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-26
// 작성내용 : 기초자료마감
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
using WNDW;
namespace HC.HCA015
{
    public partial class HCA015 : UIForm.FPCOMM1
    {
        #region 생성자
        public HCA015()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void HCA015_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpYear.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).Year.ToString();			
           
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpYear.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).Year.ToString();

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            { 
                //조회조건 필수 체크
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
                {
                    string strQuery = " usp_HCA015  @pTYPE = 'S1', @pYY = '" + dtpYear.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true); 
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
            }
           
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }       
        #endregion

        #region Save
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
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

                            string strConfirmFlag = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "마감")].Text == "True")
                            {
                                strConfirmFlag = "Y";
                            }

                            string strSql = " usp_HCA015 @pTYPE = '" + strGbn + "' ";
                            strSql = strSql + ", @pYY = '" + dtpYear.Text + "'";
                            strSql = strSql + ", @pEMP_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text + "'";
                            strSql = strSql + ", @pUP_ID  = '" + SystemBase.Base.gstrUserID.ToString() + "'";
                            strSql = strSql + ", @pCONFIRM_FLAG = '" + strConfirmFlag + "' ";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
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

        #region 체크박스 헤드 클릭시 Row Update상태 변경
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
                {
                    if (e.ColumnHeader == true)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            UIForm.FPMake.fpChange(fpSpread1, i);
                        }
                    }
                }

            }
        }
        #endregion
    }
}
