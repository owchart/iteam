/**************************************************************************************\
*                                                                                      *
* MasterService.cs -  Master service functions, types, and definitions.       *
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
    /// 上级指示
    /// </summary>
    public class MasterInfo : BaseInfo
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
    /// 上级指示记录服务
    /// </summary>
    public class MasterService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public MasterService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("MASTER", ref cookie) > 0)
            {
                try
                {
                    m_masters = JsonConvert.DeserializeObject<List<MasterInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_masters == null)
                {
                    try
                    {
                        m_masters = JsonConvert.DeserializeObject<List<MasterInfo>>(cookie.m_value);
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
        public List<MasterInfo> m_masters = new List<MasterInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int mastersSize = m_masters.Count;
            for (int i = 0; i < mastersSize; i++)
            {
                if (m_masters[i].m_ID == id)
                {
                    m_masters.RemoveAt(i);
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
            int mastersSize = m_masters.Count;
            for (int i = 0; i < mastersSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_masters[i].m_ID));
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
        public MasterInfo GetMaster(String id)
        {
            int mastersSize = m_masters.Count;
            for (int i = 0; i < mastersSize; i++)
            {
                if (m_masters[i].m_ID == id)
                {
                    return m_masters[i];
                }
            }
            return new MasterInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "MASTER";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_masters));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="master">信息</param>
        public void Save(MasterInfo master)
        {
            bool modify = false;
            int mastersSize = m_masters.Count;
            for (int i = 0; i < mastersSize; i++)
            {
                if (m_masters[i].m_ID == master.m_ID)
                {
                    m_masters[i] = master;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_masters.Add(master);
            }
            Save();
        }
    }
}
