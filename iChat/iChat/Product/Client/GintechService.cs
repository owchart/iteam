/*****************************************************************************\
*                                                                             *
* GintechService.cs -  Gintech service functions, types, and definitions            *
*                                                                             *
*               Version 1.00 ��                                               *
*                                                                             *
*               Copyright (c) 2016-2016, Client. All rights reserved.         *
*               Created by QiChunyou.                                         *
*                                                                             *
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OwLib
{
    /// <summary>
    /// ��������
    /// </summary>
    public class GintechData
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
    /// ������Ϣ
    /// </summary>
    public class HostInfo
    {
        /// <summary>
        /// IP��ַ
        /// </summary>
        public String m_ip;

        /// <summary>
        /// �˿�
        /// </summary>
        public int m_port;

        /// <summary>
        /// �˿�
        /// </summary>
        public int m_socketID;
    }

    /// <summary>
    /// �������
    /// </summary>
    public class GintechService:BaseService
    {
        #region �봺�� 2016/6/3
        /// <summary>
        /// ���캯��
        /// </summary>
        public GintechService()
        {
            ServiceID = SERVICEID_GINTECH;
        }

        /// <summary>
        /// �������ID
        /// </summary>
        public const int SERVICEID_GINTECH = 7;

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public const int FUNCTIONID_GETHOSTS = 1;

        /// <summary>
        /// �������칦��ID
        /// </summary>
        public const int FUNCTIONID_GINTECH_SEND = 2;

        /// <summary>
        /// �������칦��ID
        /// </summary>
        public const int FUNCTIONID_GINTECH_RECV = 3;

        private int m_requestID = BaseService.GetRequestID();

        /// <summary>
        /// ��ȡ����ID
        /// </summary>
        public int RequestID
        {
            get { return m_requestID; }
        }

        private int m_socketID;

        /// <summary>
        /// ��ȡ�������׽���ID
        /// </summary>
        public int SocketID
        {
            get { return m_socketID; }
            set { m_socketID = value; }
        }

        /// <summary>
        /// ��ȡ��Ļ��Ϣ
        /// </summary>
        /// <param name="datas">���ڷ��ص�Ļ��Ϣ</param>
        /// <param name="body">����</param>
        /// <param name="bodyLength">���峤��</param>
        /// <returns></returns>
        public static int GetGintechDatas(List<GintechData> datas, byte[] body, int bodyLength)
        {
            Binary br = new Binary();
            br.Write(body, bodyLength);
            int size = br.ReadInt();
            for (int i = 0; i < size; i++)
            {
                GintechData data = new GintechData();
                data.m_type = br.ReadChar();
                data.m_text = br.ReadString();
                datas.Add(data);
            }
            br.Close();
            return 1;     
        }

        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        /// <param name="body">����</param>
        /// <param name="bodyLength">���峤��</param>
        /// <returns></returns>
        public static int GetHostInfos(List<HostInfo> datas, byte[] body, int bodyLength)
        {
            Binary br = new Binary();
            br.Write(body, bodyLength);
            int size = br.ReadInt();
            for (int i = 0; i < size; i++)
            {
                HostInfo data = new HostInfo();
                data.m_ip = br.ReadString();
                data.m_port = br.ReadInt();
                data.m_socketID = br.ReadInt();
                datas.Add(data);
            }
            br.Close();
            return 1;     
        }

        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public int GetHostInfos(int requestID)
        {
            byte[] bytes = new byte[1];
            int ret = Send(new CMessage(GroupID, ServiceID, FUNCTIONID_GETHOSTS, SessionID, requestID, m_socketID, 0, CompressType, bytes.Length, bytes));
            return ret > 0 ? 1 : 0;
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="message">��Ϣ</param>
        public override void OnReceive(CMessage message)
        {
            base.OnReceive(message);
            if (message.m_functionID == FUNCTIONID_GINTECH_RECV)
            {
                SendToListener(message);
            }         
        }

        /// <summary>
        /// ���뵯Ļ
        /// </summary>
        /// <param name="userID">�û�ID</param>
        /// <param name="requestID">����ID</param>
        /// <param name="args"></param>
        public int Send(int requestID, GintechData data)
        {
            List<GintechData> datas = new List<GintechData>();
            datas.Add(data);
            int ret = SendAll(FUNCTIONID_GINTECH_SEND, requestID, datas);
            datas.Clear();
            return ret > 0 ? 1 : 0;
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="userID">����ID</param>
        /// <param name="userID">����ID</param>
        /// <param name="text">�����ַ�</param>
        public int SendAll(int functionID, int requestID, List<GintechData> datas)
        {
            Binary bw = new Binary();
            int dataSize = datas.Count;
            bw.WriteInt(dataSize);
            for (int i = 0; i < dataSize; i++)
            {
                GintechData data = datas[i];
                bw.WriteChar((char)data.m_type);
                bw.WriteString(data.m_text);
            }
            byte[] bytes = bw.GetBytes();
            int ret = Send(new CMessage(GroupID, ServiceID, functionID, SessionID, requestID, m_socketID, 0, CompressType, bytes.Length, bytes));
            bw.Close();
            return ret;
        }
        #endregion
    }
}
