#region 작성정보
/*********************************************************************/
// 단위업무명 : 환율정보등록
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-03-22
// 작성내용 : 환율정보등록 및 관리
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

namespace BZ.BZB003
{
    public partial class BZB003 : UIForm.FPCOMM1
    {
        #region 생성자
        public BZB003()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BZB003_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboRate, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z003', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "기준화폐")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "변환화폐")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "계산구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z004', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0,0);

            dtpDate.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
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
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BZB003  'S1'";
                strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
                strQuery = strQuery + ", @pBAS_DT ='" + dtpDate.Text.ToString() + "' ";
                strQuery = strQuery + ", @pFR_CURR ='" + cboRate.SelectedValue.ToString() + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0); 
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1,this.Name, "fpSpread1", true))// 그리드 상단 필수항목 체크
            {
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                string strBasDate = "";

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

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기준일")].Value.ToString() != "") strBasDate = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기준일")].Value.ToString();
                            string strFrCurr = ""; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기준화폐")].Value.ToString() != "") strFrCurr = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기준화폐")].Value.ToString();
                            string strToCurr = ""; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변환화폐")].Value.ToString() != "") strToCurr = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변환화폐")].Value.ToString();
                            string strAccType = ""; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계산구분")].Value.ToString() != "") strAccType = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계산구분")].Value.ToString();

                            double dblBasRate = Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기준환율")].Text.ToString());
                            double dblBuyRate = 0; if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전신환매입률")].Text != "") Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전신환매입률")].Text.ToString());
                            double dblSellRate = 0; if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전신환매도율")].Text != "")Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전신환매도율")].Text.ToString());
                            double dblCashBuyRate = 0; if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "현금매입률")].Text != "")Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "현금매입률")].Text.ToString());
                            double dblCashSellRate = 0; if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "현금매도율")].Text != "")Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "현금매도율")].Text.ToString());
                            double dblUsdRate = 0; if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "달러환산율")].Text != "")Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "달러환산율")].Text.ToString());
                            double dblAllowScope = 0; if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오차허용범위")].Text != "")Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오차허용범위")].Text.ToString());

                            string strSql = " usp_BZB003 '" + strGbn + "'";
                            strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                            strSql = strSql + ", @pBAS_DT = '" + strBasDate + "'";
                            strSql = strSql + ", @pFR_CURR = '" + strFrCurr + "'";
                            strSql = strSql + ", @pTO_CURR = '" + strToCurr + "'";
                            strSql = strSql + ", @pACC_TYPE = '" + strAccType + "'";
                            strSql = strSql + ", @pBAS_RATE = '" + dblBasRate + "'";
                            strSql = strSql + ", @pBUY_RATE = '" + dblBuyRate + "'";
                            strSql = strSql + ", @pSELL_RATE = '" + dblSellRate + "'";
                            strSql = strSql + ", @pCASH_BUY_RATE = '" + dblCashBuyRate + "'";
                            strSql = strSql + ", @pCASH_SELL_RATE = '" + dblCashSellRate + "'";
                            strSql = strSql + ", @pUSD_RATE = '" + dblUsdRate + "'";
                            strSql = strSql + ", @pALLOW_SCOPE = '" + dblAllowScope + "'";
                            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

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
                    MSGCode = "P0001"; // 에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strBasDate, SystemBase.Base.GridHeadIndex(GHIdx1, "기준일"));
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

    }
}
