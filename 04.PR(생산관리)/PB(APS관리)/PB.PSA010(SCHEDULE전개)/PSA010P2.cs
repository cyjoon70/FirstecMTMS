#region 작성정보
/*********************************************************************/
// 단위업무명 : SCHEDULE 전개
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-28
// 작성내용 : SCHEDULE 전개 및 관리
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
using System.Threading;

namespace PB.PSA010
{
    public partial class PSA010P2 : Form
    {
        string MSGCode = "";
        string strSCH_ID = "";
        string txtREASONCD = "";
        string txtMEMO = "";

        Thread schTh;

        public PSA010P2(string SCH_ID, string txtReasonCd, string txtMemo)
        {
            InitializeComponent();
            strSCH_ID = SCH_ID;
            txtREASONCD = txtReasonCd;
            txtMEMO = txtMemo;
        }

        #region Form Load 시
        private void PSA010P2_Load(object sender, EventArgs e)
        {
            try
            {
                Combo();
                SystemBase.Validation.GroupBox_Setting(groupBox1);


            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0066"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }
        #endregion

        #region Combo  콤보박스 초기화
        private void Combo()
        {
            string strQuery = "";
            //확정 SCHDULE ID
            strQuery = " usp_PSA010P1 @pType   = 'C1' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

            SystemBase.ComboMake.C1Combo(cboSch, strQuery, 0);

            cboSch.Text = strSCH_ID;
        }
        #endregion

        #region 스케쥴 확정
        private void btnConf_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                CheckForIllegalCrossThreadCalls = false;

                if (MessageBox.Show(SystemBase.Base.MessageRtn("P0011", cboSch.Text), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    schTh = new Thread(new ThreadStart(SchConfStart));
                    schTh.Start();

                }
            }
            catch(Exception f)
            {
                MessageBox.Show(f.ToString());

                if (schTh != null)
                    if (schTh.Join(5000))
                        schTh.Abort();
            }

            this.Cursor = Cursors.Default;
        }

        public void SchConfStart()
        {

            this.Cursor = Cursors.WaitCursor;

            string ConQuery = QUERY.CONFIRM_CHECK();
            DataTable dtCon = SystemBase.DbOpen.NoTranDataTable(ConQuery);			// MPS 정보 저장

            if (dtCon.Rows[0][0].ToString() == "N")
            {

                try
                {
                    SCH_CONF schConf = new SCH_CONF();
                    MSGCode = schConf.CONF_SCH(progressBar1, cboSch.SelectedValue.ToString(),
                        txtREASONCD, txtMEMO);
                    progressBar1.Value = 100;
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log("PSA010.SCH_CONF : ", f.ToString());
                    MSGCode = SystemBase.Base.MessageRtn("P0001");
                }

                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("이미 확정하셨습니다.", SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }

            this.Cursor = Cursors.Default;


        }
        #endregion

        #region btnClose_Click  취소버튼
        private void btnClose_Click(object sender, System.EventArgs e)
        {
            if (schTh != null)
            {
                if (schTh.Join(5000)) // 5초를 기달린다.
                {
                    schTh.Abort();
                }
            }
            this.Close();
        }
        #endregion	
    }
}
