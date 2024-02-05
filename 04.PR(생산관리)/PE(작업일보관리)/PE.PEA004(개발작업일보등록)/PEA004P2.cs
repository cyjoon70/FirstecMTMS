#region 작성정보
/*********************************************************************/
// 단위업무명 : 개발작업일보등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-25
// 작성내용 : 개발작업일보등록 및 관리
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

namespace PE.PEA004
{
    public partial class PEA004P2 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        string WoNo = "";
        #endregion

        #region 생성자
        public PEA004P2(string wono)
        {
            WoNo = wono;

            InitializeComponent();
        }

        public PEA004P2()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void PEA004P2_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            this.Text = "제조오더번호 조회";
            txtWoNo.Text = WoNo;            
            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            Search(false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            Search(true);
        }

        private void Search(bool chk)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    string Query = " usp_PEA004 @pTYPE = 'S6'";
                    Query += ", @pWORKORDER_NO = '" + txtWoNo.Text + "' ";
                    Query += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    Query += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    Query += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, chk, 0, 0, true);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }
        #endregion

        #region 그리드 선택값 입력밑 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(int R)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                returnVal = new string[fpSpread1.Sheets[0].Columns.Count];
                for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                {
                    returnVal[i] = Convert.ToString(fpSpread1.Sheets[0].Cells[R, i].Value);
                }
            }
        }
        #endregion

        #region 그리드 더블클릭
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            RtnStr(e.Row);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion
	
    }
}
