using System;
using System.Collections.Generic;


namespace GameNetwork
{
    public class PacketBufferManager
	{
		static object csBuffer = new object();
		static Stack<Packet> pool;
		static int poolCapacity;

		public static void Initialize(int capacity)
		{
			pool = new Stack<Packet>();
			poolCapacity = capacity;
			Allocate();
		}

		static void Allocate()
		{
			for (int i = 0; i < poolCapacity; ++i)
			{
				pool.Push(new Packet());
			}
		}

		public static Packet Pop()
		{
			lock (csBuffer)
			{
				if (pool.Count <= 0)
				{
					Console.WriteLine("reallocate.");
					Allocate();
				}

				return pool.Pop();
			}
		}

		public static void Push(Packet packet)
		{
			lock(csBuffer)
			{
				pool.Push(packet);
			}
		}
	}
}
