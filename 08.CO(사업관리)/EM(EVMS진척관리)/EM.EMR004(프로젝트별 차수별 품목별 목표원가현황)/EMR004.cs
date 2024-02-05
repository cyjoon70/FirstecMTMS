#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트별 차수별 품목별 목표원가현황
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-23
// 작성내용 : 프로젝트별 차수별 품목별 목표원가현황
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

namespace EM.EMR004
{
    public partial class EMR004 : UIForm.FPCOMM2
    {
        #region 변수선언
        bool form_act_chk = false;
        int TempRow = 10000;
        string strProjNo = "";
        string strItemCd = "";
        string strProjSeq = "";
        int[] iYear = new int[10];
        int[] iYear_col = new int[10];
        string[] sYearMon = new string[100];
        int iActive_Row;
        #endregion

        public EMR004()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void EMR004_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용                
        }
        #endregion
        
        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;
            fpSpread1.Sheets[0].ColumnCount = 5;
        }
        #endregion

        #region 조회조건 팝업
        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {

                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            //try
            //{
            //    WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1", "R");
            //    pu.ShowDialog();
            //    if (pu.DialogResult == DialogResult.OK)
            //    {
            //        string[] Msgs = pu.ReturnVal;

            //        txtProjectNo.Text = Msgs[3].ToString();
            //        txtProjectNm.Value = Msgs[4].ToString();
            //    }
            //}
            //catch (Exception f)
            //{
            //    SystemBase.Loggers.Log(this.Name, f.ToString());
            //    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }
        //프로젝트차수
        private void c1Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtProjSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //품목코드
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(SystemBase.Base.gstrPLANT_CD, true);
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
            }

        }
        #endregion

        #region 조회조건 TextChanged
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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

        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {

                    string strQuery = " usp_EMR004 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pPROJECT_NO ='" + txtProjectNo.Text.Trim() + "'";
                    strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ ='" + txtProjSeq.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                    fpSpread1.Sheets[0].RowCount = 0;
                    TempRow = 10000;

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
        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (TempRow != e.NewRow)
            {
                int Row = e.NewRow;
                string ProjNo = fpSpread2.Sheets[0].Cells[e.NewRow, 1].Text;
                string ProjSeq = fpSpread2.Sheets[0].Cells[e.NewRow, 3].Text;
                string ItemCd = fpSpread2.Sheets[0].Cells[e.NewRow, 4].Text;

                iActive_Row = Row;
                Detail_Search(Row, ProjNo, ProjSeq, ItemCd);
            }
        }       

        private void Detail_Search(int Row, string ProjNo, string ProjSeq, string ItemCd)
        {
            this.Cursor = Cursors.WaitCursor;
            strProjNo = ProjNo;
            strItemCd = ItemCd;
            strProjSeq = ProjSeq;
            try
            {
                string strQuery = " usp_EMR004 'S2'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pPROJECT_NO='" + strProjNo + "'";
                strQuery += ", @pITEM_CD ='" + strItemCd + "'";
                strQuery += ", @pPROJECT_SEQ ='" + strProjSeq + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                string st_ym = "";
                string ed_ym = "";

                if (dt.Rows.Count > 0)
                {
                    st_ym = dt.Rows[0][0].ToString();
                    ed_ym = dt.Rows[0][1].ToString();
                }

                if (st_ym != "" && ed_ym != "")
                {
                    fpSpread1.Sheets[0].ColumnCount = 5;
                    if (Set_Header(st_ym, ed_ym))
                        detail_SearchExec(strProjNo, strProjSeq, strItemCd);
                }

                TempRow = Row;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }

        private bool Set_Header(string strST_YM, string strED_YM)
        {
            int year_1 = Convert.ToInt16(strST_YM.Substring(0, 4));
            int year_2 = Convert.ToInt16(strED_YM.Substring(0, 4));
            int mon_1 = Convert.ToInt16(strST_YM.Substring(4, 2));
            int mon_2 = Convert.ToInt16(strED_YM.Substring(4, 2));
            int st_col = 0;
            int mon = 1;

            int col_cnt = 5;
            int fisrt = 0;
            int year_cnt = 0, mon_cnt = 0;

            FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();

            try
            {
                for (int year = year_1; year <= year_2; year++)
                {
                    col_cnt++;
                    st_col = col_cnt - 1;
                    fpSpread1.Sheets[0].ColumnCount = col_cnt;
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, st_col].Text = year.ToString();
                    iYear[year_cnt] = year;
                    iYear_col[year_cnt] = col_cnt - 1;
                    mon_cnt = 0;
                    for (mon = 1; mon <= 12; mon++)
                    {
                        if (year == year_1 && fisrt == 0)
                            mon = mon_1;
                        else if (year == year_2 && mon > mon_2)
                            break;
                        if (fisrt != 0) col_cnt++;

                        fpSpread1.Sheets[0].ColumnCount = col_cnt;
                        fpSpread1.Sheets[0].ColumnHeader.Cells[1, col_cnt - 1].Text = mon.ToString();
                        if (mon.ToString().Length == 1)
                            sYearMon[col_cnt - 1] = year.ToString() + "0" + mon.ToString();
                        else
                            sYearMon[col_cnt - 1] = year.ToString() + mon.ToString();


                        num.DecimalSeparator = ".";
                        num.DecimalPlaces = 0;
                        num.FixedPoint = true;
                        num.Separator = ",";
                        num.ShowSeparator = true;
                        num.MaximumValue = 99999999999999;
                        num.MinimumValue = -99999999999999;
                        fpSpread1.Sheets[0].Columns[col_cnt - 1].CellType = num;
                        fpSpread1.Sheets[0].Columns[col_cnt - 1].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                        fpSpread1.Sheets[0].Columns[col_cnt - 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                        fpSpread1.Sheets[0].Columns[col_cnt - 1].Locked = false;
                        fpSpread1.Sheets[0].Columns[col_cnt - 1].BackColor = Color.White;
                        fpSpread1.Sheets[0].Columns[col_cnt - 1].Width = 80;
                        mon_cnt++;
                        fisrt = 1;
                    }
                    fisrt = 0;
                    year_cnt++;
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, st_col].ColumnSpan = mon_cnt;

                }
                return true;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
                return false;
            }
        }

        private void detail_SearchExec(string ProjNo, string ProjSeq, string ItemCd)
        {

            try
            {
                string strQuery = " usp_EMR004 'S3'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pPROJECT_NO='" + ProjNo + "'";
                strQuery += ", @pITEM_CD ='" + ItemCd + "'";
                strQuery += ", @pPROJECT_SEQ ='" + ProjSeq + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                fpSpread1.Sheets[0].FrozenColumnCount = 5;
                fpSpread1.Sheets[0].ColumnHeader.Rows[1].Height = 28;

                string div = "", div1 = "";
                string tempDiv = "", tempDiv1 = "";
                int row_idx = 0, cnt = 0;
                int col_idx = 0;
                decimal price = 0;
                int rowspan = 1;
                int rowfirst = 0;
                int iYear_value;
                int iMon;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    div = dt.Rows[i]["COST_CLASS"].ToString();
                    div1 = dt.Rows[i]["COST_ELEMENT"].ToString();

                    if (tempDiv != div || tempDiv1 != div1)
                    {	//소계, 제조원가, 총원가, 계산가격 readonly
                        if (tempDiv1 == "A99" || tempDiv1 == "B99" || tempDiv1 == "C99" || tempDiv1 == "D" || tempDiv1 == "F")
                        {
                            for (int h = 5; h < fpSpread1.Sheets[0].ColumnCount; h++)
                                UIForm.FPMake.grdReMake(fpSpread1, row_idx, h + "|3");
                        }
                        cnt++;
                        fpSpread1.Sheets[0].RowCount = cnt;
                        row_idx = cnt - 1;
                    }

                    fpSpread1.Sheets[0].Cells[row_idx, 1].Text = div;
                    fpSpread1.Sheets[0].Cells[row_idx, 2].Text = div1;
                    fpSpread1.Sheets[0].Cells[row_idx, 3].Text = dt.Rows[i]["COST_CLASS_NM"].ToString();

                    if (dt.Rows[i]["COST_CLASS_NM"].ToString() == dt.Rows[i]["COST_ELEMENT_NM"].ToString())
                        fpSpread1.Sheets[0].Cells[row_idx, 3].ColumnSpan = 2;
                    else
                        fpSpread1.Sheets[0].Cells[row_idx, 4].Text = dt.Rows[i]["COST_ELEMENT_NM"].ToString();


                    if (row_idx > 0)
                    {
                        if (tempDiv == div && tempDiv1 != div1)
                        {
                            rowspan++;
                        }

                        if (tempDiv != div)
                        {
                            fpSpread1.Sheets[0].Cells[rowfirst, 3].RowSpan = rowspan;
                            rowspan = 1;
                            rowfirst = row_idx;
                        }
                    }


                    price = Convert.ToDecimal(dt.Rows[i]["COST"]);
                    if (price != 0)
                    {
                        iYear_value = Convert.ToInt16(dt.Rows[i]["YM"].ToString().Substring(0, 4));
                        iMon = Convert.ToInt16(dt.Rows[i]["YM"].ToString().Substring(4, 2));
                        for (int j = 0; j <= 10; j++)
                        {
                            if (iYear_value == iYear[j])
                            {
                                for (int k = iYear_col[j]; k <= iYear_col[j] + 12; k++)
                                {
                                    if (Convert.ToInt16(fpSpread1.Sheets[0].ColumnHeader.Cells[1, k].Value) == iMon)
                                    {
                                        col_idx = k;
                                        break;
                                    }
                                }
                                break;
                            }
                        }

                        fpSpread1.Sheets[0].Cells[row_idx, col_idx].Value = price;
                    }

                    tempDiv = div;
                    tempDiv1 = div1;
                }
                //계산가격 readonly
                for (int h = 5; h < fpSpread1.Sheets[0].ColumnCount; h++)
                    UIForm.FPMake.grdReMake(fpSpread1, row_idx, h + "|3");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion


        #region SaveExec() 데이타 저장 로직
        protected override void SaveExec()
        {
            fpSpread1.Focus();

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //그리드 상단 필수 체크
                    if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
                    {
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            string strGbn = "";

                            if (strHead.Length > 0)
                            {
                                switch (strHead)
                                {
                                    case "U": strGbn = "U1"; break;
                                    case "I": strGbn = "I1"; break;
                                    case "D": strGbn = "D1"; break;
                                    default: strGbn = ""; break;
                                }

                                //칼럼수만큼 처리
                                for (int j = 5; j < fpSpread1.Sheets[0].Columns.Count; j++)
                                {

                                    string strSql = " usp_EMR004 '" + strGbn + "'";
                                    strSql += ", @pPROJECT_NO = '" + strProjNo + "'";
                                    strSql += ", @pITEM_CD = '" + strItemCd + "'";
                                    strSql += ", @pPROJECT_SEQ = '" + strProjSeq + "'";

                                    strSql += ", @pYYYYMM = '" + sYearMon[j] + "'";

                                    strSql += ", @pCOST_CLASS = '" + fpSpread1.Sheets[0].Cells[i, 1].Text + "'";
                                    strSql += ", @pCOST_ELEMENT = '" + fpSpread1.Sheets[0].Cells[i, 2].Text + "'";

                                    if (fpSpread1.Sheets[0].Cells[i, j].Text.Trim() != "")
                                        strSql += ", @pCOST_PV_T  = '" + fpSpread1.Sheets[0].Cells[i, j].Value + "'";
                                    else
                                        strSql += ", @pCOST_PV_T  = 0 ";

                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                                }
                            }
                        }
                    }
                    else
                    {
                        Trans.Rollback();
                        this.Cursor = Cursors.Default;
                        return;
                    }
                    Trans.Commit();
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = e.Message;
                    //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                    fpSpread2.Sheets[0].SetActiveCell(iActive_Row, 5);
                    fpSpread2.Sheets[0].AddSelection(iActive_Row, 1, 1, fpSpread2.Sheets[0].ColumnCount);
                    detail_SearchExec(strProjNo, strProjSeq, strItemCd); 
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Cursor = Cursors.Default;
            }

        }
        #endregion

        private void EMR004_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtProjectNo.Focus();
        }

        private void EMR004_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
       
    }
}
