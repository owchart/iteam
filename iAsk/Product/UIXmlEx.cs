/*****************************************************************************\
*                                                                             *
* ChartUIXml.cs - Chart xml functions, types, and definitions.                *
*                                                                             *
*               Version 1.00  ★★★                                          *
*                                                                             *
*               Copyright (c) 2016-2016, iTeam. All rights reserved.      *
*               Created by Lord 2016/12/24.                                   *
*                                                                             *
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Data;

namespace OwLib
{
    /// <summary>
    /// 股票图形控件Xml解析
    /// </summary>
    public class UIXmlEx:UIXml
    {
        #region Lord 2016/12/24
        private double m_scaleFactor = 1;

        /// <summary>
        /// 获取或设置缩放因子
        /// </summary>
        public double ScaleFactor
        {
            get { return m_scaleFactor; }
            set { m_scaleFactor = value; }
        }

        /// <summary>
        /// 创建控件
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="type">类型</param>
        /// <returns>控件</returns>
        public override ControlA CreateControl(XmlNode node, String type)
        {
            INativeBase native = Native;
            if (type == "barragediv")
            {
                return new BarrageDiv();
            }
            else if (type == "itextbox")
            {
                return new iTextBox();
            }
            else if (type == "runningbutton")
            {
                return new RuningButton();
            }
            return base.CreateControl(node, type);;
        }

        /// <summary>
        /// 退出方法
        /// </summary>
        public virtual void Exit()
        {
        }

        /// <summary>
        /// 加载XML
        /// </summary>
        /// <param name="xmlPath">XML地址</param>
        public virtual void Load(String xmlPath)
        {
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public virtual void LoadData()
        {
        }

        /// <summary>
        /// 重置缩放尺寸
        /// </summary>
        /// <param name="clientSize">客户端大小</param>
        public void ResetScaleSize(SIZE clientSize)
        {
            INativeBase native = Native;
            if (native != null)
            {
                ControlHost host = native.Host;
                SIZE nativeSize = native.DisplaySize;
                native.ScaleSize = new SIZE((int)(clientSize.cx * m_scaleFactor), (int)(clientSize.cy * m_scaleFactor));
                native.Update();
            }
        }

        /// <summary>
        /// 是否确认关闭
        /// </summary>
        /// <returns>不处理</returns>
        public virtual int Submit()
        {
            return 0;
        }
        #endregion
    }
}
