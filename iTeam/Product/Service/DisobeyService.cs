/**************************************************************************************\
*                                                                                      *
* DisobeyService.cs -  Disobey service functions, types, and definitions.       *
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
    /// 抗命信息
    /// </summary>
    public class DisobeyInfo : BaseInfo
    {
        /// <summary>
        /// 内容
        /// </summary>
        public String m_content = "";

        /// <summary>
        /// 级别
        /// </summary>
        public String m_level = "";

        /// <summary>
        /// 标题
        /// </summary>
        public String m_title = "";
    }

    /// <summary>
    /// 抗命记录服务
    /// </summary>
    public class DisobeyService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public DisobeyService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("DISOBEY", ref cookie) > 0)
            {
                try
                {
                    m_disobeys = JsonConvert.DeserializeObject<List<DisobeyInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_disobeys == null)
                {
                    try
                    {
                        m_disobeys = JsonConvert.DeserializeObject<List<DisobeyInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 指示信息
        /// </summary>
        public List<DisobeyInfo> m_disobeys = new List<DisobeyInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int disobeysSize = m_disobeys.Count;
            for (int i = 0; i < disobeysSize; i++)
            {
                if (m_disobeys[i].m_ID == id)
                {
                    m_disobeys.RemoveAt(i);
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
            int disobeysSize = m_disobeys.Count;
            for (int i = 0; i < disobeysSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_disobeys[i].m_ID));
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
        public DisobeyInfo GetDisobey(String id)
        {
            int disobeysSize = m_disobeys.Count;
            for (int i = 0; i < disobeysSize; i++)
            {
                if (m_disobeys[i].m_ID == id)
                {
                    return m_disobeys[i];
                }
            }
            return new DisobeyInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "DISOBEY";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_disobeys));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="disobey">信息</param>
        public void Save(DisobeyInfo disobey)
        {
            bool modify = false;
            int disobeysSize = m_disobeys.Count;
            for (int i = 0; i < disobeysSize; i++)
            {
                if (m_disobeys[i].m_ID == disobey.m_ID)
                {
                    m_disobeys[i] = disobey;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_disobeys.Add(disobey);
            }
            Save();
        }
    }
}
