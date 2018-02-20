/**************************************************************************************\
*                                                                                      *
* RemoteService.cs -  Remote service functions, types, and definitions.       *
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
    public class RemoteInfo : BaseInfo
    {
        /// <summary>
        /// IP��ַ
        /// </summary>
        public String m_IP = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_password = "";

        /// <summary>
        /// �˿�
        /// </summary>
        public String m_port = "";

        /// <summary>
        /// ��ע
        /// </summary>
        public String m_remarks = "";

        /// <summary>
        /// �û���
        /// </summary>
        public String m_userName = "";
    }

    /// <summary>
    /// Զ�̷���ķ���
    /// </summary>
    public class RemoteService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public RemoteService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("REMOTE", ref cookie) > 0)
            {
                try
                {
                    m_remotes = JsonConvert.DeserializeObject<List<RemoteInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_remotes == null)
                {
                    try
                    {
                        m_remotes = JsonConvert.DeserializeObject<List<RemoteInfo>>(cookie.m_value);
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
        public List<RemoteInfo> m_remotes = new List<RemoteInfo>();

        /// <summary>
        /// ɾ����Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int remotesSize = m_remotes.Count;
            for (int i = 0; i < remotesSize; i++)
            {
                if (m_remotes[i].m_ID == id)
                {
                    m_remotes.RemoveAt(i);
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
            int remotesSize = m_remotes.Count;
            for (int i = 0; i < remotesSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_remotes[i].m_ID));
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
        public RemoteInfo GetRemote(String id)
        {
            int remotesSize = m_remotes.Count;
            for (int i = 0; i < remotesSize; i++)
            {
                if (m_remotes[i].m_ID == id)
                {
                    return m_remotes[i];
                }
            }
            return new RemoteInfo();
        }


        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "REMOTE";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_remotes));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="server">��Ϣ</param>
        public void Save(RemoteInfo remote)
        {
            bool modify = false;
            int remotesSize = m_remotes.Count;
            for (int i = 0; i < remotesSize; i++)
            {
                if (m_remotes[i].m_ID == remote.m_ID)
                {
                    m_remotes[i] = remote;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_remotes.Add(remote);
            }
            Save();
        }
    }
}
