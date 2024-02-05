#region 작성정보
/*********************************************************************/
// 단위업무명 : 수주등록
// 작 성 자 : 조  홍  태
// 작 성 일 : 2013-02-07
// 작성내용 : 수주등록 및 조회
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
using System.Diagnostics;

namespace SO.SOA002
{
    public partial class SOA002 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string ASSIGN_NO = "";
        string Locking = "";
        int NewFlg = 1;//마스터 데이터 수정여부 0:등록,수정X, 1:등록, 2:수정
        string strAutoSoNo = "";
        string strSearchData = "", strSaveData = ""; //컨트롤 저장 체크 변수
        #endregion

        #region 생성자
        public SOA002(string Assign_NO)
        {
            // 알리미 클릭시- 결제
            ASSIGN_NO = Assign_NO;
            InitializeComponent();
        }

        public SOA002(string So_No, string Div)
        {
            // 알리미 클릭시- 알리미
            strAutoSoNo = So_No;
            InitializeComponent();
        }

        public SOA002()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SOA002_Load(object sender, System.EventArgs e)
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

            //G1Etc[SystemBase.Base.GridHeadIndex2(fpSpread1, "통합원가대상")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM2', @pCODE = 'B029', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//통합원가대상
            G1Etc[SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM2', @pCODE = 'D007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//방사청구매부서
            G1Etc[SystemBase.Base.GridHeadIndex2(fpSpread1, "조달업체")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM2', @pCODE = 'D006', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//조달업체
            G1Etc[SystemBase.Base.GridHeadIndex2(fpSpread1, "계약단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM2', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//계약단위
            //G1Etc[SystemBase.Base.GridHeadIndex2(fpSpread1, "방산물자지정유무")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM2', @pCODE = 'B029', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//방산물자지정유무

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //폼 컨트롤 초기화
            Control_Setting();

            dtpSDelvDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpSDelvDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            rdoAll.Checked = true;

            if (ASSIGN_NO != "")
            {
                strAutoSoNo = SystemBase.Base.CodeName("ASSIGN_NO", "SO_NO", "S_SO_MASTER", ASSIGN_NO, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                SubSearch(strAutoSoNo);
            }
        }
        #endregion

        #region ControlSetting()
        private void Control_Setting()
        {
            //기타 세팅
            dtpSoDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpCustPoDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpLastDeliveryDt.Text = SystemBase.Base.ServerTime("YYMMDD");
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

            //확정버튼 Disable
            btnConfirmOk.Enabled = false;
            btnConfirmCancel.Enabled = false;

            //출문증발행번호지정
            btnDnYMD.Enabled = false;
            txtYMD.ReadOnly = true;
            txtYMD.BackColor = SystemBase.Validation.Kind_Gainsboro;
            cboYMD.Enabled = false;
            cboYMD.BackColor = SystemBase.Validation.Kind_Gainsboro;

            txtYMD.Value = "";
            cboYMD.Text = "";
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            NewFlg = 1;
            strAutoSoNo = "";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;

            //폼 컨트롤 초기화
            Control_Setting();

            dtpSDelvDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpSDelvDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            rdoAll.Checked = true;
            chkGoverment.Checked = false;
            
            //프린트 버튼 활성화여부
            UIForm.Buttons.ReButton(BtnPrint, "BtnPrint", false);
        }
        #endregion

        #region 행삭제 버튼 클릭 이벤트
        protected override void DelExec()
        {	// 행 삭제
            //확정상태가 아니면
            if (chkConfirm.Checked == true)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//확정된 데이터는 다른 작업을 할 수 없습니다.
                return;
            }

            try
            {
                UIForm.FPMake.RowRemove(fpSpread1);
                DelExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            //확정상태가 아니면
            if (chkConfirm.Checked == false)
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
                    + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "발행일자") + "|3");
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
            //확정상태가 아니면
            if (chkConfirm.Checked == false)
            {
                UIForm.FPMake.RowCopy(fpSpread1);

                //창고 흰색이면서 Locking
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고")].Locked = true;
                }

                UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex,
                    SystemBase.Base.GridHeadIndex2(fpSpread1, "발행") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "발행일자") + "|3");
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//확정된 데이터는 다른 작업을 할 수 없습니다.
            }
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            this.Cursor = Cursors.WaitCursor;

