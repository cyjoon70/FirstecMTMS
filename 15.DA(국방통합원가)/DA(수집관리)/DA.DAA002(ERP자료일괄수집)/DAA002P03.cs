using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DA.DAA002
{
    public partial class DAA002P03 : UIForm.FPCOMM1
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

        #region DAA002P03
        public DAA002P03()
        {
            InitializeComponent();
        }
        #endregion

        #region DAA002P03
        public DAA002P03(int PK_SEQ, string MNUF_CODE, string ORDR_YEAR, string DPRT_CODE, string DCSN_NUMB, string CALC_DEGR, string FormId)
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

        #region DAA002P03_Load
        private void DAA002P03_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtM_ORDR_YEAR.Value = strORDR_YEAR;   //요구연도      
            txtM_DCSN_NUMB.Value = strDCSN_NUMB;   //판단번호
            txtM_CALC_DEGR.Value = strCALC_DEGR;   //차수  

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "통화코드")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "인도조건")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'S005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "운송구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'D019', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "비목구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'D034', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

            this.Text = SystemBase.Base.GetMenuTree(strFormId) + " > 원가수입이력";
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
                    strSql += ", @pDATA_FLAG = 'IMPT' ";
                    strSql += ", @pSTD_SEQ = " + iPK_SEQ + " ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0);
                }
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox1);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region DAA002P03_Shown()
        private void DAA002P03_Shown(object sender, EventArgs e)
        {
            SearchExec();
        }
        #endregion

        #region 상세현황
        private void btnAll_Process_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToString(iPK_SEQ) == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P031 pu = new DAA002P031(iPK_SEQ, strMNUF_CODE, strORDR_YEAR, strDPRT_CODE, strDCSN_NUMB, strCALC_DEGR, strFormId);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
