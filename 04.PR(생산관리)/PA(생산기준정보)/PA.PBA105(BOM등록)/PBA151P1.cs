using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using SystemBase;

namespace PA.PBA151
{
    public partial class PBA151P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strItem_Cd = "", strItem_Nm = "", strRev_No = "", strPRNT_BOM_NO = "", strChild = "";
        string strBOM_DEV_USR_ID = "", strBOM_MFG_USR_ID = "", strBOM_QUR_USR_ID = "", strBOM_APP_USR_ID = "";
        string strMATR_URWG;
        bool chk;
        #endregion

        #region 생성자
        public PBA151P1(string Item_Cd, string Item_Nm, string Rev_No, string PRNT_BOM_NO, string BOM_DEV_USR_ID, string BOM_MFG_USR_ID, string BOM_QUR_USR_ID, string BOM_APP_USR_ID, string Child, string MATR_URWG, bool CHK)
        {
            strItem_Cd = Item_Cd;
            strItem_Nm = Item_Nm;
            strRev_No = Rev_No;
            strPRNT_BOM_NO = PRNT_BOM_NO;
            strBOM_DEV_USR_ID = BOM_DEV_USR_ID;
            strBOM_MFG_USR_ID = BOM_MFG_USR_ID;
            strBOM_QUR_USR_ID = BOM_QUR_USR_ID;
            strBOM_APP_USR_ID = BOM_APP_USR_ID;
            strChild = Child;
            strMATR_URWG = MATR_URWG;
            chk = CHK;

            InitializeComponent();
        }
        public PBA151P1()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드시
        private void PBA151P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "BOM등록(리비젼)";

            //버튼 재정의
            UIForm.Buttons.ReButton("000000010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.ComboMake.C1Combo(cboPrnt_Bom_No, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P006', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "'", 0);	// BOM TYPE

            G1Etc[6] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[15] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "'");

            txtItem_Cd.Value = strItem_Cd;				//품목코드
            txtItem_Nm.Value = strItem_Nm;				//품목명
            if (strRev_No != "")
                txtRevNo.Value = strRev_No;					//리비전번호
            else
                txtRevNo.Value = "0";

            cboPrnt_Bom_No.SelectedValue = strPRNT_BOM_NO;	//BOM TYPE
            dtxtMatrUrwg.Text = strMATR_URWG;

            dtpRevDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpMkDt.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpMfDt.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpQcDt.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpPrDt.Value = SystemBase.Base.ServerTime("YYMMDD");

            txtMk_Id.Value = strBOM_DEV_USR_ID;			//작성자
            txtMf_Id.Value = strBOM_MFG_USR_ID;			//생산검토자
            txtQc_Id.Value = strBOM_QUR_USR_ID;			//품질검토자
            txtPr_Id.Value = strBOM_APP_USR_ID;			//승인자
            
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            
            SearchExec();
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            Regex rx1 = new Regex("!!");
            string[] Msgs = rx1.Split(strChild);

            int chk = 0;

