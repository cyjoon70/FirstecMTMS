#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목정보등록
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-01-29
// 작성내용 : 품목정보등록 및 관리
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
using System.Collections.Generic;
using SystemBase;
using EDocument.Network;
using System.Windows.Forms;
using System.IO;
namespace BI.BBI001
{
    public partial class BBI001 : UIForm.FPCOMM1
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string SaveData = "", SearchData = ""; //컨트롤에 대한 조회후 데이터와 저장시 변경된 데이터 체크위한 변수
        string FilePath = "", Item_CD = "", FILES_NO = ""; //이미지 관련 변수
        string RevNo = "";
        #endregion

        #region BBI001
        public BBI001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBI001_Load(object sender, System.EventArgs e)
        {
            Control_Load(); //화면 SETTING
            
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0,0);
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

            //////////////////////////// 콤보박스 SETTING ////////////////////////////////////////////////////////////////////////
            SystemBase.ComboMake.C1Combo(cboSItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pCO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ", 3);     //조회 품목계정
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B036', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ");      //품목계정
            SystemBase.ComboMake.C1Combo(cboSItemGrp1, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B037', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 3);   //품목그룹1
            SystemBase.ComboMake.C1Combo(cboItemType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'P032', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ");      //품목구분
            SystemBase.ComboMake.C1Combo(cboItemUnit, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'Z005', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ");      //품목단위
            SystemBase.ComboMake.C1Combo(cboHsCd, "usp_B_COMMON @pTYPE ='B050', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 9);                        //HS코드
            SystemBase.ComboMake.C1Combo(cboVatType, "usp_B_COMMON @pTYPE ='B060', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 9);                     //VAT유형
            SystemBase.ComboMake.C1Combo(cboNetUnit, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'Z005', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 9);    //NETUNIT
            SystemBase.ComboMake.C1Combo(cboGrossUnit, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'Z005', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 9);  //GROSSUNIT

            //////////////////////////// 라디오버튼 SETTING /////////////////////////////////////////////////////////////////////////
            optPahntom2.Checked = true;
            optPur2.Checked = true;
            optUseFlag1.Checked = true;
            rdoStdItemN.Checked = true;     // 2017.03.17. hma 추가: 표준품목여부(아니오로 세팅)

            /////////////////////////// 유효기간 SETTING ////////////////////////////////////////////////////////////////////////////
            dtpUseDateFr.Value = "2000-01-01";
            dtpUseDateTo.Value = "2999-12-31";

            /////////////////////////// 탭 초기화 ///////////////////////////////////////////////////////////////////////////////////
            TabSetting();
        }
        #endregion

        #region TabSetting
        private void TabSetting()
        {
            UIForm.TabFPMake.TabPageColor(tabPage1); //기준정보
            UIForm.TabFPMake.TabPageColor(tabPage2); //규격정보
            UIForm.TabFPMake.TabPageColor(tabPage3); //이미지

            this.tabForms.SelectedIndex = 0;
        }
        #endregion

        #region 팝업창 열기
        //기준품목
        private void btnBasicItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW001 pu = new WNDW.WNDW001(txtBasicItem.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtBasicItem.Text = Msgs[1].ToString();
                    txtBasicItemNm.Value = Msgs[2].ToString();
                    txtBasicItem.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "기준품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
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
            SystemBase.Validation.GroupBox_Reset(groupBox4);
            SystemBase.Validation.GroupBox_Reset(groupBox5);

            Control_Load();

            RevNo = "";

            txtItemCd.Focus();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1)) //필수체크
            {
                string strQuery = "";

                strQuery += " usp_BBI001  'S1'";
                strQuery += ", @pITEM_CD ='" + txtSItemCd.Text.Trim() + "' ";
                strQuery += ", @pITEM_ACCT ='" + cboSItemAcct.SelectedValue.ToString() + "' ";
                strQuery += ", @pITEM_GRP1 ='" + cboSItemGrp1.SelectedValue.ToString() + "' ";
                strQuery += ", @pITEM_NM ='" + txtSItemNm.Text + "' ";
                strQuery += ", @pDRAW_NO ='" + txtSDrawNo.Text + "' ";
                strQuery += ", @pITEM_SPEC ='" + txtSItemSpec.Text + "' ";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "' ";             

                //그리드 Bind
                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
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
                && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox4))
            {
                //컨트롤 체크값 초기화
                SaveData = "";
                //컨트롤 체크 함수
                gBox = new GroupBox[] { groupBox3, groupBox4, groupBox5 };
                SystemBase.Validation.Control_Check(gBox, ref SaveData);

                //기존 컨트롤 데이터와 현재 컨트롤 데이터 비교
                if(SearchData == SaveData)
                {
                    //변경되거나 처리할 데이터가 없습니다.
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    return;
                }

                string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.         

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_BBI001 'U1' ";
                    strSql = strSql + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strSql = strSql + ", @pITEM_CD = '" + txtItemCd.Text.ToUpper().Trim() + "'";
                    strSql = strSql + ", @pITEM_NM= '" + txtItemNm.Text + "'";
                    strSql = strSql + ", @pITEM_FULL_NM = '" + txtItemFullNm.Text + "'";
                    strSql = strSql + ", @pITEM_NM_ENG = '" + txtItemNmEng.Text + "'"; // 영문품목명 추가

                    strSql = strSql + ", @pITEM_SPEC = '" + txtItemSpec.Text + "'";

                    strSql = strSql + ", @pSPEC_NO = '" + txtSpecNO.Text + "'";
                    strSql = strSql + ", @pQAR_NO = '" + txtQarNO.Text + "'";
                    strSql = strSql + ", @pKDS_NO = '" + txtKdsNO.Text + "'";
                    strSql = strSql + ", @pDL_NO = '" + txtDlNO.Text + "'";
                    strSql = strSql + ", @pETC_TDP_NO = '" + txtEtcTdpNO.Text + "'";

                    if (cboItemAcct.Text != "") strSql = strSql + ", @pITEM_ACCT = '" + cboItemAcct.SelectedValue.ToString() + "'";
                    if (cboItemUnit.Text != "") strSql = strSql + ", @pITEM_UNIT = '" + cboItemUnit.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pHS_CD = '" + cboHsCd.ToString() + "'";
                    strSql = strSql + ", @pHS_UNIT = '" + txtHsUnit.Text.ToString() + "'";
                    double dblNetWeight = 0; if (txtNetWeight.Text != "") dblNetWeight = Convert.ToDouble(txtNetWeight.Text.ToString());
                    strSql = strSql + ", @pNET_WEIGHT= '" + dblNetWeight + "'";
                    if (cboNetUnit.Text != "") strSql = strSql + ", @pNET_UNIT = '" + cboNetUnit.SelectedValue.ToString() + "'";
                    double dblGrossWeight = 0; if (txtGrossWeight.Text != "") dblGrossWeight = Convert.ToDouble(txtGrossWeight.Text.ToString());
                    strSql = strSql + ", @pGROSS_WEIGHT = '" + dblGrossWeight + "'";
                    if (cboGrossUnit.Text != "") strSql = strSql + ", @pGROSS_UNIT = '" + cboGrossUnit.SelectedValue.ToString() + "'"; 
                    double dblCbm = 0; if (txtCbm.Text != "") dblCbm = Convert.ToDouble(txtCbm.Text.ToString());
                    strSql = strSql + ", @pCBM = '" + dblCbm + "'";
                    strSql = strSql + ", @pCBM_DESC = '" + txtCbmDesc.Text.ToString() + "'";
                    string strPhaFlag = "N"; if (optPahntom1.Checked == true) strPhaFlag = "Y";
                    strSql = strSql + ", @pPHANTOM_FLAG = '" + strPhaFlag + "'";
                    strSql = strSql + ", @pDRAW_NO = '" + txtDrawNo.Text.ToString() + "'";
                    strSql = strSql + ", @pDRAW_REV = '" + txtDrawRev.Text.ToString() + "'";
                    strSql = strSql + ", @pDRAW_REV_DATE = '" + dtpDrawRevDate.Text + "'";
                    string strPurFlag = "N"; if (optPur1.Checked == true) strPurFlag = "Y";
                    strSql = strSql + ", @pBLANKET_PUR_FLAG = '" + strPurFlag + "'";
                    strSql = strSql + ", @pBASE_ITEM_CD = '" + txtBasicItem.Text.ToString() + "'";
                    strSql = strSql + ", @pUSE_DATE_FR = '" + dtpUseDateFr.Text.ToString() + "'";
                    strSql = strSql + ", @pUSE_DATE_TO = '" + dtpUseDateTo.Text.ToString() + "'";
                    strSql = strSql + ", @pVAT_TYPE = '" + cboVatType.SelectedValue.ToString() + "'";
                    double dblVatRate = 0; if (txtVatRate.Text != "") dblVatRate = Convert.ToDouble(txtVatRate.Text.ToString());
                    strSql = strSql + ", @pVAT_RATE = '" + dblVatRate + "'";
                    strSql = strSql + ", @pITEM_TYPE = '" + cboItemType.SelectedValue.ToString() + "'";
                    string rdoBomFlag = "E";
                    if (rdoS.Checked == true)
                    {
                        rdoBomFlag = "S";
                    }
                    else if (rdoD.Checked == true)
                    {
                        rdoBomFlag = "D";
                    }
                    else if (rdoA.Checked == true)
                    {
                        rdoBomFlag = "A";
                    }
                    strSql = strSql + ", @pBOM_FLAG = '" + rdoBomFlag + "'";
                    // 2017.03.17. hma 추가(Start): 표준품목여부 항목
                    string strStdItemYN = "N"; if (rdoStdItemY.Checked == true) strStdItemYN = "Y";
                    strSql = strSql + ", @pSTD_ITEM_YN = '" + strStdItemYN + "'";
                    // 2017.03.17. hma 추가(End)
                    strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

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

                    //컨트롤 체크값 초기화
                    SearchData = "";
                    //컨트롤 체크 함수
                    gBox = new GroupBox[] { groupBox3, groupBox4, groupBox5 };
                    SystemBase.Validation.Control_Check(gBox, ref SearchData);

                    //그리드 셀 포커스 이동
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
        #endregion

        #region DeleteExec() 데이타 삭제 로직
        protected override void DeleteExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (txtItemCd.Text != "")
            {
                if (MessageBox.Show(SystemBase.Base.MessageRtn("SY010"), "삭제", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strSql = " usp_BBI001 'D1'";
                        strSql = strSql + ", @pITEM_CD  = '" + txtItemCd.Text + "'";
                        strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

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
                        SystemBase.Validation.GroupBox_Reset(groupBox2);
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

            RevNo = "";

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

                //상세조회 SQL
                string strQuery = " usp_BBI001  'S2'";
                strQuery = strQuery + ", @pITEM_CD ='" + fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "', @pCO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    //기준정보
                    this.tabForms.SelectedIndex = 0;

                    txtItemCd.Value = dt.Rows[0]["ITEM_CD"].ToString();
                    txtItemNm.Text = dt.Rows[0]["ITEM_NM"].ToString();
                    txtItemFullNm.Text = dt.Rows[0]["ITEM_FULL_NM"].ToString();
                    txtItemNmEng.Text = dt.Rows[0]["ITEM_NM_ENG"].ToString(); //영문품목명 추가
                    cboItemAcct.SelectedValue = dt.Rows[0]["ITEM_ACCT"].ToString();
                    cboItemUnit.SelectedValue = dt.Rows[0]["ITEM_UNIT"].ToString();
                    if (dt.Rows[0]["PHANTOM_FLAG"].ToString() == "Y") { optPahntom1.Checked = true; optPahntom2.Checked = false; }
                    else { optPahntom1.Checked = false; optPahntom2.Checked = true; }
                    if (dt.Rows[0]["BLANKET_PUR_FLAG"].ToString() == "Y") { optPur1.Checked = true; optPur2.Checked = false; }
                    else { optPur1.Checked = false; optPur2.Checked = true; }
                    txtBasicItem.Text = dt.Rows[0]["BASE_ITEM_CD"].ToString();
                    if (dt.Rows[0]["USE_FLAG"].ToString() == "Y") { optUseFlag1.Checked = true; optUseFlag2.Checked = false; }
                    else { optUseFlag1.Checked = false; optUseFlag2.Checked = true; }
                    dtpUseDateFr.Text = dt.Rows[0]["USE_DATE_FR"].ToString();
                    dtpUseDateTo.Text = dt.Rows[0]["USE_DATE_TO"].ToString();
                    cboVatType.SelectedValue = dt.Rows[0]["VAT_TYPE"].ToString();
                    txtVatRate.Value = dt.Rows[0]["VAT_RATE"];
                    cboItemType.SelectedValue = dt.Rows[0]["ITEM_TYPE"];
                    // 2017.03.17. hma 추가(Start): 표준품목여부 항목값
                    if (dt.Rows[0]["STD_ITEM_YN"].ToString() == "Y") { rdoStdItemY.Checked = true; rdoStdItemN.Checked = false; }
                    else { rdoStdItemY.Checked = false; rdoStdItemN.Checked = true; }
                    // 2017.03.17. hma 추가(End)

                    //규격정보
                    this.tabForms.SelectedIndex = 1;

                    txtItemSpec.Text = dt.Rows[0]["ITEM_SPEC"].ToString();
                    txtSpecNO.Text = dt.Rows[0]["SPEC_NO"].ToString();
                    txtQarNO.Text = dt.Rows[0]["QAR_NO"].ToString();
                    txtKdsNO.Text = dt.Rows[0]["KDS_NO"].ToString();
                    txtDlNO.Text = dt.Rows[0]["DL_NO"].ToString();
                    txtEtcTdpNO.Text = dt.Rows[0]["ETC_TDP_NO"].ToString();
                        
                    cboHsCd.SelectedValue = dt.Rows[0]["HS_CD"].ToString();
                    txtHsUnit.Value = dt.Rows[0]["HS_UNIT"].ToString();
                    txtNetWeight.Value = dt.Rows[0]["NET_WEIGHT"];
                    cboNetUnit.SelectedValue = dt.Rows[0]["NET_UNIT"].ToString();
                    txtGrossWeight.Value = dt.Rows[0]["GROSS_WEIGHT"];
                    cboGrossUnit.SelectedValue = dt.Rows[0]["GROSS_UNIT"].ToString();
                    txtCbm.Value = dt.Rows[0]["CBM"];
                    txtCbmDesc.Text = dt.Rows[0]["CBM_DESC"].ToString();
                    txtDrawNo.Text = dt.Rows[0]["DRAW_NO"].ToString();
                    txtDrawRev.Text = dt.Rows[0]["DRAW_REV"].ToString();
                    RevNo = txtDrawRev.Text;

                    //rev일자 활성유무로 인해 한번 더 체크
                    if (txtDrawRev.Text != "" && txtDrawRev.Text != "0" && txtDrawRev.Text != "N/A" && RevNo != txtDrawRev.Text)
                    {
                        dtpDrawRevDate.Tag = "도면REV DATE;1;;";
                        dtpDrawRevDate.Enabled = true;
                        dtpDrawRevDate.BackColor = SystemBase.Validation.Kind_LightCyan;
                    }
                    else
                    {
                        dtpDrawRevDate.Tag = ";2;;";
                        dtpDrawRevDate.BackColor = SystemBase.Validation.Kind_Gainsboro;
                        dtpDrawRevDate.Enabled = false;
                    }

                    dtpDrawRevDate.Value = dt.Rows[0]["DRAW_REV_DATE"].ToString();

                    if (dt.Rows[0]["BOM_FLAG"].ToString() == "S")
                    {
                        rdoS.Checked = true;
                    }
                    else if (dt.Rows[0]["BOM_FLAG"].ToString() == "D")
                    {
                        rdoD.Checked = true;
                    }
                    else if (dt.Rows[0]["BOM_FLAG"].ToString() == "A")
                    {
                        rdoA.Checked = true;
                    }
                    else
                    {
                        rdoE.Checked = true;
                    }

                    //이미지 정보
                    this.tabForms.SelectedIndex = 2;
                    string FtpFile = "ftp://172.30.24.14/ITEM_IMAGE/";
                    Item_CD = txtItemCd.Text;

                    string FileKind = dt.Rows[0]["FILEEXTENSIONS"].ToString();

                    if ((FileKind.ToUpper() == "GIF" || FileKind.ToUpper() == "JPG" || FileKind.ToUpper() == "BMP"))
                    {
                        string filename = Path.GetRandomFileName();
                        MemoryStream m = Ftp.DownloadFileToStream(FtpFile + Item_CD, "E2MAX", "zemax");
                        picShow.Image = System.Drawing.Image.FromStream(m);

                        picShow.Width = groupBox5.Width - 130;
                        picShow.Height = groupBox5.Height - 26;

                        Decimal ImgHight = picShow.Image.Size.Height;
                        Decimal ImgWidth = picShow.Image.Size.Width;

                        Decimal picHeight = picShow.Height;
                        Decimal HeightCnt = picHeight / ImgHight;
                        Decimal WidthCnt = ImgWidth * HeightCnt;

                        if ((groupBox5.Width - 130) < Convert.ToInt32(WidthCnt))
                        {
                            Decimal WidCnt = Convert.ToDecimal(groupBox5.Width - 130) / ImgWidth;
                            Decimal HeiCnt = ImgHight * WidCnt;

                            picShow.Height = Convert.ToInt32(HeiCnt);
                            picShow.Width = Convert.ToInt32(groupBox5.Width - 130);
                        }
                        else
                        {
                            picShow.Width = Convert.ToInt32(WidthCnt);
                        }
                    }
                    else
                    {
                        picShow.Image = null;
                    }
                }
                else
                {
                    //그룹박스 초기화
                    SystemBase.Validation.GroupBox_Reset(groupBox3);
                    SystemBase.Validation.GroupBox_Reset(groupBox4);
                    SystemBase.Validation.GroupBox_Reset(groupBox5);
                }

                //현재 row값 설정
                PreRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                //키값 컨트롤 읽기전용으로 셋팅
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2);
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox3);
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox4);

                this.tabForms.SelectedIndex = 0;

                //컨트롤 체크값 초기화
                SearchData = "";
                //컨트롤 체크 함수
                GroupBox[] gBox = new GroupBox[] { groupBox3, groupBox4, groupBox5};
                SystemBase.Validation.Control_Check(gBox, ref SearchData);
            }
            catch(Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        #region 이미지 upload
        private void btnUpload_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text == "")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("S0005", "품목코드"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "전체(*.*)|*.*|gif 이미지(*.gif)|*.gif|jpg 이미지(*.jpg)|*.jpg|bmp 이미지(*.bmp)|*.bmp";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (picShow.Image != null)
                        picShow.Image.Dispose();
                    
                    string FileFullName = dlg.FileName.Substring(dlg.FileName.ToString().LastIndexOf(@"\") + 1, dlg.FileName.Length - dlg.FileName.ToString().LastIndexOf(@"\") - 1);
                    string FileName = FileFullName.Substring(0, FileFullName.ToString().LastIndexOf("."));
                    string FileKind = FileFullName.Substring(FileFullName.ToString().LastIndexOf(".") + 1, FileFullName.Length - FileFullName.ToString().LastIndexOf(".") - 1);

                    string FtpFile = "ftp://172.30.24.14/ITEM_IMAGE/";


                    if (FileKind.ToUpper() == "GIF" || FileKind.ToUpper() == "JPG" || FileKind.ToUpper() == "BMP")
                    {
                        DataSet ds = SystemBase.FILESAVE.FileInsert(FILES_NO, dlg.FileName, FileName, FileKind, SystemBase.Base.gstrUserID, SystemBase.Base.gstrLangCd);

                        picShow.Image = new Bitmap(dlg.FileName);	//SystemBase.Base.ProgramWhere + @"\images\"+ FileFullName.ToString()
                        picShow.Width = groupBox5.Width - 130;
                        picShow.Height = groupBox5.Height - 26;

                        Decimal ImgHight = picShow.Image.Size.Height;
                        Decimal ImgWidth = picShow.Image.Size.Width;

                        Decimal picHeight = picShow.Height;
                        Decimal HeightCnt = picHeight / ImgHight;
                        Decimal WidthCnt = ImgWidth * HeightCnt;

                        if ((groupBox5.Width - 130) < Convert.ToInt32(WidthCnt))
                        {
                            Decimal WidCnt = Convert.ToDecimal(groupBox5.Width - 130) / ImgWidth;
                            Decimal HeiCnt = ImgHight * WidCnt;

                            picShow.Height = Convert.ToInt32(HeiCnt);
                            picShow.Width = Convert.ToInt32(groupBox5.Width - 130);
                        }
                        else
                        {
                            picShow.Width = Convert.ToInt32(WidthCnt);
                        }

                        string Query = "usp_BBI001 @pType='U2', @pFILES_NO='" + Item_CD + "', @pITEM_CD='" + Item_CD + "', @pFILEEXTENSIONS ='" + FileKind + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                        SystemBase.DbOpen.NoTranNonQuery(Query);

                        Ftp.UploadFile(dlg.FileName, FtpFile + Item_CD, "E2MAX", "zemax");
                        MessageBox.Show(SystemBase.Base.MessageRtn("저장완료"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(".bmp .jpeg .gif 만사용가능합니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    SearchExec();
                    FilePath = dlg.FileName;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "이미지 upload"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 이미지 취소
        private void btnImageCancel_Click(object sender, System.EventArgs e)
        {
            if (txtItemCd.Text == "")
            {
                picShow.Image.Dispose();
                return;
            }

            string ERRCode = "ER", MSGCode = "B0015";
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_BBI001  'D2'";
                strSql = strSql + ", @pITEM_CD  = '" + txtItemCd.Text + "'";
                strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                string FtpFile = "ftp://172.30.24.14/ITEM_IMAGE/";
                Ftp.DeleteFile(FtpFile + txtItemCd.Text, "E2MAX", "zemax");
                Trans.Commit();
                SearchExec();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                picShow.Image = null ;
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
        #endregion

        #region 이미지 확대
        private void picShow_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (FilePath.Length > 0 && picShow.Image != null)
                {
                    string FileKind = FilePath.Substring(FilePath.ToString().LastIndexOf(".") + 1, FilePath.Length - FilePath.ToString().LastIndexOf(".") - 1);
                    if (FileKind.ToUpper() == "GIF" || FileKind.Trim().ToUpper() == "JPG" || FileKind.ToUpper() == "BMP")
                    {	// 이미지파일인 경우
                        UIForm.Picture pic = new UIForm.Picture(FilePath);
                        pic.ShowDialog();
                    }
                    else
                    {	// 이미지파일이 아닌경우
                        System.Diagnostics.Process.Start(SystemBase.Base.ProgramWhere + @"\images\temp." + FileKind.Trim());
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "이미지 확대"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 화면 닫을시 이미지 리소스 제거
        private void BBI001_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (picShow.Image != null)
                {
                    picShow.Image = null;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "종료"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 콤보박스 데이터 변경시
        //품목구분
        private void cboItemType_TextChanged(object sender, System.EventArgs e)
		{
			if(cboItemType.Text != "")
			{
				if (cboItemType.Text == "자작품")
				{
					txtDrawNo.Tag = "도면번호;1;;";
                    txtDrawNo.BackColor = SystemBase.Validation.Kind_LightCyan;

                    txtDrawRev.Tag = "도면REV;1;;";
                    txtDrawRev.BackColor = SystemBase.Validation.Kind_LightCyan;
				}
				else
				{
					txtDrawNo.Tag = "";
                    txtDrawNo.BackColor = SystemBase.Validation.Kind_White;

					txtDrawRev.Tag = "";
                    txtDrawRev.BackColor = SystemBase.Validation.Kind_White;
				}
			}
		}

		private void cboItemType_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			if(cboItemType.Text != "")
			{
				if (cboItemType.Text == "자작품")
				{
                    txtDrawNo.Tag = ";1;;";
					txtDrawNo.BackColor = SystemBase.Validation.Kind_LightCyan;

                    txtDrawRev.Tag = ";1;;";
                    txtDrawRev.BackColor = SystemBase.Validation.Kind_LightCyan;
				}
				else
				{
					txtDrawNo.Tag = "";
                    txtDrawNo.BackColor = SystemBase.Validation.Kind_White;

					txtDrawRev.Tag = "";
                    txtDrawRev.BackColor = SystemBase.Validation.Kind_White;
				}
			}
        }

        //HS코드
        private void cboHsCd_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {               
                txtHsUnit.Value = SystemBase.Base.CodeName("MINOR_CD", "REL_CD1", "B_COMM_CODE", cboHsCd.SelectedValue.ToString(), " AND MAJOR_CD = 'B039' AND COMP_CODE = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ");
            }
            catch { }
        }

        //VAT_TYPE
        private void cboVatType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                txtVatRate.Value = SystemBase.Base.CodeName("MINOR_CD", "REL_CD1", "B_COMM_CODE", cboVatType.SelectedValue.ToString(), " AND MAJOR_CD = 'B040' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
            }
            catch { }
        }
        #endregion

        #region 품목명 입력시 품목정식명도 같이 입력
        private void txtItemNm_Leave(object sender, EventArgs e)
        {
            if (txtItemFullNm.Text == "")
            {
                txtItemFullNm.Value = txtItemNm.Text;
            }
        }
        #endregion

        #region 기준품목코드 입력시 품목명 입력
        private void txtBasicItem_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtBasicItem.Text != "")
                {
                    txtBasicItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtBasicItem.Text, " AND CO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ");
                }
            }
            catch { }
        }
        #endregion  

        #region 도면REV 입력시 이벤트
        private void txtDrawRev_TextChanged(object sender, EventArgs e)
        {
            // 2017.09.19. hma 수정 내용(김노경C 요청)
            // 도면REV 수정시 도면REV DATE 항목이 활성화 되기는 하나 일자를 수정할 수 없도록 되어있어서
            // 수정할 수 있도록 컨트롤 Tag 속성을 ";2;;" => ""로 변경함. 소스는 수정된 사항 없음.

            if (txtDrawRev.Text != "" && txtDrawRev.Text != "0" && txtDrawRev.Text != "N/A" && RevNo != txtDrawRev.Text)
            {
                dtpDrawRevDate.Tag = "도면REV DATE;1;;";
                dtpDrawRevDate.Enabled = true;
                dtpDrawRevDate.Value = SystemBase.Base.ServerTime("YYMMDD");
                dtpDrawRevDate.BackColor = SystemBase.Validation.Kind_LightCyan;
            }
            else
            {
                dtpDrawRevDate.Tag = ";2;;";

                if (RevNo == "")
                {
                    dtpDrawRevDate.Value = "";
                }

                dtpDrawRevDate.BackColor = SystemBase.Validation.Kind_Gainsboro;
                dtpDrawRevDate.Enabled = false;
            }
        }
        #endregion
    }
}
