﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reactive;
using System.Reactive.Linq;

namespace LinqHotKey
{
	/// <summary>
	///　マウスもしくはキーボードイベントが発生した時にコールバックされるObservableクラス。
	///　<para>例	: AキーとGキーが押されたらカウントを増やす </para>
	///　<para> DeviceObservable dObserver = new DeviceObservable();
	///　dObserver.Where(x => Input.GetKeyDown(Keys.A) &amp;&amp; Input.GetKeyDown(Keys.F15))
	///			  .Subscribe(x => this.DataContext = new{ MyX = count++});
	/// </para>
	/// </summary>
	public class DeviceObservable : IObservable<DeviceEventArg>
	{
		private Subject<DeviceEventArg> subject  = new Subject<DeviceEventArg>();
		private KBDLLHOOKSTRUCT KBStructZero = new KBDLLHOOKSTRUCT();
		private MSLLHOOKSTRUCT MSStructZero = new MSLLHOOKSTRUCT();

		/// <summary>
		/// コンストラクター
		/// </summary>
		public DeviceObservable()
		{
			//後述のオブザーバーに渡されるDeviceEventArgをここでインスタンス化。
			//パフォーマンス上、イベントが呼ばれる度にインスタンス生成される事を防ぐため。
			DeviceEventArg deviceEventArg = new DeviceEventArg();
			
			/*
			 * キーボードイベントを登録する。
			 * イベントが起きるたびにStart内のデリゲートがコールバックされ、オブザーバーにDeviceEventArgを配信する。
			 */

			DeviceEvent.Start((EventMessage eventName,KBDLLHOOKSTRUCT arg) =>
			{
				//DeviceEventArgにキーボードイベントを詰め込む
				deviceEventArg.KeyEvent = arg;
				deviceEventArg.EventName = eventName;
				deviceEventArg.MouseEvent = MSStructZero;
				subject.OnNext(deviceEventArg);
			},DeviceName.Keyboard);

			DeviceEvent.Start((EventMessage eventName,MSLLHOOKSTRUCT arg) =>
			{
				//DeviceEventArgにマウスイベントを詰め込む
				
				deviceEventArg.MouseEvent = arg;
				deviceEventArg.EventName = eventName;
				deviceEventArg.KeyEvent = KBStructZero;
				subject.OnNext(deviceEventArg);
			},DeviceName.Mouse);
		}

		/// <summary>
		/// SendInputによるイベントじゃなかったら発火するObservableを返す
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public IObservable<DeviceEventArg> Where(Func<DeviceEventArg, bool> predicate)
		{

			return this.Where<DeviceEventArg>(predicate)
				.Where((arg) => arg.KeyEvent.dwExtraInfo.ToInt32() != 128);

		}

		public IDisposable Subscribe(IObserver<DeviceEventArg> observer)
		{
			subject.Subscribe(observer);
			return subject;
		}

		
	}
}
