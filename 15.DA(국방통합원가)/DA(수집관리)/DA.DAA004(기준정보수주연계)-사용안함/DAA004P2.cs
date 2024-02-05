#region 작성정보
/*********************************************************************/
// 단위업무명 : MASTER 팝업
// 작 성 자 : 유재규
// 작 성 일 : 2013-05-16
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

namespace DA.DAA004
{
    public partial class DAA004P2 : UIForm.FPCOMM1
    {
        #region 리턴될 변수선언
        private DataTable Return_Dt = null;
        public DataTable DT
        {
            get { return Return_Dt; }
        }
        #endregion

        #region 생성자
        public DAA004P2()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void DAA004P2_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            this.Text = "MASTER KEY 팝업";
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //제출업체
            SystemBase.ComboMake.C1Combo(cboH_MNUF_CODE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);   //제출업체

            txtORDR_YEAR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("")).ToString().Substring(0, 4);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달업체")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D006', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "구매부서")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "제출용도")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D008', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            //Grid_Search(false);
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        { Grid_Search(); }
        #endregion

        #region 그리드 조회
        private void Grid_Search()
        {
            this.Cursor = Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string strQuery = " usp_DAA004 'P2'";
                    strQuery += ", @pMNUF_CODE = '" + cboH_MNUF_CODE.SelectedValue.ToString() + "' ";
                    strQuery += ", @pORDR_YEAR = '" + txtORDR_YEAR.Text + "' ";
                    strQuery += ", @pDCSN_NUMB = '" + txtDCSN_NUMB.Text + "' ";
                    strQuery += ", @pCALC_DEGR = '" + txtCAL_C_DEGR.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
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

        #region 그리드 더블클릭
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                RtnStr(e.Row);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 그리드 선택값 입력밑 전송
        public DataTable ReturnVal { get { return Return_Dt; } set { Return_Dt = value; } }

        public void RtnStr(int R)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    DataTable Temp_Dt = ((System.Data.DataTable)(fpSpread1.Sheets[0].DataSource)).Copy();
                    Return_Dt = Temp_Dt.Clone();

                    DataRow Tr = Return_Dt.NewRow();
                    DataRow Dr = Temp_Dt.Rows[R];
                    for (int i = 0; i < Temp_Dt.Columns.Count; i++)
                    {
                        Tr[i] = Dr[i];
                    }
                    Return_Dt.Rows.Add(Tr);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
            }
        }
        #endregion
        
    }
}