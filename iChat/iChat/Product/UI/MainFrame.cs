/*****************************************************************************\
*                                                                             *
* MainFrame.cs -  MainFrame functions, types, and definitions.                *
*                                                                             *
*               Version 1.00  ★★★                                          *
*                                                                             *
*               Copyright (c) 2016-2016, iTeam. All rights reserved.      *
*               Created by Lord 2016/12/24.                                   *
*                                                                             *
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using OwLib;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;

namespace OwLib
{
    /// <summary>
    /// 管理系统
    /// </summary>
    public class MainFrame : UIXmlEx, IDisposable
    {
        /// <summary>
        /// 创建行情系统
        /// </summary>
        public MainFrame()
        {
        }

        /// <summary>
        /// 主机列表
        /// </summary>
        private GridA m_gridHosts;

        /// <summary>
        /// 主图层
        /// </summary>
        private DivA m_mainDiv;

        private BarrageDiv m_barrageDiv;

        /// <summary>
        /// 获取或设置弹幕控件
        /// </summary>
        public BarrageDiv BarrageDiv
        {
            get { return m_barrageDiv; }
            set { m_barrageDiv = value; }
        }

        /// <summary>
        /// 区块链数据回调
        /// </summary>
        /// <param name="message">消息</param>
        public void GintechMessageCallBack(CMessage message)
        {
            m_mainDiv.BeginInvoke(message);
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="mp">坐标</param>
        /// <param name="button">按钮</param>
        /// <param name="clicks">点击次数</param>
        /// <param name="delta">滚轮值/param>
        private void ClickEvent(object sender, POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            if (button == MouseButtonsA.Left && clicks == 1)
            {
                ControlA control = sender as ControlA;
                String name = control.Name;
                if (name == "btnSendAll")
                {
                    String text = GetTextBox("txtSend").Text;
                    if (text == null || text.Trim().Length == 0)
                    {
                        MessageBox.Show("请输入要发送的内容!", "提示");
                    }
                    GintechData gintechData = new GintechData();
                    gintechData.m_text = text;
                    foreach (GintechService gs in DataCenter.ClientGintechServices.Values)
                    {
                        if (gs.ToServer)
                        {
                            gs.SendAll(gintechData);
                        }
                    }
                }
                else if (name == "btnSend")
                {
                    String text = GetTextBox("txtSend").Text;
                    if (text == null || text.Trim().Length == 0)
                    {
                        MessageBox.Show("请输入要发送的内容!", "提示");
                    }
                    List<GridRow> selectedRows = m_gridHosts.SelectedRows;
                    int selectedRowsSize = selectedRows.Count;
                    if (selectedRowsSize > 0)
                    {
                        GridRow firstSelectedRow = selectedRows[0];
                        String ip = selectedRows[0].GetCell("colP1").GetString();
                        int port = selectedRows[0].GetCell("colP2").GetInt();
                        GintechService gintechService = null;
                        String key = ip + ":" + CStr.ConvertIntToStr(port);
                        if (DataCenter.ClientGintechServices.ContainsKey(key))
                        {
                            gintechService = DataCenter.ClientGintechServices[key];
                        }
                        else
                        {
                            int socketID = BaseService.Connect(ip, port);
                            if (socketID != -1)
                            {
                                gintechService = new GintechService();
                                gintechService.SocketID = socketID;
                                int type = selectedRows[0].GetCell("colP3").GetInt();
                                if (type == 1)
                                {
                                    gintechService.ServerIP = ip;
                                    gintechService.ServerPort = port;
                                }
                                DataCenter.ClientGintechServices[key] = gintechService;
                                BaseService.AddService(gintechService);
                            }
                        }
                        GintechData gintechData = new GintechData();
                        gintechData.m_text = text;
                        gintechService.Send(gintechData);
                    }
                }
            }
        }

        /// <summary>
        /// 销毁资源方法
        /// </summary>
        public override void Dispose()
        {
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        public override void Exit()
        {
           
        }

        /// <summary>
        /// 调用主线程返方法
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="args">参数</param>
        public void Invoke(object sender, object args)
        {
            CMessage message = args as CMessage;
            if (message.m_serviceID == GintechService.SERVICEID_GINTECH)
            {
                if (message.m_functionID == GintechService.FUNCTIONID_GINTECH_SENDALL)
                {
                    List<GintechData> datas = new List<GintechData>();
                    GintechService.GetGintechDatas(datas, message.m_body, message.m_bodyLength);
                    int datasSize = datas.Count;
                    for (int i = 0; i < datasSize; i++)
                    {
                        GintechData data = datas[i];
                        CIndicator indicator = CFunctionEx.CreateIndicator("", data.m_text, this);
                        indicator.Clear();
                        indicator.Dispose();
                    }
                }
                else if (message.m_functionID == GintechService.FUNCTIONID_GETHOSTS)
                {
                    List<GintechHostInfo> datas = new List<GintechHostInfo>();
                    int type = 0;
                    GintechService.GetHostInfos(datas, ref type, message.m_body, message.m_bodyLength);
                    m_gridHosts.BeginUpdate();
                    if (type == 0)
                    {
                        m_gridHosts.ClearRows();
                    }
                    if (type != 2)
                    {
                        int datasSize = datas.Count;
                        for (int i = 0; i < datasSize; i++)
                        {
                            GridRow row = new GridRow();
                            m_gridHosts.AddRow(row);
                            GintechHostInfo hostInfo = datas[i];
                            row.AddCell("colP1", new GridStringCell(hostInfo.m_ip));
                            row.AddCell("colP2", new GridIntCell(hostInfo.m_serverPort));
                            row.AddCell("colP3", new GridStringCell(hostInfo.m_type == 1 ? "服务端" : "客户端"));
                            //全节点
                            if (hostInfo.m_type == 1)
                            {
                                if (hostInfo.m_ip != "127.0.0.1")
                                {
                                    String newServer = hostInfo.m_ip + ":" + CStr.ConvertIntToStr(hostInfo.m_serverPort);
                                    List<GintechHostInfo> hostInfos = new List<GintechHostInfo>();
                                    UserCookie cookie = new UserCookie();
                                    if (DataCenter.UserCookieService.GetCookie("FULLSERVERS", ref cookie) > 0)
                                    {
                                        hostInfos = JsonConvert.DeserializeObject<List<GintechHostInfo>>(cookie.m_value);
                                    }
                                    int hostInfosSize = hostInfos.Count;
                                    bool contains = false;
                                    for (int j = 0; j < hostInfosSize; j++)
                                    {
                                        GintechHostInfo oldHostInfo = hostInfos[j];
                                        String key = oldHostInfo.m_ip + ":" + CStr.ConvertIntToStr(oldHostInfo.m_serverPort);
                                        if (key == newServer)
                                        {
                                            contains = true;
                                            break;
                                        }
                                    }
                                    if (!contains)
                                    {
                                        hostInfos.Add(hostInfo);
                                        cookie.m_key = "FULLSERVERS";
                                        cookie.m_value = JsonConvert.SerializeObject(hostInfos);
                                        DataCenter.UserCookieService.AddCookie(cookie);
                                    }
                                     String key2 = hostInfo.m_ip + ":" + CStr.ConvertIntToStr(hostInfo.m_serverPort);
                                     if (!DataCenter.ClientGintechServices.ContainsKey(key2))
                                     {
                                         int socketID = OwLib.BaseService.Connect(hostInfo.m_ip, hostInfo.m_serverPort);
                                         if (socketID != -1)
                                         {

                                             Console.WriteLine(hostInfo.m_ip);
                                             OwLib.GintechService clientGintechService = new OwLib.GintechService();
                                             DataCenter.ClientGintechServices[key2] = clientGintechService;
                                             OwLib.BaseService.AddService(clientGintechService);
                                             clientGintechService.ToServer = true;
                                             clientGintechService.RegisterListener(DataCenter.GintechRequestID, new ListenerMessageCallBack(GintechMessageCallBack));
                                             clientGintechService.SocketID = socketID;
                                             clientGintechService.Enter();
                                             return;
                                         }
                                     }
                                }
                            }
                        }
                    }
                    else
                    {
                        Dictionary<String, String> removeHosts = new Dictionary<String, String>();
                        foreach (GintechHostInfo hostInfo in datas)
                        {
                            removeHosts[hostInfo.m_ip + ":" + CStr.ConvertIntToStr(hostInfo.m_serverPort)] = "";
                        }
                        List<GridRow> rows = m_gridHosts.m_rows;
                        int rowsSize = rows.Count;
                        if (rowsSize > 0)
                        {
                            for (int i = 0; i < rowsSize; i++)
                            {
                                GridRow row = rows[i];
                                String key = row.GetCell("colP1").GetString() + ":" + row.GetCell("colP2").GetString();
                                if (removeHosts.ContainsKey(key))
                                {
                                    m_gridHosts.RemoveRow(row);
                                    i--;
                                    rowsSize--;
                                }
                            }
                        }
                    }
                    m_gridHosts.EndUpdate();
                    m_gridHosts.Invalidate();
                }
                else if (message.m_functionID == GintechService.FUNCTIONID_GINTECH_SEND)
                {
                    List<GintechData> datas = new List<GintechData>();
                    GintechService.GetGintechDatas(datas, message.m_body, message.m_bodyLength);
                    int datasSize = datas.Count;
                    for (int i = 0; i < datasSize; i++)
                    {
                        GintechData data = datas[i];
                        CIndicator indicator = CFunctionEx.CreateIndicator("", data.m_text, this);
                        indicator.Clear();
                        indicator.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// 是否有窗体显示
        /// </summary>
        /// <returns>是否显示</returns>
        public bool IsWindowShowing()
        {
            List<ControlA> controls = Native.GetControls();
            int controlsSize = controls.Count;
            for (int i = 0; i < controlsSize; i++)
            {
                WindowFrameA frame = controls[i] as WindowFrameA;
                if (frame != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 加载XML
        /// </summary>
        /// <param name="xmlPath">XML路径</param>
        public override void Load(String xmlPath)
        {
            LoadFile(xmlPath, null);
            DataCenter.MainUI = this;
            m_mainDiv = Native.GetControls()[0] as DivA;
            m_mainDiv.BackColor = COLOR.CONTROL;
            ControlPaintEvent paintLayoutEvent = new ControlPaintEvent(PaintLayoutDiv);
            m_mainDiv.RegisterEvent(paintLayoutEvent, EVENTID.PAINT);
            m_mainDiv.RegisterEvent(new ControlInvokeEvent(Invoke), EVENTID.INVOKE);
            DataCenter.ServerGintechService.RegisterListener(DataCenter.GintechRequestID, new ListenerMessageCallBack(GintechMessageCallBack));
            m_barrageDiv = new BarrageDiv();
            m_barrageDiv.Dock = DockStyleA.Fill;
            m_barrageDiv.TopMost = true;
            Native.AddControl(m_barrageDiv);
            m_gridHosts = GetGrid("gridHosts");
            RegisterEvents(m_mainDiv);
            Thread thread = new Thread(new ThreadStart(StartConnect));
            thread.Start();
        }

        /// <summary>
        /// 重绘菜单布局
        /// </summary>
        /// <param name="sender">调用对象</param>
        /// <param name="paint">绘图对象</param>
        /// <param name="clipRect">裁剪区域</param>
        private void PaintLayoutDiv(object sender, CPaint paint, OwLib.RECT clipRect)
        {
            ControlA control = sender as ControlA;
            int width = control.Width, height = control.Height;
            OwLib.RECT drawRect = new OwLib.RECT(0, 0, width, height);
            paint.FillGradientRect(CDraw.PCOLORS_BACKCOLOR, CDraw.PCOLORS_BACKCOLOR2, drawRect, 0, 90);
        }

        /// 注册事件
        /// </summary>
        /// <param name="control">控件</param>
        private void RegisterEvents(ControlA control)
        {
            ControlMouseEvent clickButtonEvent = new ControlMouseEvent(ClickEvent);
            List<ControlA> controls = control.GetControls();
            int controlsSize = controls.Count;
            for (int i = 0; i < controlsSize; i++)
            {
                ControlA subControl = controls[i];
                ButtonA button = subControl as ButtonA;
                if (button != null)
                {
                    button.RegisterEvent(clickButtonEvent, EVENTID.CLICK);
                }
                RegisterEvents(controls[i]);
            }
        }

        /// <summary>
        /// 显示提示窗口
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="caption">标题</param>
        /// <param name="uType">格式</param>
        /// <returns>结果</returns>
        public int ShowMessageBox(String text, String caption, int uType)
        {
            MessageBox.Show(text, caption);
            return 1;
        }

        /// <summary>
        /// 开始启动服务
        /// </summary>
        public void StartConnect()
        {
            List<GintechHostInfo> hostInfos = new List<GintechHostInfo>();
            UserCookie cookie = new UserCookie();
            if (DataCenter.UserCookieService.GetCookie("FULLSERVERS", ref cookie) > 0)
            {
                hostInfos = JsonConvert.DeserializeObject<List<GintechHostInfo>>(cookie.m_value);
            }
            else
            {
                GintechHostInfo defaultHostInfo = new GintechHostInfo();
                defaultHostInfo.m_ip = "192.168.88.103";
                defaultHostInfo.m_serverPort = 16666;
                hostInfos.Add(defaultHostInfo);
            }
            int hostInfosSize = hostInfos.Count;
            Random rd = new Random();
            while (true)
            {
                GintechHostInfo hostInfo = hostInfos[rd.Next(0, hostInfosSize)];
                int socketID = OwLib.BaseService.Connect(hostInfo.m_ip, hostInfo.m_serverPort);
                if (socketID != -1)
                {
                    String key = hostInfo.m_ip + ":" + CStr.ConvertIntToStr(hostInfo.m_serverPort);
                    Console.WriteLine(hostInfo.m_ip);
                    OwLib.GintechService clientGintechService = new OwLib.GintechService();
                    DataCenter.ClientGintechServices[key] = clientGintechService;
                    OwLib.BaseService.AddService(clientGintechService);
                    clientGintechService.ToServer = true;
                    clientGintechService.RegisterListener(DataCenter.GintechRequestID, new ListenerMessageCallBack(GintechMessageCallBack));
                    clientGintechService.SocketID = socketID;
                    clientGintechService.Enter();
                    return;
                }
            }
        }
    }
}
