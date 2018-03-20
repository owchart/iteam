/*****************************************************************************\
*                                                                             *
* EmailWindow.cs - Email window functions, types                      *
*                                                                             *
*               Version 1.00  ★                                              *
*                                                                             *
*               Copyright (c) 2017-2017, iTeam. All rights reserved.      *
*               Created by Todd 2017/6/19.                          *
*                                                                             *
******************************************************************************/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using OpenPop.Pop3;

namespace OwLib
{
    /// <summary>
    /// 上级指示窗体
    /// </summary>
    public class EmailWindow : UIXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public EmailWindow()
        {
            
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridEmail;

        /// <summary>
        /// 查询按钮、重置按钮点击事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="mp">坐标</param>
        /// <param name="button">按钮</param>
        /// <param name="clicks">点击事件</param>
        /// <param name="delta">滚轮滚动值</param>
        private void ClickButton(object sender, POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            if (button == MouseButtonsA.Left && clicks == 1)
            {
                ControlA control = sender as ControlA;
                String name = control.Name;
            }
        }

        /// <summary>
        /// 加载XML
        /// </summary>
        /// <param name="xmlPath">XML路径</param>
        public override void Load(String xmlPath)
        {
            LoadFile(xmlPath, null);
            RegisterEvents(Native.GetControls()[0]);
            m_gridEmail = GetGrid("gridFollows");
            String dir = DataCenter.GetAppPath() + "\\email";
            if (!CFileA.IsDirectoryExist(dir))
            {
                CFileA.CreateDirectory(dir);
            }
            ReadPop3();
        }

        /// <summary>
        /// 读取邮件
        /// </summary>
        public void ReadPop3()
        {
            String dir = DataCenter.GetAppPath() + "\\email";
            using (Pop3Client client = new Pop3Client())
            {
                if (client.Connected)
                {
                    client.Disconnect();
                }
                client.Connect("imap.exmail.qq.com", 110, false);
                client.Authenticate("TaoDe@gaiafintech.com", "Gaia12345678", OpenPop.Pop3.AuthenticationMethod.UsernameAndPassword);
                int messageCount = client.GetMessageCount();
                DateTime dt = DateTime.MinValue;
                if (CFileA.IsFileExist(dir + "\\datetime"))
                {
                    String content = "";
                    CFileA.Read(dir + "\\datetime", ref content);
                    dt = Convert.ToDateTime(content);
                }
                DateTime writeTime = DateTime.MinValue;
                for(int i = messageCount; i >= 1; i--)
                {
                    OpenPop.Mime.Message message = client.GetMessage(i);
                    if (message != null)
                    {
                        try
                        {
                            string sender = message.Headers.From.DisplayName;
                            string from = message.Headers.From.Address;
                            if (from.IndexOf("zhaopinmail.com") != -1)
                            {
                                string subject = message.Headers.Subject;
                                DateTime Datesent = message.Headers.DateSent;
                                if (Datesent >= dt)
                                {
                                    OpenPop.Mime.MessagePart messagePart = message.MessagePart;
                                    string body = " ";
                                    if (messagePart.IsText)
                                    {
                                        body = messagePart.GetBodyAsText();
                                    }
                                    else if (messagePart.IsMultiPart)
                                    {
                                        String key = sender + " " + Datesent.ToString("yyyyMMddHHmmss");
                                        String file = dir + "\\" + key + ".html";
                                        if (writeTime < Datesent)
                                        {
                                            writeTime = Datesent;
                                            CFileA.Write(dir + "\\datetime", writeTime.ToString());
                                        }
                                        if (!CFileA.IsFileExist(file))
                                        {
                                            OpenPop.Mime.MessagePart plainTextPart = message.FindFirstPlainTextVersion();
                                            if (plainTextPart != null)
                                            {
                                                body = plainTextPart.GetBodyAsText();
                                            }
                                            else
                                            {
                                                List<OpenPop.Mime.MessagePart> textVersions = message.FindAllTextVersions();
                                                if (textVersions.Count >= 1)
                                                    body = textVersions[0].GetBodyAsText();
                                                else
                                                    body = "<<OpenPop>> Cannot find a text version body in this message.";
                                            }
                                            if (body != null && body.Length > 0)
                                            {
                                                CFileA.Write(file, body);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }

        /// 注册事件
        /// </summary>
        /// <param name="control">控件</param>
        private void RegisterEvents(ControlA control)
        {
            ControlMouseEvent clickButtonEvent = new ControlMouseEvent(ClickButton);
            List<ControlA> controls = control.GetControls();
            int controlsSize = controls.Count;
            for (int i = 0; i < controlsSize; i++)
            {
                ControlA subControl = controls[i];
                ButtonA button = controls[i] as ButtonA;
                LinkLabelA linkLabel = subControl as LinkLabelA;
                GridColumn column = subControl as GridColumn;
                GridA grid = subControl as GridA;
                CheckBoxA checkBox = subControl as CheckBoxA;
                if (column != null)
                {
                    column.AllowDrag = true;
                    column.AllowResize = true;
                    column.BackColor = CDraw.PCOLORS_BACKCOLOR;
                    column.Font = new FONT("微软雅黑", 12, false, false, false);
                    column.ForeColor = CDraw.PCOLORS_FORECOLOR;
                }
                else if (button != null)
                {
                    button.RegisterEvent(clickButtonEvent, EVENTID.CLICK);
                }
                else if (linkLabel != null)
                {
                    linkLabel.RegisterEvent(clickButtonEvent, EVENTID.CLICK);
                }
                else if (grid != null)
                {
                    grid.GridLineColor = COLOR.CONTROLBORDER;
                    grid.RowStyle.HoveredBackColor = CDraw.PCOLORS_HOVEREDROWCOLOR;
                    grid.RowStyle.SelectedBackColor = CDraw.PCOLORS_SELECTEDROWCOLOR;
                    grid.RowStyle.SelectedForeColor = CDraw.PCOLORS_FORECOLOR4;
                    grid.RowStyle.Font = new FONT("微软雅黑", 12, false, false, false);
                    GridRowStyle alternateRowStyle = new GridRowStyle();
                    alternateRowStyle.BackColor = CDraw.PCOLORS_ALTERNATEROWCOLOR;
                    alternateRowStyle.HoveredBackColor = CDraw.PCOLORS_HOVEREDROWCOLOR;
                    alternateRowStyle.SelectedBackColor = CDraw.PCOLORS_SELECTEDROWCOLOR;
                    alternateRowStyle.SelectedForeColor = CDraw.PCOLORS_FORECOLOR4;
                    alternateRowStyle.Font = new FONT("微软雅黑", 12, false, false, false);
                    grid.AlternateRowStyle = alternateRowStyle;
                    grid.UseAnimation = true;
                }
                RegisterEvents(controls[i]);
            }
        }
    }
}