            //확정상태가 아니면
            if (chkConfirm.Checked == true)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//확정된 데이터는 다른 작업을 할 수 없습니다.
                this.Cursor = Cursors.Default;
                return;
            }

            string msg = SystemBase.Base.MessageRtn("B0027");
            DialogResult dsMsg = MessageBox.Show(msg, "삭제확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_SOA002  'D1'";
                    strSql += ", @pSO_NO = '" + strAutoSoNo + "' ";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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
                    ERRCode = "ER";
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Search();
                    fpSpread1.Sheets[0].RowCount = 0;
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

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SearchExec() Master 그리드 조회 로직
        protected override void SearchExec()
        {
            //마스터만 조회
            Search();

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //폼 컨트롤 초기화
            Control_Setting();

            //프린트 버튼 활성화여부
            UIForm.Buttons.ReButton(BtnPrint, "BtnPrint", false);

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

                    string strCfmYn = "";
                    if (rdoYes.Checked == true) { strCfmYn = "Y"; }
                    else if (rdoNo.Checked == true) { strCfmYn = "N"; }
                    else { strCfmYn = ""; }

                    string strQuery = " usp_SOA002  @pTYPE = 'S1'";
                    strQuery += ", @pSO_DT_FR = '" + dtpSSoDtFr.Text + "' ";
                    strQuery += ", @pSO_DT_TO = '" + dtpSSoDtTo.Text + "' ";
                    strQuery += ", @pDELV_DT_FR = '" + dtpSDelvDtFr.Text + "' ";
                    strQuery += ", @pDELV_DT_TO = '" + dtpSDelvDtTo.Text + "' ";
                    strQuery += ", @pENT_CD = '" + txtSEntCd.Text + "' ";
                    strQuery += ", @pSALE_DUTY = '" + cboSSaleDuty.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSO_TYPE = '" + cboSSoType.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSOLD_CUST = '" + txtSSoldCustCd.Text + "' ";
                    strQuery += ", @pSOLD_CUST_NM = '" + txtSSoldCustNm.Text + "' ";
                    strQuery += ", @pSO_CONFIRM_YN = '" + strCfmYn + "' ";
                    strQuery += ", @pSO_NO = '" + txtSSoNo.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pREF_DELV_DT_FR = '" + dtpRefDelvDtFr.Text + "' ";      // 2017.10.31. hma 추가: 납기일(참조) FROM
                    strQuery += ", @pREF_DELV_DT_TO = '" + dtpRefDelvDtTo.Text + "' ";      // 2017.10.31. hma 추가: 납기일(참조) TO

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
                Stopwatch stopwatch = new Stopwatch(); //객체 선언
                stopwatch.Start(); // 시간측정 시작

                SystemBase.Validation.GroupBox_Reset(groupBox2);

                fpSpread1.Sheets[0].Rows.Count = 0;

                //수주Master정보
                string strSql = " usp_SOA002  'S2' ";
                strSql = strSql + ", @pSO_NO ='" + strCode + "' ";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                if (dt.Rows.Count > 0)
                {
                    NewFlg = 2;

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
                    //chkAndCost.Checked = Andcost;
                    chkStdItemYN.Checked = bStdItemYN;      // 2017.03.17. hma 추가: 표준품목적용여부

                    //결재번호
                    ASSIGN_NO = dt.Rows[0]["ASSIGN_NO"].ToString();

                    if (dt.Rows[0]["CURRENCY"].ToString() == "KRW")
                    {
                        //dtxtExchRate.Value = 1;
                        dtxtExchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
                        dtxtExchRate.ReadOnly = true;
                    }
                    else
                    {
                        dtxtExchRate.BackColor = SystemBase.Validation.Kind_LightCyan;
                        dtxtExchRate.ReadOnly = false;
                    }

                    //2013-03-18 국방통합원가 관련 추가
                    //dtpOrdrYear.Value = dt.Rows[0]["ORDR_YEAR"].ToString() + "-01-01";
                    //txtDcsnNumb.Value = dt.Rows[0]["DCSN_NUMB"].ToString();
                    //txtCalcDegr.Value = dt.Rows[0]["CALC_DEGR"].ToString();
                    //cboDprtCode.SelectedValue = dt.Rows[0]["DPRT_CODE"].ToString();
                    //cboCtmfCode.SelectedValue = dt.Rows[0]["CTMF_CODE"].ToString();

                    //현재 row값 설정
                    PreRow = fpSpread2.ActiveSheet.ActiveRowIndex;

                    SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2); //Key값 컨트롤 세팅

                    //컨트롤 체크값 초기화
                    strSearchData = "";
                    //컨트롤 체크 함수
                    GroupBox[] gBox = new GroupBox[] { groupBox2};
                    SystemBase.Validation.Control_Check(gBox, ref strSearchData);

                    //수주Detail그리드 정보.
                    string strSql1 = " usp_SOA002  'S3' ";
                    strSql1 += ", @pSO_NO ='" + strCode + "' ";
                    strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    Locking = dt.Rows[0]["LOCK_YN"].ToString(); //결재여부

                    Set_Lock_yn(Locking); //확정,결재여부에 따른 그리드 Lock
                }

                stopwatch.Stop(); //시간측정 끝
                MessageBox.Show("Time : " + stopwatch.ElapsedMilliseconds.ToString() + "ms");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Set_Lock_yn 확정,결재여부에 따른 Locking
        private void Set_Lock_yn(string strLock)
        {
            // 성능개선 
            Dictionary<int, int> dicKind = new Dictionary<int, int>();

            int close_yn_idx = SystemBase.Base.GridHeadIndex2(fpSpread1, "마감여부");
            int dn_yn_idx = SystemBase.Base.GridHeadIndex2(fpSpread1, "출하여부");
            int dn_cnt_idx = SystemBase.Base.GridHeadIndex2(fpSpread1, "출고수");
            int bn_cnt_idx = SystemBase.Base.GridHeadIndex2(fpSpread1, "매출수");
            int dn_idx1 = SystemBase.Base.GridHeadIndex2(fpSpread1, "발행");

            //확정여부에 따른 화면 Locking
            if (chkConfirm.Checked == true || strLock == "Y")
            {
                SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

                string strChkQuery = "";

                //양산이나 a/s품목이면 MPS상태 체크
                if (cboContractType.SelectedValue.ToString() == "A01" || cboContractType.SelectedValue.ToString() == "C01")
                {
                    strChkQuery = " SELECT 1 FROM P_MPS_REGISTER(NOLOCK) WHERE PROJECT_NO = '" + txtProjectNo.Text + "' AND STATUS <> 'P' ";
                }
                else //창정비나 개발품이면 작업지시상태 체크
                {
                    strChkQuery = " SELECT 1 FROM P_WORKORDER_MASTER(NOLOCK) WHERE PROJECT_NO = '" + txtProjectNo.Text + "' AND ORDER_STATUS <> 'RL' ";
                }

                DataTable ChkDt = SystemBase.DbOpen.NoTranDataTable(strChkQuery);

                if (Convert.ToInt32(cboSoStatus.SelectedValue.ToString()) > 0 || ChkDt.Rows.Count > 0 || strLock == "Y")
                {
                    btnConfirmOk.Enabled = false;
                    btnConfirmCancel.Enabled = false;
                }
                else
                {
                    btnConfirmOk.Enabled = false;
                    btnConfirmCancel.Enabled = true;
                }
                
                #region 2024-01-22 성능 개선 by CYJ
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드_2"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "단위"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "수량"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "단가"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "단가구분"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "금액"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT금액"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "공장"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처_2"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT유형"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT포함구분"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "창고"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "창고_2"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "Location"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "Location_2"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "비고"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "통합원가대상"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "판단번호"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "지시연도"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "산정차수"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "조달업체"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "계약명"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번_2"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "계약단위"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "계약수량"), 3);
                dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "방산물자지정유무"), 3);

                UIForm.FPMake.grdReMake(fpSpread1, dicKind);

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, dn_yn_idx].Text == "Y" || fpSpread1.Sheets[0].Cells[i, close_yn_idx].Text == "Y" ||
                        Convert.ToInt16(fpSpread1.Sheets[0].Cells[i, dn_cnt_idx].Text) > 0 || Convert.ToInt16(fpSpread1.Sheets[0].Cells[i, bn_cnt_idx].Text) > 0)
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i, dn_idx1 + "|3");
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i, dn_idx1 + "|0");
                    }
                }
                #endregion
                
                /*
                #region 과거 소스
				//Detail Locking설정
				for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
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
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "공장") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT유형") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT포함구분") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "창고") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "창고_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "Location") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "Location_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "비고") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "통합원가대상") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "판단번호") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "지시연도") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "산정차수") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "조달업체") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "계약명") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "계약단위") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "계약수량") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "방산물자지정유무") + "|3"

                        );
                    if (fpSpread1.Sheets[0].Cells[i, dn_yn_idx].Text == "Y" || fpSpread1.Sheets[0].Cells[i, close_yn_idx].Text == "Y" ||
                        Convert.ToInt16(fpSpread1.Sheets[0].Cells[i, dn_cnt_idx].Text) > 0 || Convert.ToInt16(fpSpread1.Sheets[0].Cells[i, bn_cnt_idx].Text) > 0)
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i, dn_idx1 + "|3");
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i, dn_idx1 + "|0");
                    }
                }
                #endregion
                */
            }
            else
            {
                SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

                btnConfirmOk.Enabled = true;

                if (chkConfirm.Checked == true)
                {
                    btnConfirmOk.Enabled = false;
                }

                btnConfirmCancel.Enabled = false;
                txtSoNo.ReadOnly = true;
                txtSoNo.BackColor = SystemBase.Validation.Kind_Gainsboro;

                if (cboCurrency.Text.ToString() == "KRW")
                {
                    dtxtExchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
                    dtxtExchRate.ReadOnly = true;
                }
                else
                {
                    dtxtExchRate.BackColor = SystemBase.Validation.Kind_LightCyan;
                    dtxtExchRate.ReadOnly = false;
                }
                
                #region 2024-01-22 성능 개선 by CYJ
                if (chkConfirm.Checked == true)
				{
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드_2"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "단위"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "수량"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "단가"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "단가구분"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "금액"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT금액"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "공장"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처_2"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT유형"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT포함구분"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "창고"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "창고_2"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "Location"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "Location_2"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "비고"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "통합원가대상"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "판단번호"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "지시연도"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "산정차수"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "조달업체"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "계약명"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번_2"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "계약단위"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "계약수량"), 3);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "방산물자지정유무"), 3);
                }
				else
				{
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드_2"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "단위"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "수량"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "단가"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "단가구분"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "금액"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT금액"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "공장"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처_2"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT유형"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT포함구분"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "창고"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "창고_2"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "Location"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "Location_2"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "비고"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "통합원가대상"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "판단번호"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "지시연도"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "산정차수"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "조달업체"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "계약명"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번_2"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "계약단위"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "계약수량"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex2(fpSpread1, "방산물자지정유무"), 0);
                }
                
                UIForm.FPMake.grdReMake(fpSpread1, dicKind);
                
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, dn_yn_idx].Text == "Y" || fpSpread1.Sheets[0].Cells[i, close_yn_idx].Text == "Y" ||
                        Convert.ToInt16(fpSpread1.Sheets[0].Cells[i, dn_cnt_idx].Text) > 0 || Convert.ToInt16(fpSpread1.Sheets[0].Cells[i, bn_cnt_idx].Text) > 0)
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i, dn_idx1 + "|3");
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i, dn_idx1 + "|0");
                    }
                }
                #endregion
                /*
                #region 과거 소스
                //Detail Locking해제
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (chkConfirm.Checked == true)
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드_2") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "단위") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "수량") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "단가") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "단가구분") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "금액") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT금액") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "공장") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT유형") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT포함구분") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "창고") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "창고_2") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "Location") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "Location_2") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "비고") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형") + "|0"

                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "통합원가대상") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "판단번호") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "지시연도") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "산정차수") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "조달업체") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "계약명") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번_2") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "계약단위") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "계약수량") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "방산물자지정유무") + "|3"

                            );
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "단위") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "수량") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "단가") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "단가구분") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "금액") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT금액") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "공장") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "납품처_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT유형") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "VAT포함구분") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "창고") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "창고_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "Location") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "Location_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "비고") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "매출유형") + "|0"

                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "통합원가대상") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "판단번호") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "지시연도") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "산정차수") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "조달업체") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "계약명") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "계약단위") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "계약수량") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex2(fpSpread1, "방산물자지정유무") + "|0"
                            );
                    }

                    if (fpSpread1.Sheets[0].Cells[i, dn_yn_idx].Text == "Y" || fpSpread1.Sheets[0].Cells[i, close_yn_idx].Text == "Y" ||
                        Convert.ToInt16(fpSpread1.Sheets[0].Cells[i, dn_cnt_idx].Text) > 0 || Convert.ToInt16(fpSpread1.Sheets[0].Cells[i, bn_cnt_idx].Text) > 0)
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i, dn_idx1 + "|3");
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i, dn_idx1 + "|0");
                    }

                }
                #endregion
                */

                txtProjectNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtProjectNo.ReadOnly = true;
                txtProjectNo.Tag = ";2;;";
            }

            if (fpSpread1.Sheets[0].RowCount == 0)
            {
                btnDnYMD.Enabled = false;
                txtYMD.ReadOnly = true;
                txtYMD.BackColor = SystemBase.Validation.Kind_Gainsboro;
                cboYMD.Enabled = false;
                cboYMD.BackColor = SystemBase.Validation.Kind_Gainsboro;

                txtYMD.Value = "";
                cboYMD.Text = "";

                txtProjectNo.BackColor = SystemBase.Validation.Kind_LightCyan;
                txtProjectNo.ReadOnly = false;
                txtProjectNo.Tag = "프로젝트번호;1;;";

            }
            else if (fpSpread1.Sheets[0].Cells[0, dn_yn_idx].Text == "Y")
            {
                btnDnYMD.Enabled = false;
                txtYMD.ReadOnly = true;
                txtYMD.BackColor = SystemBase.Validation.Kind_Gainsboro;
                cboYMD.Enabled = false;
                cboYMD.BackColor = SystemBase.Validation.Kind_Gainsboro;

                txtYMD.Value = "";
                cboYMD.Text = "";

                UIForm.Buttons.ReButton(BtnPrint, "BtnPrint", false);

            }
            else
            {
                btnDnYMD.Enabled = true;
                txtYMD.ReadOnly = false;
                txtYMD.BackColor = SystemBase.Validation.Kind_White;
                cboYMD.Enabled = true;
                cboYMD.BackColor = SystemBase.Validation.Kind_White;

                txtYMD.Value = "";
                SystemBase.ComboMake.C1Combo(cboYMD, " usp_SOA002  'C1', @pSO_NO = '" + txtSoNo.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);//수주형태

                UIForm.Buttons.ReButton(BtnPrint, "BtnPrint", true);
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

            string strMstType = "";
            string strInUpFlag = "I";

            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////
            //확정상태가 아니면
            if (chkConfirm.Checked == false)
            {
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
                            if (NewFlg != 0)
                            {
                                string chkBondedYn = "";

                                if (chkBonded.Checked == true) { chkBondedYn = "Y"; }
                                else { chkBondedYn = "N"; }

                                string chkNeedQtyBondYn = "";
                                if (chkNeedQtyBond.Checked == true) { chkNeedQtyBondYn = "Y"; }
                                else { chkNeedQtyBondYn = "N"; }

                                string chkUnifyContractYn = "";
                                if (chkUnifyContract.Checked == true) { chkUnifyContractYn = "Y"; }
                                else { chkUnifyContractYn = "N"; }

                                string chkGovermentYn = "";
                                if (chkGoverment.Checked == true) { chkGovermentYn = "Y"; }
                                else { chkGovermentYn = "N"; }

                                //string chkAndCostYn = "";
                                //if (chkAndCost.Checked == true) { chkAndCostYn = "Y"; }
                                //else { chkAndCostYn = "N"; }

                                // 2017.03.17. hma 추가(Start): 표준품목적용여부
                                string strStdItemYN = "";
                                if (chkStdItemYN.Checked == true) { strStdItemYN = "Y"; }
                                else { strStdItemYN = "N"; }
                                // 2017.03.17. hma 추가(End)

                                if (NewFlg == 1) { strMstType = "I1"; }
                                else { strMstType = "U1"; }

                                string strSql = " usp_SOA002 '" + strMstType + "'";
                                strSql += ", @pSO_NO = '" + txtProjectNo.Text + "' "; //퍼스텍 요청에 의하여 수주번호는 프로젝트번호랑 같은걸로 입력 2010-01-21 CHT
                                strSql += ", @pENT_CD = '" + txtEntCd.Text + "' ";
                                strSql += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                                strSql += ", @pPROJECT_NM = '" + txtProjectNm.Text + "' ";
                                strSql += ", @pSO_DT = '" + dtpSoDt.Text + "' ";
                                strSql += ", @pSO_TYPE = '" + cboSoType.SelectedValue.ToString() + "' ";
                                if (dtpLastDeliveryDt.Text != "")
                                    strSql += ", @pLAST_DELIVERY_DT = '" + dtpLastDeliveryDt.Text + "' ";
                                strSql += ", @pCUST_PO_NO = '" + txtCustPoNo.Text + "' ";
                                if (dtpCustPoDt.Text != "")
                                    strSql += ", @pCUST_PO_DT = '" + dtpCustPoDt.Text + "' ";
                                strSql += ", @pSOLD_CUST = '" + txtSoldCustCd.Text + "' ";
                                strSql += ", @pCOLLECT_CUST = '" + txtCollectCustCd.Text + "' ";
                                strSql += ", @pSALE_DUTY = '" + cboSaleDuty.SelectedValue.ToString() + "' ";
                                strSql += ", @pPAYMENT_METH = '" + cboPaymentMeth.SelectedValue.ToString() + "' ";
                                strSql += ", @pPAYMENT_TERM = '" + dtxtPaymentTerm.Value + "' ";
                                strSql += ", @pPAYMENT_TERM_REMARK = '" + txtPaymentRemark.Text + "' ";
                                strSql += ", @pCURRENCY = '" + cboCurrency.SelectedValue.ToString() + "' ";
                                strSql += ", @pEXCH_RATE = '" + dtxtExchRate.Value + "' ";
                                strSql += ", @pBONDED_YN = '" + chkBondedYn + "' ";
                                strSql += ", @pSO_STATUS = '" + cboSoStatus.SelectedValue.ToString() + "' ";
                                strSql += ", @pCONTRACT_TYPE = '" + cboContractType.SelectedValue.ToString() + "' ";
                                strSql += ", @pCALC_TYPE = '" + cboCalcType.SelectedValue.ToString() + "' ";
                                strSql += ", @pREMARK = '" + txtRemark.Text + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pNEED_QTY_BOND_YN = '" + chkNeedQtyBondYn + "'";
                                strSql += ", @pCONTRACT_FORM = '" + cboContractForm.SelectedValue.ToString() + "'";
                                strSql += ", @pUNITY_CONTRACT_YN  = '" + chkUnifyContractYn + "'";
                                strSql += ", @pGOVERMENT_YN  = '" + chkGovermentYn + "'";
                                //strSql += ", @pANDCOST_YN  = '" + chkAndCostYn + "'";
                                strSql += ", @pSTD_ITEM_YN  = '" + strStdItemYN + "'";     // 2017.03.17. hma 추가: 표준품목적용여부
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                //2013-03-18 국방통합원가 관련 추가
                                //strSql += ", @pORDR_YEAR = '" + dtpOrdrYear.Text + "' ";
                                //strSql += ", @pDCSN_NUMB = '" + txtDcsnNumb.Text + "' ";
                                //strSql += ", @pCALC_DEGR = '" + txtCalcDegr.Text + "' ";
                                //strSql += ", @pDPRT_CODE = '" + cboDprtCode.SelectedValue.ToString() + "'";
                                //strSql += ", @pCTMF_CODE = '" + cboCtmfCode.SelectedValue.ToString() + "'";							

                                DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                                ERRCode = dt.Rows[0][0].ToString();
                                MSGCode = dt.Rows[0][1].ToString();
                                strAutoSoNo = dt.Rows[0][2].ToString();
                                ChkSoNo = dt.Rows[0][2].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }

                            /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                            //그리드 상단 필수 체크
                            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                            {
                                //Detail정보를 모두 삭제할 경우 Master정보를 삭제할지 물어보고 아니면 취소한다.
                                if (DelCheck() == false)
                                {
                                    string msg = SystemBase.Base.MessageRtn("B0027");
                                    DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                    if (dsMsg == DialogResult.Yes)
                                    {
                                        try
                                        {
                                            string strDelSql = " usp_SOA002  'D1'";
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

                                        string strSql = " usp_SOA002 '" + strGbn + "'";
                                        strSql += ", @pSO_NO = '" + strAutoSoNo + "' ";
                                        strSql += ", @pSO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "수주SEQ")].Value + "' ";
                                        strSql += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                                        strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Value + "' ";
                                        strSql += ", @pPLANT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "공장")].Value + "' ";
                                        strSql += ", @pSL_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "창고")].Text + "' ";
                                        strSql += ", @pLOCATION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "Location")].Text + "' ";
                                        strSql += ", @pDELIVERY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일")].Text + "' ";
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

                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "발행일자")].Text.Trim() != "")
                                            strSql += ", @pDN_PLAN_YMD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "발행일자")].Text.Replace("-", "") + "' ";

                                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                        //2013-03-18 국방통합원가 관련 추가
                                        if (fpSpread1.Sheets[0].Cells[i, 39].Text == "True") //통합원가대상
                                        {
                                            strSql += ", @pANDCOST_YN = 'Y'";
                                        }

                                        strSql += ", @pDCSN_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "판단번호")].Text + "'";
                                        strSql += ", @pDPRT_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "방사청구매부서")].Value + "'";
                                        strSql += ", @pORDR_YEAR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "지시연도")].Text + "'";
                                        strSql += ", @pCALC_DEGR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "산정차수")].Text + "'";
                                        strSql += ", @pCTMF_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "조달업체")].Value + "'";
                                        strSql += ", @pCONTRACT_NAME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "계약명")].Text + "'";
                                        strSql += ", @pRPST_ITEM_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번")].Text + "'";
                                        strSql += ", @pRPST_ITEM_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "계약단위")].Value + "'";
                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "계약수량")].Text != "")
                                        {
                                            strSql += ", @pRPST_ITEM_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "계약수량")].Value + "'";
                                        }

                                        if (fpSpread1.Sheets[0].Cells[i, 51].Text == "True") //방산물자지정유무
                                        {
                                            strSql += ", @pDNNP_APPN = 'Y'";
                                        }

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
                            if (NewFlg == 1) { strInUpFlag = "I"; }
                            else { strInUpFlag = "U"; }

                            string strSql1 = " usp_SOA002 'I3'";
                            strSql1 += ", @pSO_NO = '" + strAutoSoNo + "' ";
                            strSql1 += ", @pIN_UP_FLAG = '" + strInUpFlag + "' ";
                            strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt1 = SystemBase.DbOpen.TranDataTable(strSql1, dbConn, Trans);
                            ERRCode = dt1.Rows[0][0].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); MSGCode = dt1.Rows[0][1].ToString(); goto Exit; }	// ER 코드 Return시 점프

                            /////////////////////////////////////////////// 호기 정렬 및 UPDATE /////////////////////////////////////////////////
                            string Sql = " usp_SO_NBMT";
                            Sql += " @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                            Sql += " , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            DataTable SoDt = SystemBase.DbOpen.TranDataTable(Sql, dbConn, Trans);

                            ERRCode = SoDt.Rows[0][0].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); MSGCode = SoDt.Rows[0][1].ToString(); goto Exit; }	// ER 코드 Return시 점프


                            Trans.Commit();
                        }
                        catch (Exception e)
                        {
                            SystemBase.Loggers.Log(this.Name, e.ToString());
                            Trans.Rollback();
                            this.Cursor = Cursors.Default;
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
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//확정된 데이터는 다른 작업을 할 수 없습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 삭제Row Count 체크
        private bool DelCheck()
        {
            bool delChk = true;
            int delCount = 0;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                {
                    delCount++;
                }
            }

            if (delCount == fpSpread1.Sheets[0].Rows.Count)
            { delChk = false; }

            return delChk;
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
        }
        #endregion

        #region 그리드 상 데이터 변경시 연계데이터 자동입력
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {

            //대표품번
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번명")].Text
                    = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "대표품번")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ");
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

            //납기일자
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일"))
            {
                string NewItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드")].Text;
                string ItemCd = "";

                string NewDelvDt = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일")].Text;
                string DelvDt = "";

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    DelvDt = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일")].Text;
                    ItemCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드")].Text;

                    if (ItemCd == NewItemCd)
                    {
                        if (DelvDt == NewDelvDt)
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Text
                                = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Text;
                            fpSpread1.Focus();
                            break;
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Text = "";
                            fpSpread1.Focus();
                        }
                    }
                }
            }
            //생산차수
            if (Column == SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수"))
            {
                string NewItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드")].Text;
                string ItemCd = "";

                string NewDelvDt = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일")].Text;
                string NewProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Text;

                string DelvDt = "", ProjectSeq = "";

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    DelvDt = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "납기일")].Text;
                    ProjectSeq = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Text;

                    ItemCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "품목코드")].Text;

                    if (ItemCd == NewItemCd)
                    {
                        if (DelvDt != "" && ProjectSeq != "")
                        {
                            if (DelvDt == NewDelvDt && NewProjectSeq != ProjectSeq)
                            {
                                MessageBox.Show(SystemBase.Base.MessageRtn("S0012"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Text = ProjectSeq;
                                fpSpread1.Focus();
                                break;
                            }

                            if (DelvDt != NewDelvDt && NewProjectSeq == ProjectSeq)
                            {
                                MessageBox.Show(SystemBase.Base.MessageRtn("S0012"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex2(fpSpread1, "생산차수")].Text = "";
                                fpSpread1.Focus();
                                break;
                            }
                        }
                    }
                }
            }
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

        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
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

        //품목코드
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW005 pu = new WNDW.WNDW005("10");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        #region 확정,확정취소
        private void Confirm(string strConfirmYn, string Dt)
        {
            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_SOA002  'P1'";
                strSql += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD.ToString() + "' ";
                strSql += ", @pSO_NO = '" + strAutoSoNo + "' ";
                strSql += ", @pSO_CONFIRM_YN = '" + strConfirmYn + "' ";
                strSql += ", @pENT_CD = '" + txtEntCd.Text + "' ";
                strSql += ", @pC_DATE = '" + Dt + "' ";
                strSql += ", @pCONTRACT_TYPE = '" + cboContractType.SelectedValue.ToString() + "' ";
                strSql += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "' ";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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
                ERRCode = "ER";
                MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SubSearch(strAutoSoNo);
            }
            else if (ERRCode == "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            fpSpread2.Focus();
        }

        private void btnConfirmOk_Click(object sender, System.EventArgs e)
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY068", "프로젝트 " + strAutoSoNo + " "), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                Confirm("Y", "");
            }
        }

        private void btnConfirmCancel_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strChkQuery = "", msgCode = "";

                if (cboContractType.SelectedValue.ToString() == "A01" || cboContractType.SelectedValue.ToString() == "C01")
                {
                    //양산이나 a/s품목이면 MPS상태 체크

                    strChkQuery = " SELECT 1 FROM P_MPS_REGISTER(NOLOCK) WHERE PROJECT_NO = '" + txtProjectNo.Text + "' AND STATUS <> 'P' ";
                    msgCode = "S0010";

                    DataTable ChkDt = SystemBase.DbOpen.NoTranDataTable(strChkQuery);

                    if (ChkDt.Rows.Count == 0)
                    {
                        string msg = SystemBase.Base.MessageRtn(msgCode, txtProjectNo.Text + "#\n");
                        DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (dsMsg == DialogResult.Yes)
                        {
                            Confirm("N", "");
                        }
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("S0014"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    Confirm("N", "");
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "확정취소"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 출문증발행번호지정
        private void btnDnYMD_Click(object sender, System.EventArgs e)
        {
            string Query = " usp_SOA002  'C2', @pSO_NO = '" + txtSoNo.Text + "', @pDN_PLAN_YMD = '" + txtYMD.Text.Replace("-", "") + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("발행일자가 존재합니다!", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtYMD.Focus();
            }
            else
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "발행")].Text == "True" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "발행")].Locked == false)
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex2(fpSpread1, "발행일자")].Text = txtYMD.Text;
                        UIForm.FPMake.fpChange(fpSpread1, i);
                    }
                }
            }
        }
        #endregion

        #region 레포트 출력
        protected override void PrintExec()
        {
            if (cboYMD.Text == "")
            {
                MessageBox.Show("발행일자를 선택하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboYMD.Focus();
                return;
            }
            else if (txtSoNo.Text != "")
            {		
                string RptName = SystemBase.Base.ProgramWhere + @"\Report\SOA002.rpt";    // 레포트경로+레포트명
                string[] RptParmValue = new string[3];   // SP 파라메타 값

                RptParmValue[0] = "R1";
                RptParmValue[1] = txtSoNo.Text;
                RptParmValue[2] = Convert.ToString(cboYMD.SelectedValue).Replace("-","");

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, RptName, RptParmValue); //공통크리스탈 10버전				
                frm.ShowDialog();
            }
        }
        #endregion

        #region 통합원가대상, 방산물자지정유무 헤더 체크시 "U" 업데이트
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            //통합원가대상, 방산물자지정유무
            if (e.Column == 39 || e.Column == 51)
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
