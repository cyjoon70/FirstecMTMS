#region 작성정보
/*********************************************************************/
// 단위업무명 : 외주공정실적등록/취소
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-08
// 작성내용 : 외주공정실적등록/취소 및 관리
// 수 정 일 : 2014-08-05
// 수 정 자 : 최 용 준
// 수정내용 : 품질요구사항 조회 추가
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

namespace MO.MIM518
{
    public partial class MIM518 : UIForm.FPCOMM2
    {

        #region 변수선언
        string strWoNo = "";
        string strProcSeq = "";
        string strInspFlg = "";
		string strKey = "";	//조회 내부키
		string strSCM_MVMT_NO = "";
		int Row = 0;
        #endregion

        #region 생성자
        public MIM518()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void MIM518_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;

            dtpResultDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(-6).ToShortDateString().Substring(0,10);
            dtpResultDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(6).ToShortDateString().Substring(0,10);
            dtpResultDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정단계")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공정단계
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "VAT유형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT유형
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "통화")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//화폐단위


            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

            SystemBase.ComboMake.C1Combo(cboStatus, "usp_P_COMMON @pTYPE = 'P150' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시상태
            SystemBase.ComboMake.C1Combo(cboOrderFlag, "usp_B_COMMON @pType='COMM', @pCODE = 'P026', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시구분	
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD.ToString();

            dtpResultDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(-6).ToShortDateString();
            dtpResultDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(6).ToShortDateString();
            dtpResultDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            rdoNo.Checked = true;
            SystemBase.ComboMake.C1Combo(cboStatus, "usp_P_COMMON @pTYPE = 'P150' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시상태
            SystemBase.ComboMake.C1Combo(cboOrderFlag, "usp_B_COMMON @pType='COMM', @pCODE = 'P026', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시구분

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

            txtScmMvmtNo.Text = "";      //2017.02.14 SCM번호 초기화
			strSCM_MVMT_NO = "";
        }
        #endregion

        #region 조회조건 팝업
        //공장
        private void btnPlant_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P013', @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };											  // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtPlantCd.Text, "" };															  // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtPlantCd.Value = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();
                    txtPlantCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제조오더번호
        private void btnWorkOrderNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNo.Value = Msgs[1].ToString();
                    txtWorkOrderNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //사업
        private void btnEnt_Click(object sender, System.EventArgs e)
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

        //프로젝트번호
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1", "S");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtEntCd.Value = Msgs[1].ToString();
                    txtEntNm.Value = Msgs[2].ToString();
                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Value = Msgs[5].ToString();
                    txtGroupCd.Value = Msgs[6].ToString();
                    txtGroupNm.Value = Msgs[7].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목코드
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtItemCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Value = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    txtItemCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //거래처
        private void btnBp_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtBpCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtBpCd.Value = Msgs[1].ToString();
                    txtBpNm.Value = Msgs[2].ToString();
                    txtBpCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPoNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_MIM518 @pTYPE = 'P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pOUT_PO_NO", "" };
                string[] strSearch = new string[] { txtPoNo.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00078", strQuery, strWhere, strSearch, new int[] { 0 }, "발주번호 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPoNo.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발주번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnScmRef_Click(object sender, System.EventArgs e)
        {

            try
            {
                MIM518P5 frm1 = new MIM518P5();
                frm1.ShowDialog();

                if (frm1.DialogResult == DialogResult.OK)
                {
                    NewExec();

                    string[] Msgs = frm1.ReturnVal;
                    
					if (Msgs != null)
                    {
                        txtBpCd.Value = Msgs[0].ToString();

                        txtPlantCd.Value = Msgs[1].ToString();

                        dtpResultDtFr.Value = Msgs[2].ToString();
                        dtpResultDtTo.Value = Msgs[3].ToString();

                        rdoNo.Checked = true;
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

        #region 코드 입력시 코드명 자동 입력
        //공장
        private void txtPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPlantCd.Text != "")
                {
                    txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPlantNm.Value = "";
                }
            }
            catch
            {

            }
        }
        //사업
        private void txtEntCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEntCd.Text != "")
                {
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
                if (txtProjectNm.Text == "")
                {
                    txtEntCd.Value = "";
                    txtEntNm.Value = "";
                    txtProjectNm.Value = "";
                    txtProjectSeq.Value = "";
                    txtGroupCd.Value = "";
                    txtGroupNm.Value = "";
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
        //거래처
        private void txtBpCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtBpCd.Text != "")
                {
                    txtBpNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtBpCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtBpNm.Value = "";
                }
            }
            catch
            {

            }
        }
        //제품코드
        private void txtGroupCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtGroupCd.Text != "")
                {
                    txtGroupNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtGroupCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtGroupNm.Value = "";
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
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            Search(0, true);

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void Search(int Row, bool Msg)
        {
			
			strSCM_MVMT_NO = "";

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strResultYn = "";

                    if (rdoYes.Checked == true) { strResultYn = "Y"; }
                    else if (rdoNo.Checked == true) { strResultYn = "N"; }

                    string strMQuery = "";
                    strMQuery = " usp_MIM518 @pTYPE = 'S1'";
                    strMQuery += ",     @pRESULT_DT_FR = '" + dtpResultDtFr.Text + "' ";
                    strMQuery += ",     @pRESULT_DT_TO = '" + dtpResultDtTo.Text + "' ";
                    strMQuery += ",     @pENT_CD = '" + txtEntCd.Text + "' ";
                    strMQuery += ",     @pWORKORDER_NO = '" + txtWorkOrderNo.Text + "' ";
                    strMQuery += ",     @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strMQuery += ",     @pBP_CD = '" + txtBpCd.Text + "' ";
                    strMQuery += ",     @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                    strMQuery += ",     @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strMQuery += ",     @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strMQuery += ",     @pORDER_STATUS = '" + cboStatus.SelectedValue.ToString() + "' ";
                    strMQuery += ",     @pRESULT_YN = '" + strResultYn + "' ";
                    strMQuery += ",     @pPROC_SEQ = '" + txtProcSeq.Text + "' ";
                    strMQuery += ",     @pORDER_FLAG = '" + cboOrderFlag.SelectedValue.ToString() + "' ";
                    strMQuery += ",     @pOUT_PO_NO = '" + txtPoNo.Text + "' ";
                    strMQuery += ",     @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strMQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, Msg, 0, 0);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                        {
                            if (Convert.ToInt16(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "미입고량")].Value) <= 0
                                || fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "오더상태")].Value.ToString() == "CL")
                            {
                                UIForm.FPMake.grdReMake(fpSpread2, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량") + "|3");
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread2, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량") + "|3");
                            }
                        }


