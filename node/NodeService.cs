/*****************************************************************************\
*                                                                             *
* NodeService.cs -   Node service functions, types, and definitions.             *
*                                                                             *
*               Version 1.00  ★★★                                          *
*                                                                             *
*               Copyright (c) 2016-2016, OwPlan. All rights reserved.      *
*               Created by Lord 2016/3/10.                                    *
*                                                                             *
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;

namespace node
{
    /// <summary>
    /// Http数据
    /// </summary>
    public class HttpData
    {
        public int m_contentLength;
        public String m_contentType = "";
        public byte[] m_body;
        public String m_method = "";
        public Dictionary<String, String> m_parameters = new Dictionary<String, String>();
        public String m_resStr;
        public int m_statusCode = 200;
        public String m_url = "";
    }

    /// <summary>
    /// Node服务
    /// </summary>
    public class NodeService
    {
        /// <summary>
        /// 接受请求
        /// </summary>
        /// <param name="param">参数</param>
        private void ReadData(object param)
        {
            Socket socket = (Socket)param;
            int newSocketID = (int)socket.Handle;
            try
            {
                byte[] buffer = new byte[102400];
                int len = socket.Receive(buffer);
                MemoryStream memoryStream = new MemoryStream(buffer);
                StreamReader reader = new StreamReader(memoryStream);
                HttpData data = new HttpData();
                String requestHeader;
                int contentLength = 0;
                string parameters = "";
                while ((requestHeader = reader.ReadLine()) != null && !String.IsNullOrEmpty(requestHeader))
                {
                    String lowerHeader = requestHeader.ToLower();
                    if (lowerHeader.IndexOf("get") == 0)
                    {
                        int end = lowerHeader.IndexOf("http/");
                        data.m_method = "GET";
                        parameters = requestHeader.Substring(5, end - 6);
                    }
                    else if (lowerHeader.IndexOf("post") == 0)
                    {
                        int end = lowerHeader.IndexOf("http/");
                        data.m_method = "POST";
                        parameters = requestHeader.Substring(5, end - 6);
                    }
                    else if (lowerHeader.IndexOf("accept: ") == 0)
                    {
                        try
                        {
                            data.m_contentType = requestHeader.Substring(8, requestHeader.IndexOf(',') - 8);
                        }
                        catch { }
                    }
                    else if (lowerHeader.IndexOf("content-type:") == 0)
                    {
                        data.m_contentType = requestHeader.Substring(14);
                    }
                    else if (lowerHeader.IndexOf("host:") == 0)
                    {
                        data.m_url = requestHeader.Substring(requestHeader.IndexOf(':') + 2);
                    }
                    else if (lowerHeader.IndexOf("content-length") == 0)
                    {
                        int begin = lowerHeader.IndexOf("content-length:") + "content-length:".Length;
                        String postParamterLength = requestHeader.Substring(begin).Trim();
                        contentLength = Convert.ToInt32(postParamterLength);
                    }
                }
                if (contentLength > 0)
                {
                    int idx = 0, ide = 0;
                    data.m_body = new byte[contentLength];
                    while (idx < contentLength)
                    {
                        int recvData = reader.Read();
                        if (recvData != -1)
                        {
                            if (recvData != 0)
                            {
                                ide++;
                            }
                            idx++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    reader.Close();
                    memoryStream.Dispose();
                    if (ide == 0)
                    {
                        socket.Receive(data.m_body);
                    }
                    else
                    {
                        for (int i = 0; i < contentLength; i++)
                        {
                            data.m_body[i] = buffer[len - contentLength + i];
                        }
                    }
                    data.m_contentLength = contentLength;
                }
                else
                {
                    reader.Close();
                    memoryStream.Dispose();
                }
                if (data.m_method.Length == 0)
                {
                    return;
                }
                int cindex = parameters.IndexOf('?');
                if (cindex != -1)
                {
                    data.m_url = data.m_url + "/" + parameters;
                    parameters = parameters.Substring(cindex + 1);
                    String[] strs = parameters.Split(new String[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                    int strsSize = strs.Length;
                    for (int i = 0; i < strsSize; i++)
                    {
                        String[] subStrs = strs[i].Split(new String[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        data.m_parameters[subStrs[0].ToLower()] = subStrs[1];
                    }
                }
                else
                {
                    data.m_url += "/" + parameters;
                }
                //在这里处理请求
                if (data.m_method == "POST")
                {
                    if (data.m_url.IndexOf("sendresult") != -1)
                    {
                        String name = data.m_parameters["name"];
                        File.WriteAllText(Application.StartupPath + "\\" + name + ".txt", Encoding.Default.GetString(data.m_body), Encoding.Default);
                        data.m_resStr = "1";
                    }
                }
                else if (data.m_method == "GET")
                {
                    if (data.m_url.IndexOf("getresult") != -1)
                    {
                        String name = data.m_parameters["name"];
                        String file = File.ReadAllText(Application.StartupPath + "\\" + name + ".txt", Encoding.Default); 
                        data.m_resStr = file;
                    }
                }
                int resContentLength = 0;
                if (data.m_resStr != null)
                {
                    resContentLength = Encoding.Default.GetBytes(data.m_resStr).Length;
                }
                StringBuilder bld = new StringBuilder();
                bld.Append("HTTP/1.0 " + data.m_statusCode.ToString() + " OK\r\n");
                bld.Append(String.Format("Content-Length: {0}\r\n", resContentLength));
                bld.Append("Connection: close\r\n\r\n");
                bld.Append(data.m_resStr);
                socket.Send(Encoding.Default.GetBytes(bld.ToString()));
            }
            catch (Exception ex)
            {
            }
            finally
            {
                socket.Close();
            }
        }

        /// <summary>
        /// 启动监听
        /// </summary>
        public void Start(int port)
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Any, port);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(ipe);
            listener.Listen(0);
            while (true)
            {
                Socket socket = listener.Accept();
                ThreadPool.QueueUserWorkItem(new WaitCallback(ReadData), socket);
            }
        }
    }
}