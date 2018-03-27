/*****************************************************************************\
*                                                                             *
* GintechService.cs -  Gintech service functions, types, and definitions.          *
*                                                                             *
*               Version 1.00 ��                                               *
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
    /// ����������
    /// </summary>
    public class GintechData
    {
        #region �봺�� 2016/6/9
        /// <summary>
        /// ����
        /// </summary>
        public String m_text = "";
        #endregion
    }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public class GintechHostInfo
    {
        /// <summary>
        /// IP��ַ
        /// </summary>
        public String m_ip;

        /// <summary>
        /// ����˶˿�
        /// </summary>
        public int m_serverPort;

        /// <summary>
        /// ���߻�����
        /// </summary>
        public int m_type;
    }

    /// <summary>
    /// ����������
    /// </summary>
    public class GintechService:BaseService
    {
        #region �봺�� 2016/06/03
        /// <summary>
        /// ��������������
        /// </summary>
        public GintechService()
        {
            ServiceID = SERVICEID_GINTECH;
        }

        /// <summary>
        /// ��
        /// </summary>
        public object m_lock = new object();

        /// <summary>
        /// �Ự�б�
        /// </summary>
        public Dictionary<int, GintechHostInfo> m_socketIDs = new Dictionary<int, GintechHostInfo>();

        /// <summary>
        /// ��Ļ����ID
        /// </summary>
        private const int SERVICEID_GINTECH = 10000;

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public const int FUNCTIONID_GETHOSTS = 1;

        /// <summary>
        /// �㲥����������ID
        /// </summary>
        public const int FUNCTIONID_GINTECH_SENDALL = 2;

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public const int FUNCTIONID_GINTECH_SEND = 4;

        /// <summary>
        /// ����
        /// </summary>
        public const int FUNCTIONID_GINTECH_ENTER = 6;

        private int m_port = 16666;

        /// <summary>
        /// ��ȡ�����ö˿�
        /// </summary>
        public int Port
        {
            get { return m_port; }
            set { m_port = value; }
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="message">��Ϣ</param>
        /// <returns>����</returns>
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
        /// ��ȡ��Ļ��Ϣ
        /// </summary>
        /// <param name="loginInfos">��Ļ��Ϣ</param>
        /// <param name="body">����</param>
        /// <param name="bodyLength">���峤��</param>
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
        /// �ͻ��˹رշ���
        /// </summary>
        /// <param name="socketID">����ID</param>
        /// <param name="localSID">��������ID</param>
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
        /// �ͻ������ӷ���
        /// </summary>
        /// <param name="socketID">����ID</param>
        /// <param name="localSID">��������ID</param>
        /// <param name="ip">IP��ַ</param>
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
        /// ��������
        /// </summary>
        /// <param name="message">��Ϣ</param>
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
        /// socket����
        /// </summary>
        /// <param name="message">��Ϣ</param>
        /// <param name="loginInfos">��¼��Ϣ�б�</param>
        /// <returns>״̬</returns>
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
        /// ������Ϣ
        /// </summary>
        /// <param name="message">��Ϣ</param>
        /// <returns>״̬</returns>
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
        /// ����������Ϣ
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
        /// ������Ϣ
        /// </summary>
        /// <param name="message">��Ϣ</param>
        /// <returns>״̬</returns>
        public int SendMsg(CMessage message)
        {
            SendToListener(message);
            return 1;
        }
        #endregion
    }
}
