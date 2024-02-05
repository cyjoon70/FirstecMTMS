
#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매견적의뢰
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-16
// 작성내용 : 구매견적의뢰 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;

namespace ME.MEA001
{
    public partial class MEA001 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strBtn = "N";
        bool form_act_chk = false;
        #endregion

        public MEA001()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MEA001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            SystemBase.ComboMake.C1Combo(cboItemDiv, "usp_M_COMMON @pTYPE = 'M031', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpEstDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpEstDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            dtpEstDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpEstDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            UIForm.FPMake.RowInsert(fpSpread1);
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰일자")].Text = SystemBase.Base.ServerTime("YYMMDD");
            //			fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Value = "KRW";
            Set_Insert_ReType(fpSpread1.Sheets[0].ActiveRowIndex);
        }


        private void Set_Insert_ReType(int Row)
        {
            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Value = 0;
            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처지정")].Value = "N";
            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙")].Value = "N";

            UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|3");
            UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처지정_2") + "|5");

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    string strCfmYn = "";
                    if (rdoCfmYes.Checked == true) { strCfmYn = "Y"; }
                    else if (rdoCfmNo.Checked == true) { strCfmYn = "N"; }

                    string strQuery = " usp_MEA001  @pTYPE = 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pEST_DT_FR = '" + dtpEstDtFr.Text + "' ";
                    strQuery += ", @pEST_DT_TO = '" + dtpEstDtTo.Text + "' ";
                    strQuery += ", @pDELIVERY_DT_FR= '" + dtpDeliveryDtFr.Text + "' ";
                    strQuery += ", @pDELIVERY_DT_TO = '" + dtpDeliveryDtTo.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pPUR_DUTY = '" + txtUserId.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjNo.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjSeq.Text + "' ";
                    strQuery += ", @pCONFIRM_YN = '" + strCfmYn + "' ";
                    strQuery += ", @pITEM_DIV = '" + cboItemDiv.SelectedValue + "' ";
                    strQuery += ", @pCUST_CD = '" + txtSCustCd.Text.Trim() + "' ";
                    strQuery += ", @pEST_NO = '" + txtEstNo.Text + "' ";
                    strQuery += ", @pREQ_NO = '" + txtReqNo.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                    if (fpSpread1.Sheets[0].RowCount > 0) Set_Locking();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }

