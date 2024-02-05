#region 작성정보
/*********************************************************************/
// 단위업무명 : 계획대비실적등록
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-09
// 작성내용 : 계획대비실적등록
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
using System.Globalization;
using FarPoint.Win;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;

namespace XA.XAA005
{
    public partial class XAA005 : UIForm.FPCOMM1
    {
        public XAA005()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void XAA005_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            dtpYearMon.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0,7);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            //필수체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpYearMon.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            string msg = SystemBase.Base.MessageRtn("P0008");
            DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_XAA005  'D1'";
                    strSql += ", @pPR_YM = '" + dtpYearMon.Text.Replace("-", "") + "' ";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                    //MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
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
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_XAA005  @pTYPE = 'S1'";
                strQuery += ", @pPR_YM = '" + dtpYearMon.Text.Replace("-", "") + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                /*  A1	매출
                        A11	방산
                        A12	시스템
                        B1	매출원가
                        B11	방산
                        B12	시스템
                        C1	매출총이익
                        D1	판관비
                        E1	영업이익
                        F1	영업외수익
                        G1	영업외비용
                        H1	경상이익
                        I1	특별이익
                        J1	특별손실
                        K1	법인세차감전순이익
                        L1	법인세비용
                        M1	당기순이익
                */

                string code = "";
                string Str1 = UIForm.FPMake.IntToString(3);
                string Str2 = UIForm.FPMake.IntToString(5);
                string Str3 = UIForm.FPMake.IntToString(8);
                string Str4 = UIForm.FPMake.IntToString(10);

