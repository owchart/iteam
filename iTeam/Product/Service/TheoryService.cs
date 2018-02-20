/**************************************************************************************\
*                                                                                      *
* TheoryService.cs -  Theory service functions, types, and definitions.       *
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
    /// 理论信息
    /// </summary>
    public class TheoryInfo : BaseInfo
    {
        /// <summary>
        /// 内容
        /// </summary>
        public String m_content = "";
    }

    /// <summary>
    /// 个人理论服务
    /// </summary>
    public class TheoryService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public TheoryService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("THEORY", ref cookie) > 0)
            {
                try
                {
                    m_theories = JsonConvert.DeserializeObject<List<TheoryInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_theories == null)
                {
                    try
                    {
                        m_theories = JsonConvert.DeserializeObject<List<TheoryInfo>>(cookie.m_value);
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
        public List<TheoryInfo> m_theories = new List<TheoryInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int theoriesSize = m_theories.Count;
            for (int i = 0; i < theoriesSize; i++)
            {
                if (m_theories[i].m_ID == id)
                {
                    m_theories.RemoveAt(i);
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
            int theoriesSize = m_theories.Count;
            for (int i = 0; i < theoriesSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_theories[i].m_ID));
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
        public TheoryInfo GetTheory(String id)
        {
            int theoriesSize = m_theories.Count;
            for (int i = 0; i < theoriesSize; i++)
            {
                if (m_theories[i].m_ID == id)
                {
                    return m_theories[i];
                }
            }
            return new TheoryInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "THEORY";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_theories));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="master">信息</param>
        public void Save(TheoryInfo theory)
        {
            bool modify = false;
            int theoriesSize = m_theories.Count;
            for (int i = 0; i < theoriesSize; i++)
            {
                if (m_theories[i].m_ID == theory.m_ID)
                {
                    m_theories[i] = theory;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_theories.Add(theory);
            }
            Save();
        }
    }
}
