using System;
using System.Collections.Generic;
using System.Net.Sockets;


namespace GameNetwork
{
    // ------------------------------------------------------------------ //
    // 이 코드는 MSDN에(마이크로소프트 개발자 네트워크) 게시된 코드입니다 //
    // ------------------------------------------------------------------ //


    // Represents a collection of reusable SocketAsyncEventArgs objects.  
    class SocketAsyncEventArgsPool
	{
		Stack<SocketAsyncEventArgs> m_pool;

		// Initializes the object pool to the specified size
		//
		// The "capacity" parameter is the maximum number of 
		// SocketAsyncEventArgs objects the pool can hold
		public SocketAsyncEventArgsPool(int capacity)
		{
			m_pool = new Stack<SocketAsyncEventArgs>(capacity);
		}

		// Add a SocketAsyncEventArg instance to the pool
		//
		//The "item" parameter is the SocketAsyncEventArgs instance 
		// to add to the pool
		public void Push(SocketAsyncEventArgs item)
		{
			if (item == null) { throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
			lock (m_pool)
			{
				m_pool.Push(item);
			}
		}

		// Removes a SocketAsyncEventArgs instance from the pool
		// and returns the object removed from the pool
		public SocketAsyncEventArgs Pop()
		{
			lock (m_pool)
			{
				return m_pool.Pop();
			}
		}

		// The number of SocketAsyncEventArgs instances in the pool
		public int Count
		{
			get { return m_pool.Count; }
		}
	}
}
