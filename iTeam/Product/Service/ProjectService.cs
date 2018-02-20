/**************************************************************************************\
*                                                                                      *
* ProjectService.cs -  Project service functions, types, and definitions.       *
*                                                                                      *
*               Version 1.00 ★                                                        *
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
    /// 项目信息
    /// </summary>
    public class ProjectInfo
    {
        /// <summary>
        /// 中心成员
        /// </summary>
        public String m_center = "";

        /// <summary>
        /// 结束日期
        /// </summary>
        public String m_endDate = "";

        /// <summary>
        /// 成员ID
        /// </summary>
        public String m_jobIds = "";

        /// <summary>
        /// 名称
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// ID
        /// </summary>
        public String m_pID = "";

        /// <summary>
        /// 开始日期
        /// </summary>
        public String m_startDate = "";

        /// <summary>
        /// 状态
        /// </summary>
        public String m_state = "";
    }

    /// <summary>
    /// 项目服务
    /// </summary>
    public class ProjectService
    {
        /// <summary>
        /// 创建服务
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
        /// 项目信息
        /// </summary>
        public List<ProjectInfo> m_projects = new List<ProjectInfo>();

        /// <summary>
        /// 删除项目信息
        /// </summary>
        /// <param name="pID">项目ID</param>
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
        /// 获取新的员工ID
        /// </summary>
        /// <returns>员工ID</returns>
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
        /// 获取项目信息
        /// </summary>
        /// <param name="pID">项目号</param>
        /// <returns>员工信息</returns>
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
        /// 保存信息
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
        /// 保存信息
        /// </summary>
        /// <param name="project">信息</param>
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
