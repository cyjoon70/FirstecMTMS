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
using System.Collections.Generic;

using System.Diagnostics;

namespace QT.QTC004
{
    public partial class QTC004 : UIForm.FPCOMM1
    {
        #region 변수선언

        Thread th;
        UIForm.ExcelWaiting Waiting_Form = null;

        #endregion

        #region 생성자
        public QTC004()
        {
            InitializeComponent();

        }
        #endregion

        #region Form Load 시
        private void QTC004_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            // 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pTYPE = 'PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); //공장
            SystemBase.ComboMake.C1Combo(cboInspClassCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q001', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //검사분류코드

            //cboInspClassCd.SelectedValue = "R";

            //기타 세팅
            dtpReqDtFr.Text = null;
            dtpReqDtTo.Text = null;
            dtpInspDtFr.Text = null;
            dtpInspDtTo.Text = null;

            txtCloseYN.Value = "N";

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

            dtpReqDtFr.Text = null;
            dtpReqDtTo.Text = null;
            dtpInspDtFr.Text = null;
            dtpInspDtTo.Text = null;

            txtCloseYN.Value = "N";
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
                    Stopwatch stopwatch = new Stopwatch(); //객체 선언
                    stopwatch.Start(); // 시간측정 시작

                    string strQuery1 = " usp_QTC004_NEW 'C2'";
                    strQuery1 += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery1 += ", @pPROJECT_NO='" + txtProjectNo.Text.Trim() + "'";
                    strQuery1 += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery1 += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery1 += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1);
                    if (dt.Rows.Count > 0)
                    {
                        txtCloseYN.Value = dt.Rows[0][0].ToString();
                    }
                    else
                    {
                        txtCloseYN.Value = "N";
                    }

                    if (txtCloseYN.Text == "Y")
                    {
                        btnCloseY.Enabled = false;
                        btnCloseN.Enabled = true;

                        btnBOM.Enabled = false;
                        btnINOUT.Enabled = false;
                    }
                    else
                    {
                        btnCloseY.Enabled = true;
                        btnCloseN.Enabled = false;

                        btnBOM.Enabled = true;
                        btnINOUT.Enabled = true;
                    }

                    string strQuery = "usp_QTC004_NEW @pTYPE = 'S1'";
                    strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery += ", @pCUST_CD = '" + txtBpCd.Text + "'";
                    strQuery += ", @pINSPECTOR_CD = '" + txtInspectorCd.Text + "'";
                    strQuery += ", @pINSP_CLASS_CD = '" + cboInspClassCd.SelectedValue + "'";
                    strQuery += ", @pSO_ITEM_CD = '" + txtSoItemCd.Text + "'";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strQuery += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                    strQuery += ", @pWORKORDER_NO = '" + txtWorkOrderNo.Text + "'";
                    strQuery += ", @pINSP_REQ_DT_FR ='" + dtpReqDtFr.Text + "'";
                    strQuery += ", @pINSP_REQ_DT_TO ='" + dtpReqDtTo.Text + "'";
                    strQuery += ", @pINSP_DT_FR ='" + dtpInspDtFr.Text + "'";
                    strQuery += ", @pINSP_DT_TO ='" + dtpInspDtTo.Text + "'";
                    
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 7, true);

