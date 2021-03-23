using System;
using System.Threading;

namespace Survival_Game_Backend_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Start();

            Thread mainThread = new Thread(new ThreadStart(MainThread));
        }

        static void MainThread()
        {
            while(true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