        private void Set_Locking()
        {
            //Detail Locking설정
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                //확정여부에 따른 화면 Locking
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Text == "True")
                {
                    if (Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상태값")].Text) >= 2)
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "일괄선택") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰일자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출요구일자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "차수") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "차수_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품명") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "규격") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "납품요구일자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "파일견적") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "첨부파일") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                            );
                    else
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "일괄선택") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰일자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출요구일자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "차수") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "차수_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품명") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "규격") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "납품요구일자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "파일견적") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "첨부파일") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                            );

                    //					if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청번호")].Text == "")
                    //					{
                    //						if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Text != "" )
                    //							UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "첨부파일") + "|0");
                    //						else
                    //							UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "첨부파일") + "|5");
                    //					}
                    //					else
                    //						UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "첨부파일") + "|5");
                }
                else
                {
                    //Detail Locking해제
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text == "*")
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "일괄선택") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰일자") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출요구일자") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "차수") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "차수_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품명") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "규격") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "납품요구일자") + "|1"
                            //							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "파일견적") + "|0"
                            //							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "첨부파일") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                            );

                    else
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "일괄선택") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰일자") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출요구일자") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "차수") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "차수_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품명") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "규격") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "납품요구일자") + "|1"
                            //							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "파일견적") + "|0"
                            //							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "첨부파일") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                            );

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청번호")].Text == "")
                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일견적") + "|0"
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "첨부파일") + "|0");
                    else
                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일견적") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "첨부파일") + "|0");
                }

            }


        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;
            DialogResult dsMsg;

            // 그리드 상단 필수항목 체크
            if (UIForm.FPMake.FPUpCheck(fpSpread1) == true)
            {
                string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
                string strEstNo = "";
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


                            string strCfmYn = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Text == "True") { strCfmYn = "Y"; }

                            //파일견적을체크했는데 파일이 없으면 안됨
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일견적")].Text == "True")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Text != "")
                                {
                                    string Query = "usp_MEA001 @pTYPE = 'S2'";
                                    Query += ", @pFILES_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Text + "'";
                                    Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                                    if (dt.Rows.Count <= 0)
                                    {
                                        dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("첨부파일을 입력하세요!"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        this.Cursor = Cursors.Default;
                                        return;
                                    }
                                }
                                else
                                {
                                    dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("첨부파일을 입력하세요!"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    this.Cursor = Cursors.Default;
                                    return;
                                }
                            }

                            string strFileYn = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일견적")].Text == "True") { strFileYn = "Y"; }

                            if (strGbn == "U1" && strCfmYn == "Y" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처지정")].Text == "N")
                            {
                                dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0002"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.Cursor = Cursors.Default;
                                return;
                            }

                            string strSql = " usp_MEA001 '" + strGbn + "'";
                            strSql += ", @pCONFIRM_YN = '" + strCfmYn + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰번호")].Text == "")
                                strSql += ", @pEST_NO = '" + strEstNo + "'";
                            else
                                strSql += ", @pEST_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰번호")].Text + "'";

                            strSql += ", @pEST_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적순번")].Text + "'";
                            strSql += ", @pEST_REQ_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰일자")].Text + "'";


                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출요구일자")].Text.Trim() != "")
                                strSql += ", @pEST_FILING_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출요구일자")].Text + "'";

                            strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "'";
                            strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "'";
                            strSql += ", @pPUR_DUTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자")].Text + "'";
                            strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "'";
                            strSql += ", @pITEM_NM= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text + "'";
                            strSql += ", @pITEM_SPEC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text + "'";
                            strSql += ", @pREQ_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청번호")].Text + "'";

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text.Trim() != "")
                                strSql += ", @pREQ_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text + "'";

                            strSql += ", @pEST_REQ_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")].Text + "'";
                            strSql += ", @pEST_REQ_QTY = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value;
                            strSql += ", @pEST_REQ_DELIVERY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납품요구일자")].Text + "'";
                            strSql += ", @pEST_QUALITY_PROOF = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙코드")].Text + "'";
                            strSql += ", @pFILE_EST_YN = '" + strFileYn + "'";
                            strSql += ", @pFILES_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Text + "'";
                            strSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();
                            strEstNo = ds.Tables[0].Rows[0][2].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); strEstNo = ""; goto Exit; }	// ER 코드 Return시 점프						
                        }
                    }
                    Trans.Commit();
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = e.Message;
                    //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
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

                //				//기존 그리드 위치를 가져온다
                //				int chkRow = 0;
                //				if(fpSpread1.Sheets[0].Rows.Count > 0)
                //				{chkRow = fpSpread1.Sheets[0].ActiveRowIndex;}
                //
                //				if(ERRCode != "ER")
                //					SearchExec();
                //
                //				//조회후 기존 그리드 위치로 이동
                //				fpSpread1.ActiveSheet.SetActiveCell(chkRow, 1);
                //				fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 버튼 클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            strBtn = "Y";
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "거래처지정_2") && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰번호")].Text != "")
            {
                try
                {
                    bool locking = false;
                    if (Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "상태값")].Text) >= 2
                        || fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "확정값")].Text == "Y")
                        locking = true;

                    MEA001P2 frm2 = new MEA001P2(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰번호")].Text,
                                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "견적순번")].Text,
                                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처지정")].Text, locking);
                    frm2.ShowDialog();
                    if (frm2.DialogResult == DialogResult.OK)
                    {
                        string Msgs = frm2.ReturnVal;
                        if (Msgs == "") fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처지정")].Text = "N";
                        else if (Msgs != "N") fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처지정")].Text = Msgs;


                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //프로젝트번호
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2"))
            {
                try
                {

                    WNDW007 pu = new WNDW007(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text, "N");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = "";
                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            //프로젝트번호차수
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "차수_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                    string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                    //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                    pu.Width = 400;
                    pu.ShowDialog();	//공통 팝업 호출

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string MSG = pu.ReturnVal.Replace("|", "#");
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(MSG);

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = Msgs[0].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자_2"))
            {
                string strQuery = "usp_M_COMMON 'M011', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04009", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매담당자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자")].Text = Msgs[0].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자명")].Text = Msgs[1].ToString();
                }

            }

            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2"))
            {
                try
                {
                    WNDW005 pu = new WNDW005(SystemBase.Base.gstrPLANT_CD, "30", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[7].ToString();

                        string Query = "Select ISNULL(ORDER_PUR_UNIT,'') ";
                        Query += " From B_PLANT_ITEM_INFO(Nolock) Where  ITEM_CD  = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                        Query += " AND PLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Rows[0][0].ToString().Trim() == "") fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")].Text = Msgs[8].ToString();
                            else fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")].Text = dt.Rows[0][0].ToString();
                        }

                    }

                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text != "*") Set_Item_grdReMake(Row, "3");
                    else Set_Item_grdReMake(Row, "0");

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위_2"))
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'Z005', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00029", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "단위팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")].Text = Msgs[0].ToString();

                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙_2"))
            {
                try
                {
                    bool locking = false;
                    bool saved = false;

                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "상태값")].Text != "")
                    {
                        if (Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "상태값")].Text) >= 2
                            || fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "확정값")].Text == "Y")
                            locking = true;
                    }

                    if (fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text == "I") saved = false;
                    else saved = true;

                    MEA001P3 frm3 = new MEA001P3(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰번호")].Text,
                                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "견적순번")].Text, locking, saved);
                    frm3.ShowDialog();
                    if (frm3.DialogResult == DialogResult.OK && locking == false)
                    {
                        string Msgs = frm3.ReturnVal;
                        string Val = frm3.ReturnStr;
                        if (Val != fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙코드")].Text && Msgs == "Y")
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙")].Text = "Y";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙코드")].Text = Val;
                        }
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "파일견적"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, Column].Text == "True")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = "*";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = "견적파일첨부";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value = 0.00;
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value = 0.00;
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "첨부파일"))
            {
                string updndl;
                //확정여부에 따른 화면 Locking
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Text == "True") updndl = "N#N#N";
                else updndl = "Y#Y#Y";

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Text == "")
                {
                    string Query = "usp_MEA001 'C1'  ";
                    Query += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                    Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    if (dt.Rows.Count > 0)
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Text = dt.Rows[0][0].ToString();
                    }
                }
                UIForm.FileUpDown frm = new UIForm.FileUpDown(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Text, updndl);
                frm.ShowDialog();

                //파일이 등록되면 파일번호를 저장

                string Query1 = "usp_MEA001 @pTYPE = 'S2'";
                Query1 += ", @pFILES_NO = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Text + "'";
                Query1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(Query1);

                if (dt1.Rows.Count > 0) //파일 번호 저장
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일첨부여부")].Text = "Y";
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일첨부여부")].Text = "N";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Text = "";
                }

                UP_Files(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Text
                        , fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰번호")].Text
                        , fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "견적순번")].Text);
            }
            strBtn = "Y";

        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            try
            {
                // 프로젝트차수
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "차수"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text != "*"
                        || fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text != "")
                    {
                        string seq = SystemBase.Base.CodeName("PROJECT_NO", "MAX(PROJECT_SEQ)", "S_SO_DETAIL", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text, " AND PROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                        if (seq == "")
                        {	//"프로젝트차수가 잘못 입력되었습니다!"
                            MessageBox.Show(SystemBase.Base.MessageRtn("B0054"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = "";
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = seq;
                        }
                    }
                }
                //구매담당자
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자"))
                {
                    string temp = "";
                    temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자")].Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                    if (temp != "")
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자명")].Text = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    else
                    {
                        DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0001"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);				 //구매담당자가 아닙니다
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자명")].Text = "";
                    }
                }
                //품목코드
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text != "*")
                    {
                        string Query = " usp_M_COMMON @pTYPE = 'M012', @pCODE = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "', @pNAME = '" + SystemBase.Base.gstrPLANT_CD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                        if (dt.Rows.Count > 0)
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = dt.Rows[0]["ITEM_NM"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = dt.Rows[0]["ITEM_SPEC"].ToString();
                            string unit = dt.Rows[0]["ITEM_UNIT"].ToString();

                            Query = "Select ISNULL(ORDER_PUR_UNIT,'') ";
                            Query += " From B_PLANT_ITEM_INFO(Nolock) Where  ITEM_CD  = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                            Query += " AND PLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                            DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(Query);

                            if (dt1.Rows.Count > 0)
                            {
                                if (dt1.Rows[0][0].ToString().Trim() == "") fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")].Text = unit;
                                else fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")].Text = dt1.Rows[0][0].ToString();
                            }
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")].Text = "";
                        }
                        Set_Item_grdReMake(Row, "3");
                    }
                    else
                    {
                        Set_Item_grdReMake(Row, "0");
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

        }
        #endregion

        #region 품목코드 그리드 속성 재정의
        private void Set_Item_grdReMake(int Row, string Type)
        {
            UIForm.FPMake.grdReMake(fpSpread1, Row,
                SystemBase.Base.GridHeadIndex(GHIdx1, "품명") + "|" + Type
                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "규격") + "|" + Type
                );
        }
        #endregion

        #region 버튼 click
        private void btnReqREF_Click(object sender, System.EventArgs e)
        {
            try
            {
                MEA001P1 frm1 = new MEA001P1(fpSpread1);
                frm1.ShowDialog();
                if (frm1.DialogResult == DialogResult.OK)
                {
                    string Msgs = frm1.ReturnVal;
                    if (Msgs == "Y")
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            if (strHead == "I") Set_Insert_ReType(i);
                        }
                    }
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCustAll_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (get_Insert_Check() == false)
                {
                    MessageBox.Show("저장 후 사용하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (get_Check())
                {
                    MEA001P4 frm1 = new MEA001P4(fpSpread1);
                    frm1.ShowDialog();
                    if (frm1.DialogResult == DialogResult.OK)
                    {
                        string Msgs = frm1.ReturnVal;
                        if (Msgs == "Y")
                        {
                            SearchExec();
                        }
                    }
                }
                else
                    MessageBox.Show("일괄선택에 체크된 것이 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQualityAll_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (get_Insert_Check() == false)
                {
                    MessageBox.Show("저장 후 사용하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (get_Check())
                {
                    MEA001P5 frm1 = new MEA001P5(fpSpread1);
                    frm1.ShowDialog();
                    if (frm1.DialogResult == DialogResult.OK)
                    {
                        string Msgs = frm1.ReturnVal;
                        if (Msgs == "Y")
                        {
                            SearchExec();
                        }
                    }
                }
                else
                    MessageBox.Show("일괄선택에 체크된 것이 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUser_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04009", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtUserId.Text = Msgs[0].ToString();
                    txtUserNm.Value = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void butItem_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW005 pu = new WNDW005(SystemBase.Base.gstrPLANT_CD, true, txtItemCd.Text);
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnProj_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW007 pu = new WNDW007(txtProjNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjNo.Text = Msgs[3].ToString();
                    if (txtProjSeq.Text != "*") txtProjSeq.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnProjSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

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

        private void butSCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtSCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSCustCd.Text = Msgs[1].ToString();
                    txtSCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBtn = "N";
        }

        private void btnPurDuty_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurDuty.Text = Msgs[0].ToString();
                    txtPurDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnPurDutyAll_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (get_Check())
                {
                    if (txtPurDuty.Text.Trim() == "")
                    {
                        MessageBox.Show("구매담당자를 입력하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPurDuty.Focus();
                    }
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, 2].Text == "True")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자")].Text = txtPurDuty.Text;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자명")].Text = txtPurDutyNm.Text;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출요구일자")].Text = dtpEstFilingDt.Text;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text = txtRemark.Text;
                        }
                    }
                }
                else
                    MessageBox.Show("일괄선택에 체크된 것이 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReqNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_MRQ499 @pTYPE = 'P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "" };
                string[] strSearch = new string[] { txtReqNo.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00085", strQuery, strWhere, strSearch, new int[] { 0 }, "구매요청번호 조회");
                pu.Width = 580;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReqNo.Text = Msgs[0].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매요청번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        private void btnEstNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                MEA001P7 frm1 = new MEA001P7();
                frm1.ShowDialog();
                if (frm1.DialogResult == DialogResult.OK)
                {
                    txtEstNo.Text = frm1.ReturnVal;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 일괄선택 체크여부
        private bool get_Check()
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, 2].Text == "True") return true;
            }
            return false;

        }

        private bool get_Insert_Check()
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I") return false;
            }
            return true;

        }
        #endregion

        #region TextChanged
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtProjNo_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
            {
                if (txtProjSeq.Text != "*") txtProjSeq.Text = "";
            }
        }

        private void txtSCustCd_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtSCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtPurDuty_Leave(object sender, System.EventArgs e)
        {
            if (strBtn == "N" && txtUserId.Text.Trim() != "")
            {
                string temp = "";
                temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtPurDuty.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                if (temp != "")
                    txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                else
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0001"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //구매담당자가 아닙니다
                    txtPurDuty.Text = "";
                    txtPurDutyNm.Value = "";
                    txtPurDuty.Focus();
                }
            }
        }

        private void txtUserId_Leave(object sender, System.EventArgs e)
        {
            if (strBtn == "N" && txtUserId.Text.Trim() != "")
            {
                string temp = "";
                temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtUserId.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                if (temp != "")
                    txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                else
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0001"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);				  //구매담당자가 아닙니다
                    txtUserId.Text = "";
                    txtUserNm.Value = "";
                    txtUserId.Focus();
                }
            }
        }
        #endregion

        #region MEA001_Activated
        private void MEA001_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpEstDtFr.Focus();
        }

        private void MEA001_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        #region fpSpread1_CellClick
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
                {
                    if (e.ColumnHeader == true)
                    {
                        if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, e.Column].Text != "True")
                        {
                            fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = false;
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                if (fpSpread1.Sheets[0].Cells[i, e.Column].Locked == false)
                                {
                                    fpSpread1.Sheets[0].Cells[i, e.Column].Value = false;
                                    UIForm.FPMake.fpChange(fpSpread1, i);
                                }
                            }
                        }
                        else
                        {
                            fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = true;
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                if (fpSpread1.Sheets[0].Cells[i, e.Column].Locked == false)
                                {
                                    fpSpread1.Sheets[0].Cells[i, e.Column].Value = true;
                                    UIForm.FPMake.fpChange(fpSpread1, i);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region MEA001_Closing  파일존재시 삭제
        private void MEA001_Closing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (e.Cancel == false)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Text.Trim() != "")
                            Del_Files(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Text);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Del_Files(string FileNo)
        {
            string ERRCode = "ER";
            string MSGCode = "";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_B_IMAGE @pType='D2' ";
                strSql = strSql + ", @pFILES_NO = '" + FileNo + "'";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                MSGCode = "P0001";
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        Exit:
            dbConn.Close();
            if (ERRCode == "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        #endregion

        #region 견적파일명 견적마스터에 업데이트
        private void UP_Files(string FileNo, string EstNO, string EstSeq)
        {
            string ERRCode = "ER";
            string MSGCode = "";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_MEA001 @pType='U4' ";
                strSql = strSql + ", @pFILES_NO = '" + FileNo + "'";
                strSql = strSql + ", @pEST_NO = '" + EstNO + "'";
                strSql = strSql + ", @pEST_SEQ = '" + EstSeq + "'";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                MSGCode = "P0001";
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        Exit:
            dbConn.Close();
            if (ERRCode == "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

    }
}
