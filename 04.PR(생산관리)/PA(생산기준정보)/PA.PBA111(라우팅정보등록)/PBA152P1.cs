using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using WNDW;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using PA.PBA111;
namespace PA.PBA152
{
    public partial class PBA152P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strItem_Cd, strItem_Nm, strRout_No, strRout_Nm, strRev_No, strBOM_DEV_USR_ID, strBOM_MFG_USR_ID, strBOM_QUR_USR_ID, strBOM_APP_USR_ID, strRout_Seq;
        string ROUT_NO = "";
        #endregion

        #region 생성자
        public PBA152P1(string Item_Cd, string Item_Nm, string Rout_No, string Rout_Nm, string Rev_No, string BOM_DEV_USR_ID, string BOM_MFG_USR_ID, string BOM_QUR_USR_ID, string BOM_APP_USR_ID, string Rout_Seq)
        {

            strItem_Cd = Item_Cd;
            strItem_Nm = Item_Nm;
            strRout_No = Rout_No;
            strRout_Nm = Rout_Nm;
            strRev_No = Rev_No;
            strBOM_DEV_USR_ID = BOM_DEV_USR_ID;
            strBOM_MFG_USR_ID = BOM_MFG_USR_ID;
            strBOM_QUR_USR_ID = BOM_QUR_USR_ID;
            strBOM_APP_USR_ID = BOM_APP_USR_ID;
            strRout_Seq = Rout_Seq;

            InitializeComponent();
        }

        public PBA152P1()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드시
        private void PBA152P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "라우팅정보변경";

            //버튼 재정의
            UIForm.Buttons.ReButton("000000010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            G1Etc[3] = "Y#N|사내#외주";
            //공정타입
            G1Etc[19] = "Y#N|사내#외주";
            //공정타입
            G1Etc[2] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z014', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
            //시간단위

            G1Etc[15] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
            //통화
            G1Etc[34] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
            //통화
            G1Etc[17] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B040', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
            //부가세유형
            G1Etc[36] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B040', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
            //부가세유형


            txtItem_Cd.Value = strItem_Cd;				//품목코드
            txtItem_Nm.Value = strItem_Nm;				//품목명
            if (strRev_No != "")
                txtRevNo.Value = strRev_No;					//리비전번호
            else
                txtRevNo.Value = "0";

            txtRout.Value = strRout_No;
            txtRoutNm.Value = strRout_Nm;
            txtMk_Id.Value = strBOM_DEV_USR_ID;			//작성자
            txtMf_Id.Value = strBOM_MFG_USR_ID;			//생산검토자
            txtQc_Id.Value = strBOM_QUR_USR_ID;			//품질검토자
            txtPr_Id.Value = strBOM_APP_USR_ID;			//승인자

            dtpRevDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            SearchExec();
        }
        #endregion

