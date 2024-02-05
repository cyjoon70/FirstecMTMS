#region DAA004P1 작성 정보
/*************************************************************/
// 단위업무명 : 프로젝트별 품목, 국가재고번호, 부품번호 조회
// 작 성 자 :   유재규
// 작 성 일 :   2013-06-13
// 작성내용 :   
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 : 
// 참    고 : 
/*************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DA.DAA004
{
    public partial class DAA004P1 : UIForm.FPCOMM1
    {
        #region 리턴될 변수선언
        private DataTable Return_Dt = null;

        public DataTable DT
        {
            get { return Return_Dt; }
        }
        #endregion

        #region 변수선업
        string strPROJECT_NO = "";
        string strNIIN = "";
        string strITEM_CD = "";
        string strITEM_NM = "";
        string strITEM_SPEC = "";
        
        #endregion

        #region DAA004P1(KEY_GROUP)
        public DAA004P1()
        {
            InitializeComponent();
        }
        public DAA004P1(string PROJECT_NO, string NIIN, string ITEM_CD, string ITEM_NM, string ITEM_SPEC)
        {
            strPROJECT_NO = PROJECT_NO;
            strNIIN = NIIN;
            strITEM_CD = ITEM_CD;
            strITEM_NM = ITEM_NM;
            strITEM_SPEC = ITEM_SPEC;
            InitializeComponent();
        }
        #endregion

        #region DAA004P1_Load
        private void DAA004P1_Load(object sender, EventArgs e)
        {
            //panButton1.Height = 0;  // 버튼 사용은 안함..별도 버튼 처리를 위해 
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            this.Text = "수주참조";

            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtPROJECT_NO.Value = strPROJECT_NO;
            txtNIIN.Value = strNIIN;
            txtITEM_CD.Value = strITEM_CD;
            txtITEM_NM.Value = strITEM_NM;
            txtITEM_SPEC.Value = strITEM_SPEC;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수조회조건 체크
                {
                    string strSql = " usp_DAA004  ";
                    strSql += "  @pTYPE = 'P1' ";
                    //strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql += ", @pPROJECT_NO = '" + txtPROJECT_NO.Text + "' ";
                    strSql += ", @pNIIN = '" + txtNIIN.Text + "' ";
                    strSql += ", @pITEM_CD = '" + txtITEM_CD.Text + "' ";
                    strSql += ", @pITEM_NM = '" + txtITEM_NM.Text + "' ";
                    strSql += ", @pDCSN_NUMB = '" + txtITEM_SPEC.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                }
            }
            catch (Exception f)
            {
                this.Cursor = System.Windows.Forms.Cursors.Default;
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region picClose_Click 선택후 닫기
        private void picClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (strFormClosingMsg == true)
                {
                    int UpCount = 0;
                    for (int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
                    {
                        if (fpSpread1.Sheets[0].Cells[j, 1].Value.ToString() == "1")
                            UpCount++;
                    }

                    if (UpCount > 0)
                    {
                        DialogResult Rtn = MessageBox.Show(SystemBase.Base.MessageRtn("SY011"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (Rtn == DialogResult.OK)
                            this.Close();
                    }
                    else this.Close();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region fpSpread1_ButtonClicked
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                #region 선택
                //if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "선택"))
                //{
                //    fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
                //    fpSpread1.Sheets[0].RowHeader.Rows[e.Row].BackColor = SystemBase.Base.Color_Org;

                //    if (strFLAG == "SINGLE")   // 싱글모드일때는 한 라인만 선택되게처리
                //    {
                //        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                //        {
                //            if (e.Row != i)
                //            {
                //                fpSpread1.Sheets[0].Cells[i, 1].Value = 0;
                //            }
                //        }
                //    }
                //}
                #endregion
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Array_Process 변수처리
        private void Array_Process()
        {
            try
            {
                DataTable Temp_Dt = ((System.Data.DataTable)(fpSpread1.Sheets[0].DataSource)).Copy();
                Return_Dt = Temp_Dt.Clone();

                for (int iRow = 0; iRow < fpSpread1.Sheets[0].Rows.Count; iRow++)
                {
                    if (fpSpread1.Sheets[0].Cells[iRow, 1].Text == "True")
                    {
                        DataRow Tr = Return_Dt.NewRow();
                        DataRow Dr = Temp_Dt.Rows[iRow];
                        for (int i = 0; i < Temp_Dt.Columns.Count; i++)
                        {
                            Tr[i] = Dr[i];
                        }
                        Return_Dt.Rows.Add(Tr);
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 싱글모드일때 더블클릭처리
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                ////if (strFLAG == "SINGLE")   // 싱글모드일때는 더블클릭 처리.. 멀티모드일때도 처리 가능
                ////{
                //getPROJECT_NO = new string[1]; getITEM_CD = new string[1];
                //getITEM_NM = new string[1]; getNATION_STOCK_NO = new string[1];
                //getFLOOR_PLAN_NUMB = new string[1]; getMANAGER_PART_NO = new string[1];
                //getSTOCK_UM = new string[1]; getPROJECT_NAME = new string[1];

                //getPROJECT_NO[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text.ToString();
                //getITEM_CD[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "ERP품목")].Text.ToString();
                //getITEM_NM[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명세")].Text.ToString();
                //getNATION_STOCK_NO[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "국가재고번호")].Text.ToString();
                //getMANAGER_PART_NO[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부품관리번호")].Text.ToString();
                //getFLOOR_PLAN_NUMB[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호")].Text.ToString();
                //getSTOCK_UM[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text.ToString();
                //getPROJECT_NAME[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text.ToString();

                //getRETURN = 1;
                ////}
                //this.Close();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 확인버튼
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                Array_Process();
                this.DialogResult = DialogResult.OK;
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                }
                this.Close();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
