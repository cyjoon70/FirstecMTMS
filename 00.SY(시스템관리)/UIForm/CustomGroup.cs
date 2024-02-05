using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FarPoint.Win;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.Model;
  
namespace UIForm
{
    public class CustomGroup
    {
        private FarPoint.Win.Spread.FpSpread fpSpread;
        private FarPoint.Win.Spread.SortInfo[] siList;

        public CustomGroup(FarPoint.Win.Spread.FpSpread FPSpread, SortInfo[] SIList)
        {
            this.fpSpread = FPSpread;
            this.siList = SIList; 

            //칼럼 그룹지정 불가 - 하단에서 지정된 칼럼만 그룹지정 가능
            this.fpSpread.ActiveSheet.AllowGroup = false;
            //그룹바 감추기
            this.fpSpread.ActiveSheet.GroupBarInfo.Visible = false;
            
        } 
 
        #region 그룹 생성하기 
        public void CreateGroup()
        {
            if (this.fpSpread.ActiveSheet.Models.Data is GroupDataModel) return;

            GroupDataModel gdm = new GroupDataModel(this.fpSpread.ActiveSheet.Models.Data); 

            //그룹푸터 우측정렬하기
            GroupInfo gi = new GroupInfo();
            gi.FooterHorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.fpSpread.ActiveSheet.GroupInfos.Add(gi);

            //그룹 설정

            gdm.Group(this.siList);

            this.fpSpread.ActiveSheet.Models.Data = gdm;
                 
            //그룹별 타이틀 변경 
            //int column = 0; 
            gdm = (GroupDataModel)this.fpSpread.ActiveSheet.Models.Data;

            if (this.fpSpread.ActiveSheet.NonEmptyRowCount > 0)
            {
                /*for (int i = 0; i < fpSpread.ActiveSheet.NonEmptyRowCount; i++)
                {
                    if (gdm.IsGroup(i))
                    {
                        FarPoint.Win.Spread.Model.Group g;
                        g = gdm.GetGroup(i);
                        g.Expanded = true;
                        column = g.Column;
                        string s = gdm.GetValue(i, column).ToString();
                        string[] sArr = s.Split(":".ToCharArray());
                        System.Diagnostics.Debug.WriteLine("---");
                        System.Diagnostics.Debug.WriteLine(System.DateTime.Now);
                        if (sArr.Length > 1)
                            g.Text = this.fpSpread.ActiveSheet.Columns[column].Label + ":" + sArr[1].ToString();
                        else
                            g.Text = this.fpSpread.ActiveSheet.Columns[column].Label + ":" + sArr[0].ToString();

                        System.Diagnostics.Debug.WriteLine(System.DateTime.Now);

                    }
                }*/

                //그룹별 합계 
                int sortLength = gdm.SortInfo.Length;
                if (sortLength > 0)
                {
                    for (int i = 0; i < sortLength; i++)
                    {
                        for (int j = 0; j < this.fpSpread.ActiveSheet.ColumnCount; j++)
                        { 
                            if ((this.fpSpread.ActiveSheet.Columns[j].Tag == null ? "".ToString() :this.fpSpread.ActiveSheet.Columns[j].Tag.ToString()) == "NM")
                            {
                                FarPoint.Win.Spread.DefaultGroupFooter dgf = this.fpSpread.ActiveSheet.DefaultGroupFooter[gdm.SortInfo[i].Index];
                                FarPoint.Win.Spread.Model.ISheetDataModel dataModel = dgf.DataModel;

                                (dataModel as FarPoint.Win.Spread.Model.IAggregationSupport).SetCellAggregationType(0, j, FarPoint.Win.Spread.Model.AggregationType.Sum);
                            }
                        }

                        //그룹항목은 수정불가
                        this.fpSpread.ActiveSheet.Columns[gdm.SortInfo[i].Index].Locked = true;
                    }
                }
            }

            //그룹 푸터 보이기
            this.fpSpread.ActiveSheet.GroupFooterVisible = true;   

            //칼럼 합계
            this.fpSpread.ActiveSheet.ColumnFooter.Visible = true;
            this.fpSpread.ActiveSheet.ColumnFooter.RowCount = 1;
            if (gdm.SortInfo.Length > 0)
            {
                this.fpSpread.ActiveSheet.ColumnFooter.Cells[0, gdm.SortInfo[0].Index].Text = "합계";
            } 

            for (int i = 0; i < this.fpSpread.ActiveSheet.ColumnCount; i++)
            {
                if ((this.fpSpread.ActiveSheet.Columns[i].Tag == null ? "".ToString() : this.fpSpread.ActiveSheet.Columns[i].Tag.ToString()) == "NM")
                { 
                    FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType(); 
                    fpSpread.ActiveSheet.Columns[i].CellType = num;

                    num.Separator = ",";
                    num.ShowSeparator = true;
                    num.MaximumValue = 99999999999999;
                    num.MinimumValue = -99999999999999;
                    this.fpSpread.ActiveSheet.ColumnFooter.Columns[i].CellType = num;

                    this.fpSpread.ActiveSheet.ColumnFooter.Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                    this.fpSpread.ActiveSheet.ColumnFooter.SetAggregationType(0, i, FarPoint.Win.Spread.Model.AggregationType.Sum);
                }
            } 
        }
        #endregion  

        #region 그룹 여부확인
        public bool IsGroup()
        {
            return (this.fpSpread.ActiveSheet.Models.Data is FarPoint.Win.Spread.Model.GroupDataModel) ? true : false;
        }
        #endregion 

        #region 줄임
        public void CollapseAll()
        {
            this.fpSpread.ActiveSheet.GroupingPolicy = FarPoint.Win.Spread.Model.GroupingPolicy.CollapseAll;
        }
        #endregion 

        #region 펼침
        public void ExpandAll()
        {
            this.fpSpread.ActiveSheet.GroupingPolicy = FarPoint.Win.Spread.Model.GroupingPolicy.ExpandAll;
        }
        #endregion  

    }


    /*
    public void CancelGroup()
    {
        //  if (this.fpSpread.ActiveSheet.Models.Data is GroupDataModel)
        //  {
        //       this.fpSpread.ActiveSheet.Models.Data = ((FarPoint.Win.Spread.Model.GroupDataModel)this.fpSpread.ActiveSheet.Models.Data).TargetModel;
        //   }
    }
        // try
        // {
            //   if ((this.fpSpread.ActiveSheet.Models.Data != null) && ((this.fpSpread.ActiveSheet.Models.Data.GetType() == typeof(GroupDataModel))))
            //   {
                GroupDataModel gm = null;
                gm = fpSpread.ActiveSheet.Models.Data as GroupDataModel;

                int n = gm.SortInfo.Length;
                FarPoint.Win.Spread.SortInfo[] tmp = new FarPoint.Win.Spread.SortInfo[n - 1];
                int i = 0;
                while (i < n - 1)
                {
                    tmp[i] = gm.SortInfo[i];
                    i = i + 1;
                }
                n = n - 1;
                if (n >= 1)
                {
                    GroupingEventArgs fe = new GroupingEventArgs(tmp);
                    if (!fe.Cancel)
                        gm.Group(tmp, fe.GroupComparer);
                    return;
                }
                else 
            //          fpSpread.ActiveSheet.Models.Data = ((FarPoint.Win.Spread.Model.GroupDataModel)fpSpread.ActiveSheet.Models.Data).TargetModel;
            //  }

        //}
        // catch
        // {
        //  }             
  
    // }
    // #endregion */
}