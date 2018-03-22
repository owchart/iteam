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
        private static Dictionary<String, OwLib.GintechService> m_clientGintechServices = new Dictionary<String, OwLib.GintechService>();

        /// <summary>
        /// 获取客户端的聊天服务
        /// </summary>
        public static Dictionary<String, OwLib.GintechService> ClientGintechServices
        {
            get { return DataCenter.m_clientGintechServices; }
        }

        private static OwLib.GintechService m_mainGintechService;

        /// <summary>
        /// 获取主通讯服务
        /// </summary>
        public static OwLib.GintechService MainGintechService
        {
            get { return DataCenter.m_mainGintechService; }
        }

        private static OwLibSV.GintechService m_serverGintechService = new OwLibSV.GintechService();

        /// <summary>
        /// 获取服务端的聊天服务
        /// </summary>
        public static OwLibSV.GintechService ServerGintechService
        {
            get { return DataCenter.m_serverGintechService; }
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
            OwLibSV.BaseService.AddService(m_serverGintechService);
            OwLibSV.BaseService.StartServer(0, 9966);
            OwLib.GintechService clientGintechService = new OwLib.GintechService();
            String mainGintechIP = "192.168.88.103";
            m_clientGintechServices[mainGintechIP] = clientGintechService;
            OwLib.BaseService.AddService(clientGintechService);
            int socketID = OwLib.BaseService.Connect(mainGintechIP, 9966);
            clientGintechService.SocketID = socketID;
            m_mainGintechService = clientGintechService;

        }
        #endregion
    }
}
