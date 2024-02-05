#region 작성정보
/*********************************************************************/
// 단위업무명 : 품질관리/추적관리/사용재고 현황조회
// 작 성 자   : 김 창 진
// 작 성 일   : 2014-08-22
// 작성내용   : 사용재고 현황조회
// 수 정 일   :
// 수 정 자   :
// 수정내용   :
// 비    고   :
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
using C1.C1Pdf;
using FarPoint.Win;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using System.Threading;
using System.IO;

namespace QT.QTC013
{
    public partial class QTC013 : UIForm.FPCOMM1
    {
        #region 변수선언

        Thread th;
        UIForm.ExcelWaiting Waiting_Form = null;

        #endregion

        #region 생성자
        public QTC013()
        {
            InitializeComponent();

        }
        #endregion

        #region Form Load 시
        private void QTC013_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            // 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pTYPE = 'PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); //공장
            
            //cboInspClassCd.SelectedValue = "R";

            //dtpInspDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString().Substring(0, 10);
            //dtpInspDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 10);

            //그리드 세팅
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                try
                {

                    string strQuery = "usp_QTC013 @pTYPE = 'S1'";
                    strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery += ", @pWORKORDER_NO = '" + txtWorkOrderNo.Text + "'";


                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 7, true);

                    //fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "품번"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    //fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "품목명"), FarPoint.Win.Spread.Model.MergePolicy.Always);


