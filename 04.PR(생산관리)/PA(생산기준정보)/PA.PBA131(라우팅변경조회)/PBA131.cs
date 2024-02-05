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

namespace PA.PBA131
{
    public partial class PBA131 : UIForm.FPCOMM1
    {
        #region 생성자
        public PBA131()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void PBA131_Load(object sender, System.EventArgs e)
        {
            // 2022.08.04. hma 추가(Start)
            // 저장 버튼 활성화 처리
            UIForm.Buttons.ReButton("110000011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            // 공정타입 항목 
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공정타입")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P028', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            // 2022.08.04. hma 추가(End)

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P002', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

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
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            
            try
            {
                fpSpread1.Sheets[0].RowCount = 0;
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strMQuery = "";

                    strMQuery = "   usp_PBA131 @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strMQuery += ", @pITEM_CD = '" + txtSItemCd.Text + "' ";
                    if (dtpSRrvDt_Fr.Text != "")
                        strMQuery += ", @pREVISION_DATA_FR = '" + dtpSRrvDt_Fr.Text + "' ";
                    if (dtpSRrvDt_To.Text != "")
                        strMQuery += ", @pREVISION_DATA_TO = '" + dtpSRrvDt_To.Text + "' ";
                    if (txtSRout.Text != "")
                        strMQuery += ", @pROUT_NO = '" + txtSRout.Text + "' ";
                    if (txtSRevNo.Text != "")
                        strMQuery += ", @pREV_NO = '" + txtSRevNo.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    fpSpread1.ActiveSheet.Columns[2].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;        // 2022.08.04. hma 수정: 1=>2로 변경. 맨앞에 출력여부 항목 추가함.
                    fpSpread1.ActiveSheet.Columns[3].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Restricted;    // 2022.08.04. hma 수정: 2=>3로 변경. 맨앞에 출력여부 항목 추가함.
                    fpSpread1.ActiveSheet.Columns[4].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Restricted;    // 2022.08.04. hma 수정: 3=>4로 변경. 맨앞에 출력여부 항목 추가함.
                    fpSpread1.ActiveSheet.Columns[5].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Restricted;    // 2022.08.04. hma 수정: 4=>5로 변경. 맨앞에 출력여부 항목 추가함.
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

        // 2022.08.03. hma 추가(Start): 라우팅변경이력 출력여부 저장되게 함.
        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                //그리드 상단 필수 체크
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
                {
                    this.Cursor = Cursors.WaitCursor;

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
                                }

                                string strPrtYn = "N";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시서출력여부")].Text == "True")
                                    strPrtYn = "Y";

                                string strSql = " usp_PBA131 '" + strGbn + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                strSql += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
                                strSql += ", @pITEM_CD = '" + txtSItemCd.Text + "'";
                                strSql += ", @pROUT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT NO")].Text + "'";
                                strSql += ", @pREV_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "REV NO")].Text + "'";
                                strSql += ", @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정")].Text + "'";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pPRT_YN = '" + strPrtYn + "'";

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
                        MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
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

                    this.Cursor = Cursors.Default;
                }
            }
        }
        #endregion
        // 2022.08.03. hma 추가(End)

        #region 조회조건 팝업
        //품목코드(조회용)팝업
        private void btnSItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtSItemCd.Text, txtSItemNm.Text };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, "품목코드 조회", new int[] { 1, 2 }, true);
                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    txtSItemCd.Value = pu.ReturnValue[1].ToString();
                    txtSItemNm.Value = pu.ReturnValue[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        #region 품목코드 조회 변경시
        private void txtSITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            if (txtSItemCd.Text != "")
            {
                txtSItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtSItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            }
            else
            {
                txtSItemNm.Value = "";
            }
        }
        #endregion



    }
}
