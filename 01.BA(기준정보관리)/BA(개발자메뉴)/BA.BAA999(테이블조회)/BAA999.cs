using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

namespace BA.BAA999
{
    public partial class BAA999 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string SaveData = "", SearchData = ""; //컨트롤에 대한 조회후 데이터와 저장시 변경된 데이터 체크위한 변수
        #endregion

        #region 생성자
        public BAA999()
        {
            InitializeComponent();
        }
        #endregion

        #region BAA999_Load
        private void BAA999_Load(object sender, EventArgs e)
        {
            Reset();
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {

                string strQuery = " usp_BAA999  @pTYPE = 'S1'";
                strQuery = strQuery + ", @pTABLE_ID ='" + txtTABLE_ID.Text.ToString() + "' ";
   
                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, false);
            }
        }
        #endregion

        #region fpSpread2_SelectionChanged
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            Right_Search();
        }
        #endregion

        #region btnSAVE_Click
        private void btnSAVE_Click(object sender, EventArgs e)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);


                try
                {
                    string strSql = " usp_BAA999 ";
                    strSql += "  @pTYPE =  'U1' ";
                    strSql += ", @pTABLE_ID = '" + txtTABLE_ID2.Text.ToString() + "' ";
                    strSql += ", @pTABLE_NM = '" + txtTABLE_NM2.Text.ToString() + "' ";          

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "SY002"; // 에러가 발생되어 데이터 처리가 취소되었습니다.
                }

            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    //그리드 포커스 셋팅
                    UIForm.FPMake.GridSetFocus(fpSpread2, txtTABLE_ID2.Text.ToString(), SystemBase.Base.GridHeadIndex(GHIdx2, "테이블명"));

                    Right_Search();

                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (ERRCode == "ER") //ERROR
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else   //ERRCode == "WR" WARING
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;
                         
            if (MessageBox.Show(SystemBase.Base.MessageRtn("SY048"), "저장", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)  // 저장여부 묻기
            {
                string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                string strSql = "";
                try
                {      
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U2"; break;
                                case "I": strGbn = "I2"; break;
                                case "D": strGbn = "D2"; break;
                                default: strGbn = ""; break;
                            }

                            strSql = " usp_BAA999 ";
                            strSql += "  @pTYPE =  '"+strGbn+"' ";
                            strSql += ", @pTABLE_ID = '" + txtTABLE_ID2.Text.ToString() + "' ";
                            strSql += ", @pTABLE_NM = '" + txtTABLE_NM2.Text.ToString() + "' ";
                            strSql += ", @pCOLUMN_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "컬럼명")].Text + "' ";
                            strSql += ", @pCOLUMN_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "설명")].Text + "' ";

                            DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; } 	// ER 코드 Return시 점프
                        }
                    }

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "SY002"; // 에러가 발생되어 데이터 처리가 취소되었습니다.
                }

            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    //그리드 포커스 셋팅
                    UIForm.FPMake.GridSetFocus(fpSpread2, txtTABLE_ID2.Text.ToString(), SystemBase.Base.GridHeadIndex(GHIdx2, "테이블ID"));

                    Right_Search();

                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (ERRCode == "ER") //ERROR
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else   //ERRCode == "WR" WARING
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
           

            this.Cursor = Cursors.Default;
        }
        #endregion




        #region ***********************  User  Function ************************************
        
        #region Reset()
        private void Reset()
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting
            SystemBase.Validation.GroupBox_Setting(groupBox2);	//컨트롤 필수 Setting
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

            PreRow = -1;
        }
        #endregion
        
        #region Right_Search
        private void Right_Search()
        {
            try
            {
                //같은 Row 조회 되지 않게
                int intRow = fpSpread2.ActiveSheet.ActiveRowIndex;
                if (intRow < 0)
                {
                    Reset();
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }
                if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }

                // MASTER 조회
                txtTABLE_ID2.Value = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "테이블명")].Text.ToString();
                txtTABLE_NM2.Value = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "테이블설명")].Text.ToString();


                // DETAIL 조회
                string strQuery = " usp_BAA999  @pTYPE = 'S2'";
                strQuery = strQuery + ", @pTABLE_ID ='" + txtTABLE_ID2.Text.ToString() + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                                

                //현재 row값 설정
                PreRow = fpSpread2.ActiveSheet.ActiveRowIndex;

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

            }
        }
        #endregion
        
        #endregion
    }
}
