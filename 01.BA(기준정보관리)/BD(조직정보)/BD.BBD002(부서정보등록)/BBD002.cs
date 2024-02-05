#region 작성정보
/*********************************************************************/
// 단위업무명 : 부서정보등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-01-30
// 작성내용 : 부서정보등록 및 관리
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

namespace BD.BBD002
{
    public partial class BBD002 : UIForm.FPCOMM1
    {
        #region 생성자
        public BBD002()
        {
            InitializeComponent();
        }
        #endregion

        #region 팝업창 열기
        private void cmdMenu_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE = 'D020', @pSPEC1 = '" + SystemBase.Base.gstrCOMCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtReorgId.Text.Trim(), "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "부서개편조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReorgId.Text = Msgs[0].ToString();
                    txtReorgNm.Value = Msgs[1].ToString();
                    txtReorgId.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서개편조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region Form Load 시
        private void BBD001_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 콤보박스
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "사업장코드")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='B030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'",0);					 //사업장
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "비용배부레벨")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z009', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//비용배부구분
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "부서레벨")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'B002', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//부서레벨
                        
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
                string strQuery = " usp_BBD002  'S1'";
                strQuery = strQuery + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                strQuery = strQuery + ", @pREORG_ID ='" + txtReorgId.Text.Trim() + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;


            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true) // 그리드 상단 필수항목 체크
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                string strKeyCd = "";

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

                            strKeyCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text.ToString();
                            string strDeptCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text.ToString();//1
                            string strDeptNm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명")].Text.ToString();//2
                            string strUpDeptCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상위부서코드")].Text.ToString();//3
                            string strBizCd = ""; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사업장코드")].Text != "") strBizCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사업장코드")].Value.ToString();//4
                            string strCostCenter = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "코스트센터")].Text.ToString();//3
                            string strCostDistType = ""; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비용배부레벨")].Text != "") strCostDistType = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비용배부레벨")].Value.ToString();//5
                            string strDataLvl = ""; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서레벨")].Text != "") strDataLvl = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서레벨")].Value.ToString();//6
                            string strEndFlag = "N"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "END구분")].Text.ToString() == "True") strEndFlag = "Y";//7
                            string strDeptPos = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서위치")].Text.ToString();//8
                            string strDeptFullNm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서전명")].Text.ToString();//9
                            string strDeptEngNm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서영문명")].Text.ToString();//10
                            string strDeptOrder = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서순서")].Text.ToString();      //11: 2018.05.08. hma 추가
                            string strFinanceYn = "N"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재무팀여부")].Text.ToString() == "True") { strFinanceYn = "Y"; }   //12: 2022.01.14. hma 추가: 재무팀여부

                            string strSql = " usp_BBD002 '" + strGbn + "'";
                            strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                            strSql = strSql + ", @pREORG_ID = '" + txtReorgId.Text + "'";
                            strSql = strSql + ", @pDEPT_CD = '" + strDeptCd.Trim().ToUpper() + "'";
                            strSql = strSql + ", @pUP_DEPT_CD = '" + strUpDeptCd.Trim() + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pEND_FLAG = '" + strEndFlag + "'";
                            strSql = strSql + ", @pDEPT_POS = '" + strDeptPos.Trim() + "'";
                            strSql = strSql + ", @pDEPT_NM = '" + strDeptNm.Trim() + "'";
                            strSql = strSql + ", @pDEPT_FULL_NM = '" + strDeptFullNm.Trim() + "'";
                            strSql = strSql + ", @pDEPT_ENG_NM = '" + strDeptEngNm.Trim() + "'";
                            if (strKeyCd.Trim() != "*")
                            {
                                strSql = strSql + ", @pBIZ_CD = '" + strBizCd.Trim() + "'";
                                strSql = strSql + ", @pCOST_DIST_TYPE = '" + strCostDistType.Trim() + "'";
                                strSql = strSql + ", @pDATA_LVL = '" + strDataLvl.Trim() + "'";
                            }
                            strSql = strSql + ", @pCOST_CENTER = '" + strCostCenter + "'";
                            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pDEPT_ORDER = '" + strDeptOrder + "'";        // 2018.05.08. hma 추가: 부서순서
                            strSql = strSql + ", @pFINANCE_YN = '" + strFinanceYn + "'";        // 2022.01.14. hma 추가: 재무팀여부

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
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strKeyCd); //그리드 위치를 가져온다

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
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region txtReorgId 변환시  Menunm 조회
        private void txtReorgId_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtReorgId.Text != "")
                {
                    txtReorgNm.Value = SystemBase.Base.CodeName("REORG_ID", "REORG_NM", "B_REORG_INFO", txtReorgId.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtReorgNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            //코스트센터
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "코스트센터_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'D012', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "코스트센터")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00115", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "코스트센터 조회");	//코스트센터
                    pu.Width = 400;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "코스트센터")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "코스트센터명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "코스트센터 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 그리드 상 데이터 변경시 연계데이터 자동입력
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "코스트센터"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "코스트센터명")].Text
                    = SystemBase.Base.CodeName("COST_CENTER", "COST_CENTER_NM", "B_COST_CENTER", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "코스트센터")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
        }
        #endregion
    }
}
