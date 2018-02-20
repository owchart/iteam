using System;
using System.Collections.Generic;
using System.Text;

namespace OwLib
{
    /// <summary>
    /// ������Ԫ��
    /// </summary>
    public class GridAwardCell : GridStringCell
    {
        private String m_award;

        /// <summary>
        /// ��ȡ�����ûر�
        /// </summary>
        public String Award
        {
            get { return m_award; }
            set { m_award = value; }
        }

        private String m_content;

        /// <summary>
        /// ��ȡ����������
        /// </summary>
        public String Content
        {
            get { return m_content; }
            set { m_content = value; }
        }

        private int m_rate;

        /// <summary>
        /// ��ȡ�������Ǽ�
        /// </summary>
        public int Rate
        {
            get { return m_rate; }
            set { m_rate = value; }
        }

        /// <summary>
        /// �ػ浥Ԫ�񷽷�
        /// </summary>
        /// <param name="paint">��ͼ����</param>
        /// <param name="rect">����</param>
        /// <param name="clipRect">�ü�����</param>
        /// <param name="isAlternate">�Ƿ�����</param>
        public override void OnPaint(CPaint paint, RECT rect, RECT clipRect, bool isAlternate)
        {
            String text = Text;
            FONT titleFont = new FONT("΢���ź�", 18, true, false, false);
            SIZE tSize = paint.TextSize(text, titleFont);
            paint.DrawText(text, COLOR.ARGB(238, 199, 16), titleFont, new RECT(rect.left, rect.top + 5, 0, 0));
            String strStar = "";
            for (int i = 0; i < m_rate; i++)
            {
                strStar += "��";
            }
            paint.DrawText(strStar, COLOR.ARGB(255, 255, 80), titleFont, new RECT(rect.left + tSize.cx, rect.top + 5, 0, 0));
            FONT contentFont = new FONT("΢���ź�", 14, false, false, false);
            paint.DrawText("---" + m_content + "---", COLOR.ARGB(255, 255, 255), contentFont, new RECT(rect.left, rect.top + tSize.cy + 10, 0, 0));
            FONT awardFont = new FONT("΢���ź�", 16, false, false, false);
            paint.DrawText(m_award, COLOR.ARGB(80, 255, 255), awardFont, new RECT(rect.left, rect.top + tSize.cy + 30, 0, 0));
            paint.DrawLine(COLOR.ARGB(100, 100, 100), 1, 2, rect.left, rect.bottom, rect.right, rect.bottom);
        }
    }
}
