/*****************************************************************************\
*                                                                             *
* FoldSubMenuA.cs -  foldSubMenuA functions, types, and definitions           *
*                                                                             *
*               Version 1.00 ★                                               *
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
    /// 菜单项
    /// </summary>
    public class FoldSubMenuA : DivA
    {
        /// <summary>
        /// 创建菜单项
        /// </summary>
        public FoldSubMenuA()
        {
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        private ControlMouseEvent m_clickButtonEvent;

        /// <summary>
        /// 布局控件
        /// </summary>
        private LayoutDivA m_layoutDiv;

        /// <summary>
        /// 获取或设置排序按钮
        /// </summary>
        public LayoutDivA LayoutDiv
        {
            get { return m_layoutDiv; }
            set { m_layoutDiv = value; }
        }

        private ButtonA m_shrinkBtn;

        /// <summary>
        /// 获取或设置收缩按钮
        /// </summary>
        public ButtonA ShrinkButton
        {
            get { return m_shrinkBtn; }
            set { m_shrinkBtn = value; }
        }

        private LabelA m_titleLabel;

        /// <summary>
        /// 获取或设置菜单项标签
        /// </summary>
        public LabelA TitleLabel
        {
            get { return m_titleLabel; }
            set { m_titleLabel = value; }
        }

        /// <summary>
        /// 点击按钮方法
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="mp">坐标</param>
        /// <param name="button">按钮</param>
        /// <param name="click">点击次数</param>
        /// <param name="delta">滚轮滚动值</param>
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
        /// 销毁资源
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
        /// 控件添加方法
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
                m_shrinkBtn.Font = new FONT("微软雅黑", 12, false, false, false);
                m_clickButtonEvent = new ControlMouseEvent(ClickButton);
                m_shrinkBtn.RegisterEvent(m_clickButtonEvent, EVENTID.CLICK);
                AddControl(m_shrinkBtn);
            }
            if (m_titleLabel == null)
            {
                m_titleLabel = new LabelA();
                m_titleLabel.Font = new FONT("微软雅黑", 16, true, false, false);
                AddControl(m_titleLabel);
            }
        }

        /// <summary>
        /// 重绘边线方法
        /// </summary>
        /// <param name="paint">绘图对象</param>
        /// <param name="clipRect">裁剪区域</param>
        public override void OnPaintBorder(CPaint paint, RECT clipRect)
        {
            int width = Width, height = Height;
            RECT drawRect = new RECT(0, 0, width, height);
            paint.DrawRoundRect(COLOR.CONTROLBORDER, 1, 0, drawRect, 6);
        }
    }
}
