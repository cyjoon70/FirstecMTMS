using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace UIForm
{
    public partial class InsertRow : Form
    {
        System.Windows.Forms.TextBox txtInsertRowCnt;

        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1NumericEdit txtRow;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label1;
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        public InsertRow(TextBox txtRowCount)
        {  
            // Windows Form 디자이너 지원에 필요합니다.
            //

            txtInsertRowCnt = txtRowCount;

            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }

        public InsertRow()
        {
            InitializeComponent();
        }
        #region 취소
        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            txtInsertRowCnt.Text = "";
            this.Close();
        }
        #endregion

        #region 확인
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            txtInsertRowCnt.Text = txtRow.Text;
            this.Close();
        }
        #endregion

        #region 엔터 눌렀을시 확인 이벤트
        private void InsertRow_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() == "13")
            {
                btnOk_Click(null, null);
            }
        }

        private void txtRow_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOk_Click(null, null);
            }
        }
        #endregion
    }
}
