using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OwLib;
using Newtonsoft.Json;

namespace iEmail
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            String file = DataCenter.GetAppPath() + "\\logininfo.txt";
            if (CFileA.IsFileExist(file))
            {
                String content = "";
                CFileA.Read(file, ref content);
                m_emailInfo = JsonConvert.DeserializeObject<EmailInfo>(content);
                txtEmailServer.Text = m_emailInfo.m_server;
                txtUserName.Text = m_emailInfo.m_userName;
                txtPwd.Text = m_emailInfo.m_pwd;
            }
        }

        private EmailInfo m_emailInfo;

        /// <summary>
        /// 获取或设置邮箱信息
        /// </summary>
        public EmailInfo EmailInfo
        {
            get { return m_emailInfo; }
            set { m_emailInfo = value; }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            String file = DataCenter.GetAppPath() + "\\logininfo.txt";
            if (m_emailInfo == null)
            {
                m_emailInfo = new EmailInfo();
            }
            m_emailInfo.m_server = txtEmailServer.Text;
            m_emailInfo.m_userName = txtUserName.Text;
            m_emailInfo.m_pwd = txtPwd.Text;
            CFileA.Write(file, JsonConvert.SerializeObject(m_emailInfo));
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}