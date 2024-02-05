#region 작성정보
/*********************************************************************/
// 단위업무명 : 매출채권집계출력
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-11
// 작성내용 : 매출채권집계출력
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
namespace SS.SSC016
{
    public partial class SSC016 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public SSC016()
        {
            InitializeComponent();

        }
        #endregion

        #region Form Load 시
        private void SSC016_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //기타 세팅
            dtpBnDtFr.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0,7) + "-01";
            dtpBnDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");                     
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpBnDtFr.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7) + "-01";
            dtpBnDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");     
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                try
                {
                    string type = "";

                    if (rdoCust.Checked == true)
                        type = "S1";
                    else if (rdoItem.Checked == true)
                        type = "S2";
                    else
                        type = "S3";

                    string strQuery = " usp_SSC016  '" + type + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";

                    if (rdoCfmY.Checked == true)
                        strQuery += ", @pBN_CONFIRM_YN = 'Y'";
                    else if (rdoCfmN.Checked == true)
                        strQuery += ", @pBN_CONFIRM_YN = 'N'";

                    strQuery += ", @pBN_DT_FR  = '" + dtpBnDtFr.Text + "'";
                    strQuery += ", @pBN_DT_TO  = '" + dtpBnDtTo.Text + "'";
                    strQuery += ", @pSALE_DUTY = '" + txtSaleDuty.Text.Trim() + "'";
                    strQuery += ", @pITEM_CD  = '" + txtItemCd.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

                    if (fpSpread1.Sheets[0].RowCount > 0) Set_Color();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void Set_Color()
        {

            if (rdoCust.Checked == true)
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Text = "주문처";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Text = "주문처명";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Text = "품목";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Text = "품목명";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Text = "영업담당자";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].Text = "영업담당자명";
            }
            else if (rdoItem.Checked == true)
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Text = "품목";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Text = "품목명";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Text = "주문처";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Text = "주문처명";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Text = "영업담당자";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].Text = "영업담당자명";
            }
            else
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Text = "영업담당자";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Text = "영업담당자명";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Text = "영업조직";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Text = "영업조직명";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Text = "품목";
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].Text = "품목명";
            }

            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, 5].Text == "zzzzzzzz") //소계
                {
                    for (int j = 0; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor2;
                    }
                }
                else if (fpSpread1.Sheets[0].Cells[i, 2].Text == "zzzz") //합계
                {
                    for (int k = 0; k < fpSpread1.Sheets[0].ColumnCount; k++)
                    {
                        fpSpread1.Sheets[0].Cells[i, k].BackColor = SystemBase.Base.gColor1;
                    }
                }
            }
        }
        #endregion

        #region 조회조건 팝업       
        //영업담당자
        private void btnSaleDuty_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_S_COMMON 'S011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSaleDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "영업담당자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSaleDuty.Text = Msgs[0].ToString();
                    txtSaleDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }	
        }

        //품목
        private void btnItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW001 pu = new WNDW001(txtItemCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[1].ToString();
                    txtItemNm.Value = Msgs[2].ToString();
                    txtItemCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }      
        #endregion

        #region 조회조건 TextChanged   
        //주문처
        private void txtSaleDuty_TextChanged(object sender, EventArgs e)
        {
            txtSaleDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSaleDuty.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //품목
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion  

        #region Form Activated & Deactivated
        private void SSC016_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpBnDtFr.Focus();
        }

        private void SSC016_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

    }
}
