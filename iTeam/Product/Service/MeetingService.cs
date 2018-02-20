/**************************************************************************************\
*                                                                                      *
* MettingService.cs -  Over work service functions, types, and definitions.       *
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
    /// 会议记录
    /// </summary>
    public class MeetingInfo : BaseInfo
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
    public class MettingService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public MettingService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("METTING", ref cookie) > 0)
            {
                try
                {
                    m_meetings = JsonConvert.DeserializeObject<List<MeetingInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_meetings == null)
                {
                    try
                    {
                        m_meetings = JsonConvert.DeserializeObject<List<MeetingInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 会议信息
        /// </summary>
        public List<MeetingInfo> m_meetings = new List<MeetingInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int mettingsSize = m_meetings.Count;
            for (int i = 0; i < mettingsSize; i++)
            {
                if (m_meetings[i].m_ID == id)
                {
                    m_meetings.RemoveAt(i);
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
            int mettingsSize = m_meetings.Count;
            for (int i = 0; i < mettingsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_meetings[i].m_ID));
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
        public MeetingInfo GetMeeting(String id)
        {
            int mettingsSize = m_meetings.Count;
            for (int i = 0; i < mettingsSize; i++)
            {
                if (m_meetings[i].m_ID == id)
                {
                    return m_meetings[i];
                }
            }
            return new MeetingInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "METTING";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_meetings));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="overWork">信息</param>
        public void Save(MeetingInfo meeting)
        {
            bool modify = false;
            int mettingsSize = m_meetings.Count;
            for (int i = 0; i < mettingsSize; i++)
            {
                if (m_meetings[i].m_ID == meeting.m_ID)
                {
                    m_meetings[i] = meeting;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_meetings.Add(meeting);
            }
            Save();
        }
    }
}
