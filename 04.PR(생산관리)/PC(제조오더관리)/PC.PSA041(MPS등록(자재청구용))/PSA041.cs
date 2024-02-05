﻿#region 작성정보
/*********************************************************************/
// 단위업무명 : MRP등록(자재청구용)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-04
// 작성내용 : MRP등록(자재청구용) 및 관리
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

namespace PC.PSA041
{
    public partial class PSA041 : UIForm.FPCOMM1
    {
        string strMQuery = "";

        public PSA041()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void PSA041_Load(object sender, System.EventArgs e)
        {  
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "상태")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P012' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "오더고정")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P013' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "MPS구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P014' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P038' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "DATA작성유무")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B029' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            SystemBase.ComboMake.C1Combo(cboSTATUS, "usp_P_COMMON @pTYPE	= 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P012' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);	// 

            dtpRECEIVE_ST.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-6).ToShortDateString();

            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 1);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    strMQuery = " usp_PSA041 'S1'";
                    strMQuery += ", @pPLANT_CD='" + txtPlant_CD.Text + "'";
                    strMQuery += ", @pDELIVERY_DT='" + dtpDelivery_ST.Text + "'";
                    strMQuery += ", @pDELIVERY_ED='" + dtpDelivery_ED.Text + "'";
                    strMQuery += ", @pRECEIVE_ST='" + dtpRECEIVE_ST.Text + "'";
                    strMQuery += ", @pRECEIVE_ED='" + dtpRECEIVE_ED.Text + "'";
                    strMQuery += ", @pITEM_CD='" + txtITEM_CD.Text + "'";
                    strMQuery += ", @pSTATUS='" + cboSTATUS.SelectedValue.ToString() + "'";
                    strMQuery += ", @pPROJECT_NO='" + txtProject_NO.Text + "'";
                    strMQuery += ", @pPROJECT_SEQ='" + txtProject_SEQ.Text + "'";
                    strMQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 1);

                    GridReMake();
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

        #region NewExec() 신규
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpRECEIVE_ST.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-6).ToShortDateString();
            dtpRECEIVE_ED.Value = null;
            dtpDelivery_ST.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpDelivery_ED.Value = null;

            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 1);
        }
        #endregion
        
        #region GridReMake() 그리드 재정의
        public void GridReMake()
        {
            try
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상태")].Value.ToString() == "P")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i
                            , SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량") + "|1#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "생산완료일") + "|1#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "오더고정") + "|1#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드") + "|1#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드_2") + "|0#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "납기일") + "|1#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분") + "|1#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "DATA작성유무") + "|1"
                            );
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i
                            , SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량") + "|3#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "생산완료일") + "|3#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "오더고정") + "|3#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드") + "|3#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드_2") + "|3#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "납기일") + "|3#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분") + "|3#"
                            + SystemBase.Base.GridHeadIndex(GHIdx1, "DATA작성유무") + "|3"
                            );
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 재정의"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        
        #region 조회조건 팝업 이벤트
        //공장
        private void btnPlant_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P011'  , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtPlant_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPlant_CD.Text = Msgs[0].ToString();
                    txtPlant_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목코드
        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtPlant_CD.Text, true, txtITEM_CD.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Text = Msgs[2].ToString();
                    txtITEM_NM.Value = Msgs[3].ToString();
                    txtITEM_CD.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProject_NO.Text, "S1", "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProject_NO.Text = Msgs[3].ToString();
                    txtProject_NM.Text = Msgs[4].ToString();
                    txtProject_SEQ.Text = Msgs[5].ToString();
                    txtITEM_CD.Text = Msgs[6].ToString();
                    txtITEM_NM.Text = Msgs[7].ToString();

                    txtProject_NO.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        
        #region MPS계획, MPS전개 버튼 클릭시
        // MPS계획
        private void btnMPS_PLAN_Click(object sender, System.EventArgs e)
        {
            Proc("P");
        }

        // MPS확정
        private void btnMPS_CONFIRM_Click(object sender, System.EventArgs e)
        {
            Proc("F");
        }

        private void Proc(string Kind)
        {
            this.Cursor = Cursors.WaitCursor;

            if (UIForm.FPMake.FPUpCheck(fpSpread1, false) == true)
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
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text.ToString() == "True")
                        {
                            string Query = " usp_PSA041 'U2',@pKIND ='" + Kind + "' ";
                            Query += " , @pPROJECT_NO='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text.ToString() + "' ";
                            Query += " , @pPROJECT_SEQ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산차수")].Text.ToString() + "' ";
                            Query += " , @pITEM_CD='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.ToString() + "' ";
                            Query += " , @pMAKEORDER_NO='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호")].Text.ToString() + "' ";
                            Query += " , @pUP_ID='" + SystemBase.Base.gstrUserID + "' ";
                            Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);
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
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                    ERRCode = "ER";
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
            }

            this.Cursor = Cursors.Default;
        }
        #endregion
        
        #region 그리드 버튼 클릭 이벤트
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드_2"))
            {
                try
                {
                    WNDW005 pu = new WNDW005(txtPlant_CD.Text, true, fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품명")].Text = Msgs[3].ToString();

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[2].ToString();

                        Grid_Change(e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"));
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2"))
            {
                try
                {
                    //WNDW005.WNDW005 pu = new WNDW005.WNDW005(txtPlant_CD.Text, true, fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text);
                    //pu.ShowDialog();
                    //if (pu.DialogResult == DialogResult.OK)
                    //{
                    //    string[] Msgs = pu.ReturnVal;

                    //    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[2].ToString();
                    //    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = Msgs[3].ToString();
                    //    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호")].Text = Msgs[15].ToString();
                    //    string strPlantCd = Msgs[16].ToString();
                    //    if (strPlantCd != "")
                    //    {
                    //        string strPlantNm = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", strPlantCd, "");
                    //        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text = strPlantCd;
                    //        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = strPlantNm;
                    //    }
                    //}
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드_2"))
            {
                try
                {
                    string strQuery = " usp_P_COMMON 'P014', @pPLANT_CD = '" + txtPlant_CD.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04014", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고 조회");	//창고, LOCATION조회
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "창고 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "거래처코드_2"))
            {
                try
                {
                    //WNDW002.WNDW002 pu = new WNDW002.WNDW002(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처코드")].Text, "");
                    //pu.ShowDialog();
                    //if (pu.DialogResult == DialogResult.OK)
                    //{
                    //    string[] Msgs = pu.ReturnVal;

                    //    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처코드")].Text = Msgs[1].ToString();
                    //    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명")].Text = Msgs[2].ToString();

                    //    UIForm.FPMake.fpChange(fpSpread1, e.Row);//수정플래그
                    //}
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "사업코드_2"))
            {
                try
                {
                    string strQuery = " usp_P_COMMON @pType='P090' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업코드")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
                    pu.Width = 500;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업명")].Text = Msgs[1].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion
        
        #region 그리드 체인지 이벤트
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            Grid_Change(e.Row, e.Column);
        }

        private void Grid_Change(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드"))
            {
                try
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품명")].Text
                        = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text, "");

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text
                        = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text;

                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품명")].Text == "")
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = "";
                    }

                    Grid_Change(Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"));
                }
                catch
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = "";
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"))
            {
                try
                {
                    string strSql = " usp_PSA041 'S2'";
                    strSql = strSql + ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                    if (dt.Rows.Count > 0)
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = dt.Rows[0][0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = dt.Rows[0][1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호")].Text = dt.Rows[0][2].ToString();
                        string strPlantCd = dt.Rows[0][3].ToString();
                        if (strPlantCd != "")
                        {
                            string strPlantNm = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", strPlantCd, "");
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text = strPlantCd;
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = strPlantNm;
                        }
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번변호")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = "";
                    }
                }
                catch
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번변호")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = "";
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드"))
            {
                try
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text
                        = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text, " AND PLANT_CD = '" + txtPlant_CD.Text + "' ");
                }
                catch
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = "";
                }
            }
        }
        #endregion
        
        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
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
                                case "I": strGbn = "I1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            string Query = " usp_PSA041 @pType = '" + strGbn + "' ";
                            Query += ",@pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                            Query += ",@pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "'";
                            Query += ",@pPLANT_CD = '" + txtPlant_CD.Text + "'";
                            Query += ",@pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text + "' ";
                            Query += ",@pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산차수")].Text + "' ";
                            Query += ",@pGROUP_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text + "' ";
                            Query += ",@pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                            Query += ",@pSL_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text + "' ";
                            Query += ",@pITEM_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량")].Value + "' ";
                            Query += ",@pRECEIVE_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주일")].Text + "' ";
                            Query += ",@pMAKEFINISH_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산완료일")].Text + "' ";
                            Query += ",@pDELIVERY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납기일")].Text + "' ";
                            Query += ",@pSTATUS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상태")].Value + "' ";
                            Query += ",@pMPS_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더고정")].Value + "' ";
                            Query += ",@pMAKEORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호")].Text + "' ";
                            Query += ",@pRANK_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "우선순위")].Value + "' ";
                            Query += ",@pBUYER_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처코드")].Text + "' ";
                            Query += ",@pSO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text + "' ";
                            Query += ",@pSO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Text + "' ";
                            Query += ",@pETC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                            Query += ",@pMPS_KIND = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MPS구분")].Value + "' ";
                            Query += ",@PENT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사업코드")].Text + "' ";
                            Query += ",@pMF_PLAN_USER = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산담당자")].Text + "' ";
                            Query += ",@pPROCESS_GU = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")].Value + "' ";
                            Query += ",@pDATA_COMPLETE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "DATA작성유무")].Value + "' ";
                            Query += ",@pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";
                            Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);
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
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                    ERRCode = "ER";
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();

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

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 코드 입력시 코드명 자동입력
        private void txtPlant_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPlant_CD.Text != "")
                {
                    txtPlant_NM.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlant_CD.Text, "");
                }
                else
                {
                    txtPlant_NM.Value = "";
                }
            }
            catch { }
        }

        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtITEM_CD.Text != "")
                {
                    txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, "");
                }
                else
                {
                    txtITEM_NM.Value = "";
                }
            }
            catch { }
        }

        private void txtProject_NO_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProject_NO.Text != "")
                {
                    txtProject_NM.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_NO.Text, "");
                }
                else
                {
                    txtProject_NM.Value = "";
                }
                if (txtProject_NM.Text == "")
                {
                    txtProject_SEQ.Text = "";
                }
            }
            catch { }
        }
        #endregion

        #region 스케쥴 취소
        private void btnSCH_CANCEL_Click(object sender, System.EventArgs e)
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
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text.ToString() == "True")
                    {
                        string Query = " usp_PSA041 @pType = 'D2'";
                        Query += " , @pMAKEORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호")].Text + "' ";
                        Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }
                }
                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = f.Message;
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
        }
        #endregion

        #region DATA가져오기 버튼
        private void btnDataInput_Click(object sender, System.EventArgs e)
        {
            try
            {
                PSA041P1 pu = new PSA041P1();
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] strMsgs = pu.ReturnVal;

                    int intRowCount = strMsgs.Length;		//popup창에서 넘어온 Row수
                    if (intRowCount > 0)
                    {
                        UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 1);

                        int intColumnCount = 0;				//popup창에서 넘어온 Columnw수
                        int intRealRow = fpSpread1.ActiveSheet.Rows.Count;		//현재그리드 행수

                        for (int i = 0; i < intRowCount; i++)
                        {
                            Regex rx1 = new Regex("!!");
                            string[] Msgs = rx1.Split(strMsgs[i].ToString());

                            intColumnCount = Msgs.Length;		//popup창에서 넘어온 Columnw수

                            string strSql = " usp_PSA041 'M1' ";
                            strSql = strSql + ", @pMAKEORDER_NO = '" + Msgs[19].ToString() + "'";

                            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                            if (ds.Tables[0].Rows.Count < 1)
                            {
                                RowInsExec();
                                int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                                for (int j = 0; j < intColumnCount; j++)
                                {
                                    fpSpread1.Sheets[0].Cells[intRow, j + 2].Text = Msgs[j].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.MessageBoxComm.Show(f.ToString());
            }
        }
        #endregion


    }
}