                        int x = 0, y = 0;

                        if (strKey != "")
                        {
                            fpSpread2.Search(0, strKey, false, false, false, false, 0, 0, ref x, ref y);

                            if (x > 0)
                            {
                                fpSpread2.Sheets[0].SetActiveCell(x, 1);
                                fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                            }
                            else
                            {
                                x = 0;
                            }
                        }

                        fpSpread2.Sheets[0].AddSelection(x, 1, 1, fpSpread2.Sheets[0].ColumnCount);

                        //상세정보조회
                        SubSearch(x);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0;
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
        
        #region SaveExec2() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec2()
        {
            FarPoint.Win.Spread.FpSpread grid = null;

            //그리드 상단 필수 체크

            if (fpSpread1.Focused == true)
            {
                grid = fpSpread1;
            }
            else
            {
                grid = fpSpread2;
            }

            grid.Focus();



            if (SystemBase.Validation.FPGrid_SaveCheck(grid, this.Name, grid.Name, true))
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);


                // SCM번호가 있으면 해당 발주가 SCM번호와 일치하는지 검토 해줌. 2017.02.14 
                if (string.IsNullOrEmpty(txtScmMvmtNo.Text) == false )
                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text;
                        if (strHead.Length > 0)
                        {
                            string strSqlChk = " usp_MIM518 'S6'";
                            strSqlChk += ", @pSCM_MVMT_NO = '" + txtScmMvmtNo.Text + "' ";
                            strSqlChk += ", @pOUT_PO_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "발주번호")].Text + "' ";
                            strSqlChk += ", @pOUT_PO_SEQ = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "발주순번")].Value + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSqlChk, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();
                        
                            if (ERRCode == "CHK")
                            {
                                //fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                                Trans.Rollback();
                                goto Exit;
                            }
                        }
                    }
                //---------------------------------------------------------  2017.02.14 

                try
                {
                    if (grid == fpSpread2)
                    {
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text;
							int iSeq = 0; // 공정실적  seq

                            double GoodQty = 0;
                            GoodQty = Convert.ToDouble(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Value);

                            if (strHead.Length > 0)
                            {
                                string strSql = " usp_MIM518 'U1'";
                                strSql += ", @pWORKORDER_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text + "' ";
                                strSql += ", @pPROC_SEQ = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text + "' ";
                                if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "미입고량")].Text != "")
                                { strSql += ", @pPROC_ORDER_QTY = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "미입고량")].Value + "' "; }
                                else
                                { strSql += ", @pPROC_ORDER_QTY = '0' "; }
                                if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Text != "")
                                { strSql += ", @pINSP_GOOD_QTY_IN_ORDER_UNIT = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Value + "' "; }
                                else
                                { strSql += ", @pINSP_GOOD_QTY_IN_ORDER_UNIT = '0' "; }
                                if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량")].Text != "")
                                { strSql += ", @pINSP_BAD_QTY_IN_ORDER_UNIT = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량")].Value + "' "; }
                                else
                                { strSql += ", @pINSP_BAD_QTY_IN_ORDER_UNIT = '0'"; }
                                strSql += ", @pROUT_ORDER = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정단계")].Value + "' ";
                                strSql += ", @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                                strSql += ", @pITEM_CD = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text + "' ";
                                strSql += ", @pE_ITEM = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제품코드")].Text + "' ";
                                strSql += ", @pMILESTONE_FLG = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "마일스톤여부")].Text + "' ";
                                strSql += ", @pPROJECT_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text + "' ";
                                strSql += ", @pPROJECT_SEQ = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text + "' ";
                                strSql += ", @pINSP_FLG = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정검사여부")].Text + "' ";
                                strSql += ", @pMAKEORDER_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제품오더번호")].Text + "' ";
                                strSql += ", @pDEPT_CD = '" + SystemBase.Base.gstrDEPT.ToString() + "' ";
                                strSql += ", @pREORG_ID = '" + SystemBase.Base.gstrREORG_ID.ToString() + "' ";
                                strSql += ", @pWC_CD = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Text + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "' ";
                                strSql += ", @pBP_CD = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처코드")].Text + "' ";
                                strSql += ", @pSUBCONTRACT_PRC = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주단가")].Value + "' ";
                                strSql += ", @pSUBCONTRACT_AMT = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주금액")].Value + "' ";
                                strSql += ", @pCUR_CD = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Value + "' ";
                                strSql += ", @pVAT_TYPE = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "VAT유형")].Value + "' ";
                                strSql += ", @pOUT_PO_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "발주번호")].Text + "' ";
                                strSql += ", @pOUT_PO_SEQ = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "발주순번")].Value + "' ";
                                strSql += ", @pRESULT_DT = '" + dtpResultDt.Text + "' ";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

								if (string.IsNullOrEmpty(strSCM_MVMT_NO) == true)
								{
									strSCM_MVMT_NO = txtScmMvmtNo.Text;
								}

								strSql += ", @pSCM_MVMT_NO = '" + strSCM_MVMT_NO + "' ";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

								if (ERRCode != "OK")
								{
									fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "";
									Trans.Rollback();
									goto Exit;
								}	 // ER 코드 Return시 점프


								#region 증빙문서 처리

								if (string.Compare(ERRCode, "OK", true) == 0)
								{
									iSeq = Convert.ToInt32(ds.Tables[0].Rows[0][2]);

									strSql = string.Empty;

									strSql = "usp_T_DOC ";
									strSql += " @pTYPE			= 'U_SCM' ";
									strSql += ",@pCO_CD			= '" + SystemBase.Base.gstrCOMCD + "' ";
									strSql += ",@pPLANT_CD		= '" + SystemBase.Base.gstrPLANT_CD + "' ";
									strSql += ",@pDOC_CTG_CD	= 'SOUT' "; // 'SPUR', 'SOUT'

									strSql += ",@pATT_KEY1	= '" + strSCM_MVMT_NO + "' ";
									strSql += ",@pATT_KEY2	= '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "발주번호")].Text + "' ";
									strSql += ",@pATT_KEY3	= '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "발주순번")].Text + "' ";

									strSql += ",@pNEW_KEY1	= '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text + "' ";
									strSql += ",@pNEW_KEY2	= '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text + "' ";
									strSql += ",@pNEW_KEY3	= '" + iSeq.ToString() + "' ";

									strSql += ",@pUP_ID	= '" + SystemBase.Base.gstrUserID + "' ";

									DataSet dsDoc = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
									ERRCode = dsDoc.Tables[0].Rows[0][0].ToString();
									MSGCode = dsDoc.Tables[0].Rows[0][1].ToString();
									if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

								}

								#endregion

							}
                        }
                    }
                    else if (grid == fpSpread1)
                    {
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                            if (strHead.Length > 0)
                            {
                                string strSql = " usp_MIM518 'D1'";
                                strSql += ", @pWORKORDER_NO = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text + "' ";
                                strSql += ", @pPROC_SEQ = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text + "' ";
                                strSql += ", @pINSP_FLG = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "공정검사여부")].Text + "' ";
                                strSql += ", @pSEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "보고횟수")].Text + "' ";
                                strSql += ", @pINSP_GOOD_QTY_IN_ORDER_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사양품수량")].Value + "' ";
                                strSql += ", @pINSP_BAD_QTY_IN_ORDER_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사불량수량")].Value + "' ";
                                strSql += ", @pINSP_REQ_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "' ";
                                strSql += ", @pOUT_PO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text + "' ";
                                strSql += ", @pOUT_PO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Value + "' ";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
								strSql += ", @pSCM_MVMT_NO = '" + fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "SCM 입고번호")].Text + "' ";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK")
                                {
                                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                                    Trans.Rollback();
                                    goto Exit;
                                }	// ER 코드 Return시 점프
                            }
                        }
                    }
                    else
                    {
                        Trans.Rollback();
                        ERRCode = "ER";
                        MSGCode = "선택된 그리드가 없습니다.";
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

                if (ERRCode == "OK")
                {
                    Search(Row, false);
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
        }
        #endregion

        #region fpSpread2 Select 이벤트
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                Row = fpSpread2.Sheets[0].ActiveRowIndex;

                strKey = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "내부키")].Text;

                strWoNo = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text;
                strProcSeq = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text;
                strInspFlg = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정검사여부")].Text;

                SubSearch(Row);
            }
            else
            {
                Row = 0;
            }
        }
        #endregion

        #region 실적조회
        private void SubSearch(int Row)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strMQuery = "";
                    strMQuery = "   usp_MIM518 @pTYPE = 'S2'";
                    strMQuery += ",            @pWORKORDER_NO = '" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text + "' ";
                    strMQuery += ",            @pPROC_SEQ = '" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text + "' ";
                    strMQuery += ",            @pOUT_PO_NO = '" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "발주번호")].Text + "' ";
                    strMQuery += ",            @pOUT_PO_SEQ = '" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "발주순번")].Value + "' ";
                    strMQuery += ",            @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    
					UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

					if (fpSpread1.Sheets[0].Rows.Count > 0)
					{ 
						fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1,"품질증빙")].Locked = false;
					}

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 그리드 체인지시 update 체크
        private void fpSpread2_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            UIForm.FPMake.fpChange(fpSpread2, e.Row);
        }
        #endregion

        #region 그리드2 체인지시 이벤트
        private void fpSpread2_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량"))
            {
                if ((Convert.ToDouble(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Value) == 0.00 &&
                    Convert.ToDouble(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량")].Value) == 0.00) ||
                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Text == "" &&
                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량")].Text == "")
                {
                    fpSpread2.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";

                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Value = 0;
                }
            }
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, " 불량수량"))
            {
                if ((Convert.ToDouble(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Value) == 0.00 &&
                    Convert.ToDouble(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량")].Value) == 0.00) ||
                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량")].Text == "" &&
                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량")].Text == "")
                {
                    fpSpread2.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";

                    fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량")].Value = 0;
                }
            }
        }
        #endregion

        #region 공정진행현황
        private void btnProcInfo_Click(object sender, System.EventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                int Row = fpSpread2.Sheets[0].ActiveRowIndex;

                string ProjectNo = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text;
                string ProjectSeq = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text;
                string ItemCd = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text;
                string WoNo = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text;

                MIM518P1 myForm = new MIM518P1(ProjectNo, ProjectSeq, ItemCd, WoNo);
                myForm.ShowDialog();
            }
        }
        #endregion

        #region 부품내역
        private void btnItemSpec_Click(object sender, System.EventArgs e)
        {
            if (strWoNo == "")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0061", "제조오더번호"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MIM518P2 form = new MIM518P2(strWoNo, strProcSeq);
            form.ShowDialog();
        }
        #endregion
         
        #region scm번호, 자료가져오기
        private void btnScmMvmtNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                MIM518P6 frm1 = new MIM518P6();
                frm1.ShowDialog();
                if (frm1.DialogResult == DialogResult.OK)
                {
                    string Msgs = frm1.ReturnVal;
                    txtScmMvmtNo.Value = Msgs;
					strSCM_MVMT_NO = Msgs;
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
		
        private void btnScm_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtScmMvmtNo.Text.Trim() != "")
                {
                    string strMQuery = "";
                    strMQuery = "   usp_MIM518 @pTYPE = 'S5'";
                    strMQuery += ", @pSCM_MVMT_NO = '" + txtScmMvmtNo.Text + "'";
                    strMQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strMQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                        {
                            if (Convert.ToInt16(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "미입고량")].Value) <= 0
                                || fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "오더상태")].Value.ToString() == "CL")
                            {
                                UIForm.FPMake.grdReMake(fpSpread2, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량") + "|3");
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread2, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량") + "|3");
                            }
                            fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                        }


                        int x = 0, y = 0;

                        if (strKey != "")
                        {
                            fpSpread2.Search(0, strKey, false, false, false, false, 0, 0, ref x, ref y);

                            if (x > 0)
                            {
                                fpSpread2.Sheets[0].SetActiveCell(x, 1);
                                fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                            }
                            else
                            {
                                x = 0;
                            }
                        }

                        fpSpread2.Sheets[0].AddSelection(x, 1, 1, fpSpread2.Sheets[0].ColumnCount);

                        //상세정보조회
                        SubSearch(x);

                        dtpResultDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);  //2017.02.14 실적일은 현재일자로(SCM 출고일로 하지말고)
                        //dtpResultDt.Value = txtScmMvmtNo.Text.Substring(2, 4) + "-" + txtScmMvmtNo.Text.Substring(6, 2) + "-" + txtScmMvmtNo.Text.Substring(8, 2);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0;
                    }
                }
                else
                {
                    MessageBox.Show("SCM번호를 입력하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtScmMvmtNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 사용안함
        private void Search(bool div)
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strResultYn = "";

                    if (rdoYes.Checked == true) { strResultYn = "Y"; }
                    else if (rdoNo.Checked == true) { strResultYn = "N"; }

                    string strMQuery = "";
                    strMQuery = "   usp_MIM518 @pTYPE = 'S5'";
                    strMQuery += ",            @pRESULT_DT_FR = '" + dtpResultDtFr.Text + "' ";
                    strMQuery += ",            @pRESULT_DT_TO = '" + dtpResultDtTo.Text + "' ";
                    strMQuery += ",            @pENT_CD = '" + txtEntCd.Text + "' ";
                    strMQuery += ",            @pWORKORDER_NO = '" + txtWorkOrderNo.Text + "' ";
                    strMQuery += ",            @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strMQuery += ",            @pBP_CD = '" + txtBpCd.Text + "' ";
                    strMQuery += ",            @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                    strMQuery += ",            @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strMQuery += ",            @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strMQuery += ",            @pORDER_STATUS = '" + cboStatus.SelectedValue.ToString() + "' ";
                    strMQuery += ",            @pRESULT_YN = '" + strResultYn + "' ";
                    strMQuery += ",            @pPROC_SEQ = '" + txtProcSeq.Text + "' ";
                    strMQuery += ",            @pORDER_FLAG = '" + cboOrderFlag.SelectedValue.ToString() + "' ";
                    strMQuery += ",            @pOUT_PO_NO = '" + txtPoNo.Text + "' ";
                    strMQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strMQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, div, 0, 0);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                        {
                            if (Convert.ToInt16(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "미입고량")].Value) <= 0
                                || fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "오더상태")].Value.ToString() == "CL")
                            {
                                UIForm.FPMake.grdReMake(fpSpread2, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량") + "|3");
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread2, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx2, "양품수량") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "불량수량") + "|3");
                            }
                            fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                        }


                        int x = 0, y = 0;

                        if (strKey != "")
                        {
                            fpSpread2.Search(0, strKey, false, false, false, false, 0, 0, ref x, ref y);

                            if (x > 0)
                            {
                                fpSpread2.Sheets[0].SetActiveCell(x, 1);
                                fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                            }
                            else
                            {
                                x = 0;
                            }
                        }

                        fpSpread2.Sheets[0].AddSelection(x, 1, 1, fpSpread2.Sheets[0].ColumnCount);

                        //상세정보조회
                        SubSearch(x);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0;
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

		#region 품질증빙 확인
		private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{
			try
			{

				WNDW037 pu = new WNDW037();
				pu.strWORKORDER_NO = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text;
				pu.strPROC_SEQ = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text;
				pu.strSEQ = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "보고횟수")].Text;
				pu.strFormGubn = "MIM518";

				pu.ShowDialog();

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

	}
}
