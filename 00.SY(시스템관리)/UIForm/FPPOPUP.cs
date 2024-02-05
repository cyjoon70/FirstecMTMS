using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using UIForm.Extension.FpSpread;

namespace UIForm
{
    public partial class FPPOPUP : System.Windows.Forms.Form
    {
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수
        bool TopChange = false;	//TopChange 사용여부
        string TmpQuery = "";	//Query

        string returnVal = "";
        string[] returnValue = null;
        string strQuery = "";
        string[] strWhere = new string[] { };
        string[] strSearch = new string[] { };
        string[] strPHeadText = new string[] { };
        string[] strPTxtAlign = new string[] { };
        string[] strPCellType = new string[] { };
        int[] strHeadWidth = new int[] { };

        string[] PHeadText = null;
        string[] PTxtAlign = null;
        string[] PCellType = null;
        int[] PHeadWidth = null;
        private FarPoint.Win.Spread.FpSpread fpSpread5;
        int fpRow = -1;

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.Label lblEtc1;
        private System.Windows.Forms.TextBox txtEtc1;
        private System.Windows.Forms.Label lblEtc2;
        private System.Windows.Forms.TextBox txtEtc2;
        private System.Windows.Forms.Label lblEtc3;
        private System.Windows.Forms.TextBox txtEtc3;
        private System.Windows.Forms.Label lblEtc4;
        private System.Windows.Forms.TextBox txtEtc4;

