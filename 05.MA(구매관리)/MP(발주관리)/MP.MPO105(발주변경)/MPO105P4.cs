#region 작성정보
/*********************************************************************/
// 단위업무명 : 발주변경
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-01
// 작성내용 : 발주변경 및 관리
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

namespace MP.MPO105
{
    public partial class MPO105P4 : UIForm.FPCOMM1
    {
        #region 변수선언
        string returnVal;
        string strPoNo;
        FarPoint.Win.Spread.FpSpread spd;
        #endregion

        #region 생성자
        public MPO105P4()
        {
            InitializeComponent();
        }

        public MPO105P4(FarPoint.Win.Spread.FpSpread spread, string PoNo)
        {
            InitializeComponent();
            spd = spread;
            strPoNo = PoNo;
        }
        #endregion

        #region 폼로드 이벤트
        private void MPO105P4_Load(object sender, EventArgs e)
        {
            this.Text = "품질증빙 일괄적용";
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            SearchExec(); 
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = "usp_MPO105  @pTYPE = 'P4'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion
        
        #region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            string strTemp = "";

            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value.ToString() == "1")
                {
                    strTemp += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "증빙코드")].Text;
                }
            }


            if (strTemp != "")
            {
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
                    for (int i = 0; i < spd.Sheets[0].Rows.Count; i++)
                    {
                        if (spd.Sheets[0].Cells[i, 2].Text == "True")
                        {
                            string strSql = "usp_MPO105 'U3' ";
                            strSql += ", @pPO_NO = '" + strPoNo + "' ";
                            strSql += ", @pPO_SEQ = '" + spd.Sheets[0].Cells[i, 1].Text + "' ";
                            strSql += ", @pQUALITY_PROOF = '" + strTemp + "' ";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        }
                    }
                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                    //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();
                this.Cursor = Cursors.Default;
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0042"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RtnStr("Y");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            RtnStr("N");
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region 값 전송
        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(string strCode)
        {
            returnVal = strCode;
        }
        #endregion
        
        #region fpSpread1_ButtonClicked
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }
        #endregion

    }
}
