using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SystemBase
{
	public class InnoMessageBox
	{
		public InnoMessageBox()
		{
			//
			// TODO: 여기에 생성자 논리를 추가합니다.
			//
		}

		public const int C_OK_INDEX				= 0;
		public const int C_CANCEL_INDEX			= 1;
		public const int C_YES_INDEX			= 2;
		public const int C_NO_INDEX				= 3;
		public const int C_RETRY_INDEX			= 4;

		public const int C_FORM_MIN_WIDTH		= 400;
		public const int C_FROM_LEFT_SPACE		= 10;
		public const int C_FROM_RIGHT_SPACE		= 10;
		public const int C_FROM_BOTTOM_SPACE	= 40;
		public const int C_ICON_WIDTH			= 30;
		public const int C_ICON_HEIGHT			= 30;
		public const int C_OBJECT_LEFT_SPACE	= 10;
		public const int C_OBJECT_TOP_SPACE		= 20;
		public const int C_OBJECT_BOTTOM_SPACE	= 10;
		public const int C_LABEL_MAX_WIDTH		= 400;
		public const int C_LABEL_MAX_HEIGHT		= 150;


		private static Button btnOk;
		private static Button btnCancel;
		private static Button btnYes;
		private static Button btnNo;
		private static Button btnRetry;
		private static Form frmMsgBox;

		private static DialogResult gdrRtn;

		public static DialogResult Show( string btntext, string text,	string caption,	MessageBoxButtons buttons, MessageBoxIcon icon	)
		{
			try
			{

				string [] strButtonText = new string[4];
				
				if(btntext == null || btntext.Trim() == "")
				{
					btntext = " | | | | ";
				}

				strButtonText = btntext.Split('|');

				frmMsgBox = new Form();

				frmMsgBox.HelpButton = false;
				frmMsgBox.FormBorderStyle = FormBorderStyle.FixedDialog;;
				frmMsgBox.MaximizeBox = false;
				frmMsgBox.MinimizeBox = false;
				frmMsgBox.StartPosition = FormStartPosition.CenterScreen;
				frmMsgBox.Text = caption;

				btnOk		= new Button ();
				btnCancel	= new Button ();
				btnYes		= new Button ();
				btnNo		= new Button ();
				btnRetry	= new Button ();

				System.Windows.Forms.Label lblText			= new System.Windows.Forms.Label ();
				System.Windows.Forms.PictureBox picIcon		= new System.Windows.Forms.PictureBox ();
				
				btnOk.Click			+= new System.EventHandler(btnOk_Click);
				btnCancel.Click		+= new System.EventHandler(btnCancel_Click);
				btnYes.Click		+= new System.EventHandler(btnYes_Click);
				btnNo.Click			+= new System.EventHandler(btnNo_Click);
				btnRetry.Click		+= new System.EventHandler(btnRetry_Click);

				frmMsgBox.Closed	+= new System.EventHandler(frmMsgBox_Close);
				
				string strCurPath = System.Environment.CurrentDirectory + @"\image\";


				btnOk.Text			= strButtonText[C_OK_INDEX];
				btnCancel.Text		= strButtonText[C_CANCEL_INDEX];
				btnYes.Text			= strButtonText[C_YES_INDEX];
				btnNo.Text			= strButtonText[C_NO_INDEX];
				btnRetry.Text		= strButtonText[C_RETRY_INDEX];

				try
				{
					switch (icon.ToString())
					{
						case "Asterisk":
							picIcon.Image		= Image.FromFile ( strCurPath + @"Asterisk.ico");
							break;
						case "Error":
							picIcon.Image		= Image.FromFile ( strCurPath + @"Error.ico");
							break;
						case "Exclamation":
							picIcon.Image		= Image.FromFile (strCurPath + @"Exclamation.ico");
							break;
						case "Hand":
							picIcon.Image		= Image.FromFile (strCurPath + @"Hand.ico");
							break;
						case "Information":
							picIcon.Image		= Image.FromFile (strCurPath + @"Information.ico");
							break;
						case "None":
							picIcon.Image		= Image.FromFile (strCurPath + @"None.ico");
							break;
						case "Question":
							picIcon.Image		= Image.FromFile (strCurPath + @"Question.ico");
							break;
						case "Stop":
							picIcon.Image		= Image.FromFile (strCurPath + @"Stop.ico");
							break;
						case "Warning":
							picIcon.Image		= Image.FromFile (strCurPath + @"Warning.ico");
							break;
						default :
							picIcon.Image		= Image.FromFile (strCurPath + @"Exclamation.ico");
							break;
					}
				}
				catch
				{
				}

				picIcon.SizeMode		= PictureBoxSizeMode.StretchImage;
				picIcon.Location		= new Point (C_OBJECT_LEFT_SPACE, C_OBJECT_TOP_SPACE);
				picIcon.Width			= C_ICON_WIDTH;
				picIcon.Height			= C_ICON_HEIGHT;

				frmMsgBox.Controls.Add(picIcon);
				
				lblText.Text			= text;
				lblText.Location		= new Point (picIcon.Right + C_FROM_LEFT_SPACE, picIcon.Top);
				lblText.AutoSize		= true;
				
				if(lblText.Width >= C_LABEL_MAX_WIDTH)
				{
					lblText.AutoSize	= false;
					lblText.Width		= C_LABEL_MAX_WIDTH;
					lblText.Height		= C_LABEL_MAX_HEIGHT;
				}
				
				frmMsgBox.Height = (lblText.Bottom < picIcon.Bottom ? picIcon.Bottom : lblText.Bottom ) + btnOk.Height + C_OBJECT_BOTTOM_SPACE + C_FROM_BOTTOM_SPACE;
				frmMsgBox.Width = (lblText.Right < C_FORM_MIN_WIDTH ? C_FORM_MIN_WIDTH : lblText.Right ) + C_FROM_RIGHT_SPACE; 
				
				switch (buttons)
				{
					case MessageBoxButtons.OK:
						btnOk.Location		= new Point ((frmMsgBox.Width/2)-(btnOk.Width/2), 
																(lblText.Bottom < picIcon.Bottom ? picIcon.Bottom : lblText.Bottom ) + C_OBJECT_BOTTOM_SPACE);

						frmMsgBox.AcceptButton = btnOk;
						frmMsgBox.Controls.Add(btnOk);

						frmMsgBox.Controls.Add(lblText);

						break;

					case MessageBoxButtons.OKCancel:
						btnOk.Location		= new Point ((frmMsgBox.Width/2)-(btnOk.Width)-(C_OBJECT_LEFT_SPACE/2), 
																(lblText.Bottom < picIcon.Bottom ? picIcon.Bottom : lblText.Bottom ) + C_OBJECT_BOTTOM_SPACE);
						btnCancel.Location	= new Point (btnOk.Left + btnOk.Width + C_OBJECT_LEFT_SPACE, btnOk.Top);

						frmMsgBox.AcceptButton = btnOk;
						frmMsgBox.Controls.Add(btnOk);
						frmMsgBox.CancelButton = btnCancel;
						frmMsgBox.Controls.Add(btnCancel);

						frmMsgBox.Controls.Add(lblText);
						
						break;

					case MessageBoxButtons.YesNo:
						btnYes.Location		= new Point ((frmMsgBox.Width/2)-(btnYes.Width)-(C_OBJECT_LEFT_SPACE/2), 
																(lblText.Bottom < picIcon.Bottom ? picIcon.Bottom : lblText.Bottom ) + C_OBJECT_BOTTOM_SPACE);
						btnNo.Location		= new Point (btnYes.Left + btnYes.Width + C_OBJECT_LEFT_SPACE, btnYes.Top);

						frmMsgBox.AcceptButton = btnYes;
						frmMsgBox.Controls.Add(btnYes);
						frmMsgBox.CancelButton = btnNo;
						frmMsgBox.Controls.Add(btnNo);

						frmMsgBox.Controls.Add(lblText);

						break;

					case MessageBoxButtons.RetryCancel:
						btnRetry.Location		= new Point ((frmMsgBox.Width/2)-(btnRetry.Width)-(C_OBJECT_LEFT_SPACE/2), 
																(lblText.Bottom < picIcon.Bottom ? picIcon.Bottom : lblText.Bottom ) + C_OBJECT_BOTTOM_SPACE);
						btnCancel.Location		= new Point (btnRetry.Left + btnRetry.Width + C_OBJECT_LEFT_SPACE, btnRetry.Top);

						frmMsgBox.AcceptButton = btnRetry;
						frmMsgBox.Controls.Add(btnRetry);
						frmMsgBox.CancelButton = btnCancel;
						frmMsgBox.Controls.Add(btnCancel);

						frmMsgBox.Controls.Add(lblText);

						break;

					case MessageBoxButtons.YesNoCancel:
						btnYes.Location		= new Point ((frmMsgBox.Width/2)-(btnYes.Width/2)-btnYes.Width-C_OBJECT_LEFT_SPACE, 
																(lblText.Bottom < picIcon.Bottom ? picIcon.Bottom : lblText.Bottom ) + C_OBJECT_BOTTOM_SPACE);
						btnNo.Location		= new Point (btnYes.Left + btnYes.Width + C_OBJECT_LEFT_SPACE, btnYes.Top);
						
						btnCancel.Location		= new Point (btnNo.Left + btnNo.Width + C_OBJECT_LEFT_SPACE, btnNo.Top);

						frmMsgBox.AcceptButton = btnYes;
						frmMsgBox.Controls.Add(btnYes);
						frmMsgBox.CancelButton = btnNo;
						frmMsgBox.Controls.Add(btnNo);
						frmMsgBox.CancelButton = btnCancel;
						frmMsgBox.Controls.Add(btnCancel);

						frmMsgBox.Controls.Add(lblText);

						break;
				}


				frmMsgBox.ShowDialog();

				return gdrRtn;
			}
			catch
			{
				if (frmMsgBox != null)
				{
					frmMsgBox.Close();
				}

				return gdrRtn;
			}
		}

		private static void btnOk_Click(object sender, System.EventArgs e)
		{
			btnOk.DialogResult = DialogResult.OK;
			gdrRtn = DialogResult.OK;
			frmMsgBox.Close();
		}
		private static void btnCancel_Click(object sender, System.EventArgs e)
		{
			btnOk.DialogResult = DialogResult.Cancel;
			gdrRtn = DialogResult.Cancel;
			frmMsgBox.Close();
		}
		private static void btnYes_Click(object sender, System.EventArgs e)
		{
			btnOk.DialogResult = DialogResult.Yes;
			gdrRtn = DialogResult.Yes;
			frmMsgBox.Close();
		}
		private static void btnNo_Click(object sender, System.EventArgs e)
		{
			btnOk.DialogResult = DialogResult.No;
			gdrRtn = DialogResult.No;
			frmMsgBox.Close();
		}
		private static void btnRetry_Click(object sender, System.EventArgs e)
		{
			btnOk.DialogResult = DialogResult.Retry;
			gdrRtn = DialogResult.Retry;
			frmMsgBox.Close();
		}
		private static void frmMsgBox_Close(object sender, System.EventArgs e)
		{
			//Message Box Close시 처리할 코드를 추가하면 됨....
		}
	}

}
