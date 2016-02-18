using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organiser.Common.Classes
{
    public class ProcessManager
    {
        private static ProcessManager pman;
        public static ProcessManager Instance
        {
            get
            {
                if (pman == null) pman = new ProcessManager();
                return pman;
            }
        }

        public readonly List<Process> Processes = new List<Process>();
        private object mlock = new object();

        private ProcessManager() { }

        public void AddProcess(Process p)
        {
            lock (mlock)
            {
                Processes.Add(p);
            }
        }

        /// <summary>
        /// does not dispose
        /// </summary>
        /// <param name="p"></param>
        public void RemoveProcess(Process p)
        {
            lock (mlock)
            {
                Processes.Remove(p);
            }
        }

        public void DisposeAndRemoveProcess(Process p)
        {
            lock (mlock)
            {
                p.Close();
                p.Dispose();
                Processes.Remove(p);
            }
        }

        public void DisposeAllProcess()
        {
            lock (mlock)
            {
                foreach (Process p in Processes)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch { }
                    p.Close();
                    p.Dispose();
                }
            }
        }
    }
}
