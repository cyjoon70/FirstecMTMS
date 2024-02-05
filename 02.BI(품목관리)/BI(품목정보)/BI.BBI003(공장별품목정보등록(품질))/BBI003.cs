#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목정보등록(품질)
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-01
// 작성내용 : 품질 품목 정보 등록 및 관리
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

namespace BI.BBI003
{
    public partial class BBI003 : UIForm.FPCOMM1
    {

        #region 생성자
        public BBI003()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBI003_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용

            SystemBase.ComboMake.C1Combo(cboSPlant, "usp_B_COMMON @pTYPE = 'PLANT' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");	//공장
            SystemBase.ComboMake.C1Combo(cboSItemAcct, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //품목계정
            SystemBase.ComboMake.C1Combo(cboItemType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'P032', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 3);      //품목구분
            cboSPlant.SelectedValue = SystemBase.Base.gstrPLANT_CD;

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단품구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B029', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //단품구분
            // 2020.06.03. hma 추가(Start): 기계검사, 전자검사 플래그
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "기계/전자구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM2', @pCODE = 'Q035', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); // 기계/전자구분
            // 2020.06.03. hma 추가(End)
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "최종검사")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q013', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //최종검사

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            rdoLotAll.Checked = true;
            rdoSrAll.Checked = true;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            SystemBase.ComboMake.C1Combo(cboSPlant, "usp_B_COMMON @pTYPE = 'PLANT' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");	//공장
            SystemBase.ComboMake.C1Combo(cboSItemAcct, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //품목계정

            cboSPlant.SelectedValue = SystemBase.Base.gstrPLANT_CD;

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단품구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B029', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //단품구분
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "최종검사")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q013', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //최종검사

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            rdoLotAll.Checked = true;
            rdoSrAll.Checked = true;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {

            this.Cursor = Cursors.WaitCursor;

            string strLotYN = string.Empty;
            string strSerYN = string.Empty;

            try
            {

                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {

                    if (rdoLotAll.Checked) strLotYN = "";
                    if (rdoLotY.Checked) strLotYN = "Y";
                    if (rdoLotN.Checked) strLotYN = "N";

                    if (rdoSrAll.Checked) strSerYN = "";
                    if (rdoSrY.Checked) strSerYN = "Y";
                    if (rdoSrN.Checked) strSerYN = "N";

                    string strQuery = " usp_BBI003  'S1'";
                    strQuery += ", @pPLANT_CD = '" + cboSPlant.SelectedValue.ToString() + "' ";
                    strQuery += ", @pITEM_CD = '" + txtSItemCd.Text + "' ";
                    strQuery += ", @pITEM_NM = '" + txtSItemNm.Text + "' ";
                    strQuery += ", @pITEM_ACCT = '" + cboSItemAcct.SelectedValue.ToString() + "' ";
                    strQuery += ", @pDRAW_NO = '" + txtSDrawNo.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pITEM_TYPE = '" + cboItemType.SelectedValue.ToString() + "'";
                    strQuery += ", @pS_LOT_YN = '" + strLotYN + "'";
                    strQuery += ", @pS_SERIAL_YN = '" + strSerYN + "'";

                    //UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 4);
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 4);

                    // 품질증빙 문서구분에 따른 칼럼 속성 설정
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 23; i <= fpSpread1.Sheets[0].Columns.Count - 1; i++)
                        {
                            fpSpread1.Sheets[0].Columns[23].Visible = false;

                            if (i > 23)
                            {
                                fpSpread1.Sheets[0].Columns[i].Locked = true;
                                fpSpread1.Sheets[0].Columns[i].BackColor = Color.White;
                                fpSpread1.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                fpSpread1.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            }
                        }
                    }

