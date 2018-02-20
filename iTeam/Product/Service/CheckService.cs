/**************************************************************************************\
*                                                                                      *
* CheckService.cs -  Check service functions, types, and definitions.       *
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
    /// ���˼���¼
    /// </summary>
    public class CheckInfo : BaseInfo
    {
        /// <summary>
        /// �ʴ�
        /// </summary>
        public String m_answer = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_jobID = "";
    }

    /// <summary>
    /// ���˼�����
    /// </summary>
    public class CheckService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public CheckService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("CHECKCODE", ref cookie) > 0)
            {
                try
                {
                    m_checks = JsonConvert.DeserializeObject<List<CheckInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_checks == null)
                {
                    try
                    {
                        m_checks = JsonConvert.DeserializeObject<List<CheckInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// ���˼����Ϣ
        /// </summary>
        public List<CheckInfo> m_checks = new List<CheckInfo>();

        /// <summary>
        /// ɾ����Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int checkSize = m_checks.Count;
            for (int i = 0; i < checkSize; i++)
            {
                if (m_checks[i].m_ID == id)
                {
                    m_checks.RemoveAt(i);
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
            int checkSize = m_checks.Count;
            for (int i = 0; i < checkSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_checks[i].m_ID));
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
        public CheckInfo GetCheck(String id)
        {
            int checkSize = m_checks.Count;
            for (int i = 0; i < checkSize; i++)
            {
                if (m_checks[i].m_ID == id)
                {
                    return m_checks[i];
                }
            }
            return new CheckInfo();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "CHECKCODE";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_checks));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="award">��Ϣ</param>
        public void Save(CheckInfo checkInfo)
        {
            bool modify = false;
            int checkSize = m_checks.Count;
            for (int i = 0; i < checkSize; i++)
            {
                if (m_checks[i].m_ID == checkInfo.m_ID)
                {
                    m_checks[i] = checkInfo;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_checks.Add(checkInfo);
            }
            Save();
        }
    }
}
