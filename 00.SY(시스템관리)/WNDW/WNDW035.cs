#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 공정내용저장 추가 삭제 조회
// 작 성 자   : 김한진
// 작 성 일   : 2014-08-27
// 작성내용   : 공정내용
// 수 정 일   :
// 수 정 자   :
// 수정내용   :
// 비    고   :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

#region 예제 - 복사해서 쓰세요
/*
try
{
    WNDW.WNDW035 pu = new WNDW.WNDW035();
    pu.ShowDialog();
    if (pu.DialogResult == DialogResult.OK)
    {
        string[] Msgs = pu.ReturnVal;

        textBox1.Text = Msgs[1].ToString();
        textBox2.Value = Msgs[2].ToString();
    }
}
catch (Exception f)
{
    SystemBase.Loggers.Log(this.Name, f.ToString());
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 제조오더정보조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 제조오더번호 </para>
    /// <para>Msgs[2] = 제품오더번호 </para>
    /// <para>Msgs[3] = 프로젝트번호 </para>
    /// <para>Msgs[4] = 프로젝트명 </para>
    /// <para>Msgs[5] = 프로젝트차수 </para>
    /// <para>Msgs[6] = 품목코드 </para>
    /// <para>Msgs[7] = 품목명 </para>
    /// </summary>

    public partial class WNDW035 : UIForm.Buttons
    {
        #region 변수선언
        string strItemCd = "";
        string strRoutNo = "";
        string strProcSeq = "";
        string strProcPlanCd = "";
        string strProcPlanNm = "";
        string strJobCd = "";
        string strJobNm = "";
        string strGbn = "";
        string set_Save = "";
        #endregion

        #region WNDW035 생성자
        public WNDW035(string set_U, string Item_CD, string Rout_NO, string Proc_SEQ, string Proc_PLANCD, string Proc_PlANNM, string Job_CD, string Job_NM, string PROC_PLAN)
        {
            set_Save = set_U;
            strGbn = PROC_PLAN;
            strItemCd = Item_CD;
            strRoutNo = Rout_NO;
            strProcSeq = Proc_SEQ;
            strProcPlanCd = Proc_PLANCD;
            strProcPlanNm = Proc_PlANNM;
            strJobCd = Job_CD;
            strJobNm = Job_NM;

            InitializeComponent();
        }

        public WNDW035()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW035_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용
            if (set_Save == "U")
                UIForm.Buttons.ReButton(BtnInsert, "BtnInsert", true);
            else
                UIForm.Buttons.ReButton(BtnInsert, "BtnInsert", false);

            UIForm.Buttons.ReButton(BtnNew, "BtnNew", false);
            UIForm.Buttons.ReButton(BtnSearch, "BtnSearch", false);
            UIForm.Buttons.ReButton(BtnRCopy, "BtnRCopy", false);
            UIForm.Buttons.ReButton(BtnRowIns, "BtnRowIns", false);
            UIForm.Buttons.ReButton(BtnCancel, "BtnCancel", false);
            UIForm.Buttons.ReButton(BtnDel, "BtnDel", false);
            UIForm.Buttons.ReButton(BtnDelete, "BtnDelete", false);
            UIForm.Buttons.ReButton(BtnExcel, "BtnExcel", false);

            txtItemCd.Value = strItemCd;
            txtRout.Value = strRoutNo;
            txtProcSeq.Value = strProcSeq;
            TXTPROCPLANCD.Value = strProcPlanCd;
            txtProcPlanNM.Value = strProcPlanNm;
            TXTJOB_CD.Value = strJobCd;
            txtJob_NM.Value = strJobNm;

            //데이터가있을때 보여주기
            if (strGbn == "U1")
            { 
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);


            string strSql = " usp_WNDW035 '" + "S1" + "'";
            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            strSql += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD.ToString() + "'";
            strSql += ", @pITEM_CD = '" + strItemCd + "'";
            strSql += ", @pROUT_NO = '" + strRoutNo + "'";
            strSql += ", @pPROC_SEQ = '" + strProcSeq + "'";

            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
            c1TextBox8.Text = ds.Tables[0].Rows[0]["PROCESS_PLAN"].ToString();
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직

        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;
            string RPLMsg = "";
            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_WNDW035 '"  + strGbn + "'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                strSql += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD.ToString() + "'";
                strSql += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                strSql += ", @pROUT_NO = '" + txtRout.Text + "'";
                strSql += ", @pPROC_SEQ = '" + txtProcSeq.Text + "'";
                strSql += ", @pPROCESS_PLAN = '" + c1TextBox8.Text + "'";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback();}	// ER 코드 Return시 점프
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
            Trans.Commit();
            if (strGbn == "I1")
                strGbn = "U1";

            if (ERRCode == "OK")
            {
                if (RPLMsg != "")
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode, RPLMsg), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (ERRCode == "ER")
            {
                if (RPLMsg != "")
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode, RPLMsg), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (RPLMsg != "")
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode, RPLMsg), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            
            
            this.Cursor = Cursors.Default;
            this.Close();
        }
        #endregion

        #region TextBox코드입력시 코드명 자동입력
        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                    txtProcPlanNM.Value = "";
                    txtJob_NM.Value = "";
                }
            }
            catch { }
        }
        #endregion


        #region 버튼클릭

        private void btnItem_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW005 pu = new WNDW.WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[1].ToString();
                    txtItemNm.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장별품목정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnProc_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region 그리드 선택값 입력밑 전송

        public string ReturnData()
        {
            if (c1TextBox8.Text == "")
            {
                            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);


            string strSql = " usp_WNDW035 '" + "D1" + "'";
            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            strSql += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD.ToString() + "'";
            strSql += ", @pITEM_CD = '" + strItemCd + "'";
            strSql += ", @pROUT_NO = '" + strRoutNo + "'";
            strSql += ", @pPROC_SEQ = '" + strProcSeq + "'";

            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
            Trans.Commit();
            }
            return c1TextBox8.Text;
        }

        #endregion
    }
}
