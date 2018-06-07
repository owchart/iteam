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
        private static AwardService m_awardService;

        /// <summary>
        /// 获取嘉奖服务
        /// </summary>
        public static AwardService AwardService
        {
            get { return m_awardService; }
        }

        private static BSStockService m_bsStockService;

        /// <summary>
        /// 获取股票买卖服务
        /// </summary>
        public static BSStockService BSStockService
        {
            get { return m_bsStockService; }
        }

        private static BusinessCardService m_businessCardService;

        /// <summary>
        /// 获取名片服务
        /// </summary>
        public static BusinessCardService BusinessCardService
        {
            get { return DataCenter.m_businessCardService; }
        }

        private static CalendarService m_calendarService;

        /// <summary>
        /// 获取日历服务
        /// </summary>
        public static CalendarService CalendarService
        {
            get { return m_calendarService; }
        }

        private static ClueService m_clueService;

        /// <summary>
        /// 获取线索服务
        /// </summary>
        public static ClueService ClueService
        {
            get { return m_clueService; }
        }

        private static DialogService m_dialogService;

        /// <summary>
        /// 获取对话服务
        /// </summary>
        public static DialogService DialogService
        {
            get { return DataCenter.m_dialogService; }
        }

        private static DimensionService m_dimensionService;

        /// <summary>
        /// 获取六维图服务
        /// </summary>
        public static DimensionService DimensionService
        {
            get { return m_dimensionService; }
        }

        private static ExamService m_examService;

        /// <summary>
        /// 获取考试服务
        /// </summary>
        public static ExamService ExamService
        {
            get { return DataCenter.m_examService; }
        }

        private static ExportService m_exportService;

        /// <summary>
        /// 获取导出服务
        /// </summary>
        public static ExportService ExportService
        {
            get { return m_exportService; }
            set { m_exportService = value; }
        }

        private static FollowService m_followService;

        /// <summary>
        /// 获取重点关注服务
        /// </summary>
        public static FollowService FollowService
        {
            get { return m_followService; }
        }

        private static GitService m_gitService;

        /// <summary>
        /// 获取Git服务
        /// </summary>
        public static GitService GitService
        {
            get { return m_gitService; }
        }

        private static JidianService m_jidianService;

        /// <summary>
        /// 获取代码统计服务
        /// </summary>
        public static JidianService JidianService
        {
            get { return m_jidianService; }
        }

        private static LevelService m_levelService;

        /// <summary>
        /// 水平服务
        /// </summary>
        public static LevelService LevelService
        {
            get { return m_levelService; }
        }

        private static UIXmlEx m_mainUI;

        /// <summary>
        /// 设置主控件
        /// </summary>
        public static UIXmlEx MainUI
        {
            set { DataCenter.m_mainUI = value; }
        }

        private static MasterService m_masterService;

        /// <summary>
        /// 获取上级指示服务
        /// </summary>
        public static MasterService MasterService
        {
            get { return m_masterService; }
        }

        private static OpinionService m_opinionService;

        /// <summary>
        /// 获取重要意见服务
        /// </summary>
        public static OpinionService OpinionService
        {
            get { return m_opinionService; }
        }

        private static PersonalService m_personalService;

        /// <summary>
        /// 获取个人信息服务
        /// </summary>
        public static PersonalService PersonalService
        {
            get { return m_personalService; }
        }

        private static ProjectService m_projectService;

        /// <summary>
        /// 获取项目服务
        /// </summary>
        public static ProjectService ProjectService
        {
            get { return m_projectService; }
        }

        private static RemoteService m_remoteService;

        /// <summary>
        /// 获取远程服务
        /// </summary>
        public static RemoteService RemoteService
        {
            get { return m_remoteService; }
        }

        private static SecurityService m_securityService;

        /// <summary>
        /// 获取证券服务
        /// </summary>
        public static SecurityService SecurityService
        {
            get { return m_securityService; }
        }

        private static ServerService m_serverService;

        /// <summary>
        /// 获取服务器服务
        /// </summary>
        public static ServerService ServerService
        {
            get { return m_serverService; }
        }

        private static StaffService m_staffService;

        /// <summary>
        /// 获取员工服务
        /// </summary>
        public static StaffService StaffService
        {
            get { return m_staffService; }
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
        /// 加载数据
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>加载状态</returns>
        public static int LoadData(int state)
        {
            if (m_mainUI != null)
            {
                m_mainUI.LoadData();
            }
            return 0;
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <param name="appPath">程序路径</param>
        public static void StartService()
        {
            m_staffService = new StaffService();
            m_awardService = new AwardService();
            m_bsStockService = new BSStockService();
            m_businessCardService = new BusinessCardService();
            m_calendarService = new CalendarService();
            m_clueService = new ClueService();
            m_dialogService = new DialogService();
            m_dimensionService = new DimensionService();
            m_exportService = new ExportService();
            m_followService = new FollowService();
            m_gitService = new GitService();
            m_jidianService = new JidianService();
            m_levelService = new LevelService();
            m_masterService = new MasterService();
            m_opinionService = new OpinionService();
            m_personalService = new PersonalService();
            m_projectService = new ProjectService();
            m_remoteService = new RemoteService();
            m_examService = new ExamService();
            m_securityService = new SecurityService();
            m_serverService = new ServerService();
            SecurityService.Start();
        }
        #endregion
    }
}
