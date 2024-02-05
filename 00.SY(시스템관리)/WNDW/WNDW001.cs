#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 품목정보
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-01-30
// 작성내용 : 품목정보 조회
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
    WNDW.WNDW001 pu = new WNDW.WNDW001();
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
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 품목정보조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 품목코드 </para>
    /// <para>Msgs[2] = 품목명 </para>
    /// <para>Msgs[3] = 품목전명 </para>
    /// <para>Msgs[4] = 품목계정 </para>
    /// <para>Msgs[5] = 품목규격 </para>
    /// <para>Msgs[6] = 품목단위 </para>
    /// </summary>

    public partial class WNDW001 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        string strAcct = "";

        int SDown = 1;		    // 조회 횟수
        int AddRow = 100;	    // 조회 건수
        string strtype = "S1";  // 조회 조건
        string strItemCd = "";
        #endregion

        #region 생성자
        public WNDW001(string strItemAcc)
        {
            //품목계정값 10-제품, 20-반제품, 25-재공품, 30-원자재, 33-저장품, 35-부자재, 50-상품, 60-포장재, 70-공구소모품, CUST- 거래처품목

            strAcct = strItemAcc;

            InitializeComponent();
        }

        public WNDW001(string ItemCd, string strItemAcc)
        {
            //품목계정값 10-제품, 20-반제품, 25-재공품, 30-원자재, 33-저장품, 35-부자재, 50-상품, 60-포장재, 70-공구소모품, CUST- 거래처품목

            strItemCd = ItemCd;
            strAcct = strItemAcc;

            InitializeComponent();
        }

        public WNDW001()
        {
            strAcct = "";		//퀴리문 없는조건

            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW001_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch,  BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ", 3); //품목계정
            SystemBase.ComboMake.C1Combo(cboItemGrp1, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B037', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "'", 3); //품목그룹1

            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'"); //품목계정
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");//단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목그룹1")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'B037', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'"); //품목그룹1
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목그룹2")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='REL', @pCODE = 'B038', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");//품목그룹2

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            string strTitle = "";
            if (strAcct != "")
            {
                if (strAcct == "CUST")
                {
                    this.Text = "고객 품목 정보 조회";
                    strtype = "S2";
                }
                else
                {
                    cboItemAcct.SelectedValue = strAcct;
                    cboItemAcct.Enabled = false;

                    strTitle = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", strAcct, " AND MAJOR_CD = 'B036' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ");
                    strTitle += " 정보 조회";
                    this.Text = strTitle;
                }
            }
            else
            {
                this.Text = "품목 정보 조회";
            }

            txtItemCd.Text = strItemCd;
            Grid_search(strtype, false);

            txtItemCd.Focus();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        { Grid_search(strtype, true); }
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
        private void Grid_search(string strType, bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                SDown = 1;

                string strAcct = ""; if (cboItemAcct.Text != "") strAcct = cboItemAcct.SelectedValue.ToString();
                string strItemGrp1 = ""; if (cboItemGrp1.Text != "") strItemGrp1 = cboItemGrp1.SelectedValue.ToString();

                string strQuery = " usp_WNDW001 '" + strType + "'";

                strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                strQuery += ", @pITEM_NM ='" + txtItemNm.Text + "'";
                strQuery += ", @pITEM_ACCT ='" + strAcct + "'";
                strQuery += ", @pITEM_SPEC ='" + txtItemSpec.Text.Trim() + "'";
                strQuery += ", @pITEM_GRP1 ='" + strItemGrp1 + "' ";
                strQuery += ", @pDRAW_NO ='" + txtDrawNo.Text.Trim() + "'";
                strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                SDown++;

                this.Cursor = Cursors.WaitCursor;

                string strAcct = ""; if (cboItemAcct.Text != "") strAcct = cboItemAcct.SelectedValue.ToString();
                string strItemGrp1 = ""; if (cboItemGrp1.Text != "") strItemGrp1 = cboItemGrp1.SelectedValue.ToString();

                string strQuery = " usp_WNDW001 'S1'";

                strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                strQuery += ", @pITEM_NM ='" + txtItemNm.Text + "'";
                strQuery += ", @pITEM_ACCT ='" + strAcct + "'";
                strQuery += ", @pITEM_SPEC ='" + txtItemSpec.Text.Trim() + "'";
                strQuery += ", @pITEM_GRP1 ='" + strItemGrp1 + "' ";
                strQuery += ", @pDRAW_NO ='" + txtDrawNo.Text.Trim() + "'";
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
