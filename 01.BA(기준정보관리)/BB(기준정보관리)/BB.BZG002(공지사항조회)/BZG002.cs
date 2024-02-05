#region 작성정보
/*********************************************************************/
// 단위업무명 : 공지사항조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-05
// 작성내용 : 공지사항조회 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;

namespace BB.BZG002
{
    public partial class BZG002 : UIForm.FPCOMM2
    {
        #region 생성자
        public BZG002()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BZG002_Load(object sender, System.EventArgs e)
        {
            try
            {
                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
                UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

                SearchExec();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 여는"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {

            string strQuery = " USP_BZG002  'S1'";
            strQuery = strQuery + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
            strQuery = strQuery + ", @pTITLE ='" + txtbox1.Text + "' ";
            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

        }
        #endregion

        #region fpSpread2_SelectionChanged
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.ActiveSheet.GetSelection(0) != null)
            {
                G1Search(fpSpread2.ActiveSheet.GetSelection(0).Row);

            }
        }

        public void G1Search(int G1Row)
        {
            int intCode = 0;
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                if(fpSpread2.Sheets[0].Cells[G1Row, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Text != "")
                    intCode = int.Parse(fpSpread2.Sheets[0].Cells[G1Row, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Text);
                //서브스프레드 상위 텍스트 입력
                string strSql = " USP_BZG002  'S2' ";
                strSql = strSql + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                strSql = strSql + ", @pIDX = " + intCode + "";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                ChangeColor();
            }
            else
            {

            }
        }

        public void ChangeColor()
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "읽기여부")].Text == "읽지않음")
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "읽기여부")].ForeColor = Color.Red;
                }

            }
        }
        #endregion
               
    }
}