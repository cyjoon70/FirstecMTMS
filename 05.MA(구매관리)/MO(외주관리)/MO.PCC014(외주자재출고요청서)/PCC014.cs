#region 작성정보
/*********************************************************************/
// 단위업무명 : 외주자재출고요청서
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-18
// 작성내용 : 외주자재출고요청서출력 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;

namespace MO.PCC014
{
    public partial class PCC014 : UIForm.Buttons
    {
        public PCC014()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void PCC014_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //기타세팅
            txtChilePlantCd.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //필수체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //기타세팅
            txtChilePlantCd.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region 조회조건 팝업
        //공장
        private void btnChilePlantCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP' ,@pSPEC1 = 'PLANT_CD', @pSPEC2 = 'PLANT_NM', @pSPEC3 = 'B_PLANT_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtChilePlantCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장코드 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtChilePlantCd.Text = Msgs[0].ToString();
                    txtChilePlantNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //품목구분
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP' ,@pLANG_CD ='" + SystemBase.Base.gstrLangCd + "',@pSPEC1 = 'P032', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtItemCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00077", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목구분코드 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtItemCd.Text = Msgs[0].ToString();
                    txtItemNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목구분 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }
        //거래처
        private void btnCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //공장
        private void txtChilePlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtChilePlantCd.Text != "")
                {
                    txtChilePlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtChilePlantCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtChilePlantNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //품목구분
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtItemCd.Text, " AND MAJOR_CD='P032' AND LANG_CD='" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch
            {

            }
        }



        #endregion
        
        #region 레포트 출력
        private void butPreview_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            //조회 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string RptName = "";    // 레포트경로+레포트명
                string[] RptParmValue = new string[11];   // SP 파라메타 값
                string[] FormulaFieldName = new string[1]; //formula 값
                string[] FormulaFieldValue = new string[1]; //formula 이름

                if (rdoWorkOrder.Checked == true)
                {
                    RptName = SystemBase.Base.ProgramWhere + @"\Report\PCC014_1.rpt";
                }
                else
                {
                    RptName = SystemBase.Base.ProgramWhere + @"\Report\PCC014_2.rpt";
                }

                string stockYn = "N";
                if (rdoStockQty1.Checked == true)
                {
                    stockYn = "Y";
                }
                else
                {
                    stockYn = "N";
                }

                RptParmValue[0] = "R1";
                RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                RptParmValue[2] = SystemBase.Base.gstrLangCd;
                RptParmValue[3] = txtChilePlantCd.Text;
                RptParmValue[4] = txtItemCd.Text;
                RptParmValue[5] = stockYn;
                RptParmValue[6] = txtCustCd.Text;
                RptParmValue[7] = txtPoNo.Text;
                RptParmValue[8] = txtPoNoTo.Text;
                RptParmValue[9] = txtCustNm.Text;
                RptParmValue[10] = txtRemark.Text;

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", FormulaFieldValue, FormulaFieldName, RptName, RptParmValue); //공통크리스탈 10버전	
                frm.ShowDialog();
            }
            this.Cursor = Cursors.Default;
        }
        #endregion        	

    }
}
