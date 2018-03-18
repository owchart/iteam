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

        private static BeAttackedService m_beAttackedService;

        /// <summary>
        /// 获取被攻击服务
        /// </summary>
        public static BeAttackedService BeAttackedService
        {
            get { return m_beAttackedService; }
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

        private static CheckService m_checkService;

        /// <summary>
        /// 获取个人检查服务
        /// </summary>
        public static CheckService CheckService
        {
            get { return DataCenter.m_checkService; }
        }

        private static ClueService m_clueService;

        /// <summary>
        /// 获取线索服务
        /// </summary>
        public static ClueService ClueService
        {
            get { return m_clueService; }
        }

        private static CodeService m_codeService;

        /// <summary>
        /// 获取行动代号服务
        /// </summary>
        public static CodeService CodeService
        {
            get { return m_codeService; }
        }

        private static CounterspyService m_counterspyService;

        /// <summary>
        /// 获取反间谍活动服务
        /// </summary>
        public static CounterspyService CounterspyService
        {
            get { return m_counterspyService; }
        }

        private static DimensionService m_dimensionService;

        /// <summary>
        /// 获取六维图服务
        /// </summary>
        public static DimensionService DimensionService
        {
            get { return m_dimensionService; }
        }

        private static DisobeyService m_disobeyService;

        /// <summary>
        /// 获取抗命记录服务
        /// </summary>
        public static DisobeyService DisobeyService
        {
            get { return m_disobeyService; }
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

        private static OverWorkService m_overWorkService;

        /// <summary>
        /// 获取加班记录服务
        /// </summary>
        public static OverWorkService OverWorkService
        {
            get { return m_overWorkService; }
        }

        private static PersonalService m_personalService;

        /// <summary>
        /// 获取个人信息服务
        /// </summary>
        public static PersonalService PersonalService
        {
            get { return m_personalService; }
        }

        private static PlanService m_planService;

        /// <summary>
        /// 获取或设置计划服务
        /// </summary>
        public static PlanService PlanService
        {
            get { return m_planService; }
            set { m_planService = value; }
        }

        private static PlanWindow m_planWindow;

        /// <summary>
        /// 获取或设置计划窗体
        /// </summary>
        public static PlanWindow PlanWindow
        {
            get { return DataCenter.m_planWindow; }
            set { m_planWindow = value; }
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

        private static SnitchService m_snitchService;

        /// <summary>
        /// 获取打小报告服务
        /// </summary>
        public static SnitchService SnitchService
        {
            get { return m_snitchService; }
        }

        private static SpeechService m_speechService;

        /// <summary>
        /// 获取演讲服务
        /// </summary>
        public static SpeechService SpeechService
        {
            get { return m_speechService; }
        }

        private static StaffService m_staffService;

        /// <summary>
        /// 获取员工服务
        /// </summary>
        public static StaffService StaffService
        {
            get { return m_staffService; }
        }

        private static TheoryService m_theoryService;

        /// <summary>
        /// 获取理论服务
        /// </summary>
        public static TheoryService TheoryService
        {
            get { return m_theoryService; }
        }

        private static TrainingService m_trainingService;

        /// <summary>
        /// 获取训练服务
        /// </summary>
        public static TrainingService TrainingService
        {
            get { return DataCenter.m_trainingService; }
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

        private static WangYiService m_wangYiService;

        /// <summary>
        /// 妄议服务
        /// </summary>
        public static WangYiService WangYiService
        {
            get { return m_wangYiService; }
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
            m_beAttackedService = new BeAttackedService();
            m_bsStockService = new BSStockService();
            m_businessCardService = new BusinessCardService();
            m_calendarService = new CalendarService();
            m_checkService = new CheckService();
            m_clueService = new ClueService();
            m_codeService = new CodeService();
            m_counterspyService = new CounterspyService();
            m_dimensionService = new DimensionService();
            m_disobeyService = new DisobeyService();
            m_exportService = new ExportService();
            m_followService = new FollowService();
            m_gitService = new GitService();
            m_jidianService = new JidianService();
            m_levelService = new LevelService();
            m_masterService = new MasterService();
            m_opinionService = new OpinionService();
            m_overWorkService = new OverWorkService();
            m_personalService = new PersonalService();
            m_projectService = new ProjectService();
            m_remoteService = new RemoteService();
            m_examService = new ExamService();
            m_securityService = new SecurityService();
            m_serverService = new ServerService();
            m_snitchService = new SnitchService();
            m_speechService = new SpeechService();
            m_theoryService = new TheoryService();
            m_trainingService = new TrainingService();
            m_wangYiService = new WangYiService();
            SecurityService.Start();
        }
        #endregion
    }
}
