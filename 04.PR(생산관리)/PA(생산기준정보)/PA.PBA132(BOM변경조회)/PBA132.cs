#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목정보등록(멀티)
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-01
// 작성내용 : 품목 정보 등록 및 관리
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
using System.Data.OleDb;

namespace PA.PBA132
{
    public partial class PBA132 : UIForm.FPCOMM1
    {
        #region 생성자
        public PBA132()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void PBA132_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "등록위치")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P066', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            SystemBase.ComboMake.C1Combo(cboBOM_TYPE, "usp_B_COMMON @pTYPE = 'REL1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'P006', @pSPEC2 = 'S' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");	// BOM TYPE
 
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            
            try
            {
                fpSpread1.Sheets[0].RowCount = 0;
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strMQuery = "";
                    strMQuery = "   usp_PBA132 @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strMQuery += ",            @pITEM_CD = '" + txtSItemCd.Text + "' ";
                    if (dtpSRrvDt_Fr.Text != "")
                        strMQuery += ",            @pREVISION_DATA_FR = '" + dtpSRrvDt_Fr.Text + "' ";
                    if (dtpSRrvDt_To.Text != "")
                        strMQuery += ",            @pREVISION_DATA_TO = '" + dtpSRrvDt_To.Text + "' ";
                    if (txtSRevNo.Text != "")
                        strMQuery += ",            @pREV_NO = '" + txtSRevNo.Text + "' ";
                    // 2018.01.02. hma 추가(Start): PLM 리비전번호 및 시스템구분 체크
                    if (txtPLMRevision.Text != "")
                        strMQuery += ",            @pPLM_REV_NO = '" + txtPLMRevision.Text + "' ";
                    if (rdoERP.Checked == true)
                        strMQuery += ",            @pSYSTEM_TYPE = 'ERP' ";
                    else
                        strMQuery += ",            @pSYSTEM_TYPE = 'PLM' ";
                    // 2018.01.02. hma 추가(End)

                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true,0,0, true);

                    fpSpread1.ActiveSheet.Columns[1].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                    fpSpread1.ActiveSheet.Columns[2].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Restricted;
                    fpSpread1.ActiveSheet.Columns[3].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Restricted;
                    fpSpread1.ActiveSheet.Columns[4].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Restricted;
                    fpSpread1.ActiveSheet.Columns[5].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Restricted;
                    fpSpread1.ActiveSheet.Columns[6].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Restricted;
                    fpSpread1.ActiveSheet.Columns[7].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Restricted;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
 
        }
        #endregion

        #region 품목코드 조회 변경시
        private void txtSITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            if (txtSItemCd.Text != "")
            {
                txtSItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtSItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            }
            else
            {
                txtSItemNm.Value = "";
            }
        }
        #endregion


        #region 조회조건 팝업
        //품목코드(조회용)팝업
        private void btnSItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtSItemCd.Text, txtSItemNm.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, "품목코드 조회", new int[] { 1, 2 }, true);
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {

                    txtSItemCd.Value = pu.ReturnValue[1].ToString();
                    txtSItemNm.Value = pu.ReturnValue[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion


    }
}
