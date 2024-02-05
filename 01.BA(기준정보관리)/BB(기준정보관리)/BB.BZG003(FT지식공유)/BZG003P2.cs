using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BB.BZG003
{
    public partial class BZG003P2 : UIForm.FPCOMM1
    {
        #region 변수선언
        string FILES_NO = "";
        int KeyNum = 0;
        #endregion

        #region 생성자
        public BZG003P2()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드
        private void BZG003P2_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            string Query = " usp_BAA004 'S3',@PFORM_ID='" + this.Name.ToString() + "', @PGRID_NAME='fpSpread1', @PIN_ID='" + SystemBase.Base.gstrUserID + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
            DataTable dt = SystemBase.DbOpen.TranDataTable(Query);
            int G1RowCount = dt.Rows.Count + 1;

            if (G1RowCount > 0)
            {
                G1Head1 = new string[G1RowCount];// 첫번째 Head Text
                G1Head2 = new string[G1RowCount];// 두번째 Head Text
                G1Head3 = new string[G1RowCount];// 세번째 Head Text
                G1Width = new int[G1RowCount];// Cell 넓이
                G1Align = new string[G1RowCount];// Cell 데이타 정렬방식
                G1Type = new string[G1RowCount];// CellType 지정
                G1Color = new int[G1RowCount];// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)

                G1SEQ = new int[G1RowCount];// 키

                //G1Etc		= new string[G1RowCount];
                G1HeadCnt = Convert.ToInt32(dt.Rows[0][0].ToString());

                /********************1번째 숨김필드 정의******************/
                G1Head1[0] = "";
                if (Convert.ToInt32(dt.Rows[0][0].ToString()) >= 1)
                    G1Head2[0] = "";
                if (Convert.ToInt32(dt.Rows[0][0].ToString()) == 3)
                    G1Head3[0] = "";
                G1Width[0] = 0;
                G1Align[0] = "";
                G1Type[0] = "";
                G1Color[0] = 0;
                G1Etc[0] = "";
                /********************1번째 숨김필드 정의******************/

                for (int i = 1; i < G1RowCount; i++)
                {
                    G1Head1[i] = dt.Rows[i - 1][1].ToString();
                    if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) >= 1)
                        G1Head2[i] = dt.Rows[i - 1][2].ToString();
                    if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) == 3)
                        G1Head3[i] = dt.Rows[i - 1][3].ToString();

                    G1Width[i] = Convert.ToInt32(dt.Rows[i - 1][4].ToString());
                    G1Align[i] = dt.Rows[i - 1][5].ToString();
                    G1Type[i] = dt.Rows[i - 1][6].ToString();
                    G1Color[i] = Convert.ToInt32(dt.Rows[i - 1][7].ToString());

                    if (G1Etc[i] == null)
                        G1Etc[i] = dt.Rows[i - 1][8].ToString();

                    G1SEQ[i] = Convert.ToInt32(dt.Rows[i - 1][9].ToString());

                }
            }

            string Query_plant = "USP_B_COMMON @PTYPE = 'PLANT'";//공장
            G1Etc[3] = SystemBase.ComboMake.ComboOnGrid(Query_plant);		// 공장


            string strQuery = " USP_BZG003  'S4'";
            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

        }
        #endregion
    }
}
