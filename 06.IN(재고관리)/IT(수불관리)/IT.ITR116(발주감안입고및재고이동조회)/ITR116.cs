#region 작성정보
/*********************************************************************/
// 단위업무명:  발주감안오더 입고및재고이동 조회
// 작 성 자  :  한 미 애
// 작 성 일  :  2017-08-27
// 작성내용  :  발주감안등록된 구매오더의 입고 및 재고이동 이력 조회
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
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

namespace IT.ITR116
{
    public partial class ITR116 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;
        #endregion

        #region 생성자
        public ITR116()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void ITR116_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //공장
 
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpRcptDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpRcptDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0,10);
            txtSlFr.Value = "W03";
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;
 
            //기타 세팅
            dtpRcptDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpRcptDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            txtSlFr.Value = "W03";
        }
        #endregion

        #region SearchExec()  그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_ITR116 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pSL_CD ='" + txtSlFr.Text.Trim() + "'";
                    strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                    strQuery += ", @pRCPT_DT_FR ='" + dtpRcptDtFr.Text + "'";
                    strQuery += ", @pRCPT_DT_TO ='" + dtpRcptDtTo.Text + "'";
                    strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ_FR ='" + txtProject_Seq.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ_TO ='" + txtProject_Seq1.Text.Trim() + "'";
                    strQuery += ", @pPO_NO_FR ='" + txtPoNoFr.Text.Trim() + "'";
                    strQuery += ", @pPO_NO_TO ='" + txtPoNoTo.Text.Trim() + "'";                    

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 2, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;                   
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                SDown++;

                this.Cursor = Cursors.WaitCursor;

                string strQuery = " usp_ITR116 'S1'";
                strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                strQuery += ", @pTRAN_DT_FR ='" + dtpRcptDtFr.Text + "'";
                strQuery += ", @pTRAN_DT_TO ='" + dtpRcptDtTo.Text + "'";
                strQuery += ", @pSL_CD_FR ='" + txtSlFr.Text.Trim() + "'";
                strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                strQuery += ", @pPROJECT_SEQ_FR ='" + txtProject_Seq.Text.Trim() + "'";
                strQuery += ", @pPROJECT_SEQ_TO ='" + txtProject_Seq1.Text.Trim() + "'";
                strQuery += ", @pWORKORDER_NO_FR ='" + txtPoNoFr.Text.Trim() + "'";
                strQuery += ", @pWORKORDER_NO_TO ='" + txtPoNoTo.Text.Trim() + "'";
                strQuery += ", @pTOPCOUNT ='" + AddRow * SDown + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 버튼 Click
        #region btnSlFr_Click(): 창고 검색 버튼 클릭 이벤트
        private void btnSlFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B035', @pSPEC1 = '" + cboPlantCd.SelectedValue.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSlFr.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00014", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSlFr.Value = Msgs[0].ToString();
                    txtSlNmFr.Value = Msgs[1].ToString();
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

        #region btnItem_Click(): 품목코드 검색 버튼 클릭 이벤트
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCd.Text);
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region btnProject_Click(): 프로젝트 검색 버튼 클릭 이벤트
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProject_No.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProject_No.Value = Msgs[3].ToString();
                    txtProject_Nm.Value = Msgs[4].ToString();
                    if (txtProject_Seq.Text != "*") txtProject_Seq.Value = "";
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

        #region btnProjectSeq_Click(): 프로젝트차수(FROM) 검색 버튼 클릭 이벤트
        private void btnProjectSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProject_No.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };        // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };                   // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProject_Seq.Value = Msgs[0].ToString();
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

        #region btnProjectSeq_Click(): 프로젝트차수(TO) 검색 버튼 클릭 이벤트
        private void btnProjectSeq1_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProject_No.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };    // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };               // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProject_Seq1.Value = Msgs[0].ToString();
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

        #region btnPoNoFr_Click(): 구매오더(FROM) 검색 버튼 클릭 이벤트
        private void btnPoNoFr_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW018 pu = new WNDW.WNDW018();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPoNoFr.Value = Msgs[1].ToString();
                    txtPoNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매발주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnPoNoTo_Click(): 구매오더(TO) 검색 버튼 클릭 이벤트
        private void btnPoNoTo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW018 pu = new WNDW.WNDW018();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPoNoTo.Value = Msgs[1].ToString();
                    txtPoNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매발주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        #endregion

        #region TextChanged 이벤트
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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

        private void txtSlFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSlFr.Text != "")
                {
                    txtSlNmFr.Value = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", txtSlFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSlNmFr.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            
            try
            {
                if (txtProject_No.Text != "")
                {
                    txtProject_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProject_Nm.Value = "";
                }
                if (txtProject_Seq.Text != "*")
                { txtProject_Seq.Value = ""; txtProject_Seq1.Value = ""; }
            }
            catch
            {

            }
        }
        #endregion

        #region 폼 Activated & Deactivate
        private void ITR116_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void ITR116_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            ITR116P1 frm1 = new ITR116P1(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text,
                                                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text,
                                                Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Value),
                                                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청번호")].Text,
                                                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text,
                                                Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청확정수량")].Value),
                                                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text,
                                                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text,
                                                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text,
                                                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명세")].Text,
                                                Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주감안수량")].Value),
                                                Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고수량")].Value),
                                                Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고이동수량")].Value) 
                                        );
            frm1.ShowDialog();
        }
    }
}
