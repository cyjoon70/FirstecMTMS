#region 작성정보
/*********************************************************************/
// 단위업무명 : 공지사항
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-05
// 작성내용 : 공지사항 등록 및 관리
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
using System.Data.SqlClient;

namespace BB.BZG001
{
    public partial class BZG001P1 : UIForm.Buttons
    {
        #region 변수선언
        private string Gubun;
        private string Idx;
        #endregion

        #region 생성자
        public BZG001P1(string gubun, string idx)
		{
			InitializeComponent();

			Gubun = gubun;
			Idx = idx;	
		}
        #endregion

        #region 폼로드
        private void BZG001P1_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("000000110001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            if (Gubun == "W")
            {
                SelectMode();
            }
            else
            {
                SelectExec();
                SelectMode();
            }
        }

        private void SelectMode()
        {
            if (Gubun == "W")
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                textBox1.ReadOnly = true;
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true;
                textBox4.ReadOnly = true;
                textBox5.ReadOnly = false;
                textBox6.ReadOnly = false;
                UIForm.Buttons.ReButton("000000010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                textBox1.BackColor = System.Drawing.Color.WhiteSmoke;
                textBox2.BackColor = System.Drawing.Color.WhiteSmoke;
                textBox3.BackColor = System.Drawing.Color.WhiteSmoke;
                textBox4.BackColor = System.Drawing.Color.WhiteSmoke;


            }
            else if (Gubun == "R" && textBox3.Text == SystemBase.Base.gstrUserName)
            {
                textBox1.ReadOnly = true;
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true;
                textBox4.ReadOnly = true;
                textBox5.ReadOnly = false;
                textBox6.ReadOnly = false;
                UIForm.Buttons.ReButton("000000110000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                textBox1.BackColor = System.Drawing.Color.WhiteSmoke;
                textBox2.BackColor = System.Drawing.Color.WhiteSmoke;
                textBox3.BackColor = System.Drawing.Color.WhiteSmoke;
                textBox4.BackColor = System.Drawing.Color.WhiteSmoke;
            }
            else if (Gubun == "R" && textBox3.Text != SystemBase.Base.gstrUserName)
            {
                textBox1.ReadOnly = true;
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true;
                textBox4.ReadOnly = true;
                textBox5.ReadOnly = true;
                textBox6.ReadOnly = true;
                UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                textBox1.BackColor = System.Drawing.Color.WhiteSmoke;
                textBox2.BackColor = System.Drawing.Color.WhiteSmoke;
                textBox3.BackColor = System.Drawing.Color.WhiteSmoke;
                textBox4.BackColor = System.Drawing.Color.WhiteSmoke;
                textBox5.BackColor = System.Drawing.Color.WhiteSmoke;
                textBox6.BackColor = System.Drawing.Color.WhiteSmoke;
            }
        }

        private void SelectExec()
        {
            string strQuery = "";
            strQuery = " USP_BZG001 @pTYPE = 'S2' ";
            strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
            strQuery = strQuery + ", @pIDX = " + Idx;
            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

            textBox1.Text = ds.Tables[0].Rows[0][1].ToString();  //NO
            textBox2.Text = ds.Tables[0].Rows[0][2].ToString();  //조회수
            textBox3.Text = ds.Tables[0].Rows[0][3].ToString();  //등록자
            textBox4.Text = ds.Tables[0].Rows[0][4].ToString();  //등록일
            textBox5.Text = ds.Tables[0].Rows[0][5].ToString();  //제목
            textBox6.Text = ds.Tables[0].Rows[0][6].ToString();  //내용

        }
        #endregion

        #region SaveExec()
        protected override void SaveExec()
        {
            string ERRCode = "ER", MSGCode = "";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            if (Gubun == "W")
            {
                try
                {
                    string strQuery = "";
                    strQuery = " USP_BZG001 @pTYPE = 'I1' ";
                    strQuery = strQuery + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery = strQuery + ", @pTITLE ='" + textBox5.Text + "' ";
                    strQuery = strQuery + ", @pCONTENT ='" + textBox6.Text + "' ";
                    strQuery = strQuery + ", @pUSR_ID = '" + SystemBase.Base.gstrUserID + "'";
                    strQuery = strQuery + ", @pFILE_FG = '" + "N" + "'";
                    strQuery = strQuery + ", @pFILENAME1 = '" + "" + "'";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "ER")
                    {
                        Trans.Rollback();
                        goto Exit;	// ER 코드 Return시 점프
                    }
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.ToString());
                    MSGCode = "P0001";
                    goto Exit;	// ER 코드 Return시 점프
                }
                Trans.Commit();

            Exit:
                dbConn.Close();
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));

                if (ERRCode != "")
                    Dispose(true);


            }
            else if (Gubun == "R" && textBox3.Text == SystemBase.Base.gstrUserName)
            {
                try
                {
                    string strQuery = "";
                    strQuery = " USP_BZG001 @pTYPE = 'U1' ";
                    strQuery = strQuery + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery = strQuery + ", @pTITLE ='" + textBox5.Text + "' ";
                    strQuery = strQuery + ", @pCONTENT ='" + textBox6.Text + "' ";
                    strQuery = strQuery + ", @pIDX =" + textBox1.Text + "";
                    strQuery = strQuery + ", @pUSR_ID = '" + SystemBase.Base.gstrUserID + "'";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";


                    DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "ER")
                    {
                        Trans.Rollback();
                        goto Exit;	// ER 코드 Return시 점프
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.ToString());
                    MSGCode = "P0001";
                    goto Exit;	// ER 코드 Return시 점프
                }
                Trans.Commit();

            Exit:
                dbConn.Close();
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
                if (ERRCode != "")
                    Dispose(true);

            }
        }
        #endregion
        
        #region DeleteExec()
        protected override void DeleteExec()
        { 
            DialogResult result = SystemBase.MessageBoxComm.Show("삭제 하시겠습니까?", "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string ERRCode, MSGCode = "";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                if (Gubun == "R" && textBox3.Text == SystemBase.Base.gstrUserName)
                {
                    try
                    {
                        string strQuery = "";
                        strQuery = " USP_BZG001 @pTYPE = 'D1' ";
                        strQuery = strQuery + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                        strQuery = strQuery + ", @pIDX =" + textBox1.Text + "";
                        strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode == "ER")
                        {
                            Trans.Rollback();
                            goto Exit;	// ER 코드 Return시 점프
                        }
                        else
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        Trans.Rollback();
                        MessageBox.Show(ex.ToString());
                        MSGCode = "P0001";
                        goto Exit;	// ER 코드 Return시 점프
                    }
                    Trans.Commit();

                Exit:
                    dbConn.Close();
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
                    Dispose(true);
                }
            }
        }
        #endregion
	
    }
}
