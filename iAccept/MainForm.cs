using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OwLib;
using System.IO;

namespace iAccept
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            m_native = NativeHandler.CreateNative();
            m_native.Paint = new GdiPlusPaint();
            m_native.DisplaySize = new SIZE(ClientSize.Width, ClientSize.Height);
            m_host = m_native.Host as WinHost;
            m_host.Native = m_native;
            m_native.Host = m_host;
            m_host.HWnd = Handle;
            AcceptDiv acceptDiv = new AcceptDiv();
            acceptDiv.Dock = DockStyleA.Fill;
            acceptDiv.Size = m_native.DisplaySize;
            acceptDiv.Text = File.ReadAllText(Application.StartupPath + "\\accept.txt", Encoding.Default);
            m_native.AddControl(acceptDiv);
            m_native.Update();
            m_native.Invalidate();
        }

        private INativeBase m_native;

        private WinHost m_host;

        /// <summary>
        /// 尺寸改变方法
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (m_native != null)
            {
                m_native.Update();
                Invalidate();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m_host != null)
            {
                if (m_host.OnMessage(ref m) > 0)
                {
                    return;
                }
            }
            base.WndProc(ref m);
        }
    }

    public class AcceptDiv : ControlA
    {
        public override void OnAdd()
        {
            base.OnAdd();
            StartTimer(GetNewTimerID(), 10);
            if (m_records.Count == 0)
            {
                m_records.Add("培训老师");
                m_records.Add("直属主管");
                m_records.Add("技术副总监");
                m_records.Add("技术总监");
            }
        }

        private int m_step = 0;

        public override void OnMouseDown(POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            base.OnMouseDown(mp, button, clicks, delta);
            m_step++;
            Invalidate();
        }

        private List<String> m_records = new List<String>();

        private Random m_rd = new Random();

        public override void OnPaint(CPaint paint, RECT clipRect)
        {
            paint.FillGradientRect(COLOR.ARGB(200, 90, 120, 24), COLOR.ARGB(200, 122, 156, 40), clipRect, 0, 90);
            int width = Width, height = Height;
            String[] strs = Text.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            FONT tFont = new FONT("微软雅黑", 40, true, false, false);
            SIZE tSize = paint.TextSize(strs[0], tFont);
            RECT tRect = new RECT();
            tRect.left = width/2- tSize.cx/2;
            tRect.top = 50;
            paint.DrawText(strs[0], COLOR.ARGB(255, 255, 255), tFont, tRect);
            int recordsSize = m_records.Count;
            int left = 60, top = 150;
            int nWidth = width - left * 2;
            FONT nFont = new FONT("微软雅黑", 22, true, false, false);
            if(m_step >= 2)
            {
                nFont.m_fontSize = 40;
            }
            for (int i = 0; i < recordsSize; i++)
            {
                String text = m_records[i];
                if (m_step >= 2)
                {
                    text = "?";
                }
                bool drawResult = false;
                if (i == 0 && m_step >= 3)
                {
                    text = strs[1];
                    drawResult = true;
                }
                if (i == 1 && m_step >= 4)
                {
                    text = strs[2];
                    drawResult = true;
                }
                if (i == 2 && m_step >= 5)
                {
                    text = strs[3];
                    drawResult = true;
                }
                if (i == 3 && m_step >= 6)
                {
                    text = strs[4];
                    drawResult = true;
                }
                RECT ellipseRect = new RECT(left, top, left + 120, top + 120);
                if (drawResult)
                {
                    if (text == "1")
                    {
                        paint.FillGradientEllipse(COLOR.ARGB(0, 255, 0), COLOR.ARGB(0, 200, 0), ellipseRect, 90);
                    }
                    else if (text == "0")
                    {
                        paint.FillGradientEllipse(COLOR.ARGB(255, 0, 0), COLOR.ARGB(200, 0, 0), ellipseRect, 90);
                        paint.DrawLine(COLOR.ARGB(255, 255, 255), 10, 0, ellipseRect.left, ellipseRect.top, ellipseRect.right, ellipseRect.bottom);
                        paint.DrawLine(COLOR.ARGB(255, 255, 255), 10, 0, ellipseRect.left, ellipseRect.bottom, ellipseRect.right, ellipseRect.top);
                    }
                }
                else
                {
                    paint.FillGradientEllipse(COLOR.ARGB(255, 255, 0), COLOR.ARGB(200, 200, 0), ellipseRect, 90);
                    SIZE nSize = paint.TextSize(text, nFont);
                    RECT nRect = new RECT();
                    nRect.left = left + 120 / 2 - nSize.cx / 2;
                    nRect.top = top + 120 / 2 - nSize.cy / 2;
                    paint.DrawText(text, COLOR.ARGB(255, 0, 0), nFont, nRect);
                }
                left += 130;
            }
        }

        public override void OnTimer(int timerID)
        {
            base.OnTimer(timerID);
            if (m_step == 1)
            {
                int idx = m_rd.Next(0, 4);
                int idx2 = m_rd.Next(0, 4);
                String old1 = m_records[idx];
                String old2 = m_records[idx2];
                m_records[idx2] = old1;
                m_records[idx] = old2;
                Invalidate();
            }
        }
    }
}