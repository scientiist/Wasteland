namespace Conarium.Generic
{
    public class ThreadSafeValue<T>
	{
		private T _value;
		private object _lock = new object();

		public T Value
		{
			get
			{
				lock (_lock)
					return _value;
			}
			set
			{
				lock (_lock)
					_value = value;
			}
		}

		public ThreadSafeValue(T value = default(T))
		{
			Value = value;
		}
	}
}