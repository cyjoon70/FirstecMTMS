#region 작성정보
/*********************************************************************/
// 단위업무명 : 경비상세조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-15
// 작성내용 : 경비상세조회 및 관리
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

namespace MX.MEX103
{  
    public partial class MEX103P1 : UIForm.FPCOMM1
    {
        string strExpNo = "";

        public MEX103P1(string ExpNo)
        {
            strExpNo = ExpNo;
            InitializeComponent();           
        }

        #region Form Load 시
        private void MEX103P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "프로젝트번호지정팝업";
            //GroupBo x1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);
             
            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            Grid_Search(false);		
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        { Grid_Search(true); }
        #endregion

        #region 그리드조회
        private void Grid_Search(bool Msg)
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string strQuery = "usp_MEX103 @pTYPE = 'P1'";
                    strQuery += ", @pEXP_NO = '" + strExpNo + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        decimal dTotAmt = 0;
                        decimal dTotAmtLoc = 0;

                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            dTotAmt += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "경비금액")].Value);
                            dTotAmtLoc += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "경비자국금액")].Value);
                        }

                        txtTotAmt.Value = dTotAmt;
                        txtTotAmtLoc.Value = dTotAmtLoc;
                    }
                    else
                    {
                        txtTotAmt.Value = 0;
                        txtTotAmtLoc.Value = 0;
                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }

        }
        #endregion

    }
}
