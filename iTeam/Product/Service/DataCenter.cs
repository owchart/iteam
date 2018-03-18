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
        private static AwardService m_awardService;

        /// <summary>
        /// ��ȡ�ν�����
        /// </summary>
        public static AwardService AwardService
        {
            get { return m_awardService; }
        }

        private static BeAttackedService m_beAttackedService;

        /// <summary>
        /// ��ȡ����������
        /// </summary>
        public static BeAttackedService BeAttackedService
        {
            get { return m_beAttackedService; }
        }

        private static BSStockService m_bsStockService;

        /// <summary>
        /// ��ȡ��Ʊ��������
        /// </summary>
        public static BSStockService BSStockService
        {
            get { return m_bsStockService; }
        }

        private static BusinessCardService m_businessCardService;

        /// <summary>
        /// ��ȡ��Ƭ����
        /// </summary>
        public static BusinessCardService BusinessCardService
        {
            get { return DataCenter.m_businessCardService; }
        }

        private static CalendarService m_calendarService;

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        public static CalendarService CalendarService
        {
            get { return m_calendarService; }
        }

        private static CheckService m_checkService;

        /// <summary>
        /// ��ȡ���˼�����
        /// </summary>
        public static CheckService CheckService
        {
            get { return DataCenter.m_checkService; }
        }

        private static ClueService m_clueService;

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        public static ClueService ClueService
        {
            get { return m_clueService; }
        }

        private static CodeService m_codeService;

        /// <summary>
        /// ��ȡ�ж����ŷ���
        /// </summary>
        public static CodeService CodeService
        {
            get { return m_codeService; }
        }

        private static CounterspyService m_counterspyService;

        /// <summary>
        /// ��ȡ����������
        /// </summary>
        public static CounterspyService CounterspyService
        {
            get { return m_counterspyService; }
        }

        private static DimensionService m_dimensionService;

        /// <summary>
        /// ��ȡ��άͼ����
        /// </summary>
        public static DimensionService DimensionService
        {
            get { return m_dimensionService; }
        }

        private static DisobeyService m_disobeyService;

        /// <summary>
        /// ��ȡ������¼����
        /// </summary>
        public static DisobeyService DisobeyService
        {
            get { return m_disobeyService; }
        }

        private static ExamService m_examService;

        /// <summary>
        /// ��ȡ���Է���
        /// </summary>
        public static ExamService ExamService
        {
            get { return DataCenter.m_examService; }
        }

        private static ExportService m_exportService;

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        public static ExportService ExportService
        {
            get { return m_exportService; }
            set { m_exportService = value; }
        }

        private static FollowService m_followService;

        /// <summary>
        /// ��ȡ�ص��ע����
        /// </summary>
        public static FollowService FollowService
        {
            get { return m_followService; }
        }

        private static GitService m_gitService;

        /// <summary>
        /// ��ȡGit����
        /// </summary>
        public static GitService GitService
        {
            get { return m_gitService; }
        }

        private static JidianService m_jidianService;

        /// <summary>
        /// ��ȡ����ͳ�Ʒ���
        /// </summary>
        public static JidianService JidianService
        {
            get { return m_jidianService; }
        }

        private static LevelService m_levelService;

        /// <summary>
        /// ˮƽ����
        /// </summary>
        public static LevelService LevelService
        {
            get { return m_levelService; }
        }

        private static UIXmlEx m_mainUI;

        /// <summary>
        /// �������ؼ�
        /// </summary>
        public static UIXmlEx MainUI
        {
            set { DataCenter.m_mainUI = value; }
        }

        private static MasterService m_masterService;

        /// <summary>
        /// ��ȡ�ϼ�ָʾ����
        /// </summary>
        public static MasterService MasterService
        {
            get { return m_masterService; }
        }

        private static OpinionService m_opinionService;

        /// <summary>
        /// ��ȡ��Ҫ�������
        /// </summary>
        public static OpinionService OpinionService
        {
            get { return m_opinionService; }
        }

        private static OverWorkService m_overWorkService;

        /// <summary>
        /// ��ȡ�Ӱ��¼����
        /// </summary>
        public static OverWorkService OverWorkService
        {
            get { return m_overWorkService; }
        }

        private static PersonalService m_personalService;

        /// <summary>
        /// ��ȡ������Ϣ����
        /// </summary>
        public static PersonalService PersonalService
        {
            get { return m_personalService; }
        }

        private static PlanService m_planService;

        /// <summary>
        /// ��ȡ�����üƻ�����
        /// </summary>
        public static PlanService PlanService
        {
            get { return m_planService; }
            set { m_planService = value; }
        }

        private static PlanWindow m_planWindow;

        /// <summary>
        /// ��ȡ�����üƻ�����
        /// </summary>
        public static PlanWindow PlanWindow
        {
            get { return DataCenter.m_planWindow; }
            set { m_planWindow = value; }
        }

        private static ProjectService m_projectService;

        /// <summary>
        /// ��ȡ��Ŀ����
        /// </summary>
        public static ProjectService ProjectService
        {
            get { return m_projectService; }
        }

        private static RemoteService m_remoteService;

        /// <summary>
        /// ��ȡԶ�̷���
        /// </summary>
        public static RemoteService RemoteService
        {
            get { return m_remoteService; }
        }

        private static SecurityService m_securityService;

        /// <summary>
        /// ��ȡ֤ȯ����
        /// </summary>
        public static SecurityService SecurityService
        {
            get { return m_securityService; }
        }

        private static ServerService m_serverService;

        /// <summary>
        /// ��ȡ����������
        /// </summary>
        public static ServerService ServerService
        {
            get { return m_serverService; }
        }

        private static SnitchService m_snitchService;

        /// <summary>
        /// ��ȡ��С�������
        /// </summary>
        public static SnitchService SnitchService
        {
            get { return m_snitchService; }
        }

        private static SpeechService m_speechService;

        /// <summary>
        /// ��ȡ�ݽ�����
        /// </summary>
        public static SpeechService SpeechService
        {
            get { return m_speechService; }
        }

        private static StaffService m_staffService;

        /// <summary>
        /// ��ȡԱ������
        /// </summary>
        public static StaffService StaffService
        {
            get { return m_staffService; }
        }

        private static TheoryService m_theoryService;

        /// <summary>
        /// ��ȡ���۷���
        /// </summary>
        public static TheoryService TheoryService
        {
            get { return m_theoryService; }
        }

        private static TrainingService m_trainingService;

        /// <summary>
        /// ��ȡѵ������
        /// </summary>
        public static TrainingService TrainingService
        {
            get { return DataCenter.m_trainingService; }
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

        private static WangYiService m_wangYiService;

        /// <summary>
        /// �������
        /// </summary>
        public static WangYiService WangYiService
        {
            get { return m_wangYiService; }
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
        /// <param name="state">״̬</param>
        /// <returns>����״̬</returns>
        public static int LoadData(int state)
        {
            if (m_mainUI != null)
            {
                m_mainUI.LoadData();
            }
            return 0;
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="appPath">����·��</param>
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
