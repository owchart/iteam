using System;
using System.Collections.Generic;
using System.Text;

namespace OwLib
{
    /// <summary>
    /// 荣誉单元格
    /// </summary>
    public class GridAwardCell : GridStringCell
    {
        private String m_award;

        /// <summary>
        /// 获取或设置回报
        /// </summary>
        public String Award
        {
            get { return m_award; }
            set { m_award = value; }
        }

        private String m_content;

        /// <summary>
        /// 获取或设置内容
        /// </summary>
        public String Content
        {
            get { return m_content; }
            set { m_content = value; }
        }

        private int m_rate;

        /// <summary>
        /// 获取或设置星级
        /// </summary>
        public int Rate
        {
            get { return m_rate; }
            set { m_rate = value; }
        }

        /// <summary>
        /// 重绘单元格方法
        /// </summary>
        /// <param name="paint">绘图对象</param>
        /// <param name="rect">矩形</param>
        /// <param name="clipRect">裁剪区域</param>
        /// <param name="isAlternate">是否交替行</param>
        public override void OnPaint(CPaint paint, RECT rect, RECT clipRect, bool isAlternate)
        {
            String text = Text;
            FONT titleFont = new FONT("微软雅黑", 18, true, false, false);
            SIZE tSize = paint.TextSize(text, titleFont);
            paint.DrawText(text, COLOR.ARGB(238, 199, 16), titleFont, new RECT(rect.left, rect.top + 5, 0, 0));
            String strStar = "";
            for (int i = 0; i < m_rate; i++)
            {
                strStar += "★";
            }
            paint.DrawText(strStar, COLOR.ARGB(255, 255, 80), titleFont, new RECT(rect.left + tSize.cx, rect.top + 5, 0, 0));
            FONT contentFont = new FONT("微软雅黑", 14, false, false, false);
            paint.DrawText("---" + m_content + "---", COLOR.ARGB(255, 255, 255), contentFont, new RECT(rect.left, rect.top + tSize.cy + 10, 0, 0));
            FONT awardFont = new FONT("微软雅黑", 16, false, false, false);
            paint.DrawText(m_award, COLOR.ARGB(80, 255, 255), awardFont, new RECT(rect.left, rect.top + tSize.cy + 30, 0, 0));
            paint.DrawLine(COLOR.ARGB(100, 100, 100), 1, 2, rect.left, rect.bottom, rect.right, rect.bottom);
        }
    }
}
