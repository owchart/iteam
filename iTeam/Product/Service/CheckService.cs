/**************************************************************************************\
*                                                                                      *
* CheckService.cs -  Check service functions, types, and definitions.       *
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
    /// 个人检查记录
    /// </summary>
    public class CheckInfo : BaseInfo
    {
        /// <summary>
        /// 问答
        /// </summary>
        public String m_answer = "";

        /// <summary>
        /// 工号
        /// </summary>
        public String m_jobID = "";
    }

    /// <summary>
    /// 个人检查服务
    /// </summary>
    public class CheckService
    {
        /// <summary>
        /// 创建服务
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
        /// 个人检查信息
        /// </summary>
        public List<CheckInfo> m_checks = new List<CheckInfo>();

        /// <summary>
        /// 删除信息
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
        /// 获取新的ID
        /// </summary>
        /// <returns>新ID</returns>
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
        /// 获取信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>信息</returns>
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
        /// 保存信息
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
        /// 保存信息
        /// </summary>
        /// <param name="award">信息</param>
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
