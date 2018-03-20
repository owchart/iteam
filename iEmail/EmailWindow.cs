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
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using OpenPop.Pop3;
using System.Threading;
using System.Diagnostics;

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
        /// 过滤年龄
        /// </summary>
        private int m_filterAge = 40;

        /// <summary>
        /// 过滤日期
        /// </summary>
        private int m_filterDate = 10;

        /// <summary>
        /// 过滤户口
        /// </summary>
        private String m_filterHuKou = "";

        /// <summary>
        /// 过滤婚姻
        /// </summary>
        private String m_filterMarry = "全部";

        /// <summary>
        /// 过滤性别
        /// </summary>
        private String m_filterSex = "全部";

        /// <summary>
        /// 过滤状态
        /// </summary>
        private String m_filterStatus = "全部";

        /// <summary>
        /// 过滤学历
        /// </summary>
        private String m_filterXueli = "全部";

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridEmail;

        /// <summary>
        /// 绑定表格
        /// </summary>
        public void BindGrid(String[] files)
        {
            int filesSize = files.Length;
            m_gridEmail.BeginUpdate();
            for (int i = filesSize - 1; i >= 0; i--)
            {
                String file = files[i];
                String content = "";
                CFileA.Read(file, ref content);
                if (content != null && content.Length > 100)
                {
                    GridRow row = new GridRow();
                    m_gridEmail.AddRow(row);
                    String date = file.Substring(file.LastIndexOf("\\") + 1);
                    String identifier1 = "padding-left:12px;line-height:50px\">";
                    int year = CStr.ConvertStrToInt(date.Substring(0, 4));
                    int month = CStr.ConvertStrToInt(date.Substring(4, 2));
                    int day = CStr.ConvertStrToInt(date.Substring(6, 2));
                    int hour = CStr.ConvertStrToInt(date.Substring(8, 2));
                    int minute = CStr.ConvertStrToInt(date.Substring(10, 2));
                    int second = CStr.ConvertStrToInt(date.Substring(12, 2));
                    int idx = content.IndexOf(identifier1);
                    int nidx = content.IndexOf("</td>", idx + 1);
                    DateTime dt = new DateTime(year, month, day, hour, minute, second);
                    String name = content.Substring(idx + identifier1.Length, nidx - idx - identifier1.Length);
                    String identifier2 = "</small><font style=\"font-weight:bold\">";
                    int idx2 = content.IndexOf(identifier2, nidx);
                    int nidx2 = content.IndexOf("</font>", idx2 + 1);
                    String birthday = content.Substring(idx2 + identifier2.Length, nidx2 - idx2 - identifier2.Length);
                    if (birthday.IndexOf("工作经验") != -1)
                    {
                        idx2 = content.IndexOf(identifier2, nidx2);
                        nidx2 = content.IndexOf("</font>", idx2 + 1);
                        birthday = content.Substring(idx2 + identifier2.Length, nidx2 - idx2 - identifier2.Length);
                    }
                    String marry = "未知";
                    if (content.IndexOf("已婚") != -1)
                    {
                        marry = "已婚";
                    }
                    else if (content.IndexOf("未婚") != -1)
                    {
                        marry = "未婚";
                    }
                    String sex = "未知";
                    if (content.IndexOf("style=\"font-weight:bold\">男</font>") != -1)
                    {
                        sex = "男";
                    }
                    else if (content.IndexOf("style=\"font-weight:bold\">女</font>") != -1)
                    {
                        sex = "女";
                    }
                    int hukouIndex = content.IndexOf("户口");
                    String hukou = "";
                    if (hukouIndex != -1)
                    {
                        String identifier4 = "</small>";
                        int idx4 = content.IndexOf(identifier4, hukouIndex - 20);
                        hukou = content.Substring(idx4 + identifier4.Length, hukouIndex - idx4 - identifier4.Length);
                    }
                    String xueli = "未知";
                    if (content.IndexOf(">高中<") != -1)
                    {
                        xueli = "高中";
                    }
                    else if (content.IndexOf(">中技<") != -1)
                    {
                        xueli = "中技";
                    }
                    else if (content.IndexOf(">中专<") != -1)
                    {
                        xueli = "中专";
                    }
                    else if (content.IndexOf(">大专<") != -1)
                    {
                        xueli = "大专";
                    }
                    else if (content.IndexOf(">大专在读<") != -1)
                    {
                        xueli = "大专在读";
                    }
                    else if (content.IndexOf(">本科<") != -1)
                    {
                        xueli = "本科";
                    }
                    else if (content.IndexOf(">本科在读<") != -1 || content.IndexOf("&nbsp;本科<") != -1)
                    {
                        xueli = "本科在读";
                    }
                    else if (content.IndexOf(">硕士<") != -1)
                    {
                        xueli = "硕士";
                    }
                    else if (content.IndexOf(">其他<") != -1)
                    {
                        xueli = "其他";
                    }
                    else
                    {
                        Console.WriteLine("1");
                    }
                    String state = "在职";
                    if (content.IndexOf("我目前处于离职状态") != -1)
                    {
                        state = "离职";
                    }
                    String salary = "未知";
                    int salaryIndex = content.IndexOf("期望月薪");
                    if (salaryIndex != -1)
                    {
                        int salaryIndex2 = content.IndexOf("元/月", salaryIndex);
                        if (salaryIndex2 != -1)
                        {
                            String identifier5 = ">";
                            int idx5 = content.IndexOf(identifier5, salaryIndex2 - 20);
                            salary = content.Substring(idx5 + identifier5.Length, salaryIndex2 - idx5 - identifier5.Length);
                            if (Math.Abs(salaryIndex - salaryIndex2) < 50)
                            {
                                salary = "期望" + salary;
                            }
                            else
                            {
                                salary = "目前" + salary;
                            }
                        }
                    }
                    String zhiye = "未知";
                    int zhiyeIndex = content.IndexOf("期望从事职业");
                    if (zhiyeIndex != -1)
                    {
                        String identifier6 = "valign=\"top\">";
                        int idx6 = content.IndexOf(identifier6, zhiyeIndex + 20);
                        int nidx6 = content.IndexOf("</td>", idx6 + 1);
                        zhiye = content.Substring(idx6 + identifier6.Length, nidx6 - idx6 - identifier6.Length);
                    }
                    int jingYan = 0;
                    int startIndex = 0;
                    while (true)
                    {
                        int index = content.IndexOf("企业性质", startIndex);
                        if (index != -1)
                        {
                            jingYan++;
                            startIndex = index + 50;
                        }
                        else
                        {
                            break;
                        }
                    }
                    row.AddCell("colP1", new GridStringCell(dt.ToString("yyyy-MM-dd HH:mm:ss")));
                    row.AddCell("colP2", new GridStringCell(name));
                    row.AddCell("colP3", new GridStringCell(birthday));
                    row.AddCell("colP4", new GridStringCell(marry));
                    row.AddCell("colP5", new GridStringCell(sex));
                    row.AddCell("colP6", new GridStringCell(hukou));
                    row.AddCell("colP7", new GridStringCell(xueli));
                    row.AddCell("colP8", new GridStringCell(state));
                    row.AddCell("colP9", new GridStringCell(salary));
                    row.AddCell("colP10", new GridStringCell(zhiye));
                    row.AddCell("colP11", new GridStringCell(jingYan.ToString() + "次"));
                    row.AddCell("colP12", new GridStringCell(file));
                }
            }
            FilterGrid();
            m_gridEmail.EndUpdate();
            m_gridEmail.Invalidate();
        }

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
                if (name == "btnApply")
                {
                    m_filterAge = (int)GetSpin("spinAge").Value;
                    m_filterDate = (int)GetSpin("spinDate").Value;
                    m_filterHuKou = GetTextBox("txtHukou").Text;
                    m_filterMarry = GetComboBox("cbMarry").Text;
                    m_filterSex = GetComboBox("cbSex").Text;
                    m_filterStatus = GetComboBox("cbStatus").Text;
                    m_filterXueli = GetComboBox("cbXueli").Text;
                    FilterGrid();
                    m_gridEmail.Update();
                    m_gridEmail.Invalidate();
                }
            }
        }

        /// <summary>
        /// 过滤表格
        /// </summary>
        private void FilterGrid()
        {
            List<GridRow> rows = m_gridEmail.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                bool visible = true;
                GridRow row = rows[i];
                if (m_filterSex != "全部")
                {
                    visible = row.GetCell("colP5").GetString() == m_filterSex;
                }
                if (visible)
                {
                    if (m_filterMarry != "全部")
                    {
                        visible = row.GetCell("colP4").GetString() == m_filterMarry;
                    }
                }
                if (visible)
                {
                    if (m_filterStatus != "全部")
                    {
                        visible = row.GetCell("colP8").GetString() == m_filterStatus;
                    }
                }
                if(visible)
                {
                    if(m_filterXueli!="全部")
                    {
                        visible = row.GetCell("colP7").GetString() == m_filterXueli;
                    }
                }
                if (visible)
                {
                    if (m_filterHuKou.Length > 0)
                    {
                        visible = row.GetCell("colP6").GetString().IndexOf(m_filterHuKou) != -1;
                    }
                }
                if (visible)
                {
                    DateTime dt = Convert.ToDateTime(row.GetCell("colP1").GetString());
                    DateTime now = DateTime.Now;
                    if (dt <= now.AddDays(-m_filterDate))
                    {
                        visible = false;
                    }
                }
                if (visible)
                {
                    DateTime dt = Convert.ToDateTime(row.GetCell("colP3").GetString());
                    DateTime now = DateTime.Now;
                    if (dt <= now.AddYears(-m_filterAge))
                    {
                        visible = false;
                    }
                }
                row.Visible = visible;
            }
        }

        /// <summary>
        /// 单元格双击事件
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="cell">单元格</param>
        /// <param name="mp">坐标</param>
        /// <param name="button">按钮</param>
        /// <param name="clicks">点击次数</param>
        /// <param name="delta">滚轮值</param>
        private void GridCellClick(object sender, GridCell cell, POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            if (clicks == 2)
            {
                String file = cell.Row.GetCell("colP12").GetString();
                Process.Start(file);
            }
        }

        /// <summary>
        /// 调用主线程方法
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="args">参数</param>
        public void Invoke(object sender, object args)
        {
            String[] files = args as String[];
            if (files != null)
            {
                BindGrid(files);
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
            m_gridEmail.RegisterEvent(new ControlInvokeEvent(Invoke), EVENTID.INVOKE);
            m_gridEmail.RegisterEvent(new GridCellMouseEvent(GridCellClick), EVENTID.GRIDCELLCLICK);
            String dir = DataCenter.GetAppPath() + "\\email";
            if (!CFileA.IsDirectoryExist(dir))
            {
                CFileA.CreateDirectory(dir);
            }
            List<String> files = new List<String>();
            CFileA.GetFiles(dir, files);
            files.Sort();
            BindGrid(files.ToArray());
            Thread pop3Thread = new Thread(new ThreadStart(StartReadPop3));
            pop3Thread.Start();
        }

        /// <summary>
        /// 读取邮件
        /// </summary>
        public List<String> ReadPop3()
        {
            List<String> newList = new List<String>();
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
                    try
                    {
                        OpenPop.Mime.Message message = client.GetMessage(i);
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
                                    String key = Datesent.ToString("yyyyMMddHHmmss") + " " + sender;
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
                                            {
                                                body = textVersions[0].GetBodyAsText();
                                            }
                                            else
                                            {
                                                body = "<<OpenPop>> Cannot find a text version body in this message.";
                                            }
                                        }
                                        if (body != null && body.Length > 0)
                                        {
                                            CFileA.Write(file, body);
                                            newList.Add(file);
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
            return newList;
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

        /// <summary>
        /// 开始读取邮件
        /// </summary>
        private void StartReadPop3()
        {
            while (true)
            {
                List<String> strs = ReadPop3();
                if (strs.Count > 0)
                {
                    strs.Sort();
                    m_gridEmail.BeginInvoke(strs.ToArray());
                }
                Thread.Sleep(10000);
            }
        }
    }
}
