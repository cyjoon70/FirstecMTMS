#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트별 차수별 품목별 재료비현황
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-23
// 작성내용 : 프로젝트별 차수별 품목별 재료비현황
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

namespace EM.EMR010
{
    public partial class EMR010 : UIForm.FPCOMM2
    {
        #region 변수선언
        bool form_act_chk = false;
        int TempRow = 10000;
        string strProjNo = "";
        string strItemCd = "";
        string strProjSeq = "";
        int iActive_Row;
        #endregion

        public EMR010()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void EMR010_Load(object sender, System.EventArgs e)
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
        private void btnItem_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW001 pu1 = new WNDW001();
                pu1.ShowDialog();
                if (pu1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu1.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            txtProjectNm.Value	= SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND SO_CONFIRM_YN = 'Y' ");
		}
        //품목
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, "");
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

                    string strQuery = " usp_EMR010 'S1'";
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
        //private void fpSpread2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (fpSpread2.Sheets[0].Rows.Count > 0)
        //    {
        //        try
        //        {
        //            int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;

        //            //같은 Row 조회 되지 않게
        //            if (intRow < 0)
        //            {
        //                return;
        //            }

        //            if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
        //            {
        //                return;
        //            }

        //            string ProjNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text.ToString();
        //            string ProjSeq = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트명")].Text.ToString();
        //            string ItemCd = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text.ToString();

