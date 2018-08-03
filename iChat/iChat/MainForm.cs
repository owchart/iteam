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
    /// ����
    /// </summary>
    public partial class MainForm : Form
    {
        #region Lord 2012/7/4
        /// <summary>
        ///  ����ͼ�οؼ�
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// �ؼ�������
        /// </summary>
        private WinHost m_host;

        /// <summary>
        /// �ؼ���
        /// </summary>
        private INativeBase m_native;

        /// <summary>
        /// ����
        /// </summary>
        private String m_title;

        /// <summary>
        /// XML
        /// </summary>
        private UIXmlEx m_xml;


        /// <summary>
        /// ��ȡ�ͻ��˳ߴ�
        /// </summary>
        /// <returns>�ͻ��˳ߴ�</returns>
        public SIZE GetClientSize()
        {
            return new SIZE(ClientSize.Width, ClientSize.Height);
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="name">����</param>
        public void LoadXml(String name)
        {
            if (name == "MainFrame")
            {
                m_xml = new MainFrame();
                (m_xml as MainFrame).MainForm = this;
            }
            m_xml.CreateNative();
            m_native = m_xml.Native;
            m_native.Paint = new GdiPlusPaintEx();
            m_host = new WinHostEx();
            m_host.Native = m_native;
            m_native.Host = m_host;
            m_host.HWnd = Handle;
            m_native.AllowScaleSize = true;
            m_native.DisplaySize = new SIZE(ClientSize.Width, ClientSize.Height);
            m_xml.ResetScaleSize(GetClientSize());
            m_xml.Script = new GaiaScript(m_xml);
            m_xml.Native.ResourcePath = DataCenter.GetAppPath() + "\\config";
            m_xml.Load(DataCenter.GetAppPath() + "\\config\\" + name+ ".html");
            m_host.ToolTip = new ToolTipA();
            m_host.ToolTip.Font = new FONT("΢���ź�", 20, true, false, false);
            (m_host.ToolTip as ToolTipA).InitialDelay = 250;
            m_native.Update();
            Invalidate();
        }

        /// <summary>
        /// ����ر��¼�
        /// </summary>
        /// <param name="e">�¼�����</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            m_xml.Exit();
            Environment.Exit(0);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// ��ʾ���巽��
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if ((m_xml as MainFrame).BarrageForm == null)
            {
                BarrageForm barrageForm = new BarrageForm();
                barrageForm.LoadXml();
                barrageForm.Show();
                (m_xml as MainFrame).BarrageForm = barrageForm;
            }
        }

        /// <summary>
        /// �ߴ�ı䷽��
        /// </summary>
        /// <param name="e">����</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (m_host != null)
            {
                m_xml.ResetScaleSize(GetClientSize());
                Invalidate();
            }
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="e">����</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (m_host != null)
            {
                if (m_host.IsKeyPress(0x11))
                {
                    double scaleFactor = m_xml.ScaleFactor;
                    if (e.Delta > 0)
                    {
                        if (scaleFactor > 0.2)
                        {
                            scaleFactor -= 0.1;
                        }
                    }
                    else if (e.Delta < 0)
                    {
                        if (scaleFactor < 10)
                        {
                            scaleFactor += 0.1;
                        }
                    }
                    m_xml.ScaleFactor = scaleFactor;
                    m_xml.ResetScaleSize(GetClientSize());
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// ��갴���¼�
        /// </summary>
        /// <param name="e">����</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            MainFrame mainFrame = m_xml as MainFrame;
            if (mainFrame.BarrageForm.BarrageDiv != null)
            {
                mainFrame.BarrageForm.BarrageDiv.ClearBarrages();
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        public void Play()
        {
            int leftWidth = this.Left; //ָ���������ֵ
            int topWidth = this.Top; //ָ�������ϱ�ֵ 
            for (int i = 0; i < 20; i++)
            {
                if (i % 2 == 0)
                {
                    this.Left = this.Left + 10;
                }
                else //���� 
                {
                    this.Left = this.Left - 10;
                }
                if (i % 2 == 0)
                {
                    this.Top = this.Top + 10;
                }
                else//���� 
                {
                    this.Top = this.Top - 10;
                }
                System.Threading.Thread.Sleep(30);//��Ƶ�� 
            }
        }

        /// <summary>
        /// ���ñ���
        /// </summary>
        /// <param name="text">����</param>
        public void SetTitle(String title)
        {
            m_title = title;
        }

        /// <summary>
        /// ���߳����ñ���
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="e">����</param>
        public void SetTitle(object sender, EventArgs e)
        {
            Text = "����" + " ��������Ϊ" + m_title;
        }

        /// <summary>
        /// ��Ϣ����
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m_host != null)
                {
                    if (m_host.OnMessage(ref m) > 0)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("1");
            }
            base.WndProc(ref m);
        }
        #endregion
    }
}
