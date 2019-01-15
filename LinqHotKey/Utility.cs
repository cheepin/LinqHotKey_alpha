using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinqHotKey
{
	static public class Utility
	{
		public static void OutputParamater(this DeviceEventArg arg)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("--------------------\n");
			stringBuilder.Append("EventArg = " + arg.EventName + "\n");
			stringBuilder.Append("Key = " + (Keys)arg.KeyEvent.vkCode + "\n");
			stringBuilder.Append("dwExtraInfo = " + arg.MouseEvent.dwExtraInfo + "\n");
			stringBuilder.Append("--------------------\n");
			Console.WriteLine(stringBuilder);
		}
	}
}
