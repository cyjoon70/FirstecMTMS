#region 작성정보
/*********************************************************************/
// 단위업무명 : 수주변경
// 작 성 자 : 조  홍  태
// 작 성 일 : 2013-02-07
// 작성내용 : 확정된 수주에 한해 변경
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

namespace SO.SOA004
{
    public partial class SOA004 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strAutoSoNo = "";
        string strSearchData = "", strSaveData = ""; //컨트롤 저장 체크 변수
        #endregion

        #region 생성자
        public SOA004(string So_No)
        {
            // 알리미 클릭시- 알리미
            strAutoSoNo = So_No;
            InitializeComponent();
        }

        public SOA004()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SOA004_Load(object sender, System.EventArgs e)
        {
            //그룹박스 필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //GropBox1 조회조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSSaleDuty, "usp_S_COMMON @pTYPE = 'S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //영업담당
            SystemBase.ComboMake.C1Combo(cboSSoType, "usp_B_COMMON @pTYPE = 'TABLE', @pCODE = 'SO_TYPE', @pNAME = 'SO_TYPE_NM', @pSPEC1 = 'S_SO_TYPE' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);//수주형태

            //GroupBox2 입력조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSaleDuty, "usp_S_COMMON @pTYPE = 'S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); //영업담당
            SystemBase.ComboMake.C1Combo(cboSoType, "usp_B_COMMON @pTYPE = 'TABLE', @pCODE = 'SO_TYPE', @pNAME = 'SO_TYPE_NM', @pSPEC1 = 'S_SO_TYPE' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//수주형태
            SystemBase.ComboMake.C1Combo(cboPaymentMeth, "usp_B_COMMON @pType='COMM', @pCODE = 'S004', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//결재방법
            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//화폐단위
            SystemBase.ComboMake.C1Combo(cboContractType, "usp_B_COMMON @pType='COMM', @pCODE = 'S014', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//계약구분
            SystemBase.ComboMake.C1Combo(cboSoStatus, "usp_B_COMMON @pType='COMM', @pCODE = 'S017', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//진행상태
            SystemBase.ComboMake.C1Combo(cboCalcType, "usp_B_COMMON @pType='COMM', @pCODE = 'S003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//정산구분
            SystemBase.ComboMake.C1Combo(cboContractForm, "usp_B_COMMON @pType='COMM2', @pCODE = 'S035', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//계약형태
            SystemBase.ComboMake.C1Combo(cboVatType, "usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);//VAT유형

           
            //그리드 콤보박스 세팅
            //MASTER
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "화폐단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//화폐단위
            //DETAIL
            G1Etc[SystemBase.Base.GridHeadIndex2(fpSpread1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단위
            G1Etc[SystemBase.Base.GridHeadIndex2(fpSpread1, "단가구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'S011', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단가구분
            G1Etc[SystemBase.Base.GridHeadIndex2(fpSpread1, "공장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='PLANT' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            G1Etc[SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT유형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT유형
            G1Etc[SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT포함구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'S019', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT포함구분
            G1Etc[SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'S034', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 1);//매출유형(ABC)

            G1Etc[SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM2', @pCODE = 'D007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//방사청구매부서
            G1Etc[SystemBase.Base.GridHeadIndex2(fpSpread1, "조달업체")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM2', @pCODE = 'D006', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//조달업체
            G1Etc[SystemBase.Base.GridHeadIndex2(fpSpread1, "계약단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM2', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//계약단위

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //폼 컨트롤 초기화
            Control_Setting();

            dtpSDelvDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpSDelvDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");

            if (strAutoSoNo != "")
            {
                SubSearch(strAutoSoNo);
            }
        }
        #endregion

        #region ControlSetting()
        private void Control_Setting()
        {
            //기타 세팅
            dtpSoDt.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpCustPoDt.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpLastDeliveryDt.Value = SystemBase.Base.ServerTime("YYMMDD");
            cboCurrency.SelectedValue = "KRW";
            cboPaymentMeth.SelectedValue = "CH";
            cboSoType.SelectedValue = "DSO";
            dtxtExchRate.Value = 1;
            dtxtExchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
            dtxtExchRate.ReadOnly = true;
            chkBonded.Checked = true;
            txtProjectNo.BackColor = SystemBase.Validation.Kind_LightCyan;
            txtProjectNo.ReadOnly = false;
            txtProjectNo.Tag = "프로젝트번호;1;;";
            chkGoverment.Checked = false;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            strAutoSoNo = "";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //폼 컨트롤 초기화
            Control_Setting();

            dtpSDelvDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpSDelvDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");

            //프린트 버튼 활성화여부
            UIForm.Buttons.ReButton(BtnPrint, "BtnPrint", false);
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                UIForm.FPMake.RowInsert(fpSpread1);

                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "공장")].Value = SystemBase.Base.gstrPLANT_CD.ToString();//자기소속공장
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "수주잔량")].Value = 0;//0
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "수량")].Value = 0;//0
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "단가")].Value = 0;//0
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "금액")].Value = 0;//0
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT금액")].Value = 0;//0
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "단위")].Value = "EA";//EA
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "단가구분")].Value = "T";//진단가
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT유형")].Value = "A";//일반세금계산서
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT율")].Value = 10;//10
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT포함구분")].Value = "2";//별도

                if (cboVatType.SelectedText != "")
                {
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT유형")].Value = cboVatType.SelectedValue;
                }

                if (fpSpread1.Sheets[0].Rows.Count == 1)
                {
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처")].Text = txtSoldCustCd.Text;//납품처
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처명")].Text = txtSoldCustNm.Text;//납품처명
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처")].Text
                        = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex - 1, SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처")].Text;//납품처
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처명")].Text
                        = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex - 1, SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처명")].Text;//납품처명
                }

                UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex,
                    SystemBase.Base.GridHeadIndex2(fpSpread1, "발행") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "발행일자") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일") + "|1");
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//확정된 데이터는 다른 작업을 할 수 없습니다.
            }
        }
        #endregion

        #region 행복사 버튼 클릭 이벤트
        protected override void RCopyExec()
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                UIForm.FPMake.RowCopy(fpSpread1);

                //창고 흰색이면서 Locking
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고")].Locked = true;
                }

                UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex,
                    SystemBase.Base.GridHeadIndex2(fpSpread1, "발행") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "발행일자") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일") + "|1");
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//확정된 데이터는 다른 작업을 할 수 없습니다.
            }
        }
        #endregion

        #region SearchExec() Master 그리드 조회 로직
        protected override void SearchExec()
        {
            //마스터만 조회
            Search();
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion

        #region Search 조회함수
        private void Search()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    //주문처 유효성체크
                    if (txtSSoldCustCd.Text != "" && txtSSoldCustNm.Text == "")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "주문처"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //존재하지 않는 주문처 코드입니다.

                        txtSSoldCustCd.Focus();
                        this.Cursor = Cursors.Default;

                        return;
                    }

                    string strQuery = " usp_SOA004  @pTYPE = 'S1'";
                    strQuery += ", @pSO_DT_FR = '" + dtpSSoDtFr.Text + "' ";
                    strQuery += ", @pSO_DT_TO = '" + dtpSSoDtTo.Text + "' ";
                    strQuery += ", @pDELV_DT_FR = '" + dtpSDelvDtFr.Text + "' ";
                    strQuery += ", @pDELV_DT_TO = '" + dtpSDelvDtTo.Text + "' ";
                    strQuery += ", @pENT_CD = '" + txtSEntCd.Text + "' ";
                    strQuery += ", @pSALE_DUTY = '" + cboSSaleDuty.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSO_TYPE = '" + cboSSoType.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSOLD_CUST = '" + txtSSoldCustCd.Text + "' ";
                    strQuery += ", @pSOLD_CUST_NM = '" + txtSSoldCustNm.Text + "' ";
                    strQuery += ", @pSO_NO = '" + txtSSoNo.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "' ";
                    strQuery += ", @pSO_CONFIRM_YN = 'Y' "; //확정인 데이터만 조회
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pREF_DELV_DT_FR = '" + dtpRefDelvDtFr.Text + "' ";      // 2017.11.01. hma 추가: 납기일(참조) FROM
                    strQuery += ", @pREF_DELV_DT_TO = '" + dtpRefDelvDtTo.Text + "' ";      // 2017.11.01. hma 추가: 납기일(참조) TO

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Master그리드 선택시 상세정보 조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;

                    //같은 Row 조회 되지 않게
                    if (intRow < 0)
                    {
                        return;
                    }

                    if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
                    {
                        return;
                    }

                    strAutoSoNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2,"수주번호")].Text.ToString();//수주번호

                    SubSearch(strAutoSoNo);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region 상세정보 조회
        private void SubSearch(string strCode)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox2);

                fpSpread1.Sheets[0].Rows.Count = 0;

                //수주Master정보
                string strSql = " usp_SOA004  'S2' ";
                strSql = strSql + ", @pSO_NO ='" + strCode + "' ";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                if (dt.Rows.Count > 0)
                {
                    bool ConfirmChk = false, BondedChk = false, NeedQtyBondChk = false, UnifyContract = false, Goverment = false, Andcost = false;
                    bool bStdItemYN = false;        // 2017.03.17. hma 추가: 표준품목적용여부

                    //확정여부
                    if (dt.Rows[0]["SO_CONFIRM_YN"].ToString() != "")
                    {
                        if (dt.Rows[0]["SO_CONFIRM_YN"].ToString() == "Y") { ConfirmChk = true; }
                        else { ConfirmChk = false; }
                    }
                    else { ConfirmChk = false; }
 
                    //보세여부
                    if (dt.Rows[0]["BONDED_YN"].ToString() != "")
                    {
                        if (dt.Rows[0]["BONDED_YN"].ToString() == "Y") { BondedChk = true; }
                        else { BondedChk = false; }
                    }
                    else { BondedChk = false; }

                    //소요량증명유무
                    if (dt.Rows[0]["NEED_QTY_BOND_YN"].ToString() != "")
                    {
                        if (dt.Rows[0]["NEED_QTY_BOND_YN"].ToString() == "Y") { NeedQtyBondChk = true; }
                        else { NeedQtyBondChk = false; }
                    }
                    else { NeedQtyBondChk = false; }

                    //통합수주여부
                    if (dt.Rows[0]["UNITY_CONTRACT_YN"].ToString() != "")
                    {
                        if (dt.Rows[0]["UNITY_CONTRACT_YN"].ToString() == "Y") { UnifyContract = true; }
                        else { UnifyContract = false; }
                    }
                    else { UnifyContract = false; }

                    //정부과제유무
                    if (dt.Rows[0]["GOVERMENT_YN"].ToString() != "")
                    {
                        if (dt.Rows[0]["GOVERMENT_YN"].ToString() == "Y") { Goverment = true; }
                        else { Goverment = false; }
                    }
                    else { Goverment = false; }

                    // 2017.03.17. hma 추가(Start): 표준품목적용여부 체크
                    if (dt.Rows[0]["STD_ITEM_YN"].ToString() != "")
                    {
                        if (dt.Rows[0]["STD_ITEM_YN"].ToString() == "Y") { bStdItemYN = true; }
                        else { bStdItemYN = false; }
                    }
                    else { bStdItemYN = false; }
                    // 2017.03.17. hma 추가(End)

                    txtSoNo.Value = dt.Rows[0]["SO_NO"].ToString();
                    txtEntCd.Value = dt.Rows[0]["ENT_CD"].ToString();
                    txtProjectNo.Value = dt.Rows[0]["PROJECT_NO"].ToString();
                    txtProjectNm.Value = dt.Rows[0]["PROJECT_NM"].ToString();
                    dtpSoDt.Value = dt.Rows[0]["SO_DT"].ToString();
                    cboSoType.SelectedValue = dt.Rows[0]["SO_TYPE"].ToString();
                    dtpLastDeliveryDt.Value = dt.Rows[0]["LAST_DELIVERY_DT"].ToString();
                    txtCustPoNo.Value = dt.Rows[0]["CUST_PO_NO"].ToString();
                    dtpCustPoDt.Value = dt.Rows[0]["CUST_PO_DT"].ToString();
                    txtSoldCustCd.Value = dt.Rows[0]["SOLD_CUST"].ToString();
                    txtCollectCustCd.Value = dt.Rows[0]["COLLECT_CUST"].ToString();
                    cboSaleDuty.SelectedValue = dt.Rows[0]["SALE_DUTY"].ToString();
                    chkConfirm.Checked = ConfirmChk;
                    cboPaymentMeth.SelectedValue = dt.Rows[0]["PAYMENT_METH"].ToString();
                    dtxtPaymentTerm.Value = dt.Rows[0]["PAYMENT_TERM"];
                    txtPaymentRemark.Value = dt.Rows[0]["PAYMENT_TERM_REMARK"].ToString();
                    cboCurrency.SelectedValue = dt.Rows[0]["CURRENCY"].ToString();
                    dtxtExchRate.Value = dt.Rows[0]["EXCH_RATE"];
                    dtxtSoAmt.Value = dt.Rows[0]["SO_AMT"];
                    dtxtSoAmtLoc.Value = dt.Rows[0]["SO_AMT_LOC"];
                    dtxtNetAmtLoc.Value = dt.Rows[0]["NET_AMT_LOC"];
                    dtxtVatAmtLoc.Value = dt.Rows[0]["VAT_AMT_LOC"];
                    chkBonded.Checked = BondedChk;
                    cboSoStatus.SelectedValue = dt.Rows[0]["SO_STATUS"].ToString();
                    cboContractType.SelectedValue = dt.Rows[0]["CONTRACT_TYPE"].ToString();
                    cboCalcType.SelectedValue = dt.Rows[0]["CALC_TYPE"].ToString();
                    txtRemark.Value = dt.Rows[0]["REMARK"].ToString();
                    chkNeedQtyBond.Checked = NeedQtyBondChk;
                    cboContractForm.SelectedValue = dt.Rows[0]["CONTRACT_FORM"].ToString();
                    chkUnifyContract.Checked = UnifyContract;
                    chkGoverment.Checked = Goverment;
                    chkStdItemYN.Checked = bStdItemYN;      // 2017.03.17. hma 추가: 표준품목적용여부

                    if (dt.Rows[0]["CURRENCY"].ToString() == "KRW")
                    {
                        dtxtExchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
                        dtxtExchRate.ReadOnly = true;
                        dtxtExchRate.Tag = ";2;;";
                    }
                    else
                    {
                        dtxtExchRate.BackColor = SystemBase.Validation.Kind_LightCyan;
                        dtxtExchRate.ReadOnly = false;
                        dtxtExchRate.Tag = "환율;1;;";
                    }

                    //현재 row값 설정
                    PreRow = fpSpread2.ActiveSheet.ActiveRowIndex;

                    SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2); //Key값 컨트롤 세팅

                    //컨트롤 체크값 초기화
                    strSearchData = "";
                    //컨트롤 체크 함수
                    GroupBox[] gBox = new GroupBox[] { groupBox2};
                    SystemBase.Validation.Control_Check(gBox, ref strSearchData);

                    //수주Detail그리드 정보.
                    string strSql1 = " usp_SOA004  'S3' ";
                    strSql1 += ", @pSO_NO ='" + strCode + "' ";
                    strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    Set_Grid_Lock(); //그리드 locking
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Set_Grid_Lock 그리드Locking
        private void Set_Grid_Lock()
        {
			//Detail Locking설정
			for(int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
			{
				if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "마감여부")].Text == "Y")
				{
                    UIForm.FPMake.grdReMake(fpSpread1, i,
                        SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "단위") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "수량") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "단가") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "단가구분") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "금액") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT금액") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일(참조)") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT유형") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT포함구분") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "공장") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "비고") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "창고") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "창고_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "Location") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "Location_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "통합원가대상") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "판단번호") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서_2") + "|3"       // 2020.01.13. hma 추가
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청부서명") + "|3"           // 2020.01.13. hma 추가
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "지시연도") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "산정차수") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "조달업체") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "재고번호") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "재고번호품명") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "계약명") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "계약단위") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "계약수량") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "방산물자지정유무") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "호기정렬기준품목") + "|3"        // 2018.03.15. hma 추가
                        ); //방산물자지정유무
							
					fpSpread1.Sheets[0].Cells[i,1,i,fpSpread1.Sheets[0].Columns.Count-1].ForeColor = Color.Red;
				}
				else
				{
					if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "SCH진행상태")].Text == "MPS 계획"
						|| fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "SCH진행상태")].Text == "")
					{
						UIForm.FPMake.grdReMake(fpSpread1, i, 
							SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드_2") + "|3"
							//+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "단위") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "단가구분") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "공장") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처_2") + "|3"
							);
					}
					else
					{
						UIForm.FPMake.grdReMake(fpSpread1, i, 
							SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드_2") + "|3"
							//+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "단위") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "공장") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처_2") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "창고") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "창고_2") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "Location") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "Location_2") + "|3" );

						fpSpread1.Sheets[0].Cells[i,1,i,fpSpread1.Sheets[0].Columns.Count-1].ForeColor = Color.Red;
					}
				}
			}
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            txtProjectNo.Focus();

            string ChkSoNo = strAutoSoNo;
            GroupBox[] gBox = null;

            string strInUpFlag = "I";

            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////
            
            //컨트롤 체크값 초기화
            strSaveData = "";
            //컨트롤 체크 함수
            gBox = new GroupBox[] { groupBox2 };
            SystemBase.Validation.Control_Check(gBox, ref strSaveData);

            //기존 컨트롤 데이터와 현재 컨트롤 데이터 비교
            if (strSearchData == strSaveData && UIForm.FPMake.HasSaveData(fpSpread1) == false)
            {
                //변경되거나 처리할 데이터가 없습니다.
                MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Cursor = Cursors.Default;
                return;
            }

            //상단 그룹박스 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    //주문처 유효성 체크
                    if (txtSoldCustCd.Text != "" && txtSoldCustNm.Text == "")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "주문처"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //존재하지 않는 주문처 코드입니다.

                        txtSoldCustCd.Focus();
                        this.Cursor = Cursors.Default;

                        return;
                    }

                    //수금처 유효성 체크
                    if (txtCollectCustCd.Text != "" && txtCollectCustNm.Text == "")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "수금처"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //존재하지 않는 주문처 코드입니다.

                        txtSoldCustCd.Focus();
                        this.Cursor = Cursors.Default;

                        return;
                    }

                    string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                    string strSoSeq = "";

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string chkBondedYn = "";

                        if (chkBonded.Checked == true) { chkBondedYn = "Y"; }
                        else { chkBondedYn = "N"; }

                        string chkGovermentYn = "";
                        if (chkGoverment.Checked == true) { chkGovermentYn = "Y"; }
                        else { chkGovermentYn = "N"; }

                        // 2017.03.17. hma 추가(Start): 표준품목적용여부
                        string strStdItemYN = "";
                        if (chkStdItemYN.Checked == true) { strStdItemYN = "Y"; }
                        else { strStdItemYN = "N"; }
                        // 2017.03.17. hma 추가(End)

                        string strMSql = " usp_SOA004 'U1'";
                        strMSql += ", @pSO_NO = '" + txtSoNo.Text + "' ";
                        strMSql += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                        strMSql += ", @pPROJECT_NM = '" + txtProjectNm.Text + "' ";
                        strMSql += ", @pCUST_PO_NO = '" + txtCustPoNo.Text + "' ";
                        strMSql += ", @pPAYMENT_TERM = '" + dtxtPaymentTerm.Value + "' ";
                        strMSql += ", @pPAYMENT_TERM_REMARK = '" + txtPaymentRemark.Text + "' ";
                        strMSql += ", @pBONDED_YN = '" + chkBondedYn + "' ";
                        strMSql += ", @pCALC_TYPE = '" + cboCalcType.SelectedValue.ToString() + "' ";
                        strMSql += ", @pREMARK = '" + txtRemark.Text + "' ";
                        strMSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strMSql += ", @pSO_DT = '" + dtpSoDt.Text + "' ";
                        strMSql += ", @pSO_TYPE = '" + cboSoType.SelectedValue.ToString() + "' ";
                        if (dtpCustPoDt.Text != "")
                            strMSql += ", @pCUST_PO_DT = '" + dtpCustPoDt.Text + "' ";
                        strMSql += ", @pSOLD_CUST = '" + txtSoldCustCd.Text + "' ";
                        strMSql += ", @pCOLLECT_CUST = '" + txtCollectCustCd.Text + "' ";
                        strMSql += ", @pSALE_DUTY = '" + cboSaleDuty.SelectedValue.ToString() + "' ";
                        strMSql += ", @pENT_CD = '" + txtEntCd.Text + "' ";
                        strMSql += ", @pGOVERMENT_YN  = '" + chkGovermentYn + "'";
                        strMSql += ", @pSTD_ITEM_YN  = '" + strStdItemYN + "'";     // 2017.03.17. hma 추가: 표준품목적용여부

                        strMSql += ", @pEXCH_RATE = '" + dtxtExchRate.Value + "' ";
                        strMSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt = SystemBase.DbOpen.TranDataTable(strMSql, dbConn, Trans);
                        ERRCode = dt.Rows[0][0].ToString();
                        MSGCode = dt.Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                        //그리드 상단 필수 체크
                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                        {
                            string strDelErrCode = DelCheck(); //삭제할 row수와 잔량,수량비교해서 삭제가능한지 체크

                            //Detail정보를 모두 삭제할 경우 Master정보를 삭제할지 물어보고 아니면 취소한다.
                            if (strDelErrCode == "DELCNT_ER")
                            {
                                string msg = SystemBase.Base.MessageRtn("B0027");
                                DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                if (dsMsg == DialogResult.Yes)
                                {
                                    try
                                    {
                                        string strDelSql = " usp_SOA004  'D1'";
                                        strDelSql += ", @pSO_NO = '" + strAutoSoNo + "' ";
                                        strDelSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                        DataSet ds2 = SystemBase.DbOpen.TranDataSet(strDelSql, dbConn, Trans);
                                        ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                                        MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit1; }	// ER 코드 Return시 점프

                                        Trans.Commit();
                                    }
                                    catch (Exception f)
                                    {
                                        SystemBase.Loggers.Log(this.Name, f.ToString());
                                        Trans.Rollback();
                                        ERRCode = "ER";
                                        MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                                    }
                                Exit1:
                                    dbConn.Close();

                                    if (ERRCode == "OK")
                                    {
                                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        Search();

                                        //컨트롤 체크값 초기화
                                        strSearchData = "";
                                        //컨트롤 체크 함수
                                        gBox = new GroupBox[] { groupBox2 };
                                        SystemBase.Validation.Control_Check(gBox, ref strSearchData);

                                        //그리드 셀 포커스 이동
                                        UIForm.FPMake.GridSetFocus(fpSpread2, strAutoSoNo, SystemBase.Base.GridHeadIndex(GHIdx2, "수주번호"));

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

                                    return;
                                }
                                else
                                {
                                    Trans.Rollback();
                                    MessageBox.Show(SystemBase.Base.MessageRtn("B0040"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);//작업이 취소되었습니다.
                                    this.Cursor = Cursors.Default;
                                    return;
                                }
                            }

                            
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

                                    if (strGbn == "U2")
                                    {
                                        strSoSeq = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "수주SEQ")].Value.ToString();
                                    }
                                    else
                                    {
                                        strSoSeq = "0";
                                    }

                                    string strSql = " usp_SOA004 '" + strGbn + "'";
                                    strSql += ", @pSO_NO = '" + txtSoNo.Text + "' ";
                                    strSql += ", @pSO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "수주SEQ")].Value + "' ";
                                    strSql += ", @pCONTRACT_TYPE = '" + cboContractType.SelectedValue.ToString() + "' ";
                                    strSql += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                                    strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Value + "' ";
                                    strSql += ", @pPLANT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "공장")].Value + "' ";
                                    strSql += ", @pSL_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고")].Text + "' ";
                                    strSql += ", @pLOCATION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location")].Text + "' ";
                                    strSql += ", @pDELIVERY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일")].Text + "' ";
                                    strSql += ", @pREF_DELIVERY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일(참조)")].Text + "' ";
                                    strSql += ", @pSHIP_CUST = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처")].Text + "' ";
                                    strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드")].Text + "' ";
                                    strSql += ", @pSO_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "단위")].Value + "' ";
                                    strSql += ", @pSO_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "수량")].Value + "' ";
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "단가")].Text == "")
                                    {
                                        strSql += ", @pSO_PRICE = 0 ";
                                    }
                                    else
                                    {
                                        strSql += ", @pSO_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "단가")].Value + "' ";
                                    }
                                    strSql += ", @pPRICE_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "단가구분")].Value + "' ";
                                    strSql += ", @pSO_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "금액")].Value + "' ";
                                    strSql += ", @pVAT_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT유형")].Value + "' ";
                                    strSql += ", @pVAT_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT율")].Value + "' ";
                                    strSql += ", @pVAT_INC_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT포함구분")].Value + "' ";
                                    strSql += ", @pVAT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT금액")].Value + "' ";
                                    strSql += ", @pITEM_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "재고단위")].Value + "' ";
                                    strSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "비고")].Text + "' ";
                                    strSql += ", @pSALE_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형")].Value + "' ";
                                    strSql += ", @pEXCH_RATE = '" + dtxtExchRate.Value + "' ";
                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                    
                                    //2013-03-18 국방통합원가 관련 추가
                                    //if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "통합원가대상")].Text == "True") //통합원가대상
                                    if (fpSpread1.Sheets[0].Cells[i, 36].Text == "True") //통합원가대상
                                    {
                                        strSql += ", @pANDCOST_YN = 'Y'";
                                    }

                                    strSql += ", @pDCSN_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "판단번호")].Text + "'";
                                    strSql += ", @pDPRT_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서")].Value + "'";
                                    strSql += ", @pORDR_YEAR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "지시연도")].Text + "'";
                                    strSql += ", @pCALC_DEGR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "산정차수")].Text + "'";
                                    strSql += ", @pCTMF_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "조달업체")].Value + "'";
                                    strSql += ", @pNIIN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "재고번호")].Text + "'";
                                    strSql += ", @pNIIN_ITEM_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "재고번호품명")].Text + "'";
                                    strSql += ", @pCONTRACT_NAME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "계약명")].Text + "'";
                                    strSql += ", @pRPST_ITEM_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번")].Text + "'";
                                    strSql += ", @pRPST_ITEM_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "계약단위")].Value + "'";
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "계약수량")].Text != "")
                                    {
                                        strSql += ", @pRPST_ITEM_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "계약수량")].Value + "'";
                                    }

                                    //if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "방산물자지정유무")].Text == "True") //방산물자지정유무
                                    // 2021.01.11. hma 수정(Start): 항목 순서값으로 처리하지 않고 항목명을 이용하여 처리하도록 함.
                                    //if (fpSpread1.Sheets[0].Cells[i, 50].Text == "True") //방산물자지정유무
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "방산물자지정유무")].Text == "True") //방산물자지정유무
                                    // 2021.01.11. hma 수정(End)
                                    {
                                        strSql += ", @pDNNP_APPN = 'Y'";
                                    }
                                    strSql += ", @pNBMT_BASE_ITEM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "호기정렬기준품목")].Text + "'";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

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

                        /////////////////////////////////////////////// 금액집계 UPDATE 시작 /////////////////////////////////////////////////
                        strInUpFlag = "U";

                        string strSql1 = " usp_SOA004 'I3'";
                        strSql1 += ", @pSO_NO = '" + strAutoSoNo + "' ";
                        strSql1 += ", @pIN_UP_FLAG = '" + strInUpFlag + "' ";
                        strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt1 = SystemBase.DbOpen.TranDataTable(strSql1, dbConn, Trans);
                        ERRCode = dt1.Rows[0][0].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); MSGCode = dt1.Rows[0][1].ToString(); goto Exit; }	// ER 코드 Return시 점프

                        /////////////////////////////////////////////// 호기 정렬 및 UPDATE /////////////////////////////////////////////////
                        string Sql = " usp_SO_NBMT";
                        Sql += " @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                        Sql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        DataTable SoDt = SystemBase.DbOpen.TranDataTable(Sql, dbConn, Trans);

                        ERRCode = SoDt.Rows[0][0].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); MSGCode = SoDt.Rows[0][1].ToString(); goto Exit; }	// ER 코드 Return시 점프

                        Trans.Commit();
                    }
                    catch (Exception e)
                    {
                        SystemBase.Loggers.Log(this.Name, e.ToString());
                        Trans.Rollback();
                        ERRCode = "ER";
                        MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                    }
                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Search();
                        SubSearch(strAutoSoNo);

                        UIForm.FPMake.GridSetFocus(fpSpread2, strAutoSoNo, SystemBase.Base.GridHeadIndex(GHIdx2, "수주번호"));
                        UIForm.FPMake.GridSetFocus(fpSpread1, strSoSeq, SystemBase.Base.GridHeadIndex2(fpSpread1, "수주SEQ"));
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
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0038"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//최소 한건 이상의 DETAIL정보가 존재하지 않으면 등록할 수 없습니다.
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 삭제Row Count 체크 및 삭제여부 체크
		private string DelCheck()
		{
			string chkCode = "OK";
			int delCount = 0;

			for(int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
			{
				if(fpSpread1.Sheets[0].RowHeader.Cells[i,0].Text == "D")
				{
					delCount++;

					if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "수주잔량")].Value.ToString() != fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "수량")].Value.ToString())
					{
						MessageBox.Show(SystemBase.Base.MessageRtn("S0002",Convert.ToString(i + 1)), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
						chkCode = "DELCHK_ER";
						return chkCode;
					}
				}
			}
	
			if(delCount == fpSpread1.Sheets[0].Rows.Count)
			{chkCode = "DELCNT_ER";}

			return chkCode;
		}
		#endregion

        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            //대표품번
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번_2"))
            {
                try
                {
                    WNDW.WNDW005 pu = new WNDW.WNDW005(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "공장")].Value.ToString(), "10", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드")].Text);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번명")].Text = Msgs[3].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대표품번 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //품목코드
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드_2"))
            {
                try
                {
                    WNDW.WNDW005 pu = new WNDW.WNDW005(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "공장")].Value.ToString(), "10", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드")].Text);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목명")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "규격")].Text = Msgs[7].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "재고단위")].Text = Msgs[8].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고")].Text = Msgs[16].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location")].Text = Msgs[17].ToString();

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고명")].Text
                            = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location명")].Text
                            = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                        if (Msgs[5].ToString() == "10")		//제품이면
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형")].Value = "A"; //제품
                        }
                        else if (Msgs[5].ToString() == "30" || Msgs[5].ToString() == "35")		//원자재/부자재이면
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형")].Value = "B"; //상품
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형")].Text = "";
                        }

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //납품처
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처_2"))
            {
                try
                {
                    WNDW.WNDW002 pu = new WNDW.WNDW002(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처")].Text, "");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처명")].Text = Msgs[2].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "납품처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //창고
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "창고_2"))
            {
                try
                {
                    string strQuery = " usp_S_COMMON 'S020', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "공장")].Value + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고 조회");	//창고, LOCATION조회
                    pu.Width = 600;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고명")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location명")].Text = Msgs[3].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "창고 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //Location
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "Location_2"))
            {
                try
                {
                    string strQuery = " usp_S_COMMON 'S022', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고")].Value + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Location 조회");	//LOCATION조회
                    pu.Width = 600;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location명")].Text = Msgs[3].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "Location 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //발행
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "발행"))
            {
                try
                {
                    if (fpSpread1.Sheets[0].Cells[Row, Column].Text != "True")
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "발행일자")].Text = "";
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "Location 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // 2020.01.10. hma 추가(Start): 방사청구매부서 항목을 팝업창에서 선택하도록 함.
            // 방사청구매부서 
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서_2"))
            {
                try
                {
                    string strQuery = " usp_S_COMMON 'S091', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P07001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "방사청구매부서");
                    pu.Width = 500;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청부서명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "방사청구매부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // 2020.01.10. hma 추가(End)
        }
        #endregion

        #region 그리드 상 데이터 변경시 연계데이터 자동입력
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            //대표품번
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번명")].Text
                    = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "' ");
            }

            //품목코드
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드"))
            {
                string Query = " usp_S_COMMON @pTYPE = 'S030', @pCODE = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드")].Text + "', @pNAME = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "공장")].Value + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목명")].Text = dt.Rows[0]["ITEM_NM"].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "규격")].Text = dt.Rows[0]["ITEM_SPEC"].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "재고단위")].Text = dt.Rows[0]["ITEM_UNIT"].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고")].Text = dt.Rows[0]["ISSUED_SL_CD"].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location")].Text = dt.Rows[0]["ISSUED_LOCATION_CD"].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고명")].Text
                        = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location명")].Text
                        = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                    if (dt.Rows[0]["ITEM_ACCT"].ToString() == "10")		//제품이면
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형")].Value = "A"; //제품
                    }
                    else if (dt.Rows[0]["ITEM_ACCT"].ToString() == "30" || dt.Rows[0]["ITEM_ACCT"].ToString() == "35")		//원자재/부자재이면
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형")].Value = "B"; //상품
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형")].Text = "";
                    }
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "규격")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "재고단위")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형")].Text = "";
                }
            }
            //납기일
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일(참조)")].Text
                    = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일")].Text;
            }
            //납품처
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처명")].Text
                    = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
            //창고
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "창고"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고명")].Text
                    = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
            //location
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "Location"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location명")].Text
                    = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }

            double dblQty = 0, dblPrice = 0, dblSoAmt = 0;
            dblQty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "수량")].Value);
            dblPrice = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "단가")].Value);

            //수량
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "수량"))
            {
                double dblOldQty = 0, dblEtcQty = 0; //수주잔량

                //수정일떄만
                if (fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text == "U")
                {
                    //기존 DataBase에 저장되있는 수량 받아오기
                    try
                    {
                        string Query = "SELECT ISNULL(SO_QTY,0) AS SO_QTY FROM S_SO_DETAIL(NOLOCK)";
                        Query += " WHERE CO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' AND SO_NO = '" + strAutoSoNo + "' AND SO_SEQ = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "수주SEQ")].Value + "' ";

                        DataTable QtyDt = SystemBase.DbOpen.NoTranDataTable(Query);
                        if (QtyDt.Rows[0]["SO_QTY"] == null)
                        { dblOldQty = 0; }
                        else
                        { dblOldQty = Convert.ToDouble(QtyDt.Rows[0]["SO_QTY"].ToString()); }

                        dblEtcQty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "수주잔량")].Value);

                        if (dblQty < (dblOldQty - dblEtcQty))
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn("S0001")); //출고된 수량보다 적은 수량입니다. 확인하십시오.
                            fpSpread1.Sheets[0].SetActiveCell(Row, Column);
                            return;
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(f.ToString());
                    }
                }

                //수주금액구하기
                dblSoAmt = dblQty * dblPrice;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "금액")].Value = dblSoAmt;

                //VAT금액구하기
                VatAmt(Row);
            }
            //단가
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "단가"))
            {
                dblSoAmt = dblQty * dblPrice;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "금액")].Value = dblSoAmt;

                //VAT금액구하기
                VatAmt(Row);
            }

            //금액
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "금액"))
            {
                //금액만 수정 가능하므로 금액에서 바로 VAT금액구하기
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT포함구분")].Value.ToString() == "2")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT금액")].Value
                        = Math.Floor(Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "금액")].Value) * (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT율")].Value) * 0.01));
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT금액")].Value
                        = Math.Floor(Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "금액")].Value) - (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "금액")].Value) / 1.1));
                }
            }

            // 2020.01.10. hma 추가(Start)
            // 방사청구매부서
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청부서명")].Text
                    = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서")].Text, " AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "' AND MAJOR_CD = 'D007'");
            }
            // 2020.01.10. hma 추가(End)

            ////납기일자
            //if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일"))
            //{
            //    string NewItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드")].Text;
            //    string ItemCd = "";

            //    string NewDelvDt = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일")].Text;
            //    string DelvDt = "";

            //    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            //    {
            //        DelvDt = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일")].Text;
            //        ItemCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드")].Text;

            //        if (ItemCd == NewItemCd)
            //        {
            //            if (DelvDt == NewDelvDt)
            //            {
            //                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Text
            //                    = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Text;
            //                fpSpread1.Focus();
            //                break;
            //            }
            //            else
            //            {
            //                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Text = "";
            //                fpSpread1.Focus();
            //            }
            //        }
            //    }
            //}
            //생산차수
            //if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수"))
            //{
            //    string NewItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드")].Text;
            //    string ItemCd = "";

            //    string NewDelvDt = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일")].Text;
            //    string NewProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Text;

            //    string DelvDt = "", ProjectSeq = "";

            //    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            //    {
            //        DelvDt = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일")].Text;
            //        ProjectSeq = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Text;

            //        ItemCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드")].Text;

            //        if (ItemCd == NewItemCd)
            //        {
            //            if (DelvDt != "" && ProjectSeq != "")
            //            {
            //                if (DelvDt == NewDelvDt && NewProjectSeq != ProjectSeq)
            //                {
            //                    MessageBox.Show(SystemBase.Base.MessageRtn("S0012"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Text = ProjectSeq;
            //                    fpSpread1.Focus();
            //                    break;
            //                }

            //                if (DelvDt != NewDelvDt && NewProjectSeq == ProjectSeq)
            //                {
            //                    MessageBox.Show(SystemBase.Base.MessageRtn("S0012"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Text = "";
            //                    fpSpread1.Focus();
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}
        }
        #endregion

        #region Vat금액구하기
        private void VatAmt(int Row)
        {
            double dblQty = 0, dblPrice = 0, dblSoAmt = 0;
            dblQty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "수량")].Value);
            dblPrice = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "단가")].Value);

            //금액구하기
            dblSoAmt = dblQty * dblPrice;

            //VAT금액구하기
            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT포함구분")].Value.ToString() == "2")
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT금액")].Value
                    = Math.Floor(dblSoAmt * (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT율")].Value) * 0.01));
            }
            else
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT금액")].Value
                    = Math.Floor(dblSoAmt - (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "금액")].Value) / 1.1));
            }
        }
        #endregion

        #region 공장정보 변경 후 경고 메세지
        private void fpSpread1_ComboCloseUp(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            //공장
            if (e.Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "공장"))
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0037"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//공장정보를 변경할 경우 공장에 소속된 품목과 창고정보가 존재하지 않을수도 있습니다. 확인하시기 바랍니다.
            }
        }
        #endregion

        #region 그리드상 콤보박스 변경시
        private void fpSpread1_ComboSelChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            //VAT유형
            if (e.Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT유형"))
            {
                string Query = " usp_S_COMMON @pTYPE = 'S040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = '" + fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT유형")].Value + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT율")].Value = dt.Rows[0]["REL_CD1"]; }
                else
                { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT율")].Value = 0; }

                //VAT금액구하기
                VatAmt(e.Row);
            }
            //VAT포함구분
            if (e.Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT포함구분"))
            {
                //VAT금액구하기
                VatAmt(e.Row);
            }
        }
        #endregion

        #region 화폐단위 변경시 환율세팅
        private void cboCurrency_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            if (cboCurrency.SelectedValue.ToString() == "KRW")
            {
                dtxtExchRate.Value = 1;
                dtxtExchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
                dtxtExchRate.ReadOnly = true;
            }
            else
            {
                dtxtExchRate.Value = 0;
                dtxtExchRate.BackColor = SystemBase.Validation.Kind_LightCyan;
                dtxtExchRate.ReadOnly = false;
            }
        }
        #endregion

        #region 환율변경시 Detail 자동 업데이트 플래그 변경
        private void dtxtExchRate_TextChanged(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                if (strHead == "")
                { fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U"; }
            }
        }
        #endregion

        #region textBox 코드 입력시 코드명 자동입력
        //주문처 조회조건
        private void txtSSoldCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSSoldCustCd.Text != "")
                {
                    txtSSoldCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSSoldCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSSoldCustNm.Value = "";
                }
            }
            catch { }
        }

        //주문처
        private void txtSoldCustCd_TextChanged(object sender, System.EventArgs e)
        {

            try
            {
                if (txtSoldCustCd.Text != "")
                {
                    txtSoldCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSoldCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                    if (txtCollectCustCd.Text == "")
                    {
                        txtCollectCustCd.Text = txtSoldCustCd.Text;
                    }
                }
                else
                {
                    txtSoldCustNm.Value = "";
                }
            }
            catch { }

        }

        //수금처
        private void txtCollectCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCollectCustCd.Text != "")
                {
                    txtCollectCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCollectCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtCollectCustNm.Value = "";
                }
            }
            catch { }
        }

        //사업코드 조회조건
        private void txtSEntCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSEntCd.Text != "")
                {
                    txtSEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtSEntCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSEntNm.Value = "";
                }
            }
            catch { }
        }

        //사업코드
        private void txtEntCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEntCd.Text != "")
                {
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtEntNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region 팝업창 이벤트
        //주문처 조회조건
        private void btnSSoldCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtSSoldCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSSoldCustCd.Text = Msgs[1].ToString();
                    txtSSoldCustNm.Value = Msgs[2].ToString();
                    txtSSoldCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "주문처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //주문처
        private void btnSoldCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtSoldCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSoldCustCd.Text = Msgs[1].ToString();
                    txtSoldCustNm.Value = Msgs[2].ToString();

                    if (txtCollectCustCd.Text == "")
                    {
                        txtCollectCustCd.Text = txtSoldCustCd.Text;
                    }

                    txtSoldCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "주문처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 수금처
        private void btnCollectCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtCollectCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCollectCustCd.Text = Msgs[1].ToString();
                    txtCollectCustNm.Value = Msgs[2].ToString();
                    txtCollectCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수금처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //사업코드 조회조건
        private void btnSEnt_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSEntCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSEntCd.Text = Msgs[0].ToString();
                    txtSEntNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //사업코드
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
        #endregion

        #region 수주변경 이력등록
        private void btnSoHistory_Click(object sender, System.EventArgs e)
        {
            if (txtSoNo.Text != "")
            {
                SOA004P1 frm = new SOA004P1(txtSoNo.Text);
                frm.ShowDialog();
            }
            else
            {
                //수주번호가 선택되지 않았습니다.
                MessageBox.Show(SystemBase.Base.MessageRtn("B0061", "수주번호"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Warning); 
            }
        }
        #endregion

        #region 통합원가대상, 방산물자지정유무 헤더 체크시 "U" 업데이트
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            //통합원가대상, 방산물자지정유무
            if (e.Column == 36 || e.Column == 48)
            {
                if (e.ColumnHeader == true)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                    {
                        if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "I")
                        {
                            fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                        }
                    }
                }
            }
        }
        #endregion
    }
}
