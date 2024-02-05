using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.Text.RegularExpressions;

namespace PA.PBA133
{
    public partial class PBA133 : UIForm.FPCOMM3
    {
        #region 변수선언
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        #endregion

        public PBA133()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void PBA133_Load(object sender, EventArgs e)
        {
            // 콤보 처리
            SystemBase.ComboMake.C1Combo(cboCustGubun, "usp_PBA133 @pType='S5'", 0); // 거래처

            // 그리드 콤보
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            G3Etc[SystemBase.Base.GridHeadIndex(GHIdx3, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G3Etc[SystemBase.Base.GridHeadIndex(GHIdx3, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread3, null, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, 0, 0, false);

            dtpVALID_DT.Text = SystemBase.Base.ServerTime("YYMMDD");
            // 초기화
            GridCommPanel1.Visible = true;
            GridCommPanel1.Dock = DockStyle.Fill;

            GridCommPanel2.Visible = false;
            GridCommPanel2.Dock = DockStyle.None;
            GridCommPanel3.Visible = false;
            GridCommPanel3.Dock = DockStyle.None;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread3, null, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, 0, 0, false);

            dtpVALID_DT.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strGbn = "";

                    if (cboCustGubun.SelectedValue.ToString() == "1")      // 삼성
                        strGbn = "S2";
                    else if (cboCustGubun.SelectedValue.ToString() == "2") // LIG 넥스원
                        strGbn = "S3";
                    else
                        strGbn = "S4";

                    // 활성화된 그리드
                    string strQuery = " usp_PBA133 '" + strGbn + "' ";

                    strQuery += ", @pSRCH_TYPE='" + (rdoWForm.Checked ? 1 : 2) + "' ";
                    strQuery += ", @pPROJECT_NO='" + txtPROJ_NO.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ='" + txtMAKE_NO.Text + "' ";
                    strQuery += ", @pPLANT_CD='" + SystemBase.Base.gstrPLANT_CD + "' ";
                    strQuery += ", @pITEM_CD ='" + txtITEM_CD.Text + "' ";
                    strQuery += ", @pMAKEORDER_NO ='" + txtMakeOrderNo.Text + "' ";
                    strQuery += ", @pVALID_DT ='" + dtpVALID_DT.Text + "' ";
                    strQuery += ", @pPRNT_BOM_NO = '1' ";
                    strQuery += ", @pITEM_QTY ='" + cnmITEM_QTY.Value + "' ";
                    strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    if (cboCustGubun.SelectedValue.ToString() == "1")      // 삼성
                        UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt,  false, true, 0, 0, true);
                    else if (cboCustGubun.SelectedValue.ToString() == "2") // LIG 넥스원
                        UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                    else
                        UIForm.FPMake.grdCommSheet(fpSpread3, strQuery, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, true, 0, 0, true);

                    strQuery = " usp_PBA133  'S6'";
                    strQuery += ", @pPROJECT_NO='" + txtPROJ_NO.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ='" + txtMAKE_NO.Text + "' ";
                    strQuery += ", @pITEM_CD ='" + txtITEM_CD.Text + "' ";
                    strQuery += ", @pMAKEORDER_NO ='" + txtMakeOrderNo.Text + "' ";
                    strQuery += ", @pPLANT_CD='" + SystemBase.Base.gstrPLANT_CD + "' ";
                    strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows[0][1].ToString() != "0")
                    {
                        if (dt.Rows[0][0].ToString() != "0")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, 100, 3);
                            UIForm.FPMake.grdReMake(fpSpread2, 100, 3);
                            UIForm.FPMake.grdReMake(fpSpread3, 120, 3);
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region btnITEM_Click
        private void btnITEM_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtITEM_CD.Text, txtITEM_NM.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, new int[] { 1, 2 }, "품목코드 조회", true);
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtITEM_CD.Text = Msgs[1].ToString();
                    txtITEM_NM.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btn_PROJ_Click
        private void btn_PROJ_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P023' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtPROJ_NO.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00099", strQuery, strWhere, strSearch, "프로젝트 조회", new int[] { 0, 2 }, false);
                pu.Width = 870;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    txtPROJ_NO.Text = pu.ReturnValue[0].ToString();
                    txtPROJ_NM.Value = pu.ReturnValue[1].ToString();
                    txtMAKE_NO.Text = pu.ReturnValue[2].ToString();
                    txtITEM_CD.Text = pu.ReturnValue[3].ToString();
                    txtITEM_NM.Value = pu.ReturnValue[4].ToString();
                    cnmITEM_QTY.ReadOnly = false;
                    cnmITEM_QTY.Value = pu.ReturnValue[5].ToString();
                    cnmITEM_QTY.ReadOnly = true;
                    txtMakeOrderNo.Text = pu.ReturnValue[6].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 현재활성화 되어 있는 그리드 조회
        private FarPoint.Win.Spread.FpSpread getActiveGrid()
        {
            if (cboCustGubun.SelectedValue.ToString() == "1")
                return fpSpread1;
            else if (cboCustGubun.SelectedValue.ToString() == "2")
                return fpSpread2;
            else
                return fpSpread3;
        }
        #endregion

