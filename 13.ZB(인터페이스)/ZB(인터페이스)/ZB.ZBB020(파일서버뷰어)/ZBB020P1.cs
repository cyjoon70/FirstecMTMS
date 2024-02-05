#region 작성정보
/*********************************************************************/
// 단위업무명 : 파일접근이력 조회
// 작 성 자 : 유재규
// 작 성 일 : 2013-05-23
// 작성내용 : 
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ZB.ZBB020
{
    public partial class ZBB020P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        #endregion

        #region 생성자
        public ZBB020P1()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void ZBB020P1_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            this.Text = "파일접근이력조회";

            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            
            //기타 세팅
            dtpFile_Access_Dt_Fr.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpFile_Access_Dt_To.Value = SystemBase.Base.ServerTime("YYMMDD");
            //Grid_Search(false);
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        { Grid_Search(true); }
        #endregion

        #region 그리드 조회
        private void Grid_Search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string strQuery = " usp_ZBB020 'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pFILE_ACCESS_DT_FROM = '" + dtpFile_Access_Dt_Fr.Text + "' ";
                    strQuery += ", @pFILE_ACCESS_DT_TO = '" + dtpFile_Access_Dt_To.Text + "' ";
                    strQuery += ", @pUSER_ID = '" + txtUser_Id.Text + "' ";
                    strQuery += ", @pFILE_INFO = '" + txtFile_Info.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion
    }
}