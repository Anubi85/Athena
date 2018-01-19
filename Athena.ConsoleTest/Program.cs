using Athena.Commons;
using Athena.Commons.LogChannels;
using Athena.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            InternalLogger ilog = new InternalLogger();
            ilog.WriteMessage("test msg 1");
            ilog.WriteMessage("test msg 2");
            ilog.WriteMessage("test msg 3");
            ilog.WriteMessage("test msg 4");
            ilog.WriteMessage("test msg 5");
            var res = ilog.ReadMessages();
            LogManager.Logger.AddChannel<ConsoleLogChannel>();
            LogManager.Logger.CompleteMessageProcessing = false;
            LogManager.Logger.Start();
            LogManager.Logger.WriteTraceMessage("Msg");
            LogManager.Logger.WriteInfoMessage("Info");
            LogManager.Logger.Stop();
            Console.ReadKey();
            LogManager.Logger.Start();
            LogManager.Logger.WriteSuccessMessage("Success");
            LogManager.Logger.WriteWarningMessage("Warning");
            LogManager.Logger.WriteErrorMessage("Error");
            Console.ReadKey();
            LogManager.Logger.Stop();
        }        
    }
}
