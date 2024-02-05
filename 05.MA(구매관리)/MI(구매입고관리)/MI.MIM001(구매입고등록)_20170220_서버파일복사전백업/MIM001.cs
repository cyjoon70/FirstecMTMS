#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매입고등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-01
// 작성내용 : 구매입고등록 및 관리
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
using System.Drawing.Imaging;
using System.Drawing.Printing;

using System.Data.SqlClient;

namespace MI.MIM001
{
    public partial class MIM001 : UIForm.FPCOMM2
    {

        #region 변수
        string strAutoMvmtNo = "";	// 입고번호
		string strAutoMvmtSeq = "";	// 입고순번
        string strBtn = "N";
        bool btnNew_is = true;
        bool form_act_chk = false;
        string strSts = "";
		DataTable dt = new DataTable();
		DataTable dtPrint = new DataTable();
		bool bSaveLot = true;

		// Lot 분할/수정/삭제 팝업에서 Lot 수량을 변경 적용 후, Parent Form 입고수량을 수정해 주어야 하고 이때 불필요한 확인 메시지는 나타나지 않게 한다.
		bool bMsgYN = true; 
        #endregion

        #region 생성자
        public MIM001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void MIM001_Load(object sender, System.EventArgs e)
        {

            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //DETAIL
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "검사구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'M007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//검사구분
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단가구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'S011', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단가구분

			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 6, false);

			// 프린터 포트 ComboBox 설정
			SystemBase.RawPrinterHelper.SetPortCombo(cboPort);

            //기타 세팅
            dtpSMvmtDtFo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 10);
            dtpSMvmtDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpMvmtDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            strAutoMvmtNo = "";

            butRef.Enabled = true;
            btnIspPrint.Enabled = false;

