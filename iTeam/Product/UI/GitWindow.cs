/*****************************************************************************\
*                                                                             *
* GitWindow.cs - Git window functions, types                      *
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
    /// Git窗体
    /// </summary>
    public class GitWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public GitWindow(INativeBase native)
        {
            Load(native, "GitWindow", "gitWindow");
            RegisterEvents(m_window);
            m_gridGit = GetGrid("gridGits");
            m_gridGit.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindGits();
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridGit;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            GitInfo git = new GitInfo();
            git.m_ID = DataCenter.GitService.GetNewID();
            DataCenter.GitService.Save(git);
            AddGitToGrid(git);
            m_gridGit.Update();
            if (m_gridGit.VScrollBar != null)
            {
                m_gridGit.VScrollBar.ScrollToEnd();
            }
            m_gridGit.Invalidate();
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="git">信息</param>
        public void AddGitToGrid(GitInfo git)
        {
            List<GridRow> rows = m_gridGit.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == git.m_ID)
                {
                    findRow.GetCell("colP2").SetString(git.m_name);
                    findRow.GetCell("colP3").SetString(git.m_url);
                    findRow.GetCell("colP4").SetString(git.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridGit.AddRow(row);
            row.AddCell("colP1", new GridStringCell(git.m_ID));
            row.AddCell("colP2", new GridStringCell(git.m_name));
            row.AddCell("colP3", new GridStringCell(git.m_url));
            row.AddCell("colP4", new GridStringCell(git.m_createDate));
            List<GridCell> cells = row.GetCells();
            int cellsSize = cells.Count;
            for (int j = 1; j < cellsSize; j++)
            {
                cells[j].AllowEdit = true;
            }
        }

        /// <summary>
        /// 绑定员工
        /// </summary>
        private void BindGits()
        {
            m_gridGit.CellEditMode = GridCellEditMode.DoubleClick;
            List<GitInfo> gits = DataCenter.GitService.m_gits;
            int gitsSize = gits.Count;
            m_gridGit.ClearRows();
            m_gridGit.BeginUpdate();
            for (int i = 0; i < gitsSize; i++)
            {
                GitInfo git = gits[i];
                AddGitToGrid(git);
            }
            m_gridGit.EndUpdate();
            m_gridGit.Invalidate();
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
                if (name == "btnAdd")
                {
                    Add();
                }
                else if (name == "btnDelete")
                {
                    Delete();
                }
                else if (name == "btnExportExcel")
                {
                    ExportToExcel("Git.xls", m_gridGit);
                }
                else if (name == "btnExportTxt")
                {
                    ExportToTxt("Git.txt", m_gridGit);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridGit.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.GitService.Delete(pID);
                    m_gridGit.RemoveRow(deleteRow);
                    m_gridGit.Update();
                    m_gridGit.Invalidate();
                }
            }
        }

        /// <summary>
        /// 单元格编辑结束事件
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="cell">单元格</param>
        private void GridCellEditEnd(object sender, GridCell cell)
        {
            if (cell != null)
            {
                GitInfo git = DataCenter.GitService.GerGit(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    git.m_name = cellValue;
                }
                else if (colName == "colP3")
                {
                    git.m_url = cellValue;
                }
                else if (colName == "colP4")
                {
                    git.m_createDate = cellValue;
                }
                DataCenter.GitService.Save(git);
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
