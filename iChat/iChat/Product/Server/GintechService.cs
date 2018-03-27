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
        /// 锁
        /// </summary>
        public object m_lock = new object();

        /// <summary>
        /// 会话列表
        /// </summary>
        public Dictionary<int, GintechHostInfo> m_socketIDs = new Dictionary<int, GintechHostInfo>();

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
        /// 登入
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>登入</returns>
        public int Enter(CMessage message)
        {
            int rtnSocketID = message.m_socketID;
            Binary br = new Binary();
            br.Write(message.m_body, message.m_bodyLength);
            int port = br.ReadInt();
            int type = br.ReadInt();
            List<int> sendSocketIDs = new List<int>();
            List<GintechHostInfo> hostInfos = new List<GintechHostInfo>();
            lock (m_socketIDs)
            {
                if (m_socketIDs.ContainsKey(message.m_socketID))
                {
                    m_socketIDs[message.m_socketID].m_serverPort = port;
                    m_socketIDs[message.m_socketID].m_type = type;
                    hostInfos.Add(m_socketIDs[message.m_socketID]);
                }
                foreach (int socketID in m_socketIDs.Keys)
                {
                    if (socketID != rtnSocketID)
                    {
                        sendSocketIDs.Add(socketID);
                    }
                }
            }
            int sendSocketIDsSize = sendSocketIDs.Count;
            if (sendSocketIDsSize > 0)
            {
                SendHostInfos(sendSocketIDs, 1, hostInfos);
            }
            List<GintechHostInfo> allHostInfos = new List<GintechHostInfo>();
            lock(m_socketIDs)
            {
                foreach (GintechHostInfo hostInfo in m_socketIDs.Values)
                {
                    allHostInfos.Add(hostInfo);
                }
            }
            List<int> rtnSocketIDs = new List<int>();
            rtnSocketIDs.Add(rtnSocketID);
            SendHostInfos(rtnSocketIDs, 0, allHostInfos);
            rtnSocketIDs.Clear();
            hostInfos.Clear();
            sendSocketIDs.Clear();
            return 0;
        }

        /// <summary>
        /// 获取弹幕信息
        /// </summary>
        /// <param name="loginInfos">弹幕信息</param>
        /// <param name="body">包体</param>
        /// <param name="bodyLength">包体长度</param>
        public int GetGintechDatas(List<GintechData> datas, byte[] body, int bodyLength)
        {
            Binary br = new Binary();
            br.Write(body, bodyLength);
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
        public int Send(CMessage message, List<GintechData> datas)
        {
            Binary bw = new Binary();
            int gintechSize = datas.Count;
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
            GetGintechDatas(datas, message.m_body, message.m_bodyLength);
            lock (m_socketIDs)
            {
                foreach (int socketID in m_socketIDs.Keys)
                {
                    if (rtnSocketID != socketID)
                    {
                        message.m_socketID = socketID;
                        int ret = Send(message, datas);
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
