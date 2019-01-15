using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace LinqHotKey
{
	public class EveryUpdateObservable : IObservable<int>
	{
		Subject<int> subject = new Subject<int>();

		public async Task Start(int interval)
		{
			int count = 0;
			while(true)
			{
				subject.OnNext(count++);
				await Task.Delay(interval);
			}
		}

		public IDisposable Subscribe(IObserver<int> observer)
		{
			return subject.Subscribe(observer);
		}
	}
}
