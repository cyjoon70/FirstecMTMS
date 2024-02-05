#region 작성정보
/*********************************************************************/
// 단위업무명 : 정비품라우팅정보 조회
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-15
// 작성내용 : 정비품라우팅정보 조회
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

namespace PA.PBA135
{
    public partial class PBA135 : UIForm.FPCOMM2_2
    {
        #region 변수선언
        int Row = 999;
        string CHILD_ITEM_CD = "";
        string PRNT_PLANT_CD = "";
        string PRNT_ITEM_CD = "";
        string PROC_SEQ = "";

        string PRNT_BOM_NO = "";
        string CHILD_ITEM_SEQ = "";
        string CHILD_PLANT_CD = "";
        string CHILD_BOM_NO = "";
        string PROJECT_NO = "";
        string PROJECT_SEQ = "";
        string GROUP_CD = "";
        string MAKEORDER_NO = "";
        string ITEM_NM = "";

        string NEW_NODE_TAG = "";
        int NewFlg = 0;
        #endregion

        #region 생성자
        public PBA135()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void PBA119_Load(object sender, System.EventArgs e)
        {
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B011', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "유무상구분")] = "F#T|무상#유상";
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "BOM TYPE")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P006', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P001', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P002', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "자원")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P050', @pCOM_CD = '', @pCOM_NM = '', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "기준단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "생산단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")] = "Y#N|사내#외주";

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")] = "Y#N|Y#N";
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")] = "Y#N|Y#N";
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정단계")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P005', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "시간단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z014', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");

            // 외주 정보
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "통화")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z003', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B040', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");

            SystemBase.ComboMake.C1Combo(cboSPLANT_CD, "usp_P_COMMON @pType='P510', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ", 0);	// 공장

