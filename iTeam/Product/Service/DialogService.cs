/**************************************************************************************\
*                                                                                      *
* LevelService.cs -  Level service functions, types, and definitions.       *
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
    /// ˮƽ��Ϣ
    /// </summary>
    public class DialogInfo : BaseInfo
    {
        /// <summary>
        /// ����
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_date;
    }

    /// <summary>
    /// ˮƽ����
    /// </summary>
    public class DialogService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public DialogService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("DIALOG", ref cookie) > 0)
            {
                try
                {
                    m_dialogs = JsonConvert.DeserializeObject<List<DialogInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_dialogs == null)
                {
                    try
                    {
                        m_dialogs = JsonConvert.DeserializeObject<List<DialogInfo>>(cookie.m_value);
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
        public List<DialogInfo> m_dialogs = new List<DialogInfo>();

        /// <summary>
        /// ɾ����Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int dialogsSize = m_dialogs.Count;
            for (int i = 0; i < dialogsSize; i++)
            {
                if (m_dialogs[i].m_ID == id)
                {
                    m_dialogs.RemoveAt(i);
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
            int dialogsSize = m_dialogs.Count;
            for (int i = 0; i < dialogsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_dialogs[i].m_ID));
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
        public DialogInfo GetDialog(String id)
        {
            int dialogsSize = m_dialogs.Count;
            for (int i = 0; i < dialogsSize; i++)
            {
                if (m_dialogs[i].m_ID == id)
                {
                    return m_dialogs[i];
                }
            }
            return new DialogInfo();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "DIALOG";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_dialogs));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="award">��Ϣ</param>
        public void Save(DialogInfo level)
        {
            bool modify = false;
            int dialogsSize = m_dialogs.Count;
            for (int i = 0; i < dialogsSize; i++)
            {
                if (m_dialogs[i].m_ID == level.m_ID)
                {
                    m_dialogs[i] = level;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_dialogs.Add(level);
            }
            Save();
        }
    }
}
