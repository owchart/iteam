/**************************************************************************************\
*                                                                                      *
* OpinionService.cs -  Opinion service functions, types, and definitions.       *
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
    /// 意见信息
    /// </summary>
    public class OpinionInfo : BaseInfo
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
    /// 线索服务
    /// </summary>
    public class OpinionService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public OpinionService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("OPINION", ref cookie) > 0)
            {
                try
                {
                    m_opinions = JsonConvert.DeserializeObject<List<OpinionInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_opinions == null)
                {
                    try
                    {
                        m_opinions = JsonConvert.DeserializeObject<List<OpinionInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 意见信息
        /// </summary>
        public List<OpinionInfo> m_opinions = new List<OpinionInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int opinionsSize = m_opinions.Count;
            for (int i = 0; i < opinionsSize; i++)
            {
                if (m_opinions[i].m_ID == id)
                {
                    m_opinions.RemoveAt(i);
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
            int opinionsSize = m_opinions.Count;
            for (int i = 0; i < opinionsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_opinions[i].m_ID));
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
        public OpinionInfo GetOpinion(String id)
        {
            int opinionsSize = m_opinions.Count;
            for (int i = 0; i < opinionsSize; i++)
            {
                if (m_opinions[i].m_ID == id)
                {
                    return m_opinions[i];
                }
            }
            return new OpinionInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "OPINION";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_opinions));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="opinion">信息</param>
        public void Save(OpinionInfo opinion)
        {
            bool modify = false;
            int opinionsSize = m_opinions.Count;
            for (int i = 0; i < opinionsSize; i++)
            {
                if (m_opinions[i].m_ID == opinion.m_ID)
                {
                    m_opinions[i] = opinion;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_opinions.Add(opinion);
            }
            Save();
        }
    }
}
