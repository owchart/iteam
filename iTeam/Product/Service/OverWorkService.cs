/**************************************************************************************\
*                                                                                      *
* OverWorkService.cs -  Over work service functions, types, and definitions.       *
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
    /// �Ӱ��¼
    /// </summary>
    public class OverWorkInfo : BaseInfo
    {
        /// <summary>
        /// ����
        /// </summary>
        public String m_content = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_jobID = "";
    }

    /// <summary>
    /// �Ӱ��¼����
    /// </summary>
    public class OverWorkService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public OverWorkService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("OVERWORK", ref cookie) > 0)
            {
                try
                {
                    m_overWorks = JsonConvert.DeserializeObject<List<OverWorkInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_overWorks == null)
                {
                    try
                    {
                        m_overWorks = JsonConvert.DeserializeObject<List<OverWorkInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// �Ӱ���Ϣ
        /// </summary>
        public List<OverWorkInfo> m_overWorks = new List<OverWorkInfo>();

        /// <summary>
        /// ɾ����Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int overWorksSize = m_overWorks.Count;
            for (int i = 0; i < overWorksSize; i++)
            {
                if (m_overWorks[i].m_ID == id)
                {
                    m_overWorks.RemoveAt(i);
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
            int overWorksSize = m_overWorks.Count;
            for (int i = 0; i < overWorksSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_overWorks[i].m_ID));
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
        public OverWorkInfo GetOverWork(String id)
        {
            int overWorksSize = m_overWorks.Count;
            for (int i = 0; i < overWorksSize; i++)
            {
                if (m_overWorks[i].m_ID == id)
                {
                    return m_overWorks[i];
                }
            }
            return new OverWorkInfo();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "OVERWORK";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_overWorks));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="overWork">��Ϣ</param>
        public void Save(OverWorkInfo overWork)
        {
            bool modify = false;
            int overWorksSize = m_overWorks.Count;
            for (int i = 0; i < overWorksSize; i++)
            {
                if (m_overWorks[i].m_ID == overWork.m_ID)
                {
                    m_overWorks[i] = overWork;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_overWorks.Add(overWork);
            }
            Save();
        }
    }
}
