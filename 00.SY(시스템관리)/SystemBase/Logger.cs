using System;
using System.Reflection;
using System.IO;

namespace SystemBase
{
	/// <summary>
	/// Logger에 대한 요약 설명입니다.
	/// </summary>
	public enum LogLevel
	{		
		Info	,
		Warning ,
		Error	,
		None		
	}
	
	public class Logger
	{
		private StreamWriter sw;

		public Logger()
		{
		}

		private const string LOG_FOLDER  = "\\Log";
		private const string FILE_HEADER = "MHMS_ADMIN.";

		public bool Open()
		{
			try
			{
				string strLogFile = SetLogFolder();
				
				if (strLogFile != null) 
				{
					sw = File.AppendText(strLogFile);
				}
			}
			catch (Exception)
			{
				sw = null;
			}
			return (sw != null);
		}

		public void Close()
		{
			if (sw != null)
			{
				try
				{
					sw.Close();
				}
				catch (Exception)
				{
				}
			}
		}

	
		public void Log(Exception ex)
		{
			if ((int)Settings.LogLevel <= (int)LogLevel.Error)
			{
				Log(LogLevel.Error, ex.Message + "\r\n" + ex.StackTrace);
			}
		}

		private void Log(LogLevel level, string msg)
		{
			if (sw != null)
			{
				try
				{
					sw.WriteLine(DateTime.Now.ToString() + " >> " + level.ToString() + " : " + msg);
					sw.Flush();
				}
				catch (Exception)
				{
				}
			}
		}

		public void LogInfo(string msg)
		{
			if ((int)Settings.LogLevel <= (int)LogLevel.Info)
			{
				Log(LogLevel.Info, msg);
			}
		}

		public void LogWarning(string msg)
		{
			if ((int)Settings.LogLevel <= (int)LogLevel.Warning)
			{
				Log(LogLevel.Warning, msg);
			}
		}

		public void LogError(string msg)
		{
			if ((int)Settings.LogLevel <= (int)LogLevel.Error)
			{
				Log(LogLevel.Error, msg);
			}
		}


		public static void StaticLog(string msg)
		{
			StreamWriter sw = null;
			try
			{
				string strLogFile = SetLogFolder();

				sw = File.AppendText(strLogFile);

				sw.WriteLine(DateTime.Now.ToString() + ">> " + msg);
			}
			catch (Exception)
			{
			}
			finally
			{
				if (sw != null)
				{
					try
					{
						sw.Close();
					}
					catch (Exception)
					{
					}
				}
			}
		}

		private static string SetLogFolder() 
		{
			try
			{
				DateTime now = DateTime.Now;
				string strLogFolder = Directory.GetCurrentDirectory() + LOG_FOLDER;
				
				if (Directory.Exists(strLogFolder) == false) 
				{
					Directory.CreateDirectory(strLogFolder);
				}

				string strYYYYMMDD        = now.ToString("yyyyMMdd");
				string strLogYYYYMMFolder = strLogFolder + "\\" + strYYYYMMDD.Substring(0, 6);

				if (Directory.Exists(strLogYYYYMMFolder) == false) 
				{
					Directory.CreateDirectory(strLogYYYYMMFolder);
				}

				return strLogYYYYMMFolder + "\\" + FILE_HEADER + strYYYYMMDD + ".log";
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
