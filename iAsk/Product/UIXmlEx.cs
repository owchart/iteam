/*****************************************************************************\
*                                                                             *
* ChartUIXml.cs - Chart xml functions, types, and definitions.                *
*                                                                             *
*               Version 1.00  ����                                          *
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
    /// ��Ʊͼ�οؼ�Xml����
    /// </summary>
    public class UIXmlEx:UIXml
    {
        #region Lord 2016/12/24
        private double m_scaleFactor = 1;

        /// <summary>
        /// ��ȡ��������������
        /// </summary>
        public double ScaleFactor
        {
            get { return m_scaleFactor; }
            set { m_scaleFactor = value; }
        }

        /// <summary>
        /// �����ؼ�
        /// </summary>
        /// <param name="node">�ڵ�</param>
        /// <param name="type">����</param>
        /// <returns>�ؼ�</returns>
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
        /// �˳�����
        /// </summary>
        public virtual void Exit()
        {
        }

        /// <summary>
        /// ����XML
        /// </summary>
        /// <param name="xmlPath">XML��ַ</param>
        public virtual void Load(String xmlPath)
        {
        }

        /// <summary>
        /// ��������
        /// </summary>
        public virtual void LoadData()
        {
        }

        /// <summary>
        /// �������ųߴ�
        /// </summary>
        /// <param name="clientSize">�ͻ��˴�С</param>
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
        /// �Ƿ�ȷ�Ϲر�
        /// </summary>
        /// <returns>������</returns>
        public virtual int Submit()
        {
            return 0;
        }
        #endregion
    }
}
