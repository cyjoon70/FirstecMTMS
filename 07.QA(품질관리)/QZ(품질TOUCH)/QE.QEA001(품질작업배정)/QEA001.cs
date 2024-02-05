#region 작성정보
/*********************************************************************/
// 단위업무명 : 품질작업배정
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-03
// 작성내용 : 품질작업배정 및 관리
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
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using FarPoint.Win.Spread;
using WNDW;

namespace QE.QEA001
{
    public partial class QEA001 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        int SearchRow = 0;
        int ShowColumn = 0;

        private string strMQuery;
        string strWorkOrderNo = "";
        string strProcSeq = "";
        string strMfgType = "";
        string strSheetStartDt = "";
        string strSheetComptDt = "";
        string strWcCd = ""; 
        string strResCd = "";
        string strJobCd = "";
        string strInspReqNo = "";
        int BalQty = 0, WorkTmStd = 0;
        int Row = 0;
        string Key = "";
        int SaveRow = 0;
        #endregion

        #region 생성자
        public QEA001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void QEA001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

            //기타 세팅
            dtpStartDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddDays(-7).ToShortDateString();
            dtpStartDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddDays(7).ToShortDateString();

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region 행복사 버튼 클릭 이벤트
        protected override void RCopyExe()
        {
            UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex,
                SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드") + "|3"
                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드_2") + "|3"
                );
        }
        #endregion 

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅
            dtpStartDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddDays(-7).ToShortDateString();
            dtpStartDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddDays(7).ToShortDateString();

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD.ToString();
        }
        #endregion

        #region 조회조건 팝업
        //공장코드
        private void btnPlant_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P013', @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' "; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };											  // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtPlantCd.Text, "" };															  // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회");

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

        //프로젝트번호
        private void btnProject_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1", "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = Msgs[5].ToString();
                    txtProjectNo.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업장
        private void btnWc_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P612', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05009", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
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

        //품목
        private void btnItem_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtItemCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Value = Msgs[2].ToString();
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

        private void btnJob_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P001', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtJobCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정작업코드 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtJobCd.Text = Msgs[0].ToString();
                    txtJobNm.Value = Msgs[1].ToString();
                    txtJobCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //검사의뢰번호From
        private void btnInspReqNoFr_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_Q_COMMON @pType='Q081', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE" };
                string[] strSearch = new string[] { txtInspReqNoFr.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P0Q001", strQuery, strWhere, strSearch, new int[] { 0 }, "검사의뢰번호 조회");
                pu.Width = 1000;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtInspReqNoFr.Text = Msgs[0].ToString();
                    txtInspReqNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사의뢰번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //검사의뢰번호To
        private void btnInspReqNoTo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_Q_COMMON @pType='Q081', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE" };
                string[] strSearch = new string[] { txtInspReqNoTo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P0Q001", strQuery, strWhere, strSearch, new int[] { 0 }, "검사의뢰번호 조회");
                pu.Width = 1000;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtInspReqNoTo.Text = Msgs[0].ToString();
                    txtInspReqNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사의뢰번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업자
        private void btnHres_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_Q_COMMON @pType='Q100', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtHresCd.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05003", strQuery, strWhere, strSearch, new int[] { 0 }, "작업자 조회");
                pu.Width = 400;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtHresCd.Value = Msgs[0].ToString();
                    txtHresNm.Value = Msgs[1].ToString();
                    txtHresCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //공장
        private void txtPlantCd_TextChanged(object sender, EventArgs e)
        {
            txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //품목코드
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //작업장
        private void txtWcCd_TextChanged(object sender, EventArgs e)
        {
            txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P002'  AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            if (txtProjectNo.Text == "")
            {
                txtProjectNm.Value = "";
                txtProjectSeq.Text = "";
            }
        }

        //작업코드
        private void txtJobCd_TextChanged(object sender, EventArgs e)
        {
            txtJobNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtJobCd.Text, " AND MAJOR_CD = 'P001'  AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //작업자
        private void txtHresCd_TextChanged(object sender, EventArgs e)
        {
            txtHresNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtHresCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "' AND USE_YN = 'Y' AND RES_KIND = 'L' ");
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            SaveRow = 0;
            Search("");
        }

        private void Search(string InspReqNo)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    string strInspClassCd = "";
                    if (rdoR.Checked == true)
                    {
                        strInspClassCd = "R";
                    }
                    else if (rdoP.Checked == true)
                    {
                        strInspClassCd = "P";
                    }
                    else if (rdoF.Checked == true)
                    {
                        strInspClassCd = "F";
                    }

                    strMQuery = " usp_QEA001 'S1'";
                    strMQuery += ", @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                    strMQuery += ", @pPROJECT_NO ='" + txtProjectNo.Text + "'";
                    strMQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strMQuery += ", @pITEM_CD ='" + txtItemCd.Text + "'";
                    strMQuery += ", @pWC_CD ='" + txtWcCd.Text + "'";
                    strMQuery += ", @pREF_NO_FR ='" + txtRefNoFr.Text + "'";
                    strMQuery += ", @pREF_NO_TO ='" + txtRefNoTo.Text + "'";
                    strMQuery += ", @pINSP_REQ_NO_FR ='" + txtInspReqNoFr.Text + "'";
                    strMQuery += ", @pINSP_REQ_NO_TO = '" + txtInspReqNoTo.Text + "' ";
                    strMQuery += ", @pINSP_REQ_DT_FR ='" + dtpStartDtFr.Text + "'";
                    strMQuery += ", @pINSP_REQ_DT_TO = '" + dtpStartDtTo.Text + "' ";
                    strMQuery += ", @pINSP_CLASS_CD = '" + strInspClassCd + "' ";
                    strMQuery += ", @pVER = '2' ";
                    strMQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strMQuery += ", @pH_RES_CD ='" + txtHresCd.Text + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strMQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);

                    fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        for (int row = 0; row < fpSpread2.Sheets[0].Rows.Count; row++)
                        {
                            if (Convert.ToInt32(fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "지체일수")].Value) < 0)
                            {
                                fpSpread2.Sheets[0].Cells[row, 0, row, fpSpread2.Sheets[0].Columns.Count - 1].ForeColor = Color.Red;
                            }
                        }

                        int x = 0, y = 0;

                        if (InspReqNo != "")
                        {
                            fpSpread2.Search(0, InspReqNo, false, false, false, false, 0, 0, ref x, ref y);

                            if (x > 0)
                            {
                                fpSpread2.Sheets[0].SetActiveCell(x, 1);
                                fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                            }
                            else
                            {
                                if (SaveRow <= fpSpread2.Sheets[0].Rows.Count)
                                {
                                    x = SaveRow;
                                    fpSpread2.Sheets[0].SetActiveCell(x, 1);
                                    fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                                }
                                else
                                {
                                    x = fpSpread2.Sheets[0].Rows.Count - 1;
                                    fpSpread2.Sheets[0].SetActiveCell(x, 1);
                                    fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                                }
                            }
                        }
                        else
                        {
                            if (SaveRow <= fpSpread2.Sheets[0].Rows.Count)
                            {
                                x = SaveRow;
                                fpSpread2.Sheets[0].SetActiveCell(x, 1);
                                fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                            }
                            else
                            {
                                x = fpSpread2.Sheets[0].Rows.Count - 1;
                                fpSpread2.Sheets[0].SetActiveCell(x, 1);
                                fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                            }
                        }

                        fpSpread2.Sheets[0].AddSelection(x, 1, 1, fpSpread2.Sheets[0].ColumnCount);

