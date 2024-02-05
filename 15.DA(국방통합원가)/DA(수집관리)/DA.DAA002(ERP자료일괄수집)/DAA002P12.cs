using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using UIForm;
using System.IO;

namespace DA.DAA002
{
    public partial class DAA002P12 : UIForm.FPCOMM3
    {
        #region 변수선언
        int iPK_SEQ = 0;
        string strMNUF_CODE = "";   //제출업체   
        string strORDR_YEAR = "";   //요구연도
        string strDPRT_CODE = "";   //구매부서
        string strDCSN_NUMB = "";   //판단번호
        string strCALC_DEGR = "";   //차수        
        string strPROJECT_ID = "";   //프로젝트  
        string strFormId = "";
        string strSql = "";

        int PreRow = -1;
        int PreRow2 = -1;
        #endregion

        #region DAA002P12
        public DAA002P12()
        {
            InitializeComponent();
        }
        #endregion

        #region DAA002P12()
        
        public DAA002P12( int PK_SEQ, string MNUF_CODE,  string ORDR_YEAR, string DPRT_CODE , string DCSN_NUMB, string CALC_DEGR,  string FormId)
        {
            InitializeComponent();

            iPK_SEQ = PK_SEQ ;
            strMNUF_CODE = MNUF_CODE ;   //제출업체   
            strORDR_YEAR = ORDR_YEAR ;   //요구연도
            strDPRT_CODE = DPRT_CODE ;   //구매부서
            strDCSN_NUMB = DCSN_NUMB ;   //판단번호
            strCALC_DEGR = CALC_DEGR ;   //차수
            strFormId = FormId;

        }
        #endregion