                    //fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "품번"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    //fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "품목명"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    
                    #region 2024-01-18 수정 by 조홍태
                    Dictionary<int, int> dicKind = new Dictionary<int, int>();
                    dicKind.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "증빙_2"), 0);
                    dicKind.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "성적서_2"), 0);

                    if (txtCloseYN.Text == "Y")
                    {
                        dicKind.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호"), 2);
                        dicKind.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번"), 2);
                        dicKind.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번_2"), 2);
                    }
                    else
                    {
                        dicKind.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호"), 0);
                        dicKind.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번"), 0);
                        dicKind.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번_2"), 0);
                    }

                    UIForm.FPMake.grdReMake(fpSpread1, dicKind);

                    FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                    MultiType.Multiline = true;

                    FarPoint.Win.Spread.CellType.TextCellType MultiType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                    MultiType1.Multiline = true;

                    Dictionary<int, TextCellType> dicCellType = new Dictionary<int, TextCellType>();
                    dicCellType.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "소재성적서(M/S외)"), MultiType);
                    dicCellType.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "소재(공인성적서)"), MultiType1);
                    dicCellType.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "피막_2"), MultiType);
                    dicCellType.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "도장_2"), MultiType);
                    dicCellType.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "기타"), MultiType);
                    dicCellType.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "COC"), MultiType);
                    dicCellType.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "정부"), MultiType);
                    dicCellType.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "고객사"), MultiType);
                    dicCellType.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "자체"), MultiType);
                    dicCellType.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "COC(제조사)"), MultiType);
                    dicCellType.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "열처리"), MultiType);
                    dicCellType.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "MT"), MultiType);
                    dicCellType.Add(SystemBase.Base.GridHeadIndex(GHIdx1, "RT"), MultiType);

                    foreach (int col in dicCellType.Keys)
                    {
                        fpSpread1.ActiveSheet.Columns[col].CellType = dicCellType[col];
                    }

                    float sizerow = 0;
                    if (fpSpread1.ActiveSheet.RowCount > 0)
                        sizerow = fpSpread1.ActiveSheet.Rows[0].GetPreferredHeight() + 5;

                    fpSpread1.ActiveSheet.SetMultipleRowHeights(0, fpSpread1.ActiveSheet.RowCount, Convert.ToInt32(sizerow));

                    fpSpread1.BackColor = Color.White;
                    #endregion
                    /*
                    #region 기존 소스 주석 처리
                    for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
                    {

                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "증빙_2") + "|0");
                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "성적서_2") + "|0");

                        if (txtCloseYN.Text == "Y")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호") + "|2");
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번") + "|2");
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번_2") + "|2");
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호") + "|0");
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번") + "|0");
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번_2") + "|0");
                        }

                        fpSpread1.Sheets[0].Rows[i].BackColor = Color.White;
						
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
                    #endregion
                    */
                    stopwatch.Stop(); //시간측정 끝
                    MessageBox.Show("Time : " + stopwatch.ElapsedMilliseconds.ToString() + "ms");
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

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (txtCloseYN.Text == "Y")
            {
                MessageBox.Show("마감된 프로젝트는 수정/삭제 할 수 없습니다.", "하위품관리방안", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dsMsg;
            //상단 그룹박스 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    this.Cursor = Cursors.WaitCursor;

                    string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                        //그리드 상단 필수 체크
                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
                        {
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

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수정여부")].Text == "N" && strHead == "D")
                                    {
                                        dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0007"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }
                                    string strSql = " usp_QTC004_NEW '" + strGbn + "'";
                                    strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                                    strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                    strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
                                    strSql += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
                                    strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주품목")].Text.TrimEnd() + "' ";
                                    strSql += ", @pCHILD_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text + "' ";
                                    strSql += ", @pFIGNO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "FIGNO")].Text + "' ";
                                    strSql += ", @pMVMT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text + "' ";
                                    strSql += ", @pMVMT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text + "' ";
                                    
                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();
                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                                }
                            }
                        }
                        else
                        {
                            Trans.Rollback();
                            this.Cursor = Cursors.Default;
                            return;
                        }
                        Trans.Commit();
                    }
                    catch (Exception e)
                    {
                        SystemBase.Loggers.Log(this.Name, e.ToString());
                        Trans.Rollback();
                        ERRCode = "ER";
                        MSGCode = e.Message;
                        //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
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

        #region CancelExec() 폼에 입력된 데이타 저장 로직
        protected override void CancelExec()
        {
            int Row = fpSpread1.ActiveSheet.ActiveRowIndex;
            fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text = "";
            fpSpread1.Sheets[0].RowHeader.Rows[Row].BackColor = SystemBase.Base.Color_Org;
            fpSpread1.Sheets[0].Rows[Row].BackColor = Color.White;
        }
        #endregion

        #region DelExe() 폼에 입력된 데이타 저장 로직
        protected override void DelExe()
        {
            int Row = fpSpread1.ActiveSheet.ActiveRowIndex;
            fpSpread1.Sheets[0].Rows[Row].BackColor = SystemBase.Base.Color_Delete;
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
        //수주품목
        private void btnSoItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005("10");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSoItemCd.Text = Msgs[2].ToString();
                    txtSoItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목
        private void btnItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005("10");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //검사의뢰번호
        private void btnInspReqNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW009 pu = new WNDW009(Convert.ToString(cboPlantCd.SelectedValue)
                    , txtInspReqNo.Text
                    , cboInspClassCd.SelectedValue.ToString()
                    , "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtInspReqNo.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사의뢰번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        //공급처
        private void btnBpCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtBpCd.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtBpCd.Value = Msgs[1].ToString();
                    txtBpNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공급처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //검사원
        private void btnInspectorCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtInspectorCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "검사원 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtInspectorCd.Text = Msgs[0].ToString();
                    txtInspectorNm.Value = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //수주품목 
        private void txtSoItemCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtSoItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtSoItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목명 가져오기"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //품목 
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목명 가져오기"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
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
        //공급처
        private void txtBpCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtBpCd.Text != "")
                {
                    txtBpNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtBpCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtBpNm.Value = "";
                }
            }
            catch
            {

            }
        }
        //검사원
        private void txtInspectorCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtInspectorCd.Text != "")
                {
                    txtInspectorNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtInspectorCd.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtInspectorNm.Value = "";
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
                        string strItemCd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text;    // 2015.04.27. hma 수정: 그리드헤더명 변경 품목코드=>품목
                        string strUnit = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;
                        string InspQty = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합격수량")].Text;
                        string strMvmtNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text;
                        string strMvmtSeq = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text;
                        string strItemAcct = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")].Text;

                        QTC004P1 frm1 = new QTC004P1(cboPlantCd.SelectedValue.ToString(), strProjectNo, strProjectSeq, strLotNo, strItemCd, strItemAcct, strUnit, InspQty, strMvmtNo, strMvmtSeq);
                        frm1.ShowDialog();
                    }
                }
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번_2"))
                {

                    string strProjectNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                    string strProjectSeq = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
                    string strItemCd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text;      // 2015.04.27. hma 수정: 그리드헤더명 변경 품목코드=>품목
                    string strUnit = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;
                    string sQty = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "총소요량")].Text;
                    string strItemAcct = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")].Text;
                    

                    QTC004P2 frm1 = new QTC004P2(cboPlantCd.SelectedValue.ToString(), strProjectNo, strProjectSeq, strItemCd, strItemAcct, strUnit, sQty);
                    frm1.ShowDialog();

                    if (frm1.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = frm1.ReturnVal;

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Value = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text = Msgs[7].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수기여부")].Text = "Y";

                        //fpSpread1.Sheets[0].Rows[e.Row].BackColor = SystemBase.Base.Color_Update;

                    }
                    else
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
                        fpSpread1.Sheets[0].RowHeader.Rows[e.Row].BackColor = SystemBase.Base.Color_Org;
                        fpSpread1.Sheets[0].Rows[e.Row].BackColor = Color.White;
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

        #region QBOM 생성
        private void btnBOM_Click(object sender, EventArgs e)
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    string strQuery = " usp_QTC004_NEW 'C1'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPROJECT_NO='" + txtProjectNo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                    if (dt.Rows.Count > 0)
                    {
                        string msg = "프로젝트번호 : " + txtProjectNo.Text.Trim() + ", 차수 : " + txtProjectSeq.Text.Trim() + " 의 데이타가 존재합니다. \n\n재생성시 모든 데이타가 지워집니다!!!. \n\n다시 생성하시겠습니까?";
                        DialogResult dsMsg1 = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (dsMsg1 == DialogResult.No)
                        {
                            this.Cursor = Cursors.Default;
                            return;
                        }
                    }
                    
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                    this.Cursor = Cursors.Default;
                    return;
                }


                string ERRCode = "", MSGCode = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd1 = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    string strQuery = " usp_QTC004_NEW 'I1'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "OK")
                    {
                        Trans.Commit();
                    }
                    else
                    { 
                        Trans.Rollback(); 
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                }

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
        #endregion

        #region 입출고 생성
        private void btnINOUT_Click(object sender, EventArgs e)
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    string strQuery = " usp_QTC004_NEW 'C1'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPROJECT_NO='" + txtProjectNo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                    if (dt.Rows.Count > 0)
                    {
                        string msg = "프로젝트번호 : " + txtProjectNo.Text.Trim() + ", 차수 : " + txtProjectSeq.Text.Trim() + " 의 데이타가 존재합니다. \n\n재 생성시 수동 등록된 입출고 자료를 제외하고 \n\n모든 입출고 자료가 다시 생성됩니다!!!. \n\n다시 생성하시겠습니까?";
                        DialogResult dsMsg1 = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (dsMsg1 == DialogResult.No)
                        {
                            this.Cursor = Cursors.Default;
                            return;
                        }
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                    this.Cursor = Cursors.Default;
                    return;
                }


                string ERRCode = "", MSGCode = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd1 = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    string strQuery = " usp_QTC004_NEW 'I2'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "OK")
                    {
                        Trans.Commit();
                    }
                    else
                    {
                        Trans.Rollback();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                }

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
        #endregion

        #region 마감, 마감취소
        private void btnCloseY_Click(object sender, EventArgs e)
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                string ERRCode = "", MSGCode = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd1 = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    string strQuery = " usp_QTC004_NEW 'I3'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery += ", @pCLOSE_YN = 'Y' ";
                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "OK")
                    {
                        Trans.Commit();
                    }
                    else
                    {
                        Trans.Rollback();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                }

                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //SearchExec();
                    txtCloseYN.Value = "Y";
                    btnCloseY.Enabled = false;
                    btnCloseN.Enabled = true;

                    btnBOM.Enabled = false;
                    btnINOUT.Enabled = false;
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

        private void btnCloseN_Click(object sender, EventArgs e)
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                string ERRCode = "", MSGCode = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd1 = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    string strQuery = " usp_QTC004_NEW 'I3'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery += ", @pCLOSE_YN = 'N' ";
                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "OK")
                    {
                        Trans.Commit();
                    }
                    else
                    {
                        Trans.Rollback();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                }

                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //SearchExec();
                    txtCloseYN.Value = "N";
                    btnCloseY.Enabled = true;
                    btnCloseN.Enabled = false;

                    btnBOM.Enabled = true;
                    btnINOUT.Enabled = true;
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

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (dt.Rows.Count > 0)
            {

                try
                {
                    // 2015.04.20. hma 수정(Start): 실행시 오류가 발생하여 Waiting Form이 나오지 않도록 주석 처리함.
                    //th = new Thread(new ThreadStart(Show_Waiting));        
                    //th.Start();

                    //Thread.Sleep(200);
                    //Waiting_Form.Activate();

                    //Waiting_Form.progressBar_temp.Maximum = dt.Rows.Count;
                    // 2015.04.20. hma 수정(End)

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
                            excel.SetCell(iStart - 23, 7, dt.Rows[i]["SPEC_NO"].ToString());
                            excel.SetCell(iStart - 22, 7, dt.Rows[i]["DRAW_NO"].ToString());
                            excel.SetCell(iStart - 21, 7, dt.Rows[i]["ITEM_SPEC"].ToString());
                            excel.SetCell(iStart - 20, 7, dt.Rows[i]["LOT_SIZE_STOCK_UNIT"].ToString());
                            excel.SetCell(iStart - 19, 7, dt.Rows[i]["INSP_QTY"].ToString());

                            excel.SetCell(iStart - 23, 15, dt.Rows[i]["INSP_METH_NM"].ToString()); ;

                            if (dt.Rows[i]["INSP_DT"].ToString() != "")
                                excel.SetCell(iStart - 20, 15, dt.Rows[i]["INSP_DT"].ToString());

                            // 2015.05.07. hma 추가(Start): 검사책임자를 넘겨받아서 출력하도록 함.
                            excel.SetCell(iStart - 21, 15, dt.Rows[i]["QC_MAN_NAME"].ToString());
                            // 2015.05.07. hma 추가(End)

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

                        //Waiting_Form.progressBar_temp.Value = i + 1;   // 2015.04.20. hma 실행시 오류가 발생하여 Waiting Form이 나오지 않도록 주석 처리함.
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
                    // 2015.04.20. hma 수정(Start): 실행시 오류가 발생하여 Waiting Form이 나오지 않도록 주석 처리함.
                    //Waiting_Form.label_temp.Text = "완료되었습니다.";
                    //Thread.Sleep(500);
                    // 2015.04.20. hma 수정(End)
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
                    // 2015.04.20. hma 수정(Start): 실행시 오류가 발생하여 Waiting Form이 나오지 않도록 주석 처리함.
                    //Waiting_Form.Close();
                    //th.Abort();
                    // 2015.04.20. hma 수정(End)
                    File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
                }
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        private void Show_Waiting()
        {
            Waiting_Form = new UIForm.ExcelWaiting("검사성적서출력...");
            Waiting_Form.ShowDialog();
        }
        #endregion

        #region 한글 잘림 해결 테스트
        private void txtTest_Leave(object sender, EventArgs e)
        {
            
        }
        #endregion

    }
}
