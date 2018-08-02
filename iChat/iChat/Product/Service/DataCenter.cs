/*****************************************************************************\
*                                                                             *
* DataCenter.cs -  Data center functions, types, and definitions.             *
*                                                                             *
*               Version 1.00  ★★★                                          *
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
    /// 处理行情数据
    /// </summary>
    public class DataCenter
    {
        #region Lord 2016/3/10
        /// <summary>
        /// 区块链通用请求ID
        /// </summary>
        public static int ChatRequestID
        {
            get { return 9999; }
        }

        /// <summary>
        /// 客户端的区块链服务
        /// </summary>
        private static Dictionary<String, OwLib.ChatService> m_clientChatServices = new Dictionary<String, OwLib.ChatService>();

        private static HostInfo m_hostInfo = new HostInfo();

        /// <summary>
        /// 获取配置信息
        /// </summary>
        public static HostInfo HostInfo
        {
            get { return DataCenter.m_hostInfo; }
        }

        private static bool m_isFull;

        /// <summary>
        /// 获取或设置是否全节点
        /// </summary>
        public static bool IsFull
        {
            get { return m_isFull; }
            set { m_isFull = value; }
        }

        private static UIXmlEx m_mainUI;

        /// <summary>
        /// 设置主控件
        /// </summary>
        public static UIXmlEx MainUI
        {
            get { return DataCenter.m_mainUI; }
            set { DataCenter.m_mainUI = value; }
        }

        private static OwLibSV.ChatService m_serverChatService = new OwLibSV.ChatService();

        /// <summary>
        /// 获取服务端的区块链服务
        /// </summary>
        public static OwLibSV.ChatService ServerChatService
        {
            get { return DataCenter.m_serverChatService; }
        }

        private static UserCookieService m_userCookieService;

        /// <summary>
        /// 用户Cookie服务
        /// </summary>
        public static UserCookieService UserCookieService
        {
            get { return DataCenter.m_userCookieService; }
        }


        private static String m_userID = "";

        /// <summary>
        /// 获取或设置用户ID
        /// </summary>
        public static String UserID
        {
            get { return m_userID; }
            set { m_userID = value; }
        }

        private static String m_userName = "";

        /// <summary>
        /// 获取或设置用户名
        /// </summary>
        public static String UserName
        {
            get { return m_userName; }
            set { m_userName = value; }
        }

        /// <summary>
        /// 添加客户端聊天服务
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="clientChatService">聊天服务</param>
        public static void AddClientChatService(String key, OwLib.ChatService clientChatService)
        {
            lock (m_clientChatServices)
            {
                m_clientChatServices[key] = clientChatService;
            }
        }

        /// <summary>
        /// 获取程序路径
        /// </summary>
        /// <returns>程序路径</returns>
        public static String GetAppPath()
        {
            return Application.StartupPath;
        }

        /// <summary>
        /// 获取客户端聊天服务
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>聊天服务</returns>
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
        /// 获取客户端聊天服务的数量
        /// </summary>
        /// <returns>数量</returns>
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
        /// 获取用户目录
        /// </summary>
        /// <returns>用户目录</returns>
        public static String GetUserPath()
        {
            return Application.StartupPath;
        }

        /// <summary>
        /// 移除客户端聊天服务
        /// </summary>
        /// <param name="key">键</param>
        public static void RemoveClientChatService(String key)
        {
            if (m_clientChatServices.ContainsKey(key))
            {
                m_clientChatServices.Remove(key);
            }
        }

        /// <summary>
        /// 发送所有
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
        /// 开启服务
        /// </summary>
        /// <param name="appPath">程序路径</param>
        public static void StartService()
        {
            //读取配置
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
    /// 服务器配置
    /// </summary>
    public class HostInfo
    {
        /// <summary>
        /// 是否清除缓存
        /// </summary>
        public bool m_clearCache;

        /// <summary>
        /// 默认地址
        /// </summary>
        public String m_defaultHost = "";

        /// <summary>
        /// 默认端口
        /// </summary>
        public int m_defaultPort;

        /// <summary>
        /// 是否全节点
        /// </summary>
        public bool m_isFull;

        /// <summary>
        /// 本地地址
        /// </summary>
        public String m_localHost = "";

        /// <summary>
        /// 本地端口
        /// </summary>
        public int m_localPort;
    }
}
