/**************************************************************************************\
*                                                                                      *
* PersonalService.cs -  Personal service functions, types, and definitions.       *
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
    /// 个人信息
    /// </summary>
    public class PersonalInfo
    {
        /// <summary>
        /// 创建个人信息
        /// </summary>
        public PersonalInfo()
        {
        }

        /// <summary>
        /// 身体状况
        /// </summary>
        public String m_body = "";

        /// <summary>
        /// 跨界
        /// </summary>
        public String m_cross = "";

        /// <summary>
        /// 穿衣打扮
        /// </summary>
        public String m_dress = "";

        /// <summary>
        /// 喝酒
        /// </summary>
        public String m_drink = "";

        /// <summary>
        /// 饮食习惯
        /// </summary>
        public String m_eat = "";

        /// <summary>
        /// 游戏
        /// </summary>
        public String m_game = "";

        /// <summary>
        /// 同性恋
        /// </summary>
        public String m_gay = "";

        /// <summary>
        /// 房屋
        /// </summary>
        public String m_house = "";

        /// <summary>
        /// 工号
        /// </summary>
        public String m_jobID = "";

        /// <summary>
        /// 贷款
        /// </summary>
        public String m_loan = "";

        /// <summary>
        /// 爱好
        /// </summary>
        public String m_lead = "";

        /// <summary>
        /// 婚姻
        /// </summary>
        public String m_marry = "";

        /// <summary>
        /// 曾经工作经验
        /// </summary>
        public String m_oldWork = "";

        /// <summary>
        /// 父母
        /// </summary>
        public String m_parent = "";

        /// <summary>
        /// 消费
        /// </summary>
        public String m_pay = "";

        /// <summary>
        /// 偏好类型
        /// </summary>
        public String m_preference = "";

        /// <summary>
        /// 宠物
        /// </summary>
        public String m_pet = "";

        /// <summary>
        /// 兄妹
        /// </summary>
        public String m_sb = "";

        /// <summary>
        /// 抽烟
        /// </summary>
        public String m_smoke = "";

        /// <summary>
        /// 体育
        /// </summary>
        public String m_sports = "";

        /// <summary>
        /// 顾问
        /// </summary>
        public String m_teach = "";
    }

    /// <summary>
    /// 个人信息服务
    /// </summary>
    public class PersonalService
    {
        /// <summary>
        /// 创建服务
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
        /// 个人信息
        /// </summary>
        public List<PersonalInfo> m_personals = new List<PersonalInfo>();

        /// <summary>
        /// 删除个人信息
        /// </summary>
        /// <param name="jobID">员工ID</param>
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
        /// 获取新的员工ID
        /// </summary>
        /// <returns>员工ID</returns>
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
        /// 获取个人信息
        /// </summary>
        /// <param name="jobID">工号</param>
        /// <returns>个人信息</returns>
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
        /// 保存信息
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
        /// 保存信息
        /// </summary>
        /// <param name="personal">个人信息</param>
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
