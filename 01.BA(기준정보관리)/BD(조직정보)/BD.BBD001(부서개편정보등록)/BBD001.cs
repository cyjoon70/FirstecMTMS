#region 작성정보
/*********************************************************************/
// 단위업무명 : 부서개편정보등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-01-30
// 작성내용 : 부서개편정보등록 및 관리
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

namespace BD.BBD001
{
    public partial class BBD001 : UIForm.FPCOMM1
    {
        #region 변수선언
        int nowRow = 0; //그리드 체크박스 이벤트 관련 Row 변수
        #endregion

        #region 생성자
        public BBD001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBD001_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //날짜 컨트롤 셋팅
            SystemBase.Validation.C1DataEdit_ReadFormat(dtpYm.Value.ToString(), "YYYYMM");
            
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0,0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false,false, 0, 0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BBD001  'S1'";
                strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
                strQuery = strQuery + ", @pREORG_NM ='" + txtDept.Text.Trim() + "' ";
                strQuery = strQuery + ", @pSREORG_YM ='" + dtpYm.Text.Trim() + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1,this.Name, "fpSpread1", true))// 그리드 상단 필수항목 체크
            {
                string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                string strReorgId = "";

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
                                case "I": strGbn = "I1"; break;
                                default: strGbn = ""; break;
                            }

                            strReorgId = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서개편ID")].Text.ToString();
                            string strReorgNm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서개편명")].Text.ToString();
                            string strReorgDt = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서개편일자")].Text.ToString();
                            string strReorgDesc = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서개편개요")].Text.ToString();
                            string strUseFlag = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "현사용구분")].Text == "True")
                                strUseFlag = "Y";
                            string strSql = " usp_BBD001 '" + strGbn + "'";
                            strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                            strSql = strSql + ", @pREORG_ID = '" + strReorgId.Trim().ToUpper() + "'";
                            strSql = strSql + ", @pREORG_NM = '" + strReorgNm.Trim() + "'";
                            strSql = strSql + ", @pREORG_DT = '" + strReorgDt.Trim() + "'";
                            strSql = strSql + ", @pREORG_DESC = '" + strReorgDesc.Trim() + "'";
                            strSql = strSql + ", @pUSE_FLAG = '" + strUseFlag.Trim() + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }

                    //사용자정보 UPDATE
                    string strSql1 = " usp_BBD001 'U2'";
                    strSql1 = strSql1 + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                    strSql1 = strSql1 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql1, dbConn, Trans);
                    ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();

                    SystemBase.Base.gstrREORG_ID = ds1.Tables[0].Rows[0][2].ToString();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "SY002"; // 에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strReorgId);
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
        }
        #endregion

        #region 체크박스 클릭시
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (nowRow == e.Row) //현재 체크박스 클릭한 row랑 같다면 Return
            {
                return;
            }

            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "현사용구분"))
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "현사용구분")].Text = "False";
                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "")
                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                }

                nowRow = e.Row; //현재row값을 전역변수에 저장
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "현사용구분")].Text = "True";
            }
        }
        #endregion

    }
}
