/**************************************************************************************\
*                                                                                      *
* AwardService.cs -  Award service functions, types, and definitions.       *
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
    /// �ν���¼
    /// </summary>
    public class AwardInfo : BaseInfo
    {
        /// <summary>
        /// ����
        /// </summary>
        public String m_content = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_jobID = "";

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
    /// �ν���¼����
    /// </summary>
    public class AwardService
    {
        /// <summary>
        /// ��������
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
        /// �ν���Ϣ
        /// </summary>
        public List<AwardInfo> m_awards = new List<AwardInfo>();

        /// <summary>
        /// ɾ����Ϣ
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
        /// ��ȡ�µ�ID
        /// </summary>
        /// <returns>��ID</returns>
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
        /// ��ȡ�ν���Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>�ν���Ϣ</returns>
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
        /// ������Ϣ
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
        /// ������Ϣ
        /// </summary>
        /// <param name="award">�ν���Ϣ</param>
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
