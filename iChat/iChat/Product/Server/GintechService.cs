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
    /// 聊天数据
    /// </summary>
    public class GintechData
    {
        #region 齐春友 2016/6/9
        /// <summary>
        /// 内容
        /// </summary>
        public String m_text = "";

        /// <summary>
        /// 类型
        /// </summary>
        public int m_type;
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
    }

    /// <summary>
    /// 聊天服务
    /// </summary>
    public class GintechService:BaseService
    {
        #region 齐春友 2016/06/03
        /// <summary>
        /// 创建聊天服务
        /// </summary>
        public GintechService()
        {
            ServiceID = SERVICEID_GINTECH;
        }

        /// <summary>
        /// 锁
        /// </summary>
        private object m_lock = new object();

        /// <summary>
        /// 会话列表
        /// </summary>
        private Dictionary<int, String> m_socketIDs = new Dictionary<int, String>();

        /// <summary>
        /// 弹幕服务ID
        /// </summary>
        private const int SERVICEID_GINTECH = 7;

        /// <summary>
        /// 主机信息
        /// </summary>
        public const int FUNCTIONID_GETHOSTS = 1;

        /// <summary>
        /// 广播聊天功能ID
        /// </summary>
        public const int FUNCTIONID_GINTECH_SENDALL = 2;

        /// <summary>
        /// 接收聊天功能ID
        /// </summary>
        public const int FUNCTIONID_GINTECH_RECV = 3;

        /// <summary>
        /// 发送消息
        /// </summary>
        public const int FUNCTIONID_GINTECH_SEND = 4;

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
                data.m_type = (int)br.ReadChar();
                data.m_text = br.ReadString();
                datas.Add(data);
            }
            br.Close();
            return 1;
        }

        /// <summary>
        /// 获取端口信息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>状态</returns>
        public int GetHostInfos(CMessage message)
        {
            List<GintechHostInfo> hostInfos = new List<GintechHostInfo>();
            lock (m_socketIDs)
            {
                foreach (int socketID in m_socketIDs.Keys)
                {
                    GintechHostInfo hf = new GintechHostInfo();
                    String strIPPort = m_socketIDs[socketID].Replace("accept:", "");
                    String[] strs = strIPPort.Split(new String[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    hf.m_ip = strs[0];
                    hostInfos.Add(hf);
                }
            }
            int hostInfosSize = hostInfos.Count;
            Binary bw = new Binary();
            bw.WriteInt(hostInfosSize);
            for (int i = 0; i < hostInfosSize; i++)
            {
                GintechHostInfo hostInfo = hostInfos[i];
                bw.WriteString(hostInfo.m_ip);
            }
            byte[] bytes = bw.GetBytes();
            message.m_body = bytes;
            message.m_bodyLength = bytes.Length;
            int ret = Send(message);
            bw.Close();
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
            lock (m_socketIDs)
            {
                List<int> removeSocketIDs = new List<int>();
                foreach(int sid in m_socketIDs.Keys)
                {
                    if (sid == socketID)
                    {
                        removeSocketIDs.Add(sid);
                    }
                }
                int removeSocketIDsSize = removeSocketIDs.Count;
                for (int i = 0; i < removeSocketIDsSize; i++)
                {
                    m_socketIDs.Remove(removeSocketIDs[i]);
                }
                removeSocketIDs.Clear();
            }
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
                    m_socketIDs[socketID] = ip;
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
                case FUNCTIONID_GETHOSTS:
                    GetHostInfos(message);
                    break;
                case FUNCTIONID_GINTECH_SEND:
                    SendMsg(message);
                    break;
                case FUNCTIONID_GINTECH_SENDALL:
                    SendAll(message);
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
                bw.WriteChar((char)data.m_type);
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
        public int SendMsg(CMessage message)
        {
            SendToListener(message);
            return 1;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>状态</returns>
        public int SendAll(CMessage message)
        {
            List<GintechData> datas = new List<GintechData>();
            GetGintechDatas(datas, message.m_body, message.m_bodyLength);
            message.m_functionID = FUNCTIONID_GINTECH_RECV;
            lock (m_socketIDs)
            {
                List<int> socketlist = new List<int>();
                foreach (int socketID in m_socketIDs.Keys)
                {
                    message.m_socketID = socketID;
                    int ret = Send(message, datas);
                    if (ret == -1)
                    {
                        socketlist.Add(socketID);
                    }
                }
                int listsize = socketlist.Count;
                for (int i = 0; i < listsize; i++)
                {
                    m_socketIDs.Remove(socketlist[i]);
                }
            }
            datas.Clear();
            return 1;
        }
        #endregion
    }
}
