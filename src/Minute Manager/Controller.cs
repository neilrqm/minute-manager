using System;
using System.Collections.Generic;
using System.Text;

namespace MinuteManager
{
    public class InternalErrorEventArgs
    {
        public string Message { get; private set; }
        public bool Fatal { get; private set; }
        public InternalErrorEventArgs(string message, bool fatal) { Message = message; Fatal = fatal; }
    }
    public delegate void InternalErrorEventHandler(object system, InternalErrorEventArgs args);
    public class Controller
    {
        private IDataSource dataSource;
        public Controller()
        {
            dataSource = new XmlDataSource();
        }
        public event InternalErrorEventHandler OnInternalError
        {
            add { dataSource.OnInternalError += value; }
            remove { dataSource.OnInternalError -= value; }
        }
    }
}
