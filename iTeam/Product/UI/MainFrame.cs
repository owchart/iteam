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
        /// 折叠菜单
        /// </summary>
        private FoldMenuA m_foldMenu;

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
                if (name == "AA")
                {
                    DimensionWindow performanceWindow = new DimensionWindow(Native);
                    performanceWindow.ShowDialog();
                }
                else if (name == "AOA")
                {
                    StaffWindow staffWindow = new StaffWindow(Native);
                    staffWindow.ShowDialog();
                }
                else if (name == "AAM")
                {
                    ProjectWindow projectWindow = new ProjectWindow(Native);
                    projectWindow.ShowDialog();
                }
                else if (name == "AI")
                {
                    AwardWindow awardWindow = new AwardWindow(Native);
                    awardWindow.ShowDialog();
                }
                else if (name == "BAR")
                {
                    CalendarWindow calendarWindow = new CalendarWindow(Native);
                    calendarWindow.ShowDialog();
                }
                else if (name == "CA")
                {
                    MasterWindow masterWindow = new MasterWindow(Native);
                    masterWindow.ShowDialog();
                }
                else if (name == "CCM")
                {
                    GitWindow gitWindow = new GitWindow(Native);
                    gitWindow.ShowDialog();
                }
                else if (name == "COI")
                {
                    ServerWindow serverWindow = new ServerWindow(Native);
                    serverWindow.ShowDialog();
                }
                else if (name == "GTM")
                {
                    ClueWindow clueWindow = new ClueWindow(Native);
                    clueWindow.ShowDialog();
                }
                else if (name == "IAC")
                {
                    PersonalWindow personalWindow = new PersonalWindow(Native);
                    personalWindow.ShowDialog();
                }
                else if (name == "LP")
                {
                    OpinionWindow opinionWindow = new OpinionWindow(Native);
                    opinionWindow.ShowDialog();
                }
                else if (name == "OI")
                {
                    FollowWindow followWindow = new FollowWindow(Native);
                    followWindow.ShowDialog();
                }
                else if (name == "PH")
                {
                    RemoteWindow remoteWindow = new RemoteWindow(Native);
                    remoteWindow.ShowDialog();
                }
                else if (name == "RI")
                {
                    JidianWindow jidianWindow = new JidianWindow(Native);
                    jidianWindow.ShowDialog();
                }
                else if (name == "SM")
                {
                    DisobeyWindow disobeyWindow = new DisobeyWindow(Native);
                    disobeyWindow.ShowDialog();
                }
                else if (name == "SS")
                {
                    BeAttackedWindow beAttackedWindow = new BeAttackedWindow(Native);
                    beAttackedWindow.ShowDialog();
                }
                else if (name == "STM")
                {
                    WangYiWindow wangYiWindow = new WangYiWindow(Native);
                    wangYiWindow.ShowDialog();
                }
                else if (name == "TC")
                {
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = DataCenter.GetAppPath() + "\\iTeam.exe";
                    startInfo.Arguments = "-plan";
                    process.StartInfo = startInfo;
                    process.Start();
                }
                else if (name == "TCC")
                {
                    SnitchWindow snitchWindow = new SnitchWindow(Native);
                    snitchWindow.ShowDialog();
                }
                else if (name == "TH")
                {
                    CounterspyWindow counterspyWindow = new CounterspyWindow(Native);
                    counterspyWindow.ShowDialog();
                }
                else if (name == "UA")
                {
                    LevelWindow levelWindow = new LevelWindow(Native);
                    levelWindow.ShowDialog();
                }
                else if (name == "UIM")
                {
                    WindowXmlEx skyWindow = new WindowXmlEx();
                    skyWindow.Load(Native, "SkyWindow", "skyWindow");
                    skyWindow.ShowDialog();
                }
                else if (name == "BS")
                {
                    BSStockWindow bsStockWindow = new BSStockWindow(Native);
                    bsStockWindow.ShowDialog();
                }
                else if (name == "RP")
                {
                    ExamWindow reportWindow = new ExamWindow(Native);
                    reportWindow.ShowDialog();
                }
                else if (name == "OW")
                {
                    OverWorkWindow overWorkWindow = new OverWorkWindow(Native);
                    overWorkWindow.ShowDialog();
                }
                else if (name == "BC")
                {
                    BusinessCardWindow businessCardWindow = new BusinessCardWindow(Native);
                    businessCardWindow.ShowDialog();
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
            UserCookie cookie = new UserCookie();
            cookie.m_key = "MENULAYOUT" + DataCenter.UserID.ToString();
            cookie.m_userID = DataCenter.UserID;
            List<FoldSubMenuA> subMenus = m_foldMenu.SubMenus;
            int subMenusSize = subMenus.Count;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < subMenusSize; i++)
            {
                FoldSubMenuA foldSubMenu = subMenus[i];
                sb.Append(foldSubMenu.Name + "," + (foldSubMenu.LayoutDiv.Visible ? "1" : "0") + ",");
                List<ControlA> subControls = foldSubMenu.LayoutDiv.m_controls;
                int subControlsSize = subControls.Count;
                for (int j = 0; j < subControlsSize; j++)
                {
                    ControlA subControl = subControls[j];
                    sb.Append(subControl.Name);
                    if (j != subControlsSize - 1)
                    {
                        sb.Append(",");
                    }
                }
                if (i != subMenusSize - 1)
                {
                    sb.Append("\r\n");
                }
            }
            cookie.m_value = sb.ToString();
            DataCenter.UserCookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 获取App功能
        /// </summary>
        public void GetApps(List<UserAppsTable> type1List, List<UserAppsTable> type2List,
            List<UserAppsTable> type3List, List<UserAppsTable> type4List)
        {
            PADDING margin1 = new PADDING(5, 5, 5, 5);
            PADDING padding = new PADDING(5, 5, 5, 5);
            PADDING margin2 = new PADDING(5, 5, 5, 5);
            Dictionary<String, FoldSubMenuA> menuCaches = new Dictionary<String, FoldSubMenuA>();
            if (type1List != null && type1List.Count > 0)
            {
                int num = type1List.Count;
                FoldSubMenuA menuItemAccount = new FoldSubMenuA();
                m_foldMenu.AddSubMenu(menuItemAccount);
                menuItemAccount.Name = "TYPE1";
                menuCaches[menuItemAccount.Name] = menuItemAccount;
                menuItemAccount.TitleLabel.Text = "员工管理(" + num + ")";
                menuItemAccount.ShrinkButton.Text = "收缩";
                menuItemAccount.ShrinkButton.Name = "shrink";
                menuItemAccount.LayoutDiv.Margin = margin1;
                menuItemAccount.Padding = padding;
                menuItemAccount.Margin = margin2;
                menuItemAccount.TitleLabel.Size = new SIZE(150, 20);
                menuItemAccount.ShrinkButton.Size = new SIZE(80, 20);
                foreach (UserAppsTable userApp in type1List)
                {
                    FoldMenuItemA menuButtonAddUser = new FoldMenuItemA();
                    menuButtonAddUser.Text = userApp.m_appName;
                    menuButtonAddUser.Name = userApp.m_appID;
                    menuButtonAddUser.BackImage = "file='images\\" + userApp.m_appID + ".jpg'";
                    menuButtonAddUser.Margin = margin2;
                    menuItemAccount.LayoutDiv.AddControl(menuButtonAddUser);
                }
            }
            if (type2List != null && type2List.Count > 0)
            {
                int num = type2List.Count;
                FoldSubMenuA menuItemFundTrade = new FoldSubMenuA();
                m_foldMenu.AddSubMenu(menuItemFundTrade);
                menuItemFundTrade.Name = "TYPE2";
                menuCaches[menuItemFundTrade.Name] = menuItemFundTrade;
                menuItemFundTrade.TitleLabel.Text = "项目管理(" + num + ")";
                menuItemFundTrade.ShrinkButton.Text = "收缩";
                menuItemFundTrade.ShrinkButton.Name = "shrink";
                menuItemFundTrade.LayoutDiv.Margin = margin1;
                menuItemFundTrade.Padding = padding;
                menuItemFundTrade.Margin = margin2;
                menuItemFundTrade.TitleLabel.Size = new SIZE(150, 20);
                menuItemFundTrade.ShrinkButton.Size = new SIZE(80, 20);
                foreach (UserAppsTable userApp in type2List)
                {
                    FoldMenuItemA menuButtonAddUser = new FoldMenuItemA();
                    menuButtonAddUser.Text = userApp.m_appName;
                    menuButtonAddUser.Name = userApp.m_appID;
                    menuButtonAddUser.BackImage = "file='images\\" + userApp.m_appID + ".jpg'";
                    menuButtonAddUser.Margin = margin2;
                    menuItemFundTrade.LayoutDiv.AddControl(menuButtonAddUser);
                }
            }
            if (type3List != null && type3List.Count > 0)
            {
                int num = type3List.Count;
                FoldSubMenuA menuItemSystem = new FoldSubMenuA();
                menuItemSystem.Name = "TYPE3";
                menuCaches[menuItemSystem.Name] = menuItemSystem;
                m_foldMenu.AddSubMenu(menuItemSystem);
                menuItemSystem.TitleLabel.Text = "个人管理(" + num + ")";
                menuItemSystem.ShrinkButton.Text = "收缩";
                menuItemSystem.ShrinkButton.Name = "shrink";
                menuItemSystem.LayoutDiv.Margin = margin1;
                menuItemSystem.Padding = padding;
                menuItemSystem.Margin = margin2;
                menuItemSystem.TitleLabel.Size = new SIZE(150, 20);
                menuItemSystem.ShrinkButton.Size = new SIZE(80, 20);
                foreach (UserAppsTable userApp in type3List)
                {
                    FoldMenuItemA menuButtonAddUser = new FoldMenuItemA();
                    menuButtonAddUser.Text = userApp.m_appName;
                    menuButtonAddUser.Name = userApp.m_appID;
                    menuButtonAddUser.BackImage = "file='images\\" + userApp.m_appID + ".jpg'";
                    menuButtonAddUser.Margin = margin2;
                    menuItemSystem.LayoutDiv.AddControl(menuButtonAddUser);
                }
            }
            if (type4List != null && type4List.Count > 0)
            {
                int num = type4List.Count;
                FoldSubMenuA menuItemRisk = new FoldSubMenuA();
                menuItemRisk.Name = "TYPE4";
                menuCaches[menuItemRisk.Name] = menuItemRisk;
                m_foldMenu.AddSubMenu(menuItemRisk);
                menuItemRisk.TitleLabel.Text = "情报管理(" + num + ")";
                menuItemRisk.ShrinkButton.Text = "收缩";
                menuItemRisk.ShrinkButton.Name = "shrink";
                menuItemRisk.LayoutDiv.Margin = margin1;
                menuItemRisk.Padding = padding;
                menuItemRisk.Margin = margin2;
                menuItemRisk.TitleLabel.Size = new SIZE(150, 20);
                menuItemRisk.ShrinkButton.Size = new SIZE(80, 20);
                foreach (UserAppsTable userApp in type4List)
                {
                    FoldMenuItemA menuButtonAddUser = new FoldMenuItemA();
                    menuButtonAddUser.Text = userApp.m_appName;
                    menuButtonAddUser.Name = userApp.m_appID;
                    menuButtonAddUser.BackImage = "file='images\\" + userApp.m_appID + ".jpg'";
                    menuButtonAddUser.Margin = margin2;
                    menuItemRisk.LayoutDiv.AddControl(menuButtonAddUser);
                }
            }
            //读取布局信息
            UserCookie cookie = new UserCookie();
            cookie.m_key = "MENULAYOUT" + DataCenter.UserID.ToString();
            cookie.m_userID = DataCenter.UserID;
            if (DataCenter.UserCookieService.GetCookie(cookie.m_key, ref cookie) > 0)
            {
                String value = cookie.m_value;
                String[] strs = value.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                int strsSize = strs.Length;
                for (int i = 0; i < strsSize; i++)
                {
                    String[] subStrs = strs[i].Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    String foldName = subStrs[0];
                    bool visible = subStrs[1] == "1";
                    if (menuCaches.ContainsKey(foldName))
                    {
                        FoldSubMenuA foldSubMenu = menuCaches[foldName];
                        foldSubMenu.LayoutDiv.Visible = visible;
                        int subStrsSize = subStrs.Length;
                        int orderNum = 0;
                        for (int j = 2; j < subStrsSize; j++)
                        {
                            List<ControlA> subControls = foldSubMenu.LayoutDiv.m_controls;
                            int subControlsSize = subControls.Count;
                            for (int m = 0; m < subControlsSize; m++)
                            {
                                ControlA subControl = subControls[m];
                                if (subControl.Name == subStrs[j])
                                {
                                    (subControl as FoldMenuItemA).OrderNum = orderNum;
                                    orderNum++;
                                    break;
                                }
                            }
                        }
                        foldSubMenu.LayoutDiv.m_controls.Sort(new FoldMenuItemOrderNumCompare());
                    }
                }
            }
            menuCaches.Clear();
            m_foldMenu.Update();
            Native.Update();
            ControlPaintEvent paintLayoutEvent = new ControlPaintEvent(PaintLayoutDiv);
            m_foldMenu.RegisterEvent(paintLayoutEvent, EVENTID.PAINT);
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
            ControlA control = Native.GetControls()[0];
            control.BackColor = COLOR.CONTROL;
            CFunctionAjax.SetListener(control);
            m_foldMenu = FindControl("mainMenu") as FoldMenuA;
            List<UserAppsTable> type1List = new List<UserAppsTable>();
            List<UserAppsTable> type2List = new List<UserAppsTable>();
            List<UserAppsTable> type3List = new List<UserAppsTable>();
            List<UserAppsTable> type4List = new List<UserAppsTable>();
            type1List.Add(new UserAppsTable("UIM", "随机选拔"));
            type1List.Add(new UserAppsTable("AOA", "员工管理"));
            type1List.Add(new UserAppsTable("AA", "六维图"));
            type2List.Add(new UserAppsTable("AAM", "开发项目"));
            type1List.Add(new UserAppsTable("AI", "荣誉榜"));
            type3List.Add(new UserAppsTable("BAR", "日历事件"));
            type4List.Add(new UserAppsTable("CA", "上级指示"));
            type2List.Add(new UserAppsTable("CCM", "Git管理"));
            type2List.Add(new UserAppsTable("COI", "服务器管理"));
            type4List.Add(new UserAppsTable("GTM", "重要线索"));
            type1List.Add(new UserAppsTable("IAC", "个人信息"));
            type4List.Add(new UserAppsTable("LP", "重要意见"));
            type1List.Add(new UserAppsTable("OI", "重点关注"));
            type2List.Add(new UserAppsTable("PH", "远程服务"));
            type2List.Add(new UserAppsTable("RI", "代码统计"));
            type4List.Add(new UserAppsTable("SM", "抗命记录"));
            type4List.Add(new UserAppsTable("SS", "被攻击记录"));
            type4List.Add(new UserAppsTable("STM", "妄议记录"));
            type4List.Add(new UserAppsTable("TCC", "打小报告"));
            type4List.Add(new UserAppsTable("TH", "反间谍活动"));
            type1List.Add(new UserAppsTable("UA", "水平级别"));
            type3List.Add(new UserAppsTable("BS", "股票买卖"));
            type1List.Add(new UserAppsTable("RP", "考一考"));
            type1List.Add(new UserAppsTable("OW", "加班记录"));
            type3List.Add(new UserAppsTable("BC", "名片"));
            GetApps(type1List, type2List, type3List, type4List);
            RegisterEvents(control);
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
    }

    /// <summary>
    /// 用户app权限类
    /// </summary>
    public class UserAppsTable
    {
        /// <summary>
        /// 创建权限类
        /// </summary>
        /// <param name="appID">ID</param>
        /// <param name="appName">名称</param>
        public UserAppsTable(String appID, String appName)
        {
            m_appID = appID;
            m_appName = appName;
        }

        // ID
        public String m_appID = "";

        // 名称
        public String m_appName = "";
    }
}
