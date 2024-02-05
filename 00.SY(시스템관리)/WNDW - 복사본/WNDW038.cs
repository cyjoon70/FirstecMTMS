#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 공정내용저장 추가 삭제 조회
// 작 성 자   : 김한진
// 작 성 일   : 2014-08-27
// 작성내용   : 공정내용
// 수 정 일   :
// 수 정 자   :
// 수정내용   :
// 비    고   :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using EDocument.Network;
#region 예제 - 복사해서 쓰세요
/*
try
{
    WNDW.WNDW038 pu = new WNDW.WNDW038();
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
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 제조오더정보조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 제조오더번호 </para>
    /// <para>Msgs[2] = 제품오더번호 </para>
    /// <para>Msgs[3] = 프로젝트번호 </para>
    /// <para>Msgs[4] = 프로젝트명 </para>
    /// <para>Msgs[5] = 프로젝트차수 </para>
    /// <para>Msgs[6] = 품목코드 </para>
    /// <para>Msgs[7] = 품목명 </para>
    /// </summary>

    public partial class WNDW038 : UIForm.Buttons
    {
        #region 변수선언
        string FileName = "";
        #endregion

        #region WNDW038 생성자

        public WNDW038(string _FileName)
        {
            FileName = _FileName;
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW038_Load(object sender, System.EventArgs e)
        {
            MemoryStream m = Ftp.DownloadFileToStream(FileName, "E2MAX", "zemax");
            PicShow.Image = System.Drawing.Image.FromStream(m);

            PicShow.Width = groupBox2.Width - 130;
            PicShow.Height = groupBox2.Height - 26;

            Decimal ImgHight = PicShow.Image.Size.Height;
            Decimal ImgWidth = PicShow.Image.Size.Width;

            Decimal picHeight = PicShow.Height;
            Decimal HeightCnt = picHeight / ImgHight;
            Decimal WidthCnt = ImgWidth * HeightCnt;

            if ((groupBox2.Width - 130) < Convert.ToInt32(WidthCnt))
            {
                Decimal WidCnt = Convert.ToDecimal(groupBox2.Width - 130) / ImgWidth;
                Decimal HeiCnt = ImgHight * WidCnt;

                PicShow.Height = Convert.ToInt32(HeiCnt);
                PicShow.Width = Convert.ToInt32(groupBox2.Width - 130);
            }
            else
            {
                PicShow.Width = Convert.ToInt32(WidthCnt);
            }

            UIForm.Buttons.ReButton(BtnNew, "BtnNew", false);
            UIForm.Buttons.ReButton(BtnSearch, "BtnSearch", false);
            UIForm.Buttons.ReButton(BtnRCopy, "BtnRCopy", false);
            UIForm.Buttons.ReButton(BtnRowIns, "BtnRowIns", false);
            UIForm.Buttons.ReButton(BtnInsert, "BtnInsert", false);
            UIForm.Buttons.ReButton(BtnCancel, "BtnCancel", false);
            UIForm.Buttons.ReButton(BtnDel, "BtnDel", false);
            UIForm.Buttons.ReButton(BtnDelete, "BtnDelete", false);
            UIForm.Buttons.ReButton(BtnExcel, "BtnExcel", false);

        }
        #endregion
    }
}
