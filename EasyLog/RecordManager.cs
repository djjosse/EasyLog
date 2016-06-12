using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyLog
{
    public class RecordManager
    {
        private const string LOG_FILE_TIME_FORMAT = "yyyy-MM-dd_HH-mm-ss.ff";
        private const string LOG_FILE_PREFIX = "log_";
        private const string LOG_FILE_EXTENSION = ".log";

        private object _listenerToken = new object();
        private object _queueToken = new object();
        private Queue<Message> _messageQueue = new Queue<Message>();
        private List<MessageListener> _listeners = new List<MessageListener>();

        private readonly string _logFileName;

        private RecordManager(string fileName)
        {
            _logFileName = fileName;
            using (StreamWriter file = new StreamWriter(_logFileName, false))
            {
                file.WriteLine("Creating Log File");
            }
        }

        public static RecordManager Create()
        {
            string time = DateTime.Now.ToString(LOG_FILE_TIME_FORMAT);
            string fileName = String.Format("{0}{1}{2}", LOG_FILE_PREFIX, time, LOG_FILE_EXTENSION);
            return new RecordManager(fileName);
        }

        public void Print(string message, string method, eCategory category, DateTime time, string module)
        {
            lock (_queueToken)
            {
                _messageQueue.Enqueue(new Message()
                {
                    Text = message,
                    Time = time,
                    Category = category,
                    Method = method,
                    Module = module
                });
            }
            Start();
        }

        private Thread _worker;
        private object _threadToken = new object();
        private void Start()
        {
            if (_worker == null || !_worker.IsAlive)
            {
                lock (_threadToken)
                {
                    if (_worker == null || !_worker.IsAlive)
                    {
                        _worker = new Thread(() =>
                        {
                            GetMessagesFromQueue();
                        });
                        _worker.IsBackground = true;
                        _worker.Start();
                    }
                }
            }
        }

        private void GetMessagesFromQueue()
        {
            while (_messageQueue.Count > 0)
            {
                try
                {
                    Message message = null;
                    lock (_queueToken)
                    {
                        message = _messageQueue.Dequeue();
                    }
                    WriteMessageToFile(message);
                    WriteMessageToCommonList(message);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private void WriteMessageToFile(Message message)
        {
            using (StreamWriter file = new StreamWriter(_logFileName, true))
            {
                file.WriteLine(String.Format("{0}\t{1}\t{2}\t{3} {4}", message.Time.ToString("HH:mm:ss.ffff"), message.Module, message.Category, message.Method, message.Text));
            }
        }

        public void AttachListener(MessageListener messageCollection)
        {
            lock (_listenerToken)
            {
                _listeners.Add(messageCollection);
            }
        }

        public void DetachListener(MessageListener messageCollection)
        {
            lock (_listenerToken)
            {
                if (_listeners.Contains(messageCollection))
                    _listeners.Remove(messageCollection);
            }
        }

        private void WriteMessageToCommonList(Message message)
        {
            lock (_listenerToken)
            {
                foreach (MessageListener listener in _listeners)
                {
                    listener.Add(message);
                }
            }
        }
    }
}
