/*****************************************************************************\
*                                                                             *
* StaffWindow.cs - Staff window functions, types                      *
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
    /// 项目操作窗体
    /// </summary>
    public class ProjectWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public ProjectWindow(INativeBase native)
        {
            Load(native, "ProjectWindow", "projectWindow");
            RegisterEvents(m_window);
            m_gridProjects = GetGrid("gridProjects");
            m_gridProjects.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            m_gridProjects.RegisterEvent(new GridCellMouseEvent(GridCellClick), EVENTID.GRIDCELLCLICK);
            BindProjects();
        }

        /// <summary>
        /// 当前点击的列名
        /// </summary>
        private String m_curColName;

        /// <summary>
        /// 开发表格
        /// </summary>
        private GridA m_gridProjects;

        /// <summary>
        /// 选择员工窗体
        /// </summary>
        private SelectStaffWindow m_selectStaffWindow;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            ProjectInfo project = new ProjectInfo();
            project.m_pID = DataCenter.ProjectService.GetNewPID();
            DataCenter.ProjectService.Save(project);
            AddProjectToGrid(project);
            m_gridProjects.Update();
            if (m_gridProjects.VScrollBar != null)
            {
                m_gridProjects.VScrollBar.ScrollToEnd();
            }
            m_gridProjects.Invalidate();
        }

        /// <summary>
        /// 添加项目
        /// </summary>
        /// <param name="project">项目</param>
        public void AddProjectToGrid(ProjectInfo project)
        {
            StaffService staffService = DataCenter.StaffService;
            List<GridRow> rows = m_gridProjects.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == project.m_pID)
                {
                    findRow.GetCell("colP2").SetString(project.m_name);
                    findRow.GetCell("colP3").SetString(project.m_jobIds);
                    findRow.GetCell("colP4").SetString(staffService.GetNamesByJobsID(project.m_jobIds));
                    findRow.GetCell("colP5").SetString(project.m_center);
                    findRow.GetCell("colP6").SetString(staffService.GetNamesByJobsID(project.m_center));
                    findRow.GetCell("colP7").SetString(project.m_state);
                    findRow.GetCell("colP8").SetString(project.m_startDate);
                    findRow.GetCell("colP9").SetString(project.m_endDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridProjects.AddRow(row);
            row.AddCell("colP1", new GridStringCell(project.m_pID));
            row.AddCell("colP2", new GridStringCell(project.m_name));
            row.AddCell("colP3", new GridStringCell(project.m_jobIds));
            row.AddCell("colP4", new GridStringCell(staffService.GetNamesByJobsID(project.m_jobIds)));
            row.AddCell("colP5", new GridStringCell(project.m_center));
            row.AddCell("colP6", new GridStringCell(staffService.GetNamesByJobsID(project.m_center)));
            row.AddCell("colP7", new GridStringCell(project.m_state));
            row.AddCell("colP8", new GridStringCell(project.m_startDate));
            row.AddCell("colP9", new GridStringCell(project.m_endDate));
            List<GridCell> cells = row.GetCells();
            int cellsSize = cells.Count;
            for (int j = 1; j < cellsSize; j++)
            {
                if (cells[j].Column.Name != "colP3" && cells[j].Column.Name != "colP4"
                    && cells[j].Column.Name != "colP5" && cells[j].Column.Name != "colP6")
                {
                    cells[j].AllowEdit = true;
                }
            }
        }

        /// <summary>
        /// 绑定员工
        /// </summary>
        private void BindProjects()
        {
            m_gridProjects.CellEditMode = GridCellEditMode.DoubleClick;
            List<ProjectInfo> projects = DataCenter.ProjectService.m_projects;
            int projectsSize = projects.Count;
            m_gridProjects.ClearRows();
            m_gridProjects.BeginUpdate();
            for (int i = 0; i < projectsSize; i++)
            {
                ProjectInfo project = projects[i];
                AddProjectToGrid(project);
            }
            m_gridProjects.EndUpdate();
            m_gridProjects.Invalidate();
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
                    ExportToExcel("开发项目.xls", m_gridProjects);
                }
                else if (name == "btnExportTxt")
                {
                    ExportToTxt("开发项目.txt", m_gridProjects);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridProjects.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.ProjectService.Delete(pID);
                    m_gridProjects.RemoveRow(deleteRow);
                    m_gridProjects.Update();
                    m_gridProjects.Invalidate();
                }
            }
        }

        /// <summary>
        /// 单元格点击方法
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="cell">单元格</param>
        /// <param name="mp">坐标</param>
        /// <param name="button">按钮</param>
        /// <param name="clicks">点击次数</param>
        /// <param name="delta">滚轮值</param>
        private void GridCellClick(object sender, GridCell cell, POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            String colName = cell.Column.Name;
            if (colName == "colP3" || colName == "colP4"
                || colName == "colP5" || colName == "colP6")
            {
                m_curColName = colName;
                m_selectStaffWindow = new SelectStaffWindow(Native);
                m_selectStaffWindow.Parent = this;
                if (colName == "colP3" || colName == "colP4")
                {
                    m_selectStaffWindow.BindJobIdsToResultGrid(cell.Row.GetCell("colP3").GetString());
                }
                else if (colName == "colP5" || colName == "colP6")
                {
                    m_selectStaffWindow.BindJobIdsToResultGrid(cell.Row.GetCell("colP5").GetString());
                }
                m_selectStaffWindow.ShowDialog();
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
                ProjectInfo project = DataCenter.ProjectService.GetProject(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    project.m_name = cellValue;
                }
                else if (colName == "colP7")
                {
                    project.m_state = cellValue;
                }
                else if (colName == "colP8")
                {
                    project.m_startDate = cellValue;
                }
                else if (colName == "colP9")
                {
                    project.m_endDate = cellValue;
                }
                DataCenter.ProjectService.Save(project);
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

        /// <summary>
        /// 是否确认关闭
        /// </summary>
        /// <returns>不处理</returns>
        public override int Submit()
        {
            if (m_selectStaffWindow != null)
            {
                ProjectService projectService = DataCenter.ProjectService;
                List<GridRow> selectedRows = m_gridProjects.SelectedRows;
                int selectedRowsSize = selectedRows.Count;
                if (selectedRowsSize > 0)
                {
                    String newJobID = m_selectStaffWindow.GetSelectedJobIDs();
                    ProjectInfo project = projectService.GetProject(selectedRows[0].GetCell("colP1").GetString());
                    if (m_curColName == "colP3" || m_curColName == "colP4")
                    {
                        project.m_jobIds = newJobID;
                    }
                    else if (m_curColName == "colP5" || m_curColName == "colP6")
                    {
                        project.m_center = newJobID;
                    }
                    projectService.Save(project);
                    AddProjectToGrid(project);
                }
                m_selectStaffWindow = null;
            }
            return 0;
        }
    }
}