//                        FpSpread2CellClick(x, 0);
                        SubSearch(x);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0;
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }
        #endregion

        #region 마스터 그리드 클릭시 이벤트
        private void fpSpread2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;

                    //같은 Row 조회 되지 않게
                    if (intRow < 0)
                    {
                        return;
                    }

                    if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
                    {
                        return;
                    }

                    SubSearch(intRow);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region SubSearch()
        private void SubSearch(int Row)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    strMQuery = " usp_QEA001 'S2'";
                    strMQuery += ", @pINSP_REQ_NO ='" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text + "'";
                    strMQuery += ", @pWC_CD ='" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장코드")].Text + "'";
                    strMQuery += ", @pVER = '2' ";
                    strMQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f);
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 그리드 버튼 클릭 이벤트
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            SpreadClick(e.Row, e.Column);
        }

        private void SpreadClick(int Row, int Col)
        {
            if (Col == 1)
            {
                try
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "배정일자")].Text == "")
                    {
                        //						if(fpSpread1.Sheets[0].Cells[Row, 1].Text == "True")
                        //						{
                        //							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "배정수량")].Value = BalQty;
                        //						}
                        //						else
                        //						{
                        //							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "배정수량")].Value = 0;
                        //						}
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "배정수량 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 그리드 체인지 이벤트
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원명")].Text
                    = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE"
                    , fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text
                    , " And PLANT_CD = '" + SystemBase.Base.gstrPLANT_CD.ToString() + "'  AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text != ""
                    && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원명")].Text != "")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비필요여부")].Value = 1;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비필요여부")].Locked = true;
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비필요여부")].Locked = false;
                }
            }
        }
        #endregion

        #region fpSpread2_EditChange
        private void fpSpread2_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            UIForm.FPMake.fpChange(fpSpread2, e.Row);
        }
        #endregion

        #region fpSpread2_ChangeEvent Ctrl+V관련
        protected virtual void fpSpread2_ChangeEvent(int Row, int Col) { }
        private void fpSpread2_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            fpSpread1_ChangeEvent(e.Row, e.Column);
        }

        private void fpSpread2_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    fpSpread2.Sheets[0].ClipboardCopy();
                }

                if (e.Control && e.KeyCode == Keys.V)
                {
                    fpSpread2.Sheets[0].ClipboardPaste(ClipboardPasteOptions.Values);

                    // 복사된 행의 열을 구하기 위하여 클립보드 사용.

                    IDataObject iData = Clipboard.GetDataObject();

                    string strClp = (string)iData.GetData(DataFormats.Text);

                    if (strClp != "" || strClp != null || strClp.Length > 0)
                    {
                        Regex rx1 = new Regex("\r\n");
                        string[] arrData = rx1.Split(strClp.ToString());

                        int DataCount = arrData.Length - 1;

                        if (DataCount > 0)
                        {
                            int STRow = fpSpread2.ActiveSheet.ActiveRowIndex;
                            if (STRow < 0)
                                STRow = 0;

                            int ClipRowCount = STRow + DataCount;
                            if (fpSpread2.Sheets[0].RowCount < DataCount)
                                ClipRowCount = fpSpread2.Sheets[0].RowCount - STRow;

                            for (int i = STRow; i < ClipRowCount; i++)
                            {
                                if (i < fpSpread2.Sheets[0].RowCount
                                    || fpSpread2.Sheets[0].Cells[i, fpSpread2.ActiveSheet.ActiveColumnIndex].Locked != true)
                                {
                                    if (fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text != "I")
                                        fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "U";

                                    fpSpread2_ChangeEvent(i, fpSpread1.ActiveSheet.ActiveColumnIndex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "Clipboard 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 그리드 헤드 체크박스 클릭시
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                SpreadClick(i, e.Column);
            }
        }
        #endregion

        #region 저장
        private void btnAutoQty_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //detail 작업배정 저장
                //행수만큼 처리
                for (int j = 0; j < fpSpread2.Sheets[0].Rows.Count; j++)
                {
                    if (fpSpread2.Sheets[0].Cells[j, 1].Text == "True") //체크된것만
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True") //체크된것만
                            {
                                if ((Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배정수량")].Value)
                                    <= Convert.ToInt32(fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "요청수량")].Value)))
                                {
                                    string strSql = "";
                                    strSql += " usp_QEA001 'U1'";
                                    strSql += ", @pINSP_REQ_NO = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text + "'";
                                    strSql += ", @pINSP_CLASS_CD = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "검사구분코드")].Text + "'";
                                    strSql += ", @pREF_NO = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "참조번호")].Text + "'";
                                    strSql += ", @pREF_SEQ = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "참조순번")].Text + "'";
                                    strSql += ", @pH_RES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자")].Text + "'";
                                    strSql += ", @pSHEET_QTY = '" + Convert.ToInt32(fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "요청수량")].Value) + "'";
                                    strSql += ", @pWC_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text + "'";
                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pSHEET_DT ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배정일자")].Text + "'";
                                    strSql += ", @pVER = '2' ";
                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                                }
                                else
                                {
                                    MSGCode = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자")].Text + " 의 배정수량이 요청수량보다 클수 없습니다. ";
                                    ERRCode = "WR";
                                    Trans.Rollback();
                                    goto Exit;
                                }
                            }
                        }
                    }
                }
                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                Key = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text;

                SaveRow = fpSpread2.Sheets[0].ActiveRowIndex;

                Search(Key);
            }
            else if (ERRCode == "ER")
            { MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else
            { MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 취소
        private void btnClearQty_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //detail 작업배정 저장
                //행수만큼 처리
                for (int j = 0; j < fpSpread2.Sheets[0].Rows.Count; j++)
                {
                    if (fpSpread2.Sheets[0].Cells[j, 1].Text == "True") //체크된것만
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, 1].Text == "False" &&
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배정일자")].Text != "") //체크 안되고 배정일자 있는 것만
                            {
                                string strSql = "";
                                strSql += " usp_QEA001 'D1'";
                                strSql += ", @pINSP_REQ_NO = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text + "'";
                                strSql += ", @pH_RES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자")].Text + "'";
                                strSql += ", @pSHEET_DT ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배정일자")].Text + "'";
                                strSql += ", @pVER = '2' ";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
                    }
                }
                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                Key = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text;

                SaveRow = fpSpread2.Sheets[0].ActiveRowIndex;

                Search(Key);
            }
            else if (ERRCode == "ER")
            { MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else
            { MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 마감취소
        private void btnCloseCancel_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //detail 작업배정 저장
                //행수만큼 처리
                for (int j = 0; j < fpSpread2.Sheets[0].Rows.Count; j++)
                {
                    if (fpSpread2.Sheets[0].Cells[j, 1].Text == "True") //체크된것만
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, 1].Text == "False" &&
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배정일자")].Text != "") //체크 안되고 배정일자 있는 것만
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "마감여부")].Text == "Y")
                                {
                                    string strSql = "";
                                    strSql += " usp_QEA001 'D2'";
                                    strSql += ", @pINSP_REQ_NO = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text + "'";
                                    strSql += ", @pH_RES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자")].Text + "'";
                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pSHEET_DT ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배정일자")].Text + "'";
                                    strSql += ", @pVER = '2' ";
                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                                }
                                else
                                {
                                    MSGCode = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자")].Text + " 의 작업배정상태가 마감상태가 아니므로 취소할 수 없습니다. ";
                                    ERRCode = "WR";
                                    Trans.Rollback();
                                    goto Exit;
                                }
                            }
                        }
                    }
                }
                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                Key = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text;

                SaveRow = fpSpread2.Sheets[0].ActiveRowIndex;

                Search(Key);
            }
            else if (ERRCode == "ER")
            { MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else
            { MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            this.Cursor = Cursors.Default;
        }
        #endregion

        //2014-09-30 UIForm 기능 추가
        //#region fpSpread2_CellClick
        //private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        //{
        //    if (fpSpread2.Sheets[0].Rows.Count > 0)
        //    {
        //        if (fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
        //        {
        //            if (e.ColumnHeader == true)
        //            {
        //                if (fpSpread2.Sheets[0].ColumnHeader.Cells[0, e.Column].Text == "True")
        //                {
        //                    fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = false;
        //                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
        //                    {
        //                        if (fpSpread2.Sheets[0].Cells[i, e.Column].Locked == false)
        //                        {
        //                            fpSpread2.Sheets[0].Cells[i, e.Column].Value = false;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = true;
        //                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
        //                    {
        //                        if (fpSpread2.Sheets[0].Cells[i, e.Column].Locked == false)
        //                        {
        //                            fpSpread2.Sheets[0].Cells[i, e.Column].Value = true;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //#endregion

    }
}
