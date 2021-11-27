using System;
using System.Collections;
using System.Collections.Generic;

namespace Conarium.Generic
{
    public class DiskArray<T> : IEnumerable<T> {
        private T[] buffer;
		private int next = 0;

		public int Size {get; private set;}

		public DiskArray(int bufferSize) {
			Size = bufferSize;
			buffer = new T[Size];
		}


		public T this[int i] {
			get => Get(i);
			set => Set(i, value);
		}


		public void Clear()
		{
			buffer = new T[Size];
		}

		public T Get(int index)
		{
			return buffer[index % Size];
		}
		public void Get(int index, out T obj)
		{
			obj = buffer[index % Size];
		}

		public void Set(int index, T obj)
		{
			buffer[index % Size] = obj;
		}

		public void Next(T obj)
		{
			buffer[next % Size] = obj;
			next++;
		}

		public T[] GetBuffer() => buffer;

		public IEnumerator<T> GetEnumerator() => 
			((IEnumerable<T>)buffer).GetEnumerator();
		

		IEnumerator IEnumerable.GetEnumerator() => buffer.GetEnumerator();

    }
}