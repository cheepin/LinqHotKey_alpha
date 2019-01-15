using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LinqHotKey
{
	static public class Input
	{
		// マウスイベント(mouse_eventの引数と同様のデータ)
		[StructLayout(LayoutKind.Sequential)]
		private struct MOUSEINPUT
		{
			public int dx;
			public int dy;
			public int mouseData;
			public int dwFlags;
			public int time;
			public int dwExtraInfo;
		};

		// キーボードイベント(keybd_eventの引数と同様のデータ)
		[StructLayout(LayoutKind.Sequential)]
		private struct KEYBDINPUT
		{
			public short wVk;
			public short wScan;
			public int dwFlags;
			public int time;
			public int dwExtraInfo;
		};

		// ハードウェアイベント
		[StructLayout(LayoutKind.Sequential)]
		private struct HARDWAREINPUT
		{
			public int uMsg;
			public short wParamL;
			public short wParamH;
		};

		// 各種イベント(SendInputの引数データ)
		[StructLayout(LayoutKind.Explicit)]
		private struct INPUT
		{
			[FieldOffset(0)] public int type;
			[FieldOffset(4)] public MOUSEINPUT mi;
			[FieldOffset(4)] public KEYBDINPUT ki;
			[FieldOffset(4)] public HARDWAREINPUT hi;
		};



		private const int KEYEVENTF_KEYDOWN = 0x0;          // キーを押す
		private const int KEYEVENTF_KEYUP = 0xFF;            // キーを離す
		private const int KEYEVENTF_EXTENDEDKEY = 0x1;      // 拡張コード
		private const int VK_SHIFT = 0x10;                  // SHIFTキー
		private const int INPUT_MOUSE = 0;                  // マウスイベント
		private const int INPUT_KEYBOARD = 1;               // キーボードイベント
		private const int INPUT_HARDWARE = 2;               // ハードウェアイベント
		
		[DllImport("User32.dll",SetLastError=true)]
		private static extern short GetKeyState(Keys Key);
		[DllImport("User32.dll",SetLastError=true)]
		private static extern short GetAsyncKeyState(Keys Key);
		private const int GET_KEY_STATE = 0;
		[DllImport("user32.dll")]
		private static extern uint SendInput (int nInputs,ref INPUT pInputs,int cbSize);
		
		// 仮想キーコードをスキャンコードに変換
		[DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
		private extern static int MapVirtualKey(int wCode, int wMapType);


		static public bool GetKeyDown(Keys key)
		{
			return GetKeyState(key) < GET_KEY_STATE;
		}
		
		static public bool GetKeyDown(params Keys[] keys)
		{
			foreach(var key in keys)
			{
				if(GetKeyState(key) >= GET_KEY_STATE)
					return false;
			}
			return true;
		}

		
			
		static public void SendInputKey(Keys inputKey)
		{
			const int num = 1;
			INPUT[] inp = new INPUT[num];
			inp[0].type = INPUT_KEYBOARD;
			inp[0].ki.wVk = (short)inputKey;
			inp[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
			inp[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
			inp[0].ki.dwExtraInfo = 128;
			inp[0].ki.time = 0;
			SendInput(num,ref inp[0],Marshal.SizeOf(inp[0]));
		}

	}

	static public class Msg
	{
		[DllImport("user32.dll", SetLastError=true)]
		static extern int MessageBox(IntPtr hwnd, string text, string title, uint type);
		
		static public int ShowMessageBox(IntPtr handle,string text,string title)
		{
			return MessageBox(handle,text,title,0);
		}
	}
}
