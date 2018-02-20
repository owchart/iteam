/**************************************************************************************\
*                                                                                      *
* BSStockService.cs -  BS stock service functions, types, and definitions.             *
*                                                                                      *
*               Version 1.00 ��                                                        *
*                                                                                      *
*               Copyright (c) 2016-2016, iTeam. All rights reserved.                   *
*               Created by Todd.                                                       *
*                                                                                      *
***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace OwLib
{
    /// <summary>
    /// ������Ϣ
    /// </summary>
    public class BSStockInfo : BaseInfo
    {
        /// <summary>
        /// �����
        /// </summary>
        public String m_buyPrice = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_code = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_profit = "";

        /// <summary>
        /// ����
        /// </summary>
        public String m_qty = "1";

        /// <summary>
        /// ������
        /// </summary>
        public String m_sellPrice = "";
    }

    /// <summary>
    /// �ж����ŷ���
    /// </summary>
    public class BSStockService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public BSStockService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("BSSTOCK", ref cookie) > 0)
            {
                try
                {
                    m_bsStocks = JsonConvert.DeserializeObject<List<BSStockInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_bsStocks == null)
                {
                    try
                    {
                        m_bsStocks = JsonConvert.DeserializeObject<List<BSStockInfo>>(cookie.m_value);
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
        public List<BSStockInfo> m_bsStocks = new List<BSStockInfo>();

        /// <summary>
        /// ɾ����Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int bsStocksSize = m_bsStocks.Count;
            for (int i = 0; i < bsStocksSize; i++)
            {
                if (m_bsStocks[i].m_ID == id)
                {
                    m_bsStocks.RemoveAt(i);
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
            int bsStocksSize = m_bsStocks.Count;
            for (int i = 0; i < bsStocksSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_bsStocks[i].m_ID));
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
        public BSStockInfo GetBSStock(String id)
        {
            int bsStocksSize = m_bsStocks.Count;
            for (int i = 0; i < bsStocksSize; i++)
            {
                if (m_bsStocks[i].m_ID == id)
                {
                    return m_bsStocks[i];
                }
            }
            return new BSStockInfo();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "BSSTOCK";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_bsStocks));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="bsStock">��Ϣ</param>
        public void Save(BSStockInfo bsStock)
        {
            bool modify = false;
            int bsStocksSize = m_bsStocks.Count;
            for (int i = 0; i < bsStocksSize; i++)
            {
                if (m_bsStocks[i].m_ID == bsStock.m_ID)
                {
                    m_bsStocks[i] = bsStock;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_bsStocks.Add(bsStock);
            }
            Save();
        }
    }
}
