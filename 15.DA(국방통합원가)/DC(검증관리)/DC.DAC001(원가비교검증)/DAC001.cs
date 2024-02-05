#region DAC001 작성 정보
/*************************************************************/
// 단위업무명 : 원가비교 검증
// 작 성 자 :   유재규
// 작 성 일 :   2013-05-21
// 작성내용 :   
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 : 
// 참    고 : 
/*************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DC.DAC001
{
    public partial class DAC001 : UIForm.FPCOMM3
    {
        #region DAC001_Load

        public DAC001()
        {
            InitializeComponent();
        }
       
        private void DAC001_Load(object sender, EventArgs e)
        {
            // groupBox1 컨트롤 셋팅
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
        }

        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수조회조건 체크
                {
                    if (Search_Chk() == false) return;

                    /* 관리원가 조회 */
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                    #region 관리원가 조회
                    string strSql = " usp_DAC001  ";
                    strSql += "  @pTYPE = 'S2'";
                    strSql += ", @pCUST_ORDER_ID = '" + txtM_CUST_ORDER_ID.Text.ToString() + "' ";
                    strSql += ", @pCUST_ORDER_LINE_NO ='" + txtM_CUST_ORDER_LINE.Text.ToString() + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strSql, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);

                    fpSpread2.ActiveSheet.SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx2, "구분"), FarPoint.Win.Spread.Model.MergePolicy.Always);

                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();

                        if (i > 8) // 재료비 row부터 숫자컬럼 처리
                        {
                            num.Separator = ",";
                            num.ShowSeparator = true;
                            fpSpread2.Sheets[0].Cells[i, 3].CellType = num;

                            // 색깔처리
                            if (StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "명세")].Text.ToString(), "합계") == true)
                            {
                                fpSpread2.Sheets[0].Cells[i, 2].BackColor = Color.Bisque;
                                fpSpread2.Sheets[0].Cells[i, 3].BackColor = Color.Bisque;
                            }
                            if (StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "구분")].Text.ToString(), "제조원가") == true
                                || StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "구분")].Text.ToString(), "총원가") == true)
                            {
                                fpSpread2.Sheets[0].Rows[i].BackColor = Color.DarkSalmon;
                            }
                            if (StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "구분")].Text.ToString(), "손익") == true
                                || StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "구분")].Text.ToString(), "매출액") == true
                                || StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "구분")].Text.ToString(), "이윤율") == true)
                            {
                                fpSpread2.Sheets[0].Rows[i].BackColor = Color.Khaki;
                            }

                        }
                        else
                        {
                            fpSpread2.Sheets[0].Rows[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                        }

                        fpSpread2.ActiveSheet.SetRowMerge(i, FarPoint.Win.Spread.Model.MergePolicy.Always);
                    }
                    #endregion

                    #region 방산원가 조회
                    strSql = " usp_UA010_R01 ";
                    strSql += "  @pTYPE   = 'I1'";
                    strSql += ", @pCUST_ORDER_CON_ID = '" + txtM_CUST_ORDER_ID.Text.ToString() + "' ";
                    strSql += ", @pLINE = '" + txtM_CUST_ORDER_LINE.Text.ToString() + "' ";
                    strSql += ", @pCUST_ORDER_ID = '" + txtM_CUST_ORDER_ID2.Text.ToString() + "' ";
                    strSql += ", @pORDR_YEAR = '" + txtM_REQ_YEAR.Text.ToString() + "' ";
                    strSql += ", @pDCSN_NUMB = '" + txtM_REQ_NO.Text.ToString() + "' ";
                    // strSql += ", @pQTY = '" + txtM_QTY.Text.ToString() + "' ";
                    strSql += ", @pFLAG ='V' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    fpSpread1.ActiveSheet.SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "구분"), FarPoint.Win.Spread.Model.MergePolicy.Always);

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();

                        if (i > 8) // 재료비 row부터 숫자컬럼 처리
                        {
                            num.DecimalSeparator = ".";
                            num.DecimalPlaces = 2;
                            num.Separator = ",";
                            num.ShowSeparator = true;
                            fpSpread1.Sheets[0].Cells[i, 3].CellType = num;

                            // 색깔처리
                            if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "명세")].Text.ToString(), "합계") == true)
                            {
                                fpSpread1.Sheets[0].Cells[i, 2].BackColor = Color.Bisque;
                                fpSpread1.Sheets[0].Cells[i, 3].BackColor = Color.Bisque;
                            }
                            if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text.ToString(), "제조원가") == true
                                || StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text.ToString(), "총원가") == true)
                            {
                                fpSpread1.Sheets[0].Rows[i].BackColor = Color.DarkSalmon;
                            }
                            if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text.ToString(), "투하자본보상비") == true
                                || StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "명세")].Text.ToString(), "소계") == true)
                            {
                                fpSpread1.Sheets[0].Rows[i].BackColor = Color.LightCyan;
                            }
                            if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text.ToString(), "계산가격") == true
                                || StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text.ToString(), "이윤율") == true)
                            {
                                fpSpread1.Sheets[0].Rows[i].BackColor = Color.Khaki;
                            }
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Rows[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                        }

                        fpSpread1.ActiveSheet.SetRowMerge(i, FarPoint.Win.Spread.Model.MergePolicy.Always);
                    }

                    #endregion

                    #region 관리원가,방산원가 비교조회
                    strSql = " usp_DAC001  ";
                    strSql += "  @pTYPE = 'S3'";

                    UIForm.FPMake.grdCommSheet(fpSpread3, strSql, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, true, 0, 0);

                    // 색깔처리
                    for (int i = 0; i < fpSpread3.Sheets[0].Rows.Count; i++)
                    {
                        if (StringMatch(fpSpread3.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text.ToString(), "제조원가") == true
                            || StringMatch(fpSpread3.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text.ToString(), "총원가") == true)
                        {
                            fpSpread3.Sheets[0].Rows[i].BackColor = Color.DarkSalmon;
                        }
                        if (StringMatch(fpSpread3.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text.ToString(), "투하자본보상비") == true
                            || StringMatch(fpSpread3.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text.ToString(), "기타금액") == true)
                        {
                            fpSpread3.Sheets[0].Rows[i].BackColor = Color.LightCyan;
                        }
                        if (StringMatch(fpSpread3.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text.ToString(), "계산가격") == true
                            || StringMatch(fpSpread3.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text.ToString(), "이윤") == true)
                        {
                            fpSpread3.Sheets[0].Rows[i].BackColor = Color.Khaki;
                        }
                    }
                    #endregion

                    #region 관리원가비교 셋팅
                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        //재료비
                        if (StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "명세")].Text.ToString(), "재료비 합계") == true)
                        {
                            fpSpread3.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx3, "관리원가")].Text = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "금액")].Text;
                        }
                        //노무비
                        if (StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "명세")].Text.ToString(), "노무비 합계") == true)
                        {
                            fpSpread3.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx3, "관리원가")].Text = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "금액")].Text;
                        }
                        //경비
                        if (StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "명세")].Text.ToString(), "경비 합계") == true)
                        {
                            fpSpread3.Sheets[0].Cells[2, SystemBase.Base.GridHeadIndex(GHIdx3, "관리원가")].Text = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "금액")].Text;
                        }
                        //제조원가
                        if (StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "명세")].Text.ToString(), "제조원가") == true)
                        {
                            fpSpread3.Sheets[0].Cells[3, SystemBase.Base.GridHeadIndex(GHIdx3, "관리원가")].Text = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "금액")].Text;
                        }
                        //일반관리비
                        if (StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "명세")].Text.ToString(), "일반관리비") == true)
                        {
                            fpSpread3.Sheets[0].Cells[4, SystemBase.Base.GridHeadIndex(GHIdx3, "관리원가")].Text = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "금액")].Text;
                        }
                        //총원가
                        if (StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "명세")].Text.ToString(), "총원가") == true)
                        {
                            fpSpread3.Sheets[0].Cells[5, SystemBase.Base.GridHeadIndex(GHIdx3, "관리원가")].Text = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "금액")].Text;
                        }
                        //이윤
                        if (StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "명세")].Text.ToString(), "손익") == true)
                        {
                            fpSpread3.Sheets[0].Cells[6, SystemBase.Base.GridHeadIndex(GHIdx3, "관리원가")].Text = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "금액")].Text;
                        }
                        //투하자본 보상비 7
                        //기타금액 8
                        //매출액
                        if (StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "명세")].Text.ToString(), "매출액") == true)
                        {
                            fpSpread3.Sheets[0].Cells[9, SystemBase.Base.GridHeadIndex(GHIdx3, "관리원가")].Text = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "금액")].Text;
                        }
                        //이윤율
                        if (StringMatch(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "명세")].Text.ToString(), "이윤율") == true)
                        {
                            fpSpread3.Sheets[0].Cells[10, SystemBase.Base.GridHeadIndex(GHIdx3, "관리원가")].Text = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "금액")].Text;
                        }
                    }
                    #endregion

                    #region 방산원가비교 셋팅
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        //재료비
                        if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "명세")].Text.ToString(), "재료비 합계") == true)
                        {
                            fpSpread3.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx3, "방산원가")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text;
                        }
                        //노무비
                        if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "명세")].Text.ToString(), "노무비 합계") == true)
                        {
                            fpSpread3.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx3, "방산원가")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text;
                        }
                        //경비
                        if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "명세")].Text.ToString(), "경비 합계") == true)
                        {
                            fpSpread3.Sheets[0].Cells[2, SystemBase.Base.GridHeadIndex(GHIdx3, "방산원가")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text;
                        }
                        //제조원가
                        if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "명세")].Text.ToString(), "제조원가") == true)
                        {
                            fpSpread3.Sheets[0].Cells[3, SystemBase.Base.GridHeadIndex(GHIdx3, "방산원가")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text;
                        }
                        //일반관리비
                        if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "명세")].Text.ToString(), "일반관리비") == true)
                        {
                            fpSpread3.Sheets[0].Cells[4, SystemBase.Base.GridHeadIndex(GHIdx3, "방산원가")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text;
                        }
                        //총원가
                        if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "명세")].Text.ToString(), "총원가") == true)
                        {
                            fpSpread3.Sheets[0].Cells[5, SystemBase.Base.GridHeadIndex(GHIdx3, "방산원가")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text;
                        }
                        //이윤
                        if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "명세")].Text.ToString(), "이윤 소계") == true)
                        {
                            fpSpread3.Sheets[0].Cells[6, SystemBase.Base.GridHeadIndex(GHIdx3, "방산원가")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text;
                        }
                        //투하자본보상비
                        if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "명세")].Text.ToString(), "투하자본보상비") == true)
                        {
                            fpSpread3.Sheets[0].Cells[7, SystemBase.Base.GridHeadIndex(GHIdx3, "방산원가")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text;
                        }
                        //기타금액 
                        if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "명세")].Text.ToString(), "기타 소계") == true)
                        {
                            fpSpread3.Sheets[0].Cells[8, SystemBase.Base.GridHeadIndex(GHIdx3, "방산원가")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text;
                        }
                        //매출액
                        if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "명세")].Text.ToString(), "계산가격") == true)
                        {
                            fpSpread3.Sheets[0].Cells[9, SystemBase.Base.GridHeadIndex(GHIdx3, "방산원가")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text;
                        }
                        //이윤율
                        if (StringMatch(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "명세")].Text.ToString(), "이윤율") == true)
                        {
                            fpSpread3.Sheets[0].Cells[10, SystemBase.Base.GridHeadIndex(GHIdx3, "방산원가")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text;
                        }
                    }
                    #endregion

                    #region 비교 차이셋팅
                    for (int i = 0; i < fpSpread3.Sheets[0].Rows.Count - 1; i++)
                    {
                        fpSpread3.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx3, "차이")].Value =
                            SystemBase.Validation.Decimal_Data(fpSpread3.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx3, "관리원가")].Text.ToString(), ",")
                                - SystemBase.Validation.Decimal_Data(fpSpread3.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx3, "방산원가")].Text.ToString(), ",");
                    }
                    #endregion

                    this.Cursor = System.Windows.Forms.Cursors.Default;
                }
            }
            catch (Exception f)
            {
                this.Cursor = System.Windows.Forms.Cursors.Default;
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnCUST_ORDER_ID_Click(계약번호)
        private void btnCUST_ORDER_ID_Click(object sender, EventArgs e)
        {
            try
            {
                DAC001P1 pu = new DAC001P1();
                pu.ShowDialog();
                if (pu.RETURN > 0)
                {
                    txtM_CUST_ORDER_ID.Value = pu.CUSTOMER_ORDER_ID[0].ToString();
                    txtM_CUST_ORDER_LINE.Value = pu.CUSTOMER_ORDER_LINE[0].ToString();
                    txtM_PART_ID.Value = pu.PART_ID[0].ToString();
                    txtM_PART_NAME.Value = pu.PART_NAME[0].ToString();
                    txtM_PROJECT_ID.Value = pu.PROJECT_ID[0].ToString();
                    txtM_PROJECT_NAME.Value = pu.PROJECT_NAME[0].ToString();
                    txtM_REQ_YEAR.Value = pu.REQ_YEAR[0].ToString();
                    txtM_REQ_NO.Value = pu.REQ_NO[0].ToString();
                    txtM_REQ_DEPT.Value = pu.REQ_DEPT[0].ToString();
                    txtM_CUST_ORDER_ID2.Value = pu.CUST_ORDER_ID[0].ToString();
                    txtM_QTY.Value = pu.QTY[0].ToString();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Search_Chk 비교조건 체크
        private bool Search_Chk()
        {
            bool Search_Chk = true;

            string ERRCode = "";
            string MSGCode = "";
            try
            {
                string strSql = " usp_DAC001 ";
                strSql += "  @pTYPE   = 'S4'";
                strSql += ", @pCUST_ORDER_ID = '" + txtM_CUST_ORDER_ID.Text.ToString() + "' ";
                strSql += ", @pCUST_ORDER_LINE_NO = '" + txtM_CUST_ORDER_LINE.Text.ToString() + "' ";
                strSql += ", @pDCSN_NUMB = '" + txtM_REQ_NO.Text.ToString() + "' ";
                strSql += ", @pCUST_ORDER_ID2 = '" + txtM_CUST_ORDER_ID2.Text.ToString() + "' ";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();
                }
                else
                {
                    ERRCode = "WR" ;
                    MSGCode = "자료가 존재하지 않습니다.";
                }                

                if (ERRCode != "OK")  goto Exit; 
               
            }
            catch (Exception)
            {                
                Search_Chk = false;
            }
        Exit: 

            if (ERRCode == "WR")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Search_Chk = false;
            }

            return Search_Chk;

        }
        #endregion

        private bool StringMatch(string sSentences, string sPattern)
        {
            bool result = false;

            string[] Sentences = { sSentences };

            foreach (string s in Sentences)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(s, sPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
