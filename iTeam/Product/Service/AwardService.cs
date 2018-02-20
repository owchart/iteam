/**************************************************************************************\
*                                                                                      *
* AwardService.cs -  Award service functions, types, and definitions.       *
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
    /// 嘉奖记录
    /// </summary>
    public class AwardInfo : BaseInfo
    {
        /// <summary>
        /// 内容
        /// </summary>
        public String m_content = "";

        /// <summary>
        /// 工号
        /// </summary>
        public String m_jobID = "";

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
    /// 嘉奖记录服务
    /// </summary>
    public class AwardService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public AwardService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("AWARD", ref cookie) > 0)
            {
                try
                {
                    m_awards = JsonConvert.DeserializeObject<List<AwardInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_awards == null)
                {
                    try
                    {
                        m_awards = JsonConvert.DeserializeObject<List<AwardInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 嘉奖信息
        /// </summary>
        public List<AwardInfo> m_awards = new List<AwardInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int awardsSize = m_awards.Count;
            for (int i = 0; i < awardsSize; i++)
            {
                if (m_awards[i].m_ID == id)
                {
                    m_awards.RemoveAt(i);
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
            int awardsSize = m_awards.Count;
            for (int i = 0; i < awardsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_awards[i].m_ID));
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
        /// 获取嘉奖信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>嘉奖信息</returns>
        public AwardInfo GetAward(String id)
        {
            int awardsSize = m_awards.Count;
            for (int i = 0; i < awardsSize; i++)
            {
                if (m_awards[i].m_ID == id)
                {
                    return m_awards[i];
                }
            }
            return new AwardInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "AWARD";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_awards));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="award">嘉奖信息</param>
        public void Save(AwardInfo award)
        {
            bool modify = false;
            int awardsSize = m_awards.Count;
            for (int i = 0; i < awardsSize; i++)
            {
                if (m_awards[i].m_ID == award.m_ID)
                {
                    m_awards[i] = award;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_awards.Add(award);
            }
            Save();
        }
    }
}
