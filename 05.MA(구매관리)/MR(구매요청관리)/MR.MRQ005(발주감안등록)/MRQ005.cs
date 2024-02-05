
#region 작성정보
/*********************************************************************/
// 단위업무명 :  발주감안등록
// 작 성 자   :  박 은 수
// 작 성 일   :  2016.12.26
// 작성내용   :  구매요청건들에 대해 발주의 미입고분으로 감안 관리하도록 한다.
// 수 정 일   :  2017.04.01. ~
// 수 정 자   :  한 미 애
// 수정내용   :  
// 비    고   :
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

namespace MR.MRQ005
{
    public partial class MRQ005 : UIForm.FPCOMM1
    {

        #region 변수선언
        string strBtn = "N";
        private bool form_act_chk = false;
		DataTable dtPrint = new DataTable();
        #endregion

		#region 생성자
		public MRQ005()
        {
            InitializeComponent();
        }
		#endregion

		#region Form Load 시
		private void MRQ005_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            rdoNo.Checked = true;
            rdoAvailAll.Checked = true;     // 2019.03.05. hma 추가: 가능수량구분 검색조건에서 기본적으로 '전체'가 선택되도록 함.
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoNo.Checked = true;

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    string strYn = "";
                    if (rdoYes.Checked == true) { strYn = "Y"; }
                    else if (rdoNo.Checked == true) { strYn = "N"; }

                    string strReqPart = "";
                    if (rdoMpr.Checked == true) { strReqPart = "M"; }
                    else if (rdoSpr.Checked == true) { strReqPart = "S"; }

                    string strReqType = "";
                    if (rdoMrp.Checked == true) { strReqType = "M"; }
                    else if (rdoManual.Checked == true) { strReqType = "E"; }
                    else if (rdoP.Checked == true) { strReqType = "P"; }

                    // 2017.07.24. hma 추가(Start): 발주감안가능수량 구분
                    string strAvailQtyYN = "";
                    if (rdoAvailY.Checked == true) { strAvailQtyYN = "Y"; }
                    else if (rdoAvailN.Checked == true) { strAvailQtyYN = "N"; }
                    // 2017.07.24. hma 추가(End)

