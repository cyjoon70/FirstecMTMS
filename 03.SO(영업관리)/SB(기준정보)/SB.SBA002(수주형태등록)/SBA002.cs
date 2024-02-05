#region 작성정보
/*********************************************************************/
// 단위업무명 : 수주형태등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-01-31
// 작성내용 : 수주형태등록 및 관리
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

namespace SB.SBA002
{
    public partial class SBA002 : UIForm.FPCOMM1
    {
        #region 생성자
        public SBA002()
        {
            InitializeComponent();
        }
        #endregion

        #region 수주형태 팝업
        private void btnSoType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'SO_TYPE', @pSPEC2 = 'SO_TYPE_NM', @pSPEC3 = 'S_SO_TYPE', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSoType.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00009", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수주형태조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSoType.Text = Msgs[0].ToString();
                    txtSoTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "팝업 호출"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Form Load 시
        private void BBD001_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 콤보박스 세팅
            //출하형태는 추후 예정
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "출하형태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='TABLE', @pCODE = 'MOVE_TYPE', @pNAME = 'MOVE_TYPE_NM', @pSPEC1 = 'I_MOVE_TYPE' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//출하형태
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "매출채권형태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='TABLE', @pCODE = 'BN_TYPE', @pNAME = 'BN_TYPE_NM', @pSPEC1 = 'S_BN_TYPE' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//매출채권형태

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

            txtSoType.Focus();
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            UIForm.FPMake.RowInsert(fpSpread1);

            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "사용여부")].Text = "True";
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;
            rdoAll.Checked = true;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            string strUseYn = "";
            if (rdoYes.Checked == true) { strUseYn = "Y"; }
            else if (rdoNo.Checked == true) { strUseYn = "N"; }
            else { strUseYn = ""; }

            try
            {
                if(SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_SBA002  @pTYPE = 'S1'";
                    strQuery += ", @pSO_TYPE = '" + txtSoType.Text + "' ";
                    strQuery += ", @pUSE_YN = '" + strUseYn + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            //수출여부가 Y 이면 반품여부 Enable
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수출여부")].Text == "True")
                            { UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "반품여부") + "|3"); }
                            else
                            { UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "반품여부") + "|0"); }

                            //통관여부가 Y 이면 출하여부 Enable
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관여부")].Text == "True")
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "출하여부") + "|3");
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "출하형태") + "|3");
                            }
                            else
                            { UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "출하여부") + "|0"); }

                            //출하여부가 Y 이면 출하형태 Enable
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출하여부")].Text == "True")
                            { UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "출하형태") + "|1"); }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출하형태")].Text = "";
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "출하형태") + "|3");
                            }

                            //매출여부가 Y 이면 매출채권형태 Enable
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출여부")].Text == "True")
                            { UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출채권형태") + "|1"); }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출채권형태")].Text = "";
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출채권형태") + "|3");
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            //그리드 상단 필수 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                string soType = "";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;
                                case "I": strGbn = "I1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            soType = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주형태")].Text;

                            string strExYn = "N";//수출여부
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수출여부")].Text == "True") { strExYn = "Y"; }
                            string strReYn = "N";//반품여부
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반품여부")].Text == "True") { strReYn = "Y"; }
                            string strCcYn = "N";//통관여부;
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관여부")].Text == "True") { strCcYn = "Y"; }
                            string strDnYn = "N";//출하여부
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출하여부")].Text == "True") { strDnYn = "Y"; }
                            string strBnYn = "N";//매출여부
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출여부")].Text == "True") { strBnYn = "Y"; }
                            string strUseYn = "N";//사용여부
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용여부")].Text == "True") { strUseYn = "Y"; }

                            string strSql = " usp_SBA002 '" + strGbn + "'";
                            strSql += ", @pSO_TYPE = '" + soType + "'";
                            strSql += ", @pSO_TYPE_NM = N'" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주형태명")].Text + "'";
                            strSql += ", @pEX_YN = '" + strExYn + "'";
                            strSql += ", @pRE_YN = '" + strReYn + "'";
                            strSql += ", @pCC_YN = '" + strCcYn + "'";
                            strSql += ", @pDN_YN = '" + strDnYn + "'";
                            strSql += ", @pBN_YN = '" + strBnYn + "'";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출하형태")].Text != "")
                                strSql += ", @pMOVE_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출하형태")].Value.ToString() + "'";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출채권형태")].Text != "")
                                strSql += ", @pBN_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출채권형태")].Value.ToString() + "'";
                            strSql += ", @pUSE_YN = '" + strUseYn + "'";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        }
                    }
                    Trans.Commit();
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, soType, SystemBase.Base.GridHeadIndex(GHIdx1, "수주형태")); //저장 후 그리드 포커스 이동
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 출하여부, 매출여부 클릭시 출하형태, 매출채권형태 상태값 변경
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "출하여부"))
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출하여부")].Text == "True")
                { UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출하형태") + "|1"); } //필수
                else
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출하형태")].Text = "";
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출하형태") + "|3"); //읽기전용
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "매출여부"))
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출여부")].Text == "True")
                { UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출채권형태") + "|1"); }
                else
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출채권형태")].Text = "";
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출채권형태") + "|3");
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "수출여부"))
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수출여부")].Text == "True")
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반품여부")].Text = "False";
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반품여부") + "|3");
                }
                else
                {
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반품여부") + "|0");
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "통관여부"))
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "통관여부")].Text == "True")
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출하여부")].Text = "False";
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출하여부") + "|3");
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출하형태")].Text = "";
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출하형태") + "|3");
                }
                else
                {
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출하여부") + "|0");
                }
            }
        }
        #endregion

        #region 수주형태코드 입력시 수주형태명 변환
        private void txtSoType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSoType.Text != "")
                {
                    txtSoTypeNm.Value = SystemBase.Base.CodeName("SO_TYPE", "SO_TYPE_NM", "S_SO_TYPE", txtSoType.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSoTypeNm.Value = "";
                }
            }
            catch { }
        }
        #endregion
    }
}
