
#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목정보조회
// 작 성 자 : 김 현근
// 작 성 일 : 2013-04-24
// 작성내용 : 품목정보조회
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

namespace BI.BBI004
{
    public partial class BBI004 : UIForm.FPCOMM1
    {

        #region 생성자
        public BBI004()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBI004_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용

            //콤보박스 세팅
            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //품목계정
            SystemBase.ComboMake.C1Combo(cboItemGrp1, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B037', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //품목그룹1
            SystemBase.ComboMake.C1Combo(cboItemGrp2, "usp_B_COMMON @pTYPE='REL', @pCODE = 'B038', @pSPEC1 = '" + cboItemGrp1.SelectedValue.ToString() + "', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);//품목그룹2

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            //기타 세팅
            dtpDate.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpDate.Value = SystemBase.Base.ServerTime("YYMMDD");

            rdoE.Checked = true;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strAcct = ""; if (cboItemAcct.Text != "") strAcct = cboItemAcct.SelectedValue.ToString();
                string strItemGrp1 = ""; if (cboItemGrp1.Text != "") strItemGrp1 = cboItemGrp1.SelectedValue.ToString();
                string strItemGrp2 = ""; if (cboItemGrp2.Text != "") strItemGrp2 = cboItemGrp2.SelectedValue.ToString();

                string strQuery = " usp_BBI004  @pTYPE = 'S1'";
                strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                strQuery += ", @pITEM_NM ='" + txtItemNm.Text + "'";

                // 2017.03.17. hma 추가(Start): 표준품목여부 검색조건 값 지정 
                string strStdItemYN = "";
                if (rdoStdItemY.Checked == true) { strStdItemYN = "Y"; }
                else if (rdoStdItemN.Checked == true) { strStdItemYN = "N"; }

                if ((strStdItemYN == "Y") && (strAcct != "30"))
                {
                    MessageBox.Show("표준품목여부가 Y인 품목계정은 원자재만 가능합니다. 원자재만 조회합니다.");
                    strAcct = "30";
                    cboItemAcct.SelectedValue = "30";       // 화면의 품목계정을 원자재로 변경
                }
                strQuery += ", @pSTD_ITEM_YN = '" + strStdItemYN + "' ";
                // 2017.03.17. hma 추가(End)

                strQuery += ", @pITEM_ACCT ='" + strAcct + "'";
                strQuery += ", @pITEM_SPEC ='" + txtItemSpec.Text.Trim() + "'";
                strQuery += ", @pITEM_GRP1 ='" + strItemGrp1 + "' ";
                strQuery += ", @pITEM_GRP2 ='" + strItemGrp2 + "' ";
                strQuery += ", @pDRAW_NO ='" + txtDrawNo.Text.Trim() + "'";
                strQuery += ", @pDATE = '" + dtpDate.Text + "'";
                strQuery += ", @pNIIN = '" + txtKkjgbh.Text + "'";

                string BomFlag = "";
                if (rdoS.Checked == true)
                {
                    BomFlag = "S";
                }
                else if (rdoD.Checked == true)
                {
                    BomFlag = "D";
                }
                else if (rdoA.Checked == true)
                {
                    BomFlag = "A";
                }
                else if (rdoE.Checked == true)
                {
                    BomFlag = "E";
                }

                strQuery += ", @pBOM_FLAG = '" + BomFlag + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 5, true);

				// 품질증빙 문서구분에 따른 칼럼 속성 설정
				DataTable dtExcel = SystemBase.DbOpen.NoTranDataTable(strQuery);

				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					for (int i = 21; i <= fpSpread1.Sheets[0].Columns.Count - 1; i++)
					{
						if (i == 21 || i == 22)
						{
							fpSpread1.Sheets[0].Columns[21].Locked = true;
							fpSpread1.Sheets[0].Columns[22].Locked = true;
						}

						fpSpread1.Sheets[0].Columns[23].Visible = false;
						fpSpread1.Sheets[0].ColumnHeader.Cells[0, 23].Text = "품질증빙 설정키값";

						if (i > 23)
						{
							// 자동 생성 칼럼의 경우 아래처럼 칼럼명을 지정해주어야 엑셀 다운로드에서 해당 칼럼명이 보임
							fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text = dtExcel.Columns[i].ColumnName;

							fpSpread1.Sheets[0].Columns[i].Locked = true;
							fpSpread1.Sheets[0].Columns[i].BackColor = Color.White;
							fpSpread1.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
							fpSpread1.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
						}
					}
				}
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 콤보 대,중,소분류 관련       
        private void cboItemGrp1_RowChange(object sender, EventArgs e)
        {
            if (cboItemGrp1.SelectedValue.ToString() != "")
            {
                string strItemLvl1 = cboItemGrp1.SelectedValue.ToString();
                SystemBase.ComboMake.C1Combo(cboItemGrp2, "usp_B_COMMON @pType='REL', @pCODE = 'B038', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pSPEC1='" + strItemLvl1 + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            }
            cboItemGrp2.Text = "";
        }
        private void cboItemGrp2_Click(object sender, EventArgs e)
        {
            if (cboItemGrp1.SelectedValue.ToString() == "")
            {
                MessageBox.Show("그룹1을 먼저 선택하셔야 합니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        #endregion

    }
}
