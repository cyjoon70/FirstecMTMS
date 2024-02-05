#region 작성정보
/*********************************************************************/
// 단위업무명 : 종합불량현황(공정별)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-22
// 작성내용 : 종합불량현황(공정별) 및 관리
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
using System.IO;
using FarPoint.Win.Spread.CellType;
using System.Threading;
using WNDW;

namespace QE.QRE011
{
    public partial class QRE011 : UIForm.FPCOMM1
    {
        #region 변수선언
        Thread th;
        UIForm.ExcelWaiting Waiting_Form = null;
        #endregion

        #region 생성자
        public QRE011()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void QRE011_Load(object sender, System.EventArgs e)
        { 
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장

            //그리드초기화
            Grd_Set();

            //기타 세팅
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpInspDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpInspDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
        }
        #endregion
        
        #region 그리드 디자인
        private void Grd_Set()
        {
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            try
            {
                string strQuery = " usp_QRE011  @pTYPE = 'S2'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    //그리드 헤드 디자인
                    fpSpread1.Sheets[0].AlternatingRows[0].BackColor = Color.FromArgb(230, 230, 230);
                    fpSpread1.Sheets[0].AlternatingRows[1].BackColor = Color.FromArgb(245, 245, 245);

                    fpSpread1.Sheets[0].Columns[1].CellType = new TextCellType();
                    FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                    textCellType1.Multiline = true;
                    textCellType1.WordWrap = true;
                    fpSpread1.Sheets[0].Columns.Get(1).CellType = textCellType1;

                    fpSpread1.Sheets[0].ColumnCount = 11 + dt.Rows.Count;
                    fpSpread1.Sheets[0].ColumnHeader.Columns.Count = fpSpread1.Sheets[0].ColumnCount;
                    FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                    FarPoint.Win.Spread.CellType.PercentCellType num1 = new FarPoint.Win.Spread.CellType.PercentCellType();
                    num.DecimalSeparator = ".";
                    num.DecimalPlaces = 2;
                    num.FixedPoint = true;
                    num.Separator = ",";
                    num.ShowSeparator = true;
                    num.MaximumValue = 99999999999999;
                    num.MinimumValue = -99999999999999;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + 10].Text = dt.Rows[i]["DEFECT_TYPE_NM"].ToString();

                        fpSpread1.Sheets[0].Columns[i + 10].CellType = num;
                        fpSpread1.Sheets[0].Columns[i + 10].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                        fpSpread1.Sheets[0].Columns[i + 10].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                        fpSpread1.Sheets[0].Columns[i + 10].Locked = true;
                        fpSpread1.Sheets[0].Columns[i + 10].Width = 60;

                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 팝업
        //품목코드
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCd.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Value = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //사업코드
        private void btnEntCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEntCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEntCd.Value = Msgs[0].ToString();
                    txtEntNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 조회조건 TextChanged
        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //사업코드
        private void txtEntCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEntCd.Text != "")
                {
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtEntNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpInspDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpInspDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {

                string strQuery = " usp_QRE011  @pTYPE = 'S1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                strQuery += ", @pENT_CD = '" + txtEntCd.Text + "'";
                strQuery += ", @pINSP_DT_FR = '" + dtpInspDtFr.Text + "'";
                strQuery += ", @pINSP_DT_TO = '" + dtpInspDtTo.Text + "'";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    string div = "";
                    int row_idx = 0;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (div != dt.Rows[i]["JOB_CD"].ToString())
                        {
                            fpSpread1.Sheets[0].RowCount = row_idx + 1;

                            fpSpread1.Sheets[0].Cells[row_idx, 1].Text = dt.Rows[i]["JOB_CD"].ToString();
                            fpSpread1.Sheets[0].Cells[row_idx, 2].Text = dt.Rows[i]["JOB_NM"].ToString();
                            //							fpSpread1.Sheets[0].Cells[row_idx,3].Text = dt.Rows[i]["PROJECT_NO"].ToString();
                            fpSpread1.Sheets[0].Cells[row_idx, 3].Value = dt.Rows[i]["INSP_QTY"];
                            fpSpread1.Sheets[0].Cells[row_idx, 4].Value = dt.Rows[i]["GOOD_QTY"];
                            fpSpread1.Sheets[0].Cells[row_idx, 5].Value = dt.Rows[i]["DEFECT_QTY"];
                            fpSpread1.Sheets[0].Cells[row_idx, 6].Value = dt.Rows[i]["INSP_RATE"];
                            fpSpread1.Sheets[0].Cells[row_idx, 7].Value = dt.Rows[i]["손폐비용"];
                            fpSpread1.Sheets[0].Cells[row_idx, 8].Value = dt.Rows[i]["손폐율"];

                            div = dt.Rows[i]["JOB_CD"].ToString();

                            row_idx++;
                        }

                        if (dt.Rows[i]["DEFECT_TYPE_NM"].ToString() != "")
                        {
                            for (int j = 10; j < fpSpread1.Sheets[0].ColumnCount; j++)
                            {
                                if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, j].Text == dt.Rows[i]["DEFECT_TYPE_NM"].ToString())
                                {
                                    fpSpread1.Sheets[0].Cells[row_idx - 1, j].Value = dt.Rows[i]["DEFECT_QTY2"];
                                    break;
                                }
                            }
                        }
                    }

                    fpSpread1.Sheets[0].Columns[9, 10].BackColor = fpSpread1.Sheets[0].Columns[11].BackColor;
                    fpSpread1.Sheets[0].Columns[7, 8].BackColor = fpSpread1.Sheets[0].Columns[9].BackColor;
                    fpSpread1.Sheets[0].Columns[5, 6].BackColor = fpSpread1.Sheets[0].Columns[7].BackColor;
                    fpSpread1.Sheets[0].Columns[3, 4].BackColor = fpSpread1.Sheets[0].Columns[5].BackColor;
                    fpSpread1.Sheets[0].Columns[1, 2].BackColor = fpSpread1.Sheets[0].Columns[3].BackColor;