                for(int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
				{
					code  = fpSpread1.Sheets[0].Cells[i, 1].Text;	
					if(code == "A1" || code == "B1" ||code == "C1"||code == "E1"||code == "H1" ||code == "K1" || code == "M1")
					{
						UIForm.FPMake.grdReMake(fpSpread1, i, "3|2");
					}					

					Cell r1 = fpSpread1.ActiveSheet.Cells[i , 7];  //차액
					Cell r2 = fpSpread1.ActiveSheet.Cells[i , 12]; //차액

					Cell r3 = fpSpread1.ActiveSheet.Cells[i , 4]; // %
					Cell r4 = fpSpread1.ActiveSheet.Cells[i , 6]; // %
					Cell r5 = fpSpread1.ActiveSheet.Cells[i , 9]; // %
					Cell r6 = fpSpread1.ActiveSheet.Cells[i , 11]; // %


					r1.Formula = Str2+(i+1)+"-"+Str1+(i+1); 
					r2.Formula = Str4+(i+1)+"-"+Str3+(i+1); 

					if(i == 4)
					{
						r3.Formula = Str1+(i+1)+"/"+Str1+"2*100" ; 
						r4.Formula = Str2+(i+1)+"/"+Str2+"2*100" ;  
						r5.Formula = Str3+(i+1)+"/"+Str3+"2*100" ; 
						r6.Formula = Str4+(i+1)+"/"+Str4+"2*100" ; 
					}
					else if (i == 5)
					{
						r3.Formula = Str1+(i+1)+"/"+Str1+"3*100" ; 
						r4.Formula = Str2+(i+1)+"/"+Str2+"3*100" ;  
						r5.Formula = Str3+(i+1)+"/"+Str3+"3*100" ; 
						r6.Formula = Str4+(i+1)+"/"+Str4+"3*100" ; 
					}
					else
					{
						r3.Formula = Str1+(i+1)+"/"+Str1+"1*100" ; 
						r4.Formula = Str2+(i+1)+"/"+Str2+"1*100" ;  
						r5.Formula = Str3+(i+1)+"/"+Str3+"1*100" ; 
						r6.Formula = Str4+(i+1)+"/"+Str4+"1*100" ; 
					}
				}
			}
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }

        #endregion

        #region SaveExec()
        protected override void SaveExec()
        {
            //그리드상단 필수 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
            {
                this.Cursor = Cursors.WaitCursor;

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
                                default: strGbn = ""; break;
                            }

                            // 그리드 상단 필수항목 체크
                            if (strGbn.Length > 0)
                            {
                                string strSql = " usp_XAA005 '" + strGbn + "'";
                                strSql += ", @pPR_YM= '" + dtpYearMon.Text.Replace("-", "") + "' ";
                                strSql += ", @pACCT_CD3  = '" + fpSpread1.Sheets[0].Cells[i, 1].Text + "'";

                                if (fpSpread1.Sheets[0].Cells[i, 3].Text != "0" && fpSpread1.Sheets[0].Cells[i, 3].Text != "")//합계
                                    strSql += ", @pPLAN_AMT = '" + fpSpread1.Sheets[0].Cells[i, 3].Value + "'";

                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
                    }
                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001"; //에러가 발생되어 데이터 처리가 취소되었습니다.
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

                this.Cursor = Cursors.Default;
            }
        }
        #endregion	

        #region fpSpread1_Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            try
            {
                decimal amt1 = 0;
                decimal amt2 = 0;
                decimal amt3 = 0;
                decimal amt4 = 0;
                decimal amt5 = 0;
                decimal amt6 = 0;
                decimal amt7 = 0;
                decimal amt8 = 0;
                decimal amt9 = 0;
                decimal amt10 = 0;
                decimal amt11 = 0;
                decimal amt12 = 0;
                decimal amt13 = 0;


                if (Row == 1 || Row == 2) //매출
                {
                    if (fpSpread1.Sheets[0].Cells[3, 3].Text != "") //매출원가
                        amt13 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[3, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[1, 3].Text != "") //방산
                        amt11 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[1, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[2, 3].Text != "") //시스템
                        amt12 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[2, 3].Value);

                    fpSpread1.Sheets[0].Cells[0, 3].Value = amt11 + amt12; //매출
                    fpSpread1.Sheets[0].RowHeader.Cells[0, 0].Text = "U";

                    amt9 = (amt11 + amt12) - amt13; //매출총이익
                    fpSpread1.Sheets[0].Cells[6, 3].Value = amt9;
                    fpSpread1.Sheets[0].RowHeader.Cells[6, 0].Text = "U";

                    if (fpSpread1.Sheets[0].Cells[7, 3].Text != "") //판매비와 관리비
                        amt10 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[7, 3].Value);

                    amt6 = amt9 - amt10;
                    fpSpread1.Sheets[0].Cells[8, 3].Value = amt6; //영업이익
                    fpSpread1.Sheets[0].RowHeader.Cells[8, 0].Text = "U";

                    if (fpSpread1.Sheets[0].Cells[9, 3].Text != "") //영업외이익
                        amt7 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[9, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[10, 3].Text != "") //영업외비용
                        amt8 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[10, 3].Value);

                    amt3 = amt6 + amt7 - amt8;
                    fpSpread1.Sheets[0].Cells[11, 3].Value = amt3; //경상이익
                    fpSpread1.Sheets[0].RowHeader.Cells[11, 0].Text = "U";

                    if (fpSpread1.Sheets[0].Cells[12, 3].Text != "") //특별이익
                        amt4 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[12, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[13, 3].Text != "") //특별손실
                        amt5 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[13, 3].Value);


                    amt1 = amt3 + amt4 - amt5;    //법인세 차감전 순이익

                    fpSpread1.Sheets[0].Cells[14, 3].Value = amt1; //법인세 차감전 순이익
                    fpSpread1.Sheets[0].RowHeader.Cells[14, 0].Text = "U";

                    if (fpSpread1.Sheets[0].Cells[15, 3].Text != "") //법인세 비용
                        amt2 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[15, 3].Value);

                    fpSpread1.Sheets[0].Cells[16, 3].Value = amt1 - amt2; //당기순이익
                    fpSpread1.Sheets[0].RowHeader.Cells[16, 0].Text = "U";

                }
                else if (Row == 4 || Row == 5)  //매출원가
                {

                    if (fpSpread1.Sheets[0].Cells[0, 3].Text != "") //매출
                        amt13 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[4, 3].Text != "") //방산
                        amt11 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[4, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[5, 3].Text != "") //시스템
                        amt12 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[5, 3].Value);

                    fpSpread1.Sheets[0].Cells[3, 3].Value = amt11 + amt12; //매출원가
                    fpSpread1.Sheets[0].RowHeader.Cells[3, 0].Text = "U";

                    amt9 = amt13 - (amt11 + amt12); //매출총이익
                    fpSpread1.Sheets[0].Cells[6, 3].Value = amt9;
                    fpSpread1.Sheets[0].RowHeader.Cells[6, 0].Text = "U";

                    if (fpSpread1.Sheets[0].Cells[7, 3].Text != "") //판매비와 관리비
                        amt10 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[7, 3].Value);

                    amt6 = amt9 - amt10;
                    fpSpread1.Sheets[0].Cells[8, 3].Value = amt6; //영업이익
                    fpSpread1.Sheets[0].RowHeader.Cells[8, 0].Text = "U";

                    if (fpSpread1.Sheets[0].Cells[9, 3].Text != "") //영업외이익
                        amt7 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[9, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[10, 3].Text != "") //영업외비용
                        amt8 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[10, 3].Value);

                    amt3 = amt6 + amt7 - amt8;
                    fpSpread1.Sheets[0].Cells[11, 3].Value = amt3; //경상이익
                    fpSpread1.Sheets[0].RowHeader.Cells[11, 0].Text = "U";

                    if (fpSpread1.Sheets[0].Cells[12, 3].Text != "") //특별이익
                        amt4 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[12, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[13, 3].Text != "") //특별손실
                        amt5 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[13, 3].Value);


                    amt1 = amt3 + amt4 - amt5;    //법인세 차감전 순이익

                    fpSpread1.Sheets[0].Cells[14, 3].Value = amt1; //법인세 차감전 순이익
                    fpSpread1.Sheets[0].RowHeader.Cells[14, 0].Text = "U";

                    if (fpSpread1.Sheets[0].Cells[15, 3].Text != "") //법인세 비용
                        amt2 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[15, 3].Value);

                    fpSpread1.Sheets[0].Cells[16, 3].Value = amt1 - amt2; //당기순이익
                    fpSpread1.Sheets[0].RowHeader.Cells[16, 0].Text = "U";
                }
                else if (Row == 7)  //판매비와 관리비
                {
                    if (fpSpread1.Sheets[0].Cells[6, 3].Text != "") //매출총이익
                        amt9 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[6, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[7, 3].Text != "") //판매비와 관리비
                        amt10 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[7, 3].Value);

                    amt6 = amt9 - amt10;
                    fpSpread1.Sheets[0].Cells[8, 3].Value = amt6; //영업이익
                    fpSpread1.Sheets[0].RowHeader.Cells[8, 0].Text = "U";

                    if (fpSpread1.Sheets[0].Cells[9, 3].Text != "") //영업외이익
                        amt7 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[9, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[10, 3].Text != "") //영업외비용
                        amt8 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[10, 3].Value);

                    amt3 = amt6 + amt7 - amt8;
                    fpSpread1.Sheets[0].Cells[11, 3].Value = amt3; //경상이익
                    fpSpread1.Sheets[0].RowHeader.Cells[11, 0].Text = "U";

                    if (fpSpread1.Sheets[0].Cells[12, 3].Text != "") //특별이익
                        amt4 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[12, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[13, 3].Text != "") //특별손실
                        amt5 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[13, 3].Value);


                    amt1 = amt3 + amt4 - amt5;    //법인세 차감전 순이익

                    fpSpread1.Sheets[0].Cells[14, 3].Value = amt1; //법인세 차감전 순이익
                    fpSpread1.Sheets[0].RowHeader.Cells[14, 0].Text = "U";

                    if (fpSpread1.Sheets[0].Cells[15, 3].Text != "") //법인세 비용
                        amt2 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[15, 3].Value);

                    fpSpread1.Sheets[0].Cells[16, 3].Value = amt1 - amt2; //당기순이익
                    fpSpread1.Sheets[0].RowHeader.Cells[16, 0].Text = "U";
                }
                else if (Row == 9 || Row == 10)  //영업외수익, 영업외비용
                {

                    if (fpSpread1.Sheets[0].Cells[8, 3].Text != "") //영업이익
                        amt6 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[8, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[9, 3].Text != "") //영업외이익
                        amt7 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[9, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[10, 3].Text != "") //영업외비용
                        amt8 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[10, 3].Value);

                    amt3 = amt6 + amt7 - amt8;
                    fpSpread1.Sheets[0].Cells[11, 3].Value = amt3; //경상이익
                    fpSpread1.Sheets[0].RowHeader.Cells[11, 0].Text = "U";

                    if (fpSpread1.Sheets[0].Cells[12, 3].Text != "") //특별이익
                        amt4 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[12, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[13, 3].Text != "") //특별손실
                        amt5 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[13, 3].Value);


                    amt1 = amt3 + amt4 - amt5;    //법인세 차감전 순이익

                    fpSpread1.Sheets[0].Cells[14, 3].Value = amt1; //법인세 차감전 순이익
                    fpSpread1.Sheets[0].RowHeader.Cells[14, 0].Text = "U";

                    if (fpSpread1.Sheets[0].Cells[15, 3].Text != "") //법인세 비용
                        amt2 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[15, 3].Value);

                    fpSpread1.Sheets[0].Cells[16, 3].Value = amt1 - amt2; //당기순이익
                    fpSpread1.Sheets[0].RowHeader.Cells[16, 0].Text = "U";
                }
                else if (Row == 12 || Row == 13)  //특별이익, 특별손실
                {

                    if (fpSpread1.Sheets[0].Cells[11, 3].Text != "") //경상이익
                        amt3 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[11, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[12, 3].Text != "") //특별이익
                        amt4 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[12, 3].Value);

                    if (fpSpread1.Sheets[0].Cells[13, 3].Text != "") //특별손실
                        amt5 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[13, 3].Value);


                    amt1 = amt3 + amt4 - amt5;    //법인세 차감전 순이익

                    fpSpread1.Sheets[0].Cells[14, 3].Value = amt1; //법인세 차감전 순이익
                    fpSpread1.Sheets[0].RowHeader.Cells[14, 0].Text = "U";


                    if (fpSpread1.Sheets[0].Cells[15, 3].Text != "") //법인세 비용
                        amt2 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[15, 3].Value);

                    fpSpread1.Sheets[0].Cells[16, 3].Value = amt1 - amt2; //당기순이익
                    fpSpread1.Sheets[0].RowHeader.Cells[16, 0].Text = "U";
                }

                else if (Row == 15) //법인세비용
                {

                    if (fpSpread1.Sheets[0].Cells[14, 3].Text != "")  //법인세 차감전 순이익
                        amt1 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[14, 3].Value);
                    if (fpSpread1.Sheets[0].Cells[15, 3].Text != "") //법인세 비용
                        amt2 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[15, 3].Value);

                    fpSpread1.Sheets[0].Cells[16, 3].Value = amt1 - amt2; //당기순이익
                    fpSpread1.Sheets[0].RowHeader.Cells[16, 0].Text = "U";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void btnConfirmOk_Click(object sender, EventArgs e)
        {
            XAA005P1 frm1 = new XAA005P1(dtpYearMon.Text.Substring(0, 4));
            frm1.ShowDialog();
        }

    }
}
