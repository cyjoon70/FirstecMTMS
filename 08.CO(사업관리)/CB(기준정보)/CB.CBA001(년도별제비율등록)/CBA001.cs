#region 작성정보
/*********************************************************************/
// 단위업무명 : 년도별제비율등록
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-08
// 작성내용 : 년도별제비율등록
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
using System.Globalization;

namespace CB.CBA001
{
    public partial class CBA001 : UIForm.FPCOMM1
    {
        public CBA001()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void CBA001_Load(object sender, System.EventArgs e)
        {
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'C005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "' ", 0); //구분

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            
            //기타 세팅		
            dtpYyyyFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToString().Substring(0,4);
            dtpYyyyTo.Text = SystemBase.Base.ServerTime("Y"); 
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            //필수체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅		
            dtpYyyyFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToShortDateString();
            dtpYyyyTo.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region 행추가
        protected override void RowInsExec()
        {
            UIForm.FPMake.RowInsert(fpSpread1);
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "포장재료비율")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사율")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "간접여유율")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "간접재료비단가")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "감가상각비단가")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "간접노무비비율")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "간접경비비율")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "일반관리비비율")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "감가상각비단가(수출)")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "간접노무비비율(수출)")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "일반관리비비율(수출)")].Value = 0;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_CBA001 @pTYPE = 'S1'";
                strQuery += ", @pYYYYFR = '" + dtpYyyyFr.Text + "'";
                strQuery += ", @pYYYYTO = '" + dtpYyyyTo.Text + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "마감년월")].Text != "" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정여부")].Text == "Y")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "포장재료비율") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사율") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "간접여유율") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "간접재료비단가") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "감가상각비단가") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "간접노무비비율") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "간접경비비율") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "일반관리비비율") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "감가상각비단가(수출)") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "간접노무비비율(수출)") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "일반관리비비율(수출)") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "구분") + "|3"
                                );
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "포장재료비율") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사율") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "간접여유율") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "간접재료비단가") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "감가상각비단가") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "간접노무비비율") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "간접경비비율") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "일반관리비비율") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "감가상각비단가(수출)") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "간접노무비비율(수출)") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "일반관리비비율(수출)") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "구분") + "|1"
                                );
                        }
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }

        #endregion

        #region SaveExec()
        protected override void SaveExec()
        {
            string fcsStr = "";

            //그리드상단 필수 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
            {
                this.Cursor = Cursors.WaitCursor;

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
                                case "I": strGbn = "I1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            fcsStr = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "년도")].Text;

                            string strQuery = " usp_CBA001 @pTYPE = '" + strGbn + "'";
                            strQuery += ", @pYYYY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "년도")].Text + "'";
                            strQuery += ", @pAPP_YMD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "적용일자")].Text + "'";
                            strQuery += ", @pPAVE_STOCK_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "포장재료비율")].Value + "'";
                            strQuery += ", @pPUBLIC_INSP_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사율")].Value + "'";
                            strQuery += ", @pINDIRECT_SUR_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "간접여유율")].Value + "'";
                            strQuery += ", @pINDIRECT_MAT_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "간접재료비단가")].Value + "'";
                            strQuery += ", @pDEPRECIATION_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "감가상각비단가")].Value + "'";
                            strQuery += ", @pINDIRECT_WAGES_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "간접노무비비율")].Value + "'";
                            strQuery += ", @pINDIRECT_COST_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "간접경비비율")].Value + "'";
                            strQuery += ", @pGENERAL_MANA_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "일반관리비비율")].Value + "'";
                            strQuery += ", @pEXP_DEPRECIATION_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "감가상각비단가(수출)")].Value + "'";
                            strQuery += ", @pEXP_INDIRECT_WAGES_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "간접노무비비율(수출)")].Value + "'";
                            strQuery += ", @pEXP_GENERAL_MANA_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "일반관리비비율(수출)")].Value + "'";
                            strQuery += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                            strQuery += ", @pRATE_CLASS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Value + "'";
                            strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; } //ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001"; //에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, fcsStr); //저장 후 그리드 포커스 이동
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

                this.Cursor = Cursors.Default;
            }
        }
        #endregion	

        #region 확정, 취소
        private void btnConfirmOk_Click(object sender, EventArgs e)
        {
            Confirm("Y"); 
        }

        private void btnConfirmCancel_Click(object sender, EventArgs e)
        {
            Confirm("N"); 
        }

        private void Confirm(string strConfirmYn)
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
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                    {
                        string strSql = " usp_CBA001  'P1'";
                        strSql += ", @pYYYY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "년도")].Text + "'";
                        strSql += ", @pAPP_YMD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "적용일자")].Text + "'";
                        strSql += ", @pRATE_CLASS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Value + "'";
                        strSql += ", @pCONFIRM_YN  = '" + strConfirmYn + "'";
                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

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
                ERRCode = "ER";
                MSGCode = f.Message;
                //MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
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
        #endregion

    }
}
