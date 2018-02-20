/**************************************************************************************\
*                                                                                      *
* BeAttackedService.cs -  BeAttacked service functions, types, and definitions.       *
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
    /// 被攻击信息
    /// </summary>
    public class BeAttackedInfo : BaseInfo
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
    public class BeAttackedService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public BeAttackedService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("BEATTACKED", ref cookie) > 0)
            {
                try
                {
                    m_beAttackeds = JsonConvert.DeserializeObject<List<BeAttackedInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_beAttackeds == null)
                {
                    try
                    {
                        m_beAttackeds = JsonConvert.DeserializeObject<List<BeAttackedInfo>>(cookie.m_value);
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
        public List<BeAttackedInfo> m_beAttackeds = new List<BeAttackedInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int beAttackedsSize = m_beAttackeds.Count;
            for (int i = 0; i < beAttackedsSize; i++)
            {
                if (m_beAttackeds[i].m_ID == id)
                {
                    m_beAttackeds.RemoveAt(i);
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
            int beAttackedsSize = m_beAttackeds.Count;
            for (int i = 0; i < beAttackedsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_beAttackeds[i].m_ID));
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
        public BeAttackedInfo GetBeAttacked(String id)
        {
            int beAttackedsSize = m_beAttackeds.Count;
            for (int i = 0; i < beAttackedsSize; i++)
            {
                if (m_beAttackeds[i].m_ID == id)
                {
                    return m_beAttackeds[i];
                }
            }
            return new BeAttackedInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "BEATTACKED";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_beAttackeds));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="disobey">信息</param>
        public void Save(BeAttackedInfo beAttacked)
        {
            bool modify = false;
            int beAttackedsSize = m_beAttackeds.Count;
            for (int i = 0; i < beAttackedsSize; i++)
            {
                if (m_beAttackeds[i].m_ID == beAttacked.m_ID)
                {
                    m_beAttackeds[i] = beAttacked;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_beAttackeds.Add(beAttacked);
            }
            Save();
        }
    }
}
