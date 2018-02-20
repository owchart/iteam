/**************************************************************************************\
*                                                                                      *
* SpeechService.cs -  Speech service functions, types, and definitions.       *
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
    /// �ݽ���Ϣ
    /// </summary>
    public class SpeechInfo : BaseInfo
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
    /// �ݽ�����
    /// </summary>
    public class SpeechService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public SpeechService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("SPEECH", ref cookie) > 0)
            {
                try
                {
                    m_speeches = JsonConvert.DeserializeObject<List<SpeechInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_speeches == null)
                {
                    try
                    {
                        m_speeches = JsonConvert.DeserializeObject<List<SpeechInfo>>(cookie.m_value);
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
        public List<SpeechInfo> m_speeches = new List<SpeechInfo>();

        /// <summary>
        /// ɾ����Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int speechesSize = m_speeches.Count;
            for (int i = 0; i < speechesSize; i++)
            {
                if (m_speeches[i].m_ID == id)
                {
                    m_speeches.RemoveAt(i);
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
            int speechesSize = m_speeches.Count;
            for (int i = 0; i < speechesSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_speeches[i].m_ID));
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
        public SpeechInfo GetSpeech(String id)
        {
            int speechesSize = m_speeches.Count;
            for (int i = 0; i < speechesSize; i++)
            {
                if (m_speeches[i].m_ID == id)
                {
                    return m_speeches[i];
                }
            }
            return new SpeechInfo();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "SPEECH";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_speeches));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="speech">��Ϣ</param>
        public void Save(SpeechInfo speech)
        {
            bool modify = false;
            int speechesSize = m_speeches.Count;
            for (int i = 0; i < speechesSize; i++)
            {
                if (m_speeches[i].m_ID == speech.m_ID)
                {
                    m_speeches[i] = speech;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_speeches.Add(speech);
            }
            Save();
        }
    }
}
