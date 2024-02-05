using System;
using System.Windows.Forms;
using FarPoint.Win;
using FarPoint.Win.Spread;
using System.Data;
using System.Data.SqlClient;

namespace UIForm
{
    public class ExcelDown
    {
        #region 엑셀 컴포넌트 이용 엑셀 다운로드(FarPoint)
        public static void ExcelDownLoad(FarPoint.Win.Spread.FpSpread baseGrid, string formName)
        {	// 상단 툴바 사용
            ExcelProges fm = new ExcelProges(baseGrid, formName);
            fm.ShowDialog();
        }
        #endregion

        #region 엑셀 컴포넌트 이용 엑셀 다운로드(DataTable)
        public static void ExcelDownLoad(string FormID, string FormName, string GridName, DataTable dt)
        {	// UIForm.ExcelDown.ExcelDownLoad("BZM001", "fpSpread1", dt, @"d:\AAA.xls");

            ExcelProges fm = new ExcelProges(FormID, FormName, GridName, dt);
            fm.ShowDialog();

        }
        #endregion

        #region 엑셀 컴포넌트 이용 엑셀 다운로드(DataTable)
        public static void ExcelDownLoad(string FormName, DataTable dt)
        {	// UIForm.ExcelDown.ExcelDownLoad("BZM001", dt);

            ExcelProges fm = new ExcelProges(FormName, dt);
            fm.ShowDialog();

        }
        #endregion

        #region 엑셀 컴포넌트 이용 엑셀 다운로드(FarPoint)
        public static void ExcelDownLoadS(FarPoint.Win.Spread.FpSpread baseGrid, string formName)
        {	// 상단 툴바 사용
            ExcelProges fm = new ExcelProges(baseGrid, formName);
            fm.Show();
        }
        #endregion

        #region 엑셀 컴포넌트 이용 엑셀 다운로드(DataTable)
        public static void ExcelDownLoadS(string FormID, string FormName, string GridName, DataTable dt)
        {	// UIForm.ExcelDown.ExcelDownLoad("BZM001", "fpSpread1", dt, @"d:\AAA.xls");
            ExcelProges fm = new ExcelProges(FormID, FormName, GridName, dt);
            fm.Show();
        }
        #endregion

        #region 엑셀 컴포넌트 이용 엑셀 다운로드(DataTable)
        public static void ExcelDownLoadS(string FormName, DataTable dt)
        {	// UIForm.ExcelDown.ExcelDownLoad("BZM001", "fpSpread1", dt, @"d:\AAA.xls");
            ExcelProges fm = new ExcelProges(FormName, dt);
            fm.Show();
        }
        #endregion

    }
}
