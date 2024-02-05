#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 공장별품목정보
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-01
// 작성내용 : 공장별품목정보 조회
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
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

#region 예제 - 복사해서 쓰세요
/*
try
{
    WNDW.WNDW005 pu = new WNDW.WNDW005();
    pu.ShowDialog();
    if (pu.DialogResult == DialogResult.OK)
    {
        string[] Msgs = pu.ReturnVal;

        textBox1.Text = Msgs[1].ToString();
        textBox2.Value = Msgs[2].ToString();
    }
}
catch (Exception f)
{
    SystemBase.Loggers.Log(this.Name, f.ToString());
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장별품목정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 공장별품목정보조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 공장코드 </para>
    /// <para>Msgs[2] = 품목코드 </para>
    /// <para>Msgs[3] = 품목명 </para>
    /// </summary>

    public partial class WNDW005 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        string strAcct = "", strPlant_Cd = "";
        string strItemCd = "";

        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수
        #endregion

        #region WNDW005 생성자
        public WNDW005(string strItemAcc)
        {
            //품목계정값 10-제품, 20-반제품, 25-재공품, 30-원자재, 33-저장품, 35-부자재, 50-상품, 60-포장재, 70-공구소모품 

            strAcct = strItemAcc;

            InitializeComponent();
        }

        public WNDW005(string strPlantCd, bool chk)
        {
            //공장

            strPlant_Cd = strPlantCd;

            InitializeComponent();
        }

        public WNDW005(string ITEM_CD, string strItemAcc)
        {
            //품목계정값 10-제품, 20-반제품, 25-재공품, 30-원자재, 33-저장품, 35-부자재, 50-상품, 60-포장재, 70-공구소모품 

            strAcct = strItemAcc;
            strItemCd = ITEM_CD;

            InitializeComponent();
        }

        public WNDW005(string strPlantCd, bool chk, string ITEM_CD)
        {
            //공장

            strPlant_Cd = strPlantCd;
            strItemCd = ITEM_CD;

            InitializeComponent();
        }

        public WNDW005(string strPlantCd, string strItemAcc, string ITEM_CD)
        {
            //공장

            strAcct = strItemAcc;
            strPlant_Cd = strPlantCd;
            strItemCd = ITEM_CD;

            InitializeComponent();
        }

        public WNDW005()
        {
            strAcct = "";		//퀴리문 없는조건

            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW005_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //조회 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlant, "usp_B_COMMON @pTYPE='PLANT'");	//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 3); //품목계정
            SystemBase.ComboMake.C1Combo(cboItemType, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B011', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 3); //조달구분
            SystemBase.ComboMake.C1Combo(cboProdEnv, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B041', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 3); //생산전략
            SystemBase.ComboMake.C1Combo(cboInspFlag, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'Q001', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 3); //검사구분
            SystemBase.ComboMake.C1Combo(cboTrarking, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B029', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 3); //전용품유무

            if (strPlant_Cd == "")
            { cboPlant.SelectedValue = SystemBase.Base.gstrPLANT_CD; }
            else
            { cboPlant.SelectedValue = strPlant_Cd; }

            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='PLANT', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");	//공장	
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");	//품목계정
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'B011', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");	//조달구분
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");	//재고단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "생산전략")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'B041', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");//생산전략
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='B032', @pSPEC1 = '" + SystemBase.Base.gstrPLANT_CD + "', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");//입고창고
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "출고창고")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='B032', @pSPEC1 = '" + SystemBase.Base.gstrPLANT_CD + "', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");//출고창고
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "입고위치")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='TABLE', @pSPEC1 = 'B_LOCATION_INFO', @pCODE='LOCATION_CD', @pNAME = 'LOCATION_NM', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ");//입고위치
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "출고위치")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='TABLE', @pSPEC1 = 'B_LOCATION_INFO', @pCODE='LOCATION_CD', @pNAME = 'LOCATION_NM', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ");//출고위치
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "최종검사")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'Q013', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");//출고위치

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            string strTitle = "";
            if (strAcct != "")
            {
                cboItemAcct.SelectedValue = strAcct;
                //				cboItemAcct.Enabled = false;

                strTitle = "공장별 ";
                strTitle += SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", strAcct, " AND MAJOR_CD = 'B036' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ");
                strTitle += " 정보 조회";
                this.Text = strTitle;
            }
            else
            {
                this.Text = "공장별 품목 정보 조회";
            }

            txtItemCd.Text = strItemCd;
            dtpDate.Text = DateTime.Now.ToShortDateString();
            Grid_search(false);

            txtItemCd.Focus();
        }
        #endregion

        #region 조회버튼 클릭
        protected override void SearchExec()
        { Grid_search(true); }
        #endregion

        #region 그리드 더블클릭
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            RtnStr(e.Row);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region 그리드 선택값 입력밑 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(int R)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                returnVal = new string[fpSpread1.Sheets[0].Columns.Count];
                for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                {
                    returnVal[i] = Convert.ToString(fpSpread1.Sheets[0].Cells[R, i].Value);
                }
            }
        }
        #endregion

        #region 조회함수
        private void Grid_search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                SDown = 1;

                string strPlant = ""; if (cboPlant.Text != "") strPlant = cboPlant.SelectedValue.ToString();
                string strAcct = ""; if (cboItemAcct.Text != "") strAcct = cboItemAcct.SelectedValue.ToString();
                string strItemType = ""; if (cboItemType.Text != "") strItemType = cboItemType.SelectedValue.ToString();
                string strProdEnv = ""; if (cboProdEnv.Text != "") strProdEnv = cboProdEnv.SelectedValue.ToString();
                string strTrarking = ""; if (cboTrarking.Text != "") strTrarking = cboTrarking.SelectedValue.ToString();
                string strInspFlag = ""; if (cboInspFlag.Text != "") strInspFlag = cboInspFlag.SelectedValue.ToString();
                string strInspSql = "";
                switch (strInspFlag)
                {
                    case "P": strInspSql = ", @pPROD_INSP_FLAG = 'Y'"; break;
                    case "R": strInspSql = ", @pRECV_INSP_FLAG = 'Y'"; break;
                    case "F": strInspSql = ", @pFINAL_INSP_FLAG = 'Y'"; break;
                    case "S": strInspSql = ", @pSHIP_INSP_FLAG = 'Y'"; break;
                    default: strInspSql = ""; break;
                }

                string strQuery = " usp_WNDW005 'S1'";
                strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pPLANT_CD ='" + strPlant + "'";
                strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                strQuery += ", @pITEM_NM ='" + txtItemNm.Text + "'";
                strQuery += ", @pITEM_ACCT ='" + strAcct + "'";
                strQuery += ", @pITEM_TYPE ='" + strItemType + "'";
                strQuery += ", @pITEM_SPEC ='" + txtItemSpec.Text.Trim() + "'";
                strQuery += ", @pPROD_ENV ='" + strProdEnv + "'";
                strQuery += ", @pDRAW_NO ='" + txtDrawNo.Text.Trim() + "'";
                strQuery += ", @pTRACKING_FLAG ='" + strTrarking + "'";
                strQuery += ", @pDATE = '" + dtpDate.Text + "' ";
                strQuery += ", @pNIIN = '" + txtNiin.Text + "' ";
                strQuery += strInspSql;
                strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Text에서 Enter시 조회
        private void txtItemCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtItemNm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtItemSpec_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtDrawNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtNiin_KeyDown(object sender, KeyEventArgs e)
        {if (e.KeyCode == Keys.Enter) Grid_search(true); }
        #endregion

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                SDown++;

                this.Cursor = Cursors.WaitCursor;

                string strPlant = ""; if (cboPlant.Text != "") strPlant = cboPlant.SelectedValue.ToString();
                string strAcct = ""; if (cboItemAcct.Text != "") strAcct = cboItemAcct.SelectedValue.ToString();
                string strItemType = ""; if (cboItemType.Text != "") strItemType = cboItemType.SelectedValue.ToString();
                string strProdEnv = ""; if (cboProdEnv.Text != "") strProdEnv = cboProdEnv.SelectedValue.ToString();
                string strTrarking = ""; if (cboTrarking.Text != "") strTrarking = cboTrarking.SelectedValue.ToString();
                string strInspFlag = ""; if (cboInspFlag.Text != "") strInspFlag = cboInspFlag.SelectedValue.ToString();
                string strInspSql = "";
                switch (strInspFlag)
                {
                    case "P": strInspSql = ", @pPROD_INSP_FLAG = 'Y'"; break;
                    case "R": strInspSql = ", @pRECV_INSP_FLAG = 'Y'"; break;
                    case "F": strInspSql = ", @pFINAL_INSP_FLAG = 'Y'"; break;
                    case "S": strInspSql = ", @pSHIP_INSP_FLAG = 'Y'"; break;
                    default: strInspSql = ""; break;
                }

                string strQuery = " usp_WNDW005 'S1'";
                strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pPLANT_CD ='" + strPlant + "'";
                strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                strQuery += ", @pITEM_NM ='" + txtItemNm.Text + "'";
                strQuery += ", @pITEM_ACCT ='" + strAcct + "'";
                strQuery += ", @pITEM_TYPE ='" + strItemType + "'";
                strQuery += ", @pITEM_SPEC ='" + txtItemSpec.Text.Trim() + "'";
                strQuery += ", @pPROD_ENV ='" + strProdEnv + "'";
                strQuery += ", @pDRAW_NO ='" + txtDrawNo.Text.Trim() + "'";
                strQuery += ", @pTRACKING_FLAG ='" + strTrarking + "'";
                strQuery += ", @pDATE = '" + dtpDate.Text + "' ";
                strQuery += ", @pNIIN = '" + txtNiin.Text + "' ";
                strQuery += strInspSql;
                strQuery += ", @pTOPCOUNT ='" + AddRow * SDown + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                this.Cursor = Cursors.Default;
            }
        }
        #endregion
    }
}
