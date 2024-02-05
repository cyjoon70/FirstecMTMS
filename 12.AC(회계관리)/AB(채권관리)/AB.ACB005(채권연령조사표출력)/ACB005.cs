

#region 작성정보
/*********************************************************************/
// 단위업무명 : 채권상세출력
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-07
// 작성내용 : 채권상세출력
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

namespace AB.ACB005
{
    public partial class ACB005 : UIForm.Buttons
    {
        public ACB005()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACB005_Load(object sender, System.EventArgs e)
        {
            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            SystemBase.ComboMake.C1Combo(cboBizAreaCdFrom, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboBizAreaCdTo, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장

            string YYMMDD = SystemBase.Base.ServerTime("YYMMDD");
            dtpStdDt.Value = YYMMDD;
            //dtpArDtFrom.Value = YYMMDD.Substring(0,4) + "-01-01";
            //dtpArDtTo.Value = YYMMDD;
            dtpArDtFrom.Value = "";
            dtpArDtTo.Value = "";
            txtTermDay.Text = "30";
        }
        #endregion

        #region PrintExec() PRINT 버튼 클릭 이벤트
        protected override void PrintExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string RptName = "";
                    if (optPrtinP1.Checked == true)
                    {
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\ACB005_1.rpt";    // 레포트경로+레포트명
                    }
                    else
                    {
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\ACB005_2.rpt";    // 레포트경로+레포트명
                    }
                    string[] RptParmValue = new string[10];   // SP 파라메타 값

                    if (optPrtinP1.Checked == true)
                    {
                        RptParmValue[0] = "P1";
                    }
                    else
                    {
                        RptParmValue[0] = "P2";
                    }

                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    if (dtpArDtFrom.Text == "")
                    {
                        RptParmValue[2] = "1900-01-01";
                    }
                    else
                    {
                        RptParmValue[2] = dtpArDtFrom.Text;
                    }
                    if (dtpArDtTo.Text == "")
                    {
                        RptParmValue[3] = "2999-12-31";
                    }
                    else
                    {
                        RptParmValue[3] = dtpArDtTo.Text;
                    }
                    RptParmValue[4] = dtpStdDt.Text;
                    RptParmValue[5] = txtCustCdFrom.Text;
                    RptParmValue[6] = txtCustCdTo.Text;
                    RptParmValue[7] = cboBizAreaCdFrom.SelectedValue.ToString();
                    RptParmValue[8] = cboBizAreaCdTo.SelectedValue.ToString();
                    RptParmValue[9] = txtTermDay.Text.Replace(",","");
                    
                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, null, RptName, RptParmValue); //공통크리스탈 10버전
                    //UIForm.PRINT10 frm = new UIForm.PRINT10( this.Text + "출력", null, RptName, RptParmValue);	//공통크리스탈 10버전
                    frm.ShowDialog();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 텍스트 체인지
        //거래처FROM
        private void txtCustCdFrom_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtCustNmFrom.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCdFrom.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //거래처TO
        private void txtCustCdTo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtCustNmTo.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 팝업 클릭
        //거래처FROM
        private void BtnCustFrom_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtCustCdFrom.Text, "PS");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCdFrom.Value = Msgs[1].ToString();
                    txtCustNmFrom.Value = Msgs[2].ToString();
                    txtCustCdFrom.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //거래처TO
        private void BtnCustTo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtCustCdTo.Text, "PS");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCdTo.Value = Msgs[1].ToString();
                    txtCustNmTo.Value = Msgs[2].ToString();
                    txtCustCdTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion
    }
}
