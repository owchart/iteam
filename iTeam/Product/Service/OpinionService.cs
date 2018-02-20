/**************************************************************************************\
*                                                                                      *
* OpinionService.cs -  Opinion service functions, types, and definitions.       *
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
    /// �����Ϣ
    /// </summary>
    public class OpinionInfo : BaseInfo
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
    /// ��������
    /// </summary>
    public class OpinionService
    {
        /// <summary>
        /// ��������
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
        /// �����Ϣ
        /// </summary>
        public List<OpinionInfo> m_opinions = new List<OpinionInfo>();

        /// <summary>
        /// ɾ����Ϣ
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
        /// ��ȡ�µ�ID
        /// </summary>
        /// <returns>��ID</returns>
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
        /// ��ȡ��Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>��Ϣ</returns>
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
        /// ������Ϣ
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
        /// ������Ϣ
        /// </summary>
        /// <param name="opinion">��Ϣ</param>
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
