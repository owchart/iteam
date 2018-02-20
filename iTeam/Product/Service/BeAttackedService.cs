/**************************************************************************************\
*                                                                                      *
* BeAttackedService.cs -  BeAttacked service functions, types, and definitions.       *
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
    /// ��������Ϣ
    /// </summary>
    public class BeAttackedInfo : BaseInfo
    {
        /// <summary>
        /// ����
        /// </summary>
        public String m_content = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_level = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_title = "";
    }

    /// <summary>
    /// ������¼����
    /// </summary>
    public class BeAttackedService
    {
        /// <summary>
        /// ��������
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
        /// ָʾ��Ϣ
        /// </summary>
        public List<BeAttackedInfo> m_beAttackeds = new List<BeAttackedInfo>();

        /// <summary>
        /// ɾ����Ϣ
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
        /// ��ȡ�µ�ID
        /// </summary>
        /// <returns>��ID</returns>
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
        /// ��ȡ��Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>��Ϣ</returns>
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
        /// ������Ϣ
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
        /// ������Ϣ
        /// </summary>
        /// <param name="disobey">��Ϣ</param>
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
