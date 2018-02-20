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
            BackColor = COLOR.EMPTY;
            BorderColor = COLOR.EMPTY;
            Font = new FONT("微软雅黑", 100, true, false, true);
            ForeColor = COLOR.ARGB(255, 255, 40, 40);
        }

        /// <summary>
        /// 子弹
        /// </summary>
        public List<Bullet> m_bullets = new List<Bullet>();

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
        /// 计时器
        /// </summary>
        private Stopwatch m_stopWatch = new Stopwatch();

        /// <summary>
        /// 系统颜色
        /// </summary>
        private long[] m_sysColors = new long[] { COLOR.ARGB(200, 0, 0) };

        /// <summary>
        /// 计数
        /// </summary>
        private int m_ticks;

        /// <summary>
        /// 秒表ID
        /// </summary>
        private int m_timerID = ControlA.GetNewTimerID();

        [SuppressUnmanagedCodeSecurity, DllImport("owmath.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void M107(float x1, float y1, float x2, float y2, float oX, float oY, ref float k, ref float b);

        private MainFrame m_mainFrame;

        /// <summary>
        /// 获取或设置主窗体
        /// </summary>
        public MainFrame MainFrame
        {
            get { return m_mainFrame; }
            set { m_mainFrame = value; }
        }

        /// <summary>
        /// 是否包含坐标
        /// </summary>
        /// <param name="point">坐标点</param>
        /// <returns>是否包含</returns>
        public override bool ContainsPoint(POINT point)
        {
            return false;
        }

        /// <summary>
        /// 创建子弹
        /// </summary>
        public void CreateBullets(int count)
        {
            int speed = 2;
            for (int i = 0; i < count; i++)
            {
                Bullet bullet = new Bullet();
                float k = 0f;
                float b = 0f;
                while (k == 0 && b == 0)
                {
                    POINT location = new POINT(m_random.Next(20, Width - 20), -5);
                    bullet.Location = location;
                    M107(location.x, location.y, (float)m_random.Next(0, Width), Height, 0f, 0f, ref k, ref b);
                }
                bullet.K = k;
                bullet.B = b;
                bullet.Speed = speed;
                bullet.BackColor = m_sysColors[m_random.Next(0, m_sysColors.Length)];
                m_bullets.Add(bullet);
                speed++;
            }
        }

        /// <summary>
        /// 销毁资源方法
        /// </summary>
        public override void Dispose()
        {
            if(!IsDisposed)
            {
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
                for (int i = 0; i < m_bullets.Count; i++)
                {
                    Bullet bullet = m_bullets[i];
                    if (!bullet.IsClick)
                    {
                        POINT location = bullet.Location;
                        RECT rect = new RECT(location.x - 25, location.y - 25, location.x + 25, location.y + 25);
                        if (mp.x >= rect.left && mp.x <= rect.right && mp.y >= rect.top && mp.y <= rect.bottom)
                        {
                            bullet.IsClick = true;
                            bullet.Speed = 4;
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
                if (m_mainFrame.Mode == 5)
                {
                    POINT mp = MousePoint;
                    int eSize = 100;
                    RECT eRect = new RECT();
                    eRect.left = mp.x - eSize;
                    eRect.top = mp.y - eSize;
                    eRect.right = mp.x + eSize;
                    eRect.bottom = mp.y + eSize;
                    paint.DrawImage("file='fire.jpg' highcolor='120,120,120' lowcolor='0,0,0'", eRect);
                    int eSize2 = (m_ticks / 3) % 6 + 2;
                    int eyeX1 = eRect.left + 70, eyeX2 = eRect.left + 120, eyeY1 = eRect.bottom - 40;
                    paint.FillEllipse(COLOR.ARGB(0, 0, 50), new RECT(eyeX1 - eSize2, eyeY1 - eSize2, eyeX1 + eSize2, eyeY1 + eSize2));
                    paint.FillEllipse(COLOR.ARGB(0, 0, 50), new RECT(eyeX2 - eSize2, eyeY1 - eSize2, eyeX2 + eSize2, eyeY1 + eSize2));
                }
                String text = Text;
                FONT font = Font;
                SIZE tSize = new SIZE(195, 234);
                POINT tPoint = new POINT(width / 2 - tSize.cx / 2, height / 2 - tSize.cy / 2);
                RECT tRect = new RECT(tPoint.x, tPoint.y, tPoint.x + tSize.cx, tPoint.y + tSize.cy);
                if (text == "1")
                {
                    paint.DrawImage("file='num1.jpg' highcolor='150,150,150' lowcolor='0,0,0'", tRect);
                }
                else if (text == "2")
                {
                    paint.DrawImage("file='num2.jpg' highcolor='150,150,150' lowcolor='0,0,0'", tRect);
                }
                else if (text == "3")
                {
                    paint.DrawImage("file='num3.jpg' highcolor='150,150,150' lowcolor='0,0,0'", tRect);
                }
                else
                {
                    tSize = paint.TextSize(text, Font);
                    tPoint = new POINT(width / 2 - tSize.cx / 2, height / 2 - tSize.cy / 2);
                    tRect = new RECT(tPoint.x, tPoint.y, tPoint.x + tSize.cx, tPoint.y + tSize.cy);
                    paint.DrawText(text, ForeColor, Font, tRect);
                }
                int pointsSize = m_points.Count;
                for (int i = 0; i < pointsSize; i++)
                {
                    POINT point = m_points[i];
                    RECT pRect = new RECT(point.x - 10, point.y - 10, point.x + 10, point.y + 10);
                    paint.FillGradientEllipse(COLOR.ARGB(50, 255, 255, 255), COLOR.ARGB(50, 220, 220, 220), pRect, 90);
                    paint.DrawEllipse(COLOR.ARGB(50, 100, 100, 100), 1, 0, pRect);
                }
            }
        }

        /// <summary>
        /// 秒表方法
        /// </summary>
        /// <param name="timerID">秒表ID</param>
        public override void OnTimer(int timerID)
        {
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
            m_ticks++;
            if (m_ticks > 100000)
            {
                m_ticks = 0;
            }
        }

        /// <summary>
        /// 移除子弹
        /// </summary>
        private void RemoveOutBullets()
        {
            bool hasRemove = false;
            int m_bulletsSize = m_bullets.Count;
            for (int i = 0; i < m_bulletsSize; i++)
            {
                Bullet bullet = m_bullets[i];
                POINT bmp = bullet.Location;
                float x = bmp.x;
                float y = bmp.y;
                if ((y < -10) || (y > Height + 50))
                {
                    m_bullets.RemoveAt(i);
                    i--;
                    m_bulletsSize--;
                    if (!bullet.IsClick && y > Height + 50)
                    {
                        hasRemove = true;
                    }
                }
            }
            if (hasRemove)
            {
                for (int i = 0; i < m_bulletsSize; i++)
                {
                    Bullet bullet = m_bullets[i];
                    if (!bullet.IsClick)
                    {
                        bullet.IsClick = true;
                        bullet.Speed = 4;
                        POINT location = bullet.Location;
                        RECT rect = new RECT(location.x - 25, location.y - 25, location.x + 25, location.y + 25);
                        for (int p = 0; p < 10; p++)
                        {
                            POINT point = new POINT(m_random.Next(rect.left - 100, rect.right + 100), m_random.Next(rect.top - 100, rect.bottom + 200));
                            m_points.Add(point);
                        }
                    }
                }
                if (m_mainFrame != null)
                {
                    m_mainFrame.BulletLose();
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
                    RECT drawRect = new RECT(m_location.x - m_tick - 10, m_location.y - m_tick - 10, m_location.x + m_tick + 10, m_location.y + m_tick + 10);
                    paint.FillGradientEllipse(m_backColor, COLOR.RatioColor(paint, m_backColor, 1.1), drawRect, 90);
                    paint.DrawEllipse(m_backColor, 2, 0, new RECT(m_location.x - 25, m_location.y - 25, m_location.x + 25, m_location.y + 25));
                    paint.DrawLine(m_backColor, 5, 0, m_location.x + 20, m_location.y - 15, m_location.x + 30, m_location.y - 25);
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
            }
        }
    }
}
