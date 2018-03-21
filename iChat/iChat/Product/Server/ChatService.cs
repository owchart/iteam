/*****************************************************************************\
*                                                                             *
* ChatService.cs -  Login service functions, types, and definitions.          *
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
    /// ��������
    /// </summary>
    public class ChatData
    {
        #region �봺�� 2016/6/9
        /// <summary>
        /// ����
        /// </summary>
        public String m_text = "";

        /// <summary>
        /// ����
        /// </summary>
        public int m_type;
        #endregion
    }

    /// <summary>
    /// �������
    /// </summary>
    public class ChatService:BaseService
    {
        #region �봺�� 2016/06/03
        /// <summary>
        /// �����������
        /// </summary>
        public ChatService()
        {
            ServiceID = SERVICEID_CHAT;
        }

        /// <summary>
        /// ��
        /// </summary>
        private object m_lock = new object();

        /// <summary>
        /// �Ự�б�
        /// </summary>
        private Dictionary<int, String> m_socketIDs = new Dictionary<int, String>();

        /// <summary>
        /// ��Ļ����ID
        /// </summary>
        private const int SERVICEID_CHAT = 7;

        /// <summary>
        /// �������칦��ID
        /// </summary>
        public const int FUNCTIONID_CHAT_SENDALL = 2;

        /// <summary>
        /// �������칦��ID
        /// </summary>
        public const int FUNCTIONID_CHAT_RECV = 3;

        /// <summary>
        /// ��ȡ��Ļ��Ϣ
        /// </summary>
        /// <param name="loginInfos">��Ļ��Ϣ</param>
        /// <param name="body">����</param>
        /// <param name="bodyLength">���峤��</param>
        public int GetChatDatas(List<ChatData> datas, byte[] body, int bodyLength)
        {
            Binary br = new Binary();
            br.Write(body, bodyLength);
            int chatSize = br.ReadInt();
            for (int i = 0; i < chatSize; i++)
            {
                ChatData chat = new ChatData();
                chat.m_type = (int)br.ReadChar();
                chat.m_text = br.ReadString();
                datas.Add(chat);
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
        /// �ͻ������ӷ���
        /// </summary>
        /// <param name="socketID"></param>
        /// <param name="localSID"></param>
        /// <param name="ip"></param>
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
        /// ��������
        /// </summary>
        /// <param name="message">��Ϣ</param>
        public override void OnReceive(CMessage message)
        {
            base.OnReceive(message);
            switch (message.m_functionID)
            {
                case FUNCTIONID_CHAT_SENDALL:
                    SendToAllClients(message);
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
        public int Send(CMessage message, List<ChatData> datas)
        {
            Binary bw = new Binary();
            int chatsize = datas.Count;
            bw.WriteInt(chatsize);
            for (int i = 0; i < chatsize; i++)
            {
                ChatData chat = datas[i];
                bw.WriteChar((char)chat.m_type);
                bw.WriteString(chat.m_text);
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
        public int SendToAllClients(CMessage message)
        {
            List<ChatData> datas = new List<ChatData>();
            GetChatDatas(datas, message.m_body, message.m_bodyLength);
            message.m_functionID = FUNCTIONID_CHAT_RECV;
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
