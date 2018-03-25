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

        /// <summary>
        /// ������ͨ������ID
        /// </summary>
        public static int GintechRequestID
        {
            get { return 9999; }
        }

        private static OwLibSV.GintechService m_serverGintechService = new OwLibSV.GintechService();

        /// <summary>
        /// ��ȡ����˵�����������
        /// </summary>
        public static OwLibSV.GintechService ServerGintechService
        {
            get { return DataCenter.m_serverGintechService; }
        }

        private static UIXmlEx m_mainUI;

        /// <summary>
        /// �������ؼ�
        /// </summary>
        public static UIXmlEx MainUI
        {
            set { DataCenter.m_mainUI = value; }
        }

        private static UserCookieService m_userCookieService = new UserCookieService();

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
            OwLibSV.BaseService.AddService(m_serverGintechService);
            Random rd = new Random();
            //m_serverGintechService.Port = rd.Next(10001, 20000);
            OwLibSV.BaseService.StartServer(0, m_serverGintechService.Port);
            OwLib.GintechService clientGintechService = new OwLib.GintechService();
            String[] servers = new String[] { "127.0.0.1:9966" };
            foreach (String server in servers)
            {
                String[] strs = server.Split(new String[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                m_clientGintechServices[server] = clientGintechService;
                OwLib.BaseService.AddService(clientGintechService);
                clientGintechService.ToServer = true;
                int socketID = OwLib.BaseService.Connect(strs[0], CStr.ConvertStrToInt(strs[1]));
                clientGintechService.SocketID = socketID;
            }
        }
        #endregion
    }
}
