#region 작성정보
/*********************************************************************/
// 단위업무명 : 채번정보등록
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-01-22
// 작성내용 : 채번정보등록 및 관리
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

namespace BA.BAA006
{
    public partial class BAA006 : UIForm.FPCOMM1
    {
        #region 생성자
        public BAA006()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BAA006_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            
            //콤보박스 Setting
            SystemBase.ComboMake.C1Combo(cboCO_CD, "usp_B_COMMON @pTYPE = 'CO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//법인정보
            cboCO_CD.SelectedValue = SystemBase.Base.gstrCOMCD.ToString();

            SystemBase.ComboMake.C1Combo(cboAUTO_NO_TYPE, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 3);//채번구분

            //그리드 콤보박스 Setting
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "채번구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B004', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "사업장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='BIZ', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "일자형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'Z008', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt,false, false, 0, 0);  
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BAA006  'S1'";
                strQuery = strQuery + ", @pAUTO_NO_TYPE ='" + cboAUTO_NO_TYPE.SelectedValue.ToString() + "' ";
                strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))// 그리드 필수항목 체크 
            {

                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                string strAutoNoType = "", strAutoNoTypeNm = "";

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

                            string strBizCd = "";
                            if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사업장")].Text != "")
                            {
                                strBizCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사업장")].Value.ToString();
                            }
                            string strApplyDt = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "적용시작일")].Text;
                            strAutoNoType = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "채번구분")].Value.ToString();
                            strAutoNoTypeNm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "채번구분")].Text;
                            string strNoPrefix = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "채번접두어")].Text;
                            string strDtType = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "일자형")].Value.ToString();
                            int intSeqLen = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번자리수")].Text);
                            int intSeqAdd = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번증가치")].Text);
                            string strAutoFlag = "N"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Auto")].Text == "True") strAutoFlag = "Y";

                            string strSql = " usp_BAA006 '" + strGbn + "'";
                            strSql = strSql + ", @pAUTO_NO_TYPE   = '" + strAutoNoType + "'";
                            if (strBizCd != "") strSql = strSql + ", @pBIZ_CD   = '" + strBizCd + "'";
                            if (strApplyDt != "") strSql = strSql + ", @pAPPLY_DT   = '" + strApplyDt + "'";
                            strSql = strSql + ", @pNO_PREFIX   = '" + strNoPrefix + "'";
                            strSql = strSql + ", @pDT_TYPE   = '" + strDtType + "'";
                            strSql = strSql + ", @pSEQ_LEN   = '" + intSeqLen + "'";
                            strSql = strSql + ", @pSEQ_ADD   = '" + intSeqAdd + "'";
                            strSql = strSql + ", @pAUTO_FLAG   = '" + strAutoFlag + "'";
                            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

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
                   MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                   SearchExec();
                   UIForm.FPMake.GridSetFocus(fpSpread1, strAutoNoTypeNm, SystemBase.Base.GridHeadIndex(GHIdx1, "채번구분"));
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
