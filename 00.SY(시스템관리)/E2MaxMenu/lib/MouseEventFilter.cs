using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Bee
{

	/// <summary>
	/// 마우스 휠이 커서 위치의 컴포넌트에 전달되도록 하는 메시지 필터입니다.
	/// </summary>
	public class MouseEventFilter : IMessageFilter
	{
		[DllImport("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

		public const int WM_MOUSEWHEEL = 0x020A;
		public const int WM_MOUSEMOVE = 0x0200;
		public const int WM_XBUTTONDOWN = 0x020B;
		public const int WM_XBUTTONUP = 0x020C;
		public const int WM_XBUTTONDBLCLK = 0x020D;
		public const int MK_XBUTTON1 = 0x0020;
		public const int MK_XBUTTON2 = 0x0040;

		bool mEnabled = false;
		bool mWheelEnabled = false;
		bool mXButtonEnabled = false;
		Control mWheelReceiver = null;
		Control mXButtonReceiver = null;
		int mWheelDelta = 0;
		List<Control> mWheelDeltaReceivers = new List<Control>();


		#region Properties

		/// <summary>
		/// 메시지 필터링을 켜거나 끕니다.
		/// </summary>
		public bool Enabled
		{
			get { return mEnabled; }
			set
			{
				if (mEnabled != value)
				{
					if (value) Application.AddMessageFilter(this);
					else Application.RemoveMessageFilter(this);
					mEnabled = value;
				}
			}

		}

		/// <summary>
		/// 마우스 휠을 필터링을 켜거나 끕니다.
		/// </summary>
		public bool WheelEnabled
		{
			get { return mWheelEnabled; }
			set { mWheelEnabled = value; }
		}

		/// <summary>
		/// 마우스 X 버튼 필터링을 켜거나 끕니다.
		/// </summary>
		public bool XButtonEnabled
		{
			get { return mXButtonEnabled; }
			set { mXButtonEnabled = value; }
		}

		/// <summary>
		/// 마우스 휠 이벤트를 독점하여 받을 컨트롤을 지정합니다. 기본값은 null이며 커서위치의 컨트롤에 이벤트를 전달합니다.
		/// </summary>
		public Control WheelReceiver
		{
			get { return mWheelReceiver; }
			set { mWheelReceiver = value; }
		}

		/// <summary>
		/// 마우스 X 버튼 이벤트를 독점하여 받을 컨트롤을 지정합니다. 기본값은 null이며 커서위치의 컨트롤에 이벤트를 전달합니다.
		/// </summary>
		public Control XButtonReceiver
		{
			get { return mXButtonReceiver; }
			set { mXButtonReceiver = value; }
		}

		/// <summary>
		/// 마우스 휠의 델타값을 설정합니다. 기본 값은 0이며 원래 값(윈도우 설정)을 반환합니다.
		/// </summary>
		public int WheelDelta
		{
			get { return mWheelDelta; }
			set { mWheelDelta = value; }
		}

		/// <summary>
		/// 지정한 마우스 휠의 델타값을 적용할 컨트롤 목록입니다. WheelDelta값이 0인 경우 무시됩니다.
		/// </summary>
		public List<Control> WheelDeltaReceivers
		{
			get { return mWheelDeltaReceivers; }
		}

		#endregion

		/// <summary>
		/// 마우스 휠 메시지 필터링을 끕니다.
		/// </summary>
		public void Disable()
		{
			this.Enabled = false;
		}

		/// <summary>
		/// 마우스 휠 메시지 필터링을 켭니다.
		/// </summary>
		public void Enable()
		{
			this.Enabled = true;
		}

		public bool PreFilterMessage(ref Message m)
		{
			switch (m.Msg)
			{
				case WM_MOUSEWHEEL:
					if (mWheelEnabled)
					{
						if (mWheelReceiver != null)
						{
							if (mWheelDelta != 0 && (mWheelDeltaReceivers.Count == 0 || mWheelDeltaReceivers.Contains(mWheelReceiver)))
								m.WParam = new IntPtr(m.WParam.ToInt32() > 0 ? (mWheelDelta << 16) : (-mWheelDelta << 16));
							SendMessage(mWheelReceiver.Handle, m.Msg, m.WParam, m.LParam);
						}
						else
						{
							Control control = Bee.ControlHelper.FindControlAtCursor(Form.ActiveForm);
							if (control != null)
							{
								if (mWheelDelta != 0 && (mWheelDeltaReceivers.Count == 0 || mWheelDeltaReceivers.Contains(control)))
									m.WParam = new IntPtr(m.WParam.ToInt32() > 0 ? (mWheelDelta << 16) : (-mWheelDelta << 16));
								SendMessage(control.Handle, m.Msg, m.WParam, m.LParam);
							}
						}
						return true;
					}
					break;

				case WM_XBUTTONDOWN:
				case WM_XBUTTONUP:
				case WM_XBUTTONDBLCLK:
					if (mXButtonEnabled)
					{
						if (mXButtonReceiver != null)
						{
							SendMessage(mXButtonReceiver.Handle, m.Msg, m.WParam, m.LParam);
						}
						else
						{
							Control control = Bee.ControlHelper.FindControlAtCursor(Form.ActiveForm);
							if (control != null) SendMessage(control.Handle, m.Msg, m.WParam, m.LParam);
						}
						return true;
					}
					break;
			}

			return false;
		}
	}

}
