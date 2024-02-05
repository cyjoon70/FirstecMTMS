#region 작성정보
/*********************************************************************/
// 단위업무명 : 외주공정의뢰등록/출력
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-14
// 작성내용 : 외주공정의뢰등록/출력 및 관리
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

namespace PC.PCC009
{  
    public partial class PCC009P1 : UIForm.FPCOMM1
    {
        FarPoint.Win.Spread.FpSpread fpGrid = null;
        public PCC009P1(FarPoint.Win.Spread.FpSpread fpRtrGrid)
        {
            fpGrid = fpRtrGrid;
            InitializeComponent();           
        }

        #region Form Load 시
        private void PCC009P1_Load(object sender, System.EventArgs e)
        { 
            //GroupBo x1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.Buttons.ReButton("010000001001",  BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            this.Text = "공정외주참조 조회";

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;

            dtpDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(-6).ToShortDateString().Substring(0,10);
            dtpDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(6).ToShortDateString().Substring(0, 10);

            SystemBase.ComboMake.C1Combo(cboStatus, "usp_P_COMMON @pTYPE = 'P150'  AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시상태

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT유형
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "통화")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'", 0);//화폐단위

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 5, false);
	
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            Search(true);
        }
        #endregion

        #region 조회함수
        private void Search(bool Msg)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string resultYn = "Y";
                if (rdoAll.Checked == true)
                { resultYn = "N"; }

