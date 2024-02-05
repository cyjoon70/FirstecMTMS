#region 작성정보
/*********************************************************************/
// 단위업무명 : 인보이스별대금지급현황
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-15
// 작성내용 : 인보이스별대금지급현황 및 관리
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

namespace MV.MIV507
{
    public partial class MIV507 : UIForm.FPCOMM1
    {
        bool form_act_chk = false;

        public MIV507()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MIV507_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            //기타 세팅
            dtpLoadDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpLoadDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;
            //기타 세팅
            dtpLoadDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 10);
            dtpLoadDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);

        }
        #endregion

        #region SearchExec()  그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    if (CheckSearchFields())            // 2016.02.17. hma 추가: 검색조건 체크
                    {
                        string strQuery = " usp_MIV507 'S1'";
                        strQuery += ", @pLOADING_DT_FR ='" + dtpLoadDtFr.Text + "'";
                        strQuery += ", @pLOADING_DT_TO ='" + dtpLoadDtTo.Text + "'";

                        strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                        strQuery += ", @pPO_NO ='" + txtPoNo.Text.Trim() + "'";
                        strQuery += ", @pBL_NO ='" + txtBlNo.Text.Trim() + "'";
                        strQuery += ", @pINVOICE_NO ='" + txtINVOICE_NO.Text.Trim() + "'";
                        // 2016.02.17. hma 추가(Start): 지급일FROM,TO 검색조건 추가
                        strQuery += ", @pCLS_DT_FR ='" + dtpClsDtFr.Text + "'";
                        strQuery += ", @pCLS_DT_TO ='" + dtpClsDtTo.Text + "'";
                        // 2016.02.17. hma 추가(End)

                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0);
                        fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Form Activated & Deactivate
        private void MIV507_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpLoadDtFr.Focus();
        }

        private void MIV507_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        #region 버튼 클릭 이벤트
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProject_No.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProject_No.Text = Msgs[3].ToString();
                    txtProject_Nm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

        }
        //발주번호
        private void btnPoNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW018 frm1 = new WNDW018();
                frm1.ShowDialog();
                if (frm1.DialogResult == DialogResult.OK)
                {
                    string Msgs = frm1.ReturnVal[1];                 
                    txtPoNo.Text = Msgs;
                }
              
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //B/L번호
        private void btnBlNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW022 frm1 = new WNDW022();
                frm1.ShowDialog();
                if (frm1.DialogResult == DialogResult.OK)
                {
                    string Msgs = frm1.ReturnVal[1];
                    txtBlNo.Text = Msgs;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

      
        #endregion

        #region 텍스트박스 TextChanged
        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProject_No.Text != "")
                {
                    txtProject_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProject_Nm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 그리드 상 팝업
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            //전표조회
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호_2"))
            {
                try
                {
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호")].Text != "")
                    {
                        WNDW.WNDW026 pu = new WNDW.WNDW026(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호")].Text);
                        pu.ShowDialog();
                    }

                    fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전표번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        
        #region Method
        
        // 2016.02.17. hma 추가(Start)
        #region 지급일자 기간 입력 체크
        private bool CheckSearchFields()
        {
            bool iRtnValue = true;
            if ((dtpClsDtFr.Text != "" && dtpClsDtTo.Text == "") || (dtpClsDtFr.Text == "" && dtpClsDtTo.Text != ""))
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0069", "지급일자"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                iRtnValue = false;
            }
            return iRtnValue;
        }
        #endregion
        // 2016.02.17. hma 추가(End)

        #endregion

    }
}
