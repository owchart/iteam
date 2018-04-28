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

namespace OwLib
{
    /// <summary>
    /// ����ϵͳ
    /// </summary>
    public class MainFrame : UIXmlEx, IDisposable
    {
        /// <summary>
        /// ��������ϵͳ
        /// </summary>
        public MainFrame()
        {
        }

        /// <summary>
        /// �۵��˵�
        /// </summary>
        private FoldMenuA m_foldMenu;

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
        /// ��ȡApp����
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
                menuItemAccount.TitleLabel.Text = "Ա������(" + num + ")";
                menuItemAccount.ShrinkButton.Text = "����";
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
                menuItemFundTrade.TitleLabel.Text = "��Ŀ����(" + num + ")";
                menuItemFundTrade.ShrinkButton.Text = "����";
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
                menuItemSystem.TitleLabel.Text = "���˹���(" + num + ")";
                menuItemSystem.ShrinkButton.Text = "����";
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
                menuItemRisk.TitleLabel.Text = "�鱨����(" + num + ")";
                menuItemRisk.ShrinkButton.Text = "����";
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
            //��ȡ������Ϣ
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
        /// �Ƿ��д�����ʾ
        /// </summary>
        /// <returns>�Ƿ���ʾ</returns>
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
        /// ����XML
        /// </summary>
        /// <param name="xmlPath">XML·��</param>
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
            type1List.Add(new UserAppsTable("UIM", "���ѡ��"));
            type1List.Add(new UserAppsTable("AOA", "Ա������"));
            type1List.Add(new UserAppsTable("AA", "��άͼ"));
            type2List.Add(new UserAppsTable("AAM", "������Ŀ"));
            type1List.Add(new UserAppsTable("AI", "������"));
            type3List.Add(new UserAppsTable("BAR", "�����¼�"));
            type4List.Add(new UserAppsTable("CA", "�ϼ�ָʾ"));
            type2List.Add(new UserAppsTable("CCM", "Git����"));
            type2List.Add(new UserAppsTable("COI", "����������"));
            type4List.Add(new UserAppsTable("GTM", "��Ҫ����"));
            type1List.Add(new UserAppsTable("IAC", "������Ϣ"));
            type4List.Add(new UserAppsTable("LP", "��Ҫ���"));
            type1List.Add(new UserAppsTable("OI", "�ص��ע"));
            type2List.Add(new UserAppsTable("PH", "Զ�̷���"));
            type2List.Add(new UserAppsTable("RI", "����ͳ��"));
            type4List.Add(new UserAppsTable("SM", "������¼"));
            type4List.Add(new UserAppsTable("SS", "��������¼"));
            type4List.Add(new UserAppsTable("STM", "�����¼"));
            type4List.Add(new UserAppsTable("TCC", "��С����"));
            type4List.Add(new UserAppsTable("TH", "������"));
            type1List.Add(new UserAppsTable("UA", "ˮƽ����"));
            type3List.Add(new UserAppsTable("BS", "��Ʊ����"));
            type1List.Add(new UserAppsTable("RP", "��һ��"));
            type1List.Add(new UserAppsTable("OW", "�Ӱ��¼"));
            type3List.Add(new UserAppsTable("BC", "��Ƭ"));
            GetApps(type1List, type2List, type3List, type4List);
            RegisterEvents(control);
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
        /// ��ʾ��ʾ����
        /// </summary>
        /// <param name="text">�ı�</param>
        /// <param name="caption">����</param>
        /// <param name="uType">��ʽ</param>
        /// <returns>���</returns>
        public int ShowMessageBox(String text, String caption, int uType)
        {
            MessageBox.Show(text, caption);
            return 1;
        }
    }

    /// <summary>
    /// �û�appȨ����
    /// </summary>
    public class UserAppsTable
    {
        /// <summary>
        /// ����Ȩ����
        /// </summary>
        /// <param name="appID">ID</param>
        /// <param name="appName">����</param>
        public UserAppsTable(String appID, String appName)
        {
            m_appID = appID;
            m_appName = appName;
        }

        // ID
        public String m_appID = "";

        // ����
        public String m_appName = "";
    }
}
