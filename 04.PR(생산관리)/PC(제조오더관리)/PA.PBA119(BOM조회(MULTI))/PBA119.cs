#region 작성정보
/*********************************************************************/
// 단위업무명 : BOM조회(MULTI)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-15
// 작성내용 : BOM조회(MULTI) 및 관리
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
using WNDW;
using System.Threading;

namespace PA.PBA119
{
    public partial class PBA119 : UIForm.FPCOMM1
    {
        #region 변수선언
        UIForm.ExcelWaiting Waiting_Form = null;
        #endregion

        #region 생성자
        public PBA119()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void PBA119_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            // 그리드 콤보 셋팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "유무상구분")] = "F#T|무상#유상";
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "BOM TYPE")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            //콤보박스세팅
            SystemBase.ComboMake.C1Combo(cboBOM_NO, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// BOM TYPE
            SystemBase.ComboMake.C1Combo(cboITEM_UNIT, "usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //단위
            SystemBase.ComboMake.C1Combo(cboITEM_ACCT, "usp_B_COMMON @pType='COMM', @pCODE = 'B036', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //단위

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타세팅
            txtPLANT_CD.Value = SystemBase.Base.gstrPLANT_CD;
            dtpSTD_FROM_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
        }
        #endregion
        
        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타세팅
            txtPLANT_CD.Value = SystemBase.Base.gstrPLANT_CD;
            dtpSTD_FROM_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            txtVALID_FROM_DT.Value = "";
            txtVALID_TO_DT.Value = "";
        }
        #endregion

        #region 조회 조건 팝업
        //공장
        private void btnPLANT_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP' ,@pSPEC1 = 'PLANT_CD', @pSPEC2 = 'PLANT_NM', @pSPEC3 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPLANT_CD.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장코드 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPLANT_CD.Value = Msgs[0].ToString();
                    txtPLANT_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //품목
        private void btnITEM_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtPLANT_CD.Text, true, txtITEM_CD.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Value = Msgs[2].ToString();
                    txtITEM_NM.Value = Msgs[3].ToString();
                    txtITEM_CD.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }
        #endregion

