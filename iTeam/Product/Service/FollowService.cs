/**************************************************************************************\
*                                                                                      *
* FollowService.cs -  Follow service functions, types, and definitions.       *
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
    /// ��ע��Ϣ
    /// </summary>
    public class FollowInfo : BaseInfo
    {
        /// <summary>
        /// ����
        /// </summary>
        public String m_content = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_level = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_title = "";
    }

    /// <summary>
    /// �ص��ע����
    /// </summary>
    public class FollowService
    {
        /// <summary>
        /// ��������
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
        /// ָʾ��Ϣ
        /// </summary>
        public List<FollowInfo> m_follows = new List<FollowInfo>();

        /// <summary>
        /// ɾ����Ϣ
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
        /// ��ȡ�µ�ID
        /// </summary>
        /// <returns>��ID</returns>
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
        /// ��ȡ��Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>��Ϣ</returns>
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
        /// ������Ϣ
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
        /// ������Ϣ
        /// </summary>
        /// <param name="master">��Ϣ</param>
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
