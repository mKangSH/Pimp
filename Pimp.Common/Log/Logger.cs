using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimp.Common.Log
{
    public class Logger
    {
        // singleton 
        private static Logger _instance;
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }

        public delegate void LogAddedDelegate(string log);
        public event LogAddedDelegate LogAdded;

        public void AddLog(string log)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            LogAdded?.Invoke($"{log}");
        }
    }
}
