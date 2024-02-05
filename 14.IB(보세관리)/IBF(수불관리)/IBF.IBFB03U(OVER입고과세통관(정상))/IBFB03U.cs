#region 작성정보
/*********************************************************************/
// 단위업무명 : OVER입고과세통관(정상)
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-06-10
// 작성내용 : OVER입고과세통관(정상) 관리
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
using FarPoint.Win.Spread.CellType;

namespace IBF.IBFB03U
{
    public partial class IBFB03U : UIForm.FPCOMM1
    {
        #region 변수선언
        private bool chk = false;
        #endregion

        #region 생성자
        public IBFB03U()
        {
            InitializeComponent();
        }
        #endregion 

        #region Form Load 시
        private void IBFB03U_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            dtpDT.Value = DateTime.Today.ToString().Substring(0, 10);
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {

                    string strQuery = " usp_IBFB03U  'S1',";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "',  ";
                    strQuery = strQuery + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    //				strQuery =	strQuery + " @pISSUE_DT_FR = '" + dtpIssueFrDt.Text  + "',"; 
                    //				strQuery =	strQuery + " @pISSUE_DT_TO = '" + dtpIssueToDt.Text  + "',";
                    //				strQuery =	strQuery + " @pDECLARE_DT_FR = '" + dtpNotifyFrDt.Text  + "',";
                    //				strQuery =	strQuery + " @pDECLARE_DT_TO = '" + dtpNotifyToDt.Text  + "',";
                    //strQuery =	strQuery + " @pDECLARE_NO = '" + txtNotifyNo.Text  + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                    fpSpread1.Sheets[0].Rows.Count = 0;
                    decimal dblrest = 0;
                    if (dt.Rows.Count > 0)
                    {
                        int i = 0;
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            dblrest = Convert.ToDecimal(dt.Rows[j][8].ToString()) - Convert.ToDecimal(dt.Rows[j][9].ToString()) - Convert.ToDecimal(dt.Rows[j][7].ToString());

                            if (Convert.ToDecimal(dt.Rows[j][9].ToString()) > 0 || dblrest > 0)
                            {
                                fpSpread1.Sheets[0].AddRows(i, 1);
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = dt.Rows[j][1].ToString();
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No.")].Text = dt.Rows[j][2].ToString();
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text = dt.Rows[j][3].ToString();
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목명")].Text = dt.Rows[j][4].ToString();
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = dt.Rows[j][5].ToString();
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = dt.Rows[j][6].ToString();
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value = Convert.ToDecimal(dt.Rows[j][7].ToString());
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Value = Convert.ToDecimal(dt.Rows[j][8].ToString());
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value = Convert.ToDecimal(dt.Rows[j][9].ToString());
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value = dblrest;

                                if (dblrest == 0)
                                {
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].BackColor = Color.Gainsboro;
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Locked = true;
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].CanFocus = true;

                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고번호")].BackColor = Color.Gainsboro;
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고번호")].Locked = true;
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고번호")].CanFocus = true;

                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고일자")].BackColor = Color.Gainsboro;
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고일자")].Locked = true;
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고일자")].CanFocus = true;
                                }
                                //								fpSpread1.Sheets[0].Cells[i,14].Text = dt.Rows[j][14].ToString();

                                i++;
                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            this.Cursor = Cursors.Default;
            fpSpread1.Focus();
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {

            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                this.Cursor = Cursors.WaitCursor;
                //행수만큼 처리
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                    string strGbn = "";
                    if (strHead.Length > 0 && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Text.Trim() == "" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Value.ToString() == "0")
                        {
                            MessageBox.Show("과세신고수량을 입력세요", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            fpSpread1.Sheets[0].SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5"));
                            Trans.Rollback(); goto Exit;
                        }
                        else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고번호")].Text.Trim() == "")
                        {
                            MessageBox.Show("신고번호를 입력세요", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            fpSpread1.Sheets[0].SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고번호"));
                            Trans.Rollback(); goto Exit;
                        }
                        else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고일자")].Text.Trim() == "")
                        {
                            MessageBox.Show("신고일자를 입력세요", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            fpSpread1.Sheets[0].SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고일자"));
                            Trans.Rollback(); goto Exit;
                        }

                        switch (strHead)
                        {
                            case "U": strGbn = "U1"; break;   //수정
                            //								case "D": strGbn = "D1"; break;   //삭제
                            //								case "I": strGbn = "I1"; break;   //입력
                            default: strGbn = ""; break;
                        }

                        string strQuery = " usp_IBFB03U '" + strGbn + "'";
                        strQuery = strQuery + ", @pTRACKING_NO = '" + txtTRNo.Text + "'";
                        strQuery = strQuery + ", @pCHILD_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text + "'";
                        strQuery = strQuery + ", @pNOTIFY_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고번호")].Text + "'";
                        strQuery = strQuery + ", @pNOTIFY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고일자")].Text + "'";
                        strQuery = strQuery + ", @pUNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "'";
                        strQuery = strQuery + ", @pNOTIFY_QTY = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Value;
                        strQuery = strQuery + ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    }
                }
                Trans.Commit();
                this.Cursor = Cursors.Default;
                SearchExec();
            }
            catch
            {
                Trans.Rollback();
                MSGCode = "P0019";
            }
        Exit:
            this.Cursor = Cursors.Default;
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

        }
        #endregion

        #region 버튼 Click
        private void btnTRNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF11' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Tracking No.팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTRNo.Text = Msgs[0].ToString();
                    txtBUSINESS_CD.Value = Msgs[7].ToString();
                    txtBUSINESS_NM.Value = Msgs[8].ToString();

                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void butCompute_Click(object sender, System.EventArgs e)
        {
            if (txtNotifyNo.Text.Trim() != "")
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Text.Trim() != "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고일자")].Value = dtpDT.Text;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고번호")].Value = txtNotifyNo.Text;
                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                    }
                }
            }
        }
        #endregion

        #region TextChanged
        protected override void fpSpread1_ChangeEvent(int Row, int Col)
        {
            if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5"))
            {
                decimal rest_qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Text);
                if (rest_qty > 0)
                {
                    decimal noti_qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Text);
                    if (noti_qty > rest_qty)
                    {
                        MessageBox.Show("과세신고수량은 잔량보다 클 수 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].ResetValue();
                    }
                    if (noti_qty > 0) fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 1;
                    else fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 0;
                }
            }

        }

        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {

                string strITEM_CD = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text;
                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value) > 0)
                {
                    IBFB03P frm = new IBFB03P(txtTRNo.Text.Trim(), strITEM_CD);
                    frm.ShowDialog();

                    if (frm.ReturnVal == "Y") SearchExec();

                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtpDT_Leave(object sender, System.EventArgs e)
        {
            if (dtpDT.Text.Trim() != "")
            {
//                if (SystemBase.Base.IsDate(dtpDT.Text) == false)
//                {
//                    MessageBox.Show(SystemBase.Base.MessageRtn("B023"));
//                    dtpDT.Focus();
//                    dtpDT.SelectAll();
//                }
            }
        }


        private void txtTRNo_Leave(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTRNo.Text.Trim() != "")
                {
                    string strSql = "Select ENT_CD, ENT_NM  From UVW_S_PROJECT_ENT  Where PROJECT_NO  = '" + txtTRNo.Text.Trim() + "' AND BONDED_YN = 'Y' AND Rtrim(ENT_NM) <> '' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtBUSINESS_CD.Value = ds.Tables[0].Rows[0][0].ToString();
                        txtBUSINESS_NM.Value = ds.Tables[0].Rows[0][1].ToString();
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region 폼 Activated & Deactivated
        private void IBFB03U_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB03U_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion

        private void txtTRNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

    }
}