            SystemBase.Validation.GroupBox_Setting(groupBox8);	//컨트롤 필수 Setting
            SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);

            //기타세팅
            dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpFROM_VALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpTO_VALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpREV_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region 품목코드 조회
        private void btnSITEM_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtSITEM_CD.Text, txtSITEM_NM.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, "품목코드 조회", new int[] { 1, 2 }, true);
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {

                    txtSITEM_CD.Value = pu.ReturnValue[1].ToString();
                    txtSITEM_NM.Value = pu.ReturnValue[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
                
        #region 트리 클릭 이벤트
        private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            NEW_NODE_TAG = e.Node.Tag.ToString();
            // 라우팅정보 화면 출력
            ShowRoutInfo();
        }
        #endregion

        #region 품목코드 조회 변경시
        private void txtSITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            if (txtSITEM_CD.Text != "")
            {
                txtSITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtSITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            }
            else
            {
                txtSITEM_NM.Value = "";
            }
        }
        #endregion

        #region 라우팅 콤보 조회
        private void txtSITEM_CD_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                txtSITEM_NM.Value = "";
        }
        #endregion

        #region SearchExec() 왼쪽 트리뷰 조회
        protected override void SearchExec()
        {
            // TREE정보 설정
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox8))
                TreeViewSearch();
        }
        private void txtSSCH_CD_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 13)
            {
                TreeViewSearch();
            }
        }

        public void TreeViewSearch()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox8))
            {
                try
                {
                    treeView1.Nodes.Clear();
                    string Query = " exec usp_PBA135 'S1'";
                    Query += ", @pPROJECT_NO = '" + txtPROJECT_NO.Text + "'";
                    Query += ", @pPROJECT_SEQ = '" + txtPROJECT_SEQ.Text + "'";
                    Query += ", @pGROUP_CD = '" + txtSITEM_CD.Text + "'";
                    Query += ", @pMAKEORDER_NO = '" + txtMAKEORDER_NO.Text + "'";
                    Query += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue.ToString() + "'";
                    Query += ", @pITEM_CD = '" + txtSITEM_CD.Text + "'";
                    Query += ", @pVALID_DT = '" + dtpSVALID_DT.Text + "'";
                    Query += ", @pPRNT_BOM_NO = '1'";

                    if (rdoLEVEL1.Checked == true)
                        Query += ", @pLEVEL = '1'";
                    else
                        Query += ", @pLEVEL = '0'";

                    Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(Query);

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataView dvwData = null;
                        UIForm.TreeView.CommonTreeView(ds.Tables[0].Rows[0]["PRNT_ITEM_CD"].ToString()
                            , ds.Tables[0].Rows[0]["FIGNO"].ToString()
                            , (TreeNode)null
                            , treeView1
                            , ds
                            , dvwData
                            , imageList1
                            , 0
                            , true); // 라우팅없을 것에 대한 색깔 처리

                        treeView1.Focus();
                        treeView1.ExpandAll();
                    }
                    else
                    {
                        SystemBase.Validation.GroupBox_Reset(groupBox1);
                        UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                        MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "TreeView 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 자품목투입정보 조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                int intRow = fpSpread2.ActiveSheet.ActiveRowIndex;

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    fpSpread2Search(intRow);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자품목투입정보 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void fpSpread2Search(int R)
        {
            try
            {
                string Query = " usp_PBA135 @pTYPE = 'S4'";
                Query += " ,@pPRNT_PLANT_CD='" + cboSPLANT_CD.SelectedValue + "'";
                Query += " ,@pPRNT_ITEM_CD='" + fpSpread2.Sheets[0].Cells[R, 0].Text + "'";
                Query += " ,@pVALID_DT='" + dtpSVALID_DT.Text + "'";
                Query += " ,@pPROC_SEQ='" + fpSpread2.Sheets[0].Cells[R, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text + "'";
                Query += ", @pROUT_NO = '" + txtROUTING_NO.Text + "'";

                Query += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
                Query += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
                Query += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";
                Query += ", @pMAKEORDER_NO = '" + MAKEORDER_NO.ToString() + "'";
                Query += ", @pBOM_NO = '" + CHILD_BOM_NO.ToString() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

                PRNT_PLANT_CD = cboSPLANT_CD.SelectedValue.ToString();	// 라우팅 콤보
                PROC_SEQ = fpSpread2.Sheets[0].Cells[R, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                SystemBase.MessageBoxComm.Show(f.ToString());
            }
        }
        #endregion

        #region fpSpread2_ButtonClicked 그리드 상단 버튼 클릭
        private void fpSpread2_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 공정조회
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드_2"))
                {
                    string strQuery = " usp_P_COMMON 'P042' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC", "@pLANG_CD" };
                    string[] strSearch = new string[] { "", "", "P001", SystemBase.Base.gstrLangCd };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("WB5101", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                    pu.Width = 500;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드")].Text = Msgs[0].ToString(); //공정코드
                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")].Value = Msgs[1].ToString(); //공정명

                        UIForm.FPMake.fpChange(fpSpread2, e.Row);
                    }
                }
                //부서조회
                else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "자원_2"))
                {
                    string strQuery = " usp_P_COMMON 'P051', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { "", "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05005", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                    pu.Width = 500;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원")].Text = Msgs[0].ToString();	//자원코드
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원명")].Value = Msgs[1].ToString(); //자원명
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Value = Msgs[2].ToString(); //작업장
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value = Msgs[3].ToString();	//공정타입

                        UIForm.FPMake.fpChange(fpSpread2, e.Row);

                        if (Msgs[3].ToString() == "N")  // 자원이 외주일 경우
                        {
                            // 외주란을 활성화 시간다.
                            UIForm.FPMake.grdReMake(fpSpread2, e.Row,
                                SystemBase.Base.GridHeadIndex(GHIdx2, "외주처") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "통화") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형") + "|1"
                                );
                            // 초기값 설정
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Value = "KRW";
                        }
                    }
                }
                else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "외주처_2")) // 외주조회
                {

                    // 공정 타입이 외주일 경우 처리
                    if (fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value == null ||
                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value.ToString() == "Y")
                    {
                        SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn("P0031"));
                        return;
                    }

                    WNDW002 pu = new WNDW002("P");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].Text = Msgs[1].ToString();
                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명")].Text = Msgs[2].ToString();
                    }
                    UIForm.FPMake.fpChange(fpSpread2, e.Row);

                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.Message);
            }
        }
        #endregion

        #region 스프레드 붙여넣기
        private void fpSpread2_ClipboardPasting(object sender, FarPoint.Win.Spread.ClipboardPastingEventArgs e)
        {
            // 자원정보 붙여넣기 일 경우
            if (fpSpread2.Sheets[0].ActiveColumnIndex == SystemBase.Base.GridHeadIndex(GHIdx2, "자원"))
            {
                // PASTE되는 ROW, COL
                int row = fpSpread2.Sheets[0].ActiveRowIndex;
                int col = fpSpread2.Sheets[0].ActiveColumnIndex;

                IDataObject dataObject = Clipboard.GetDataObject();
                string clipVal = dataObject.GetData(DataFormats.StringFormat).ToString().Replace("\r\n", ""); // \r\n 제거

                string strQuery = "";
                strQuery += " usp_P_COMMON 'P051' ";
                strQuery += " , @pCOM_CD='" + clipVal + "'";
                strQuery += ", @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {

                    // 자원정보를 조회한다.
                    fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원")].Text = dt.Rows[0]["RES_CD"].ToString();	//자원코드
                    fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원명")].Value = dt.Rows[0]["RES_DIS"].ToString();	//자원명
                    fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Value = dt.Rows[0]["WORKCENTER_CD"].ToString();	//작업장
                    fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value = dt.Rows[0]["INSIDE_FLG"].ToString();	//공정타입

                    UIForm.FPMake.fpChange(fpSpread2, row);

                    if (dt.Rows[0]["INSIDE_FLG"].ToString() == "N")  // 자원이 외주일 경우
                    {
                        // 외주란을 활성화 시간다.
                        UIForm.FPMake.grdReMake(fpSpread2, row,
                            SystemBase.Base.GridHeadIndex(GHIdx2, "외주처") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "통화") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형") + "|1"
                            );
                        // 초기값 설정
                        fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Value = "KRW";
                    }
                }
            }
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBoxControlsLock(groupBox8, false);
            SystemBase.Validation.GroupBoxControlsLock(groupBox1, false);

            txtROUTING_NO.Text = "";
            txtROUTING_NM.Text = "";
            dtpFROM_VALID_DT.Text = DateTime.Now.Date.ToString();
            dtpTO_VALID_DT.Text = "2999-12-31";

            txtROU_DEV_USR_ID.Text = "";
            txtROU_MFG_USR_ID.Text = "";
            txtROU_QUR_USR_ID.Text = "";
            txtROU_APP_USR_ID.Text = "";
            txtROU_DEV_USR_NM.Text = "";
            txtROU_MFG_USR_NM.Text = "";
            txtROU_QUR_USR_NM.Text = "";
            txtROU_APP_USR_NM.Text = "";

            txtREV_NO.Text = "";
            dtpREV_DT.Text = DateTime.Now.Date.ToString();

            if (getMajorFlg() == "Y")  // 주라우팅
            {
                rdoROUTINT1.Checked = true;
            }
            else  // 창정비 라우팅
            {
                string Query = " usp_PBA135 @pTYPE = 'S7' ";
                Query += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
                Query += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
                Query += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";
                Query += ", @pMAKEORDER_NO = '" + MAKEORDER_NO.ToString() + "'";
                Query += ", @pITEM_CD='" + CHILD_ITEM_CD + "'";
                Query += ", @pBOM_NO='" + CHILD_BOM_NO + "'";
                Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                // READ ONLY 처리
                txtITEM_CD.Text = CHILD_ITEM_CD;
                txtITEM_NM.Text = dt.Rows[0]["ITEM_NM"].ToString();
                txtROUTING_NO.Text = dt.Rows[0]["ROUT_NO"].ToString();

                txtROUTING_NO.ReadOnly = true;
                txtROUTING_NO.BackColor = Color.Gainsboro;

                rdoROUTINT2.Checked = true;
            }

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

            NewFlg = 1; // 새로 등록으로 처리
        }
        #endregion

        #region txtROUTING_NO_Leave
        private void txtROUTING_NO_Leave(object sender, System.EventArgs e)
        {

            string Query = " usp_PBA135 @pTYPE = 'S2'";
            Query += ", @pPLANT_CD='" + CHILD_PLANT_CD.ToString() + "'";
            Query += ", @pITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
            Query += ", @pROUT_NO='" + txtROUTING_NO.Text + "'";
            Query += ", @pVALID_DT='" + dtpSVALID_DT.Text + "'";
            Query += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
            Query += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
            Query += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";
            Query += ", @pMAKEORDER_NO = '" + MAKEORDER_NO.ToString() + "'";
            Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

            UIForm.FPMake.grdCommSheet(fpSpread2, Query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 2, true);
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                fpSpread2.ActiveSheet.SetActiveCell(0, 1);
                fpSpread2.ActiveSheet.AddSelection(0, 1, 1, 1);
                fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);

                fpSpread2Search(0);
            }
            else
            {
                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0,0, true);
            }

            /////////////////라우팅 정보////////////////////
            string Query2 = " usp_PBA135 @pTYPE = 'S5' ";
            Query2 += ", @pPROJECT_NO='" + PROJECT_NO.ToString() + "'";
            Query2 += ", @pPROJECT_SEQ='" + PROJECT_SEQ.ToString() + "'";
            Query2 += ", @pGROUP_CD='" + GROUP_CD.ToString() + "'";
            Query2 += ", @pMAKEORDER_NO='" + MAKEORDER_NO.ToString() + "'";
            Query2 += ", @pPLANT_CD='" + CHILD_PLANT_CD.ToString() + "'";
            Query2 += ", @pITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
            Query2 += ", @pROUT_NO='" + txtROUTING_NO.Text + "'";
            Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query2);

            if (dt.Rows.Count > 0)
            {
                txtITEM_CD.Text = dt.Rows[0]["ITEM_CD"].ToString();
                txtITEM_NM.Text = dt.Rows[0]["ITEM_NM"].ToString();
                txtROUTING_NO.Text = dt.Rows[0]["ROUT_NO"].ToString();
                txtROUTING_NM.Text = dt.Rows[0]["DESCRIPTION"].ToString();
                dtpFROM_VALID_DT.Text = dt.Rows[0]["VALID_FROM_DT"].ToString();
                dtpTO_VALID_DT.Text = dt.Rows[0]["VALID_TO_DT"].ToString();

                if (dt.Rows[0]["MAJOR_FLG"].ToString() == "Y")
                    rdoROUTINT1.Checked = true;
                else
                    rdoROUTINT1.Checked = false;

            }
            else
            {

                txtROUTING_NM.Text = "";
                dtpFROM_VALID_DT.Text = "";
                dtpTO_VALID_DT.Text = "";

                rdoROUTINT1.Checked = true;

            }
            /////////////////라우팅 정보////////////////////

            //기타세팅
            dtpSVALID_DT.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpFROM_VALID_DT.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpTO_VALID_DT.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpREV_DT.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion


        #region btnPROJECT_Click
        private void btnPROJECT_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P023', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtPROJECT_NO.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00099", strQuery, strWhere, strSearch, "프로젝트 조회", new int[] { 0, 2 }, false);
                pu.Width = 870;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    txtPROJECT_NO.Text = pu.ReturnValue[0].ToString();
                    txtPROJECT_NM.Value = pu.ReturnValue[1].ToString();
                    txtPROJECT_SEQ.Text = pu.ReturnValue[2].ToString();
                    txtSITEM_CD.Text = pu.ReturnValue[3].ToString();
                    txtSITEM_NM.Value = pu.ReturnValue[4].ToString();
                    txtMAKEORDER_NO.Text = pu.ReturnValue[6].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                SystemBase.MessageBoxComm.Show(f.Message);
            }
        }
        #endregion
        
        #region BOM 설계자 조회 조회
        private void btnDEV_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtROU_DEV_USR_ID.Text, txtROU_DEV_USR_NM.Text, "RD" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정설계자 조회", true);

                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtROU_DEV_USR_ID.Text = Msgs[0].ToString();
                    txtROU_DEV_USR_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("PBA116 스케줄 전개 : ", f);
                MessageBox.Show(f.Message);
            }
        }
        #endregion

        #region 생산검토자 조회 조회
        private void btnMFG_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtROU_MFG_USR_ID.Text, txtROU_MFG_USR_NM.Text, "RM" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "생산검토자 조회", true);

                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtROU_MFG_USR_ID.Text = Msgs[0].ToString();
                    txtROU_MFG_USR_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("PBA116 스케줄 전개 : ", f);
                MessageBox.Show(f.Message);
            }
        }
        #endregion

        #region 품질검토자 조회 조회
        private void btnQUR_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtROU_QUR_USR_ID.Text, txtROU_QUR_USR_NM.Text, "RQ" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품질검토자 조회", true);

                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtROU_QUR_USR_ID.Text = Msgs[0].ToString();
                    txtROU_QUR_USR_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("PBA116 스케줄 전개 : ", f);
                MessageBox.Show(f.Message);
            }
        }
        #endregion

        #region 라우팅확인자 조회
        private void btnAPP_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtROU_APP_USR_ID.Text, txtROU_APP_USR_NM.Text, "RA" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "라우팅확인자 조회", true);

                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtROU_APP_USR_ID.Text = Msgs[0].ToString();
                    txtROU_APP_USR_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("PBA116 스케줄 전개 : ", f);
                MessageBox.Show(f.Message);
            }
        }
        #endregion

        #region 라우팅정보 화면 출력
        private void ShowRoutInfo()
        {
            try
            {
                string NODETAG = NEW_NODE_TAG;

                PRNT_PLANT_CD = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                PRNT_ITEM_CD = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                PRNT_BOM_NO = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                CHILD_ITEM_SEQ = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                CHILD_PLANT_CD = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                CHILD_ITEM_CD = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                CHILD_BOM_NO = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                PROJECT_NO = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                PROJECT_SEQ = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                GROUP_CD = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                ITEM_NM = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                MAKEORDER_NO = NODETAG;

                /////////////////라우팅 정보////////////////////
                string Query = " usp_PBA135 @pTYPE = 'S5' ";
                Query += ", @pPROJECT_NO='" + PROJECT_NO.ToString() + "'";
                Query += ", @pPROJECT_SEQ='" + PROJECT_SEQ.ToString() + "'";
                Query += ", @pGROUP_CD='" + GROUP_CD.ToString() + "'";
                Query += ", @pMAKEORDER_NO ='" + MAKEORDER_NO.ToString() + "'";
                Query += ", @pPLANT_CD='" + CHILD_PLANT_CD.ToString() + "'";
                Query += ", @pITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
                Query += ", @pMAJOR_FLG = '" + getMajorFlg() + "'";           // 주라우팅 여부
                Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    txtITEM_CD.Text = dt.Rows[0]["ITEM_CD"].ToString();
                    txtITEM_NM.Text = dt.Rows[0]["ITEM_NM"].ToString();
                    txtROUTING_NO.Text = dt.Rows[0]["ROUT_NO"].ToString();
                    txtROUTING_NM.Text = dt.Rows[0]["DESCRIPTION"].ToString();
                    dtpFROM_VALID_DT.Text = dt.Rows[0]["VALID_FROM_DT"].ToString();
                    dtpTO_VALID_DT.Text = dt.Rows[0]["VALID_TO_DT"].ToString();

                    txtROU_DEV_USR_ID.Text = dt.Rows[0]["ROU_DEV_USER_ID"].ToString();
                    txtROU_MFG_USR_ID.Text = dt.Rows[0]["ROU_MFG_USER_ID"].ToString();
                    txtROU_QUR_USR_ID.Text = dt.Rows[0]["ROU_QUR_USER_ID"].ToString();
                    txtROU_APP_USR_ID.Text = dt.Rows[0]["ROU_APP_USER_ID"].ToString();

                    txtROU_DEV_USR_NM.Text = dt.Rows[0]["ROU_DEV_USER_NM"].ToString();
                    txtROU_MFG_USR_NM.Text = dt.Rows[0]["ROU_MFG_USER_NM"].ToString();
                    txtROU_QUR_USR_NM.Text = dt.Rows[0]["ROU_QUR_USER_NM"].ToString();
                    txtROU_APP_USR_NM.Text = dt.Rows[0]["ROU_APP_USER_NM"].ToString();

                    txtREV_NO.Text = dt.Rows[0]["REV_NO"].ToString();
                    dtpREV_DT.Text = dt.Rows[0]["REV_DT"].ToString();

                    if (dt.Rows[0]["MAJOR_FLG"].ToString() == "Y")
                        rdoROUTINT1.Checked = true;
                    else
                        rdoROUTINT2.Checked = true;

                    SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);
                    dtpTO_VALID_DT.Enabled = true;

                    // 수정 가능한 항목 설정
                    txtROU_DEV_USR_ID.BackColor = Color.White;
                    txtROU_DEV_USR_ID.ReadOnly = false;
                    txtROU_MFG_USR_ID.BackColor = Color.White;
                    txtROU_MFG_USR_ID.ReadOnly = false;
                    txtROU_QUR_USR_ID.BackColor = Color.White;
                    txtROU_QUR_USR_ID.ReadOnly = false;
                    txtROU_APP_USR_ID.BackColor = Color.White;
                    txtROU_APP_USR_ID.ReadOnly = false;

                    txtREV_NO.BackColor = Color.White;
                    txtREV_NO.ReadOnly = false;

                    dtpREV_DT.Enabled = true;

                    btnDEV.Enabled = true;
                    btnMFG.Enabled = true;
                    btnQUR.Enabled = true;
                    btnAPP.Enabled = true;

                    NewFlg = 0;
                }
                else
                {
                    // 제품이나 반제품일 경우 라우팅 등록 활성화
                    Query = " usp_PBA135 @pTYPE = 'S6' ";
                    Query += ", @pITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
                    Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    // 제품,반제품의 경우 ROUTING 정보가 없을 경우 등록 가능하도록 설정
                    if (dt.Rows.Count > 0 &&
                        (dt.Rows[0]["ITEM_ACCT"].ToString() == "10" ||
                         dt.Rows[0]["ITEM_ACCT"].ToString() == "20"))
                    {
                        NewExec();
                    }
                    else
                    {
                        // 화면 초기화 및 LOCK
                        txtITEM_CD.Text = "";
                        txtITEM_NM.Text = "";
                        txtROUTING_NO.Text = "";
                        txtROUTING_NM.Text = "";

                        txtROU_DEV_USR_ID.Text = "";
                        txtROU_MFG_USR_ID.Text = "";
                        txtROU_QUR_USR_ID.Text = "";
                        txtROU_APP_USR_ID.Text = "";

                        txtROU_DEV_USR_NM.Text = "";
                        txtROU_MFG_USR_NM.Text = "";
                        txtROU_QUR_USR_NM.Text = "";
                        txtROU_APP_USR_NM.Text = "";

                        txtREV_NO.Text = "";

                        SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);

                        UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                        UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);
                    }

                    return;

                }

                /////////////////라우팅 정보////////////////////
                Query = " usp_PBA135 @pTYPE = 'S2'";
                Query += ", @pPLANT_CD = '" + CHILD_PLANT_CD + "'";
                Query += ", @pITEM_CD = '" + CHILD_ITEM_CD + "'";
                Query += ", @pMAJOR_FLG = '" + getMajorFlg() + "'";       // 주라우팅 여부
                Query += ", @pVALID_DT = '" + dtpSVALID_DT.Text + "'";
                Query += ", @pPROJECT_NO = '" + PROJECT_NO + "'";
                Query += ", @pPROJECT_SEQ = '" + PROJECT_SEQ + "'";
                Query += ", @pGROUP_CD = '" + GROUP_CD + "'";
                Query += ", @pMAKEORDER_NO = '" + MAKEORDER_NO + "'";
                Query += ", @pBOM_NO   = '" + CHILD_BOM_NO + "'";
                Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread2, Query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 2, true);
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    fpSpread2.ActiveSheet.SetActiveCell(0, 1);
                    fpSpread2.ActiveSheet.AddSelection(0, 1, 1, 1);
                    fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);

                    fpSpread2Search(0);

                    // 외주 관련 활성화
                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value != null &&
                            fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value.ToString() == "N")  // 외주일 경우
                        {
                            // 외주란을 활성화 시간다.
                            UIForm.FPMake.grdReMake(fpSpread2, i,
                                SystemBase.Base.GridHeadIndex(GHIdx2, "외주처") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "통화") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형") + "|1"
                                );
                        }
                    }
                }
                else
                {
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                SystemBase.MessageBoxComm.Show(f.ToString());
            }
        }
        #endregion

        #region 등록/수정 FLAG를 수정으로 설정
        private void txtROU_DEV_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정
        }

        private void txtROU_MFG_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정
        }

        private void txtROU_QUR_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정
        }

        private void txtROU_APP_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정
        }
        #endregion

        #region 주 OR 창정비 라우팅 여부
        private String getMajorFlg()
        {
            if (CHILD_BOM_NO == "C") // 교환일경우
            {
                return "Y"; // 주라우팅 반환
            }
            return "N";     // 보조라우팅 반환
        }
        #endregion

        #region 붙여넣기 처리
        protected override void fpSpread2_ChangeEvent(int Row, int Col)
        {
            // 자원 컬럼
            int rouCol = SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드");
            int resCol = SystemBase.Base.GridHeadIndex(GHIdx2, "자원");

            // 공정정보 붙여넣기 일 경우
            if (fpSpread2.Sheets[0].Cells[Row, rouCol].Text != "")
            {
                string strQuery = "";
                strQuery += " usp_P_COMMON 'P042' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += " , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "'";
                strQuery += " , @pETC='P001'";
                strQuery += " , @pCOM_CD='" + fpSpread2.Sheets[0].Cells[Row, rouCol].Text.Trim() + "'";
                
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {

                    // 자원정보를 조회한다.
                    fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드")].Text = dt.Rows[0]["MINOR_CD"].ToString();	//자원코드
                    fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")].Value = dt.Rows[0]["CD_NM"].ToString();	//자원명

                    UIForm.FPMake.fpChange(fpSpread2, Row);

                }
            }

            // 자원정보 붙여넣기 일 경우
            if (fpSpread2.Sheets[0].Cells[Row, resCol].Text != "")
            {
                string strQuery = "";
                strQuery += " usp_P_COMMON 'P051' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += " , @pCOM_CD='" + fpSpread2.Sheets[0].Cells[Row, resCol].Text + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {

                    // 자원정보를 조회한다.
                    fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원")].Text = dt.Rows[0]["RES_CD"].ToString();	//자원코드
                    fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원명")].Value = dt.Rows[0]["RES_DIS"].ToString();	//자원명
                    fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Value = dt.Rows[0]["WORKCENTER_CD"].ToString();	//작업장
                    fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value = dt.Rows[0]["INSIDE_FLG"].ToString();	//공정타입

                    UIForm.FPMake.fpChange(fpSpread2, Row);

                    if (dt.Rows[0]["INSIDE_FLG"].ToString() == "N")  // 자원이 외주일 경우
                    {
                        // 외주란을 활성화 시간다.
                        UIForm.FPMake.grdReMake(fpSpread2, Row,
                            SystemBase.Base.GridHeadIndex(GHIdx2, "외주처") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "통화") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형") + "|1"
                            );
                        // 초기값 설정
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Value = "KRW";
                    }
                }
            }
        }
        #endregion

        #region fpSpread1_CellClick 이벤트
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
                {
                    if (e.ColumnHeader == true)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "I")
                            {
                                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                            }
                        }
                    }
                }
            }
        }
        #endregion

    }
}