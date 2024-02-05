using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace UIForm
{
    public partial class Buttons : Form
    {
        private System.Windows.Forms.ToolTip totBTN;
        //private System.ComponentModel.IContainer components;

        public string strJumpFileName1 = "";
        public string strJumpFileName2 = "";
        public string strJumpFileName3 = "";
        public string strJumpFileName4 = "";
        public string strJumpFileName5 = "";
        public string strJumpFileName6 = "";
       // public string strFormName = "";
        public string[,] GHIdx1 = null;	// Grid Head 위치
        public string[,] GHIdx2 = null;	// Grid Head 위치
        public string[,] GHIdx3 = null;	// Grid Head 위치
        public string[,] GHIdx4 = null;	// Grid Head 위치

        public object[] param = null;

        public bool strFormClosingMsg = true;

        public Buttons()
        {
            //SystemBase.Base.gstrFormClosingMsg = 1;

            InitializeComponent();
        }
        
        private void Buttons_Load(object sender, System.EventArgs e)
        {
            try
            {
                //lblFormName.Text = SystemBase.Base.RodeFormText;
                this.Text = SystemBase.Base.RodeFormText;
                this.BackColor = Color.White;

                if (SystemBase.Base.ProgramWhere.Length > 0)
                {
                    LoadButton(BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show("Toolbar 버튼 구성중 예기치 못한 오류가 발생하였습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region LoadButton 버튼 권한 설정
        public static void LoadButton(PictureBox BtnNew, PictureBox BtnSearch, PictureBox BtnRCopy, PictureBox BtnRowIns, PictureBox BtnCancel, PictureBox BtnDel, PictureBox BtnDelete, PictureBox BtnInsert, PictureBox BtnExcel, PictureBox BtnPrint, PictureBox BtnHelp, PictureBox BtnClose)
        {
            string btnQuery = "usp_TOOLBARSET '" + SystemBase.Base.gstrUserID.ToString() + "','" + SystemBase.Base.RodeFormID.ToString() + "'";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(btnQuery);

            // 신규
            if (dt.Rows[0][0].ToString() == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\New.gif");
                BtnNew.Image = bitMap;
                BtnNew.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dNew.gif");
                BtnNew.Image = bitMap;
                BtnNew.Enabled = false;
            }

            //조회
            if (dt.Rows[0][1].ToString() == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Search.gif");
                BtnSearch.Image = bitMap;
                BtnSearch.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dSearch.gif");
                BtnSearch.Image = bitMap;
                BtnSearch.Enabled = false;
            }
            
            //행복사
            if (dt.Rows[0][2].ToString() == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\RCopy.gif");
                BtnRCopy.Image = bitMap;
                BtnRCopy.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dRCopy.gif");
                BtnRCopy.Image = bitMap;
                BtnRCopy.Enabled = false;
            }
            
            //행추가
            if (dt.Rows[0][3].ToString() == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\RAdd.gif");
                BtnRowIns.Image = bitMap;
                BtnRowIns.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dRAdd.gif");
                BtnRowIns.Image = bitMap;
                BtnRowIns.Enabled = false;
            }
           
            //행취소
            if (dt.Rows[0][4].ToString() == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Cancel.gif");
                BtnCancel.Image = bitMap;
                BtnCancel.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dCancel.gif");
                BtnCancel.Image = bitMap;
                BtnCancel.Enabled = false;
            }
     
           //행삭제
           if (dt.Rows[0][5].ToString() == "1")
           {
               Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\RDelete.gif");
               BtnDel.Image = bitMap;
               BtnDel.Enabled = true;
           }
           else
           {
               Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dRDelete.gif");
               BtnDel.Image = bitMap;
               BtnDel.Enabled = false;
           }

           //삭제
           if (dt.Rows[0][6].ToString() == "1")
           {
               Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Delete.gif");
               BtnDelete.Image = bitMap;
               BtnDelete.Enabled = true;
           }
           else
           {
               Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dDelete.gif");
               BtnDelete.Image = bitMap;
               BtnDelete.Enabled = false;
           }

           //저장
           if (dt.Rows[0][7].ToString() == "1")
           {
               Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Save.gif");
               BtnInsert.Image = bitMap;
               BtnInsert.Enabled = true;
           }
           else
           {
               Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dSave.gif");
               BtnInsert.Image = bitMap;
               BtnInsert.Enabled = false;
           }

           //엑셀
           if (dt.Rows[0][8].ToString() == "1")
           {
               Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Excel.gif");
               BtnExcel.Image = bitMap;
               BtnExcel.Enabled = true;
           }
           else
           {
               Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dExcel.gif");
               BtnExcel.Image = bitMap;
               BtnExcel.Enabled = false;
           }

           //출력
           if (dt.Rows[0][9].ToString() == "1")
           {
               Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Print.gif");
               BtnPrint.Image = bitMap;
               BtnPrint.Enabled = true;
           }
           else
           {
               Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dPrint.gif");
               BtnPrint.Image = bitMap;
               BtnPrint.Enabled = false;
           }

           //도움말
           if (dt.Rows[0][10].ToString() == "1")
           {
               Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Help.gif");
               BtnHelp.Image = bitMap;
               BtnHelp.Enabled = true;
           }
           else
           {
               Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dHelp.gif");
               BtnHelp.Image = bitMap;
               BtnHelp.Enabled = false;
           }

           //종료
           if (dt.Rows[0][11].ToString() == "1")
           {
               Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Fcls.gif");
               BtnClose.Image = bitMap;
               BtnClose.Enabled = true;
           }
           else
           {
               Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dFcls.gif");
               BtnClose.Image = bitMap;
               BtnClose.Enabled = false;
           }
         
        }
        #endregion

        #region ReButton 버튼 권한 재정의
        //예제 : UIForm.Buttons.ReButton("001010101010", BtnNew, BtnSearch,  BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, , BtnClose);
        public static void ReButton(string Btn, PictureBox BtnNew, PictureBox BtnSearch, PictureBox BtnRCopy, PictureBox BtnRowIns, PictureBox BtnCancel, PictureBox BtnDel, PictureBox BtnDelete, PictureBox BtnInsert, PictureBox BtnExcel, PictureBox BtnPrint, PictureBox BtnHelp, PictureBox BtnClose)
        {
            if (Btn.Substring(0, 1) == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\New.gif");
                BtnNew.Image = bitMap;
                BtnNew.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dNew.gif");
                BtnNew.Image = bitMap;
                BtnNew.Enabled = false;
            }


            if (Btn.Substring(1, 1) == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Search.gif");
                BtnSearch.Image = bitMap;
                BtnSearch.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dSearch.gif");
                BtnSearch.Image = bitMap;
                BtnSearch.Enabled = false;
            }

            if (Btn.Substring(2, 1) == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\RCopy.gif");
                BtnRCopy.Image = bitMap;
                BtnRCopy.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dRCopy.gif");
                BtnRCopy.Image = bitMap;
                BtnRCopy.Enabled = false;
            }

            if (Btn.Substring(3, 1) == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\RAdd.gif");
                BtnRowIns.Image = bitMap;
                BtnRowIns.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dRAdd.gif");
                BtnRowIns.Image = bitMap;
                BtnRowIns.Enabled = false;
            }

            if (Btn.Substring(4, 1) == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Cancel.gif");
                BtnCancel.Image = bitMap;
                BtnCancel.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dCancel.gif");
                BtnCancel.Image = bitMap;
                BtnCancel.Enabled = false;
            }

            if (Btn.Substring(5, 1) == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\RDelete.gif");
                BtnDel.Image = bitMap;
                BtnDel.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dRDelete.gif");
                BtnDel.Image = bitMap;
                BtnDel.Enabled = false;
            }

            if (Btn.Substring(6, 1) == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Delete.gif");
                BtnDelete.Image = bitMap;
                BtnDelete.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dDelete.gif");
                BtnDelete.Image = bitMap;
                BtnDelete.Enabled = false;
            }

            if (Btn.Substring(7, 1) == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Save.gif");
                BtnInsert.Image = bitMap;
                BtnInsert.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dSave.gif");
                BtnInsert.Image = bitMap;
                BtnInsert.Enabled = false;
            }


            if (Btn.Substring(8, 1) == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Excel.gif");
                BtnExcel.Image = bitMap;
                BtnExcel.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dExcel.gif");
                BtnExcel.Image = bitMap;
                BtnExcel.Enabled = false;
            }

            if (Btn.Substring(9, 1) == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Print.gif");
                BtnPrint.Image = bitMap;
                BtnPrint.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dPrint.gif");
                BtnPrint.Image = bitMap;
                BtnPrint.Enabled = false;
            }

            if (Btn.Substring(10, 1) == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Help.gif");
                BtnHelp.Image = bitMap;
                BtnHelp.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dHelp.gif");
                BtnHelp.Image = bitMap;
                BtnHelp.Enabled = false;
            }

            if (Btn.Substring(11, 1) == "1")
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Fcls.gif");
                BtnClose.Image = bitMap;
                BtnClose.Enabled = true;
            }
            else
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dFcls.gif");
                BtnClose.Image = bitMap;
                BtnClose.Enabled = false;
            }
        }
        #endregion

        #region ReButton 버튼 권한 개별 재정의
        //예제 : UIForm.Buttons.ReButton(BtnNew, "BtnNew", true);	//BtnPrint, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnHelp, BtnExcel, BtnClose
        public static void ReButton(PictureBox PBox, string BtnName, bool Kind)
        {
            if (BtnName == "BtnNew" && Kind == true)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\New.gif");
                PBox.Image = bitMap;
                PBox.Enabled = true;
            }
            else if (BtnName == "BtnNew" && Kind == false)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dNew.gif");
                PBox.Image = bitMap;
                PBox.Enabled = false;
            }

            if (BtnName == "BtnPrint" && Kind == true)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Print.gif");
                PBox.Image = bitMap;
                PBox.Enabled = true;
            }
            else if (BtnName == "BtnPrint" && Kind == false)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dPrint.gif");
                PBox.Image = bitMap;
                PBox.Enabled = false;
            }

            if (BtnName == "BtnSearch" && Kind == true)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Search.gif");
                PBox.Image = bitMap;
                PBox.Enabled = true;
            }
            else if (BtnName == "BtnSearch" && Kind == false)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dSearch.gif");
                PBox.Image = bitMap;
                PBox.Enabled = false;
            }

            if (BtnName == "BtnRCopy" && Kind == true)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\RCopy.gif");
                PBox.Image = bitMap;
                PBox.Enabled = true;
            }
            else if (BtnName == "BtnRCopy" && Kind == false)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dRCopy.gif");
                PBox.Image = bitMap;
                PBox.Enabled = false;
            }

            if (BtnName == "BtnRowIns" && Kind == true)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\RAdd.gif");
                PBox.Image = bitMap;
                PBox.Enabled = true;
            }
            else if (BtnName == "BtnRowIns" && Kind == false)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dRAdd.gif");
                PBox.Image = bitMap;
                PBox.Enabled = false;
            }

            if (BtnName == "BtnCancel" && Kind == true)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Cancel.gif");
                PBox.Image = bitMap;
                PBox.Enabled = true;
            }
            else if (BtnName == "BtnCancel" && Kind == false)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dCancel.gif");
                PBox.Image = bitMap;
                PBox.Enabled = false;
            }

            if (BtnName == "BtnDel" && Kind == true)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\RDelete.gif");
                PBox.Image = bitMap;
                PBox.Enabled = true;
            }
            else if (BtnName == "BtnDel" && Kind == false)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dRDelete.gif");
                PBox.Image = bitMap;
                PBox.Enabled = false;
            }

            if (BtnName == "BtnDelete" && Kind == true)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Delete.gif");
                PBox.Image = bitMap;
                PBox.Enabled = true;
            }
            else if (BtnName == "BtnDelete" && Kind == false)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dDelete.gif");
                PBox.Image = bitMap;
                PBox.Enabled = false;
            }

            if (BtnName == "BtnInsert" && Kind == true)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Save.gif");
                PBox.Image = bitMap;
                PBox.Enabled = true;
            }
            else if (BtnName == "BtnInsert" && Kind == false)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dSave.gif");
                PBox.Image = bitMap;
                PBox.Enabled = false;
            }

            if (BtnName == "BtnHelp" && Kind == true)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Help.gif");
                PBox.Image = bitMap;
                PBox.Enabled = true;
            }
            else if (BtnName == "BtnHelp" && Kind == false)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dHelp.gif");
                PBox.Image = bitMap;
                PBox.Enabled = false;
            }

            if (BtnName == "BtnExcel" && Kind == true)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Excel.gif");
                PBox.Image = bitMap;
                PBox.Enabled = true;
            }
            else if (BtnName == "BtnExcel" && Kind == false)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dExcel.gif");
                PBox.Image = bitMap;
                PBox.Enabled = false;
            }

            if (BtnName == "BtnClose" && Kind == true)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Fcls.gif");
                PBox.Image = bitMap;
                PBox.Enabled = true;
            }
            else if (BtnName == "BtnClose" && Kind == false)
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dFcls.gif");
                PBox.Image = bitMap;
                PBox.Enabled = false;
            }
        }
        #endregion

        protected virtual void NewExec() { }
        protected virtual void PrintExec() { }
        protected virtual void SearchExec() { }
        protected virtual void RCopyExec() { }
        protected virtual void RowInsExec() { }

        protected virtual void CancelExec() { }
        protected virtual void DelExec() { }
        protected virtual void DeleteExec() { }
        //protected virtual void SaveExec2() { }

        #region SaveExec 저장
        protected virtual void SaveExec() { }
        protected virtual void SaveExec2()
        {	// 저장
            try
            {
                this.BtnInsert.Focus();
                SaveExec();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "저장"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        protected virtual void HelpExec() { }
        protected virtual void ExcelExec() { }

        protected virtual void _flex1Relode() { }
        protected virtual void _flex2Relode() { }
        protected virtual void FXGrid21Relode() { }
        protected virtual void FXGrid22Relode() { }

        protected virtual void Link1Exec() { }
        protected virtual void Link2Exec() { }
        protected virtual void Link3Exec() { }
        protected virtual void Link4Exec() { }
        protected virtual void Link5Exec() { }
        protected virtual void Link6Exec() { }

        #region 버튼 클릭 이벤트
        private void BtnNew_Click(object sender, System.EventArgs e)
        {
            //((Control)sender).Focus();
            NewExec();
        }
        private void BtnPrint_Click(object sender, System.EventArgs e)
        {
            //((Control)sender).Focus();
            PrintExec();
        }
        private void BtnSearch_Click(object sender, System.EventArgs e)
        {
            ((Control)sender).Focus();
            SearchExec();
        }
        private void BtnRCopy_Click(object sender, System.EventArgs e)
        {
            //((Control)sender).Focus();
            RCopyExec();
        }
        private void BtnRowIns_Click(object sender, System.EventArgs e)
        {
            //((Control)sender).Focus();
            RowInsExec();
        }
        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            //((Control)sender).Focus();
            CancelExec();
        }
        private void BtnDel_Click(object sender, System.EventArgs e)
        {
            //((Control)sender).Focus();
            DelExec();
        }
        private void BtnDelete_Click(object sender, System.EventArgs e)
        {
            ((Control)sender).Focus();
            DeleteExec();
        }
        private void BtnInsert_Click(object sender, System.EventArgs e)
        {
            //((Control)sender).Focus();
            SaveExec2();
        }
        private void BtnHelp_Click(object sender, System.EventArgs e)
        {
            //((Control)sender).Focus();
            HelpExec();
        }
        private void BtnExcel_Click(object sender, System.EventArgs e)
        {
            //((Control)sender).Focus();
            ExcelExec();
        }
        private void BtnClose_Click(object sender, System.EventArgs e)
        {
            //((Control)sender).Focus();
            this.Close();
        }
        #endregion

        #region MouseUpDown 이벤트

        private void BtnNew_MouseLeave(object sender, System.EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\New.gif");
            BtnNew.Image = bitMap;
        }
        private void BtnNew_MouseEnter(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uNew.gif");
            BtnNew.Image = bitMap;
        }

        private void BtnSearch_MouseLeave(object sender, System.EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Search.gif");
            BtnSearch.Image = bitMap;
        }
        private void BtnSearch_MouseEnter(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uSearch.gif");
            BtnSearch.Image = bitMap;
        }

        private void BtnRCopy_MouseLeave(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\RCopy.gif");
            BtnRCopy.Image = bitMap;
        }
        private void BtnRCopy_MouseEnter(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uRCopy.gif");
            BtnRCopy.Image = bitMap;
        }

        private void BtnRowIns_MouseLeave(object sender, System.EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\RAdd.gif");
            BtnRowIns.Image = bitMap;
        } 
        private void BtnRowIns_MouseEnter(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uRAdd.gif");
            BtnRowIns.Image = bitMap;
        }


        private void BtnCancel_MouseLeave(object sender, System.EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Cancel.gif");
            BtnCancel.Image = bitMap;
        }
        private void BtnCancel_MouseEnter(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uCancel.gif");
            BtnCancel.Image = bitMap;
        }


        private void BtnDel_MouseLeave(object sender, System.EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\RDelete.gif");
            BtnDel.Image = bitMap;
        }
        private void BtnDel_MouseEnter(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uRDelete.gif");
            BtnDel.Image = bitMap;
        }

        private void BtnDelete_MouseLeave(object sender, System.EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Delete.gif");
            BtnDelete.Image = bitMap;
        }
        private void BtnDelete_MouseEnter(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uDelete.gif");
            BtnDelete.Image = bitMap;
        }

        private void BtnInsert_MouseLeave(object sender, System.EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Save.gif");
            BtnInsert.Image = bitMap;
        }
        private void BtnInsert_MouseEnter(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uSave.gif");
            BtnInsert.Image = bitMap;
        }

        private void BtnExcel_MouseLeave(object sender, System.EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Excel.gif");
            BtnExcel.Image = bitMap;
        }
        private void BtnExcel_MouseEnter(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uExcel.gif");
            BtnExcel.Image = bitMap;
        }

        private void BtnPrint_MouseLeave(object sender, System.EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Print.gif");
            BtnPrint.Image = bitMap;
        }
        private void BtnPrint_MouseEnter(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uPrint.gif");
            BtnPrint.Image = bitMap;
        }

        private void BtnHelp_MouseLeave(object sender, System.EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Help.gif");
            BtnHelp.Image = bitMap;
        }

        private void BtnHelp_MouseEnter(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uHelp.gif");
            BtnHelp.Image = bitMap; 
        }


        private void BtnClose_MouseLeave(object sender, System.EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Fcls.gif");
            BtnClose.Image = bitMap;
        }
        private void BtnClose_MouseEnter(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uFcls.gif");
            BtnClose.Image = bitMap;
        }
        


        #endregion

        #region lnkJump1_Click 점프 클릭 이벤트
        private void lnkJump1_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName1.Length > 0)
                {
                    string DllName = strJumpFileName1.Substring(0, strJumpFileName1.IndexOf("."));
                    string FrmName = strJumpFileName1.Substring(strJumpFileName1.IndexOf(".") + 1, strJumpFileName1.Length - strJumpFileName1.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName)
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link1Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName1), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lnkJump2_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName2.Length > 0)
                {
                    string DllName = strJumpFileName2.Substring(0, strJumpFileName2.IndexOf("."));
                    string FrmName = strJumpFileName2.Substring(strJumpFileName2.IndexOf(".") + 1, strJumpFileName2.Length - strJumpFileName2.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName)
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link2Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName2), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void lnkJump3_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName3.Length > 0)
                {
                    string DllName = strJumpFileName3.Substring(0, strJumpFileName3.IndexOf("."));
                    string FrmName = strJumpFileName3.Substring(strJumpFileName3.IndexOf(".") + 1, strJumpFileName3.Length - strJumpFileName3.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName)
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link3Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName3), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void lnkJump4_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName4.Length > 0)
                {
                    string DllName = strJumpFileName4.Substring(0, strJumpFileName4.IndexOf("."));
                    string FrmName = strJumpFileName4.Substring(strJumpFileName4.IndexOf(".") + 1, strJumpFileName4.Length - strJumpFileName4.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName)
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link4Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName4), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lnkJump5_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName5.Length > 0)
                {
                    string DllName = strJumpFileName5.Substring(0, strJumpFileName5.IndexOf("."));
                    string FrmName = strJumpFileName5.Substring(strJumpFileName5.IndexOf(".") + 1, strJumpFileName5.Length - strJumpFileName5.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName)
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link5Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName5), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lnkJump6_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName6.Length > 0)
                {
                    string DllName = strJumpFileName6.Substring(0, strJumpFileName6.IndexOf("."));
                    string FrmName = strJumpFileName6.Substring(strJumpFileName6.IndexOf(".") + 1, strJumpFileName6.Length - strJumpFileName6.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName)
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link6Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName6), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region lnkResize 점프 Drawing 재정렬
        /**********************************************************
		 * 사용법 :	lnkJump1.Text = "메뉴정보 등록";  //링크명
		 *          strJumpFileName1 = "PROD.PSA003"; //호출할 화면명
		 * ********************************************************/
        public void lnkResize()
        {
            //int Jump1Drawing = panButton6.Width - lnkJump1.Width - 10;
            //int Jump2Drawing = Jump1Drawing - lnkJump2.Width - 10;
            //int Jump3Drawing = Jump2Drawing - lnkJump3.Width - 10;
            //int Jump4Drawing = Jump3Drawing - lnkJump4.Width - 10;
            //int Jump5Drawing = Jump4Drawing - lnkJump5.Width - 10;
            //int Jump6Drawing = Jump5Drawing - lnkJump6.Width - 10;

            //lnkJump1.Location = new System.Drawing.Point(Jump1Drawing, 8);
            //lnkJump2.Location = new System.Drawing.Point(Jump2Drawing, 8);
            //lnkJump3.Location = new System.Drawing.Point(Jump3Drawing, 8);
            //lnkJump4.Location = new System.Drawing.Point(Jump4Drawing, 8);
            //lnkJump5.Location = new System.Drawing.Point(Jump5Drawing, 8);
            //lnkJump6.Location = new System.Drawing.Point(Jump6Drawing, 8);
        }

        private void lnkJump1_SizeChanged(object sender, System.EventArgs e)
        {
            lnkResize();
        }

        private void lnkJump2_SizeChanged(object sender, System.EventArgs e)
        {
            lnkResize();
        }

        private void lnkJump3_SizeChanged(object sender, System.EventArgs e)
        {
            lnkResize();
        }

        private void lnkJump4_SizeChanged(object sender, System.EventArgs e)
        {
            lnkResize();
        }

        private void lnkJump5_SizeChanged(object sender, System.EventArgs e)
        {
            lnkResize();
        }

        private void lnkJump6_SizeChanged(object sender, System.EventArgs e)
        {
            lnkResize();
        }
        #endregion

        private void Buttons_Activated(object sender, System.EventArgs e)
        {
            SystemBase.Base.RodeFormName = this.Name;
        }

        private void Buttons_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.N) && BtnNew.Enabled == true)
            {
                //((Control)sender).Focus();
                NewExec();
            }
            else if ((e.Control && e.KeyCode == Keys.P) && BtnPrint.Enabled == true)
            {
                //((Control)sender).Focus();
                PrintExec();
            }
            else if ((e.Control && e.KeyCode == Keys.Enter) && BtnSearch.Enabled == true)
            {
                //((Control)sender).Focus();
                BtnSearch.Focus();
                SearchExec();
            }
            else if ((e.Control && e.KeyCode == Keys.S) && BtnInsert.Enabled == true)
            {
                //((Control)sender).Focus();
                SaveExec2();
            }
            else if ((e.Control && e.KeyCode == Keys.E) && BtnClose.Enabled == true)
            {
                //((Control)sender).Focus();
                this.Close();
            }
        }

        private void panButton1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblFormName_Click(object sender, EventArgs e)
        {

        }























    }
}
