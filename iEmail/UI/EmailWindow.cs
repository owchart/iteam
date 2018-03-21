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
using Newtonsoft.Json;
using node.gs;

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
        /// 条件表格
        /// </summary>
        private GridA m_gridCondition;

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridEmail;

        /// <summary>
        /// 邮件的条件
        /// </summary>
        private EmailCondition m_emailCondition = new EmailCondition();

        /// <summary>
        /// 所有的条件
        /// </summary>
        private List<EmailCondition> m_emailConditions = new List<EmailCondition>();

        private EmailInfo m_emailInfo;

        /// <summary>
        /// 获取或设置邮箱信息
        /// </summary>
        public EmailInfo EmailInfo
        {
            get { return m_emailInfo; }
            set { m_emailInfo = value; }
        }

        /// <summary>
        /// 绑定条件
        /// </summary>
        public void BindConditions()
        {
            m_gridCondition.ClearRows();
            m_gridCondition.BeginUpdate();
            int conditionsSize = m_emailConditions.Count;
            for (int i = 0; i < conditionsSize; i++)
            {
                EmailCondition emailCondition = m_emailConditions[i];
                GridRow row = new GridRow();
                row.AllowEdit = true;
                m_gridCondition.AddRow(row);
                row.Tag = emailCondition;
                row.AddCell("colP1", new GridStringCell(emailCondition.m_id));
                GridStringCell nameCell = new GridStringCell(emailCondition.m_name);
                nameCell.AllowEdit = true;
                row.AddCell("colP2", nameCell);
                row.AddCell("colP3", new GridStringCell(emailCondition.ToString()));
                ButtonA deleteButton = new ButtonA();
                deleteButton.RegisterEvent(new ControlMouseEvent(ClickButton), EVENTID.CLICK);
                deleteButton.Font = new FONT("微软雅黑", 16, true, false, false);
                deleteButton.BackColor = COLOR.ARGB(255, 80, 80);
                deleteButton.ForeColor = COLOR.ARGB(255, 255, 255);
                deleteButton.Text = "删除";
                deleteButton.Name = "btnDeleteCondition";
                deleteButton.Size = new SIZE(80, 20);
                row.EditButton = deleteButton;
            }
            m_gridCondition.EndUpdate();
            m_gridCondition.Invalidate();
        }

        /// <summary>
        /// 绑定表格
        /// </summary>
        public void BindGrid(String[] files, bool isNew)
        {
            int filesSize = files.Length;
            m_gridEmail.BeginUpdate();
            String sendDir = DataCenter.GetAppPath() + "\\send";
            String contactDir = DataCenter.GetAppPath() + "\\contact";
            String imageDir = DataCenter.GetAppPath() + "\\image";
            for (int i = filesSize - 1; i >= 0; i--)
            {
                String file = files[i];
                String content = "";
                CFileA.Read(file, ref content);
                if (content != null && content.Length > 100)
                {
                    GridRow row = new GridRow();
                    row.Height = 40;
                    if (isNew)
                    {
                        m_gridEmail.InsertRow(0, row);
                    }
                    else
                    {
                        m_gridEmail.AddRow(row);
                    }
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
                            }
                            else
                            {
                                salary = salary + "以上";
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
                    if (jingYan == 0)
                    {
                        startIndex = 0;
                        while (true)
                        {
                            int index = content.IndexOf("工作描述：", startIndex);
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
                    }
                    String imageFilePath = "";
                    String imageName = "";
                    int imgIndex = content.IndexOf("https://mypics.zhaopin.cn/pic/");
                    if (imgIndex == -1)
                    {
                        imgIndex = content.IndexOf("https://mypics.zhaopin.cn/avatar/");
                    }
                    if (imgIndex != -1)
                    {
                        int imgIndex2 = content.IndexOf("\">", imgIndex);
                        String imageUrl = content.Substring(imgIndex, imgIndex2 - imgIndex);
                        imageName = imageUrl.Substring(imageUrl.LastIndexOf("/") + 1);
                        imageFilePath = imageDir + "\\" + imageName;
                        if (!CFileA.IsFileExist(imageFilePath))
                        {
                            HttpGetService.Download(imageFilePath, imageUrl);
                        }
                    }
                    else
                    {
                        Console.WriteLine("1");
                    }
                    GridControlCell imageCell = new GridControlCell();
                    imageCell.Control = new ControlA();
                    imageCell.Control.Native = Native;
                    imageCell.Control.BackImage = imageName;
                    row.AddCell("colP0", imageCell);
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
                    row.AddCell("colP11", new GridStringCell(jingYan.ToString()));
                    String contactFile = contactDir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                    String strPhone = "";
                    String strEmail = "";
                    if (CFileA.IsFileExist(contactFile))
                    {
                        String content2 = "";
                        CFileA.Read(contactFile, ref content2);
                        String[] strs = content2.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        strPhone = strs[0];
                        strEmail = strs[1];
                        row.AddCell("colP12", new GridStringCell(strPhone));
                        row.AddCell("colP13", new GridStringCell(strEmail));
                    }
                    else
                    {
                        if (isNew)
                        {
                            int idxH = content.IndexOf("https://ihr.zhaopin.com/job/relay.html?");
                            if (idxH == -1)
                            {
                                idxH = content.IndexOf("http://rd.zhaopin.com/resumepreview/resume/emailim?ldparam=");
                            }
                            int idxH2 = content.IndexOf(">", idxH + 10);
                            String url = content.Substring(idxH, idxH2 - idxH - 1);
                            String contentUrl = "https://ihr.zhaopin.com/resumemanage/emailim.do?s=" + url.Replace("https://ihr.zhaopin.com/job/relay.html?param=", "").Replace("http://rd.zhaopin.com/resumepreview/resume/emailim?ldparam=", "");
                            String text = HttpGetService.Get(contentUrl);
                            if (text != null && text.Length > 0)
                            {
                                try
                                {
                                    String identifierP = "\"phone\":\"";
                                    int pIndex = text.IndexOf(identifierP);
                                    int pIndex2 = text.IndexOf("\",\"email\"");
                                    strPhone = text.Substring(pIndex + identifierP.Length, pIndex2 - pIndex - identifierP.Length);
                                    identifierP = "\",\"email\":\"";
                                    int pIndex3 = text.IndexOf("\",\"gid\":\"");
                                    strEmail = text.Substring(pIndex2 + identifierP.Length, pIndex3 - pIndex2 - identifierP.Length);
                                    CFileA.Write(contactFile, strPhone + "," + strEmail);
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                        row.AddCell("colP12", new GridStringCell(strPhone));
                        row.AddCell("colP13", new GridStringCell(strEmail));
                    }
                    if (strPhone.Length > 0 && CFileA.IsFileExist(sendDir + "\\" + strPhone))
                    {
                        row.AddCell("colP14", new GridStringCell("已发送"));
                        row.GetCell("colP14").Style = new GridCellStyle();
                        row.GetCell("colP14").Style.BackColor = COLOR.ARGB(255, 0, 0);
                        row.GetCell("colP14").Style.ForeColor = COLOR.ARGB(255, 255, 255);
                    }
                    else
                    {
                        row.AddCell("colP14", new GridStringCell("点击发送"));
                        row.GetCell("colP14").Style = new GridCellStyle();
                        row.GetCell("colP14").Style.BackColor = COLOR.ARGB(0, 255, 0);
                        row.GetCell("colP14").Style.ForeColor = COLOR.ARGB(0, 0, 0);
                    }
                    row.AddCell("colP15", new GridStringCell(file));
                    row.AddCell("colP16", new GridStringCell(content));
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
                if (name == "btnApplyCondition")
                {
                    m_emailCondition.m_filterAge = (int)GetSpin("spinAge").Value;
                    m_emailCondition.m_filterDate = (int)GetSpin("spinDate").Value;
                    m_emailCondition.m_filterHuKou = GetTextBox("txtHukou").Text;
                    m_emailCondition.m_filterMarry = GetComboBox("cbMarry").Text;
                    m_emailCondition.m_filterSex = GetComboBox("cbSex").Text;
                    m_emailCondition.m_filterStatus = GetComboBox("cbStatus").Text;
                    m_emailCondition.m_filterXueli = GetComboBox("cbXueli").Text;
                    m_emailCondition.m_filterJingYan = GetTextBox("txtJingYan").Text;
                    m_emailCondition.m_filterKey = GetTextBox("txtZhiYe").Text;
                    FilterGrid();
                    m_gridEmail.Update();
                    m_gridEmail.Invalidate();
                }
                else if (name == "btnSaveCondition")
                {
                    EmailCondition emailCondition = new EmailCondition();
                    emailCondition.m_id = CStrA.GetGuid();
                    emailCondition.m_filterAge = (int)GetSpin("spinAge").Value;
                    emailCondition.m_filterDate = (int)GetSpin("spinDate").Value;
                    emailCondition.m_filterHuKou = GetTextBox("txtHukou").Text;
                    emailCondition.m_filterMarry = GetComboBox("cbMarry").Text;
                    emailCondition.m_filterSex = GetComboBox("cbSex").Text;
                    emailCondition.m_filterStatus = GetComboBox("cbStatus").Text;
                    emailCondition.m_filterXueli = GetComboBox("cbXueli").Text;
                    emailCondition.m_filterJingYan = GetTextBox("txtJingYan").Text;
                    emailCondition.m_filterKey = GetTextBox("txtZhiYe").Text;
                    m_emailConditions.Add(emailCondition);
                    String conditionFilePath = DataCenter.GetAppPath() + "\\condition.dat";
                    CFileA.Write(conditionFilePath, JsonConvert.SerializeObject(m_emailConditions));
                    BindConditions();
                }
                else if (name == "btnDeleteCondition")
                {
                    List<GridRow> selectedRows = m_gridCondition.SelectedRows;
                    int selectedRowsSize = selectedRows.Count;
                    if (selectedRowsSize > 0)
                    {
                        GridRow selectedRow = selectedRows[0];
                        m_emailConditions.Remove(selectedRow.Tag as EmailCondition);
                        m_gridCondition.OnRowEditEnd();
                        m_gridCondition.RemoveRow(selectedRow);
                        String conditionFilePath = DataCenter.GetAppPath() + "\\condition.dat";
                        CFileA.Write(conditionFilePath, JsonConvert.SerializeObject(m_emailConditions));
                        BindConditions();
                    }
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
                if (m_emailCondition.m_filterSex != "全部")
                {
                    visible = row.GetCell("colP5").GetString() == m_emailCondition.m_filterSex;
                }
                if (visible)
                {
                    if (m_emailCondition.m_filterMarry != "全部")
                    {
                        visible = row.GetCell("colP4").GetString() == m_emailCondition.m_filterMarry;
                    }
                }
                if (visible)
                {
                    if (m_emailCondition.m_filterStatus != "全部")
                    {
                        visible = row.GetCell("colP8").GetString() == m_emailCondition.m_filterStatus;
                    }
                }
                if (visible)
                {
                    if (m_emailCondition.m_filterXueli != "全部")
                    {
                        visible = row.GetCell("colP7").GetString() == m_emailCondition.m_filterXueli;
                    }
                }
                if (visible)
                {
                    if (m_emailCondition.m_filterHuKou.Length > 0)
                    {
                        visible = row.GetCell("colP6").GetString().IndexOf(m_emailCondition.m_filterHuKou) != -1;
                    }
                }
                if (visible)
                {
                    DateTime dt = Convert.ToDateTime(row.GetCell("colP1").GetString());
                    DateTime now = DateTime.Now;
                    if (dt <= now.AddHours(-m_emailCondition.m_filterDate * 24))
                    {
                        visible = false;
                    }
                }
                if (visible)
                {
                    DateTime dt = DateTime.MinValue;
                    DateTime.TryParse(row.GetCell("colP3").GetString(), out dt);
                    DateTime now = DateTime.Now;
                    if (dt <= now.AddYears(-m_emailCondition.m_filterAge))
                    {
                        visible = false;
                    }
                }
                if (visible)
                {
                    if (m_emailCondition.m_filterJingYan.Length > 0)
                    {
                        if (row.GetCell("colP11").GetString() != m_emailCondition.m_filterJingYan)
                        {
                            visible = false;
                        }
                    }
                }
                if (visible)
                {
                    if (m_emailCondition.m_filterKey.Length > 0)
                    {
                        String[] keys = m_emailCondition.m_filterKey.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        int keysSize = keys.Length;
                        String content = row.GetCell("colP16").GetString();
                        for (int j = 0; j < keysSize; j++)
                        {
                            if (content.IndexOf(keys[j]) == -1)
                            {
                                visible = false;
                                break;
                            }
                        }
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
            if (cell.Grid == m_gridCondition)
            {
                if (clicks == 2)
                {
                    m_emailCondition = (cell.Row.Tag as EmailCondition).Copy();
                    m_gridEmail.BeginUpdate();
                    FilterGrid();
                    m_gridEmail.EndUpdate();
                    m_gridEmail.Invalidate();
                }
            }
            else
            {
                if (cell.Column.Name == "colP14")
                {
                    if (clicks == 1)
                    {
                        String file = cell.Row.GetCell("colP15").GetString();
                        String content = "";
                        CFileA.Read(file, ref content);
                        if (content != null && content.Length > 0)
                        {
                            try
                            {
                                String strPhone = "";
                                String strEmail = "";
                                String contactDir = DataCenter.GetAppPath() + "\\contact";
                                String contactFile = contactDir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                                if (CFileA.IsFileExist(contactFile))
                                {
                                    String content2 = "";
                                    CFileA.Read(contactFile, ref content2);
                                    String[] strs = content2.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    strPhone = strs[0];
                                    strEmail = strs[1];
                                }
                                else
                                {
                                    int idxH = content.IndexOf("https://ihr.zhaopin.com/job/relay.html?");
                                    if (idxH == -1)
                                    {
                                        idxH = content.IndexOf("http://rd.zhaopin.com/resumepreview/resume/emailim?ldparam=");
                                    }
                                    int idxH2 = content.IndexOf(">", idxH + 10);
                                    String url = content.Substring(idxH, idxH2 - idxH - 1);
                                    String contentUrl = "https://ihr.zhaopin.com/resumemanage/emailim.do?s=" + url.Replace("https://ihr.zhaopin.com/job/relay.html?param=", "").Replace("http://rd.zhaopin.com/resumepreview/resume/emailim?ldparam=", "");
                                    String text = HttpGetService.Get(contentUrl);
                                    if (text != null && text.Length > 0)
                                    {
                                        try
                                        {
                                            String identifierP = "\"phone\":\"";
                                            int pIndex = text.IndexOf(identifierP);
                                            int pIndex2 = text.IndexOf("\",\"email\"");
                                            identifierP = "\",\"email\":\"";
                                            int pIndex3 = text.IndexOf("\",\"gid\":\"");
                                            strEmail = text.Substring(pIndex2 + identifierP.Length, pIndex3 - pIndex2 - identifierP.Length);
                                            CFileA.Write(contactFile, strPhone + "," + strEmail);
                                            cell.Row.GetCell("colP12").SetString(strPhone);
                                            cell.Row.GetCell("colP13").SetString(strPhone);
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                }
                                if (strPhone != null && strPhone.Length > 0)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.AppendLine("职位:" + cell.Row.GetCell("colP10").GetString());
                                    sb.AppendLine("姓名:" + cell.Row.GetCell("colP2").GetString());
                                    sb.AppendLine("性别:" + cell.Row.GetCell("colP5").GetString());
                                    sb.AppendLine("户口:" + cell.Row.GetCell("colP6").GetString());
                                    sb.AppendLine("月薪:" + cell.Row.GetCell("colP9").GetString());
                                    sb.AppendLine("工作经验:" + cell.Row.GetCell("colP11").GetString() + "次");
                                    sb.AppendLine("状态:" + cell.Row.GetCell("colP8").GetString());
                                    sb.AppendLine("手机:" + strPhone);
                                    sb.AppendLine("邮箱:" + strEmail);
                                    SendMail("MingYue.Xu@gaiafintech.com", "请通知面试", sb.ToString());
                                    String sendDir = DataCenter.GetAppPath() + "\\send";
                                    String filePath = sendDir + "\\" + strPhone;
                                    CFileA.Write(filePath, "");
                                    cell.Row.GetCell("colP14").Text = "已发送";
                                    cell.Row.GetCell("colP14").Style = new GridCellStyle();
                                    cell.Row.GetCell("colP14").Style.BackColor = COLOR.ARGB(255, 0, 0);
                                    cell.Row.GetCell("colP14").Style.ForeColor = COLOR.ARGB(255, 255, 255);
                                    m_gridEmail.Invalidate();
                                }
                                else
                                {
                                    MessageBox.Show("发送失败");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("发送失败");
                            }
                        }
                    }
                }
                else
                {
                    if (clicks == 2)
                    {
                        if (cell.Column.Name == "colP0")
                        {
                            String imageDir = DataCenter.GetAppPath() + "\\image";
                            Process.Start(imageDir + "\\" + (cell as GridControlCell).Control.BackImage);
                        }
                        else
                        {
                            String file = cell.Row.GetCell("colP15").GetString();
                            Process.Start(file);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 单元格编辑结束事件
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="cell">单元格</param>
        private void GridCellEditEnd(object sender, GridCell cell)
        {
            if (cell != null)
            {
                (cell.Row.Tag as EmailCondition).m_name = cell.GetString();
                String conditionFilePath = DataCenter.GetAppPath() + "\\condition.dat";
                CFileA.Write(conditionFilePath, JsonConvert.SerializeObject(m_emailConditions));
                BindConditions();
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
                BindGrid(files, true);
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
            m_gridCondition = GetGrid("gridCondition");
            m_gridCondition.RegisterEvent(new GridCellMouseEvent(GridCellClick), EVENTID.GRIDCELLCLICK);
            m_gridCondition.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public override void LoadData()
        {
            String dir = DataCenter.GetAppPath() + "\\email";
            if (!CFileA.IsDirectoryExist(dir))
            {
                CFileA.CreateDirectory(dir);
            }
            String sendDir = DataCenter.GetAppPath() + "\\send";
            if (!CFileA.IsDirectoryExist(sendDir))
            {
                CFileA.CreateDirectory(sendDir);
            }
            String contactDir = DataCenter.GetAppPath() + "\\contact";
            if (!CFileA.IsDirectoryExist(contactDir))
            {
                CFileA.CreateDirectory(contactDir);
            }
            String imageDir = DataCenter.GetAppPath() + "\\image";
            if (!CFileA.IsDirectoryExist(imageDir))
            {
                CFileA.CreateDirectory(imageDir);
            }
            List<String> files = new List<String>();
            CFileA.GetFiles(dir, files);
            files.Sort();
            BindGrid(files.ToArray(), false);
            String conditionFilePath = DataCenter.GetAppPath() + "\\condition.dat";
            if (CFileA.IsFileExist(conditionFilePath))
            {
                String content = "";
                CFileA.Read(conditionFilePath, ref content);
                m_emailConditions = JsonConvert.DeserializeObject<List<EmailCondition>>(content);
            }
            BindConditions();
            Thread pop3Thread = new Thread(new ThreadStart(StartReadPop3));
            pop3Thread.Start();
        }

        /// <summary>
        /// 读取邮件
        /// </summary>
        public void ReadPop3()
        {
            try
            {
                String dir = DataCenter.GetAppPath() + "\\email";
                using (Pop3Client client = new Pop3Client())
                {
                    //client.Connect("imap.exmail.qq.com", 110, false);
                    client.Connect(m_emailInfo.m_server, 995, true);
                    //client.Authenticate("TaoDe@gaiafintech.com", "Gaia12345678", OpenPop.Pop3.AuthenticationMethod.UsernameAndPassword);
                    client.Authenticate(m_emailInfo.m_userName, m_emailInfo.m_pwd, OpenPop.Pop3.AuthenticationMethod.UsernameAndPassword);
                    int messageCount = client.GetMessageCount();
                    DateTime dt = DateTime.MinValue;
                    if (CFileA.IsFileExist(dir + "\\datetime"))
                    {
                        String content = "";
                        CFileA.Read(dir + "\\datetime", ref content);
                        dt = Convert.ToDateTime(content);
                    }
                    DateTime writeTime = DateTime.Now.AddDays(-3);
                    String writeStr = "";
                    for (int i = messageCount; i >= 1; i--)
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
                                            writeStr = writeTime.ToString();
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
                                                List<String> files = new List<String>();
                                                files.Add(file);
                                                m_gridEmail.BeginInvoke(files.ToArray());
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
                    if (writeStr != null && writeStr.Length > 0)
                    {
                        CFileA.Write(dir + "\\datetime", writeStr);
                    }
                }
            }
            catch (Exception ex)
            {
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

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sendTo"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public void SendMail(string sendTo, string subject, string body)
        {
            System.Web.Mail.MailMessage mailmsg = new System.Web.Mail.MailMessage();
            mailmsg.To = sendTo;
            //mailmsg.Cc = cc;  
            mailmsg.Subject = subject;
            mailmsg.Body = body;

            //sender here  
            mailmsg.From = m_emailInfo.m_userName;
            // certify needed    
            mailmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");//1 is to certify  
            //the user id     
            mailmsg.Fields.Add(
                "http://schemas.microsoft.com/cdo/configuration/sendusername",
                m_emailInfo.m_userName);
            //the password  
            mailmsg.Fields.Add(
                "http://schemas.microsoft.com/cdo/configuration/sendpassword",
                 m_emailInfo.m_pwd);

            System.Web.Mail.SmtpMail.SmtpServer = "smtp.exmail.qq.com";
            System.Web.Mail.SmtpMail.Send(mailmsg);
        }

        /// <summary>
        /// 开始读取邮件
        /// </summary>
        private void StartReadPop3()
        {
            while (true)
            {
                ReadPop3();
                Thread.Sleep(10000);
            }
        }
    }

    public class EmailInfo
    {
        /// <summary>
        /// 端口
        /// </summary>
        public String m_port;

        /// <summary>
        /// 密码
        /// </summary>
        public String m_pwd;

        /// <summary>
        /// 服务器
        /// </summary>
        public String m_server;

        /// <summary>
        /// 用户名
        /// </summary>
        public String m_userName;
    }

    /// <summary>
    /// 邮件的条件
    /// </summary>
    public class EmailCondition
    {
        /// <summary>
        /// 过滤年龄
        /// </summary>
        public int m_filterAge = 40;

        /// <summary>
        /// 过滤日期
        /// </summary>
        public int m_filterDate = 10;

        /// <summary>
        /// 过滤户口
        /// </summary>
        public String m_filterHuKou = "";

        /// <summary>
        /// 过滤关键字
        /// </summary>
        public String m_filterKey = "";

        /// <summary>
        /// 几次工作
        /// </summary>
        public String m_filterJingYan = "";

        /// <summary>
        /// 过滤婚姻
        /// </summary>
        public String m_filterMarry = "全部";

        /// <summary>
        /// 过滤性别
        /// </summary>
        public String m_filterSex = "全部";

        /// <summary>
        /// 过滤状态
        /// </summary>
        public String m_filterStatus = "全部";

        /// <summary>
        /// 过滤学历
        /// </summary>
        public String m_filterXueli = "全部";

        /// <summary>
        /// 标识ID
        /// </summary>
        public String m_id = "";

        /// <summary>
        /// 名称
        /// </summary>
        public String m_name = "请取名";

        /// <summary>
        /// 拷贝
        /// </summary>
        /// <returns></returns>
        public EmailCondition Copy()
        {
            EmailCondition copyCondition = new EmailCondition();
            copyCondition.m_filterAge = m_filterAge;
            copyCondition.m_filterDate = m_filterDate;
            copyCondition.m_filterHuKou = m_filterHuKou;
            copyCondition.m_filterJingYan = m_filterJingYan;
            copyCondition.m_filterMarry = m_filterMarry;
            copyCondition.m_filterSex = m_filterSex;
            copyCondition.m_filterStatus = m_filterStatus;
            copyCondition.m_filterXueli = m_filterXueli;
            copyCondition.m_filterKey = m_filterKey;
            return copyCondition;
        }

        /// <summary>
        /// 转换为String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("最近" + m_filterDate + "天 ");
            sb.Append(m_filterAge + "岁以下 ");
            sb.Append(m_filterHuKou.Length > 0 ? m_filterHuKou + " " : "");
            sb.Append(m_filterMarry != "全部" ? m_filterMarry + " " : "");
            sb.Append(m_filterSex != "全部" ? m_filterSex + " " : "");
            sb.Append(m_filterStatus != "全部" ? m_filterStatus + " " : "");
            sb.Append(m_filterXueli != "全部" ? m_filterXueli + " " : "");
            sb.Append(m_filterJingYan.Length > 0 ? "上过" + m_filterJingYan + "次班 " : "");
            sb.Append(m_filterKey.Length > 0 ? m_filterKey : "");
            return sb.ToString();
        }
    }
}
