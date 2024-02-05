#region 작성정보
/*********************************************************************/
// 단위업무명 : 공장별 품목정보등록
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-01-31
// 작성내용 : 공장별 품목정보등록 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;


namespace BI.BBI002
{
    public partial class BBI002 : UIForm.FPCOMM1
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string Data = "", SaveData = "", SearchData = ""; //컨트롤에 대한 조회후 데이터와 저장시 변경된 데이터 체크위한 변수
        TreeNode docCategory = new TreeNode("root");
        //2번쨰그리드변수
        public string[] G2Head1 = null;// 첫번째 Head Text
        public string[] G2Head2 = null;// 두번째 Head Text
        public string[] G2Head3 = null;// 세번째 Head Text
        public int[] G2Width = null;// Cell 넓이
        public string[] G2Align = null;// Cell 데이타 정렬방식
        public string[] G2Type = null;// CellType 지정
        public int[] G2Color = null;// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
        public string[] G2Etc = null;// Mask 양식 등
        public int G2HeadCnt = 0;   // Head 수
        public int[] G2SEQ = null;// 키


        private System.Windows.Forms.ContextMenu ctmGrid2;
        public FarPoint.Win.Spread.FpSpread fpSpread2;
        public FarPoint.Win.Spread.SheetView fpSpread2_Sheet1;

        #endregion

        #region BBI002
        public BBI002()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBI002_Load(object sender, System.EventArgs e)
        {
            Control_Load(); //화면 SETTING

            FPCOMM2_Load();

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

            txtHLotYn.Value = "";

        }
        #endregion

        #region 화면 컨트롤 SETTING
        private void Control_Load()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            SystemBase.Validation.GroupBox_Setting(groupBox3);
            SystemBase.Validation.GroupBox_Setting(groupBox4);
            SystemBase.Validation.GroupBox_Setting(groupBox5);
            SystemBase.Validation.GroupBox_Setting(groupBox6);
            SystemBase.Validation.GroupBox_Setting(groupBox7);

            //////////////////////////// 콤보박스 SETTING ////////////////////////////////////////////////////////////////////////
            //조회
            SystemBase.ComboMake.C1Combo(cboSItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 3);   //품목계정
            SystemBase.ComboMake.C1Combo(cboSPlant, "usp_B_COMMON @pTYPE = 'PLANT', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ");	                    //공장

