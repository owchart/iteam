/**************************************************************************************\
*                                                                                      *
* WangYiService.cs -  WangYi service functions, types, and definitions.       *
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
    /// 妄议信息
    /// </summary>
    public class WangYiInfo : BaseInfo
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
    /// 妄议服务
    /// </summary>
    public class WangYiService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public WangYiService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("WANGYI", ref cookie) > 0)
            {
                try
                {
                    m_wangyis = JsonConvert.DeserializeObject<List<WangYiInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_wangyis == null)
                {
                    try
                    {
                        m_wangyis = JsonConvert.DeserializeObject<List<WangYiInfo>>(cookie.m_value);
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
        public List<WangYiInfo> m_wangyis = new List<WangYiInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int wangyisSize = m_wangyis.Count;
            for (int i = 0; i < wangyisSize; i++)
            {
                if (m_wangyis[i].m_ID == id)
                {
                    m_wangyis.RemoveAt(i);
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
            int wangyisSize = m_wangyis.Count;
            for (int i = 0; i < wangyisSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_wangyis[i].m_ID));
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
        public WangYiInfo GetWangYi(String id)
        {
            int wangyisSize = m_wangyis.Count;
            for (int i = 0; i < wangyisSize; i++)
            {
                if (m_wangyis[i].m_ID == id)
                {
                    return m_wangyis[i];
                }
            }
            return new WangYiInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "WANGYI";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_wangyis));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="wangYi">信息</param>
        public void Save(WangYiInfo wangYi)
        {
            bool modify = false;
            int wangyisSize = m_wangyis.Count;
            for (int i = 0; i < wangyisSize; i++)
            {
                if (m_wangyis[i].m_ID == wangYi.m_ID)
                {
                    m_wangyis[i] = wangYi;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_wangyis.Add(wangYi);
            }
            Save();
        }
    }
}
