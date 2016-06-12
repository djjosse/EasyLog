using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyLog
{
    public sealed class Log
    {
        public const string ALL = "All";

        /// <summary>
        /// Log Instance Creation Synchronization Token 
        /// </summary>
        private static object _token = new object();

        /// <summary>
        /// Log Manager Instance
        /// </summary>
        private RecordManager _logManager;

        /// <summary>
        /// Log Singleton Instance
        /// </summary>
        private static Log _instance;

        /// <summary>
        /// Log Singleton Instance Property 
        /// </summary>
        private static Log Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_token)
                    {
                        if (_instance == null)
                        {
                            _instance = new Log();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Log Constructor
        /// </summary>
        private Log()
        {
            _logManager = RecordManager.Create();
        }

        /// <summary>
        /// Print message to log
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="category">Message category [Default is eCategory.Debug]</param>
        /// <param name="module">Responsible module [Default is eModule.Common]</param>
        /// <param name="method">Responsible method name [Default for autocomplete]</param>
        /// <param name="sourceFile">Responsible source file name [Default for autocomplete]</param>
        /// <param name="lineNumber">Responsible source file line number [Default for autocomplete]</param>
        public static void Print(string message, eCategory category = eCategory.Debug, string module = ALL,
            [CallerMemberName]string method = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int lineNumber = 0)
        {
            DateTime time = DateTime.Now;
            string call = String.Format("[{0}]({1}:{2})", method, Path.GetFileName(sourceFile), lineNumber);
            Instance._logManager.Print(message, call, category, time, module);
        }

        /// <summary>
        /// Print message to log
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="module">Responsible module</param>
        /// <param name="category">Message category [Default is eCategory.Debug]</param>
        /// <param name="method">Responsible method name [Default for autocomplete]</param>
        /// <param name="sourceFile">Responsible source file name [Default for autocomplete]</param>
        /// <param name="lineNumber">Responsible source file line number [Default for autocomplete]</param>
        public static void Print(string message, string module, eCategory category = eCategory.Debug,
            [CallerMemberName]string method = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int lineNumber = 0)
        {
            Print(message, category, module, method, sourceFile, lineNumber);
        }

        /// <summary>
        /// Attach message listener to all log prints to receive log messages
        /// </summary>
        /// <param name="messageListener">New message listener to be attached</param>
        public static void AttachListener(MessageListener messageListener)
        {
            Instance._logManager.AttachListener(messageListener);
        }

        /// <summary>
        /// Detach message listener from all log prints to stop receiving log messages
        /// </summary>
        /// <param name="messageListener">Message listener to be detached</param>
        public static void DetachListener(MessageListener messageListener)
        {
            Instance._logManager.DetachListener(messageListener);
        }
    }
}
