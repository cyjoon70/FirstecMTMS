using System;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Threading;

namespace SystemBase
{
	/// <program>
	///   <date>2004-11-15</date><writer>김재홍</writer>
	/// </program>
	/// <summary>
	/// Application 에서 사용하는 Message의 다국어 처리를 위한 공통모듈을 관리하는 CLASS입니다.
	/// </summary>
	public class LocalMessages
	{
		public LocalMessages()
		{
		}

		/* -----------------------------------------------
			* Attribute
		   ----------------------------------------------- */
		const string LOCALIZATION_RESOURCE_NAME = "Innobank.Common.ApplicationMessages";


		/* -----------------------------------------------
			* Method
		   ----------------------------------------------- */


		/// <program>
		///   <date>2004-11-15</date><writer>김재홍</writer>
		/// </program>
		/// 
		/// <summary>
		/// 다국어 Resource 정보로부터 MESSAGE를 가져온다.
		/// </summary>
		/// <param name="strResName">Resource Item Name</param>
		/// <returns></returns>
		public static string getMsgString(string strResName)
		{
			// Create a resource manager to retrieve resources.
			ResourceManager rm = new ResourceManager(LOCALIZATION_RESOURCE_NAME, 
				Assembly.GetExecutingAssembly());

			// Get the culture of the currently executing thread.
			CultureInfo ci = Thread.CurrentThread.CurrentCulture;
        
			// Retrieve the value of the string resource named 
			return rm.GetString(strResName, ci);

			
		}

	}


}
