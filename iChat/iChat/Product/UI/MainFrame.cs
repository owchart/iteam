/*****************************************************************************\
*                                                                             *
* MainFrame.cs -  MainFrame functions, types, and definitions.                *
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
using OwLib;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace OwLib
{
    /// <summary>
    /// ����ϵͳ
    /// </summary>
    public class MainFrame : UIXmlEx, IDisposable
    {
        /// <summary>
        /// ��������ϵͳ
        /// </summary>
        public MainFrame()
        {
        }

        /// <summary>
        /// ��ͼ��
        /// </summary>
        private DivA m_mainDiv;

        private BarrageDiv m_barrageDiv;

        /// <summary>
        /// ��ȡ�����õ�Ļ�ؼ�
        /// </summary>
        public BarrageDiv BarrageDiv
        {
            get { return m_barrageDiv; }
            set { m_barrageDiv = value; }
        }

        /// <summary>
        /// �������ݻص�
        /// </summary>
        /// <param name="message">��Ϣ</param>
        public void ChatMessageCallBack(CMessage message)
        {
            m_mainDiv.BeginInvoke(message);
        }

        /// <summary>
        /// ����¼�
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="mp">����</param>
        /// <param name="button">��ť</param>
        /// <param name="clicks">�������</param>
        /// <param name="delta">����ֵ/param>
        private void ClickEvent(object sender, POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            if (button == MouseButtonsA.Left && clicks == 1)
            {
                ControlA control = sender as ControlA;
                String name = control.Name;
                if (name == "btnSend")
                {
                    ChatData chatData = new ChatData();
                    chatData.m_text = GetTextBox("txtSend").Text;
                    DataCenter.ClientChatService.Send(DataCenter.ClientChatService.RequestID, chatData);
                }
            }
        }

        /// <summary>
        /// ������Դ����
        /// </summary>
        public override void Dispose()
        {
        }

        /// <summary>
        /// �˳�����
        /// </summary>
        public override void Exit()
        {
           
        }

        /// <summary>
        /// �������̷߳�����
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="args">����</param>
        public void Invoke(object sender, object args)
        {
            CMessage message = args as CMessage;
            if (message.m_serviceID == ChatService.SERVICEID_CHAT)
            {
                List<ChatData> datas = new List<ChatData>();
                ChatService.GetChatDatas(datas, message.m_body, message.m_bodyLength);
                int datasSize = datas.Count;
                for (int i = 0; i < datasSize; i++)
                {
                    ChatData data = datas[i];
                    CIndicator indicator = CFunctionEx.CreateIndicator("", data.m_text, this);
                    indicator.Clear();
                    indicator.Dispose();
                }
            }
        }

        /// <summary>
        /// �Ƿ��д�����ʾ
        /// </summary>
        /// <returns>�Ƿ���ʾ</returns>
        public bool IsWindowShowing()
        {
            List<ControlA> controls = Native.GetControls();
            int controlsSize = controls.Count;
            for (int i = 0; i < controlsSize; i++)
            {
                WindowFrameA frame = controls[i] as WindowFrameA;
                if (frame != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ����XML
        /// </summary>
        /// <param name="xmlPath">XML·��</param>
        public override void Load(String xmlPath)
        {
            LoadFile(xmlPath, null);
            DataCenter.MainUI = this;
            m_mainDiv = Native.GetControls()[0] as DivA;
            m_mainDiv.BackColor = COLOR.CONTROL;
            ControlPaintEvent paintLayoutEvent = new ControlPaintEvent(PaintLayoutDiv);
            m_mainDiv.RegisterEvent(paintLayoutEvent, EVENTID.PAINT);
            m_mainDiv.RegisterEvent(new ControlInvokeEvent(Invoke), EVENTID.INVOKE);
            DataCenter.ClientChatService.RegisterListener(DataCenter.ClientChatService.RequestID, new ListenerMessageCallBack(ChatMessageCallBack));
            m_barrageDiv = new BarrageDiv();
            m_barrageDiv.Dock = DockStyleA.Fill;
            m_barrageDiv.TopMost = true;
            Native.AddControl(m_barrageDiv);
            RegisterEvents(m_mainDiv);
        }

        /// <summary>
        /// �ػ�˵�����
        /// </summary>
        /// <param name="sender">���ö���</param>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clipRect">�ü�����</param>
        private void PaintLayoutDiv(object sender, CPaint paint, OwLib.RECT clipRect)
        {
            ControlA control = sender as ControlA;
            int width = control.Width, height = control.Height;
            OwLib.RECT drawRect = new OwLib.RECT(0, 0, width, height);
            paint.FillGradientRect(CDraw.PCOLORS_BACKCOLOR, CDraw.PCOLORS_BACKCOLOR2, drawRect, 0, 90);
        }

        /// ע���¼�
        /// </summary>
        /// <param name="control">�ؼ�</param>
        private void RegisterEvents(ControlA control)
        {
            ControlMouseEvent clickButtonEvent = new ControlMouseEvent(ClickEvent);
            List<ControlA> controls = control.GetControls();
            int controlsSize = controls.Count;
            for (int i = 0; i < controlsSize; i++)
            {
                ControlA subControl = controls[i];
                ButtonA button = subControl as ButtonA;
                if (button != null)
                {
                    button.RegisterEvent(clickButtonEvent, EVENTID.CLICK);
                }
                RegisterEvents(controls[i]);
            }
        }

        /// <summary>
        /// ��ʾ��ʾ����
        /// </summary>
        /// <param name="text">�ı�</param>
        /// <param name="caption">����</param>
        /// <param name="uType">��ʽ</param>
        /// <returns>���</returns>
        public int ShowMessageBox(String text, String caption, int uType)
        {
            MessageBox.Show(text, caption);
            return 1;
        }
    }
}
