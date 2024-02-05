#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 필수품질증빙설정
// 작 성 자   : 김창진
// 작 성 일   : 2014-07-17
// 작성내용   : 필수품질증빙설정
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
    WNDW.WNDW031 pu = new WNDW.WNDW031();
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

    public partial class WNDW031 : UIForm.FPCOMM1
    {
        #region 변수선언

        string strType = "";
        string strKeyNo = "";
        string strKeySeq = "";
        string strItemCd = "";
        string strProjectNo = "";
        string strCfmYn = "";
        string strWorkorderNo = "";
        string strProcSeq = "";
        string strJobCd = "";
        
        #endregion

        #region WNDW031 생성자
        public WNDW031(string Type, string KeyNo, string KeySeq, string ItemCd, string ProjectNo, string CfmYn)
        {
            strType = Type;
            strKeyNo = KeyNo;
            strKeySeq = KeySeq;
            strItemCd = ItemCd;
            strProjectNo = ProjectNo;
            strCfmYn = CfmYn;

            InitializeComponent();
        }

        public WNDW031(string Type, string KeyNo, string KeySeq, string ItemCd, string ProjectNo, string WorkorderNo, string ProcSeq, string JobCd, string CfmYn)
        {
            strType = Type;
            strKeyNo = KeyNo;
            strKeySeq = KeySeq;
            strItemCd = ItemCd;
            strProjectNo = ProjectNo;
            strWorkorderNo = WorkorderNo;
            strProcSeq = ProcSeq;
            strJobCd = JobCd;
            strCfmYn = CfmYn;

            InitializeComponent();
        }

        public WNDW031()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW031_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            if (strCfmYn == "Y")
            {
                UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            }
            else
            {
                UIForm.Buttons.ReButton("010000010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            }
            

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용


            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            
            txtKeyNo.Value = strKeyNo;
            txtKeySeq.Value = strKeySeq;
            txtItemCd.Value = strItemCd;
            txtProjectNo.Value = strProjectNo;
            txtWorkorderNo.Value = strWorkorderNo;
            txtProcSeq.Value = strProcSeq;
            txtJobCd.Value = strJobCd;

            if (strType == "RM" || strType == "RP" )
            {
                c1Label7.Text = "요청번호";
            }
            else if (strType == "PO")
            {
                c1Label7.Text = "발주번호";
            }

            if (txtKeyNo.Text != "")
                Grid_search(false);

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        { 
            Grid_search(true); 
        }
        #endregion

        #region 그리드조회
        private void Grid_search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string strQuery = " usp_WNDW031 @pTYPE = 'S1'";
                    strQuery += ", @pKEY_TYPE = '" + strType + "' ";
                    strQuery += ", @pKEY_NO = '" + txtKeyNo.Value + "' ";
                    strQuery += ", @pKEY_SEQ = '" + txtKeySeq.Value + "' ";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0);
                    
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "부서"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "분류"), FarPoint.Win.Spread.Model.MergePolicy.Restricted);

                    if (strCfmYn == "Y")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "필수여부") + "|3");
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "필수여부") + "|0");
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
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
                                default: strGbn = ""; break;
                            }

                            string strChkYn = "N";//필수여부
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "필수여부")].Text == "True") { strChkYn = "Y"; }

                            string strSql = " usp_WNDW031 '" + strGbn + "'";
                            strSql += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                            strSql += ", @pKEY_TYPE = '" + strType + "' ";
                            strSql += ", @pKEY_NO = '" + txtKeyNo.Value + "' ";
                            strSql += ", @pKEY_SEQ = '" + txtKeySeq.Value + "' ";
                            strSql += ", @pDOC_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "문서코드")].Text + "' ";
                            strSql += ", @pDOC_REQ_YN = '" + strChkYn + "' ";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
                }
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
                    txtItemSpec.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_SPEC", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                    txtItemSpec.Value = "";
                }
            }
            catch { }
        }

        //프로젝트
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Value != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
            }
            catch { }
        }

        //작업
        private void txtJobCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtJobCd.Value != "")
                {
                    txtJobNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtJobCd.Text, " AND MAJOR_CD = 'P001' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtJobNm.Value = "";
                }
            }
            catch { }
        }
        #endregion


    }
}
