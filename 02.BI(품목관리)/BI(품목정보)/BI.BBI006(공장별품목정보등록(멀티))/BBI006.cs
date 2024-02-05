#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목정보등록(멀티)
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-01
// 작성내용 : 품목 정보 등록 및 관리
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
using System.Data.OleDb;

namespace BI.BBI006
{
    public partial class BBI006 : UIForm.FPCOMM1
    {
        #region 생성자
        public BBI006()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBI006_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용

            SystemBase.ComboMake.C1Combo(cboPlant, "usp_B_COMMON @pTYPE = 'PLANT', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ");	//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 3); //품목계정

            cboPlant.SelectedValue = SystemBase.Base.gstrPLANT_CD;

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM'  , @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM2' , @pCODE = 'P032', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM'  , @pCODE = 'B011', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재질구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM'  , @pCODE = 'D035', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "생산전략")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM',  @pCODE ='B041', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "Lot Size")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM2', @pCODE ='B022', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM' , @pCODE ='B030', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "출고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM' , @pCODE ='Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM' , @pCODE ='Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE ='Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE ='Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "최종검사")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q013', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            // 2016.05.30. hma 추가: 입고방법 항목이 리스트로 처리되도록 함. 출고방법과 동일한 공통코드 사용
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "반제품자동입고방법")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM' , @pCODE ='B030', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);

            //원가정보 추가
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "양산구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM',  @pCODE ='B060', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "통합원가부품구분(계정)")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE ='B061', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "방산물자지정여부")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM' , @pCODE ='B029', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "구매구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM' , @pCODE ='B062', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "시효구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE ='B029', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "ESD구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE ='B029', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "MSL구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B029', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "규격화구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM' , @pCODE ='B063', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "국방도면종류")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE ='B064', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "중량단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE ='Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "부피단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
			
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region RCopyExec() Copy버튼 클릭 이벤트
        protected override void RCopyExe()
        {
            if (fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")].Text != "" && fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")].Value.ToString() == "M")
                UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더단위") + "|1#" + 
                                                                                         SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더L/T") + "|1#" + 
                                                                                         SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더단위") + "|0#" + 
                                                                                         SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T") + "|0#" + 
                                                                                         SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직") + "|0");
            else
                UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더단위") + "|0#" +
                                                                                         SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더L/T") + "|0#" +
                                                                                         SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더단위") + "|1#" +
                                                                                         SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T") + "|1#" +
                                                                                         SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직") + "|1");

            if (fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot Size")].Text != "" && fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot Size")].Value.ToString() == "P")
                UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간") + "|1");
            else
                UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간") + "|2");

            if (fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분")].Text != "" && fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분")].Value.ToString() == "99")
                UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호") + "|1#" +
                                                                                         SystemBase.Base.GridHeadIndex(GHIdx1, "도면REV") + "|1"); //품목구분
            else
                UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호") + "|0#" +
                                                                                         SystemBase.Base.GridHeadIndex(GHIdx1, "도면REV") + "|0"); //품목구분
        }
        #endregion

        #region RowInsExec() RowInsert 버튼 클릭 이벤트
        protected override void RowInsExe()
        {
            try
            {
                fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간")].Value = 0;
                fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "방산물지정여부")].Value = "N";
                fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "시효구분")].Value = "N";
                fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "ESD구분")].Value = "N";
                fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "MSL구분")].Value = "N";
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BBI006  'S1'";
                strQuery = strQuery + ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "' ";
                strQuery = strQuery + ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "' ";
                strQuery = strQuery + ", @pPLANT_CD ='" + cboPlant.SelectedValue.ToString() + "' ";
                strQuery = strQuery + ", @pITEM_NM ='" + txtItemNm.Text.Trim() + "' ";
                strQuery = strQuery + ", @pDRAW_NO ='" + txtDrawNo.Text.Trim() + "' ";
                strQuery = strQuery + ", @pITEM_SPEC ='" + txtItemSpec.Text.Trim() + "' ";
                strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                if (fpSpread1.Sheets[0].RowCount > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")].Text != "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")].Value.ToString() == "M")
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더단위") + "|1#" +
                                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더L/T") + "|1#" +
                                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더단위") + "|0#" +
                                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T") + "|0#" +
                                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직") + "|0");
                        else
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더단위") + "|0#" +
                                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더L/T") + "|0#" +
                                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더단위") + "|1#" +
                                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T") + "|1#" +
                                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직") + "|1");

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot Size")].Text != "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot Size")].Value.ToString() == "P")
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간") + "|1");
                        else
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간") + "|2");

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분")].Text != "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분")].Value.ToString() == "99")
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호") + "|1#" +
                                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "도면REV") + "|1"); //품목구분
                        else
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호") + "|0#" +
                                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "도면REV") + "|0");
                    }
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true))// 그리드 필수항목 체크 
            {
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
                                case "I": strGbn = "I1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            strItemCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                            string trackFlag = "N"; //전용품여부 
                            string DPGBFlag = "N"; //단품구분 
                            string RecvFlag = "N";	//수입검사
                            string ProdFlag = "N";	//공정검사
                            string ShipFlag = "N";	//출고검사
                            string GovernmentFlag = "N";	//관급품여부

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전용품여부")].Text == "True")
                            { trackFlag = "Y"; }
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단품구분")].Text == "True")
                            { DPGBFlag = "Y"; }
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관급품여부")].Text == "True")
                            { GovernmentFlag = "Y"; }

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수입검사")].Text == "True")
                            { RecvFlag = "Y"; }
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정검사")].Text == "True")
                            { ProdFlag = "Y"; }
                            //if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고검사")].Text == "True")
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출하검사")].Text == "True")     // 2017.08.31. hma 수정
                            { ShipFlag = "Y"; }

                            string strSql = " usp_BBI006 '" + strGbn + "'";
                            strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pLANG_CD  = '" + SystemBase.Base.gstrLangCd + "'";
                            strSql = strSql + ", @pPLANT_CD = '" + cboPlant.SelectedValue + "' ";
                            strSql = strSql + ", @pITEM_CD = '" + strItemCd + "' ";
                            strSql = strSql + ", @pITEM_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text + "' ";
                            strSql = strSql + ", @pITEM_FULL_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목정식명칭")].Text + "' ";
                            strSql = strSql + ", @pITEM_NM_ENG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "영문품목명")].Text + "' ";
                            strSql = strSql + ", @pITEM_SPEC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목규격")].Text + "' ";
                            strSql = strSql + ", @pITEM_ACCT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")].Value + "' ";
                            strSql = strSql + ", @pITEM_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분")].Value + "' ";
                            strSql = strSql + ", @pITEM_TYPE1 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")].Value + "' ";

                            strSql = strSql + ", @pTRACKING_FLAG = '" + trackFlag + "' ";
                            strSql = strSql + ", @pDPGB = '" + DPGBFlag + "' ";
                            strSql = strSql + ", @pPROD_ENV = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산전략")].Value + "' ";
                            strSql = strSql + ", @pLOT_SIZING = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot Size")].Value + "' ";

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간")].Text != "")
                                strSql = strSql + ", @pROUND_PERD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간")].Value + "' ";

                            strSql = strSql + ", @pSL_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text + "'";
                            strSql = strSql + ", @pISSUED_MTHD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법")].Value + "' ";
                            strSql = strSql + ", @pISSUED_SL_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고창고")].Text + "'";
                            strSql = strSql + ", @pISSUED_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고단위")].Value + "' ";
                            strSql = strSql + ", @pSTOCK_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")].Value + "' ";

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고실사주기")].Text != "")
                                strSql = strSql + ", @pCYCLE_CNT_PERD  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고실사주기")].Value + "' ";

                            strSql = strSql + ", @pORDER_MFG_UNIT= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더단위")].Value + "' ";

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더L/T")].Text != "")
                                strSql = strSql + ", @pORDER_MFG_LT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더L/T")].Text + "' ";

                            strSql = strSql + ", @pORDER_PUR_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더단위")].Value + "' ";

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T")].Text != "")
                                strSql = strSql + ", @pORDER_PUR_LT= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T")].Text + "' ";
                            strSql = strSql + ", @pPUR_ORG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직")].Text + "' ";

                            strSql = strSql + ", @pFINAL_INSP_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "최종검사")].Value + "' ";
                            strSql = strSql + ", @pRECV_INSP_FLAG = '" + RecvFlag + "' ";
                            strSql = strSql + ", @pPROD_INSP_FLAG = '" + ProdFlag + "' ";
                            strSql = strSql + ", @pSHIP_INSP_FLAG = '" + ShipFlag + "' ";

                            strSql = strSql + ", @pGOVERNMENT_FLAG = '" + GovernmentFlag + "' ";
                            strSql = strSql + ", @pRCPT_MTHD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제품자동입고방법")].Value + "' ";

                            //2013-03-13 원가정보 추가
                            strSql = strSql + ", @pFSC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "군급분류")].Text + "' ";
                            strSql = strSql + ", @pNIIN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "국가재고번호")].Text + "' ";
                            strSql = strSql + ", @pMTMG_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부품관리번호")].Text + "' ";
                            strSql = strSql + ", @pMASS_PROD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양산구분")].Value + "' ";
                            strSql = strSql + ", @pCOST_ITEM_ACCT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통합원가부품구분(계정)")].Value + "' ";
                            strSql = strSql + ", @pDNNP_APPN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "방산물자지정여부")].Value + "' ";
                            strSql = strSql + ", @pDNNP_AUTHORITY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "방산물자지정근거")].Text + "' ";
                            strSql = strSql + ", @pPUR_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매구분")].Value + "' ";
                            strSql = strSql + ", @pPRESCRIP_YN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시효구분")].Value + "' ";
                            strSql = strSql + ", @pESD_YN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "ESD구분")].Value + "' ";
                            strSql = strSql + ", @pMSL_YN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MSL구분")].Value + "' ";
                            strSql = strSql + ", @pITEM_NM_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품명부호")].Text + "' ";
                            strSql = strSql + ", @pITEM_IDENTIFY_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목식별부호")].Text + "' ";
                            strSql = strSql + ", @pSPEC_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격화구분")].Value + "' ";
                            strSql = strSql + ", @pDNNP_DRAW_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "국방도면종류")].Value + "' ";
                            strSql = strSql + ", @pDNNP_DRAW_ITEM_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "국방도면부품번호")].Text + "' ";
                            strSql = strSql + ", @pDRAW_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호")].Text + "' ";
                            strSql = strSql + ", @pDRAW_REV = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호REV")].Value + "' ";
                            strSql = strSql + ", @pDRAW_REV_DATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호REV DATE")].Text + "' ";
                            strSql = strSql + ", @pMNG_EMP_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "형상식별담당자")].Text + "' ";
                            strSql = strSql + ", @pSPEC_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격번호")].Text + "' ";
                            strSql = strSql + ", @pSPEC_ITEM_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격부품번호")].Text + "' ";
                            strSql = strSql + ", @pSUEN_ITEM_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산자부품번호")].Text + "' ";
                            strSql = strSql + ", @pSUEN_ITEM_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산자품명")].Text + "' ";
                            strSql = strSql + ", @pSUEN_BINO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산자사업자등록번호")].Text + "' ";
                            strSql = strSql + ", @pSUEN_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산자사업자명")].Text + "' ";
                            strSql = strSql + ", @pSUEN_MATL_MARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원자재재질기호")].Text + "' ";
                            strSql = strSql + ", @pSUEN_SPEC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원자재규격")].Text + "' ";
                            strSql = strSql + ", @pMAIN_ITEM_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "주장비명")].Text + "' ";
                            strSql = strSql + ", @pASSY_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조립체명")].Text + "' ";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "중량")].Text != "")
                                strSql = strSql + ", @pWEIGHT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "중량")].Value + "' ";

                            strSql = strSql + ", @pWEIGHT_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "중량단위")].Value + "' ";

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부피")].Text != "")
                                strSql = strSql + ", @pBULK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부피")].Value + "' ";

                            strSql = strSql + ", @pBULK_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부피단위")].Value + "' ";
                            strSql = strSql + ", @pSPECIFICATION = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사양")].Text + "' ";
                            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pTEMP_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "임시품목코드")].Text + "' ";   // 2017.12.19. hma 추가:임시품목코드

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
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

        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'B035', @pSPEC1 = '" + cboPlant.SelectedValue + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00014", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "출고창고_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'B035', @pSPEC1 =  '" + cboPlant.SelectedValue + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고창고")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00014", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고창고")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고창고명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'COMM_POP', @pSPEC1 = 'M001', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00014", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매조직팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
                }
            }

        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.Length >= 2)
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.Substring(0, 2) == "PA" || fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.Substring(0, 2) == "VA")
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재질구분")].BackColor = SystemBase.Validation.Kind_LightCyan;
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재질구분")].BackColor = SystemBase.Validation.Kind_White;
                    }
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고명")].Text = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, Column].Text, " AND CO_CD = '"+ SystemBase.Base.gstrCOMCD +"'");
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "출고창고"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고창고명")].Text = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, Column].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직명")].Text = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, Column].Text, " AND MAJOR_CD = 'M001' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ");
            }
        }

        private void fpSpread1_ComboSelChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분"))
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, e.Column].Text != "" && fpSpread1.Sheets[0].Cells[e.Row, e.Column].Value.ToString() == "M")
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더단위") + "|1#" +
                                                              SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더L/T") + "|1#" +
                                                              SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더단위") + "|0#" +
                                                              SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T") + "|0#" +
                                                              SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직") + "|0");		//제조오더단위, 제조오더 L/T
                else
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더단위") + "|0#" +
                                                              SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더L/T") + "|0#" +
                                                              SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더단위") + "|1#" +
                                                              SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T") + "|1#" +
                                                              SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직") + "|1");	//구매오더단위,구매L/T,구매조직
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Lot Size"))
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, e.Column].Text != "" && fpSpread1.Sheets[0].Cells[e.Row, e.Column].Value.ToString() == "P")
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간") + "|1"); //올림기간
                else
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간") + "|2");
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분"))
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, e.Column].Text != "" && fpSpread1.Sheets[0].Cells[e.Row, e.Column].Value.ToString() == "99")
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호") + "|1#" +
                                                              SystemBase.Base.GridHeadIndex(GHIdx1, "도면REV") + "|1"); 
                else
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호") + "|0#" +
                                                              SystemBase.Base.GridHeadIndex(GHIdx1, "도면REV") + "|0"); 
            }
        }
        #endregion

        #region 엑셀
        private void btnUpload_Click(object sender, System.EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "통합 Excel 문서(*.xls)|*.xls|2007 Excel 문서(*.xlsx)|*.xlsx";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // 2017.11.02. CWL 수정(Start): 윈도우 보안 업데이트후 문제가 생겨서 엑셀 업로드시 OLEDB 부분 수정함.
                    //string connectionString = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;Imex=1;hdr=yes;""", dlg.FileName);
                    // 2019.02.28. hma 수정(Start): 구버전 PC에서도 정상적으로 처리되도록 두가지 버전으로 구분하여 처리되도록 함.
                    //string connectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0;Imex=1;hdr=yes;""", dlg.FileName);
                    string connectionString;
                    if (rdoOld.Checked == true)
                        connectionString = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;Imex=1;hdr=yes;""", dlg.FileName);
                    else
                        connectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0;Imex=1;hdr=yes;""", dlg.FileName);
                    // 2019.02.28. hma 수정(End)
                    // 2017.11.02. CWL 수정(End)
                    OleDbConnection conn = new OleDbConnection(connectionString);
                    conn.Open();

                    DataTable worksheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    string commandString = String.Format("SELECT * FROM [{0}]", worksheets.Rows[0]["TABLE_NAME"]);
                    OleDbCommand cmd = new OleDbCommand(commandString, conn);

                    OleDbDataAdapter dapt = new OleDbDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    dapt.Fill(ds);
                    conn.Close();

                    fpSpread1.Sheets[0].RowCount = 0;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        UIForm.FPMake.RowInsert(fpSpread1);

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = ds.Tables[0].Rows[i][0].ToString().Trim();		//품목코드		0
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목규격")].Text = ds.Tables[0].Rows[i][1].ToString().Trim();		//품목규격		1
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = ds.Tables[0].Rows[i][2].ToString().Trim();			//품목명		2
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목정식명칭")].Text = ds.Tables[0].Rows[i][3].ToString().Trim();	//품목정식명칭	3
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "영문품목명")].Text = ds.Tables[0].Rows[i][4].ToString().Trim();		//품목정식명칭	4
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")].Text = ds.Tables[0].Rows[i][5].ToString().Trim();		//품목계정		5
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분")].Text = ds.Tables[0].Rows[i][6].ToString().Trim();		//품목구분		6
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")].Text = ds.Tables[0].Rows[i][7].ToString().Trim();		//조달구분		7

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")].Text != "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")].Value.ToString() == "M")
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더단위") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더L/T") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더단위") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직") + "|0");
                        else
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더단위") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더L/T") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더단위") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직") + "|1");

                        if (ds.Tables[0].Rows[i][8].ToString().Trim().ToUpper() == "Y")
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전용품여부")].Value = 1; //전용품여부	8

                        if (ds.Tables[0].Rows[i][9].ToString().Trim() == "Y")
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단품구분")].Value = 1;  //단품구분	9

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산전략")].Text = ds.Tables[0].Rows[i][10].ToString().Trim().ToUpper(); //생산전략	10
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot Size")].Text = ds.Tables[0].Rows[i][11].ToString().Trim(); //Lot Size	11

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot Size")].Text != "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot Size")].Value.ToString() == "P")
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간") + "|1");
                        else
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간") + "|3");

                        if (ds.Tables[0].Rows[i][12].ToString().Trim() == "")
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간")].Value = 0; //올림기간	12
                        else
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간")].Value = ds.Tables[0].Rows[i][12];

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")].Value.ToString() == "20") //반제품이면
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제품자동입고방법") + "|1");
                        else
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제품자동입고방법") + "|3");

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제품자동입고방법")].Text = ds.Tables[0].Rows[i][13].ToString().Trim().ToUpper(); //반제품자동입고방법	13

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text = ds.Tables[0].Rows[i][14].ToString().Trim().ToUpper(); //입고창고	14
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고명")].Text = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", ds.Tables[0].Rows[i][14].ToString().Trim(), " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'"); //입고창고명
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고명")].Text.Trim() == "") fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text = "";

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법")].Text = ds.Tables[0].Rows[i][15].ToString().Trim().ToUpper(); //출고방법	15

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고창고")].Text = ds.Tables[0].Rows[i][16].ToString().Trim().ToUpper(); //출고창고	16
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고창고명")].Text = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", ds.Tables[0].Rows[i][16].ToString().Trim(), " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'"); //출고창고명
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고창고명")].Text.Trim() == "") fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고창고")].Text = "";

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고단위")].Text = ds.Tables[0].Rows[i][17].ToString().Trim().ToUpper(); //출고단위	17
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")].Text = ds.Tables[0].Rows[i][18].ToString().Trim().ToUpper(); //재고단위	18

                        if (ds.Tables[0].Rows[i][19].ToString().Trim() == "")
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고실사주기")].Value = 0; //재고실사주기	19
                        else
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고실사주기")].Value = ds.Tables[0].Rows[i][19];

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더단위")].Text = ds.Tables[0].Rows[i][20].ToString().Trim().ToUpper(); //제조오더단위	20

                        if (ds.Tables[0].Rows[i][21].ToString().Trim() == "")
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더L/T")].Value = 0;  //제조오더 L/T 21
                        else
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더L/T")].Value = ds.Tables[0].Rows[i][21]; //제조오더 L/T(일)	21

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더단위")].Text = ds.Tables[0].Rows[i][22].ToString().Trim().ToUpper(); //구매오더단위	22

                        if (ds.Tables[0].Rows[i][23].ToString().Trim() == "")
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T")].Value = 0;  //구매L/T	23
                        else
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T")].Value = ds.Tables[0].Rows[i][23]; //구매L/T	23

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직")].Text = ds.Tables[0].Rows[i][24].ToString().Trim(); //구매조직	24

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직명")].Text = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", ds.Tables[0].Rows[i][24].ToString().Trim(), " AND MAJOR_CD = 'M001'  AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'"); //구매조직명
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직명")].Text.Trim() == "") fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직")].Text = "";

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "최종검사")].Text = ds.Tables[0].Rows[i][25].ToString().Trim().ToUpper(); //최종검사	25

                        if (ds.Tables[0].Rows[i][26].ToString().Trim().ToUpper() == "Y")
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정검사")].Value = 1; //공정검사	26

                        if (ds.Tables[0].Rows[i][27].ToString().Trim().ToUpper() == "Y")
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수입검사")].Value = 1; //수입검사	27

                        if (ds.Tables[0].Rows[i][28].ToString().Trim().ToUpper() == "Y")
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출하검사")].Value = 1; //출하검사	28 

                        //2013-03-13 원가정보 추가
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "군급분류")].Text = ds.Tables[0].Rows[i][29].ToString().Trim();					//군급분류					29
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "국가재고번호")].Text = ds.Tables[0].Rows[i][30].ToString().Trim();				//국가재고번호				30
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부품관리번호")].Text = ds.Tables[0].Rows[i][31].ToString().Trim();				//부품관리번호				31
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양산구분")].Text = ds.Tables[0].Rows[i][32].ToString().Trim();					//양산구분					32
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통합원가부품구분(계정)")].Value = ds.Tables[0].Rows[i][33].ToString().Trim();	//통합원가부품구분(계정)	33
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "방산물자지정여부")].Text = ds.Tables[0].Rows[i][34].ToString();			        //방산물자지정여부			34
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "방산물자지정근거")].Text = ds.Tables[0].Rows[i][35].ToString();			        //방산물자지정근거			35
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매구분")].Text = ds.Tables[0].Rows[i][36].ToString().Trim();					//구매구분					36
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시효구분")].Text = ds.Tables[0].Rows[i][37].ToString().Trim();					//시효구분					37
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "ESD구분")].Text = ds.Tables[0].Rows[i][38].ToString().Trim();					//ESD구분					38
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MSL구분")].Text = ds.Tables[0].Rows[i][39].ToString().Trim();					//MSL구분					39
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품명부호")].Text = ds.Tables[0].Rows[i][40].ToString().Trim();					//품명부호					40
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목식별부호")].Text = ds.Tables[0].Rows[i][41].ToString().Trim();				//품목식별부호				41
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격화구분")].Value = ds.Tables[0].Rows[i][42].ToString().Trim();				//규격화구분				42
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "국방도면종류")].Value = ds.Tables[0].Rows[i][43].ToString().Trim();				//국방도면종류				43
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "국방도면부품번호")].Text = ds.Tables[0].Rows[i][44].ToString().Trim();			//국방도면부품번호			44
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호")].Text = ds.Tables[0].Rows[i][45].ToString().Trim().ToUpper();		//도면번호					45
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호REV")].Text = ds.Tables[0].Rows[i][46].ToString().Trim();				//도면번호REV				46
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호REV DATE")].Text = ds.Tables[0].Rows[i][47].ToString();					//도면번호REV DATE			47
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "형상식별담당자")].Text = ds.Tables[0].Rows[i][48].ToString();			        //형상식별담당자			48
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격번호")].Text = ds.Tables[0].Rows[i][49].ToString().Trim();					//규격번호					49
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격부품번호")].Text = ds.Tables[0].Rows[i][50].ToString().Trim();				//규격부품번호				50
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산자부품번호")].Text = ds.Tables[0].Rows[i][51].ToString().Trim();			//원생산자부품번호			51
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산자품명")].Text = ds.Tables[0].Rows[i][52].ToString().Trim();				//원생산자품명				52
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산자사업자등록번호")].Text = ds.Tables[0].Rows[i][53].ToString().Trim();	//원생산자사업자등록번호	53
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산자사업자명")].Text = ds.Tables[0].Rows[i][54].ToString().Trim();			//원생산자사업자명			54
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원자재재질기호")].Text = ds.Tables[0].Rows[i][55].ToString().Trim();			//원자재재질기호			55
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원자재규격")].Text = ds.Tables[0].Rows[i][56].ToString().Trim();				//원자재규격				56
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "주장비명")].Text = ds.Tables[0].Rows[i][57].ToString().Trim();					//주장비명					57
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조립체명")].Text = ds.Tables[0].Rows[i][58].ToString().Trim();					//조립체명					58
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "중량")].Value = ds.Tables[0].Rows[i][59];										//중량						59
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "중량단위")].Text = ds.Tables[0].Rows[i][60].ToString().Trim();					//중량단위					60
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부피")].Value = ds.Tables[0].Rows[i][61];										//부피						62
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부피단위")].Text = ds.Tables[0].Rows[i][62].ToString().Trim();					//부피단위					62
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사양")].Text = ds.Tables[0].Rows[i][63].ToString().Trim();						//사양						63
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재질구분")].Value = ds.Tables[0].Rows[i][64].ToString().Trim();					//재질구분					64

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.Length >= 2)
                        {
                            // 2020.07.21. hma 수정(Start): 재질구분 항목이 필수로 관리될 필요가 없어서 주석 처리함.
                            //if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.Substring(0, 2) == "PA" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.Substring(0, 2) == "VA")
                            //{
                            //    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재질구분")].BackColor = SystemBase.Validation.Kind_LightCyan;
                            //}
                            //else
                            //{
                            // 2020.07.21. hma 수정(End)
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재질구분")].BackColor = SystemBase.Validation.Kind_White;
                            //} // 2020.07.21. hma 수정: 주석 처리
                        }
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "임시품목코드")].Value = ds.Tables[0].Rows[i][65].ToString().Trim();		// 2017.12.19. hma 추가: 임시품목코드   // 2017.12.21. hma 수정: 70=>65
                    }
                }
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDownload_Click(object sender, System.EventArgs e)
        {
            string updndl = "";

            if (SystemBase.Base.gstrUserID == "ADMIN") updndl = "Y#Y#Y";
            else updndl = "N#Y#N";

            UIForm.FileUpDown form1 = new UIForm.FileUpDown(this.Name, updndl);
            form1.ShowDialog();
        }

        #endregion
    }
}
