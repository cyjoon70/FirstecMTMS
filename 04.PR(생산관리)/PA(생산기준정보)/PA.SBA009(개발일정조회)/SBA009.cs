#region 작성정보
/*********************************************************************/
// 단위업무명 : 개발일정조회
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-15
// 작성내용 : 개발일정조회
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

namespace PA.SBA009
{
    public partial class SBA009 : UIForm.FPCOMM1
    {
        public SBA009()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void SBA009_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P002', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);//작업장
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "직/간구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P015', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);//직/간구분	
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "EIS적용")] = SystemBase.ComboMake.ComboOnGrid("usp_C_COMMON @pType='E010', @pCODE = 'EIS001', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);//EIS적용	

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string strQuery = " usp_SBA009  @pTYPE = 'S1'";
                    strQuery += ", @pENT_CD = '" + txtEntCd.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        string ItemCd = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text;
                        string Dtype = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "부문")].Text;
                        int ItemCnt = 1;
                        int SpanCnt = 1;

                        fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "품목"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                        fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "부문"), FarPoint.Win.Spread.Model.MergePolicy.Always);
						
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion                
        
        #region 프로젝트 팝업(수주참조)
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                SBA009P1 frm = new SBA009P1(txtProjectNo.Text);
                frm.ShowDialog();

                if (frm.DialogResult == DialogResult.OK)
                {
                    txtProjectNo.Text = frm.strProjectNo;
                    txtProjectNm.Value = frm.strProjectNm;
                    txtProjectSeq.Text = frm.strProjectSeq;
                    txtEntCd.Text = frm.strEntCd;
                    txtEntNm.Value = frm.strEntNm;
                    txtShipCd.Value = frm.strShipCd;
                    txtShipNm.Value = frm.strShipNm;
                    txtItemCd.Text = frm.strItemCd;
                    txtItemNm.Value = frm.strItemNm;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수주참조 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region  텍스트박스 체인지
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtEntCd_TextChanged(object sender, EventArgs e)
        {
            txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

    }
}
