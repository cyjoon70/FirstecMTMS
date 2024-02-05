using System.Drawing;
using System.Windows.Forms;

namespace Bee
{
	public class ControlHelper
	{
		/// <summary>
		/// 지정한 위치에 있는 최상위 컨트롤을 찾습니다.
		/// </summary>
		/// <param name="container">폼 또는 컨트롤</param>
		/// <param name="pos">로컬 위치</param>
		/// <returns></returns>
		public static Control FindControlAtPoint(Control container, Point pos)
		{
			Control child;
			foreach (Control c in container.Controls)
			{
				if (c.Visible && c.Bounds.Contains(pos))
				{
					child = FindControlAtPoint(c, new Point(pos.X - c.Left, pos.Y - c.Top));
					if (child == null) return c;
					else return child;
				}
			}
			return null;
		}

		public static Control FindControlAtCursor(Form form)
		{
			Point pos = Cursor.Position;
			if (form.Bounds.Contains(pos))
				return FindControlAtPoint(form, form.PointToClient(Cursor.Position));
			return null;
		}
	}

}
