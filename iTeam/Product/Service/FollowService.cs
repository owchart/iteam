/**************************************************************************************\
*                                                                                      *
* FollowService.cs -  Follow service functions, types, and definitions.       *
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
    /// 关注信息
    /// </summary>
    public class FollowInfo : BaseInfo
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
    /// 重点关注服务
    /// </summary>
    public class FollowService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public FollowService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("FOLLOW", ref cookie) > 0)
            {
                try
                {
                    m_follows = JsonConvert.DeserializeObject<List<FollowInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_follows == null)
                {
                    try
                    {
                        m_follows = JsonConvert.DeserializeObject<List<FollowInfo>>(cookie.m_value);
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
        public List<FollowInfo> m_follows = new List<FollowInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int followsSize = m_follows.Count;
            for (int i = 0; i < followsSize; i++)
            {
                if (m_follows[i].m_ID == id)
                {
                    m_follows.RemoveAt(i);
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
            int followsSize = m_follows.Count;
            for (int i = 0; i < followsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_follows[i].m_ID));
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
        public FollowInfo GetFollow(String id)
        {
            int followsSize = m_follows.Count;
            for (int i = 0; i < followsSize; i++)
            {
                if (m_follows[i].m_ID == id)
                {
                    return m_follows[i];
                }
            }
            return new FollowInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "FOLLOW";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_follows));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="master">信息</param>
        public void Save(FollowInfo follow)
        {
            bool modify = false;
            int followsSize = m_follows.Count;
            for (int i = 0; i < followsSize; i++)
            {
                if (m_follows[i].m_ID == follow.m_ID)
                {
                    m_follows[i] = follow;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_follows.Add(follow);
            }
            Save();
        }
    }
}
