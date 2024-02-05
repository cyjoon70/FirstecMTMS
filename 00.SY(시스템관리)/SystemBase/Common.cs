using System;
using System.IO;

namespace SystemBase
{
	/// <summary>
	/// Common에 대한 요약 설명입니다.
	/// </summary>
	public class Common
	{
		private Common()
		{
		}

		static Common()
		{
		}

		// -----------------------------------------------
		//	* Attribute
		// -----------------------------------------------

		private static int intCommandTimeOut = 60;

		// -----------------------------------------------
		//	* Method
		// -----------------------------------------------

		public static int CommandTimeOut
		{
			get { return intCommandTimeOut; }
		}

		public static string getBackUpPath()
		{
			try
			{
				DateTime now = DateTime.Now;
				string strBackUpFolder = AutoConfig.MBackupFolder;
				
				if (Directory.Exists(strBackUpFolder) == false) 
				{
					Directory.CreateDirectory(strBackUpFolder);
				}

				string strYYYYMMDD             = now.ToString("yyyyMMdd");
				string strBackUpYYYYMMFolder   = "";
				string strBackUpYYYYMMDDFolder = "";
				
				// MONTH Folder
				strBackUpYYYYMMFolder = strBackUpFolder + strYYYYMMDD.Substring(0, 6) + "\\";

				if (Directory.Exists(strBackUpYYYYMMFolder) == false) 
				{
					Directory.CreateDirectory(strBackUpYYYYMMFolder);
				}

				// DATE Folder
				strBackUpYYYYMMDDFolder = strBackUpYYYYMMFolder + strYYYYMMDD + "\\";

				if (Directory.Exists(strBackUpYYYYMMDDFolder) == false) 
				{
					Directory.CreateDirectory(strBackUpYYYYMMDDFolder);
				}

				return strBackUpYYYYMMDDFolder;

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

        public static string getBackUpPath(string sType)
        {
            try
            {
                DateTime now = DateTime.Now;
                string strBackUpFolder = AutoConfig.OBackupFolder;
				
                if (Directory.Exists(strBackUpFolder) == false) 
                {
                    Directory.CreateDirectory(strBackUpFolder);
                }

                string strBackUpTypeFolder = strBackUpFolder + sType + "\\";
                
                if (Directory.Exists(strBackUpTypeFolder) == false) 
                {
                    Directory.CreateDirectory(strBackUpTypeFolder);
                }

                string strYYYYMMDD             = now.ToString("yyyyMMdd");
                string strBackUpYYYYMMFolder   = "";
                string strBackUpYYYYMMDDFolder = "";
				
                // MONTH Folder
                strBackUpYYYYMMFolder = strBackUpTypeFolder + strYYYYMMDD.Substring(0, 6) + "\\";

                if (Directory.Exists(strBackUpYYYYMMFolder) == false) 
                {
                    Directory.CreateDirectory(strBackUpYYYYMMFolder);
                }

                // DATE Folder
                strBackUpYYYYMMDDFolder = strBackUpYYYYMMFolder + strYYYYMMDD + "\\";

                if (Directory.Exists(strBackUpYYYYMMDDFolder) == false) 
                {
                    Directory.CreateDirectory(strBackUpYYYYMMDDFolder);
                }

                return strBackUpYYYYMMDDFolder;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
