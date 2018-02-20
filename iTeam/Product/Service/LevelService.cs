/**************************************************************************************\
*                                                                                      *
* LevelService.cs -  Level service functions, types, and definitions.       *
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
    /// 水平信息
    /// </summary>
    public class LevelInfo : BaseInfo
    {
        /// <summary>
        /// 日期
        /// </summary>
        public String m_date;

        /// <summary>
        /// 级别
        /// </summary>
        public String m_level = "";

        /// <summary>
        /// 级别2
        /// </summary>
        public String m_level2 = "";

        /// <summary>
        /// 级别3
        /// </summary>
        public String m_level3 = "";
    }

    /// <summary>
    /// 水平服务
    /// </summary>
    public class LevelService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public LevelService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("LEVEL", ref cookie) > 0)
            {
                try
                {
                    m_levels = JsonConvert.DeserializeObject<List<LevelInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_levels == null)
                {
                    try
                    {
                        m_levels = JsonConvert.DeserializeObject<List<LevelInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 信息
        /// </summary>
        public List<LevelInfo> m_levels = new List<LevelInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int levelsSize = m_levels.Count;
            for (int i = 0; i < levelsSize; i++)
            {
                if (m_levels[i].m_ID == id)
                {
                    m_levels.RemoveAt(i);
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
            int levelsSize = m_levels.Count;
            for (int i = 0; i < levelsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_levels[i].m_ID));
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
        public LevelInfo GetLevel(String id)
        {
            int levelsSize = m_levels.Count;
            for (int i = 0; i < levelsSize; i++)
            {
                if (m_levels[i].m_ID == id)
                {
                    return m_levels[i];
                }
            }
            return new LevelInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "LEVEL";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_levels));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="award">信息</param>
        public void Save(LevelInfo level)
        {
            bool modify = false;
            int levelsSize = m_levels.Count;
            for (int i = 0; i < levelsSize; i++)
            {
                if (m_levels[i].m_ID == level.m_ID)
                {
                    m_levels[i] = level;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_levels.Add(level);
            }
            Save();
        }
    }
}
