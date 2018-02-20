/**************************************************************************************\
*                                                                                      *
* StaffService.cs -  Staff service functions, types, and definitions.       *
*                                                                                      *
*               Version 1.00 ★                                                        *
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
    /// 成员信息
    /// </summary>
    public class StaffInfo
    {
        /// <summary>
        /// 创建信息
        /// </summary>
        public StaffInfo()
        {
        }

        /// <summary>
        /// 创建员工信息
        /// </summary>
        /// <param name="birthDay">生日</param>
        /// <param name="degree">学位</param>
        /// <param name="education">学历</param>
        /// <param name="entryDay">入职时间</param>
        /// <param name="jobID">工号</param>
        /// <param name="name">姓名</param>
        /// <param name="sex">性别</param>
        /// <param name="state">目前状态</param>
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
        /// 生日
        /// </summary>
        public String m_birthDay = "";

        /// <summary>
        /// 是否允许选拔
        /// </summary>
        public String m_canSelect = "是";

        /// <summary>
        /// 学位
        /// </summary>
        public String m_degree = "";

        /// <summary>
        /// 学历
        /// </summary>
        public String m_education = "";

        /// <summary>
        /// 进入日期
        /// </summary>
        public String m_entryDay = "";

        /// <summary>
        /// 是否管理者
        /// </summary>
        public String m_isManager = "";

        /// <summary>
        /// 工号
        /// </summary>
        public String m_jobID = "";

        /// <summary>
        /// 姓名
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// 性别
        /// </summary>
        public String m_sex = "";

        /// <summary>
        /// 目前状态
        /// </summary>
        public String m_state = "";
    }

    /// <summary>
    /// 员工信息服务
    /// </summary>
    public class StaffService
    {
        /// <summary>
        /// 创建服务
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
        /// 员工信息
        /// </summary>
        public List<StaffInfo> m_staffs = new List<StaffInfo>();

        /// <summary>
        /// 删除员工信息
        /// </summary>
        /// <param name="jobID">员工ID</param>
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
        /// 获取可选择的员工
        /// </summary>
        /// <returns>可选择员工</returns>
        public List<StaffInfo> GetAliveStaffs()
        {
            List<StaffInfo> staffs = new List<StaffInfo>();
            int staffsSize = m_staffs.Count;
            for (int i = 0; i < staffsSize; i++)
            {
                if (m_staffs[i].m_canSelect == "是")
                {
                    staffs.Add(m_staffs[i]);
                }
            }
            return staffs;
        }

        /// <summary>
        /// 获取新的员工ID
        /// </summary>
        /// <returns>员工ID</returns>
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
        /// 根据工号获取姓名
        /// </summary>
        /// <param name="jobIDs">工号</param>
        /// <returns>姓名</returns>
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
        /// 获取员工信息
        /// </summary>
        /// <param name="jobID">工号</param>
        /// <returns>员工信息</returns>
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
        /// 保存信息
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
        /// 保存信息
        /// </summary>
        /// <param name="staff">员工信息</param>
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
