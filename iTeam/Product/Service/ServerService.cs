/**************************************************************************************\
*                                                                                      *
* ServerService.cs -  Server service functions, types, and definitions.       *
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
    /// 服务器信息
    /// </summary>
    public class ServerInfo : BaseInfo
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
        /// 用户名
        /// </summary>
        public String m_userName = "";
    }

    /// <summary>
    /// 服务器服务
    /// </summary>
    public class ServerService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public ServerService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("SERVER", ref cookie) > 0)
            {
                try
                {
                    m_servers = JsonConvert.DeserializeObject<List<ServerInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_servers == null)
                {
                    try
                    {
                        m_servers = JsonConvert.DeserializeObject<List<ServerInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 服务器信息
        /// </summary>
        public List<ServerInfo> m_servers = new List<ServerInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int serversSize = m_servers.Count;
            for (int i = 0; i < serversSize; i++)
            {
                if (m_servers[i].m_ID == id)
                {
                    m_servers.RemoveAt(i);
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
            int serversSize = m_servers.Count;
            for (int i = 0; i < serversSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_servers[i].m_ID));
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
        public ServerInfo GerServer(String id)
        {
            int serversSize = m_servers.Count;
            for (int i = 0; i < serversSize; i++)
            {
                if (m_servers[i].m_ID == id)
                {
                    return m_servers[i];
                }
            }
            return new ServerInfo();
        }


        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "SERVER";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_servers));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="server">信息</param>
        public void Save(ServerInfo server)
        {
            bool modify = false;
            int serversSize = m_servers.Count;
            for (int i = 0; i < serversSize; i++)
            {
                if (m_servers[i].m_ID == server.m_ID)
                {
                    m_servers[i] = server;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_servers.Add(server);
            }
            Save();
        }
    }
}
