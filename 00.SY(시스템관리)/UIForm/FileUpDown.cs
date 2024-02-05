using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace UIForm
{
    public partial class FileUpDown : Form
    {
        #region 변수선언

        string FILES_No = "";
        string UpDnDl = "";

        private System.Windows.Forms.TextBox txtAttachCnt;

        //그리드 디자인 변수
        public string[] G1Head1 = null;// 첫번째 Head Text
        public string[] G1Head2 = null;// 두번째 Head Text
        public string[] G1Head3 = null;// 세번째 Head Text
        public int[] G1Width = null;// Cell 넓이
        public string[] G1Align = null;// Cell 데이타 정렬방식
        public string[] G1Type = null;// CellType 지정
        public int[] G1Color = null;// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
        public string[] G1Etc = new string[100];// Mask 양식 등
        public int[] G1SEQ = null;
        public int G1HeadCnt = 0;	// Head 수

        #endregion

        #region 생성자
        public FileUpDown()
        {
            InitializeComponent();
        }

        public FileUpDown(string FILES_NO, string updndl)
        {
            InitializeComponent();
            FILES_No = FILES_NO;
            UpDnDl = updndl;
        }

        public FileUpDown(string FILES_NO, string updndl, TextBox txtFileCnt)
        {
            InitializeComponent();
            FILES_No = FILES_NO;		// 파일 번호
            UpDnDl = updndl;		// 버튼 권한 업로드,다운로드,삭제 표기 -> "Y#Y#Y"
            txtAttachCnt = txtFileCnt;	// 파일 첨부수량 TextBox에 보여줄때
        }
        #endregion

        #region 폼로드 이벤트
        private void FileUpDown_Load(object sender, EventArgs e)
        {
            Regex rx = new Regex("#");
            string[] QValue = rx.Split(UpDnDl.Replace("|", "#"));

            if (QValue.Length > 0)
            {
                if (QValue[0].ToString() == "N")
                    btnUpload.Enabled = false;
            }

            if (QValue.Length > 1)
            {
                if (QValue[1].ToString() == "N")
                    btnDownload.Enabled = false;
            }

            if (QValue.Length > 2)
            {
                if (QValue[2].ToString() == "N")
                    btnDelete.Enabled = false;
            }

            FPDesign();

            Search();
        }
        #endregion

        #region FPDesign - 그리드 디자인
        public void FPDesign()
        {
            try
            {
                string Query = " usp_BAA004 'S3',@PFORM_ID='" + this.Name.ToString() + "', @PGRID_NAME='fpSpread1', @PIN_ID='" + SystemBase.Base.gstrUserID + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                int G1RowCount = dt.Rows.Count + 1;

                if (G1RowCount > 0)
                {
                    G1Head1 = new string[G1RowCount];	// 첫번째 Head Text
                    G1Head2 = new string[G1RowCount];	// 두번째 Head Text
                    G1Head3 = new string[G1RowCount];	// 세번째 Head Text
                    G1Width = new int[G1RowCount];		// Cell 넓이
                    G1Align = new string[G1RowCount];	// Cell 데이타 정렬방식
                    G1Type = new string[G1RowCount];	// CellType 지정
                    G1Color = new int[G1RowCount];		// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
                    G1SEQ = new int[G1RowCount];		// 키
                    G1HeadCnt = Convert.ToInt32(dt.Rows[0][0].ToString());

                    //####################1번째 숨김필드 정의######################
                    G1Head1[0] = "";
                    if (Convert.ToInt32(dt.Rows[0][0].ToString()) >= 1)
                        G1Head2[0] = "";
                    if (Convert.ToInt32(dt.Rows[0][0].ToString()) == 3)
                        G1Head3[0] = "";
                    G1Width[0] = 0;
                    G1Align[0] = "";
                    G1Type[0] = "";
                    G1Color[0] = 0;
                    G1Etc[0] = "";
                    //####################1번째 숨김필드 정의######################


                    for (int i = 1; i < G1RowCount; i++)
                    {
                        G1Head1[i] = dt.Rows[i - 1][1].ToString();
                        if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) >= 1)
                            G1Head2[i] = dt.Rows[i - 1][2].ToString();
                        if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) == 3)
                            G1Head3[i] = dt.Rows[i - 1][3].ToString();
                        G1Width[i] = Convert.ToInt32(dt.Rows[i - 1][4].ToString());
                        G1Align[i] = dt.Rows[i - 1][5].ToString();
                        G1Type[i] = dt.Rows[i - 1][6].ToString();
                        G1Color[i] = Convert.ToInt32(dt.Rows[i - 1][7].ToString());
                        if (G1Etc[i] == null)
                            G1Etc[i] = dt.Rows[i - 1][8].ToString();
                        G1SEQ[i] = Convert.ToInt32(dt.Rows[i - 1][9].ToString());

                        fpSpread1.Sheets[0].Rows[i].Height = 20;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region 조회
        public void Search()
        {
            try
            {
                string Query = " usp_B_IMAGES 'S1', @pFILES_NO='" + FILES_No + "' ";
                UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

                if (txtAttachCnt != null)
                {
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                        txtAttachCnt.Text = fpSpread1.Sheets[0].Rows.Count.ToString();	// 첨부수량 변경
                    else
                        txtAttachCnt.Text = "0";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show("파일 첨부 조회 중 예기치 못한 오류가 발생하였습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 파일업로드
        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                SystemBase.FILESAVE.FileInsert(FILES_No);
                Search();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdCommSheet (그리드공통 조회실패 )", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 파일 다운로드
        private void btnDownload_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                string FilePath = fd.SelectedPath.ToString();
                int DownCnt = 0;

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, 1].Value.ToString() == "True")
                    {
                        string Query = " usp_B_IMAGE @pType='S2' ";
                        Query += ", @pFILES_NO = '" + fpSpread1.Sheets[0].Cells[i, 0].Value + "'";
                        DataTable DT = SystemBase.DbOpen.NoTranDataTable(Query);

                        if (DT.Rows.Count > 0)
                        {
                            byte[] MyData = null;
                            MyData = (byte[])DT.Rows[0][2];
                            int ArraySize = new int();
                            ArraySize = MyData.GetUpperBound(0);

                            FileStream fs = new FileStream(FilePath + @"\" + DT.Rows[0][0].ToString() + "." + DT.Rows[0][1].ToString(), FileMode.Create, FileAccess.Write);

                            fs.Write(MyData, 0, ArraySize + 1);
                            fs.Close();
                        }
                        DownCnt++;
                    }
                }
                if (DownCnt > 0)
                {
                    MessageBox.Show("선택한 폴드에 " + DownCnt.ToString() + "개의 파일을 다운로드 하였습니다.", SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("다운로드 할 파일을 선택하지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        #endregion

        #region 파일삭제
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string ERRCode = "ER";
            string MSGCode = "P0000";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                int DownCnt = 0;
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, 1].Value.ToString() == "True")
                    {
                        string strSql = " usp_B_IMAGE @pType='D1' ";
                        strSql = strSql + ", @pFILES_NO = '" + fpSpread1.Sheets[0].Cells[i, 0].Value.ToString() + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        DownCnt++;
                    }
                }
                Trans.Commit();

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                MSGCode = "P0001";
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        Exit:
            dbConn.Close();
            if (ERRCode != "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Search();
        }
        #endregion

        #region 닫기
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