                    double dInspQty = 0, dDefectQty = 0;

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량")].Text != "")
                        {
                            dInspQty += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량")].Value);
                        }

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Text != "")
                        {
                            dDefectQty += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value);
                        }
                    }

                    txtInspQty.Value = dInspQty;
                    txtDefectQty.Value = dDefectQty;
                    txtDefectRate.Value = (dDefectQty / dInspQty) * 100;

                }
                else
                {
                    fpSpread1.Sheets[0].Rows.Count = 0;
                    SystemBase.Validation.GroupBox_Reset(groupBox2);
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 종합불량현황 출력
        private void btnPrivew_Click(object sender, System.EventArgs e)
        {
            string strSheetPage1 = "종합불량현황";
            string strFileName = SystemBase.Base.ProgramWhere + @"\Report\종합불량현황.xls";

            try
            {
                th = new Thread(new ThreadStart(Show_Waiting));
                th.Start();
                Thread.Sleep(200);
                Waiting_Form.Activate();

                string strQuery = " usp_QRE011  @pTYPE = 'R1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                strQuery += ", @pENT_CD = '" + txtEntCd.Text + "'";
                strQuery += ", @pINSP_DT_FR = '" + dtpInspDtFr.Text + "'";
                strQuery += ", @pINSP_DT_TO = '" + dtpInspDtTo.Text + "'";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    Waiting_Form.progressBar_temp.Maximum = dt.Rows.Count;

                    UIForm.VkExcel excel = null;

                    if (File.Exists(strFileName))
                    {
                        File.SetAttributes(strFileName, System.IO.FileAttributes.ReadOnly);
                    }
                    else
                    {
                        // 엑셀 데이터를 생성할 수 없습니다. 원본 파일이 존재하지 않습니다.
                        MessageBox.Show("엑셀 데이터를 생성할 수 없습니다. 원본 파일이 존재하지 않습니다."); ;
                        return;
                    }

                    excel = new UIForm.VkExcel(false);

                    excel.OpenFile(strFileName);
                    // 현재 시트 선택
                    excel.FindExcelWorksheet(strSheetPage1);

                    //TITLE 
                    excel.SetCell(1, 1, "종합불량현황 ( 공정별 )");
                    excel.SetCell(3, 1, "( " + dtpInspDtFr.Text + " ~ " + dtpInspDtTo.Text + " )");

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        excel.SetCell(19, 2 + i, dt.Rows[i]["INSP_DT"].ToString());
                        excel.SetCell(20, 2 + i, dt.Rows[i]["INSP_QTY"].ToString());
                        excel.SetCell(21, 2 + i, dt.Rows[i]["DEFECT_QTY"].ToString());
                        excel.SetCell(22, 2 + i, dt.Rows[i]["INSP_RATE"].ToString());

                        Waiting_Form.progressBar_temp.Value = i + 1;
                    }

                    excel.SetCell(24, 1, "공정코드");
                    excel.SetCell(24, 3, "공정명");

                    string strQuery1 = " usp_QRE011  @pTYPE = 'R2'";
                    strQuery1 += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery1 += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery1 += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery1 += ", @pENT_CD = '" + txtEntCd.Text + "'";
                    strQuery1 += ", @pINSP_DT_FR = '" + dtpInspDtFr.Text + "'";
                    strQuery1 += ", @pINSP_DT_TO = '" + dtpInspDtTo.Text + "'";
                    strQuery1 += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(strQuery1);

                    if (dt1.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            excel.SetCell(25 + i, 1, dt1.Rows[i]["JOB_CD"].ToString());
                            excel.SetCell(25 + i, 3, dt1.Rows[i]["JOB_NM"].ToString());
                            excel.SetCell(25 + i, 7, dt1.Rows[i]["INSP_QTY"].ToString());
                            excel.SetCell(25 + i, 9, dt1.Rows[i]["GOOD_QTY"].ToString());
                            excel.SetCell(25 + i, 11, dt1.Rows[i]["DEFECT_QTY"].ToString());
                        }
                    }

                    string strQuery2 = " usp_QRE011  @pTYPE = 'R3'";
                    strQuery2 += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery2 += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery2 += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery2 += ", @pENT_CD = '" + txtEntCd.Text + "'";
                    strQuery2 += ", @pINSP_DT_FR = '" + dtpInspDtFr.Text + "'";
                    strQuery2 += ", @pINSP_DT_TO = '" + dtpInspDtTo.Text + "'";
                    strQuery2 += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strQuery2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt2 = SystemBase.DbOpen.NoTranDataTable(strQuery2);

                    if (dt2.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt2.Rows.Count; i++)
                        {
                            excel.SetCell(32 + i, 20, dt2.Rows[i]["DEFECT_TYPE_NM"].ToString());
                            excel.SetCell(32 + i, 21, dt2.Rows[i]["DEFECT_QTY"].ToString());
                        }
                    }

                    Waiting_Form.label_temp.Text = "완료되었습니다.";
                    Thread.Sleep(500);
                    excel.ShowExcel(true);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "종합불량현황출력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Waiting_Form.Close();
                th.Abort();
                File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
            }
        }

        private void Show_Waiting()
        {
            Waiting_Form = new UIForm.ExcelWaiting("종합불량현황출력...");
            Waiting_Form.ShowDialog();
        }
        #endregion		

    }
}
