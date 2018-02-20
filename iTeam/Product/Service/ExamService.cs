/**************************************************************************************\
*                                                                                      *
* ReportService.cs -  Report service functions, types, and definitions.       *
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
    public class ExamInfo : BaseInfo
    {
        /// <summary>
        /// ��
        /// </summary>
        public String m_answer = "";

        /// <summary>
        /// ��ʱ
        /// </summary>
        public String m_interval = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_title = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_type = "����";
    }

    /// <summary>
    /// ���Է���
    /// </summary>
    public class ExamService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public ExamService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("EXAM", ref cookie) > 0)
            {
                try
                {
                    m_exams = JsonConvert.DeserializeObject<List<ExamInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_exams == null)
                {
                    try
                    {
                        m_exams = JsonConvert.DeserializeObject<List<ExamInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// ָʾ��Ϣ
        /// </summary>
        public List<ExamInfo> m_exams = new List<ExamInfo>();

        /// <summary>
        /// ɾ����Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int examsSize = m_exams.Count;
            for (int i = 0; i < examsSize; i++)
            {
                if (m_exams[i].m_ID == id)
                {
                    m_exams.RemoveAt(i);
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
            int reportsSize = m_exams.Count;
            for (int i = 0; i < reportsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_exams[i].m_ID));
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
        public ExamInfo GetExam(String id)
        {
            int examsSize = m_exams.Count;
            for (int i = 0; i < examsSize; i++)
            {
                if (m_exams[i].m_ID == id)
                {
                    return m_exams[i];
                }
            }
            return new ExamInfo();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "EXAM";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_exams));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="master">��Ϣ</param>
        public void Save(ExamInfo exam)
        {
            bool modify = false;
            int examsSize = m_exams.Count;
            for (int i = 0; i < examsSize; i++)
            {
                if (m_exams[i].m_ID == exam.m_ID)
                {
                    m_exams[i] = exam;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_exams.Add(exam);
            }
            Save();
        }
    }
}
