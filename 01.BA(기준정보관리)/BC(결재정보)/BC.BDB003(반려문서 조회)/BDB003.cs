#region 작성정보
/*********************************************************************/
// 단위업무명 : 반려문서 조회
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-24
// 작성내용 : 반려문서 조회
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
using System.Data.SqlClient;
using WNDW;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using System.Reflection;

namespace BC.BDB003
{
    public partial class BDB003 : UIForm.FPCOMM2
    {
        #region 생성자
        public BDB003()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BDB003_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용     

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboGbn, "usp_BDB003 @pType='C1' ,@pLANG_CD ='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
            SystemBase.ComboMake.C1Combo(cboStatus, "usp_BDB003 @pType='C2' ,@pLANG_CD ='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);

            G1Etc[6] = SystemBase.ComboMake.ComboOnGrid("SELECT MINOR_CD, CD_NM FROM B_COMM_CODE WHERE	LANG_CD	= 'KOR' AND MAJOR_CD = 'BZ27' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");

            dtpFrDt.Text = DateTime.Now.AddDays(-7).ToShortDateString();
            dtpToDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            cboGbn.SelectedValue = "N";
        }
        #endregion
        
        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            dtpFrDt.Text = DateTime.Now.AddDays(-7).ToShortDateString();
            dtpToDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            cboGbn.SelectedValue = "N";
        }
        #endregion

        #region PrintExec()
        protected override void PrintExec()
        {
            if (fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, 12].Text == "MOB001")
            {

            }
            else if (fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, 12].Text == "MRB001")
            {

            }
            else if (fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, 12].Text == "PRA002_SH")
            {

            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_BDB003  'S1'";
                    strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery = strQuery + ", @pYMFR ='" + dtpFrDt.Text + "' ";
                    strQuery = strQuery + ", @pYMTO ='" + dtpToDt.Text + "' ";

                    if (cboGbn.SelectedValue.ToString() != "") strQuery = strQuery + ", @pGBN ='" + cboGbn.SelectedValue.ToString() + "' ";
                    if (cboStatus.SelectedValue.ToString() != "") strQuery = strQuery + ", @pSTATUS ='" + cboStatus.SelectedValue.ToString() + "'";

                    strQuery = strQuery + ", @pDOCUNM ='" + txtDocuNm.Text + "' ";
                    strQuery = strQuery + ", @pDOCUNO ='" + txtDocuNo.Text + "' ";
                    strQuery = strQuery + ", @pWRITERNM ='" + txtWriter.Text + "' ";
                    strQuery = strQuery + ", @pUP_ID ='" + SystemBase.Base.gstrUserID + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        string strCode = fpSpread2.Sheets[0].Cells[0, 11].Text.ToString();
                        Right_Search(strCode);
                    }
                    else
                        fpSpread1.Sheets[0].RowCount = 0;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 좌측그리드 방향키 이동시 우측조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                string strCode = fpSpread2.Sheets[0].Cells[intRow, 11].Text.ToString();
                Right_Search(strCode);
            }
        }
        #endregion

        #region 하위 그리드 조회
        private void Right_Search(string strNo)
        {
            if (strNo.ToString() != "")
            {
                string strSql = " usp_BDB003  'S2'";
                strSql = strSql + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                strSql = strSql + ", @pDOCUNO = '" + strNo + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, 5].Text == "반려")
                        {
                            for (int j = 0; j < fpSpread1.Sheets[0].ColumnCount; j++)
                            {
                                fpSpread1.Sheets[0].Cells[i, j].ForeColor = Color.Red;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region fpSpread2_CellDoubleClick
        private void fpSpread2_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            object[] param = new object[1];
            param[0] = fpSpread2.Sheets[0].Cells[e.Row, 1].Text;

            Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + fpSpread2.Sheets[0].Cells[e.Row, 12].Text + ".dll");
            Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(fpSpread2.Sheets[0].Cells[e.Row, 12].Text.ToString() + "." + fpSpread2.Sheets[0].Cells[e.Row, 12].Text.ToString()), param);

            myForm.Show();
        }
        #endregion

        #region fpSpread2_ButtonClicked
        private void fpSpread2_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == 10)
            {
                try
                {
                    UIForm.FileUpDown frm = new UIForm.FileUpDown(fpSpread2.Sheets[0].Cells[e.Row, 1].Text, "N#Y#N");
                    frm.ShowDialog();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.ToString());
                }
            }
        }
        #endregion
    }
}
