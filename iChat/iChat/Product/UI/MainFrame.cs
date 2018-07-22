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
using System.Runtime.InteropServices;
using System.Drawing;

namespace OwLib
{
    /// <summary>
    /// 系统
    /// </summary>
    public class MainFrame : UIXmlEx, IDisposable
    {
        /// <summary>
        /// 创建系统
        /// </summary>
        public MainFrame()
        {
        }

        /// <summary>
        /// 主机列表
        /// </summary>
        private GridA m_gridHosts;


        private BarrageDiv m_barrageDiv;

        /// <summary>
        /// 获取或设置弹幕控件
        /// </summary>
        public BarrageDiv BarrageDiv
        {
            get { return m_barrageDiv; }
            set { m_barrageDiv = value; }
        }

        private DivA m_mainDiv;

        /// <summary>
        /// 获取或设置主图层
        /// </summary>
        public DivA MainDiv
        {
            get { return m_mainDiv; }
            set { m_mainDiv = value; }
        }

        private MainForm m_mainForm;

        /// <summary>
        /// 获取或设置主窗体
        /// </summary>
        public MainForm MainForm
        {
            get { return m_mainForm; }
            set { m_mainForm = value; }
        }

        /// <summary>
        /// 区块链数据回调
        /// </summary>
        /// <param name="message">消息</param>
        public void ChatMessageCallBack(CMessage message)
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
                        MessageBox.Show("Please input the content you want send!", "Attention");
                    }
                    RadioButtonA rbBarrage = GetRadioButton("rbBarrage");
                    RadioButtonA rbText = GetRadioButton("rbText");
                    if (rbBarrage.Checked)
                    {
                        text = "addbarrage('" + text + "');";
                    }
                    else if (rbText.Checked)
                    {
                        text = "addtext('" + text + "');";
                    }
                    ChatData chatData = new ChatData();
                    chatData.m_content = text;
                    chatData.m_sender = DataCenter.UserName;
                    foreach (ChatService gs in DataCenter.ClientChatServices.Values)
                    {
                        if (gs.ToServer && gs.Connected)
                        {
                            gs.SendAll(chatData);
                        }
                    }
                    if (rbBarrage.Checked)
                    {
                        CIndicator indicator = CFunctionEx.CreateIndicator("", text, this);
                        indicator.Clear();
                        indicator.Dispose();
                    }
                    else if (rbText.Checked)
                    {
                        TextBoxA txtReceive = GetTextBox("txtReceive");
                        txtReceive.Text += "i say:\r\n" + GetTextBox("txtSend").Text + "\r\n";
                        txtReceive.Invalidate();
                        if (txtReceive.VScrollBar != null && txtReceive.VScrollBar.Visible)
                        {
                            txtReceive.VScrollBar.ScrollToEnd();
                            txtReceive.Update();
                            txtReceive.Invalidate();
                        }
                    }
                }
                else if (name == "btnSend")
                {
                    String text = GetTextBox("txtSend").Text;
                    if (text == null || text.Trim().Length == 0)
                    {
                        MessageBox.Show("Please input the content you want send!", "Attention");
                    }
                    RadioButtonA rbBarrage = GetRadioButton("rbBarrage");
                    RadioButtonA rbText = GetRadioButton("rbText");
                    if (rbBarrage.Checked)
                    {
                        text = "addbarrage('" + text + "');";
                    }
                    else if (rbText.Checked)
                    {
                        text = "addtext('" + text + "');";
                    }
                    List<GridRow> selectedRows = m_gridHosts.SelectedRows;
                    int selectedRowsSize = selectedRows.Count;
                    bool sendAll = false;
                    if (selectedRowsSize > 0)
                    {
                        GridRow firstSelectedRow = selectedRows[0];
                        String ip = selectedRows[0].GetCell("colP1").GetString();
                        int port = selectedRows[0].GetCell("colP2").GetInt();
                        String userID = selectedRows[0].GetCell("colP3").GetString();
                        ChatService chatService = null;
                        String key = ip + ":" + CStr.ConvertIntToStr(port);
                        if (DataCenter.ClientChatServices.ContainsKey(key))
                        {
                            chatService = DataCenter.ClientChatServices[key];
                            if (!chatService.Connected)
                            {
                                int socketID = OwLib.BaseService.Connect(ip, port);
                                if (socketID != -1)
                                {
                                    chatService.Connected = true;
                                    chatService.SocketID = socketID;
                                    chatService.Enter();
                                }
                                else
                                {
                                    sendAll = true;
                                }
                            }
                        }
                        else
                        {
                            int socketID = BaseService.Connect(ip, port);
                            if (socketID != -1)
                            {
                                chatService = new ChatService();
                                chatService.SocketID = socketID;
                                int type = selectedRows[0].GetCell("colP5").GetInt();
                                if (type == 1)
                                {
                                    chatService.ServerIP = ip;
                                    chatService.ServerPort = port;
                                    chatService.ToServer = type == 1;
                                }
                                DataCenter.ClientChatServices[key] = chatService;
                                BaseService.AddService(chatService);
                            }
                            else
                            {
                                sendAll = true;
                            }
                        }
                        ChatData chatData = new ChatData();
                        chatData.m_content = text;
                        chatData.m_sender = DataCenter.UserName;
                        if (sendAll)
                        {
                            chatData.m_receiver = userID;
                            foreach (ChatService gs in DataCenter.ClientChatServices.Values)
                            {
                                if (gs.ToServer && gs.Connected)
                                {
                                    gs.SendAll(chatData);
                                }
                            }
                        }
                        else
                        {
                            chatService.Send(chatData);
                        }
                        if (rbBarrage.Checked)
                        {
                            CIndicator indicator = CFunctionEx.CreateIndicator("", text, this);
                            indicator.Clear();
                            indicator.Dispose();
                        }
                        else if (rbText.Checked)
                        {
                            TextBoxA txtReceive = GetTextBox("txtReceive");
                            txtReceive.Text += "i say:\r\n" + GetTextBox("txtSend").Text + "\r\n";
                            txtReceive.Invalidate();
                            if (txtReceive.VScrollBar != null && txtReceive.VScrollBar.Visible)
                            {
                                txtReceive.VScrollBar.ScrollToEnd();
                                txtReceive.Update();
                                txtReceive.Invalidate();
                            }
                        }
                    }
                }
                else if (name == "btnLogin")
                {
                    String phone = GetTextBox("txtPhone").Text.Trim();
                    String userName = GetTextBox("txtUserName").Text.Trim();
                    if (phone.Length == 0)
                    {
                        MessageBox.Show("Please input your phone!", "Attention");
                        return;
                    }
                    if (userName.Length == 0)
                    {
                        MessageBox.Show("Please input your name!", "Attention");
                        return;
                    }
                    DataCenter.UserID = phone;
                    DataCenter.UserName = userName;
                    UserCookie cookie = new UserCookie();
                    cookie.m_key = "USERINFO";
                    cookie.m_value = phone + "," + userName;
                    DataCenter.UserCookieService.AddCookie(cookie);
                    ButtonA btnLogin = GetButton("btnLogin");
                    btnLogin.Enabled = false;
                    btnLogin.Text = "Logined";
                    btnLogin.Invalidate();
                    Thread thread = new Thread(new ThreadStart(StartConnect));
                    thread.Start();

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
            if (message != null)
            {
                if (message.m_serviceID == ChatService.SERVICEID_CHAT)
                {
                    if (message.m_functionID == ChatService.FUNCTIONID_SENDALL)
                    {
                        ChatData chatData = new ChatData();
                        ChatService.GetChatData(chatData, message.m_body, message.m_bodyLength);
                        CIndicator indicator = CFunctionEx.CreateIndicator2("", chatData, this);
                        indicator.Clear();
                        indicator.Dispose();
                    }
                    else if (message.m_functionID == ChatService.FUNCTIONID_GETHOSTS)
                    {
                        List<ChatHostInfo> datas = new List<ChatHostInfo>();
                        int type = 0;
                        ChatService.GetHostInfos(datas, ref type, message.m_body, message.m_bodyLength);
                        m_gridHosts.BeginUpdate();
                        if (type != 2)
                        {
                            int datasSize = datas.Count;
                            for (int i = 0; i < datasSize; i++)
                            {
                                ChatHostInfo hostInfo = datas[i];
                                List<GridRow> rows = m_gridHosts.m_rows;
                                int rowsSize = rows.Count;
                                bool containsRow = false;
                                for (int j = 0; j < rowsSize; j++)
                                {
                                    GridRow oldRow = rows[j];
                                    if (oldRow.GetCell("colP1").GetString() == hostInfo.m_ip && oldRow.GetCell("colP2").GetInt() == hostInfo.m_serverPort)
                                    {
                                        containsRow = true;
                                    }
                                }
                                if (!containsRow)
                                {
                                    GridRow row = new GridRow();
                                    m_gridHosts.AddRow(row);
                                    row.AddCell("colP1", new GridStringCell(hostInfo.m_ip));
                                    row.AddCell("colP2", new GridIntCell(hostInfo.m_serverPort));
                                    if (hostInfo.m_type == 1)
                                    {
                                        String userID = hostInfo.m_ip + ":" + CStr.ConvertIntToStr(hostInfo.m_serverPort);
                                        row.AddCell("colP3", new GridStringCell(userID));
                                        row.AddCell("colP4", new GridStringCell(userID));
                                    }
                                    else
                                    {
                                        row.AddCell("colP3", new GridStringCell(hostInfo.m_userID));
                                        row.AddCell("colP4", new GridStringCell(hostInfo.m_userName));
                                    }
                                    row.AddCell("colP5", new GridStringCell(hostInfo.m_type == 1 ? "Server" : "Client"));
                                }
                            }
                        }
                        else
                        {
                            Dictionary<String, String> removeHosts = new Dictionary<String, String>();
                            foreach (ChatHostInfo hostInfo in datas)
                            {
                                removeHosts[hostInfo.ToString()] = "";
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
                    else if (message.m_functionID == ChatService.FUNCTIONID_SEND)
                    {
                        ChatData chatData = new ChatData();
                        ChatService.GetChatData(chatData, message.m_body, message.m_bodyLength);
                        CIndicator indicator = CFunctionEx.CreateIndicator2("", chatData, this);
                        indicator.Clear();
                        indicator.Dispose();
                    }
                }
            }
            String newStr = args as String;
            if (newStr != null)
            {
                if (newStr == "showchat")
                {
                    FlashWindow(m_mainForm.Handle, true);
                    SetForegroundWindow(m_mainForm.Handle);
                }
                else if (newStr == "shake")
                {
                    m_mainForm.Play();
                }
                else if (newStr.StartsWith("how:"))
                {
                    String text = newStr.Substring(4);
                    Form form = new Form();
                    form.Text = "HOW";
                    form.MaximizeBox = false;
                    form.MinimizeBox = false;
                    form.FormBorderStyle = FormBorderStyle.None;
                    form.BackColor = Color.Black;
                    form.WindowState = FormWindowState.Maximized;
                    form.TransparencyKey = Color.Black;
                    form.TopMost = true;
                    Label label = new Label();
                    label.ForeColor = Color.Red;
                    label.Text = text;
                    label.Font = new Font("宋体", 100, FontStyle.Bold);
                    label.Dock = DockStyle.Fill;
                    label.TextAlign = ContentAlignment.MiddleCenter;
                    label.MouseDown += new MouseEventHandler(form_MouseDown);
                    form.Controls.Add(label);
                    form.Show();
                }
                else
                {
                    TextBoxA txtReceive = GetTextBox("txtReceive");
                    txtReceive.Text += newStr;
                    txtReceive.Invalidate();
                    if (txtReceive.VScrollBar != null && txtReceive.VScrollBar.Visible)
                    {
                        txtReceive.VScrollBar.ScrollToEnd();
                        txtReceive.Update();
                        txtReceive.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// 窗体点击方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void form_MouseDown(object sender, MouseEventArgs e)
        {
            Form form = (sender as Label).Parent as Form;
            form.Close();
        }

        [DllImport("user32.dll")]
        public static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow(); //获得本窗体的句柄
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//设置此窗体为活动窗体

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
            DataCenter.ServerChatService.RegisterListener(DataCenter.ChatRequestID, new ListenerMessageCallBack(ChatMessageCallBack));
            m_barrageDiv = new BarrageDiv();
            m_barrageDiv.Dock = DockStyleA.Fill;
            m_barrageDiv.TopMost = true;
            Native.AddControl(m_barrageDiv);
            m_gridHosts = GetGrid("gridHosts");
            RegisterEvents(m_mainDiv);
            //全节点服务器
            if (DataCenter.IsFull)
            {
                DataCenter.UserID = DataCenter.HostInfo.m_localHost + ":" + CStr.ConvertIntToStr(DataCenter.HostInfo.m_localPort);
                DataCenter.UserName = DataCenter.UserID;
                Thread thread = new Thread(new ThreadStart(StartConnect));
                thread.Start();
            }
            else
            {
                UserCookie cookie = new UserCookie();
                if (DataCenter.UserCookieService.GetCookie("USERINFO", ref cookie) > 0)
                {
                    String[] strs = cookie.m_value.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    GetTextBox("txtPhone").Text = strs[0];
                    GetTextBox("txtUserName").Text = strs[1];
                }
            }
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
        /// 开始启动服务
        /// </summary>
        public void StartConnect()
        {
            List<ChatHostInfo> hostInfos = new List<ChatHostInfo>();
            UserCookie cookie = new UserCookie();
            if (DataCenter.UserCookieService.GetCookie("FULLSERVERS2", ref cookie) > 0)
            {
                hostInfos = JsonConvert.DeserializeObject<List<ChatHostInfo>>(cookie.m_value);
            }
            else
            {
                if (DataCenter.HostInfo.m_defaultHost.Length > 0)
                {
                    ChatHostInfo defaultHostInfo = new ChatHostInfo();
                    defaultHostInfo.m_ip = DataCenter.HostInfo.m_defaultHost;
                    defaultHostInfo.m_serverPort = DataCenter.HostInfo.m_defaultPort;
                    hostInfos.Add(defaultHostInfo);
                }
            }
            int hostInfosSize = hostInfos.Count;
            if (DataCenter.IsFull && hostInfosSize == 0)
            {
                ChatHostInfo defaultHostInfo = new ChatHostInfo();
                defaultHostInfo.m_ip = "127.0.0.1";
                defaultHostInfo.m_serverPort = 16666;
                hostInfos.Add(defaultHostInfo);
            }
            if (hostInfosSize > 0)
            {
                Random rd = new Random();
                while (true)
                {
                    ChatHostInfo hostInfo = hostInfos[rd.Next(0, hostInfosSize)];
                    int socketID = OwLib.BaseService.Connect(hostInfo.m_ip, hostInfo.m_serverPort);
                    if (socketID != -1)
                    {
                        String key = hostInfo.ToString();
                        if (m_mainForm != null)
                        {
                            m_mainForm.SetTitle(key);
                            m_mainForm.BeginInvoke(new EventHandler(m_mainForm.SetTitle));
                        }
                        Console.WriteLine(hostInfo.m_ip);
                        OwLib.ChatService clientChatService = new OwLib.ChatService();
                        DataCenter.ClientChatServices[key] = clientChatService;
                        OwLib.BaseService.AddService(clientChatService);
                        clientChatService.ToServer = true;
                        clientChatService.Connected = true;
                        if (!DataCenter.IsFull)
                        {
                            clientChatService.RegisterListener(DataCenter.ChatRequestID, new ListenerMessageCallBack(ChatMessageCallBack));
                        }
                        clientChatService.SocketID = socketID;
                        clientChatService.Enter();
                        return;
                    }
                }
            }
        }
    }
}
