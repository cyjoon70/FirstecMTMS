using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TD.TDA009
{
	/// <summary>
	/// 삭제파일정리 대화상자
	/// </summary>
	public partial class DeleteDialog : Form
	{
		/// <summary>
		/// 삭제파일정리 대화상자를 생성합니다.
		/// </summary>
		public DeleteDialog()
		{
			InitializeComponent();
		}

		#region 속성
		/// <summary>
		/// 정리대상으로 문서파일이 선택되었는지 여부입니다.
		/// </summary>
		public bool DocumentFileChecked
		{
			get { return chkDocFile.Checked; }
		}

		/// <summary>
		/// 정리대상으로 기술자료파일이 선택되었는지 여부입니다.
		/// </summary>
		public bool SourceFileChecked
		{
			get { return chkSourceFile.Checked; }
		}
		#endregion

		#region 공용함수
		/// <summary>
		///  대화상자를 승인할 수 있는 상태인지 확인합니다.
		/// </summary>
		/// <returns>가능 여부</returns>
		bool CheckSubmitable()
		{
			return (chkDocFile.Checked || chkSourceFile.Checked) && txtPassword.Text != "";
		}
		
		/// <summary>
		/// 대화상자를 승인합니다.
		/// </summary>
		void Submit()
		{
			// 비밀번호 발리데이션
			DataTable pwTable = SystemBase.DbOpen.NoTranDataTable("usp_USERLOGIN @pType='S1', @pUSR_ID='" + SystemBase.Base.gstrUserID + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
			if (pwTable.Rows.Count > 0 && SystemBase.Base.DeCode(pwTable.Rows[0][0].ToString()) == txtPassword.Text)
			{
				this.DialogResult = DialogResult.OK;
				Close();
			}
			else
			{
				MessageBox.Show("비밀번호가 일치하지 않습니다.", "삭제파일정리", MessageBoxButtons.OK, MessageBoxIcon.Information);
				txtPassword.Focus();
				txtPassword.SelectAll();
			}
		}
		#endregion

		#region 컨트롤 이벤트 핸들러
		/// <summary>
		/// 승인버튼 클릭
		/// </summary>
		private void btnOk_Click(object sender, EventArgs e)
		{
			Submit();
		}

		/// <summary>
		/// 입력 컨트롤이 변경될 때 마다 승인버튼 활성화 업데이트
		/// </summary>
		private void InputControls_ConditionChanged(object sender, EventArgs e)
		{
			btnOk.Enabled = CheckSubmitable();
		}

		private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r' && CheckSubmitable()) Submit();
		}
		#endregion
	}
}
