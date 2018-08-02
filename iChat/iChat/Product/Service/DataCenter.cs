/*****************************************************************************\
*                                                                             *
* DataCenter.cs -  Data center functions, types, and definitions.             *
*                                                                             *
*               Version 1.00  ����                                          *
*                                                                             *
*               Copyright (c) 2016-2016, iTeam. All rights reserved.      *
*               Created by Lord 2016/3/10.                                    *
*                                                                             *
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Data;

namespace OwLib
{
    /// <summary>
    /// ������������
    /// </summary>
    public class DataCenter
    {
        #region Lord 2016/3/10
        /// <summary>
        /// ������ͨ������ID
        /// </summary>
        public static int ChatRequestID
        {
            get { return 9999; }
        }

        /// <summary>
        /// �ͻ��˵�����������
        /// </summary>
        private static Dictionary<String, OwLib.ChatService> m_clientChatServices = new Dictionary<String, OwLib.ChatService>();

        private static HostInfo m_hostInfo = new HostInfo();

        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        public static HostInfo HostInfo
        {
            get { return DataCenter.m_hostInfo; }
        }

        private static bool m_isFull;

        /// <summary>
        /// ��ȡ�������Ƿ�ȫ�ڵ�
        /// </summary>
        public static bool IsFull
        {
            get { return m_isFull; }
            set { m_isFull = value; }
        }

        private static UIXmlEx m_mainUI;

        /// <summary>
        /// �������ؼ�
        /// </summary>
        public static UIXmlEx MainUI
        {
            get { return DataCenter.m_mainUI; }
            set { DataCenter.m_mainUI = value; }
        }

        private static OwLibSV.ChatService m_serverChatService = new OwLibSV.ChatService();

        /// <summary>
        /// ��ȡ����˵�����������
        /// </summary>
        public static OwLibSV.ChatService ServerChatService
        {
            get { return DataCenter.m_serverChatService; }
        }

        private static UserCookieService m_userCookieService;

        /// <summary>
        /// �û�Cookie����
        /// </summary>
        public static UserCookieService UserCookieService
        {
            get { return DataCenter.m_userCookieService; }
        }


        private static String m_userID = "";

        /// <summary>
        /// ��ȡ�������û�ID
        /// </summary>
        public static String UserID
        {
            get { return m_userID; }
            set { m_userID = value; }
        }

        private static String m_userName = "";

        /// <summary>
        /// ��ȡ�������û���
        /// </summary>
        public static String UserName
        {
            get { return m_userName; }
            set { m_userName = value; }
        }

        /// <summary>
        /// ��ӿͻ����������
        /// </summary>
        /// <param name="key">��</param>
        /// <param name="clientChatService">�������</param>
        public static void AddClientChatService(String key, OwLib.ChatService clientChatService)
        {
            lock (m_clientChatServices)
            {
                m_clientChatServices[key] = clientChatService;
            }
        }

        /// <summary>
        /// ��ȡ����·��
        /// </summary>
        /// <returns>����·��</returns>
        public static String GetAppPath()
        {
            return Application.StartupPath;
        }

        /// <summary>
        /// ��ȡ�ͻ����������
        /// </summary>
        /// <param name="key">��</param>
        /// <returns>�������</returns>
        public static OwLib.ChatService GetClientChatService(String key)
        {
            OwLib.ChatService clientChatService = null;
            lock (m_clientChatServices)
            {
                if (m_clientChatServices.ContainsKey(key))
                {
                    clientChatService = m_clientChatServices[key];
                }
            }
            return clientChatService;
        }

        /// <summary>
        /// ��ȡ�ͻ���������������
        /// </summary>
        /// <returns>����</returns>
        public static int GetClientChatServiceSize()
        {
            int size = 0;
            lock (m_clientChatServices)
            {
                size = m_clientChatServices.Count;
            }
            return size;
        }

        /// <summary>
        /// ��ȡ�û�Ŀ¼
        /// </summary>
        /// <returns>�û�Ŀ¼</returns>
        public static String GetUserPath()
        {
            return Application.StartupPath;
        }

        /// <summary>
        /// �Ƴ��ͻ����������
        /// </summary>
        /// <param name="key">��</param>
        public static void RemoveClientChatService(String key)
        {
            if (m_clientChatServices.ContainsKey(key))
            {
                m_clientChatServices.Remove(key);
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        public static void SendAll(ChatData chatData)
        {
            foreach (ChatService gs in m_clientChatServices.Values)
            {
                if (gs.ToServer && gs.Connected)
                {
                    gs.SendAll(chatData);
                }
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="appPath">����·��</param>
        public static void StartService()
        {
            //��ȡ����
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DataCenter.GetAppPath() + "\\hostinfo.xml");
            XmlNode root = xmlDoc.DocumentElement;
            foreach (XmlNode node in root.ChildNodes)
            {
                String name = node.Name.ToLower();
                String value = node.InnerText;
                if (name == "clearcache")
                {
                    m_hostInfo.m_clearCache = value == "1";
                }
                else if (name == "defaulthost")
                {
                    if (value.Length > 0)
                    {
                        String[] strs = value.Split(new String[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        m_hostInfo.m_defaultHost = strs[0];
                        m_hostInfo.m_defaultPort = CStr.ConvertStrToInt(strs[1]);
                    }
                }
                else if (name == "isfull")
                {
                    m_hostInfo.m_isFull = value == "1";
                }
                else if (name == "localhost")
                {
                    if (value.Length > 0)
                    {
                        String[] strs = value.Split(new String[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        m_hostInfo.m_localHost = strs[0];
                        m_hostInfo.m_localPort = CStr.ConvertStrToInt(strs[1]);
                    }
                }
            }
            if (m_hostInfo.m_clearCache)
            {
                CFileA.RemoveFile(DataCenter.GetAppPath() + "\\usercookies.db");
            }
            m_userCookieService = new UserCookieService();
            Random rd = new Random();
            m_isFull = m_hostInfo.m_isFull;
            String[] servers = new String[] { };
            OwLibSV.BaseService.AddService(m_serverChatService);
            if (m_isFull)
            {
                OwLibSV.BaseService.StartServer(0, m_serverChatService.Port);
            }
        }
        #endregion
    }

    /// <summary>
    /// ����������
    /// </summary>
    public class HostInfo
    {
        /// <summary>
        /// �Ƿ��������
        /// </summary>
        public bool m_clearCache;

        /// <summary>
        /// Ĭ�ϵ�ַ
        /// </summary>
        public String m_defaultHost = "";

        /// <summary>
        /// Ĭ�϶˿�
        /// </summary>
        public int m_defaultPort;

        /// <summary>
        /// �Ƿ�ȫ�ڵ�
        /// </summary>
        public bool m_isFull;

        /// <summary>
        /// ���ص�ַ
        /// </summary>
        public String m_localHost = "";

        /// <summary>
        /// ���ض˿�
        /// </summary>
        public int m_localPort;
    }
}
