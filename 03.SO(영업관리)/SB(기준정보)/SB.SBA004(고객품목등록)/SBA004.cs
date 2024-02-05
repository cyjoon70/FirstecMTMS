#region 작성정보
/*********************************************************************/
// 단위업무명 : 고객품목등록
// 작 성 자 : 김 현근
// 작 성 일 : 2013-04-24
// 작성내용 : 고객품목등록
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

namespace SB.SBA004
{
    public partial class SBA004 : UIForm.FPCOMM1
    {
        #region 생성자
        public SBA004()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SBA004_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용

            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//품목단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "고객품목단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//고객품목단위

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            txtCustCd.Focus();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_SBA004  @pTYPE = 'S1'";
                strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                strQuery += ", @pCUST_NM = '" + txtCustNm.Text + "' ";
                strQuery += ", @pCUST_ITEM_CD = '" + txtCustItemCd.Text + "' ";
                strQuery += ", @pCUST_ITEM_NM = '" + txtCustItemNm.Text + "' ";
                strQuery += ", @pCUST_ITEM_SPEC = '" + txtCustItemSpec.Text + "' ";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                strQuery += ", @pITEM_NM = '" + txtItemNm.Text + "' ";
                strQuery += ", @pITEM_SPEC = '" + txtItemSpec.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                //Detail Locking설정
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    UIForm.FPMake.grdReMake(fpSpread1,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "고객코드_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|3"
                        );
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

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
            {
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                string strKeyCd = "";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
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

                            // 그리드 상단 필수항목 체크

                            string strSql = " usp_SBA004 '" + strGbn + "'";
                            strSql += ", @pCUST_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "고객코드")].Text + "' ";
                            strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                            strSql += ", @pCUST_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "고객품목코드")].Text + "' ";
                            strSql += ", @pCUST_ITEM_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "고객품목명")].Text + "' ";
                            strSql += ", @pCUST_ITEM_SPEC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "고객품목규격")].Text + "' ";
                            strSql += ", @pCUST_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "고객품목단위")].Text + "' ";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();
                            strKeyCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "고객코드")].Text;
                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        }
                    }
                    Trans.Commit();
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                //기존 그리드 위치를 가져온다
                int chkRow = 0;
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                { chkRow = fpSpread1.Sheets[0].ActiveRowIndex; }

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strKeyCd);
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                //조회후 기존 그리드 위치로 이동
                fpSpread1.ActiveSheet.SetActiveCell(chkRow, 1);
                fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 팝업창열기
        //고객
        private void btnCust_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                    txtCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //고객품목코드
        private void btnCustItem_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW001 pu = new WNDW001(txtCustItemCd.Text, "CUST");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustItemCd.Text = Msgs[1].ToString();
                    txtCustItemNm.Value = Msgs[2].ToString();
                    txtCustItemSpec.Value = Msgs[5].ToString();
                    txtCustItemCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "고객 품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //품목코드
        private void btnItem_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW001 pu = new WNDW001(txtItemCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[1].ToString();
                    txtItemNm.Value = Msgs[2].ToString();
                    txtItemSpec.Text = Msgs[5].ToString();
                    txtItemCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 텍스트 체인지
        //고객
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //고객품목
        private void txtCustItemCd_TextChanged(object sender, EventArgs e)
        {
            txtCustItemNm.Value = SystemBase.Base.CodeName("CUST_ITEM_CD", "CUST_ITEM_NM", "S_CUST_ITEM", txtCustItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //품목
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 그리드 팝업
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "고객코드_2"))
            {
                try
                {
                    WNDW002 pu = new WNDW002(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "고객코드")].Text, "");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "고객코드")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "고객명")].Text = Msgs[2].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2"))
            {
                try
                {
                    WNDW001 pu = new WNDW001(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, "");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목단위")].Text = Msgs[6].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목규격")].Text = Msgs[5].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 그리드상의 코드 입력시 코드명 자동변환
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "고객코드"))
            {
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "고객명")].Text
                    = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "고객코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"))
            {
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text
                    = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목단위")].Text
                    = SystemBase.Base.CodeName("ITEM_CD", "ITEM_UNIT", "B_ITEM_INFO", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목규격")].Text
                    = SystemBase.Base.CodeName("ITEM_CD", "ITEM_SPEC", "B_ITEM_INFO", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
        }
        #endregion
    }
}
