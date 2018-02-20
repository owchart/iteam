/**************************************************************************************\
*                                                                                      *
* CalendarService.cs -  Calendar service functions, types, and definitions.       *
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
    /// ������Ϣ
    /// </summary>
    public class CalendarInfo
    {
        /// <summary>
        /// �¼�
        /// </summary>
        public List<EventInfo> m_events = new List<EventInfo>();
    }

    public class EventInfo
    {
        /// <summary>
        /// ����
        /// </summary>
        public String m_content = "";

        /// <summary>
        /// ��������
        /// </summary>
        public String m_createDate = "";

        /// <summary>
        /// �¼�ID
        /// </summary>
        public String m_eventID = "";

        /// <summary>
        /// ʱ��
        /// </summary>
        public String m_time = "";

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
    /// ��������
    /// </summary>
    public class CalendarService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public CalendarService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("CALENDAR", ref cookie) > 0)
            {
                try
                {
                    m_calendars = JsonConvert.DeserializeObject<Dictionary<String, CalendarInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_calendars == null)
                {
                    try
                    {
                        m_calendars = JsonConvert.DeserializeObject<Dictionary<String, CalendarInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// ��Ϣ
        /// </summary>
        public Dictionary<String, CalendarInfo> m_calendars = new Dictionary<String, CalendarInfo>();

        /// <summary>
        /// ����¼�
        /// </summary>
        /// <param name="date">����</param>
        /// <param name="eventInfo">�¼�</param>
        public void AddEvent(String date, EventInfo eventInfo)
        {
            if (!m_calendars.ContainsKey(date))
            {
                m_calendars[date] = new CalendarInfo();
            }
            eventInfo.m_eventID = System.Guid.NewGuid().ToString();
            m_calendars[date].m_events.Add(eventInfo);
            Save();
        }

        /// <summary>
        /// ��ȡ�¼�
        /// </summary>
        /// <param name="date">����</param>
        /// <param name="eventID">�¼�ID</param>
        /// <returns>�¼���Ϣ</returns>
        public EventInfo GetEvent(String date, String eventID)
        {
            if (m_calendars.ContainsKey(date))
            {
                List<EventInfo> events = m_calendars[date].m_events;
                int eventsSize = events.Count;
                for (int i = 0; i < eventsSize; i++)
                {
                    EventInfo iEvent = events[i];
                    if (iEvent.m_eventID == eventID)
                    {
                        return iEvent;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ��ȡ�¼�
        /// </summary>
        /// <param name="date">����</param>
        /// <returns>�¼��б�</returns>
        public List<EventInfo> GetEvents(String date)
        {
            if (m_calendars.ContainsKey(date))
            {
                return m_calendars[date].m_events;
            }
            else
            {
                return new List<EventInfo>();
            }
        }

        /// <summary>
        /// �޸��¼�
        /// </summary>
        /// <param name="date">����</param>
        /// <param name="eventInfo">�¼�</param>
        public void ModifyEvent(String date, EventInfo eventInfo)
        {
            if (m_calendars.ContainsKey(date))
            {
                List<EventInfo> events = m_calendars[date].m_events;
                int eventsSize = events.Count;
                for (int i = 0; i < eventsSize; i++)
                {
                    EventInfo iEvent = events[i];
                    if (iEvent.m_eventID == eventInfo.m_eventID)
                    {
                        events[i] = eventInfo;
                        break;
                    }
                }
            }
            Save();
        }

        /// <summary>
        /// �Ƴ��¼�
        /// </summary>
        /// <param name="date">����</param>
        /// <param name="eventID">�¼�ID</param>
        public void RemoveEvent(String date, String eventID)
        {
            if (m_calendars.ContainsKey(date))
            {
                List<EventInfo> events = m_calendars[date].m_events;
                foreach (EventInfo eventInfo in events)
                {
                    if (eventInfo.m_eventID == eventID)
                    {
                        events.Remove(eventInfo);
                        break;
                    }
                }
            }
            Save();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "CALENDAR";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_calendars));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }
    }
}
