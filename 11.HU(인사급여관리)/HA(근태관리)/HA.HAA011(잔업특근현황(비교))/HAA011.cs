#region 작성정보
/*********************************************************************/
// 단위업무명 : 잔업특근현황(비교)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-24
// 작성내용 : 잔업특근현황(비교) 및 관리
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

namespace HA.HAA011
{
    public partial class HAA011 : UIForm.FPCOMM1
    {
        #region 생성자
        public HAA011()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void HAA011_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpDateTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpDateTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 8, false);	
		
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_HAA011  @pTYPE = 'S1', @pINTERNAL_CD = '" + txtInternalCd.Text + "' ";
                strQuery = strQuery + " , @pDATE = '" + dtpDate.Text + "' ";
                strQuery = strQuery + " , @pDATE_TO = '" + dtpDateTo.Text + "' ";
                strQuery = strQuery + " , @pEMP_NO = '" + txtEmpNo.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 8);

                //Merge
                fpSpread1.Sheets[0].SetColumnMerge(1, FarPoint.Win.Spread.Model.MergePolicy.Always);
                fpSpread1.Sheets[0].SetColumnMerge(2, FarPoint.Win.Spread.Model.MergePolicy.Always);
                fpSpread1.Sheets[0].SetColumnMerge(3, FarPoint.Win.Spread.Model.MergePolicy.Always);
                fpSpread1.Sheets[0].SetColumnMerge(4, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                fpSpread1.Sheets[0].SetColumnMerge(5, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                fpSpread1.Sheets[0].SetColumnMerge(6, FarPoint.Win.Spread.Model.MergePolicy.Restricted);

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 부서코드 팝업
        private void btnDept_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_H_COMMON @pType='H014', @pDATE = '" + dtpDate.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtDeptCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("H00001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "부서 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtDeptCd.Value = Msgs[0].ToString();
                    txtDeptNm.Value = Msgs[1].ToString();
                    txtInternalCd.Value = Msgs[2].ToString();
                    txtDeptCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 사원번호 팝업
        private void btnEmpNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_H_COMMON @pType='H003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtEmpNo.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("H00002", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사원 조회");
                pu.Width = 700;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEmpNo.Value = Msgs[0].ToString();
                    txtEmpNm.Value = Msgs[1].ToString();
                    txtEmpNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 사원번호TextChanged 이벤트
        private void txtEmpNo_TextChanged(object sender, System.EventArgs e)
        {
            string strQuery = "usp_H_COMMON @pType='H004', @pCOM_CD = '" + txtEmpNo.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtEmpNm.Value = dt.Rows[0][1].ToString();
                txtEmpNo.Focus();
            }
            else
            {
                txtEmpNm.Value = "";
                txtEmpNo.Focus();
            }
        }
        #endregion

        #region 부서코드 TextChanged 이벤트
        private void txtDeptCd_TextChanged(object sender, System.EventArgs e)
        {
            string strQuery = "usp_H_COMMON @pType='H002', @pDATE = '" + dtpDate.Text + "', @pCOM_CD = '" + txtDeptCd.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtDeptNm.Value = dt.Rows[0][1].ToString();
                txtInternalCd.Value = dt.Rows[0][2].ToString();
                txtDeptCd.Focus();
            }
            else
            {
                txtDeptNm.Value = "";
                txtInternalCd.Value = "";
                txtDeptCd.Focus();
            }
        }
        #endregion

    }
}