                string strMQuery = "";
                strMQuery = "   usp_PCC009 @pTYPE = 'S4'";
                strMQuery += ",            @pDT_FR = '" + dtpDtFr.Text + "' ";
                strMQuery += ",            @pDT_TO = '" + dtpDtTo.Text + "' ";
                strMQuery += ",            @pENT_CD = '" + txtEntCd.Text + "' ";
                strMQuery += ",            @pWORKORDER_NO = '" + txtWorkOrderNo.Text + "' ";
                strMQuery += ",            @pITEM_CD = '" + txtItemCd.Text + "' ";
                strMQuery += ",            @pWC_CD = '" + txtWcCd.Text + "' ";
                strMQuery += ",            @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                strMQuery += ",            @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                strMQuery += ",            @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                strMQuery += ",            @pORDER_STATUS = '" + cboStatus.SelectedValue.ToString() + "' ";
                strMQuery += ",            @pRESULT_YN = '" + resultYn + "' ";
                strMQuery += ",            @pGROUP_CD = '" + txtGroupCd.Text + "' ";
                strMQuery += ",            @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 5, true);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정실적수량")].Value) > 0
                            || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "CHK")].Text == "1")
                        {
                            fpSpread1.Sheets[0].Cells[i, 1].Locked = false;
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[i, 1].Locked = true;
                        }
                    }
                }

                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 조회조건 팝업
        //공장
        private void btnPlant_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P013', @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "'  AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'"; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };											  // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtPlantCd.Text, "" };															  // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회", false);

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

                    txtWorkOrderNo.Text = Msgs[1].ToString();
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
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO' , @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEntCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEntCd.Text = Msgs[0].ToString();
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

                    txtEntCd.Text = Msgs[1].ToString();
                    txtEntNm.Value = Msgs[2].ToString();
                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = Msgs[5].ToString();
                    txtGroupCd.Text = Msgs[6].ToString();
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

                    txtItemCd.Text = Msgs[2].ToString();
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

        //작업장
        private void btnWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002'  AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCd.Text = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                    txtWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, "");
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
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, "");
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
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, "");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
                if (txtProjectNm.Text == "")
                {
                    txtEntCd.Text = "";
                    txtEntNm.Value = "";
                    txtProjectNm.Value = "";
                    txtProjectSeq.Text = "";
                    txtGroupCd.Text = "";
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
                if (txtPlantCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, "");
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

        //작업장
        private void txtWcCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWcCd.Text != "")
                {
                    txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P002' ");
                }
                else
                {
                    txtWcNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtGroupCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtGroupCd.Text != "")
                {
                    txtGroupNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtGroupCd.Text, "");
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

        #region 확인(선택된 값 넘기기)
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                int Row = fpGrid.Sheets[0].RowCount;
                bool Realchk = true;

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True") //체크박스를 체크했다면
                    {

                        Realchk = true;

                        for (int j = 0; j < fpGrid.Sheets[0].RowCount; j++)
                        {
                            if (fpGrid.Sheets[0].Cells[j, 2].Text == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text
                                && fpGrid.Sheets[0].Cells[j, 3].Text == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text)
                            {
                                Realchk = false;
                            }

                        }

                        if (Realchk == true)
                        {
                            fpGrid.Sheets[0].Rows.Count = fpGrid.Sheets[0].Rows.Count + 1;
                            fpGrid.Sheets[0].RowHeader.Cells[Row, 0].Text = "I";

                            fpGrid.Sheets[0].Cells[Row, 2].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text;
                            fpGrid.Sheets[0].Cells[Row, 3].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text;
                            fpGrid.Sheets[0].Cells[Row, 4].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                            fpGrid.Sheets[0].Cells[Row, 5].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text;
                            fpGrid.Sheets[0].Cells[Row, 6].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text;
                            fpGrid.Sheets[0].Cells[Row, 7].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text;
                            fpGrid.Sheets[0].Cells[Row, 8].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text;
                            fpGrid.Sheets[0].Cells[Row, 9].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text;
                            fpGrid.Sheets[0].Cells[Row, 10].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자원")].Text;
                            fpGrid.Sheets[0].Cells[Row, 11].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text;
                            fpGrid.Sheets[0].Cells[Row, 12].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업명")].Text;
                            fpGrid.Sheets[0].Cells[Row, 13].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "착수예정일")].Text;
                            fpGrid.Sheets[0].Cells[Row, 14].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "완료예정일")].Text;
                            fpGrid.Sheets[0].Cells[Row, 15].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드")].Text;
                            fpGrid.Sheets[0].Cells[Row, 17].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처명")].Text;

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정실적수량")].Value.ToString() == "0.0000"
                                || Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정실적수량")].Value) == 0.0)
                            {
                                fpGrid.Sheets[0].Cells[Row, 18].Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정오더수량")].Value)
                                                                            - Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "현공정실적수량")].Value);
                            }
                            else
                            {
                                fpGrid.Sheets[0].Cells[Row, 18].Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정실적수량")].Value)
                                                                            - Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "현공정실적수량")].Value);
                            }

                            fpGrid.Sheets[0].Cells[Row, 19].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가")].Value;
                            fpGrid.Sheets[0].Cells[Row, 20].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액")].Value;

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통화")].Text == "")
                            {
                                fpGrid.Sheets[0].Cells[Row, 21].Value = "KRW";
                            }
                            else
                            {
                                fpGrid.Sheets[0].Cells[Row, 21].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통화")].Value;
                            }
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Text == "")
                            {
                                fpGrid.Sheets[0].Cells[Row, 22].Value = "A";
                            }
                            else
                            {
                                fpGrid.Sheets[0].Cells[Row, 22].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Value;
                            }

                            fpGrid.Sheets[0].Cells[Row, 23].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정문서")].Text;
                            fpGrid.Sheets[0].Cells[Row, 24].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정규격")].Text;
                            fpGrid.Sheets[0].Cells[Row, 25].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                            fpGrid.Sheets[0].Cells[Row, 26].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
                            fpGrid.Sheets[0].Cells[Row, 27].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text;
                            fpGrid.Sheets[0].Cells[Row, 28].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품명")].Text;

                            Row++;
                        }
                    }
                }

                strFormClosingMsg = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        #endregion

        #region 종료
        private void PCC009P1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            strFormClosingMsg = false;
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion        

    }
}