        #region DAA002P12_Load()
        private void DAA002P12_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("011111011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            
            txtM_ORDR_YEAR.Value = strORDR_YEAR;   //요구연도      
            txtM_DCSN_NUMB.Value = strDCSN_NUMB;   //판단번호
            txtM_CALC_DEGR.Value = strCALC_DEGR;   //차수  

            G3Etc[SystemBase.Base.GridHeadIndex(GHIdx3, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D020', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D020', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D020', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "포장형태")] = SystemBase.ComboMake.ComboOnGrid("usp_CO_COMM_CODE @pTYPE='COMM', @pCOMP_CODE = 'SYS', @pCODE = 'GA016'", 0);

            this.Text = SystemBase.Base.GetMenuTree(strFormId) + " > 포장재료비 수집";

        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            try
            {
                UIForm.FPMake.RowInsert(fpSpread1);

                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY_ID")].Text = iPK_SEQ.ToString();
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "계약순번")].Text
                            = fpSpread3.Sheets[0].Cells[fpSpread3.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx3, "순번")].Text.ToString();
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "구성순번")].Text
                            = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text.ToString();
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "모품목번호")].Text
                            = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "ERP품목번호")].Text.ToString();
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "BOM레벨")].Text = "1";
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    strSql = " usp_DAA002P01  ";
                    strSql += "  @pTYPE = 'S1'";
                    strSql += ", @pDATA_FLAG = 'BOM' ";   // 포장 재료비이지만 리스트는  BOM정보 조회
                    strSql += ", @pMASTER_SEQ = " + iPK_SEQ + " ";
                    strSql += ", @pMNUF_CODE = '" + strMNUF_CODE + "' ";
                    strSql += ", @pORDR_YEAR = '" + txtM_ORDR_YEAR.Text + "' ";
                    strSql += ", @pDPRT_CODE = '" + strDPRT_CODE + "' ";
                    strSql += ", @pDCSN_NUMB = '" + txtM_DCSN_NUMB.Text + "' ";
                    strSql += ", @pCALC_DEGR = '" + txtM_CALC_DEGR.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread3, strSql, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, true, 0, 0);
                    UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    PreRow = -1;
                    PreRow2 = -1;

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

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수여부체크
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, "DAA002P12", "fpSpread1", true) == true)
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY048"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
                        string strHead = ""; string strGbn = "";
                        string ERRCode = "", MSGCode = "";
                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                        this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                        try
                        {
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                                strGbn = "";
                                if (strHead.Length > 0)
                                {
                                    switch (strHead)
                                    {
                                        case "D": strGbn = "D1"; break;
                                        case "U": strGbn = "U1"; break;
                                        case "I": strGbn = "I1"; break;
                                        default: strGbn = ""; break;
                                    }

                                    string strSql = " usp_DAA002P01 ";
                                    strSql += "  @pTYPE = '" + strGbn + "'";
                                    strSql += ", @pDATA_FLAG = 'PACK' ";
                                    strSql += ", @pKEY_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY_ID")].Text.ToString() + "' ";
                                    strSql += ", @pCONTRACT_ITEM_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계약순번")].Text.ToString() + "' ";
                                    strSql += ", @pCOMPOSITION_ITEM_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구성순번")].Text.ToString() + "' ";

                                    strSql += ", @pPARENT_PART_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "모품목번호")].Text.ToString() + "' ";
                                    strSql += ", @pCHILD_PART_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목번호")].Text.ToString() + "' ";
                                    strSql += ", @pCHILD_PART_NAME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목명세")].Text.ToString() + "' ";
                                    strSql += ", @pBOM_LEVEL = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "BOM레벨")].Text.ToString() + "' ";

                                    strSql += ", @pPACKING_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "포장형태")].Value.ToString() + "' ";
                                    strSql += ", @pSTOCK_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value.ToString() + "' ";

                                    strSql += ", @pQTY_PER = '" + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "소요량")].Text.ToString(), ",") + "' ";
                                    strSql += ", @pPACKING_QTY = '" + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "포장수량")].Text.ToString(), ",") + "' ";
                                    strSql += ", @pPRICE = '" + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text.ToString(), ",") + "' ";
                                    strSql += ", @pAPST_NBMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시작호기")].Text.ToString() + "' ";
                                    strSql += ", @pAPFN_NBMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종료호기")].Text.ToString() + "' ";

                                    strSql += ", @pIN_ID ='" + SystemBase.Base.gstrUserID + "' ";                                  //사용자

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }
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
                            fpSpread1_Search();

                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (ERRCode == "ER")
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
        #endregion


        #region SelectionChanged
        private void fpSpread3_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                if (fpSpread3.Sheets[0].Rows.Count > 0)
                {
                    int Row = fpSpread3.ActiveSheet.ActiveRowIndex;
                    int Col = fpSpread3.ActiveSheet.ActiveColumnIndex;

                    if (Row < 0) return;
                    if (PreRow == Row && PreRow != -1) return;

                    strSql = " usp_DAA002P01  ";
                    strSql += "  @pTYPE = 'S2'";
                    strSql += ", @pDATA_FLAG = 'PACK' ";
                    strSql += ", @pMASTER_SEQ = " + fpSpread3.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx3, "순번")].Text.ToString() + " ";
                    strSql += ", @pMNUF_CODE = '" + strMNUF_CODE + "' ";
                    strSql += ", @pORDR_YEAR = '" + txtM_ORDR_YEAR.Text + "' ";
                    strSql += ", @pDPRT_CODE = '" + strDPRT_CODE + "' ";
                    strSql += ", @pDCSN_NUMB = '" + txtM_DCSN_NUMB.Text + "' ";
                    strSql += ", @pCALC_DEGR = '" + txtM_CALC_DEGR.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strSql, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);



                    PreRow = fpSpread3.ActiveSheet.ActiveRowIndex;
                    PreRow2 = -1;

                    this.Cursor = Cursors.Default;

                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Cursor = Cursors.Default;
            }
        }        

        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    int Row = fpSpread2.ActiveSheet.ActiveRowIndex;
                    int Col = fpSpread2.ActiveSheet.ActiveColumnIndex;

                    if (Row < 0) return;
                    if (PreRow2 == Row && PreRow2 != -1) return;

                    fpSpread1_Search();

                    PreRow2 = fpSpread2.ActiveSheet.ActiveRowIndex;

                    this.Cursor = Cursors.Default;

                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region fpSpread1_ButtonClicked
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                #region 자품목
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자품목번호_2"))
                {
                    string[] strSearch = null;
                    string strQuery = " usp_ERP_COMM @pERP_TYPE = 'PART' ";
                    string[] strWhere = new string[] { "@pERP_CODE", "@pERP_NAME" };
                    strSearch = new string[] { "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P1060", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목번호")].Value = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목명세")].Value = Msgs[1].ToString();
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion
        #endregion

        #region fpSpread1_Search
        private void fpSpread1_Search()
        {
            try
            {
                strSql = " usp_DAA002P01  ";
                strSql += "  @pTYPE = 'S3'";
                strSql += ", @pDATA_FLAG = 'PACK' ";
                strSql += ", @pMASTER_SEQ = " + iPK_SEQ + " ";
                strSql += ", @pDETAIL_SEQ = " + fpSpread3.Sheets[0].Cells[fpSpread3.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx3, "순번")].Text.ToString() + " ";
                strSql += ", @pITEM_SEQ = " + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text.ToString() + " ";
                strSql += ", @pMNUF_CODE = '" + strMNUF_CODE + "' ";
                strSql += ", @pORDR_YEAR = '" + txtM_ORDR_YEAR.Text + "' ";
                strSql += ", @pDPRT_CODE = '" + strDPRT_CODE + "' ";
                strSql += ", @pDCSN_NUMB = '" + txtM_DCSN_NUMB.Text + "' ";
                strSql += ", @pCALC_DEGR = '" + txtM_CALC_DEGR.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region DAA002P12_Shown()
        private void DAA002P12_Shown(object sender, EventArgs e)
        {
            SearchExec();
        }
        #endregion
    }
}
