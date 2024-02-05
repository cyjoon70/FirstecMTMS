#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목별재공금액현황
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-24
// 작성내용 : 품목별재공금액현황 및 관리
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

namespace EI.EIS208
{
    public partial class EIS208 : UIForm.FPCOMM1
    {
        #region 생성자
        public EIS208()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void EIS208_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion
                
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {

                string strQuery = "usp_EIS208 @pTYPE = 'S1'";
                strQuery += ", @pENT_CD = '" + txtEntCd.Text + "'";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                strQuery += ", @pDATE = '" + dtpDate.Text + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    double A_amt = 0, B_amt = 0, C_amt = 0, D_amt = 0;

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        A_amt = A_amt + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "노무비")].Value); //노무비
                        B_amt = B_amt + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재료비")].Value); //재료비
                        C_amt = C_amt + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "경비")].Value); //경비
                        D_amt = D_amt + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합계")].Value); //합계
                    }

                    fpSpread1.Sheets[0].Rows.Count = fpSpread1.Sheets[0].Rows.Count + 1;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, 1].Text = "합 계";
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "노무비")].Value = A_amt;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "재료비")].Value = B_amt;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "경비")].Value = C_amt;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "합계")].Value = D_amt;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion
        
        #region 팝업
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEnt_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEntCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEntCd.Value = Msgs[0].ToString();
                    txtEntNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtItemCd.Value = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region textBox 체인지 이벤트
        private void txtEntCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEntCd.Text != "")
                {
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtEntNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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

    }
}
