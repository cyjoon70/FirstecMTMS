#region DAA003P09 작성 정보
/*************************************************************/
// 단위업무명 : BOM정보와 단가표정보로 국내구입 부품비 자료를 생성한다.
// 작 성 자 :   유재규
// 작 성 일 :   2012-11-06
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

namespace DA.DAA003
{
    public partial class DAA003P09 : UIForm.FPCOMM1
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

        #region DAA003P09()
        public DAA003P09()
        {
            InitializeComponent();
        }
        #endregion

        #region DAA003P09()
        public DAA003P09(string ORDR_YEAR, string DCSN_NUMB, string CALC_DEGR, string DPRT_CODE, 
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

        #region DAA003P09_Load
        private void DAA003P09_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

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

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수조회조건 체크
                {
                    FpGrid_DataSet();
                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, FrozenRow, FrozenCol, true);
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
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D020', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

                FrozenCol = 4; // 고정컬럼(업체품번 까지)

                strSql = " usp_DAA003_국내구입부품비  ";
                strSql += "  @pTYPE = 'S1'";
                strSql += ", @pKEY_GROUP = '" + strKeyGroup + "' ";
                strSql += ", @pESB_BIZNES_TRNSTN_ID ='" + strESB_BIZNES_TRNSTN_ID + "' ";
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

        #region DAA003P09_Shown
        private void DAA003P09_Shown(object sender, EventArgs e)
        {
            SearchExec();
        }
        #endregion
    }
}
