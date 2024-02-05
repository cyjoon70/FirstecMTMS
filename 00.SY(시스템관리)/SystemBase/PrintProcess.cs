using System;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace SystemBase
{
	/// <summary>
	/// SystemBase.PrintProcess pp = new SystemBase.PrintProcess(FILE_NAME);
	/// pp.FilePrint();
	/// </summary>
	public class PrintProcess
	{
		string[] FILE_NAMES = null;
//		public PrintProcess()
//		{
//		}

		public PrintProcess(string[] FILE_NAME)
		{
			FILE_NAMES = FILE_NAME;
		}

		#region 파일 프린트
		public void FilePrint()
		{	//
			try
			{
				Thread th = new Thread(new ThreadStart(FilePrints));
				th.Start();
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("FilePrint", f.ToString());
				MessageBox.Show("파일 프린트 중 예기치 못한 오류가 발생하였습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 파일 프린트
		public void FilePrints()
		{	//
			try
			{

				System.Diagnostics.Process pr = new System.Diagnostics.Process(); 
				pr.StartInfo.Verb = "Print";

				for(int i=0; i<FILE_NAMES.Length; i++)
				{
					if(FILE_NAMES[i] != "" && FILE_NAMES[i] != null)
					{
						pr.StartInfo.FileName = FILE_NAMES[i];
						pr.Start();
					}
				}	
				pr.WaitForInputIdle();
				pr.WaitForExit();
				pr.Close();
				pr.Dispose();
				//pr.Kill();
				//Thread.Sleep(1000);

				/*///////////프로세스 Kill/////////////
				System.Diagnostics.Process[] proc1 = System.Diagnostics.Process.GetProcessesByName("AcroRd32");  //실행파일명
				int ProcessCnt2 = proc1.Length;  //프로세스 로드 수
				//if(ProcessCnt2 > 0) Check = true;

				for(int j = 0; j < ProcessCnt2; j++)
				{	
					proc1[j].Kill();
				}
				*////////////프로세스 Kill/////////////

				////////////로칼저장파일 삭제/////////////
				/*
				if(FILE_NAMES.Length > 0)
				{
					for(int i = 0; i < FILE_NAMES.Length; i++)
					{
						if(FILE_NAMES[i] != null && FILE_NAMES[i].ToString().Substring(FILE_NAMES[i].ToString().Length-3, 3).ToUpper() == "PDF" )
							File.Delete(FILE_NAMES[i]);
					}
				}
				*/
				////////////로칼저장파일 삭제/////////////

			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("FilePrint", f.ToString());
				MessageBox.Show("파일 프린트 중 예기치 못한 오류가 발생하였습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion
	}
}
