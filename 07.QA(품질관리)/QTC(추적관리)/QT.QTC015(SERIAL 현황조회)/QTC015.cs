#region 작성정보
/*********************************************************************/
// 단위업무명 : SERIAL 관리품목 조회
// 작 성 자   : 김 한 진
// 작 성 일   : 2014-10-28
// 작성내용   : SERIAL 관리품목 조회
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
namespace QT.QTC015
{
    public partial class QTC015 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strAutoSoNo = "";
        #endregion

        #region 생성자
        public QTC015()
        {
            InitializeComponent();

        }
        public QTC015(string So_No)
        {
            // 알리미 클릭시- 알리미
            strAutoSoNo = So_No;
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void QTC015_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            // 콤보박스 세팅

            //기타 세팅
            dtpInspDtFr.Text = null;
            dtpInspDtTo.Text = null;

            //그리드 세팅
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpInspDtFr.Text = null;
            dtpInspDtTo.Text = null;
            //dtpInspDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString().Substring(0, 10);
            //dtpInspDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 10);

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
                    string strOldItem_CD = "";
                    string strNewItem_CD = "";
                    int intRow = 0;
                    int ColCnt = 0;

					string strQuery = "usp_QTC015 @pTYPE = 'S1'";
                    strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "'";
					strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery += ", @pINSPECTOR_CD = '" + txtInspectorCd.Text + "'"; //검사원
					strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strQuery += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";    //검사의뢰번호
                    strQuery += ", @pWORKORDER_NO = '" + txtWorkOrderNo.Text + "'"; //제조오더번호
                    strQuery += ", @pINSP_DT_FR ='" + dtpInspDtFr.Text + "'";       //검사FR
                    strQuery += ", @pINSP_DT_TO ='" + dtpInspDtTo.Text + "'";       //검사TO
                    strQuery += ", @pENT_CD = '" + txtEntCd.Text + "'";


                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        fpSpread1.Sheets[0].RowCount = 0;

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            
                            if (i == 0)
                            {
                                UIForm.FPMake.RowInsert(fpSpread1);
                                intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = ds.Tables[0].Rows[i][0].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "수주품목")].Text = ds.Tables[0].Rows[i][1].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "레벨")].Text = ds.Tables[0].Rows[i][2].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = ds.Tables[0].Rows[i][3].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = ds.Tables[0].Rows[i][4].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = ds.Tables[0].Rows[i][5].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot추적")].Text = ds.Tables[0].Rows[i][6].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "Serial추적")].Text = ds.Tables[0].Rows[i][7].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = ds.Tables[0].Rows[i][8].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "소요량")].Text = ds.Tables[0].Rows[i][9].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "총소요량")].Text = ds.Tables[0].Rows[i][10].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text = ds.Tables[0].Rows[i][11].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "S/N 1")].Text = ds.Tables[0].Rows[i][12].ToString();

                                strOldItem_CD = ds.Tables[0].Rows[i][3].ToString();
                                strNewItem_CD = ds.Tables[0].Rows[i + 1][3].ToString();
                                if (strOldItem_CD != strNewItem_CD)
                                {
                                    continue;
                                }
                                else
                                {
                                    ColCnt++;
                                    i++;
                                }
                                    
                            }
                            if (strOldItem_CD != strNewItem_CD)
                            {                             
                                ColCnt = 0;
                                UIForm.FPMake.RowInsert(fpSpread1);
                                intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = ds.Tables[0].Rows[i][0].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "수주품목")].Text = ds.Tables[0].Rows[i][1].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "레벨")].Text = ds.Tables[0].Rows[i][2].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = ds.Tables[0].Rows[i][3].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = ds.Tables[0].Rows[i][4].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = ds.Tables[0].Rows[i][5].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot추적")].Text = ds.Tables[0].Rows[i][6].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "Serial추적")].Text = ds.Tables[0].Rows[i][7].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = ds.Tables[0].Rows[i][8].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "소요량")].Text = ds.Tables[0].Rows[i][9].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "총소요량")].Text = ds.Tables[0].Rows[i][10].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text = ds.Tables[0].Rows[i][11].ToString();
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "S/N 1")].Text = ds.Tables[0].Rows[i][12].ToString();

                            }
                            else
                            {              
                                ColCnt++;                       
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "S/N 1") + ColCnt].Text = ds.Tables[0].Rows[i][12].ToString();


                            }

                            if (i + 1 < ds.Tables[0].Rows.Count)
                            {
                                strOldItem_CD = ds.Tables[0].Rows[i][3].ToString();
                                strNewItem_CD = ds.Tables[0].Rows[i+1][3].ToString();
                            }
                        }
                        for (int z = 0; z < fpSpread1.Sheets[0].RowCount; z++)
                        {
                            fpSpread1.Sheets[0].RowHeader.Cells[z, 0].Text = "";
                            fpSpread1.Sheets[0].RowHeader.Rows[z].BackColor = SystemBase.Base.Color_Org;

                            for (int j = 0; j < fpSpread1.Sheets[0].ColumnCount; j++)
                                fpSpread1.Sheets[0].Cells[z, j].Locked = true;
                        }
                        fpSpread1.Sheets[0].SetActiveCell(0, 1);
                        fpSpread1.ActiveSheet.AddSelection(0, 1, 1, 1);
                        fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
 
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

        //사업코드
        private void btnEntCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP', @pSPEC1='ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEntCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업코드 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEntCd.Text = Msgs[0].ToString();
                    txtEntNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
            }

        }

        private void btnProjectSeqFr_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

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
        #endregion

        #region 조회조건 TextChanged
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

        //사업코드
        private void txtEntCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtEntCd.Text != "")
                {
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtEntNm.Value = "";
                }
            }
            catch { }
        }
        #endregion


    }
}
