/*****************************************************************************\
*                                                                             *
* ChatService.cs -  Chat service functions, types, and definitions.          *
*                                                                             *
*               Version 1.00 ★                                               *
*                                                                             *
*               Copyright (c) 2016-2016, Server. All rights reserved.         *
*               Created by QiChunyou.                                         *
*                                                                             *
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SQLite;
using OwLib;

namespace OwLibSV
{
    /// <summary>
    /// 区块链数据
    /// </summary>
    public class ChatData
    {
        #region 齐春友 2016/6/9
        public String m_aes = "";

        /// <summary>
        /// 包体长度
        /// </summary>
        public int m_bodyLength;

        /// <summary>
        /// 包体
        /// </summary>
        public byte[] m_body;

        /// <summary>
        /// 内容
        /// </summary>
        public String m_content = "";

        /// <summary>
        /// 发送者
        /// </summary>
        public String m_from = "";

        /// <summary>
        /// 接收用户
        /// </summary>
        public String m_to = "";

        /// <summary>
        /// 标识
        /// </summary>
        public String m_tokens = "";
        #endregion
    }

    /// <summary>
    /// 主机信息
    /// </summary>
    public class ChatHostInfo
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public String m_ip;

        /// <summary>
        /// 服务端端口
        /// </summary>
        public int m_serverPort;

        /// <summary>
        /// 上线或下线
        /// </summary>
        public int m_type;

        /// <summary>
        /// 用户ID
        /// </summary>
        public String m_userID = "";

        /// <summary>
        /// 用户名
        /// </summary>
        public String m_userName = "";

        /// <summary>
        /// 转换为String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_ip + ":" + CStr.ConvertIntToStr(m_serverPort);
        }
    }

    /// <summary>
    /// 区块链服务
    /// </summary>
    public class ChatService:BaseService
    {
        #region 齐春友 2016/06/03
        /// <summary>
        /// 创建区块链服务
        /// </summary>
        public ChatService()
        {
            ServiceID = SERVICEID_CHAT;
        }

        /// <summary>
        /// 弹幕服务ID
        /// </summary>
        private const int SERVICEID_CHAT = 10000;

        /// <summary>
        /// 主机信息
        /// </summary>
        public const int FUNCTIONID_GETHOSTS = 1;

        /// <summary>
        /// 广播区块链功能ID
        /// </summary>
        public const int FUNCTIONID_SENDALL = 2;

        /// <summary>
        /// 发送消息
        /// </summary>
        public const int FUNCTIONID_SEND = 4;

        /// <summary>
        /// 登入
        /// </summary>
        public const int FUNCTIONID_ENTER = 6;

        /// <summary>
        /// 服务端主机
        /// </summary>
        private List<ChatHostInfo> m_serverHosts = new List<ChatHostInfo>();

        /// <summary>
        /// 会话列表
        /// </summary>
        public Dictionary<int, ChatHostInfo> m_socketIDs = new Dictionary<int, ChatHostInfo>();

        private int m_port = 16666;

        /// <summary>
        /// 获取或设置端口
        /// </summary>
        public int Port
        {
            get { return m_port; }
            set { m_port = value; }
        }

        private String m_token = System.Guid.NewGuid().ToString();

        /// <summary>
        /// 获取客户端唯一标识
        /// </summary>
        public String Token
        {
            get { return m_token; }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="hostInfo">主机信息</param>
        public void AddServerHosts(ChatHostInfo hostInfo)
        {
            lock (m_serverHosts)
            {
                int serverHostsSize = m_serverHosts.Count;
                bool contains = false;
                for (int i = 0; i < serverHostsSize; i++)
                {
                    ChatHostInfo oldHostInfo = m_serverHosts[i];
                    if (oldHostInfo.m_ip == hostInfo.m_ip && oldHostInfo.m_serverPort == hostInfo.m_serverPort)
                    {
                        contains = true;
                        break;
                    }
                }
                if (!contains)
                {
                    m_serverHosts.Add(hostInfo);
                }
            }
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>登入</returns>
        public int Enter(CMessage message)
        {
            int rtnSocketID = message.m_socketID;
            Binary br = new Binary();
            br.Write(message.m_body, message.m_bodyLength);
            String ip = "";
            int port = br.ReadInt();
            int type = br.ReadInt();
            String userID = br.ReadString();
            String userName = br.ReadString();
            br.Close();
            List<int> sendSocketIDs = new List<int>();
            List<ChatHostInfo> hostInfos = new List<ChatHostInfo>();
            lock (m_socketIDs)
            {
                if (m_socketIDs.ContainsKey(rtnSocketID))
                {
                    ip = m_socketIDs[rtnSocketID].m_ip;
                    m_socketIDs[rtnSocketID].m_serverPort = port;
                    m_socketIDs[rtnSocketID].m_type = type;
                    m_socketIDs[rtnSocketID].m_userID = userID;
                    m_socketIDs[rtnSocketID].m_userName = userName;
                    hostInfos.Add(m_socketIDs[message.m_socketID]);
                    foreach (int socketID in m_socketIDs.Keys)
                    {
                        if (socketID != rtnSocketID)
                        {
                            ChatHostInfo gs = m_socketIDs[socketID];
                            if (gs.m_type == 0)
                            {
                                sendSocketIDs.Add(socketID);
                            }
                            else if (gs.m_type == 1)
                            {
                                if (type == 1)
                                {
                                    sendSocketIDs.Add(socketID);
                                }
                            }
                        }
                    }
                }
            }
            int sendSocketIDsSize = sendSocketIDs.Count;
            if (sendSocketIDsSize > 0)
            {
                SendHostInfos(sendSocketIDs, 1, hostInfos);
            }
            Dictionary<String, ChatHostInfo> allHostInfos = new Dictionary<string, ChatHostInfo>();
            lock(m_socketIDs)
            {
                foreach (int sid in m_socketIDs.Keys)
                {
                    if (sid != rtnSocketID)
                    {
                        allHostInfos[m_socketIDs[sid].ToString()] = m_socketIDs[sid];
                    }
                }
            }
            //发送本地IP地址
            if (DataCenter.HostInfo.m_localHost.Length > 0)
            {
                ChatHostInfo localHostInfo = new ChatHostInfo();
                localHostInfo.m_ip = DataCenter.HostInfo.m_localHost;
                localHostInfo.m_serverPort = DataCenter.HostInfo.m_localPort;
                localHostInfo.m_type = 1;
                allHostInfos[localHostInfo.ToString()] = localHostInfo;
            }
            lock (m_serverHosts)
            {
                foreach (ChatHostInfo serverHost in m_serverHosts)
                {
                    allHostInfos[serverHost.ToString()] = serverHost;
                }
            }
            List<int> rtnSocketIDs = new List<int>();
            rtnSocketIDs.Add(rtnSocketID);
            List<ChatHostInfo> sendAllHosts = new List<ChatHostInfo>();
            foreach (ChatHostInfo sendHost in allHostInfos.Values)
            {
                sendAllHosts.Add(sendHost);
            }
            SendHostInfos(rtnSocketIDs, 0, sendAllHosts);
            sendAllHosts.Clear();
            rtnSocketIDs.Clear();
            hostInfos.Clear();
            sendSocketIDs.Clear();
            if (DataCenter.IsFull && type == 1)
            {
                if (DataCenter.ClientChatServices.Count == 0)
                {
                    int socketID = OwLib.BaseService.Connect(ip, port);
                    if (socketID != -1)
                    {
                        String key = ip + ":" + CStr.ConvertIntToStr(port);
                        OwLib.ChatService clientChatService = new OwLib.ChatService();
                        DataCenter.ClientChatServices[key] = clientChatService;
                        OwLib.BaseService.AddService(clientChatService);
                        clientChatService.ToServer = true;
                        clientChatService.Connected = true;
                        clientChatService.SocketID = socketID;
                        clientChatService.Enter();
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 获取弹幕信息
        /// </summary>
        /// <param name="loginInfos">弹幕信息</param>
        /// <param name="body">包体</param>
        /// <param name="bodyLength">包体长度</param>
        public int GetChatData(ChatData chatData, byte[] body, int bodyLength)
        {
            Binary br = new Binary();
            br.Write(body, bodyLength);
            chatData.m_aes = br.ReadString();
            chatData.m_tokens = br.ReadString();
            chatData.m_from = br.ReadString();
            chatData.m_to = br.ReadString();
            chatData.m_content = br.ReadString();
            chatData.m_bodyLength = br.ReadInt();
            if (chatData.m_bodyLength > 0)
            {
                byte[] bytes = new byte[chatData.m_bodyLength];
                br.ReadBytes(bytes);
            }
            br.Close();
            return 1;
        }

        /// <summary>
        /// 客户端关闭方法
        /// </summary>
        /// <param name="socketID">连接ID</param>
        /// <param name="localSID">本地连接ID</param>
        public override void OnClientClose(int socketID, int localSID)
        {
            base.OnClientClose(socketID, localSID);
            List<ChatHostInfo> removeHostInfos = new List<ChatHostInfo>();
            List<int> sendSocketIDs = new List<int>();
            lock (m_socketIDs)
            {
                List<int> removeSocketIDs = new List<int>();
                foreach(int sid in m_socketIDs.Keys)
                {
                    if (sid == socketID)
                    {
                        removeHostInfos.Add(m_socketIDs[sid]);
                        removeSocketIDs.Add(sid);
                    }
                }
                int removeSocketIDsSize = removeSocketIDs.Count;
                for (int i = 0; i < removeSocketIDsSize; i++)
                {
                    m_socketIDs.Remove(removeSocketIDs[i]);
                }
                foreach (int sid in m_socketIDs.Keys)
                {
                    sendSocketIDs.Add(sid);
                }
                removeSocketIDs.Clear();
            }
            int sendSocketIDsSize = sendSocketIDs.Count;
            if (sendSocketIDsSize > 0)
            {
                SendHostInfos(sendSocketIDs, 2, removeHostInfos);
            }
            sendSocketIDs.Clear();
            removeHostInfos.Clear();
        }

        /// <summary>
        /// 客户端连接方法
        /// </summary>
        /// <param name="socketID">连接ID</param>
        /// <param name="localSID">本地连接ID</param>
        /// <param name="ip">IP地址</param>
        public override void OnClientConnect(int socketID, int localSID, string ip)
        {
            base.OnClientConnect(socketID, localSID, ip);
            lock (m_socketIDs)
            {
                if (!m_socketIDs.ContainsKey(socketID))
                {
                    m_socketIDs[socketID] = new ChatHostInfo();
                    String strIPPort = ip.Replace("accept:", "");
                    String[] strs = strIPPort.Split(new String[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    m_socketIDs[socketID].m_ip = strs[0];
                }
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="message">消息</param>
        public override void OnReceive(CMessage message)
        {
            base.OnReceive(message);
            switch (message.m_functionID)
            {
                case FUNCTIONID_SEND:
                    SendMsg(message);
                    break;
                case FUNCTIONID_SENDALL:
                    SendAll(message);
                    break;
                case FUNCTIONID_ENTER:
                    Enter(message);
                    break;
                default:
                    break;           
            }
        }

        /// <summary>
        /// socket发送
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="loginInfos">登录信息列表</param>
        /// <returns>状态</returns>
        public int Send(CMessage message, ChatData chatData)
        {
            String tokens = chatData.m_tokens;
            Binary bw = new Binary();
            if (DataCenter.IsFull)
            {
                String key = Token;
                if (tokens.IndexOf(key) != -1)
                {
                    return 1;
                }
                else
                {
                    tokens += key;
                }
            }
            bw.WriteString(chatData.m_aes);
            bw.WriteString(tokens);
            bw.WriteString(chatData.m_from);
            bw.WriteString(chatData.m_to);
            bw.WriteString(chatData.m_content);
            bw.WriteInt(chatData.m_bodyLength);
            if (chatData.m_bodyLength > 0)
            {
                bw.WriteBytes(chatData.m_body);
            }
            byte[] bytes = bw.GetBytes();
            message.m_body = bytes;
            message.m_bodyLength = bytes.Length;
            int ret = Send(message);
            bw.Close();
            return ret;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>状态</returns>
        public int SendAll(CMessage message)
        {
            int rtnSocketID = message.m_socketID;
            ChatData chatData = new ChatData();
            GetChatData(chatData, message.m_body, message.m_bodyLength);
            lock (m_socketIDs)
            {
                foreach (int socketID in m_socketIDs.Keys)
                {
                    if (rtnSocketID != socketID)
                    {
                        message.m_socketID = socketID;
                        if (m_socketIDs[socketID].m_type == 0 && chatData.m_to.Length > 0)
                        {
                            if (chatData.m_to.IndexOf(m_socketIDs[socketID].m_userID) == -1)
                            {
                                continue;
                            }
                        }
                        int ret = Send(message, chatData);
                    }
                }
            }
            return 1;
        }

        /// <summary>
        /// 发送主机信息
        /// </summary>
        /// <returns></returns>
        public int SendHostInfos(List<int> socketIDs, int type, List<ChatHostInfo> hostInfos)
        {
            int hostInfosSize = hostInfos.Count;
            Binary bw = new Binary();
            bw.WriteInt(hostInfosSize);
            bw.WriteInt(type);
            for (int i = 0; i < hostInfosSize; i++)
            {
                ChatHostInfo hostInfo = hostInfos[i];
                bw.WriteString(hostInfo.m_ip);
                bw.WriteInt(hostInfo.m_serverPort);
                bw.WriteInt(hostInfo.m_type);
                bw.WriteString(hostInfo.m_userID);
                bw.WriteString(hostInfo.m_userName);
            }
            byte[] bytes = bw.GetBytes();
            CMessage message = new CMessage(GroupID, ServiceID, FUNCTIONID_GETHOSTS, SessionID, DataCenter.ChatRequestID, 0, 0, CompressType, bytes.Length, bytes);
            foreach (int socketID in socketIDs)
            {
                message.m_socketID = socketID;
                Send(message);
            }
            bw.Close();
            return 1;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>状态</returns>
        public int SendMsg(CMessage message)
        {
            SendToListener(message);
            return 1;
        }
        #endregion
    }
}