            for (int i = 0; i < Msgs.Length; i++)
            {
                string strQuery = "";
                strQuery += " usp_PBA151 'S4' ";
                strQuery += " , @pPLANT_CD='" + SystemBase.Base.gstrPLANT_CD + "'";
                strQuery += " , @pCHILD_ITEM_CD ='" + txtItem_Cd.Text + "'";
                strQuery += " , @pCHILD_BOM_NO ='" + cboPrnt_Bom_No.SelectedValue + "'";
                strQuery += " , @pCHILD_SEQ='" + Msgs[i].ToString() + "'";
                strQuery += " , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    RowInsExec();

                    fpSpread1.Sheets[0].Cells[i, 0].Text = dt.Rows[0]["CHILD_ITEM_SEQ"].ToString();		//자품목순번
                    fpSpread1.Sheets[0].Cells[i, 1].Text = dt.Rows[0]["CHILD_SEQ"].ToString();			//정렬순번
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전")].Text = dt.Rows[0]["CHILD_ITEM_CD"].ToString();		//품목코드
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_2")].Text = dt.Rows[0]["ITEM_NM"].ToString();			//품목명
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_3")].Text = dt.Rows[0]["ITEM_SPEC"].ToString();			//규격

                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_4")].Text = dt.Rows[0]["CHILD_ITEM_QTY"].ToString();		//자품목기준수
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_5")].Value = dt.Rows[0]["CHILD_ITEM_UNIT"].ToString();	//단위
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_6")].Text = dt.Rows[0]["MAT_SIZE"].ToString();			//재료규격
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_7")].Text = dt.Rows[0]["VALID_FROM_DT"].ToString();		//유효일자(FROM)
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_8")].Text = dt.Rows[0]["VALID_TO_DT"].ToString();		//유효일자(TO)
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_9")].Text = dt.Rows[0]["MATR_CNQY"].ToString();			//원소재량(Kg)
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_10")].Text = dt.Rows[0]["Q_FIG_NO"].ToString();			//품질FIGNO

                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후")].Text = dt.Rows[0]["CHILD_ITEM_CD"].ToString();		//품목코드
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_3")].Text = dt.Rows[0]["ITEM_NM"].ToString();			//품목명
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_4")].Text = dt.Rows[0]["ITEM_SPEC"].ToString();			//규격
                                                                                        
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_5")].Text = dt.Rows[0]["CHILD_ITEM_QTY"].ToString();	//자품목기준수
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_6")].Value = dt.Rows[0]["CHILD_ITEM_UNIT"].ToString();	//단위
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_7")].Text = dt.Rows[0]["MAT_SIZE"].ToString();			//재료규격
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_8")].Text = dt.Rows[0]["VALID_FROM_DT"].ToString();		//유효일자(FROM)
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_9")].Text = dt.Rows[0]["VALID_TO_DT"].ToString();		//유효일자(TO)
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_10")].Text = dt.Rows[0]["MATR_CNQY"].ToString();			//원소재량(Kg)
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_11")].Text = dt.Rows[0]["Q_FIG_NO"].ToString();			//품질FIGNO

                    if (dt.Rows[0]["CHILD_ITEM_CD"].ToString().Substring(0, 2) == "PA" || dt.Rows[0]["CHILD_ITEM_CD"].ToString().Substring(0, 2) == "VA")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_10") + "|1");//원소재량(Kg)

                        chk++;
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_10") + "|0");//원소재량(Kg)
                    }
                }
            }

            if (chk > 0)
            {
                dtxtMatrUrwg.Tag = "실제품량;1;;";
                dtxtMatrUrwg.BackColor = SystemBase.Validation.Kind_LightCyan;
                dtxtMatrUrwg.ReadOnly = false;
            }
            else
            {
                dtxtMatrUrwg.Tag = "";
                dtxtMatrUrwg.BackColor = SystemBase.Validation.Kind_White;
                dtxtMatrUrwg.ReadOnly = false;
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                string tempRevNo = "";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_PBA151 ";
                    strSql = strSql + " @pType = 'I1'";

                    strSql += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";	//공장코드
                    strSql += ", @pPRNT_BOM_NO = '" + cboPrnt_Bom_No.SelectedValue.ToString().TrimEnd() + "' ";	//BOM TYPE
                    strSql += ", @pITEM_CD = '" + txtItem_Cd.Text.TrimEnd() + "' ";			//품목코드
                    strSql += ", @pREV_NO = '" + txtRevNo.Text.TrimEnd() + "' ";			//리비전번호
                    strSql += ", @pREVISION_DATE = '" + dtpRevDt.Text.TrimEnd() + "' ";		//변경일자
                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "' ";
                    strSql += ", @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "'";
                    strSql += ", @pMATR_URWG = '" + dtxtMatrUrwg.Value + "' ";				//원소재량

                    strSql += ", @pREVISION_MK_ID = '" + txtMk_Id.Text.TrimEnd() + "' ";			//작성자
                    strSql += ", @pREVISION_MF_ID = '" + txtMf_Id.Text.TrimEnd() + "' ";				//생산검토자
                    strSql += ", @pREVISION_QC_ID = '" + txtQc_Id.Text.TrimEnd() + "' ";				//품질검토자
                    strSql += ", @pREVISION_PR_ID = '" + txtPr_Id.Text.TrimEnd() + "' ";				//승인자


                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);

                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();
                    tempRevNo = ds.Tables[0].Rows[0][2].ToString();

                    if (ERRCode != "OK")
                    {
                        Trans.Rollback();
                        goto Exit;
                    }	// ER 코드 Return시 점프

                    if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                    {
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            strSql = " usp_PBA151 ";
                            strSql = strSql + " @pType = 'I2'";
                            strSql += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";	//공장코드
                            strSql += ", @pPRNT_BOM_NO = '" + cboPrnt_Bom_No.SelectedValue.ToString().TrimEnd() + "' ";	//BOM TYPE
                            strSql += ", @pITEM_CD = '" + txtItem_Cd.Text + "' ";				//모품목코드
                            strSql += ", @pREV_NO = '" + tempRevNo + "' ";						//REV NO
                            strSql += ", @pCHILD_ITEM_SEQ = '" + fpSpread1.Sheets[0].Cells[i, 0].Value + "' ";	//자품목순번
                            strSql += ", @pCHILD_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후")].Text + "' ";	//자품목 품목코드
                            strSql += ", @pCHILD_ITEM_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_5")].Value + "' ";	//자품목기준수
                            strSql += ", @pCHILD_ITEM_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_6")].Value + "' ";	//자품목단위
                            strSql += ", @pMAT_SIZE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_7")].Text + "' ";		//재질규격						
                            strSql += ", @pVALID_FROM_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_8")].Text + "' ";	//시작일
                            strSql += ", @pVALID_TO_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_9")].Text + "' ";		//종료일
                            strSql += ", @pMATR_CNQY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_10")].Value + "' ";	//원생산량
                            strSql += ", @pQ_FIG_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_11")].Text + "' ";		//품질figno
                            strSql += ", @pREV_RESAN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경사유")].Text + "' ";		//변경이유


                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "' ";
                            strSql += " , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "'";

                            DataSet df = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = df.Tables[0].Rows[0][0].ToString();
                            MSGCode = df.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK")
                            {
                                Trans.Rollback();
                                goto Exit;
                            }	// ER 코드 Return시 점프				
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
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
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

        #region 팝업 이벤트
        //BOM설계자
        private void btnMk_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE = 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtMk_Id.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMk_Id.Value = Msgs[0].ToString();
                    txtMk_Nm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사용자조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //생산검토자
        private void btnMf_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE = 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtMf_Id.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMf_Id.Value = Msgs[0].ToString();
                    txtMf_Nm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사용자조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //품질검토자
        private void btnQc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE = 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtQc_Id.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작성자 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtQc_Id.Value = Msgs[0].ToString();
                    txtQc_Nm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사용자조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //승인자팝업
        private void btnPr_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE = 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPr_Id.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPr_Id.Value = Msgs[0].ToString();
                    txtPr_Nm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사용자조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        #endregion

        #region TextChanged 이벤트
        //BOM설계자
        private void txtBOM_DEV_USR_ID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtMk_Id.Text != "")
                {
                    txtMk_Nm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtMk_Id.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtMk_Nm.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "BOM설계자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //생산검토자
        private void txtBOM_MFG_USR_ID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtMf_Id.Text != "")
                {
                    txtMf_Nm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtMf_Id.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtMf_Nm.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "생산검토자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //품질검토자
        private void txtBOM_QUR_USR_ID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtQc_Id.Text != "")
                {
                    txtQc_Nm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtQc_Id.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtQc_Nm.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품질검토자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //승인자팝업
        private void txtBOM_APP_USR_ID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtPr_Id.Text != "")
                {
                    txtPr_Nm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPr_Id.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPr_Nm.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "BOM확인자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 셀버튼 클릭
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_2"))
                {
                    WNDW.WNDW005 pu = new WNDW.WNDW005();
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후")].Text = Msgs[2].ToString();		// 자품목코드
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_3")].Text = Msgs[3].ToString();		// 자품목명
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_4")].Text = Msgs[7].ToString();		    // 규격
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(this.Name, f.ToString());
            }
        }
        #endregion

        #region 스프레드 값 변경 처리
        protected override void fpSpread1_ChangeEvent(int Row, int Col)
        {
            // 자품목 코드 컬럼

            // 공정정보 붙여넣기 일 경우
            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후")].Text != "")
            {
                string strQuery = "";
                strQuery += " usp_P_COMMON 'P170' ";
                strQuery += " , @pCOM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후")].Text + "'";
                strQuery += " , @pPLANT_CD= '" + SystemBase.Base.gstrPLANT_CD + "'";
                strQuery += " , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    // 자품목정보를 조회한다.
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_3")].Text = dt.Rows[0]["ITEM_NM"].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_4")].Text = dt.Rows[0]["ITEM_SPEC"].ToString();

                    UIForm.FPMake.fpChange(fpSpread1, Row);
                }
            }
        }
        #endregion
    }
}