        //            Detail_Search(ProjNo, ProjSeq, ItemCd);
        //        }
        //        catch (Exception f)
        //        {
        //            SystemBase.Loggers.Log(this.Name, f.ToString());
        //            MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
        //        }
        //    }
        //}

        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (TempRow != e.NewRow)
            {
                int Row = e.NewRow;
                string ProjNo = fpSpread2.Sheets[0].Cells[e.NewRow, 1].Text;
                string ProjSeq = fpSpread2.Sheets[0].Cells[e.NewRow, 3].Text;
                string ItemCd = fpSpread2.Sheets[0].Cells[e.NewRow, 4].Text;

                iActive_Row = Row;
                Detail_Search(ProjNo, ProjSeq, ItemCd, Row);
                TempRow = Row;
            }
        }
        private void Detail_Search(string ProjNo, string ProjSeq, string ItemCd, int Row)
        {
            this.Cursor = Cursors.WaitCursor;
            strProjNo = ProjNo;
            strItemCd = ItemCd;
            strProjSeq = ProjSeq;
            try
            {
                string strQuery = " usp_EMR010 'S2'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pPROJECT_NO='" + ProjNo + "'";
                strQuery += ", @pITEM_CD ='" + ItemCd + "'";
                strQuery += ", @pPROJECT_SEQ ='" + ProjSeq + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 1);
                if (fpSpread1.Sheets[0].RowCount > 0) fpSpread1_Summary();
                if (TempRow != Row)
                {
                    Set_Merge();
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
        private void Detail_Search(string ProjNo, string ProjSeq, string ItemCd)
        {
            this.Cursor = Cursors.WaitCursor;
            strProjNo = ProjNo;
            strItemCd = ItemCd;
            strProjSeq = ProjSeq;
            try
            {
                string strQuery = " usp_EMR010 'S2'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pPROJECT_NO='" + ProjNo + "'";
                strQuery += ", @pITEM_CD ='" + ItemCd + "'";
                strQuery += ", @pPROJECT_SEQ ='" + ProjSeq + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 1);
                if (fpSpread1.Sheets[0].RowCount > 0) fpSpread1_Summary();
                if (TempRow != fpSpread1.Sheets[0].ActiveRowIndex)
                {
                    Set_Merge();
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

        private void Set_Merge()
        {
            int rowspan = 1;
            int first_row_idx = 0;
            string temp_item = "";

            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, 5].Text == "" && fpSpread1.Sheets[0].Cells[i, 6].Text == "")
                    UIForm.FPMake.grdReMake(fpSpread1, i, "8|3#9|3#10|3");
                if (i == 0)
                {
                    temp_item = fpSpread1.Sheets[0].Cells[0, 3].Text;
                    first_row_idx = i;
                }
                else
                {
                    if (temp_item == fpSpread1.Sheets[0].Cells[i, 3].Text)
                    {
                        rowspan++;
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[first_row_idx, 3].RowSpan = rowspan;
                        fpSpread1.Sheets[0].Cells[first_row_idx, 3].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;

                        first_row_idx = i;
                        rowspan = 1;
                        temp_item = fpSpread1.Sheets[0].Cells[i, 3].Text;
                    }
                }

            }
        }        
        private void fpSpread1_Summary()
        {
            try
            {
                fpSpread1.Sheets[0].Rows.Count = fpSpread1.Sheets[0].Rows.Count + 1;
                int row_idx = fpSpread1.Sheets[0].Rows.Count - 1;
                for (int i = 1; i < fpSpread1.Sheets[0].ColumnCount; i++)
                {
                    if (i == 1)
                    {
                        fpSpread1.Sheets[0].FrozenTrailingRowCount = 1;	//하단 Column 1줄 고정

                        fpSpread1.Sheets[0].RowHeader.Cells[row_idx, 0].Text = "합계";
                        fpSpread1.Sheets[0].Rows[row_idx].BackColor = System.Drawing.Color.FromName("Beige");
                        fpSpread1.Sheets[0].Rows[row_idx].Locked = true;
                    }
                    FarPoint.Win.ComplexBorder complexBorder1 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine, System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(100)), ((System.Byte)(100)))), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None));
                    fpSpread1.Sheets[0].Cells.Get(row_idx, i).Border = complexBorder1;

                    if (i == 5 || i == 6 || i == 9)
                    {
                        string Str = UIForm.FPMake.IntToString(i);
                        string Area = Str + "1:" + Str + Convert.ToString(row_idx);
                        Cell r = fpSpread1.ActiveSheet.Cells[row_idx, i];

                        r.Formula = "SUM(" + Area + ")";
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[row_idx, i].CellType = new TextCellType();
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion        

        
        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
            {
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                        if (strHead == "U")
                        {
                            string strSql = " usp_EMR010 'U1' ";
                            strSql += ", @pPROJECT_NO = '" + strProjNo + "'";
                            strSql += ", @pITEM_CD = '" + strItemCd + "'";
                            strSql += ", @pPROJECT_SEQ = '" + strProjSeq + "'";

                            strSql += ", @pYYYYMM = '" + fpSpread1.Sheets[0].Cells[i, 1].Text + "'";
                            strSql += ", @pCOST_ELEMENT  = '" + fpSpread1.Sheets[0].Cells[i, 2].Text + "'";

                            if (fpSpread1.Sheets[0].Cells[i, 8].Text.Trim() != "")
                                strSql += ", @pEV_RATE  = '" + fpSpread1.Sheets[0].Cells[i, 8].Value + "'";
                            else
                                strSql += ", @pEV_RATE  = 0 ";


                            if (fpSpread1.Sheets[0].Cells[i, 9].Text.Trim() != "")
                                strSql += ", @pCOST_EV_T  = '" + fpSpread1.Sheets[0].Cells[i, 9].Value + "'";
                            else
                                strSql += ", @pCOST_EV_T  = 0 ";

                            strSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, 10].Text.Trim() + "'";

                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        }
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
                    Detail_Search(strProjNo, strProjSeq, strItemCd); 
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
      
        #region 실적금액 일괄생성
        private void btnCreate_Click(object sender, EventArgs e)
        {
            string ERRCode = "ER", MSGCode = "P0000";
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            this.Cursor = Cursors.WaitCursor;

            try
            {
                for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                {
                    string strSql = " usp_EMR010";
                    strSql = strSql + " @pType = 'P1' ";
                    strSql = strSql + ", @pPROJECT_NO = '" + fpSpread2.Sheets[0].Cells[i, 1].Text + "'";
                    strSql = strSql + ", @pITEM_CD = '" + fpSpread2.Sheets[0].Cells[i, 4].Text + "'";
                    strSql = strSql + ", @pPROJECT_SEQ  = '" + fpSpread2.Sheets[0].Cells[i, 3].Text + "'";
                    strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK")
                    { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프					 

                }

                Trans.Commit();

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = f.Message;
                //MSGCode = "P0019";
            }
        Exit:
            dbConn.Close();
            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        #endregion

        #region 그리드 ChangeEvent
        protected override void fpSpread1_ChangeEvent(int Row, int Col)
        {
            try
            {

                if (Col == 8 && fpSpread1.Sheets[0].Cells[Row, 8].Text != "")
                {
                    decimal rt = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, 8].Value.ToString());
                    decimal pv = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, 5].Value.ToString());
                    fpSpread1.Sheets[0].Cells[Row, 9].Value = rt / 100 * pv;

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        #endregion

        private void EMR010_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtProjectNo.Focus();
        }

        private void EMR010_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }

       

    }
}