                    for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
                    {

                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "증빙_2") + "|0");
                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "성적서_2") + "|0");

                        FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                        MultiType.Multiline = true;

                        FarPoint.Win.Spread.CellType.TextCellType MultiType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                        MultiType1.Multiline = true;

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "소재성적서(M/S외)")].CellType = MultiType;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "소재(공인성적서)")].CellType = MultiType1;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "피막_2")].CellType = MultiType;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도장_2")].CellType = MultiType;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기타")].CellType = MultiType;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "COC")].CellType = MultiType;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "정부")].CellType = MultiType;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "고객사")].CellType = MultiType;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자체")].CellType = MultiType;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "COC(제조사)")].CellType = MultiType;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "열처리")].CellType = MultiType;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MT")].CellType = MultiType;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "RT")].CellType = MultiType;

                        //Row Height 변경
                        FarPoint.Win.Spread.Row row;
                        float sizerow;
                        row = fpSpread1.ActiveSheet.Rows[i];
                        sizerow = row.GetPreferredHeight();
                        row.Height = sizerow + 5;

                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 조회조건 팝업
        //프로젝트번호 
        //프로젝트번호 
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW007 pu = new WNDW.WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트 차수
        private void btnProjectSeq_Click(object sender, EventArgs e)
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

        //제조오더번호
        private void btnWorkOrderNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNo.Text);
                pu.MaximizeBox = false;
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사의뢰번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //프로젝트 번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {

            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region fpSpread1 ButtonClicked  팝업
        private void fpSpread1_ButtonClicked(object sender, EditorNotifyEventArgs e)
        {
            if (e.Row >= 0)
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "증빙_2"))
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
                    fpSpread1.Sheets[0].RowHeader.Rows[e.Row].BackColor = SystemBase.Base.Color_Org;

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text != "")
                    {
                        string strProjectNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                        string strProjectSeq = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
                        string strLotNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "LOT NO")].Text;
                        string strItemCd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                        string strUnit = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;
                        string InspQty = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합격수량")].Text;
                        string strMvmtNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text;
                        string strMvmtSeq = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text;
                        string strItemAcct = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")].Text;

                        QTC013P1 frm1 = new QTC013P1(cboPlantCd.SelectedValue.ToString(), strProjectNo, strProjectSeq, strLotNo, strItemCd, strItemAcct, strUnit, InspQty, strMvmtNo, strMvmtSeq);
                        frm1.ShowDialog();
                    }
                }
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "성적서_2"))
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
                    fpSpread1.Sheets[0].RowHeader.Rows[e.Row].BackColor = SystemBase.Base.Color_Org;

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text != "")
                    {
                        string strInspReqNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text;
                        string strUnityInspReqNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "통합의뢰번호")].Text;
                        string strItemAcct = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")].Text;

                        Print_Doc(strInspReqNo, strUnityInspReqNo, strItemAcct);

                        fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
                        fpSpread1.Sheets[0].RowHeader.Rows[e.Row].BackColor = SystemBase.Base.Color_Org;
                        fpSpread1.Sheets[0].Rows[e.Row].BackColor = Color.White;
                    }
                }
            }
        }
        #endregion

        #region 성적서 출력
        private void Print_Doc(string sInspReqNo, string sUnityInspReqNo, string sItemAcct)
        {
            string strInspReqNo = "";
            string strInspItemCd = "";
            string strUnityInspReqNo = "";
            string strINSP_CLASS_CD = "";

            bool bHeard = true;
            int iStart = 28;
            int iListRow = 18;
            int iCel = 0;
            int[] iAddCol = { 6, 8, 10, 12, 14, 16, 18, 20, 22, 24 };
            int iPage = 1;


            string strFileName = "";
            string strPrint_Type = "";


            strFileName = SystemBase.Base.ProgramWhere + @"\Report\검사성적서.xls";
            strPrint_Type = "A";


            string strSheetPage1 = "검사성적서";

            string strGbn = "";

            if (sItemAcct == "10" || sItemAcct == "20")
            {
                strGbn = "R2"; //최종검사
                strINSP_CLASS_CD = "F";
            }
            else
            {
                strGbn = "R1";
                strINSP_CLASS_CD = "R"; //수입검사
            }

            if (sUnityInspReqNo != "")
            {
                sInspReqNo = sUnityInspReqNo;
            }

            string strQuery = " usp_QRE010  @pTYPE = '" + strGbn + "'";
            strQuery += ", @pINSP_CLASS_CD = '" + strINSP_CLASS_CD + "'";
            strQuery += ", @pPLANT_CD = '" + Convert.ToString(cboPlantCd.SelectedValue) + "'";
            //strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
            //strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
            strQuery += ", @pINSP_REQ_NO_FR = '" + sInspReqNo + "'";
            strQuery += ", @pINSP_REQ_NO_TO = '" + sInspReqNo + "'";
            //strQuery += ", @pINSP_REQ_DT_FR = '" + dtpInspReqDtFr.Text + "'";
            //strQuery += ", @pINSP_REQ_DT_TO = '" + dtpInspReqDtTo.Text + "'";
            //strQuery += ", @pINSP_DT_FR = '" + dtpInspDtFr.Text + "'";
            //strQuery += ", @pINSP_DT_TO = '" + dtpInspDtTo.Text + "'";
            strQuery += ", @pPRINT_TYPE = '" + strPrint_Type + "'";
            strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);


            if (dt.Rows.Count > 0)
            {

                try
                {

                    th = new Thread(new ThreadStart(Show_Waiting));
                    th.Start();

                    Thread.Sleep(200);
                    Waiting_Form.Activate();

                    Waiting_Form.progressBar_temp.Maximum = dt.Rows.Count;

                    UIForm.VkExcel excel = null;

                    if (File.Exists(strFileName))
                    {
                        File.SetAttributes(strFileName, System.IO.FileAttributes.ReadOnly);
                    }
                    else
                    {
                        // 엑셀 데이터를 생성할 수 없습니다. 원본 파일이 존재하지 않습니다.
                        MessageBox.Show("엑셀 데이터를 생성할 수 없습니다. 원본 파일이 존재하지 않습니다."); ;
                        return;
                    }

                    #region excel export

                    excel = new UIForm.VkExcel(false);

                    excel.OpenFile(strFileName);

                    // 현재 시트 선택
                    excel.FindExcelWorksheet(strSheetPage1);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (i > 0)
                        {
                            if (dt.Rows[i]["INSP_REQ_NO"].ToString() != strInspReqNo)
                            {
                                bHeard = true;
                            }
                            else
                            {
                                bHeard = false;
                            }
                        }

                        // Heard 값
                        if (bHeard == true)
                        {

                            if (i > 0 && strUnityInspReqNo != "")
                            {
                                string strQuery2 = " usp_QRE010  @pTYPE = 'R3'";
                                strQuery2 += ", @pINSP_REQ_NO_FR = '" + strUnityInspReqNo + "'";
                                strQuery2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataTable dt2 = SystemBase.DbOpen.NoTranDataTable(strQuery2);

                                if (dt2.Rows.Count > 0)
                                {
                                    excel.SetSelect("A" + iStart, "A" + iStart);
                                    excel.RunMacro("PageOrderAdd");

                                    iStart += 37;

                                    iPage++;

                                    excel.SetCell(iStart - 37, 3, dt.Rows[i - 1]["PROJECT_NM"].ToString());
                                    excel.SetCell(iStart - 37, 7, dt.Rows[i - 1]["ITEM_NM"].ToString());
                                    excel.SetCell(iStart - 37, 15, dt.Rows[i - 1]["ITEM_CD"].ToString());
                                    excel.SetCell(iStart - 37, 24, dt.Rows[i - 1]["TPAGE"].ToString() + " 매중 " + iPage.ToString() + " 매");

                                    for (int j = 0; j < dt2.Rows.Count; j++)
                                    {
                                        if (j <= 34)
                                        {
                                            excel.SetCell(iStart - 35 + j, 2, dt2.Rows[j]["INSP_REQ_NO"].ToString());
                                            excel.SetCell(iStart - 35 + j, 13, dt2.Rows[j]["WORKORDER_NO"].ToString());
                                        }
                                        else
                                        {
                                            excel.SetCell(iStart - 35 + j - 35, 5, dt2.Rows[j]["INSP_REQ_NO"].ToString());
                                            excel.SetCell(iStart - 35 + j - 35, 22, dt2.Rows[j]["WORKORDER_NO"].ToString());
                                        }
                                    }
                                }

                            }

                            if (i > 0)
                            {
                                excel.SetSelect("A" + iStart, "A" + iStart);
                                excel.RunMacro("PageHeadAdd");
                                iStart += 27;
                                iListRow = 18;
                            }

                            excel.SetCell(iStart - 27, 24, dt.Rows[i]["TPAGE"].ToString() + " 매중 1 매");

                            excel.SetCell(iStart - 24, 15, dt.Rows[i]["INSP_REQ_NO"].ToString());
                            if (dt.Rows[i]["UNITY_INSP_REQ_NO"].ToString() != "")
                            {
                                string strQuery1 = " usp_QRE010  @pTYPE = 'R3'";
                                strQuery1 += ", @pINSP_REQ_NO_FR = '" + dt.Rows[i]["INSP_REQ_NO"].ToString() + "'";
                                strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(strQuery1);

                            }

                            //사업명 계약번호 재고번호품명 제작처&구입처
                            excel.SetCell(iStart - 24, 3, dt.Rows[i]["PROJECT_NM"].ToString());
                            excel.SetCell(iStart - 23, 3, dt.Rows[i]["PROJECT_NO"].ToString());
                            excel.SetCell(iStart - 22, 3, dt.Rows[i]["KKJGBH"].ToString());
                            excel.SetCell(iStart - 21, 3, dt.Rows[i]["ITEM_NM"].ToString());
                            excel.SetCell(iStart - 20, 3, dt.Rows[i]["MAKE_BUY"].ToString());

                            excel.SetCell(iStart - 19, 3, dt.Rows[i]["MATERIAL"].ToString());

                            //품목코드 규격번호 도면번호/REV.NO 부품번호 로트수량(단위), 검사수량(단위)
                            excel.SetCell(iStart - 24, 7, dt.Rows[i]["ITEM_CD"].ToString());
                            excel.SetCell(iStart - 23, 7, dt.Rows[i]["BUPUM_NO"].ToString());
                            excel.SetCell(iStart - 22, 7, dt.Rows[i]["DRAW_NO"].ToString());
                            excel.SetCell(iStart - 20, 7, dt.Rows[i]["LOT_SIZE_STOCK_UNIT"].ToString());
                            excel.SetCell(iStart - 19, 7, dt.Rows[i]["INSP_QTY"].ToString());


                            excel.SetCell(iStart - 23, 15, dt.Rows[i]["INSP_METH_NM"].ToString()); ;

                            if (dt.Rows[i]["INSP_DT"].ToString() != "")
                                excel.SetCell(iStart - 20, 15, dt.Rows[i]["INSP_DT"].ToString());


                            if (dt.Rows[i]["INSPECTOR_NM"].ToString() != "")
                                excel.SetCell(iStart - 19, 15, dt.Rows[i]["INSPECTOR_NM"].ToString());

                            strInspReqNo = dt.Rows[i]["INSP_REQ_NO"].ToString();
                            strUnityInspReqNo = dt.Rows[i]["UNITY_INSP_REQ_NO"].ToString();

                            iPage = 1;

                        }

                        //내용입력
                        if (strInspItemCd != dt.Rows[i]["INSP_ITEM_CD"].ToString() || bHeard == true)
                        {

                            if (iListRow <= 2)//리스트페이지를 불러와야할때
                            {

                                excel.SetSelect("A" + iStart, "A" + iStart);
                                excel.RunMacro("PageListAdd");
                                iStart += 23;


                                iPage++;
                                iListRow = 20;
                                excel.SetCell(iStart - iListRow - 3, 24, dt.Rows[i]["TPAGE"].ToString() + " 매중 " + iPage.ToString() + " 매");
                                excel.SetCell(iStart - iListRow - 3, 3, dt.Rows[i]["PROJECT_NM"].ToString());
                                excel.SetCell(iStart - iListRow - 3, 7, dt.Rows[i]["ITEM_NM"].ToString());
                                excel.SetCell(iStart - iListRow - 3, 15, dt.Rows[i]["ITEM_CD"].ToString());

                            }
                            else
                            {
                                iListRow -= 2;
                            }

                            excel.SetCell(iStart - iListRow, 1, dt.Rows[i]["INSP_SEQ"].ToString());
                            excel.SetCell(iStart - iListRow, 2, dt.Rows[i]["INSP_ITEM_NM"].ToString());
                            excel.SetCell(iStart - iListRow + 1, 2, dt.Rows[i]["MAP_COOR"].ToString());
                            excel.SetCell(iStart - iListRow, 3, dt.Rows[i]["INSP_SPEC"].ToString().Replace("\r\n", "\n"));
                            excel.SetCell(iStart - iListRow, 4, dt.Rows[i]["MEASURE_NM"].ToString());
                            excel.SetCell(iStart - iListRow, 26, dt.Rows[i]["AQL"].ToString());

                            strInspItemCd = dt.Rows[i]["INSP_ITEM_CD"].ToString();
                            iCel = 0;
                        }
                        else
                        {

                            if (iCel > 8)
                            {
                                if (iListRow <= 2)//리스트페이지를 불러와야할때
                                {
                                    excel.SetSelect("A" + iStart, "A" + iStart);
                                    excel.RunMacro("PageListAdd");
                                    iStart += 23;
                                    iListRow = 20;

                                    iPage++;
                                    excel.SetCell(iStart - iListRow - 3, 24, dt.Rows[i]["TPAGE"].ToString() + " 매중 " + iPage.ToString() + " 매");
                                    excel.SetCell(iStart - iListRow - 3, 3, dt.Rows[i]["PROJECT_NM"].ToString());
                                    excel.SetCell(iStart - iListRow - 3, 7, dt.Rows[i]["ITEM_NM"].ToString());
                                    excel.SetCell(iStart - iListRow - 3, 15, dt.Rows[i]["ITEM_CD"].ToString());
                                }
                                else
                                {
                                    iListRow -= 2;
                                }
                                iCel = 0;
                            }
                            else
                            {
                                iCel++;
                            }
                        }

                        if (dt.Rows[i]["VALUE"].ToString() != "")
                        {
                            excel.SetCell(iStart - iListRow, iAddCol[iCel], dt.Rows[i]["VALUE"].ToString());
                        }

                        Waiting_Form.progressBar_temp.Value = i + 1;
                    }

                    if (dt.Rows.Count > 0 && strUnityInspReqNo != "")
                    {
                        string strQuery3 = " usp_QRE010  @pTYPE = 'R3'";
                        strQuery3 += ", @pINSP_REQ_NO_FR = '" + strUnityInspReqNo + "'";
                        strQuery3 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt3 = SystemBase.DbOpen.NoTranDataTable(strQuery3);

                        if (dt3.Rows.Count > 0)
                        {
                            excel.SetSelect("A" + iStart, "A" + iStart);
                            excel.RunMacro("PageOrderAdd");

                            iStart += 37;

                            iPage++;

                            excel.SetCell(iStart - 37, 3, dt.Rows[dt.Rows.Count - 1]["PROJECT_NM"].ToString());
                            excel.SetCell(iStart - 37, 7, dt.Rows[dt.Rows.Count - 1]["ITEM_NM"].ToString());
                            excel.SetCell(iStart - 37, 15, dt.Rows[dt.Rows.Count - 1]["ITEM_CD"].ToString());
                            excel.SetCell(iStart - 37, 24, dt.Rows[dt.Rows.Count - 1]["TPAGE"].ToString() + " 매중 " + iPage.ToString() + " 매");

                            for (int j = 0; j < dt3.Rows.Count; j++)
                            {
                                if (j <= 34)
                                {
                                    excel.SetCell(iStart - 35 + j, 2, dt3.Rows[j]["INSP_REQ_NO"].ToString());
                                    excel.SetCell(iStart - 35 + j, 13, dt3.Rows[j]["WORKORDER_NO"].ToString());
                                }
                                else
                                {
                                    excel.SetCell(iStart - 35 + j - 35, 5, dt3.Rows[j]["INSP_REQ_NO"].ToString());
                                    excel.SetCell(iStart - 35 + j - 35, 22, dt3.Rows[j]["WORKORDER_NO"].ToString());
                                }
                            }
                        }

                    }

                    excel.SetSelect("A1", "A1");
                    Waiting_Form.label_temp.Text = "완료되었습니다.";
                    Thread.Sleep(500);
                    excel.ShowExcel(true);

                    #endregion

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사성적서출력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Waiting_Form.Close();
                    th.Abort();
                    File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
                }
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void Show_Waiting()
        {
            Waiting_Form = new UIForm.ExcelWaiting("검사성적서출력...");
            Waiting_Form.ShowDialog();
        }
        #endregion
    }
}
