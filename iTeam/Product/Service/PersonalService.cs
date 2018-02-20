/**************************************************************************************\
*                                                                                      *
* PersonalService.cs -  Personal service functions, types, and definitions.       *
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
    public class PersonalInfo
    {
        /// <summary>
        /// ����������Ϣ
        /// </summary>
        public PersonalInfo()
        {
        }

        /// <summary>
        /// ����״��
        /// </summary>
        public String m_body = "";

        /// <summary>
        /// ���
        /// </summary>
        public String m_cross = "";

        /// <summary>
        /// ���´��
        /// </summary>
        public String m_dress = "";

        /// <summary>
        /// �Ⱦ�
        /// </summary>
        public String m_drink = "";

        /// <summary>
        /// ��ʳϰ��
        /// </summary>
        public String m_eat = "";

        /// <summary>
        /// ��Ϸ
        /// </summary>
        public String m_game = "";

        /// <summary>
        /// ͬ����
        /// </summary>
        public String m_gay = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_house = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_jobID = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_loan = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_lead = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_marry = "";

        /// <summary>
        /// ������������
        /// </summary>
        public String m_oldWork = "";

        /// <summary>
        /// ��ĸ
        /// </summary>
        public String m_parent = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_pay = "";

        /// <summary>
        /// ƫ������
        /// </summary>
        public String m_preference = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_pet = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_sb = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_smoke = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_sports = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_teach = "";
    }

    /// <summary>
    /// ������Ϣ����
    /// </summary>
    public class PersonalService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public PersonalService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("PERSONAL", ref cookie) > 0)
            {
                try
                {
                    m_personals = JsonConvert.DeserializeObject<List<PersonalInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_personals == null)
                {
                    try
                    {
                        m_personals = JsonConvert.DeserializeObject<List<PersonalInfo>>(cookie.m_value);
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
        public List<PersonalInfo> m_personals = new List<PersonalInfo>();

        /// <summary>
        /// ɾ��������Ϣ
        /// </summary>
        /// <param name="jobID">Ա��ID</param>
        public void Delete(String jobID)
        {
            int personalsSize = m_personals.Count;
            for (int i = 0; i < personalsSize; i++)
            {
                if (m_personals[i].m_jobID == jobID)
                {
                    m_personals.RemoveAt(i);
                    Save();
                    break;
                }
            }
        }

        /// <summary>
        /// ��ȡ�µ�Ա��ID
        /// </summary>
        /// <returns>Ա��ID</returns>
        public String GetNewJobID()
        {
            List<int> ids = new List<int>();
            int personalsSize = m_personals.Count;
            for (int i = 0; i < personalsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_personals[i].m_jobID));
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
        /// ��ȡ������Ϣ
        /// </summary>
        /// <param name="jobID">����</param>
        /// <returns>������Ϣ</returns>
        public PersonalInfo GerPersonal(String jobID)
        {
            int personalsSize = m_personals.Count;
            for (int i = 0; i < personalsSize; i++)
            {
                if (m_personals[i].m_jobID == jobID)
                {
                    return m_personals[i];
                }
            }
            return new PersonalInfo();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "PERSONAL";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_personals));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="personal">������Ϣ</param>
        public void Save(PersonalInfo personal)
        {
            bool modify = false;
            int personalsSize = m_personals.Count;
            for (int i = 0; i < personalsSize; i++)
            {
                if (m_personals[i].m_jobID == personal.m_jobID)
                {
                    m_personals[i] = personal;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_personals.Add(personal);
            }
            Save();
        }
    }
}
