using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Main.Logs;

namespace Main.Thrs
{
    public class ParallelThr
    {
        public event EventHandler StartParallelThreadsEnded;

        async public void StartParallelThreads(MethodInfo methodToStart, List<object[]> thrsParameters, int maxParallelThrs = 0, int thrTimeOutSec = 0)
        {
            try
            {
                //Thread thrsManager = Thr.AvviaNuovo(() => ParallelThreadsManager(start, ThrsParameters, maxParallelThrs, thrTimeOut));
                Task<bool> taskThread = Task<bool>.Factory.StartNew(() => ParallelThreadsManager(methodToStart, thrsParameters, maxParallelThrs, thrTimeOutSec));
                await taskThread;

                if (taskThread.Result == true)
                    StartParallelThreadsEnded?.Invoke(this, new GenericEventArgs());
                else
                    StartParallelThreadsEnded?.Invoke(this, new GenericEventArgs(inErr: true));
            }
            catch (Exception ex)
            {
                StartParallelThreadsEnded?.Invoke(this, new GenericEventArgs(descErr: ex.Message,  inErr: true));
            }
        }

        private bool ParallelThreadsManager(MethodInfo methodToStart, List<object[]> thrsParameters, int maxParallelThrs, int thrTimeOutSec)
        {
            if (maxParallelThrs < 0)
            {
                Log.main.Add(new Mess(LogType.ERR, "", "Ricevuto maxParallelThrs minore di 0"));
                return false;
            }

            if (thrTimeOutSec <= 0)
            {
                Log.main.Add(new Mess(LogType.ERR, "", "Ricevuto thrTimeOutSec minore uguale a 0"));
                return false;
            }

            List<ThreadInfo> threadsInfo = new List<ThreadInfo>();
            foreach (object[] parameters in thrsParameters)
            {
                while (true)
                {
                    KillThrsInTimeOut(threadsInfo, thrTimeOutSec);
                    if (IsMaxParallelThrReached(threadsInfo, maxParallelThrs) == false) break;
                    Thread.Sleep(1);
                }

                ThreadInfo thrInfo = new ThreadInfo();
                thrInfo.thread = Thr.AvviaNuovo(() => CallMethod(methodToStart, parameters));
                thrInfo.startTime = DateTime.Now;
                threadsInfo.Add(thrInfo);
            }

            while ((from tmp in threadsInfo where tmp.thread.IsAlive == true select tmp).Count() > 0) //Attesa ultimazione dei thread
            { KillThrsInTimeOut(threadsInfo, thrTimeOutSec); }

            return true;
        }

        private bool IsMaxParallelThrReached(List<ThreadInfo> threadsInfo, int maxParallelThrs)
        {
            if (maxParallelThrs > 0 && (from tmp in threadsInfo where tmp.thread.IsAlive == true select tmp).Count() >= maxParallelThrs)
                return true;
            else
                return false;
        }

        private void KillThrsInTimeOut(List<ThreadInfo> threadsInfo, int thrTimeoutSec)
        {
            foreach (var threadInTimeout in (from tmp in threadsInfo where tmp.thread.IsAlive == true && (DateTime.Now - tmp.startTime).TotalSeconds > thrTimeoutSec select tmp.thread))
            {
                threadInTimeout.Abort();
            }
        }

        private void CallMethod(MethodInfo methodToStart, object[] parameters)
        {
            //MethodInfo theMethod = start.Method;
            methodToStart.Invoke(null, parameters);
        }

        private struct ThreadInfo
        {
            public Thread thread;
            public DateTime startTime;
        }
    }
}
