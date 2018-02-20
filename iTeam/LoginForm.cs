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
    /// 登陆窗体
    /// </summary>
    public partial class LoginForm : Form
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        public LoginForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            AESHelper.m_key = txtPassword.Text + AESHelper.m_key;
        }
    }
}