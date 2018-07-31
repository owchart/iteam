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
using System.IO;

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
        /// 聊天组
        /// </summary>
        private List<ChatGroup> m_chatGroups = new List<ChatGroup>();

        /// <summary>
        /// 当前的组名称
        /// </summary>
        private String m_currentGroupName = "";

        /// <summary>
        /// 组列表
        /// </summary>
        private GridA m_gridGroups;

        /// <summary>
        /// 主机列表
        /// </summary>
        private GridA m_gridHosts;

        private BarrageForm m_barrageForm;

        /// <summary>
        /// 获取或设置弹幕窗体
        /// </summary>
        public BarrageForm BarrageForm
        {
            get { return m_barrageForm; }
            set { m_barrageForm = value; }
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
        /// 绑定所有的群组
        /// </summary>
        private void BindGroups()
        {
            m_gridGroups.UseAnimation = true;
            List<GridRow> rows = m_gridGroups.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow row = rows[i];
                if (row.EditButton != null)
                {
                    m_gridGroups.RemoveControl(row.EditButton);
                    row.EditButton = null;
                }
                m_gridGroups.RemoveRow(row);
                i--;
                rowsSize--;
            }
            m_gridGroups.Update();
            GridRow firstRow = new GridRow();
            m_gridGroups.AddRow(firstRow);
            GridStringCell cell1 = new GridStringCell("");
            firstRow.AddCell("colG1", cell1);
            GridStringCell cell2 = new GridStringCell("全部");
            firstRow.AddCell("colG2", cell2);
            GridStringCell cell3 = new GridStringCell("");
            firstRow.AddCell("colG3", cell3);
            int groupsSize = m_chatGroups.Count;
            for (int i = 0; i < groupsSize; i++)
            {
                ChatGroup chatGroup = m_chatGroups[i];
                GridRow cRow = new GridRow();
                m_gridGroups.AddRow(cRow);
                ButtonA deleteButton = new ButtonA();
                deleteButton.Height = cRow.Height;
                deleteButton.Name = "btnDelete";
                deleteButton.Tag = chatGroup.Name;
                deleteButton.BackColor = COLOR.ARGB(255, 0, 0);
                deleteButton.Native = m_gridHosts.Native;
                deleteButton.Text = "删除";
                cRow.EditButton = deleteButton;
                cRow.AllowEdit = true;
                GridStringCell cCell1 = new GridStringCell(chatGroup.Name);
                cRow.AddCell("colG1", cCell1);
                GridStringCell cCell2 = new GridStringCell(chatGroup.DisplayName);
                cRow.AddCell("colG2", cCell2);
                String strIDs = "";
                int userIDsSize = chatGroup.UserIDs.Count;
                for (int j = 0; j < userIDsSize; j++)
                {
                    strIDs += chatGroup.UserIDs[j];
                    if (j != userIDsSize - 1)
                    {
                        strIDs += ",";
                    }
                }
                GridStringCell cCell3 = new GridStringCell(strIDs);
                cRow.AddCell("colG3", cCell3);
   
                ControlMouseEvent clickButtonEvent = new ControlMouseEvent(ClickEvent);
                deleteButton.RegisterEvent(clickButtonEvent, EVENTID.CLICK);
            }
            m_gridGroups.Update();
            m_gridGroups.Invalidate();
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
                    SendAll();
                }
                else if (name == "btnSend")
                {
                    List<GridRow> selectedRows = m_gridHosts.SelectedRows;
                    Send(selectedRows);
                }
                else if (name == "btnSendGroup")
                {
                    List<GridRow> sendRows = new List<GridRow>();
                    foreach (GridRow row in m_gridHosts.m_rows)
                    {
                        if (row.Visible)
                        {
                            sendRows.Add(row);
                        }
                    }
                    Send(sendRows);
                    sendRows.Clear();
                }
                else if (name == "btnLogin")
                {
                    Login();
                }
                else if (name == "btnSaveGroup")
                {
                    SaveGroup();
                }
                else if (name == "btnDelete")
                {
                    String groupName = (sender as ButtonA).Tag.ToString();
                    int chatGroupsSize = m_chatGroups.Count;
                    for (int i = 0; i < chatGroupsSize; i++)
                    {
                        ChatGroup chatGroup = m_chatGroups[i];
                        if (chatGroup.Name == groupName)
                        {
                            m_chatGroups.Remove(chatGroup);
                            i--;
                            chatGroupsSize--;
                        }
                    }
                    if (m_currentGroupName == groupName)
                    {
                        m_currentGroupName = "";
                    }
                    ChatGroup.SaveGroups(m_chatGroups);
                    BindGroups();
                    SetHostGridRowVisible();
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
        /// 表格单元格点击事件
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="cell">单元格</param>
        /// <param name="mp">坐标</param>
        /// <param name="buttons">按钮</param>
        /// <param name="clicks">点击次数</param>
        /// <param name="delta">滚轮值</param>
        private void GridCellClick(object sender, GridCell cell, POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            if (clicks == 2)
            {
                m_currentGroupName = cell.Row.GetCell("colG1").GetString();
                SetHostGridRowVisible();
            }
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
                                        row.AddCell("colP3", new GridStringCell("--"));
                                        row.AddCell("colP4", new GridStringCell("--"));
                                    }
                                    else
                                    {
                                        row.AddCell("colP3", new GridStringCell(hostInfo.m_userID));
                                        row.AddCell("colP4", new GridStringCell(hostInfo.m_userName));
                                    }
                                    row.AddCell("colP5", new GridStringCell(hostInfo.m_type == 1 ? "服务器" : "客户端"));
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
                        SetHostGridRowVisible();
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
                    Barrage barrage = new Barrage();
                    barrage.Font = new FONT("宋体", 100, true, false, false);
                    barrage.Text = text;
                    barrage.Mode = 1;
                    m_barrageForm.BarrageDiv.AddBarrage(barrage);
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
        /// 登陆
        /// </summary>
        private void Login()
        {
            String phone = GetTextBox("txtPhone").Text.Trim();
            String userName = GetTextBox("txtUserName").Text.Trim();
            if (phone.Length == 0)
            {
                MessageBox.Show("请输入手机号码!", "提示");
                return;
            }
            if (userName.Length == 0)
            {
                MessageBox.Show("请输入姓名!", "提示");
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
            btnLogin.Text = "已登陆";
            btnLogin.Invalidate();
            Thread thread = new Thread(new ThreadStart(StartConnect));
            thread.Start();
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
            m_gridHosts = GetGrid("gridHosts");
            m_gridHosts.GridLineColor = COLOR.CONTROLBORDER;
            GridRowStyle rowStyle = new GridRowStyle();
            rowStyle.HoveredBackColor = CDraw.PCOLORS_HOVEREDROWCOLOR;
            rowStyle.SelectedBackColor = CDraw.PCOLORS_SELECTEDROWCOLOR;
            rowStyle.SelectedForeColor = CDraw.PCOLORS_FORECOLOR4;
            rowStyle.Font = new FONT("微软雅黑", 12, false, false, false);
            m_gridHosts.RowStyle = rowStyle;
            GridRowStyle alternateRowStyle = new GridRowStyle();
            alternateRowStyle.BackColor = CDraw.PCOLORS_ALTERNATEROWCOLOR;
            alternateRowStyle.HoveredBackColor = CDraw.PCOLORS_HOVEREDROWCOLOR;
            alternateRowStyle.SelectedBackColor = CDraw.PCOLORS_SELECTEDROWCOLOR;
            alternateRowStyle.SelectedForeColor = CDraw.PCOLORS_FORECOLOR4;
            alternateRowStyle.Font = new FONT("微软雅黑", 12, false, false, false);
            m_gridHosts.AlternateRowStyle = alternateRowStyle;
            m_gridGroups = GetGrid("gridGroups");
            m_gridGroups.RowStyle = rowStyle;
            m_gridGroups.AlternateRowStyle = alternateRowStyle;
            m_gridGroups.RegisterEvent(new GridCellMouseEvent(GridCellClick), EVENTID.GRIDCELLCLICK);
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
            m_chatGroups = ChatGroup.ReadGroups();
            BindGroups();
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

        /// <summary>
        /// 保存为群
        /// </summary>
        private void SaveGroup()
        {
            String groupName = GetTextBox("txtGroupName").Text;
            if (groupName == null || groupName.Length == 0)
            {
                MessageBox.Show("请输入群的名称!", "提示");
                return;
            }
            List<GridRow> selectedRows = m_gridHosts.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 1)
            {
                List<String> userIDs = new List<String>();
                for (int i = 0; i < selectedRowsSize; i++)
                {
                    userIDs.Add(selectedRows[i].GetCell("colP3").GetString());
                }
                ChatGroup chatGroup = new ChatGroup();
                chatGroup.Name = System.Guid.NewGuid().ToString();
                chatGroup.DisplayName = groupName;
                chatGroup.UserIDs = userIDs;
                m_chatGroups.Add(chatGroup);
                ChatGroup.SaveGroups(m_chatGroups);
                BindGroups();
            }
            else
            {
                MessageBox.Show("请选择至少两个人!", "提示");
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        private void Send(List<GridRow> rows)
        {
            byte[] fileBytes = null;
            String text = GetTextBox("txtSend").Text;
            RadioButtonA rbBarrage = GetRadioButton("rbBarrage");
            RadioButtonA rbText = GetRadioButton("rbText");
            RadioButtonA rbFile = GetRadioButton("rbFile");
            RadioButtonA rbAttention = GetRadioButton("rbAttention");
            String sayText = text;
            if (rbFile.Checked)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    text = "sendfile('" + new FileInfo(openFileDialog.FileName).Name + "');";
                    fileBytes = File.ReadAllBytes(openFileDialog.FileName);
                    sayText = text;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (text == null || text.Trim().Length == 0)
                {
                    MessageBox.Show("请输入你想说的内容!", "提示");
                }
            }
            if (rbBarrage.Checked)
            {
                text = "addbarrage('" + text + "');";
            }
            else if (rbText.Checked)
            {
                text = "addtext('" + text + "');";
            }
            else if (rbAttention.Checked)
            {
                text = "how('" + GetTextBox("txtUserName").Text + "说:" + text + "');";
            }
            int rowsSize = rows.Count;
            bool sendAll = false;
            if (rowsSize > 0)
            {
                for (int i = 0; i < rowsSize; i++)
                {
                    GridRow thisRow = rows[i];
                    String ip = thisRow.GetCell("colP1").GetString();
                    int port = thisRow.GetCell("colP2").GetInt();
                    String userID = thisRow.GetCell("colP3").GetString();
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
                            int type = thisRow.GetCell("colP5").GetInt();
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
                    if (fileBytes != null)
                    {
                        chatData.m_body = fileBytes;
                        chatData.m_bodyLength = fileBytes.Length;
                    }
                    chatData.m_from = DataCenter.UserName;
                    if (sendAll)
                    {
                        chatData.m_to = userID;
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
                    TextBoxA txtReceive = GetTextBox("txtReceive");
                    txtReceive.Text += "我说:\r\n" + sayText + "\r\n";
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
        /// 发送全体消息
        /// </summary>
        private void SendAll()
        {
            byte[] fileBytes = null;
            RadioButtonA rbBarrage = GetRadioButton("rbBarrage");
            RadioButtonA rbText = GetRadioButton("rbText");
            RadioButtonA rbFile = GetRadioButton("rbFile");
            RadioButtonA rbAttention = GetRadioButton("rbAttention");
            String text = GetTextBox("txtSend").Text;
            String sayText = text;
            if (rbFile.Checked)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    text = "sendfile('" + new FileInfo(openFileDialog.FileName).Name + "');";
                    fileBytes = File.ReadAllBytes(openFileDialog.FileName);
                    sayText = text;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (text == null || text.Trim().Length == 0)
                {
                    MessageBox.Show("请输入你想说的内容!", "提示");
                }
            }
            if (rbBarrage.Checked)
            {
                text = "addbarrage('" + text + "');";
            }
            else if (rbText.Checked)
            {
                text = "addtext('" + text + "');";
            }
            else if (rbAttention.Checked)
            {
                text = "how('" + GetTextBox("txtUserName").Text + "说:" + text + "');";
            }
            ChatData chatData = new ChatData();
            chatData.m_content = text;
            if (fileBytes != null)
            {
                chatData.m_body = fileBytes;
                chatData.m_bodyLength = fileBytes.Length;
            }
            chatData.m_from = DataCenter.UserName;
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
            TextBoxA txtReceive = GetTextBox("txtReceive");
            txtReceive.Text += "我说:\r\n" + sayText + "\r\n";
            txtReceive.Invalidate();
            if (txtReceive.VScrollBar != null && txtReceive.VScrollBar.Visible)
            {
                txtReceive.VScrollBar.ScrollToEnd();
                txtReceive.Update();
                txtReceive.Invalidate();
            }
        }

        /// <summary>
        /// 设置主机表格可见
        /// </summary>
        private void SetHostGridRowVisible()
        {
            ChatGroup chatGroup = null;
            int chatGroupsSize = m_chatGroups.Count;
            for (int i = 0; i < chatGroupsSize; i++)
            {
                if (m_chatGroups[i].Name == m_currentGroupName)
                {
                    chatGroup = m_chatGroups[i];
                    break;
                }
            }
            List<GridRow> rows = m_gridHosts.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow row = rows[i];
                if (m_currentGroupName == "")
                {
                    row.Visible = true;
                }
                else
                {
                    if (chatGroup != null)
                    {
                        if (chatGroup.UserIDs.Contains(row.GetCell("colP3").GetString()))
                        {
                            row.Visible = true;
                        }
                        else
                        {
                            row.Visible = false;
                        }
                    }
                    else
                    {
                        row.Visible = true;
                    }
                }
            }
            m_gridHosts.Update();
            m_gridHosts.Invalidate();
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
            if (DataCenter.UserCookieService.GetCookie("DANDANSERVERS", ref cookie) > 0)
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

    /// <summary>
    /// 聊天群组
    /// </summary>
    public class ChatGroup
    {
        private String m_displayName = "";

        /// <summary>
        /// 获取或设置显示名称
        /// </summary>
        public String DisplayName
        {
            get { return m_displayName; }
            set { m_displayName = value; }
        }

        private String m_name = "";

        /// <summary>
        /// 获取或设置唯一名称
        /// </summary>
        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private List<String> m_userIDs = new List<String>();

        /// <summary>
        /// 获取或设置用户ID
        /// </summary>
        public List<String> UserIDs
        {
            get { return m_userIDs; }
            set { m_userIDs = value; }
        }

        /// <summary>
        /// Json到组列表对象
        /// </summary>
        /// <param name="json">Json字符串</param>
        /// <returns>组列表</returns>
        public static List<ChatGroup> ReadGroups()
        {
            String file = DataCenter.GetAppPath() + "\\groups.txt";
            String content = "";
            if (CFileA.IsFileExist(file))
            {
                CFileA.Read(file, ref content);
                return JsonConvert.DeserializeObject<List<ChatGroup>>(content);
            }
            else
            {
                return new List<ChatGroup>();
            }   
        }

        /// <summary>
        /// 保存组信息
        /// </summary>
        /// <param name="groups">组列表</param>
        public static void SaveGroups(List<ChatGroup> groups)
        {
            String file = DataCenter.GetAppPath() + "\\groups.txt";
            CFileA.Write(file, JsonConvert.SerializeObject(groups));
        }
    }
}
