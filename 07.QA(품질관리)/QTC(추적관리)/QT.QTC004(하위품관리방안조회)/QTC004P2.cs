#region 작성정보
/*********************************************************************/
// 단위업무명 : 전표조회
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-25
// 작성내용 : 전표조회
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

#region 예제 - 복사해서 쓰세요
/*
try
{
    WNDW.QTC004P2 pu = new WNDW.QTC004P2(txtSLIP_NO.Text);
    pu.ShowDialog();
}
catch (Exception f)
{
    SystemBase.Loggers.Log(this.Name, f.ToString());
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전표정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace QT.QTC004
{
    /// <summary>
    /// 전표정보 조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// </summary>
    public partial class QTC004P2 : UIForm.FPCOMM2
    {
        #region 변수선언

        string[] returnVal = null;

        string strPlantCd = "";
        string strProjectNo = "";
        string strProjectSeq = "";
        string strItemCd = "";
        string strUnit = "";
        string strQty = "";
        string strItemAcct = "";

        #endregion

        public QTC004P2()
        {
            InitializeComponent();
        }

        public QTC004P2(string sPlantCd, string sProjectNo, string sProjectSeq, string sItemCd, string sItemAcct, string sUnit, string sQty)
        {
            strPlantCd = sPlantCd;
            strProjectNo = sProjectNo;
            strProjectSeq = sProjectSeq;
            strItemCd = sItemCd;
            strUnit = sUnit;
            strQty = sQty;
            strItemAcct = sItemAcct;

            InitializeComponent();
        }

        #region Form Load 시
        private void QTC004P2_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            NewExec();

            this.Text = "입고번호조회";

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pTYPE = 'PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); //공장

            dtpRcptDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-12).ToShortDateString().Substring(0, 10);
            dtpRcptDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 10);

            cboPlantCd.SelectedValue = strPlantCd;
            txtProjectNo.Value = strProjectNo;
            txtProjectSeq.Value = strProjectSeq;
            txtItemCd.Value = strItemCd;
            txtQty.Value = strQty;
            txtUnit.Value = strUnit;
            txtItemAcct.Value = strItemAcct;

            if (txtItemAcct.Text == "10" || txtItemAcct.Text == "20")
            {
                c1Label1.Text = "실적일자";
            }
            else
            {
                c1Label1.Text = "입고일자";
            }

            SearchExec_INOUT();
            SearchExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox1);
        }
        #endregion

        #region 입출고이력조회
        private void SearchExec_INOUT()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_QTC004_NEW  'P3'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pPLANT_CD = '" + strPlantCd + "' ";
                    strQuery += ", @pCHILD_ITEM_CD = '" + txtItemCd.Text.ToString() + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text.ToString() + "' ";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text.ToString() + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

                    //PreRow = -1;
                    //UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직 입고이력
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                //상세조회 SQL
                string strType = "";

                if (txtItemAcct.Text == "10" || txtItemAcct.Text == "20")
                {
                    strType = "P5";
                }
                else
                {
                    strType = "P4";
                }

                string strQuery = " usp_QTC004_NEW  '" + strType + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pPLANT_CD = '" + strPlantCd + "' ";
                strQuery += ", @pCHILD_ITEM_CD = '" + txtItemCd.Text.ToString() + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text.ToString() + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text.ToString() + "' ";
                strQuery += ", @pRCPT_DT_FT = '" + dtpRcptDtFr.Text + "' ";
                strQuery += ", @pRCPT_DT_TO = '" + dtpRcptDtTo.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                if (txtItemAcct.Text == "10" || txtItemAcct.Text == "20")
                {
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Text = "입고일자";
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Text = "제조오더번호";
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Text = "실적순번";
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].Text = "실적수량";
                } 

            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;

        }
        #endregion

        #region TextChanged
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    txtItemSpec.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_SPEC", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                    txtItemSpec.Value = "";
                }
            }
            catch
            {

            }
        }
        //프로젝트번호
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

            if (txtProjectNm.Text == "")
            {
                txtProjectSeq.Value = "";
            }
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
        #endregion

        #region ButtonClicked 첨부문서 팝업
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Row >= 0)
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "첨부문서_2"))
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
                    fpSpread1.Sheets[0].RowHeader.Rows[e.Row].BackColor = SystemBase.Base.Color_Org;

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "첨부문서")].Text != "")
                    {
                        string strProjectNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                        string strProjectSeq = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
                        string strLotNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "LOT NO")].Text;
                        string strItemCd = txtItemCd.Text;
                        string strUnit = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;
                        string InspQty = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Text;
                        string strMvmtNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text;
                        string strMvmtSeq = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text;
                        string strItemAcct = txtItemAcct.Text;

                        QTC004P1 frm1 = new QTC004P1(cboPlantCd.SelectedValue.ToString(), strProjectNo, strProjectSeq, strLotNo, strItemCd, strItemAcct, strUnit, InspQty, strMvmtNo, strMvmtSeq);
                        frm1.ShowDialog();
                    }
                }
            }
        }
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
    }
}
