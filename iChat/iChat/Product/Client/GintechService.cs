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
        /// ����
        /// </summary>
        public int m_type;
    }

    /// <summary>
    /// ����������
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
        /// ����������ID
        /// </summary>
        public const int SERVICEID_GINTECH = 10000;

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

        private bool m_toServer;

        /// <summary>
        /// ��ȡ�������Ƿ��͵������
        /// </summary>
        public bool ToServer
        {
            get { return m_toServer; }
            set { m_toServer = value; }
        }

        private String m_serverIP;

        /// <summary>
        /// ��ȡ�����÷�������ַ
        /// </summary>
        public String ServerIP
        {
            get { return m_serverIP; }
            set { m_serverIP = value; }
        }

        private int m_serverPort;

        /// <summary>
        /// ��ȡ�����÷������˿�
        /// </summary>
        public int ServerPort
        {
            get { return m_serverPort; }
            set { m_serverPort = value; }
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <returns>״̬</returns>
        public int Enter()
        {
            Binary bw = new Binary();
            bw.WriteInt(DataCenter.ServerGintechService.Port);
            bw.WriteInt(DataCenter.IsFull ? 1 : 0);
            byte[] bytes = bw.GetBytes();
            int ret = Send(new CMessage(GroupID, ServiceID, FUNCTIONID_GINTECH_ENTER, SessionID, DataCenter.GintechRequestID, SocketID, 0, CompressType, bytes.Length, bytes));
            bw.Close();
            return ret;
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
        public static int GetHostInfos(List<GintechHostInfo> datas, ref int type, byte[] body, int bodyLength)
        {
            Binary br = new Binary();
            br.Write(body, bodyLength);
            int size = br.ReadInt();
            type = br.ReadInt();
            for (int i = 0; i < size; i++)
            {
                GintechHostInfo data = new GintechHostInfo();
                data.m_ip = br.ReadString();
                data.m_serverPort = br.ReadInt();
                data.m_type = br.ReadInt();
                datas.Add(data);
            }
            br.Close();
            return 1;     
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="message">��Ϣ</param>
        public override void OnReceive(CMessage message)
        {
            base.OnReceive(message);
            SendToListener(message);
            if (DataCenter.IsFull && message.m_functionID == FUNCTIONID_GINTECH_SENDALL && message.m_socketID != SocketID)
            {
                DataCenter.ServerGintechService.SendAll(message);
            }
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="userID">�û�ID</param>
        /// <param name="requestID">����ID</param>
        /// <param name="args"></param>
        public int Send(GintechData data)
        {
            List<GintechData> datas = new List<GintechData>();
            datas.Add(data);
            int ret = Send(FUNCTIONID_GINTECH_SEND, DataCenter.GintechRequestID, datas);
            datas.Clear();
            return ret > 0 ? 1 : 0;
        }

        /// <summary>
        /// ���뵯Ļ
        /// </summary>
        /// <param name="userID">�û�ID</param>
        /// <param name="requestID">����ID</param>
        /// <param name="args"></param>
        public int SendAll(GintechData data)
        {
            List<GintechData> datas = new List<GintechData>();
            datas.Add(data);
            int ret = Send(FUNCTIONID_GINTECH_SENDALL, DataCenter.GintechRequestID, datas);
            datas.Clear();
            return ret > 0 ? 1 : 0;
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="userID">����ID</param>
        /// <param name="userID">����ID</param>
        /// <param name="text">�����ַ�</param>
        public int Send(int functionID, int requestID, List<GintechData> datas)
        {
            Binary bw = new Binary();
            int dataSize = datas.Count;
            bw.WriteInt(dataSize);
            for (int i = 0; i < dataSize; i++)
            {
                GintechData data = datas[i];
                bw.WriteString(data.m_text);
            }
            byte[] bytes = bw.GetBytes();
            int ret = Send(new CMessage(GroupID, ServiceID, functionID, SessionID, requestID, SocketID, 0, CompressType, bytes.Length, bytes));
            bw.Close();
            return ret;
        }
        #endregion
    }
}
