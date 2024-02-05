
#region 작성정보
/*********************************************************************/
// 단위업무명 : 일별O/T시간등록(TOUCH용) 조회
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-08
// 작성내용 : 일별O/T시간등록(TOUCH용) 및 관리
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

namespace PA.PBA114
{
    public partial class PBA114 : UIForm.FPCOMM1
    {
        #region 변수선언
        private string strMQuery;
        string strPlant = "";
        string strSCH = "";
        #endregion

        public PBA114()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void PBA114_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //조회조건 콤보
            SystemBase.ComboMake.C1Combo(cboSchId, "usp_P_COMMON @pTYPE = 'P081', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
            dtpSTART_DT.Text = DateTime.Now.ToShortDateString();
            dtpEND_DT.Text = DateTime.Now.ToShortDateString();
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    strPlant = txtPlantCd.Text;
                    strSCH = Convert.ToString(cboSchId.SelectedValue);

                    strMQuery = " usp_PBA114 'S1'";
                    strMQuery += ", @pSTART_DT = '" + dtpSTART_DT.Text + "'";
                    strMQuery += ", @pEND_DT = '" + dtpEND_DT.Text + "'";
                    strMQuery += ", @pRES_CD = '" + txtRES_CD.Text + "'";
                    strMQuery += ", @pWC_CD = '" + txtWc_CD.Text + "'";
                    strMQuery += ", @pSCH_ID = '" + strSCH + "'";
                    strMQuery += ", @pPLANT_CD = '" + strPlant + "'";
                    strMQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0);
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (Convert.ToString(fpSpread1.Sheets[0].Cells[i, 16].Text) == "1")
                        {
                            fpSpread1.Sheets[0].Cells[i, 1].ForeColor = Color.Blue;
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
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            string fcsStr = "";
            this.Cursor = Cursors.WaitCursor;

            if (UIForm.FPMake.FPUpCheck(fpSpread1) == true)
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
                        string strGbn = "";

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;
                                default: strGbn = ""; break;
                            }

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "일자")].Text == "") continue; //하단합계 제외
                            // 그리드 상단 필수항목 체크
                            //							if(fpSpread1.Sheets[0].Cells[i, 9].Text.Trim()  == "" && fpSpread1.Sheets[0].Cells[i, 12].Text.Trim()  == "") continue;							
                            if ((fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오전")].Text.Trim() == "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오전_2")].Text.Trim() != "") ||
                                (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오전")].Text.Trim() != "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오전_2")].Text.Trim() == ""))
                            {
                                ERRCode = "WR";
                                MSGCode = "오전시간을 입력하세요!";
                                Trans.Rollback(); goto Exit;
                            }
                            else if ((fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오후")].Text.Trim() == "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오후_2")].Text.Trim() != "") ||
                                (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오후")].Text.Trim() != "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오후_2")].Text.Trim() == ""))
                            {
                                ERRCode = "WR";
                                MSGCode = "오후시간을 입력하세요!";
                                Trans.Rollback(); goto Exit;
                            }

                            string holiday = Convert.ToString(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "휴일유무")].Text);
                            /*
                            if(holiday == "0")		
                            {
                                if(fpSpread1.Sheets[0].Cells[i, 9].Text != "" && fpSpread1.Sheets[0].Cells[i, 9].Text != "00:00")
                                {
                                    if(Convert.ToInt16(fpSpread1.Sheets[0].Cells[i, 9].Text.Substring(0,2)) < 17)
                                    {										
                                        ERRCode = "ER";
                                        MSGCode = "평일날 O/T시작시간은 17시 이후여야 합니다!";
                                        Trans.Rollback();goto Exit;	
                                    }
                                }
                            }*/

                            fcsStr = fpSpread1.Sheets[0].Cells[i,SystemBase.Base.GridHeadIndex(GHIdx1, "자원")].Text;

                            string strSql = " usp_PBA114 '" + strGbn + "'";
                            strSql += ", @pSCH_ID = '" + strSCH + "'";
                            strSql += ", @pRES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자원")].Text + "'";
                            strSql += ", @pSTD_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "일자")].Text + "'";
                            strSql += ", @pWORKCENTER_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "주작업장")].Text + "'";
                            strSql += ", @pGRES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자원그룹")].Text + "'";
                            strSql += ", @pPLANT_CD = '" + strPlant + "'";
                            strSql += ", @pMON_ST_TM  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오전")].Text.Replace(":", "") + "'";
                            strSql += ", @pMON_ED_TM  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오전_2")].Text.Replace(":", "") + "'";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오전_3")].Text.Trim() != "")
                                strSql += ", @pMON_GAP  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오전_3")].Value + "'";
                            strSql += ", @pAFT_ST_TM  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오후")].Text.Replace(":", "") + "'";
                            strSql += ", @pAFT_ED_TM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오후_2")].Text.Replace(":", "") + "'";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오후_3")].Text.Trim() != "")
                                strSql += ", @pAFT_GAP  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오후_3")].Value + "'";
                            strSql += ", @pOT_REMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업내용")].Text + "'";
                            strSql += ", @pHDAY_FLG = '" + holiday + "'";
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
                    UIForm.FPMake.GridSetFocus(fpSpread1, fcsStr, SystemBase.Base.GridHeadIndex(GHIdx1, "자원")); //저장 후 그리드 포커스 이동
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

        #region 조회조건 팝업
        //공장
        private void btnPlant_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P011', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' "; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };											  // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtPlantCd.Text, "" };											  // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회");

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtPlantCd.Text = Msgs[0].ToString();
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
        //작업장 조회
        private void btnWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWc_CD.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWc_CD.Text = Msgs[0].ToString();
                    txtWc_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 자원조회
        private void btnRES_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P056', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtRES_CD.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "자원 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtRES_CD.Text = Msgs[0].ToString();
                    txtRES_DIS.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 코드 입력시 코드명 자동입력
        //작업장
        private void txtWc_CD_TextChanged(object sender, System.EventArgs e)
        {
            txtWc_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWc_CD.Text, " AND MAJOR_CD = 'P002' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //자원
        private void txtRES_CD_TextChanged(object sender, System.EventArgs e)
        {
            txtRES_DIS.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtRES_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //공장
        private void txtPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            if (Column == 8)
            {
                fpSpread1.Sheets[0].Cells[Row, 9].Text = "";
                fpSpread1.Sheets[0].Cells[Row, 10].Text = "";
                fpSpread1.Sheets[0].Cells[Row, 11].Text = "0";
                fpSpread1.Sheets[0].Cells[Row, 12].Text = "";
                fpSpread1.Sheets[0].Cells[Row, 13].Text = "";
                fpSpread1.Sheets[0].Cells[Row, 14].Text = "0";
                fpSpread1.Sheets[0].Cells[Row, 15].Text = "";
            }
        }
        #endregion

        #region 그리드 change
        protected override void fpSpread1_ChangeEvent(int Row, int Col)
        {
            /*
            if(Col == 9)
            {
                string holiday  = Convert.ToString(fpSpread1.Sheets[0].Cells[Row,13].Text);
                if(holiday == "0")		
                {
                    if(fpSpread1.Sheets[0].Cells[Row, 9].Text != "")
                    {
                        if(Convert.ToInt16(fpSpread1.Sheets[0].Cells[Row, 9].Text.Substring(0,2)) < 17 && Convert.ToInt16(fpSpread1.Sheets[0].Cells[Row, 9].Text.Substring(0,2)) >= 8)
                        {
                            MessageBox.Show("평일날 O/T시작시간은 8시 이전이거나 17시 이후여야 합니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            fpSpread1.Sheets[0].Cells[Row, 9].Text = "";	
                        }
                    }
                }				 
            }*/
            try
            {
                string strSql = "";
                if (Col == 9 || Col == 10)
                {
                    if (fpSpread1.Sheets[0].Cells[Row, 9].Text.Replace("_", "").Length != 5)
                    {
                        fpSpread1.Sheets[0].Cells[Row, 9].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, 11].Value = 0;
                        return;
                    }
                    if (fpSpread1.Sheets[0].Cells[Row, 10].Text.Replace("_", "").Length != 5)
                    {
                        fpSpread1.Sheets[0].Cells[Row, 10].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, 11].Value = 0;
                        return;
                    }
                    if (Convert.ToInt16(fpSpread1.Sheets[0].Cells[Row, 9].Text.Replace(":", "")) > 1200)
                    {
                        MessageBox.Show("오전시간이 아닙니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fpSpread1.Sheets[0].Cells[Row, 9].Text = "";
                        return;
                    }
                    else if (Convert.ToInt16(fpSpread1.Sheets[0].Cells[Row, 10].Text.Replace(":", "")) > 1200)
                    {
                        MessageBox.Show("오전시간이 아닙니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fpSpread1.Sheets[0].Cells[Row, 10].Text = "";
                        return;
                    }
                    else if (fpSpread1.Sheets[0].Cells[Row, 9].Text.Trim() != "" && fpSpread1.Sheets[0].Cells[Row, 10].Text.Trim() != "" && Convert.ToInt16(fpSpread1.Sheets[0].Cells[Row, 9].Text.Replace(":", "")) > Convert.ToInt16(fpSpread1.Sheets[0].Cells[Row, 10].Text.Replace(":", "")))
                    {
                        MessageBox.Show("오전완료시간이 시작시간보다 작습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fpSpread1.Sheets[0].Cells[Row, 10].Text = "";
                        return;
                    }

                    if (fpSpread1.Sheets[0].Cells[Row, 9].Text.Trim() != "" && fpSpread1.Sheets[0].Cells[Row, 10].Text.Trim() != "" &&
                        fpSpread1.Sheets[0].Cells[Row, 10].Text.Replace(":", "") != "0000")
                    {
                        strSql = " usp_PBA114 'C1' ";
                        strSql += ", @pAFT_ST_TM  = '" + fpSpread1.Sheets[0].Cells[Row, 9].Text.Replace(":", "") + "'";
                        strSql += ", @pAFT_ED_TM = '" + fpSpread1.Sheets[0].Cells[Row, 10].Text.Replace(":", "") + "'";
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                        if (dt.Rows.Count > 0)
                        {
                            fpSpread1.Sheets[0].Cells[Row, 11].Value = dt.Rows[0][0];
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, 11].Value = 0;
                        }
                    }


                }
                else if (Col == 12 || Col == 13)
                {
                    if (fpSpread1.Sheets[0].Cells[Row, 12].Text.Replace("_", "").Length != 5)
                    {
                        fpSpread1.Sheets[0].Cells[Row, 12].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, 14].Value = 0;
                        return;
                    }
                    if (fpSpread1.Sheets[0].Cells[Row, 13].Text.Replace("_", "").Length != 5)
                    {
                        fpSpread1.Sheets[0].Cells[Row, 13].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, 14].Value = 0;
                        return;
                    }

                    if (Convert.ToInt16(fpSpread1.Sheets[0].Cells[Row, 12].Text.Replace(":", "")) < 1200)
                    {
                        MessageBox.Show("오후시간이 아닙니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fpSpread1.Sheets[0].Cells[Row, 12].Text = "";
                    }
                    else if (Convert.ToInt16(fpSpread1.Sheets[0].Cells[Row, 13].Text.Replace(":", "")) < 1200)
                    {
                        MessageBox.Show("오후시간이 아닙니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fpSpread1.Sheets[0].Cells[Row, 13].Text = "";
                    }
                    else if (fpSpread1.Sheets[0].Cells[Row, 12].Text.Trim() != "" && fpSpread1.Sheets[0].Cells[Row, 13].Text.Trim() != "" && Convert.ToInt16(fpSpread1.Sheets[0].Cells[Row, 12].Text.Replace(":", "")) > Convert.ToInt16(fpSpread1.Sheets[0].Cells[Row, 13].Text.Replace(":", "")))
                    {
                        MessageBox.Show("오후완료시간이 시작시간보다 작습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fpSpread1.Sheets[0].Cells[Row, 13].Text = "";
                        return;
                    }

                    if (fpSpread1.Sheets[0].Cells[Row, 12].Text.Trim() != "" && fpSpread1.Sheets[0].Cells[Row, 13].Text.Trim() != "" &&
                        fpSpread1.Sheets[0].Cells[Row, 12].Text.Replace(":", "") != "0000")
                    {
                        strSql = " usp_PBA114 'C1' ";
                        strSql += ", @pAFT_ST_TM  = '" + fpSpread1.Sheets[0].Cells[Row, 12].Text.Replace(":", "") + "'";
                        strSql += ", @pAFT_ED_TM = '" + fpSpread1.Sheets[0].Cells[Row, 13].Text.Replace(":", "") + "'";
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";


                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                        if (dt.Rows.Count > 0)
                        {
                            fpSpread1.Sheets[0].Cells[Row, 14].Value = dt.Rows[0][0];
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, 14].Value = 0;
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "시간설정"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
}
