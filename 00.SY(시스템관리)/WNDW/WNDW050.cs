using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using SystemBase;
using System.Reflection;

namespace WNDW
{
    public partial class WNDW050 : UIForm.TREE_FPCOMM1
    {
        #region Field
        DataTable dt = null;

        TreeNode node;
        string selectNodeTag = "";

        string strNodeEmpCd = "";
        string strNodeEmpFigNo = "";
        string strNodeEmpIndex = "";
        string strNodeEmpName = "";
        string strNodeEmpLevel = "";
        string strNodeEmpDept = "";
        #endregion

        #region Initialize
        public WNDW050()
        {
            InitializeComponent();
        }
        #endregion

        #region TreeViewMethods
        /// <summary>
        /// 부서 트리조회
        /// </summary>
        public void TreeViewSearch()
        {
            try
            {
                treeView1.Nodes.Clear();
                string Query = " exec usp_WNDW050 'sDeptHierarchy' ";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(Query);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataView dvwData = null;
                    CommonTreeView_Dept(ds.Tables[0].Rows[0]["UP_DEPT_CD"].ToString()
                        , ds.Tables[0].Rows[0]["FIGNO"].ToString()
                        , (TreeNode)null
                        , treeView1
                        , ds
                        , dvwData
                        , imageList1
                        , 0
                        , false);

                    treeView1.Focus();
                    treeViewExpand();    // 자기부서 폴더만 확장 처리					
                }
                else
                {
                    MessageBox.Show("부서별 정보가 존재하지 않습니다.", "TreeView 조회", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show("부서별 TreeView 조회중 오류가 발생하였습니다.", "TreeView 조회", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 부서 트리조회
        /// </summary>
        public void CommonTreeView_Dept(string iParent, string iFigNo, TreeNode pNode, System.Windows.Forms.TreeView treeView1, DataSet ds,
                                        DataView dvwData, ImageList imageList1, int starts, bool isNeedColor)
        {
            try
            {
                treeView1.ImageList = imageList1;

                if (iParent.ToString() == iParent)
                {   // 루트 메뉴인 경우
                    dvwData = new DataView(ds.Tables[0]);
                    dvwData.RowFilter = "[UP_DEPT_CD] = '" + iParent + "' AND [FIGNO] LIKE '" + iFigNo + "%'";
                    starts++;
                }
                else
                {   // 하위 메뉴
                    if (starts > 0)
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[UP_DEPT_CD] = '" + iParent + "'";
                    }
                    else
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[DEPT_CD] = '" + iParent.ToString() + "'";
                        starts++;
                    }
                }

                foreach (DataRowView Row in dvwData)
                {
                    TreeNode zNode;

                    if (pNode == null)
                    {
                        zNode = treeView1.Nodes.Add(Row["DEPT_NM"].ToString());
                        zNode.Tag = Row["DEPT_CD"].ToString() + "||" + Row["FIGNO"].ToString() + "||" + zNode.Index.ToString() + "||" + zNode.Name.ToString() + "||" + zNode.Level.ToString() + "||" + Row["UP_DEPT_CD"].ToString() + "||";

                        if (Row["ENTRY_YN"].ToString() == "X")
                        {
                            zNode.ImageIndex = 0;
                            zNode.SelectedImageIndex = 0;
                        }
                        else if (Row["ENTRY_YN"].ToString() == "N")
                        {
                            zNode.ImageIndex = 1;
                            zNode.SelectedImageIndex = 1;
                        }
                        else if (Row["ENTRY_YN"].ToString() == "Y")
                        {
                            zNode.ImageIndex = 2;
                            zNode.SelectedImageIndex = 2;
                        }

                        string strNode = zNode.FullPath;
                        CommonTreeView_Dept(Row["DEPT_CD"].ToString(), Row["FIGNO"].ToString(), zNode, treeView1, ds, dvwData, imageList1, starts, isNeedColor);
                    }
                    else
                    {
                        zNode = pNode.Nodes.Add(Row["DEPT_NM"].ToString());
                        zNode.Tag = Row["DEPT_CD"].ToString() + "||" + Row["FIGNO"].ToString() + "||" + zNode.Index.ToString() + "||" + zNode.Parent.ToString() + "||" + zNode.Level.ToString() + "||" + Row["UP_DEPT_CD"].ToString() + "||";

                        if (Row["ENTRY_YN"].ToString() == "X")
                        {
                            zNode.ImageIndex = 0;
                            zNode.SelectedImageIndex = 0;
                        }
                        else if (Row["ENTRY_YN"].ToString() == "N")
                        {
                            zNode.ImageIndex = 1;
                            zNode.SelectedImageIndex = 1;
                        }
                        else if (Row["ENTRY_YN"].ToString() == "Y")
                        {
                            zNode.ImageIndex = 2;
                            zNode.SelectedImageIndex = 2;
                        }

                        string strNode = zNode.FullPath;
                        CommonTreeView_Dept(Row["DEPT_CD"].ToString(), Row["FIGNO"].ToString(), zNode, treeView1, ds, dvwData, imageList1, starts, isNeedColor);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("TreeView 생성중 오류가 발생하였습니다.", "결재라인등록", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 로그인자의 부서만 트리 확장
        /// </summary>
        public void treeViewExpand()
        {
            string strUsrNm = SystemBase.Base.gstrDEPTNM;  // 자신부서까지만...

            // 개인 node 찾기
            //TreeNode nodeEmployee = SearchNode(treeView1.Nodes, Session.User.Name);
            TreeNode nodeEmployee = SearchNode(treeView1.Nodes, strUsrNm);
            EmpNodeTagSplit(selectNodeTag);

            // 개인 Node 포커스 처리
            treeView1.Select();
            treeView1.SelectedNode = nodeEmployee;
        }
        public TreeNode SearchNode(TreeNodeCollection objNodes, string strKey)
        {
            try
            {
                // Nodes의 node를 가지고 찾을 때까지 반복합니다.
                foreach (TreeNode node in objNodes)
                {
                    // 해당 Node를 찾을 경우 Node를 리턴합니다.
                    if (node.Text == strKey)
                    {
                        selectNodeTag = node.Tag.ToString();
                        return node;
                    }
                    // 없을 경우 하위 Nodes를 가지고 다시 SearchNode를 호출합니다.
                    TreeNode findNode = SearchNode(node.Nodes, strKey);

                    // 하위노드 검색 결과를 비교하여 Null이 아닐경우(찾은 경우) node를 리턴합니다.
                    if (findNode != null)
                        return findNode;
                }

            }
            catch (Exception f)
            {
                MessageBox.Show("TreeView 생성중 오류가 발생하였습니다.", "결재라인등록", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        /// <summary>
        /// 개인 EmpNodeTagSplit 함수
        /// </summary>
        private void EmpNodeTagSplit(string selectNodeTag)
        {
            try
            {
                if (string.IsNullOrEmpty(selectNodeTag)) return;

                strNodeEmpCd = selectNodeTag.Substring(0, selectNodeTag.IndexOf("||"));
                selectNodeTag = selectNodeTag.Substring(selectNodeTag.IndexOf("||") + 2, selectNodeTag.Length - selectNodeTag.IndexOf("||") - 2);

                strNodeEmpFigNo = selectNodeTag.Substring(0, selectNodeTag.IndexOf("||"));
                selectNodeTag = selectNodeTag.Substring(selectNodeTag.IndexOf("||") + 2, selectNodeTag.Length - selectNodeTag.IndexOf("||") - 2);

                strNodeEmpIndex = selectNodeTag.Substring(0, selectNodeTag.IndexOf("||"));
                selectNodeTag = selectNodeTag.Substring(selectNodeTag.IndexOf("||") + 2, selectNodeTag.Length - selectNodeTag.IndexOf("||") - 2);

                strNodeEmpName = selectNodeTag.Substring(0, selectNodeTag.IndexOf("||"));
                selectNodeTag = selectNodeTag.Substring(selectNodeTag.IndexOf("||") + 2, selectNodeTag.Length - selectNodeTag.IndexOf("||") - 2);

                strNodeEmpLevel = selectNodeTag.Substring(0, selectNodeTag.IndexOf("||"));
                selectNodeTag = selectNodeTag.Substring(selectNodeTag.IndexOf("||") + 2, selectNodeTag.Length - selectNodeTag.IndexOf("||") - 2);

                strNodeEmpDept = selectNodeTag.Substring(0, selectNodeTag.IndexOf("||"));
                selectNodeTag = selectNodeTag.Substring(selectNodeTag.IndexOf("||") + 2, selectNodeTag.Length - selectNodeTag.IndexOf("||") - 2);

            }
            catch (Exception f)
            {
                MessageBox.Show( "NodeTagSplit 생성중 오류가 발생하였습니다.", "결재라인등록", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Form Load 시
        /// <summary>
        /// 폼로드
        /// </summary>
        private void WNDW050_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B091', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            
            TreeViewSearch();
        }

        #endregion

        #region 버튼 이벤트
        /// <summary>
        /// 확인
        /// </summary>
        public DataTable ReturnDt { get { return dt; } set { dt = value; } }
        private void btnLineConfirm_Click(object sender, EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";

                DataTable dt = new DataTable();
                DataRow row = null;

                dt.Columns.Add(new DataColumn("결재자", typeof(string)));
                dt.Columns.Add(new DataColumn("결재자명", typeof(string)));
                dt.Columns.Add(new DataColumn("결재단계", typeof(string)));

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    row = dt.NewRow();
                    row["결재자"] = fpSpread1.Sheets[0].Cells[i, 1].Text;
                    row["결재자명"] = fpSpread1.Sheets[0].Cells[i, 2].Text;
                    row["결재단계"] = fpSpread1.Sheets[0].Cells[i, 3].Value;
                    dt.Rows.Add(row);
                }
                if (dt.Rows.Count > 0) { ReturnDt = dt; }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// 취소
        /// </summary>
        private void btnAllCancel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        /// <summary>
        /// 검토
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            LineAdd("B");
        }
        /// <summary>
        /// 승인
        /// </summary>
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            LineAdd("D");

        }
        /// <summary>
        /// 참조
        /// </summary>
        private void btnReference_Click(object sender, EventArgs e)
        {
            LineAdd("H");
        }
        /// <summary>
        /// 결재라인생성
        /// </summary>
        void LineAdd(string LineType)
        {
            UIForm.FPMake.RowInsert(fpSpread1);
            int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

            fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text = strNodeEmpCd;
            fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")].Value = LineType;           

            //부서선택후 사원검토 체크를 위해 직접 DB Select
            string strSql = " exec usp_WNDW050 'sEmpNm'  ";
            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strSql += ", @pUSR_ID = '" + strNodeEmpCd + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명")].Text = dt.Rows[0]["USR_NM"].ToString();
            }

        }

        /// <summary>
        /// 삭제
        /// </summary>
        private void btnLineDel_Click(object sender, EventArgs e)
        {
            UIForm.FPMake.RowRemove(fpSpread1);
        }
        
        /// <summary>
        /// treeView1 NodeMouseClick
        /// </summary>
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // 새로운 노드선택
            node = e.Node;            
            selectNodeTag = e.Node.Tag.ToString();
            EmpNodeTagSplit(selectNodeTag);
        }

        #endregion


    }
}