			if (string.IsNullOrEmpty(strAutoMvmtNo) == true)
			{
				btnPrintAll.Enabled = false;
				btnAttView.Enabled = false;
			}
			else
			{
				btnPrintAll.Enabled = true;
				btnAttView.Enabled = true;
			}

        }
        #endregion

		#region NewExec() New 버튼 클릭 이벤트
		protected override void NewExec()
        {

			bSaveLot = true; 
			dt.Clear();

            if (btnNew_is)
            {
                SystemBase.Validation.GroupBox_Reset(groupBox1);
                //기타 세팅
                dtpSMvmtDtFo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().ToString().Substring(0, 10);
                dtpSMvmtDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            }

            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅


            dtpMvmtDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            strAutoMvmtNo = "";

            butRef.Enabled = true;
            butScm.Enabled = true;
            btnIspPrint.Enabled = false;
            fpSpread2.Sheets[0].Rows.Count = 0;

			// 프린터 포트 ComboBox 설정
			SystemBase.RawPrinterHelper.SetPortCombo(cboPort);

			if (string.IsNullOrEmpty(strAutoMvmtNo) == true)
			{
				btnPrintAll.Enabled = false;
				btnAttView.Enabled = false;
			}
			else
			{
				btnPrintAll.Enabled = true;
				btnAttView.Enabled = true;
			}

        }
        #endregion

        #region SearchExec() Master 그리드 조회 로직
        protected override void SearchExec()
        {
			dt.Clear();
            Search("");
        }

        private void Search(string strMvmtNo)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_MIM001  @pTYPE = 'S1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pMVMT_DT_FR = '" + dtpSMvmtDtFo.Text + "' ";
                strQuery += ", @pMVMT_DT_TO = '" + dtpSMvmtDtTo.Text + "' ";
                strQuery += ", @pCUST_CD = '" + txtSCustCd.Text + "' ";
                strQuery += ", @pPUR_DUTY = '" + txtSPurDuty.Text + "' ";
                strQuery += ", @pIO_TYPE = '" + txtSIoType.Text + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtSProjectSeq.Text + "' ";
                strQuery += ", @pMVMT_NO = '" + txtSMvmtNo.Text + "' ";
                strQuery += ", @pITEM_CD= '" + txtItemCd.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, true);
                fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    int x = 0, y = 0;

                    if (strMvmtNo != "")
                    {
                        fpSpread2.Search(0, strMvmtNo, false, false, false, false, 0, 0, ref x, ref y);

                        if (x > 0)
                        {
                            fpSpread2.Sheets[0].SetActiveCell(x, y);
                        }
                        else
                        {
                            x = 0;
                        }

                    }
                    fpSpread2.Sheets[0].AddSelection(x, 1, 1, fpSpread2.Sheets[0].ColumnCount);
                    strAutoMvmtNo = fpSpread2.Sheets[0].Cells[x, SystemBase.Base.GridHeadIndex(GHIdx2, "입고번호")].Text;

                    //상세정보조회
                    SubSearch(strAutoMvmtNo);
                }
                else
                {
                    strAutoMvmtNo = "";
                    btnNew_is = false;
                    NewExec();
                    btnNew_is = true;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            txtMvmtNo.Focus();
            bool All_del = false;
            DialogResult dsMsg;
			string strItemCD = string.Empty;
			string strProjectNo = string.Empty;
			string strBAR_CODE = string.Empty;
			string strLotNo = string.Empty;
			bool bCUDR = true;
			
            
			//상단 그룹박스 필수 체크
			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					this.Cursor = Cursors.WaitCursor;

					string ERRCode = "ER", MSGCode = "P0000", strRelease = string.Empty; ;  //처리할 내용이 없습니다.
					SqlConnection dbConn = SystemBase.DbOpen.DBCON();
					SqlCommand cmd = dbConn.CreateCommand();
					SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

					try
					{

						/////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
						//그리드 상단 필수 체크
						if (SystemBase.Validation.FPGrid_SaveCheck_NEW(fpSpread1, this.Name, "fpSpread1", true))
						{

							if (DelCheck() == false) All_del = true;

							//FarPoint.Win.Spread.SortInfo[] si = new FarPoint.Win.Spread.SortInfo[3];
							//si[0] = new FarPoint.Win.Spread.SortInfo(2, true, System.Collections.Comparer.Default);
							//si[1] = new FarPoint.Win.Spread.SortInfo(5, true, System.Collections.Comparer.Default);
							//si[2] = new FarPoint.Win.Spread.SortInfo(6, true, System.Collections.Comparer.Default);
							//fpSpread1.Sheets[0].SortRows(0, fpSpread1.Sheets[0].Rows.Count, si);

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

									if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "RELEASE")].Text == "True")
										strRelease = "Y";
									else
										strRelease = "N";

									if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수정여부")].Text == "N" && strHead == "D")
									{
										dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0007"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
										return;
									}
									string strSql = " usp_MIM001 '" + strGbn + "'";

									if (string.IsNullOrEmpty(txtMvmtNo.Text) == false)
									{
										strSql += ", @pMVMT_NO = '" + txtMvmtNo.Text.Trim() + "' ";
										strSql += ", @pMVMT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, " 입고순번")].Value + "' ";
									}
									else
									{
										strSql += ", @pMVMT_NO = '" + strAutoMvmtNo + "' ";
										strSql += ", @pMVMT_SEQ = 0 ";
									}

									strSql += ", @pMVMT_DT = '" + dtpMvmtDt.Text + "' ";
									strSql += ", @pIO_TYPE = '" + txtIoType.Text + "' ";
									strSql += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
									strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
									strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
									strSql += ", @pPLANT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value + "' ";
									strSql += ", @pSL_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text.TrimEnd() + "' ";
									strSql += ", @pLOCATION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text + "' ";
									strSql += ", @pPO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text + "' ";
									strSql += ", @pPO_SEQ= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text + "' ";
									strSql += ", @pCC_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관번호")].Text + "' ";
									if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관순번")].Text != "")
										strSql += ", @pCC_SEQ= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관순번")].Text + "' ";

									strSql += ", @pIV_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입번호")].Text + "' ";
									if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입순번")].Text != "")
										strSql += ", @pIV_SEQ= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입순번")].Text + "' ";

									strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
									strSql += ", @pINSP_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사구분")].Value + "' ";
									strSql += ", @pPUR_DUTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당ID")].Text + "' ";
									strSql += ", @pCURRENCY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text + "' ";
									strSql += ", @pEXCH_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text + "' ";
									strSql += ", @pPRICE_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가구분")].Value + "' ";
									strSql += ", @pMVMT_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고단위")].Text.TrimEnd() + "' ";

									strSql += ", @pTEMP_MVMT_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Value + "' ";
									strSql += ", @pMVMT_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value + "' "; ;
									strSql += ", @pTEMP_MVMT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고금액")].Value + "' ";
									strSql += ", @pTEMP_MVMT_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고자국금액")].Value + "' ";
									strSql += ", @pSCM_MVMT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SCM입고번호")].Text + "' ";
									strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
									strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
									strSql += ", @pRELEASE_YN = '" + strRelease + "' ";

									DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
									ERRCode = ds.Tables[0].Rows[0][0].ToString();
									MSGCode = ds.Tables[0].Rows[0][1].ToString();
									strAutoMvmtNo = ds.Tables[0].Rows[0][2].ToString();
									strAutoMvmtSeq = ds.Tables[0].Rows[0][3].ToString();
									if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프


									// 먼저 모든 바코드를 자동생성한 후 프로젝트, 품목, lot번호 별로 같은 바코드로 묶어 주는 작업을 해준다.

									// Lot 수기 등록 저장
									if (
										string.Compare(strHead, "I", true) == 0 &&
										fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True" &&
										string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text.Replace(" ", ""), "Lot분할", true) != 0
									   )
									{
										bCUDR = false;

										strSql = string.Empty;

										strSql = " usp_T_IN_INFO_CUDR ";
										strSql += " @pTYPE		  = 'I2' ";
										strSql += ",@pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
										strSql += ",@pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";
										strSql += ",@pBAR_CODE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "' ";
										strSql += ",@pLOT_NO	  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text + "' ";
										strSql += ",@pMVMT_NO     = '" + strAutoMvmtNo + "' ";
										strSql += ",@pMVMT_SEQ    = '" + strAutoMvmtSeq + "' ";
										strSql += ",@pITEM_CD     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
										strSql += ",@pTR_TYPE     = 'I' ";
										strSql += ",@pIN_DATE     = NULL ";
										strSql += ",@pPROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
										strSql += ",@pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
										strSql += ",@pRCPT_QTY    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Value + "' ";
										strSql += ",@pIN_TRAN_NO  = NULL ";
										strSql += ",@pIN_TRAN_SEQ = NULL ";
										strSql += ",@pIN_TRAN_QTY = 0 ";
										strSql += ",@pSTOCK_QTY   = 0 ";
										strSql += ",@pSTOCK_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고단위")].Text + "' ";
										strSql += ",@pEND_YN      = 'N' ";
										strSql += ",@pREMARK      = '' ";
										strSql += ",@pIN_ID       = '" + SystemBase.Base.gstrUserID + "' ";
										strSql += ",@pUP_ID       = '" + SystemBase.Base.gstrUserID + "' ";

										DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
										ERRCode = ds2.Tables[0].Rows[0][0].ToString();
										MSGCode = ds2.Tables[0].Rows[0][1].ToString();
										if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
										strBAR_CODE = ds2.Tables[0].Rows[0][2].ToString();
										strLotNo = ds2.Tables[0].Rows[0][3].ToString();
									}

									if (
										string.Compare(strHead, "D", true) == 0 &&
										fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True"
									   )
									{

										bCUDR = false;

										strSql = string.Empty;

										strSql = " usp_T_IN_INFO_CUDR ";
										strSql += " @pTYPE		  = 'D1' ";
										strSql += ",@pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
										strSql += ",@pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";
										strSql += ",@pBAR_CODE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "' ";
										strSql += ",@pMVMT_NO     = '" + strAutoMvmtNo + "' ";
										strSql += ",@pMVMT_SEQ    = '" + strAutoMvmtSeq + "' ";
										strSql += ",@pITEM_CD     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
										strSql += ",@pTR_TYPE     = 'I' ";
										strSql += ",@pIN_DATE     = NULL ";
										strSql += ",@pLOT_NO      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text + "' ";
										strSql += ",@pPROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
										strSql += ",@pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
										strSql += ",@pRCPT_QTY    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Value + "' ";
										strSql += ",@pIN_TRAN_NO  = NULL ";
										strSql += ",@pIN_TRAN_SEQ = NULL ";
										strSql += ",@pIN_TRAN_QTY = 0 ";
										strSql += ",@pSTOCK_QTY   = 0 ";
										strSql += ",@pSTOCK_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고단위")].Text + "' ";
										strSql += ",@pEND_YN      = 'N' ";
										strSql += ",@pREMARK      = '' ";
										strSql += ",@pIN_ID       = '" + SystemBase.Base.gstrUserID + "' ";
										strSql += ",@pUP_ID       = '" + SystemBase.Base.gstrUserID + "' ";

										DataSet ds3 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
										ERRCode = ds3.Tables[0].Rows[0][0].ToString();
										MSGCode = ds3.Tables[0].Rows[0][1].ToString();
										strBAR_CODE = string.Empty;
										strLotNo = string.Empty;
										if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
									}

									// Lot 분할 저장 
									if (dt.Rows.Count > 0)
									{

										bCUDR = false;
										
										for (int j = 0; j <= dt.Rows.Count - 1; j++)
										{

											if (
												//string.Compare(dt.Rows[j]["SingleYN"].ToString(), "N", true) == 0 && // 신규등록할 때 LOT 분할/수정/삭제 버튼 활성화하면 주석을 해제해야 함.
												(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text == dt.Rows[j]["PO_NO"].ToString()) &&
												(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text == dt.Rows[j]["PO_SEQ"].ToString())
											   )
											{
												strSql = string.Empty;

												strSql = " usp_T_IN_INFO_CUDR ";
												strSql += "@pTYPE		  = 'I2' ";
												strSql += ",@pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
												strSql += ",@pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";
												strSql += ",@pBAR_CODE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "' ";
												strSql += ",@pLOT_NO      = '" + dt.Rows[j]["LOT_NO"].ToString() + "' ";
												strSql += ",@pMVMT_NO     = '" + strAutoMvmtNo + "' ";
												strSql += ",@pMVMT_SEQ    = '" + strAutoMvmtSeq + "' ";
												strSql += ",@pITEM_CD     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
												strSql += ",@pTR_TYPE     = 'I' ";
												strSql += ",@pIN_DATE     = NULL ";
												strSql += ",@pPROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
												strSql += ",@pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
												strSql += ",@pRCPT_QTY    = '" + dt.Rows[j]["RCPT_QTY"].ToString() + "' ";
												strSql += ",@pIN_TRAN_NO  = NULL ";
												strSql += ",@pIN_TRAN_SEQ = NULL ";
												strSql += ",@pIN_TRAN_QTY = 0 ";
												strSql += ",@pSTOCK_QTY   = 0 ";
												strSql += ",@pSTOCK_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고단위")].Text + "' ";
												strSql += ",@pEND_YN      = 'N' ";
												strSql += ",@pREMARK      = '" + dt.Rows[j]["REMARK"].ToString() + "' ";
												strSql += ",@pIN_ID       = '" + SystemBase.Base.gstrUserID + "' ";
												strSql += ",@pUP_ID       = '" + SystemBase.Base.gstrUserID + "' ";

												DataSet ds5 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
												ERRCode = ds5.Tables[0].Rows[0][0].ToString();
												MSGCode = ds5.Tables[0].Rows[0][1].ToString();
												if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
												strBAR_CODE = ds5.Tables[0].Rows[0][2].ToString();
												strLotNo = ds5.Tables[0].Rows[0][3].ToString();
											}
										}
									}
								}
							}

						}
						else
						{
							Trans.Rollback();
							this.Cursor = Cursors.Default;
							return;
						}


						#region 프로젝트, 품목, lot번호 별로 같은 바코드로 묶어 준다.

						if (string.IsNullOrEmpty(strAutoMvmtNo) == false && bCUDR == false)
						{
							string strBarCD = string.Empty;
							string strMvmtNo = string.Empty;
							string strMvmtSeq = string.Empty;
							string strPreBarCD = string.Empty;
							string strPreLotNo = string.Empty;

							string strQry = "SELECT BAR_CODE, MVMT_NO, MVMT_SEQ, ITEM_CD, PROJECT_NO, LOT_NO FROM T_IN_INFO(NOLOCK) ";
							strQry += "WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND PLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' AND MVMT_NO = '" + strAutoMvmtNo + "' ";
							strQry += "ORDER BY PROJECT_NO, ITEM_CD, LOT_NO, BAR_CODE";
							DataTable dtResult = SystemBase.DbOpen.NoTranDataTable(strQry);

							strItemCD = string.Empty;
							strProjectNo = string.Empty;
							strLotNo = string.Empty;
							strBarCD = string.Empty;
							strPreBarCD = string.Empty;
							strPreLotNo = string.Empty;

							if (dtResult.Rows.Count > 0)
							{
								for (int m = 0; m <= dtResult.Rows.Count - 1; m++)
								{

									if (m == 0)
									{
										strProjectNo = dtResult.Rows[m]["PROJECT_NO"].ToString();
										strItemCD = dtResult.Rows[m]["ITEM_CD"].ToString();
										strLotNo = dtResult.Rows[m]["LOT_NO"].ToString();
										strPreBarCD = dtResult.Rows[m]["BAR_CODE"].ToString();
										strPreLotNo = dtResult.Rows[m]["LOT_NO"].ToString();
									}

									if (
										string.Compare(dtResult.Rows[m]["PROJECT_NO"].ToString(), strProjectNo, true) == 0 &&
										string.Compare(dtResult.Rows[m]["ITEM_CD"].ToString(), strItemCD, true) == 0
									   )
									{

										if (
											string.Compare(dtResult.Rows[m]["LOT_NO"].ToString(), strPreLotNo, true) == 0 ||
											string.Compare(dtResult.Rows[m]["MVMT_NO"].ToString() + "-" + dtResult.Rows[m]["MVMT_SEQ"].ToString(), dtResult.Rows[m]["LOT_NO"].ToString(), true) == 0
										   )
										{
											strBarCD = dtResult.Rows[m]["BAR_CODE"].ToString();
											strMvmtNo = dtResult.Rows[m]["MVMT_NO"].ToString();
											strMvmtSeq = dtResult.Rows[m]["MVMT_SEQ"].ToString();

											string strSql = string.Empty;

											strSql = " usp_T_IN_INFO_CUDR ";
											strSql += " @pTYPE		  = 'U5' ";
											strSql += ",@pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
											strSql += ",@pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";
											strSql += ",@pBAR_CODE    = '" + strBarCD + "' ";
											strSql += ",@pPRE_BAR_CODE = '" + strPreBarCD + "' ";
											strSql += ",@pPRE_LOT_NO = '" + strPreLotNo + "' ";
											strSql += ",@pMVMT_NO     = '" + strMvmtNo + "' ";
											strSql += ",@pMVMT_SEQ    = '" + strMvmtSeq + "' ";
											strSql += ",@pUP_ID       = '" + SystemBase.Base.gstrUserID + "' ";

											DataSet dsFinal = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
											ERRCode = dsFinal.Tables[0].Rows[0][0].ToString();
											MSGCode = dsFinal.Tables[0].Rows[0][1].ToString();
											if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
										}
										else
										{
											strProjectNo = dtResult.Rows[m]["PROJECT_NO"].ToString();
											strItemCD = dtResult.Rows[m]["ITEM_CD"].ToString();
											strLotNo = dtResult.Rows[m]["LOT_NO"].ToString();
											strPreBarCD = dtResult.Rows[m]["BAR_CODE"].ToString();
											strPreLotNo = dtResult.Rows[m]["LOT_NO"].ToString();
										}
									}
									else
									{
										strProjectNo = dtResult.Rows[m]["PROJECT_NO"].ToString();
										strItemCD = dtResult.Rows[m]["ITEM_CD"].ToString();
										strLotNo = dtResult.Rows[m]["LOT_NO"].ToString();
										strPreBarCD = dtResult.Rows[m]["BAR_CODE"].ToString();
										strPreLotNo = dtResult.Rows[m]["LOT_NO"].ToString();
									}
								}
							}

							bCUDR = true;
						}

						#endregion


						Trans.Commit();
						dt.Clear();
					}
					catch (Exception e)
					{
						SystemBase.Loggers.Log(this.Name, e.ToString());
						Trans.Rollback();
						ERRCode = "ER";
						MSGCode = e.Message;
						//MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
					}
				Exit:
					dbConn.Close();
					if (ERRCode == "OK")
					{
						if (bMsgYN == true)
						{
							MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
						}

						if (All_del) //
						{
							Search("");
							SystemBase.Validation.GroupBox_Reset(groupBox1);
							SystemBase.Validation.GroupBox_Reset(groupBox2);
							SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

							dtpSMvmtDtFo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().ToString().Substring(0, 10);
							dtpSMvmtDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
							dtpMvmtDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

							fpSpread1.Sheets[0].Rows.Count = 0;

							strAutoMvmtNo = "";

							butRef.Enabled = true;
							butScm.Enabled = true;
							bSaveLot = true;

						}
						else Search(strAutoMvmtNo);
					}
					else if (ERRCode == "ER")
					{
						MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else
					{
						MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}

					dt.Clear();
					bMsgYN = true;
					this.Cursor = Cursors.Default;
				}

            }

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
            strBtn = "Y";
            //창고
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'B035', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00014", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            //위치
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'B036', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Value + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00030", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고위치팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            strBtn = "N";
        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            try
            {
                //수량, 단가, 금액
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량"))
                {
                    Set_Amt(Row);
                }
                // 창고 
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text
                        = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                //위치
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text.Trim() == "")
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = "";
                    else
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text
                            = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text, " AND SL_CD ='" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.				
            }
        }
        #endregion

        #region 금액계산
        private void Set_Amt(int Row)
        {
            decimal Amt = 0;
            decimal LocAmt = 0;
            decimal Price = 0;
            decimal Qty = 0;
            decimal Xch_rate = 0;

            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Text.Trim() != "")
                Qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Value);
            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text.Trim() != "")
                Price = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value);
            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text.Trim() != "")
                Xch_rate = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value);
            if (Price != 0 && Qty != 0)
            {
                Amt = Price * Qty;
                LocAmt = Amt * Xch_rate;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고금액")].Value = Amt;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고자국금액")].Value = LocAmt;

            }

        }
        #endregion

        #region Master그리드 선택시 상세정보 조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRow = fpSpread2.Sheets[0].GetSelection(0).Row;
                    strAutoMvmtNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "입고번호")].Text.ToString();

                    SubSearch(strAutoMvmtNo);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.				
                }
            }
        }
        #endregion

        #region DETAIL GRID 조회
        private void SubSearch(string strCode)
        {
            this.Cursor = Cursors.WaitCursor;
            strBtn = "Y";
            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox2);

				// 프린터 포트 ComboBox 설정
				SystemBase.RawPrinterHelper.SetPortCombo(cboPort);

                fpSpread1.Sheets[0].Rows.Count = 0;

                //수주Master정보
                string strSql = " usp_MIM001  'S2' ";
                strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql = strSql + ", @pMVMT_NO = '" + strCode + "' ";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                txtMvmtNo.Value = dt.Rows[0]["MVMT_NO"].ToString();
                txtMvmtNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtMvmtNo.ReadOnly = true;

                dtpMvmtDt.Value = dt.Rows[0]["MVMT_DT"].ToString();

                txtCustCd.Value = dt.Rows[0]["CUST_CD"].ToString();
                txtCustNm.Value = dt.Rows[0]["CUST_NM"].ToString();
                txtIoType.Value = dt.Rows[0]["IO_TYPE"].ToString();
                txtIoTypeNm.Value = dt.Rows[0]["IO_TYPE_NM"].ToString();
                txtTempMvmtAmt.Value = dt.Rows[0]["TEMP_MVMT_AMT"];
                txtTempMvmtAmtLoc.Value = dt.Rows[0]["TEMP_MVMT_AMT_LOC"];
                strSts = dt.Rows[0]["DEL_IS"].ToString();

                if (dt.Rows.Count > 0)
                {
					//SystemBase.Base.GroupBoxLock(groupBox2, true);
					SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
                    butRef.Enabled = false;
                    butScm.Enabled = false;
                    btnIspPrint.Enabled = true;
					btnAttView.Enabled = true;
					btnPrintAll.Enabled = true;
					cboPort.Enabled = true;
                }
                else
                {
					//SystemBase.Base.GroupBoxLock(groupBox2, false);
					SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
                    butRef.Enabled = false;
                    butScm.Enabled = false;
                    btnIspPrint.Enabled = false;
					btnAttView.Enabled = false;
					btnPrintAll.Enabled = false;
					cboPort.Enabled = false;
                }

                //Detail그리드 정보.
                string strSql1 = " usp_MIM001  'S3' ";
                strSql1 = strSql1 + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql1 = strSql1 + ", @pMVMT_NO ='" + strCode + "' ";
                strSql1 = strSql1 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 6);

                int col = SystemBase.Base.GridHeadIndex(GHIdx1, "수정여부");
                int col2 = SystemBase.Base.GridHeadIndex(GHIdx1, "검사요청번호");
				int col3 = SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적");

				// Detail Locking 설정
				for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
				{
					//화면 Locking
					if (fpSpread1.Sheets[0].Cells[i, col].Text == "N" || fpSpread1.Sheets[0].Cells[i, col2].Text != "") // DETAIL DATA 수정 불가능
					{

						bSaveLot = false;

						UIForm.FPMake.grdReMake(fpSpread1, i,
							SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치") + "|3"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Release") + "|3"
							);

						if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사구분")].Text == "무검사")
						{
							UIForm.FPMake.grdReMake(fpSpread1, i,
									SystemBase.Base.GridHeadIndex(GHIdx1, "Release") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|3"
									);
						}

						//-----------------------------------------------------------------------------------------------------
						// 추적관리 관련 Locking 설정
						//-----------------------------------------------------------------------------------------------------

						// 추적관리 대상이면 출력매수, 바코드출력 => active
						if (fpSpread1.Sheets[0].Cells[i, col3].Text == "True")
						{
							UIForm.FPMake.grdReMake(fpSpread1, i,
								SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|0"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|0"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할/수정/삭제") + "|0"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할") + "|3"
								);
						}
						else
						{
							UIForm.FPMake.grdReMake(fpSpread1, i,
								SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할/수정/삭제") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할") + "|3"
								);
						}

					}
					else // DETAIL DATA 수정 가능
					{

						bSaveLot = true; 

						UIForm.FPMake.grdReMake(fpSpread1, i,
							SystemBase.Base.GridHeadIndex(GHIdx1, "창고") + "|1"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치") + "|1"
							);

						if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사구분")].Text == "무검사")
						{
							UIForm.FPMake.grdReMake(fpSpread1, i,
									SystemBase.Base.GridHeadIndex(GHIdx1, "Release") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|3"
									);
						}

						//-----------------------------------------------------------------------------------------------------
						// 추적관리 관련 Locking 설정
						//-----------------------------------------------------------------------------------------------------

						// 추적관리 대상이면 출력매수, 바코드출력 => active
						if (fpSpread1.Sheets[0].Cells[i, col3].Text == "True")
						{
							UIForm.FPMake.grdReMake(fpSpread1, i,
								SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|0"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|0"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할/수정/삭제") + "|0"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할") + "|0"
								);
						}
						else
						{
							UIForm.FPMake.grdReMake(fpSpread1, i,
								SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량") + "|0"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할/수정/삭제") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할") + "|3"
								);
						}
						
					}

				}


				if (string.IsNullOrEmpty(strAutoMvmtNo) == true)
				{
					btnPrintAll.Enabled = false;
					btnAttView.Enabled = false;
					cboPort.Enabled = false;
					txtMvmtNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
					txtMvmtNo.ReadOnly = true;
				}
				else
				{
					btnPrintAll.Enabled = true;
					btnAttView.Enabled = true;
					cboPort.Enabled = true;
				}

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            strBtn = "N";
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 버튼 Click
        private void btnSPurDuty_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSPurDuty.Text = Msgs[0].ToString();
                    txtSPurDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void butSCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtSCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSCustCd.Text = Msgs[1].ToString();
                    txtSCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }


        private void btnSIoType_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_M_COMMON 'M020' , @pSPEC1 = '' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSIoType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "입고형태 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSIoType.Text = Msgs[0].ToString();
                    txtSIoTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }


        private void btnSProj_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW007 pu = new WNDW007(txtSProjectNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtSProjectNo.Text = Msgs[3].ToString();
                    txtSProjectSeq.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnSProjSeq_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtSProjectNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtSProjectSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnIspPrint_Click(object sender, System.EventArgs e)
        {
            if (txtMvmtNo.Text.Trim() == "" || txtMvmtNo.ReadOnly == false)
            {
                return;

            }

            string RptName = SystemBase.Base.ProgramWhere + @"\Report\MIM511.rpt";    // 레포트경로+레포트명
            string[] RptParmValue = new string[20];   // SP 파라메타 값

            RptParmValue[0] = "R1";
            RptParmValue[1] = SystemBase.Base.gstrCOMCD;
            RptParmValue[2] = SystemBase.Base.gstrLangCd;
            RptParmValue[3] = " "; //cboInspStatus.SelectedValue.ToString();				
            RptParmValue[4] = " "; //txtPlantCd.Text;
            RptParmValue[5] = " "; //txtItemCd.Text;
            RptParmValue[6] = " "; //txtBpCd.Text;
            RptParmValue[7] = " "; //txtIoType.Text;
            RptParmValue[8] = " "; //txtProjectNo.Text;
            RptParmValue[9] = " "; //txtProjectSeq.Text;				
            RptParmValue[10] = txtMvmtNo.Text.Trim(); //txtMvmtNo.Text;
            RptParmValue[11] = " "; //txtPoNo.Text;				
            RptParmValue[12] = " "; //txtEntCd.Text;				
            RptParmValue[13] = " "; //dtpMvmtDtFr.Text;				
            RptParmValue[14] = " "; //dtpMvmtDtTo.Text;			
            RptParmValue[15] = " ";
            RptParmValue[16] = " ";
            RptParmValue[17] = " ";
			RptParmValue[18] = " ";
			RptParmValue[19] = " ";

            UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, null, RptName, RptParmValue); //공통크리스탈 10버전				
            frm.ShowDialog();
        }

        #endregion

        #region 참조버튼
        private void butRef_Click(object sender, System.EventArgs e)
        {
            try
            {
                MIM001P1 frm1 = new MIM001P1(fpSpread1);
                frm1.ShowDialog();
                if (frm1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = frm1.ReturnVal;
                    if (Msgs != null)
                    {
                        txtCustCd.Value = Msgs[0].ToString();
                        txtCustNm.Value = Msgs[1].ToString();
                        txtIoType.Value = Msgs[2].ToString();
                        txtIoTypeNm.Value = Msgs[3].ToString();

                        txtTempMvmtAmt.Enabled = true;
                        txtTempMvmtAmt.ReadOnly = false;
                        txtTempMvmtAmtLoc.Enabled = true;
                        txtTempMvmtAmtLoc.ReadOnly = false;

                        txtTempMvmtAmt.Value = Msgs[4].ToString();
                        txtTempMvmtAmtLoc.Value = Msgs[5].ToString();

                        txtTempMvmtAmt.Enabled = false;
                        txtTempMvmtAmt.ReadOnly = true;
                        txtTempMvmtAmtLoc.Enabled = false;
                        txtTempMvmtAmtLoc.ReadOnly = true;

						for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
						{
							// 추적관리 대상이면 Lot No, Lot 분할 => active
							if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
							{
								UIForm.FPMake.grdReMake(fpSpread1, i,
									SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|0"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할/수정/삭제") + "|3");

								UIForm.FPMake.grdReMake(fpSpread1, i,
									SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 수정/삭제") + "|3");
							}
							else
							{
								UIForm.FPMake.grdReMake(fpSpread1, i,
									SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 수정/삭제") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할/수정/삭제") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할") + "|3");
							}

						}

                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void butScm_Click(object sender, System.EventArgs e)
        {
            try
            {
                MIM001P5 frm1 = new MIM001P5(fpSpread1);
                frm1.ShowDialog();
                if (frm1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = frm1.ReturnVal;
                    if (Msgs != null)
                    {
                        txtCustCd.Value = Msgs[0].ToString();
                        txtCustNm.Value = Msgs[1].ToString();
                        txtIoType.Value = Msgs[2].ToString();
                        txtIoTypeNm.Value = Msgs[3].ToString();

                        txtTempMvmtAmt.Enabled = true;
                        txtTempMvmtAmt.ReadOnly = false;
                        txtTempMvmtAmtLoc.Enabled = true;
                        txtTempMvmtAmtLoc.ReadOnly = false;

                        txtTempMvmtAmt.Value = Msgs[4].ToString();
                        txtTempMvmtAmtLoc.Value = Msgs[5].ToString();

                        txtTempMvmtAmt.Enabled = false;
                        txtTempMvmtAmt.ReadOnly = true;
                        txtTempMvmtAmtLoc.Enabled = false;
                        txtTempMvmtAmtLoc.ReadOnly = true;
                        if (txtMvmtNo.Text.Trim() == "")
                        {
                            txtMvmtNo.Value = Msgs[6].ToString();
                            txtMvmtNo.ReadOnly = true;
                            dtpMvmtDt.Value = Msgs[7].ToString();

                        }

						for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
						{
							// 추적관리 대상이면 Lot No, Lot 분할 => active
							if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
							{
								//UIForm.FPMake.grdReMake(fpSpread1, i,
								//    SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|0"
								//    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할/수정/삭제") + "|0");

								// Release 임시. 추후 삭제 예정
								UIForm.FPMake.grdReMake(fpSpread1, i,
									SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|0"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할/수정/삭제") + "|3");

								UIForm.FPMake.grdReMake(fpSpread1, i,
									SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 수정/삭제") + "|3");
							}
							else
							{
								UIForm.FPMake.grdReMake(fpSpread1, i,
									SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 수정/삭제") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할/수정/삭제") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할") + "|3");
							}

						}

                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged
        private void txtSCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtSCustCd.Text != "")
                    {
                        txtSCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSCustNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtSIoType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtSIoType.Text != "")
                    {
                        txtSIoTypeNm.Value = SystemBase.Base.CodeName("IO_TYPE", "IO_TYPE_NM", "M_MVMT_TYPE", txtSIoType.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSIoTypeNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtSProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtSProjectSeq.Text = "";
        }

        private void txtSPurDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N" && txtSPurDuty.Text.Trim() != "")
                {
                    string temp = "";
                    temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtSPurDuty.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                    if (temp != "")
                    {
                        if (txtSPurDuty.Text != "")
                        {
                            txtSPurDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSPurDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                        }
                        else
                        {
                            txtSPurDutyNm.Value = "";
                        }
                    }
                }
                else if (txtSPurDuty.Text.Trim() == "") txtSPurDutyNm.Value = "";
            }
            catch
            {

            }
        }
        #endregion

        #region Form Activated & Deactivate
        private void MIM001_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpSMvmtDtFo.Focus();
        }

        private void MIM001_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

		#region 품질증빙 확인
		private void btnAttView_Click(object sender, EventArgs e)
		{
			try
			{

				WNDW036 pu = new WNDW036();
				pu.strKEY_NO = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "입고번호")].Text.ToString();
				pu.strREQ_TYPE = "PO";
				pu.strDOC_TYPE = "PUR";
				pu.strFormGubn = "MIM001";
				
				pu.ShowDialog();

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region Grid Button Click Event
		private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{

			decimal dSum = 0;

			try
			{

				if ((e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할")) || (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 분할/수정/삭제")))
				{
					MIM001P7 mim001p7 = new MIM001P7();
					mim001p7.strPO_NO = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text.ToString();
					mim001p7.strPO_SEQ = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text.ToString();
					mim001p7.strMVMT_NO = strAutoMvmtNo;
					mim001p7.strMVMT_SEQ = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text.ToString();
					mim001p7.bSave = bSaveLot;

					mim001p7.ShowDialog();

					if (mim001p7.DialogResult == DialogResult.OK)
					{

						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = "";
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Value = 0;

						if (dt.Rows.Count > 0)
						{
							for (int i = 0; i <= mim001p7.dt.Rows.Count - 1; i++)
							{
								DataRow dr = mim001p7.dt.Rows[i];
								dt.Rows.Add(dr.ItemArray);
							}
						}
						else
						{
							dt = mim001p7.dt;
						}

						if (mim001p7.dt.Rows.Count > 1)
						{

							for (int i = 0; i <= mim001p7.dt.Rows.Count - 1; i++)
							{
								if (mim001p7.dt.Rows[0]["RCPT_QTY"] == DBNull.Value) mim001p7.dt.Rows[0]["RCPT_QTY"] = 0;
								dSum += Convert.ToDecimal(mim001p7.dt.Rows[i]["RCPT_QTY"]);
							}

							fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = "Lot 분할";
							fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Value = dSum;

							fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = true;
							fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Locked = true;
						}
						else if (mim001p7.dt.Rows.Count == 1)
						{
							fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = mim001p7.dt.Rows[0]["LOT_NO"].ToString();
							fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Value = mim001p7.dt.Rows[0]["RCPT_QTY"].ToString();

							fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = true;
							fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Locked = true;
						}

						Set_Amt(fpSpread1.Sheets[0].ActiveRowIndex);

					}

					// 입고 저장 후 Lot 분할/수정/삭제 팝업화면에서 변경사항이 적용되면 Parent Form Reload
					if ((string.IsNullOrEmpty(strAutoMvmtNo) == false) && (mim001p7.strSaveYN == "Y"))
					{
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Value = mim001p7.dLotSum;
						fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "U";
						bMsgYN = false;
						SaveExec();
						SubSearch(strAutoMvmtNo);
					}

					//if (dt.Rows.Count > 1 && mim001p7.strSaveYN == "Y") { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = "Lot 분할"; }

					mim001p7.strSaveYN = string.Empty;
					mim001p7.dLotSum = 0;
					bMsgYN = true;
				}
				else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력"))
				{
					if (cboPort.SelectedText == "선택")
					{
						MessageBox.Show("프린터 포트를 선택해주세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					GetPrintData(e.Row, "E");
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 바코드 일괄 출력
		private void btnPrintAll_Click(object sender, EventArgs e)
		{
			try
			{

				if (cboPort.SelectedText == "선택")
				{
					MessageBox.Show("프린터 포트를 선택해주세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				GetPrintData(0, "A");
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 바코드 인쇄
		private void PrintBarCode(int row, string flag)
		{
			string strZPL = string.Empty;

			int X = -30;
			int Y = 5;

			if (dtPrint.Rows.Count > 0)
			{

				for (int i = 0; i <= dtPrint.Rows.Count - 1; i++)
				{
					strZPL = "";
					strZPL += "^XA";					// start format

					strZPL += "^LL440";					// label hight
					strZPL += "^PW600";					// print length

					strZPL += "^LS0";					// print length
					strZPL += "^LH5,5";					// label home location - 최초 시작 위치(x, y)

					
					strZPL += "^SEE:UHANGUL.DAT^FS";	// 인코딩 지정, ^FS:field separator, ^FO:field origin
					strZPL += "^CWJ,E:KFONT3.FNT^FS";	// 폰트

					// FO : 인쇄할 항목의 인쇄 위치 지정(X,Y)
					// GB500(라인 길이),150(라인 높이),7(라인 두께),(라인 색상),5(라인 모서리 둥글기)
					//strZPL += "^FO50,0^GB550,140,7,,5^FS";	//라인 박스 그리기

					// BY2,2,80 - 바코드 속성 중 좁은 바 넓이를 2로 하고, 넓은 바는 좁은 바의 2배로 지정. 바코드 높이는 80 
					// BCN(문자회전 NORMAL, R:90도, I:180도, B:270도),80(바코드 높이),Y(바코드 밑에 문자인쇄 여부),N(바코드 위에 문자인쇄 여부),N(CHECK DIGIT 사용 여부) 
					strZPL += "^FO" + (X + 80) + "," + (Y + 10) + "^BY2,2.2,90^BCN,90,Y,N,N^FD" + dtPrint.Rows[i]["BAR_CODE"].ToString() + "^FS";	//^BC:Code 128(USD-6)체계

					strZPL += "^FO" + (X + 80) + "," + (Y + 140) + "^CI28^AJN,25,25^FDPrj No^FS" + "^FO" + (X + 180) + "," + (Y + 140) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["PROJECT_NO"].ToString() + "^FS";
					strZPL += "^FO" + (X + 80) + "," + (Y + 170) + "^CI28^AJN,40,40^FDCode No : " + dtPrint.Rows[i]["ITEM_CD"].ToString() + "^FS";
					strZPL += "^FO" + (X + 80) + "," + (Y + 220) + "^CI28^AJN,25,25^FDDesc^FS" + "^FO" + (X + 180) + "," + (Y + 220) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["ITEM_NM"].ToString() + "^FS";
					strZPL += "^FO" + (X + 80) + "," + (Y + 250) + "^CI28^AJN,25,25^FDPart No^FS" + "^FO" + (X + 180) + "," + (Y + 250) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["ITEM_SPEC"].ToString() + "^FS";
					strZPL += "^FO" + (X + 80) + "," + (Y + 280) + "^CI28^AJN,25,25^FDRec No^FS" + "^FO" + (X + 180) + "," + (Y + 280) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["MVMT_NO"].ToString() + "^FS";
					strZPL += "^FO" + (X + 80) + "," + (Y + 310) + "^CI28^AJN,25,25^FDLot No^FS" + "^FO" + (X + 180) + "," + (Y + 310) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["LOT_NO"].ToString() + "^FS";
					strZPL += "^FO" + (X + 80) + "," + (Y + 340) + "^CI28^AJN,25,25^FDVendor^FS" + "^FO" + (X + 180) + "," + (Y + 340) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["VENDOR"].ToString() + "^FS";
					strZPL += "^FO" + (X + 80) + "," + (Y + 370) + "^CI28^AJN,25,25^FDQ'ty^FS" + "^FO" + (X + 180) + "," + (Y + 370) + "^CI28^AJN,25,25^FD : " + SetConvert(Convert.ToDecimal(dtPrint.Rows[i]["STOCK_QTY"])) + " "
																							   + dtPrint.Rows[i]["STOCK_UNIT"].ToString() + "^FS"
																							   + "^FO" + (X + 370) + "," + (Y + 370) + "^CI28^AJN,25,25^FD(" + SystemBase.Base.gstrUserName + ")^FS"; 	
					strZPL += "^FO" + (X + 80) + "," + (Y + 400) + "^CI28^AJN,25,25^FDPrint^FS" + "^FO" + (X + 180) + "," + (Y + 400) + "^CI28^AJN,25,25^FD : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "^FS";
					
					if (flag == "A")
					{
						strZPL += "^PQ" + "1" + "^FS";	// 라벨 인쇄 매수
					}
					else
					{
						strZPL += "^PQ" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수")].Text + "^FS";	// 라벨 인쇄 매수					
					}

					strZPL += "^XZ";		// end format


					if (string.Compare(cboPort.SelectedText.Substring(0,3), "LPT", true) == 0)
					{
						if (SystemBase.RawPrinterHelper.SendStringToPrinter("LPT1", strZPL) == false)
						{
							throw new Exception("바코드 발행 중 오류가 발생했습니다.");
						}
					}
					else
					{
						if (SystemBase.RawPrinterHelper.PrintZPL(cboPort.SelectedText, strZPL) == false)
						{
							throw new Exception("바코드 발행 중 오류가 발생했습니다.");
						}						
					}

				}
			}
		}
		#endregion

		#region 바코드 정보 조회
		private void GetPrintData(int row, string flag)
		{
			string strQuery = string.Empty;
			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();

			dtPrint.Clear();

			/*
			바코드, 출고수량, LOT NO, 품목코드, 제조오더번호  
			*/

			if (flag == "A")
			{
				strQuery = " usp_T_IN_INFO_CUDR ";
				strQuery += " @pTYPE = 'P1' ";
				strQuery += ",@pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery += ",@pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
				strQuery += ",@pMVMT_NO = '" + strAutoMvmtNo + "' ";
				strQuery += ",@pGUBUN = 'A' ";
			}
			else
			{
				strQuery = " usp_T_IN_INFO_CUDR ";
				strQuery += " @pTYPE = 'P1' ";
				strQuery += ",@pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery += ",@pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
				strQuery += ",@pMVMT_NO = '" + strAutoMvmtNo + "' ";
				strQuery += ",@pMVMT_SEQ = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text + "' ";
				strQuery += ",@pGUBUN = 'E' ";
			}

			dtPrint = SystemBase.DbOpen.NoTranDataTable(strQuery);

			if (dtPrint.Rows.Count > 0)
			{
				PrintBarCode(row, flag);
			}
			else 
			{
				MessageBox.Show("검색된 데이터가 없습니다.", SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
		#endregion

		#region 수량 형식 변경
		private string SetConvert(decimal dNumber)
		{
			string strReturn = string.Empty;

			strReturn = double.Parse(dNumber.ToString()).ToString();

			return strReturn;
		}
		#endregion

		#region Release 전체 선택 / 해제
		private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
		{
			try
			{
				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					if (fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
					{
						if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Release"))
						{
							for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
							{
								if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release")].Locked == false)
								{
									if (string.IsNullOrEmpty(txtMvmtNo.Text))
									{
										fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Insert;
									}
									else
									{
										fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
										fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Update;
									}
								}
							}
						}
					}
				}
			}
			catch (Exception f)
			{
				MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 프린터 포트 저장
		private void cboPort_SelectedValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(cboPort.SelectedText) == false && cboPort.SelectedText != "선택")
				{
					SystemBase.RawPrinterHelper.SavePrinterPort(cboPort.SelectedText);
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region lot 번호는 'Lot 분할' 입력 금지
		private void fpSpread1_EditModeOff(object sender, EventArgs e)
		{
			try
			{
				if (fpSpread1.Sheets[0].ActiveColumnIndex == SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No"))
				{
					if (
						string.Compare(fpSpread1.Sheets[0].RowHeader.Cells[fpSpread1.Sheets[0].ActiveRowIndex, 0].Text, "I", true) == 0 &&
						string.Compare(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text.Replace(" ", ""), "LOT분할", true) == 0
					   )
					{
						MessageBox.Show("Lot 번호는 'Lot 분할' 값을 직접 입력할 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
						fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text = "";
					}
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region lot 팝업창에서 처리한 데이터가 있으면 lot 번호 수기 입력 금지
		private void fpSpread1_EditModeOn(object sender, EventArgs e)
		{
			try
			{
				//for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
				//{
				//    for (int j = 0; j <= dt.Rows.Count - 1; j++)
				//    {

				//        if ((fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text ==
				//            dt.Rows[j]["PO_NO"].ToString()) &&
				//            (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text ==
				//            dt.Rows[j]["PO_SEQ"].ToString()))
				//        {
				//            MessageBox.Show("Lot 번호 분할이 이루어졌으므로 'Lot 분할' 값을 직접 입력할 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text = "";
				//            break;
				//        }
				//    }
				//}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

	}
}
