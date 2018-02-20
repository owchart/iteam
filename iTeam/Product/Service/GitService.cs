/**************************************************************************************\
*                                                                                      *
* GitService.cs -  Git service functions, types, and definitions.       *
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
    /// Git��Ϣ
    /// </summary>
    public class GitInfo : BaseInfo
    {
        /// <summary>
        /// ����
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// ��ַ
        /// </summary>
        public String m_url = "";
    }

    /// <summary>
    /// Git����
    /// </summary>
    public class GitService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public GitService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("GIT", ref cookie) > 0)
            {
                try
                {
                    m_gits = JsonConvert.DeserializeObject<List<GitInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_gits == null)
                {
                    try
                    {
                        m_gits = JsonConvert.DeserializeObject<List<GitInfo>>(cookie.m_value);
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
        public List<GitInfo> m_gits = new List<GitInfo>();

        /// <summary>
        /// ɾ����Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int gitsSize = m_gits.Count;
            for (int i = 0; i < gitsSize; i++)
            {
                if (m_gits[i].m_ID == id)
                {
                    m_gits.RemoveAt(i);
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
            int gitsSize = m_gits.Count;
            for (int i = 0; i < gitsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_gits[i].m_ID));
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
        public GitInfo GerGit(String id)
        {
            int gitsSize = m_gits.Count;
            for (int i = 0; i < gitsSize; i++)
            {
                if (m_gits[i].m_ID == id)
                {
                    return m_gits[i];
                }
            }
            return new GitInfo();
        }


        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "GIT";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_gits));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="server">��Ϣ</param>
        public void Save(GitInfo git)
        {
            bool modify = false;
            int gitsSize = m_gits.Count;
            for (int i = 0; i < gitsSize; i++)
            {
                if (m_gits[i].m_ID == git.m_ID)
                {
                    m_gits[i] = git;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_gits.Add(git);
            }
            Save();
        }
    }
}
