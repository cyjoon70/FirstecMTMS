#region 작성정보
/*********************************************************************/
// 단위업무명 : SCHEDULE 전개
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-28
// 작성내용 : SCHEDULE 전개 및 관리
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

namespace PB.PSA010
{
    public partial class PSA010P3 : Form
    {
        public PSA010P3()
        {
            InitializeComponent();
        }

        // 그리드 정보
        private string[] strPHeadText = new string[] { };
        private string[] strPTxtAlign = new string[] { };
        private string[] strPCellType = new string[] { };
        int[] strHeadWidth = new int[] { };

        string[] PHeadText = null;
        string[] PTxtAlign = null;
        string[] PCellType = null;
        int[] PHeadWidth = null;

        private void PSA010P3_Load(object sender, EventArgs e)
        {
            try
            {
                if (SystemBase.Base.ProgramWhere.Length > 0)
                {

                    string HeadQuery = "";
                    string itemFrmId = "BBI001"; // 품목 FORM ID

                    HeadQuery += " SELECT HEAD_ONE, DATA_ALIGN, DATA_TYPE, HEAD_WIDTH ";
                    HeadQuery += " FROM   CO_GRID_DESIGN ";
                    HeadQuery += " WHERE  FORM_ID='" + itemFrmId + "'";
                    HeadQuery += " ORDER BY DATA_SEQ ";

                    DataTable dt = SystemBase.DbOpen.TranDataTable(HeadQuery);
                    int G1RowCount = dt.Rows.Count;

                    if (G1RowCount > 0)
                    {
                        PHeadText = new string[G1RowCount];
                        PTxtAlign = new string[G1RowCount];
                        PCellType = new string[G1RowCount];
                        PHeadWidth = new int[G1RowCount];

                        for (int i = 0; i < G1RowCount; i++)
                        {
                            PHeadText[i] = dt.Rows[i][0].ToString();
                            PTxtAlign[i] = dt.Rows[i][1].ToString();
                            PCellType[i] = dt.Rows[i][2].ToString();
                            PHeadWidth[i] = Convert.ToInt32(dt.Rows[i][3].ToString());
                        }
                    }
                }

                // 그리드 HEAD정보 설정
                strPHeadText = PHeadText;
                strPTxtAlign = PTxtAlign;
                strPCellType = PCellType;
                strHeadWidth = PHeadWidth;

                // 공정확인조회
                string Query = " usp_PSA010 @pType='C2' ";
                Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdMakeSheet(fpSpread1, Query, strPHeadText, strPTxtAlign, strPCellType, strHeadWidth);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                fpSpread1.AutoClipboard = true;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
