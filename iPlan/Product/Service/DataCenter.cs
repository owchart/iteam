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
        private static ExportService m_exportService;

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        public static ExportService ExportService
        {
            get { return DataCenter.m_exportService; }
            set { DataCenter.m_exportService = value; }
        }

        private static UIXmlEx m_mainUI;

        /// <summary>
        /// �������ؼ�
        /// </summary>
        public static UIXmlEx MainUI
        {
            set { DataCenter.m_mainUI = value; }
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
        #endregion
    }
}
