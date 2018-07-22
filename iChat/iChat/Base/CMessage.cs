using System;
using System.Collections.Generic;
using System.Text;

namespace OwLib
{
    /// <summary>
    /// 消息回调委托
    /// </summary>
    /// <param name="socketID">连接ID</param>
    /// <param name="localSID">本地连接ID</param>
    /// <param name="str">数据</param>
    /// <param name="len">长度</param>
    public delegate void MessageCallBack(int socketID, int localSID, IntPtr str, int len);

    /// <summary>
    /// 监听消息
    /// </summary>
    /// <param name="message">消息</param>
    public delegate void ListenerMessageCallBack(CMessage message);

    /// <summary>
    /// 日志报告委托
    /// </summary>
    /// <param name="socketID">连接ID</param>
    /// <param name="localSID">本地连接ID</param>
    /// <param name="state">状态</param>
    /// <param name="log">日志</param>
    public delegate void WriteLogCallBack(int socketID, int localSID, int state, String log);

    /// <summary>
    /// 消息结构
    /// </summary>
    public class CMessage
    {
        /// <summary>
        /// 创建消息
        /// </summary>
        public CMessage()
        {
        }

        /// <summary>
        /// 创建消息
        /// </summary>
        /// <param name="groupID">组ID</param>
        /// <param name="serviceID">服务ID</param>
        /// <param name="functionID">功能ID</param>
        /// <param name="sessionID">登录ID</param>
        /// <param name="requestID">请求ID</param>
        /// <param name="socketID">连接ID</param>
        /// <param name="state">状态</param>
        /// <param name="compressType">压缩类型</param>
        /// <param name="bodyLength">包体长度</param>
        /// <param name="body">包体</param>
        public CMessage(int groupID, int serviceID, int functionID, int sessionID, int requestID, int socketID, int state, int compressType, int bodyLength, byte[] body)
        {
            m_groupID = groupID;
            m_serviceID = serviceID;
            m_functionID = functionID;
            m_sessionID = sessionID;
            m_requestID = requestID;
            m_socketID = socketID;
            m_state = state;
            m_compressType = compressType;
            m_bodyLength = bodyLength;
            m_body = body;
        }

        /// <summary>
        /// 组ID
        /// </summary>
        public int m_groupID;

        /// <summary>
        /// 服务ID
        /// </summary>
        public int m_serviceID;

        /// <summary>
        /// 功能ID
        /// </summary>
        public int m_functionID;

        /// <summary>
        /// 登录ID
        /// </summary>
        public int m_sessionID;

        /// <summary>
        /// 请求ID
        /// </summary>
        public int m_requestID;

        /// <summary>
        /// 连接ID
        /// </summary>
        public int m_socketID;

        /// <summary>
        /// 状态
        /// </summary>
        public int m_state;

        /// <summary>
        /// 压缩类型
        /// </summary>
        public int m_compressType;

        /// <summary>
        /// 包体长度
        /// </summary>
        public int m_bodyLength;

        /// <summary>
        /// 包体
        /// </summary>
        public byte[] m_body;

        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="message">消息</param>
        public void Copy(CMessage message)
        {
            m_groupID = message.m_groupID;
            m_serviceID = message.m_serviceID;
            m_functionID = message.m_functionID;
            m_sessionID = message.m_sessionID;
            m_requestID = message.m_requestID;
            m_socketID = message.m_socketID;
            m_state = message.m_state;
            m_compressType = message.m_compressType;
            m_bodyLength = message.m_bodyLength;
            m_body = message.m_body;
        }
    }

    /// <summary>
    /// 消息监听
    /// </summary>
    public class MessageListener
    {
        /// <summary>
        /// 创建消息监听
        /// </summary>
        public MessageListener()
        {
        }

        /// <summary>
        /// 析构方法
        /// </summary>
        ~MessageListener()
        {
            Clear();
        }

        /// <summary>
        /// 监听回调列表
        /// </summary>
        private List<ListenerMessageCallBack> m_callBacks = new List<ListenerMessageCallBack>();

        /// <summary>
        /// 添加回调
        /// </summary>
        /// <param name="callBack">回调</param>
        public void Add(ListenerMessageCallBack callBack)
        {
            m_callBacks.Add(callBack);
        }

        /// <summary>
        /// 回调方法
        /// </summary>
        public void CallBack(CMessage message)
        {
            int callBackSize = m_callBacks.Count;
            for (int i = 0; i < callBackSize; i++)
            {
                m_callBacks[i](message);
            }
        }

        /// <summary>
        /// 清除监听
        /// </summary>
        public void Clear()
        {
            m_callBacks.Clear();
        }

        /// <summary>
        /// 移除回调
        /// </summary>
        /// <param name="callBack">回调</param>
        public void Remove(ListenerMessageCallBack callBack)
        {
            m_callBacks.Remove(callBack);
        }
    }

    /// <summary>
    /// 套接字连接组信息
    /// </summary>
    public class SocketArray
    {
        /// <summary>
        /// 套接字ID组
        /// </summary>
        private List<int> m_sockets = new List<int>();

        /// <summary>
        /// 添加套接字ID
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        public void AddSocket(int socketID)
        {
            int socketsSize = m_sockets.Count;
            for (int i = 0; i < socketsSize; i++)
            {
                if (m_sockets[i] == socketID)
                {
                    return;
                }
            }
            m_sockets.Add(socketID);
        }

        /// <summary>
        /// 获取套接字列表
        /// </summary>
        /// <param name="socketList">套接字列表</param>
        public void GetSocketList(List<int> socketList)
        {
            int socketsSize = m_sockets.Count;
            for (int i = 0; i < socketsSize; i++)
            {
                socketList.Add(m_sockets[i]);
            }
        }

        /// <summary>
        /// 移除套接字ID
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        public void RemoveSocket(int socketID)
        {
            m_sockets.Remove(socketID);
        }
    }

}
