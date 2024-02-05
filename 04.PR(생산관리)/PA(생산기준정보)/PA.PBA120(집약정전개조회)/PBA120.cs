#region 작성정보
/*********************************************************************/
// 단위업무명 : 집약정전개조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-15
// 작성내용 : 집약정전개조회 및 관리
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
using WNDW;
using System.Threading;

namespace PA.PBA120
{
    public partial class PBA120 : UIForm.FPCOMM1
    {
        #region 생성자
        public PBA120()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void PBA120_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            // 그리드 콤보 셋팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "BOM TYPE")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            //콤보박스세팅
            SystemBase.ComboMake.C1Combo(cboBOM_NO, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// BOM TYPE
            SystemBase.ComboMake.C1Combo(cboITEM_UNIT, "usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //단위
            SystemBase.ComboMake.C1Combo(cboITEM_TYPE, "usp_B_COMMON @pType='COMM', @pCODE = 'B011', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //조달구분

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타세팅
            txtPLANT_CD.Value = SystemBase.Base.gstrPLANT_CD;
            dtpVALID_FROM_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            txtWORK_QTY.Value = 1;
            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타세팅
            txtPLANT_CD.Value = SystemBase.Base.gstrPLANT_CD;
            dtpVALID_FROM_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            txtWORK_QTY.Value = 1;
        }
        #endregion

        #region 조회 조건 팝업
        //공장
        private void btnPLANT_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP' ,@pSPEC1 = 'PLANT_CD', @pSPEC2 = 'PLANT_NM', @pSPEC3 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPLANT_CD.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장코드 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPLANT_CD.Value = Msgs[0].ToString();
                    txtPLANT_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //품목
        private void btnITEM_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtPLANT_CD.Text, true, txtITEM_CD.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Value = Msgs[2].ToString();
                    txtITEM_NM.Value = Msgs[3].ToString();
                    txtITEM_CD.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }
        #endregion

        #region 조회조건 TextChanged
        //공장
        private void txtPLANT_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPLANT_CD.Text != "")
                {
                    txtPLANT_NM.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPLANT_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPLANT_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        //품목
        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtITEM_CD.Text != "")
                {
                    txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtITEM_NM.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {

                    // 품목 정보 조회
                    string strQuery = "usp_PBA120 @pTYPE = 'S2'";

                    strQuery += ", @pPLANT_CD = '" + txtPLANT_CD.Text + "'";
                    strQuery += ", @pITEM_CD  = '" + txtITEM_CD.Text + "'";
                    strQuery += ", @pBOM_NO   = '" + cboBOM_NO.SelectedValue.ToString() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {

                        // 품목정보 셋팅
                        txtBOM_NO.Value = dt.Rows[0]["BOM_NO"].ToString();
                        txtBOM_REMARK.Value = dt.Rows[0]["ITEM_SPEC"].ToString();
                        cboITEM_UNIT.SelectedValue = dt.Rows[0]["ITEM_UNIT"];
                        txtBOM_REMARK.Value = dt.Rows[0]["REMARK"].ToString();

                        // BOM 정보 조회
                        string strRelCd = "";

                        if (Convert.ToString(cboITEM_TYPE.SelectedValue) == "P")
                        {
                            if (rdoRel3.Checked == true) { strRelCd = "3"; }
                            else if (rdoRel4.Checked == true) { strRelCd = "4"; }
                        }

                        strQuery = "usp_PBA120 @pTYPE = 'S1'";
                        strQuery += ", @pPLANT_CD = '" + txtPLANT_CD.Text + "'";
                        strQuery += ", @pITEM_CD  = '" + txtITEM_CD.Text + "'";
                        strQuery += ", @pBOM_NO   = '" + cboBOM_NO.SelectedValue.ToString() + "'";
                        strQuery += ", @pWORK_QTY = '" + txtWORK_QTY.Text + "'";
                        strQuery += ", @pVALID_FROM_DT = '" + dtpVALID_FROM_DT.Text + "'";
                        strQuery += ", @pITEM_TYPE = '" + Convert.ToString(cboITEM_TYPE.SelectedValue) + "'";
                        strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                        strQuery += ", @pREL_CD = '" + strRelCd + "'";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);
                    }
                    else
                    {
                        MessageBox.Show("BOM 정보가 없습니다.");
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }

        }
        #endregion

        #region cboITEM_TYPE_SelectedIndexChanged
        private void cboITEM_TYPE_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Convert.ToString(cboITEM_TYPE.SelectedValue) == "P")
            {
                panel2.Enabled = true;
            }
            else
            {
                panel2.Enabled = false;
                rdoRelAll.Checked = true;
            }
        }
        #endregion
        
    }
}
