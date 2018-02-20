/**************************************************************************************\
*                                                                                      *
* ClueService.cs -  Clue service functions, types, and definitions.       *
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
    public class ClueInfo : BaseInfo
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
    public class ClueService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public ClueService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("CLUE", ref cookie) > 0)
            {
                try
                {
                    m_clues = JsonConvert.DeserializeObject<List<ClueInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_clues == null)
                {
                    try
                    {
                        m_clues = JsonConvert.DeserializeObject<List<ClueInfo>>(cookie.m_value);
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
        public List<ClueInfo> m_clues = new List<ClueInfo>();

        /// <summary>
        /// ɾ����Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int cluesSize = m_clues.Count;
            for (int i = 0; i < cluesSize; i++)
            {
                if (m_clues[i].m_ID == id)
                {
                    m_clues.RemoveAt(i);
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
            int cluesSize = m_clues.Count;
            for (int i = 0; i < cluesSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_clues[i].m_ID));
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
        public ClueInfo GetClue(String id)
        {
            int cluesSize = m_clues.Count;
            for (int i = 0; i < cluesSize; i++)
            {
                if (m_clues[i].m_ID == id)
                {
                    return m_clues[i];
                }
            }
            return new ClueInfo();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "CLUE";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_clues));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="master">��Ϣ</param>
        public void Save(ClueInfo clue)
        {
            bool modify = false;
            int cluesSize = m_clues.Count;
            for (int i = 0; i < cluesSize; i++)
            {
                if (m_clues[i].m_ID == clue.m_ID)
                {
                    m_clues[i] = clue;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_clues.Add(clue);
            }
            Save();
        }
    }
}
