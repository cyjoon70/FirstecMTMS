
#region 작성정보
/*********************************************************************/
// 단위업무명 : 근무제외시간등록(TOUCH용) 조회
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-08
// 작성내용 : 근무제외시간등록(TOUCH용) 및 관리
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

namespace PA.PBA118
{
    public partial class PBA118 : UIForm.FPCOMM1
    {
        public PBA118()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void PBA118_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            rdoUse2.Checked = true;

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "휴일구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P041', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//휴일구분
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strUseFlag = "";
                if (rdoUse2.Checked == true) { strUseFlag = "Y"; }
                else if (rdoUse3.Checked == true) { strUseFlag = "N"; }

                string strQuery = " usp_PBA118  'S1', @pUSE_FLAG = '" + strUseFlag + "' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            rdoUse2.Checked = true;
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            string fcsStr = "";
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            //Major 코드 필수항목 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (UIForm.FPMake.FPUpCheck(fpSpread1) == true) // 그리드 상단 필수항목 체크
                {
                    string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.

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

                                string strUseFlag = "N";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용유무")].Text == "True")
                                    strUseFlag = "Y";

                                fcsStr = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근무제외항목")].Text;

                                string strSql = " usp_PBA118 '" + strGbn + "'";
                                strSql += ", @pIDX = '" + fpSpread1.Sheets[0].Cells[i, 0].Value + "'";
                                strSql += ", @pEXCEPT_DESC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근무제외항목")].Text + "'";
                                strSql += ", @pSTART_TM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시작시간")].Text.Replace(":", "") + "'";
                                strSql += ", @pEND_TM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종료시간")].Text.Replace(":", "") + "'";
                                strSql += ", @pEXCEPT_TM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제외시수")].Value + "'";
                                strSql += ", @pSTART_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용시작일자")].Text + "'";
                                strSql += ", @pDAY_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "휴일구분")].Value + "'";
                                strSql += ", @pUSE_FLAG = '" + strUseFlag + "'";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
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
                        UIForm.FPMake.GridSetFocus(fpSpread1, fcsStr, SystemBase.Base.GridHeadIndex(GHIdx1, "근무제외항목")); //저장 후 그리드 포커스 이동
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

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 그리드 체인지 이벤트
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            string startTm = "";
            string endTm = "";

            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "시작시간"))
            {
                startTm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시작시간")].Text.Replace(":", "");
                endTm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "종료시간")].Text.Replace(":", "");

                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제외시수")].Value = SystemBase.Base.TimeCheck(startTm, endTm, 0);
            }

            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "종료시간"))
            {
                startTm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시작시간")].Text.Replace(":", "");
                endTm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "종료시간")].Text.Replace(":", "");

                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제외시수")].Value = SystemBase.Base.TimeCheck(startTm, endTm, 0);
            }
        }
        #endregion

    }
}
