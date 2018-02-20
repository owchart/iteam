/**************************************************************************************\
*                                                                                      *
* RemoteService.cs -  Remote service functions, types, and definitions.       *
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
    /// 服务信息
    /// </summary>
    public class RemoteInfo : BaseInfo
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public String m_IP = "";

        /// <summary>
        /// 名称
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// 密码
        /// </summary>
        public String m_password = "";

        /// <summary>
        /// 端口
        /// </summary>
        public String m_port = "";

        /// <summary>
        /// 备注
        /// </summary>
        public String m_remarks = "";

        /// <summary>
        /// 用户名
        /// </summary>
        public String m_userName = "";
    }

    /// <summary>
    /// 远程服务的服务
    /// </summary>
    public class RemoteService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public RemoteService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("REMOTE", ref cookie) > 0)
            {
                try
                {
                    m_remotes = JsonConvert.DeserializeObject<List<RemoteInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_remotes == null)
                {
                    try
                    {
                        m_remotes = JsonConvert.DeserializeObject<List<RemoteInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 服务信息
        /// </summary>
        public List<RemoteInfo> m_remotes = new List<RemoteInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int remotesSize = m_remotes.Count;
            for (int i = 0; i < remotesSize; i++)
            {
                if (m_remotes[i].m_ID == id)
                {
                    m_remotes.RemoveAt(i);
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
            int remotesSize = m_remotes.Count;
            for (int i = 0; i < remotesSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_remotes[i].m_ID));
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
        public RemoteInfo GetRemote(String id)
        {
            int remotesSize = m_remotes.Count;
            for (int i = 0; i < remotesSize; i++)
            {
                if (m_remotes[i].m_ID == id)
                {
                    return m_remotes[i];
                }
            }
            return new RemoteInfo();
        }


        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "REMOTE";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_remotes));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="server">信息</param>
        public void Save(RemoteInfo remote)
        {
            bool modify = false;
            int remotesSize = m_remotes.Count;
            for (int i = 0; i < remotesSize; i++)
            {
                if (m_remotes[i].m_ID == remote.m_ID)
                {
                    m_remotes[i] = remote;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_remotes.Add(remote);
            }
            Save();
        }
    }
}
