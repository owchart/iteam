/*****************************************************************************\
*                                                                             *
* MainFrame.cs -  MainFrame functions, types, and definitions.                *
*                                                                             *
*               Version 1.00  ����                                          *
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
    /// ϵͳ
    /// </summary>
    public class MainFrame : UIXmlEx, IDisposable
    {
        /// <summary>
        /// ����ϵͳ
        /// </summary>
        public MainFrame()
        {
        }

        /// <summary>
        /// ������
        /// </summary>
        private List<ChatGroup> m_chatGroups = new List<ChatGroup>();

        /// <summary>
        /// ��ǰ��������
        /// </summary>
        private String m_currentGroupName = "";

        /// <summary>
        /// ���б�
        /// </summary>
        private GridA m_gridGroups;

        /// <summary>
        /// �����б�
        /// </summary>
        private GridA m_gridHosts;

        private BarrageForm m_barrageForm;

        /// <summary>
        /// ��ȡ�����õ�Ļ����
        /// </summary>
        public BarrageForm BarrageForm
        {
            get { return m_barrageForm; }
            set { m_barrageForm = value; }
        }

        private DivA m_mainDiv;

        /// <summary>
        /// ��ȡ��������ͼ��
        /// </summary>
        public DivA MainDiv
        {
            get { return m_mainDiv; }
            set { m_mainDiv = value; }
        }

        private MainForm m_mainForm;

        /// <summary>
        /// ��ȡ������������
        /// </summary>
        public MainForm MainForm
        {
            get { return m_mainForm; }
            set { m_mainForm = value; }
        }

        /// <summary>
        /// �����е�Ⱥ��
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
            GridStringCell cell2 = new GridStringCell("ȫ��");
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
                deleteButton.Text = "ɾ��";
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
        /// ���������ݻص�
        /// </summary>
        /// <param name="message">��Ϣ</param>
        public void ChatMessageCallBack(CMessage message)
        {
            m_mainDiv.BeginInvoke(message);
        }

        /// <summary>
        /// ����¼�
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="mp">����</param>
        /// <param name="button">��ť</param>
        /// <param name="clicks">�������</param>
        /// <param name="delta">����ֵ/param>
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
        /// ������Դ����
        /// </summary>
        public override void Dispose()
        {
        }

        /// <summary>
        /// �˳�����
        /// </summary>
        public override void Exit()
        {

        }

        /// <summary>
        /// ���Ԫ�����¼�
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="cell">��Ԫ��</param>
        /// <param name="mp">����</param>
        /// <param name="buttons">��ť</param>
        /// <param name="clicks">�������</param>
        /// <param name="delta">����ֵ</param>
        private void GridCellClick(object sender, GridCell cell, POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            if (clicks == 2)
            {
                m_currentGroupName = cell.Row.GetCell("colG1").GetString();
                SetHostGridRowVisible();
            }
        }

        /// <summary>
        /// �������̷߳�����
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="args">����</param>
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
                                    row.AddCell("colP5", new GridStringCell(hostInfo.m_type == 1 ? "������" : "�ͻ���"));
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
                    barrage.Font = new FONT("����", 100, true, false, false);
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
        /// ��½
        /// </summary>
        private void Login()
        {
            String phone = GetTextBox("txtPhone").Text.Trim();
            String userName = GetTextBox("txtUserName").Text.Trim();
            if (phone.Length == 0)
            {
                MessageBox.Show("�������ֻ�����!", "��ʾ");
                return;
            }
            if (userName.Length == 0)
            {
                MessageBox.Show("����������!", "��ʾ");
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
            btnLogin.Text = "�ѵ�½";
            btnLogin.Invalidate();
            Thread thread = new Thread(new ThreadStart(StartConnect));
            thread.Start();
        }

        [DllImport("user32.dll")]
        public static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow(); //��ñ�����ľ��
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//���ô˴���Ϊ�����

        /// <summary>
        /// ����XML
        /// </summary>
        /// <param name="xmlPath">XML·��</param>
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
            rowStyle.Font = new FONT("΢���ź�", 12, false, false, false);
            m_gridHosts.RowStyle = rowStyle;
            GridRowStyle alternateRowStyle = new GridRowStyle();
            alternateRowStyle.BackColor = CDraw.PCOLORS_ALTERNATEROWCOLOR;
            alternateRowStyle.HoveredBackColor = CDraw.PCOLORS_HOVEREDROWCOLOR;
            alternateRowStyle.SelectedBackColor = CDraw.PCOLORS_SELECTEDROWCOLOR;
            alternateRowStyle.SelectedForeColor = CDraw.PCOLORS_FORECOLOR4;
            alternateRowStyle.Font = new FONT("΢���ź�", 12, false, false, false);
            m_gridHosts.AlternateRowStyle = alternateRowStyle;
            m_gridGroups = GetGrid("gridGroups");
            m_gridGroups.RowStyle = rowStyle;
            m_gridGroups.AlternateRowStyle = alternateRowStyle;
            m_gridGroups.RegisterEvent(new GridCellMouseEvent(GridCellClick), EVENTID.GRIDCELLCLICK);
            RegisterEvents(m_mainDiv);
            //ȫ�ڵ������
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
        /// �ػ�˵�����
        /// </summary>
        /// <param name="sender">���ö���</param>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clipRect">�ü�����</param>
        private void PaintLayoutDiv(object sender, CPaint paint, OwLib.RECT clipRect)
        {
            ControlA control = sender as ControlA;
            int width = control.Width, height = control.Height;
            OwLib.RECT drawRect = new OwLib.RECT(0, 0, width, height);
            paint.FillGradientRect(CDraw.PCOLORS_BACKCOLOR, CDraw.PCOLORS_BACKCOLOR2, drawRect, 0, 90);
        }

        /// <summary>
        /// ����ΪȺ
        /// </summary>
        private void SaveGroup()
        {
            String groupName = GetTextBox("txtGroupName").Text;
            if (groupName == null || groupName.Length == 0)
            {
                MessageBox.Show("������Ⱥ������!", "��ʾ");
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
                MessageBox.Show("��ѡ������������!", "��ʾ");
            }
        }

        /// <summary>
        /// ������Ϣ
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
                    MessageBox.Show("����������˵������!", "��ʾ");
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
                text = "how('" + GetTextBox("txtUserName").Text + "˵:" + text + "');";
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
                    txtReceive.Text += "��˵:\r\n" + sayText + "\r\n";
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
        /// ����ȫ����Ϣ
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
                    MessageBox.Show("����������˵������!", "��ʾ");
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
                text = "how('" + GetTextBox("txtUserName").Text + "˵:" + text + "');";
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
            txtReceive.Text += "��˵:\r\n" + sayText + "\r\n";
            txtReceive.Invalidate();
            if (txtReceive.VScrollBar != null && txtReceive.VScrollBar.Visible)
            {
                txtReceive.VScrollBar.ScrollToEnd();
                txtReceive.Update();
                txtReceive.Invalidate();
            }
        }

        /// <summary>
        /// �����������ɼ�
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

        /// ע���¼�
        /// </summary>
        /// <param name="control">�ؼ�</param>
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
        /// ��ʼ��������
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
    /// ����Ⱥ��
    /// </summary>
    public class ChatGroup
    {
        private String m_displayName = "";

        /// <summary>
        /// ��ȡ��������ʾ����
        /// </summary>
        public String DisplayName
        {
            get { return m_displayName; }
            set { m_displayName = value; }
        }

        private String m_name = "";

        /// <summary>
        /// ��ȡ������Ψһ����
        /// </summary>
        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private List<String> m_userIDs = new List<String>();

        /// <summary>
        /// ��ȡ�������û�ID
        /// </summary>
        public List<String> UserIDs
        {
            get { return m_userIDs; }
            set { m_userIDs = value; }
        }

        /// <summary>
        /// Json�����б����
        /// </summary>
        /// <param name="json">Json�ַ���</param>
        /// <returns>���б�</returns>
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
        /// ��������Ϣ
        /// </summary>
        /// <param name="groups">���б�</param>
        public static void SaveGroups(List<ChatGroup> groups)
        {
            String file = DataCenter.GetAppPath() + "\\groups.txt";
            CFileA.Write(file, JsonConvert.SerializeObject(groups));
        }
    }
}