        #region 현재활성화 되어 있는 그리드 인덱스 조회
        private string[,] getActiveGridIdx()
        {
            if (cboCustGubun.SelectedValue.ToString() == "1")
                return GHIdx1;
            else if (cboCustGubun.SelectedValue.ToString() == "2")
                return GHIdx2;
            else
                return GHIdx3;
        }
        #endregion

        #region 회사별 검사 결과 처리
        private void saveExamRst(int row, string prntPlantCd, string prntItemCd, string prntBomNo, string childItemSeq,
                                  string childPlantCd, string childBomNo, SqlConnection dbConn, SqlCommand cmd, SqlTransaction Trans)
        {

            int stCol = 0; // 컬럼의 시작위치

            if (cboCustGubun.SelectedValue.ToString() == "1")
                return;
            else if (cboCustGubun.SelectedValue.ToString() == "2")
                stCol = 18;
            else
                stCol = 18;

            string[,] GHIdx = getActiveGridIdx();

            // 활성화된 그리드
            FarPoint.Win.Spread.FpSpread fpSpread = getActiveGrid();

            string strQuery = " usp_PBA133 'I3' ";

            strQuery += ", @pPROJECT_NO = '" + txtPROJ_NO.Text + "'";
            strQuery += ", @pPROJECT_SEQ = '" + txtMAKE_NO.Text + "'";
            strQuery += ", @pGROUP_CD = '" + txtITEM_CD.Text + "'";
            strQuery += ", @pMAKEORDER_NO = '" + txtMakeOrderNo.Text + "'";
            strQuery += ", @pPRNT_PLANT_CD = '" + prntPlantCd + "'";
            strQuery += ", @pPRNT_ITEM_CD = '" + prntItemCd + "'";
            strQuery += ", @pPRNT_BOM_NO = '" + prntBomNo + "'";
            strQuery += ", @pCHILD_ITEM_SEQ = '" + childItemSeq + "'";
            strQuery += ", @pCHILD_PLANT_CD = '" + childPlantCd + "'";
            strQuery += ", @pCHILD_BOM_NO = '" + childBomNo + "'";
            strQuery += ", @pFIG_NO = '" + fpSpread.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx, "FIG NO")].Value + "'";

            strQuery += ", @pRST_QTY_001='" + fpSpread.Sheets[0].Cells[row, stCol + 1].Value + "'";
            strQuery += ", @pRST_QTY_002='" + fpSpread.Sheets[0].Cells[row, stCol + 2].Value + "'";
            strQuery += ", @pRST_QTY_003='" + fpSpread.Sheets[0].Cells[row, stCol + 3].Value + "'";
            strQuery += ", @pRST_QTY_004='" + fpSpread.Sheets[0].Cells[row, stCol + 4].Value + "'";
            strQuery += ", @pRST_QTY_005='" + fpSpread.Sheets[0].Cells[row, stCol + 5].Value + "'";
            strQuery += ", @pRST_QTY_006='" + fpSpread.Sheets[0].Cells[row, stCol + 6].Value + "'";
            strQuery += ", @pRST_QTY_007='" + fpSpread.Sheets[0].Cells[row, stCol + 7].Value + "'";
            strQuery += ", @pRST_QTY_008='" + fpSpread.Sheets[0].Cells[row, stCol + 8].Value + "'";
            strQuery += ", @pRST_QTY_009='" + fpSpread.Sheets[0].Cells[row, stCol + 9].Value + "'";
            strQuery += ", @pRST_QTY_010='" + fpSpread.Sheets[0].Cells[row, stCol + 10].Value + "'";
            strQuery += ", @pRST_QTY_011='" + fpSpread.Sheets[0].Cells[row, stCol + 11].Value + "'";
            strQuery += ", @pRST_QTY_012='" + fpSpread.Sheets[0].Cells[row, stCol + 12].Value + "'";
            strQuery += ", @pRST_QTY_013='" + fpSpread.Sheets[0].Cells[row, stCol + 13].Value + "'";

