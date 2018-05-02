/*******************************************************************************************\
*                                                                                           *
* HttpPostService.cs -  Http post service functions, types, and definitions.                *
*                                                                                           *
*               Version 1.00  ★★★                                                        *
*                                                                                           *
*               Copyright (c) 2016-2016, Piratecat. All rights reserved.                    *
*               Created by Lord 2016/10/17.                                                  *
*                                                                                           *
********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO.Compression;
using OwLib;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace node.gs
{
    /// <summary>
    /// HTTP的POST服务
    /// </summary>
    public class HttpPostService
    {
        /// <summary>
        /// 创建HTTP服务
        /// </summary>
        public HttpPostService()
        {
        }

        private int m_timeout = 10;
        /// <summary>
        /// 获取或者设置Timeout时间
        /// </summary>
        public int Timeout
        {
            get { return m_timeout; }
            set { m_timeout = value; }
        }

        private string m_url;

        /// <summary>
        /// 获取或设置地址
        /// </summary>
        public string Url
        {
            get { return m_url; }
            set { m_url = value; }
        }

        /// <summary>
        /// 发送POST数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">数据</param>
        /// <returns>结果</returns>
        public String Post(String url)
        {
            return Post(url, "");
        }

        /// <summary>
        /// 发送POST数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">数据</param>
        /// <returns>结果</returns>
        public String Post(String url, String data)
        {
            byte[] sendDatas = Encoding.Default.GetBytes(data);
            byte[] bytes = Post(url, sendDatas);
            if (bytes != null)
            {
                return Encoding.Default.GetString(bytes);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 发送POST数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">数据</param>
        /// <returns>结果</returns>
        public byte[] Post(String url, byte[] sendDatas)
        {
            HttpWebRequest request = null;
            Stream reader = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                //request.Timeout = m_timeout;
                request.ContentType = "application/x-www-form-urlencoded";
                if (sendDatas != null)
                {
                    request.ContentLength = sendDatas.Length;
                    Stream writer = request.GetRequestStream();
                    writer.Write(sendDatas, 0, sendDatas.Length);
                    writer.Close();
                }
                response = (HttpWebResponse)request.GetResponse();
                reader = response.GetResponseStream();
                long contentLength = response.ContentLength;
                byte[] recvDatas = new byte[contentLength];
                for (int i = 0; i < contentLength; i++)
                {
                    recvDatas[i] = (byte)reader.ReadByte();
                }
                return recvDatas;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
    }
}
