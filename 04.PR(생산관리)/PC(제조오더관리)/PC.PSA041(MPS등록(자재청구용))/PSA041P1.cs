#region 작성정보
/*********************************************************************/
// 단위업무명 : MRP등록(자재청구용)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-04
// 작성내용 : MRP등록(자재청구용) 및 관리
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

namespace PC.PSA041
{ 
    public partial class PSA041P1 : UIForm.FPCOMM1
    {
        string[] returnVal = null;

        public PSA041P1()
        { 
            InitializeComponent();           
        }

        #region Form Load 시
        private void PSA041P1_Load(object sender, System.EventArgs e)
        { 
            //GroupBo x1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "상태")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P012', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "오더고정")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P013', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "MPS구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P014', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P038', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "DATA작성유무")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B029', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

            SystemBase.ComboMake.C1Combo(cboSTATUS, "usp_P_COMMON @pTYPE	= 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P012', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 3);	// 

            dtpRECEIVE_ST.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-6).ToShortDateString();

            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 1);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strMQuery = "";
                    strMQuery = " usp_PSA041 'S3'";
                    strMQuery += ", @pPLANT_CD='" + txtPlant_CD.Text + "'";
                    strMQuery += ", @pDELIVERY_DT='" + dtpDelivery_ST.Text + "'";
                    strMQuery += ", @pDELIVERY_ED='" + dtpDelivery_ED.Text + "'";
                    strMQuery += ", @pRECEIVE_ST='" + dtpRECEIVE_ST.Text + "'";
                    strMQuery += ", @pRECEIVE_ED='" + dtpRECEIVE_ED.Text + "'";
                    strMQuery += ", @pITEM_CD='" + txtITEM_CD.Text + "'";
                    strMQuery += ", @pSTATUS='" + cboSTATUS.SelectedValue.ToString() + "'";
                    strMQuery += ", @pPROJECT_NO='" + txtProject_NO.Text + "'";
                    strMQuery += ", @pPROJECT_SEQ='" + txtProject_SEQ.Text + "'";
                    strMQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 1);
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

        #region NewExec() 신규
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpRECEIVE_ST.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-6).ToShortDateString();
            dtpRECEIVE_ED.Value = null;
            dtpDelivery_ST.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpDelivery_ED.Value = null;

            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 1);
        }
        #endregion

        #region 조회조건 팝업 이벤트
        //공장
        private void btnPlant_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P011' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtPlant_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPlant_CD.Text = Msgs[0].ToString();
                    txtPlant_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목코드
        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtPlant_CD.Text, true, txtITEM_CD.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Text = Msgs[2].ToString();
                    txtITEM_NM.Value = Msgs[3].ToString();
                    txtITEM_CD.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProject_NO.Text, "S1", "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProject_NO.Text = Msgs[3].ToString();
                    txtProject_NM.Value = Msgs[4].ToString();
                    txtProject_SEQ.Text = Msgs[5].ToString();
                    txtITEM_CD.Text = Msgs[6].ToString();
                    txtITEM_NM.Value = Msgs[7].ToString();

                    txtProject_NO.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 리턴값 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }
        public void RtnStr()
        {
            int realRow = 0;
            int intRow = fpSpread1.ActiveSheet.Rows.Count;

            if (intRow > 0)
            {
                for (int i = 0; i < intRow; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True") realRow = realRow + 1;
                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = Convert.ToString(i + 1);
                }
                if (realRow > 0)
                {
                    returnVal = new string[realRow];
                    realRow = 0;

                    for (int i = 0; i < intRow; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                        {
                            returnVal[realRow] = "";
                            for (int j = 2; j < fpSpread1.ActiveSheet.Columns.Count; j++)
                            {
                                if (returnVal[realRow].ToString() != "")
                                    returnVal[realRow] = returnVal[realRow] + "!!" + fpSpread1.Sheets[0].Cells[i, j].Text.ToString();
                                else
                                    returnVal[realRow] = fpSpread1.Sheets[0].Cells[i, j].Value.ToString();
                            }
                            realRow = realRow + 1;
                        }
                    }
                    this.DialogResult = DialogResult.OK;
                }
            }
            this.Close();
        }
        #endregion

        #region 버튼 클릭 이벤트
        private void btnDataOk_Click(object sender, System.EventArgs e)
        {
            RtnStr();
        }

        private void btnAllSelect_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.ActiveSheet.Rows.Count > 0)
            {
                for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "H")
                        fpSpread1.ActiveSheet.Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 1;
                }
            }
        }

        private void btnAllCancel_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.ActiveSheet.Rows.Count > 0)
            {
                for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
                    fpSpread1.ActiveSheet.Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 0;
            }
        }
        #endregion
    }
}