            if (cboCustGubun.SelectedValue.ToString() == "3")
            {

                strQuery += ", @pRST_QTY_014='" + fpSpread.Sheets[0].Cells[row, stCol + 14].Value + "'";
                strQuery += ", @pRST_QTY_015='" + fpSpread.Sheets[0].Cells[row, stCol + 15].Value + "'";
                strQuery += ", @pRST_QTY_016='" + fpSpread.Sheets[0].Cells[row, stCol + 16].Value + "'";
                strQuery += ", @pRST_QTY_017='" + fpSpread.Sheets[0].Cells[row, stCol + 17].Value + "'";
                strQuery += ", @pRST_QTY_018='" + fpSpread.Sheets[0].Cells[row, stCol + 18].Value + "'";
                strQuery += ", @pRST_QTY_019='" + fpSpread.Sheets[0].Cells[row, stCol + 19].Value + "'";
                strQuery += ", @pRST_QTY_020='" + fpSpread.Sheets[0].Cells[row, stCol + 20].Value + "'";
                strQuery += ", @pRST_QTY_021='" + fpSpread.Sheets[0].Cells[row, stCol + 21].Value + "'";
                strQuery += ", @pRST_QTY_022='" + fpSpread.Sheets[0].Cells[row, stCol + 22].Value + "'";
                strQuery += ", @pRST_QTY_023='" + fpSpread.Sheets[0].Cells[row, stCol + 23].Value + "'";
                strQuery += ", @pRST_QTY_024='" + fpSpread.Sheets[0].Cells[row, stCol + 24].Value + "'";
                strQuery += ", @pRST_QTY_025='" + fpSpread.Sheets[0].Cells[row, stCol + 25].Value + "'";
                strQuery += ", @pRST_QTY_026='" + fpSpread.Sheets[0].Cells[row, stCol + 26].Value + "'";
                strQuery += ", @pRST_QTY_027='" + fpSpread.Sheets[0].Cells[row, stCol + 27].Value + "'";
                strQuery += ", @pRST_QTY_028='" + fpSpread.Sheets[0].Cells[row, stCol + 28].Value + "'";
                strQuery += ", @pRST_QTY_029='" + fpSpread.Sheets[0].Cells[row, stCol + 29].Value + "'";
                strQuery += ", @pRST_QTY_030='" + fpSpread.Sheets[0].Cells[row, stCol + 30].Value + "'";
                strQuery += ", @pRST_QTY_031='" + fpSpread.Sheets[0].Cells[row, stCol + 31].Value + "'";
                strQuery += ", @pRST_QTY_032='" + fpSpread.Sheets[0].Cells[row, stCol + 32].Value + "'";
                strQuery += ", @pRST_QTY_033='" + fpSpread.Sheets[0].Cells[row, stCol + 33].Value + "'";
                strQuery += ", @pRST_QTY_034='" + fpSpread.Sheets[0].Cells[row, stCol + 34].Value + "'";
                strQuery += ", @pRST_QTY_035='" + fpSpread.Sheets[0].Cells[row, stCol + 35].Value + "'";
                strQuery += ", @pRST_QTY_036='" + fpSpread.Sheets[0].Cells[row, stCol + 36].Value + "'";
                strQuery += ", @pRST_QTY_037='" + fpSpread.Sheets[0].Cells[row, stCol + 37].Value + "'";
                strQuery += ", @pRST_QTY_038='" + fpSpread.Sheets[0].Cells[row, stCol + 38].Value + "'";
                strQuery += ", @pRST_QTY_039='" + fpSpread.Sheets[0].Cells[row, stCol + 39].Value + "'";
                strQuery += ", @pRST_QTY_040='" + fpSpread.Sheets[0].Cells[row, stCol + 40].Value + "'";
                strQuery += ", @pRST_QTY_041='" + fpSpread.Sheets[0].Cells[row, stCol + 41].Value + "'";
                strQuery += ", @pRST_QTY_042='" + fpSpread.Sheets[0].Cells[row, stCol + 42].Value + "'";
                strQuery += ", @pRST_QTY_043='" + fpSpread.Sheets[0].Cells[row, stCol + 43].Value + "'";
                strQuery += ", @pRST_QTY_044='" + fpSpread.Sheets[0].Cells[row, stCol + 44].Value + "'";
                strQuery += ", @pRST_QTY_045='" + fpSpread.Sheets[0].Cells[row, stCol + 45].Value + "'";
                strQuery += ", @pRST_QTY_046='" + fpSpread.Sheets[0].Cells[row, stCol + 46].Value + "'";
                strQuery += ", @pRST_QTY_047='" + fpSpread.Sheets[0].Cells[row, stCol + 47].Value + "'";
                strQuery += ", @pRST_QTY_048='" + fpSpread.Sheets[0].Cells[row, stCol + 48].Value + "'";
                strQuery += ", @pRST_QTY_049='" + fpSpread.Sheets[0].Cells[row, stCol + 49].Value + "'";
                strQuery += ", @pRST_QTY_050='" + fpSpread.Sheets[0].Cells[row, stCol + 50].Value + "'";
                strQuery += ", @pRST_QTY_051='" + fpSpread.Sheets[0].Cells[row, stCol + 51].Value + "'";
                strQuery += ", @pRST_QTY_052='" + fpSpread.Sheets[0].Cells[row, stCol + 52].Value + "'";
                strQuery += ", @pRST_QTY_053='" + fpSpread.Sheets[0].Cells[row, stCol + 53].Value + "'";
                strQuery += ", @pRST_QTY_054='" + fpSpread.Sheets[0].Cells[row, stCol + 54].Value + "'";
                strQuery += ", @pRST_QTY_055='" + fpSpread.Sheets[0].Cells[row, stCol + 55].Value + "'";
                strQuery += ", @pRST_QTY_056='" + fpSpread.Sheets[0].Cells[row, stCol + 56].Value + "'";
                strQuery += ", @pRST_QTY_057='" + fpSpread.Sheets[0].Cells[row, stCol + 57].Value + "'";
                strQuery += ", @pRST_QTY_058='" + fpSpread.Sheets[0].Cells[row, stCol + 58].Value + "'";
                strQuery += ", @pRST_QTY_059='" + fpSpread.Sheets[0].Cells[row, stCol + 59].Value + "'";
                strQuery += ", @pRST_QTY_060='" + fpSpread.Sheets[0].Cells[row, stCol + 60].Value + "'";
                strQuery += ", @pRST_QTY_061='" + fpSpread.Sheets[0].Cells[row, stCol + 61].Value + "'";
                strQuery += ", @pRST_QTY_062='" + fpSpread.Sheets[0].Cells[row, stCol + 62].Value + "'";
                strQuery += ", @pRST_QTY_063='" + fpSpread.Sheets[0].Cells[row, stCol + 63].Value + "'";
                strQuery += ", @pRST_QTY_064='" + fpSpread.Sheets[0].Cells[row, stCol + 64].Value + "'";
                strQuery += ", @pRST_QTY_065='" + fpSpread.Sheets[0].Cells[row, stCol + 65].Value + "'";
                strQuery += ", @pRST_QTY_066='" + fpSpread.Sheets[0].Cells[row, stCol + 66].Value + "'";
                strQuery += ", @pRST_QTY_067='" + fpSpread.Sheets[0].Cells[row, stCol + 67].Value + "'";
                strQuery += ", @pRST_QTY_068='" + fpSpread.Sheets[0].Cells[row, stCol + 68].Value + "'";
                strQuery += ", @pRST_QTY_069='" + fpSpread.Sheets[0].Cells[row, stCol + 69].Value + "'";
                strQuery += ", @pRST_QTY_070='" + fpSpread.Sheets[0].Cells[row, stCol + 70].Value + "'";
                strQuery += ", @pRST_QTY_071='" + fpSpread.Sheets[0].Cells[row, stCol + 71].Value + "'";
                strQuery += ", @pRST_QTY_072='" + fpSpread.Sheets[0].Cells[row, stCol + 72].Value + "'";
                strQuery += ", @pRST_QTY_073='" + fpSpread.Sheets[0].Cells[row, stCol + 73].Value + "'";
                strQuery += ", @pRST_QTY_074='" + fpSpread.Sheets[0].Cells[row, stCol + 74].Value + "'";
                strQuery += ", @pRST_QTY_075='" + fpSpread.Sheets[0].Cells[row, stCol + 75].Value + "'";
                strQuery += ", @pRST_QTY_076='" + fpSpread.Sheets[0].Cells[row, stCol + 76].Value + "'";
                strQuery += ", @pRST_QTY_077='" + fpSpread.Sheets[0].Cells[row, stCol + 77].Value + "'";
                strQuery += ", @pRST_QTY_078='" + fpSpread.Sheets[0].Cells[row, stCol + 78].Value + "'";
                strQuery += ", @pRST_QTY_079='" + fpSpread.Sheets[0].Cells[row, stCol + 79].Value + "'";
                strQuery += ", @pRST_QTY_080='" + fpSpread.Sheets[0].Cells[row, stCol + 80].Value + "'";
                strQuery += ", @pRST_QTY_081='" + fpSpread.Sheets[0].Cells[row, stCol + 81].Value + "'";
                strQuery += ", @pRST_QTY_082='" + fpSpread.Sheets[0].Cells[row, stCol + 82].Value + "'";
                strQuery += ", @pRST_QTY_083='" + fpSpread.Sheets[0].Cells[row, stCol + 83].Value + "'";
                strQuery += ", @pRST_QTY_084='" + fpSpread.Sheets[0].Cells[row, stCol + 84].Value + "'";
                strQuery += ", @pRST_QTY_085='" + fpSpread.Sheets[0].Cells[row, stCol + 85].Value + "'";
                strQuery += ", @pRST_QTY_086='" + fpSpread.Sheets[0].Cells[row, stCol + 86].Value + "'";
                strQuery += ", @pRST_QTY_087='" + fpSpread.Sheets[0].Cells[row, stCol + 87].Value + "'";
                strQuery += ", @pRST_QTY_088='" + fpSpread.Sheets[0].Cells[row, stCol + 88].Value + "'";
                strQuery += ", @pRST_QTY_089='" + fpSpread.Sheets[0].Cells[row, stCol + 89].Value + "'";
                strQuery += ", @pRST_QTY_090='" + fpSpread.Sheets[0].Cells[row, stCol + 90].Value + "'";
                strQuery += ", @pRST_QTY_091='" + fpSpread.Sheets[0].Cells[row, stCol + 91].Value + "'";
                strQuery += ", @pRST_QTY_092='" + fpSpread.Sheets[0].Cells[row, stCol + 92].Value + "'";
                strQuery += ", @pRST_QTY_093='" + fpSpread.Sheets[0].Cells[row, stCol + 93].Value + "'";
                strQuery += ", @pRST_QTY_094='" + fpSpread.Sheets[0].Cells[row, stCol + 94].Value + "'";
                strQuery += ", @pRST_QTY_095='" + fpSpread.Sheets[0].Cells[row, stCol + 95].Value + "'";
                strQuery += ", @pRST_QTY_096='" + fpSpread.Sheets[0].Cells[row, stCol + 96].Value + "'";
                strQuery += ", @pRST_QTY_097='" + fpSpread.Sheets[0].Cells[row, stCol + 97].Value + "'";
                strQuery += ", @pRST_QTY_098='" + fpSpread.Sheets[0].Cells[row, stCol + 98].Value + "'";
                strQuery += ", @pRST_QTY_099='" + fpSpread.Sheets[0].Cells[row, stCol + 99].Value + "'";
                strQuery += ", @pRST_QTY_100='" + fpSpread.Sheets[0].Cells[row, stCol + 100].Value + "'";

            }
            strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
            string ERRCode = ds.Tables[0].Rows[0][0].ToString();
            string MSGCode = ds.Tables[0].Rows[0][1].ToString();

