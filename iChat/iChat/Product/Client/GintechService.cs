/*****************************************************************************\
*                                                                             *
* GintechService.cs -  Gintech service functions, types, and definitions            *
*                                                                             *
*               Version 1.00 ★                                               *
*                                                                             *
*               Copyright (c) 2016-2016, Client. All rights reserved.         *
*               Created by QiChunyou.                                         *
*                                                                             *
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OwLib
{
    /// <summary>
    /// 聊天数据
    /// </summary>
    public class GintechData
    {
        #region 齐春友 2016/6/9
        /// <summary>
        /// 内容
        /// </summary>
        public String m_text = "";

        /// <summary>
        /// 类型
        /// </summary>
        public int m_type;
        #endregion
    }

    /// <summary>
    /// 主机信息
    /// </summary>
    public class GintechHostInfo
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public String m_ip;
    }

    /// <summary>
    /// 聊天服务
    /// </summary>
    public class GintechService:BaseService
    {
        #region 齐春友 2016/6/3
        /// <summary>
        /// 构造函数
        /// </summary>
        public GintechService()
        {
            ServiceID = SERVICEID_GINTECH;
        }

        /// <summary>
        /// 聊天服务ID
        /// </summary>
        public const int SERVICEID_GINTECH = 7;

        /// <summary>
        /// 主机信息
        /// </summary>
        public const int FUNCTIONID_GETHOSTS = 1;

        /// <summary>
        /// 广播聊天功能ID
        /// </summary>
        public const int FUNCTIONID_GINTECH_SENDALL = 2;

        /// <summary>
        /// 接收聊天功能ID
        /// </summary>
        public const int FUNCTIONID_GINTECH_RECV = 3;

        /// <summary>
        /// 发送消息
        /// </summary>
        public const int FUNCTIONID_GINTECH_SEND = 4;

        private int m_requestID = BaseService.GetRequestID();

        /// <summary>
        /// 获取请求ID
        /// </summary>
        public int RequestID
        {
            get { return m_requestID; }
        }

        private int m_socketID;

        /// <summary>
        /// 获取或设置套接字ID
        /// </summary>
        public int SocketID
        {
            get { return m_socketID; }
            set { m_socketID = value; }
        }

        /// <summary>
        /// 获取弹幕信息
        /// </summary>
        /// <param name="datas">用于返回弹幕信息</param>
        /// <param name="body">包体</param>
        /// <param name="bodyLength">包体长度</param>
        /// <returns></returns>
        public static int GetGintechDatas(List<GintechData> datas, byte[] body, int bodyLength)
        {
            Binary br = new Binary();
            br.Write(body, bodyLength);
            int size = br.ReadInt();
            for (int i = 0; i < size; i++)
            {
                GintechData data = new GintechData();
                data.m_type = br.ReadChar();
                data.m_text = br.ReadString();
                datas.Add(data);
            }
            br.Close();
            return 1;     
        }

        /// <summary>
        /// 获取主机信息
        /// </summary>
        /// <param name="body">包体</param>
        /// <param name="bodyLength">包体长度</param>
        /// <returns></returns>
        public static int GetHostInfos(List<GintechHostInfo> datas, byte[] body, int bodyLength)
        {
            Binary br = new Binary();
            br.Write(body, bodyLength);
            int size = br.ReadInt();
            for (int i = 0; i < size; i++)
            {
                GintechHostInfo data = new GintechHostInfo();
                data.m_ip = br.ReadString();
                datas.Add(data);
            }
            br.Close();
            return 1;     
        }

        /// <summary>
        /// 获取主机信息
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public int GetHostInfos(int requestID)
        {
            byte[] bytes = new byte[1];
            int ret = Send(new CMessage(GroupID, ServiceID, FUNCTIONID_GETHOSTS, SessionID, requestID, m_socketID, 0, CompressType, bytes.Length, bytes));
            return ret > 0 ? 1 : 0;
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="message">消息</param>
        public override void OnReceive(CMessage message)
        {
            base.OnReceive(message);
            SendToListener(message);  
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="requestID">请求ID</param>
        /// <param name="args"></param>
        public int Send(int requestID, GintechData data)
        {
            List<GintechData> datas = new List<GintechData>();
            datas.Add(data);
            int ret = Send(FUNCTIONID_GINTECH_SEND, requestID, datas);
            datas.Clear();
            return ret > 0 ? 1 : 0;
        }

        /// <summary>
        /// 进入弹幕
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="requestID">请求ID</param>
        /// <param name="args"></param>
        public int SendAll(int requestID, GintechData data)
        {
            List<GintechData> datas = new List<GintechData>();
            datas.Add(data);
            int ret = Send(FUNCTIONID_GINTECH_SENDALL, requestID, datas);
            datas.Clear();
            return ret > 0 ? 1 : 0;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="userID">方法ID</param>
        /// <param name="userID">请求ID</param>
        /// <param name="text">发送字符</param>
        public int Send(int functionID, int requestID, List<GintechData> datas)
        {
            Binary bw = new Binary();
            int dataSize = datas.Count;
            bw.WriteInt(dataSize);
            for (int i = 0; i < dataSize; i++)
            {
                GintechData data = datas[i];
                bw.WriteChar((char)data.m_type);
                bw.WriteString(data.m_text);
            }
            byte[] bytes = bw.GetBytes();
            int ret = Send(new CMessage(GroupID, ServiceID, functionID, SessionID, requestID, m_socketID, 0, CompressType, bytes.Length, bytes));
            bw.Close();
            return ret;
        }
        #endregion
    }
}
