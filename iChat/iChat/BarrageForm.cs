using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using OwLib;

namespace OwLib
{
    /// <summary>
    /// 窗体
    /// </summary>
    public partial class BarrageForm : Form
    {
        #region Lord 2012/7/4
        /// <summary>
        ///  创建图形控件
        /// </summary>
        public BarrageForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 控件管理器
        /// </summary>
        private WinHost m_host;

        /// <summary>
        /// 控件库
        /// </summary>
        private INativeBase m_native;

        private BarrageDiv m_barrageDiv;

        /// <summary>
        /// 获取或设置弹幕控件
        /// </summary>
        public BarrageDiv BarrageDiv
        {
            get { return m_barrageDiv; }
            set { m_barrageDiv = value; }
        }

        /// <summary>
        /// 获取客户端尺寸
        /// </summary>
        /// <returns>客户端尺寸</returns>
        public SIZE GetClientSize()
        {
            return new SIZE(ClientSize.Width, ClientSize.Height);
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="name">名称</param>
        public void LoadXml()
        {
            m_native = NativeHandler.CreateNative();
            m_native.Paint = new GdiPlusPaintEx();
            m_host = new WinHostEx();
            m_host.Native = m_native;
            m_native.Host = m_host;
            m_host.HWnd = Handle;
            m_native.DisplaySize = new SIZE(ClientSize.Width, ClientSize.Height);
            m_barrageDiv = new BarrageDiv();
            m_barrageDiv.Dock = DockStyleA.Fill;
            m_barrageDiv.TopMost = true;
            m_native.AddControl(m_barrageDiv);
            m_native.Update();
            Invalidate();
        }

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (m_barrageDiv != null)
            {
                m_barrageDiv.ClearBarrages();
            }
        }

        /// <summary>
        /// 消息监听
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {            
            if (m_host != null)
            {
                if (m_host.OnMessage(ref m) > 0)
                {
                    return;
                }
            }
             base.WndProc(ref m);
        }
        #endregion
    }
}
