/**************************************************************************************\
*                                                                                      *
* BSStockService.cs -  BS stock service functions, types, and definitions.             *
*                                                                                      *
*               Version 1.00 ★                                                        *
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
    /// 炒股信息
    /// </summary>
    public class BSStockInfo : BaseInfo
    {
        /// <summary>
        /// 买入价
        /// </summary>
        public String m_buyPrice = "";

        /// <summary>
        /// 代码
        /// </summary>
        public String m_code = "";

        /// <summary>
        /// 名称
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// 利润
        /// </summary>
        public String m_profit = "";

        /// <summary>
        /// 数量
        /// </summary>
        public String m_qty = "1";

        /// <summary>
        /// 卖出价
        /// </summary>
        public String m_sellPrice = "";
    }

    /// <summary>
    /// 行动代号服务
    /// </summary>
    public class BSStockService
    {
        /// <summary>
        /// 创建服务
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
        /// 指示信息
        /// </summary>
        public List<BSStockInfo> m_bsStocks = new List<BSStockInfo>();

        /// <summary>
        /// 删除信息
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
        /// 获取新的ID
        /// </summary>
        /// <returns>新ID</returns>
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
        /// 获取信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>信息</returns>
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
        /// 保存信息
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
        /// 保存信息
        /// </summary>
        /// <param name="bsStock">信息</param>
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
