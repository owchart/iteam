using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OwLib;

namespace iTeam
{
    /// <summary>
    /// ��½����
    /// </summary>
    public partial class LoginForm : Form
    {
        /// <summary>
        /// ��������
        /// </summary>
        public LoginForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ����ر��¼�
        /// </summary>
        /// <param name="e">����</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            AESHelper.m_key = txtPassword.Text + AESHelper.m_key;
        }
    }
}