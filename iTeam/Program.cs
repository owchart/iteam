using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;
using OwLib;
using System.Runtime.InteropServices;
using System.Text;
using iTeam;
using OpenPop.Pop3;

namespace OwLib
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            if (args == null || args.Length == 0)
            {
                MainForm mainForm = new MainForm();
                mainForm.LoadXml("MainFrame");
                Application.Run(mainForm);
            }
            else if (args[0].Trim() == "-plan")
            {
                CDraw.m_drawType = 1;
                MainForm mainForm = new MainForm();
                mainForm.LoadXml("PlanWindow");
                Application.Run(mainForm);
            }
            else if (args[0].Trim() == "-email")
            {
                MainForm mainForm = new MainForm();
                mainForm.LoadXml("EmailWindow");
                Application.Run(mainForm);
            }
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message + "\r\n" + e.Exception.StackTrace);
            Console.WriteLine("1");
        }
    }
}