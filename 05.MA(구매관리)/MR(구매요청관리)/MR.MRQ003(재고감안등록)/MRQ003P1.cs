#region 작성정보
/*********************************************************************/
// 단위업무명 : 재고감안등록
// 작 성 자 : 
// 작 성 일 : 
// 작성내용 : 재고감안등록 및 관리
// 수 정 일 : 2014-10-13
// 수 정 자 : 최 용 준
// 수정내용 : lot 기능 추가
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;

namespace MR.MRQ003
{
    public partial class MRQ003P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string returnVal;
        decimal returnRef;
        string strReqNo;
        string strReqSeq;
        string strItemCd;
        string strItemNm;
        decimal dReqQty;
        decimal dRefQty;
        decimal sum = 0;
        bool first = false;
        string strTranNo = "";					// 수불번호
        string strMovTranNo = "";				// 수불순번
        string strBtn = "N";
        bool locked = false;
        bool isCheck = true;					// 요구수량 대 참조수량 체크
        bool isDetail_save = false;				// 디테일 저장여부
		string strLotYn = string.Empty;			// lot 추적 여부
		string strProjectNo  = string.Empty;	// 요청 프로젝트번호
		string strProjectSeq = string.Empty;	// 요청 프로젝트차수
		string strRefYN = string.Empty;			// 재고감안여부 
        #endregion

