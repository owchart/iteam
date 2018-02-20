/**************************************************************************************\
*                                                                                      *
* DimensionService.cs -  Dimension service functions, types, and definitions.       *
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
    /// ��άͼ����
    /// </summary>
    public class DimensionService
    {
         /// <summary>
        /// ��������
        /// </summary>
        public DimensionService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("DIMENSION", ref cookie) > 0)
            {
                try
                {
                    m_dimensions = JsonConvert.DeserializeObject<List<DimensionInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_dimensions == null)
                {
                    try
                    {
                        m_dimensions = JsonConvert.DeserializeObject<List<DimensionInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// ��άͼ��Ϣ
        /// </summary>
        public List<DimensionInfo> m_dimensions = new List<DimensionInfo>();

        /// <summary>
        /// ��ȡ��άͼ��Ϣ
        /// </summary>
        /// <param name="jobID">����ID</param>
        /// <returns>��άͼ</returns>
        public DimensionInfo GetDimension(String jobID)
        {
            int dimensionsSize = m_dimensions.Count;
            for (int i = 0; i < dimensionsSize; i++)
            {
                if (m_dimensions[i].m_jobID == jobID)
                {
                    return m_dimensions[i];
                }
            }
            return new DimensionInfo();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "DIMENSION";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_dimensions));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="dimension">��άͼ��Ϣ</param>
        public void Save(DimensionInfo dimension)
        {
            bool modify = false;
            int dimensionsSize = m_dimensions.Count;
            for (int i = 0; i < dimensionsSize; i++)
            {
                if (m_dimensions[i].m_jobID == dimension.m_jobID)
                {
                    m_dimensions[i] = dimension;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_dimensions.Add(dimension);
            }
            Save();
        }
    }
}
