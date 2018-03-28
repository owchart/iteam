/*****************************************************************************\
*                                                                             *
* BaseService.cs -  Base service functions, types, and definitions            *
*                                                                             *
*               Version 1.00 ★★★★★                                       *
*                                                                             *
*               Copyright (c) 2016-2016, Client. All rights reserved.         *
*               Created by Todd.                                              *
*                                                                             *
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO.Compression;

namespace OwLib
{
    /// <summary>
    /// 基础服务类
    /// </summary>
    public class BaseService:IDisposable
    {
        #region Lord 2016/7/5
        /// <summary>
        /// 创建服务
        /// </summary>
        public BaseService()
        {
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~BaseService()
        {
            lock (m_listeners)
            {
                m_listeners.Clear();
            }
            lock (m_waitMessages)
            {
                m_waitMessages.Clear();
            }
        }

        /// <summary>
        /// 无压缩
        /// </summary>
        public static int COMPRESSTYPE_NONE = 0;

        /// <summary>
        /// GZIP压缩
        /// </summary>
        public static int COMPRESSTYPE_GZIP = 1;

        /// <summary>
        /// 监听者集合
        /// </summary>
        private Dictionary<int, MessageListener> m_listeners = new Dictionary<int, MessageListener>();

        /// <summary>
        /// 消息回调
        /// </summary>
        private static MessageCallBack m_messageCallBack;

        /// <summary>
        /// 请求ID
        /// </summary>
        private static int m_requestID = 10000;

        /// <summary>
        /// 所有的服务
        /// </summary>
        public static List<BaseService> m_services = new List<BaseService>();

        /// <summary>
        /// 等待消息队列
        /// </summary>
        private Dictionary<int, CMessage> m_waitMessages = new Dictionary<int, CMessage>();

        /// <summary>
        /// 写日志回调
        /// </summary>
        private static WriteLogCallBack m_writeLogCallBack;

        private int m_compressType = COMPRESSTYPE_NONE;

        /// <summary>
        /// 获取或设置压缩类型
        /// </summary>
        public int CompressType
        {
            get { return m_compressType; }
            set { m_compressType = value; }
        }

        private static long m_downFlow;

        /// <summary>
        /// 获取或设置下载流量
        /// </summary>
        public static long DownFlow
        {
            get { return BaseService.m_downFlow; }
            set { BaseService.m_downFlow = value; }
        }

        private int m_groupID = 0;

        /// <summary>
        /// 获取或设置组ID
        /// </summary>
        public int GroupID
        {
            get { return m_groupID; }
            set { m_groupID = value; }
        }

        private bool m_isDisposed = false;

        /// <summary>
        /// 获取对象是否已被销毁
        /// </summary>
        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }

        private int m_serviceID = 0;

        /// <summary>
        /// 获取或设置服务的ID
        /// </summary>
        public int ServiceID
        {
            get { return m_serviceID; }
            set { m_serviceID = value; }
        }

        private int m_sessionID = 0;

        /// <summary>
        /// 获取或设置登录ID
        /// </summary>
        public int SessionID
        {
            get { return m_sessionID; }
            set { m_sessionID = value; }
        }

        private int m_socketID;

        /// <summary>
        /// 获取或设置套接字ID
        /// </summary>
        public int SocketID
        {
            get { return m_socketID; }
            set { m_socketID = value; }
        }

        private static long m_upFlow;

        /// <summary>
        /// 获取或设置上传流量
        /// </summary>
        public static long UpFlow
        {
            get { return m_upFlow; }
            set { m_upFlow = value; }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="socketID">连接ID</param>
        /// <returns>状态</returns>
        [DllImport("owsock_client.dll", EntryPoint = "CloseClient", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static int CloseClient(int socketID);

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="proxyType">代理类型</param>
        /// <param name="ip">地址</param>
        /// <param name="port">端口</param>
        /// <param name="proxyIp">代理地址</param>
        /// <param name="proxyPort">代理端口</param>
        /// <param name="proxyUserName">代理名称</param>
        /// <param name="proxyUserPwd">代理密码</param>
        /// <param name="proxyDomain">代理域</param>
        /// <param name="timeout">超时</param>
        /// <returns>状态</returns>
        [DllImport("owsock_client.dll", EntryPoint = "ConnectToServer", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static int ConnectToServer(int type, int proxyType, String ip, int port, String proxyIp, int proxyPort, String proxyUserName, String proxyUserPwd, String proxyDomain, int timeout);

        /// <summary>
        /// 注册回调
        /// </summary>
        /// <param name="callBack">回调函数</param>
        [DllImport("owsock_client.dll", EntryPoint = "RegisterClientCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void RegisterCallBack(MessageCallBack callBack);

        /// <summary>
        /// 注册日志
        /// </summary>
        /// <param name="callBack">回调函数</param>
        [DllImport("owsock_client.dll", EntryPoint = "RegisterClientLog", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void RegisterLog(WriteLogCallBack callBack);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="socketID">连接ID</param>
        /// <param name="str">数据</param>
        /// <param name="len">长度</param>
        /// <returns>状态</returns>
        [DllImport("owsock_client.dll", EntryPoint = "SendByClient", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SendByClient(int socketID, IntPtr str, int len);

        /// <summary>
        /// 添加服务
        /// </summary>
        /// <param name="service">服务</param>
        public static void AddService(BaseService service)
        {
            if (m_messageCallBack == null)
            {
                m_messageCallBack = new MessageCallBack(CallBack);
                RegisterCallBack(m_messageCallBack);
                m_writeLogCallBack = new WriteLogCallBack(WriteServerLog);
                RegisterLog(m_writeLogCallBack);
            }
            m_services.Add(service);
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ip">地址</param>
        /// <param name="port">端口</param>
        public static int Connect(String ip, int port)
        {
            int socketID = ConnectToServer(0, 0, ip, port, "", 0, "", "", "", 6);
            return socketID;
        }

        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="socketID">连接ID</param>
        /// <param name="localSID">本地连接ID</param>
        /// <param name="str">数据</param>
        /// <param name="len">长度</param>
        public static void CallBack(int socketID, int localSID, IntPtr str, int len)
        {
            m_downFlow += len;
            try
            {
                if (len > 4)
                {
                    byte[] bytes = GetValueChar(str, len);
                    Binary br = new Binary();
                    br.Write(bytes, len);
                    int head = br.ReadInt();
                    //int groupID = br.ReadShort();
                    int serviceID = br.ReadShort();
                    foreach (BaseService service in m_services)
                    {
                        if (service.SocketID == localSID)
                        {
                            service.OnCallBack(br, socketID, localSID, len);
                        }
                    }
                    br.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        public virtual void Dispose()
        {
            if (!m_isDisposed)
            {
                m_listeners.Clear();
                m_isDisposed = true;
            }
        }

        /// <summary>
        /// 获取请求ID
        /// </summary>
        /// <returns>请求ID</returns>
        public static int GetRequestID()
        {
            return m_requestID++;
        }

        /// <summary>
        /// 获取所有的服务
        /// </summary>
        /// <param name="services">服务列表</param>
        public static void GetServices(List<BaseService> services)
        {
            foreach (BaseService service in m_services)
            {
                services.Add(service);
            }
        }

        /// <summary>
        /// 根据句柄获取流
        /// </summary>
        /// <param name="pSrc">句柄</param>
        /// <param name="nSrcLen">长度</param>
        /// <returns>流数组</returns>
        static unsafe byte[] GetValueChar(IntPtr pSrc, int nSrcLen)
        {
            byte[] pb = new byte[nSrcLen];
            byte* p = (byte*)pSrc;
            for (int nL = 0; nL < nSrcLen; ++nL)
            {
                pb[nL] = p[nL];
            }
            return pb;
        }

        /// <summary>
        /// 保持活跃
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        /// <returns>状态</returns>
        public virtual int KeepAlive(int socketID)
        {
            Binary bw = new Binary();
            bw.WriteInt((int)4);
            byte[] bytes = bw.GetBytes();
            int length = bytes.Length;
            IntPtr ptr = Marshal.AllocHGlobal(sizeof(byte) * length);
            for (int i = 0; i < length; i++)
            {
                IntPtr iptr = (IntPtr)((int)ptr + i);
                Marshal.WriteByte(iptr, bytes[i]);
            }
            int ret = SendByClient(socketID, ptr, length);
            Marshal.FreeHGlobal(ptr);
            bw.Close();
            return ret;
        }

        /// <summary>
        /// 收到消息
        /// </summary>
        /// <param name="br">流</param>
        /// <param name="socketID">套接字ID</param>
        /// <param name="localSID">本地套接字ID</param>
        /// <param name="len">长度</param>
        public virtual void OnCallBack(Binary br, int socketID, int localSID, int len)
        {
            int headSize = sizeof(int) * 4 + sizeof(short) * 2 + sizeof(byte) * 2;
            int functionID = br.ReadShort();
            int sessionID = br.ReadInt();
            int requestID = br.ReadInt();
            int state = br.ReadByte();
            int compressType = br.ReadByte();
            int bodyLength = br.ReadInt();
            byte[] body = new byte[len - headSize];
            br.ReadBytes(body);
            if (compressType == COMPRESSTYPE_GZIP)
            {
                using (MemoryStream dms = new MemoryStream())
                {
                    using (MemoryStream cms = new MemoryStream(body))
                    {
                        using (GZipStream gzip = new GZipStream(cms, CompressionMode.Decompress))
                        {
                            byte[] buffer = new byte[1024];
                            int size = 0;
                            while ((size = gzip.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                dms.Write(buffer, 0, size);
                            }
                            body = dms.ToArray();
                        }
                    }
                }
            }
            CMessage message = new CMessage(GroupID, ServiceID, functionID, sessionID, requestID, socketID, state, compressType, bodyLength, body);
            OnReceive(message);
            OnWaitMessageHandle(message);
            body = null;
        }

        /// <summary>
        /// 客户端关闭方法
        /// </summary>
        /// <param name="socketID">连接ID</param>
        /// <param name="localSID">本地连接ID</param>
        public virtual void OnClientClose(int socketID, int localSID)
        {
        }

        /// <summary>
        /// 客户端连接方法
        /// </summary>
        /// <param name="socketID">连接ID</param>
        /// <param name="localSID">本地连接ID</param>
        public virtual void OnClientConnect(int socketID, int localSID)
        {
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="message">消息</param>
        public virtual void OnReceive(CMessage message)
        {
        }

        /// <summary>
        /// 等待消息的处理
        /// </summary>
        /// <param name="message">消息</param>
        public virtual void OnWaitMessageHandle(CMessage message)
        {
            if (m_waitMessages.Count > 0)
            {
                lock (m_waitMessages)
                {
                    if (m_waitMessages.ContainsKey(message.m_requestID))
                    {
                        CMessage waitMessage = m_waitMessages[message.m_requestID];
                        waitMessage.Copy(message);
                    }
                }
            }
        }

        /// <summary>
        /// 注册数据监听
        /// </summary>
        /// <param name="requestID">请求ID</param>
        /// <param name="callBack">回调函数</param>
        public virtual void RegisterListener(int requestID, ListenerMessageCallBack callBack)
        {
            lock (m_listeners)
            {
                MessageListener listener = null;
                if (!m_listeners.ContainsKey(requestID))
                {
                    listener = new MessageListener();
                    m_listeners[requestID] = listener;
                }
                else
                {
                    listener = m_listeners[requestID];
                }
                listener.Add(callBack);
            }
        }

        /// <summary>
        /// 注册等待
        /// </summary>
        /// <param name="requestID">请求ID</param>
        /// <param name="message">消息</param>
        public virtual void RegisterWait(int requestID, CMessage message)
        {
            lock (m_waitMessages)
            {
                m_waitMessages[requestID] = message;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        public virtual int Send(CMessage message)
        {
            Binary bw = new Binary();
            byte[] body = message.m_body;
            int bodyLength = message.m_bodyLength;
            int uncBodyLength = bodyLength;
            if (message.m_compressType == COMPRESSTYPE_GZIP)
            {
                using (MemoryStream cms = new MemoryStream())
                {
                    using (GZipStream gzip = new GZipStream(cms, CompressionMode.Compress))
                    {
                        gzip.Write(body, 0, body.Length);
                    }
                    body = cms.ToArray();
                    bodyLength = body.Length;
                }
            }
            int len = sizeof(int) * 4 + bodyLength + sizeof(short) * 2 + sizeof(byte) * 2;
            bw.WriteInt(len);
            //bw.WriteShort((short)message.m_groupID);
            bw.WriteShort((short)message.m_serviceID);
            bw.WriteShort((short)message.m_functionID);
            bw.WriteInt(message.m_sessionID);
            bw.WriteInt(message.m_requestID);
            bw.WriteByte((byte)message.m_state);
            bw.WriteByte((byte)message.m_compressType);
            bw.WriteInt(uncBodyLength);
            bw.WriteBytes(body);
            byte[] bytes = bw.GetBytes();
            int length = bytes.Length;
            IntPtr ptr = Marshal.AllocHGlobal(sizeof(byte) * length);
            for (int i = 0; i < length; i++)
            {
                IntPtr iptr = (IntPtr)((int)ptr + i);
                Marshal.WriteByte(iptr, bytes[i]);
            }
            int ret = SendByClient(message.m_socketID, ptr, length);
            m_upFlow += ret;
            Marshal.FreeHGlobal(ptr);
            bw.Close();
            return ret;
        }

        /// <summary>
        /// 发送到监听者
        /// </summary>
        /// <param name="message">消息</param>
        public virtual void SendToListener(CMessage message)
        {
            MessageListener listener = null;
            lock (m_listeners)
            {
                if (m_listeners.ContainsKey(message.m_requestID))
                {
                    listener = m_listeners[message.m_requestID];
                }
            }
            if (listener != null)
            {
                listener.CallBack(message);
            }
        }

        /// <summary>
        /// 取消注册数据监听
        /// </summary>
        /// <param name="requestID">请求ID</param>
        public virtual void UnRegisterListener(int requestID)
        {
            lock (m_listeners)
            {
                m_listeners.Remove(requestID);
            }
        }

        /// <summary>
        /// 取消注册监听
        /// </summary>
        /// <param name="requestID">请求ID</param>
        /// <param name="callBack">回调函数</param>
        public virtual void UnRegisterListener(int requestID, ListenerMessageCallBack callBack)
        {
            lock (m_listeners)
            {
                if (m_listeners.ContainsKey(requestID))
                {
                    m_listeners[requestID].Remove(callBack);
                }
            }
        }

        /// <summary>
        /// 取消注册等待
        /// </summary>
        /// <param name="requestID">请求ID</param>
        public virtual void UnRegisterWait(int requestID)
        {
            lock (m_waitMessages)
            {
                if (m_waitMessages.ContainsKey(requestID))
                {
                    m_waitMessages.Remove(requestID);
                }
            }
        }

        /// <summary>
        /// 等待消息
        /// </summary>
        /// <param name="requestID">请求ID</param>
        /// <param name="timeout">超时</param>
        /// <returns>状态</returns>
        public virtual int WaitMessage(int requestID, int timeout)
        {
            int state = 0;
            while (timeout > 0)
            {
                lock (m_waitMessages)
                {
                    if (m_waitMessages.ContainsKey(requestID))
                    {
                        if (m_waitMessages[requestID].m_bodyLength > 0)
                        {
                            state = 1;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                timeout -= 10;
                Thread.Sleep(10);
            }
            UnRegisterWait(requestID);
            return state;
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="socketID">连接ID</param>
        /// <param name="localSID">本地连接ID</param>
        /// <param name="state">状态</param>
        /// <param name="log">日志</param>
        public static void WriteServerLog(int socketID, int localSID, int state, String log)
        {
            if (state == 1)
            {
                foreach (BaseService service in m_services)
                {
                    if (service.SocketID == socketID)
                    {
                        service.OnClientConnect(socketID, localSID);
                    }
                }
            }
            else if (state == 2 || state == 3)
            {
                foreach (BaseService service in m_services)
                {
                    if (service.SocketID == socketID)
                    {
                        service.OnClientClose(socketID, localSID);
                    }
                }
            }
        }
        #endregion
    }
}