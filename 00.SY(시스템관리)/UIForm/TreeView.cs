using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

namespace UIForm
{
    /// <summary>
    /// TreeView에 대한 요약 설명입니다.
    /// </summary>
    public class TreeView
    {
        private static Font SysFont = new System.Drawing.Font("돋움",10F);

        /*****************************************************
         * MBOP TREEVIEW
         *****************************************************/
        public static void MBOPTreeView(
            string iParent,
            int starts,
            TreeNode pNode,
            System.Windows.Forms.TreeView treeView1,
            DataSet ds,
            DataView dvwData,
            ImageList imageList1)
        {
            try
            {
                treeView1.Font = SysFont;
                treeView1.ImageList = imageList1;

                if (iParent.ToString() == "*")
                {	// 루트 메뉴인 경우
                    dvwData = new DataView(ds.Tables[0]);
                    dvwData.RowFilter = "[PRNT_ITEM_CD] = '" + iParent + "'";
                    starts++;
                }
                else
                {	// 하위 메뉴
                    if (starts > 0)
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[PRNT_ITEM_CD] = '" + iParent + "'";
                    }
                    else
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[CHILD_ITEM_CD] = '" + iParent.ToString() + "'";
                        starts++;
                    }
                }

                foreach (DataRowView Row in dvwData)
                {
                    TreeNode zNode;

                    if (pNode == null)
                    {
                        zNode = treeView1.Nodes.Add(Row["CHILD_ITEM_NM"].ToString());
                        zNode.Tag = Row["PRNT_PLANT_CD"].ToString() + "||" + Row["PRNT_ITEM_CD"].ToString() + "||" + Row["PRNT_BOM_NO"].ToString() + "||" + Row["CHILD_ITEM_SEQ"].ToString() + "||" + Row["CHILD_PLANT_CD"].ToString() + "||" + Row["CHILD_ITEM_CD"].ToString() + "||" + Row["CHILD_BOM_NO"].ToString() + "||" + Row["PROJECT_NO"].ToString() + "||" + Row["PROJECT_SEQ"].ToString() + "||" + Row["GROUP_CD"].ToString() + "||" + Row["ITEM_NM"].ToString();
                        zNode.ImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());
                        zNode.SelectedImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());

