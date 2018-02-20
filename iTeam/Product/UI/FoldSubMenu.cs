/*****************************************************************************\
*                                                                             *
* FoldSubMenuA.cs -  foldSubMenuA functions, types, and definitions           *
*                                                                             *
*               Version 1.00 ��                                               *
*                                                                             *
*               Copyright (c) 2016-2016, Lord's layout. All rights reserved.  *
*               create right 2017/5/23 by Lord.                               *
*                                                                             *
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace OwLib
{
    /// <summary>
    /// �˵���
    /// </summary>
    public class FoldSubMenuA : DivA
    {
        /// <summary>
        /// �����˵���
        /// </summary>
        public FoldSubMenuA()
        {
        }

        /// <summary>
        /// ����¼�
        /// </summary>
        private ControlMouseEvent m_clickButtonEvent;

        /// <summary>
        /// ���ֿؼ�
        /// </summary>
        private LayoutDivA m_layoutDiv;

        /// <summary>
        /// ��ȡ����������ť
        /// </summary>
        public LayoutDivA LayoutDiv
        {
            get { return m_layoutDiv; }
            set { m_layoutDiv = value; }
        }

        private ButtonA m_shrinkBtn;

        /// <summary>
        /// ��ȡ������������ť
        /// </summary>
        public ButtonA ShrinkButton
        {
            get { return m_shrinkBtn; }
            set { m_shrinkBtn = value; }
        }

        private LabelA m_titleLabel;

        /// <summary>
        /// ��ȡ�����ò˵����ǩ
        /// </summary>
        public LabelA TitleLabel
        {
            get { return m_titleLabel; }
            set { m_titleLabel = value; }
        }

        /// <summary>
        /// �����ť����
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="mp">����</param>
        /// <param name="button">��ť</param>
        /// <param name="click">�������</param>
        /// <param name="delta">���ֹ���ֵ</param>
        private void ClickButton(object sender, POINT mp, MouseButtonsA button, int click, int delta)
        {
            if (button == MouseButtonsA.Left && click == 1)
            {
                ControlA control = sender as ControlA;
                String name = control.Name;
                if (name == "shrink")
                {
                    m_layoutDiv.Visible = !m_layoutDiv.Visible;
                    m_parent.Update();
                    m_parent.Invalidate();
                }
                else if (name == "sort")
                {

                }
            }
        }

        /// <summary>
        /// ������Դ
        /// </summary>
        public override void Dispose()
        {
            m_titleLabel = null;
            if (m_shrinkBtn != null)
            {
                m_shrinkBtn.UnRegisterEvent(m_clickButtonEvent, EVENTID.CLICK);
                m_shrinkBtn = null;
            }
            m_layoutDiv = null;
            base.Dispose();
        }

        /// <summary>
        /// �ؼ���ӷ���
        /// </summary>
        public override void OnAdd()
        {
            base.OnAdd();
            if (m_layoutDiv == null)
            {
                m_layoutDiv = new LayoutDivA();
                m_layoutDiv.BorderColor = COLOR.EMPTY;
                m_layoutDiv.LayoutStyle = LayoutStyleA.LeftToRight;
                m_layoutDiv.AutoWrap = true;
                AddControl(m_layoutDiv);
            }
            if (m_shrinkBtn == null)
            {
                m_shrinkBtn = new RibbonButton2();
                m_shrinkBtn.Font = new FONT("΢���ź�", 12, false, false, false);
                m_clickButtonEvent = new ControlMouseEvent(ClickButton);
                m_shrinkBtn.RegisterEvent(m_clickButtonEvent, EVENTID.CLICK);
                AddControl(m_shrinkBtn);
            }
            if (m_titleLabel == null)
            {
                m_titleLabel = new LabelA();
                m_titleLabel.Font = new FONT("΢���ź�", 16, true, false, false);
                AddControl(m_titleLabel);
            }
        }

        /// <summary>
        /// �ػ���߷���
        /// </summary>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clipRect">�ü�����</param>
        public override void OnPaintBorder(CPaint paint, RECT clipRect)
        {
            int width = Width, height = Height;
            RECT drawRect = new RECT(0, 0, width, height);
            paint.DrawRoundRect(COLOR.CONTROLBORDER, 1, 0, drawRect, 6);
        }
    }
}
