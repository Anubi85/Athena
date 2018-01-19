using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Athena.Commons
{
    /// <summary>
    /// An internal logger used to log messages from the logger itself.
    /// </summary>
    internal sealed class InternalLogger : IDisposable
    {
        #region IDisposable interface

        /// <summary>
        /// Release object resources.
        /// </summary>
        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }
            AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
            m_Mutex.WaitOne();
            short instanceCount = ReadHeaderValue(DataTypes.InstanceCount);
            WriteHeaderValue(DataTypes.InstanceCount, --instanceCount);
            m_Mutex.ReleaseMutex();
            m_Mutex.Close();
            m_Mutex.Dispose();
            m_Reader.Dispose();
            m_BReader.Dispose();
            m_Writer.Dispose();
            m_BWriter.Dispose();
            m_MMFStream.Close();
            m_MMFStream.Dispose();
            if (instanceCount == 0)
            {
                m_MMF.Dispose();
            }
            m_IsDisposed = true;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Flag that idicates if the dispose method has already been called.
        /// </summary>
        private bool m_IsDisposed;
        /// <summary>
        /// The <see cref="Mutex"/> used to sincronize access to <see cref="MemoryMappedFile"/>.
        /// </summary>
        private Mutex m_Mutex;
        /// <summary>
        /// The <see cref="MemoryMappedFile"/> where the message are read/written.
        /// </summary>
        private MemoryMappedFile m_MMF;
        /// <summary>
        /// A <see cref="Stream"/> object that allow access to the <see cref="MemoryMappedFile"/>.
        /// </summary>
        private MemoryMappedViewStream m_MMFStream;
        /// <summary>
        /// <see cref="StreamWriter"/> used to simplify write operation to <see cref="MemoryMappedFile"/>.
        /// </summary>
        private StreamWriter m_Writer;
        /// <summary>
        /// <see cref="StreamReader"/> used to simplify read operation from <see cref="MemoryMappedFile"/>.
        /// </summary>
        private StreamReader m_Reader;
        /// <summary>
        /// <see cref="BinaryWriter"/> used to simplify write operation to <see cref="MemoryMappedFile"/>.
        /// </summary>
        private BinaryWriter m_BWriter;
        /// <summary>
        /// <see cref="BinaryReader"/> used to simplify write operation to <see cref="MemoryMappedFile"/>.
        /// </summary>
        private BinaryReader m_BReader;
        /// <summary>
        /// <see cref="Encoding"/> class used to encode strings to bytes.
        /// </summary>
        private Encoding m_StringEncoding;
        /// <summary>
        /// The name of the process that create the <see cref="InternalLogger"/> instance.
        /// </summary>
        private string m_ProcessName;
        /// <summary>
        /// Number of byte of the encoded <see cref="m_ProcessName"/> string.
        /// </summary>
        private int m_ProcessNameByteCount;
        /// <summary>
        /// The ID of the last message read from the current instance.
        /// </summary>
        private short m_LastReadID;

        #endregion

        #region Constants

        /// <summary>
        /// <see cref="MemoryMappedFile"/> name when used in user session.
        /// </summary>
        private const string c_MMFName = "InternalLoggerMMF";
        /// <summary>
        /// <see cref="Mutex"/> name when used in user session.
        /// </summary>
        private const string c_MutexName = "InternalLoggerMutex";
        /// <summary>
        /// Prefix to be added when using system wide <see cref="InternalLogger"/>.
        /// </summary>
        private const string c_GlobalNamePrefix = "Global\\";
        /// <summary>
        /// Maximum number of characters in one message.
        /// </summary>
        private const int c_MaxMsgSize = 512;
        /// <summary>
        /// Maximum number of messages.
        /// </summary>
        private const int c_MaxMsgNum = 32;
        /// <summary>
        /// Size of the <see cref="MemoryMappedFile"/> header.
        /// The header contains:
        /// - byte 1 -> Instance count.
        /// - byte 2 -> Index of the last written message.
        /// - byte 3 -> Total number of messages.
        /// - byte 4 -> Not used.
        /// </summary>
        private const int c_HeaderSize = 4;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new <see cref="InternalLogger"/> instance connecting it to the correct <see cref="MemoryMappedFile"/>.
        /// </summary>
        /// <param name="global">Indicates if the <see cref="MemoryMappedFile"/> shall be created in the global or user memory space.</param>
        public InternalLogger(bool global = false)
        {
            //create MMF, mutex and view stream
            m_MMF = MemoryMappedFile.CreateOrOpen(global ? c_GlobalNamePrefix + c_MMFName : c_MMFName, c_MaxMsgSize * c_MaxMsgNum + c_HeaderSize);
            m_Mutex = new Mutex(false, global ? c_GlobalNamePrefix + c_MutexName : c_MutexName);
            m_MMFStream = m_MMF.CreateViewStream();
            //initialize stream readers and writers
            m_StringEncoding = new UTF8Encoding(false);
            m_Writer = new StreamWriter(m_MMFStream, m_StringEncoding, c_MaxMsgSize, true);
            m_Writer.AutoFlush = true;
            m_BWriter = new BinaryWriter(m_MMFStream, m_StringEncoding, true);
            m_Reader = new StreamReader(m_MMFStream, m_StringEncoding, false, c_MaxMsgSize, true);
            m_BReader = new BinaryReader(m_MMFStream, m_StringEncoding, true);
            //update instance count
            m_Mutex.WaitOne();
            short instanceCount = ReadHeaderValue(DataTypes.InstanceCount);
            WriteHeaderValue(DataTypes.InstanceCount, ++instanceCount);
            m_Mutex.ReleaseMutex();
            //get current process name
            m_ProcessName = Process.GetCurrentProcess().ProcessName;
            m_ProcessNameByteCount = m_StringEncoding.GetByteCount(m_ProcessName);
            //register to application exit event
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Call the dispose method that release resources on process exit.
        /// </summary>
        /// <param name="sender">The object that generate the event.</param>
        /// <param name="e">Event parameters.</param>
        private void OnProcessExit(object sender, EventArgs e)
        {
            Dispose();
        }

        #endregion

        #region Enums

        /// <summary>
        /// Defines data types that can be read/written from/to <see cref="MemoryMappedFile"/>.
        /// </summary>
        private enum DataTypes
        {
            /// <summary>
            /// Number of <see cref="InternalLogger"/> instances.
            /// </summary>
            InstanceCount,
            /// <summary>
            /// ID of the next message to be written.
            /// </summary>
            NextMsgId,
            /// <summary>
            /// Log message.
            /// </summary>
            Msg,
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the <see cref="Stream"/> position to the location specified.
        /// </summary>
        /// <param name="dataType">Type of the data that has to be read/write.</param>
        /// <param name="dataId">ID of the message.</param>
        private void MoveTo(DataTypes dataType, int dataId = 0)
        {
            switch (dataType)
            {
                case DataTypes.InstanceCount:
                    m_MMFStream.Seek(0, SeekOrigin.Begin);
                    break;
                case DataTypes.NextMsgId:
                    m_MMFStream.Seek(2, SeekOrigin.Begin);
                    break;
                case DataTypes.Msg:
                    long offset = c_HeaderSize + (dataId * c_MaxMsgSize);
                    m_MMFStream.Seek(offset, SeekOrigin.Begin);
                    break;
            }
        }

        /// <summary>
        /// Reads a value from the <see cref="MemoryMappedFile"/> header.
        /// </summary>
        /// <param name="dataType">The type of the value that has to be read.</param>
        /// <returns>The read value.</returns>
        private short ReadHeaderValue(DataTypes dataType)
        {
            MoveTo(dataType);
            return m_BReader.ReadInt16();
        }

        /// <summary>
        /// Write a value to the <see cref="MemoryMappedFile"/> header.
        /// </summary>
        /// <param name="dataType">The type of the value that has to be written.</param>
        /// <param name="value">The value to write.</param>
        private void WriteHeaderValue(DataTypes dataType, short value)
        {
            MoveTo(dataType);
            m_BWriter.Write(value);
        }

        /// <summary>
        /// Writes a new message to the internal logger.
        /// </summary>
        /// <param name="msg">The message that has to be written.</param>
        public void WriteMessage(string msg)
        {
            int fixedMsgSize = m_ProcessNameByteCount +
                sizeof(int) + //number of bytes of process name
                sizeof(long); //number of bytes of msg date
            string validatedMsg = msg;
            //check message size before write
            while (m_StringEncoding.GetByteCount(validatedMsg + m_Writer.NewLine) + fixedMsgSize > c_MaxMsgSize)
            {
                validatedMsg = validatedMsg.Remove(validatedMsg.Length - 1);
            }
            m_Mutex.WaitOne();
            short msgId = ReadHeaderValue(DataTypes.NextMsgId);
            MoveTo(DataTypes.Msg, msgId % c_MaxMsgNum);
            m_BWriter.Write(DateTime.Now.ToBinary());
            m_BWriter.Write(m_ProcessNameByteCount);
            m_Writer.Write(m_ProcessName);
            m_Writer.WriteLine(validatedMsg);
            WriteHeaderValue(DataTypes.NextMsgId, ++msgId);
            m_Mutex.ReleaseMutex();
        }

        /// <summary>
        /// Gets the number of instances connected to the <see cref="InternalLogger"/>.
        /// </summary>
        /// <returns>The number of connected <see cref="InternalLogger"/> instances.</returns>
        public short GetInstanceCount()
        {
            m_Mutex.WaitOne();
            short res = ReadHeaderValue(DataTypes.InstanceCount);
            m_Mutex.ReleaseMutex();
            return res;
        }

        /// <summary>
        /// Gets the total number of messages written to the <see cref="InternalLogger"/>.
        /// </summary>
        /// <returns>The total number of written messages.</returns>
        public short GetMessagesCount()
        {
            m_Mutex.WaitOne();
            short res = ReadHeaderValue(DataTypes.NextMsgId);
            m_Mutex.ReleaseMutex();
            return res;
        }

        /// <summary>
        /// Gets all the new messages avaialble from the last <see cref="ReadMessages"/> call.
        /// </summary>
        /// <returns>A collection that contains the new messages.</returns>
        public IEnumerable<InternalLogMessage> ReadMessages()
        {
            List<InternalLogMessage> res = new List<InternalLogMessage>();
            //detect how much messages must be read
            short msgCount = GetMessagesCount();
            int msgToRead = msgCount - m_LastReadID;
            if (msgToRead > c_MaxMsgNum)
            {
                msgToRead = c_MaxMsgNum;
            }
            m_LastReadID = (short)(msgCount - 1);
            //check if there are new messages
            if (msgToRead != 0)
            {
                //get idx of last message
                int msgIdx = m_LastReadID % c_MaxMsgNum;
                //copy the MMF content
                m_Mutex.WaitOne();
                for (int i = 0; i < msgToRead; i++)
                {
                    InternalLogMessage newMsg;
                    MoveTo(DataTypes.Msg, msgIdx);
                    newMsg.Date = DateTime.FromBinary(m_BReader.ReadInt64());
                    newMsg.ProcessName = m_StringEncoding.GetString(m_BReader.ReadBytes(m_BReader.ReadInt32()));
                    newMsg.Message = m_Reader.ReadLine();
                    m_Reader.DiscardBufferedData();
                    res.Add(newMsg);
                    msgIdx--;
                    if (msgIdx < 0)
                    {
                        msgIdx += c_MaxMsgNum;
                    }
                }
                m_Mutex.ReleaseMutex();
            }
            return res;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the message data separator character.
        /// </summary>
        public string DataSeparator
        {
            get { return "§"; }
        }

        #endregion
    }
}
