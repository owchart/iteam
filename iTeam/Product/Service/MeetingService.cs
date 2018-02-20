/**************************************************************************************\
*                                                                                      *
* MettingService.cs -  Over work service functions, types, and definitions.       *
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
    /// �����¼
    /// </summary>
    public class MeetingInfo : BaseInfo
    {
        /// <summary>
        /// ����
        /// </summary>
        public String m_content = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_jobID = "";
    }

    /// <summary>
    /// �Ӱ��¼����
    /// </summary>
    public class MettingService
    {
        /// <summary>
        /// ��������
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
        /// ������Ϣ
        /// </summary>
        public List<MeetingInfo> m_meetings = new List<MeetingInfo>();

        /// <summary>
        /// ɾ����Ϣ
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
        /// ��ȡ�µ�ID
        /// </summary>
        /// <returns>��ID</returns>
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
        /// ��ȡ��Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>��Ϣ</returns>
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
        /// ������Ϣ
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
        /// ������Ϣ
        /// </summary>
        /// <param name="overWork">��Ϣ</param>
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
