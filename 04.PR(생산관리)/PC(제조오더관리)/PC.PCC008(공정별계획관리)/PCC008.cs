#region 작성정보
/*********************************************************************/
// 단위업무명 : 공정별계획관리
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-05
// 작성내용 : 공정별계획관리
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

namespace PC.PCC008
{
    public partial class PCC008 : UIForm.FPCOMM2
    {
        #region 변수선언
        string WoCd = "";
        string MstItemCd = "";
        string strWorkOrderNo = "";
        string strProcSeq = "";
        string strStatus = "";
        string MstWoNo = "";
        #endregion

        #region 생성자
        public PCC008()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼 로드 이벤트
        private void PCC008_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1); //필수체크

            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD.ToString();

            dtpPlanDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(-1).ToShortDateString();
            dtpPlanDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(1).ToShortDateString();

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "마일스톤여부")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B029', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공정검사여부")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B029', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "지시상태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P020', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT유형
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "통화")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//화폐단위

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "지시상태")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P150', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "지시구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P026', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 3);

            SystemBase.ComboMake.C1Combo(cboStatus, "usp_P_COMMON @pTYPE = 'P150' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시상태
            SystemBase.ComboMake.C1Combo(cboOrderFlag, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P026', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시구분

            cboStatus.SelectedValue = "RL";

            strStatus = "";
        }
        #endregion

        #region RowInsert() 행추가
        protected override void RowInsExec()
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    if (MstWoNo == "")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0061", "제조오더번호"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    UIForm.FPMake.RowInsert(fpSpread1);

                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간")].Value = 0;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "공정부하시수")].Value = 0;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value = 0;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간")].Value = 0;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량")].Value = 1;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "제조L/T")].Value = 0;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "마일스톤여부")].Value = "N";
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "공정검사여부")].Value = "N";
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "공정검사여부")].Value = "N";
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "지시상태")].Value = strStatus;

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0052"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        #endregion

        #region RowInsert() 행복사
        protected override void RCopyExec()
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        string BefStatus = "", AftStatus = "";

                        BefStatus = Convert.ToString(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "지시상태")].Value);

                        if (fpSpread1.Sheets[0].Rows.Count - 1 == fpSpread1.Sheets[0].ActiveRowIndex)
                        {
                            UIForm.FPMake.RowCopy(fpSpread1);

                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "지시상태")].Value = "RL";
                        }
                        else
                        {
                            AftStatus = Convert.ToString(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "지시상태")].Value);

                            if (BefStatus != "RL" && AftStatus != "RL")
                            {
                                MessageBox.Show(SystemBase.Base.MessageRtn("P0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                UIForm.FPMake.RowCopy(fpSpread1);

                                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "지시상태")].Value = "RL";
                            }
                        }
                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0052"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
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

        //제조오더번호
        private void btnWorkOrderNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNo.Text = Msgs[1].ToString();
                    txtWorkOrderNo.Focus();
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
        #endregion

        #region 코드 입력시 코드명 자동 입력
        //공장
        private void txtPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try 
            {
                if (txtPlantCd.Text != "")
                {
                    txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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
                    txtItemCd.Text = "";
                    txtItemNm.Value = "";
                }

            }
            catch { }
        }

        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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
        //제품코드
        private void txtGroupCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtGroupCd.Text != "")
                {
                    txtGroupNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtGroupCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strMQuery = "";
                strMQuery = "   usp_PCC008 @pTYPE = 'S1'";
                strMQuery += ",            @pRESULT_DT_FR = '" + dtpPlanDtFr.Text + "' ";
                strMQuery += ",            @pRESULT_DT_TO = '" + dtpPlanDtTo.Text + "' ";
                strMQuery += ",            @pENT_CD = '" + txtEntCd.Text + "' ";
                strMQuery += ",            @pWORKORDER_NO = '" + txtWorkOrderNo.Text + "' ";
                strMQuery += ",            @pITEM_CD = '" + txtItemCd.Text + "' ";
                strMQuery += ",            @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                strMQuery += ",            @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                strMQuery += ",            @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                strMQuery += ",            @pORDER_STATUS = '" + cboStatus.SelectedValue.ToString() + "' ";
                strMQuery += ",            @pORDER_FLAG = '" + cboOrderFlag.SelectedValue.ToString() + "' ";
                strMQuery += ",            @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strMQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 3, true);

                strStatus = "";

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 3);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SubSearch() 작업지시 상세조회
        private void SubSearch(string WoNo)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strMQuery = "";
                strMQuery = "   usp_PCC008 @pTYPE = 'S2'";
                strMQuery += ",            @pWORKORDER_NO = '" + WoNo + "' ";
                strMQuery += ",            @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        //지시상태가 ST이면 ROW 전체 LOCK
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지시상태")].Value.ToString() == "ST"
                            || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지시상태")].Value.ToString() == "CL")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "마일스톤여부") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정검사여부") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정문서") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정규격") + "|3"
                                );
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "마일스톤여부") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정검사여부") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정문서") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정규격") + "|0"
                                );

                            //자원유형이 외주이면 외주관련 필드 필수처리 아니면 읽기전용
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자원유형")].Text == "O")
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "통화") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|0"
                                    );

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통화")].Text == "")
                                {
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통화")].Value = "KRW";
                                }
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Text == "")
                                {
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Value = "A";
                                }
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드_2") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "통화") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|3"
                                    );
                            }
                        }
                    }
                }
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            //그리드 상단 필수 체크
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
                                case "I": strGbn = "I1"; break;
                                case "U": strGbn = "U1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            if (MstWoNo == "")
                            {
                                MessageBox.Show(SystemBase.Base.MessageRtn("B0061", "제조오더번호"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            string strSql = " usp_PCC008 '" + strGbn + "'";
                            strSql += ", @pWORKORDER_NO = '" + MstWoNo + "' ";
                            strSql += ", @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + "' ";
                            strSql += ", @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                            strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                            strSql += ", @pGROUP_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text + "' ";
                            strSql += ", @pWC_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text + "' ";
                            strSql += ", @pJOB_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text + "' ";
                            strSql += ", @pRES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text + "' ";
                            strSql += ", @pPLAN_START_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "착수예정일")].Text + "' ";
                            strSql += ", @pPLAN_START_TM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "예정시간")].Text + "' ";
                            strSql += ", @pPLAN_COMPT_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "완료예정일")].Text + "' ";
                            strSql += ", @pPLAN_COMPT_TM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "완료시간")].Text + "' ";
                            strSql += ", @pBP_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드")].Text + "' ";
                            strSql += ", @pSUBCONTRACT_PRC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가")].Value + "' ";
                            strSql += ", @pSUBCONTRACT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액")].Value + "' ";
                            strSql += ", @pCUR_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통화")].Value + "' ";
                            strSql += ", @pTAX_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Value + "' ";
                            strSql += ", @pROUT_DOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정문서")].Text + "' ";
                            strSql += ", @pROUT_SIZE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정규격")].Text + "' ";
                            strSql += ", @pMILESTONE_FLG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "마일스톤여부")].Text + "' ";
                            strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                            strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
                            strSql += ", @pINSP_FLG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정검사여부")].Text + "' ";

                            strSql += ", @pRUN_TIME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간")].Value + "' ";
                            strSql += ", @pSETUP_TIME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value + "' ";
                            strSql += ", @pRUN_TIME_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량")].Value + "' ";
                            strSql += ", @pMFG_LT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조L/T")].Value + "' ";
                            strSql += ", @pWORK_TM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간")].Value + "' ";
                            strSql += ", @pWORK_TM_LOAD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정부하시수")].Value + "' ";
                            strSql += ", @pORDER_STATUS = '" + Convert.ToString(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지시상태")].Value) + "' ";

                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        }
                    }
                    //ROUT_ORDER UPDATE, 말공정 검사여부 Y, 마일스톤 Y 수정
                    string strSql1 = " usp_PCC008 'C1' ";
                    strSql1 += ", @pWORKORDER_NO = '" + MstWoNo + "' ";
                    strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql1, dbConn, Trans);
                    ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

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
                    SubSearch(MstWoNo);
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

        #region Master그리드 셀클릭
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            MstItemCd = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text;
            strStatus = Convert.ToString(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "지시상태")].Value);
            MstWoNo = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text;

            SubSearch(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text);
        }
        #endregion

        #region 상세그리드 팝업이벤트
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드_2"))
            {
                try
                {
                    string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                    pu.Width = 500;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장명")].Text = Msgs[1].ToString();

                        if (Msgs[0].ToString() == "R009") //외주이면
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text = "F009-G";
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text = "외주";
                        }

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드_2"))
            {
                try
                {
                    string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P001' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정작업 조회");
                    pu.Width = 500;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공정작업 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드_2"))
            {
                try
                {
                    WNDW002 pu = new WNDW002(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드")].Text, "");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처명")].Text = Msgs[2].ToString();

                        string strSql = " usp_MIM519 @pTYPE = 'S4' ";
                        strSql += "                , @pITEM_CD = '" + MstItemCd + "' ";
                        strSql += "                , @pCUST_CD = '" + fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드")].Text + "' ";
                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                        if (dt.Rows.Count > 0)
                        {
                            double po_price = 0, po_Qty = 0;
                            po_price = Convert.ToDouble(dt.Rows[0][0].ToString());
                            if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정오더수량")].Text != "" && fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정오더수량")].Value.ToString() != "0")
                            {
                                po_Qty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정오더수량")].Value.ToString());
                            }

                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가")].Value = po_price;
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액")].Value = po_price * po_Qty;
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가")].Value = 0;
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액")].Value = 0;
                        }

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드_2"))
            {
                try
                {
                    string strQuery = " usp_P_COMMON @pType='P062' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "자원 조회");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원유형")].Text = Msgs[3].ToString();

                        //작업장 세팅
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text
                            = SystemBase.Base.CodeName("RES_CD", "WORKCENTER_CD", "P_RESO_MANAGE", Msgs[0].ToString(), " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장명")].Text
                            = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text, " AND MAJOR_CD = 'P002'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");

                        if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원유형")].Text != "O")
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드")].Text = "";
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처명")].Text = "";
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가")].Value = 0;
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액")].Value = 0;
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "통화")].Text = "";
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Text = "";
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조L/T")].Value = 0;

                            UIForm.FPMake.grdReMake(fpSpread1, e.Row,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "통화") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제조L/T") + "|3"
                                );

                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, e.Row,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "통화") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제조L/T") + "|0"
                                );

                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조L/T")].Value = 0;
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "통화")].Value = "KRW";
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Value = "A";
                        }

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 그리드 체인지 이벤트
        protected override void fpSpread1_ChangeEvent(int Row, int Col)
        {
            if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장명")].Text
                    = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text, " AND MAJOR_CD = 'P002'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text == "R009")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text = "F009-G";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text = "외주";
                }
            }
            else if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업명")].Text
                    = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text, " AND MAJOR_CD = 'P001'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");
            }
            else if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처명")].Text
                    = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                string strSql = " usp_MIM519 @pTYPE = 'S4' ";
                strSql += "                , @pITEM_CD = '" + MstItemCd + "' ";
                strSql += "                , @pCUST_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드")].Text + "' ";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                if (dt.Rows.Count > 0)
                {
                    double po_price = 0, po_Qty = 0;
                    po_price = Convert.ToDouble(dt.Rows[0][0].ToString());
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정오더수량")].Text != "" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정오더수량")].Value.ToString() != "0")
                    {
                        po_Qty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정오더수량")].Value.ToString());
                    }

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가")].Value = po_price;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액")].Value = po_price * po_Qty;
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가")].Value = 0;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액")].Value = 0;
                }
            }
            else if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text
                    = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text, " AND PLANT_CD = '" + SystemBase.Base.gstrPLANT_CD.ToString() + "'  AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원유형")].Text
                    = SystemBase.Base.CodeName("RES_CD", "RES_KIND", "P_RESO_MANAGE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text, " AND PLANT_CD = '" + SystemBase.Base.gstrPLANT_CD.ToString() + "'  AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                //작업장 세팅
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text
                    = SystemBase.Base.CodeName("RES_CD", "WORKCENTER_CD", "P_RESO_MANAGE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장명")].Text
                    = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text, " AND MAJOR_CD = 'P002'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원유형")].Text != "O")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가")].Value = 0;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액")].Value = 0;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "통화")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조L/T")].Value = 0;

                    UIForm.FPMake.grdReMake(fpSpread1, Row,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "통화") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제조L/T") + "|3"
                        );

                }
                else
                {
                    UIForm.FPMake.grdReMake(fpSpread1, Row,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드_2") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "통화") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제조L/T") + "|0"
                        );

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조L/T")].Value = 0;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "통화")].Value = "KRW";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Value = "A";
                }
            }
            else if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "공정검사여부"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "마일스톤여부")].Value.ToString() == "N" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정검사여부")].Value.ToString() == "Y")
                {
                    MessageBox.Show("마일스톤여부가 'N'일 경우 공정검사여부는 'Y'가 될 수 없습니다.");
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정검사여부")].Value = "N";
                    return;
                }
            }
            else if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "마일스톤여부"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "마일스톤여부")].Value.ToString() == "N" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정검사여부")].Value.ToString() == "Y")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정검사여부")].Value = "N";
                    return;
                }
            }
            else if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간"))
            {
                if (Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value) == 0
                    || Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간")].Value) == 0
                    || Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량")].Value) == 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value);
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정부하시수")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value);
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value)
                        + ((Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정오더수량")].Value)
                        * Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간")].Value))
                        / Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량")].Value));
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정부하시수")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value)
                        + ((Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정오더수량")].Value)
                        * Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간")].Value))
                        / Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량")].Value));
                }
            }
            else if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간"))
            {
                if (Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value) == 0
                    || Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간")].Value) == 0
                    || Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량")].Value) == 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value);
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정부하시수")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value);
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value)
                        + ((Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정오더수량")].Value)
                        * Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간")].Value))
                        / Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량")].Value));
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정부하시수")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value)
                        + ((Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정오더수량")].Value)
                        * Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간")].Value))
                        / Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량")].Value));
                }
            }
            else if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량"))
            {
                if (Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value) == 0
                    || Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간")].Value) == 0
                    || Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량")].Value) == 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value);
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정부하시수")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value);
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value)
                        + ((Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정오더수량")].Value)
                        * Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간")].Value))
                        / Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량")].Value));
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정부하시수")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간")].Value)
                        + ((Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정오더수량")].Value)
                        * Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간")].Value))
                        / Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량")].Value));
                }
            }
        }

        #endregion

        #region 부품내역
        private void btnSubItem_Click(object sender, System.EventArgs e)
        {
            if (strProcSeq == "")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0061", "공정순서"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PCC008P1 form = new PCC008P1(strWorkOrderNo, strProcSeq);
            form.ShowDialog();
        }
        #endregion

        #region 상세조회 그리드 클릭시 workorder_no, proc_seq 반환
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                strWorkOrderNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text;
                strProcSeq = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text;
            }
        }
        #endregion
                	
    }
}