        #region SearchExec() 조회
        protected override void SearchExec()
        {
            Regex rx1 = new Regex("!!");
            string[] Msgs = rx1.Split(strRout_Seq);

            //string strChild_Seq = "";

            for (int i = 0; i < Msgs.Length; i++)
            {

                string strQuery = "";
                strQuery += " usp_PBA152 'S8' ";
                strQuery += " , @pPLANT_CD='" + SystemBase.Base.gstrPLANT_CD + "'";
                strQuery += " , @pITEM_CD ='" + txtItem_Cd.Text + "'";
                strQuery += " , @pROUT_NO='" + txtRout.Text + "'";
                strQuery += " , @pPROC_SEQ='" + Msgs[i].ToString() + "'";
                strQuery += " , @pCO_CD='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    RowInsExec();

                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정번호")].Text = dt.Rows[0]["PROC_SEQ"].ToString();			//공정번호
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시간단위")].Value = dt.Rows[0]["TIME_UNIT"].ToString();			//시간단위	

                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전")].Value = dt.Rows[0]["INSIDE_FLG"].ToString();		    //공정타입
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_2")].Text = dt.Rows[0]["JOB_CD"].ToString();				//공정CD
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_3")].Text = dt.Rows[0]["JOB_NM"].ToString();				//공정명
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_4")].Text = dt.Rows[0]["RES_CD"].ToString();				//자원CD
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_5")].Text = dt.Rows[0]["RES_DIS"].ToString();			//자원
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_6")].Text = dt.Rows[0]["SETUP_TIME"].ToString();			//설치시간
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_7")].Text = dt.Rows[0]["RUN_TIME"].ToString();			//변동가동시간					
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_8")].Text = dt.Rows[0]["MFG_LT"].ToString();			    //제조LT	
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_9")].Text = dt.Rows[0]["ROUT_DOC"].ToString();			//공정문서
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_10")].Text = dt.Rows[0]["ROUT_SIZE"].ToString();			//공정규격
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_11")].Text = dt.Rows[0]["BP_CD"].ToString();				//외주처
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_12")].Text = dt.Rows[0]["CUST_NM"].ToString();			//외주처명
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_13")].Value = dt.Rows[0]["CUR_CD"].ToString();			//통화
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_14")].Text = dt.Rows[0]["SUBCONTRACT_PRC"].ToString();	//외주공정단가
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_15")].Value = dt.Rows[0]["TAX_TYPE"].ToString();			//부가세유형
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전_16")].Value = dt.Rows[0]["MTMG_NUMB"].ToString();		//부품관리번호

                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후")].Value = dt.Rows[0]["INSIDE_FLG"].ToString();		    //공정타입
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_2")].Text = dt.Rows[0]["JOB_CD"].ToString();				//공정CD
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_4")].Text = dt.Rows[0]["JOB_NM"].ToString();				//공정명
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_5")].Text = dt.Rows[0]["RES_CD"].ToString();				//자원CD
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_7")].Text = dt.Rows[0]["RES_DIS"].ToString();			//자원
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_8")].Text = dt.Rows[0]["SETUP_TIME"].ToString();			//설치시간
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_9")].Text = dt.Rows[0]["RUN_TIME"].ToString();			//변동가동시간					
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_10")].Text = dt.Rows[0]["MFG_LT"].ToString();			//제조LT	
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_11")].Text = dt.Rows[0]["ROUT_DOC"].ToString();			//공정문서
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_12")].Text = dt.Rows[0]["ROUT_SIZE"].ToString();			//공정규격
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_13")].Text = dt.Rows[0]["BP_CD"].ToString();				//외주처
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_15")].Text = dt.Rows[0]["CUST_NM"].ToString();			//외주처명
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_16")].Value = dt.Rows[0]["CUR_CD"].ToString();			//통화
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_17")].Text = dt.Rows[0]["SUBCONTRACT_PRC"].ToString();	//외주공정단가
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_18")].Value = dt.Rows[0]["TAX_TYPE"].ToString();			//부가세유형
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_19")].Value = dt.Rows[0]["MTMG_NUMB"].ToString();		//부품관리번호

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후")].Value.ToString() == "N")  // 자원이 외주일 경우
                    {
                        // 외주란을 활성화 시간다.
                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_13") + "|1" //외주처코드
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_14") + "|0" //외주처팝업
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_16") + "|1" //통화
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_17") + "|1" //외주공정단가
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_18") + "|1" //부가세유형
                                                );
                    }
                    else
                    {
                        // 외주란을 비활성화 시간다.
                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_13") + "|3" //외주처코드
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_14") + "|3" //외주처팝업
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_16") + "|3" //통화
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_17") + "|3" //외주공정단가
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_18") + "|3" //부가세유형
                                                );

                    }

                }
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true)
                {
                    this.Cursor = Cursors.WaitCursor;

                    string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                    string tempRevNo = "";

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strSql = " usp_PBA152 ";
                        strSql = strSql + " @pType = 'I1'";

                        strSql += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";		//공장코드
                        strSql += ", @pITEM_CD = '" + txtItem_Cd.Text.TrimEnd() + "' ";			//품목코드
                        strSql += ", @pROUT_NO = '" + txtRout.Text.TrimEnd() + "' ";			//라우팅번호
                        strSql += ", @pREV_NO = '" + txtRevNo.Text.TrimEnd() + "' ";			//리비전번호
                        strSql += ", @pREV_DT = '" + dtpRevDt.Text.TrimEnd() + "' ";		//변경일자		
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "' ";

                        strSql += ", @pROU_DEV_USER_ID = '" + txtMk_Id.Text.TrimEnd() + "' ";
                        strSql += ", @pROU_MFG_USER_ID = '" + txtMf_Id.Text.TrimEnd() + "' ";
                        strSql += ", @pROU_QUR_USER_ID = '" + txtQc_Id.Text.TrimEnd() + "' ";
                        strSql += ", @pROU_APP_USER_ID = '" + txtPr_Id.Text.TrimEnd() + "' ";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);

                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();
                        tempRevNo = ds.Tables[0].Rows[0][2].ToString();


                        if (ERRCode != "OK")
                        {
                            Trans.Rollback();
                            goto Exit;
                        }	// ER 코드 Return시 점프


                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            strSql = "";
                            strSql = " usp_PBA152 ";
                            strSql = strSql + " @pType = 'I2'";

                            strSql += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";	//공장코드
                            strSql += ", @pITEM_CD = '" + txtItem_Cd.Text + "' ";				//품목코드
                            strSql += ", @pROUT_NO = '" + txtRout.Text.TrimEnd() + "' ";		//라우팅번호
                            strSql += ", @pREV_NO = '" + tempRevNo + "' ";						//REV NO

                            strSql += ", @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정번호")].Value.ToString() + "' ";	//공정번호
                            strSql += ", @pTIME_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시간단위")].Value + "' ";		//시간단위

                            strSql += ", @pINSIDE_FLG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후")].Value + "' ";		//공정타입	
                            strSql += ", @pJOB_CD= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_2")].Text + "' ";		//공정CD
                            strSql += ", @pRES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_5")].Text + "' ";		//자원CD
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_8")].Text != "")
                            {
                                strSql += ", @pSETUP_TIME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_8")].Value + "' ";	//설치시간
                            }
                            else
                            {
                                strSql += ", @pSETUP_TIME = 0 ";	//설치시간
                            }
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_9")].Text != "")
                            {
                                strSql += ", @pRUN_TIME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_9")].Value + "' ";		//변동가동시간
                            }
                            else
                            {
                                strSql += ", @pRUN_TIME = 0 ";
                            }

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_10")].Text != "")
                            {
                                strSql += ", @pMFG_LT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_10")].Value + "' ";			//제조L/T
                            }
                            else
                            {
                                strSql += ", @pMFG_LT = 0 ";
                            }
                            strSql += ", @pROUT_DOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_11")].Text + "' ";		//공정문서
                            strSql += ", @pROUT_SIZE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_12")].Text + "' ";		//공정규격
                            strSql += ", @BP_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_13")].Text + "' ";				//외주처						
                            strSql += ", @CUR_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_16")].Value + "' ";				//통화코드
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_17")].Text != "")
                            {
                                strSql += ", @pSUBCONTRACT_PRC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_17")].Value + "' ";		//외주단가
                            }
                            else
                            {
                                strSql += ", @pSUBCONTRACT_PRC = 0 ";
                            }
                            strSql += ", @TAX_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_18")].Value + "' ";				//부가세유형
                            strSql += ", @pMTMG_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_19")].Text + "' ";				//부품관리번호

                            strSql += ", @pREV_RESAN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_20")].Text + "' ";		//변경이유

                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "' ";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";


                            DataSet df = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = df.Tables[0].Rows[0][0].ToString();
                            MSGCode = df.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK")
                            {
                                Trans.Rollback();
                                goto Exit;
                            }	// ER 코드 Return시 점프
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
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else if (ERRCode == "ER")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
        }
        #endregion

        #region 코드 입력시 코드명 자동 입력

        //작성자
        private void txtMk_Id_TextChanged(object sender, System.EventArgs e)
        {
            txtMk_Nm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtMk_Id.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
        }

        //생산검토자
        private void txtMf_Id_TextChanged(object sender, System.EventArgs e)
        {
            txtMf_Nm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtMf_Id.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
        }

        //품질검토자
        private void txtQc_Id_TextChanged(object sender, System.EventArgs e)
        {
            txtQc_Nm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtQc_Id.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
        }

        //승인자
        private void txtPr_Id_TextChanged(object sender, System.EventArgs e)
        {
            txtPr_Nm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPr_Id.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
        }
        #endregion

        #region 셀버튼 클릭
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 공정조회
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_3"))
                {
                    string strQuery = " usp_P_COMMON 'P042', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC", "@pLANG_CD" };
                    string[] strSearch = new string[] { "", "", "P001", SystemBase.Base.gstrLangCd };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("WB5101", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                    pu.Width = 500;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_2")].Text = Msgs[0].ToString(); //공정코드
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_4")].Value = Msgs[1].ToString(); //공정명

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);
                    }
                }
                // 자원조회
                else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_6"))
                {

                    string strQuery = " usp_P_COMMON 'P051', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { "", "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05005", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                    pu.Width = 500;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_5")].Text = Msgs[0].ToString();	//자원코드
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_7")].Value = Msgs[1].ToString();	//자원명
                        //						fpSpread2.Sheets[0].Cells[e.Row,SystemBase.Base.GridHeadIndex(GHIdx2,"작업장")].Value	= Msgs[2].ToString();	//작업장
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후")].Value = Msgs[3].ToString();	//공정타입

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);

                        if (Msgs[3].ToString() == "N")  // 자원이 외주일 경우
                        {
                            // 외주란을 활성화 시간다.
                            UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_13") + "|1" //외주처코드
                                                              + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_14") + "|0" //외주처팝업
                                                              + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_16") + "|1" //통화
                                                              + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_17") + "|1" //외주공정단가
                                                              + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_18") + "|1" //부가세유형
                                                   );

                            // 초기값 설정
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_16")].Value = "KRW";
                        }
                        else
                        {
                            // 외주란을 비활성화 시간다.
                            UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_13") + "|3" //외주처코드
                                                              + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_14") + "|3" //외주처팝업
                                                              + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_16") + "|3" //통화
                                                              + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_17") + "|3" //외주공정단가
                                                              + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_18") + "|3" //부가세유형
                                                   );

                            // 초기값 설정
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_16")].Value = "";
                        }
                    }
                }
                // 외주처
                else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_14"))
                {
                    // 공정 타입이 외주일 경우 처리
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후")].Value == null ||
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후")].Value.ToString() == "Y")
                    {
                        SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn("P0031"));
                        return;
                    }

                    WNDW.WNDW002 pu = new WNDW.WNDW002("P");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_13")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_15")].Text = Msgs[2].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);
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
            // 공정정보 붙여넣기 일 경우
            if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_2"))
            {
                string strQuery = "";
                strQuery += " usp_P_COMMON 'P042' ";
                strQuery += " , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "'";
                strQuery += " , @pETC='P001'";
                strQuery += " , @pCOM_CD='" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_2")].Text.Trim() + "'";
                strQuery += " , @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {

                    // 공정정보를 조회한다.
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_2")].Text = dt.Rows[0]["MINOR_CD"].ToString();	//공정코드
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_4")].Text = dt.Rows[0]["CD_NM"].ToString();    //공정명

                    UIForm.FPMake.fpChange(fpSpread1, Row);

                }
            }

            // 자원정보 붙여넣기 일 경우
            if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_5"))
            {
                string strQuery = "";
                strQuery += " usp_P_COMMON 'P051' ";
                strQuery += " , @pCOM_CD='" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_5")].Text + "'";
                strQuery += " , @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {

                    // 자원정보를 조회한다.
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_5")].Text = dt.Rows[0]["RES_CD"].ToString();	//자원코드
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_6")].Value = dt.Rows[0]["RES_DIS"].ToString();	//자원명
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후")].Value = dt.Rows[0]["INSIDE_FLG"].ToString();	//공정타입

                    UIForm.FPMake.fpChange(fpSpread1, Row);

                    if (dt.Rows[0]["INSIDE_FLG"].ToString() == "N")  // 자원이 외주일 경우
                    {
                        // 외주란을 활성화 시간다.
                        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_13") + "|1" //외주처코드
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_14") + "|0" //외주처팝업
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_16") + "|1" //통화
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_17") + "|1" //외주공정단가
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_18") + "|1" //부가세유형
                                                );

                        // 초기값 설정
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_16")].Value = "KRW";
                    }
                    else
                    {
                        // 외주란을 비활성화 시간다.
                        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_13") + "|3" //외주처코드
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_14") + "|3" //외주처팝업
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_16") + "|3" //통화
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_17") + "|3" //외주공정단가
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_18") + "|3" //부가세유형
                                                );

                        // 초기값 설정
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_16")].Value = "";
                    }
                }
            }
            // 외주거래처 붙여넣기 일 경우
            if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_13"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_15")].Text
                    = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후_13")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
            }
        }
        #endregion
        
        #region 공정 설계자 조회 조회
        private void btnDEV_Click(object sender, EventArgs e)
        {


            try
            {
                string strQuery = " usp_P_COMMON @pType='P140', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtMk_Id.Text, txtMk_Nm.Text, "RD" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정설계자 조회", true);

                pu.Width = 500;
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공정설계자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region 생산검토자 조회 조회
        private void btnMFG_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtMf_Id.Text, txtMf_Nm.Text, "RM" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "생산검토자 조회", true);

                pu.Width = 500;
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "생산검토자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 품질검토자 조회 조회
        private void btnQUR_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtQc_Id.Text, txtQc_Nm.Text, "RQ" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품질검토자 조회", true);

                pu.Width = 500;
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품질검토자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 라우팅확인자 조회 조회
        private void btnAPP_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtPr_Id.Text, txtPr_Nm.Text, "RA" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "라우팅확인자 조회", true);

                pu.Width = 500;
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "라우팅확인자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
}
