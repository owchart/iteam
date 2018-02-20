/*****************************************************************************\
*                                                                             *
* FoldMenu.cs -  foldMenu functions, types, and definitions                   *
*                                                                             *
*               Version 1.00 ★                                               *
*                                                                             *
*               Copyright (c) 2016-2016, Lord's layout. All rights reserved.  *
*               create right 2017/5/23 by wangshaoxu.                         *
*                                                                             *
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace OwLib
{
    /// <summary>
    /// 折叠菜单
    /// </summary>
    public class FoldMenuA : DivA
    {
        /// <summary>
        /// 创建折叠菜单
        /// </summary>
        public FoldMenuA()
        {
            ShowVScrollBar = true;
        }

        /// <summary>
        /// 所有子菜单项
        /// </summary>
        private List<FoldSubMenuA> m_subMenus = new List<FoldSubMenuA>();

        /// <summary>
        /// 获取或设置收缩按钮
        /// </summary>
        public List<FoldSubMenuA> SubMenus
        {
            get { return m_subMenus; }
            set { m_subMenus = value; }
        }

        /// <summary>
        /// 添加子菜单项
        /// </summary>
        /// <param name="subMenu">子菜单</param>
        public void AddSubMenu(FoldSubMenuA subMenu)
        {
            m_subMenus.Add(subMenu);
            AddControl(subMenu);
        }

        /// <summary>
        /// 销毁资源
        /// </summary>
        public override void Dispose()
        {
            m_subMenus.Clear();
            base.Dispose();
        }

        /// <summary>
        /// 插入子菜单项
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="item">菜单项</param>
        public void InsertItem(int index, FoldSubMenuA item)
        {
            m_subMenus.Insert(index, item);
        }

        /// <summary>
        /// 删除子菜单项
        /// </summary>
        /// <param name="item">菜单项</param>
        public void RemoveItem(FoldSubMenuA item)
        {
            if (m_subMenus.Contains(item))
            {
                m_subMenus.Remove(item);
            }
        }

        /// <summary>
        /// 绘图更新方法
        /// </summary>
        public override void Update()
        {
            int width = Width, height = Height;
            int menuSize = m_subMenus.Count;
            if (menuSize > 0)
            {
                PADDING padding = Padding;
                int left = padding.left, top = padding.top;
                int cwidth = width - padding.left - padding.right;
                for (int i = 0; i < menuSize; i++)
                {
                    FoldSubMenuA menu = m_subMenus[i];
                    PADDING menuMargin = menu.Margin;
                    int mwidth = cwidth - menuMargin.left - menuMargin.right;
                    top += menuMargin.top;
                    menu.Location = new POINT(padding.left + menuMargin.left, top);
                    PADDING menuPadding = menu.Padding;
                    int mLeft = menuPadding.left;
                    int mTop = menuPadding.top;
                    menu.TitleLabel.Location = new POINT(mLeft, mTop);
                    menu.ShrinkButton.Location = new POINT(mwidth - menuPadding.right - menu.ShrinkButton.Width, mTop);
                    int menuHeight = 0;
                    if (menu.LayoutDiv.Visible)
                    {
                        mTop += menu.TitleLabel.Height + menu.LayoutDiv.Margin.top;
                        menu.LayoutDiv.Top = mTop;
                        menu.LayoutDiv.Width = mwidth;
                        menu.LayoutDiv.Height = menu.LayoutDiv.GetContentHeight() + menuPadding.bottom;
                        menuHeight = menu.LayoutDiv.Bottom;
                    }
                    else
                    {
                        menuHeight = menu.TitleLabel.Height + menu.LayoutDiv.Margin.top;
                    }
                    menu.Size = new OwLib.SIZE(mwidth, menuHeight);
                    top += menuHeight + menuMargin.bottom;
                }
            }
            base.Update();
        }
    }
}
