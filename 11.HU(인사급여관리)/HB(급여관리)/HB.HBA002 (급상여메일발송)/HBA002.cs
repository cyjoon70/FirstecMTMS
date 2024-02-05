#region 작성정보
/*********************************************************************/
// 단위업무명 : 급상여메일발송
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-11
// 작성내용 : 급상여메일발송 
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

namespace HB.HBA002
{
    public partial class HBA002 : UIForm.FPCOMM1
    {
        public HBA002()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void HBA002_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0,7);
            SystemBase.ComboMake.C1Combo(cboProvType, "usp_H_COMMON @pTYPE = 'H007', @pCOM_CD = 'H0040'", 0);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0,0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
            SystemBase.ComboMake.C1Combo(cboProvType, "usp_H_COMMON @pTYPE = 'H007', @pCOM_CD = 'H0040'", 0);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_HBA002  @pTYPE = 'S1' ";
                strQuery = strQuery + " , @pPROV_TYPE = '" + cboProvType.SelectedValue.ToString() + "' ";
                strQuery = strQuery + " , @pPAY_YYMM = '" + dtpDate.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0); 
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.

            try
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                    {
                        string strQuery = " usp_HBA002  @pTYPE = 'S2' ";
                        strQuery = strQuery + " , @pPROV_TYPE = '" + cboProvType.SelectedValue.ToString() + "' ";
                        strQuery = strQuery + " , @pPAY_YYMM = '" + dtpDate.Text + "' ";
                        strQuery = strQuery + " , @pEMP_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text + "' ";

                        DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

                        //에러코드가 없으면 "현재 명령에서 서버 오류가 발생했습니다. 결과가 있을 경우 이를 무시해야 합니다." 예외로 보고 무시한다.
                        if (ds.Tables[0].Rows[0][0].ToString() == "")
                        {
                            ERRCode = "OK";
                            MSGCode = "메일이 발송되었습니다.";
                        }
                        else
                        {
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();
                        }

                        if (ERRCode != "OK" && ERRCode != "") { goto Exit; }	// ER 코드 Return시 점프
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                ERRCode = "OK";
                MSGCode = "메일이 발송되었습니다.";	//에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:

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

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

    }
}
