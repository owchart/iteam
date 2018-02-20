/*****************************************************************************\
*                                                                             *
* FoldMenuItemA.cs -  foldMenuItemA functions, types, and definitions         *
*                                                                             *
*               Version 1.00 ��                                               *
*                                                                             *
*               Copyright (c) 2016-2016, Lord's layout. All rights reserved.  *
*               create right 2017/5/23 by wangshaoxu.                         *
*                                                                             *
*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OwLib
{
    /// <summary>
    /// ����˵���ť
    /// </summary>
    public class FoldMenuItemA : ButtonA
    {
        /// <summary>
        /// �����˵���ť
        /// </summary>
        public FoldMenuItemA()
        {
            AllowDrag = true;
            Font = new FONT("΢���ź�", 14, false, false, false);
            Size = new SIZE(140, 140);
        }

        /// <summary>
        /// ����
        /// </summary>
        private int m_direction;

        /// <summary>
        /// ��ͼ����
        /// </summary>
        private RECT m_paintRect = new RECT();

        /// <summary>
        /// �����
        /// </summary>
        private List<POINT> m_points = new List<POINT>(); 

        /// <summary>
        /// �����
        /// </summary>
        private Random m_rd = new Random();

        /// <summary>
        /// ���ֵ
        /// </summary>
        private int m_tick;

        /// <summary>
        /// ���ID
        /// </summary>
        private int m_timerID = ControlA.GetNewTimerID();

        private int m_orderNum;

        /// <summary>
        /// ��ȡ������������
        /// </summary>
        public int OrderNum
        {
            get { return m_orderNum; }
            set { m_orderNum = value; }
        }

        /// <summary>
        /// ��ȡҪ���Ƶı���ɫ
        /// </summary>
        /// <returns></returns>
        protected override long GetPaintingBackColor()
        {
            if (Native.PushedControl == this)
            {
                return CDraw.PCOLORS_BACKCOLOR4;
            }
            else if (Native.HoveredControl == this)
            {
                return CDraw.PCOLORS_BACKCOLOR3;
            }
            else
            {
                return BackColor;
            }
        }

        /// <summary>
        /// ������¼�
        /// </summary>
        /// <param name="mp">����</param>
        /// <param name="button">��ť</param>
        /// <param name="clicks">����¼�</param>
        /// <param name="delta">���ֹ���ֵ</param>
        public override void OnClick(POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            StopTimer(m_timerID);
            m_direction = 0;
            m_tick = 0;
            base.OnClick(mp, button, clicks, delta);
        }

        /// <summary>
        /// �϶���ʼ����
        /// </summary>
        /// <returns>�Ƿ������϶�</returns>
        public override bool OnDragBegin()
        {
            m_direction = 0;
            m_tick = 0;
            m_points.Clear();
            StopTimer(m_timerID);
            m_paintRect = Bounds;
            return base.OnDragBegin();
        }

        /// <summary>
        /// �϶���������
        /// </summary>
        public override void OnDragEnd()
        {
            base.OnDragEnd();
            ControlA parent = Parent;
            parent.Update();
            parent.Invalidate();
        }

        /// <summary>
        /// �϶��з���
        /// </summary>
        public override void OnDragging()
        {
            base.OnDragging();
            ControlA parent = Parent;
            ControlHost host = Native.Host;
            RECT tempRect = new RECT();
            RECT bounds = Bounds;
            List<ControlA> controls = parent.m_controls;
            int controlsSize = controls.Count;
            int thisIndex = -1;
            for (int i = 0; i < controlsSize; i++)
            {
                ControlA iControl = controls[i];
                if (iControl == this)
                {
                    thisIndex = i;
                    break;
                }
            }
            int mx = bounds.left + (bounds.right - bounds.left) / 2;
            int my = bounds.top + (bounds.bottom - bounds.top) / 2;
            for (int i = 0; i < controlsSize; i++)
            {
                ControlA iCell = controls[i];
                if (iCell != this && !(iCell is ScrollBarA))
                {
                    RECT iBounds = iCell.Bounds;
                    if (host.GetIntersectRect(ref tempRect, ref bounds, ref iBounds) > 0)
                    {
                        if (mx >= iBounds.left && mx <= iBounds.right && my >= iBounds.top && my <= iBounds.bottom)
                        {
                            controls[thisIndex] = iCell;
                            controls[i] = this;
                            RECT oldBounds = iCell.Bounds;
                            iCell.Bounds = m_paintRect;
                            m_paintRect = oldBounds;
                            parent.Invalidate();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ��������¼�
        /// </summary>
        /// <param name="mp">����</param>
        /// <param name="button">��ť</param>
        /// <param name="clicks">����¼�</param>
        /// <param name="delta">���ֹ���ֵ</param>
        public override void OnMouseEnter(POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            base.OnMouseEnter(mp, button, clicks, delta);
            StartTimer(m_timerID, 20);
            m_direction = 1;
            int width = Width, height = Height;
            for (int i = 0; i < 10; i++)
            {
                POINT point = new POINT(m_rd.Next(0, width), m_rd.Next(0, height) - 20);
                m_points.Add(point);
            }
            m_tick = 0;
        }

        /// <summary>
        /// ����Ƴ��¼�
        /// </summary>
        /// <param name="mp">����</param>
        /// <param name="button">��ť</param>
        /// <param name="clicks">����¼�</param>
        /// <param name="delta">���ֹ���ֵ</param>
        public override void OnMouseLeave(POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            base.OnMouseLeave(mp, button, clicks, delta);
            StartTimer(m_timerID, 20);
            m_direction = 0;
            m_points.Clear();
        }

        /// <summary>
        /// ���Ʊ�������
        /// </summary>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clipRect">�ü�����</param>
        public override void OnPaintBackground(CPaint paint, RECT clipRect)
        {
            int width = Width;
            int height = Height;
            int border = m_tick;
            if (border > 10)
            {
                border = 10;
            }
            int cornerRadius = 10;
            RECT drawRect = new RECT(10 - border, 10 - border, width - (10 - border), height - (10 - border));
            paint.FillRoundRect(GetPaintingBackColor(), drawRect, cornerRadius);
            paint.FillGradientRect(COLOR.ARGB(m_tick * 10, 255, 255, 255),
                COLOR.ARGB(m_tick * 10, 220, 220, 220),
                drawRect, cornerRadius, 45);
        }

        /// <summary>
        /// ���Ʊ��߷���
        /// </summary>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clipRect">�ü�����</param>
        public override void OnPaintBorder(CPaint paint, RECT clipRect)
        {
            int width = Width, height = Height;
            int border = m_tick;
            if (border > 10)
            {
                border = 10;
            }
            RECT drawRect = new RECT(10 - border, 10 - border, width - (10 - border), height - (10 - border));
            String text = Text;
            int cornerRadius = 10;
            paint.DrawRoundRect(GetPaintingBorderColor(), IsDragging ? 2 : 1, IsDragging ? 1 : 0, drawRect, cornerRadius);
        }

        /// <summary>
        /// �ػ�ǰ������
        /// </summary>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clipRect">�ü�����</param>
        public override void OnPaintForeground(CPaint paint, RECT clipRect)
        {
            int width = Width;
            int height = Height;
            FONT font = new FONT("΢���ź�", 14, true, false, false);
            if (m_tick != 0)
            {
                font.m_fontSize += 2;
            }
            long foreColor = GetPaintingForeColor();
            String backImage = GetPaintingBackImage();
            int border = m_tick;
            if (border > 10)
            {
                border = 10;
            }
            int imgWidth = 88 - (10 - border);
            int imgHeight = 88 - (10 - border);
            int gap = 15;
            RECT imageRect = new RECT((width - imgWidth) / 2, (height - imgHeight) / 2 - gap, (width + imgWidth) / 2, (height + imgHeight) / 2 - gap);
            if (backImage != null && backImage.Length > 0)
            {
                if (m_tick != 0)
                {
                    paint.SetOpacity((float)(0.5 + border * 0.05f));
                }
                paint.DrawImage(backImage, imageRect);
                paint.SetOpacity(1);
            }
            if (m_text != null && m_name.Length > 0)
            {
                SIZE tSize = paint.TextSize(m_text, font);
                RECT tRect = new RECT();
                tRect.left = (width - tSize.cx) / 2;
                tRect.top = imageRect.bottom + gap;
                tRect.right = tRect.left + tSize.cx;
                tRect.bottom = tRect.top + tSize.cy;
                paint.DrawText(m_text, foreColor, font, tRect);
            }
            if (m_direction == 1)
            {
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
        /// �����
        /// </summary>
        /// <param name="timerID">���ID</param>
        public override void OnTimer(int timerID)
        {
            base.OnTimer(timerID);
            if(m_timerID == timerID)
            {
                if (m_direction == 1)
                {
                    m_tick++;
                    int pointsSize = m_points.Count;
                    for (int i = 0; i < pointsSize; i++)
                    {
                        POINT point = m_points[i];
                        point.x -= m_rd.Next(1, 10) - 5;
                        point.y -= m_rd.Next(1, 10);
                        m_points[i] = point;
                    }
                    if (m_tick >= 20)
                    {
                        m_points.Clear();
                        StopTimer(m_timerID);
                    }
                }
                else if (m_direction == 0)
                {
                    m_tick--;
                    if (m_tick <= 0)
                    {
                        m_points.Clear();
                        m_tick = 0;
                        StopTimer(m_timerID);
                    }
                }
                Invalidate();
            }
        }
    }

    /// <summary>
    /// �˵���Ƚ�
    /// </summary>
    public class FoldMenuItemLocationCompare : IComparer<FoldMenuItemA>
    {
        /// <summary>
        /// �Ƚ�
        /// </summary>
        /// <param name="x">����X</param>
        /// <param name="y">����Y</param>
        /// <returns>��ֵ</returns>
        public int Compare(FoldMenuItemA x, FoldMenuItemA y)
        {
            return x.Left.CompareTo(y.Left);
        }
    }

    public class FoldMenuItemOrderNumCompare : IComparer<ControlA>
    {
        /// <summary>
        /// �Ƚ�
        /// </summary>
        /// <param name="x">����X</param>
        /// <param name="y">����Y</param>
        /// <returns>��ֵ</returns>
        public int Compare(ControlA x, ControlA y)
        {
            FoldMenuItemA itemLeft = x as FoldMenuItemA;
            FoldMenuItemA itemRight = y as FoldMenuItemA;
            return itemLeft.OrderNum.CompareTo(itemRight.OrderNum);
        }
    }
}
