#region 작성정보
/*********************************************************************/
// 단위업무명 : 생산자원관리
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-03-04
// 작성내용 : 생산자원관리 등록 및 조회
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
using SystemBase;

namespace PA.PBA101
{
    public partial class PBA101 : UIForm.FPCOMM2T
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string SaveData = "", SearchData = ""; //컨트롤에 대한 조회후 데이터와 저장시 변경된 데이터 체크위한 변수
        bool NewChk = true;

        #endregion

        #region PBA101
        public PBA101()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void PBA101_Load(object sender, System.EventArgs e)
        {
            Control_Load(); //화면 SETTING
            
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0,0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region 화면 컨트롤 SETTING
        private void Control_Load()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            SystemBase.Validation.GroupBox_Setting(groupBox3);

            //////////////////////////// 콤보박스 SETTING ////////////////////////////////////////////////////////////////////////
            //콤보 데이터
            SystemBase.ComboMake.C1Combo(cboSResKind, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P019', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3, true);	//자원유형
            SystemBase.ComboMake.C1Combo(cboResClass, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P021', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);	//장비분류
            SystemBase.ComboMake.C1Combo(cboOtKind, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B029' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0, true);		//OT구분
            SystemBase.ComboMake.C1Combo(cboStopRemark, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P031' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);		//장비정지사유

            //그리드 콤보데이터
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "자원유형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P019', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P002', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            //기초정보 Setting
            Base_Setting();

            /////////////////////////// 탭 초기화 ///////////////////////////////////////////////////////////////////////////////////
            TabSetting();
        }
        #endregion

        #region Base_Setting
        private void Base_Setting()
        {
            c1DockingTab1.TabPages[0].TabVisible = true;
            c1DockingTab1.TabPages[1].TabVisible = false;
            dtxtMoveCount.Value = 1;

            txtSPlantCd.Value = SystemBase.Base.gstrPLANT_CD;
            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;

            //자원사용여부
            dtpStopDt.Value = null;
            cboStopRemark.Tag = ";2;;";

            dtxtMakePow1.Value = 1;
            dtxtWcRate.Value = 100;

            chkProjectRes.Visible = false;
            chkGroupKind.Visible = false;

            rdoResKind1.Checked = true;

            NewChk = true;
        }
        #endregion

        #region TabSetting
        private void TabSetting()
        {
            UIForm.TabFPMake.TabPageColor(c1DockingTabPage2); //기준정보
            UIForm.TabFPMake.TabPageColor(c1DockingTabPage1); //그룹정보

            c1DockingTabPage1.Visible = false;

            this.c1DockingTab1.SelectedIndex = 0;
        }
        #endregion

        #region RowInsExec() 행추가
        protected override void RowInsExec()
        {	// 행 추가
            try
            {
                if (SystemBase.Base.GroupBoxExceptions(groupBox2))
                {
                    if (c1DockingTab1.TabPages[1].TabVisible == true && c1DockingTab1.SelectedIndex.ToString() == "1")
                    {
                        UIForm.FPMake.RowInsert(fpSpread1);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region DelExec() 행삭제
        protected override void DelExec()
        {	// 행 삭제
            try
            {
                if (SystemBase.Base.GroupBoxExceptions(groupBox2))
                {
                    if (c1DockingTab1.TabPages[1].TabVisible == true && c1DockingTab1.SelectedIndex.ToString() == "1")
                    {
                        UIForm.FPMake.RowRemove(fpSpread1);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region RCopyExec 그리드 Row 복사
        protected override void RCopyExec()
        {
            try
            {
                if (c1DockingTab1.TabPages[1].TabVisible == true && c1DockingTab1.SelectedIndex.ToString() == "1")
                {
                    UIForm.FPMake.RowCopy(fpSpread1);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행복사"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TEXTBOX Change이벤트
        //조회조건 자원
        private void txtSResCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSResCd.Text != "")
                {
                    txtSResDis.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtSResCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSResDis.Value = "";
                }
            }
            catch { }
        }
        //조회조건 작업장
        private void txtSWcCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSWcCd.Text != "")
                {
                    txtSWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtSWcCd.Text, " AND MAJOR_CD = 'P002' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSWcNm.Value = "";
                }
            }
            catch { }
        }
        //조회조건 공장
        private void txtSPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSPlantCd.Text != "")
                {
                    txtSPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtSPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSPlantNm.Value = "";
                }
            }
            catch { }
        }
        //대체자원
        private void txtChangeResCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtChangeResCd.Text != "")
                {
                    txtChangeResNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtChangeResCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtChangeResNm.Value = "";
                }
            }
            catch { }
        }
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
            catch { }
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
            catch { }
        }
        //보조작업장
        private void txtSubWcCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSubWcCd.Text != "")
                {
                    txtSubWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtSubWcCd.Text, " AND MAJOR_CD = 'P061'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSubWcNm.Value = "";
                }
            }
            catch { }
        }
        //자원코드
        private void txtResCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtResCd.Text != "")
                {
                    txtResDis.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtResCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtResDis.Value = "";
                }
            }
            catch { }
        }
        //기준자원
        private void txtBaseResCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtBaseResCd.Text != "")
                {
                    txtBaseResNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtBaseResCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtBaseResNm.Value = "";
                }
            }
            catch { }
        }

        //기계자원 주 작업자
        private void txtMainWorkDutyCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtMainWorkDutyCd.Text != "")
                {
                    txtMainWorkDutyNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtMainWorkDutyCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtMainWorkDutyNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region 팝업창 열기
        //조회조건 자원
        private void btnSRes_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P049' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtSResCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "자원 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSResCd.Text = Msgs[0].ToString();
                    txtSResDis.Value = Msgs[1].ToString();
                    txtSResCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //조회조건 작업장
        private void btnSWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtSWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSWcCd.Text = Msgs[0].ToString();
                    txtSWcNm.Value = Msgs[1].ToString();
                    txtSWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //조회조건 공장
        private void btnSPlant_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P011' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };											  // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtSPlantCd.Text, "" };											  // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회");

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtSPlantCd.Text = Msgs[0].ToString();
                    txtSPlantNm.Value = Msgs[1].ToString();
                    txtSPlantCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //대체자원
        private void btnChangeRes_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P056' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtChangeResCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "대체자원 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtChangeResCd.Text = Msgs[0].ToString();
                    txtChangeResNm.Value = Msgs[1].ToString();
                    txtChangeResCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대체자원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //공장
        private void btnPlant_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P011' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };											  // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtPlantCd.Text, "" };											  // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회");

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
        //작업장
        private void btnWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
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
        //보조작업장
        private void btnSubWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P061' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "보조작업장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSubWcCd.Text = Msgs[0].ToString();
                    txtSubWcNm.Value = Msgs[1].ToString();
                    txtSubWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "보조작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //기준자원
        private void btnBaseRes_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P056' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtBaseResCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "기준자원 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBaseResCd.Text = Msgs[0].ToString();
                    txtBaseResNm.Value = Msgs[1].ToString();
                    txtBaseResCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "기준자원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //기계자원 주 작업자
        private void btnMainWorkDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P057' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtMainWorkDutyCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "주 작업자 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMainWorkDutyCd.Value = Msgs[0].ToString();
                    txtMainWorkDutyNm.Value = Msgs[1].ToString();
                    txtMainWorkDutyCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "기계자원 주 작업자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            //화면 로드시 필수 체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox3);

            Base_Setting();

            txtPlantCd.Tag = "공장코드;1;true;;";
            txtPlantCd.BackColor = SystemBase.Validation.Kind_LightCyan;
            txtPlantCd.ReadOnly = false;

            txtResCd.Tag = "자원코드;1;true;;";
            txtResCd.BackColor = SystemBase.Validation.Kind_LightCyan;
            txtResCd.ReadOnly = false;

            txtResDis.Tag = "자원명;1;true;;";
            txtResDis.BackColor = SystemBase.Validation.Kind_LightCyan;
            txtResDis.ReadOnly = false;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1)) //필수체크
            {
                try
                {
                    string strUseYn = "Y"; //사용여부
                    if (rdoSUseYn2.Checked == true) { strUseYn = ""; }

                    string strQuery = " usp_PBA101  @pTYPE = 'S1', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPLANT_CD = '" + txtSPlantCd.Text + "' ";
                    strQuery += ", @pRES_CD = '" + txtSResCd.Text + "' ";
                    strQuery += ", @pWC_CD = '" + txtSWcCd.Text + "' ";
                    strQuery += ", @pRES_KIND = '" + cboSResKind.SelectedValue.ToString() + "' ";
                    strQuery += ", @pRES_USE_YN = '" + strUseYn + "' ";

                    //그리드 Binding
                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            GroupBox[] gBox = null;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2)
                && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3))
            {
                //컨트롤 체크값 초기화
                SaveData = "";
                //컨트롤 체크 함수
                gBox = new GroupBox[] { groupBox2, groupBox3};
                SystemBase.Validation.Control_Check(gBox, ref SaveData);

                //기존 컨트롤 데이터와 현재 컨트롤 데이터 비교
                if (SearchData == SaveData && UIForm.FPMake.HasSaveData(fpSpread1) == false)
                {
                    //변경되거나 처리할 데이터가 없습니다.
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    return;
                }

                string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.
                string strSResCd = "", strResCd = "";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    /*######################################## 자원등록 ###########################################################*/

                    string strResKind = "L"; //개인
                    if (rdoResKind2.Checked == true) { strResKind = "M"; }
                    else if (rdoResKind3.Checked == true) { strResKind = "O"; }
                    else if (rdoResKind4.Checked == true) { strResKind = "G"; }
                    else { strResKind = "L"; }

                    string strMainRes = "N"; //주자원여부
                    if (rdoMainRes1.Checked == true) { strMainRes = "Y"; }

                    string strMainOt = "N"; //주OT여부
                    if (rdoMainOt1.Checked == true) { strMainOt = "Y"; }

                    string strAutoReport = "0"; //자동보고서
                    if (chkAutoReport.Checked == true) { strAutoReport = "1"; }

                    string strProjectRes = "0"; //스케쥴 자원별
                    if (chkProjectRes.Checked == true) { strProjectRes = "1"; }

                    string strGroupKind = "0"; //그룹구분(1:기계/0:인적)
                    if (chkGroupKind.Checked == true) { strGroupKind = "1"; }

                    string strReflection = "0"; //이동시간및 최소이동수량 반영
                    if (chkReflection.Checked == true) { strReflection = "1"; }

                    string strUseYn = "Y"; //사용여부
                    if (rdoUseYn2.Checked == true) { strUseYn = "N"; }

                    string strNightYn = "N"; //야간여부
                    if (chkNightFlag.Checked == true) { strNightYn = "Y"; }

                    string strUseRemark = "";
                    if (cboStopRemark.SelectedValue.ToString() != "") { strUseRemark = cboStopRemark.SelectedValue.ToString(); }

                    strSResCd = txtResCd.Text;

                    string strSql = " usp_PBA101 @pTYPE = 'I1' ";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql += ", @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "' ";
                    strSql += ", @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                    strSql += ", @pRES_CD = '" + txtResCd.Text + "' ";
                    strSql += ", @pRES_DIS = '" + txtResDis.Text + "' ";
                    strSql += ", @pRES_KIND = '" + strResKind + "' ";
                    strSql += ", @pMAKE_POW1 = '" + dtxtMakePow1.Value + "' ";
                    strSql += ", @pMAKE_POW2 = '" + dtxtMakePow2.Value + "' ";
                    strSql += ", @pMAKE_POW3 = '" + dtxtMakePow3.Value + "' ";
                    strSql += ", @pCHANGE_RES = '" + txtChangeResCd.Text + "' ";
                    strSql += ", @pCHANGE_MODU = '" + dtxtChangeModu.Value + "' ";
                    strSql += ", @pAUTO_REPORT = '" + strAutoReport + "' ";
                    strSql += ", @pPROG_RES = '" + strProjectRes + "' ";
                    strSql += ", @pMOVETIME = '" + dtxtMoveTime.Value + "' ";
                    strSql += ", @pMOVECOUNT = '" + dtxtMoveCount.Value + "' ";
                    strSql += ", @pREFLECTION = '" + strReflection + "' ";
                    strSql += ", @pMAXPOWER = '" + dtxtMaxPower.Value + "' ";
                    strSql += ", @pMINPOWER = '" + dtxtMinPower.Value + "' ";
                    strSql += ", @pWC_RATE = '" + dtxtWcRate.Value + "' ";
                    strSql += ", @pRES_CLASS = '" + cboResClass.SelectedValue.ToString() + "' ";
                    strSql += ", @pRES_COUNT = '" + dtxtResCount.Value + "' ";
                    strSql += ", @pOT_KIND = '" + cboOtKind.SelectedValue.ToString() + "' ";
                    strSql += ", @pMAIN_RES_YN = '" + strMainRes + "' ";
                    strSql += ", @pMAIN_OT_YN = '" + strMainOt + "' ";
                    strSql += ", @pWC_CD = '" + txtWcCd.Text + "' ";
                    strSql += ", @pRES_USE_YN = '" + strUseYn + "' ";
                    strSql += ", @pUSE_STOP_REMARK = '" + strUseRemark + "' ";
                    strSql += ", @pUSE_STOP_DT = '" + dtpStopDt.Text + "' ";
                    strSql += ", @pBASE_RES_CD = '" + txtBaseResCd.Text + "' ";
                    strSql += ", @pMAIN_WORK_DUTY = '" + txtMainWorkDutyCd.Text + "' ";
                    strSql += ", @pGROUP_KIND = '" + strGroupKind + "' ";
                    strSql += ", @pSUB_WC_CD = '" + txtSubWcCd.Text + "' ";
                    strSql += ", @pNIGHT_FLAG = '" + strNightYn + "' ";

                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    /*######################################## 그룹 자원등록 #######################################################*/

                    if (c1DockingTab1.TabPages[1].TabVisible == true)
                    {
                        //그리드 상단 필수 체크
                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
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
                                        case "U": strGbn = "U2"; break;
                                        case "I": strGbn = "I2"; break;
                                        case "D": strGbn = "D2"; break;
                                        default: strGbn = ""; break;
                                    }

                                    string strGUseYn = "0";//사용여부
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용여부")].Text == "True") { strGUseYn = "1"; }

                                    strResCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text;

                                    string strSubSql = " usp_PBA101 '" + strGbn + "'";
                                    strSubSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                    strSubSql += ", @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "' ";
                                    strSubSql += ", @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                                    strSubSql += ", @pGRES_CD = '" + txtResCd.Text + "'";
                                    strSubSql += ", @pRES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text + "'";
                                    strSubSql += ", @pSEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순서")].Text + "'";
                                    strSubSql += ", @pUSE_YN = '" + strGUseYn + "'";
                                    strSubSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                                    DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSubSql, dbConn, Trans);
                                    ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                                }
                            }
                        }
                        else
                        {
                            Trans.Rollback();
                            this.Cursor = Cursors.Default;
                            return;
                        }
                    }

                    Trans.Commit();
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
                    if (NewChk == true) { SearchExec(); }
                    else { Right_Search(); }

                    //컨트롤 체크값 초기화
                    SearchData = "";
                    //컨트롤 체크 함수
                    gBox = new GroupBox[] { groupBox2, groupBox3};
                    SystemBase.Validation.Control_Check(gBox, ref SearchData);

                    //그리드 셀 포커스 이동
                    UIForm.FPMake.GridSetFocus(fpSpread2, strSResCd, SystemBase.Base.GridHeadIndex(GHIdx2, "자원코드"));
                    UIForm.FPMake.GridSetFocus(fpSpread1, strResCd, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드"));
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (ERRCode == "ER") //ERROR
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else   //ERRCode == "WR" WARING
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region DeleteExec() 데이타 삭제 로직
        protected override void DeleteExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                if (MessageBox.Show(SystemBase.Base.MessageRtn("SY010"), "삭제", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strSql = " usp_PBA101 'D1'";
                        strSql += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
                        strSql += ", @pRES_CD = '" + txtResCd.Text + "'";
                        strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        Trans.Commit();
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
                        NewExec();
                        SearchExec();
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

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 좌측그리드 방향키 이동 및 클릭시 우측조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            Right_Search();

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void Right_Search()
        {
            try
            {
                //컨트롤 체크값 초기화
                SearchData = "";

                //같은 Row 조회 되지 않게
                int intRow = fpSpread2.ActiveSheet.ActiveRowIndex;
                if (intRow < 0)
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }

                if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }

                //c1DockingTab1.SelectedIndex = 0;

                txtPlantCd.Value = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공장")].Text;
                txtResCd.Value = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "자원코드")].Text;

                string strQuery = " usp_PBA101  @pTYPE = 'S2'";
                strQuery += ", @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                strQuery += ", @pRES_CD = '" + txtResCd.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable Dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (Dt.Rows.Count > 0)
                {
                    NewChk = false;

                    /*########################### 기본 ###########################################*/
                    //자원유형
                    if (Dt.Rows[0]["RES_KIND"].ToString() == "M") { rdoResKind2.Checked = true; }
                    else if (Dt.Rows[0]["RES_KIND"].ToString() == "O") { rdoResKind3.Checked = true; }
                    else if (Dt.Rows[0]["RES_KIND"].ToString() == "G") { rdoResKind4.Checked = true; }
                    else { rdoResKind1.Checked = true; }

                    //생산능력
                    dtxtMakePow1.Value = Dt.Rows[0]["MAKE_POW1"];
                    dtxtMakePow2.Value = Dt.Rows[0]["MAKE_POW2"];
                    dtxtMakePow3.Value = Dt.Rows[0]["MAKE_POW3"];
                    if (Dt.Rows[0]["NIGHT_FLAG"].ToString() == "Y")
                    { chkNightFlag.Checked = true; }
                    else
                    { chkNightFlag.Checked = false; }

                    //대체자원
                    txtChangeResCd.Value = Dt.Rows[0]["CHANGE_RES"].ToString();
                    dtxtChangeModu.Value = Dt.Rows[0]["CHANGE_MODU"];

                    //기계자원 주 작업자
                    txtMainWorkDutyCd.Value = Dt.Rows[0]["MAIN_WORK_DUTY"].ToString();

                    //사용여부
                    if (Dt.Rows[0]["USE_YN"].ToString() == "Y") { rdoUseYn1.Checked = true; }
                    else
                    {
                        rdoUseYn2.Checked = true;
                        if (Dt.Rows[0]["USE_STOP_DT"].ToString() != "" && Dt.Rows[0]["USE_STOP_DT"] != null)
                        {
                            dtpStopDt.Value = Dt.Rows[0]["USE_STOP_DT"].ToString();
                        }
                        else
                        {
                            dtpStopDt.Text = "";
                        }

                        if (Dt.Rows[0]["USE_STOP_REMARK"].ToString() != "")
                        {
                            cboStopRemark.SelectedValue = Dt.Rows[0]["USE_STOP_REMARK"].ToString();
                        }
                        else
                        {
                            cboStopRemark.Text = "";
                        }
                    }

                    //기타정보
                    if (Dt.Rows[0]["RES_CLASS"].ToString().Trim() != "") { cboResClass.SelectedValue = Dt.Rows[0]["RES_CLASS"].ToString(); }
                    else { cboResClass.Text = ""; }
                    dtxtResCount.Value = Dt.Rows[0]["RES_COUNT"];
                    cboOtKind.SelectedValue = Dt.Rows[0]["OT_KIND"].ToString();
                    dtxtWcRate.Value = Dt.Rows[0]["WORKCENTER_RATE"];
                    txtWcCd.Value = Dt.Rows[0]["WORKCENTER_CD"].ToString();
                    txtSubWcCd.Value = Dt.Rows[0]["SUB_WC_CD"].ToString();

                    if (Dt.Rows[0]["MAIN_RES_YN"].ToString() == "Y") { rdoMainRes1.Checked = true; }
                    else { rdoMainRes2.Checked = true; }
                    if (Dt.Rows[0]["MAIN_OT_YN"].ToString() == "Y") { rdoMainOt1.Checked = true; }
                    else { rdoMainOt2.Checked = true; }

                    txtBaseResCd.Value = Dt.Rows[0]["BASE_RES_CD"].ToString();

                    chkAutoReport.Checked = Convert.ToBoolean(Dt.Rows[0]["AUTO_REPORT"].ToString());
                    chkProjectRes.Checked = Convert.ToBoolean(Dt.Rows[0]["PROG_RES"].ToString());
                    chkGroupKind.Checked = Convert.ToBoolean(Dt.Rows[0]["GROUP_KIND"].ToString());

                    dtxtMoveTime.Value = Dt.Rows[0]["MOVE_TIME"];
                    dtxtMoveCount.Value = Dt.Rows[0]["MOVE_COUNT"];
                    dtxtMaxPower.Value = Dt.Rows[0]["MAX_POWER"];
                    dtxtMinPower.Value = Dt.Rows[0]["MIN_POWER"];
                    chkReflection.Checked = Convert.ToBoolean(Dt.Rows[0]["REFLECTION"].ToString());

                    /*########################### 그룹 ###########################################*/
                    if (Dt.Rows[0]["RES_KIND"].ToString() == "G") //그룹이면
                    {
                        UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                        c1DockingTab1.TabPages[1].TabVisible = true;
                        chkProjectRes.Visible = true;
                        chkGroupKind.Visible = true;
                        GroupResSearch(txtPlantCd.Text, txtResCd.Text);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0;
                        c1DockingTab1.TabPages[1].TabVisible = false;
                        chkProjectRes.Visible = false;
                        chkGroupKind.Visible = false;
                    }
                }
                else
                {
                    //그룹박스 초기화
                    SystemBase.Validation.GroupBox_Reset(groupBox2);
                    SystemBase.Validation.GroupBox_Reset(groupBox3);
                }

                //현재 row값 설정
                PreRow = fpSpread2.Sheets[0].ActiveRowIndex;

                //키값 컨트롤 읽기전용으로 셋팅
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2);
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox3);

                //this.c1DockingTab1.SelectedIndex = 0;

                //컨트롤 체크값 초기화
                SearchData = "";
                //컨트롤 체크 함수
                GroupBox[] gBox = new GroupBox[] { groupBox2, groupBox3};
                SystemBase.Validation.Control_Check(gBox, ref SearchData);
            }
            catch(Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 그룹 자원 그리드 조회
        private void GroupResSearch(string strPlantCd, string strGresCd)
        {
            try
            {
                string GQuery = "";
                GQuery += " usp_PBA101 @pTYPE = 'S3' ";
                GQuery += ", @pPLANT_CD = '" + strPlantCd + "' ";
                GQuery += ", @pGRES_CD = '" + strGresCd + "' ";
                GQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, GQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    UIForm.FPMake.grdReMake(fpSpread1, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드_2") + "|3");
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 그리드 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            //자원코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드_2"))
            {
                try
                {
                    string strQuery = " usp_P_COMMON @pType='P056' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "자원 조회");	//자원조회
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text = Msgs[1].ToString();
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

        #region 그리드 상 데이터 변경시 연계데이터 자동입력
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            //품목코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text
                    = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' AND PLANT_CD = '"+ SystemBase.Base.gstrPLANT_CD.ToString() +"' ");
            }
        }
        #endregion

        #region 라디오버튼 체크 이벤트
        //사용여부 
        private void rdoUseYn1_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoUseYn2.Checked == true)
            {
                dtpStopDt.ReadOnly = false;
                dtpStopDt.Text = SystemBase.Base.ServerTime("YYMMDD");
                dtpStopDt.Tag = "기준일자;1;;";
                dtpStopDt.BackColor = SystemBase.Validation.Kind_LightCyan;

                cboStopRemark.Enabled = true;
                cboStopRemark.Tag = "정지사유;1;;";
                SystemBase.ComboMake.C1Combo(cboStopRemark, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P031', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0, true);		//장비정지사유
                cboStopRemark.EditorBackColor = SystemBase.Validation.Kind_LightCyan;
            }
            else
            {
                dtpStopDt.Value = null;
                dtpStopDt.Tag = ";2;;";
                dtpStopDt.BackColor = SystemBase.Validation.Kind_Gainsboro;
                dtpStopDt.ReadOnly = true;

                SystemBase.ComboMake.C1Combo(cboStopRemark, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P031' ", 9, true);		//장비정지사유
                cboStopRemark.Tag = ";2;;";
                cboStopRemark.EditorBackColor = SystemBase.Validation.Kind_Gainsboro;
                cboStopRemark.Enabled = false;
            }
        }
        //자원유형 개인 선택시
        private void rdoResKind1_CheckedChanged(object sender, System.EventArgs e)
        {
            chkProjectRes.Visible = false;
            chkGroupKind.Visible = false;
            chkProjectRes.Checked = false;
            chkGroupKind.Checked = false;

            c1DockingTab1.TabPages[1].TabVisible = false;
            fpSpread1.Sheets[0].Rows.Count = 0;
        }

        //자원유형 기계 선택시
        private void rdoResKind2_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoResKind2.Checked == true)
            {
                txtMainWorkDutyCd.Tag = "";
                txtMainWorkDutyCd.BackColor = SystemBase.Validation.Kind_White;
                txtMainWorkDutyCd.ReadOnly = false;
                btnMainWorkDuty.Enabled = true;
            }
            else
            {
                txtMainWorkDutyCd.Value = "";

                txtMainWorkDutyCd.Tag = ";2;;";
                txtMainWorkDutyCd.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtMainWorkDutyCd.ReadOnly = true;
                btnMainWorkDuty.Enabled = false;
            }

            chkProjectRes.Visible = false;
            chkGroupKind.Visible = false;
            chkProjectRes.Checked = false;
            chkGroupKind.Checked = false;

            c1DockingTab1.TabPages[1].TabVisible = false;
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        //자원유형 외주 선택시
        private void rdoResKind3_CheckedChanged(object sender, System.EventArgs e)
        {
            chkProjectRes.Visible = false;
            chkGroupKind.Visible = false;
            chkProjectRes.Checked = false;
            chkGroupKind.Checked = false;

            c1DockingTab1.TabPages[1].TabVisible = false;
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        //자원유형 그룹 선택시
        private void rdoResKind4_CheckedChanged(object sender, System.EventArgs e)
        {
            chkProjectRes.Visible = true;
            chkGroupKind.Visible = true;

            c1DockingTab1.TabPages[1].TabVisible = true;
        }
        #endregion

    }
}
