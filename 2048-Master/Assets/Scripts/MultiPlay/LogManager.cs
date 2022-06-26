using UnityEngine;

public static class LogManager
{
	public static void log(string format)
	{
		Debug.Log(string.Format("[{0}] {1}", System.Threading.Thread.CurrentThread.ManagedThreadId, format));
	}
}

