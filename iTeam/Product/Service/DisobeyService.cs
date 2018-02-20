/**************************************************************************************\
*                                                                                      *
* DisobeyService.cs -  Disobey service functions, types, and definitions.       *
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
    public class DisobeyInfo : BaseInfo
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
    /// ������¼����
    /// </summary>
    public class DisobeyService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public DisobeyService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("DISOBEY", ref cookie) > 0)
            {
                try
                {
                    m_disobeys = JsonConvert.DeserializeObject<List<DisobeyInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_disobeys == null)
                {
                    try
                    {
                        m_disobeys = JsonConvert.DeserializeObject<List<DisobeyInfo>>(cookie.m_value);
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
        public List<DisobeyInfo> m_disobeys = new List<DisobeyInfo>();

        /// <summary>
        /// ɾ����Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int disobeysSize = m_disobeys.Count;
            for (int i = 0; i < disobeysSize; i++)
            {
                if (m_disobeys[i].m_ID == id)
                {
                    m_disobeys.RemoveAt(i);
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
            int disobeysSize = m_disobeys.Count;
            for (int i = 0; i < disobeysSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_disobeys[i].m_ID));
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
        public DisobeyInfo GetDisobey(String id)
        {
            int disobeysSize = m_disobeys.Count;
            for (int i = 0; i < disobeysSize; i++)
            {
                if (m_disobeys[i].m_ID == id)
                {
                    return m_disobeys[i];
                }
            }
            return new DisobeyInfo();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "DISOBEY";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_disobeys));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="disobey">��Ϣ</param>
        public void Save(DisobeyInfo disobey)
        {
            bool modify = false;
            int disobeysSize = m_disobeys.Count;
            for (int i = 0; i < disobeysSize; i++)
            {
                if (m_disobeys[i].m_ID == disobey.m_ID)
                {
                    m_disobeys[i] = disobey;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_disobeys.Add(disobey);
            }
            Save();
        }
    }
}
