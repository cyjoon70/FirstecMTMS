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

namespace PB.PSA010
{
    public partial class PSA010P1 : Form
    {
        string strSCHST_DT = "";
        string strSCHST_TM = "";
        string strPLANT_CD = "";
        string strPTF = "";
        string strPROJECT_NO = "";
        string strPROJECT_SEQ = "";
        string strSCH_MEMO = "";
        string strMEMO = "";
        int intWORK_TYPE = 0;
        Thread th;

        FarPoint.Win.Spread.FpSpread fpSpread1;

        public PSA010P1(FarPoint.Win.Spread.FpSpread fpSpread2, string PLANT_CD, string SCHST_DT,
                        string SCHST_TM, string PTF, string PROJECT_NO, string PROJECT_SEQ, int WORK_TYPE,
                        string SCH_MEMO, string MEMO)
        {
            InitializeComponent();
            fpSpread1 = fpSpread2;
            strSCHST_DT = SCHST_DT;
            strSCHST_TM = SCHST_TM;
            strPLANT_CD = PLANT_CD;
            strPTF = PTF;
            strPROJECT_NO = PROJECT_NO;
            strPROJECT_SEQ = PROJECT_SEQ;
            intWORK_TYPE = WORK_TYPE;
            strSCH_MEMO = SCH_MEMO;
            strMEMO = MEMO;
        }

        private void PSA010P1_Load(object sender, EventArgs e)
        {
            try
            {
                CheckForIllegalCrossThreadCalls = false;

                if (intWORK_TYPE == PSA010.WORK_TYPE_BOM_DEPLOY)
                    th = new Thread(new ThreadStart(BomDeploy));
                else
                    th = new Thread(new ThreadStart(SchStart));
                th.Start();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("PSA010.SCHEDULE.Scheduld() ", f.ToString());
                SystemBase.MessageBoxComm.Show(f.ToString());
            }
        }

        /// /////////////////////////////////////////////////////////////////
        //  스케쥴 시간 전개
        /// //////////////////////////////////////////////////////////////////
        public void SchStart()
        {
            string RtnMsg = SCHEDULE.SCH_DEPLOY(fpSpread1,
                                                progressBar1,
                                                progressBar2,
                                                label1,
                                                label2,
                                                strSCHST_DT,
                                                strSCHST_TM,
                                                strPLANT_CD,
                                                strPTF,
                                                strPROJECT_NO,
                                                strPROJECT_SEQ,
                                                strSCH_MEMO,
                                                strMEMO);


            MessageBox.Show(RtnMsg);

            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        /// /////////////////////////////////////////////////////////////////
        //  스케쥴 BOM 전개
        /// //////////////////////////////////////////////////////////////////
        public void BomDeploy()
        {
            string RtnMsg = SCHEDULE.BOM_DEPLOY(fpSpread1,
                                                progressBar1,
                                                progressBar2,
                                                label1,
                                                label2,
                                                strSCHST_DT,
                                                strSCHST_TM,
                                                strPLANT_CD,
                                                strPTF,
                                                strPROJECT_NO,
                                                strPROJECT_SEQ);

            MessageBox.Show(SystemBase.Base.MessageRtn(RtnMsg), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information); 

            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void PSA010P1_FormClosed(object sender, FormClosedEventArgs e)
        {
            th.Abort();
        }
    }
}
