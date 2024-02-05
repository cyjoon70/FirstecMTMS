#region 작성정보
/*********************************************************************/
// 단위업무명 : 이미지View
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-01-29
// 작성내용 : 이미지View
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

namespace UIForm
{
    public partial class Picture : Form
    {
        string strPicture = null;

        public Picture(string Picture)
        {
            strPicture = Picture;
            InitializeComponent();
        }

        #region 폼 로드
        private void Picture_Load(object sender, EventArgs e)
        {
            try
            {
                if (strPicture != null && strPicture != "")
                {
                    this.c1PictureBox1.Image = new Bitmap(strPicture.ToString());

                    if (c1PictureBox1.Width > 800 || c1PictureBox1.Height > 600)
                    {
                        this.Width = 800;
                        this.Height = 600;
                    }
                    else
                    {
                        this.Width = c1PictureBox1.Width + 18;
                        this.Height = c1PictureBox1.Height + 18;
                    }

                    int XXX = (System.Windows.Forms.SystemInformation.WorkingArea.Width - this.Width) / 2;
                    int YYY = (System.Windows.Forms.SystemInformation.WorkingArea.Height - this.Height) / 2;

                    this.Location = new System.Drawing.Point(XXX, YYY);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show("이미지를 찾을 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
        #endregion

        #region 이미지 리소스 해제
        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.c1PictureBox1.Image.Dispose();
                this.Close();
            }
            catch
            {
            }
        }

        private void panel1_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.c1PictureBox1.Image.Dispose();
                this.Close();
            }
            catch
            {
            }
        }
        #endregion
    }
}