            //입력
            SystemBase.ComboMake.C1Combo(cboPlant, "usp_B_COMMON @pTYPE = 'PLANT', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ");	//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");	        //품목계정
            SystemBase.ComboMake.C1Combo(cboItemType, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B011', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", true); //조달구분
            SystemBase.ComboMake.C1Combo(cboMaterialType, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D035', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 9);	    //자재구분
            SystemBase.ComboMake.C1Combo(cboDpgb, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B029', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 9);	        //단품구분			
            SystemBase.ComboMake.C1Combo(cboIssuedMthd, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B030', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");	        //출고방법
            SystemBase.ComboMake.C1Combo(cboRcptMthd, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B030', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 9);	    //반제품입고방법
            SystemBase.ComboMake.C1Combo(cboIssuedUnit, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");	        //출고단위
            SystemBase.ComboMake.C1Combo(cboABCFlag, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B031', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", true);		//ABC구분
            SystemBase.ComboMake.C1Combo(cboPriceGbn, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B032', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");		    //단가구분
            SystemBase.ComboMake.C1Combo(cboProdEnv, "usp_B_COMMON @pTYPE='COMM', @pCODE ='B041', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", true);		//생산계획
            SystemBase.ComboMake.C1Combo(cboOrderFrom, "usp_B_COMMON @pTYPE='COMM2', @pCODE ='B019', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");	        //오더생성구분
            SystemBase.ComboMake.C1Combo(cboLotSizing, "usp_B_COMMON @pTYPE='COMM2', @pCODE ='B022', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", true);	//LOT SIZING
            SystemBase.ComboMake.C1Combo(cboOrderMfgUnit, "usp_B_COMMON @pTYPE='COMM2', @pCODE ='Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");	    //제조오더단위
            SystemBase.ComboMake.C1Combo(cboOrderPurUnit, "usp_B_COMMON @pTYPE='COMM2', @pCODE ='Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");	    //구매단위
            SystemBase.ComboMake.C1Combo(cboStockUnit, "usp_B_COMMON @pTYPE='COMM', @pCODE='Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");		    //재고단위
            SystemBase.ComboMake.C1Combo(cboFinalInspFlag, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'Q013',@pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", true);   //최종검사레벨

            //////////////////////////// 초기값 셋팃 /////////////////////////////////////////////////////////////////////////
            //조회
            cboSPlant.SelectedValue = SystemBase.Base.gstrPLANT_CD.ToString();

            //품목정보
            this.tabForms.SelectedIndex = 0;

            cboPlant.SelectedValue = SystemBase.Base.gstrPLANT_CD.ToString();
            dtpUseDateFr.Text = "2000-01-01";
            dtpUseDateTo.Text = "2999-12-31";
            dtpBomUseDateFr.Value = "2000-01-01";
            dtpBomUseDateTo.Value = "2999-12-31";
            optTracking2.Checked = true;
            rdoNo.Checked = true;
            rdoStdItemN.Checked = true;         // 2017.03.17. hma 추가: 표준품목여부에서 '아니오'를 기본값으로 지정
            pnlStdItemYN.Enabled = false;       // 2017.03.17. hma 추가: 표준품목여부 항목 비활성화

            //계획정보
            this.tabForms.SelectedIndex = 1;

            rdoMpsFlag1.Checked = true;
            rdoOrderFlag1.Checked = true;

            //재고정보
            this.tabForms.SelectedIndex = 2;

            rdoLotNo2.Checked = true;

            cboIssuedMthd.SelectedValue = "M";
            cboStockUnit.SelectedValue = "EA";

            //오더정보
            this.tabForms.SelectedIndex = 3;

            rdoSngRoutFlag2.Checked = true;
            cboOrderMfgUnit.SelectedValue = "EA";
            cboOrderPurUnit.SelectedValue = "EA";

            //원가정보
            this.tabForms.SelectedIndex = 4;

            //원가정보 2013-04-22 추가
            SystemBase.ComboMake.C1Combo(cboMassProd, "usp_B_COMMON @pTYPE='COMM2', @pCODE = 'B060', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");		//양산구분
            SystemBase.ComboMake.C1Combo(cboCostItemAcct, "usp_B_COMMON @pTYPE='COMM2', @pCODE = 'B061', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");	//통합원가부품구분(계정)
            SystemBase.ComboMake.C1Combo(cboPurType, "usp_B_COMMON @pTYPE='COMM2', @pCODE = 'B062', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");		    //구매구분
            SystemBase.ComboMake.C1Combo(cboSpecType, "usp_B_COMMON @pTYPE='COMM2', @pCODE = 'B063', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");		//규격화구분
            SystemBase.ComboMake.C1Combo(cboDnnpDrawType, "usp_B_COMMON @pTYPE='COMM2', @pCODE = 'B064', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");	//국방도면종류
            SystemBase.ComboMake.C1Combo(cboDnnpAppn, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B029', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");			//방산물자지정여부
            SystemBase.ComboMake.C1Combo(cboPrescripYn, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B029', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");		//시효구분
            SystemBase.ComboMake.C1Combo(cboEsdYn, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B029', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");			//ESD구분
            SystemBase.ComboMake.C1Combo(cboMslYn, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B029', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");			//MSL구분
            SystemBase.ComboMake.C1Combo(cboWeightUnit, "usp_B_COMMON @pTYPE='COMM2', @pCODE = 'Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");	    //중량단위
            SystemBase.ComboMake.C1Combo(cboBulkUnit, "usp_B_COMMON @pTYPE='COMM2', @pCODE = 'Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");		//부피단위
            SystemBase.ComboMake.C1Combo(cboListupYn, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B029', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");			//재고번호확인(목록화여부)

            /////////////////////////// 탭 초기화 ///////////////////////////////////////////////////////////////////////////////////
            TabSetting();

            // 2017.06.27. hma 추가(Start): 로그인ID가 'ADMIN'인 경우 BOM유효기간 수정할 수 있도록 처리. 가끔씩 일자가 1900-01-01로 들어가는 경우 있어서.
            if (SystemBase.Base.gstrUserID == "ADMIN")
            {
                dtpBomUseDateFr.ReadOnly = false;
                dtpBomUseDateTo.ReadOnly = false;
            }
            // 2017.06.27. hma 추가(End)
        }
        #endregion

        #region TabSetting
        private void TabSetting()
        {
            UIForm.TabFPMake.TabPageColor(tabPage1); //품목정보
            UIForm.TabFPMake.TabPageColor(tabPage2); //계획정보
            UIForm.TabFPMake.TabPageColor(tabPage3); //재고정보
            UIForm.TabFPMake.TabPageColor(tabPage4); //오더정보
            UIForm.TabFPMake.TabPageColor(tabPage5); //원가더정보
            UIForm.TabFPMake.TabPageColor(tabpage6);

            this.tabForms.SelectedIndex = 0;
        }
        #endregion

        #region 팝업창 열기
        //품목코드
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW001 pu = new WNDW.WNDW001(txtItemCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[1].ToString();
                    txtItemNm.Value = Msgs[2].ToString();
                    cboStockUnit.SelectedValue = Msgs[6].ToString();

                    txtDrawNo.Value = Msgs[9].ToString();
                    txtDrawRev.Value = Msgs[15].ToString();
                    dtpDrawRevDate.Value = Msgs[16].ToString();

                    if (txtItemNm.Text != "" && txtItemFullNm.Text == "")
                    {
                        txtItemFullNm.Text = txtItemNm.Text;
                    }
                    txtItemCd.Focus();

                    string strQuery2 = "usp_BBI002 'S4'";
                    strQuery2 = strQuery2 + ", @pITEM_CD ='" + "" + "' ";
                    strQuery2 = strQuery2 + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery2, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);

                    // 조회후 그리드셀값 조정
                    FarPoint.Win.Spread.SheetView sheet = fpSpread1.Sheets[0];
                    int colDeptCd = SystemBase.Base.GridHeadIndex(GHIdx1, "부서");
                    int colSectCd = SystemBase.Base.GridHeadIndex(GHIdx1, "분류");

                    fpSpread2.ActiveSheet.Columns[1].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                    fpSpread2.ActiveSheet.Columns[2].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //입고창고
        private void btnSlCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_S_COMMON 'S020', @pSPEC1='" + cboPlant.SelectedValue.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSlCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "입고창고 조회");
                pu.Width = 600;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSlCd.Value = Msgs[0].ToString();
                    txtSlNm.Value = Msgs[1].ToString();
                    txtIssuedSlCd.Value = Msgs[0].ToString();
                    txtIssuedSlNm.Value = Msgs[1].ToString();
                    txtRcptLocCd.Value = Msgs[2].ToString();
                    txtIssuedLocCd.Value = Msgs[2].ToString();
                    txtSlCd.Focus();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "입고창고 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //출고창고
        private void btnIssuedSlCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_S_COMMON 'S020', @pSPEC1='" + cboPlant.SelectedValue.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtIssuedSlCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "출고창고 조회");
                pu.Width = 600;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtIssuedSlCd.Value = Msgs[0].ToString();
                    txtIssuedSlNm.Value = Msgs[1].ToString();
                    txtIssuedLocCd.Value = Msgs[2].ToString();
                    txtIssuedSlCd.Focus();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "출고창고 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //구매조직
        private void btnPurDept_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'M001', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDept.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BBI002P1", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매조직 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurDept.Value = Msgs[0].ToString();
                    txtPurDeptNm.Value = Msgs[1].ToString();
                    txtPurDept.Focus();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매조직 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업장
        private void btnWcCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE = 'COMM_POP', @pSPEC1 = 'P002', @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCd.Value = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                    txtWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            //화면 로드시 필수 체크
            SystemBase.Base.GroupBoxReset(groupBox1);
            SystemBase.Base.GroupBoxReset(groupBox2);
            SystemBase.Base.GroupBoxReset(groupBox3);
            SystemBase.Base.GroupBoxReset(groupBox4);
            SystemBase.Base.GroupBoxReset(groupBox5);
            SystemBase.Base.GroupBoxReset(groupBox6);

            Control_Load();

            //if (cboItemAcct.SelectedValue.ToString() == "10" || cboItemAcct.SelectedValue.ToString() == "20")
            //{
            //    dtpBomUseDateFr.Tag = "BOM유효기간FROM;1;;";
            //    dtpBomUseDateTo.Tag = "BOM유효기간TO;1;;";
            //}

            txtHLotYn.Value = "";

            txtItemCd.Focus();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1)) //필수체크
            {
                string strQuery = " usp_BBI002  'S1'";
                strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
                strQuery = strQuery + ", @pITEM_CD ='" + txtSItemCd.Text.Trim() + "' ";
                string strAcct = ""; if (cboSItemAcct.Text != "") strAcct = cboSItemAcct.SelectedValue.ToString();
                strQuery = strQuery + ", @pITEM_ACCT ='" + strAcct + "' ";
                string strSPlant = ""; if (cboSPlant.Text != "") strSPlant = cboSPlant.SelectedValue.ToString();
                strQuery = strQuery + ", @pPLANT_CD ='" + strSPlant + "' ";
                strQuery = strQuery + ", @pITEM_NM ='" + txtSItemNm.Text + "' ";
                strQuery = strQuery + ", @pDRAW_NO ='" + txtSDrawNo.Text + "' ";
                strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            GroupBox[] gBox = null;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3)
                && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox4)
                && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox5)
                && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox6)
                && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox7)
                && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox8)
                && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox9))
            {
                //컨트롤 체크값 초기화
                SaveData = "";
                //컨트롤 체크 함수
                gBox = new GroupBox[] { groupBox3, groupBox4, groupBox5, groupBox6, groupBox7, groupBox8, groupBox9 };
                SystemBase.Validation.Control_Check(gBox, ref SaveData);

                if (txtHLotYn.Text == "N" && rdoLotNo1.Checked == true)
                {
                    DialogResult Rtn = MessageBox.Show("LOT NO 추적 대상으로 지정 할 경우 기초 LOT 재고가 생성 됩니다. \n\n신중하게 선택 하십시요. \n\nLOT 재고정보를 생성 하시겠습니까?", "LOT 재고 생성", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (Rtn != DialogResult.Yes)
                    {
                        this.Cursor = Cursors.Default;
                        return;
                    }
                }
                if (txtHLotYn.Text == "Y" && rdoLotNo2.Checked == true)
                {
                    DialogResult Rtn = MessageBox.Show("LOT NO 추적 대상에서 해제하면 LOT 재고가 초기화 됩니다. \n\n신중하게 선택 하십시요. \n\nLOT 재고정보를 삭제 하시겠습니까?", "LOT 재고 삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (Rtn != DialogResult.Yes)
                    {
                        this.Cursor = Cursors.Default;
                        return;
                    }
                }

                string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                if ((SearchData != SaveData || (SystemBase.Validation.FPGrid_SaveCheck(fpSpread2, this.Name, "fpSpread2", true) == true)))// 그리드 필수항목 체크 
                {
                    try
                    {
                        string strSql = " usp_BBI002 'U1' ";
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                        strSql += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                        strSql += ", @pITEM_CD = '" + txtItemCd.Text.ToUpper().Trim() + "'";
                        strSql += ", @pITEM_NM = '" + txtItemNm.Text + "'";
                        strSql += ", @pITEM_FULL_NM = '" + txtItemFullNm.Text + "'";

                        if (cboPlant.Text != "") strSql += ", @pPLANT_CD = '" + cboPlant.SelectedValue.ToString() + "'";
                        if (cboItemAcct.Text != "") strSql += ", @pITEM_ACCT = '" + cboItemAcct.SelectedValue.ToString() + "'";
                        if (cboItemType.Text != "") strSql += ", @pITEM_TYPE = '" + cboItemType.SelectedValue.ToString() + "'";
                        if (cboMaterialType.Text != "") strSql += ", @pMATERIAL_TYPE = '" + cboMaterialType.SelectedValue.ToString() + "'";
                        strSql += ", @pUSE_DATE_FR= '" + dtpUseDateFr.Text + "'";
                        strSql += ", @pUSE_DATE_TO= '" + dtpUseDateTo.Text + "'";
                        //bom 유효일자 추가
                        strSql += ", @pBOM_USE_DATE_FR= '" + dtpBomUseDateFr.Text + "'";
                        strSql += ", @pBOM_USE_DATE_TO= '" + dtpBomUseDateTo.Text + "'";

                        string strTracking = "N"; if (optTracking1.Checked == true) strTracking = "Y";
                        strSql += ", @pTRACKING_FLAG = '" + strTracking + "'";
                        strSql += ", @pSL_CD = '" + txtSlCd.Text.Trim() + "'";
                        strSql += ", @pRCPT_LOCATION_CD = '" + txtRcptLocCd.Text.Trim() + "'";
                        strSql += ", @pISSUED_SL_CD = '" + txtIssuedSlCd.Text.Trim() + "'";
                        strSql += ", @pISSUED_LOCATION_CD = '" + txtIssuedLocCd.Text.Trim() + "'";
                        if (cboIssuedMthd.Text != "") strSql += ", @pISSUED_MTHD = '" + cboIssuedMthd.SelectedValue.ToString() + "'";
                        if (cboRcptMthd.Text != "") strSql += ", @pRCPT_MTHD = '" + cboRcptMthd.SelectedValue.ToString() + "'";
                        if (cboIssuedUnit.Text != "") strSql += ", @pISSUED_UNIT = '" + cboIssuedUnit.SelectedValue.ToString() + "'";
                        int CyclyCntPerd = 0; if (dtxtCyclyCntPerd.Text != "") CyclyCntPerd = Convert.ToInt32(dtxtCyclyCntPerd.Text.ToString());
                        strSql += ", @pCYCLE_CNT_PERD = '" + CyclyCntPerd + "'";
                        if (cboABCFlag.Text != "") strSql += ", @pABC_FLAG = '" + cboABCFlag.SelectedValue.ToString() + "'";
                        if (cboPriceGbn.Text != "") strSql += ", @pPRICE_GBN = '" + cboPriceGbn.SelectedValue.ToString() + "'";
                        double StdPrice = 0; if (dtxtStdPrice.Text != "") StdPrice = Convert.ToDouble(dtxtStdPrice.Text.ToString());
                        strSql += ", @pSTD_PRICE = '" + StdPrice + "'";
                        double LastMonthPrice = 0; if (dtxtLastMonthPrice.Text != "") LastMonthPrice = Convert.ToDouble(dtxtLastMonthPrice.Text.ToString());
                        strSql += ", @pLAST_MONTH_PRICE = '" + LastMonthPrice + "'";
                        double MoveAvgPrice = 0; if (dtxtMoveAvgPrice.Text != "") MoveAvgPrice = Convert.ToDouble(dtxtMoveAvgPrice.Text.ToString());
                        strSql += ", @pMOVE_AVG_PRICE = '" + MoveAvgPrice + "'";
                        if (cboStockUnit.Text != "") strSql += ", @pSTOCK_UNIT = '" + cboStockUnit.SelectedValue.ToString() + "'";
                        if (cboProdEnv.Text != "") strSql += ", @pPROD_ENV = '" + cboProdEnv.SelectedValue.ToString() + "'";
                        string strMpsFlag = "N"; if (rdoMpsFlag1.Checked == true) strMpsFlag = "Y";
                        strSql += ", @pMPS_FLAG = '" + strMpsFlag.Trim() + "'";
                        string strOrderFlag = "N"; if (rdoOrderFlag1.Checked == true) strOrderFlag = "Y";
                        strSql += ", @pORDER_FLAG = '" + strOrderFlag + "'";
                        if (cboOrderFrom.Text != "") strSql += ", @pORDER_FROM = '" + cboOrderFrom.SelectedValue.ToString() + "'";
                        double ReorderPnt = 0; if (dtxtReorderPnt.Text != "") ReorderPnt = Convert.ToDouble(dtxtReorderPnt.Text.ToString());
                        strSql += ", @pREORDER_PNT = '" + ReorderPnt + "'";
                        if (cboLotSizing.Text != "") strSql += ", @pLOT_SIZING = '" + cboLotSizing.SelectedValue.ToString() + "'";
                        int RoundPerd = 0; if (dtxtRoundPerd.Text != "") RoundPerd = Convert.ToInt32(dtxtRoundPerd.Text.ToString());
                        strSql += ", @pROUND_PERD = '" + RoundPerd + "'";
                        if (cboOrderMfgUnit.Text != "") strSql += ", @pORDER_MFG_UNIT = '" + cboOrderMfgUnit.SelectedValue.ToString() + "'";
                        int OrderMfgLt = 0; if (dtxtOrderMfgLt.Text != "") OrderMfgLt = Convert.ToInt32(dtxtOrderMfgLt.Text.ToString());
                        strSql += ", @pORDER_MFG_LT = '" + OrderMfgLt + "'";
                        if (cboOrderPurUnit.Text != "") strSql += ", @pORDER_PUR_UNIT = '" + cboOrderPurUnit.SelectedValue.ToString() + "'";
                        int OrderPurLt = 0; if (dtxtOrderPurLt.Text != "") OrderPurLt = Convert.ToInt32(dtxtOrderPurLt.Text.ToString());
                        strSql += ", @pORDER_PUR_LT = '" + OrderPurLt + "'";
                        strSql += ", @pREORG_ID = '" + SystemBase.Base.gstrREORG_ID.ToString() + "'";
                        strSql += ", @pPUR_ORG = '" + txtPurDept.Text + "'";
                        string strProdInspFlag = "N"; if (chkProdInspFlag.Checked == true) strProdInspFlag = "Y";
                        strSql += ", @pPROD_INSP_FLAG = '" + strProdInspFlag + "'";
                        string strRecvInspFlag = "N"; if (chkRecvInspFlag.Checked == true) strRecvInspFlag = "Y";
                        strSql += ", @pRECV_INSP_FLAG = '" + strRecvInspFlag + "'";
                        strSql += ", @pFINAL_INSP_FLAG = '" + cboFinalInspFlag.SelectedValue.ToString() + "'";
                        string strShipInspFlag = "N"; if (chkShipInspFlag.Checked == true) strShipInspFlag = "Y";
                        strSql += ", @pSHIP_INSP_FLAG = '" + strShipInspFlag + "'";
                        strSql += ", @pITEM_SPEC = '" + txtItemSpec.Text + "'";
                        strSql += ", @pDRAW_NO = '" + txtDrawNo.Text + "'";
                        double MinMrpQty = 0; if (dtxtMinMrpQty.Text != "") MinMrpQty = Convert.ToDouble(dtxtMinMrpQty.Text.ToString());
                        strSql += ", @pMIN_MRP_QTY	= '" + MinMrpQty + "'";
                        double FixMrpQty = 0; if (dtxtFixMrpQty.Text != "") FixMrpQty = Convert.ToDouble(dtxtFixMrpQty.Text.ToString());
                        strSql += ", @pFIX_MRP_QTY	= '" + FixMrpQty + "'";
                        double RoundQty = 0; if (dtxtRoundQty.Text != "") RoundQty = Convert.ToDouble(dtxtRoundQty.Text.ToString());
                        strSql += ", @pROUND_QTY	= '" + RoundQty + "'";
                        double SafetyQty = 0; if (dtxtSafetyQty.Text != "") SafetyQty = Convert.ToDouble(dtxtSafetyQty.Text.ToString());
                        strSql += ", @pSAFETY_QTY	= '" + SafetyQty + "'";
                        double MfgScrapQty = 0; if (dtxtMfgScrapQty.Text != "") MfgScrapQty = Convert.ToDouble(dtxtMfgScrapQty.Text.ToString());
                        strSql += ", @pMFG_SCRAP_QTY= '" + MfgScrapQty + "'";
                        double PurScrapQty = 0; if (dtxtPurScrapQty.Text != "") PurScrapQty = Convert.ToDouble(dtxtPurScrapQty.Text.ToString());
                        strSql += ", @pPUR_SCRAP_QTY= '" + PurScrapQty + "'";
                        string strSng_Rout_Flag = "N"; if (rdoSngRoutFlag1.Checked == true) strSng_Rout_Flag = "Y";
                        strSql += ", @pSNG_ROUT_FLAG = '" + strSng_Rout_Flag.Trim() + "'";
                        strSql += ", @pWORK_CENTER = '" + txtWcCd.Text + "'";
                        strSql += ", @pQUALITY_FIG_NO = '" + txtQualityFigNo.Text + "'";
                        strSql += ", @pNIIN = '" + txtNiin.Text + "'";
                        strSql += ", @pDPGB = '" + cboDpgb.SelectedValue.ToString() + "'";
                        string strGovernmentFlag = "N"; if (rdoYes.Checked == true) strGovernmentFlag = "Y";
                        strSql += ", @pGOVERNMENT_FLAG = '" + strGovernmentFlag + "'";
                        strSql += ", @pITEM_JJ = '" + txtItemJj.Text + "'";
                        strSql += ", @pMILITARY_SPEC = '" + txtMilitarySpec.Text + "'";
                        strSql += ", @pTB_PIC_NO = '" + txtTbPicNo.Text + "'";
                        strSql += ", @pEXAM_TYPE = '" + txtExamType.Text + "'";
                        strSql += ", @pSET_NM    = '" + txtSetNm.Text + "'";

                        //2013-03-11 원가정보,품목영문명 추가
                        strSql += ", @pFSC = '" + txtFsc.Text + "'";
                        strSql += ", @pMTMG_NUMB = '" + txtMtmgNumb.Text + "'";
                        if (cboMassProd.Text != "") strSql += ", @pMASS_PROD = '" + cboMassProd.SelectedValue.ToString() + "'";
                        if (cboCostItemAcct.Text != "") strSql += ", @pCOST_ITEM_ACCT = '" + cboCostItemAcct.SelectedValue.ToString() + "'";
                        if (cboDnnpAppn.Text != "") strSql += ", @pDNNP_APPN = '" + cboDnnpAppn.SelectedValue.ToString() + "'";
                        strSql += ", @pDNNP_AUTHORITY = '" + txtDnnpAuthority.Text + "'";
                        if (cboPurType.Text != "") strSql += ", @pPUR_TYPE = '" + cboPurType.SelectedValue.ToString() + "'";
                        if (cboPrescripYn.Text != "") strSql += ", @pPRESCRIP_YN = '" + cboPrescripYn.SelectedValue.ToString() + "'";
                        if (cboEsdYn.Text != "") strSql += ", @pESD_YN = '" + cboEsdYn.SelectedValue.ToString() + "'";
                        if (cboMslYn.Text != "") strSql += ", @pMSL_YN = '" + cboMslYn.SelectedValue.ToString() + "'";
                        strSql += ", @pITEM_NM_CODE = '" + txtItemNmCode.Text + "'";
                        strSql += ", @pITEM_IDENTIFY_CODE = '" + txtItemIdentifyCode.Text + "'";
                        if (cboSpecType.Text != "") strSql += ", @pSPEC_TYPE = '" + cboSpecType.SelectedValue.ToString() + "'";
                        if (cboDnnpDrawType.Text != "") strSql += ", @pDNNP_DRAW_TYPE = '" + cboDnnpDrawType.SelectedValue.ToString() + "'";
                        strSql += ", @pDNNP_DRAW_ITEM_NO = '" + txtDnnpDrawItemNo.Text + "'";
                        strSql += ", @pMNG_EMP_NO = '" + txtMngEmpNo.Text + "'";
                        strSql += ", @pSPEC_NO = '" + txtSpecNo.Text + "'";
                        strSql += ", @pSPEC_ITEM_NO = '" + txtSpecItemNo.Text + "'";
                        strSql += ", @pSUEN_ITEM_NO = '" + txtSuenItemNo.Text + "'";
                        strSql += ", @pSUEN_ITEM_NM = '" + txtSuenItemNm.Text + "'";
                        strSql += ", @pSUEN_BINO = '" + txtSuenBino.Text + "'";
                        strSql += ", @pSUEN_NM = '" + txtSuenNm.Text + "'";
                        strSql += ", @pSUEN_MATL_MARK = '" + txtSuenMatlMark.Text + "'";
                        strSql += ", @pSUEN_SPEC = '" + txtSuenSpec.Text + "'";
                        strSql += ", @pMAIN_ITEM_NM = '" + txtMainItemNm.Text + "'";
                        strSql += ", @pASSY_NM = '" + txtAssyNm.Text + "'";
                        double dblWeight = 0; if (dtxtWeight.Text != "") dblWeight = Convert.ToDouble(dtxtWeight.Text.ToString());
                        strSql += ", @pWEIGHT = '" + dblWeight + "'";
                        double dblBulk = 0; if (dtxtBulk.Text != "") dblBulk = Convert.ToDouble(dtxtBulk.Text.ToString());
                        strSql += ", @pBULK = '" + dblBulk + "'";
                        if (cboWeightUnit.Text != "") strSql += ", @pWEIGHT_UNIT = '" + cboWeightUnit.SelectedValue.ToString() + "'";
                        if (cboBulkUnit.Text != "") strSql += ", @pBULK_UNIT = '" + cboBulkUnit.SelectedValue.ToString() + "'";
                        strSql += ", @pSPECIFICATION = '" + txtSpecification.Text + "'";
                        strSql += ", @pITEM_NM_ENG = '" + txtItemNmEng.Text + "'";
                        if (cboListupYn.Text != "") strSql += ", @pLISTUP_YN = '" + cboListupYn.SelectedValue.ToString() + "'";
                        strSql += ", @pNON_ITEM_NM = '" + txtNonItemNm.Text + "'";
                        strSql += ", @pNON_ITEM_NM_ENG = '" + txtNonItemNmEng.Text + "'";
                        strSql += ", @pSLMS_NO = '" + txtSlmsNo.Text + "'";

                        //Lot, Serial 정보
                        string strLotYn = "N";
                        string strSerialYn = "N";
                        if (rdoLotNo1.Checked)
                            strLotYn = "Y";
                        if (rdoSerialFlag1.Checked)
                            strSerialYn = "Y";

                        strSql = strSql + ", @pLOT_YN = '" + strLotYn + "'";//Lot 체크
                        strSql = strSql + ", @pSERIAL_NO_YN = '" + strSerialYn + "'";//Serial 체크


                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                        DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        
                        ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        txtHLotYn.Value = strLotYn;

                        for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text;
                            string strGbn = "";
                            if (strHead.Length > 0)
                            {
                                switch (strHead)
                                {
                                    case "I": strGbn = "I1"; break;
                                    default: strGbn = ""; break;
                                }

                                string strSectCd = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "분류")].Text.ToString();
                                string strDocNm = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "문서명")].Text.ToString();
                                string strDocCd = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "문서코드")].Text.ToString();
                                string strYn = "N";

                                if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "필수")].Text == "True")
                                { strYn = "Y"; }

                                string strSql2 = " usp_TDA003 '" + strGbn + "'";
                                strSql2 = strSql2 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";//법인코드
                                strSql2 = strSql2 + ", @pTARGET_KEY = '" + txtItemCd.Text + "'";//품목코드
                                strSql2 = strSql2 + ", @pPLANT_CD = '" + cboPlant.SelectedValue.ToString() + "'";
                                strSql2 = strSql2 + ", @pTARGET_TYPE = '" + "I" + "'";//품목키
                                strSql2 = strSql2 + ", @pDOC_CD = '" + strDocCd.Trim().ToUpper() + "'";//문서코드
                                strSql2 = strSql2 + ", @pDOC_REQ_YN = '" + strYn.Trim().ToUpper() + "'";//필수
                                strSql2 = strSql2 + ", @pREG_ID = '" + SystemBase.Base.gstrUserID + "'";//사용자ID

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql2, dbConn, Trans);

                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }

                        Trans.Commit();
                        //저장후 다시 조회
                        //int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                        //string strItemCd = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;

                        string strQuery2 = "usp_BBI002 'S4'";
                        strQuery2 = strQuery2 + ", @pITEM_CD ='" + txtItemCd.Text + "' ";
                        strQuery2 = strQuery2 + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                        UIForm.FPMake.grdCommSheet(fpSpread2, strQuery2, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                       
                        int colDeptCd = SystemBase.Base.GridHeadIndex(GHIdx1, "부서");
                        int colSectCd = SystemBase.Base.GridHeadIndex(GHIdx1, "분류");
                                        
                        

                        fpSpread2.ActiveSheet.Columns[1].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                        fpSpread2.ActiveSheet.Columns[2].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;


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
                        SearchExec();

                        //컨트롤 체크변수 초기화
                        SearchData = "";
                        //컨트롤 체크 함수
                        gBox = new GroupBox[] { groupBox3, groupBox4, groupBox5, groupBox6, groupBox7, groupBox8, groupBox9 };
                        SystemBase.Validation.Control_Check(gBox, ref SearchData);

                        //그리드 포커스 셋팅
                        UIForm.FPMake.GridSetFocus(fpSpread1, txtItemCd.Text, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"));

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

        }
        #endregion

        #region DeleteExec() 데이타 삭제 로직
        protected override void DeleteExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (txtItemCd.Text != "")
            {
                string msg = SystemBase.Base.MessageRtn("B0035");//품목마스터도 함께 삭제 하시겠습니까?
                DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strSql = " usp_BBI002  'D2'";
                        strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                        strSql = strSql + ", @pLANG_CD  = '" + SystemBase.Base.gstrLangCd + "'";
                        strSql = strSql + ", @pITEM_CD  = '" + txtItemCd.Text.Trim() + "'";
                        string strPlant1 = ""; if (cboPlant.Text != "") strPlant1 = cboPlant.SelectedValue.ToString();
                        strSql = strSql + ", @pPLANT_CD  = '" + strPlant1 + "'";
                        strSql = strSql + ", @pUP_ID  = '" + SystemBase.Base.gstrUserID + "'";

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
                        SearchExec();
                        SystemBase.Validation.GroupBox_Reset(groupBox2); //그룹박스 리셋
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
        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            Right_Search();

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void Right_Search()
        {
            try
            {
                //같은 Row 조회 되지 않게
                int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

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


                string strItemCd = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;

                //상세조회 SQL
                string strPlant = cboPlant.SelectedValue.ToString();
                string strSql = " usp_BBI002  'S2' ";
                strSql = strSql + ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                strSql = strSql + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "'";
                strSql = strSql + ", @pITEM_CD = '" + strItemCd + "'";
                strSql = strSql + ", @pPLANT_CD = '" + strPlant + "'";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                string strQuery2 = "usp_BBI002 'S4'";
                strQuery2 = strQuery2 + ", @pITEM_CD ='" + strItemCd + "' ";
                strQuery2 = strQuery2 + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery2, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);

                fpSpread2.ActiveSheet.Columns[1].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                fpSpread2.ActiveSheet.Columns[2].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    tabForms.SelectedIndex = 0;
                    cboPlant.SelectedValue = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드")].Text.ToString();
                    txtItemCd.Value = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.ToString();
                    txtItemFullNm.Value = ds.Tables[0].Rows[0]["ITEM_FULL_NM"].ToString();
                    if (ds.Tables[0].Rows[0]["ITEM_ACCT"].ToString() != "") cboItemAcct.SelectedValue = ds.Tables[0].Rows[0]["ITEM_ACCT"];
                    if (ds.Tables[0].Rows[0]["ITEM_TYPE"].ToString() != "") cboItemType.SelectedValue = ds.Tables[0].Rows[0]["ITEM_TYPE"];
                    //if (ds.Tables[0].Rows[0]["MATERIAL_TYPE"].ToString() != "") 
                    cboMaterialType.SelectedValue = ds.Tables[0].Rows[0]["MATERIAL_TYPE"];
                    txtItemSpec.Value = ds.Tables[0].Rows[0]["ITEM_SPEC"].ToString();
                    dtpUseDateFr.Value = ds.Tables[0].Rows[0]["USE_DATE_FR"].ToString();
                    dtpUseDateTo.Value = ds.Tables[0].Rows[0]["USE_DATE_TO"].ToString();
                    //BOM유효일자 추가
                    dtpBomUseDateFr.Value = ds.Tables[0].Rows[0]["BOM_USE_DATE_FR"].ToString();
                    dtpBomUseDateTo.Value = ds.Tables[0].Rows[0]["BOM_USE_DATE_TO"].ToString();

                    if (ds.Tables[0].Rows[0]["STD_ITEM_YN"].ToString() == "Y") rdoStdItemY.Checked = true;
                    else rdoStdItemN.Checked = true;

                    if (ds.Tables[0].Rows[0]["TRACKING_FLAG"].ToString() == "Y") optTracking1.Checked = true;
                    else optTracking2.Checked = true;
                    txtSlCd.Value = ds.Tables[0].Rows[0]["SL_CD"].ToString();
                    txtRcptLocCd.Value = ds.Tables[0].Rows[0]["RCPT_LOCATION_CD"].ToString();
                    txtIssuedSlCd.Value = ds.Tables[0].Rows[0]["ISSUED_SL_CD"].ToString();
                    txtIssuedLocCd.Value = ds.Tables[0].Rows[0]["ISSUED_LOCATION_CD"].ToString();
                    if (ds.Tables[0].Rows[0]["ISSUED_MTHD"].ToString() != "") cboIssuedMthd.SelectedValue = ds.Tables[0].Rows[0]["ISSUED_MTHD"];
                    if (ds.Tables[0].Rows[0]["RCPT_MTHD"].ToString() != "") cboRcptMthd.SelectedValue = ds.Tables[0].Rows[0]["RCPT_MTHD"];
                    if (ds.Tables[0].Rows[0]["ISSUED_UNIT"].ToString() != "") cboIssuedUnit.SelectedValue = ds.Tables[0].Rows[0]["ISSUED_UNIT"];
                    if (ds.Tables[0].Rows[0]["LOT_YN"].ToString() == "Y") rdoLotNo1.Checked = true;
                    dtxtCyclyCntPerd.Value = ds.Tables[0].Rows[0]["CYCLE_CNT_PERD"];
                    if (ds.Tables[0].Rows[0]["ABC_FLAG"].ToString() != "") cboABCFlag.SelectedValue = ds.Tables[0].Rows[0]["ABC_FLAG"];
                    if (ds.Tables[0].Rows[0]["PRICE_GBN"].ToString() != "") cboPriceGbn.SelectedValue = ds.Tables[0].Rows[0]["PRICE_GBN"];
                    dtxtStdPrice.Value = ds.Tables[0].Rows[0]["STD_PRICE"];
                    dtxtLastMonthPrice.Value = ds.Tables[0].Rows[0]["LAST_MONTH_PRICE"];
                    dtxtMoveAvgPrice.Value = ds.Tables[0].Rows[0]["MOVE_AVG_PRICE"];
                    if (ds.Tables[0].Rows[0]["PROD_ENV"].ToString() != "") cboProdEnv.SelectedValue = ds.Tables[0].Rows[0]["PROD_ENV"];
                    if (ds.Tables[0].Rows[0]["MPS_FLAG"].ToString() == "Y") rdoMpsFlag1.Checked = true;
                    if (ds.Tables[0].Rows[0]["ORDER_FLAG"].ToString() == "Y") rdoOrderFlag1.Checked = true;
                    if (ds.Tables[0].Rows[0]["ORDER_FROM"].ToString() != "") cboOrderFrom.SelectedValue = ds.Tables[0].Rows[0]["ORDER_FROM"];
                    dtxtReorderPnt.Value = ds.Tables[0].Rows[0]["REORDER_PNT"];
                    if (ds.Tables[0].Rows[0]["LOT_SIZING"].ToString() != "") cboLotSizing.SelectedValue = ds.Tables[0].Rows[0]["LOT_SIZING"].ToString();
                    dtxtRoundPerd.Value = ds.Tables[0].Rows[0]["ROUND_PERD"];
                    if (ds.Tables[0].Rows[0]["ORDER_MFG_UNIT"].ToString() != "") cboOrderMfgUnit.SelectedValue = ds.Tables[0].Rows[0]["ORDER_MFG_UNIT"];
                    dtxtOrderMfgLt.Value = ds.Tables[0].Rows[0]["ORDER_MFG_LT"];
                    if (ds.Tables[0].Rows[0]["ORDER_PUR_UNIT"].ToString() != "") cboOrderPurUnit.SelectedValue = ds.Tables[0].Rows[0]["ORDER_PUR_UNIT"];
                    dtxtOrderPurLt.Value = ds.Tables[0].Rows[0]["ORDER_PUR_LT"];
                    dtxtOrderPurLtWeek.Value = ds.Tables[0].Rows[0]["ORDER_PUR_LT_WEEK"]; //2021.04.13. ksh 추가 : 구매오더L/T(주)
                    txtPurDept.Value = ds.Tables[0].Rows[0]["PUR_ORG"].ToString();
                    if (ds.Tables[0].Rows[0]["PROD_INSP_FLAG"].ToString() == "Y") chkProdInspFlag.Checked = true;
                    else chkProdInspFlag.Checked = false;
                    if (ds.Tables[0].Rows[0]["RECV_INSP_FLAG"].ToString() == "Y") chkRecvInspFlag.Checked = true;
                    else chkRecvInspFlag.Checked = false;
                    cboFinalInspFlag.SelectedValue = ds.Tables[0].Rows[0]["FINAL_INSP_FLAG"].ToString();
                    if (ds.Tables[0].Rows[0]["SHIP_INSP_FLAG"].ToString() == "Y") chkShipInspFlag.Checked = true;
                    else chkShipInspFlag.Checked = false;
                    if (ds.Tables[0].Rows[0]["STOCK_UNIT"].ToString() != "") cboStockUnit.SelectedValue = ds.Tables[0].Rows[0]["STOCK_UNIT"];
                    txtMilitarySpec.Value = ds.Tables[0].Rows[0]["MILITARY_SPEC"];
                    dtxtMinMrpQty.Value = ds.Tables[0].Rows[0]["MIN_MRP_QTY"];
                    dtxtFixMrpQty.Value = ds.Tables[0].Rows[0]["FIX_MRP_QTY"];
                    dtxtRoundQty.Value = ds.Tables[0].Rows[0]["ROUND_QTY"];
                    dtxtSafetyQty.Value = ds.Tables[0].Rows[0]["SAFETY_QTY"];
                    dtxtMfgScrapQty.Value = ds.Tables[0].Rows[0]["MFG_SCRAP_RATE"];
                    dtxtPurScrapQty.Value = ds.Tables[0].Rows[0]["PUR_SCRAP_RATE"];
                    if (ds.Tables[0].Rows[0]["SNG_ROUT_FLAG"].ToString() == "Y") rdoSngRoutFlag1.Checked = true;
                    txtWcCd.Value = ds.Tables[0].Rows[0]["WORK_CENTER"].ToString();
                    txtQualityFigNo.Value = ds.Tables[0].Rows[0]["QUALITY_FIG_NO"].ToString();

                    txtSetNm.Value = ds.Tables[0].Rows[0]["SET_NM"].ToString(); // 뭉치명
                    txtExamType.Value = ds.Tables[0].Rows[0]["EXAM_TYPE"].ToString(); // 검사 구분
                    txtTbPicNo.Value = ds.Tables[0].Rows[0]["TB_PIC_NO"].ToString(); // 교범그림번호

                    if (ds.Tables[0].Rows[0]["DPGB"].ToString() != "")
                        cboDpgb.SelectedValue = ds.Tables[0].Rows[0]["DPGB"].ToString();

                    txtItemJj.Value = ds.Tables[0].Rows[0]["ITEM_JJ"].ToString();

                    if (ds.Tables[0].Rows[0]["GOVERNMENT_FLAG"].ToString() == "Y") rdoYes.Checked = true;
                    else rdoNo.Checked = true;

                    dtpBomUseDateFr.Value = ds.Tables[0].Rows[0]["VALID_FROM_DT"].ToString();
                    dtpBomUseDateTo.Value = ds.Tables[0].Rows[0]["VALID_TO_DT"].ToString();

                    //원가정보 2013-03-11 추가
                    txtFsc.Value = ds.Tables[0].Rows[0]["FSC"].ToString();
                    txtNiin.Value = ds.Tables[0].Rows[0]["NIIN"].ToString();
                    txtMtmgNumb.Value = ds.Tables[0].Rows[0]["MTMG_NUMB"].ToString();
                    if (ds.Tables[0].Rows[0]["MASS_PROD"].ToString() != "")
                    {
                        cboMassProd.SelectedValue = ds.Tables[0].Rows[0]["MASS_PROD"];
                    }
                    if (ds.Tables[0].Rows[0]["COST_ITEM_ACCT"].ToString() != "")
                    {
                        cboCostItemAcct.SelectedValue = ds.Tables[0].Rows[0]["COST_ITEM_ACCT"];
                    }
                    if (ds.Tables[0].Rows[0]["DNNP_APPN"].ToString() != "")
                    {
                        cboDnnpAppn.SelectedValue = ds.Tables[0].Rows[0]["DNNP_APPN"];
                    }
                    txtDnnpAuthority.Value = ds.Tables[0].Rows[0]["DNNP_AUTHORITY"].ToString();
                    if (ds.Tables[0].Rows[0]["PUR_TYPE"].ToString() != "")
                    {
                        cboPurType.SelectedValue = ds.Tables[0].Rows[0]["PUR_TYPE"];
                    }
                    if (ds.Tables[0].Rows[0]["PRESCRIP_YN"].ToString() != "")
                    {
                        cboPrescripYn.SelectedValue = ds.Tables[0].Rows[0]["PRESCRIP_YN"];
                    }
                    if (ds.Tables[0].Rows[0]["ESD_YN"].ToString() != "")
                    {
                        cboEsdYn.SelectedValue = ds.Tables[0].Rows[0]["ESD_YN"];
                    }
                    if (ds.Tables[0].Rows[0]["MSL_YN"].ToString() != "")
                    {
                        cboMslYn.SelectedValue = ds.Tables[0].Rows[0]["MSL_YN"];
                    }
                    txtItemNmCode.Value = ds.Tables[0].Rows[0]["ITEM_NM_CODE"].ToString();
                    txtItemIdentifyCode.Value = ds.Tables[0].Rows[0]["ITEM_IDENTIFY_CODE"].ToString();
                    if (ds.Tables[0].Rows[0]["SPEC_TYPE"].ToString() != "")
                    {
                        cboSpecType.SelectedValue = ds.Tables[0].Rows[0]["SPEC_TYPE"];
                    }
                    if (ds.Tables[0].Rows[0]["DNNP_DRAW_TYPE"].ToString() != "")
                    {
                        cboDnnpDrawType.SelectedValue = ds.Tables[0].Rows[0]["DNNP_DRAW_TYPE"];
                    }
                    txtDnnpDrawItemNo.Value = ds.Tables[0].Rows[0]["DNNP_DRAW_ITEM_NO"].ToString();
                    txtMngEmpNo.Value = ds.Tables[0].Rows[0]["MNG_EMP_NO"].ToString();
                    txtSpecNo.Value = ds.Tables[0].Rows[0]["SPEC_NO"].ToString();
                    txtSpecItemNo.Value = ds.Tables[0].Rows[0]["SPEC_ITEM_NO"].ToString();
                    txtSuenItemNo.Value = ds.Tables[0].Rows[0]["SUEN_ITEM_NO"].ToString();
                    txtSuenItemNm.Value = ds.Tables[0].Rows[0]["SUEN_ITEM_NM"].ToString();
                    txtSuenBino.Value = ds.Tables[0].Rows[0]["SUEN_BINO"].ToString();
                    txtSuenNm.Value = ds.Tables[0].Rows[0]["SUEN_NM"].ToString();
                    txtSuenMatlMark.Value = ds.Tables[0].Rows[0]["SUEN_MATL_MARK"].ToString();
                    txtSuenSpec.Value = ds.Tables[0].Rows[0]["SUEN_SPEC"].ToString();
                    txtMainItemNm.Value = ds.Tables[0].Rows[0]["MAIN_ITEM_NM"].ToString();
                    txtAssyNm.Value = ds.Tables[0].Rows[0]["ASSY_NM"].ToString();
                    dtxtWeight.Value = ds.Tables[0].Rows[0]["WEIGHT"];
                    dtxtBulk.Value = ds.Tables[0].Rows[0]["BULK"];
                    if (ds.Tables[0].Rows[0]["WEIGHT_UNIT"].ToString() != "")
                    {
                        cboWeightUnit.SelectedValue = ds.Tables[0].Rows[0]["WEIGHT_UNIT"];
                    }
                    if (ds.Tables[0].Rows[0]["BULK_UNIT"].ToString() != "")
                    {
                        cboBulkUnit.SelectedValue = ds.Tables[0].Rows[0]["BULK_UNIT"];
                    }
                    txtSpecification.Value = ds.Tables[0].Rows[0]["SPECIFICATION"].ToString();
                    txtDrawNo.Value = ds.Tables[0].Rows[0]["DRAW_NO"].ToString();
                    txtDrawRev.Value = ds.Tables[0].Rows[0]["DRAW_REV"].ToString();
                    dtpDrawRevDate.Value = ds.Tables[0].Rows[0]["DRAW_REV_DATE"].ToString();

                    txtItemNmEng.Value = ds.Tables[0].Rows[0]["ITEM_NM_ENG"].ToString(); //영문품명

                    if (ds.Tables[0].Rows[0]["LISTUP_YN"].ToString() != "")
                    {
                        cboListupYn.SelectedValue = ds.Tables[0].Rows[0]["LISTUP_YN"];
                    }
                    txtNonItemNm.Value = ds.Tables[0].Rows[0]["NON_ITEM_NM"].ToString(); //영문품명
                    txtNonItemNmEng.Value = ds.Tables[0].Rows[0]["NON_ITEM_NM_ENG"].ToString(); //영문품명
                    txtSlmsNo.Value = ds.Tables[0].Rows[0]["SLMS_NO"].ToString(); //영문품명

                    //LOT, SERIAL체크여부 확인 부분
                    rdoLotNo2.Checked = true;
                    rdoSerialFlag2.Checked = true;
                    txtHLotYn.Value = "N";

                    if (ds.Tables[0].Rows[0]["LOT_YN"].ToString() == "1")
                    {
                        rdoLotNo1.Checked = true;
                        txtHLotYn.Value = "Y";
                    }
                    if (ds.Tables[0].Rows[0]["SERIAL_NO_YN"].ToString() == "1")
                    {
                        rdoSerialFlag1.Checked = true;
                    }

                    //컨트롤 체크값 초기화
                    SearchData = "";
                    //컨트롤 체크 함수
                    GroupBox[] gBox = new GroupBox[] { groupBox3, groupBox4, groupBox5, groupBox6, groupBox7 };
                    SystemBase.Validation.Control_Check(gBox, ref SearchData);

                    if (cboItemAcct.SelectedValue.ToString() == "10" || cboItemAcct.SelectedValue.ToString() == "20")
                    {
                        dtpBomUseDateFr.Tag = "BOM유효기간FROM;1;;";
                        dtpBomUseDateTo.Tag = "BOM유효기간TO;1;;";
                        dtpBomUseDateFr.ReadOnly = false;
                        dtpBomUseDateTo.ReadOnly = false;
                        dtpBomUseDateFr.BackColor = SystemBase.Validation.Kind_LightCyan;
                        dtpBomUseDateTo.BackColor = SystemBase.Validation.Kind_LightCyan;
                    }
                    else
                    {
                        dtpBomUseDateFr.Tag = ";2;;";
                        dtpBomUseDateTo.Tag = ";2;;";
                        dtpBomUseDateFr.BackColor = SystemBase.Validation.Kind_Gainsboro;
                        dtpBomUseDateTo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                        dtpBomUseDateFr.ReadOnly = true;
                        dtpBomUseDateTo.ReadOnly = true;
                        dtpBomUseDateFr.Value = "";
                        dtpBomUseDateTo.Value = "";
                    }
                }
                else
                {
                    //그룹박스 초기화
                    SystemBase.Validation.GroupBox_Reset(groupBox3);
                    SystemBase.Validation.GroupBox_Reset(groupBox4);
                    SystemBase.Validation.GroupBox_Reset(groupBox5);
                    SystemBase.Validation.GroupBox_Reset(groupBox6);
                    SystemBase.Validation.GroupBox_Reset(groupBox7);
                }

                //현재 row값 설정
                PreRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                //키값 컨트롤 읽기전용으로 셋팅
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox3);
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox4);
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox5);
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox6);
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox7);

                this.tabForms.SelectedIndex = 0;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        #region 품목명 입력시 품목정식명도 같이 입력
        private void txtItemNm_Leave(object sender, EventArgs e)
        {
            if (txtItemFullNm.Text == "")
            {
                txtItemFullNm.Text = txtItemNm.Text;
            }
        }
        #endregion

        #region 품목계정 변경 이벤트
        private void cboItemAcct_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboItemAcct.Text != "")
                {
                    string strItemAcct = cboItemAcct.SelectedValue.ToString();

                    if (strItemAcct == "20")
                    {
                        cboRcptMthd.Tag = "반제품입고방법;1;;";
                        cboRcptMthd.Text = "";
                        cboRcptMthd.EditorBackColor = SystemBase.Validation.Kind_LightCyan;
                    }
                    else
                    {
                        cboRcptMthd.Tag = "";
                        cboRcptMthd.Text = "";
                        cboRcptMthd.EditorBackColor = SystemBase.Validation.Kind_White;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        #region  단공정여부 변경시
        private void rdoSngRoutFlag1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoSngRoutFlag1.Checked == true)
                {
                    txtWcCd.Enabled = true;
                    txtWcCd.Tag = "작업장;1;;";
                    txtWcCd.Value = "";
                    txtWcCd.BackColor = SystemBase.Validation.Kind_LightCyan;
                    btnWcCd.Enabled = true;
                    btnWcCd.Tag = "";
                }
                else
                {
                    txtWcCd.Enabled = true;
                    txtWcCd.Tag = ";2;;";
                    txtWcCd.Value = "";
                    txtWcCd.Enabled = false;
                    txtWcCd.BackColor = SystemBase.Validation.Kind_Gainsboro;
                    btnWcCd.Enabled = true;
                    btnWcCd.Tag = ";2;;";
                    btnWcCd.Enabled = false;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        #region 오더생성구분 클릭시 발주점 상태 변경
        private void cboOrderFrom_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cboOrderFrom.Text != "")
            {
                if (cboOrderFrom.SelectedValue.ToString() == "R")
                {
                    dtxtReorderPnt.Tag = "발주점;1;;";
                    dtxtReorderPnt.BackColor = SystemBase.Validation.Kind_LightCyan;
                    dtxtReorderPnt.Enabled = true;
                }
                else
                {
                    dtxtReorderPnt.Tag = ";2;;";
                    dtxtReorderPnt.Value = 0;
                    dtxtReorderPnt.BackColor = SystemBase.Validation.Kind_Gainsboro;
                    dtxtReorderPnt.Enabled = false;
                }
            }
        }
        #endregion

        #region 조달구분에 따른 구매입력 타입변환
        private void cboItemType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                if (cboItemType.Text != "")
                {
                    string strItemType = cboItemType.SelectedValue.ToString();
                    if (strItemType == "M")
                    {
                        rdoSngRoutFlag1.Checked = true;
                        rdoSngRoutFlag1.Enabled = true;
                        rdoSngRoutFlag2.Enabled = true;

                        cboOrderMfgUnit.Tag = "제조오더단위;1;;";
                        cboOrderMfgUnit.EditorBackColor = SystemBase.Validation.Kind_LightCyan;
                        dtxtOrderMfgLt.Tag = "제조오더L/T;1;;";
                        dtxtOrderMfgLt.Value = 0;
                        dtxtOrderMfgLt.BackColor = SystemBase.Validation.Kind_LightCyan;

                        cboOrderPurUnit.Tag = "";
                        cboOrderPurUnit.Text = "";
                        cboOrderPurUnit.EditorBackColor = SystemBase.Validation.Kind_White;
                        dtxtOrderPurLt.Tag = "";
                        dtxtOrderPurLt.BackColor = SystemBase.Validation.Kind_White;
                        dtxtOrderPurLt.Value = 0;
                        txtPurDept.Tag = "";
                        txtPurDept.Value = "";
                        txtPurDept.BackColor = SystemBase.Validation.Kind_White;
                    }
                    else
                    {
                        rdoSngRoutFlag2.Checked = true;
                        rdoSngRoutFlag1.Enabled = false;
                        rdoSngRoutFlag2.Enabled = false;

                        txtWcCd.Value = "";
                        txtWcCd.Tag = ";2;;";
                        txtWcCd.Enabled = false;
                        btnWcCd.Enabled = false;
                        btnWcCd.Tag = ";2;;";

                        cboOrderMfgUnit.Tag = "";
                        cboOrderMfgUnit.Text = "";
                        cboOrderMfgUnit.EditorBackColor = SystemBase.Validation.Kind_White;
                        dtxtOrderMfgLt.Tag = "";
                        dtxtOrderMfgLt.Value = 0;
                        dtxtOrderMfgLt.BackColor = SystemBase.Validation.Kind_White;

                        cboOrderPurUnit.Tag = "구매오더단위;1;;";
                        cboOrderPurUnit.Text = "";
                        cboOrderPurUnit.EditorBackColor = SystemBase.Validation.Kind_LightCyan;
                        dtxtOrderPurLt.BackColor = SystemBase.Validation.Kind_LightCyan;
                        dtxtOrderPurLt.Tag = "구매오더L/T;1;;";
                        txtPurDept.Value = "";
                        txtPurDept.Tag = "구매조직;1;;";
                        txtPurDept.BackColor = SystemBase.Validation.Kind_LightCyan;

                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region 오더생성여부 체크시 오더생성구분 입력
        private void rdoOrderFlag1_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoOrderFlag1.Checked == true)
            {
                //오더생성구분
                cboOrderFrom.Tag = ";2;;";
                cboOrderFrom.Text = "";
                cboOrderFrom.Enabled = false;
                cboOrderFrom.EditorBackColor = SystemBase.Validation.Kind_Gainsboro;

                //발주점
                dtxtReorderPnt.Tag = ";2;;";
                dtxtReorderPnt.Value = 0;
                dtxtReorderPnt.BackColor = SystemBase.Validation.Kind_Gainsboro;
                dtxtReorderPnt.Enabled = false;

                //Lot Sizing
                cboLotSizing.Tag = "Lot Size;1;;";
                cboLotSizing.Enabled = true;
                cboLotSizing.Text = "Lot For Lot";
                cboLotSizing.EditorBackColor = SystemBase.Validation.Kind_LightCyan;
            }
            else
            {
                //오더생성구분
                cboOrderFrom.Tag = "오더생성구분;1;;";
                cboOrderFrom.Enabled = true;
                cboOrderFrom.Text = "Reorder Point";
                cboOrderFrom.EditorBackColor = SystemBase.Validation.Kind_LightCyan;

                //발주점
                dtxtReorderPnt.Tag = "발주점;1;;";
                dtxtReorderPnt.BackColor = SystemBase.Validation.Kind_LightCyan;
                dtxtReorderPnt.Enabled = true;

                //Lot Sizing
                cboLotSizing.Tag = ";2;;";
                cboLotSizing.Text = "";
                cboLotSizing.Enabled = false;
                cboLotSizing.EditorBackColor = SystemBase.Validation.Kind_Gainsboro;

                //올림기간
                dtxtRoundPerd.Tag = ";2;;";
                dtxtRoundPerd.Value = 0;
                dtxtRoundPerd.BackColor = SystemBase.Validation.Kind_Gainsboro;
                dtxtRoundPerd.Enabled = false;
            }
        }
        #endregion

        #region LotSize 따른 올림기간 상태 변환
        private void cboLotSizing_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cboLotSizing.Text != "")
            {
                if (cboLotSizing.SelectedValue.ToString() == "P")
                {
                    dtxtRoundPerd.Tag = "올림기간;1;;";
                    dtxtRoundPerd.BackColor = SystemBase.Validation.Kind_LightCyan;
                    dtxtRoundPerd.Enabled = true;
                }
                else
                {
                    dtxtRoundPerd.Tag = ";2;;";
                    dtxtRoundPerd.Value = 0;
                    dtxtRoundPerd.BackColor = SystemBase.Validation.Kind_Gainsboro;
                    dtxtRoundPerd.Enabled = false;
                }
            }
        }
        #endregion

        #region TextChanged시 이벤트 발생
        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtItemNm.Value = "";
                }

                if (txtItemNm.Text != "")
                {
                    string strSql = " usp_BBI002  'S3' ";
                    strSql = strSql + ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtItemFullNm.Value = ds.Tables[0].Rows[0]["ITEM_FULL_NM"].ToString();
                        if (ds.Tables[0].Rows[0]["ITEM_ACCT"].ToString() != "") cboItemAcct.SelectedValue = ds.Tables[0].Rows[0]["ITEM_ACCT"];
                        txtItemSpec.Value = ds.Tables[0].Rows[0]["ITEM_SPEC"].ToString();
                        txtDrawNo.Value = ds.Tables[0].Rows[0]["DRAW_NO"].ToString();
                        cboStockUnit.SelectedValue = ds.Tables[0].Rows[0]["ITEM_UNIT"].ToString();
                        txtDrawRev.Value = ds.Tables[0].Rows[0]["DRAW_REV"].ToString();
                        dtpDrawRevDate.Value = ds.Tables[0].Rows[0]["DRAW_REV_DATE"].ToString();
                    }
                }
                else
                {
                    txtItemFullNm.Value = "";
                    cboItemAcct.Text = "";
                    txtItemSpec.Value = "";
                    txtDrawNo.Value = "";
                    cboStockUnit.Text = "";
                }

                if (txtItemCd.Text.Length >= 2)
                {
                    if (txtItemCd.Text.Substring(0, 2) == "PA" || txtItemCd.Text.Substring(0, 2) == "VA")
                    {
                       // cboMaterialType.Tag = "자재구분;1;;";
                      //  cboMaterialType.EditorBackColor = SystemBase.Validation.Kind_LightCyan;
                    }
                    else
                    {
                        cboMaterialType.Tag = "";
                        cboMaterialType.EditorBackColor = SystemBase.Validation.Kind_White;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목정보입력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rdoSerialFlag1_CheckedChanged(object sender, EventArgs e)
        {
            if(rdoSerialFlag1.Checked)
            rdoLotNo1.Checked = true;
        }

        private void rdoLotNo2_CheckedChanged(object sender, EventArgs e)
        {
            if(rdoLotNo2.Checked)
            rdoSerialFlag2.Checked = true;
        }
        //입고창고
        private void txtSlCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSlCd.Text != "")
                {
                    txtSlNm.Value = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", txtSlCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtSlNm.Value = "";
                }
            }
            catch { }
        }

        //출고창고
        private void txtIssuedSlCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtIssuedSlCd.Text != "")
                {
                    txtIssuedSlNm.Value = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", txtIssuedSlCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtIssuedSlNm.Value = "";
                }
            }
            catch { }
        }
        //라디오버튼 클릭시.
        //입고위치
        private void txtRcptLocCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtRcptLocCd.Text != "")
                {
                    txtRcptLocNm.Value = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", txtRcptLocCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtRcptLocNm.Value = "";
                }
            }
            catch { }
        }

        //출고위치
        private void txtIssuedLocCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtIssuedLocCd.Text != "")
                {
                    txtIssuedLocNm.Value = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", txtIssuedLocCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtIssuedLocNm.Value = "";
                }
            }
            catch { }
        }

        //구매부서
        private void txtPurDept_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPurDept.Text != "")
                {
                    txtPurDeptNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPurDept.Text, " AND MAJOR_CD = 'M001' AND LANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtPurDeptNm.Value = "";
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
                    txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P002' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWcNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region FPCOMM2_Load()
        void FPCOMM2_Load()
        {
            string Query2 = " usp_BAA004 'S3', @PFORM_ID='" + this.Name.ToString() + "', @PGRID_NAME='fpSpread2', @PIN_ID='" + SystemBase.Base.gstrUserID + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
            DataTable dt2 = SystemBase.DbOpen.TranDataTable(Query2);
            int G2RowCount = dt2.Rows.Count + 1;

            if (G2RowCount > 1)
            {
                G2Head1 = new string[G2RowCount];// 첫번째 Head Text
                G2Head2 = new string[G2RowCount];// 두번째 Head Text
                G2Head3 = new string[G2RowCount];// 세번째 Head Text
                G2Width = new int[G2RowCount];// Cell 넓이
                G2Align = new string[G2RowCount];// Cell 데이타 정렬방식
                G2Type = new string[G2RowCount];// CellType 지정
                G2Color = new int[G2RowCount];// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
                G2Etc = new string[G2RowCount];
                G2HeadCnt = Convert.ToInt32(dt2.Rows[0][0].ToString());
                G2SEQ = new int[G2RowCount];// 키

                /********************1번째 숨김필드 정의******************/
                G2Head1[0] = "";
                if (Convert.ToInt32(dt2.Rows[0][0].ToString()) >= 1)
                    G2Head2[0] = "";
                if (Convert.ToInt32(dt2.Rows[0][0].ToString()) >= 2)
                    G2Head3[0] = "";
                G2Width[0] = 0;
                G2Align[0] = "";
                G2Type[0] = "";
                G2Color[0] = 0;
                G2Etc[0] = "";
                /********************1번째 숨김필드 정의******************/

                //####################그리드 Head 순번######################
                GHIdx2 = new string[G2RowCount - 1, 2];	// 그리드 Head Index 변수 길이
                //string OldHeadName2 = null;
                int OldHeadNameCount2 = 1;
                //####################그리드 Head 순번######################
                for (int i = 1; i < G2RowCount; i++)
                {
                    G2Head1[i] = dt2.Rows[i - 1][1].ToString();
                    if (Convert.ToInt32(dt2.Rows[i - 1][0].ToString()) >= 1)
                        G2Head2[i] = dt2.Rows[i - 1][2].ToString();
                    if (Convert.ToInt32(dt2.Rows[i - 1][0].ToString()) >= 2)
                        G2Head3[i] = dt2.Rows[i - 1][3].ToString();

                    G2Width[i] = Convert.ToInt32(dt2.Rows[i - 1][4].ToString());
                    G2Align[i] = dt2.Rows[i - 1][5].ToString();
                    G2Type[i] = dt2.Rows[i - 1][6].ToString();
                    G2Color[i] = Convert.ToInt32(dt2.Rows[i - 1][7].ToString());
                    G2Etc[i] = dt2.Rows[i - 1][8].ToString();

                    G2SEQ[i] = Convert.ToInt32(dt2.Rows[i - 1][9].ToString());


                    //####################그리드 Head 순번######################
                    OldHeadNameCount2 = 1;
                    GHIdx2[0, 0] = dt2.Rows[0][1].ToString().ToUpper();
                    for (int k = 0; k < i - 1; k++)
                    {
                        if (dt2.Rows[i - 1][1].ToString().ToUpper() == GHIdx2[k, 0].ToUpper())
                        {
                            OldHeadNameCount2++;
                        }
                        else if (GHIdx2[k, 0].ToUpper().LastIndexOf("_") > 0 && dt2.Rows[i - 1][1].ToString().ToUpper() == GHIdx2[k, 0].ToUpper().Substring(0, GHIdx2[k, 0].ToUpper().LastIndexOf("_")))
                        {
                            OldHeadNameCount2++;
                        }

                    }

                    if (OldHeadNameCount2 > 1)
                    {
                        GHIdx2[i - 1, 0] = dt2.Rows[i - 1][1].ToString().ToUpper() + "_" + OldHeadNameCount2.ToString();	// 그리드 Head명
                    }
                    else
                    {
                        GHIdx2[i - 1, 0] = dt2.Rows[i - 1][1].ToString().ToUpper();	// 그리드 Head명
                    }

                    GHIdx2[i - 1, 1] = Convert.ToString(i);			// 그리드 Head 위치
                }
            }
        }
        #endregion

        #region fpSpread2_ButtonClicked

        private void fpSpread2_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "필수")].Text == "True")
                fpSpread2.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "I";
            else
                fpSpread2.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "I";
        }
        #endregion

    }
}
