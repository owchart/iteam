/*****************************************************************************\
*                                                                             *
* JidianWindow.cs - Jidian window functions, types                      *
*                                                                             *
*               Version 1.00  ★                                              *
*                                                                             *
*               Copyright (c) 2017-2017, iTeam. All rights reserved.      *
*               Created by Todd 2017/6/19.                          *
*                                                                             *
******************************************************************************/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace OwLib
{
    /// <summary>
    /// 代码统计窗体
    /// </summary>
    public class JidianWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public JidianWindow(INativeBase native)
        {
            Load(native, "JidianWindow", "jidianWindow");
            RegisterEvents(m_window);
            String codeDirCachePath = DataCenter.GetAppPath() + "\\CODEDIR.txt";
            if (CFileA.IsFileExist(codeDirCachePath))
            {
                String content = "";
                CFileA.Read(codeDirCachePath, ref content);
                GetTextBox("txtCodeDir").Text = content;
            }
            String dataDirCacheDir = DataCenter.GetAppPath() + "\\DATADIR.txt";
            if (CFileA.IsFileExist(dataDirCacheDir))
            {
                String content = "";
                CFileA.Read(dataDirCacheDir, ref content);
                GetTextBox("txtDataDir").Text = content;
            }
        }

        /// <summary>
        /// 查询按钮、重置按钮点击事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="mp">坐标</param>
        /// <param name="button">按钮</param>
        /// <param name="clicks">点击事件</param>
        /// <param name="delta">滚轮滚动值</param>
        private void ClickButton(object sender, POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            if (button == MouseButtonsA.Left && clicks == 1)
            {
                ControlA control = sender as ControlA;
                String name = control.Name;
                if (name == "btnSelectCodeDir")
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (DialogResult.OK == fbd.ShowDialog())
                    {
                        GetTextBox("txtCodeDir").Text = fbd.SelectedPath;
                        String codeDirCacheDir = DataCenter.GetAppPath() + "\\CODEDIR.txt";
                        CFileA.Write(codeDirCacheDir, fbd.SelectedPath);
                        Native.Invalidate();
                    }
                    fbd.Dispose();
                }
                else if (name == "btnSelectDataDir")
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (DialogResult.OK == fbd.ShowDialog())
                    {
                        GetTextBox("txtDataDir").Text = fbd.SelectedPath;
                        String dataDirCacheDir = DataCenter.GetAppPath() + "\\DATADIR.txt";
                        CFileA.Write(dataDirCacheDir, fbd.SelectedPath);
                        Native.Invalidate();
                    }
                    fbd.Dispose();
                }
                else if (name == "btnGenerate")
                {
                    //GAIA_FUTURE_CPP_S1:PengChen,100%;
                    String codeDir = GetTextBox("txtCodeDir").Text;
                    String dataDir = GetTextBox("txtDataDir").Text;
                    ProjectJidian pJidian = new ProjectJidian();
                    DataCenter.JidianService.Dir = codeDir;
                    DataCenter.JidianService.GetJidian(ref pJidian);
                    if (pJidian.Lines > 0)
                    {
                        CFileA.Write(dataDir + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".jidian", pJidian.ToString());
                        GetTextBox("txtResults").Text = pJidian.ToString();
                        Native.Invalidate();
                    }
                }
            }
        }

        /// 注册事件
        /// </summary>
        /// <param name="control">控件</param>
        private void RegisterEvents(ControlA control)
        {
            ControlMouseEvent clickButtonEvent = new ControlMouseEvent(ClickButton);
            List<ControlA> controls = control.GetControls();
            int controlsSize = controls.Count;
            for (int i = 0; i < controlsSize; i++)
            {
                ControlA subControl = controls[i];
                ButtonA button = controls[i] as ButtonA;
                LinkLabelA linkLabel = subControl as LinkLabelA;
                GridColumn column = subControl as GridColumn;
                GridA grid = subControl as GridA;
                CheckBoxA checkBox = subControl as CheckBoxA;
                if (column != null)
                {
                    column.AllowDrag = true;
                    column.AllowResize = true;
                    column.BackColor = CDraw.PCOLORS_BACKCOLOR;
                    column.Font = new FONT("微软雅黑", 12, false, false, false);
                    column.ForeColor = CDraw.PCOLORS_FORECOLOR;
                }
                else if (button != null)
                {
                    button.RegisterEvent(clickButtonEvent, EVENTID.CLICK);
                }
                else if (linkLabel != null)
                {
                    linkLabel.RegisterEvent(clickButtonEvent, EVENTID.CLICK);
                }
                else if (grid != null)
                {
                    grid.GridLineColor = COLOR.CONTROLBORDER;
                    grid.RowStyle.HoveredBackColor = CDraw.PCOLORS_HOVEREDROWCOLOR;
                    grid.RowStyle.SelectedBackColor = CDraw.PCOLORS_SELECTEDROWCOLOR;
                    grid.RowStyle.SelectedForeColor = CDraw.PCOLORS_FORECOLOR4;
                    grid.RowStyle.Font = new FONT("微软雅黑", 12, false, false, false);
                    GridRowStyle alternateRowStyle = new GridRowStyle();
                    alternateRowStyle.BackColor = CDraw.PCOLORS_ALTERNATEROWCOLOR;
                    alternateRowStyle.HoveredBackColor = CDraw.PCOLORS_HOVEREDROWCOLOR;
                    alternateRowStyle.SelectedBackColor = CDraw.PCOLORS_SELECTEDROWCOLOR;
                    alternateRowStyle.SelectedForeColor = CDraw.PCOLORS_FORECOLOR4;
                    alternateRowStyle.Font = new FONT("微软雅黑", 12, false, false, false);
                    grid.AlternateRowStyle = alternateRowStyle;
                    grid.UseAnimation = true;
                }
                RegisterEvents(controls[i]);
            }
        }
    }
}
