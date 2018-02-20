/**************************************************************************************\
*                                                                                      *
* GridProgressCell.cs -  Grid progress cell functions, types, and definitions.       *
*                                                                                      *
*               Version 1.00 ★                                                        *
*                                                                                      *
*               Copyright (c) 2016-2016, iTeam. All rights reserved.               *
*               Created by Todd.                                                 *
*                                                                                      *
***************************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace OwLib
{
    /// <summary>
    /// 进度条单元格
    /// </summary>
    public class GridProgressCell : GridStringCell
    {
        private double m_rate;

        /// <summary>
        /// 获取或设置进度比例
        /// </summary>
        public double Rate
        {
            get { return m_rate; }
            set { m_rate = value; }
        }

        /// <summary>
        /// 重绘单元格
        /// </summary>
        /// <param name="paint">绘图对象</param>
        /// <param name="rect">区域</param>
        /// <param name="clipRect">裁剪区域</param>
        /// <param name="isAlternate">是否交替</param>
        public override void OnPaint(CPaint paint, RECT rect, RECT clipRect, bool isAlternate)
        {
            base.OnPaint(paint, rect, clipRect, isAlternate);
            int width = rect.right - rect.left;
            if (width > 0)
            {
                width = (int)(width * m_rate / 100);
                int newRight = rect.left + width;
                if (clipRect.right > newRight)
                {
                    clipRect.right = newRight;
                }
                long pColor = COLOR.ARGB(100, 255, 0, 0);
                if (m_rate > 80)
                {
                    pColor = COLOR.ARGB(100, 0, 255, 0);
                }
                else if (m_rate > 60)
                {
                    pColor = COLOR.ARGB(100, 255, 255, 0);
                }
                else if (m_rate > 40)
                {
                    pColor = COLOR.ARGB(100, 0, 255, 255);
                }
                else if (m_rate > 20)
                {
                    pColor = COLOR.ARGB(100, 255, 0, 255);
                }
                paint.FillRect(pColor, clipRect);
            }
        }
    }
}
