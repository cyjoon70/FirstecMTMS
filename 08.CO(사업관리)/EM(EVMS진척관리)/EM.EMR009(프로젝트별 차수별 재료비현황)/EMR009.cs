﻿#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트별 차수별 재료비현황
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-23
// 작성내용 : 프로젝트별 차수별 재료비현황
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
using System.Data.SqlClient;
using WNDW;

namespace EM.EMR009
{
    public partial class EMR009 : UIForm.FPCOMM2
    {
        #region 변수선언
        bool form_act_chk = false;
        int TempRow = 10000;
        #endregion

        public EMR009()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void EMR009_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용                
        }
        #endregion
        
        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;
        }
        #endregion

        #region 조회조건 팝업
        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {

                WNDW007 pu = new WNDW007(txtProjectNo.Text);
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //프로젝트차수
        private void c1Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtProjSeq.Text = Msgs[0].ToString();
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

        #region 조회조건 TextChanged
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {

                    string strQuery = " usp_EMR009 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pPROJECT_NO ='" + txtProjectNo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ ='" + txtProjSeq.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                    fpSpread1.Sheets[0].RowCount = 0;
                    TempRow = 10000;

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (TempRow != e.Row && e.ColumnHeader != true) Detail_Search(e.Row);
        }

        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (TempRow != e.NewRow) Detail_Search(e.NewRow);
        }

        private void Detail_Search(int Row)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strQuery = " usp_EMR009 'S2'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pPROJECT_NO='" + fpSpread2.Sheets[0].Cells[Row, 1].Text + "'";
                strQuery += ", @pPROJECT_SEQ ='" + fpSpread2.Sheets[0].Cells[Row, 3].Text + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 1, true);
                TempRow = Row;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }
        #endregion        

        private void EMR009_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtProjectNo.Focus();
        }

        private void EMR009_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }

      
       
    }
}
