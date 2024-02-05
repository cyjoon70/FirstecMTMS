#region 작성정보
/*********************************************************************/
// 단위업무명 : 수주조회팦업
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-11
// 작성내용 : 수주현황조회
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

namespace SO.SOB004
{
    public partial class SOB004P2 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strSoNo = "";
        int iSoSeq = 0;
        #endregion

        #region 생성자
        public SOB004P2()
        {
            InitializeComponent();
        }

        public SOB004P2(string SoNo, int SoSeq)
        {
            strSoNo = SoNo;
            iSoSeq = SoSeq;

            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SOB004P2_Load(object sender, System.EventArgs e)
        {
            this.Text = "수주이력 상세조회";

            //버튼 재정의
            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = "usp_SOB004 @pTYPE = 'S2'";
                strQuery += ", @pSO_NO = '" + strSoNo + "'";
                strQuery += ", @pSO_SEQ = '" + iSoSeq + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
            
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        for (int j = 0; j < fpSpread1.Sheets[0].Columns.Count; j++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, j].Text.IndexOf("->") > 0)
                            {
                                fpSpread1.Sheets[0].Cells[i, j].ForeColor = Color.Red;
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

    }
}