                    for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적") + "|3");
                        }
                    }
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if ((SystemBase.Validation.FPGrid_SaveCheck_NEW(fpSpread1, this.Name, "fpSpread1", true) == true))// 그리드 필수항목 체크 
            {
                string strChange = "N";

                for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
                {
                    string strHead1 = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                    string strPreLotFlag = "N";	    //LOT여부
                    string strLotFlag = "N";	    //LOT여부
                    

                    if (strHead1.Length > 0)
                    {
                        strPreLotFlag = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "LOT YN")].Text;

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
                        {
                            strLotFlag = "Y";
                        }

                        if (strPreLotFlag == "N" && strLotFlag == "Y")
                        {
                            strChange = "Y";
                            break;
                        }

                    }
                }

                if (strChange == "Y")
                {
                    DialogResult Rtn = MessageBox.Show("LOT NO 추적 대상으로 지정 할 경우 기초 LOT 재고가 생성 되며 \n\n 관리자만 LOT NO 추적 취소를 지정 할 수 있습니다. \n\n신중하게 선택 하십시요. \n\n진행 하시겠습니까?", "LOT 재고 생성", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (Rtn != DialogResult.Yes)
                    {
                        this.Cursor = Cursors.Default;
                        return;
                    }
                }


                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                string strItemCd = "";

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

                            string RecvFlag = "N";	//수입검사
                            string ProdFlag = "N";	//공정검사
                            string ShipFlag = "N";	//출고검사

                            string LotFlag = "N";	    //LOT여부
                            string SerialFlag = "N";	//Serial여부

                            strItemCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수입검사")].Text == "True")
                            { RecvFlag = "Y"; }
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정검사")].Text == "True")
                            { ProdFlag = "Y"; }
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고검사")].Text == "True")
                            { ShipFlag = "Y"; }
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
                            { LotFlag = "Y"; }
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Serial 추적")].Text == "True")
                            { SerialFlag = "Y"; }

                            string strSql = " usp_BBI003 '" + strGbn + "'";
                            strSql = strSql + ", @pLANG_CD  = '" + SystemBase.Base.gstrLangCd + "'";
                            strSql = strSql + ", @pDRAW_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호")].Text + "' ";
                            strSql = strSql + ", @pNIIN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "국가재고번호")].Text + "' ";
                            strSql = strSql + ", @pQUALITY_FIG_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질FIG_NO")].Text + "' ";
                            strSql = strSql + ", @pDPGB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단품구분")].Value + "' ";
                            strSql = strSql + ", @pMATL_INSP_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기계/전자구분")].Value + "' ";       // 2020.06.03. hma 추가: 기계검사, 전자검사 구분
                            strSql = strSql + ", @pSET_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "뭉치명")].Text + "' ";
                            strSql = strSql + ", @pTB_PIC_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "교범그림번호")].Text + "' ";
                            strSql = strSql + ", @pEXAM_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사구분")].Text + "' ";
                            strSql = strSql + ", @pPLANT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Text + "' ";
                            strSql = strSql + ", @pITEM_CD = '" + strItemCd + "' ";
                            strSql = strSql + ", @pFINAL_INSP_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "최종검사")].Value + "' ";
                            strSql = strSql + ", @pRECV_INSP_FLAG = '" + RecvFlag + "' ";
                            strSql = strSql + ", @pPROD_INSP_FLAG = '" + ProdFlag + "' ";
                            strSql = strSql + ", @pSHIP_INSP_FLAG = '" + ShipFlag + "' ";
                            strSql = strSql + ", @pINS_MFG_LT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "최종검사L/T")].Value + "' ";
                            strSql = strSql + ", @pINS_PUR_LT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수입검사L/T")].Value + "' ";
                            strSql = strSql + ", @pCHLT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출하L/T")].Value + "'";
                            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            strSql = strSql + ", @pQA_REQ_REMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질보증요구사항")].Text + "' ";
                            strSql = strSql + ", @pLOT_YN = '" + LotFlag + "' ";
                            strSql = strSql + ", @pSERIAL_NO_YN = '" + SerialFlag + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }

                    Trans.Commit();
                    SearchExec();

                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strItemCd);
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

        #region fpSpread1_ButtonClicked
        protected override void fpButtonClick(int Row, int Column)
        {
            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Serial 추적")].Text == "True")
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text = "True";
        }
        #endregion

        #region fpSpread1_CellClick
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        if (fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
                        {
                            if (e.ColumnHeader == true && e.Column == 5)
                            {
                                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                                {
                                    if (fpSpread1.Sheets[0].Cells[i, e.Column].Locked == false)
                                    {
                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text != "True")
                                        {
                                            fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Org;
                                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Serial 추적")].Text = "false";
                                        }
                                        else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
                                        {
                                            fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                                            fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Update;
                                        }
                                    }
                                }
                            }
                            else if (e.ColumnHeader == true && e.Column == 6)
                            {

                                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                                {
                                    if (fpSpread1.Sheets[0].Cells[i, e.Column].Locked == false)
                                    {
                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Serial 추적")].Text != "True")
                                        {
                                            fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Org;
                                        }
                                        else
                                        {
                                            fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text = "True";
                                            fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Update;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

    }
}