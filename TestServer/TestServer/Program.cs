using System;
using System.Threading;

// See https://aka.ms/new-console-template for more information

namespace TestServer
{
    class Program
    {
        private static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.Title = "Test Game Server";
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainHread));
            mainThread.Start();

            Server.Start(50, 26950);
        }


        private static void MainHread()
        {
            Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks per second.");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    GameLogic.Update();
                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if (_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}



