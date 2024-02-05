
#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목정보조회
// 작 성 자 : 김한진
// 작 성 일 : 2014-09-24
// 작성내용 : LOT 이력조회
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
namespace IN.INV130
{
    public partial class INV130 : UIForm.FPCOMM1
    {
        private string strMQuery;

        #region 생성자
        public INV130()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void INV130_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용

            //콤보박스 세팅

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            SystemBase.ComboMake.C1Combo(cboSPLANT_CD, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//공장
          

            //기타 세팅
            InDT_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            InDT_TO.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            InDT_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            InDT_TO.Text = SystemBase.Base.ServerTime("YYMMDD");
            cboSPLANT_CD.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                strMQuery = " usp_INV130 'S1'";
                strMQuery += ", @pPLANT_CD='" + cboSPLANT_CD.SelectedValue.ToString() + "'";
                strMQuery += ", @pITEM_CD='" + txtSITEM_CD.Text + "'";
                strMQuery += ", @pPROJECT_NO ='" + txtProject_No.Text + "'";
                strMQuery += ", @pINDT_FR ='" + InDT_FR.Text.ToString() + "'";
                strMQuery += ", @pINDT_TO ='" + InDT_TO.Text.ToString() + "'";
                strMQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
                strMQuery += ", @pLOT_NO ='" + txtLot_NO.Text.ToString() + "'";
                strMQuery += ", @pBARCODE_NO ='" + txtBARCODE_NO.Text.ToString() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion


        #region TextChanged
        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSITEM_CD.Text != "")
                {
                    txtSITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtSITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtSITEM_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProject_No.Text != "")
                {
                    txtProject_Name.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProject_Name.Value = "";
                }
            }
            catch { }
        }
        #endregion

        private void btnSITEM_CD_Click(object sender, EventArgs e)
        {
            try
            {
                //string strItemType = "03"; //제품
                WNDW.WNDW005 pu = new WNDW.WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSITEM_CD.Value = Msgs[2].ToString();		// 자품목코드
                    txtSITEM_NM.Value = Msgs[3].ToString();		// 자품목명

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnProject_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProject_No.Text, "S1", "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProject_No.Text = Msgs[3].ToString();
                    txtProject_Name.Value = Msgs[4].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
