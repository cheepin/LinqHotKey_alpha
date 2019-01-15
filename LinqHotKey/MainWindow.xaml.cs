using System;
using System.Linq;
using System.Windows;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Reactive;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Windows.Interop;
using static System.Windows.Forms.DataFormats;

namespace LinqHotKey
{


	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		event Action OnClick;
		event EventHandler<MouseEventArgs> OnClick2;
		delegate int Count();
		Count Count10;
		event Count OnCount10;

		IDisposable observable1;
		IDisposable observable2;

		public MainWindow()
		{
			InitializeComponent();
			//ユースケース 1(F13単押し)
			DeviceObservable keyBoard = new DeviceObservable();
			
			EveryUpdateObservable drivedObject = new EveryUpdateObservable();
			var task = drivedObject.Start(10);
			//drivedObject.Where(_=>Input.GetKeyboardDown(Keys.X,Keys.F1))
			//.Subscribe((_)=> Dispatcher.InvokeAsync(()=> DataContext = new {X = "押されてる" }));
			
			bool flag = false;
			keyBoard.Where(arg=> arg.EventName == EventMessage.RBUTTON_DOWN )
					.Subscribe(arg=>
					{
						arg.OutputParamater();
						Input.SendInputKey(Keys.B);
					});

			//keyBoard.Where(arg=>arg.EventName == EventMessage.WM_MOUSEWHEEL)
			//		.Subscribe(arg=>Console.WriteLine("wPalam ={0}",arg.MouseEvent.mouseData));

			//drivedObject.Where(_=>Input.GetKeyDown(Keys.E))
			//	.Subscribe(_=>Console.WriteLine(String.Format("{0:X8}",Marshal.GetLastWin32Error())));
			//drivedObject.Where(_=>(!Input.GetKeyboardDown(Keys.B) || !Input.GetKeyboardDown(Keys.X) )&&_ % 40 == 0)
			//			.Subscribe((_)=> Dispatcher.InvokeAsync(()=> DataContext = new {X = "押されてない" }));
			drivedObject.Where(_=>_ % 100 == 0).Subscribe((_)=> Dispatcher.InvokeAsync(()=> DataContext = new {X = "押されてない" }));
			//ユースケース 2(F13 & 右クリック)
			//IObservable<DeviceEventArg> keyBoard2 = new KeyBoardStreamMock();
			//keyBoard2.Where(x => x.KeyEvent.Key == Keys.F13 && x.MouseEvent.Button == KeyList.RButton)
			//		.Subscribe(x => Console.WriteLine("Pressed F13 & RButton!!"));
			
			////ユースケース 3(F13を100フレーム押した)
			//IObservable<DeviceEventArg> keyBoard3 = new KeyBoardStreamMock();
			//keyBoard3.Where(x => x.KeyEvent.Key == Keys.F13).Skip(100);

			//TestEvent test = new TestEvent();
			//Observable.FromEventPattern<EventHandler,EventArgs>(
			//	h => new EventHandler(h.Invoke),
			//	h => test.Down += h.Invoke,
			//	h => test.Down -= h.Invoke)
			//	.Subscribe(c => Console.WriteLine("うおおお！"));
			//test.TestCall();
			//test.TestCall();
			//test.TestCall();

		}
		[DllImport("User32.dll")]
		private static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey); // Keys enumeration
		private void Event(object sender, EventArgs e) { Console.WriteLine("Left mouse click!"); }
	}

	public class CounterObservable : IObservable<long>
	{
		private Subject<long> subject = new Subject<long>();
		private int counter = 0;
		
		public void IncleaseCount()
		{
			subject.OnNext(counter++);
		}

		public IDisposable Subscribe(IObserver<long> observer)
		{
			return subject.Subscribe(observer);
		}
	}

	public struct KeyList
	{
		static public string RButton = "RButton";
		static public string F13 = "0x66";
		static public string F14 = "0x68";
	}


	public class DeviceEventArg : EventArgs
	{
		public KBDLLHOOKSTRUCT KeyEvent{get; set;} = new KBDLLHOOKSTRUCT();
		public MSLLHOOKSTRUCT MouseEvent{get; set;} = new MSLLHOOKSTRUCT();
		public EventMessage EventName;
	}

	public class KeyBoardStreamMock : IObservable<DeviceEventArg>
	{
		Subject<DeviceEventArg> subject = new Subject<DeviceEventArg>();
		
		public void CallEvent()
		{
			var args = new DeviceEventArg();
			subject.OnNext(args);
		}

		public IDisposable Subscribe(IObserver<DeviceEventArg> observer)
		{
			subject.Subscribe(observer);
			return subject;
		}


	}



	public class KeyEventArg
	{
		public Keys Key
		{
			get;
			set;
		}
		public bool Pressed
		{
			get;
			set;
		}
	}

	public class MouseEventArg
	{
		public string Button
		{
			get;
			set;
		}
	}

	class MyEventArge : EventArgs
	{
		public string MyName{get;set;}
	}

	public class TestEvent
	{
		public event MouseDown Down;
		public delegate void MouseDown(object target,EventArgs args);

		public void TestCall()
		{

			Down(this,new EventArgs());
		}

	}

	public class MyViewModel : INotifyPropertyChanged
	{
		private string myX;
		public string MyX
		{
			get
			{
				return myX; 
			}
			set
			{ 
				myX = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyX)));
			}}	

		public event PropertyChangedEventHandler PropertyChanged;
		
		
		public MyViewModel()
        {
			int frame = 0;

			//KeyBoardEvent.Start((keyboardStruct) => {MyX = 
			//		 "vkCode = " + keyboardStruct.vkCode + "  scanCode = " + keyboardStruct.scanCode 
			//		+ "\nTime = " + keyboardStruct.time + " flags = " + keyboardStruct.flags + "FLAME : " + frame++;
			//});


			//IObservable<DeviceEventArg> keyBoard = new DeviceObservable();

			//keyBoard.Subscribe.Start((keyboardStruct) => {MyX = 
			//		 "vkCode = " + keyboardStruct.vkCode + "  scanCode = " + keyboardStruct.scanCode 
			//		+ "\nTime = " + keyboardStruct.time + " flags = " + keyboardStruct.flags + "FLAME : " + frame++;

			//});
			//KeyBoardEvent.MouseAction += new EventHandler(Event);

			//Task.Run(()=>{
			//	for(int i=0; i<10; i++)
			//	{
			//		Thread.Sleep(500);
			//		Console.WriteLine(i);
			//	}
			//}).ContinueWith((task)=>{ 
			//	Console.WriteLine("Complete!");
			//	MyX = 420;
			//});

			//MyX = 220;

		}

	}



}
