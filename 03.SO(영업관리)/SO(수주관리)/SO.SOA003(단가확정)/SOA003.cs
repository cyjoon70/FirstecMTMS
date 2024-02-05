#region 작성정보
/*********************************************************************/
// 단위업무명 : 수주현황조회
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-03-25
// 작성내용 : 수주현황조회
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

namespace SO.SOA003
{
    public partial class SOA003 : UIForm.FPCOMM1
    {
        #region 생성자
        public SOA003()
        {
            InitializeComponent();

        }
        #endregion

        #region Form Load 시
        private void SOA003_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSSaleDuty, "usp_S_COMMON @pTYPE = 'S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //영업담당
            SystemBase.ComboMake.C1Combo(cboSSoType, "usp_B_COMMON @pTYPE = 'TABLE', @pCODE = 'SO_TYPE', @pNAME = 'SO_TYPE_NM', @pSPEC1 = 'S_SO_TYPE' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);//수주형태

            //그리드콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//화폐단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "영업담당")] = SystemBase.ComboMake.ComboOnGrid("usp_S_COMMON @pTYPE = 'S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //영업담당
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "수주형태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'TABLE', @pCODE = 'SO_TYPE', @pNAME = 'SO_TYPE_NM', @pSPEC1 = 'S_SO_TYPE' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//수주형태
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='PLANT', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

            //기타 세팅	
            dtpSSoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpSSoDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
            dtpSSoDtFr.Focus();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;
            rdoNo.Checked = true;

			dtpSSoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
			dtpSSoDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            //단가확정구분
            string strCfmYn = "";
            if (rdoYes.Checked == true) { strCfmYn = "T"; } //진단가
            else if (rdoNo.Checked == true) { strCfmYn = "F"; } //가단가
            else { strCfmYn = ""; }

            try
            {
                string strQuery = " usp_SOA003  @pTYPE = 'S1'";
                strQuery += ", @pSO_DT_FR = '" + dtpSSoDtFr.Text + "' ";
                strQuery += ", @pSO_DT_TO = '" + dtpSSoDtTo.Text + "' ";
                strQuery += ", @pSOLD_CUST = '" + txtSSoldCustCd.Text + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "' ";
                strQuery += ", @pSALE_DUTY = '" + cboSSaleDuty.SelectedValue.ToString() + "' ";
                strQuery += ", @pSO_TYPE = '" + cboSSoType.SelectedValue.ToString() + "' ";
                strQuery += ", @pPRICE_FLAG = '" + strCfmYn + "' ";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pREF_DELV_DT_FR = '" + dtpRefDelvDtFr.Text + "' ";      // 2017.11.01. hma 추가: 납기일(참조) FROM
                strQuery += ", @pREF_DELV_DT_TO = '" + dtpRefDelvDtTo.Text + "' ";      // 2017.11.01. hma 추가: 납기일(참조) TO

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
            }

            //진단가로 등록되었다면 체크박스 Disable, 가단가면 Enable, 매출등록되었다면 진단가 Disable, 아니면 Enable
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출등록여부")].Text == "Y")
                {
                    UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|3"
                                                  + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "진단가") + "|3");
                }
                else
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가구분")].Text == "T")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|3");
                    }
                }

            }

            this.Cursor = Cursors.Default;
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

                        if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                        {
                            string strCfmYn = "F";//단가확정여부
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Text == "True") { strCfmYn = "T"; }

                            string strSql = " usp_SOA003 'U1'";
                            strSql += ", @pPRICE_FLAG = '" + strCfmYn + "' ";
                            strSql += ", @pSO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text + "' ";
                            strSql += ", @pSO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text + "' ";
                            strSql += ", @pSO_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "진단가")].Value + "' ";
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
                    ERRCode = "ER";
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

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

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 거래처 팝업
        private void btnSSoldCust_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtSSoldCustCd.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSSoldCustCd.Text = Msgs[1].ToString();
                    txtSSoldCustNm.Value = Msgs[2].ToString();
                    txtSSoldCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SOA003", "주문처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 거래처코드 입력시 거래처명 변환
        private void txtSSoldCustCd_TextChanged(object sender, EventArgs e)
        {
            txtSSoldCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSSoldCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region txtItemCd_TextChanged
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목명 가져오기"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }	
        }
        #endregion

        #region btnItemCd_Click
        private void btnItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW005 pu = new WNDW.WNDW005("10");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
}
