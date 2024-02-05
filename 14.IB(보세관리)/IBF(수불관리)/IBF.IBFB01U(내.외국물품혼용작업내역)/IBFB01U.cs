#region 작성정보
/*********************************************************************/
// 단위업무명 : 내.외국물품혼용작업내역
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-06-10
// 작성내용 : 내.외국물품혼용작업내역 관리
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

namespace IBF.IBFB01U
{
    public partial class IBFB01U : UIForm.FPCOMM1
    {
        #region 변수선언
        private bool chk = false;
        #endregion

        #region 생성자
        public IBFB01U()
        {
            InitializeComponent();
        }
        #endregion 

        #region Form Load 시
        private void IBFB01U_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            txtTRNo.Focus();
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            txtTRNo.Focus();
        }
        #endregion

        #region RCopyExec 그리드 Row 복사
        protected override void RCopyExec()
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    if (fpSpread1.ActiveSheet.GetSelection(0) == null)
                    {
                        MessageBox.Show("복사할 Row를 선택하지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        int SelectedRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                        if (fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text.Trim() == "")
                        {
                            MessageBox.Show("합계가 있는 Row는 복사할 수 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        UIForm.FPMake.RowInsert(fpSpread1);

                        for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                        {
                            fpSpread1.Sheets[0].Cells[SelectedRow + 1, i].Value = fpSpread1.Sheets[0].Cells[SelectedRow, i].Value;
                        }
                    }
                    //					fpSpread1.EditMode = true;
                    //					fpSpread1.EditModePermanent = true;
                    //					fpSpread1.EditModeReplace = true;
                }
                else
                {
                    MessageBox.Show("복사할 데이타가 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("RowCopy (Row 복사 실패)", f.ToString());
                MessageBox.Show("Row 복사 실패", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region RowRemove - 그리드 삭제 플레그 등록
        protected override void DelExec()
        {	// 행 삭제
            try
            {
                int BeforeRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                int BeforeRowCount = fpSpread1.ActiveSheet.GetSelection(0).RowCount;
                int TmpRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                for (int i = BeforeRow; i < BeforeRow + BeforeRowCount; i++)
                {
                    if (fpSpread1.Sheets[0].RowHeader.Cells[TmpRow, 0].Text == "I")
                        fpSpread1.Sheets[0].Rows.Remove(TmpRow, 1);
                    else
                    {
                        if (fpSpread1.Sheets[0].Cells[TmpRow, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text.Trim() != "")
                        {
                            fpSpread1.Sheets[0].RowHeader.Cells[TmpRow, 0].Text = "D";
                            TmpRow++;
                        }
                        else TmpRow++;

                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("RowRemove (그리드 삭제버튼 클릭에러)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0007"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region DeleteExe 전체 삭제로직
        protected override void DeleteExec()
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count <= 0) return;

                if (MessageBox.Show(SystemBase.Base.MessageRtn("P0010"), "DELETE", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    string strSql = " usp_IBFB01U  'D2',";
                    strSql = strSql + " @pTRACKING_NO = '" + txtTRNo.Text + "',";
                    strSql = strSql + " @pSO_NO = '" + txtSO_NO.Text + "',";
                    strSql = strSql + " @pWORK_DEGREE = '" + txtWORK_DEGREE.Text + "', ";
                    strSql = strSql + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    string MSGCode = SystemBase.DbOpen.TranNonQuery(strSql, "P0002");
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Cursor = Cursors.Default;

                    SearchExec();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region PrintExec() 그리드 출력 로직
        protected override void PrintExec()
        {

            string[] RptParmValue = new string[5];
            string RptName = "";

            if (fpSpread1.Sheets[0].Rows.Count <= 0) return;
            //--레포트 파일 선택

            RptName = @"Report\" + "IBFB21P.rpt";
            RptParmValue[0] = "R1";
            RptParmValue[1] = txtTRNo.Text;
            RptParmValue[2] = txtWORK_DEGREE.Text;
            RptParmValue[3] = txtItemCd.Text;
            RptParmValue[4] = SystemBase.Base.gstrCOMCD;

            UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + " 출력", null, null, RptName, RptParmValue);	//공통크리스탈 10버전
            frm.ShowDialog();
        }
        #endregion
        
        #region SearchExec()  그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {

                    string strQuery = " usp_IBFB01U  'S1',";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "',";
                    strQuery = strQuery + " @pSO_NO = '" + txtSO_NO.Text + "',";
                    strQuery = strQuery + " @pWORK_DEGREE = '" + txtWORK_DEGREE.Text + "',";
                    strQuery = strQuery + " @pITEM_CD = '" + txtItemCd.Text + "',";
                    strQuery = strQuery + " @pCHILD_ITEM_CD = '" + txtChildItemCd.Text + "',";
                    strQuery = strQuery + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 5);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

            if (fpSpread1.Sheets[0].Rows.Count > 0) Spread_Sum();

            this.Cursor = Cursors.Default;
            fpSpread1.Focus();
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true)
            {
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
                    this.Cursor = Cursors.WaitCursor;

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";
                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;   //수정
                                case "D": strGbn = "D1"; break;   //삭제
                                case "I": strGbn = "I1"; break;   //입력
                                default: strGbn = ""; break;
                            }

                            string strQuery = " usp_IBFB01U '" + strGbn + "'";
                            strQuery = strQuery + ", @pTRACKING_NO = '" + txtTRNo.Text + "'";
                            strQuery = strQuery + ", @pSO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text + "'";
                            strQuery = strQuery + ", @pWORK_DEGREE = '" + txtWORK_DEGREE.Text + "'";
                            strQuery = strQuery + ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품번")].Text + "'";
                            strQuery = strQuery + ", @pSO_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "'";
                            strQuery = strQuery + ", @pCHILD_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목")].Text + "'";
                            strQuery = strQuery + ", @pCHILD_ITEM_QTY = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "집약원 단위량")].Value;
                            strQuery = strQuery + ", @pCHILD_ITEM_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "소요단위")].Text + "'";
                            strQuery = strQuery + ", @pSO_SUM_QTY = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "소요량")].Value;
                            strQuery = strQuery + ", @pLOC_PRICE = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "내자단가")].Value;
                            strQuery = strQuery + ", @pEXCH_PRICE = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외자단가CIF")].Value;
                            strQuery = strQuery + ", @pCUR= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")].Text + "'";
                            strQuery = strQuery + ", @pEXCH_RATE = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value;
                            strQuery = strQuery + ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
                    SearchExec();
                    this.Cursor = Cursors.Default;
                }
                catch
                {
                    this.Cursor = Cursors.Default;
                    Trans.Rollback();
                    MSGCode = "P0019";
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

            }
        }
        #endregion

        private void Spread_Sum()
        {
            decimal amt1 = 0, amt2 = 0, amt3 = 0;
            decimal tot1 = 0, tot2 = 0, tot3 = 0;
            int i = 0;

            try
            {
                if (fpSpread1.Sheets[0].Rows.Count == 1)
                {
                    Spread_Relock(0);

                    amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value);
                    amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value);
                    amt3 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value);

                    tot1 += amt1;
                    tot2 += amt2;
                    tot3 += amt3;

                    fpSpread1.Sheets[0].Rows.Add(1, 1);
                    fpSpread1.Sheets[0].Rows[1].BackColor = Color.Lavender;
                    fpSpread1.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = "합계";
                    fpSpread1.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = amt1;
                    fpSpread1.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value = amt2;
                    fpSpread1.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value = amt3;


                }
                else
                {
                    Spread_Relock(0);

                    for (i = 1; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        Spread_Relock(i);

                        if (fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품번")].Text == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품번")].Text)
                        {
                            amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value);
                            amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value);
                            amt3 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value);

                            if (i == fpSpread1.Sheets[0].Rows.Count - 1)
                            {
                                amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value);
                                amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value);
                                amt3 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value);

                                tot1 += amt1;
                                tot2 += amt2;
                                tot3 += amt3;

                                fpSpread1.Sheets[0].Rows.Add(i + 1, 1);
                                fpSpread1.Sheets[0].Rows[i + 1].BackColor = Color.Lavender;
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = "합계";
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = amt1;
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value = amt2;
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value = amt3;

                                amt1 = 0; amt2 = 0; amt3 = 0;

                                i = i + 1;
                            }

                        }
                        else
                        {
                            amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value);
                            amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value);
                            amt3 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value);

                            tot1 += amt1;
                            tot2 += amt2;
                            tot3 += amt3;

                            fpSpread1.Sheets[0].Rows.Add(i, 1);
                            fpSpread1.Sheets[0].Rows[i].BackColor = Color.Lavender;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = "합계";
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = amt1;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value = amt2;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value = amt3;

                            amt1 = 0; amt2 = 0; amt3 = 0;
                            i = i + 1;

                            if (i == fpSpread1.Sheets[0].Rows.Count - 1)
                            {
                                amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value);
                                amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value);
                                amt3 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value);

                                tot1 += amt1;
                                tot2 += amt2;
                                tot3 += amt3;

                                fpSpread1.Sheets[0].Rows.Add(i + 1, 1);
                                fpSpread1.Sheets[0].Rows[i + 1].BackColor = Color.Lavender;
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = "합계";
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = amt1;
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value = amt2;
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value = amt3;

                                amt1 = 0; amt2 = 0; amt3 = 0;

                                i = i + 1;
                            }

                        }

                    }

                }
                int cnt = fpSpread1.Sheets[0].Rows.Count;
                fpSpread1.Sheets[0].Rows.Add(cnt, 1);
                fpSpread1.Sheets[0].Rows[cnt].BackColor = Color.LightSteelBlue;
                fpSpread1.Sheets[0].Cells[cnt, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text = "";
                fpSpread1.Sheets[0].Cells[cnt, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = "총합계";

                fpSpread1.Sheets[0].Cells[cnt, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = tot1;
                fpSpread1.Sheets[0].Cells[cnt, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value = tot2;
                fpSpread1.Sheets[0].Cells[cnt, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value = tot3;

                fpSpread1.Sheets[0].Rows.Add(cnt + 1, 1);
                fpSpread1.Sheets[0].Rows[cnt + 1].BackColor = Color.LightSteelBlue;
                fpSpread1.Sheets[0].Cells[cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text = "";
                fpSpread1.Sheets[0].Cells[cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품번")].Text = "제조비율";
                fpSpread1.Sheets[0].Cells[cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = "내국재";

                if (tot1 + tot3 == 0)
                    fpSpread1.Sheets[0].Cells[cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = 0;
                else
                    fpSpread1.Sheets[0].Cells[cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = (tot1 / (tot1 + tot3)) * 100;

                fpSpread1.Sheets[0].Rows.Add(cnt + 2, 1);
                fpSpread1.Sheets[0].Rows[cnt + 2].BackColor = Color.LightSteelBlue;
                fpSpread1.Sheets[0].Cells[cnt + 2, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text = "";
                fpSpread1.Sheets[0].Cells[cnt + 2, SystemBase.Base.GridHeadIndex(GHIdx1, "품번")].Text = "제조비율";
                fpSpread1.Sheets[0].Cells[cnt + 2, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = "외국재";

                if (tot1 + tot3 == 0)
                    fpSpread1.Sheets[0].Cells[cnt + 2, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = 0;
                else
                    fpSpread1.Sheets[0].Cells[cnt + 2, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = (tot3 / (tot1 + tot3)) * 100;

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Spread_Relock(int row)
        {
            if (fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목")].Text.ToString().Substring(0, 1) == "P")
            {

                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "내자단가")].BackColor = Color.White;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "내자단가")].Locked = false;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "내자단가")].CanFocus = true;

                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "외자단가CIF")].Value = 0;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "외자단가CIF")].BackColor = Color.Gainsboro;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "외자단가CIF")].Locked = true;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "외자단가CIF")].CanFocus = true;

                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")].Text = "KRW";
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")].BackColor = Color.Gainsboro;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")].Locked = true;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")].CanFocus = true;

                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value = 1;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].BackColor = Color.Gainsboro;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Locked = true;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].CanFocus = true;

            }
            else
            {

                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "내자단가")].Value = 0;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "내자단가")].BackColor = Color.Gainsboro;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "내자단가")].Locked = true;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "내자단가")].CanFocus = true;

                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "외자단가CIF")].BackColor = Color.White;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "외자단가CIF")].Locked = false;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "외자단가CIF")].CanFocus = true;

                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")].BackColor = Color.White;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")].Locked = false;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")].CanFocus = true;

                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].BackColor = Color.White;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Locked = false;
                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].CanFocus = true;
            }
        }

        #region 버튼 Click
        private void btnTRNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF01', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Tracking No.팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTRNo.Value = Msgs[0].ToString();
                    txtSO_NO.Value = Msgs[1].ToString();
                    txtBUSINESS_CD.Value = Msgs[7].ToString();
                    txtBUSINESS_NM.Value = Msgs[8].ToString();
                }
                this.Cursor = Cursors.Default;
                txtWORK_DEGREE.Focus();
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                //품목 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF04', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue", "@pNAME" };
                string[] strSearch = new string[] { txtItemCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtItemCd.Text = Msgs[0].ToString();
                    txtItemNm.Value = Msgs[1].ToString();
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

        private void btnChildItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                //품목 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF22', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue", "@pNAME" };
                string[] strSearch = new string[] { txtChildItemCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "자품목 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtChildItemCd.Text = Msgs[0].ToString();
                    txtChildItemNm.Value = Msgs[1].ToString();
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


        private void butEXCH_RATE_Click(object sender, System.EventArgs e)
        {
            try
            {
                //환율 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF08', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue", "@pNAME" };
                string[] strSearch = new string[] { "USD", "KRW" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "환율 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    neEXCH_RATE.Value = Msgs[3].ToString();
                    txtCUR.Text = Msgs[0].ToString();
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


        private void btnCreate_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    if (MessageBox.Show("선택한 Tracking No.의 내.외국물품 혼용 작업 내역서를 생성하시겠습니까?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        string RtnMsg = SystemBase.Base.MessageRtn("B0042");

                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            this.Cursor = Cursors.WaitCursor;

                            string strQuery = "exec usp_IBFB01U_P ";
                            strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "',";
                            strQuery = strQuery + " @pSO_NO = '" + txtSO_NO.Text + "',";
                            strQuery = strQuery + " @pWORK_DEGREE = '" + txtWORK_DEGREE.Text + "',";
                            strQuery = strQuery + " @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "', ";
                            strQuery = strQuery + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);

                            string strRETURN = ds.Tables[0].Rows[0][0].ToString();
                            string strMSG_CD = ds.Tables[0].Rows[0][1].ToString();

                            if (strRETURN == "ER")
                            {
                                MessageBox.Show(strMSG_CD, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Trans.Rollback();
                                goto exit;
                            }

                            Trans.Commit();
                            this.Cursor = Cursors.Default;

                        exit:
                            dbConn.Close();

                            SearchExec();

                        }

                        catch (Exception f)
                        {
                            this.Cursor = Cursors.Default;
                            Trans.Rollback();
                            RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
                        }
                        dbConn.Close();
                        MessageBox.Show(RtnMsg, SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }

            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void butCompute_Click(object sender, System.EventArgs e)
        {
            if (neEXCH_RATE.Text.Trim() != "" && fpSpread1.Sheets[0].Rows.Count > 0)
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외자단가CIF")].Locked == false)    //&& fpSpread1.Sheets[0].Cells[i, 15].Text != "0.00"
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목")].Text != "")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")].Text = txtCUR.Text;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value = neEXCH_RATE.Value;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value = Convert.ToDecimal(neEXCH_RATE.Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, 14].Value);
                            fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                        }
                    }
                }

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text == "")
                    {
                        fpSpread1.Sheets[0].Rows.Remove(i, 1);
                    }
                }

                fpSpread1.Sheets[0].Rows.Remove(fpSpread1.Sheets[0].Rows.Count - 1, 1);
                fpSpread1.Sheets[0].Rows.Remove(fpSpread1.Sheets[0].Rows.Count - 1, 1);

                Spread_Sum();

            }
        }

        private void butOrder_Change_Click(object sender, System.EventArgs e)
        {
            if (txtOrder_Qty.Text.Trim() == "")
            {
                MessageBox.Show("소요량을 입력하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOrder_Qty.Focus();
                return;
            }

            if (txtItemCd.Text.Trim() == "")
            {
                MessageBox.Show("품목코드를 입력하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtItemCd.Focus();
                return;
            }

            char[] a;
            a = txtOrder_Qty.Text.ToCharArray();
            for (int j = 0; j < a.Length; j++)
            {
                if ((a[j] >= '0' && a[j] <= '9')) continue;
                else
                {
                    MessageBox.Show("숫자가 아닙니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }


            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {

                this.Cursor = Cursors.WaitCursor;

                string strQuery = " usp_IBFB01U 'P1' ";
                strQuery = strQuery + ", @pTRACKING_NO = '" + txtTRNo.Text + "'";
                strQuery = strQuery + ", @pWORK_DEGREE = '" + txtWORK_DEGREE.Text + "'";
                strQuery = strQuery + ", @pITEM_CD = '" + txtItemCd.Text + "'";
                strQuery = strQuery + ", @pSO_SUM_QTY = " + txtOrder_Qty.Text;
                strQuery = strQuery + ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
                SearchExec();
                this.Cursor = Cursors.Default;
            }
            catch
            {
                this.Cursor = Cursors.Default;
                Trans.Rollback();
                MSGCode = "P0019";
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


        }
        #endregion

        #region TextChanged
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtChildItemCd_TextChanged(object sender, System.EventArgs e)
        {
            txtChildItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtChildItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtSO_NO_TextChanged(object sender, System.EventArgs e)
        {
            if (txtTRNo.Text.Trim() != "" && txtSO_NO.Text.Trim() != "")
            {
                txtMax_Degree.Value = SystemBase.Base.CodeName("TRACKING_NO", "MAX(WORK_DEGREE)", "BF_MIXED_WORK_HDR", txtTRNo.Text.Trim(), " AND SO_NO ='" + txtSO_NO.Text.Trim() + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
        }

        private void txtTRNo_Leave(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTRNo.Text.Trim() != "")
                {
                    string strSql = "Select ENT_CD, ENT_NM  From UVW_S_PROJECT_ENT Where PROJECT_NO = '" + txtTRNo.Text.Trim() + "' AND BONDED_YN = 'Y' AND Rtrim(ENT_NM) <> '' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtBUSINESS_CD.Value = ds.Tables[0].Rows[0][0].ToString();
                        txtBUSINESS_NM.Value = ds.Tables[0].Rows[0][1].ToString();
                    }
                    txtSO_NO.Value = txtTRNo.Text.Trim();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                if (e.Column == 4)
                {
                    string strChild_Item_Cd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목")].Text.ToString().Trim();
                    if (strChild_Item_Cd != "")
                    {
                        string strSql = "Select ITEM_NM, ITEM_SPEC  From B_ITEM_INFO(Nolock) Where ITEM_CD = '" + strChild_Item_Cd + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목명")].Text = ds.Tables[0].Rows[0][0].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = ds.Tables[0].Rows[0][1].ToString();
                        }

                        Spread_Relock(e.Row);
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
        private void IBFB01U_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB01U_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion

		private void txtWORK_DEGREE_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter) SearchExec(); 
			
		}

		private void txtItemCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter) SearchExec(); 
		}

		private void txtChildItemCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter) SearchExec(); 
		}

    }
}








