#region TabGridHead 작성 정보
/*************************************************************/
// 단위업무명 : TabGridHead 배열 인덱스 정의
// 작 성 자 :   전 성 표
// 작 성 일 :   2013-01-17
// 작성내용 :   
// 수 정 일 :  
// 수 정 자 :   
// 수정내용 :   각 Tab별로 탭 그리드 헤더 인덱스 생성
// 비    고 : 
// 참    고 : 
/*************************************************************/
#endregion
using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FarPoint.Win;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.Model;

namespace UIForm
{
    public class TabGridHead
    {
        private string[] strTabHead1 = null;
        private string[] strTabHead2 = null;
        private string[] strTabHead3 = null;
        private int[] iTabWidth = null;
        private string[] strTabAlign = null;
        private string[] strTabType = null;
        private int[] iTabColor = null;
        private string[] strTabEtc = new string[100];
        private int[] iTabSEQ = null;
        private int iTabHeadCnt = 0;
        private string[,] strTabGHIdx = null;

        public string[] TabHead1
        {
            get { return strTabHead1; }
        }
        public  string[] TabHead2
        {
            get { return strTabHead2; }
        }
        public  string[] TabHead3
        {
            get { return strTabHead3; }
        }
        public  int[] TabWidth
        {
            get { return iTabWidth; }
        }
        public  string[] TabAlign
        {
            get { return strTabAlign; }
        }
        public  string[] TabType
        {
            get { return strTabType; }
        }
        public  int[] TabColor
        {
            get { return iTabColor; }
        }
        public  string[] TabEtc
        {
            get { return strTabEtc; }
        }
        public  int[] TabSEQ
        {
            get { return iTabSEQ; }
        }
        public  int TabHeadCnt
        {
            get { return iTabHeadCnt; }
        }
        public  string[,] TabGHIdx
        {
            get { return strTabGHIdx; }
        }

        public TabGridHead(string FormID, string GridNM)
        {
            try
            {
                string Query = " usp_BAA004 'S3',@PFORM_ID='" + FormID.ToString() + "', @PGRID_NAME='" + GridNM + "', @PIN_ID='" + SystemBase.Base.gstrUserID + "' ";
                DataTable dt = SystemBase.DbOpen.TranDataTable(Query);
                int G1RowCount = dt.Rows.Count + 1;

                if (G1RowCount > 0)
                {
                    strTabHead1 = new string[G1RowCount];// 첫번째 Head Text
                    strTabHead2 = new string[G1RowCount];// 두번째 Head Text
                    strTabHead3 = new string[G1RowCount];// 세번째 Head Text
                    iTabWidth = new int[G1RowCount];// Cell 넓이
                    strTabAlign = new string[G1RowCount];// Cell 데이타 정렬방식
                    strTabType = new string[G1RowCount];// CellType 지정
                    iTabColor = new int[G1RowCount];// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)

                    iTabSEQ = new int[G1RowCount];// 키
                    iTabHeadCnt = Convert.ToInt32(dt.Rows[0][0].ToString());


                    //####################1번째 숨김필드 정의######################
                    strTabHead1[0] = "";
                    if (Convert.ToInt32(dt.Rows[0][0].ToString()) >= 1)
                        strTabHead2[0] = "";
                    if (Convert.ToInt32(dt.Rows[0][0].ToString()) == 3)
                        strTabHead3[0] = "";
                    iTabWidth[0] = 0;
                    strTabAlign[0] = "";
                    strTabType[0] = "";
                    iTabColor[0] = 0;
                    strTabEtc[0] = "";

                    strTabGHIdx = new string[G1RowCount - 1, 2];	// 그리드 Head Index 변수 길이                    
                    int OldHeadNameCount = 1;

                    for (int i = 1; i < G1RowCount; i++)
                    {

                        strTabHead1[i] = dt.Rows[i - 1][1].ToString();
                        if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) >= 1)
                            strTabHead2[i] = dt.Rows[i - 1][2].ToString();
                        if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) == 3)
                            strTabHead3[i] = dt.Rows[i - 1][3].ToString();

                        iTabWidth[i] = Convert.ToInt32(dt.Rows[i - 1][4].ToString());
                        strTabAlign[i] = dt.Rows[i - 1][5].ToString();
                        strTabType[i] = dt.Rows[i - 1][6].ToString();
                        iTabColor[i] = Convert.ToInt32(dt.Rows[i - 1][7].ToString());

                        if (strTabEtc[i] == null)
                            strTabEtc[i] = dt.Rows[i - 1][8].ToString();

                        iTabSEQ[i] = Convert.ToInt32(dt.Rows[i - 1][9].ToString());


                        OldHeadNameCount = 1;
                        strTabGHIdx[0, 0] = dt.Rows[0][1].ToString().ToUpper();
                        for (int k = 0; k < i - 1; k++)
                        {
                            if (dt.Rows[i - 1][1].ToString().ToUpper() == strTabGHIdx[k, 0].ToUpper())
                            {
                                OldHeadNameCount++;
                            }
                            else if (strTabGHIdx[k, 0].ToUpper().LastIndexOf("_") > 0 && dt.Rows[i - 1][1].ToString().ToUpper() == strTabGHIdx[k, 0].ToUpper().Substring(0, strTabGHIdx[k, 0].ToUpper().LastIndexOf("_")))
                            {
                                OldHeadNameCount++;
                            }

                        }

                        if (OldHeadNameCount > 1)
                        {
                            strTabGHIdx[i - 1, 0] = dt.Rows[i - 1][1].ToString().ToUpper() + "_" + OldHeadNameCount.ToString();	// 그리드 Head명
                        }
                        else
                        {
                            strTabGHIdx[i - 1, 0] = dt.Rows[i - 1][1].ToString().ToUpper();	// 그리드 Head명
                        }
                        strTabGHIdx[i - 1, 1] = Convert.ToString(i);			// 그리드 Head 위치
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(FormID, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 헤더배열생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

    }
}
