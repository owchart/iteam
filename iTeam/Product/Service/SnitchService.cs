/**************************************************************************************\
*                                                                                      *
* SnitchService.cs -  Snitch service functions, types, and definitions.       *
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
    /// 打小报告记录
    /// </summary>
    public class SnitchInfo : BaseInfo
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
    public class SnitchService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public SnitchService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("SNITCH", ref cookie) > 0)
            {
                try
                {
                    m_snitches = JsonConvert.DeserializeObject<List<SnitchInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_snitches == null)
                {
                    try
                    {
                        m_snitches = JsonConvert.DeserializeObject<List<SnitchInfo>>(cookie.m_value);
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
        public List<SnitchInfo> m_snitches = new List<SnitchInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int snitchesSize = m_snitches.Count;
            for (int i = 0; i < snitchesSize; i++)
            {
                if (m_snitches[i].m_ID == id)
                {
                    m_snitches.RemoveAt(i);
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
            int snitchesSize = m_snitches.Count;
            for (int i = 0; i < snitchesSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_snitches[i].m_ID));
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
        public SnitchInfo GetSnitch(String id)
        {
            int snitchesSize = m_snitches.Count;
            for (int i = 0; i < snitchesSize; i++)
            {
                if (m_snitches[i].m_ID == id)
                {
                    return m_snitches[i];
                }
            }
            return new SnitchInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "SNITCH";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_snitches));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="master">信息</param>
        public void Save(SnitchInfo snitch)
        {
            bool modify = false;
            int snitchesSize = m_snitches.Count;
            for (int i = 0; i < snitchesSize; i++)
            {
                if (m_snitches[i].m_ID == snitch.m_ID)
                {
                    m_snitches[i] = snitch;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_snitches.Add(snitch);
            }
            Save();
        }
    }
}
