using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OwLib;

namespace iChat
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            DataCenter.StartService();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CDraw.m_drawType = 1;
            MainForm mainForm = new MainForm();
            mainForm.LoadXml("MainFrame");
            Application.Run(mainForm);
        }
    }
}