                        MBOPTreeView(Row["CHILD_ITEM_CD"].ToString(), starts, zNode, treeView1, ds, dvwData, imageList1);
                    }
                    else
                    {
                        zNode = pNode.Nodes.Add(Row["CHILD_ITEM_NM"].ToString());
                        zNode.Tag = Row["PRNT_PLANT_CD"].ToString() + "||" + Row["PRNT_ITEM_CD"].ToString() + "||" + Row["PRNT_BOM_NO"].ToString() + "||" + Row["CHILD_ITEM_SEQ"].ToString() + "||" + Row["CHILD_PLANT_CD"].ToString() + "||" + Row["CHILD_ITEM_CD"].ToString() + "||" + Row["CHILD_BOM_NO"].ToString() + "||" + Row["PROJECT_NO"].ToString() + "||" + Row["PROJECT_SEQ"].ToString() + "||" + Row["GROUP_CD"].ToString() + "||" + Row["ITEM_NM"].ToString();
                        zNode.ImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());
                        zNode.SelectedImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());

                        MBOPTreeView(Row["CHILD_ITEM_CD"].ToString(), starts, zNode, treeView1, ds, dvwData, imageList1);
                    }
                }
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("TreeView 생성오류", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "TreeView 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*****************************************************
        * 창정 TREEVIEW
        *****************************************************/
        public static void CBOPTreeView(
            string itemCd,
            string bomNo,
            int starts,
            TreeNode pNode,
            System.Windows.Forms.TreeView treeView1,
            DataSet ds,
            DataView dvwData,
            ImageList imageList1,
            bool isNeedColor)
        {
            try
            {
                treeView1.Font = SysFont;
                treeView1.ImageList = imageList1;

                if (itemCd == "*")
                {	// 루트 메뉴인 경우
                    dvwData = new DataView(ds.Tables[0]);
                    dvwData.RowFilter = "[PRNT_ITEM_CD] = '" + itemCd + "'";
                    starts++;
                }
                else
                {	// 하위 메뉴
                    if (starts > 0)
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[PRNT_ITEM_CD] = '" + itemCd + "' AND [PRNT_BOM_NO] = '" + bomNo + "'";

                    }
                    else
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[CHILD_ITEM_CD] = '" + itemCd + "' AND [PRNT_BOM_NO] = '" + bomNo + "'";

                        starts++;
                    }
                }

                foreach (DataRowView Row in dvwData)
                {
                    TreeNode zNode;

                    if (pNode == null)
                    {
                        zNode = treeView1.Nodes.Add(Row["CHILD_ITEM_NM"].ToString());
                        zNode.Tag = Row["PRNT_PLANT_CD"].ToString() + "||" + Row["PRNT_ITEM_CD"].ToString() + "||" + Row["PRNT_BOM_NO"].ToString() + "||" + Row["CHILD_ITEM_SEQ"].ToString() + "||" + Row["CHILD_PLANT_CD"].ToString() + "||" + Row["CHILD_ITEM_CD"].ToString() + "||" + Row["CHILD_BOM_NO"].ToString() + "||" + Row["PROJECT_NO"].ToString() + "||" + Row["PROJECT_SEQ"].ToString() + "||" + Row["GROUP_CD"].ToString() + "||" + Row["ITEM_NM"].ToString() + "||" + Row["MAKEORDER_NO"].ToString();
                        zNode.ImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());
                        zNode.SelectedImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());

                        CBOPTreeView(Row["CHILD_ITEM_CD"].ToString(), Row["CHILD_BOM_NO"].ToString(), starts, zNode, treeView1, ds, dvwData, imageList1, isNeedColor);
                    }
                    else
                    {
                        zNode = pNode.Nodes.Add(Row["CHILD_ITEM_NM"].ToString());
                        zNode.Tag = Row["PRNT_PLANT_CD"].ToString() + "||" + Row["PRNT_ITEM_CD"].ToString() + "||" + Row["PRNT_BOM_NO"].ToString() + "||" + Row["CHILD_ITEM_SEQ"].ToString() + "||" + Row["CHILD_PLANT_CD"].ToString() + "||" + Row["CHILD_ITEM_CD"].ToString() + "||" + Row["CHILD_BOM_NO"].ToString() + "||" + Row["PROJECT_NO"].ToString() + "||" + Row["PROJECT_SEQ"].ToString() + "||" + Row["GROUP_CD"].ToString() + "||" + Row["ITEM_NM"].ToString() + "||" + Row["MAKEORDER_NO"].ToString();
                        zNode.ImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());
                        zNode.SelectedImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());

                        CBOPTreeView(Row["CHILD_ITEM_CD"].ToString(), Row["CHILD_BOM_NO"].ToString(), starts, zNode, treeView1, ds, dvwData, imageList1, isNeedColor);
                    }

                    // 폰트 색깔 처리 추가
                    if (isNeedColor && Row["COLOR"].ToString() == "FR")
                        zNode.ForeColor = Color.Red;
                    else
                        zNode.ForeColor = Color.Black;
                }
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("TreeView 생성오류", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "TreeView 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /*****************************************************
        * MBOP 조회중 MAKEORDER NO, WORKORDER NO, FIG NO 추가
        *****************************************************/
        public static void MBOPTreeView(
            string iParent,
            int starts,
            TreeNode pNode,
            System.Windows.Forms.TreeView treeView1,
            DataSet ds,
            DataView dvwData,
            ImageList imageList1,
            string iFIGNO,
            string Type)
        {
            try
            {
                treeView1.Font = SysFont;
                treeView1.ImageList = imageList1;

                if (iParent.ToString() == "*")
                {	// 루트 메뉴인 경우
                    dvwData = new DataView(ds.Tables[0]);
                    dvwData.RowFilter = "[PRNT_ITEM_CD] = '" + iParent + "'";
                    starts++;
                }
                else
                {	// 하위 메뉴
                    if (starts > 0)
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[PRNT_ITEM_CD] = '" + iParent + "' AND [FIGNO] LIKE '" + iFIGNO + "%'";
                    }
                    else
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[CHILD_ITEM_CD] = '" + iParent.ToString() + "'";
                        starts++;
                    }
                }

                foreach (DataRowView Row in dvwData)
                {
                    TreeNode zNode;

                    if (pNode == null)
                    {
                        zNode = treeView1.Nodes.Add(Row["CHILD_ITEM_NM"].ToString());
                        zNode.Tag = Row["PRNT_PLANT_CD"].ToString() + "||" + Row["PRNT_ITEM_CD"].ToString() + "||" + Row["PRNT_BOM_NO"].ToString() + "||" + Row["CHILD_ITEM_SEQ"].ToString() + "||" + Row["CHILD_PLANT_CD"].ToString() + "||" + Row["CHILD_ITEM_CD"].ToString() + "||" + Row["CHILD_BOM_NO"].ToString() + "||" + Row["PROJECT_NO"].ToString() + "||" + Row["PROJECT_SEQ"].ToString() + "||" + Row["GROUP_CD"].ToString() + "||" + Row["ITEM_NM"].ToString() + "||" + Row["MAKEORDER_NO"].ToString() + "||" + Row["WORKORDER_NO_OG"].ToString() + "||" + Row["FIGNO"].ToString();
                        zNode.ImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());
                        zNode.SelectedImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());

                        MBOPTreeView(Row["CHILD_ITEM_CD"].ToString(), starts, zNode, treeView1, ds, dvwData, imageList1, Row["FIGNO"].ToString(), "W");
                    }
                    else
                    {
                        zNode = pNode.Nodes.Add(Row["CHILD_ITEM_NM"].ToString());
                        zNode.Tag = Row["PRNT_PLANT_CD"].ToString() + "||" + Row["PRNT_ITEM_CD"].ToString() + "||" + Row["PRNT_BOM_NO"].ToString() + "||" + Row["CHILD_ITEM_SEQ"].ToString() + "||" + Row["CHILD_PLANT_CD"].ToString() + "||" + Row["CHILD_ITEM_CD"].ToString() + "||" + Row["CHILD_BOM_NO"].ToString() + "||" + Row["PROJECT_NO"].ToString() + "||" + Row["PROJECT_SEQ"].ToString() + "||" + Row["GROUP_CD"].ToString() + "||" + Row["ITEM_NM"].ToString() + "||" + Row["MAKEORDER_NO"].ToString() + "||" + Row["WORKORDER_NO_OG"].ToString() + "||" + Row["FIGNO"].ToString();
                        zNode.ImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());
                        zNode.SelectedImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());

                        MBOPTreeView(Row["CHILD_ITEM_CD"].ToString(), starts, zNode, treeView1, ds, dvwData, imageList1, Row["FIGNO"].ToString(), "W");
                    }
                }
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("TreeView 생성오류", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "TreeView 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /*****************************************************
        * MBOP 조회중 MAKEORDER NO, WORKORDER NO 추가
        *****************************************************/
        public static void MBOPTreeView(
            string iParent,
            int starts,
            TreeNode pNode,
            System.Windows.Forms.TreeView treeView1,
            DataSet ds,
            DataView dvwData,
            ImageList imageList1,
            string Type)
        {
            try
            {
                treeView1.Font = SysFont;
                treeView1.ImageList = imageList1;

                if (iParent.ToString() == "*")
                {	// 루트 메뉴인 경우
                    dvwData = new DataView(ds.Tables[0]);
                    dvwData.RowFilter = "[PRNT_ITEM_CD] = '" + iParent + "'";
                    starts++;
                }
                else
                {	// 하위 메뉴
                    if (starts > 0)
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[PRNT_ITEM_CD] = '" + iParent + "'";
                    }
                    else
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[CHILD_ITEM_CD] = '" + iParent.ToString() + "'";
                        starts++;
                    }
                }

                foreach (DataRowView Row in dvwData)
                {
                    TreeNode zNode;

                    if (pNode == null)
                    {
                        zNode = treeView1.Nodes.Add(Row["CHILD_ITEM_NM"].ToString());
                        zNode.Tag = Row["PRNT_PLANT_CD"].ToString() + "||" + Row["PRNT_ITEM_CD"].ToString() + "||" + Row["PRNT_BOM_NO"].ToString() + "||" + Row["CHILD_ITEM_SEQ"].ToString() + "||" + Row["CHILD_PLANT_CD"].ToString() + "||" + Row["CHILD_ITEM_CD"].ToString() + "||" + Row["CHILD_BOM_NO"].ToString() + "||" + Row["PROJECT_NO"].ToString() + "||" + Row["PROJECT_SEQ"].ToString() + "||" + Row["GROUP_CD"].ToString() + "||" + Row["ITEM_NM"].ToString() + "||" + Row["MAKEORDER_NO"].ToString() + "||" + Row["WORKORDER_NO_OG"].ToString();
                        zNode.ImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());
                        zNode.SelectedImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());

                        MBOPTreeView(Row["CHILD_ITEM_CD"].ToString(), starts, zNode, treeView1, ds, dvwData, imageList1, "W");
                    }
                    else
                    {
                        zNode = pNode.Nodes.Add(Row["CHILD_ITEM_NM"].ToString());
                        zNode.Tag = Row["PRNT_PLANT_CD"].ToString() + "||" + Row["PRNT_ITEM_CD"].ToString() + "||" + Row["PRNT_BOM_NO"].ToString() + "||" + Row["CHILD_ITEM_SEQ"].ToString() + "||" + Row["CHILD_PLANT_CD"].ToString() + "||" + Row["CHILD_ITEM_CD"].ToString() + "||" + Row["CHILD_BOM_NO"].ToString() + "||" + Row["PROJECT_NO"].ToString() + "||" + Row["PROJECT_SEQ"].ToString() + "||" + Row["GROUP_CD"].ToString() + "||" + Row["ITEM_NM"].ToString() + "||" + Row["MAKEORDER_NO"].ToString() + "||" + Row["WORKORDER_NO_OG"].ToString();
                        zNode.ImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());
                        zNode.SelectedImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());

                        MBOPTreeView(Row["CHILD_ITEM_CD"].ToString(), starts, zNode, treeView1, ds, dvwData, imageList1, "W");
                    }
                }
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("TreeView 생성오류", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "TreeView 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static void CommonTreeView(
            string iParent,
            int starts,
            TreeNode pNode,
            System.Windows.Forms.TreeView treeView1,
            DataSet ds,
            DataView dvwData,
            ImageList imageList1)
        {
            CommonTreeView(iParent, starts, pNode, treeView1, ds, dvwData, imageList1, false);
        }

        public static void CommonTreeView(
            string iParent,
            int starts,
            TreeNode pNode,
            System.Windows.Forms.TreeView treeView1,
            DataSet ds,
            DataView dvwData,
            ImageList imageList1,
            bool isNeedColor)
        {
            try
            {
                treeView1.Font = SysFont;
                treeView1.ImageList = imageList1;

                if (iParent.ToString() == iParent)
                {	// 루트 메뉴인 경우
                    dvwData = new DataView(ds.Tables[0]);
                    dvwData.RowFilter = "[PRNT_ITEM_CD] = '" + iParent + "'";
                    starts++;
                }
                else
                {	// 하위 메뉴
                    if (starts > 0)
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[PRNT_ITEM_CD] = '" + iParent + "'";
                    }
                    else
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[CHILD_ITEM_CD] = '" + iParent.ToString() + "'";
                        starts++;
                    }
                }

                foreach (DataRowView Row in dvwData)
                {
                    TreeNode zNode;

                    if (pNode == null)
                    {
                        zNode = treeView1.Nodes.Add(Row["CHILD_ITEM_NM"].ToString());
                        zNode.Tag = Row["PRNT_PLANT_CD"].ToString() + "||" + Row["PRNT_ITEM_CD"].ToString() + "||" + Row["PRNT_BOM_NO"].ToString() + "||" + Row["CHILD_ITEM_SEQ"].ToString() + "||" + Row["CHILD_PLANT_CD"].ToString() + "||" + Row["CHILD_ITEM_CD"].ToString() + "||" + Row["CHILD_BOM_NO"].ToString() + "||" + Row["PROJECT_NO"].ToString() + "||" + Row["PROJECT_SEQ"].ToString() + "||" + Row["GROUP_CD"].ToString() + "||" + Row["ITEM_NM"].ToString();
                        zNode.ImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());
                        zNode.SelectedImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());

                        if (Row["CHILD_BOM_NO"].ToString() == "C")
                            CommonTreeView(Row["CHILD_ITEM_CD"].ToString(), starts, zNode, treeView1, ds, dvwData, imageList1, isNeedColor);
                    }
                    else
                    {
                        zNode = pNode.Nodes.Add(Row["CHILD_ITEM_NM"].ToString());
                        zNode.Tag = Row["PRNT_PLANT_CD"].ToString() + "||" + Row["PRNT_ITEM_CD"].ToString() + "||" + Row["PRNT_BOM_NO"].ToString() + "||" + Row["CHILD_ITEM_SEQ"].ToString() + "||" + Row["CHILD_PLANT_CD"].ToString() + "||" + Row["CHILD_ITEM_CD"].ToString() + "||" + Row["CHILD_BOM_NO"].ToString() + "||" + Row["PROJECT_NO"].ToString() + "||" + Row["PROJECT_SEQ"].ToString() + "||" + Row["GROUP_CD"].ToString() + "||" + Row["ITEM_NM"].ToString();
                        zNode.ImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());
                        zNode.SelectedImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());

                        if (Row["CHILD_BOM_NO"].ToString() == "C")
                            CommonTreeView(Row["CHILD_ITEM_CD"].ToString(), starts, zNode, treeView1, ds, dvwData, imageList1, isNeedColor);
                    }

                    // 폰트 색깔 처리 추가
                    if (isNeedColor && Row["COLOR"].ToString() == "FR")
                        zNode.ForeColor = Color.Red;
                    else
                        zNode.ForeColor = Color.Black;
                }

            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("TreeView 생성오류", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "TreeView 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void CommonTreeView(
            string iParent,
            TreeNode pNode,
            System.Windows.Forms.TreeView treeView1,
            DataSet ds,
            DataView dvwData,
            ImageList imageList1,
            int starts)
        {
            CommonTreeView(iParent, pNode, treeView1, ds, dvwData, imageList1, starts, false);
        }

        public static void CommonTreeView(
            string iParent,
            TreeNode pNode,
            System.Windows.Forms.TreeView treeView1,
            DataSet ds,
            DataView dvwData,
            ImageList imageList1,
            int starts,
            bool isNeedColor)
        {
            try
            {
                treeView1.Font = SysFont;
                treeView1.ImageList = imageList1;

                if (iParent.ToString() == iParent)
                {	// 루트 메뉴인 경우
                    dvwData = new DataView(ds.Tables[0]);
                    dvwData.RowFilter = "[PRNT_ITEM_CD] = '" + iParent + "'";
                    starts++;
                }
                else
                {	// 하위 메뉴
                    if (starts > 0)
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[PRNT_ITEM_CD] = '" + iParent + "'";
                    }
                    else
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[CHILD_ITEM_CD] = '" + iParent.ToString() + "'";
                        starts++;
                    }
                }

                foreach (DataRowView Row in dvwData)
                {
                    TreeNode zNode;

                    if (pNode == null)
                    {
                        zNode = treeView1.Nodes.Add(Row["CHILD_ITEM_NM"].ToString());
                        zNode.Tag = Row["PRNT_PLANT_CD"].ToString() + "||" + Row["PRNT_ITEM_CD"].ToString() + "||" + Row["PRNT_BOM_NO"].ToString() + "||" + Row["CHILD_ITEM_SEQ"].ToString() + "||" + Row["CHILD_PLANT_CD"].ToString() + "||" + Row["CHILD_ITEM_CD"].ToString() + "||" + Row["CHILD_BOM_NO"].ToString() + "||" + Row["ITEM_NM"].ToString();
                        zNode.ImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());
                        zNode.SelectedImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());

                        CommonTreeView(Row["CHILD_ITEM_CD"].ToString(), zNode, treeView1, ds, dvwData, imageList1, starts, isNeedColor);
                    }
                    else
                    {
                        zNode = pNode.Nodes.Add(Row["CHILD_ITEM_NM"].ToString());
                        zNode.Tag = Row["PRNT_PLANT_CD"].ToString() + "||" + Row["PRNT_ITEM_CD"].ToString() + "||" + Row["PRNT_BOM_NO"].ToString() + "||" + Row["CHILD_ITEM_SEQ"].ToString() + "||" + Row["CHILD_PLANT_CD"].ToString() + "||" + Row["CHILD_ITEM_CD"].ToString() + "||" + Row["CHILD_BOM_NO"].ToString() + "||" + Row["ITEM_NM"].ToString();
                        zNode.ImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());
                        zNode.SelectedImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());

                        CommonTreeView(Row["CHILD_ITEM_CD"].ToString(), zNode, treeView1, ds, dvwData, imageList1, starts, isNeedColor);
                    }
                    // 폰트 색깔 처리 추가
                    if (isNeedColor && Row["COLOR"].ToString() == "FR")
                        zNode.ForeColor = Color.Red;
                    else
                        zNode.ForeColor = Color.Black;
                }
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("TreeView 생성오류", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "TreeView 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //-----------------------------------------------------------------------------------------
        public static void CommonTreeView(string iParent, TreeNode pNode, System.Windows.Forms.TreeView treeView1, DataSet ds, DataView dvwData, int starts, ImageList imageList1)
        {
            try
            {
                treeView1.Font = SysFont;
                treeView1.ImageList = imageList1;

                if (iParent.ToString() == iParent)
                {	// 루트 메뉴인 경우
                    dvwData = new DataView(ds.Tables[0]);
                    dvwData.RowFilter = "[MenuParent] = '" + iParent + "'";
                    starts++;
                }
                else
                {	// 하위 메뉴
                    if (starts > 0)
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[MenuParent] = '" + iParent + "'";
                    }
                    else
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[MenuID] = '" + iParent.ToString() + "'";
                        starts++;
                    }
                }

                foreach (DataRowView Row in dvwData)
                {
                    TreeNode zNode;

                    if (pNode == null)
                    {

                        zNode = treeView1.Nodes.Add(Row["MenuName"].ToString());
                        zNode.Tag = Row["MenuID"].ToString();

                        if (Row["ITEM_KIND"].ToString() == "G")
                        {
                            zNode.ImageIndex = 0;
                            zNode.SelectedImageIndex = 0;
                        }
                        else if (Row["ITEM_KIND"].ToString() == "P")
                        {
                            zNode.ImageIndex = 1;
                            zNode.SelectedImageIndex = 1;
                        }
                        else if (Row["ITEM_KIND"].ToString() == "M")
                        {
                            zNode.ImageIndex = 2;
                            zNode.SelectedImageIndex = 2;
                        }

                        CommonTreeView(Row["MenuID"].ToString().Substring(0, Row["MenuID"].ToString().IndexOf("#")), zNode, treeView1, ds, dvwData, starts, imageList1);
                    }
                    else
                    {
                        zNode = pNode.Nodes.Add(Row["MenuName"].ToString());
                        zNode.Tag = Row["MenuID"].ToString();

                        if (Row["ITEM_KIND"].ToString() == "G")
                        {
                            zNode.ImageIndex = 0;
                            zNode.SelectedImageIndex = 0;
                        }
                        else if (Row["ITEM_KIND"].ToString() == "P")
                        {
                            zNode.ImageIndex = 1;
                            zNode.SelectedImageIndex = 1;
                        }
                        else if (Row["ITEM_KIND"].ToString() == "M")
                        {
                            zNode.ImageIndex = 2;
                            zNode.SelectedImageIndex = 2;
                        }

                        CommonTreeView(Row["MenuID"].ToString().Substring(0, Row["MenuID"].ToString().IndexOf("#")), zNode, treeView1, ds, dvwData, starts, imageList1);
                    }
                }
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("TreeView 생성오류", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "TreeView 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static void CreateTreeView(string iParent, TreeNode pNode, System.Windows.Forms.TreeView treeView1, DataSet ds, DataView dvwData, int starts)
        {
            try
            {
                treeView1.Font = SysFont;

                if (iParent.ToString() == "*")
                {	// 루트 메뉴인 경우
                    dvwData = new DataView(ds.Tables[0]);
                    dvwData.RowFilter = "[MenuParent] = '" + iParent + "'";
                    starts++;
                }
                else
                {	// 하위 메뉴
                    if (starts > 0)
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[MenuParent] = '" + iParent + "'";
                    }
                    else
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[MenuID] = '" + iParent.ToString() + "'";
                        starts++;
                    }
                }

                foreach (DataRowView Row in dvwData)
                {
                    TreeNode zNode;
                    if (pNode == null)
                    {
                        if (Row["MdiForm"].ToString() == "*")
                        {
                            zNode = treeView1.Nodes.Add(Row["MenuName"].ToString());
                            if (zNode.Text == "공통기준관리")//.Index == 0) //공통기준관리
                            {
                                zNode.ImageIndex = 3;
                                zNode.SelectedImageIndex = 3;
                            }
                            else if (zNode.Text == "품목관리") //.Index == 1) //품목관리
                            {
                                zNode.ImageIndex = 4;
                                zNode.SelectedImageIndex = 4;
                            }
                            else if (zNode.Text == "영업관리")//.Index == 2) //영업관리
                            {
                                zNode.ImageIndex = 5;
                                zNode.SelectedImageIndex = 5;
                            }
                            else if (zNode.Text == "생산관리")//.Index == 3) //생산관리
                            {
                                zNode.ImageIndex = 6;
                                zNode.SelectedImageIndex = 6;
                            }
                            else if (zNode.Text == "구매관리")//.Index == 4) //구매관리
                            {
                                zNode.ImageIndex = 7;
                                zNode.SelectedImageIndex = 7;
                            }
                            else if (zNode.Text == "재고관리")//.Index == 5) //재고관리
                            {
                                zNode.ImageIndex = 8;
                                zNode.SelectedImageIndex = 8;
                            }
                            else if (zNode.Text == "품질관리")//.Index == 6) //품질관리
                            {
                                zNode.ImageIndex = 9;
                                zNode.SelectedImageIndex = 9;
                            }
                            else if (zNode.Text == "사업관리")//.Index == 7) //사업관리
                            {
                                zNode.ImageIndex = 10;
                                zNode.SelectedImageIndex = 10;
                            }
                            else if (zNode.Text == "EIS")//.Index == 8) //EIS
                            {
                                zNode.ImageIndex = 11;
                                zNode.SelectedImageIndex = 11;
                            }
                            else if (zNode.Text == "재무관리")//.Index == 9) //재무관리
                            {
                                zNode.ImageIndex = 12;
                                zNode.SelectedImageIndex = 12;
                            }
                            else if (zNode.Text == "인사급여관리")//.Index == 10) //인사급여관리
                            {
                                zNode.ImageIndex = 13;
                                zNode.SelectedImageIndex = 13;
                            }
                            else if (zNode.Text == "인터페이스")//.Index == 11) //인터페이스
                            {
                                zNode.ImageIndex = 14;
                                zNode.SelectedImageIndex = 14;
                            }
                            else if (zNode.Text == "회계관리")//.Index == 12) //회계관리
                            {
                                zNode.ImageIndex = 15;
                                zNode.SelectedImageIndex = 15;
                            }
                            else if (zNode.Text == "보세관리")//.Index == 13) //보세관리
                            {
                                zNode.ImageIndex = 16;
                                zNode.SelectedImageIndex = 16;
                            }
                            else if (zNode.Text == "국방통합원가")//.Index == 14) //국방통합원가
                            {
                                zNode.ImageIndex = 17;
                                zNode.SelectedImageIndex = 17;
                            }
                            else if (zNode.Text == "SCM")//.Index == 14) //SCM
                            {
                                zNode.ImageIndex = 18;
                                zNode.SelectedImageIndex = 18;
                            }
                            else if (zNode.Text == "문서관리")//.Index == 14) //문서관리
                            {
                                zNode.ImageIndex = 19;
                                zNode.SelectedImageIndex = 19;
                            }

                            zNode.Tag = Row["MenuID"].ToString();
                            CreateTreeView(Row["MenuID"].ToString(), zNode, treeView1, ds, dvwData, starts);
                        }
                        else
                        {
                            zNode = treeView1.Nodes.Add(Row["MenuName"].ToString());
                            zNode.Tag = Row["MenuID"].ToString();
                            CreateTreeView(Row["MenuID"].ToString(), zNode, treeView1, ds, dvwData, starts);
                        }
                    }
                    else
                    {
                        if (Row["MdiForm"].ToString() == "*")
                        {
                            zNode = pNode.Nodes.Add(Row["MenuName"].ToString());
                            zNode.ImageIndex = 0;
                            zNode.SelectedImageIndex = 1;
                            zNode.Tag = Row["MenuID"].ToString();
                            CreateTreeView(Row["MenuID"].ToString(), zNode, treeView1, ds, dvwData, starts);
                        }
                        else
                        {
                            zNode = pNode.Nodes.Add(Row["MenuName"].ToString());
                            zNode.Tag = Row["MenuID"].ToString();
                            CreateTreeView(Row["MenuID"].ToString(), zNode, treeView1, ds, dvwData, starts);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("TreeView 생성오류", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "TreeView 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #region BOM 등록 트리 (FIGNO 추가)
        /*****************************************************
		* BOM 등록 트리
		*****************************************************/

        public static void CommonTreeView(
            string iParent,
            string iFigNo,
            TreeNode pNode,
            System.Windows.Forms.TreeView treeView1,
            DataSet ds,
            DataView dvwData,
            ImageList imageList1,
            int starts)
        {
            CommonTreeView(iParent, iFigNo, pNode, treeView1, ds, dvwData, imageList1, starts, false);
        }

        public static void CommonTreeView(
            string iParent,
            string iFigNo,
            TreeNode pNode,
            System.Windows.Forms.TreeView treeView1,
            DataSet ds,
            DataView dvwData,
            ImageList imageList1,
            int starts,
            bool isNeedColor)
        {
            try
            {
                treeView1.Font = SysFont;
                treeView1.ImageList = imageList1;

                if (iParent.ToString() == iParent)
                {	// 루트 메뉴인 경우
                    dvwData = new DataView(ds.Tables[0]);
                    dvwData.RowFilter = "[PRNT_ITEM_CD] = '" + iParent + "' AND [FIGNO] LIKE '" + iFigNo + "%'";
                    starts++;
                }
                else
                {	// 하위 메뉴
                    if (starts > 0)
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[PRNT_ITEM_CD] = '" + iParent + "'";
                    }
                    else
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[CHILD_ITEM_CD] = '" + iParent.ToString() + "'";
                        starts++;
                    }
                }

                foreach (DataRowView Row in dvwData)
                {
                    TreeNode zNode;

                    if (pNode == null)
                    {
                        zNode = treeView1.Nodes.Add(Row["CHILD_ITEM_NM"].ToString());
                        zNode.Tag = Row["PRNT_PLANT_CD"].ToString() + "||" + Row["PRNT_ITEM_CD"].ToString() + "||" + Row["PRNT_BOM_NO"].ToString() + "||" + Row["CHILD_ITEM_SEQ"].ToString() + "||" + Row["CHILD_PLANT_CD"].ToString() + "||" + Row["CHILD_ITEM_CD"].ToString() + "||" + Row["CHILD_BOM_NO"].ToString() + "||" + Row["ITEM_NM"].ToString();
                        zNode.ImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());
                        zNode.SelectedImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());

                        string strNode = zNode.FullPath;

                        CommonTreeView(Row["CHILD_ITEM_CD"].ToString(), Row["FIGNO"].ToString(), zNode, treeView1, ds, dvwData, imageList1, starts, isNeedColor);
                    }
                    else
                    {
                        zNode = pNode.Nodes.Add(Row["CHILD_ITEM_NM"].ToString());
                        zNode.Tag = Row["PRNT_PLANT_CD"].ToString() + "||" + Row["PRNT_ITEM_CD"].ToString() + "||" + Row["PRNT_BOM_NO"].ToString() + "||" + Row["CHILD_ITEM_SEQ"].ToString() + "||" + Row["CHILD_PLANT_CD"].ToString() + "||" + Row["CHILD_ITEM_CD"].ToString() + "||" + Row["CHILD_BOM_NO"].ToString() + "||" + Row["ITEM_NM"].ToString();
                        zNode.ImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());
                        zNode.SelectedImageIndex = Convert.ToInt32(Row["BOM_KIND"].ToString());

                        string strNode = zNode.FullPath;

                        CommonTreeView(Row["CHILD_ITEM_CD"].ToString(), Row["FIGNO"].ToString(), zNode, treeView1, ds, dvwData, imageList1, starts, isNeedColor);
                    }
                    // 폰트 색깔 처리 추가
                    if (isNeedColor && Row["COLOR"].ToString() == "FR")
                        zNode.ForeColor = Color.Red;
                    else
                        zNode.ForeColor = Color.Black;
                }
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("TreeView 생성오류", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "TreeView 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
