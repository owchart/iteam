/*******************************************************************************\
*                                                                               *
* RunButton.cs - Task button functions, types, and definitions.                *
*                                                                               *
*               Version 6.00 ��                                                 *
*                                                                               *
*               Copyright (c) 2016-2016, Piratecat. All rights reserved.        *
*               Created by Lord 2016/12/2.                                      *
*                                                                               *
********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace OwLib
{
    /// <summary>
    /// ����ť
    /// </summary>
    public class RuningButton : ButtonA
    {
        #region Lord 2016/4/21
        /// <summary>
        /// ����͸����ť
        /// </summary>
        public RuningButton()
        {
            BorderColor = COLOR.EMPTY;
            Font = new FONT("΢���ź�", 30, true, false, false);
        }

        private MainFrame m_mainFrame;

        /// <summary>
        /// ��ȡ�����������
        /// </summary>
        public MainFrame MainFrame
        {
            get { return m_mainFrame; }
            set { m_mainFrame = value; }
        }

        /// <summary>
        /// �ػ汳������
        /// </summary>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clipRect">�ü�����</param>
        public override void OnPaintBackground(CPaint paint, RECT clipRect)
        {
            int width = Width - 10, height = Height - 10;
            RECT drawRect = new RECT(0, 0, width, height);
            RECT shadowRect = new RECT(5, 5, width + 2, height + 5);
            paint.FillEllipse(COLOR.ARGB(100, 0, 0, 0), shadowRect);
            if (m_mainFrame != null && m_mainFrame.Mode == 5)
            {
                paint.FillGradientEllipse(COLOR.ARGB(255, 40, 24), COLOR.ARGB(255, 120, 40), drawRect, 45);
            }
            else
            {
                paint.FillGradientEllipse(COLOR.ARGB(90, 120, 24), COLOR.ARGB(122, 156, 40), drawRect, 45);
            }
        }

        /// <summary>
        /// �ػ�ǰ������
        /// </summary>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clipRect">�ü�����</param>
        public override void OnPaintForeground(CPaint paint, RECT clipRect)
        {
            int width = Width - 10, height = Height - 10;
            if (width > 0 && height > 0)
            {
                String text = Text;
                if (text != null && text.Length > 0)
                {
                    SIZE tSize = paint.TextSize(text, Font);
                    RECT tRect = new RECT();
                    tRect.left = (width - tSize.cx) / 2;
                    tRect.top = (height - tSize.cy) / 2;
                    tRect.right = tRect.left + tSize.cx;
                    tRect.bottom = tRect.top + tSize.cy;
                    paint.DrawText(text, COLOR.ARGB(255, 255, 255), Font, tRect);
                }
            }
        }
        #endregion
    }
}
