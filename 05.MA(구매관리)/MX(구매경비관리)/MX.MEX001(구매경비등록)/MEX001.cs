#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매경비등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-08
// 작성내용 : 구매경비등록 및 관리
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
using WNDW;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Data.OleDb;

namespace MX.MEX001
{
    public partial class MEX001 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strTEMP_SLIP_NO, strSLIP_NO;
        string strBtn = "N";
        bool form_act_chk = false;
        Random rnd = null;
        bool add_chk = false;
        #endregion

        #region 생성자
        public MEX001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void MEX001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //그리드 콤보박스 세팅			
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//화폐단위
            // 2022.01.26. hma 추가(Start): 결재상태
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            // 2022.01.26. hma 추가(End)

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅		
            dtpExpDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpExpDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            rdoCfm_All.Checked = true;

            strTEMP_SLIP_NO = "";
            strSLIP_NO = "";

            dtpSlipDtFr.Value = "";
            dtpSlipDtTo.Value = "";
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpExpDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpExpDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            strTEMP_SLIP_NO = "";
            strSLIP_NO = "";

            dtpSlipDtFr.Value = "";
            dtpSlipDtTo.Value = "";
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            UIForm.FPMake.RowInsert(fpSpread1);

            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "발생일자")].Text = SystemBase.Base.ServerTime("YYMMDD");
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Value = "KRW";
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value = 1;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "발생자국금액")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형")].Value = "A";//일반세금계산서
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "유형명")].Text = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", "A", " AND MAJOR_CD = 'B040' AND LANG_CD ='" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Value = 10;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "지급액")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "지급자국금액")].Value = 0;
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "발생근거번호지정")].Value = "N";
            UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장") + "|1");

            if (add_chk == false)
            {
                add_chk = true;
                rnd = new Random();
            }
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "랜덤ID")].Text = Convert.ToString(rnd.Next());

        }
        #endregion

        #region 행복사 버튼 클릭 이벤트
        protected override void RCopyExec()
        {
            UIForm.FPMake.RowCopy(fpSpread1);
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "발생근거번호지정")].Value = "N";
            
            if (add_chk == false)
            {
                add_chk = true;
                rnd = new Random();
            }
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "랜덤ID")].Text = Convert.ToString(rnd.Next());
        }
        #endregion

        #region SearchExec()  그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Base.GroupBoxExceptions(groupBox1))
                {
                    strBtn = "Y";
                    string strCfmYn = "";

                    if (rdoCfm_Y.Checked == true) { strCfmYn = "Y"; }
                    else if (rdoCfm_N.Checked == true) { strCfmYn = "N"; }
                    else if (rdoCfm_N_M.Checked == true) { strCfmYn = "M"; }       // 2022.02.16. hma 추가: 반제대상 선택한 경우도 포함되게.

                    string strQuery = " usp_MEX001  @pTYPE = 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pEXP_DT_FR = '" + dtpExpDtFr.Text + "' ";
                    strQuery += ", @pEXP_DT_TO = '" + dtpExpDtTo.Text + "' ";
                    strQuery += ", @pEXP_STEPS = '" + txtExpSteps.Text + "' ";
                    strQuery += ", @pPUR_DUTY = '" + txtPurDuty.Text + "' ";
                    strQuery += ", @pCONFIRM_YN = '" + strCfmYn + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery += ", @pEXP_REF_NO = '" + txtExpRefNo.Text + "' ";
                    strQuery += ", @pEXP_CD = '" + txtExpCd.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pSLIP_DT_FR = '" + dtpSlipDtFr.Text + "' ";
                    strQuery += ", @pSLIP_DT_TO = '" + dtpSlipDtTo.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    txtPurDuty1.Value = txtPurDuty.Text;
                    txtPurDutyNm1.Value = txtPurDutyNm.Text;

                    // 2022.01.26. hma 추가(Start)
                    string strSlipNo = "";
                    string strCSlipNo = "", strCSlipConfirm = "", strCSlipGwStatus = "", strMinusConfirm = "";
                    string strMSlipNo = "", strMSlipConfirm = "", strMSlipGwStatus = "";
                    // 2022.01.26. hma 추가(End)

                    //확정여부에 따른 화면 Locking
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        // 2022.01.26. hma 추가(Start)
                        strSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호")].Text;
                        strCSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표번호")].Text;
                        strCSlipConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표승인")].Text;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")].Text == "")
                            strCSlipGwStatus = "";
                        else
                            strCSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")].Value.ToString();
                        strMinusConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제승인")].Text;
                        strMSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표번호")].Text;
                        strMSlipConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표승인")].Text;
                        strMSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Text;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Text == "")
                            strMSlipGwStatus = "";
                        else
                            strMSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Value.ToString();
                        // 2022.01.26. hma 추가(End)

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Text == "True")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분_2") + "|5"      // 읽기전용, 필수항목에서 제외, Focus 제외
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목_2") + "|5"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발생일자") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급처") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급처_2") + "|5"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행처") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행처_2") + "|5"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형_2") + "|5"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장_2") + "|5"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "환율") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발생자국금액") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형_2") + "|5"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급액") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급자국금액") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "만기일자") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "물대포함") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출금은행") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "은행명") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌_2") + "|5"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호_2") + "|5"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발생처") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호_2") + "|5"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                                );

                            // 2022.01.26. hma 추가(Start)
                            // 전표번호가 공백이 아니면서 확정전표번호가 공백이거나, 확정전표번호가 공백이 아니면서 확정전표 결재상태가 상신대기/반려 이거나 결재상태가 승인이면서 반제승인 Y인경우 
                            // 확정 항목 활성화.
                            if ((strSlipNo != "" && strCSlipNo == "") ||
                               ((strCSlipNo != "") &&
                                 (strCSlipGwStatus == "READY" || strCSlipGwStatus == "REJECT" ||            // 확정전표결재상태가 상신대기/반제 이거나
                                  (strCSlipGwStatus == "APPR" && strMinusConfirm == "Y"))))                 // 확정전표결재상태가 승인이면서 반제승인이 Y인 경우
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|0"      // 일반
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소") + "|5"     // 2022.01.28. hma 추가: 확정건은 반제취소 버튼 비활성화
                                    );
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                       SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|3"       // 읽기전용이면서 필수항목에서 제외
                                       + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소") + "|5"     // 2022.01.28. hma 추가: 확정건은 반제취소 버튼 비활성화
                                   );
                            }
                            // 2022.01.26. hma 추가(End)
                        }
                        else
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형")].Text == "")
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발생일자") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급처") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급처_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행처") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행처_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "환율") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발생자국금액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급자국금액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "만기일자") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "물대포함") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                    );
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발생일자") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급처") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급처_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행처") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행처_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "환율") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발생자국금액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급자국금액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "만기일자") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "물대포함") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                    );
                            }

                            // 2022.01.26. hma 추가(Start)
                            // 미확정상태이지만 반제전표 결재상태가 승인이면서 반제승인이 Y인 경우 확정 항목 활성화.
                            if ((strMSlipNo == "") ||
                                (strMSlipNo != "" &&
                                 (strMSlipGwStatus == "APPR" && strMinusConfirm == "Y")))
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|0"      // 일반
                                    );
                            }
                            else 
                            {
                                // 입력 항목들 비활성화.
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분_2") + "|5"      // 읽기전용, 필수항목에서 제외, Focus 제외
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목_2") + "|5"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발생일자") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급처") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급처_2") + "|5"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행처") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행처_2") + "|5"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형_2") + "|5"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장_2") + "|5"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "환율") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발생자국금액") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형_2") + "|5"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급액") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급자국금액") + "|5"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "만기일자") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "물대포함") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|3"      // 읽기전용이면서 필수항목에서 제외
                                    );
                            }

                            // 미확정건이지만 반제전표가 생성되어 결재상태가 상신대기/반려이면 반제취소 버튼 활성화.
                            if (strMSlipNo != "" &&
                                 (strMSlipGwStatus == "READY" || strMSlipGwStatus == "REJECT"))
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소") + "|0"    // 일반
                                    );
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소") + "|5"    // 읽기전용, 필수항목에서 제외, Focus 제외
                                    );
                            }
                            // 2022.01.26. hma 추가(End)

                            GridSet(i);
                        }

                    }

                    strBtn = "N";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                string TempNo = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                    //그리드 상단 필수 체크
                    if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
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

                                if (strHead == "I" && txtPurDuty1.Text.Trim() == "")
                                {
                                    ERRCode = "ER";
                                    MSGCode = "구매담당자를 입력하세요!";
                                    goto Exit;
                                }


                                string strCfmYn = "N";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Text == "True") { strCfmYn = "Y"; }

                                if (strHead == "D" && strCfmYn == "Y")
                                {
                                    ERRCode = "ER";
                                    MSGCode = "확정된 데이타는 삭제할 수 없습니다!";
                                    goto Exit;
                                }

                                string strGoodsYn = "N";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "물대포함")].Text == "True") { strGoodsYn = "Y"; }

                                if (strCfmYn == "Y" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생근거번호지정")].Text == "N")
                                {
                                    ERRCode = "ER";
                                    MSGCode = "M0008";
                                    goto Exit;
                                }

                                string strSql = " usp_MEX001 '" + strGbn + "'";
                                strSql += ", @pEXP_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리번호")].Text + "' ";
                                strSql += ", @pEXP_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생일자")].Text + "' ";
                                strSql += ", @pEXP_STEPS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분")].Text + "' ";
                                strSql += ", @pEXP_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목")].Text + "' ";

                                if (strHead == "I")
                                    strSql += ", @pPUR_DUTY = '" + txtPurDuty1.Text + "' ";
                                else
                                    strSql += ", @pPUR_DUTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자")].Text + "' ";

                                strSql += ", @pTAX_BIZ_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장")].Text + "' ";

                                strSql += ", @pCURRENCY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Value + "' ";
                                strSql += ", @pEXCH_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value + "' ";

                                strSql += ", @pVAT_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형")].Text + "' ";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Text != "")
                                    strSql += ", @pVAT_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Value + "' ";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Text != "")
                                    strSql += ", @pVAT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value + "' ";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Text != "")
                                    strSql += ", @pVAT_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value + "' ";

                                strSql += ", @pBILL_CUST = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발행처")].Text + "' ";
                                strSql += ", @pPAYMENT_CUST= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지급처")].Text + "' ";
                                strSql += ", @pPAYMENT_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text + "' ";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지급액")].Text != "")
                                    strSql += ", @pPAYMENT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지급액")].Value + "' ";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지급자국금액")].Text != "")
                                    strSql += ", @pPAYMENT_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지급자국금액")].Value + "' ";

                                strSql += ", @pBANK_CD= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출금은행")].Text + "' ";
                                strSql += ", @pBANK_ACCT_NO= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌")].Text + "' ";
                                strSql += ", @pPRPAYM_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호")].Text + "' ";
                                strSql += ", @pNOTE_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호")].Text + "' ";

                                strSql += ", @pEXP_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액")].Value + "' ";
                                strSql += ", @pEXP_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생자국금액")].Value + "' ";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "만기일자")].Text != "")
                                    strSql += ", @pEXPIRY_DT= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "만기일자")].Text + "' ";
                                strSql += ", @pGOODS_INCLUDE_YN = '" + strGoodsYn + "' ";
                                strSql += ", @pCONFIRM_YN = '" + strCfmYn + "' ";

                                strSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                                strSql += ", @pRND_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "랜덤ID")].Text + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();
                                TempNo = ds.Tables[0].Rows[0][2].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                                //금액 변경시 경비계산 UPDATE 
                                if ((strGbn == "U1"
                                   && (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액")].Value.ToString()) != Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, 52].Value.ToString())
                                    || Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생자국금액")].Value.ToString()) != Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, 53].Value.ToString()))
                                   ) || strGbn == "I1")
                                {
                                    strSql = " usp_MEX001 'T3'";
                                    strSql += ", @pEXP_STEPS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분")].Text + "' ";
                                    if (strGbn == "I1")
                                        strSql += ", @pEXP_NO = '" + TempNo + "' ";
                                    else
                                        strSql += ", @pEXP_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리번호")].Text + "' ";
                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                    DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                                }
                            }
                        }
                    }
                    else
                    {
                        Trans.Rollback();
                        this.Cursor = Cursors.Default;
                        return;
                    }
                    Trans.Commit();
                    rnd = null;
                    add_chk = false;
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
                this.Cursor = Cursors.Default;
            }

            //			}

        }
        #endregion

        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            strBtn = "Y";
            //창고
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분_2"))
            {
                try
                {
                    string strQuery = "usp_B_COMMON @pType='COMM_POP', @pSPEC1 = 'M015', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00051", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "진행구분 조회");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분명")].Text = Msgs[1].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='M013' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00061", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "경비항목 팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {

                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "지급처_2"))
            {
                try
                {
                    WNDW002 pu = new WNDW002(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급처")].Text, "P");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급처")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급처명")].Text = Msgs[2].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "발행처_2"))
            {
                try
                {
                    WNDW002 pu = new WNDW002(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발행처")].Text, "P");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발행처")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발행처명")].Text = Msgs[2].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'COMM_POP1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'B040' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00032", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "부가세유형 팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "유형명")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Text = Msgs[2].ToString();

                        Vat_Change(Row);
                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장") + "|1");
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장_2"))
            {
                try
                {
                    string strQuery = "usp_S_COMMON @pTYPE = 'S070', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00010", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "세금신고사업장 조회");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            //지급유형
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형_2"))
            {
                try
                {
                    string strQuery = " usp_M_COMMON 'M060', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00011", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "지급유형조회");	//지급유형조회
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그

                        GridSet(Row);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            //계좌번호
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌_2"))
            {
                try
                {
                    MV.MIV002.MIV002P1 myForm = new MV.MIV002.MIV002P1(fpSpread1, Row);
                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(myForm.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출금은행")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행명")].Text = Msgs[2].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            //어음번호
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호_2"))
            {
                try
                {
                   
                     MV.MIV002.MIV002P2 myForm = new MV.MIV002.MIV002P2(fpSpread1, Row, fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발행처")].Text);
                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(myForm.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호")].Text = Msgs[0].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            //선급금번호
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호_2"))
            {
                try
                {
                    MV.MIV002.MIV002P3 myForm = new MV.MIV002.MIV002P3(fpSpread1, Row, fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급처")].Text, fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text);
                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(myForm.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호")].Text = Msgs[0].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "발생근거번호지정_2"))
            {
                try
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분")].Text.Trim() == "")
                    {
                        MessageBox.Show("진행구분을 먼저 입력하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fpSpread1.Sheets[0].SetActiveCell(Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분"));
                        strBtn = "N";
                        return;
                    }

                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생자국금액")].Text == "0" ||
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생자국금액")].Text == "" ||
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액")].Text == "" ||
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액")].Text == "0")
                    {
                        MessageBox.Show("발생금액을 먼저 입력하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fpSpread1.Sheets[0].SetActiveCell(Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액"));
                        strBtn = "N";
                        return;
                    }

                    if (fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text == "I")
                    {
                        MEX001P1 myForm
                            = new MEX001P1(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "랜덤ID")].Text,
                                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분")].Text,
                                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분명")].Text,
                                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생근거번호지정")].Text,
                                            Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생자국금액")].Value),
                                            Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value),
                                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Text,
                                            Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액")].Value),
                                            // 2022.01.28. hma 추가(Start): 반제전표관련 항목값도 넘어가도록 함.
                                            "",
                                            "",
                                            "",
                                            // 2022.01.28. hma 추가(End)
                                            true);

                        myForm.ShowDialog();

                        if (myForm.DialogResult == DialogResult.OK)
                        {
                            string Msgs = myForm.ReturnVal;
                            if (Msgs == "Y") fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생근거번호지정")].Text = "Y";
                            else fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생근거번호지정")].Text = "N";


                        }
                    }
                    else
                    {
                        // 2022.01.28. hma 추가(Start): 반제전표결재상태 항목값
                        string strMinusGwStatus = "";
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Text != "")
                            strMinusGwStatus = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Value.ToString();
                        // 2022.01.28. hma 추가(End)

                        MEX001P1 myForm
                            = new MEX001P1(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리번호")].Text,
                                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분")].Text,
                                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분명")].Text,
                                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생근거번호지정")].Text,
                                            Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생자국금액")].Value),
                                            Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value),
                                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Text,
                                            Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액")].Value),
                                            // 2022.01.28. hma 추가(Start): 반제전표관련 항목값도 넘어가도록 함.
                                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표번호")].Text,
                                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표승인")].Text,
                                            strMinusGwStatus, 
                                            false
                                            // 2022.01.28. hma 추가(End)
                                          );

                        myForm.ShowDialog();

                        if (myForm.DialogResult == DialogResult.OK)
                        {
                            string Msgs = myForm.ReturnVal;
                            if (Msgs == "Y") fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생근거번호지정")].Text = "Y";
                            else fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생근거번호지정")].Text = "N";


                        }
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            strBtn = "N";
        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            int i_xrate = SystemBase.Base.GridHeadIndex(GHIdx1, "환율");
            int i_exp = SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액");
            int i_exp_loc = SystemBase.Base.GridHeadIndex(GHIdx1, "발생자국금액");
            int i_vat = SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액");
            int i_vat_loc = SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액");
            int i_pay = SystemBase.Base.GridHeadIndex(GHIdx1, "지급액");
            int i_pay_loc = SystemBase.Base.GridHeadIndex(GHIdx1, "지급자국금액");
            int i_vrate = SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율");
            try
            {
                if (strBtn == "N")
                {
                    if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분"))
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분명")].Text
                            = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분")].Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'M015' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목"))
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목명")].Text
                            = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목")].Text, " AND MAJOR_CD = 'M013' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "지급처"))
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급처명")].Text
                            = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급처")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "발행처"))
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발행처명")].Text
                            = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발행처")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형"))
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "유형명")].Text
                            = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형")].Text, " AND MAJOR_CD = 'B040' AND LANG_CD ='" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Text
                            = SystemBase.Base.CodeName("MINOR_CD", "REL_CD1", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세유형")].Text, " AND MAJOR_CD = 'B040' AND LANG_CD ='" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "유형명")].Text.Trim() == "")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장") + "|0");

                            fpSpread1.Sheets[0].Cells[Row, i_vrate].Value = 0;
                            fpSpread1.Sheets[0].Cells[Row, i_vat].Value = 0;
                            fpSpread1.Sheets[0].Cells[Row, i_vat_loc].Value = 0;
                            fpSpread1.Sheets[0].Cells[Row, i_pay].Value = fpSpread1.Sheets[0].Cells[Row, i_exp].Value;
                            fpSpread1.Sheets[0].Cells[Row, i_pay_loc].Value = fpSpread1.Sheets[0].Cells[Row, i_exp_loc].Value;
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장") + "|1");

                            fpSpread1.Sheets[0].Cells[Row, i_vat].Value
                                = Math.Floor(Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, i_vrate].Value) / 100 * Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, i_exp].Value));

                            fpSpread1.Sheets[0].Cells[Row, i_vat_loc].Value
                                = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_xrate].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_vat].Value);

                            fpSpread1.Sheets[0].Cells[Row, i_pay].Value
                                = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_exp].Value) + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_vat].Value);

                            fpSpread1.Sheets[0].Cells[Row, i_pay_loc].Value
                                = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_exp_loc].Value) + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_vat_loc].Value);
                        }

                    }
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장"))
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장명")].Text
                            = SystemBase.Base.CodeName("BIZ_CD", "BIZ_NM", "B_BIZ_PLACE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "신고사업장")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형"))
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형명")].Text
                            = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text, " And MAJOR_CD = 'S012' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ");

                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급금유형명")].Text != "")
                        {
                            GridSet(Row);
                        }
                    }
                    else if (Column == i_exp) //발생금액
                    {
                        fpSpread1.Sheets[0].Cells[Row, i_exp_loc].Value
                            = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_xrate].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_exp].Value);

                        fpSpread1.Sheets[0].Cells[Row, i_vat].Value
                            = Math.Floor(Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, i_vrate].Value) / 100 * Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, i_exp].Value));

                        fpSpread1.Sheets[0].Cells[Row, i_vat_loc].Value
                            = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_xrate].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_vat].Value);

                        fpSpread1.Sheets[0].Cells[Row, i_pay].Value
                            = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_exp].Value) + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_vat].Value);

                        fpSpread1.Sheets[0].Cells[Row, i_pay_loc].Value
                            = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_exp_loc].Value) + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_vat_loc].Value);
                    }
                    else if (Column == i_xrate) //환율
                    {
                        fpSpread1.Sheets[0].Cells[Row, i_exp_loc].Value
                            = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_xrate].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_exp].Value);

                        fpSpread1.Sheets[0].Cells[Row, i_vat_loc].Value
                            = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_xrate].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_vat].Value);

                        fpSpread1.Sheets[0].Cells[Row, i_pay_loc].Value
                            = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_xrate].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_pay].Value);
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

        #region fpSpread1_ComboSelChange
        private void Vat_Change(int Row)
        {
            int i_vrate = SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율");
            int i_xrate = SystemBase.Base.GridHeadIndex(GHIdx1, "환율");
            int i_exp = SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액");
            int i_exp_loc = SystemBase.Base.GridHeadIndex(GHIdx1, "발생자국금액");
            int i_vat = SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액");
            int i_vat_loc = SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액");
            int i_pay = SystemBase.Base.GridHeadIndex(GHIdx1, "지급액");
            int i_pay_loc = SystemBase.Base.GridHeadIndex(GHIdx1, "지급자국금액");


            fpSpread1.Sheets[0].Cells[Row, i_vat].Value
                = (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_vrate].Value) / 100) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_exp].Value);

            fpSpread1.Sheets[0].Cells[Row, i_vat_loc].Value
                = Math.Floor(Convert.ToDouble(Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_xrate].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_vat].Value)));

            fpSpread1.Sheets[0].Cells[Row, i_pay].Value
                = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_exp].Value) + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_vat].Value);

            fpSpread1.Sheets[0].Cells[Row, i_pay_loc].Value
                = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_exp_loc].Value) + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, i_vat_loc].Value);

        }

        private void fpSpread1_ComboSelChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            int i_xrate = SystemBase.Base.GridHeadIndex(GHIdx1, "환율");
            int i_exp = SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액");
            int i_exp_loc = SystemBase.Base.GridHeadIndex(GHIdx1, "발생자국금액");
            int i_vat = SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액");
            int i_vat_loc = SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액");
            int i_pay = SystemBase.Base.GridHeadIndex(GHIdx1, "지급액");
            int i_pay_loc = SystemBase.Base.GridHeadIndex(GHIdx1, "지급자국금액");

            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위"))
            {
                fpSpread1.Sheets[0].Cells[e.Row, i_exp_loc].Value
                    = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, i_xrate].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, i_exp].Value);

                fpSpread1.Sheets[0].Cells[e.Row, i_vat_loc].Value
                    = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, i_xrate].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, i_vat].Value);

                fpSpread1.Sheets[0].Cells[e.Row, i_pay_loc].Value
                    = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, i_xrate].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, i_pay].Value);
            }
        }
        #endregion

        #region 그리드 필수, 일반, 읽기적용 세팅
        private void GridSet(int Row)
        {
            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text == "DP")//계좌번호
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호")].Text = "";

                UIForm.FPMake.grdReMake(fpSpread1, Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌_2") + "|0"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호_2") + "|3"
                    );
            }
            else if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text == "NP")//어음/수표번호
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출금은행")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행명")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호")].Text = "";

                UIForm.FPMake.grdReMake(fpSpread1, Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호_2") + "|0"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호_2") + "|3"
                    );
            }
            else if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text == "PP")//선급금번호
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출금은행")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행명")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호")].Text = "";

                UIForm.FPMake.grdReMake(fpSpread1, Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호_2") + "|0"
                    );
            }
            else
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출금은행")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행명")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호")].Text = "";

                UIForm.FPMake.grdReMake(fpSpread1, Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출금계좌_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음/수표번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호_2") + "|3"
                    );
            }
        }
        #endregion

        #region 조회조건 팝업
        //구매담당자
        private void btnPurDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_M_COMMON @pTYPE = 'M011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매담당자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurDuty.Value = Msgs[0].ToString();
                    txtPurDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //구매담당자1
        private void btnPurDuty1_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_M_COMMON @pTYPE = 'M011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDuty1.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매담당자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurDuty1.Value = Msgs[0].ToString();
                    txtPurDutyNm1.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //진행구분
        private void btnExpSteps_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pType='COMM_POP', @pSPEC1 = 'M015', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtExpSteps.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00051", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "진행구분 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtExpSteps.Value = Msgs[0].ToString();
                    txtExpStepsNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
            }
        }



        private void btnProj_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectSeq.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnProjSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeq.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }


        //근거번호
        private void btnExpRefNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strExpSteps = txtExpSteps.Text.Trim();
                if (strExpSteps == "")
                {
                    MessageBox.Show("진행구분를 먼저 입력하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtExpSteps.Focus();
                    return;
                }
                //발주
                if (strExpSteps == "PO")
                {
                    WNDW.WNDW018 pu = new WNDW.WNDW018();
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtExpRefNo.Value = Msgs[1].ToString();
                        txtExpRefNo.Focus();
                    }

                }
                else if (strExpSteps == "VB") // 수입선적
                {
                    WNDW.WNDW022 pu = new WNDW.WNDW022();
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtExpRefNo.Value = Msgs[2].ToString();
                        txtExpRefNo.Focus();
                    }
                }
                else if (strExpSteps == "VD") // 통관
                {
                    WNDW.WNDW023 pu = new WNDW.WNDW023();
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtExpRefNo.Value = Msgs[1].ToString();
                        txtExpRefNo.Focus();
                    }

                }
                else if (strExpSteps == "VL") // L/C
                {
                    WNDW.WNDW021 pu = new WNDW.WNDW021();
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtExpRefNo.Value = Msgs[1].ToString();
                        txtExpRefNo.Focus();
                    }
                }
                else if (strExpSteps == "VO") // LOCAL L/C
                {
                    WNDW.WNDW021 pu = new WNDW.WNDW021();
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtExpRefNo.Value = Msgs[1].ToString();
                        txtExpRefNo.Focus();
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

            }
        }

        private void btnExpCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='M013' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtExpCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00061", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "경비항목 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtExpCd.Value = Msgs[0].ToString();
                    txtExpNm.Value = Msgs[1].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        #endregion

        #region 조회조건 TextChanged
        //진행구분
        private void txtExpSteps_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtExpSteps.Text != "")
                    {
                        txtExpStepsNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtExpSteps.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'M015' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtExpStepsNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }


        private void txtPurDuty1_Leave(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N" && txtPurDuty1.Text.Trim() != "")
                {
                    string temp = "";
                    temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtPurDuty1.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                    if (temp != "")
                    {
                        if (txtPurDuty1.Text != "")
                        {
                            txtPurDutyNm1.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDuty1.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                        }
                        else
                        {
                            txtPurDutyNm1.Value = "";
                        }
                        if (txtPurDuty.Text.Trim() == "")
                        {
                            txtPurDuty.Text = txtPurDuty1.Text;
                            txtPurDutyNm.Text = txtPurDutyNm1.Text;
                        }
                    }
                    else
                    {
                        DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0001"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPurDuty1.Text = "";
                        txtPurDutyNm1.Text = "";
                        txtPurDuty1.Focus();
                    }
                }
                else if (txtPurDuty1.Text.Trim() == "") txtPurDutyNm.Text = "";                
            }
            catch
            {

            }
        }

        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            txtProjectSeq.Value = "";
        }

        private void txtExpCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtExpCd.Text != "")
                {
                    txtExpNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtExpCd.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'M013' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtExpNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtPurDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N" && txtPurDuty.Text.Trim() != "")
                {
                    string temp = "";
                    temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtPurDuty.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                    if (temp != "")
                    {
                        if (txtPurDuty.Text != "")
                        {
                            txtPurDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                        }
                        else
                        {
                            txtPurDutyNm.Value = "";
                        }
                    }
                }
                else if (txtPurDuty.Text.Trim() == "") txtPurDutyNm.Value = "";
            }
            catch
            {

            }
        }

        private void txtPurDuty1_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N" && txtPurDuty1.Text.Trim() != "")
                {
                    string temp = "";
                    temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtPurDuty1.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                    if (temp != "")
                    {
                        if (txtPurDuty1.Text != "")
                        {
                            txtPurDutyNm1.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDuty1.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                        }
                        else
                        {
                            txtPurDutyNm1.Value = "";
                        }
                        if (txtPurDuty.Text.Trim() == "")
                        {
                            txtPurDuty.Value = txtPurDuty1.Text;
                            txtPurDutyNm.Value = txtPurDutyNm1.Text;
                        }
                    }
                }
                else if (txtPurDuty1.Text.Trim() == "") txtPurDutyNm1.Value = "";
            }
            catch
            {

            }
        }

        #endregion
        
        #region MEX001_Activated
        private void MEX001_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpExpDtFr.Focus();
        }

        private void MEX001_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        #region 엑셀UPLOAD
        private void btnFileUpload_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                string ERRCode = "", MSGCode = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd1 = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                string strSql = "";
                bool isBreak = true;
                bool isFirst = false;
                bool isEnd = false;  //마지막에 I4,T1 했는지 구분
                string strStep = "";
                string strNo = "";
                string tempNo = "";
                int row_idx = 0;
                string strRefNo = "";
                decimal vat_rate = 0;
                decimal vat_amt = 0;

                //int iLineNo = 0;                // 2020.04.09. hma 추가     // 다시 주석 처리
                //string strQueryStr = "";        // 2020.04.09. hma 추가     // 다시 주석 처리

                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "통합 Excel 문서(*.xls)|*.xls|2007 Excel 문서(*.xlsx)|*.xlsx";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 2017.11.01. CWL 수정(Start): 윈도우 보안 업데이트후 문제가 생겨서 엑셀 업로드시 OLEDB 부분 수정함.
                        //string connectionString = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;Imex=1;hdr=yes;""", dlg.FileName);
                        string connectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0;Imex=1;hdr=yes;""", dlg.FileName);
                        // 2017.11.01. CWL 수정(End)
                        OleDbConnection conn = new OleDbConnection(connectionString);
                        conn.Open();

                        DataTable worksheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                        string commandString = String.Format("SELECT * FROM [{0}]", worksheets.Rows[0]["TABLE_NAME"]);
                        OleDbCommand cmd = new OleDbCommand(commandString, conn);

                        OleDbDataAdapter dapt = new OleDbDataAdapter(cmd);
                        DataSet ds = new DataSet();

                        dapt.Fill(ds);
                        conn.Close();

                        //행수만큼 처리
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            row_idx = i;
                            //iLineNo = i;        // 2020.04.09. hma 추가     // 다시 주석 처리

                            if (ds.Tables[0].Rows[i][2].ToString().Trim() == "") { isBreak = false; break; }
                            if (i > 0 && ds.Tables[0].Rows[i][2].ToString().Trim() != tempNo)
                            {
                                strSql = " usp_MEX001 'T3'";
                                strSql += ", @pEXP_STEPS = '" + strStep + "' ";
                                strSql += ", @pEXP_NO = '" + strNo + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                //strQueryStr = strSql;       // 2020.04.09. hma 추가     // 다시 주석 처리

                                DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; } 	// ER 코드 Return시 점프 
                                isEnd = true;
                            }

                            if (ds.Tables[0].Rows[i][2].ToString().Trim() == ds.Tables[0].Rows[i][3].ToString().Trim())
                            {
                                strSql = " usp_MEX001 'I1'";
                                strSql += ", @pEXP_NO = '' ";
                                strSql += ", @pEXP_DT = '" + Convert.ToDateTime(ds.Tables[0].Rows[i][6].ToString()).ToShortDateString() + "' ";
                                strStep = ds.Tables[0].Rows[i][0].ToString();
                                strSql += ", @pEXP_STEPS = '" + strStep + "' ";
                                strSql += ", @pEXP_CD = '" + ds.Tables[0].Rows[i][4].ToString() + "' ";
                                strSql += ", @pPUR_DUTY = '" + txtPurDuty1.Text + "' ";

                                if (ds.Tables[0].Rows[i][11].ToString().Trim() != "" && ds.Tables[0].Rows[i][13].ToString().Trim() == "")
                                {
                                    ERRCode = "WR";
                                    MSGCode = " 배부근거 : " + ds.Tables[0].Rows[i][2].ToString() + " 발생근거 : " + ds.Tables[0].Rows[i][2].ToString() + " 부가세유형에 값이 있으면 신고사업장은 필수로 입력해야 합니다.";
                                    Trans.Rollback(); goto Exit;
                                }
                                strSql += ", @pTAX_BIZ_CD = '" + ds.Tables[0].Rows[i][13].ToString() + "' ";

                                strSql += ", @pCURRENCY = '" + ds.Tables[0].Rows[i][15].ToString() + "' ";

                                if (ds.Tables[0].Rows[i][15].ToString() == "KRW")
                                    strSql += ", @pEXCH_RATE = '1' ";
                                else
                                {
                                    if (ds.Tables[0].Rows[i][17].ToString().Trim() == "1" || ds.Tables[0].Rows[i][17].ToString() == "0"
                                        || ds.Tables[0].Rows[i][17].ToString() == "" || ds.Tables[0].Rows[i][17].ToString() == "-")
                                    {
                                        ERRCode = "WR";
                                        MSGCode = " 배부근거 : " + ds.Tables[0].Rows[i][2].ToString() + " 발생근거 : " + ds.Tables[0].Rows[i][2].ToString() + " 환율이 잘못 되었습니다.";
                                        Trans.Rollback(); goto Exit;
                                    }
                                    else
                                        strSql += ", @pEXCH_RATE = '" + ds.Tables[0].Rows[i][17] + "' ";
                                }

                                strSql += ", @pVAT_TYPE = '" + ds.Tables[0].Rows[i][11].ToString() + "' ";
                                if (ds.Tables[0].Rows[i][19].ToString() != "" && ds.Tables[0].Rows[i][20].ToString() != "-")
                                {
                                    strSql += ", @pVAT_RATE = '" + ds.Tables[0].Rows[i][19] + "' ";
                                    vat_rate = Convert.ToDecimal(ds.Tables[0].Rows[i][19]);
                                }
                                else vat_rate = 0;

                                if (ds.Tables[0].Rows[i][20].ToString() != "" && ds.Tables[0].Rows[i][20].ToString() != "-")
                                {
                                    strSql += ", @pVAT_AMT = '" + ds.Tables[0].Rows[i][20].ToString().Replace(",", "") + "' ";
                                    vat_amt = Convert.ToDecimal(ds.Tables[0].Rows[i][20]);
                                }
                                else vat_amt = 0;

                                if (vat_rate > 0 && vat_amt == 0)
                                {
                                    ERRCode = "WR";
                                    MSGCode = " 배부근거 : " + ds.Tables[0].Rows[i][2].ToString() + " 발생근거 : " + ds.Tables[0].Rows[i][2].ToString() + "부가세금액이 입력되지 않았습니다.";
                                    Trans.Rollback(); goto Exit;
                                }

                                if (ds.Tables[0].Rows[i][21].ToString() != "" && ds.Tables[0].Rows[i][21].ToString() != "-")
                                    strSql += ", @pVAT_AMT_LOC = '" + ds.Tables[0].Rows[i][21].ToString().Replace(",", "") + "' ";

                                strSql += ", @pBILL_CUST = '" + ds.Tables[0].Rows[i][9].ToString() + "' ";
                                strSql += ", @pPAYMENT_CUST= '" + ds.Tables[0].Rows[i][7].ToString() + "' ";
                                strSql += ", @pPAYMENT_TYPE = '" + ds.Tables[0].Rows[i][22].ToString() + "' ";
                                if (ds.Tables[0].Rows[i][24].ToString() != "" && ds.Tables[0].Rows[i][24].ToString() != "-")
                                    strSql += ", @pPAYMENT_AMT = '" + ds.Tables[0].Rows[i][24].ToString().Replace(",", "") + "' ";
                                if (ds.Tables[0].Rows[i][25].ToString() != "" && ds.Tables[0].Rows[i][25].ToString() != "-")
                                    strSql += ", @pPAYMENT_AMT_LOC = '" + ds.Tables[0].Rows[i][25].ToString().Replace(",", "") + "' ";

                                strSql += ", @pBANK_CD= '" + ds.Tables[0].Rows[i][28].ToString() + "' ";
                                strSql += ", @pBANK_ACCT_NO= '" + ds.Tables[0].Rows[i][30].ToString() + "' ";
                                strSql += ", @pPRPAYM_NO = '" + ds.Tables[0].Rows[i][32].ToString() + "' ";
                                strSql += ", @pNOTE_NO = '" + ds.Tables[0].Rows[i][31].ToString() + "' ";

                                strSql += ", @pEXP_AMT = '" + ds.Tables[0].Rows[i][16].ToString().Replace(",", "") + "' ";
                                strSql += ", @pEXP_AMT_LOC = '" + ds.Tables[0].Rows[i][18].ToString().Replace(",", "") + "' ";
                                if (ds.Tables[0].Rows[i][26].ToString() != "")
                                    strSql += ", @pEXPIRY_DT= '" + Convert.ToDateTime(ds.Tables[0].Rows[i][26].ToString()).ToShortDateString() + "' ";
                                strSql += ", @pGOODS_INCLUDE_YN = '" + ds.Tables[0].Rows[i][27].ToString() + "' ";
                                strSql += ", @pCONFIRM_YN = 'N' ";
                                strSql += ", @pREMARK = '" + ds.Tables[0].Rows[i][34].ToString() + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                isFirst = true;
                                isEnd = false;
                            }
                            else
                            {
                                strSql = " usp_MEX001 'I2'";
                                strSql += ", @pEXP_STEPS = '" + strStep + "' ";
                                strSql += ", @pEXP_NO = '" + strNo + "' ";
                                strSql += ", @pEXP_REF_NO = '" + ds.Tables[0].Rows[i][3].ToString() + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                strRefNo = ds.Tables[0].Rows[i][3].ToString();
                                isEnd = false;
                            }
                            tempNo = ds.Tables[0].Rows[i][2].ToString().Trim();

                            //strQueryStr = strSql;       // 2020.04.09. hma 추가     // 다시 주석 처리

                            DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);

                            ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; } 	// ER 코드 Return시 점프 
                            if (ERRCode == "OK") { strNo = ds1.Tables[0].Rows[0][2].ToString(); }

                            if (isFirst)
                            {
                                strSql = " usp_MEX001 'I2'";
                                strSql += ", @pEXP_STEPS = '" + strStep + "' ";
                                strSql += ", @pEXP_NO = '" + strNo + "' ";
                                strSql += ", @pEXP_REF_NO = '" + ds.Tables[0].Rows[i][3].ToString() + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                strRefNo = ds.Tables[0].Rows[i][3].ToString();

                                //strQueryStr = strSql;       // 2020.04.09. hma 추가     // 다시 주석 처리

                                DataSet ds4 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);

                                ERRCode = ds4.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds4.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; } 	// ER 코드 Return시 점프 
                                if (ERRCode == "OK") { strNo = ds4.Tables[0].Rows[0][2].ToString(); }
                                isEnd = false;
                            }
                            isFirst = false;
                        }

                        if (isBreak || isEnd == false)
                        {
                            strSql = " usp_MEX001 'T3'";
                            strSql += ", @pEXP_STEPS = '" + strStep + "' ";
                            strSql += ", @pEXP_NO = '" + strNo + "' ";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            //strQueryStr = strSql;       // 2020.04.09. hma 추가     // 다시 주석 처리

                            DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; } 	// ER 코드 Return시 점프 
                        }
                        Trans.Commit();
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        Trans.Rollback();
                        ERRCode = "ER";
                        MSGCode = f.Message;
                    }
                Exit:
                    dbConn.Close();
                    if (ERRCode == "OK")
                    {
                        MessageBox.Show("등록되었습니다.", SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SearchExec();

                    }
                    else if (ERRCode == "ER")
                    {
                        //MessageBox.Show(Convert.ToString(iLineNo) + ": " + strQueryStr + "!!", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);    // 2020.04.09. hma 추가     // 다시 주석 처리
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        int idx = MSGCode.IndexOf("이미 존재하는 데이터");
                        if (idx > 0)
                            MessageBox.Show(Convert.ToString(row_idx + 2) + "번째열이 발생근거번호[" + strRefNo + "]가 중복됩니다! ", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        int idx1 = MSGCode.IndexOf("부가세");
                        int idx2 = MSGCode.IndexOf("환율");
                        if (idx1 > 0 || idx2 > 0)
                            MessageBox.Show(Convert.ToString(row_idx + 2) + "번째열에서 " + MSGCode, SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
            }
            this.Cursor = Cursors.Default;

        }
        #endregion
        
        #region MEX001_Closing 임시데이타 삭제
        private void MEX001_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string ERRCode = "ER", MSGCode = "";
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_MEX001  'DD' ";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = f.Message;
                //MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();
            if (ERRCode != "OK")
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        #endregion

        #region 셀 클릭시 결의전표, 회계전표 가져오기
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                if (e.Row >= 0)
                {
                    strSLIP_NO = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호")].Text;
                }
                else
                {
                    strSLIP_NO = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString()); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion
        
        #region 전표조회 이벤트
        private void btnRef_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strSLIP_NO != "")
                {
                    WNDW.WNDW026 pu = new WNDW.WNDW026(strSLIP_NO);
                    pu.ShowDialog();
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("S0016"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전표정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

       
        #region 전체 선택 버튼
        private void btnSelectAll_Click(object sender, System.EventArgs e)
        {
            bool bIsLock = false;       // 2022.01.28. hma 추가

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                string strCfm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Text;
                bIsLock = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Locked;     // 2022.01.28. hma 추가: 확정 비활성화 여부

                if ((strCfm != "True")
                     && (bIsLock == false))     // 2022.01.28. hma 추가: 확정 항목이 비활성화인 건은 제외되게.
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Value = 1;
                    UIForm.FPMake.fpChange(fpSpread1, i);
                }
            }
        }

        // 2022.01.28. hma 추가(Start)
        #region fpSpread1_ButtonClicked(): 그리드 버튼 클릭시
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소"))
                {
                    // 2022.02.16. hma 추가: 메시지 확인
                    DialogResult dsMsg = MessageBox.Show("반제취소 처리하시겠습니까?", SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dsMsg == DialogResult.Yes)
                    {
                        string strExpNo = "";
                        string strCSlipNo = "", strCSlipConfirm = "", strCSlipGwStatus = "", strMinusConfirm = "";
                        string strMSlipNo = "", strMSlipConfirm = "", strMSlipGwStatus = "";

                        int i;
                        i = e.Row;

                        strExpNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리번호")].Text;        // 경비번호
                        strCSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표번호")].Text;
                        strCSlipConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표승인")].Text;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")].Text == "")
                            strCSlipGwStatus = "";
                        else
                            strCSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")].Value.ToString();
                        strMinusConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제승인")].Text;
                        strMSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표번호")].Text;
                        strMSlipConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표승인")].Text;
                        strMSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Text;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Text == "")
                            strMSlipGwStatus = "";
                        else
                            strMSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Value.ToString();

                        // 확정전표 결재상태가 승인이고, 반제전표 결재상태가 상신대기/반려인 경우 반제전표 삭제 가능하게 함.
                        if (strCSlipGwStatus == "APPR" && strMinusConfirm == "Y" && strMSlipNo != "" &&
                            (strMSlipGwStatus == "READY" || strMSlipGwStatus == "REJECT"))
                        {
                            MinusSlipDelete(strExpNo);
                        }
                        else
                        {
                            MessageBox.Show("확정전표 결재상태가 승인이고 반제전표 결재상태가 상신대기/반려인 경우 반제취소 가능합니다.");
                            return;
                        }
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "결재자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        #region MinusSlipDelete(): 해당 경비번호에 대한 반제전표 삭제 처리
        private void MinusSlipDelete(string EXP_NO)
        {
            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_MEX001  'D4'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strSql += ", @pEXP_NO = '" + EXP_NO + "' ";     // 구매경비번호
                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = f.Message;
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
        }
        #endregion
        // 2022.01.28. hma 추가(End)

        //선택 취소 버튼
        private void btnSelectCancel_Click(object sender, System.EventArgs e)
        {
            bool bIsLock = false;       // 2022.01.28. hma 추가

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                string strCfm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Text;
                bIsLock = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Locked;     // 2022.01.28. hma 추가: 확정 비활성화 여부

                if ((strCfm == "True") 
                    && (bIsLock == false) )     // 2022.01.28. hma 추가: 확정 항목이 비활성화인 건은 제외되게.
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Value = 0;
                    UIForm.FPMake.fpChange(fpSpread1, i);
                }
            }

        }

        #endregion    
        
    }
}
