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
        private static OwLib.ChatService m_clientChatService = new OwLib.ChatService();

        /// <summary>
        /// 获取客户端的聊天服务
        /// </summary>
        public static OwLib.ChatService ClientChatService
        {
            get { return DataCenter.m_clientChatService; }
        }

        private static OwLibSV.ChatService m_serverChatService = new OwLibSV.ChatService();

        /// <summary>
        /// 获取服务端的聊天服务
        /// </summary>
        public static OwLibSV.ChatService ServerChatService
        {
            get { return DataCenter.m_serverChatService; }
        }

        private static UIXmlEx m_mainUI;

        /// <summary>
        /// 设置主控件
        /// </summary>
        public static UIXmlEx MainUI
        {
            set { DataCenter.m_mainUI = value; }
        }

        private static UserCookieService m_userCookieService = new UserCookieService();

        /// <summary>
        /// 用户Cookie服务
        /// </summary>
        public static UserCookieService UserCookieService
        {
            get { return DataCenter.m_userCookieService; }
        }


        private static int m_userID = -1;

        /// <summary>
        /// 获取用户ID
        /// </summary>
        public static int UserID
        {
            get { return m_userID; }
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
        /// 获取用户目录
        /// </summary>
        /// <returns>用户目录</returns>
        public static String GetUserPath()
        {
            return Application.StartupPath;
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <param name="appPath">程序路径</param>
        public static void StartService()
        {
            OwLibSV.BaseService.AddService(m_serverChatService);
            OwLibSV.BaseService.StartServer(0, 9966);
            OwLib.BaseService.AddService(m_clientChatService);
            int socketID = OwLib.BaseService.Connect("127.0.0.1", 9966);
            m_clientChatService.SocketID = socketID;

        }
        #endregion
    }
}
