#region 작성정보
/*********************************************************************/
// 단위업무명 : SCHEDULE 전개
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-28
// 작성내용 : SCHEDULE 전개 및 관리
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
using System.Threading;
using System.Data.SqlClient;

namespace PC.PUA101
{
    public partial class PUA101P1 : Form
    {
        #region 변수선언
        Thread th;
        bool stopFlg = false;
        public string EMG_SCH_ID = "";
        public static string PROC_STATUS_CAL_RESO_TIME = "CT";
        #endregion

        #region 생성자
        public PUA101P1(string SCH_ID)
        {

            InitializeComponent();
            EMG_SCH_ID = SCH_ID;

        }
        #endregion

        #region PUA010P1 Load
        private void PUA010P1_Load(object sender, System.EventArgs e)
        {
            try
            {
                CheckForIllegalCrossThreadCalls = false;

                th = new Thread(new ThreadStart(SchStart));
                th.Start();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("PUA010.SCHEDULE.Scheduld() ", f.ToString());
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 긴급작지 처리
        public void SchStart()
        {
            string ERRCode = "OK";
            string MSGCode = "P0010";

            string SchDplNm = SCH_PROG.HasDplySch();

            if (SchDplNm != null)
            {
                SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn("P0038", SchDplNm));
                goto EXIT_FUNC;
            }

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            cmd.Transaction = Trans;
            cmd.CommandTimeout = 10000;

            try
            {
                /////////////////////// 진행중인 스케쥴 정보 등록 ///////////////////
                SCH_PROG.InsSchProc(PUA101.PROC_TYPE, PROC_STATUS_CAL_RESO_TIME, cmd);
                /////////////////////// 진행중인 스케쥴 정보 등록 ///////////////////

                /////////////////////// RESO_WORK_TIME_TEMP 삭제(배정정보삭제)///////
                SCH_PROG.DelResoWorkTime(cmd);
                /////////////////////// RESO_WORK_TIME_TEMP 삭제(배정정보삭제)///////

                ////////////////////////////// MBOP 데이터 생성 ////////////////////////////////////////
                MBOPCOPY.MBOP_COPY(progressBar1, label1, EMG_SCH_ID, cmd, dbConn, Trans);
                ////////////////////////////// MBOP 데이터 생성 ////////////////////////////////////////

                ////////////////////////////// 스케쥴 전개 ////////////////////////////////////////
                BACKWARD.BACKWARD_EMEG(progressBar1, label1, EMG_SCH_ID, cmd, dbConn, Trans);
                ////////////////////////////// 스케쥴 전개 ////////////////////////////////////////

                /////////////////// 결과 반영 (MPS 등록, 작업지시서, 자재등록) ////////////////////
                RST_UP.SCH_RST_UP(label1, cmd, dbConn, Trans);
                /////////////////// 결과 반영 (MPS 등록, 작업지시서, 자재등록) ////////////////////

                /////////////////////// 진행중인 스케쥴 정보 삭제 ///////////////////
                SCH_PROG.DelSchProc(cmd);
                /////////////////////// 진행중인 스케쥴 정보 삭제 ///////////////////

                progressBar1.Value = 100;

                Trans.Commit();
            }
            catch (Exception f)
            {
                Trans.Rollback();

                ERRCode = "ER";
                MSGCode = f.ToString();

                SystemBase.Loggers.Log("PUA101", f.ToString());
            }
            dbConn.Close();
 
            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        EXIT_FUNC:

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion
        
        #region 닫기
        private void PUA101P1_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopFlg = true;    // 중지 시킨다.

            if (th.Join(3000))  // 5000초 동안 기다린다.
            {
                th.Abort();
            }
        }
        #endregion

    }
}
