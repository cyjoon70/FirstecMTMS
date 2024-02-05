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
    public partial class DAA002P16 : UIForm.FPCOMM1
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

        #region DAA002P16
        public DAA002P16()
        {
            InitializeComponent();
        }
        #endregion

        #region DAA002P16 
        public DAA002P16(int PK_SEQ, string MNUF_CODE, string ORDR_YEAR, string DPRT_CODE, string DCSN_NUMB, string CALC_DEGR, string FormId)
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

        #region DAA002P16_Load
        private void DAA002P16_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtM_ORDR_YEAR.Value = strORDR_YEAR;   //요구연도      
            txtM_DCSN_NUMB.Value = strDCSN_NUMB;   //판단번호
            txtM_CALC_DEGR.Value = strCALC_DEGR;   //차수  

           // G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재료비구분")] = SystemBase.ComboMake.ComboOnGrid("usp_CO_COMM_CODE @pTYPE='COMM', @pCOMP_CODE = 'SYS', @pCODE = 'GA017'", 0);

            this.Text = SystemBase.Base.GetMenuTree(strFormId) + " > 순매출액경비(공제)";
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
                    strSql += ", @pDATA_FLAG = 'REVD' ";
                    strSql += ", @pMASTER_SEQ = " + iPK_SEQ + " ";
                    strSql += ", @pMNUF_CODE = '" + strMNUF_CODE + "' ";
                    strSql += ", @pORDR_YEAR = '" + txtM_ORDR_YEAR.Text + "' ";
                    strSql += ", @pDPRT_CODE = '" + strDPRT_CODE + "' ";
                    strSql += ", @pDCSN_NUMB = '" + txtM_DCSN_NUMB.Text + "' ";
                    strSql += ", @pCALC_DEGR = '" + txtM_CALC_DEGR.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                }
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox1);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region DAA002P16_Shown 조회
        private void DAA002P16_Shown(object sender, EventArgs e)
        {
            SearchExec();
        }
        #endregion


 
    }
}
