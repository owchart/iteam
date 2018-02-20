/**************************************************************************************\
*                                                                                      *
* OverWorkService.cs -  Over work service functions, types, and definitions.       *
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
    /// 加班记录
    /// </summary>
    public class OverWorkInfo : BaseInfo
    {
        /// <summary>
        /// 内容
        /// </summary>
        public String m_content = "";

        /// <summary>
        /// 工号
        /// </summary>
        public String m_jobID = "";
    }

    /// <summary>
    /// 加班记录服务
    /// </summary>
    public class OverWorkService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public OverWorkService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("OVERWORK", ref cookie) > 0)
            {
                try
                {
                    m_overWorks = JsonConvert.DeserializeObject<List<OverWorkInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_overWorks == null)
                {
                    try
                    {
                        m_overWorks = JsonConvert.DeserializeObject<List<OverWorkInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 加班信息
        /// </summary>
        public List<OverWorkInfo> m_overWorks = new List<OverWorkInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int overWorksSize = m_overWorks.Count;
            for (int i = 0; i < overWorksSize; i++)
            {
                if (m_overWorks[i].m_ID == id)
                {
                    m_overWorks.RemoveAt(i);
                    Save();
                    break;
                }
            }
        }

        /// <summary>
        /// 获取新的ID
        /// </summary>
        /// <returns>新ID</returns>
        public String GetNewID()
        {
            List<int> ids = new List<int>();
            int overWorksSize = m_overWorks.Count;
            for (int i = 0; i < overWorksSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_overWorks[i].m_ID));
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
        /// 获取信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>信息</returns>
        public OverWorkInfo GetOverWork(String id)
        {
            int overWorksSize = m_overWorks.Count;
            for (int i = 0; i < overWorksSize; i++)
            {
                if (m_overWorks[i].m_ID == id)
                {
                    return m_overWorks[i];
                }
            }
            return new OverWorkInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "OVERWORK";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_overWorks));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="overWork">信息</param>
        public void Save(OverWorkInfo overWork)
        {
            bool modify = false;
            int overWorksSize = m_overWorks.Count;
            for (int i = 0; i < overWorksSize; i++)
            {
                if (m_overWorks[i].m_ID == overWork.m_ID)
                {
                    m_overWorks[i] = overWork;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_overWorks.Add(overWork);
            }
            Save();
        }
    }
}
