/**************************************************************************************\
*                                                                                      *
* ProjectService.cs -  Project service functions, types, and definitions.       *
*                                                                                      *
*               Version 1.00 ��                                                        *
*                                                                                      *
*               Copyright (c) 2016-2016, iTeam. All rights reserved.               *
*               Created by Todd.                                                 *
*                                                                                      *
***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace OwLib
{
    /// <summary>
    /// ��Ŀ��Ϣ
    /// </summary>
    public class ProjectInfo
    {
        /// <summary>
        /// ���ĳ�Ա
        /// </summary>
        public String m_center = "";

        /// <summary>
        /// ��������
        /// </summary>
        public String m_endDate = "";

        /// <summary>
        /// ��ԱID
        /// </summary>
        public String m_jobIds = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// ID
        /// </summary>
        public String m_pID = "";

        /// <summary>
        /// ��ʼ����
        /// </summary>
        public String m_startDate = "";

        /// <summary>
        /// ״̬
        /// </summary>
        public String m_state = "";
    }

    /// <summary>
    /// ��Ŀ����
    /// </summary>
    public class ProjectService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public ProjectService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("PROJECT", ref cookie) > 0)
            {
                try
                {
                    m_projects = JsonConvert.DeserializeObject<List<ProjectInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_projects == null)
                {
                    try
                    {
                        m_projects = JsonConvert.DeserializeObject<List<ProjectInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// ��Ŀ��Ϣ
        /// </summary>
        public List<ProjectInfo> m_projects = new List<ProjectInfo>();

        /// <summary>
        /// ɾ����Ŀ��Ϣ
        /// </summary>
        /// <param name="pID">��ĿID</param>
        public void Delete(String pID)
        {
            int projectsSize = m_projects.Count;
            for (int i = 0; i < projectsSize; i++)
            {
                if (m_projects[i].m_pID == pID)
                {
                    m_projects.RemoveAt(i);
                    Save();
                    break;
                }
            }
        }

        /// <summary>
        /// ��ȡ�µ�Ա��ID
        /// </summary>
        /// <returns>Ա��ID</returns>
        public String GetNewPID()
        {
            List<int> ids = new List<int>();
            int projectsSize = m_projects.Count;
            for (int i = 0; i < projectsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_projects[i].m_pID));
            }
            ids.Sort();
            int idsSize = ids.Count;
            if (idsSize > 0)
            {
                return CStr.ConvertIntToStr(ids[idsSize - 1] + 1);
            }
            else
            {
                return "1";
            }
        }

        /// <summary>
        /// ��ȡ��Ŀ��Ϣ
        /// </summary>
        /// <param name="pID">��Ŀ��</param>
        /// <returns>Ա����Ϣ</returns>
        public ProjectInfo GetProject(String pID)
        {
            int projectsSize = m_projects.Count;
            for (int i = 0; i < projectsSize; i++)
            {
                if (m_projects[i].m_pID == pID)
                {
                    return m_projects[i];
                }
            }
            return new ProjectInfo();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "PROJECT";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_projects));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="project">��Ϣ</param>
        public void Save(ProjectInfo project)
        {
            bool modify = false;
            int projectsSize = m_projects.Count;
            for (int i = 0; i < projectsSize; i++)
            {
                if (m_projects[i].m_pID == project.m_pID)
                {
                    m_projects[i] = project;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_projects.Add(project);
            }
            Save();
        }
    }
}
