/*****************************************************************************\
*                                                                             *
* Sky.cs - Sky functions, types, and definitions.                             *
*                                                                             *
*               Version 1.00  ★★★                                          *
*                                                                             *
*               Copyright (c) 2016-2016, Sky. All rights reserved.            *
*               Created by Lord 2016/6/1.                                     *
*                                                                             *
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using OwLib;
using System.Diagnostics;
using System.Security;
using System.Runtime.InteropServices;

namespace OwLib
{
    /// <summary>
    /// 天空
    /// </summary>
    public class Sky : ControlA
    {
        /// <summary>
        /// 创建填空
        /// </summary>
        public Sky()
        {
            BackColor = COLOR.ARGB(255, 255, 255);
            BorderColor = COLOR.EMPTY;
            ForeColor = COLOR.ARGB(0, 0, 0);
            Font = new FONT("Arial", 80, true, false, true);
        }

        /// <summary>
        /// 子弹
        /// </summary>
        private List<Bullet> m_bullets = new List<Bullet>();

        /// <summary>
        /// 难度
        /// </summary>
        private int m_difficult = 2;

        /// <summary>
        /// 所有成员
        /// </summary>
        private List<String> m_members = new List<String>();

        /// <summary>
        /// 漂浮的点
        /// </summary>
        private List<POINT> m_points = new List<POINT>();

        /// <summary>
        /// 随机种子
        /// </summary>
        private Random m_random = new Random();

        /// <summary>
        /// 当前分数
        /// </summary>
        private int m_score;

        /// <summary>
        /// 计时器
        /// </summary>
        private Stopwatch m_stopWatch = new Stopwatch();

        /// <summary>
        /// 系统颜色
        /// </summary>
        private long[] m_sysColors = new long[] { COLOR.ARGB(0, 0, 0), COLOR.ARGB(255, 255, 0), COLOR.ARGB(255, 0, 255),
            COLOR.ARGB(0, 255, 0), COLOR.ARGB(82, 255, 255), COLOR.ARGB(255, 82, 82) };

        /// <summary>
        /// 计数
        /// </summary>
        private int m_ticks;

        /// <summary>
        /// 秒表ID
        /// </summary>
        private int m_timerID = ControlA.GetNewTimerID();

        /// <summary>
        /// 总计
        /// </summary>
        private int m_total;

        [SuppressUnmanagedCodeSecurity, DllImport("owmath.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void M107(float x1, float y1, float x2, float y2, float oX, float oY, ref float k, ref float b);

        /// <summary>
        /// 创建子弹
        /// </summary>
        private void CreateBullets()
        {
            int size = Width / 200;
            if (size < 2)
            {
                size = 2;
            }
            for (int i = 0; i < size; i++)
            {
                Bullet bullet = new Bullet();
                if (m_random.Next(0, 30) == 0)
                {
                    bullet.Security = SecurityService.GetAutoSecurity();
                }
                float k = 0f;
                float b = 0f;
                while (k == 0 && b == 0)
                {
                    int type = m_random.Next(0, 4);
                    POINT location = new POINT();
                    if (type == 0)
                    {
                        location = new POINT(-5, m_random.Next(0, 100));
                    }
                    else if (type == 1)
                    {
                        location = new POINT(Width + 5, m_random.Next(0, 100));
                    }
                    else
                    {
                        location = new POINT(m_random.Next(0, Width), -5);
                    }
                    bullet.Location = location;
                    M107(location.x, location.y, (float)m_random.Next(0, Width), Height, 0f, 0f, ref k, ref b);
                }
                bullet.K = k;
                bullet.B = b;
                int d = m_difficult / 4;
                bullet.Speed = m_random.Next(2, 5 + m_difficult);
                bullet.BackColor = m_sysColors[m_random.Next(0, m_sysColors.Length)];
                if (bullet.Security != null)
                {
                    m_bullets.Add(bullet);
                }
            }
            size = 2;
            for (int i = 0; i < size; i++)
            {
                if (m_random.Next(0, 2) == 0)
                {
                    Bullet bullet = new Bullet();
                    if (m_random.Next(0, 10) == 0)
                    {
                        bullet.Security = SecurityService.GetAutoSecurity();
                    }
                    float k = 0f;
                    float b = 0f;
                    while (k == 0 && b == 0)
                    {
                        int x = m_random.Next(0, Width);
                        bullet.Location = new POINT(x, -5);
                        M107((float)x, bullet.Location.y, m_random.Next(0, Width), Height, 0f, 0f, ref k, ref b);
                    }
                    bullet.K = k;
                    bullet.B = b;
                    bullet.Speed = m_random.Next(3, 10) + m_difficult;
                    bullet.BackColor = m_sysColors[m_random.Next(0, m_sysColors.Length)];
                    if (bullet.Security != null)
                    {
                        m_bullets.Add(bullet);
                    }
                }
            }
        }

        /// <summary>
        /// 销毁资源方法
        /// </summary>
        public override void Dispose()
        {
            if(!IsDisposed)
            {
                StopTimer(m_timerID);
                m_bullets.Clear();
            }
            base.Dispose();
        }

        /// <summary>
        /// 移除子弹
        /// </summary>
        private void MoveBullets()
        {
            if (m_ticks % 2 == 0)
            {
                int bulletsSize = m_bullets.Count;
                for (int i = 0; i < bulletsSize; i++)
                {
                    Bullet bullet = m_bullets[i];
                    float k = bullet.K;
                    float b = bullet.B;
                    int speed = bullet.Speed;
                    float y = bullet.Location.y + bullet.Speed;
                    float x = (y - b) / k;
                    int subX = Math.Abs(bullet.Location.x - (int)x);
                    int subY = Math.Abs(bullet.Location.y - (int)y);
                    POINT bmp = bullet.Location;
                    if (subX > speed)
                    {
                        if (bmp.x > x)
                        {
                            x = bmp.x + speed;
                        }
                        else if (bmp.x > x)
                        {
                            x = bmp.x - speed;
                        }
                    }
                    if (subY > speed)
                    {
                        if (bmp.y > y)
                        {
                            y = bmp.y + speed;
                        }
                        else if (bmp.y > y)
                        {
                            y = bmp.y - speed;
                        }
                    }
                    bullet.Location = new POINT(x, y);
                }
            }
        }

        /// <summary>
        /// 控件添加方法
        /// </summary>
        public override void OnAdd()
        {
            base.OnAdd();
            m_stopWatch.Start();
            StartTimer(m_timerID, 10);
        }

        /// <summary>
        /// 鼠标按下方法
        /// </summary>
        /// <param name="mp">坐标</param>
        /// <param name="button">按钮</param>
        /// <param name="clicks">点击方法</param>
        /// <param name="delta">滚轮值</param>
        public override void OnMouseDown(POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            int width = Width, height = Height;
            base.OnMouseDown(mp, button, clicks, delta);
            if (button == MouseButtonsA.Left && clicks == 1)
            {
                m_total++;
                for (int i = 0; i < m_bullets.Count; i++)
                {
                    Bullet bullet = m_bullets[i];
                    if (!bullet.IsClick)
                    {
                        POINT location = bullet.Location;
                        RECT rect = new RECT(location.x - 25, location.y - 25, location.x + 25, location.y + 25);
                        if (mp.x >= rect.left && mp.x <= rect.right && mp.y >= rect.top && mp.y <= rect.bottom)
                        {
                            if (bullet.Security != null)
                            {
                                bullet.IsClick = true;
                                bullet.Speed = 4;
                                m_score++;
                                String autoMember = SecurityService.GetAutoMember();
                                while (m_members.Contains(autoMember))
                                {
                                    autoMember = SecurityService.GetAutoMember();
                                }
                                m_members.Add(autoMember);
                                for (int p = 0; p < 10; p++)
                                {
                                    POINT point = new POINT(m_random.Next(rect.left - 100, rect.right + 100), m_random.Next(rect.top - 100, rect.bottom + 200));
                                    m_points.Add(point);
                                }
                                Invalidate();
                                break;
                            }
                        }
                    }
                }
            }
            else if (button == MouseButtonsA.Right && clicks == 1)
            {
                m_score = 0;
                m_total = 0;
                m_bullets.Clear();
                m_difficult = 1;
                m_stopWatch.Stop();
                m_stopWatch.Reset();
                m_stopWatch.Start();
                m_members.Clear();
                Invalidate();
            }
        }

        /// <summary>
        /// 重绘方法
        /// </summary>
        /// <param name="paint">绘图对象</param>
        /// <param name="clipRect">裁剪区域</param>
        public override void OnPaint(CPaint paint, RECT clipRect)
        {
            int width = Width;
            int height = Height;
            if (width > 0 && height > 0)
            {
                //绘制背景
                RECT drawRect = new RECT(0, 0, width, height);
                paint.FillRect(GetPaintingBackColor(), clipRect);  
                for (int i = 0; i < m_bullets.Count; i++)
                {
                    Bullet bullet = m_bullets[i];
                    bullet.OnPaintBackground(paint);
                }
                String text = String.Format("{0}/{1}", m_score, m_total);
                FONT font = Font;
                SIZE tSize = paint.TextSize(text, font);
                POINT tPoint = new POINT(width / 2 - tSize.cx / 2, 10);
                RECT tRect = new RECT(tPoint.x, tPoint.y, tPoint.x + tSize.cx, tSize.cy);
                //paint.DrawText(text, ForeColor, font, tRect);
                int pointsSize = m_points.Count;
                for (int i = 0; i < pointsSize; i++)
                {
                    POINT point = m_points[i];
                    RECT pRect = new RECT(point.x - 10, point.y - 10, point.x + 10, point.y + 10);
                    paint.FillGradientEllipse(COLOR.ARGB(50, 0, 0, 0), COLOR.ARGB(50, 30, 30, 30), pRect, 90);
                    paint.DrawEllipse(COLOR.ARGB(50, 100, 100, 100), 1, 0, pRect);
                }
                String title = "随机选拔";
                FONT tiFont = new FONT("微软雅黑", 30, true, false, false);
                SIZE tiSize = paint.TextSize(title, tiFont);
                RECT tiRect = new RECT();
                tiRect.left = (width - tiSize.cx) / 2;
                tiRect.top = height - 120;
                tiRect.right = tiRect.left + tiSize.cx;
                tiRect.bottom = tiRect.top + tiSize.cy;
                paint.DrawText(title, COLOR.ARGB(255, 0, 0), tiFont, tiRect);
                int membersSize = m_members.Count;
                if (membersSize > 0)
                {
                    int avgWidth = width / membersSize, mLeft = 0;
                    for (int i = 0; i < membersSize; i++)
                    {
                        FONT mFont = new FONT("微软雅黑", 35, true, false, false);
                        String member = m_members[i];
                        SIZE mSize = paint.TextSize(member, mFont);
                        RECT mRect = new RECT();
                        mRect.left = mLeft + (avgWidth - mSize.cx) / 2;
                        mRect.top = height - 80;
                        mRect.right = mRect.left + mSize.cx;
                        mRect.bottom = mRect.top + mSize.cy;
                        paint.DrawText(member, m_sysColors[m_random.Next(0, m_sysColors.Length)], mFont, mRect);
                        mLeft += avgWidth;
                    }
                }
            }
        }

        /// <summary>
        /// 秒表方法
        /// </summary>
        /// <param name="timerID">秒表ID</param>
        public override void OnTimer(int timerID)
        {
            if ((m_ticks % 5) == 0)
            {
                CreateBullets();
            }
            if (m_ticks % 500 == 0 && m_difficult < 8)
            {
                m_difficult++;
            }
            MoveBullets();
            RemoveOutBullets();
            int width = Width, height = Height;
            int pointsSize = m_points.Count;
            for (int i = 0; i < pointsSize; i++)
            {
                POINT point = m_points[i];
                point.x -= m_random.Next(1, 20) - 10;
                point.y -= m_random.Next(10, 20);
                m_points[i] = point;
                if (point.y < 0)
                {
                    m_points.RemoveAt(i);
                    i--;
                    pointsSize--;
                }
            }
            Invalidate();
            m_ticks++;
        }

        /// <summary>
        /// 移除子弹
        /// </summary>
        private void RemoveOutBullets()
        {
            int m_bulletsSize = m_bullets.Count;
            for (int i = 0; i < m_bulletsSize; i++)
            {
                Bullet bullet = m_bullets[i];
                POINT bmp = bullet.Location;
                float x = bmp.x;
                float y = bmp.y;
                if ((y < -10) || (y > Height + 50))
                {
                    if (!bullet.IsClick)
                    {
                        m_total++;
                    }
                    m_bullets.RemoveAt(i);
                    i--;
                    m_bulletsSize--;
                }
            }
        }
    }

    /// <summary>
    /// 子弹
    /// </summary>
    public class Bullet
    {
        /// <summary>
        /// 创建子弹
        /// </summary>
        public Bullet()
        {
            
        }

        /// <summary>
        /// 模式
        /// </summary>
        private int m_mode;

        /// <summary>
        /// 随机种子
        /// </summary>
        private Random m_rd = new Random();

        /// <summary>
        /// 计数
        /// </summary>
        private int m_tick = 1;

        /// <summary>
        /// 计数2
        /// </summary>
        private int m_tick2 = 2;

        /// <summary>
        /// 计数3
        /// </summary>
        private int m_tick3 = 20, m_tick4 = 20, m_tick5 = 20;

        private float m_b;

        /// <summary>
        /// 获取或设置直线参数B
        /// </summary>
        public float B
        {
            get { return m_b; }
            set { m_b = value; }
        }

        private long m_backColor = COLOR.ARGB(255, 0, 0);

        /// <summary>
        /// 获取或设置背景色
        /// </summary>
        public long BackColor
        {
            get { return m_backColor; }
            set { m_backColor = value; }
        }

        private bool m_isClick;

        /// <summary>
        /// 是否被点击
        /// </summary>
        public bool IsClick
        {
            get { return m_isClick; }
            set { m_isClick = value; }
        }

        private float m_k;

        /// <summary>
        /// 获取或设置直线参数K
        /// </summary>
        public float K
        {
            get { return m_k; }
            set { m_k = value; }
        }

        private POINT m_location = new POINT();

        /// <summary>
        /// 获取或设置位置
        /// </summary>
        public POINT Location
        {
            get { return m_location; }
            set { m_location = value; }
        }

        private Security m_security;

        /// <summary>
        /// 获取或设置股票
        /// </summary>
        public Security Security
        {
            get { return m_security; }
            set { m_security = value; }
        }

        private int m_speed = 1;

        /// <summary>
        /// 获取或设置速度
        /// </summary>
        public int Speed
        {
            get { return m_speed; }
            set { m_speed = value; }
        }

        /// <summary>
        /// 重绘背景方法
        /// </summary>
        /// <param name="paint">绘图对象</param>
        public void OnPaintBackground(CPaint paint)
        {
            if (m_k != 0 && m_b != 0)
            {
                if (IsClick)
                {
                    int a = 0, r = 0, g = 0, b = 0;
                    COLOR.ToARGB(paint, m_backColor, ref a, ref r, ref g, ref b);
                    RECT bRect = new RECT(m_location.x - m_tick3, m_location.y - m_tick3, m_location.x + m_tick3, m_location.y + m_tick3);
                    if (m_tick3 < 400)
                    {
                        paint.FillEllipse(COLOR.ARGB(200 - 200 * m_tick3 / 400, r, g, b), bRect);
                    }
                    paint.DrawEllipse(m_backColor, 2, 0, bRect);
                    m_tick3 += 40;
                    bRect = new RECT(m_location.x - m_tick4, m_location.y - m_tick4, m_location.x + m_tick4, m_location.y + m_tick4);
                    if (m_tick4 < 300)
                    {

                        paint.FillEllipse(COLOR.ARGB(150 - 150 * m_tick4 / 400, r, g, b), bRect);
                    }
                    m_tick4 += 20;
                    bRect = new RECT(m_location.x - m_tick5, m_location.y - m_tick5, m_location.x + m_tick5, m_location.y + m_tick5);
                    if (m_tick5 < 200)
                    {
                        paint.FillEllipse(COLOR.ARGB(100 - 100 * m_tick5 / 400, r, g, b), bRect);
                    }
                    m_tick5 += 10;
                }
                else
                {
                    RECT drawRect = new RECT(m_location.x - m_tick, m_location.y - m_tick, m_location.x + m_tick, m_location.y + m_tick);
                    paint.FillGradientEllipse(m_backColor, COLOR.RatioColor(paint, m_backColor, 1.1), drawRect, 90);
                    paint.DrawEllipse(m_backColor, 2, 0, new RECT(m_location.x - 20, m_location.y - 20, m_location.x + 20, m_location.y + 20));
                }
                if (m_tick2 % 5 == 0)
                {
                    if (m_mode == 0)
                    {
                        m_tick++;
                        if (m_tick > 10)
                        {
                            m_mode = 1;
                        }
                    }
                    else if (m_mode == 1)
                    {
                        m_tick--;
                        if (m_tick < 4)
                        {
                            m_mode = 0;
                        }
                    }
                }
                m_tick2++;
                if (m_tick2 > 1000)
                {
                    m_tick2 = 0;
                }
                if (m_security != null)
                {
                    SecurityLatestData latestData = new SecurityLatestData();
                    if (SecurityService.GetLatestData(m_security.m_code, ref latestData) == 0)
                    {
                        latestData.m_securityName = m_security.m_name;
                    }
                    if (IsClick)
                    {
                        FONT nFont = new FONT("Arial", 30, true, false, false);
                        FONT iFont = new FONT("SimSum", 30, true, false, false);
                        String name = m_security.m_name;
                        SIZE tSize = paint.TextSize(name, iFont);
                        int x = m_location.x - tSize.cx / 2;
                        int mid = x + tSize.cx / 2;
                        int y = m_location.y - 90;
                        RECT tRect = new RECT(x, y, x + tSize.cx, y + tSize.cy);
                        paint.DrawText(name, m_backColor, iFont, tRect);
                        double diff = 0;
                        if (latestData.m_lastClose > 0)
                        {
                            diff = 100 * (latestData.m_close - latestData.m_lastClose) / latestData.m_lastClose;
                        }
                        y += tSize.cy;
                        String strClose = latestData.m_close.ToString();
                        tSize = paint.TextSize(strClose, nFont);
                        x = mid - tSize.cx / 2;
                        tRect = new RECT(x, y, x + tSize.cx, y + tSize.cy);
                        if (diff >= 0)
                        {
                            paint.DrawText(strClose, COLOR.ARGB(255, 82, 82), nFont, tRect);
                        }
                        else
                        {
                            paint.DrawText(strClose, COLOR.ARGB(82, 255, 82), nFont, tRect);
                        }
                        y += tSize.cy;
                        String value = diff.ToString("0.00") + "%";
                        if (diff >= 0)
                        {
                            value = "+" + value;
                        }
                        if (latestData.m_close == 0)
                        {
                            value = "停牌";
                        }
                        tSize = paint.TextSize(value, nFont);
                        x = mid - tSize.cx / 2;
                        tRect = new RECT(x, y, x + tSize.cx, y + tSize.cy);
                        if (diff >= 0)
                        {
                            paint.DrawText(value, COLOR.ARGB(255, 82, 82), nFont, tRect);
                        }
                        else
                        {
                            paint.DrawText(value, COLOR.ARGB(82, 255, 82), nFont, tRect);
                        }
                        y += tSize.cy;
                        double volume = latestData.m_volume / 100;
                        String unit = "";
                        if (volume > 100000000)
                        {
                            volume /= 100000000;
                            unit = "亿";
                        }
                        else if (volume > 10000)
                        {
                            volume /= 10000;
                            unit = "万";
                        }
                        String strVol = (volume).ToString("0.00") + unit;
                        tSize = paint.TextSize(strVol, nFont);
                        x = mid - tSize.cx / 2;
                        tRect = new RECT(x, y, x + tSize.cx, y + tSize.cy);
                        paint.DrawText(strVol, COLOR.ARGB(255, 255, 0), nFont, tRect);
                    }
                    else
                    {
                        FONT iFont = new FONT("SimSum", 30, true, false, false);
                        String name = m_security.m_name;
                        SIZE tSize = paint.TextSize(name, iFont);
                        int x = m_location.x - tSize.cx / 2;
                        int mid = x + tSize.cx / 2;
                        int y = m_location.y - 60;
                        RECT tRect = new RECT(x, y, x + tSize.cx, y + tSize.cy);
                        paint.DrawText(name, m_backColor, iFont, tRect);
                    }
                }
            }
        }
    }
}
