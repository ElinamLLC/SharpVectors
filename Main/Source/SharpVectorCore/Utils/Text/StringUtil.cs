using System;

namespace SharpVectors.Text
{
	public class StringUtil
	{
		public static bool IsInArray(string[] arrayToLookIn, string strToFind)
		{
			foreach (string item in arrayToLookIn)
			{
				if (item == strToFind) 
                    return true;
			}
			return false;
		}
	}
}
