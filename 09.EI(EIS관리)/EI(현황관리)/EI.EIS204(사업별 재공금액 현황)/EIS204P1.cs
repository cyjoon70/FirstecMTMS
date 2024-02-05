using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EI.EIS204
{
    public partial class EIS204P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strEntCd = "";
        string strEntNm = "";
        #endregion

        public EIS204P1(string EntCd, string EntNm)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            strEntCd = EntCd;
            strEntNm = EntNm;

            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }

        public EIS204P1()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void EIS204P1_Load(object sender, System.EventArgs e)
        {
            txtEntCd.Value = strEntNm;

            SearchExec();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            Search(true);
        }

        private void Search(bool msg)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = "usp_EIS204 @pTYPE = 'S3'";
                strQuery += ", @pENT_CD = '" + strEntCd + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    double A_amt = 0, B_amt = 0, C_amt = 0, D_amt = 0;

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        A_amt = A_amt + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, 3].Value); //노무비
                        B_amt = B_amt + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, 4].Value); //재료비
                        C_amt = C_amt + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, 5].Value); //경비
                        D_amt = D_amt + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, 6].Value); //합계
                    }

                    fpSpread1.Sheets[0].Rows.Count = fpSpread1.Sheets[0].Rows.Count + 1;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, 1].Text = "합 계";
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, 3].Value = A_amt;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, 4].Value = B_amt;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, 5].Value = C_amt;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, 6].Value = D_amt;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 닫기버튼클릭
        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
