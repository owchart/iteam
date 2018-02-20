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
    public class ResearchWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public ResearchWindow(INativeBase native)
        {
            Load(native, "ResearchWindow", "researchWindow");
            RegisterEvents(m_window);
            m_gridResearches = GetGrid("gridResearches");
            m_gridResearches.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            m_gridResearches.RegisterEvent(new GridCellMouseEvent(GridCellClick), EVENTID.GRIDCELLCLICK);
            BindProjects();
        }

        /// <summary>
        /// 研发表格
        /// </summary>
        private GridA m_gridResearches;

        /// <summary>
        /// 选择员工窗体
        /// </summary>
        private SelectStaffWindow m_selectStaffWindow;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            ResearchService researchService = DataCenter.ResearchService;
            ResearchInfo research = new ResearchInfo();
            research.m_pID = DataCenter.ResearchService.GetNewPID();
            DataCenter.ResearchService.Save(research);
            AddResearchToGrid(research);
            m_gridResearches.Update();
            if (m_gridResearches.VScrollBar != null)
            {
                m_gridResearches.VScrollBar.ScrollToEnd();
            }
            m_gridResearches.Invalidate();
        }

        /// <summary>
        /// 添加项目
        /// </summary>
        /// <param name="research">项目</param>
        public void AddResearchToGrid(ResearchInfo research)
        {
            StaffService staffService = DataCenter.StaffService;
            List<GridRow> rows = m_gridResearches.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == research.m_pID)
                {
                    findRow.GetCell("colP2").SetString(research.m_name);
                    findRow.GetCell("colP3").SetString(research.m_jobIds);
                    findRow.GetCell("colP4").SetString(staffService.GetNamesByJobsID(research.m_jobIds));
                    findRow.GetCell("colP5").SetString(research.m_state);
                    findRow.GetCell("colP6").SetString(research.m_startDate);
                    findRow.GetCell("colP7").SetString(research.m_endDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridResearches.AddRow(row);
            row.AddCell("colP1", new GridStringCell(research.m_pID));
            row.AddCell("colP2", new GridStringCell(research.m_name));
            row.AddCell("colP3", new GridStringCell(research.m_jobIds));
            row.AddCell("colP4", new GridStringCell(staffService.GetNamesByJobsID(research.m_jobIds)));
            row.AddCell("colP5", new GridStringCell(research.m_state));
            row.AddCell("colP6", new GridStringCell(research.m_startDate));
            row.AddCell("colP7", new GridStringCell(research.m_endDate));
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
        private void BindProjects()
        {
            m_gridResearches.CellEditMode = GridCellEditMode.DoubleClick;
            List<ResearchInfo> researches = DataCenter.ResearchService.m_researchs;
            int researchesSize = researches.Count;
            m_gridResearches.ClearRows();
            m_gridResearches.BeginUpdate();
            for (int i = 0; i < researchesSize; i++)
            {
                ResearchInfo research = researches[i];
                AddResearchToGrid(research);
            }
            m_gridResearches.EndUpdate();
            m_gridResearches.Invalidate();
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
                    ExportToExcel("研发项目.xls", m_gridResearches);
                }
                else if (name == "btnExportTxt")
                {
                    ExportToTxt("研发项目.txt", m_gridResearches);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridResearches.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.ResearchService.Delete(pID);
                    m_gridResearches.RemoveRow(deleteRow);
                    m_gridResearches.Update();
                    m_gridResearches.Invalidate();
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
                ResearchInfo research = DataCenter.ResearchService.GetResearch(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    research.m_name = cellValue;
                }
                else if (colName == "colP5")
                {
                    research.m_state = cellValue;
                }
                else if (colName == "colP6")
                {
                    research.m_startDate = cellValue;
                }
                else if (colName == "colP7")
                {
                    research.m_endDate = cellValue;
                }
                DataCenter.ResearchService.Save(research);
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
            if (colName == "colP3" || colName == "colP4")
            {
                m_selectStaffWindow = new SelectStaffWindow(Native);
                m_selectStaffWindow.Parent = this;
                m_selectStaffWindow.BindJobIdsToResultGrid(cell.Row.GetCell("colP3").GetString());
                m_selectStaffWindow.ShowDialog();
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
                ResearchService researchService = DataCenter.ResearchService;
                List<GridRow> selectedRows = m_gridResearches.SelectedRows;
                int selectedRowsSize = selectedRows.Count;
                if (selectedRowsSize > 0)
                {
                    String newJobID = m_selectStaffWindow.GetSelectedJobIDs();
                    ResearchInfo research = researchService.GetResearch(selectedRows[0].GetCell("colP1").GetString());
                    research.m_jobIds = newJobID;
                    researchService.Save(research);
                    AddResearchToGrid(research);
                }
                m_selectStaffWindow = null;
            }
            return 0;
        }
    }
}
