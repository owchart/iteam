/*****************************************************************************\
*                                                                             *
* CometA.cs -    Comet functions, types, and definitions                       *
*                                                                             *
*               Version 4.00 ★★★                                           *
*                                                                             *
*               Copyright (c) 2016-2016, Lord's UI. All rights reserved.      *
*                                                                             *
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace OwLib
{
    /// <summary>
    /// 彗星
    /// </summary>
    public class CometA : ControlA
    {
        /// <summary>
        /// 创建彗星
        /// </summary>
        public CometA()
        {
            BorderColor = COLOR.EMPTY;
            Size = new SIZE(100, 90);
        }

        /// <summary>
        /// 随机种子
        /// </summary>
        private static Random m_rd = new Random();

        /// <summary>
        /// 秒表ID
        /// </summary>
        private int m_timerID = ControlA.GetNewTimerID();

        /// <summary>
        /// 是否包含控件
        /// </summary>
        /// <param name="control">控件</param>
        /// <returns>是否包含</returns>
        public override bool ContainsControl(ControlA control)
        {
            return false;
        }

        /// <summary>
        /// 销毁方法
        /// </summary>
        public override void Dispose()
        {
            if (!IsDisposed)
            {
                StopTimer(m_timerID);
                base.Dispose();
            }
        }

        /// <summary>
        /// 添加控件方法
        /// </summary>
        public override void OnAdd()
        {
            StartTimer(m_timerID, 10);
            SIZE displaySize = Native.DisplaySize;
            POINT location = new POINT(m_rd.Next(0, displaySize.cx + 300), -Height + m_rd.Next(0, 200));
            Location = location;
        }

        /// <summary>
        /// 重绘背景方法
        /// </summary>
        /// <param name="paint">绘图对象</param>
        /// <param name="clipRect">裁剪区域</param>
        public override void OnPaintBackground(CPaint paint, RECT clipRect)
        {
            int width = Width, height = Height;
            float PI = 3.14159265f;
	        int realAngle = -40;
	        int radius = 15;
	        int halfRadius = radius / 2;
	        int r2 = (int)(1.5 * radius);
	        int r3 = 2 * radius;
	        int exLen = 2 * radius;
	        int angle = 60;
	        int lineWidth = 5;
            long color1 = COLOR.ARGB(255, 255, 122, 40);
            long color2 = COLOR.ARGB((int)(255 * 0.8), 255, 40, 40);
            long color3 = COLOR.ARGB((int)(255 * 0.6), 255, 40, 40);
	        POINT []points1 = new POINT[7];
	        POINT p10 = new POINT(10, 80);
	        POINT p11 = new POINT((int)(p10.x + Math.Cos(angle * PI / 180) * radius), (int)(p10.y - Math.Sin(angle * PI / 180) * radius));
	        POINT p12 = new POINT(p11.x + radius, p11.y);
	        POINT p13 = new POINT(p10.x + 2 * radius, p10.y);
	        POINT p15 = new POINT((int)(p10.x + Math.Cos(angle * PI / 180) * radius), (int)(p10.y + Math.Sin(angle * PI /180)  *radius));
	        POINT p14 = new POINT((int)(p15.x + Math.Cos(0 * PI / 180) * radius), p15.y);
	        POINT p16 = p10;
	        points1[0] = paint.Rotate(p10, p10, realAngle);
	        points1[1] = paint.Rotate(p10, p11, realAngle);
	        points1[2] = paint.Rotate(p10, p12, realAngle);
	        points1[3] = paint.Rotate(p10, p13, realAngle);
	        points1[4] = paint.Rotate(p10, p14, realAngle);
	        points1[5] = paint.Rotate(p10, p15, realAngle);
	        points1[6] = paint.Rotate(p10, p16, realAngle);
	        paint.FillPolygon(color1, points1);
	        POINT []points2 = new POINT[7];
	        POINT p20 = p13;
	        POINT p21 = p12;
	        POINT p22 = new POINT(p21.x + r2, p21.y);
	        POINT p23 = new POINT(p10.x + 2 * radius + r2, p10.y);
	        POINT p24 = new POINT(p14.x + r2, p14.y);
	        POINT p25 = p14;
	        POINT p26 = p13;
	        points2[0] = paint.Rotate(p10, p20, realAngle);
	        points2[1] = paint.Rotate(p10, p21, realAngle);
	        points2[2] = paint.Rotate(p10, p22, realAngle);
	        points2[3] = paint.Rotate(p10, p23, realAngle);
	        points2[4] = paint.Rotate(p10, p24, realAngle);
	        points2[5] = paint.Rotate(p10, p25, realAngle);
	        points2[6] = paint.Rotate(p10, p26, realAngle);
	        paint.FillPolygon(color2, points2);
	        POINT []points3 = new POINT[7];
	        POINT p30 = p23;
	        POINT p31 = p22;
	        POINT p32 = new POINT(p31.x + r3, p31.y);
	        POINT p33 = new POINT(p10.x + 2 * radius + r2 + r3, p10.y);
	        POINT p34 = new POINT(p24.x + r3, p24.y);
	        POINT p35 = p24;
	        POINT p36 = p23;
	        points3[0] = paint.Rotate(p10, p30, realAngle);
	        points3[1] = paint.Rotate(p10, p31, realAngle);
	        points3[2] = paint.Rotate(p10, p32, realAngle);
	        points3[3] = paint.Rotate(p10, p33, realAngle);
	        points3[4] = paint.Rotate(p10, p34, realAngle);
	        points3[5] = paint.Rotate(p10, p35, realAngle);
	        points3[6] = paint.Rotate(p10, p36, realAngle);
	        paint.FillPolygon(color3, points3);
	        paint.SetLineCap(2, 2);
	        POINT point11 = new POINT((points1[2].x + points1[3].x) / 2, (points1[2].y + points1[3].y) / 2);
	        POINT point12 = new POINT((p22.x + p23.x) / 2 - halfRadius, (p22.y + p23.y) / 2);
	        point12 = paint.Rotate(p10, point12, realAngle);
	        paint.DrawLine(color1, lineWidth, 0, point11, point12);
	        POINT point13 = points1[3];
	        POINT point14 = new POINT(p23.x - halfRadius, p23.y);
	        point14 = paint.Rotate(p10, point14, realAngle);
	        paint.DrawLine(color1, lineWidth, 0, point13, point14);
	        POINT point15 = new POINT((points1[3].x + points1[4].x) / 2, (points1[3].y + points1[4].y) / 2);
	        POINT point16 = new POINT((p23.x + p24.x) / 2 - radius, (p23.y + p24.y) / 2);
	        point16 = paint.Rotate(p10, point16, realAngle);
	        paint.DrawLine(color1, lineWidth, 0, point15, point16);
	        POINT point21 = new POINT((points2[2].x + points2[3].x) / 2, (points2[2].y + points2[3].y) / 2);
	        POINT point22 = new POINT((p32.x + p33.x) / 2 - halfRadius, (p32.y + p33.y) / 2);
	        point22 = paint.Rotate(p10, point22, realAngle);
	        paint.DrawLine(color2, lineWidth, 0, point21, point22);
	        POINT point23 = points2[3];
	        POINT point24 = new POINT(p33.x - halfRadius - radius, p33.y);
	        point24 = paint.Rotate(p10, point24, realAngle);
	        paint.DrawLine(color2, lineWidth, 0, point23, point24);
	        POINT point25 = new POINT((points2[3].x + points2[4].x) / 2, (points2[3].y + points2[4].y) / 2);
	        POINT point26 = new POINT((p33.x + p34.x) / 2 - halfRadius, (p33.y + p34.y) / 2);
	        point26 = paint.Rotate(p10, point26, realAngle);
	        paint.DrawLine(color2, lineWidth, 0, point25, point26);	
	        POINT point31 = new POINT((p32.x + p33.x) / 2, (p32.y + p33.y) / 2);
	        POINT point32 = new POINT((p32.x + p33.x) / 2 + exLen, (p32.y + p33.y) / 2);
	        POINT temp31 = new POINT((point31.x + point32.x) / 2 - 3, (point31.y + point32.y) / 2);
	        POINT temp32 = new POINT((point31.x + point32.x) / 2 + 3, (point31.y + point32.y) / 2);
	        point31 = paint.Rotate(p10, point31, realAngle);
	        temp31 = paint.Rotate(p10, temp31, realAngle);
	        temp32 = paint.Rotate(p10, temp32, realAngle);
	        point32 = paint.Rotate(p10, point32, realAngle);
	        paint.DrawLine(color3, lineWidth, 0, point31, temp31);
	        paint.DrawLine(color3, lineWidth, 0, temp32, point32);
	        POINT point33 = p33;
	        POINT point34 = new POINT(p33.x + exLen, p33.y);
	        POINT temp33 = new POINT(point34.x - halfRadius - 3, p33.y);
	        POINT temp34 = new POINT(point34.x - halfRadius + 3, p33.y);
	        point33 = paint.Rotate(p10, point33, realAngle);
	        temp33 = paint.Rotate(p10, temp33, realAngle);
	        temp34 = paint.Rotate(p10, temp34, realAngle);
	        point34 = paint.Rotate(p10, point34, realAngle);
	        paint.DrawLine(color3, lineWidth, 0, point33, temp33);
	        paint.DrawLine(color3, lineWidth, 0, temp34, point34);
	        POINT point35 = new POINT((points3[3].x + points3[4].x) / 2, (points3[3].y + points3[4].y) / 2);
	        POINT point36 = new POINT((p33.x + p34.x) / 2 + exLen / 2, (p33.y + p34.y) / 2);
	        point36 = paint.Rotate(p10, point36, realAngle);
	        paint.DrawLine(color3, lineWidth, 0, point35, point36);
	        paint.SetLineCap(0, 0);
        }

        /// <summary>
        /// 秒表方法
        /// </summary>
        /// <param name="timerID">秒表ID</param>
        public override void OnTimer(int timerID)
        {
            POINT location = Location;
            SIZE size = Size;
            SIZE displaySize = Native.DisplaySize;
            if (location.x + size.cx < 0 || location.y + size.cy > displaySize.cy)
            {
                POINT newLocation = new POINT(m_rd.Next(0, displaySize.cx + 300), -Height);
                Location = newLocation;
                Opacity = 1;
            }
            else
            {
                if (location.y + size.cy > displaySize.cy - 50)
                {
                    float opacity = Opacity;
                    opacity -= 0.01f;
                    if (opacity < 0.1f)
                    {
                        opacity = 0.1f;
                    }
                    Opacity = opacity;
                }
                location.x -= 10;
                location.y += 10;
                Location = location;
            }
        }
    }
}
