#region 작성정보
/*********************************************************************/
// 단위업무명 : 공적실적등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-22
// 작성내용 : 공적실적등록 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
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

namespace PC.PCC007
{
    public partial class PCC007 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strWoNo = "";
        string strProcSeq = "";
        string strInspFlg = "";
        int Row = 0;
        string strKey = "";
        #endregion

        #region 생성자
        public PCC007()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void PCC007_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;

            dtpResultDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(-1).ToShortDateString().Substring(0,10);
            dtpResultDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(1).ToShortDateString().Substring(0,10);
            dtpResultDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정단계")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공정단계

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

            SystemBase.ComboMake.C1Combo(cboStatus, "usp_P_COMMON @pTYPE = 'P150' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시상태
            SystemBase.ComboMake.C1Combo(cboOrderFlag, "usp_B_COMMON @pType='COMM', @pCODE = 'P026', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시구분
            SystemBase.ComboMake.C1Combo(cboProclvl, "usp_B_COMMON @pType='COMM', @pCODE = 'P005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //공정단계		
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD.ToString();

            dtpResultDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(-1).ToShortDateString().Substring(0, 10);
            dtpResultDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(1).ToShortDateString().Substring(0, 10);
            dtpResultDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

            SystemBase.ComboMake.C1Combo(cboStatus, "usp_P_COMMON @pTYPE = 'P150' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시상태
            SystemBase.ComboMake.C1Combo(cboOrderFlag, "usp_B_COMMON @pType='COMM', @pCODE = 'P026', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시구분
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

        #region 조회조건 팝업
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
                    txtGroupCd.Text = Msgs[6].ToString();
                    txtGroupNm.Value = Msgs[7].ToString();
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

        //대표오더번호
        private void btnUnityOrder_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW028 pu = new WNDW.WNDW028();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtUnityOrderNo.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대표오더정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnJob_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P001' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtJobCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정작업코드 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtJobCd.Text = Msgs[0].ToString();
                    txtJobNm.Value = Msgs[1].ToString();
                    txtJobCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
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
                    txtGroupCd.Text = "";
                    txtGroupNm.Value = "";
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
        //제품코드
        private void txtGroupCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtGroupCd.Text != "")
                {
                    txtGroupNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtGroupCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtGroupNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtJobCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtJobCd.Text != "")
                {
                    txtJobNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtJobCd.Text, " AND MAJOR_CD = 'P001'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtJobNm.Value = "";
                }
            }
            catch
            {

            }
        }
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
                    strMQuery = "   usp_PCC007 @pTYPE = 'S1'";
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
                    strMQuery += ",            @pWORKORDER_NO_RS = '" + txtUnityOrderNo.Text + "' ";
                    strMQuery += ",            @pPROC_SEQ = '" + txtProcSeq.Text + "' ";
                    strMQuery += ",            @pGROUP_CD = '" + txtGroupCd.Text + "' ";
                    strMQuery += ",            @pORDER_FLAG = '" + cboOrderFlag.SelectedValue.ToString() + "' ";
                    strMQuery += ",            @pJOB_NM = '" + txtJobCd.Text + "' ";
                    strMQuery += ",            @pDELV_DT_FR = '" + dtpDelvDtFr.Text + "' ";
                    strMQuery += ",            @pDELV_DT_TO = '" + dtpDelvDtTo.Text + "' ";
                    strMQuery += ",            @pPROC_LVL = '" + cboProclvl.SelectedValue.ToString() + "' ";
                    strMQuery += ",            @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strMQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 5);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                        {
                            if (Convert.ToInt32(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "생산수량")].Value)
                                < Convert.ToInt32(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정오더수량")].Value)
                                && fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "오더상태")].Value.ToString() != "CL")
                            {
                                UIForm.FPMake.grdReMake(fpSpread2, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량") + "|1");
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread2, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량") + "|3");
                            }
                        }

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

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec2()
        { 
            FarPoint.Win.Spread.FpSpread grid = null;

            //그리드 상단 필수 체크

            if (fpSpread1.Focused == true)
            {
                grid = fpSpread1;
            }
            else
            {
                grid = fpSpread2;
            }

            grid.Focus();

            //if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
            if (SystemBase.Validation.FPGrid_SaveCheck(grid, this.Name, grid.Name, true))
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    if (grid == fpSpread2)
                    {
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text;

                            double GoodQty = 0, TotQty = 0;
                            GoodQty = Convert.ToDouble(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Value);
                            TotQty = Convert.ToDouble(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "생산수량")].Value);

                            if (strHead.Length > 0)
                            {
                                string strSql = " usp_PCC007 'U1'";
                                strSql += ", @pWORKORDER_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text + "' ";
                                strSql += ", @pPROC_SEQ = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text + "' ";
                                if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정오더수량")].Text != "")
                                { strSql += ", @pPROC_ORDER_QTY = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정오더수량")].Value + "' "; }
                                else
                                { strSql += ", @pPROC_ORDER_QTY = '0' "; }
                                if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "생산수량")].Text != "")
                                { strSql += ", @pPROD_QTY_IN_ORDER_UNIT = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "생산수량")].Value + "' "; }
                                else
                                { strSql += ", @pPROD_QTY_IN_ORDER_UNIT = '0' "; }
                                if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Text != "")
                                { strSql += ", @pINSP_GOOD_QTY_IN_ORDER_UNIT = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Value + "' "; }
                                else
                                { strSql += ", @pINSP_GOOD_QTY_IN_ORDER_UNIT = '0' "; }
                                if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량")].Text != "")
                                { strSql += ", @pINSP_BAD_QTY_IN_ORDER_UNIT = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량")].Value + "' "; }
                                else
                                { strSql += ", @pINSP_BAD_QTY_IN_ORDER_UNIT = '0'"; }
                                strSql += ", @pROUT_ORDER = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정단계")].Value + "' ";
                                strSql += ", @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                                strSql += ", @pITEM_CD = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text + "' ";
                                strSql += ", @pE_ITEM = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제품코드")].Text + "' ";
                                strSql += ", @pMILESTONE_FLG = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "마일스톤여부")].Text + "' ";
                                strSql += ", @pPROJECT_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text + "' ";
                                strSql += ", @pPROJECT_SEQ = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text + "' ";
                                strSql += ", @pINSP_FLG = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정검사여부")].Text + "' ";
                                strSql += ", @pINSP_REQ_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text + "' ";
                                strSql += ", @pMAKEORDER_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제품오더번호")].Text + "' ";
                                strSql += ", @pDEPT_CD = '" + SystemBase.Base.gstrDEPT.ToString() + "' ";
                                strSql += ", @pREORG_ID = '" + SystemBase.Base.gstrREORG_ID.ToString() + "' ";
                                strSql += ", @pWC_CD = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Text + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "' ";
                                strSql += ", @pRESULT_DT = '" + dtpResultDt.Text + "' ";
                                strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

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
                    }
                    else if (grid == fpSpread1)
                    {
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                            if (strHead.Length > 0)
                            {
                                string strSql = " usp_PCC007 'D1'";
                                strSql += ", @pWORKORDER_NO = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text + "' ";
                                strSql += ", @pPROC_SEQ = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text + "' ";
                                strSql += ", @pINSP_FLG = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "공정검사여부")].Text + "' ";
                                strSql += ", @pSEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "보고횟수")].Text + "' ";
                                strSql += ", @pINSP_GOOD_QTY_IN_ORDER_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사양품수량")].Value + "' ";
                                strSql += ", @pINSP_BAD_QTY_IN_ORDER_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사불량수량")].Value + "' ";
                                strSql += ", @pINSP_REQ_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "' ";
                                strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK")
                                {
                                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                                    Trans.Rollback();
                                    goto Exit;
                                }	// ER 코드 Return시 점프
                            }
                        }
                    }
                    else
                    {
                        Trans.Rollback();
                        ERRCode = "ER";
                        MSGCode = "선택된 그리드가 없습니다.";
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
                    AutoMessageBox mBox = new AutoMessageBox(SystemBase.Base.MessageRtn(MSGCode));
                    mBox.ShowDialog();
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

        #region 실적조회
        private void SubSearch(int Row)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strMQuery = "";
                    strMQuery = "   usp_PCC007 @pTYPE = 'S2'";
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

        #region 그리드 체인지시 update 체크
        private void fpSpread2_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            UIForm.FPMake.fpChange(fpSpread2, e.Row);
        }
        #endregion

        #region 그리드2 체인지시 이벤트
        private void fpSpread2_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량"))
            {
                if ((Convert.ToDouble(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Value) == 0.00 &&
                    Convert.ToDouble(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량")].Value) == 0.00) ||
                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Text == "" &&
                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량")].Text == "")
                {
                    fpSpread2.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";

                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Value = 0;
                }
            }
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, " 불량수량"))
            {
                if ((Convert.ToDouble(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Value) == 0.00 &&
                    Convert.ToDouble(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량")].Value) == 0.00) ||
                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Text == "" &&
                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량")].Text == "")
                {
                    fpSpread2.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";

                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량")].Value = 0;
                }
            }
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

                PCC007P1 myForm = new PCC007P1(ProjectNo, ProjectSeq, ItemCd, WoNo);
                myForm.ShowDialog();
            }
        }
        #endregion

        #region 부품내역
        private void btnItemSpec_Click(object sender, System.EventArgs e)
        {
            if (strWoNo == "")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0061", "제조오더번호"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PCC007P2 form = new PCC007P2(strWoNo, strProcSeq);
            form.ShowDialog();
        }
        #endregion

        #region fpSpread2 Select 이벤트
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

    }
}
