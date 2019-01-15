using System;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace LinqHotKey
{
	[StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
		public uint vkCode;
		public uint scanCode;
		public uint flags;
		public uint time;
		public IntPtr dwExtraInfo;
    }

	[StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public int mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

	public enum DeviceName
	{
		Keyboard = 13,
		Mouse = 14
	}

	public enum EventMessage
	{
		LBUTTON_DOWN = 0x0201,
		LBUTTON_UP = 0x0202,
		RBUTTON_DOWN = 0x0204,
		RBUTTON_UP  =0x0205,
		WM_MOUSEWHEEL =0x020A,
		WM_MOUSEWHEEL_SIDE =0x020B,
		KEYUP = 0x0101,
		KEYDOWN = 0x0100,
	}

	public static class DeviceEvent
    {
		private static event Action<EventMessage,KBDLLHOOKSTRUCT> KeyBoardAction;
		private static event Action<EventMessage,MSLLHOOKSTRUCT> MouseAction;
        private delegate IntPtr LowLevelDeviceProc(int nCode, IntPtr wParam, IntPtr lParam);
		private static readonly LowLevelDeviceProc KeyboardProc = HookKeyboardCallBack;
		private static readonly LowLevelDeviceProc MouseProc = HookMouseCallBack;
        private static IntPtr _hookID = IntPtr.Zero;
        
		public static void Start(Action<EventMessage,KBDLLHOOKSTRUCT> callback,DeviceName deviceName)
        {
            _hookID = SetHook(KeyboardProc,(int)deviceName);
			KeyBoardAction += callback;
        }

        public static void Start(Action<EventMessage,MSLLHOOKSTRUCT> callback,DeviceName deviceName)
        {
            _hookID = SetHook(MouseProc,(int)deviceName);
			MouseAction += callback;
        }
        public static void Stop()
        {
            UnhookWindowsHookEx(_hookID);
        }



		private static IntPtr SetHook(LowLevelDeviceProc proc,int deviceName)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookExA(deviceName, proc,
                  GetModuleHandle(curModule.ModuleName), 0);
            }
        }


        private static IntPtr HookKeyboardCallBack(
          int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
				KeyBoardAction?.Invoke((EventMessage)wParam,hookStruct);
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static IntPtr HookMouseCallBack(
          int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
				MouseAction?.Invoke((EventMessage)wParam,hookStruct);
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookExA(int idHook,
          LowLevelDeviceProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
          IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
