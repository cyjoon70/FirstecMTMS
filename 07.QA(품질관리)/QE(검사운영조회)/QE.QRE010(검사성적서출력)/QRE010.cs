#region 작성정보
/*********************************************************************/
// 단위업무명 : 검사성적서출력
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-22
// 작성내용 : 검사성적서출력 및 관리
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
using WNDW;
using System.Threading;
using System.IO;

namespace QE.QRE010
{
    public partial class QRE010 : UIForm.Buttons
    {
        #region 변수선언
        Thread th;
        UIForm.ExcelWaiting Waiting_Form = null;
        #endregion 

        #region 생성자
        public QRE010()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void QRE010_Load(object sender, System.EventArgs e)
        { 
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboInspClassCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q001', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사분류코드

            dtpInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            dtpInspDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 10);
            dtpInspDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            rdoA.Checked = true;
            c1Label12.Hide();
            panel3.Hide();
            rdoA.Hide();
            rdoB.Hide();

        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //필수체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            dtpInspDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 10);
            dtpInspDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            rdoA.Checked = true;
        }
        #endregion

        #region 조회조건팝업
        //검사의뢰번호 FROM
        private void btnInspReqNoFr_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strQuery = " usp_Q_COMMON 'Q080', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtInspReqNoFr.Text, "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P06012", strQuery, strWhere, strSearch, new int[] { 0 }, "검사의뢰번호 조회", false);
                pu.Width = 700;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtInspReqNoFr.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }

        //검사의뢰번호 TO
        private void btnInspReqNoTo_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strQuery = " usp_Q_COMMON 'Q080', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtInspReqNoTo.Text, "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P06012", strQuery, strWhere, strSearch, new int[] { 0 }, "검사의뢰번호 조회", false);
                pu.Width = 700;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtInspReqNoTo.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }

        //프로젝트번호 FROM
        private void btnProjectNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtEntCd.Value = Msgs[1].ToString();
                    txtEntNm.Value = Msgs[2].ToString();
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

        //품목코드
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(Convert.ToString(cboPlantCd.SelectedValue), true, txtItemCd.Text);
                pu.MaximizeBox = false;
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
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
                txtEntCd.Text = SystemBase.Base.CodeName("PROJECT_NO", "ENT_CD", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                if (txtEntCd.Text != "")
                {
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtEntNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 검사성적서 출력
        private void butPreview_Click(object sender, System.EventArgs e)
        {
            string strInspReqNo = "";
            string strInspItemCd = "";
            string strUnityInspReqNo = "";

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

            if (Convert.ToString(cboInspClassCd.SelectedValue) == "F")
                strGbn = "R2";
            else if (Convert.ToString(cboInspClassCd.SelectedValue) == "R")
                strGbn = "R4";
            else
                strGbn = "R1";

            string strQuery = " usp_QRE010  @pTYPE = '" + strGbn + "'";
            strQuery += ", @pINSP_CLASS_CD = '" + Convert.ToString(cboInspClassCd.SelectedValue) + "'";
            strQuery += ", @pPLANT_CD = '" + Convert.ToString(cboPlantCd.SelectedValue) + "'";
            strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
            strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
            strQuery += ", @pINSP_REQ_NO_FR = '" + txtInspReqNoFr.Text + "'";
            strQuery += ", @pINSP_REQ_NO_TO = '" + txtInspReqNoTo.Text + "'";
            strQuery += ", @pINSP_REQ_DT_FR = '" + dtpInspReqDtFr.Text + "'";
            strQuery += ", @pINSP_REQ_DT_TO = '" + dtpInspReqDtTo.Text + "'";
            strQuery += ", @pINSP_DT_FR = '" + dtpInspDtFr.Text + "'";
            strQuery += ", @pINSP_DT_TO = '" + dtpInspDtTo.Text + "'";
            strQuery += ", @pPRINT_TYPE = '" + strPrint_Type + "'";
            strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);


            if (dt.Rows.Count > 0)
            {

                try
                {
                    //th = new Thread(new ThreadStart(Show_Waiting));               // 2015.05.06. 주석 처리
                    //th.Start();              
                    //Thread.Sleep(200);
                    //Waiting_Form.Activate();

                    //Waiting_Form.progressBar_temp.Maximum = dt.Rows.Count;        // 2015.05.06. 주석 처리 

                    this.Cursor = Cursors.WaitCursor;       // 2015.05.12. hma 추가

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
                                    excel.SetCell(iStart - 37, 24, dt.Rows[i-1]["TPAGE"].ToString() + " 매중 " + iPage.ToString() + " 매");

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

                            // 2015.05.06. hma 추가(Start): 검사책임자를 넘겨받아서 출력하도록 함.
                            excel.SetCell(iStart - 21, 15, dt.Rows[i]["QC_MAN_NAME"].ToString());
                            // 2015.05.06. hma 추가(End)


                            if (dt.Rows[i]["INSPECTOR_NM"].ToString() != "")
                                excel.SetCell(iStart - 19, 15, dt.Rows[i]["INSPECTOR_NM"].ToString());

                            strInspReqNo = dt.Rows[i]["INSP_REQ_NO"].ToString();
                            strUnityInspReqNo = dt.Rows[i]["UNITY_INSP_REQ_NO"].ToString();

                            iPage = 1;

                        }

                        //내용입력
                        if (strInspItemCd != dt.Rows[i]["INSP_ITEM_CD"].ToString() || bHeard == true)
                        {
                            if (rdoA.Checked == true)
                            {
                                if (iListRow <= 2)//리스트페이지를 불러와야할때
                                {
                                    
                                    excel.SetSelect("A" + iStart, "A" + iStart);
                                    excel.RunMacro("PageListAdd");
                                    iStart += 23;
                                    

                                    iPage++;
                                    iListRow = 20;
                                    excel.SetCell(iStart -  iListRow-3, 24, dt.Rows[i]["TPAGE"].ToString() + " 매중 " + iPage.ToString() + " 매");
                                    excel.SetCell(iStart - iListRow-3, 3, dt.Rows[i]["PROJECT_NM"].ToString());
                                    excel.SetCell(iStart - iListRow-3, 7, dt.Rows[i]["ITEM_NM"].ToString());
                                    excel.SetCell(iStart - iListRow-3, 15, dt.Rows[i]["ITEM_CD"].ToString());

                                }
                                else
                                {
                                    iListRow -= 2;
                                }

                                excel.SetCell(iStart - iListRow, 1, dt.Rows[i]["INSP_SEQ"].ToString());
                                excel.SetCell(iStart - iListRow, 2, dt.Rows[i]["INSP_ITEM_NM"].ToString());
                                excel.SetCell(iStart - iListRow+1, 2, dt.Rows[i]["MAP_COOR"].ToString());
                                excel.SetCell(iStart - iListRow, 3, dt.Rows[i]["INSP_SPEC"].ToString().Replace("\r\n", "\n"));
                                excel.SetCell(iStart - iListRow, 4, dt.Rows[i]["MEASURE_NM"].ToString());
                                excel.SetCell(iStart - iListRow, 26, dt.Rows[i]["AQL"].ToString());
                            }
                            strInspItemCd = dt.Rows[i]["INSP_ITEM_CD"].ToString();
                            iCel = 0;
                        }
                        else
                        {
                            if (rdoA.Checked == true)
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
                                        excel.SetCell(iStart - iListRow-3, 24, dt.Rows[i]["TPAGE"].ToString() + " 매중 " + iPage.ToString() + " 매");
                                        excel.SetCell(iStart - iListRow-3, 3, dt.Rows[i]["PROJECT_NM"].ToString());
                                        excel.SetCell(iStart - iListRow-3, 7, dt.Rows[i]["ITEM_NM"].ToString());
                                        excel.SetCell(iStart - iListRow-3, 15, dt.Rows[i]["ITEM_CD"].ToString());
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
                        }

                        if (rdoA.Checked == true)
                        {
                            if (dt.Rows[i]["VALUE"].ToString() != "")
                            {
                                excel.SetCell(iStart - iListRow, iAddCol[iCel], dt.Rows[i]["VALUE"].ToString());
                            }
                        }
                        // Waiting_Form.progressBar_temp.Value = i + 1;      // 2015.05.06. 주석 처리
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
                    //Waiting_Form.label_temp.Text = "완료되었습니다.";        // 2015.05.06. 주석 처리 
                    //Thread.Sleep(500);
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
                    // Waiting_Form.Close();        // 2015.05.06. 주석 처리
                   // th.Abort();
                    File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
                }
                this.Cursor = Cursors.Default;       // 2015.05.12. hma 추가

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
