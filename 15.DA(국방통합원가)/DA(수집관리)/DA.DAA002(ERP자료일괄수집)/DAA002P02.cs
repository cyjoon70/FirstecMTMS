using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DA.DAA002
{
    public partial class DAA002P02 : UIForm.FPCOMM1
    {
        #region 변수선언
        int iPK_SEQ = 0;
        string strMNUF_CODE = "";   //제출업체   
        string strORDR_YEAR = "";   //요구연도
        string strDPRT_CODE = "";   //구매부서
        string strDCSN_NUMB = "";   //판단번호
        string strCALC_DEGR = "";   //차수        
        string strFormId = "";
        string strSql = "";

        #endregion

        #region DAA002P02
        public DAA002P02()
        {
            InitializeComponent();
        }
        #endregion

        #region DAA002P02
        public DAA002P02(int PK_SEQ, string MNUF_CODE, string ORDR_YEAR, string DPRT_CODE, string DCSN_NUMB, string CALC_DEGR, string FormId)
        {
            InitializeComponent();

            iPK_SEQ = PK_SEQ;
            strMNUF_CODE = MNUF_CODE;   //제출업체   
            strORDR_YEAR = ORDR_YEAR;   //요구연도
            strDPRT_CODE = DPRT_CODE;   //구매부서
            strDCSN_NUMB = DCSN_NUMB;   //판단번호
            strCALC_DEGR = CALC_DEGR;   //차수
            strFormId = FormId;
        }
        #endregion

        #region DAA002P02_Load
        private void DAA002P02_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtM_ORDR_YEAR.Value = strORDR_YEAR;   //요구연도      
            txtM_DCSN_NUMB.Value = strDCSN_NUMB;   //판단번호
            txtM_CALC_DEGR.Value = strCALC_DEGR;   //차수  

            //G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='REL1', @pCODE = 'D017', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC2 = '" + strMNUF_CODE + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "통화코드")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목구입선")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D014', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "자재구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D018', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "구매단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D020', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D020', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "운송형태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D019', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            
            this.Text = SystemBase.Base.GetMenuTree(strFormId) + " > 입고이력및외주단가수집";
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수조회조건 체크
                {
                    strSql = " usp_DAA002P01  ";
                    strSql += "  @pTYPE = 'S3'";
                    strSql += ", @pDATA_FLAG = 'RCPT' ";
                    strSql += ", @pSTD_SEQ = " + iPK_SEQ + " ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, false, 0, 0);

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목부품관리번호")].Text == "ERROR")
                            {
                                fpSpread1.Sheets[0].Cells[i, 0, i, fpSpread1.Sheets[0].ColumnCount - 1].ForeColor = Color.Red;
                            }
                        }
                    }
                }
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox1);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {

            string strSql = ""; string strHead = ""; string strGbn = "";
            string ERRCode = "", MSGCode = "";
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true) // 그리드 상단 필수항목 체크
                {
                    for (int i = 0; i < (fpSpread1.Sheets[0].Rows.Count - fpSpread1.Sheets[0].FrozenTrailingRowCount); i++)
                    {
                        strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        strGbn = "";
                        if (strHead.Length > 0)
                        {
                            if (strHead != "합계")
                            {
                                switch (strHead)
                                {
                                    case "U": strGbn = "U1"; break;
                                    //case "D": strGbn = "D1"; break;
                                    // case "I": strGbn = "I1"; break;
                                    default: strGbn = ""; break;
                                }

                                strSql = " usp_DAA002P01  ";
                                strSql += "  @pTYPE = '" + strGbn + "'";
                                strSql += ", @pDATA_FLAG = 'RCPT' ";
                                strSql += ", @pKEY_ID                     = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MASTER KEY")].Text.ToString(), ",");
                                strSql += ", @pCONTRACT_ITEM_SEQ          = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계약품목순번")].Text.ToString(), ",");
                                strSql += ", @pRECEIPT_TRANS_ID           = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고트랜잭션ID")].Text.ToString() + "'";
                                strSql += ", @pCHILD_PART_ID              = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목번호")].Text.ToString() + "'";
                                strSql += ", @pCHILD_PART_NAME              = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목명")].Text.ToString() + "'";

                                strSql += ", @pNIIN                        = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계약품목재고번호")].Text.ToString() + "'";
                                strSql += ", @pCONTRACT_ERP_PART_ID        = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계약품목ERP품목번호")].Text.ToString() + "'";
                                strSql += ", @pCOMPOSITION_ITEM_SEQ        = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구성품목순번")].Text.ToString(), ",");
                                strSql += ", @pCOMPOSITION_NATION_STOCK_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구성품목국가재고번호")].Text.ToString() + "'";
                                strSql += ", @pCOMPOSITION_ERP_PART_ID     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구성품목ERP품목번호")].Text.ToString() + "'";
                                //strSql += ", @pCHILD_PART_ID               = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목번호")].Text.ToString() + "'";  
                                strSql += ", @pFACTORY_CODE                = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드")].Value + "'";
                                strSql += ", @pCURRENCY_CODE               = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통화코드")].Value + "'";
                                strSql += ", @pQTY                         = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Text.ToString(), ",");
                                strSql += ", @pPRICE                       = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text.ToString(), ",");
                                strSql += ", @pAMT                         = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text.ToString(), ",");
                                strSql += ", @pPURCHASE_ORDER_NO           = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더번호")].Text.ToString() + "'";
                                strSql += ", @pPURCHASE_ORDER_LINE_NO      = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더라인번호")].Text.ToString(), ",");
                                strSql += ", @pMANUFACTURE_ORDER_NO        = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산오더번호")].Text.ToString() + "'";
                                strSql += ", @pMANUFACTURE_OPERATION_NO    = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산오더공정번호")].Text.ToString(), ",");
                                strSql += ", @pRECEIPT_NO                  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text.ToString() + "'";
                                strSql += ", @pRECEIPT_LINE                = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고라인번호")].Text.ToString(), ",");
                                strSql += ", @pDELIVERY_DATE               = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도착일자")].Text.Replace("-", "") + "'";
                                strSql += ", @pRECEIPT_DATE                = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고일자")].Text.Replace("-", "") + "'";
                                strSql += ", @pITEM_PURCHASE_LOCATION      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목구입선")].Value + "'";
                                strSql += ", @pMATERIALS_FLAG              = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자재구분")].Value + "'";
                                strSql += ", @pPURCHASE_UNIT               = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매단위")].Value + "'";
                                strSql += ", @pSTOCK_UNIT                  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")].Value + "'";
                                strSql += ", @pCHANGE_QTY                  = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환산량")].Text.ToString(), ",");
                                strSql += ", @pSUPPLIER_CODE               = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자코드")].Text.ToString() + "'";
                                strSql += ", @pSUPPLIER_NAME               = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자이름")].Text.ToString() + "'";
                                strSql += ", @pORIGINAL_MAKER_NAME         = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산자명")].Text.ToString() + "'";
                                strSql += ", @pTAX_BILL_APPROVAL_NO        = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세금계산서승인번호")].Text.ToString() + "'";
                                strSql += ", @pSUPPLY_PRICE                = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총공급가액")].Text.ToString(), ",");
                                strSql += ", @pTAX_BILL_PART_SEQ           = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세금계산서품목순번")].Text.ToString(), ",");
                                strSql += ", @pTAX_BILL_PART_ID            = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세금계산서품목번호")].Text.ToString() + "'";
                                strSql += ", @pTAX_BILL_PART_NAME          = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세금계산서품목명")].Text.ToString() + "'";
                                strSql += ", @pPART_SUPPLY_PRICE           = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목별공급가액")].Text.ToString(), ",");
                                strSql += ", @pPART_MANAGER_NO             = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부품관리번호")].Text.ToString() + "'";
                                strSql += ", @pBL_NO                       = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L번호")].Text.ToString() + "'";
                                strSql += ", @pLC_NO                       = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C번호")].Text.ToString() + "'";
                                strSql += ", @pIMPORT_DECLARATION_PATMENT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수입신고필증신고번호")].Text.ToString() + "'";
                                strSql += ", @pTAKE_IN_DATE                = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반입일자")].Text.Replace("-", "") + "'";
                                strSql += ", @pTRANSPORT_FLAG              = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "운송형태")].Value + "'";
                                strSql += ", @pPORT_NATION                 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "적출국")].Text.ToString() + "'";
                                strSql += ", @pEXTRADION_FLAG              = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "인도조건")].Text.ToString() + "'";
                                strSql += ", @pDECLARATION_PATMENT_AMT     = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고필증결재금액")].Text.ToString(), ",");
                                strSql += ", @pPATMENT_WAY                 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재방법")].Text.ToString() + "'";
                                strSql += ", @pSTANDARD_NO                 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "란번호")].Text.ToString() + "'";
                                strSql += ", @pSTANDARD_SEQ                = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격일련번호")].Text.ToString(), ",");
                                strSql += ", @pMODEL_STANDARD_NAME         = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "모델규격명")].Text.ToString() + "'";
                                strSql += ", @pDECLARATION_PATMENT_QTY     = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고필증품목수량")].Text.ToString(), ",");
                                strSql += ", @pHS_CODE                     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세번번호(HS코드)")].Text.ToString() + "'";
                                strSql += ", @pTARIFF_RATE                 = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세종=관 관세율")].Text.ToString(), ",");
                                strSql += ", @pREDUCTION_RATE              = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세종=관 감면율")].Text.ToString(), ",");
                                strSql += ", @pAGRICULTURAL_RATE           = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "농특세율")].Text.ToString(), ",");
                                strSql += ", @pINPUT_QTY                   = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "투입수량")].Text.ToString(), ",");
                                strSql += ", @pIN_ID                       = '" + SystemBase.Base.gstrUserID + "'";         //사용자                             

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }
                            }
                        }
                    }
                }

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
            }

        Exit:
            dbConn.Close();
            this.Cursor = System.Windows.Forms.Cursors.Default;

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SearchExec();
            }
            else if (ERRCode == "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (ERRCode == "WR")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region DAA002P02_Shown()
        private void DAA002P02_Shown(object sender, EventArgs e)
        {
            SearchExec();
        }
        #endregion
    }
}
