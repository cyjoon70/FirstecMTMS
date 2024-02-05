#region 작성정보
/*********************************************************************/
// 단위업무명 :구매견적채택
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-24
// 작성내용 :구매견적채택
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
namespace ME.MEA002
{
    public partial class MEA002 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strBtn = "N";
        bool form_act_chk = false;
        #endregion

        public MEA002()
        {
            InitializeComponent();

        }

        #region Form Load 시
        private void MEA002_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);//화폐단위

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' " + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//화폐단위
            
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            
            //기타 세팅
            dtpEstDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpEstDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            cboSort1.SelectedText = "품목코드";
            cboSort2.SelectedText = "제출단가";
            cboSort3.SelectedText = "납품가능일";

            rdoSelect_CheckedChanged();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            cboSort1.SelectedText = "품목코드";
            cboSort2.SelectedText = "제출단가";
            cboSort3.SelectedText = "납품가능일";
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                try
                {
                    string strDiv = "";
                    if (rdoSelectY.Checked == true) { strDiv = "Y"; }
                    else if (rdoSelectN.Checked == true) { strDiv = "N"; }

                    string strQuery = " usp_MEA002  @pTYPE = 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pEST_DT_FR = '" + dtpEstDtFr.Text + "' ";
                    strQuery += ", @pEST_DT_TO = '" + dtpEstDtTo.Text + "' ";
                    strQuery += ", @pDELIVERY_DT_FR= '" + dtpDeliveryDtFr.Text + "' ";
                    strQuery += ", @pDELIVERY_DT_TO = '" + dtpDeliveryDtTo.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pCURRENCY = '" + cboCurrency.SelectedValue.ToString() + "' ";
                    strQuery += ", @pPUR_DUTY = '" + txtUserId.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjNo.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjSeq.Text + "' ";
                    strQuery += ", @pCUST_CD = '" + txtSCustCd.Text.Trim() + "' ";
                    strQuery += ", @pSTATUS = '" + strDiv + "' ";
                    strQuery += ", @pEST_NO = '" + txtEstNo.Text + "' ";
                    strQuery += ", @pREQ_NO = '" + txtReqNo.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {
                        Set_Locking();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        private void Set_Locking()
        {
            int cnt0 = 1;
            int cnt1 = 1;
            int i1 = SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰번호");
            int i2 = SystemBase.Base.GridHeadIndex(GHIdx1, "순번");
            int i3 = SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드");
            int i4 = SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2");
            int i5 = SystemBase.Base.GridHeadIndex(GHIdx1, "품명");
            int i6 = SystemBase.Base.GridHeadIndex(GHIdx1, "규격");

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주여부")].Text == "Y") //  
                    UIForm.FPMake.grdReMake(fpSpread1, i,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|5"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결정금액") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유") + "|3"
                        );
                else
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상태값")].Text == "4") // 채택
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결정금액") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유") + "|0"
                            );
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결정금액") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유") + "|3"
                            );
                    }

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Value.ToString() == "*")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|0"
                            );
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|5"
                            );
                    }
                }
                //셀병합
                if (i >= 1)
                {
                    if (fpSpread1.Sheets[0].Cells[i - 1, i1].Text == fpSpread1.Sheets[0].Cells[i, i1].Text)
                    {
                        cnt0++;

                        fpSpread1.Sheets[0].Cells[i - cnt0 + 1, i1].RowSpan = cnt0;

                        if (fpSpread1.Sheets[0].Cells[i - 1, i2].Text == fpSpread1.Sheets[0].Cells[i, i2].Text)
                        {
                            cnt1++;
                            fpSpread1.Sheets[0].Cells[i - cnt1 + 1, i2].RowSpan = cnt1;
                            fpSpread1.Sheets[0].Cells[i - cnt1 + 1, i3].RowSpan = cnt1;
                            fpSpread1.Sheets[0].Cells[i - cnt1 + 1, i4].RowSpan = cnt1;
                            fpSpread1.Sheets[0].Cells[i - cnt1 + 1, i5].RowSpan = cnt1;
                            fpSpread1.Sheets[0].Cells[i - cnt1 + 1, i6].RowSpan = cnt1;

                            fpSpread1.Sheets[0].Cells[i - cnt1 + 1, i2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i - cnt1 + 1, i3].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i - cnt1 + 1, i4].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i - cnt1 + 1, i5].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i - cnt1 + 1, i6].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        }
                        else
                        {
                            cnt1 = 1;
                        }

                        fpSpread1.Sheets[0].Cells[i - cnt0 + 1, i1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;

                    }
                    else
                    {
                        cnt0 = 1;
                    }
                }
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;
            DialogResult dsMsg;
            // 그리드 상단 필수항목 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
            {
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                //				string state = "N";
                //				string strEstNo = "", 
                //				string strItemCd = "";
                int iEstNo = 0;
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                iEstNo = SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰번호");
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

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text == "*")
                            {
                                dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0005"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.Cursor = Cursors.Default;
                                //가품목으로 채택할 수 없습니다. 품목코드를 입력하세요. 
                                return;
                            }

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text == "*")
                            {
                                dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0004"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                // 가거래처로 채택할 수 없습니다. 거래처코드를 입력하세요.
                                this.Cursor = Cursors.Default;
                                return;
                            }


                            //							if(strEstNo !=  fpSpread1.Sheets[0].Cells[i,iEstNo ].Text ) 
                            //							{
                            //								strEstNo = fpSpread1.Sheets[0].Cells[i, iEstNo].Text;
                            //								strItemCd =  fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                            //								if(fpSpread1.Sheets[0].Cells[i,SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text == "*")
                            //								{
                            //									dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0005"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //									this.Cursor = Cursors.Default;
                            //									//가품목으로 채택할 수 없습니다. 품목코드를 입력하세요. 
                            //									return;
                            //								}
                            //								for(int j = i  ; j < fpSpread1.Sheets[0].Rows.Count; j++)
                            //								{
                            //									if(strEstNo !=  fpSpread1.Sheets[0].Cells[j, iEstNo].Text && j != i)
                            //										break;
                            //									else
                            //									{
                            //										if(fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "채택")].Text =="True")	
                            //										{	
                            ////											if(Convert.ToDecimal(fpSpread1.Sheets[0].Cells[j,SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량")].Value) > Convert.ToDecimal(fpSpread1.Sheets[0].Cells[j,SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value))
                            ////											{
                            ////												dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0003"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ////												//채택수량이 요청수량보다 많습니다.
                            ////												return;
                            ////											}
                            ////											else 
                            //											if(fpSpread1.Sheets[0].Cells[j,SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text == "*")
                            //											{
                            //												dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0004"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //												// 가거래처로 채택할 수 없습니다. 거래처코드를 입력하세요.
                            //												this.Cursor = Cursors.Default;
                            //												return;
                            //											}
                            //											state = "Y"; 
                            //											break;
                            //										}
                            //										else
                            //										{ state = "N";}
                            //									}									
                            //								}
                            //							}


                            string strSql = " usp_MEA002 '" + strGbn + "'";
                            //							strSql += ", @pSTATUS = '" + state + "'";
                            //							strSql += ", @pEST_NO = '" + strEstNo + "'";

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "채택")].Text == "True")
                            {
                                strSql += ", @pEST_STATUS = '4'";
                            }
                            else
                            {
                                strSql += ", @pEST_STATUS = '9'";
                            }


                            strSql += ", @pSTATUS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적상태")].Text + "'";
                            strSql += ", @pEST_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰번호")].Text + "'";
                            strSql += ", @pEST_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적순번")].Text + "'";
                            strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "'";
                            strSql += ", @pCUST_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text + "'";
                            strSql += ", @pEST_CHOICE_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량")].Value + "'";

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Text == "")
                                strSql += ", @pEST_CHOICE_PRICE = 0";
                            else
                                strSql += ", @pEST_CHOICE_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Value + "'";

                            //							strSql += ", @pEST_CHOICE_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결정금액")].Value + "'";
                            strSql += ", @pEST_CHOICE_REMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유")].Text + "'";
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

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            try
            {
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "채택")].Text == "True")
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정금액")].Value
                            = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량")].Value.ToString())
                            * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Value.ToString());
                    }
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "채택")].Text == "True")
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정금액")].Value
                            = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량")].Value.ToString())
                            * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Value.ToString());
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region 조회조건 팝업 
        //프로젝트번호
        private void btnProj_Click(object sender, EventArgs e)
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
                    if (txtProjSeq.Text != "*") txtProjSeq.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }
        //프로젝트차수
        private void btnProjSeq_Click(object sender, EventArgs e)
        {
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
        }
        //견적담당자
        private void btnUser_Click(object sender, EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04009", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtUserId.Text = Msgs[0].ToString();
                    txtUserNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }
        //품목
        private void butItem_Click(object sender, EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW005 pu = new WNDW005(SystemBase.Base.gstrPLANT_CD, true, txtItemCd.Text);
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
            }
            strBtn = "N";

        }
        //거래처
        private void butSCust_Click(object sender, EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtSCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSCustCd.Text = Msgs[1].ToString();
                    txtSCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBtn = "N";
        }
        //견적의뢰번호
        private void btnEstNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW017 pu = new WNDW017();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtEstNo.Text = Msgs[1].ToString();
                }
                //MEA002P7 frm1 = new MEA002P7();
                //frm1.ShowDialog();
                //if (frm1.DialogResult == DialogResult.OK)
                //{
                //    txtEstNo.Text = frm1.ReturnVal;
                //}
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //구매요청번호
        private void btnReqNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW016 pu = new WNDW016();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtReqNo.Text = Msgs[1].ToString();
                }

                //MEA002P6 frm1 = new MEA002P6();
                //frm1.ShowDialog();
                //if (frm1.DialogResult == DialogResult.OK)
                //{
                //    txtReqNo.Text = frm1.ReturnVal;
                //}
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //거래처2
        private void butCust_Click(object sender, EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBtn = "N";
        }
        #endregion

        #region 조회조건 TextChanged  
        //품목
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            if (strBtn == "N")
                txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        //프로젝트번호
        private void txtProjNo_TextChanged(object sender, EventArgs e)
        {
            if (strBtn == "N")
            {
                if (txtProjSeq.Text != "*") txtProjSeq.Text = "";
            }
        }
        private void txtUserId_TextChanged(object sender, EventArgs e)
        {
            if (strBtn == "N" && txtUserId.Text.Trim() != "")
            {
                string temp = "";
                temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtUserId.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                if (temp != "")
                    txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                else
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0001"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //구매담당자가 아닙니다
                    txtUserId.Text = "";
                    txtUserNm.Value = "";
                    txtUserId.Focus();
                }
            }
        }
        //거래처
        private void txtSCustCd_TextChanged(object sender, EventArgs e)
        {
            if (strBtn == "N")
                txtSCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        //거래처2
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            if (strBtn == "N")
                txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }  
        #endregion  

        private void MEA002_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpEstDtFr.Focus();
        }

        private void MEA002_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }

        #region 멀티 소트
        private void Grid_Sort()
        {
            int SortNb1 = SystemBase.Base.GridHeadIndex(GHIdx1, cboSort1.SelectedText.ToString());
            int SortNb2 = SystemBase.Base.GridHeadIndex(GHIdx1, cboSort2.SelectedText.ToString());
            int SortNb3 = SystemBase.Base.GridHeadIndex(GHIdx1, cboSort3.SelectedText.ToString());

            fpSpread1.Sheets[0].SortRows(0, fpSpread1.Sheets[0].RowCount,
                                        new FarPoint.Win.Spread.SortInfo[]{new FarPoint.Win.Spread.SortInfo(SortNb1, true), 
																		   new FarPoint.Win.Spread.SortInfo(SortNb2, true),
																		   new FarPoint.Win.Spread.SortInfo(SortNb3, true)});
        }
        private void cboSort1_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            Grid_Sort();
        }
        private void cboSort2_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            Grid_Sort();
        }

        private void cboSort3_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            Grid_Sort();
        }
        #endregion	

        #region 그리드 버튼 클릭
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            int Column = e.Column;
            int Row = e.Row;

            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "거래처_2") && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text == "*")
            {

                try
                {
                    WNDW002 pu = new WNDW002("P");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명")].Text = Msgs[2].ToString();
                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text == "*")
            {
                try
                {
                    WNDW005 pu = new WNDW005(SystemBase.Base.gstrPLANT_CD, true);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[7].ToString();

                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "요구품질증빙_2"))
            {
                MEA002P1 frm1 = new MEA002P1(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "견적번호")].Text, "1", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "견적순번")].Text);
                frm1.ShowDialog();

            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "가능품질증빙_2"))
            {
                MEA002P1 frm2 = new MEA002P1(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "견적번호")].Text, "2", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "견적순번")].Text, fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text);
                frm2.ShowDialog();
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "채택"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "채택")].Text == "True")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량")].Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제출수량")].Value;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제출단가")].Value;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정금액")].Value
                        = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량")].Value.ToString())
                        * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Value.ToString());

                    UIForm.FPMake.grdReMake(fpSpread1, Row,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가") + "|1");

                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text == "*")
                        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "거래처_2") + "|0");
                }
                else
                {


                    UIForm.FPMake.grdReMake(fpSpread1, Row,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가") + "|3");

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량")].Value = 0;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Value = null;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정금액")].Value = null;
                    //					UIForm.FPMake.grdReMake(fpSpread1, Row, 
                    //						SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유") + "|3"
                    //						+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가") + "|3"
                    //						+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "거래처") + "|3"
                    //						+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "거래처_2") + "|5"
                    //						);	

                    //제출마감을 체크할수 있게 한다
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "견적번호")].Text == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적번호")].Text
                            && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "견적순번")].Text == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적순번")].Text
                            && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상태값")].Text == "9")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "채택") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택단가") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유") + "|3");
                        }
                    }

                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "상태값")].Text == "3" || fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "상태값")].Text == "9")
                        fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text = "";
                }
            }
        }
        #endregion

        private void rdoSelect_CheckedChanged(object sender, System.EventArgs e)
        {
            rdoSelect_CheckedChanged();
        }

        private void rdoSelect_CheckedChanged()
        {
            if (rdoSelectN.Checked == true)
            {
                btnAutoSelect.Visible = true;
                cboSort1.Enabled = false;
                cboSort2.Enabled = false;
                cboSort3.Enabled = false;
            }
            else
            {
                btnAutoSelect.Visible = false;
                cboSort1.Enabled = true;
                cboSort2.Enabled = true;
                cboSort3.Enabled = true;
            }
        }

        #region 거래처 일괄적용
        private void btnCustAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCustCd.Text.Trim() == "")
                {
                    MessageBox.Show("거래처를 입력하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCustCd.Focus();
                }
                int col_cust = SystemBase.Base.GridHeadIndex(GHIdx1, "거래처");
                int col_sel = SystemBase.Base.GridHeadIndex(GHIdx1, "채택");
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, col_cust].Text == txtCustCd.Text)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, col_sel].Locked == false)
                        {
                            fpSpread1.Sheets[0].Cells[i, col_sel].Value = 1;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량")].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출수량")].Value;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출단가")].Value;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결정금액")].Value
                                = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량")].Value.ToString())
                                * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Value.ToString());

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text == "*")
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "거래처") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "거래처_2") + "|0"
                                    );
                            else
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가") + "|1"
                                                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유") + "|0");
                        }
                        UIForm.FPMake.fpChange(fpSpread1, i);//수정플래그 
                    }
                    else
                    {
                        if (fpSpread1.Sheets[0].Cells[i, col_sel].Locked == false && fpSpread1.Sheets[0].Cells[i, col_sel].Text == "True")
                        {
                            fpSpread1.Sheets[0].Cells[i, col_sel].Value = 0;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량")].Value = 0;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Value = 0;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결정금액")].Value = 0;
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "거래처") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "거래처_2") + "|5"
                                );
                            UIForm.FPMake.fpChange(fpSpread1, i);//수정플래그 
                        }
                    }
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 자동선택
        private void btnAutoSelect_Click(object sender, EventArgs e)
        {
            if (fpSpread1.Sheets[0].RowCount < 0) return;
            //행수만큼 처리
            string temp_EstNo = fpSpread1.Sheets[0].Cells[0, 1].Text;
            string temp_EstSeq = fpSpread1.Sheets[0].Cells[0, 2].Text;
            decimal temp_price = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "제출단가")].Value.ToString());
            int sel_row = 0;

            int col_sel = SystemBase.Base.GridHeadIndex(GHIdx1, "채택");

            for (int i = 1; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {

                if (temp_EstNo == fpSpread1.Sheets[0].Cells[i, 1].Text && temp_EstSeq == fpSpread1.Sheets[0].Cells[i, 2].Text)
                {
                    if (temp_price > Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출단가")].Value.ToString()))
                        sel_row = i;

                }
                else
                {
                    fpSpread1.Sheets[0].Cells[sel_row, col_sel].Value = 1;
                    fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량")].Value = fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "제출수량")].Value;
                    fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Value = fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "제출단가")].Value;
                    fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정금액")].Value
                        = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량")].Value.ToString())
                        * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Value.ToString());

                    if (fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text == "*")
                        UIForm.FPMake.grdReMake(fpSpread1, sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "거래처") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "거래처_2") + "|0"
                            );
                    else
                        UIForm.FPMake.grdReMake(fpSpread1, sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유") + "|0");

                    UIForm.FPMake.fpChange(fpSpread1, sel_row);//수정플래그 
                    temp_price = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출단가")].Value.ToString());
                    sel_row = i;

                }

                temp_EstNo = fpSpread1.Sheets[0].Cells[i, 1].Text; //견적번호
                temp_EstSeq = fpSpread1.Sheets[0].Cells[i, 2].Text; //견적순번
            }

            fpSpread1.Sheets[0].Cells[sel_row, col_sel].Value = 1;
            fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량")].Value = fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "제출수량")].Value;
            fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Value = fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "제출단가")].Value;
            fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정금액")].Value
                = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "채택수량")].Value.ToString())
                * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Value.ToString());

            if (fpSpread1.Sheets[0].Cells[sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text == "*")
                UIForm.FPMake.grdReMake(fpSpread1, sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유") + "|0"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "거래처") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "거래처_2") + "|0"
                    );
            else
                UIForm.FPMake.grdReMake(fpSpread1, sel_row, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유") + "|0");

            UIForm.FPMake.fpChange(fpSpread1, sel_row);//수정플래그 
        }
        #endregion        

    }
}
