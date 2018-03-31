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
        private static Dictionary<String, OwLib.GintechService> m_clientGintechServices = new Dictionary<String, OwLib.GintechService>();

        /// <summary>
        /// ��ȡ�ͻ��˵�����������
        /// </summary>
        public static Dictionary<String, OwLib.GintechService> ClientGintechServices
        {
            get { return DataCenter.m_clientGintechServices; }
        }

        private static Config m_config = new Config();

        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        public static Config Config
        {
            get { return DataCenter.m_config; }
        }

        /// <summary>
        /// ������ͨ������ID
        /// </summary>
        public static int GintechRequestID
        {
            get { return 9999; }
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
            set { DataCenter.m_mainUI = value; }
        }

        private static OwLibSV.GintechService m_serverGintechService = new OwLibSV.GintechService();

        /// <summary>
        /// ��ȡ����˵�����������
        /// </summary>
        public static OwLibSV.GintechService ServerGintechService
        {
            get { return DataCenter.m_serverGintechService; }
        }

        private static UserCookieService m_userCookieService;

        /// <summary>
        /// �û�Cookie����
        /// </summary>
        public static UserCookieService UserCookieService
        {
            get { return DataCenter.m_userCookieService; }
        }


        private static int m_userID = -1;

        /// <summary>
        /// ��ȡ�û�ID
        /// </summary>
        public static int UserID
        {
            get { return m_userID; }
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
        /// ��ȡ�û�Ŀ¼
        /// </summary>
        /// <returns>�û�Ŀ¼</returns>
        public static String GetUserPath()
        {
            return Application.StartupPath;
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="appPath">����·��</param>
        public static void StartService()
        {
            //��ȡ����
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DataCenter.GetAppPath() + "\\config.xml");
            XmlNode root = xmlDoc.DocumentElement;
            foreach (XmlNode node in root.ChildNodes)
            {
                String name = node.Name.ToLower();
                String value = node.InnerText;
                if (name == "clearcache")
                {
                    m_config.m_clearCache = value == "1";
                }
                else if (name == "defaulthost")
                {
                    if (value.Length > 0)
                    {
                        String[] strs = value.Split(new String[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        m_config.m_defaultHost = strs[0];
                        m_config.m_defaultPort = CStr.ConvertStrToInt(strs[1]);
                    }
                }
                else if (name == "isfull")
                {
                    m_config.m_isFull = value == "1";
                }
                else if (name == "localhost")
                {
                    if (value.Length > 0)
                    {
                        String[] strs = value.Split(new String[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        m_config.m_localHost = strs[0];
                        m_config.m_localPort = CStr.ConvertStrToInt(strs[1]);
                    }
                }
            }
            if (m_config.m_clearCache)
            {
                CFileA.RemoveFile(DataCenter.GetAppPath() + "\\usercookies.db");
            }
            m_userCookieService = new UserCookieService();
            Random rd = new Random();
            m_isFull = m_config.m_isFull;
            String[] servers = new String[] { };
            if (!m_isFull)
            {
                m_serverGintechService.Port = rd.Next(10000, 20000);
            }
            OwLibSV.BaseService.AddService(m_serverGintechService);
            OwLibSV.BaseService.StartServer(0, m_serverGintechService.Port);
        }
        #endregion
    }

    /// <summary>
    /// ����������
    /// </summary>
    public class Config
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
