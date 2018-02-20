/**************************************************************************************\
*                                                                                      *
* BusinessCardService.cs -  Business card service functions, types, and definitions.       *
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
    public class BusinessCardInfo : BaseInfo
    {
        /// <summary>
        /// 内容
        /// </summary>
        public String m_content = "";

        /// <summary>
        /// 姓名
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// 电话
        /// </summary>
        public String m_phone = "";
    }

    /// <summary>
    /// 名片服务
    /// </summary>
    public class BusinessCardService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public BusinessCardService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("BUSINESSCARD", ref cookie) > 0)
            {
                try
                {
                    m_businessCards = JsonConvert.DeserializeObject<List<BusinessCardInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_businessCards == null)
                {
                    try
                    {
                        m_businessCards = JsonConvert.DeserializeObject<List<BusinessCardInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 名片信息
        /// </summary>
        public List<BusinessCardInfo> m_businessCards = new List<BusinessCardInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int businessCardsSize = m_businessCards.Count;
            for (int i = 0; i < businessCardsSize; i++)
            {
                if (m_businessCards[i].m_ID == id)
                {
                    m_businessCards.RemoveAt(i);
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
            int businessCardsSize = m_businessCards.Count;
            for (int i = 0; i < businessCardsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_businessCards[i].m_ID));
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
        public BusinessCardInfo GetCard(String id)
        {
            int businessCardsSize = m_businessCards.Count;
            for (int i = 0; i < businessCardsSize; i++)
            {
                if (m_businessCards[i].m_ID == id)
                {
                    return m_businessCards[i];
                }
            }
            return new BusinessCardInfo();
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "BUSINESSCARD";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_businessCards));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="master">信息</param>
        public void Save(BusinessCardInfo card)
        {
            bool modify = false;
            int businessCardsSize = m_businessCards.Count;
            for (int i = 0; i < businessCardsSize; i++)
            {
                if (m_businessCards[i].m_ID == card.m_ID)
                {
                    m_businessCards[i] = card;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_businessCards.Add(card);
            }
            Save();
        }
    }
}