        public FPPOPUP(string strFORM_ID, string Query, string[] Where, string[] Search, int[] PSearchLabel)
        {
            InitializeComponent();

            if (SystemBase.Base.ProgramWhere.Length > 0)
            {
                string HeadQuery = " SELECT HEAD_ONE, DATA_ALIGN, DATA_TYPE, HEAD_WIDTH  FROM CO_GRID_DESIGN WHERE FORM_ID='" + strFORM_ID.ToString() + "' ORDER BY DATA_SEQ ";
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

            if (PSearchLabel.Length > 0)
            {
                lblCode.Text = PHeadText[PSearchLabel[0]].ToString();
                panel1.Height = 56;
            }
            if (PSearchLabel.Length > 1)
            {
                lblCodeName.Text = PHeadText[PSearchLabel[1]].ToString();
                panel1.Height = 80;
                lblCodeName.Visible = true;
                txtCodeName.Visible = true;
            }
            if (PSearchLabel.Length > 2)
            {
                if (PHeadText.Length > 2)
                    lblEtc1.Text = PHeadText[PSearchLabel[2]].ToString();
                panel1.Height = 104;
                lblEtc1.Visible = true;
                txtEtc1.Visible = true;
            }
            if (PSearchLabel.Length > 3)
            {
                if (PHeadText.Length > 3)
                    lblEtc2.Text = PHeadText[PSearchLabel[3]].ToString();
                panel1.Height = 128;
                lblEtc2.Visible = true;
                txtEtc2.Visible = true;
            }
            if (PSearchLabel.Length > 4)
            {
                if (PHeadText.Length > 4)
                    lblEtc3.Text = PHeadText[PSearchLabel[4]].ToString();
                panel1.Height = 152;
                lblEtc3.Visible = true;
                txtEtc3.Visible = true;
            }
            if (PSearchLabel.Length > 5)
            {
                if (PHeadText.Length > 5)
                    lblEtc4.Text = PHeadText[PSearchLabel[5]].ToString();
                panel1.Height = 176;
                lblEtc4.Visible = true;
                txtEtc4.Visible = true;
            }

            if (Search.Length > 0)
                txtCode.Text = Search[0].ToString();
            if (Search.Length > 1)
                txtCodeName.Text = Search[1].ToString();
            if (Search.Length > 2)
                txtEtc1.Text = Search[2].ToString();
            if (Search.Length > 3)
                txtEtc2.Text = Search[3].ToString();
            if (Search.Length > 4)
                txtEtc3.Text = Search[4].ToString();
            if (Search.Length > 5)
                txtEtc4.Text = Search[5].ToString();

            strQuery = Query;
            strWhere = Where;
            strSearch = Search;
            strPHeadText = PHeadText;
            strPTxtAlign = PTxtAlign;
            strPCellType = PCellType;
            strHeadWidth = PHeadWidth;

            TmpQuery = strQuery;

            if (strWhere.Length > 0 && strWhere[0].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[0].ToString() + "='" + txtCode.Text + "'";
            if (strWhere.Length > 1 && strWhere[1].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[1].ToString() + "='" + txtCodeName.Text + "'";
            if (strWhere.Length > 2 && strWhere[2].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[2].ToString() + "='" + txtEtc1.Text + "'";
            if (strWhere.Length > 3 && strWhere[3].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[3].ToString() + "='" + txtEtc2.Text + "'";
            if (strWhere.Length > 4 && strWhere[4].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[4].ToString() + "='" + txtEtc3.Text + "'";
            if (strWhere.Length > 5 && strWhere[5].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[5].ToString() + "='" + txtEtc4.Text + "'";

            FPMake.grdMakeSheet(fpSpread1, TmpQuery, strPHeadText, strPTxtAlign, strPCellType, strHeadWidth);
            RtnStr(0);
        }


        public FPPOPUP(string strFORM_ID, string Query, string[] Where, string[] Search, int[] PSearchLabel, bool TopChangeEV)
        {
            InitializeComponent();

            TopChange = TopChangeEV;

            if (SystemBase.Base.ProgramWhere.Length > 0)
            {
                string HeadQuery = " SELECT HEAD_ONE, DATA_ALIGN, DATA_TYPE, HEAD_WIDTH  FROM CO_GRID_DESIGN WHERE FORM_ID='" + strFORM_ID.ToString() + "' ORDER BY DATA_SEQ ";
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

            if (PSearchLabel.Length > 0)
            {
                lblCode.Text = PHeadText[PSearchLabel[0]].ToString();
                panel1.Height = 56;
            }
            if (PSearchLabel.Length > 1)
            {
                lblCodeName.Text = PHeadText[PSearchLabel[1]].ToString();
                panel1.Height = 80;
                lblCodeName.Visible = true;
                txtCodeName.Visible = true;
            }
            if (PSearchLabel.Length > 2)
            {
                if (PHeadText.Length > 2)
                    lblEtc1.Text = PHeadText[PSearchLabel[2]].ToString();
                panel1.Height = 104;
                lblEtc1.Visible = true;
                txtEtc1.Visible = true;
            }
            if (PSearchLabel.Length > 3)
            {
                if (PHeadText.Length > 3)
                    lblEtc2.Text = PHeadText[PSearchLabel[3]].ToString();
                panel1.Height = 128;
                lblEtc2.Visible = true;
                txtEtc2.Visible = true;
            }
            if (PSearchLabel.Length > 4)
            {
                if (PHeadText.Length > 4)
                    lblEtc3.Text = PHeadText[PSearchLabel[4]].ToString();
                panel1.Height = 152;
                lblEtc3.Visible = true;
                txtEtc3.Visible = true;
            }
            if (PSearchLabel.Length > 5)
            {
                if (PHeadText.Length > 5)
                    lblEtc4.Text = PHeadText[PSearchLabel[5]].ToString();
                panel1.Height = 176;
                lblEtc4.Visible = true;
                txtEtc4.Visible = true;
            }

            if (Search.Length > 0)
                txtCode.Text = Search[0].ToString();
            if (Search.Length > 1)
                txtCodeName.Text = Search[1].ToString();
            if (Search.Length > 2)
                txtEtc1.Text = Search[2].ToString();
            if (Search.Length > 3)
                txtEtc2.Text = Search[3].ToString();
            if (Search.Length > 4)
                txtEtc3.Text = Search[4].ToString();
            if (Search.Length > 5)
                txtEtc4.Text = Search[5].ToString();

            strQuery = Query;
            strWhere = Where;
            strSearch = Search;
            strPHeadText = PHeadText;
            strPTxtAlign = PTxtAlign;
            strPCellType = PCellType;
            strHeadWidth = PHeadWidth;

            TmpQuery = strQuery;

            if (strWhere.Length > 0 && strWhere[0].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[0].ToString() + "='" + txtCode.Text + "'";
            if (strWhere.Length > 1 && strWhere[1].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[1].ToString() + "='" + txtCodeName.Text + "'";
            if (strWhere.Length > 2 && strWhere[2].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[2].ToString() + "='" + txtEtc1.Text + "'";
            if (strWhere.Length > 3 && strWhere[3].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[3].ToString() + "='" + txtEtc2.Text + "'";
            if (strWhere.Length > 4 && strWhere[4].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[4].ToString() + "='" + txtEtc3.Text + "'";
            if (strWhere.Length > 5 && strWhere[5].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[5].ToString() + "='" + txtEtc4.Text + "'";

            string TmpQuery2 = "";
            if (TopChange)
                TmpQuery2 = TmpQuery + ", @pTOPCOUNT ='" + AddRow + "'";
            else
                TmpQuery2 = TmpQuery;

            FPMake.grdMakeSheet(fpSpread1, TmpQuery2, strPHeadText, strPTxtAlign, strPCellType, strHeadWidth);
            RtnStr(0);
        }


        public FPPOPUP(string strFORM_ID, string Query, string[] Where, string[] Search, int[] PSearchLabel, string TitleText)
        {
            InitializeComponent();

            this.Text = TitleText;
            if (SystemBase.Base.ProgramWhere.Length > 0)
            {
                string HeadQuery = " SELECT HEAD_ONE, DATA_ALIGN, DATA_TYPE, HEAD_WIDTH  FROM CO_GRID_DESIGN WHERE FORM_ID='" + strFORM_ID.ToString() + "' ORDER BY DATA_SEQ ";
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

            if (PSearchLabel.Length > 0)
            {
                lblCode.Text = PHeadText[PSearchLabel[0]].ToString();
                panel1.Height = 56;
            }
            if (PSearchLabel.Length > 1)
            {
                lblCodeName.Text = PHeadText[PSearchLabel[1]].ToString();
                panel1.Height = 80;
                lblCodeName.Visible = true;
                txtCodeName.Visible = true;
            }
            if (PSearchLabel.Length > 2)
            {
                if (PHeadText.Length > 2)
                    lblEtc1.Text = PHeadText[PSearchLabel[2]].ToString();
                panel1.Height = 104;
                lblEtc1.Visible = true;
                txtEtc1.Visible = true;
            }
            if (PSearchLabel.Length > 3)
            {
                if (PHeadText.Length > 3)
                    lblEtc2.Text = PHeadText[PSearchLabel[3]].ToString();
                panel1.Height = 128;
                lblEtc2.Visible = true;
                txtEtc2.Visible = true;
            }
            if (PSearchLabel.Length > 4)
            {
                if (PHeadText.Length > 4)
                    lblEtc3.Text = PHeadText[PSearchLabel[4]].ToString();
                panel1.Height = 152;
                lblEtc3.Visible = true;
                txtEtc3.Visible = true;
            }
            if (PSearchLabel.Length > 5)
            {
                if (PHeadText.Length > 5)
                    lblEtc4.Text = PHeadText[PSearchLabel[5]].ToString();
                panel1.Height = 176;
                lblEtc4.Visible = true;
                txtEtc4.Visible = true;
            }

            if (Search.Length > 0)
                txtCode.Text = Search[0].ToString();
            if (Search.Length > 1)
                txtCodeName.Text = Search[1].ToString();
            if (Search.Length > 2)
                txtEtc1.Text = Search[2].ToString();
            if (Search.Length > 3)
                txtEtc2.Text = Search[3].ToString();
            if (Search.Length > 4)
                txtEtc3.Text = Search[4].ToString();
            if (Search.Length > 5)
                txtEtc4.Text = Search[5].ToString();

            strQuery = Query;
            strWhere = Where;
            strSearch = Search;
            strPHeadText = PHeadText;
            strPTxtAlign = PTxtAlign;
            strPCellType = PCellType;
            strHeadWidth = PHeadWidth;

            TmpQuery = strQuery;

            if (strWhere.Length > 0 && strWhere[0].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[0].ToString() + "='" + txtCode.Text + "'";
            if (strWhere.Length > 1 && strWhere[1].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[1].ToString() + "='" + txtCodeName.Text + "'";
            if (strWhere.Length > 2 && strWhere[2].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[2].ToString() + "='" + txtEtc1.Text + "'";
            if (strWhere.Length > 3 && strWhere[3].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[3].ToString() + "='" + txtEtc2.Text + "'";
            if (strWhere.Length > 4 && strWhere[4].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[4].ToString() + "='" + txtEtc3.Text + "'";
            if (strWhere.Length > 5 && strWhere[5].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[5].ToString() + "='" + txtEtc4.Text + "'";

            FPMake.grdMakeSheet(fpSpread1, TmpQuery, strPHeadText, strPTxtAlign, strPCellType, strHeadWidth);
            RtnStr(0);

            //기능삭제 : 데이터 한건일때 바로 return받기 위해 만듬 2013-01-23
            //if (fpSpread1.Sheets[0].Rows.Count == 1)
            //{
            //    RtnStr(0);
            //    this.DialogResult = DialogResult.OK;
            //    btnClose_Click(null, null);
            //} 
        }


        public FPPOPUP(string strFORM_ID, string Query, string[] Where, string[] Search, int[] PSearchLabel, string TitleText, bool TopChangeEV)
        {
            InitializeComponent();

            TopChange = TopChangeEV;

            this.Text = TitleText;
            if (SystemBase.Base.ProgramWhere.Length > 0)
            {
                string HeadQuery = " SELECT HEAD_ONE, DATA_ALIGN, DATA_TYPE, HEAD_WIDTH  FROM CO_GRID_DESIGN WHERE FORM_ID='" + strFORM_ID.ToString() + "' ORDER BY DATA_SEQ ";
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

            if (PSearchLabel.Length > 0)
            {
                lblCode.Text = PHeadText[PSearchLabel[0]].ToString();
                panel1.Height = 56;
            }
            if (PSearchLabel.Length > 1)
            {
                lblCodeName.Text = PHeadText[PSearchLabel[1]].ToString();
                panel1.Height = 80;
                lblCodeName.Visible = true;
                txtCodeName.Visible = true;
            }
            if (PSearchLabel.Length > 2)
            {
                if (PHeadText.Length > 2)
                    lblEtc1.Text = PHeadText[PSearchLabel[2]].ToString();
                panel1.Height = 104;
                lblEtc1.Visible = true;
                txtEtc1.Visible = true;
            }
            if (PSearchLabel.Length > 3)
            {
                if (PHeadText.Length > 3)
                    lblEtc2.Text = PHeadText[PSearchLabel[3]].ToString();
                panel1.Height = 128;
                lblEtc2.Visible = true;
                txtEtc2.Visible = true;
            }
            if (PSearchLabel.Length > 4)
            {
                if (PHeadText.Length > 4)
                    lblEtc3.Text = PHeadText[PSearchLabel[4]].ToString();
                panel1.Height = 152;
                lblEtc3.Visible = true;
                txtEtc3.Visible = true;
            }
            if (PSearchLabel.Length > 5)
            {
                if (PHeadText.Length > 5)
                    lblEtc4.Text = PHeadText[PSearchLabel[5]].ToString();
                panel1.Height = 176;
                lblEtc4.Visible = true;
                txtEtc4.Visible = true;
            }

            if (Search.Length > 0)
                txtCode.Text = Search[0].ToString();
            if (Search.Length > 1)
                txtCodeName.Text = Search[1].ToString();
            if (Search.Length > 2)
                txtEtc1.Text = Search[2].ToString();
            if (Search.Length > 3)
                txtEtc2.Text = Search[3].ToString();
            if (Search.Length > 4)
                txtEtc3.Text = Search[4].ToString();
            if (Search.Length > 5)
                txtEtc4.Text = Search[5].ToString();

            strQuery = Query;
            strWhere = Where;
            strSearch = Search;
            strPHeadText = PHeadText;
            strPTxtAlign = PTxtAlign;
            strPCellType = PCellType;
            strHeadWidth = PHeadWidth;

            TmpQuery = strQuery;

            if (strWhere.Length > 0 && strWhere[0].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[0].ToString() + "='" + txtCode.Text + "'";
            if (strWhere.Length > 1 && strWhere[1].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[1].ToString() + "='" + txtCodeName.Text + "'";
            if (strWhere.Length > 2 && strWhere[2].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[2].ToString() + "='" + txtEtc1.Text + "'";
            if (strWhere.Length > 3 && strWhere[3].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[3].ToString() + "='" + txtEtc2.Text + "'";
            if (strWhere.Length > 4 && strWhere[4].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[4].ToString() + "='" + txtEtc3.Text + "'";
            if (strWhere.Length > 5 && strWhere[5].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[5].ToString() + "='" + txtEtc4.Text + "'";

            string TmpQuery2 = "";
            if (TopChange)
                TmpQuery2 = TmpQuery + ", @pTOPCOUNT ='" + AddRow + "'";
            else
                TmpQuery2 = TmpQuery;

            FPMake.grdMakeSheet(fpSpread1, TmpQuery2, strPHeadText, strPTxtAlign, strPCellType, strHeadWidth);
            RtnStr(0);
        }

        public FPPOPUP(string strFORM_ID, string Query, string[] Where, string[] Search, string TitleText, int[] PSearchLabel, bool TopChangeEV)
        {	// 공통팝업 배열로 Return 090323 추가
            InitializeComponent();

            TopChange = TopChangeEV;

            this.Text = TitleText;
            if (SystemBase.Base.ProgramWhere.Length > 0)
            {
                string HeadQuery = " SELECT HEAD_ONE, DATA_ALIGN, DATA_TYPE, HEAD_WIDTH  FROM CO_GRID_DESIGN WHERE FORM_ID='" + strFORM_ID.ToString() + "' ORDER BY DATA_SEQ ";
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

            if (PSearchLabel.Length > 0)
            {
                lblCode.Text = PHeadText[PSearchLabel[0]].ToString();
                panel1.Height = 56;
            }
            if (PSearchLabel.Length > 1)
            {
                lblCodeName.Text = PHeadText[PSearchLabel[1]].ToString();
                panel1.Height = 80;
                lblCodeName.Visible = true;
                txtCodeName.Visible = true;
            }
            if (PSearchLabel.Length > 2)
            {
                if (PHeadText.Length > 2)
                    lblEtc1.Text = PHeadText[PSearchLabel[2]].ToString();
                panel1.Height = 104;
                lblEtc1.Visible = true;
                txtEtc1.Visible = true;
            }
            if (PSearchLabel.Length > 3)
            {
                if (PHeadText.Length > 3)
                    lblEtc2.Text = PHeadText[PSearchLabel[3]].ToString();
                panel1.Height = 128;
                lblEtc2.Visible = true;
                txtEtc2.Visible = true;
            }
            if (PSearchLabel.Length > 4)
            {
                if (PHeadText.Length > 4)
                    lblEtc3.Text = PHeadText[PSearchLabel[4]].ToString();
                panel1.Height = 152;
                lblEtc3.Visible = true;
                txtEtc3.Visible = true;
            }
            if (PSearchLabel.Length > 5)
            {
                if (PHeadText.Length > 5)
                    lblEtc4.Text = PHeadText[PSearchLabel[5]].ToString();
                panel1.Height = 176;
                lblEtc4.Visible = true;
                txtEtc4.Visible = true;
            }

            if (Search.Length > 0)
                txtCode.Text = Search[0].ToString();
            if (Search.Length > 1)
                txtCodeName.Text = Search[1].ToString();
            if (Search.Length > 2)
                txtEtc1.Text = Search[2].ToString();
            if (Search.Length > 3)
                txtEtc2.Text = Search[3].ToString();
            if (Search.Length > 4)
                txtEtc3.Text = Search[4].ToString();
            if (Search.Length > 5)
                txtEtc4.Text = Search[5].ToString();

            strQuery = Query;
            strWhere = Where;
            strSearch = Search;
            strPHeadText = PHeadText;
            strPTxtAlign = PTxtAlign;
            strPCellType = PCellType;
            strHeadWidth = PHeadWidth;

            TmpQuery = strQuery;

            if (strWhere.Length > 0 && strWhere[0].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[0].ToString() + "='" + txtCode.Text + "'";
            if (strWhere.Length > 1 && strWhere[1].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[1].ToString() + "='" + txtCodeName.Text + "'";
            if (strWhere.Length > 2 && strWhere[2].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[2].ToString() + "='" + txtEtc1.Text + "'";
            if (strWhere.Length > 3 && strWhere[3].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[3].ToString() + "='" + txtEtc2.Text + "'";
            if (strWhere.Length > 4 && strWhere[4].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[4].ToString() + "='" + txtEtc3.Text + "'";
            if (strWhere.Length > 5 && strWhere[5].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[5].ToString() + "='" + txtEtc4.Text + "'";

            string TmpQuery2 = "";
            if (TopChange)
                TmpQuery2 = TmpQuery + ", @pTOPCOUNT ='" + AddRow + "'";
            else
                TmpQuery2 = TmpQuery;

            FPMake.grdMakeSheet(fpSpread1, TmpQuery2, strPHeadText, strPTxtAlign, strPCellType, strHeadWidth);
            RtnString(0);
        }


        public FPPOPUP(string strFORM_ID, string Query, string[] Where, string[] Search, int[] PSearchLabel, string TitleText, FarPoint.Win.Spread.FpSpread baseGrid, int Row)
        {
            fpSpread5 = baseGrid;
            fpRow = Row;
            InitializeComponent();

            this.Text = TitleText;
            if (SystemBase.Base.ProgramWhere.Length > 0)
            {
                string HeadQuery = " SELECT HEAD_ONE, DATA_ALIGN, DATA_TYPE, HEAD_WIDTH  FROM CO_GRID_DESIGN WHERE FORM_ID='" + strFORM_ID.ToString() + "' ORDER BY DATA_SEQ ";
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

            if (PSearchLabel.Length > 0)
            {
                lblCode.Text = PHeadText[PSearchLabel[0]].ToString();
                panel1.Height = 56;
            }
            if (PSearchLabel.Length > 1)
            {
                lblCodeName.Text = PHeadText[PSearchLabel[1]].ToString();
                panel1.Height = 80;
                lblCodeName.Visible = true;
                txtCodeName.Visible = true;
            }
            if (PSearchLabel.Length > 2)
            {
                if (PHeadText.Length > 2)
                    lblEtc1.Text = PHeadText[PSearchLabel[2]].ToString();
                panel1.Height = 104;
                lblEtc1.Visible = true;
                txtEtc1.Visible = true;
            }
            if (PSearchLabel.Length > 3)
            {
                if (PHeadText.Length > 3)
                    lblEtc2.Text = PHeadText[PSearchLabel[3]].ToString();
                panel1.Height = 128;
                lblEtc2.Visible = true;
                txtEtc2.Visible = true;
            }
            if (PSearchLabel.Length > 4)
            {
                if (PHeadText.Length > 4)
                    lblEtc3.Text = PHeadText[PSearchLabel[4]].ToString();
                panel1.Height = 152;
                lblEtc3.Visible = true;
                txtEtc3.Visible = true;
            }
            if (PSearchLabel.Length > 5)
            {
                if (PHeadText.Length > 5)
                    lblEtc4.Text = PHeadText[PSearchLabel[5]].ToString();
                panel1.Height = 176;
                lblEtc4.Visible = true;
                txtEtc4.Visible = true;
            }

            if (Search.Length > 0)
                txtCode.Text = Search[0].ToString();
            if (Search.Length > 1)
                txtCodeName.Text = Search[1].ToString();
            if (Search.Length > 2)
                txtEtc1.Text = Search[2].ToString();
            if (Search.Length > 3)
                txtEtc2.Text = Search[3].ToString();
            if (Search.Length > 4)
                txtEtc3.Text = Search[4].ToString();
            if (Search.Length > 5)
                txtEtc4.Text = Search[5].ToString();

            strQuery = Query;
            strWhere = Where;
            strSearch = Search;
            strPHeadText = PHeadText;
            strPTxtAlign = PTxtAlign;
            strPCellType = PCellType;
            strHeadWidth = PHeadWidth;

            TmpQuery = strQuery;

            if (strWhere.Length > 0 && strWhere[0].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[0].ToString() + "='" + txtCode.Text + "'";
            if (strWhere.Length > 1 && strWhere[1].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[1].ToString() + "='" + txtCodeName.Text + "'";
            if (strWhere.Length > 2 && strWhere[2].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[2].ToString() + "='" + txtEtc1.Text + "'";
            if (strWhere.Length > 3 && strWhere[3].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[3].ToString() + "='" + txtEtc2.Text + "'";
            if (strWhere.Length > 4 && strWhere[4].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[4].ToString() + "='" + txtEtc3.Text + "'";
            if (strWhere.Length > 5 && strWhere[5].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[5].ToString() + "='" + txtEtc4.Text + "'";

            FPMake.grdMakeSheet(fpSpread1, TmpQuery, strPHeadText, strPTxtAlign, strPCellType, strHeadWidth);
            RtnStr(0);
        }


        public FPPOPUP(string strFORM_ID, string Query, string[] Where, string[] Search, int[] PSearchLabel, string TitleText, FarPoint.Win.Spread.FpSpread baseGrid, int Row, bool TopChangeEV)
        {
            fpSpread5 = baseGrid;
            fpRow = Row;
            InitializeComponent();

            TopChange = TopChangeEV;

            this.Text = TitleText;
            if (SystemBase.Base.ProgramWhere.Length > 0)
            {
                string HeadQuery = " SELECT HEAD_ONE, DATA_ALIGN, DATA_TYPE, HEAD_WIDTH  FROM CO_GRID_DESIGN WHERE FORM_ID='" + strFORM_ID.ToString() + "' ORDER BY DATA_SEQ ";
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

            if (PSearchLabel.Length > 0)
            {
                lblCode.Text = PHeadText[PSearchLabel[0]].ToString();
                panel1.Height = 56;
            }
            if (PSearchLabel.Length > 1)
            {
                lblCodeName.Text = PHeadText[PSearchLabel[1]].ToString();
                panel1.Height = 80;
                lblCodeName.Visible = true;
                txtCodeName.Visible = true;
            }
            if (PSearchLabel.Length > 2)
            {
                if (PHeadText.Length > 2)
                    lblEtc1.Text = PHeadText[PSearchLabel[2]].ToString();
                panel1.Height = 104;
                lblEtc1.Visible = true;
                txtEtc1.Visible = true;
            }
            if (PSearchLabel.Length > 3)
            {
                if (PHeadText.Length > 3)
                    lblEtc2.Text = PHeadText[PSearchLabel[3]].ToString();
                panel1.Height = 128;
                lblEtc2.Visible = true;
                txtEtc2.Visible = true;
            }
            if (PSearchLabel.Length > 4)
            {
                if (PHeadText.Length > 4)
                    lblEtc3.Text = PHeadText[PSearchLabel[4]].ToString();
                panel1.Height = 152;
                lblEtc3.Visible = true;
                txtEtc3.Visible = true;
            }
            if (PSearchLabel.Length > 5)
            {
                if (PHeadText.Length > 5)
                    lblEtc4.Text = PHeadText[PSearchLabel[5]].ToString();
                panel1.Height = 176;
                lblEtc4.Visible = true;
                txtEtc4.Visible = true;
            }

            if (Search.Length > 0)
                txtCode.Text = Search[0].ToString();
            if (Search.Length > 1)
                txtCodeName.Text = Search[1].ToString();
            if (Search.Length > 2)
                txtEtc1.Text = Search[2].ToString();
            if (Search.Length > 3)
                txtEtc2.Text = Search[3].ToString();
            if (Search.Length > 4)
                txtEtc3.Text = Search[4].ToString();
            if (Search.Length > 5)
                txtEtc4.Text = Search[5].ToString();

            strQuery = Query;
            strWhere = Where;
            strSearch = Search;
            strPHeadText = PHeadText;
            strPTxtAlign = PTxtAlign;
            strPCellType = PCellType;
            strHeadWidth = PHeadWidth;

            TmpQuery = strQuery;

            if (strWhere.Length > 0 && strWhere[0].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[0].ToString() + "='" + txtCode.Text + "'";
            if (strWhere.Length > 1 && strWhere[1].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[1].ToString() + "='" + txtCodeName.Text + "'";
            if (strWhere.Length > 2 && strWhere[2].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[2].ToString() + "='" + txtEtc1.Text + "'";
            if (strWhere.Length > 3 && strWhere[3].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[3].ToString() + "='" + txtEtc2.Text + "'";
            if (strWhere.Length > 4 && strWhere[4].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[4].ToString() + "='" + txtEtc3.Text + "'";
            if (strWhere.Length > 5 && strWhere[5].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[5].ToString() + "='" + txtEtc4.Text + "'";

            string TmpQuery2 = "";
            if (TopChange)
                TmpQuery2 = TmpQuery + ", @pTOPCOUNT ='" + AddRow + "'";
            else
                TmpQuery2 = TmpQuery;

            FPMake.grdMakeSheet(fpSpread1, TmpQuery, strPHeadText, strPTxtAlign, strPCellType, strHeadWidth);
            RtnStr(0);
        }

        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public string[] ReturnValue { get { return returnValue; } set { returnValue = value; } }


        public int GetSpreadRows {
            get { return fpSpread1.Sheets[0].Rows.Count; } 
        }

		private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
		{
			// 헤더 체크박스 클릭시 전체행 체크 처리
			if (e.ColumnHeader)
				if (((FarPoint.Win.Spread.FpSpread)sender).ActiveSheet.ToggleCheckAll(e.Column))
					e.Cancel = true;
		} 

        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
			// 헤더 체크박스 클릭시 전체행 체크 처리
			if (e.ColumnHeader)
				if (((FarPoint.Win.Spread.FpSpread)sender).ActiveSheet.ToggleCheckAll(e.Column))
				{
					e.Cancel = true;
					return;
				}
			
			try
            {
                if (returnValue != null && returnValue.Length > 0)
                {
                    RtnString(e.Row);
                }
                else
                {
                    RtnStr(e.Row);
                }

                if (fpRow > -1)
                {
                    if (fpSpread5.Sheets[0].RowHeader.Cells[fpRow, 0].Text != "I")
                        fpSpread5.Sheets[0].RowHeader.Cells[fpRow, 0].Text = "U";
                }
                this.DialogResult = DialogResult.OK;
            }
            catch { }
            //this.Close();
        }

        private void fpSpread1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                // RtnStr(fpSpread1.ActiveSheet.GetSelection(0).Row);
                if (fpSpread1.ActiveSheet.GetSelection(0) == null)
                    RtnStr(fpSpread1.ActiveSheet.ActiveRowIndex);
                else
                    RtnStr(fpSpread1.ActiveSheet.GetSelection(0).Row); 


               
                if (fpRow > -1)
                {
                    if (fpSpread5.Sheets[0].RowHeader.Cells[fpRow, 0].Text != "I")
                        fpSpread5.Sheets[0].RowHeader.Cells[fpRow, 0].Text = "U";
                }
                this.DialogResult = DialogResult.OK;
                //this.Close();
            }
        }

        private void fpSpread1_EnterCell(object sender, FarPoint.Win.Spread.EnterCellEventArgs e)
        {
            RtnStr(e.Row);
        }

        public void RtnStr(int R)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    returnVal = "";
                    for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                    {
                        if (returnVal.Length > 0)
                            returnVal = returnVal + "#" + fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
                        else
                            returnVal = fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
                    }
                }
            }
            catch { }
        }

        public void RtnString(int R)
        {	// 공통팝업 배열로 Return 090323 추가
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    returnValue = new string[fpSpread1.Sheets[0].Columns.Count];
                    for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                    {
                        returnValue[i] = fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
                    }
                }
            }
            catch { }
        }

        private void Code_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (TopChange)
                    SDown = 1;

                TmpQuery = strQuery;

                if (strWhere.Length > 0 && strWhere[0].ToString().Length > 0)
                    TmpQuery = TmpQuery + "," + strWhere[0].ToString() + "='" + txtCode.Text + "'";
                if (strWhere.Length > 1 && strWhere[1].ToString().Length > 0)
                    TmpQuery = TmpQuery + "," + strWhere[1].ToString() + "='" + txtCodeName.Text + "'";
                if (strWhere.Length > 2 && strWhere[2].ToString().Length > 0)
                    TmpQuery = TmpQuery + "," + strWhere[2].ToString() + "='" + txtEtc1.Text + "'";
                if (strWhere.Length > 3 && strWhere[3].ToString().Length > 0)
                    TmpQuery = TmpQuery + "," + strWhere[3].ToString() + "='" + txtEtc2.Text + "'";
                if (strWhere.Length > 4 && strWhere[4].ToString().Length > 0)
                    TmpQuery = TmpQuery + "," + strWhere[4].ToString() + "='" + txtEtc3.Text + "'";
                if (strWhere.Length > 5 && strWhere[5].ToString().Length > 0)
                    TmpQuery = TmpQuery + "," + strWhere[5].ToString() + "='" + txtEtc4.Text + "'";

                string TmpQuery2 = "";
                if (TopChange)
                    TmpQuery2 = TmpQuery + ", @pTOPCOUNT ='" + AddRow + "'";
                else
                    TmpQuery2 = TmpQuery;

                FPMake.grdMakeSheet(fpSpread1, TmpQuery2, strPHeadText, strPTxtAlign, strPCellType, strHeadWidth);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    //RtnString(0);
                    fpSpread1.Focus();
                    fpSpread1.ActiveSheet.SetActiveCell(0, 0);
                    fpSpread1.ActiveSheet.AddSelection(0, 0, 1, 1);
                    fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                txtCode.Text = "";
                txtCodeName.Text = "";
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if (TopChange)
                SDown = 1;

            TmpQuery = strQuery;

            if (strWhere.Length > 0 && strWhere[0].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[0].ToString() + "='" + txtCode.Text + "'";
            if (strWhere.Length > 1 && strWhere[1].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[1].ToString() + "='" + txtCodeName.Text + "'";
            if (strWhere.Length > 2 && strWhere[2].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[2].ToString() + "='" + txtEtc1.Text + "'";
            if (strWhere.Length > 3 && strWhere[3].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[3].ToString() + "='" + txtEtc2.Text + "'";
            if (strWhere.Length > 4 && strWhere[4].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[4].ToString() + "='" + txtEtc3.Text + "'";
            if (strWhere.Length > 5 && strWhere[5].ToString().Length > 0)
                TmpQuery = TmpQuery + "," + strWhere[5].ToString() + "='" + txtEtc4.Text + "'";

            string TmpQuery2 = "";
            if (TopChange)
                TmpQuery2 = TmpQuery + ", @pTOPCOUNT ='" + AddRow + "'";
            else
                TmpQuery2 = TmpQuery;

            FPMake.grdMakeSheet(fpSpread1, TmpQuery2, strPHeadText, strPTxtAlign, strPCellType, strHeadWidth);
            RtnString(0);


            //1 Reocrd이면 화면 닫기
            if (fpSpread1.Sheets[0].Rows.Count == 1)
            {
                //RtnStr(0);
                //this.DialogResult = DialogResult.OK;
                //btnClose_Click(null, null);
            } 
            //2 Record이상이면 첫번째 Row, 첫번째 Col에 커서주기
            else if (fpSpread1.Sheets[0].Rows.Count >= 2)
            {
                fpSpread1.Focus();
                fpSpread1.ActiveSheet.SetActiveCell(0, 0);
            }
        }

        private void PopUpSP_Load(object sender, System.EventArgs e)
        {
            //Thread.Sleep(1000);
            fpSpread1.Focus();
        }

        private void fpSpread1_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "Down" || e.KeyCode.ToString() == "Up")
            {
                if (fpSpread1.ActiveSheet.GetSelection(0).Row.ToString() == null)
                {
                    fpSpread1.ActiveSheet.SetActiveCell(0, 0);
                }
                else
                {
                    RtnStr(fpSpread1.ActiveSheet.GetSelection(0).Row);
                }
            }
        }

        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            if (TopChange)
            {
                int FPHeight = (fpSpread1.Size.Height - 28) / 20;
                if (e.NewTop >= ((AddRow * SDown) - FPHeight))
                {
                    SDown++;

                    string TmpQuery2 = TmpQuery + ", @pTOPCOUNT ='" + AddRow * SDown + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, TmpQuery2);
                }
            }
        }

    }
}
