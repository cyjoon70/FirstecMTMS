#region DAA003P26 작성 정보
/*************************************************************/
// 단위업무명 : 기타추가경비 각 항목별 내용을 팝업으로 보여준다
// 작 성 자 :   유재규
// 작 성 일 :   2012-11-05
// 작성내용 :   
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 : 
// 참    고 : 
/*************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;

namespace DA.DAA003
{
    public partial class DAA003P26 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strORDR_YEAR = "";   //요구연도
        string strDCSN_NUMB = "";   //판단번호
        string strCALC_DEGR = "";   //차수
        string strDPRT_CODE = "";   //구매부서
        string strCTMP_CODE = "";   //조달업체
        string strMNUF_CODE = "";   //제출업체                
        string strSTD_YRMON = "";   //제출년월 
        string strNIIN = "";        //재고번호
        string strUNIT = "";        //단위
        string strDMST_ITNB = "";   //항목번호
        string strKeyGroup = "";    //키그룹
        string strSql = "";
        int FrozenCol = 0;
        int FrozenRow = 0;
        string strFormId = "";
        string strFormName = "";
        string strESB_BIZNES_TRNSTN_ID = "";     //트랜잭션 아이디
        #endregion

        #region DAA003P26()
        public DAA003P26()
        {
            InitializeComponent();
        }
        #endregion

        #region DAA003P26()
        public DAA003P26(string ORDR_YEAR, string DCSN_NUMB, string CALC_DEGR, string DPRT_CODE, 
                       string CTMP_CODE, string MNUF_CODE, string STD_YRMON, string NIIN,
                       string UNIT, string DMST_ITNB, string KeyGroup, string FormId, string FormName, string ESB_BIZNES_TRNSTN_ID)
        {
            InitializeComponent();

            strORDR_YEAR = ORDR_YEAR;   //요구연도
            strDCSN_NUMB = DCSN_NUMB;   //판단번호
            strCALC_DEGR = CALC_DEGR;   //차수
            strDPRT_CODE = DPRT_CODE;   //구매부서
            strCTMP_CODE = CTMP_CODE;   //조달업체
            strMNUF_CODE = MNUF_CODE;   //제출업체                
            strSTD_YRMON = STD_YRMON;   //제출년월 
            strNIIN = NIIN;             //재고번호
            strUNIT = UNIT;             //단위
            strDMST_ITNB = DMST_ITNB;   //항목번호
            strKeyGroup = KeyGroup;     //키그룹
            strFormId = FormId;
            strFormName = FormName;
            strESB_BIZNES_TRNSTN_ID = ESB_BIZNES_TRNSTN_ID;     //트랜잭션 아이디
        }
        #endregion

        #region DAA003P26_Load
        private void DAA003P26_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("011111011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboM_MNUF_CODE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'");   //제출업체
            SystemBase.ComboMake.C1Combo(cboM_CTMP_CODE, "usp_B_COMMON @pTYPE='REL1', @pCODE = 'D006', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = '" + cboM_MNUF_CODE.SelectedValue.ToString() + "'"); //계약업체

            txtM_ORDR_YEAR.Value = strORDR_YEAR;   //요구연도
            txtM_DCSN_NUMB.Value = strDCSN_NUMB;   //판단번호
            txtM_CALC_DEGR.Value = strCALC_DEGR;   //차수
            txtM_DPRT_CODE.Value = strDPRT_CODE;   //구매부서
            cboM_CTMP_CODE.SelectedValue = strCTMP_CODE;   //조달업체
            cboM_MNUF_CODE.SelectedValue = strMNUF_CODE;   //제출업체                
            dtM_STD_YRMON.Value = SystemBase.Validation.C1DataEdit_WriteFormat(strSTD_YRMON,"YYYY-MM");   //제출년월 
            txtM_NIIN.Value = strNIIN;             //재고번호
            txtM_UNIT.Value = strUNIT;             //단위
            txtM_DMST_ITNB.Value = strDMST_ITNB;   //항목번호
                       
            //SearchExec();
            this.Text = SystemBase.Base.GetMenuTree(strFormId) + " > " + SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", strFormName, "AND COMP_CODE =  '" + SystemBase.Base.gstrCOMCD + "' AND MAJOR_CD = 'D023'  ");
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY048"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dsMsg == DialogResult.Yes)
            {

                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수여부체크
                {
                    string strSql = ""; string strHead = ""; string strGbn = "";                   
                    string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                    try
                    {   
                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, "DAA003P24", "fpSpread1", false) == true)
                        {
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                                strGbn = "";
                                if (strHead.Length > 0)
                                {
                                    switch (strHead)
                                    {
                                        case "U": strGbn = "U1"; break;
                                        case "D": strGbn = "D1"; break;
                                        case "I": strGbn = "U1"; break;  // insert 처리도 U1으로
                                        default: strGbn = ""; break;
                                    }

                                    strSql = " usp_DAA003기타추가경비 ";
                                    strSql += "  @pTYPE = '" + strGbn + "'";
                                    strSql += ", @pORDR_YEAR = '" + txtM_ORDR_YEAR.Value.ToString() + "'";   //요구년도
                                    strSql += ", @pDCSN_NUMB = '" + txtM_DCSN_NUMB.Value.ToString() + "'";   //판단번호
                                    strSql += ", @pCALC_DEGR ='" + txtM_CALC_DEGR.Value.ToString()+"' ";     //제출차수
                                    strSql += ", @pDPRT_CODE ='" + txtM_DPRT_CODE.Value.ToString() + "' ";   //구매부서코드
                                    strSql += ", @pCTMF_CODE ='" + cboM_CTMP_CODE.SelectedValue.ToString() + "' ";    //계약업체                                     
                                    strSql += ", @pMNUF_CODE ='" + cboM_MNUF_CODE.SelectedValue.ToString() + "' ";  //제출업체
                                    strSql += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준년월

                                    strSql += ", @pNIIN ='" + txtM_NIIN.Value.ToString() + "' "; //재고번호
                                    strSql += ", @pUNIT ='" + txtM_UNIT.Value.ToString() + "' "; //단위
                                    strSql += ", @pDMST_ITNB='" + txtM_DMST_ITNB.Value.ToString() + "' "; //항목
                                    strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";       //사용자

                                    strSql += ", @pREGE_SNUM =" + (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "등록순번")].Value == null ? 0 : fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "등록순번")].Value  )+ " ";
                                    strSql += ", @pADDX_DIVS ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비목명")].Text.ToString() + "' ";
                                    strSql += ", @pUNIT_CAMA ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상각방법")].Value + "' ";
                                    strSql += ", @pOCRC_DATE ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생일자")].Text.ToString().Replace("-", "") + "' ";
                                    strSql += ", @pOCIT_NAME ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생항목명")].Text.ToString() + "' ";
                                    strSql += ", @pOCRC_AMNT =" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액")].Value + " ";
                                    strSql += ", @pTOTL_TIME =" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총작업시간")].Value + " ";
                                    strSql += ", @pSTND_QNTY =" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배부기준량")].Value + " ";
                                    strSql += ", @pPROF_NUMB ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "증빙")].Text.ToString() + "' ";
                                    strSql += ", @pNOTE ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text.ToString() + "' ";  
                                  
  
                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();
                                    
                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }
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
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수조회조건 체크
                {
                    FpGrid_DataSet();
                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, FrozenRow, FrozenCol, false, true);
                }
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox1);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region FpGrid_DataSet
        private void FpGrid_DataSet()
        {
            try
            {
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "비목명")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D027', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "상각방법")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D030', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

                FrozenCol = SystemBase.Base.GridHeadIndex(GHIdx1, "발생항목명"); // 고정컬럼

                strSql = " usp_DAA003_기타추가경비  ";
                strSql += "  @pTYPE = 'S1'";
                strSql += ", @pKEY_GROUP = '" + strKeyGroup + "' ";
                strSql += ", @pESB_BIZNES_TRNSTN_ID ='" + strESB_BIZNES_TRNSTN_ID + "'";
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 구매부서코드 변환시  구매부서명 조회
        private void txtM_DPRT_CODE_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strSql = "AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' AND MAJOR_CD = 'D007'";
                txtM_DPRT_NAME.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtM_DPRT_CODE.Text, strSql);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 단위코드 변환시  단위코드명 조회
        private void txtM_UNIT_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strSql = "AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' AND MAJOR_CD = 'D020'";
                txtM_UNIT_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtM_UNIT.Text, strSql);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region SPREAD EditChange
        private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                if (fpSpread1.ActiveSheet.ColumnHeader.Cells[0, e.Column].Text.ToString() == "발생금액"
                        || fpSpread1.ActiveSheet.ColumnHeader.Cells[0, e.Column].Text.ToString() == "총작업시간"
                        || fpSpread1.ActiveSheet.ColumnHeader.Cells[0, e.Column].Text.ToString() == "배부기준량")
                {
                    if (fpSpread1.ActiveSheet.Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "상각방법")].Value.ToString() == "L")
                    {
                        if (SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "총작업시간")].Text.ToString(), ",") > 0)
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "경비")].Value =
                                 SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액")].Text.ToString(), ",")
                               / SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "총작업시간")].Text.ToString(), ",")
                               * SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "배부기준량")].Text.ToString(), ",");
                        }
                    }
                    else
                    {
                        if (SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "배부기준량")].Text.ToString(), ",") > 0)
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "경비")].Value =
                                 SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액")].Text.ToString(), ",")
                               / SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "배부기준량")].Text.ToString(), ",");
                        }
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region DAA003P26_Shown
        private void DAA003P26_Shown(object sender, EventArgs e)
        {
            SearchExec();
        }
        #endregion
    }
}
