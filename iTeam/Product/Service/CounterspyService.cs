/**************************************************************************************\
*                                                                                      *
* CounterspyService.cs -  Counterspy service functions, types, and definitions.       *
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
    /// 反间谍活动记录
    /// </summary>
    public class CounterspyInfo : BaseInfo
    {
        /// <summary>
        /// 内容
        /// </summary>
        public String m_content = "";

        /// <summary>
        /// 级别
        /// </summary>
        public String m_level = "";

        /// <summary>
        /// 标题
        /// </summary>
        public String m_title = "";
    }

    /// <summary>
    /// 打小报告记录服务
    /// </summary>
    public class CounterspyService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public CounterspyService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("COUNTERSPY", ref cookie) > 0)
            {
                try
                {
                    m_counterspys = JsonConvert.DeserializeObject<List<CounterspyInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_counterspys == null)
                {
                    try
                    {
                        m_counterspys = JsonConvert.DeserializeObject<List<CounterspyInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 指示信息
        /// </summary>
        public List<CounterspyInfo> m_counterspys = new List<CounterspyInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int counterspysSize = m_counterspys.Count;
            for (int i = 0; i < counterspysSize; i++)
            {
                if (m_counterspys[i].m_ID == id)
                {
                    m_counterspys.RemoveAt(i);
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
            int counterspysSize = m_counterspys.Count;
            for (int i = 0; i < counterspysSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_counterspys[i].m_ID));
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
        public CounterspyInfo GetCounterspy(String id)
        {
            int counterspysSize = m_counterspys.Count;
            for (int i = 0; i < counterspysSize; i++)
            {
                if (m_counterspys[i].m_ID == id)
                {
                    return m_counterspys[i];
                }
            }
            return new CounterspyInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "COUNTERSPY";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_counterspys));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="master">信息</param>
        public void Save(CounterspyInfo counterspy)
        {
            bool modify = false;
            int counterspysSize = m_counterspys.Count;
            for (int i = 0; i < counterspysSize; i++)
            {
                if (m_counterspys[i].m_ID == counterspy.m_ID)
                {
                    m_counterspys[i] = counterspy;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_counterspys.Add(counterspy);
            }
            Save();
        }
    }
}
