/*****************************************************************************\
*                                                                             *
* GintechService.cs -  Gintech service functions, types, and definitions.          *
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
    public class GintechData
    {
        #region 齐春友 2016/6/9
        /// <summary>
        /// 内容
        /// </summary>
        public String m_text = "";
        #endregion
    }

    /// <summary>
    /// 主机信息
    /// </summary>
    public class GintechHostInfo
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
    public class GintechService:BaseService
    {
        #region 齐春友 2016/06/03
        /// <summary>
        /// 创建区块链服务
        /// </summary>
        public GintechService()
        {
            ServiceID = SERVICEID_GINTECH;
        }

        /// <summary>
        /// 弹幕服务ID
        /// </summary>
        private const int SERVICEID_GINTECH = 10000;

        /// <summary>
        /// 主机信息
        /// </summary>
        public const int FUNCTIONID_GETHOSTS = 1;

        /// <summary>
        /// 广播区块链功能ID
        /// </summary>
        public const int FUNCTIONID_GINTECH_SENDALL = 2;

        /// <summary>
        /// 发送消息
        /// </summary>
        public const int FUNCTIONID_GINTECH_SEND = 4;

        /// <summary>
        /// 登入
        /// </summary>
        public const int FUNCTIONID_GINTECH_ENTER = 6;

        /// <summary>
        /// 锁
        /// </summary>
        public object m_lock = new object();

        /// <summary>
        /// 服务端主机
        /// </summary>
        private List<GintechHostInfo> m_serverHosts = new List<GintechHostInfo>();

        /// <summary>
        /// 会话列表
        /// </summary>
        public Dictionary<int, GintechHostInfo> m_socketIDs = new Dictionary<int, GintechHostInfo>();

        private int m_port = 16666;

        /// <summary>
        /// 获取或设置端口
        /// </summary>
        public int Port
        {
            get { return m_port; }
            set { m_port = value; }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="hostInfo">主机信息</param>
        public void AddServerHosts(GintechHostInfo hostInfo)
        {
            lock (m_serverHosts)
            {
                int serverHostsSize = m_serverHosts.Count;
                bool contains = false;
                for (int i = 0; i < serverHostsSize; i++)
                {
                    GintechHostInfo oldHostInfo = m_serverHosts[i];
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
            List<int> sendSocketIDs = new List<int>();
            List<GintechHostInfo> hostInfos = new List<GintechHostInfo>();
            lock (m_socketIDs)
            {
                if (m_socketIDs.ContainsKey(rtnSocketID))
                {
                    ip = m_socketIDs[rtnSocketID].m_ip;
                    m_socketIDs[rtnSocketID].m_serverPort = port;
                    m_socketIDs[rtnSocketID].m_type = type;
                    hostInfos.Add(m_socketIDs[message.m_socketID]);
                    foreach (int socketID in m_socketIDs.Keys)
                    {
                        if (socketID != rtnSocketID)
                        {
                            GintechHostInfo gs = m_socketIDs[socketID];
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
            Dictionary<String, GintechHostInfo> allHostInfos = new Dictionary<string, GintechHostInfo>();
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
            if (DataCenter.Config.m_localHost.Length > 0)
            {
                GintechHostInfo localHostInfo = new GintechHostInfo();
                localHostInfo.m_ip = DataCenter.Config.m_localHost;
                localHostInfo.m_serverPort = DataCenter.Config.m_localPort;
                localHostInfo.m_type = 1;
                allHostInfos[localHostInfo.ToString()] = localHostInfo;
            }
            lock (m_serverHosts)
            {
                foreach (GintechHostInfo serverHost in m_serverHosts)
                {
                    allHostInfos[serverHost.ToString()] = serverHost;
                }
            }
            List<int> rtnSocketIDs = new List<int>();
            rtnSocketIDs.Add(rtnSocketID);
            List<GintechHostInfo> sendAllHosts = new List<GintechHostInfo>();
            foreach (GintechHostInfo sendHost in allHostInfos.Values)
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
                if (DataCenter.ClientGintechServices.Count == 0)
                {
                    int socketID = OwLib.BaseService.Connect(ip, port);
                    if (socketID != -1)
                    {
                        String key = ip + ":" + CStr.ConvertIntToStr(port);
                        OwLib.GintechService clientGintechService = new OwLib.GintechService();
                        DataCenter.ClientGintechServices[key] = clientGintechService;
                        OwLib.BaseService.AddService(clientGintechService);
                        clientGintechService.ToServer = true;
                        clientGintechService.Connected = true;
                        clientGintechService.SocketID = socketID;
                        clientGintechService.Enter();
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
        public int GetGintechDatas(List<GintechData> datas, List<String> ips, byte[] body, int bodyLength)
        {
            Binary br = new Binary();
            br.Write(body, bodyLength);
            int ipsSize = br.ReadInt();
            for (int j = 0; j < ipsSize; j++)
            {
                ips.Add(br.ReadString());
            }
            int gintechSize = br.ReadInt();
            for (int i = 0; i < gintechSize; i++)
            {
                GintechData data = new GintechData();
                data.m_text = br.ReadString();
                datas.Add(data);
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
            List<GintechHostInfo> removeHostInfos = new List<GintechHostInfo>();
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
                    m_socketIDs[socketID] = new GintechHostInfo();
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
                case FUNCTIONID_GINTECH_SEND:
                    SendMsg(message);
                    break;
                case FUNCTIONID_GINTECH_SENDALL:
                    SendAll(message);
                    break;
                case FUNCTIONID_GINTECH_ENTER:
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
        public int Send(CMessage message, List<String> ips, List<GintechData> datas)
        {
            Binary bw = new Binary();
            int gintechSize = datas.Count;
            if (DataCenter.IsFull)
            {
                String key = DataCenter.Config.m_localHost + ":" + CStr.ConvertIntToStr(DataCenter.Config.m_localPort);
                if (ips.Contains(key))
                {
                    return 1;
                }
                else
                {
                    ips.Add(key);
                }
            }
            int ipsSize = ips.Count;
            bw.WriteInt(ipsSize);
            for (int j = 0; j < ipsSize; j++)
            {
                bw.WriteString(ips[j]);
            }
            bw.WriteInt(gintechSize);
            for (int i = 0; i < gintechSize; i++)
            {
                GintechData data = datas[i];

                bw.WriteString(data.m_text);
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
            List<GintechData> datas = new List<GintechData>();
            List<String> ips = new List<String>();
            GetGintechDatas(datas, ips, message.m_body, message.m_bodyLength);
            lock (m_socketIDs)
            {
                foreach (int socketID in m_socketIDs.Keys)
                {
                    if (rtnSocketID != socketID)
                    {
                        List<String> copyIPs = new List<String>();
                        copyIPs.AddRange(ips.ToArray());
                        message.m_socketID = socketID;
                        int ret = Send(message, copyIPs, datas);
                    }
                }
            }
            datas.Clear();
            return 1;
        }

        /// <summary>
        /// 发送主机信息
        /// </summary>
        /// <returns></returns>
        public int SendHostInfos(List<int> socketIDs, int type, List<GintechHostInfo> hostInfos)
        {
            int hostInfosSize = hostInfos.Count;
            Binary bw = new Binary();
            bw.WriteInt(hostInfosSize);
            bw.WriteInt(type);
            for (int i = 0; i < hostInfosSize; i++)
            {
                GintechHostInfo hostInfo = hostInfos[i];
                bw.WriteString(hostInfo.m_ip);
                bw.WriteInt(hostInfo.m_serverPort);
                bw.WriteInt(hostInfo.m_type);
            }
            byte[] bytes = bw.GetBytes();
            CMessage message = new CMessage(GroupID, ServiceID, FUNCTIONID_GETHOSTS, SessionID, DataCenter.GintechRequestID, 0, 0, CompressType, bytes.Length, bytes);
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
