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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace BB.BZG001
{
    public partial class BZG001 : UIForm.FPCOMM1
    {
        #region 생성자
        public BZG001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BZG001_Load(object sender, System.EventArgs e)
        {
            linkLabel1.Text = "공지사항조회";  //링크명
            strJumpFileName1 = "BB.BZG002.BZG002"; //호출할 화면명

            linkLabel2.Text = "FT 지식공유";  //링크명
            strJumpFileName2 = "BB.BZG003.BZG003"; //호출할 화면명
            
            SelectExec(false);     
        }
        #endregion
        
        #region SelectExec() 그리드 조회 로직
        private void SelectExec(bool Msg)
        {
            try
            {
                string strQuery = "";
                strQuery = " USP_BZG001 @pTYPE = 'S1' ";
                strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
                strQuery = strQuery + ", @pTITLE ='" + txtTitle.Text.ToString().Trim() + "' ";
                strQuery = strQuery + ", @pCONTENT ='" + txtContent.Text.ToString().Trim() + "' ";
                strQuery = strQuery + ", @pIDX ='" + "" + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, Msg);

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region fpSpread1_CellDoubleClick
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {

            HitUpdate(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Text);
            SelectExec(false);
            BZG001P1 myForm = new BZG001P1("R", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Text);
            myForm.ShowDialog();
            SelectExec(false);

        }

        private void HitUpdate(string idx)
        {
            string strQuery = "";
            strQuery = " USP_BZG001 @pTYPE = 'H1' ";
            strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
            strQuery = strQuery + ", @pIDX = " + idx + "";
            strQuery = strQuery + ", @pUSR_ID = '" + SystemBase.Base.gstrUserID + "'";
            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            SystemBase.DbOpen.NoTranDataTable(strQuery);
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SearchExec() -- 검색
        protected override void SearchExec()
        {
            SelectExec(true);
        }
        #endregion

        #region RowInsExec() -- 등록
        protected override void RowInsExec()
        { 
            BZG001P1 myForm = new BZG001P1("W", "0");
            myForm.ShowDialog();
            SelectExec(false);
        }
        #endregion

        #region BZG001_Activated
        private void BZG001_Activated(object sender, System.EventArgs e)
        {
            SelectExec(false);
        }
        #endregion

        #region lnkJump_Click 점프 클릭 이벤트
        private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            if (strJumpFileName1.Length > 0)
            {
                string DllName = strJumpFileName1.Substring(0, strJumpFileName1.IndexOf("."));
                string FrmName = strJumpFileName1.Substring(strJumpFileName1.IndexOf(".") + 1, strJumpFileName1.Length - strJumpFileName1.IndexOf(".") - 1);

                for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                {	// 폼이 이미 열려있으면 닫기
                    if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                    {
                        MdiParent.MdiChildren[k].BringToFront(); //화면을 앞으로 가져오고.. 
                        MdiParent.MdiChildren[k].Close();
                        break;
                    }
                }
                Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName1));
                myForm.MdiParent = this.MdiParent;
                SystemBase.Base.RodeFormID = "BZG002";
                SystemBase.Base.RodeFormText = "공지사항조회";
                myForm.Show();
            }
        }       

        private void linkLabel2_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            if (strJumpFileName2.Length > 0)
            {
                string DllName = strJumpFileName2.Substring(0, strJumpFileName2.IndexOf("."));
                string FrmName = strJumpFileName2.Substring(strJumpFileName2.IndexOf(".") + 1, strJumpFileName2.Length - strJumpFileName2.IndexOf(".") - 1);

                for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                {	// 폼이 이미 열려있으면 닫기
                    if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                    {
                        MdiParent.MdiChildren[k].BringToFront(); //화면을 앞으로 가져오고.. 
                        MdiParent.MdiChildren[k].Close();
                        break;
                    }
                }
                Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName2));
                myForm.MdiParent = this.MdiParent;
                SystemBase.Base.RodeFormID = "BZG003";
                SystemBase.Base.RodeFormText = "FT 지식공유";
                myForm.Show();
            }
        }
        #endregion
    }
}
