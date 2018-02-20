/**************************************************************************************\
*                                                                                      *
* BusinessCardService.cs -  Business card service functions, types, and definitions.       *
*                                                                                      *
*               Version 1.00 ��                                                        *
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
    /// �ϼ�ָʾ
    /// </summary>
    public class BusinessCardInfo : BaseInfo
    {
        /// <summary>
        /// ����
        /// </summary>
        public String m_content = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// �绰
        /// </summary>
        public String m_phone = "";
    }

    /// <summary>
    /// ��Ƭ����
    /// </summary>
    public class BusinessCardService
    {
        /// <summary>
        /// ��������
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
        /// ��Ƭ��Ϣ
        /// </summary>
        public List<BusinessCardInfo> m_businessCards = new List<BusinessCardInfo>();

        /// <summary>
        /// ɾ����Ϣ
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
        /// ��ȡ�µ�ID
        /// </summary>
        /// <returns>��ID</returns>
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
        /// ��ȡ��Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>��Ϣ</returns>
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
        /// ������Ϣ
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
        /// ������Ϣ
        /// </summary>
        /// <param name="master">��Ϣ</param>
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
