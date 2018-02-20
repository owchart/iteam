/**************************************************************************************\
*                                                                                      *
* CounterspyService.cs -  Counterspy service functions, types, and definitions.       *
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
    /// ��������¼
    /// </summary>
    public class CounterspyInfo : BaseInfo
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
    /// ��С�����¼����
    /// </summary>
    public class CounterspyService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public CounterspyService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("COUNTERSPY", ref cookie) > 0)
            {
                try
                {
                    m_counterspys = JsonConvert.DeserializeObject<List<CounterspyInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_counterspys == null)
                {
                    try
                    {
                        m_counterspys = JsonConvert.DeserializeObject<List<CounterspyInfo>>(cookie.m_value);
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
        public List<CounterspyInfo> m_counterspys = new List<CounterspyInfo>();

        /// <summary>
        /// ɾ����Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int counterspysSize = m_counterspys.Count;
            for (int i = 0; i < counterspysSize; i++)
            {
                if (m_counterspys[i].m_ID == id)
                {
                    m_counterspys.RemoveAt(i);
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
            int counterspysSize = m_counterspys.Count;
            for (int i = 0; i < counterspysSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_counterspys[i].m_ID));
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
        public CounterspyInfo GetCounterspy(String id)
        {
            int counterspysSize = m_counterspys.Count;
            for (int i = 0; i < counterspysSize; i++)
            {
                if (m_counterspys[i].m_ID == id)
                {
                    return m_counterspys[i];
                }
            }
            return new CounterspyInfo();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "COUNTERSPY";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_counterspys));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="master">��Ϣ</param>
        public void Save(CounterspyInfo counterspy)
        {
            bool modify = false;
            int counterspysSize = m_counterspys.Count;
            for (int i = 0; i < counterspysSize; i++)
            {
                if (m_counterspys[i].m_ID == counterspy.m_ID)
                {
                    m_counterspys[i] = counterspy;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_counterspys.Add(counterspy);
            }
            Save();
        }
    }
}
