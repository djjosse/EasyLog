using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyLog
{
    public class MessageListener
    {
        public Object Token { get; private set; }

        public MessageListener()
        {
            AllMessages = new ObservableCollection<Message>();
            Token = new object();
        }

        public ObservableCollection<Message> AllMessages;

        public void Add(Message message)
        {
            lock (Token)
            {
                AllMessages.Add(message);
            }
        }
    }
}
