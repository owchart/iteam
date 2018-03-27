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
    /// 区块链数据
    /// </summary>
    public class GintechData
    {
        #region 齐春友 2016/6/9
        /// <summary>
        /// 内容
        /// </summary>
        public String m_text = "";
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

        /// <summary>
        /// 服务端端口
        /// </summary>
        public int m_serverPort;

        /// <summary>
        /// 类型
        /// </summary>
        public int m_type;
    }

    /// <summary>
    /// 区块链服务
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
        /// 区块链服务ID
        /// </summary>
        public const int SERVICEID_GINTECH = 10000;

        /// <summary>
        /// 主机信息
        /// </summary>
        public const int FUNCTIONID_GETHOSTS = 1;

        /// <summary>
        /// 广播区块链功能ID
        /// </summary>
        public const int FUNCTIONID_GINTECH_SENDALL = 2;

        /// <summary>
        /// 发送消息
        /// </summary>
        public const int FUNCTIONID_GINTECH_SEND = 4;

        /// <summary>
        /// 进入
        /// </summary>
        public const int FUNCTIONID_GINTECH_ENTER = 6;

        private bool m_toServer;

        /// <summary>
        /// 获取或设置是否发送到服务端
        /// </summary>
        public bool ToServer
        {
            get { return m_toServer; }
            set { m_toServer = value; }
        }

        private String m_serverIP;

        /// <summary>
        /// 获取或设置服务器地址
        /// </summary>
        public String ServerIP
        {
            get { return m_serverIP; }
            set { m_serverIP = value; }
        }

        private int m_serverPort;

        /// <summary>
        /// 获取或设置服务器端口
        /// </summary>
        public int ServerPort
        {
            get { return m_serverPort; }
            set { m_serverPort = value; }
        }

        /// <summary>
        /// 进入区块链
        /// </summary>
        /// <returns>状态</returns>
        public int Enter()
        {
            Binary bw = new Binary();
            bw.WriteInt(DataCenter.ServerGintechService.Port);
            bw.WriteInt(DataCenter.IsFull ? 1 : 0);
            byte[] bytes = bw.GetBytes();
            int ret = Send(new CMessage(GroupID, ServiceID, FUNCTIONID_GINTECH_ENTER, SessionID, DataCenter.GintechRequestID, SocketID, 0, CompressType, bytes.Length, bytes));
            bw.Close();
            return ret;
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
        public static int GetHostInfos(List<GintechHostInfo> datas, ref int type, byte[] body, int bodyLength)
        {
            Binary br = new Binary();
            br.Write(body, bodyLength);
            int size = br.ReadInt();
            type = br.ReadInt();
            for (int i = 0; i < size; i++)
            {
                GintechHostInfo data = new GintechHostInfo();
                data.m_ip = br.ReadString();
                data.m_serverPort = br.ReadInt();
                data.m_type = br.ReadInt();
                datas.Add(data);
            }
            br.Close();
            return 1;     
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="message">消息</param>
        public override void OnReceive(CMessage message)
        {
            base.OnReceive(message);
            SendToListener(message);
            if (DataCenter.IsFull && message.m_functionID == FUNCTIONID_GINTECH_SENDALL && message.m_socketID != SocketID)
            {
                DataCenter.ServerGintechService.SendAll(message);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="requestID">请求ID</param>
        /// <param name="args"></param>
        public int Send(GintechData data)
        {
            List<GintechData> datas = new List<GintechData>();
            datas.Add(data);
            int ret = Send(FUNCTIONID_GINTECH_SEND, DataCenter.GintechRequestID, datas);
            datas.Clear();
            return ret > 0 ? 1 : 0;
        }

        /// <summary>
        /// 进入弹幕
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="requestID">请求ID</param>
        /// <param name="args"></param>
        public int SendAll(GintechData data)
        {
            List<GintechData> datas = new List<GintechData>();
            datas.Add(data);
            int ret = Send(FUNCTIONID_GINTECH_SENDALL, DataCenter.GintechRequestID, datas);
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
                bw.WriteString(data.m_text);
            }
            byte[] bytes = bw.GetBytes();
            int ret = Send(new CMessage(GroupID, ServiceID, functionID, SessionID, requestID, SocketID, 0, CompressType, bytes.Length, bytes));
            bw.Close();
            return ret;
        }
        #endregion
    }
}
