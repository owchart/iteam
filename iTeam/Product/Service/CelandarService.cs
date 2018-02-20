/**************************************************************************************\
*                                                                                      *
* CalendarService.cs -  Calendar service functions, types, and definitions.       *
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
    /// 日历信息
    /// </summary>
    public class CalendarInfo
    {
        /// <summary>
        /// 事件
        /// </summary>
        public List<EventInfo> m_events = new List<EventInfo>();
    }

    public class EventInfo
    {
        /// <summary>
        /// 内容
        /// </summary>
        public String m_content = "";

        /// <summary>
        /// 创建日期
        /// </summary>
        public String m_createDate = "";

        /// <summary>
        /// 事件ID
        /// </summary>
        public String m_eventID = "";

        /// <summary>
        /// 时间
        /// </summary>
        public String m_time = "";

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
    /// 日历服务
    /// </summary>
    public class CalendarService
    {
        /// <summary>
        /// 创建服务
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
        /// 信息
        /// </summary>
        public Dictionary<String, CalendarInfo> m_calendars = new Dictionary<String, CalendarInfo>();

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="eventInfo">事件</param>
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
        /// 获取事件
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="eventID">事件ID</param>
        /// <returns>事件信息</returns>
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
        /// 获取事件
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>事件列表</returns>
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
        /// 修改事件
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="eventInfo">事件</param>
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
        /// 移除事件
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="eventID">事件ID</param>
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
        /// 保存信息
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