		#region 생성자
		public MRQ003P1(string ReqNo, string ReqSeq, string ItemCd, string ItemNm, decimal ReqQty, decimal RefQty, bool locking, string LotYn, string ProjectNo, string ProjectSeq, string Ref_YN)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();
            strReqNo = ReqNo;
            strReqSeq = ReqSeq;
            strItemCd = ItemCd;
            strItemNm = ItemNm;
            dReqQty = ReqQty;
            dRefQty = RefQty;
            locked = locking;
			strLotYn = LotYn;
			strProjectNo  = ProjectNo;
			strProjectSeq = ProjectSeq;
			strRefYN = Ref_YN;

        }

        public MRQ003P1()
        {
            InitializeComponent();
        }
		#endregion

		#region Form Load 시
		private void MRQ003P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "재고참조팝업";

            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            UIForm.Buttons.ReButton("010000001000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);//공장
			cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD.ToString();

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='B030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "창고")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='SL', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='LOC', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

            txtItemCd.Value = strItemCd;
            txtItemNm.Value = strItemNm;
            txtReqNo.Value = strReqNo;
            txtReqSeq.Value = strReqSeq;

            if (dReqQty != 0)
                txtReqQty.Value = dReqQty;
            else
                txtReqQty.Value = 0;

            if (dRefQty != 0)
                txtRefQty.Value = dRefQty;
            else
                txtRefQty.Value = 0;

            txtReqQty.Enabled = false;
            txtRefQty.Enabled = false;

            strTranNo = "";
            strMovTranNo = "";
            first = true;

            SearchExec();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
				string strQuery = " usp_MRQ003 ";

				// lot 추적 여부에 따라 프로시저 구분됨
				//if (string.Compare(strLotYn, "True", true) == 0)
				//{
				//    strQuery += "  @pTYPE = 'P11'";
				//}
				//else
				//{
				//    strQuery += "  @pTYPE = 'P1'";
				//}

				strQuery += "  @pTYPE = 'P1'";
                				
				strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pITEM_CD = '" + strItemCd + "' ";
                strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProjNo.Text + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtProjSeq.Text + "' ";
				strQuery += ", @ppPROJECT_NO = '" + strProjectNo + "' ";
				strQuery += ", @ppPROJECT_SEQ = '" + strProjectSeq + "' ";
                strQuery += ", @pREQ_NO = '" + strReqNo + "' ";
                strQuery += ", @pREQ_SEQ = '" + strReqSeq + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
				strQuery += ", @pSTOCK_REF_YN = '" + strRefYN + "' ";
				strQuery += ", @pG_STOCK_REF_QTY = " + dRefQty + " ";


				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

				if (fpSpread1.Sheets[0].Rows.Count == 0)
				{
					this.Close();
				}

                int idx = SystemBase.Base.GridHeadIndex(GHIdx1, "수불번호");

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, idx].Text.Trim() != "")
                    {
                        strTranNo = fpSpread1.Sheets[0].Cells[i, idx].Text;
                        strMovTranNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수불번호")].Text;
                        txtTranDuty.Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불담당자")].Text;
                        txtTranDutyNm.Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자명")].Text;
                        break;
                    }
                }

                if (txtTranDuty.Text == "")
                {
                    txtTranDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", SystemBase.Base.gstrUserID, " AND TRAN_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    if (txtTranDutyNm.Text != "") txtTranDuty.Text = SystemBase.Base.gstrUserID;

                }

                for (int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
                {
                    if (locked)
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, j, SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량") + "|3");
                    }
                    else if (fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, j, SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|0#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량") + "|3");
                    }

                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec_Detail() 폼에 입력된 데이타 저장 로직
        private string SaveExec_Detail()
        {
			
			this.Cursor = Cursors.WaitCursor;

			strTranNo = "";
			strMovTranNo = "";

			string strTranSeq = "";					
			string strMovTranSeq = "0";
			
            if (isCheck != true) return "ER";

            //그리드 상단 필수 체크
            if (UIForm.FPMake.FPUpCheck(fpSpread1, false) == true && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {

                string ERRCode = "", MSGCode = "";
                int cnt = 0;
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

						strTranNo = "";
						strMovTranNo = "0";

						if (strHead.Length > 0)
						{
							fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";

							if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
							{ strGbn = "U2"; }
							else
								strGbn = "D2";

							string strSql = " usp_MRQ003 '" + strGbn + "'";
							strSql += ", @pREQ_NO = '" + strReqNo + "'";
							strSql += ", @pREQ_SEQ = " + strReqSeq;
							strSql += ", @pITEM_CD = '" + strItemCd + "'";
							strSql += ", @pPLANT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value + "'";
							strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text + "'";
							strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "'";
							strSql += ", @pSL_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Value + "'";
							strSql += ", @pLOCATION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Value + "'";
							//strSql += ", @pCURRENCY = '" +  fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text + "'";	
							strSql += ", @pREQ_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")].Value + "'";
							strSql += ", @pG_STOCK_QTY = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value;
							strSql += ", @pG_STOCK_REF_QTY =  " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value;
							strSql += ", @pTRAN_DUTY  =  '" + txtTranDuty.Text + "'";
							strSql += ", @pTRAN_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불번호")].Value + "'";
							strSql += ", @pMOV_TRAN_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수불번호")].Value + "'";
							strSql += ", @pTRAN_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불순번")].Text + "'";
							strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
							strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
							strSql += ", @pBAR_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "' ";

							DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
							ERRCode = ds.Tables[0].Rows[0][0].ToString();
							MSGCode = ds.Tables[0].Rows[0][1].ToString();
							if (ERRCode != "OK") { Trans.Rollback(); strTranNo = ""; strMovTranNo = ""; cnt++; goto Exit; }	// ER 코드 Return시 점프

							if (ERRCode == "OK")
							{
								strTranNo = ds.Tables[0].Rows[0][2].ToString();
								strMovTranNo = ds.Tables[0].Rows[0][3].ToString();
								strTranSeq = ds.Tables[0].Rows[0][4].ToString();	// T_IN_INFO.IN_TRAN_NO,	T_IN_INFO.MVMT_NO
								strMovTranSeq = ds.Tables[0].Rows[0][5].ToString();	// T_IN_INFO.IN_TRAN_SEQ,	T_IN_INFO.MVMT_SEQ

								if (string.Compare(strGbn, "D2", true) == 0 && (string.IsNullOrEmpty(strTranNo) == true || string.IsNullOrEmpty(strMovTranNo) == true))
								{
									strTranNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text;
									strMovTranNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동후 입고번호")].Text;
									strTranSeq = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text;
									strMovTranSeq = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동후 입고순번")].Text;
								}

								#region Lot 정보 저장

								if (string.Compare(strLotYn, "True", true) == 0 && string.IsNullOrEmpty(strTranNo) == false)
								{

									if (string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text, "True", true) == 0)
										strGbn = "I1";
									else
										strGbn = "D1";


									// 1. LOT 출고처리 - strTranNo, strMovTranNo
									strSql = "  usp_T_OUT_INFO_CUDR ";
									strSql += " @pTYPE            = '" + strGbn + "' ";
									strSql += ",@pCO_CD           = '" + SystemBase.Base.gstrCOMCD + "' ";
									strSql += ",@pPLANT_CD        = '" + SystemBase.Base.gstrPLANT_CD + "' ";
									strSql += ",@pBAR_CODE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "' ";
									strSql += ",@pMVMT_NO     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text + "' ";
									strSql += ",@pMVMT_SEQ    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text + "' ";

									// 출고 삭제의 경우 이동후 수불번호(MOV_TRAN_NO)가 기준이 되므로 이에 대한 처리를 달리해줘야 함
									if (string.Compare(strGbn, "D1", true) == 0)
									{
										strSql += ",@pOUT_TRAN_NO     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불번호")].Text + "' ";
										strSql += ",@pOUT_TRAN_SEQ    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불순번")].Text + "' ";
									}
									else
									{
										strSql += ",@pOUT_TRAN_NO     = '" + strTranNo + "' ";
										strSql += ",@pOUT_TRAN_SEQ    = '" + strTranSeq + "' ";
									}

									strSql += ",@pITEM_CD         = '" + strItemCd + "' ";
									strSql += ",@pTR_TYPE         = 'M' ";
									strSql += ",@pOUT_DATE        = '" + DateTime.Today.ToShortDateString() + "' ";
									strSql += ",@pLOT_NO          = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text + "' ";
									strSql += ",@pOUT_PROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text + "' ";
									strSql += ",@pOUT_PROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
									strSql += ",@pOUT_QTY         = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value + "' ";
									strSql += ",@pSTOCK_UNIT      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")].Text + "' ";
									strSql += ",@pREMARK          = '" + strMovTranNo + "/" + strMovTranSeq + "' ";
									strSql += ",@pIN_ID           = '" + SystemBase.Base.gstrUserID + "' ";
									strSql += ",@pUP_ID           = '" + SystemBase.Base.gstrUserID + "' ";
									strSql += ",@pORG_MVMT_NO	  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원본 입고번호")].Text + "' ";
									strSql += ",@pORG_MVMT_SEQ	  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원본 입고순번")].Text + "' ";

									DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
									ERRCode = ds2.Tables[0].Rows[0][0].ToString();
									MSGCode = ds2.Tables[0].Rows[0][1].ToString();
									if (ERRCode != "OK") { Trans.Rollback(); strTranNo = ""; strMovTranNo = ""; cnt++; goto Exit; }	// ER 코드 Return시 점프


									// 2. LOT 입고처리 - strTranNo2, strMovTranNo2
									if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value) > 0)
									{
										strSql = "  usp_T_IN_INFO_CUDR ";
										strSql += " @pTYPE        = '" + strGbn + "' ";
										strSql += ",@pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
										strSql += ",@pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";


										// 입고 삭제의 경우 이동후 수불번호(MOV_TRAN_NO)가 기준이 되므로 이에 대한 처리를 달리해줘야 함
										if (string.Compare(strGbn, "D1", true) == 0)
										{
											strSql += ",@pBAR_CODE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동후 바코드")].Text + "' ";
											strSql += ",@pMVMT_NO     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동후 입고번호")].Text + "' ";
											strSql += ",@pMVMT_SEQ    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동후 입고순번")].Text + "' ";
										}
										else
										{
											strSql += ",@pBAR_CODE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "' ";
											strSql += ",@pMVMT_NO     = '" + strMovTranNo + "' ";
											strSql += ",@pMVMT_SEQ    = '" + strMovTranSeq + "' ";
										}

										strSql += ",@pITEM_CD = '" + strItemCd + "' ";
										strSql += ",@pTR_TYPE     = 'M' ";
										strSql += ",@pIN_DATE     = '" + DateTime.Today.ToShortDateString() + "' ";
										strSql += ",@pLOT_NO      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text + "' ";
										strSql += ",@pPROJECT_NO  = '" + strProjectNo + "' ";
										strSql += ",@pPROJECT_SEQ = '" + strProjectSeq + "' ";
										strSql += ",@pRCPT_QTY    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value + "' ";
										strSql += ",@pIN_TRAN_NO  = '" + strMovTranNo + "' ";
										strSql += ",@pIN_TRAN_SEQ = '" + strMovTranSeq + "' ";
										strSql += ",@pIN_TRAN_QTY = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value + " ";
										strSql += ",@pSTOCK_QTY   = 0 ";
										strSql += ",@pSTOCK_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")].Text + "' ";
										strSql += ",@pEND_YN      = 'N' ";
										strSql += ",@pREMARK      = '" + strTranNo + "/" + strTranSeq + "' ";
										strSql += ",@pIN_ID       = '" + SystemBase.Base.gstrUserID + "' ";
										strSql += ",@pUP_ID       = '" + SystemBase.Base.gstrUserID + "' ";
										strSql += ",@pORG_MVMT_NO	  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원본 입고번호")].Text + "' ";
										strSql += ",@pORG_MVMT_SEQ	  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원본 입고순번")].Text + "' ";

										DataSet ds3 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
										ERRCode = ds3.Tables[0].Rows[0][0].ToString();
										MSGCode = ds3.Tables[0].Rows[0][1].ToString();
										if (ERRCode != "OK") { Trans.Rollback(); strTranNo = ""; strMovTranNo = ""; cnt++; goto Exit; }	// ER 코드 Return시 점프
									}

								}

								#endregion

							}

							isDetail_save = true;

						}
					}

					if (cnt == 0) Trans.Commit();
				}
				catch (Exception e)
				{
					SystemBase.Loggers.Log(this.Name, e.ToString());
					Trans.Rollback();
					strTranNo = ""; 
					strMovTranNo = "";
					ERRCode = "ER";
					MSGCode = e.Message;
					//MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
				}
				finally
				{
					this.Cursor = Cursors.Default;
				}

            Exit:
                dbConn.Close();
                if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (ERRCode == "WR")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return ERRCode;
            }
            else
            {
                return "ER";
            }

            return "OK";
        }
        #endregion

        #region SaveExec_Master() 폼에 입력된 데이타 저장 로직
        private string SaveExec_Master()
        {

            // 그리드 상단 필수항목 체크
            if (UIForm.FPMake.FPUpCheck(fpSpread1, false) == true)
            {
                string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_MRQ003 'U3'";

                    strSql += ", @pREQ_NO = '" + strReqNo + "'";
                    strSql += ", @pREQ_SEQ = " + strReqSeq;
                    strSql += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
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
                DialogResult dsMsg;
                if (ERRCode == "ER")
                    dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ERRCode;
            }
            else
            {
                return "ER";
            }

            return "OK";
        }
        #endregion

        #region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
        {

            try
            {
                if (SaveExec_Detail() == "ER") return;
                if (isCheck != true) return;
                if (isDetail_save)
                {
                    if (SaveExec_Master() == "ER") return;
                }
                Sum_RefQty();
                RtnStr("Y", sum);

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            Close();
            this.DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            RtnStr("N", 0);
            Close();
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region 값 전송
        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }
        public decimal ReturnRef { get { return returnRef; } set { returnRef = value; } }

        public void RtnStr(string strCode, decimal strValue)
        {
            returnVal = strCode;
            returnRef = strValue;
        }
        #endregion

        #region fpSpread1_ButtonClicked
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {


                int Col = SystemBase.Base.GridHeadIndex(GHIdx1, "선택");
                if (e.Column == Col)
                {
                    if (fpSpread1.Sheets[0].Cells[e.Row, Col].Text == "False")
                    {
                        if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Locked == false)
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value = 0;
                    }
                    else
                    {
                        if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value) == 0)
                        {
                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value) >= (dReqQty - dRefQty - sum))
								fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value = dReqQty - dRefQty - sum;
                            else
                                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value
                                    = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value;
                        }
                    }
                }

                Sum_RefQty();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }


        private void Sum_RefQty()
        {
            int Col = SystemBase.Base.GridHeadIndex(GHIdx1, "선택");
            sum = 0;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, Col].Text == "True")
                    sum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value);
            }

            txtRefQty.Enabled = true;
            txtRefQty.Value = sum;
            txtRefQty.Enabled = false;
            
			if (dReqQty < sum)
            {
                DialogResult dsMsg = MessageBox.Show("참조수량이 요구수량보다 많습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

				if (dRefQty != 0)
					txtRefQty.Value = dRefQty;
				else
					txtRefQty.Value = 0;
                
				isCheck = false;
                
				for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                    {
                        fpSpread1.Sheets[0].Cells[i, Col].Value = 0;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value = 0;
                        isCheck = true;
                    }
                }
            }
            else
                isCheck = true;

        }
        #endregion

        #region fpSpread1_Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            int Col = SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량");
            if (Column == Col)
            {
                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, Col].Value) >
                    Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value))
                {
                    DialogResult dsMsg = MessageBox.Show("참조수량이 재고수량보다 클 수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fpSpread1.Sheets[0].Cells[Row, Col].Value = 0;
                    fpSpread1.ActiveSheet.SetActiveCell(Row, Col);
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 1;
                }

                Sum_RefQty();
            }
        }
        #endregion

        #region MRQ003P1_Activated
        private void MRQ003P1_Activated(object sender, System.EventArgs e)
        {
            //			if(first) 	SearchExec(); 
            //			first = false;
        }
        #endregion

        #region 조건버튼 Click
        private void btnTranDuty_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_I_COMMON @pTYPE= 'I012', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTranDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수불담당자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTranDuty.Text = Msgs[0].ToString();
                    txtTranDutyNm.Value = Msgs[1].ToString();
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

        private void btnProj_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {

                WNDW007 pu = new WNDW007(txtProjNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjNo.Text = Msgs[3].ToString();
                    if (txtProjSeq.Text != "*") txtProjSeq.Text = "";
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

        private void btnProjSeq_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtProjSeq.Text = Msgs[0].ToString();
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
        #endregion

        #region TextChange
        private void txtTranDuty_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtTranDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtTranDuty.Text, " AND TRAN_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtProjSeq_Leave(object sender, System.EventArgs e)
        {
            if (strBtn == "N" && txtProjSeq.Text != "*")
                txtProjSeq.Text = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_SEQ", "P_MPS_REGISTER", txtProjNo.Text, " AND PROJECT_SEQ = '" + txtProjSeq.Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

        }
        #endregion
    }
}
