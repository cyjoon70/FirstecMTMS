using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UIForm;

namespace DA.DAA002
{
    public partial class DAA002P01 : UIForm.FPCOMM2
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

        int PreRow = -1;
        int PreRow2 = -1;
        #endregion

        #region DAA002P01
        public DAA002P01()
        {
            InitializeComponent();
        }
        #endregion

        #region DAA002P01()

        public DAA002P01( int PK_SEQ, string MNUF_CODE,  string ORDR_YEAR, string DPRT_CODE , string DCSN_NUMB, string CALC_DEGR,  string FormId)
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

        #region DAA002P01_Load()
        private void DAA002P01_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            
            txtM_ORDR_YEAR.Value = strORDR_YEAR;   //요구연도      
            txtM_DCSN_NUMB.Value = strDCSN_NUMB;   //판단번호
            txtM_CALC_DEGR.Value = strCALC_DEGR;   //차수  

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D020', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D020', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "환산단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D020', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목구입선")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D014', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "WBS타입")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D015', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "소재구입선")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D016', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

            this.Text = SystemBase.Base.GetMenuTree(strFormId) + " > BOM정보수집";

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수조회조건 체크
                {
                    //strSql = " usp_DAA002P01  ";
                    //strSql += "  @pTYPE = 'S1'";
                    //strSql += ", @pDATA_FLAG = 'BOM' ";
                    //strSql += ", @pMASTER_SEQ = " + iPK_SEQ + " ";
                    //strSql += ", @pMNUF_CODE = '" + strMNUF_CODE + "' ";
                    //strSql += ", @pORDR_YEAR = '" + txtM_ORDR_YEAR.Text + "' ";
                    //strSql += ", @pDPRT_CODE = '" + strDPRT_CODE + "' ";
                    //strSql += ", @pDCSN_NUMB = '" + txtM_DCSN_NUMB.Text + "' ";
                    //strSql += ", @pCALC_DEGR = '" + txtM_CALC_DEGR.Text + "' ";

                    //UIForm.FPMake.grdCommSheet(fpSpread3, strSql, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, true, true, 0, 0);
                    //UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

                    strSql = " usp_DAA002P01  ";
                    strSql += "  @pTYPE = 'S2'";
                    strSql += ", @pDATA_FLAG = 'BOM' ";
                    strSql += ", @pSTD_SEQ = " + iPK_SEQ + " ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strSql, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region ExcelExec() 엑셀다운로드 로직
        protected override void ExcelExec()
        {
            try
            {
                string strSql = "";

                strSql = " usp_DAA002P01  ";
                strSql += "  @pTYPE    = 'E1'";
                strSql += ", @pSTD_SEQ = " + iPK_SEQ + " ";
                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                ExcelDown.ExcelDownLoad(this.Text,  ds.Tables[0]);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        #endregion

        #region SelectionChanged
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    int Row = fpSpread2.ActiveSheet.ActiveRowIndex;
                    int Col = fpSpread2.ActiveSheet.ActiveColumnIndex;

                    if (Row < 0) return;
                    if (PreRow2 == Row && PreRow2 != -1) return;

                    strSql = " usp_DAA002P01  ";
                    strSql += "  @pTYPE = 'S3'";
                    strSql += ", @pDATA_FLAG = 'BOM' ";
                    strSql += ", @pSTD_SEQ = " + iPK_SEQ + " ";
                    strSql += ", @pSTD_DTL_SEQ = " + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text.ToString() + " ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

                    PreRow2 = fpSpread2.ActiveSheet.ActiveRowIndex;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region DAA002P01_Shown() 
        private void DAA002P01_Shown(object sender, EventArgs e)
        {
            SearchExec();
        }
        #endregion

    }
}
