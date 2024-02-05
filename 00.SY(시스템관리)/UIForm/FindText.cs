using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UIForm
{
    public partial class FindText : Form
    {
        FarPoint.Win.Spread.FpSpread grid;
        int startRow = 0;
        int startColumn = 0;

        public FindText(FarPoint.Win.Spread.FpSpread spread)
        {
            grid = spread;

            InitializeComponent();
        }

        public FindText()
        {
            InitializeComponent();
        }

        #region 취소(닫기)
        private void btnFindClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 다음 찾기
        private void btnNextFind_Click(object sender, EventArgs e)
        {
            string findWord = txtFindText.Text;

            int foundRow = 0;
            int foundColumn = -1;

            if (findWord.Trim().Length > 0)
            {
                while (foundRow != -1)
                {
                    grid.Search(grid.ActiveSheetIndex, findWord.Trim(), false, false, false, false, true, false, false, startRow, startColumn, ref foundRow, ref foundColumn);
                    grid.ActiveSheet.SetActiveCell(foundRow, foundColumn);
                    grid.ShowRow(0, foundRow, FarPoint.Win.Spread.VerticalPosition.Nearest);     //자동스크롤
                    grid.ShowColumn(0, foundColumn, FarPoint.Win.Spread.HorizontalPosition.Nearest);

                    startRow = foundRow;
                    startColumn = foundColumn + 1;

                    return;
                }
            }
        }
        #endregion

        #region 텍스트 입력후 엔터키 시 찾기 버튼
        private void txtFindText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnNextFind_Click(null, null);

                btnNextFind.Focus();
            }
        }
        #endregion
    }
}
