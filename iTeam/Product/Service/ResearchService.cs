/**************************************************************************************\
*                                                                                      *
* ResearchService.cs -  Research service functions, types, and definitions.       *
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
    public class ResearchInfo
    {
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
    /// 研发任务服务
    /// </summary>
    public class ResearchService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public ResearchService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("RESEARCH", ref cookie) > 0)
            {
                try
                {
                    m_researchs = JsonConvert.DeserializeObject<List<ResearchInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_researchs == null)
                {
                    try
                    {
                        m_researchs = JsonConvert.DeserializeObject<List<ResearchInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 员工信息
        /// </summary>
        public List<ResearchInfo> m_researchs = new List<ResearchInfo>();

        /// <summary>
        /// 删除研发信息
        /// </summary>
        /// <param name="pID">项目ID</param>
        public void Delete(String pID)
        {
            int researchsSize = m_researchs.Count;
            for (int i = 0; i < researchsSize; i++)
            {
                if (m_researchs[i].m_pID == pID)
                {
                    m_researchs.RemoveAt(i);
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
            int researchsSize = m_researchs.Count;
            for (int i = 0; i < researchsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_researchs[i].m_pID));
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
        public ResearchInfo GetResearch(String pID)
        {
            int researchsSize = m_researchs.Count;
            for (int i = 0; i < researchsSize; i++)
            {
                if (m_researchs[i].m_pID == pID)
                {
                    return m_researchs[i];
                }
            }
            return new ResearchInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "RESEARCH";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_researchs));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="research">信息</param>
        public void Save(ResearchInfo research)
        {
            bool modify = false;
            int researchsSize = m_researchs.Count;
            for (int i = 0; i < researchsSize; i++)
            {
                if (m_researchs[i].m_pID == research.m_pID)
                {
                    m_researchs[i] = research;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_researchs.Add(research);
            }
            Save();
        }
    }
}
