using System;
using System.Collections.Generic;
using System.Text;

namespace OwLib
{
    /// <summary>
    /// ��Ϣ�ص�ί��
    /// </summary>
    /// <param name="socketID">����ID</param>
    /// <param name="localSID">��������ID</param>
    /// <param name="str">����</param>
    /// <param name="len">����</param>
    public delegate void MessageCallBack(int socketID, int localSID, IntPtr str, int len);

    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="message">��Ϣ</param>
    public delegate void ListenerMessageCallBack(CMessage message);

    /// <summary>
    /// ��־����ί��
    /// </summary>
    /// <param name="socketID">����ID</param>
    /// <param name="localSID">��������ID</param>
    /// <param name="state">״̬</param>
    /// <param name="log">��־</param>
    public delegate void WriteLogCallBack(int socketID, int localSID, int state, String log);

    /// <summary>
    /// ��Ϣ�ṹ
    /// </summary>
    public class CMessage
    {
        /// <summary>
        /// ������Ϣ
        /// </summary>
        public CMessage()
        {
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="groupID">��ID</param>
        /// <param name="serviceID">����ID</param>
        /// <param name="functionID">����ID</param>
        /// <param name="sessionID">��¼ID</param>
        /// <param name="requestID">����ID</param>
        /// <param name="socketID">����ID</param>
        /// <param name="state">״̬</param>
        /// <param name="compressType">ѹ������</param>
        /// <param name="bodyLength">���峤��</param>
        /// <param name="body">����</param>
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
        /// ��ID
        /// </summary>
        public int m_groupID;

        /// <summary>
        /// ����ID
        /// </summary>
        public int m_serviceID;

        /// <summary>
        /// ����ID
        /// </summary>
        public int m_functionID;

        /// <summary>
        /// ��¼ID
        /// </summary>
        public int m_sessionID;

        /// <summary>
        /// ����ID
        /// </summary>
        public int m_requestID;

        /// <summary>
        /// ����ID
        /// </summary>
        public int m_socketID;

        /// <summary>
        /// ״̬
        /// </summary>
        public int m_state;

        /// <summary>
        /// ѹ������
        /// </summary>
        public int m_compressType;

        /// <summary>
        /// ���峤��
        /// </summary>
        public int m_bodyLength;

        /// <summary>
        /// ����
        /// </summary>
        public byte[] m_body;

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="message">��Ϣ</param>
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
    /// ��Ϣ����
    /// </summary>
    public class MessageListener
    {
        /// <summary>
        /// ������Ϣ����
        /// </summary>
        public MessageListener()
        {
        }

        /// <summary>
        /// ��������
        /// </summary>
        ~MessageListener()
        {
            Clear();
        }

        /// <summary>
        /// �����ص��б�
        /// </summary>
        private List<ListenerMessageCallBack> m_callBacks = new List<ListenerMessageCallBack>();

        /// <summary>
        /// ��ӻص�
        /// </summary>
        /// <param name="callBack">�ص�</param>
        public void Add(ListenerMessageCallBack callBack)
        {
            m_callBacks.Add(callBack);
        }

        /// <summary>
        /// �ص�����
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
        /// �������
        /// </summary>
        public void Clear()
        {
            m_callBacks.Clear();
        }

        /// <summary>
        /// �Ƴ��ص�
        /// </summary>
        /// <param name="callBack">�ص�</param>
        public void Remove(ListenerMessageCallBack callBack)
        {
            m_callBacks.Remove(callBack);
        }
    }

    /// <summary>
    /// �׽�����������Ϣ
    /// </summary>
    public class SocketArray
    {
        /// <summary>
        /// �׽���ID��
        /// </summary>
        private List<int> m_sockets = new List<int>();

        /// <summary>
        /// ����׽���ID
        /// </summary>
        /// <param name="socketID">�׽���ID</param>
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
        /// ��ȡ�׽����б�
        /// </summary>
        /// <param name="socketList">�׽����б�</param>
        public void GetSocketList(List<int> socketList)
        {
            int socketsSize = m_sockets.Count;
            for (int i = 0; i < socketsSize; i++)
            {
                socketList.Add(m_sockets[i]);
            }
        }

        /// <summary>
        /// �Ƴ��׽���ID
        /// </summary>
        /// <param name="socketID">�׽���ID</param>
        public void RemoveSocket(int socketID)
        {
            m_sockets.Remove(socketID);
        }
    }

}
