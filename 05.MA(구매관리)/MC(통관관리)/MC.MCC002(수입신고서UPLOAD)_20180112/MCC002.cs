#region 작성정보
/*********************************************************************/
// 단위업무명 :통관현황조회
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-17
// 작성내용 : 통관현황조회
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
using WNDW;
using System.Data.OleDb;

namespace MC.MCC002
{
    public partial class MCC002 : UIForm.FPCOMM1
    {
        #region 생성자
        public MCC002()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void MCC002_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "운송형태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D019', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//운송형태
            
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            dtpLiceDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpLiceDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            //기타 세팅	
            dtpLiceDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpLiceDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string strQuery = "usp_MCC002 @pTYPE = 'S1'";
                    strQuery += ", @pIMPO_LICE = '" + txtImpoLice.Text + "'";
                    strQuery += ", @pBLXX_NUMB = '" + txtBlxxNumb.Text + "'";
                    strQuery += ", @pLICE_DATE_FR = '" + dtpLiceDtFr.Text + "'";
                    strQuery += ", @pLICE_DATE_TO = '" + dtpLiceDtTo.Text + "'";
                    strQuery += ", @pCLMN_NUMB = '" + txtClmnNumb.Text + "'";
                    strQuery += ", @pSPEC_NUMB = '" + txtSpecNumb.Text + "'";
                    strQuery += ", @pSPEC_NAME = '" + txtSpecName.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                    strQuery += ", @pBL_NO = '" + txtBlNo.Text +"' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true)
            {
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
                                case "I": strGbn = "I1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            string strQuery = " usp_MCC002 @pType = '" + strGbn + "', @pCO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ";

                            strQuery += ", @pBL_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "송장번호")].Text + "'";
                            strQuery += ", @pBL_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Value + "'";
                            strQuery += ", @pIMPO_LICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고번호")].Text + "'";
                            strQuery += ", @pBLXX_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L NO")].Text + "'";
                            strQuery += ", @pCUST_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "운송자명")].Text + "'";
                            strQuery += ", @pLICE_DATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고일")].Text + "'";
                            strQuery += ", @pTKIN_DATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반입일")].Text + "'";
                            strQuery += ", @pTRNS_DIVS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "운송형태")].Value + "'";
                            strQuery += ", @pSUEN_NACD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "적출국")].Text + "'";
                            strQuery += ", @pFOBX_DIVS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "인도조건")].Text + "'";
                            strQuery += ", @pSANC_AMNT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결제금액")].Value + "'";
                            strQuery += ", @pSANC_PRRT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value + "'";
                            strQuery += ", @pSANC_DIVS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결제방법")].Text + "'";
                            strQuery += ", @pCLMN_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "란번호")].Text + "'";
                            strQuery += ", @pSPEC_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격일련번호")].Text + "'";
                            strQuery += ", @pSPEC_NAME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "모델규격명")].Text + "'";
                            strQuery += ", @pLICE_QNTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목수량")].Value + "'";
                            strQuery += ", @pHSXX_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세번부호(HS코드)")].Text + "'";
                            strQuery += ", @pCUST_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세종=관세율(%)")].Value + "'";
                            strQuery += ", @pGAMX_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세종=관감면율(%)")].Value + "'";
                            strQuery += ", @pAGRI_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "농특세율(%)")].Value + "'";

                            strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
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
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                    ERRCode = "ER";
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 엑셀
        private void btnUpLoad_Click(object sender, System.EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "통합 Excel 문서(*.xls)|*.xls|2007 Excel 문서(*.xlsx)|*.xlsx";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string connectionString = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;Imex=1;hdr=yes;""", dlg.FileName);
                    OleDbConnection conn = new OleDbConnection(connectionString);
                    conn.Open();

                    DataTable worksheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    string commandString = String.Format("SELECT * FROM [수입신고서$]", worksheets.Rows[0]["TABLE_NAME"]);
                    OleDbCommand cmd = new OleDbCommand(commandString, conn);

                    OleDbDataAdapter dapt = new OleDbDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    dapt.Fill(ds);
                    conn.Close();

                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            UIForm.FPMake.RowInsert(fpSpread1);

                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "송장번호")].Text = ds.Tables[0].Rows[i][0].ToString().Trim();								//0.송장번호
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Value = Convert.ToInt32(ds.Tables[0].Rows[i][1].ToString().Trim());					//1.순번
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고번호")].Text = ds.Tables[0].Rows[i][2].ToString().Trim();								//2.신고번호
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L NO")].Text = ds.Tables[0].Rows[i][3].ToString().Trim();									//3.B/L NO
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "운송자명")].Text = ds.Tables[0].Rows[i][4].ToString().Trim();								//4.운송자명
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고일")].Text = ds.Tables[0].Rows[i][5].ToString().Trim();									//5.신고일
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반입일")].Text = ds.Tables[0].Rows[i][6].ToString().Trim();									//6.반입일
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "운송형태")].Value = ds.Tables[0].Rows[i][7].ToString().Trim();								//7.운송형태
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "적출국")].Text = ds.Tables[0].Rows[i][8].ToString().Trim();									//8.적출국
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "인도조건")].Text = ds.Tables[0].Rows[i][9].ToString().Trim();								//9.인도조건
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결제금액")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][10].ToString().Trim());			//10.결제금액
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][11].ToString().Trim());				//11.환율
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결제방법")].Text = ds.Tables[0].Rows[i][12].ToString().Trim();								//12.결제방법
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "란번호")].Text = ds.Tables[0].Rows[i][13].ToString().Trim();								//13.란번호
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격일련번호")].Text = ds.Tables[0].Rows[i][14].ToString().Trim();							//14.규격일련번호
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "모델규격명")].Text = ds.Tables[0].Rows[i][15].ToString().Trim();							//15.모델규격명
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목수량")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][16].ToString().Trim());			//16.품목수량
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세번부호(HS코드)")].Text = ds.Tables[0].Rows[i][17].ToString().Trim();						//17.세번부호(HS코드)
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세종=관세율(%)")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][18].ToString().Trim());		//18.세종=관세율(%)
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세종=관감면율(%)")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][19].ToString().Trim());	//19.세종=관감면율(%)")
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "농특세율(%)")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][20].ToString().Trim());		//20.농특세율(%)
                        }
                    }
                }
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void btnDnLoad_Click(object sender, System.EventArgs e)
        {
            string updndl = "";

            if (SystemBase.Base.gstrUserID == "ADMIN") updndl = "Y#Y#Y";
            else updndl = "N#Y#N";

            UIForm.FileUpDown form1 = new UIForm.FileUpDown(this.Name, updndl);
            form1.ShowDialog();
        }

        #endregion
    }
}
