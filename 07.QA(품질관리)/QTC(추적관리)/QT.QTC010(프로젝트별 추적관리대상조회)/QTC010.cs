#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트별 추적관리대상조회
// 작 성 자 : 김 한 진
// 작 성 일 : 2014-10-08
// 작성내용 : 프로젝트별 추적관리대상조회
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion


using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Extensions.C1ComboExtension;
using EDocument.Network;
using EDocument.Spread;
using WNDW;

namespace QT.QTC010
{
    public partial class QTC010 : UIForm.FPCOMM1
    {
        #region 필드
        const int defaultColWidth = 115;

        Dictionary<string, string> docColumns = null;
        /// <summary>문서코드별 문서번호 유무</summary>
        Dictionary<string, string> docNoReqs = null;
        /// <summary>첨부문서표시 관리자</summary>
        AttachmentManager attachmentManager;
        #endregion


        #region 생성자
        public QTC010()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void QTC010_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용

            SystemBase.ComboMake.C1Combo(cboSPlant, "usp_B_COMMON @pTYPE = 'PLANT' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");	//공장
            cboSPlant.SelectedValue = SystemBase.Base.gstrPLANT_CD;

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단품구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B029', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //단품구분
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "최종검사")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q013', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //최종검사

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            docNoReqs = SystemBase.Base.CreateDictionary("usp_T_DOC_CODE @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"); // 문서번호 필수인 문서종류

            //docNoReqs = SystemBase.Base.CreateDictionary("usp_T_DOC_CODE @pTYPE = 'S1',@pDOC_CTG_CD = 'SOD', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"); // 문서번호 필수인 문서종류
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            SystemBase.ComboMake.C1Combo(cboSPlant, "usp_B_COMMON @pTYPE = 'PLANT' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");	//공장

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            string strLotYN = string.Empty;
            string strSerYN = string.Empty;

            try
            {

                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_QTC010  'S1'";
                    strQuery += ", @pPLANT_CD = '" + cboSPlant.SelectedValue.ToString() + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pORDERITEM_CD = '" + txtOrderitem_Cd.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ_FR = '" + txtProjectSeqFr.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ_TO = '" + txtProjectSeqTo.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                    //char[] delimiterChars = { ',', ':',' ' };
                    //int m_Req_DOC = 0;
                    //int m_Req_INFO_DOC = 0;

                    fpSpread1.ActiveSheet.Columns[4].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                    fpSpread1.ActiveSheet.Columns[2].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                    fpSpread1.ActiveSheet.Columns[3].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                    

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주품목")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주품목명")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "첨부문서")].Text != "")
                        {
                            //string CharInSplit = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "필수첨부문서")].Text;
                            //string CharSPlit = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "첨부문서")].Text;
                            //string[] REQ_Split = CharSPlit.Split(delimiterChars);
                            
                            ////필수 체크
                            //for (int k = 1; k < REQ_Split.Length; k += 4)
                            //{
                            //    if (CharInSplit == REQ_Split[k].ToString())
                            //    {
                            //        m_Req_INFO_DOC = Convert.ToInt16(REQ_Split[k+1].ToString());
                            //    }
                            //}
                            ////아닌거체크
                            //for (int j = 2; j < REQ_Split.Length; j += 4)
                            //{
                            //    m_Req_DOC += Convert.ToInt16(REQ_Split[j].ToString());
                            //}
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "증빙")].Locked = false;
                        }
                        //fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "필수첨부문서")].Text = m_Req_INFO_DOC.ToString();
                        //if (m_Req_DOC == 0)
                        //    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "첨부문서")].Text = "";
                        //else
                        //     fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "첨부문서")].Text = m_Req_DOC.ToString();
                        //m_Req_DOC = 0;
                    }
                        // fpSpread1.ActiveSheet.Lock(true, true); // 편집 잠금

                    // 품질증빙 문서구분에 따른 칼럼 속성 설정
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region fpSpread1_CellClick
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        if (fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
                        {
                            if (e.ColumnHeader == true && e.Column == 5)
                            {
                                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                                {
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text != "True")
                                    {
                                        fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Org;
                                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Serial 추적")].Text = "false";
                                    }
                                    else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
                                    {
                                        fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Update;
                                    }
                                }
                            }
                            else if (e.ColumnHeader == true && e.Column == 6)
                            {

                                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                                {
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Serial 추적")].Text != "True")
                                    {
                                        fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Org;
                                    }
                                    else
                                    {
                                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text = "True";
                                        fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Update;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 조회 조건 팝업
        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();

                    txtProjectNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목코드
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(Convert.ToString(cboSPlant.SelectedValue), true, txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
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

        //수주품목코드
        private void c1Button1_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(Convert.ToString(cboSPlant.SelectedValue), true, txtOrderitem_Cd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtOrderitem_Cd.Text = Msgs[2].ToString();
                    txtOrderitem_Nm.Value = Msgs[3].ToString();

                    txtOrderitem_Cd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 텍스트체인지
        private void txtOrderitem_Cd_TextChanged(object sender, EventArgs e)
        {
            if (txtOrderitem_Cd.Text != "") txtOrderitem_Nm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtOrderitem_Cd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            else txtOrderitem_Nm.Value = "";
        }
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            if (txtItemCd.Text != "") txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            else txtItemNm.Value = "";
        }
        //프로젝트 코드
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
        /// <summary>
        /// 프로젝트차수 FR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    txtProjectSeqFr.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        /// <summary>
        /// 프로젝트 차수 TO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProjectSeqTo_Click(object sender, EventArgs e)
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
                    txtProjectSeqTo.Text = Msgs[0].ToString();
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

        #region 그리드 상단 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                if (Column == fpSpread1.Sheets[0].FindHeaderColumnIndex("증빙"))
                {
                    WNDW.WNDW039 pu = new WNDW.WNDW039();
                    pu.strKEY_NO = txtProjectNo.Text;
                    pu.strKEY_SEQ = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text.ToString();
                    pu.strITEM_CD = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.ToString();
                    pu.strREQ_TYPE = "PO";
                    pu.strDOC_TYPE = "PUR";
                    pu.strFormGubn = "MIM001";

                    pu.ShowDialog();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}