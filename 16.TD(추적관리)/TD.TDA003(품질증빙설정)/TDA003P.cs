#region 작성정보
/*********************************************************************/
// 단위업무명 : 라우팅정보출력
// 작 성 자 : 김 한 진
// 작 성 일 : 2013-01-30
// 작성내용 : 라우팅정보 출력
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
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using EDocument;
using EDocument.Spread;
using EDocument.Network;
using EDocument.Extensions.C1ComboExtension;
using EDocument.Extensions.FpSpreadExtension;

#region 예제 - 복사해서 쓰세요
/*
try
{
    WNDW.TDA003P pu = new WNDW.TDA003P();
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
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace TDA003P
{
    /// <summary>
    /// 품목정보조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 품목코드 </para>
    /// <para>Msgs[2] = 품목명 </para>
    /// <para>Msgs[3] = 품목전명 </para>
    /// <para>Msgs[4] = 품목계정 </para>
    /// <para>Msgs[5] = 품목규격 </para>
    /// <para>Msgs[6] = 품목단위 </para>
    /// </summary>

    public partial class TDA003P : UIForm.FPCOMM1
    {
        #region 변수선언
        string strAcct = "";

        string strItemCd = "";
        #endregion

        #region 생성자
        public TDA003P(string ItemCd)
        {
            //품목계정값 10-제품, 20-반제품, 25-재공품, 30-원자재, 33-저장품, 35-부자재, 50-상품, 60-포장재, 70-공구소모품, CUST- 거래처품목

            strItemCd = ItemCd;

            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void TDA003P_Load(object sender, System.EventArgs e)
        {
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공정타입")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P028', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            SearchExec();
        }
        #endregion



        #region 그리드 조회
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string query = "usp_TDA003 'S4'"
                    + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                    + ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "'"
                    + ", @pITEM_CD = '" + strItemCd + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

                string mainquery = "usp_TDA003 'S5'"
                   + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                   + ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "'"
                   + ", @pITEM_CD = '" + strItemCd + "'";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(mainquery);

                if (fpSpread1.Sheets[0].RowCount > 0)
                {
                    string MAINROUT = dt.Rows[0]["ROUT_NO"].ToString();

                    for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "라우팅번호")].Value.ToString() == MAINROUT)
                        {
                            Row oRow = fpSpread1.Sheets[0].Rows[i];
                            oRow.SetApprearance(CellAppearance.Discard);
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
    }
}
