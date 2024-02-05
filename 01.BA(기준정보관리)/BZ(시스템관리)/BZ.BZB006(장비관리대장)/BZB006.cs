#region 작성정보
/*********************************************************************/
// 단위업무명 : 단위환산정보등록
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-03-22
// 작성내용 : 단위환산정보등록 및 관리
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

namespace BZ.BZB006
{
    public partial class BZB006 : UIForm.FPCOMM1
    {
        #region 생성자
        public BZB006()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BZB006_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.ComboMake.C1Combo(cboChangeCycle, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z010', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B070', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "사용여부")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z010', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "적용주기")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B071', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);

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
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BZB006  'S1'";
                strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @pUSE_YN ='" + cboChangeCycle.SelectedValue.ToString() + "' ";

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
                string strFrUnit = "";

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

                            string strSql = " usp_BZB006 '" + strGbn + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pEQIP_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Value + "'";
                            strSql = strSql + ", @pEQIP_SOLUTION = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "솔루션")].Text.ToString() + "'";
                            strSql = strSql + ", @pEQIP_MODEL = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "모델")].Text.ToString() + "'";
                            strSql = strSql + ", @pEQIP_IP = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "IP")].Text.ToString() + "'";
                            strSql = strSql + ", @pEQIP_TOOL = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리도구")].Text.ToString() + "'";
                            strSql = strSql + ", @pEQIP_DATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "일자")].Text.ToString() + "'";
                            strSql = strSql + ", @pCON_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "콘솔 계정")].Text.ToString() + "'";
                            strSql = strSql + ", @pCON_PWD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "콘솔 P/W")].Text.ToString() + "'";
                            strSql = strSql + ", @pOS_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "OS 계정")].Text.ToString() + "'";
                            strSql = strSql + ", @pOS_PWD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "OS P/W")].Text.ToString() + "'";
                            strSql = strSql + ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text.ToString() + "'";
                            strSql = strSql + ", @pUSE_YN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용여부")].Value + "'";
                            strSql = strSql + ", @pCHANGE_CYCLE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "적용주기")].Value + "'";
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
                    UIForm.FPMake.GridSetFocus(fpSpread1, strFrUnit, SystemBase.Base.GridHeadIndex(GHIdx1, "기준단위"));
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
