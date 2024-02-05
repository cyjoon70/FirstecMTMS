#region 작성정보
/*********************************************************************/
// 단위업무명 : 비밀번호등록
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-03-25
// 작성내용 : 비밀번호등록 및 관리
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

namespace BB.BBB006
{
    public partial class BBB006 : Form
    {
        #region 생성자
        public BBB006()
        {
            InitializeComponent();
        }
        #endregion

        #region 비밀번호 변경
        private void cmdChange_Click(object sender, EventArgs e)
        {
            string Query = "usp_UserLogin @pType='S1', @pUSR_ID='" + SystemBase.Base.gstrUserID + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            string DeCode = SystemBase.Base.DeCode(dt.Rows[0][0].ToString());

            if (DeCode == txtNowPW.Text)
            {
                if (txtChPW.Text == txtChPWCf.Text)
                {
                    string Query2 = " usp_UserLogin @pType='S2', @pUSR_ID='" + SystemBase.Base.gstrUserID + "', @pUSR_PW='" + SystemBase.Base.EnCode(txtChPW.Text) + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string Msg = SystemBase.DbOpen.TranNonQuery(Query2, "성공적으로 변경되었습니다.");
                    MessageBox.Show(Msg.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("비밀번호와 비밀번호 확인 내용이 다릅니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtChPWCf.Focus();
                }
            }
            else
            {
                MessageBox.Show("기존 비밀번호가 잘못 입력되었습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNowPW.Focus();
            }
        }
        #endregion

        #region 취소
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 기존비밀번호 입력 이벤트
        private void txtNowPW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                groupBox1.SelectNextControl(sender as Control, true, true, false, false);
            }
        }
        #endregion

        #region 새비밀번호 확인 입력 이벤트
        private void txtChPWCf_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cmdChange.Focus();
            }			
        }
        #endregion

    }
}
