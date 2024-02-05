
#region 작성정보
/*********************************************************************/
// 단위업무명 : 공급처별단가조회
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-16
// 작성내용 : 공급처별단가조회 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

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

namespace MA.MAB003
{
    public partial class MAB003 : UIForm.FPCOMM1
    {
        #region 변수선언
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수
        int iHead_Cnt = 0;
        #endregion

        public MAB003()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MAB003_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
            dtpApplyDt.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
            dtpApplyDt.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region 조회조건 팝업
        //공장
        private void btnPlantCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP' ,@pSPEC1 = 'PLANT_CD', @pSPEC2 = 'PLANT_NM', @pSPEC3 = 'B_PLANT_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPlantCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장코드 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPlantCd.Text = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //품목 From
        private void btnItemCdFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtPlantCd.Text, true, txtItemCdFr.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCdFr.Text = Msgs[2].ToString();
                    txtItemNmFr.Value = Msgs[3].ToString();
                    txtItemCdFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //품목 To
        private void btnItemCdTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtPlantCd.Text, true, txtItemCdTo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCdTo.Text = Msgs[2].ToString();
                    txtItemNmTo.Value = Msgs[3].ToString();
                    txtItemCdTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }
        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트차수
        private void btnProjectSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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
                    txtProjectSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //거래처 From
        private void btnCustCdFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCdFr.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCdFr.Text = Msgs[1].ToString();
                    txtCustNmFr.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //거래처 To
        private void btnCustCdTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCdTo.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCdTo.Text = Msgs[1].ToString();
                    txtCustNmTo.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //공장
        private void txtPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        //품목 From
        private void txtItemCdFr_TextChanged(object sender, System.EventArgs e)
        {
            txtItemNmFr.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        //품목 To
        private void txtItemCdTo_TextChanged(object sender, System.EventArgs e)
        {
            txtItemNmTo.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

            if (txtProjectNm.Text == "")
                txtProjectSeq.Text = "";
        }

		//거래처 From
		private void txtCustCdFr_TextChanged(object sender, System.EventArgs e)
		{
            txtCustNmFr.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCdFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");	
		}

		//거래처 To
		private void txtCustCdTo_TextChanged(object sender, System.EventArgs e)
		{
            txtCustNmTo.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}        
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    SDown = 1;

                    string strQuery = "usp_MAB003 @pTYPE = 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pPLANT_CD = '" + txtPlantCd.Text.Trim() + "'";
                    strQuery += ", @pAPPLY_DT = '" + dtpApplyDt.Text.Trim() + "'";
                    strQuery += ", @pITEM_CD_FR = '" + txtItemCdFr.Text.Trim() + "'";
                    strQuery += ", @pITEM_CD_TO = '" + txtItemCdTo.Text.Trim() + "'";
                    strQuery += ", @pCUST_CD_FR = '" + txtCustCdFr.Text.Trim() + "'";
                    strQuery += ", @pCUST_CD_TO = '" + txtCustCdTo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text.Trim() + "'";
                    strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    iHead_Cnt = 0;

                    if (fpSpread1.Sheets[0].RowCount > 0) Set_Section(SDown);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }

        }
        #endregion

        #region 그리드 머지
        private void Set_Section(int SDown)
        {
            int iItem_Row = 0;

            string strItem_Cd = "";
            string strCust_Cd = "";

            for (int i = SDown; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                //첫 행
                if (i == 0)
                {
                    strItem_Cd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text;
                    strCust_Cd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text;
                }
                //마지막 행
                else if (i == fpSpread1.Sheets[0].RowCount - 1)
                {
                    if (iItem_Row == 0)
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[i - iItem_Row - 1, 0].Text = Convert.ToString(iHead_Cnt + 1);
                        fpSpread1.Sheets[0].RowHeader.Cells[i - iItem_Row, 0].Text = Convert.ToString(iHead_Cnt + 2);
                    }
                    else
                    {
                        //품목별 셀 병합
                        Set_ITEM_MURGE(i, iItem_Row, iHead_Cnt);
                    }
                }
                else
                {
                    //거래처가 같을 경우
                    if (strCust_Cd == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text)
                    {
                        //품목이 같을 경우
                        if (strItem_Cd == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text)
                        {
                            iItem_Row = iItem_Row + 1;
                        }
                        //품목이 달라지면 그리드 머지
                        else
                        {
                            if (iItem_Row == 0)
                            {
                                fpSpread1.Sheets[0].RowHeader.Cells[i - iItem_Row - 1, 0].Text = Convert.ToString(iHead_Cnt + 1);
                            }
                            else
                            {
                                //품목별 셀 병합
                                Set_ITEM_MURGE(i, iItem_Row, iHead_Cnt);
                            }

                            strItem_Cd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text;
                            iHead_Cnt = iHead_Cnt + 1;
                            iItem_Row = 0;
                        }
                    }
                    //거래처가 달라질 경우
                    else
                    {
                        if (iItem_Row == 0)
                        {
                            fpSpread1.Sheets[0].RowHeader.Cells[i - iItem_Row - 1, 0].Text = Convert.ToString(iHead_Cnt + 1);
                        }
                        else
                        {
                            //품목별 셀 병합
                            Set_ITEM_MURGE(i, iItem_Row, iHead_Cnt);
                        }

                        iHead_Cnt = iHead_Cnt + 1;

                        strItem_Cd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text;
                        strCust_Cd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text;

                        iItem_Row = 0;
                    }
                }
            }
        }
        #endregion

        #region 그리드 품목별 셀 병합
        private void Set_ITEM_MURGE(int iRow, int iCnt, int iHead_Cnt)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[iRow - iCnt - 1, 0].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].RowHeader.Cells[iRow - iCnt - 1, 0].Text = Convert.ToString(iHead_Cnt + 1);

            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "구매단위")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")].RowSpan = iCnt + 1;
        }
        #endregion

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {

            try
            {
                int FPHeight = (fpSpread1.Size.Height - 28) / 20;
                if (e.NewTop >= ((AddRow * SDown) - FPHeight))
                {
                    int cnt_prev = AddRow * SDown;
                    SDown++;
                    int cnt = AddRow * SDown;

                    this.Cursor = Cursors.WaitCursor;

                    string strQuery = "usp_MAB003 @pTYPE = 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pPLANT_CD = '" + txtPlantCd.Text.Trim() + "'";
                    strQuery += ", @pAPPLY_DT = '" + dtpApplyDt.Text.Trim() + "'";
                    strQuery += ", @pITEM_CD_FR = '" + txtItemCdFr.Text.Trim() + "'";
                    strQuery += ", @pITEM_CD_TO = '" + txtItemCdTo.Text.Trim() + "'";
                    strQuery += ", @pCUST_CD_FR = '" + txtCustCdFr.Text.Trim() + "'";
                    strQuery += ", @pCUST_CD_TO = '" + txtCustCdTo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text.Trim() + "'";
                    strQuery += ", @pTOPCOUNT ='" + cnt + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);

                    if (fpSpread1.Sheets[0].RowCount > cnt_prev) Set_Section(SDown);
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
            }
        }
        #endregion
    }
}
