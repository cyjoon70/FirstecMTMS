#region 작성정보
/*********************************************************************/
// 단위업무명:  검사공정진행현황
// 작 성 자  :  한 미 애
// 작 성 일  :  2015-09-24
// 작성내용  :  검사공정진행현황
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :  공정실적등록/취소 프로그램 이용하여 수정함.
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
using WNDW;
using FarPoint.Win.Spread;

namespace QE.QRE013
{
    public partial class QRE013 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strWoNo = "";
        string strProcSeq = "";
        string strInspFlg = "";
        int Row = 0;
        string strKey = "";
        #endregion

        #region 생성자
        public QRE013()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void QRE013_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;

            dtpResultDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(-1).ToShortDateString().Substring(0,10);
            dtpResultDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(1).ToShortDateString().Substring(0,10);
            //dtpResultDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정단계")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공정단계

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

            SystemBase.ComboMake.C1Combo(cboStatus, "usp_P_COMMON @pTYPE = 'P150' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시상태
            // 2019.02.11. hma 수정: 지시구분 검색조건을 삭제하여 주석 처리함.
            //SystemBase.ComboMake.C1Combo(cboOrderFlag, "usp_B_COMMON @pType='COMM', @pCODE = 'P026', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시구분
            SystemBase.ComboMake.C1Combo(cboProclvl, "usp_B_COMMON @pType='COMM', @pCODE = 'P005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //공정단계		

            // 2019.02.14. hma 수정: 검사완료목표일이 필수항목이 아닌걸로 변경되어 주석 처리함.
            // 2019.02.11. hma 추가(Start): 검사완료목표일 FROM,TO 검색조건의 디폴트 일자가 완료예정일자 검색조건과 동일하게 들어가도록 함. 
            //dtpInspPlanFirmDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(-1).ToShortDateString().Substring(0, 10);
            //dtpInspPlanFirmDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(1).ToShortDateString().Substring(0, 10);
            // 2019.02.11. hma 추가(End)

            // 2019.02.14. hma 추가(Start)
            dtpDelvDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(-1).ToShortDateString().Substring(0, 10);
            dtpDelvDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(1).ToShortDateString().Substring(0, 10);
            // 2019.02.14. hma 추가(End)
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD.ToString();

            dtpResultDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(-1).ToShortDateString().Substring(0, 10);
            dtpResultDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(1).ToShortDateString().Substring(0, 10);
            //dtpResultDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

            SystemBase.ComboMake.C1Combo(cboStatus, "usp_P_COMMON @pTYPE = 'P150' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시상태
            // 2019.02.11. hma 수정: 지시구분 검색조건을 삭제하여 주석 처리함.
            //SystemBase.ComboMake.C1Combo(cboOrderFlag, "usp_B_COMMON @pType='COMM', @pCODE = 'P026', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시구분
            SystemBase.ComboMake.C1Combo(cboProclvl, "usp_B_COMMON @pType='COMM', @pCODE = 'P005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //공정단계

            rdoNo.Checked = true;
        }
        #endregion

        #region DelExe()
        protected override void DelExe()
        {
            fpSpread1.Focus();
        }
        #endregion

        #region 검색조건 버튼 클릭시 팝업창 띄우기
        //공장
        private void btnPlant_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P013', @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };											  // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtPlantCd.Text, "" };															  // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtPlantCd.Text = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();
                    txtPlantCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제조오더번호_FR
        private void btnWorkOrderNo_Fr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNo_Fr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNo_Fr.Text = Msgs[1].ToString();
                    txtWorkOrderNo_Fr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제조오더번호_FR
        private void btnWorkOrderNo_To_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNo_To.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNo_To.Text = Msgs[1].ToString();
                    txtWorkOrderNo_To.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //사업
        private void btnEnt_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEntCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEntCd.Text = Msgs[0].ToString();
                    txtEntNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트번호
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1", "S");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtEntCd.Text = Msgs[1].ToString();
                    txtEntNm.Value = Msgs[2].ToString();
                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = Msgs[5].ToString();
                    //txtGroupCd.Text = Msgs[6].ToString();         // 2019.02.11. hma 수정: 제품코드 검색조건을 삭제하여 주석 처리함.
                    //txtGroupNm.Value = Msgs[7].ToString();        // 2019.02.11. hma 수정: 제품코드 검색조건을 삭제하여 주석 처리함.
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목코드
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtItemCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    txtItemCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업장
        private void btnWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCd.Text = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                    txtWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2019.02.11. hma 수정(Start): 대표오더번호 검색조건을 삭제하여 주석 처리함.
        //대표오더번호
        //private void btnUnityOrder_Click(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        WNDW.WNDW028 pu = new WNDW.WNDW028();
        //        pu.ShowDialog();
        //        if (pu.DialogResult == DialogResult.OK)
        //        {
        //            string[] Msgs = pu.ReturnVal;
        //            txtUnityOrderNo.Value = Msgs[1].ToString();
        //        }
        //    }
        //    catch (Exception f)
        //    {
        //        SystemBase.Loggers.Log(this.Name, f.ToString());
        //        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대표오더정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        // 2019.02.11. hma 수정(End)

        // 2019.02.11. hma 수정(Start): 작업코드 검색조건을 삭제하여 주석 처리함.
        //private void btnJob_Click(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P001' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
        //        string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
        //        string[] strSearch = new string[] { txtJobCd.Text, "" };
        //        UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정작업코드 조회");
        //        pu.Width = 500;
        //        pu.ShowDialog();
        //        if (pu.DialogResult == DialogResult.OK)
        //        {
        //            Regex rx1 = new Regex("#");
        //            string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
        //            txtJobCd.Text = Msgs[0].ToString();
        //            txtJobNm.Value = Msgs[1].ToString();
        //            txtJobCd.Focus();
        //        }
        //    }
        //    catch (Exception f)
        //    {
        //        SystemBase.Loggers.Log(this.Name, f.ToString());
        //        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        // 2019.02.11. hma 수정(End)
        #endregion

        #region 코드 입력시 코드명 자동 입력
        //공장
        private void txtPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPlantCd.Text != "")
                {
                    txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPlantNm.Value = "";
                }
            }
            catch
            {

            }
        }
        //사업
        private void txtEntCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEntCd.Text != "")
                {
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtEntNm.Value = "";
                }
            }
            catch
            {

            }
        }
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
                if (txtProjectNm.Text == "")
                {
                    txtEntCd.Text = "";
                    txtEntNm.Value = "";
                    txtProjectNm.Value = "";
                    txtProjectSeq.Text = "";
                    //txtGroupCd.Text = "";     // 2019.02.11. hma 수정: 제품코드 검색조건을 제외하여 주석 처리함.
                    //txtGroupNm.Value = "";    // 2019.02.11. hma 수정: 제품코드 검색조건을 제외하여 주석 처리함.
                }
            }
            catch
            {

            }
        }

        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //작업장
        private void txtWcCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWcCd.Text != "")
                {
                    txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P002'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtWcNm.Value = "";
                }
            }
            catch
            {

            }
        }

        // 2019.02.11. hma 수정(Start): 제품코드 검색조건을 제외하여 주석 처리함.
        //제품코드
        //private void txtGroupCd_TextChanged(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        if (txtGroupCd.Text != "")
        //        {
        //            txtGroupNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtGroupCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        //        }
        //        else
        //        {
        //            txtGroupNm.Value = "";
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}
        // 2019.02.11. hma 수정(End)

        // 2019.02.11. hma 수정(Start): 작업코드 검색조건을 제외하여 주석 처리함.
        //private void txtJobCd_TextChanged(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        if (txtJobCd.Text != "")
        //        {
        //            txtJobNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtJobCd.Text, " AND MAJOR_CD = 'P001'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");
        //        }
        //        else
        //        {
        //            txtJobNm.Value = "";
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}
        // 2019.02.11. hma 수정(End)
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            Search(0, true);

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void Search(int Row, bool Msg)
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strResultYn = "";

                    if (rdoYes.Checked == true) { strResultYn = "Y"; }
                    else if (rdoNo.Checked == true) { strResultYn = "N"; }

                    string strMQuery = "";
                    strMQuery = "   usp_QRE013 @pTYPE = 'S1'";
                    strMQuery += ",            @pRESULT_DT_FR = '" + dtpResultDtFr.Text + "' ";
                    strMQuery += ",            @pRESULT_DT_TO = '" + dtpResultDtTo.Text + "' ";
                    strMQuery += ",            @pENT_CD = '" + txtEntCd.Text + "' ";
                    strMQuery += ",            @pWORKORDER_NO_FR = '" + txtWorkOrderNo_Fr.Text + "' ";
                    strMQuery += ",            @pWORKORDER_NO_TO = '" + txtWorkOrderNo_To.Text + "' ";
                    strMQuery += ",            @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strMQuery += ",            @pWC_CD = '" + txtWcCd.Text + "' ";
                    strMQuery += ",            @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                    strMQuery += ",            @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strMQuery += ",            @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strMQuery += ",            @pORDER_STATUS = '" + cboStatus.SelectedValue.ToString() + "' ";
                    strMQuery += ",            @pRESULT_YN = '" + strResultYn + "' ";
                    //strMQuery += ",          @pWORKORDER_NO_RS = '" + txtUnityOrderNo.Text + "' ";              // 2019.02.11. hma 수정: 대표오더번호 검색조건을 삭제하여 주석 처리함.
                    strMQuery += ",            @pPROC_SEQ = '" + txtProcSeq.Text + "' ";
                    //strMQuery += ",          @pGROUP_CD = '" + txtGroupCd.Text + "' ";                          // 2019.02.11. hma 수정: 제품코드 검색조건을 삭제하여 주석 처리함.
                    //strMQuery += ",          @pORDER_FLAG = '" + cboOrderFlag.SelectedValue.ToString() + "' ";  // 2019.02.11. hma 수정: 지시구분 검색조건을 삭제하여 주석 처리함.
                    //strMQuery += ",          @pJOB_NM = '" + txtJobCd.Text + "' ";                              // 2019.02.11. hma 수정: 작업코드 검색조건을 삭제하여 주석 처리함.
                    strMQuery += ",            @pDELV_DT_FR = '" + dtpDelvDtFr.Text + "' ";
                    strMQuery += ",            @pDELV_DT_TO = '" + dtpDelvDtTo.Text + "' ";
                    strMQuery += ",            @pPROC_LVL = '" + cboProclvl.SelectedValue.ToString() + "' ";
                    strMQuery += ",            @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strMQuery += ",            @pQA_DUTY = '" + txtQaDuty.Text + "'";                       // 품보담당자
                    // 2019.02.11. hma 수정(Start): 기계QC담당과 전자QC담당 검색조건 대신 기계QC담당1과 전자QC담당1로 체크하도록 하고, 검사완료목표일, 검사Release일자 추가
                    //strMQuery += ",          @pQC_MACH_DUTY = '" + txtMachInspCd.Text + "'";              // 2015.10.15. hma 추가: 기계검사원
                    //strMQuery += ",          @pQC_ELEC_DUTY = '" + txtElecInspCd.Text + "'";              // 2015.10.15. hma 추가: 전자검사원
                    strMQuery += ",            @pQC_MACH_DUTY1 = '" + txtMachInspCd.Text + "'";             // 기계QC담당1
                    strMQuery += ",            @pQC_ELEC_DUTY1 = '" + txtElecInspCd.Text + "'";             // 전자QC담당1
                    strMQuery += ",            @pINSP_PLAN_DT_FR = '" + dtpInspPlanFirmDtFr.Text + "' ";    // 검사완료목표일FROM
                    strMQuery += ",            @pINSP_PLAN_DT_TO = '" + dtpInspPlanFirmDtTo.Text + "' ";    // 검사완료목표일TO
                    strMQuery += ",            @pINSP_REL_DT_FR = '" + dtpInspRelDtFr.Text + "' ";          // 검사Release일자FROM
                    strMQuery += ",            @pINSP_REL_DT_TO = '" + dtpInspRelDtTo.Text + "' ";          // 검사Release일자TO
                    // 2019.02.11. hma 수정(End)

                    UIForm.FPMake.grdCommSheet(fpSpread2, strMQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 5);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        int x = 0, y = 0;

                        if (strKey != "")
                        {
                            fpSpread2.Search(0, strKey, false, false, false, false, 0, 0, ref x, ref y);

                            if (x > 0)
                            {
                                fpSpread2.Sheets[0].SetActiveCell(x, 0);
                                fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                                fpSpread2.Sheets[0].AddSelection(x, y, 1, fpSpread2.Sheets[0].ColumnCount);
                            }
                            else
                            {
                                x = 0;
                            }
                        }

                        fpSpread2.Sheets[0].AddSelection(x, 1, 1, fpSpread2.Sheets[0].ColumnCount);

                        // 2019.02.20. hma 추가(Start): 전자담당1과 기계담당1 저장/조회 데이터 구분에 따라 글자색깔 다르게 지정. 조회 데이터인 경우 보라색으로.
                        for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "전자담당1YN")].Text == "N")
                            {
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "전자QC담당1")].ForeColor = Color.BlueViolet;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "전자QC담당명")].ForeColor = Color.BlueViolet;
                            }
                            if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "기계담당1YN")].Text == "N")
                            {
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "기계QC담당1")].ForeColor = Color.BlueViolet;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "기계QC담당명")].ForeColor = Color.BlueViolet;
                            }
                        }
                        // 2019.02.20. hma 추가(End)

                        //상세정보조회
                        SubSearch(x);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SubSearch(): 실적조회
        private void SubSearch(int Row)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strMQuery = "";
                    strMQuery = "   usp_QRE013 @pTYPE = 'S2'";
                    strMQuery += ",            @pWORKORDER_NO = '" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text + "' ";
                    strMQuery += ",            @pPROC_SEQ = '" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text + "' ";
                    strMQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 그리드 항목값 변경시 Update 체크
        private void fpSpread2_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            UIForm.FPMake.fpChange(fpSpread2, e.Row);
        }
        #endregion

        #region 공정진행현황
        private void btnProcInfo_Click(object sender, System.EventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                int Row = fpSpread2.Sheets[0].ActiveRowIndex;

                string ProjectNo = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text;
                string ProjectSeq = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text;
                string ItemCd = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text;
                string WoNo = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text;

                QRE013P1 myForm = new QRE013P1(ProjectNo, ProjectSeq, ItemCd, WoNo);
                myForm.ShowDialog();
            }
        }
        #endregion

        #region btnItemSpec_Click():  부품내역 버튼 클릭 처리
        private void btnItemSpec_Click(object sender, System.EventArgs e)
        {
            if (strWoNo == "")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0061", "제조오더번호"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            QRE013P2 form = new QRE013P2(strWoNo, strProcSeq);
            form.ShowDialog();
        }
        #endregion

        #region fpSpread2_LeaveCell():  fpSpread2 Select 이벤트
        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                strKey = fpSpread2.Sheets[0].Cells[e.NewRow, 0].Text;

                strWoNo = fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text;
                strProcSeq = fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text;
                strInspFlg = fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공정검사여부")].Text;

                if (e.Row != e.NewRow)
                {
                    SubSearch(e.NewRow);
                }
            }
            else
            {
                Row = 0;
            }
        }
        #endregion

        #region fpSpread2_KeyDown
        private void fpSpread2_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    fpSpread2.Sheets[0].ClipboardCopy();
                }

                if (e.Control && e.KeyCode == Keys.V)
                {

                    fpSpread2.Sheets[0].ClipboardPaste(ClipboardPasteOptions.Values);

                    // 복사된 행의 열을 구하기 위하여 클립보드 사용.

                    IDataObject iData = Clipboard.GetDataObject();

                    string strClp = (string)iData.GetData(DataFormats.Text);

                    if (strClp != "" || strClp != null || strClp.Length > 0)
                    {
                        Regex rx1 = new Regex("\r\n");
                        string[] arrData = rx1.Split(strClp.ToString());

                        int DataCount = arrData.Length - 1;

                        if (DataCount > 0)
                        {
                            int STRow = fpSpread2.ActiveSheet.ActiveRowIndex;
                            if (STRow < 0)
                                STRow = 0;

                            int ClipRowCount = STRow + DataCount;
                            if (fpSpread2.Sheets[0].RowCount < DataCount)
                                ClipRowCount = fpSpread2.Sheets[0].RowCount - STRow;

                            for (int i = STRow; i < ClipRowCount; i++)
                            {
                                if (i < fpSpread2.Sheets[0].RowCount
                                    || fpSpread2.Sheets[0].Cells[i, fpSpread2.ActiveSheet.ActiveColumnIndex].Locked != true)
                                {
                                    if (fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text != "I")
                                    { fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "U"; }
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "Clipboard 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnQaDuty_Click():  품보담당자 검색 팝업
        private void btnQaDuty_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pSPEC1 = 'Q030' ";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                string[] strWhere = new string[] { "@pCODE", "@pNAME" };    // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtQaDuty.Text, "" };   // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00055", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품보담당자 조회");
                pu.ShowDialog();	// 공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtQaDuty.Text = Msgs[0].ToString();
                    txtQaDutyNm.Value = Msgs[1].ToString();
                    txtQaDuty.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품보담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region txtQaDuty_TextChanged():  품보담당자 수정시
        private void txtQaDuty_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtQaDuty.Text != "")
                {
                    // 2019.02.11. hma 수정(Start): 품보담당자명을 가져오기 위함이므로 품보담당자 항목값을 가져가도록 함.
                    //txtQaDutyNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtJobCd.Text, " AND MAJOR_CD = 'Q030'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");
                    txtQaDutyNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtQaDuty.Text, " AND MAJOR_CD = 'Q030'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");
                    // 2019.02.11. hma 수정(End)
                }
                else
                {
                    txtQaDutyNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        // 2015.11.03. hma 추가(Start): 검사지연사유 입력분 저장
        #region SaveExec(): 폼에 입력된 데이타 저장 로직
        protected override void SaveExec2()
        {
            //FarPoint.Win.Spread.FpSpread grid = null;

            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread2, this.Name, fpSpread2.Name, true))
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text;

                        if (strHead.Length > 0)
                        {
                            string strSql = " usp_QRE013 'U1'";
                            strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                            strSql += ", @pWORKORDER_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text + "' ";
                            strSql += ", @pPROC_SEQ = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text + "' ";
                            strSql += ", @pITEM_CD = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text + "' ";
                            strSql += ", @pPROJECT_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text + "' ";
                            strSql += ", @pPROJECT_SEQ = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text + "' ";
                            strSql += ", @pRES_CD = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "자원")].Text + "' ";
                            strSql += ", @pMAKEORDER_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제품오더번호")].Text + "' ";
                            strSql += ", @pWC_CD = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Text + "' ";
                            strSql += ", @pDELAY_REASON = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "검사지연사유")].Text + "' ";
                            // 2019.02.11. hma 추가(Start): 검사완료목표일, 전자QC담당1, 기계QC담당1 항목도 저장되도록 함.
                            strSql += ", @pQC_ELEC_DUTY1 = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "전자QC담당1")].Text + "' ";
                            strSql += ", @pQC_MACH_DUTY1 = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "기계QC담당1")].Text + "' ";
                            strSql += ", @pINSP_PLAN_DT = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "완료목표일")].Text + "' ";
                            // 2019.02.11. hma 추가(End)
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK")
                            {
                                fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                                Trans.Rollback();
                                goto Exit;
                            }	// ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                Search(Row, false);
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
        // 2015.11.03. hma 추가(End)

        // 2015.10.15. hma 추가(Start)
        #region 검색조건 항목들에 대한 버튼 클릭시 팝업창 띄우기
        // 기계검사원 팝업을 위한 매개변수로 검사원 팝업 호출
        private void btnInspectorCd_Click(object sender, EventArgs e)
        {
            InspectorCd_Popup("F007-G", txtMachInspCd.Text.Trim(), "기계QC담당");
        }

        // 전자검사원 팝업을 위한 매개변수로 검사원 팝업 호출
        private void c1Button1_Click(object sender, EventArgs e)
        {
            InspectorCd_Popup("F008-G", txtElecInspCd.Text.Trim(), "전자QC담당");
        }

        // InspectorCd_Popup(): 검사원 팝업 처리
        private void InspectorCd_Popup(string InspType, string InputInspCd, string PopupTitle)
        {
            try
            {
                // 그룹자원 데이터에서 기계QC담당자는 그룹자원코드가 F007-G이고, 전자QC담당자는 F008-G임.
                //string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q005', @pSPEC2='" + InspType + "',@pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                //string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                //string[] strSearch = new string[] { InputInspCd, "" };
                //UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, PopupTitle);
                string strQuery = " usp_P_COMMON @pType='P058' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC", "@pUSE_YN" };
                string[] strSearch = new string[] { InputInspCd, "", InspType, "1" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, PopupTitle);                
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    switch (InspType)
                    {
                        case "F007-G": txtMachInspCd.Text = Msgs[0].ToString();
                            txtMachInspNm.Value = Msgs[1].ToString();
                            break;
                        case "F008-G": txtElecInspCd.Text = Msgs[0].ToString();
                            txtElecInspNm.Value = Msgs[1].ToString();
                            break;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void txtMachInspCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtMachInspCd.Text != "")
                {
                    txtMachInspNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_GROUP", txtMachInspCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtMachInspNm.Value = "";
                }
            }
            catch { }
        }

        private void txtElecInspCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtElecInspCd.Text != "")
                {
                    txtElecInspNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_GROUP", txtElecInspCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtElecInspNm.Value = "";
                }
            }
            catch { }
        }
        #endregion
        // 2015.10.15. hma 추가(End)

        // 2019.02.11. hma 추가(Start)
        #region txtWorkOrderNo_Fr_TextChanged():  제조오더번호FROM 검색조건에 입력시 제조오더번호TO 항목에 FROM값이 들어가도록 함. 
        private void txtWorkOrderNo_Fr_TextChanged(object sender, EventArgs e)
        {
            txtWorkOrderNo_To.Text = txtWorkOrderNo_Fr.Text;
        }
        #endregion

        #region fpSpread2_ButtonClicked(): 그리드 버튼 클릭 처리. 전자QC담당1 및 기계QC담당1 버튼 클릭시 해당하는 팝업창을 띄워준다.
        private void fpSpread2_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            string strQcDuty = "";

            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "전자QC담당1_2"))
            {
                string strQuery = " usp_QBA032 @pTYPE = 'P1' , @pGRES_CD='F008-G'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                string[] strWhere = new string[] { "@pRES_CD", "@pRES_DIS" };
                string[] strSearch = new string[] { fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "전자QC담당1")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "자원 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    strQcDuty = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "전자QC담당1")].Text;

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "전자QC담당1")].Text = Msgs[0].ToString();
                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "전자QC담당명")].Text = Msgs[1].ToString();

                    if (Msgs[0].ToString() != strQcDuty)        // 기존 항목값과 다른 값을 선택한 경우 그리드 플래그값을 변경시켜줌.
                    {
                        UIForm.FPMake.fpChange(fpSpread2, e.Row);
                    }
                }                
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "기계QC담당1_2"))
            {
                string strQuery = " usp_QBA032 @pTYPE = 'P1' , @pGRES_CD='F007-G'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                string[] strWhere = new string[] { "@pRES_CD", "@pRES_DIS" };
                string[] strSearch = new string[] { fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "기계QC담당1")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "자원 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    strQcDuty = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "기계QC담당1")].Text;

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "기계QC담당1")].Text = Msgs[0].ToString();
                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "기계QC담당명")].Text = Msgs[1].ToString();

                    if (Msgs[0].ToString() != strQcDuty)        // 기존 항목값과 다른 값을 선택한 경우 그리드 플래그값을 변경시켜줌.
                    {
                        UIForm.FPMake.fpChange(fpSpread2, e.Row);
                    }
                }
            }
        }
        #endregion

        #region fpSpread2_Change(): 그리드 변경 처리. 전자QC담당1 및 기계QC담당1 변경시 입력된 코드에 대한 담당자명이 담당자명 항목에 들어가게 한다.
        private void fpSpread2_Change(object sender, ChangeEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "전자QC담당1"))
            {
                fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "전자QC담당명")].Text
                    = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_GROUP", fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "전자QC담당1")].Text, " AND GRES_CD = 'F008-G' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "기계QC담당1"))
            {
                fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "기계QC담당명")].Text
                    = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_GROUP", fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "기계QC담당1")].Text, " AND GRES_CD = 'F007-G' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
        }
        #endregion
        // 2019.02.11. hma 추가(End)
    }
}
