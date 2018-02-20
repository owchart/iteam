/*****************************************************************************\
*                                                                             *
* DimemsionDiv.cs -    Dimemsion div functions, types, and definitions             *
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
    /// 六维图信息
    /// </summary>
    public class DimensionInfo
    {
        /// <summary>
        /// 创建六维图信息
        /// </summary>
        public DimensionInfo()
        {
        }

        /// <summary>
        /// 创建六维图信息
        /// </summary>
        /// <param name="business">业务</param>
        /// <param name="eq">情商</param>
        /// <param name="jobID">工号</param>
        /// <param name="knowledge">学识</param>
        /// <param name="iq">智商</param>
        /// <param name="love">管理</param>
        /// <param name="technology">技术</param>
        public DimensionInfo(int business, int eq, String jobID, int knowledge, int iq, int love, int technology)
        {
            m_business = business;
            m_EQ = eq;
            m_jobID = jobID;
            m_knowledge = knowledge;
            m_IQ = iq;
            m_lead = love;
            m_technology = technology;
        }

        /// <summary>
        /// 业务
        /// </summary>
        public int m_business;

        /// <summary>
        /// 情商
        /// </summary>
        public int m_EQ;

        /// <summary>
        /// 工号
        /// </summary>
        public String m_jobID = "";

        /// <summary>
        /// 学识
        /// </summary>
        public int m_knowledge;

        /// <summary>
        /// 智商
        /// </summary>
        public int m_IQ;

        /// <summary>
        /// 管理
        /// </summary>
        public int m_lead;

        /// <summary>
        /// 技术
        /// </summary>
        public int m_technology;
    }

    /// <summary>
    /// 维度图
    /// </summary>
    public class DimemsionDiv : ButtonA
    {
        /// <summary>
        /// 创建维度图
        /// </summary>
        public DimemsionDiv()
        {
            ForeColor = COLOR.ARGB(255, 255, 255);
            Size = new SIZE(400, 400);
        }

        /// <summary>
        /// 系统颜色
        /// </summary>
        private long[] m_sysColors = new long[] { COLOR.ARGB(255, 255, 255), COLOR.ARGB(255,255,0), COLOR.ARGB(255, 0, 255),
            COLOR.ARGB(0, 255, 0), COLOR.ARGB(82, 255, 255), COLOR.ARGB(255, 82, 82) };

        private DimensionInfo m_dimension;

        /// <summary>
        /// 获取或设置六维图信息
        /// </summary>
        public DimensionInfo Dimension
        {
            get { return m_dimension; }
            set { m_dimension = value; }
        }

        /// <summary>
        /// 获取圆上的点
        /// </summary>
        /// <param name="oX">圆心横坐标</param>
        /// <param name="oY">圆心纵坐标</param>
        /// <param name="r">半径</param>
        /// <param name="angle">角度</param>
        /// <returns>圆上的点</returns>
        public POINT GetCyclePoint(int oX, int oY, int r, float angle)
        {
            POINT cyclePoint = new POINT();
            cyclePoint.x = oX + (int)(r * Math.Cos(angle * 3.14 / 180));
            cyclePoint.y = oY + (int)(r * Math.Sin(angle * 3.14 / 180));
            return cyclePoint;
        }

        /// <summary>
        /// 获取特征点
        /// </summary>
        /// <returns>坐标集合</returns>
        public POINT[] GetScopePoints()
        {
            int width = Width - 1, height = Height - 1;
            int outSize = 40;
            int oX = width / 2, oY = height / 2, r = (width - outSize * 2) / 2;
            int[] angles = new int[] { 240, 180, 120, 60, 0, -60 };
            int[] values = new int[6];
            if (m_dimension != null)
            {
                values = new int[] { m_dimension.m_business, m_dimension.m_EQ, m_dimension.m_knowledge,
                m_dimension.m_IQ, m_dimension.m_lead, m_dimension.m_technology};
            }
            POINT[] scopePoints = new POINT[6];
            for (int i = 0; i < 6; i++)
            {
                POINT cp = GetCyclePoint(oX, oY, r, angles[i]);
                scopePoints[i] = GetCyclePoint(oX, oY, r * values[i] / 100, angles[i]);
            }
            return scopePoints;
        }

        /// <summary>
        /// 获取标题的坐标点
        /// </summary>
        /// <returns>坐标点列表</returns>
        public POINT[] GetTitlePoints()
        {
            int outSize = 40;
            int width = Width - 1 - outSize * 2, height = Height - 1 - outSize * 2;
            POINT[] points = new POINT[6];
            int length = width / 2;
            points[0] = new POINT(length / 2, 0);
            points[1] = new POINT(-20, height / 2);
            points[2] = new POINT(length / 2, height);
            points[3] = new POINT(width - length / 2, height);
            points[4] = new POINT(width + 20, height / 2);
            points[5] = new POINT(width - length / 2, 0);
            int pointsSize = points.Length;
            for (int i = 0; i < pointsSize; i++)
            {
                POINT mp = points[i];
                mp.x += outSize;
                mp.y += outSize;
                points[i] = mp;
            }
            return points;
        }

        /// <summary>
        /// 绘制背景
        /// </summary>
        /// <param name="paint">绘图对象</param>
        /// <param name="clipRect">裁剪区域</param>
        public override void OnPaintBackground(CPaint paint, RECT clipRect)
        {
            int width = Width - 1, height = Height - 1;
            RECT drawRect = new RECT(0, 0, width, height);
            paint.FillRect(COLOR.ARGB(0, 0, 0), drawRect);
            int outSize = 40;
            int oX = width / 2, oY = height / 2, r = (width - outSize * 2) / 2;
            long innerBorderColor = COLOR.ARGB(100, 100, 100);
            int[] angles = new int[] { 240, 180, 120, 60, 0, -60 };
            int[] rs = new int[] { r, r * 2 / 3, r / 3};
            for (int i = 0; i < 3; i++)
            {
                POINT[] drawPoints = new POINT[6];
                for (int j = 0; j < 6; j++)
                {
                    drawPoints[j] = GetCyclePoint(oX, oY, rs[i], angles[j]);
                }
                paint.DrawPolygon(innerBorderColor, 1, 0, drawPoints);
            }
            FONT pFont = new FONT("微软雅黑", 16, false, false, false);
            POINT[] points = GetTitlePoints();
            int pointsSize = points.Length;
            String[] strs = new String[] { "业务", "情商", "学识", "智商", "管理", "技术" };
            int[] values = new int[6];
            if (m_dimension != null)
            {
                values = new int[] { m_dimension.m_business, m_dimension.m_EQ, m_dimension.m_knowledge,
                m_dimension.m_IQ, m_dimension.m_lead, m_dimension.m_technology};
            }
            POINT []scopePoints = GetScopePoints();
            for (int i = 0; i < pointsSize; i++)
            {
                int bSize = 5;
                POINT bPoint = points[i];
                RECT bRect = new RECT(bPoint.x - bSize, bPoint.y - bSize, bPoint.x + bSize, bPoint.y + bSize);
                SIZE pSize = paint.TextSize(strs[i], pFont);
                if (i == 0 || i == 5)
                {
                    bPoint.y -= 20;
                }
                CDraw.DrawText(paint, strs[i], m_sysColors[i], pFont, bPoint.x - pSize.cx / 2, bPoint.y - pSize.cy / 2);
                int iSize = 5;
                POINT cp = GetCyclePoint(oX, oY, r, angles[i]);
                paint.DrawLine(innerBorderColor, 1, 0, oX, oY, cp.x, cp.y);
                paint.FillEllipse(m_sysColors[i], new RECT(cp.x - iSize, cp.y - iSize, cp.x + iSize, cp.y + iSize));
                String text = String.Format("({0})", CStr.ConvertIntToStr(values[i]));
                SIZE sSize = paint.TextSize(text, pFont);
                CDraw.DrawText(paint, text, m_sysColors[i], pFont, bPoint.x - sSize.cx / 2, bPoint.y + sSize.cy / 2);
            }
            if (m_dimension != null)
            {
                paint.FillGradientPolygon(COLOR.ARGB(200, 50, 105, 217), COLOR.ARGB(200, 50, 105, 217), scopePoints, 90);
                paint.DrawPolygon(COLOR.ARGB(100, 255, 255, 255), 1, 0, scopePoints);
            }
        }

        /// <summary>
        /// 绘制边线
        /// </summary>
        /// <param name="paint">绘图对象</param>
        /// <param name="clipRect">裁剪区域</param>
        public override void OnPaintBorder(CPaint paint, RECT clipRect)
        {
        }
    }
}
