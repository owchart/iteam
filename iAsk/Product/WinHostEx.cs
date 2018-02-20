/*********************************************************************************\
*                                                                                 *
* WinHostEx.cs -Winform control host functions, types, and definitions.           *
*                                                                                 *
*               Version 6.00                                                      *
*                                                                                 *
*               Copyright (c) 2016-2016, iTeam. All rights reserved.             *
*               Created by Todd 2016/12/2.                                        *
*                                                                                 *
**********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using OwLib;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace OwLib
{
    /// <summary>
    /// 设计器管理
    /// </summary>
    public class WinHostEx : WinHost
    {
        #region Lord 2016/12/2
        [Flags]
        enum MouseEventFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }

        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_CHAR = 0x0102;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SYSKEYUP = 0x0105;
        public const int WM_SYSCHAR = 0x0106;

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(MouseEventFlag flags, int dx, int dy, int data, int extraInfo);

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(int hWnd, int Msg, int wParam, String lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(int hWnd, int Msg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
        private static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        public static extern bool GetCursorPos(ref POINT lpPoint);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(String lpClassName, String lpWindowName);

        /// <summary>
        /// 执行程序
        /// </summary>
        /// <param name="cmd">命令</param>
        public static void Execute(String cmd)
        {
            try
            {
                Process.Start(cmd);
            }
            catch { }
        }

        /// <summary>
        /// 获取文本
        /// </summary>
        /// <returns>文本</returns>
        public static String GetText()
        {
            POINT mp = new POINT();
            GetCursorPos(ref mp);
            IntPtr handle = WindowFromPoint(mp.x, mp.y);
            if (handle != IntPtr.Zero)
            {
                StringBuilder sb = new StringBuilder(10240);
                SendMessage((int)handle, 0xD, 10240, sb);
                return sb.ToString();
            }
            return "";
        }

        /// <summary>
        /// 触发鼠标事件
        /// </summary>
        /// <param name="eventID">事件ID</param>
        /// <param name="dx">横坐标</param>
        /// <param name="dy">纵坐标</param>
        /// <param name="data">滚轮值</param>
        public static void MouseEvent(String eventID, int dx, int dy, int data)
        {
            MouseEventFlag flag = MouseEventFlag.Move;
            if (eventID == "SETCURSOR")
            {
                SetCursorPos(dx, dy);
                return;
            }
            else if (eventID == "MOVE")
            {
                flag = MouseEventFlag.Move;
            }
            else if (eventID == "LEFTDOWN")
            {
                flag = MouseEventFlag.LeftDown;
            }
            else if (eventID == "LEFTUP")
            {
                flag = MouseEventFlag.LeftUp;
            }
            else if (eventID == "RIGHTDOWN")
            {
                flag = MouseEventFlag.RightDown;
            }
            else if (eventID == "RIGHTUP")
            {
                flag = MouseEventFlag.RightUp;
            }
            else if (eventID == "MIDDLEDOWN")
            {
                flag = MouseEventFlag.MiddleDown;
            }
            else if (eventID == "MIDDLEUP")
            {
                flag = MouseEventFlag.MiddleUp;
            }
            else if (eventID == "XDOWN")
            {
                flag = MouseEventFlag.XDown;
            }
            else if (eventID == "XUP")
            {
                flag = MouseEventFlag.XUp;
            }
            else if (eventID == "WHEEL")
            {
                flag = MouseEventFlag.Wheel;
            }
            else if (eventID == "VIRTUALDESK")
            {
                flag = MouseEventFlag.VirtualDesk;
            }
            else if (eventID == "ABSOLUTE")
            {
                flag = MouseEventFlag.Absolute;
            }
            mouse_event(flag, dx, dy, data, 0);
        }

        /// <summary>
        /// 触发键盘事件
        /// </summary>
        /// <param name="key">命令</param>
        public static void SendKey(String key)
        {
            SendKeys.Send(key);
        }

        /// <summary>
        /// 设置文字
        /// </summary>
        /// <param name="text">文字</param>
        public static void SetText(String text)
        {
            if (text != null && text.Length > 0)
            {
                Clipboard.Clear();
                Clipboard.SetText(text);
                SendKeys.SendWait("^v");
            }
        }

        /// <summary>
        /// 显示ToolTip
        /// </summary>
        /// <param name="text">文字</param>
        /// <param name="mp">位置</param>
        public override void ShowToolTip(String text, POINT mp)
        {
            //toolTip.Show(text, Control.FromHandle(HWnd), new Point(mp.x, mp.y));
            base.ShowToolTip(text, mp);
        }
        #endregion
    }
}