                    string strQuery = " usp_MRQ005  @pTYPE = 'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
                    strQuery += ", @pREQ_DT_FR = '" + dtpReqDtFr.Text + "' ";
                    strQuery += ", @pREQ_DT_TO = '" + dtpReqDtTo.Text + "' ";
                    strQuery += ", @pREQ_DEPT_CD = '" + txtReqDeptCd.Text.Trim() + "' ";
                    strQuery += ", @pREQ_REORG_ID = '" + txtReqReorgId.Text.Trim() + "' ";
                    strQuery += ", @pREQ_ID = '" + txtUserId.Text.Trim() + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjNo.Text.Trim() + "' ";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjSeq.Text.Trim() + "' ";
                    strQuery += ", @pREQ_PART = '" + strReqPart + "' ";
                    strQuery += ", @pREQ_TYPE = '" + strReqType + "' ";
                    strQuery += ", @pREQ_PO_REF_YN = '" + strYn + "' ";
                    strQuery += ", @pREQ_NO = '" + txtReqNo.Text.Trim() + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text.Trim() + "' ";
                    strQuery += ", @pAVAIL_QTY_YN = '" + strAvailQtyYN + "' ";      // 2017.07.24. hma 추가: 발주감안가능수량

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 5);
					if (fpSpread1.Sheets[0].RowCount > 0)
					{
						Set_ReMake(); 
					}

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }

        private void Set_ReMake()
        {
            for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청상태코드")].Text == "0" ||
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청상태코드")].Text == "10")     // 2017.07.04. hma 수정: 상태코드 10으로 변경
                {
                    UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주확인") + "|0");

                    //if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청조정")].Text == "True")
                    //    UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량") + "|1#"
                    //                            + SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량_2") + "|3");
                    //else
                    //{
                    //    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "참조정보여부")].Text == "Y")
                    //        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청조정") + "|3#"
                    //                            + SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량") + "|3#"
                    //                            + SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량_2") + "|0");
                    //    else

                    //        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청조정") + "|0#"
                    //                                + SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량") + "|3#"
                    //                                + SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량_2") + "|0");
                    //}
                }
                else
                {
                    UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주확인") + "|3#"
                                            //+ SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청조정") + "|3#"
                                            + SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량") + "|3#"
                                            + SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량_2") + "|3");
                }
            }
        }

        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;
            // 그리드 상단 필수항목 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)// 그리드 필수항목 체크 
            {
                string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.

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
						string strSql = string.Empty;

                        if (strHead.Length > 0)
                        {

                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;
                                case "I": strGbn = "I1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }
                            string strYn = "N";
                            string strYn1 = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주확인")].Text == "True") { strYn = "Y"; }
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청조정")].Text == "True") { strYn1 = "Y"; }

							strSql = string.Empty;

                            strSql = " usp_MRQ005 '" + strGbn + "'";
                            strSql += ", @pREQ_PO_REF_YN = '" + strYn + "'";
                            strSql += ", @pREQ_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호")].Text + "'";
                            strSql += ", @pREQ_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text + "'";
                            strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "'";
                            //strSql += ", @pREQ_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "'";
                            //strSql += ", @pDISUSE_REF = '" + strYn1 + "'";
                            strSql += ", @pPO_REF_QTY = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량")].Value;
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

                //기존 그리드 위치를 가져온다
                int chkRow = 0;
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                { chkRow = fpSpread1.Sheets[0].ActiveRowIndex; }

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
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

        #region 그리드 버튼 클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            DialogResult dsMsg;
            try
            {
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청조정"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, Column].Text == "True")
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조정보여부")].Text == "Y")
                        {
                            MessageBox.Show("재고참조건 있습니다! 재고참조팝업에서 선택된 부분을 취소해야 합니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            fpSpread1.Sheets[0].Cells[Row, Column].Value = 0;
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량_2") + "|3");
                        }
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량") + "|3#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량_2") + "|0");

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량")].Text = "0";
                    }
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량_2"))
                {
                    decimal ref_qty = 0;

                    string Query = " usp_MRQ005 'C1'";
                    Query += ", @pREQ_NO = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호")].Text + "'";
                    Query += ", @pREQ_SEQ = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text + "'";
                    Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    if (dt.Rows.Count > 0)
                    {
                        ref_qty = Convert.ToDecimal(dt.Rows[0][0]);
                    }
					
                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주미입고량")].Value) == 0
                        && Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량")].Value) == 0
                        && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조정보여부")].Text == "N")
                    {
                        dsMsg = MessageBox.Show("발주미입고량이 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량")].Value) != 0
                        && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조정보여부")].Text == "N")
                    {
                        dsMsg = MessageBox.Show("참조정보가 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주확인")].Text != "True")
                    {
                        dsMsg = MessageBox.Show("발주확인를 먼저 체크하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fpSpread1.Sheets[0].SetActiveCell(Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주확인"));
                        return;
                    }

                    bool locking = false;

                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청상태코드")].Text == "0" ||
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청상태코드")].Text == "9")
                    {

                        locking = false;
                    }
                    else
                    {
                        locking = true;
                    }

					MRQ005P1 frm1 = new MRQ005P1(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호")].Text,
												fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text,
												fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text,
												fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text,
												Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value),
												Convert.ToDecimal(ref_qty), locking,
												fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text,
												fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text,
												fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조여부")].Text,
                                                Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value) -
                                                    Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고참조량")].Value));    // 2017.07.05. hma 추가


					frm1.ShowDialog();

					if (frm1.DialogResult == DialogResult.OK)
					{
						string Msgs = frm1.ReturnVal;
						decimal Val = frm1.ReturnRef;

						if (Msgs == "Y")
						{
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량")].Value = Val;
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주확인")].Value = 1;

							if (Val > 0)
							{
								fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조정보여부")].Text = "Y";
								UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청조정") + "|3");
							}
							else
							{
								fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조정보여부")].Text = "N";
								UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청조정") + "|0");
							}

                            // 실요청량 = 요청수량 - 재고감안량 - 발주참조량 
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "실요청량")].Value
								= Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value) 
                                    - Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고참조량")].Value)   // 2017.07.11. hma 추가: 재고감안수량도 차감하도록 
                                    - Val;
						}
					}
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region 그리드 change 클릭
        protected override void fpSpread1_ChangeEvent(int Row, int Col)
        {
            try
            {
                if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량"))
                {
                    decimal req_qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value);
                    decimal ref_qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량")].Value);
                    if (ref_qty > req_qty)
                    {
                        MessageBox.Show("발주참조량은 요청수량보다 많을 수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량")].Value = 0;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 버튼 click
        private void butReqDept_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'D011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtReqDeptCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "부서 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReqDeptCd.Text = Msgs[0].ToString();
                    txtReqDeptNm.Value = Msgs[1].ToString();
                    txtReqReorgId.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnUser_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'B011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtUserId.Text = Msgs[0].ToString();
                    txtUserNm.Value = Msgs[1].ToString();
                    //					txtReqDeptCd.Text = Msgs[2].ToString();
                    //					txtReqDeptNm.Text = Msgs[3].ToString();
                    //					txtReqReorgId.Text = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnProj_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {

                WNDW007 pu = new WNDW007(txtProjNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjNo.Text = Msgs[3].ToString();
                    txtProjSeq.Text = "";
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }


        private void btnProjSeq_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

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
            strBtn = "N";
        }


        private void butItem_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW005 pu = new WNDW005(txtItemCd.Text, "30");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }


        private void btnReqNo_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW016 pu = new WNDW016();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtReqNo.Text = Msgs[1].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }
        #endregion

        #region TextChanged
        private void txtUserId_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }


        private void txtReqDeptCd_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
            {
                string Query = " usp_B_COMMON 'D021' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    txtReqReorgId.Value = dt.Rows[0][0].ToString();
                }
                else
                {
                    txtReqReorgId.Value = "";
                }

                txtReqDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtReqDeptCd.Text, " And REORG_ID = '" + txtReqReorgId.Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
        }

        private void txtProjNo_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtProjSeq.Text = "";
        }

        private void txtProjSeq_Leave(object sender, System.EventArgs e)
        {
            if (strBtn == "N" && txtProjSeq.Text != "*")
            {
                string seq = SystemBase.Base.CodeName("PROJECT_NO", "MAX(PROJECT_SEQ)", "S_SO_DETAIL", txtProjNo.Text, " AND PROJECT_SEQ = '" + txtProjSeq.Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                if (seq == "")
                {	//"프로젝트차수가 잘못 입력되었습니다!"
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0054"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProjSeq.Text = "";
                    txtProjSeq.Focus();
                }
                else
                {
                    txtProjSeq.Text = seq;
                }
            }
        }

        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text.Trim(), " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 선택버튼 click
        private void btnSelectAll_Click(object sender, System.EventArgs e)
        {
            int col = SystemBase.Base.GridHeadIndex(GHIdx1, "발주확인");

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, col].Text != "True" && fpSpread1.Sheets[0].Cells[i, col].Locked == false)
                {
                    fpSpread1.Sheets[0].Cells[i, col].Value = 1;
                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                }
            }
        }

        private void btnSelectCancel_Click(object sender, System.EventArgs e)
        {
            int col = SystemBase.Base.GridHeadIndex(GHIdx1, "발주확인");

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, col].Text == "True" && fpSpread1.Sheets[0].Cells[i, col].Locked == false)
                {
                    fpSpread1.Sheets[0].Cells[i, col].Value = 0;
                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                }
            }
        }
        #endregion

        #region Activated, Deactivate
        private void MRQ005_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpReqDtFr.Focus();
        }

        private void MRQ005_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion


		#region 수량 형식 변경
		private string SetConvert(decimal dNumber)
		{
			string strReturn = string.Empty;

			strReturn = double.Parse(dNumber.ToString()).ToString();

			return strReturn;
		}
		#endregion

		#region 현재행 강조
		private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
		{
			fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;
		}
		#endregion

		#region 선택 행 강조
		private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{
			fpSpread1.Sheets[0].AddSelection(e.Row, 0, 1, fpSpread1.Sheets[0].ColumnCount);
		}
		#endregion

	}
}