            if (ERRCode == "ER")
                throw new Exception(MSGCode);
        }
        #endregion

        #region 거래처 변경
        private void cboCustGubun_RowChange(object sender, EventArgs e)
        {       
            if (cboCustGubun.SelectedValue.ToString() == "1")      // 삼성
            {
                GridCommPanel1.Visible = true;
                GridCommPanel1.Dock = DockStyle.Fill;

                GridCommPanel2.Visible = false;
                GridCommPanel2.Dock = DockStyle.None;
                GridCommPanel3.Visible = false;
                GridCommPanel3.Dock = DockStyle.None;
            }
            else if (cboCustGubun.SelectedValue.ToString() == "2") // LIG
            {
                GridCommPanel2.Visible = true;
                GridCommPanel2.Dock = DockStyle.Fill;

                GridCommPanel1.Visible = false;
                GridCommPanel1.Dock = DockStyle.None;
                GridCommPanel3.Visible = false;
                GridCommPanel3.Dock = DockStyle.None;
            }
            else // 조달
            {
                GridCommPanel3.Visible = true;
                GridCommPanel3.Dock = DockStyle.Fill;

                GridCommPanel1.Visible = false;
                GridCommPanel1.Dock = DockStyle.None;
                GridCommPanel2.Visible = false;
                GridCommPanel2.Dock = DockStyle.None;
            }
        }
        #endregion

        #region Excel 생성 버튼
        private void btnExcelSub_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            // 활성화된 그리드
            FarPoint.Win.Spread.FpSpread fpSpread = getActiveGrid();

            if (fpSpread.Sheets[0].Rows.Count <= 0)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0053"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Excel 다운로드 위치 지정";
            dlg.InitialDirectory = dlg.FileName;
            dlg.Filter = "전체(*.*)|*.*|Excel Files(*.xls)|*.xls";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;
            dlg.FileName = this.Text.ToString().Replace(@"/", "_") + ".xls";
            dlg.OverwritePrompt = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {

                th = new Thread(new ThreadStart(Show_Waiting));
                th.Start();

                Excel.Application oAppln;
                Excel.Workbook oWorkBook;
                Excel.Worksheet oWorkSheet;
                Excel.Range oRange;

                try
                {

                    oAppln = new Excel.Application();
                    oWorkBook = (Excel.Workbook)(oAppln.Workbooks.Add(true));
                    oWorkSheet = (Excel.Worksheet)oWorkBook.ActiveSheet;

                    if (fpSpread.Sheets[0].Rows.Count > 0)
                    {

                        Waiting_Form.Activate();
                        Waiting_Form.label_temp.Text = "엑셀 HEAD를 생성중입니다.";

                        // header 저장
                        int headRow = 0;
                        int shtTitleSpanTmp = 1, shtTitleSpanTmp2 = 1, shtTitleSpanTmp3 = 1;

                        for (int HeadColCnt = 1, excelColNo = 1; HeadColCnt < fpSpread.Sheets[0].Columns.Count; HeadColCnt++)
                        {
                            headRow = 1;

                            // HIDDEN일 경우 처리
                            if (!fpSpread.Sheets[0].GetColumnVisible(HeadColCnt))
                                continue;

                            for (int HeadRowCnt = 0; HeadRowCnt < fpSpread.Sheets[0].ColumnHeaderRowCount; HeadRowCnt++)
                            {
                                oWorkSheet.Cells[headRow, excelColNo] = fpSpread.Sheets[0].ColumnHeader.Cells[HeadRowCnt, HeadColCnt].Text;
                                headRow++;
                            }

                            //ColHead 합치기
                            if (fpSpread.Sheets[0].ColumnHeaderRowCount > 3)
                            {
                                if (HeadColCnt > 1 && fpSpread.Sheets[0].ColumnHeader.Cells[2, HeadColCnt].Text == fpSpread.Sheets[0].ColumnHeader.Cells[2, HeadColCnt - 1].Text)
                                {
                                    shtTitleSpanTmp3++;
                                }
                                else
                                {
                                    if (HeadColCnt > 1)
                                    {
                                        oWorkSheet.Application.DisplayAlerts = false;

                                        Excel.Range eRange = oWorkSheet.get_Range(oWorkSheet.Cells[3, excelColNo - shtTitleSpanTmp3], oWorkSheet.Cells[3, excelColNo - 1]);
                                        eRange.Merge(Type.Missing);
                                        eRange.HorizontalAlignment = HorizontalAlignment.Center;

                                        oWorkSheet.Application.DisplayAlerts = true;
                                    }

                                    shtTitleSpanTmp3 = 1;
                                }

                                //RowHead 합치기
                                if (fpSpread.Sheets[0].ColumnHeader.Cells[0, HeadColCnt].Text == fpSpread.Sheets[0].ColumnHeader.Cells[1, HeadColCnt].Text
                                    && fpSpread.Sheets[0].ColumnHeader.Cells[1, HeadColCnt].Text == fpSpread.Sheets[0].ColumnHeader.Cells[2, HeadColCnt].Text)
                                {
                                    oWorkSheet.Application.DisplayAlerts = false;

                                    Excel.Range eRange = oWorkSheet.get_Range(oWorkSheet.Cells[1, excelColNo], oWorkSheet.Cells[3, excelColNo]);
                                    eRange.Merge(Type.Missing);
                                    eRange.HorizontalAlignment = HorizontalAlignment.Center;

                                    oWorkSheet.Application.DisplayAlerts = true;
                                }
                            }

                            if (fpSpread.Sheets[0].ColumnHeaderRowCount > 2)
                            {
                                if (HeadColCnt > 1 && fpSpread.Sheets[0].ColumnHeader.Cells[1, HeadColCnt].Text == fpSpread.Sheets[0].ColumnHeader.Cells[1, HeadColCnt - 1].Text)
                                {
                                    shtTitleSpanTmp2++;
                                }
                                else
                                {
                                    if (HeadColCnt > 1)
                                    {
                                        oWorkSheet.Application.DisplayAlerts = false;

                                        Excel.Range eRange = oWorkSheet.get_Range(oWorkSheet.Cells[2, excelColNo - shtTitleSpanTmp2], oWorkSheet.Cells[2, excelColNo - 1]);
                                        eRange.Merge(Type.Missing);
                                        eRange.HorizontalAlignment = HorizontalAlignment.Center;

                                        oWorkSheet.Application.DisplayAlerts = true;
                                    }

                                    shtTitleSpanTmp2 = 1;
                                }

                                //RowHead 합치기
                                if (fpSpread.Sheets[0].ColumnHeader.Cells[1, HeadColCnt].Text == fpSpread.Sheets[0].ColumnHeader.Cells[2, HeadColCnt].Text)
                                {
                                    oWorkSheet.Application.DisplayAlerts = false;

                                    Excel.Range eRange = oWorkSheet.get_Range(oWorkSheet.Cells[2, excelColNo], oWorkSheet.Cells[3, excelColNo]);
                                    eRange.Merge(Type.Missing);
                                    eRange.HorizontalAlignment = HorizontalAlignment.Center;

                                    oWorkSheet.Application.DisplayAlerts = true;
                                }
                            }

                            if (fpSpread.Sheets[0].ColumnHeaderRowCount > 1)
                            {
                                if (excelColNo > 1 && fpSpread.Sheets[0].ColumnHeader.Cells[0, HeadColCnt].Text == fpSpread.Sheets[0].ColumnHeader.Cells[0, HeadColCnt - 1].Text)
                                {
                                    shtTitleSpanTmp++;
                                }
                                else
                                {
                                    if (excelColNo > 1)
                                    {
                                        oWorkSheet.Application.DisplayAlerts = false;

                                        Excel.Range eRange = oWorkSheet.get_Range(oWorkSheet.Cells[1, excelColNo - shtTitleSpanTmp], oWorkSheet.Cells[1, excelColNo - 1]);
                                        eRange.Merge(Type.Missing);
                                        eRange.HorizontalAlignment = HorizontalAlignment.Center;

                                        oWorkSheet.Application.DisplayAlerts = true;
                                    }

                                    shtTitleSpanTmp = 1;
                                }

                                //RowHead 합치기
                                if (fpSpread.Sheets[0].ColumnHeader.Cells[0, HeadColCnt].Text == fpSpread.Sheets[0].ColumnHeader.Cells[0 + 1, HeadColCnt].Text)
                                {
                                    oWorkSheet.Application.DisplayAlerts = false;

                                    Excel.Range eRange = oWorkSheet.get_Range(oWorkSheet.Cells[1, excelColNo], oWorkSheet.Cells[2, excelColNo]);
                                    eRange.Merge(Type.Missing);
                                    eRange.HorizontalAlignment = HorizontalAlignment.Center;

                                    oWorkSheet.Application.DisplayAlerts = true;
                                }
                            }
                            // EXCEL COLUMN 증가
                            excelColNo++;
                        }
                        int iRow = headRow;

                        // 프로그래스 BAR 설정
                        Waiting_Form.progressBar_temp.Maximum = fpSpread.Sheets[0].Rows.Count;

                        //내용 저장
                        for (int rowNo = 0; rowNo < fpSpread.Sheets[0].Rows.Count; rowNo++)
                        {
                            // HIDDEN이 아닌 요소만 출력
                            for (int colNo = 1, excelColNo = 1; colNo < fpSpread.Sheets[0].Columns.Count; colNo++)
                            {
                                if (fpSpread.Sheets[0].GetColumnVisible(colNo))
                                {
                                    oWorkSheet.Cells[iRow, excelColNo] = fpSpread.Sheets[0].Cells[rowNo, colNo].Text;
                                    excelColNo++;
                                }
                            }
                            iRow++;
                            Waiting_Form.progressBar_temp.Value = rowNo + 1;
                            Waiting_Form.label_temp.Text = "총" + fpSpread.Sheets[0].Rows.Count.ToString() + " Row 중 " + iRow.ToString() + " Row를 저장하였습니다.";
                        }
                    }
                    Waiting_Form.label_temp.Text = "엑셀 Sheet를 열고 있습니다.";
                    //range of the excel sheet
                    oRange = oWorkSheet.get_Range("A1", "IV1");
                    oRange.EntireColumn.AutoFit();
                    oAppln.UserControl = false;

                    oAppln.Visible = true;	// 저장후 저장된 내용 실행여부

                    // 엑셀 파일로 저장
                    oWorkBook.SaveAs(dlg.FileName, Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Excel.XlSaveAsAccessMode.xlNoChange, false, false, null, null, null);
                    Waiting_Form.label_temp.Text = "완료되었습니다.";

                    this.Cursor = Cursors.Default;

                }
                catch (Exception f)
                {
                    // EXCEL생성중 오류 발생
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0009"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Waiting_Form.Close();
                    if (th != null && !th.Join(1000))  // 1초 내로 종료 되지 않으면
                    {
                        // 강제 종료
                        th.Abort();
                    }

                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void Show_Waiting()
        {
            Waiting_Form = new UIForm.ExcelWaiting();
            Waiting_Form.ShowDialog();
        }
        #endregion

        #region 제출용에서는 붙여 넣기를 못하게 한다.
        private void fpSpread1_ClipboardPasting(object sender, FarPoint.Win.Spread.ClipboardPastingEventArgs e)
        {
            // 제출용에서는 붙여 넣기를 못하게 한다.
            if (rdoSForm.Checked)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("P0034"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Handled = true;
                return;
            }
        }

        private void fpSpread2_ClipboardPasting(object sender, FarPoint.Win.Spread.ClipboardPastingEventArgs e)
        {
            // 제출용에서는 붙여 넣기를 못하게 한다.
            if (rdoSForm.Checked)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("P0034"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Handled = true;
                return;
            }
        }

        private void fpSpread3_ClipboardPasting(object sender, FarPoint.Win.Spread.ClipboardPastingEventArgs e)
        {
            // 제출용에서는 붙여 넣기를 못하게 한다.
            if (rdoSForm.Checked)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("P0034"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Handled = true;
                return;
            }
        }
        #endregion

        private void txtPROJ_NO_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtPROJ_NO.Text != "")
                {
                    txtPROJ_NM.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtPROJ_NO.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");                   
                }
                else
                {
                    txtPROJ_NM.Value = "";
                }
            }
            catch { }
        }

        private void txtITEM_CD_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtITEM_CD.Text != "")
                {
                    txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtITEM_NM.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목명 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