        #region 조회조건 TextChanged
        //공장
        private void txtPLANT_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPLANT_CD.Text != "")
                {
                    txtPLANT_NM.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPLANT_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPLANT_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        //품목
        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtITEM_CD.Text != "")
                {
                    txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtITEM_NM.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string strGbn = "";

                    // BOM 정보 조회
                    if (rdoUnfoldGo.Checked)
                        strGbn = "S3";
                    else
                        strGbn = "S4";

                    // 품목 정보 조회
                    string strQuery = "usp_PBA119 @pTYPE = '" + strGbn + "'";

                    strQuery += ", @pPLANT_CD = '" + txtPLANT_CD.Text + "'";
                    strQuery += ", @pITEM_CD  = '" + txtITEM_CD.Text + "'";
                    strQuery += ", @pBOM_NO   = '" + cboBOM_NO.SelectedValue.ToString() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {

                        // 품목정보 셋팅
                        cboITEM_ACCT.SelectedValue = dt.Rows[0]["ITEM_ACCT"];
                        txtITEM_SEPC.Value = dt.Rows[0]["ITEM_SPEC"].ToString();
                        cboITEM_UNIT.SelectedValue = dt.Rows[0]["ITEM_UNIT"];

                        txtVALID_FROM_DT.Value = dt.Rows[0]["VALID_FROM_DT"].ToString();
                        txtVALID_TO_DT.Value = dt.Rows[0]["VALID_TO_DT"].ToString();

                        // BOM 정보 조회
                        if (rdoUnfoldGo.Checked)
                            strGbn = "S1";
                        else
                            strGbn = "S2";

                        strQuery = "usp_PBA119 @pTYPE = '" + strGbn + "'";

                        strQuery += ", @pPLANT_CD = '" + txtPLANT_CD.Text + "'";
                        strQuery += ", @pITEM_CD  = '" + txtITEM_CD.Text + "'";
                        strQuery += ", @pBOM_NO	  = '" + cboBOM_NO.SelectedValue.ToString() + "'";
                        strQuery += ", @pVALID_FROM_DT = '" + dtpSTD_FROM_DT.Text + "'";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                    }
                    else
                    {
                        MessageBox.Show("BOM 정보가 없습니다.");
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

        #region 엑셀
        protected override void ExcelExec()
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count <= 0)
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0053"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (rdoBom.Checked == true)
                {
                    UIForm.ExcelProges fm = new UIForm.ExcelProges(fpSpread1, "BOM");
                    fm.ShowDialog();
                }
                else if (rdoBomMajorRout.Checked == true) ExcelCreate("Y");
                else ExcelCreate("");

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "Excel 저장"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //주라우팅
        private void ExcelCreate(string major_yn)
        {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Excel 다운로드 위치 지정";
            dlg.InitialDirectory = dlg.FileName;
            dlg.Filter = "전체(*.*)|*.*|Excel Files(*.xls)|*.xls";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;
            if (major_yn == "Y")
                dlg.FileName = this.Text.ToString().Replace(@"/", "_") + "_주라우팅.xls";
            else
                dlg.FileName = this.Text.ToString().Replace(@"/", "_") + "_보조라우팅포함.xls";
            dlg.OverwritePrompt = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                CheckForIllegalCrossThreadCalls = false;

                Thread th = new Thread(new ThreadStart(Show_Waiting));
                th.Start();

                Thread.Sleep(2000);

                Waiting_Form.Activate();
                Waiting_Form.label_temp.Text = "엑셀 데이타 준비중입니다.";

                Excel.Application oAppln;
                Excel.Workbook oWorkBook;
                Excel.Worksheet oWorkSheet;
                Excel.Range oRange;

                try
                {
                    Waiting_Form.label_temp.Text = "엑셀 HEAD를 생성중입니다.";

                    oAppln = new Excel.Application();
                    oWorkBook = (Excel.Workbook)(oAppln.Workbooks.Add(true));
                    oWorkSheet = (Excel.Worksheet)oWorkBook.ActiveSheet;
	 
                    Waiting_Form.progressBar_temp.Maximum = fpSpread1.Sheets[0].Rows.Count;

                    // header 저장
                    int HeadColCnt;
                    for (HeadColCnt = 1; HeadColCnt < fpSpread1.Sheets[0].Columns.Count; HeadColCnt++)
                    {
                        oWorkSheet.Cells[1, HeadColCnt] = fpSpread1.Sheets[0].ColumnHeader.Cells[0, HeadColCnt].Text;
                    }

                    string Query = "SELECT CD_NM FROM B_COMM_CODE(NOLOCK) WHERE COMP_CODE = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' AND MAJOR_CD = 'T001' ORDER BY MINOR_CD ";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    int col2 = HeadColCnt;
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            oWorkSheet.Cells[1, HeadColCnt + i] = dt.Rows[i][0].ToString();
                        }
                    }

                    //내용 저장

                    int iRow = 2;
                    int colNo;
                    int col_idx = SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정");
                    int col_idx1 = SystemBase.Base.GridHeadIndex(GHIdx1, "자품목");

                    for (int rowNo = 0; rowNo < fpSpread1.Sheets[0].Rows.Count; rowNo++)
                    {

                        for (colNo = 0; colNo < fpSpread1.Sheets[0].Columns.Count - 1; colNo++)
                        {
                            oWorkSheet.Cells[iRow, colNo + 1] = fpSpread1.Sheets[0].Cells[rowNo, colNo + 1].Text;
                        }

                        if (fpSpread1.Sheets[0].Cells[rowNo, col_idx].Value.ToString() != "30")
                        {
                            string Query1 = " usp_PBA119 'S5'  ";
                            Query1 += " , @pPLANT_CD = '" + txtPLANT_CD.Text.Trim() + "'";
                            Query1 += " , @pITEM_CD  = '" + fpSpread1.Sheets[0].Cells[rowNo, col_idx1].Text + "'";
                            Query1 += " , @pMAJOR_FLG = '" + major_yn + "'";
                            Query1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(Query1);

                            if (dt1.Rows.Count > 0)
                            {
                                for (int j = 0; j < dt1.Rows.Count; j++)
                                {
                                    for (int k = 0; k < dt1.Columns.Count; k++)
                                    {
                                        oWorkSheet.Cells[iRow, col2 + k] = dt1.Rows[j][k].ToString();
                                    }

                                    string Query2 = " usp_PBA119 'S6'  ";
                                    Query2 += " , @pPLANT_CD = '" + txtPLANT_CD.Text.Trim() + "'";
                                    Query2 += " , @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[rowNo, col_idx1].Text + "'";
                                    Query2 += " , @pROUT_NO = '" + dt1.Rows[j]["ROUT_NO"].ToString() + "'";
                                    Query2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                    DataTable dt2 = SystemBase.DbOpen.NoTranDataTable(Query2);
                                    if (dt2.Rows.Count > 0)
                                    {
                                        for (int m = 0; m < dt2.Rows.Count; m++)
                                        {
                                            for (int n = 0; n < dt2.Columns.Count; n++)
                                            {
                                                oWorkSheet.Cells[iRow, col2 + 11 + n] = dt2.Rows[m][n].ToString();
                                            }
                                            iRow++;
                                        }
                                    }
                                    else
                                    {
                                        iRow++;
                                    }
                                }
                            }
                            else
                            {
                                iRow++;
                            }

                        }
                        else
                        {
                            iRow++;
                        }

                        Waiting_Form.progressBar_temp.Value = rowNo + 1;
                        Waiting_Form.label_temp.Text = "총" + fpSpread1.Sheets[0].Rows.Count.ToString() + " 개 중 " + (rowNo + 1).ToString() + " 개를 저장하였습니다.";
                    }


                    Waiting_Form.label_temp.Text = "엑셀 Sheet를 열고 있습니다.";
                    //range of the excel sheet
                    oRange = oWorkSheet.get_Range("A1", "IV1");
                    oRange.EntireColumn.AutoFit();
                    oAppln.UserControl = false;

                    oAppln.Visible = true;	// 저장후 저장된 내용 실행여부

                    // 엑셀 파일로 저장
                    oWorkBook.SaveAs(dlg.FileName, Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Excel.XlSaveAsAccessMode.xlNoChange, false, false, null, null, null);

                    Waiting_Form.label_temp.Text = "완료되었습니다.";
                }
                catch   //(Exception ex)
                {
                    th.Abort();
                    Waiting_Form.Close();
                }
                th.Abort();
                Waiting_Form.Close();
            }
        }

        private void Show_Waiting()
        {
            Waiting_Form = new UIForm.ExcelWaiting();
            Waiting_Form.ShowDialog();
        }
        #endregion

    }
}
