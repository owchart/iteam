/**************************************************************************************\
*                                                                                      *
* StaffService.cs -  Staff service functions, types, and definitions.       *
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
    /// ��Ա��Ϣ
    /// </summary>
    public class StaffInfo
    {
        /// <summary>
        /// ������Ϣ
        /// </summary>
        public StaffInfo()
        {
        }

        /// <summary>
        /// ����Ա����Ϣ
        /// </summary>
        /// <param name="birthDay">����</param>
        /// <param name="degree">ѧλ</param>
        /// <param name="education">ѧ��</param>
        /// <param name="entryDay">��ְʱ��</param>
        /// <param name="jobID">����</param>
        /// <param name="name">����</param>
        /// <param name="sex">�Ա�</param>
        /// <param name="state">Ŀǰ״̬</param>
        public StaffInfo(String birthDay, String degree, String education, String entryDay, String jobID, String name, String sex, String state)
        {
            m_birthDay = birthDay;
            m_degree = degree;
            m_education = education;
            m_entryDay = entryDay;
            m_jobID = jobID;
            m_name = name;
            m_sex = sex;
            m_state = state;
        }

        /// <summary>
        /// ����
        /// </summary>
        public String m_birthDay = "";

        /// <summary>
        /// �Ƿ�����ѡ��
        /// </summary>
        public String m_canSelect = "��";

        /// <summary>
        /// ѧλ
        /// </summary>
        public String m_degree = "";

        /// <summary>
        /// ѧ��
        /// </summary>
        public String m_education = "";

        /// <summary>
        /// ��������
        /// </summary>
        public String m_entryDay = "";

        /// <summary>
        /// �Ƿ������
        /// </summary>
        public String m_isManager = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_jobID = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// �Ա�
        /// </summary>
        public String m_sex = "";

        /// <summary>
        /// Ŀǰ״̬
        /// </summary>
        public String m_state = "";
    }

    /// <summary>
    /// Ա����Ϣ����
    /// </summary>
    public class StaffService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public StaffService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("STAFF", ref cookie) > 0)
            {
                try
                {
                    m_staffs = JsonConvert.DeserializeObject<List<StaffInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_staffs == null)
                {
                    try
                    {
                        m_staffs = JsonConvert.DeserializeObject<List<StaffInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Ա����Ϣ
        /// </summary>
        public List<StaffInfo> m_staffs = new List<StaffInfo>();

        /// <summary>
        /// ɾ��Ա����Ϣ
        /// </summary>
        /// <param name="jobID">Ա��ID</param>
        public void Delete(String jobID)
        {
            int staffsSize = m_staffs.Count;
            for (int i = 0; i < staffsSize; i++)
            {
                if (m_staffs[i].m_jobID == jobID)
                {
                    m_staffs.RemoveAt(i);
                    Save();
                    break;
                }
            }
        }

        /// <summary>
        /// ��ȡ��ѡ���Ա��
        /// </summary>
        /// <returns>��ѡ��Ա��</returns>
        public List<StaffInfo> GetAliveStaffs()
        {
            List<StaffInfo> staffs = new List<StaffInfo>();
            int staffsSize = m_staffs.Count;
            for (int i = 0; i < staffsSize; i++)
            {
                if (m_staffs[i].m_canSelect == "��")
                {
                    staffs.Add(m_staffs[i]);
                }
            }
            return staffs;
        }

        /// <summary>
        /// ��ȡ�µ�Ա��ID
        /// </summary>
        /// <returns>Ա��ID</returns>
        public String GetNewJobID()
        {
            List<int> ids = new List<int>();
            int staffsSize = m_staffs.Count;
            for (int i = 0; i < staffsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_staffs[i].m_jobID));
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
        /// ���ݹ��Ż�ȡ����
        /// </summary>
        /// <param name="jobIDs">����</param>
        /// <returns>����</returns>
        public String GetNamesByJobsID(String jobIDs)
        {
            List<String> names = new List<String>();
            String[] strs = jobIDs.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int strsSize = strs.Length;
            for (int i = 0; i < strsSize; i++)
            {
                StaffInfo staff = GetStaff(strs[i]);
                if (staff.m_jobID != null && staff.m_jobID.Length > 0)
                {
                    names.Add(staff.m_name);
                }
            }
            int namesSize = names.Count;
            String strNames = "";
            for (int i = 0; i < namesSize; i++)
            {
                strNames += names[i];
                if (i != namesSize - 1)
                {
                    strNames += ",";
                }
            }
            return strNames;
        }

        /// <summary>
        /// ��ȡԱ����Ϣ
        /// </summary>
        /// <param name="jobID">����</param>
        /// <returns>Ա����Ϣ</returns>
        public StaffInfo GetStaff(String jobID)
        {
            int staffsSize = m_staffs.Count;
            for (int i = 0; i < staffsSize; i++)
            {
                if (m_staffs[i].m_jobID == jobID)
                {
                    return m_staffs[i];
                }
            }
            return new StaffInfo();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "STAFF";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_staffs));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="staff">Ա����Ϣ</param>
        public void Save(StaffInfo staff)
        {
            bool modify = false;
            int staffsSize = m_staffs.Count;
            for (int i = 0; i < staffsSize; i++)
            {
                if (m_staffs[i].m_jobID == staff.m_jobID)
                {
                    m_staffs[i] = staff;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_staffs.Add(staff);
            }
            Save();
        }
    }
}
