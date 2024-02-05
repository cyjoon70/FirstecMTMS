#region DAA003P30 작성 정보
/*************************************************************/
// 단위업무명 : 원가자료 등록 오류내역보기
// 작 성 자 :   유재규
// 작 성 일 :   2012-10-30
// 작성내용 :   
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 : 원가연계기준정보 Upload및 29항목 테이블 Upload된 자료중 오류내역을 확인
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

namespace DA.DAA003
{
    public partial class DAA003P30 : UIForm.FPCOMM1
    {
        string strKeyGroup = "";

        public DAA003P30()
        {
            InitializeComponent();
        }

        #region DAA003P30(KEY_GROUP)
        public DAA003P30(string KEY_GROUP)
        {
            strKeyGroup = KEY_GROUP;
         
            InitializeComponent();
        }
        #endregion

        private void DAA003P30_Load(object sender, EventArgs e)
        {
            //Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dSave.gif");
            //BtnInsert.Image = bitMap;
            //BtnInsert.Enabled = false;

            //Bitmap bitMap2 = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\dDelete.gif");
            //BtnDelete.Image = bitMap2;
            //BtnDelete.Enabled = false;
            
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox1);  

 
            string strSql = " usp_DAA003 ";
            strSql += "  @pTYPE = 'S4' ";
            strSql += ", @pKEY_GROUP = '" + strKeyGroup + "' ";
 
            UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 3, true);

        }
    }
}
