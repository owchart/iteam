/*****************************************************************************\
*                                                                             *
* CheckWindow.cs - Check code window functions, types                      *
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
    /// 操作窗体
    /// </summary>
    public class CheckWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public CheckWindow(INativeBase native)
        {
            Load(native, "CheckWindow", "checkWindow");
            RegisterEvents(m_window);
            m_gridCheck = GetGrid("gridCheck");
            m_gridCheck.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            m_gridCheck.RegisterEvent(new GridCellMouseEvent(GridCellClick), EVENTID.GRIDCELLCLICK);
            BindAwards();
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridCheck;

        /// <summary>
        /// 选择员工窗体
        /// </summary>
        private SelectStaffWindow m_selectStaffWindow;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            CheckService checkService = DataCenter.CheckService;
            CheckInfo checkInfo = new CheckInfo();
            checkInfo.m_ID = checkService.GetNewID();
            checkService.Save(checkInfo);
            AddCheckToGrid(checkInfo);
            m_gridCheck.Update();
            if (m_gridCheck.VScrollBar != null)
            {
                m_gridCheck.VScrollBar.ScrollToEnd();
            }
            m_gridCheck.Invalidate();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="checkInfo">信息</param>
        public void AddCheckToGrid(CheckInfo checkInfo)
        {
            StaffService staffService = DataCenter.StaffService;
            StaffInfo staff = staffService.GetStaff(checkInfo.m_jobID);
            List<GridRow> rows = m_gridCheck.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == checkInfo.m_ID)
                {
                    findRow.GetCell("colP2").SetString(checkInfo.m_jobID);
                    findRow.GetCell("colP3").SetString(staffService.GetNamesByJobsID(checkInfo.m_jobID));
                    findRow.GetCell("colP4").SetString(checkInfo.m_answer);
                    findRow.GetCell("colP5").SetString(checkInfo.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridCheck.AddRow(row);
            row.AddCell("colP1", new GridStringCell(checkInfo.m_ID));
            row.AddCell("colP2", new GridStringCell(checkInfo.m_jobID));
            row.AddCell("colP3", new GridStringCell(staffService.GetNamesByJobsID(checkInfo.m_jobID)));
            row.AddCell("colP4", new GridStringCell(checkInfo.m_answer));
            row.AddCell("colP5", new GridStringCell(checkInfo.m_createDate));
            List<GridCell> cells = row.GetCells();
            int cellsSize = cells.Count;
            for (int j = 1; j < cellsSize; j++)
            {
                if (cells[j].Column.Name != "colP2" && cells[j].Column.Name != "colP3")
                {
                    cells[j].AllowEdit = true;
                }
            }
        }

        /// <summary>
        /// 绑定员工
        /// </summary>
        private void BindAwards()
        {
            m_gridCheck.CellEditMode = GridCellEditMode.DoubleClick;
            List<CheckInfo> checks = DataCenter.CheckService.m_checks;
            int checkSize = checks.Count;
            m_gridCheck.ClearRows();
            m_gridCheck.BeginUpdate();
            for (int i = 0; i < checkSize; i++)
            {
                CheckInfo checkInfo = checks[i];
                AddCheckToGrid(checkInfo);
            }
            m_gridCheck.EndUpdate();
            m_gridCheck.Invalidate();
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
                    ExportToExcel("个人检查.xls", m_gridCheck);
                }
                else if (name == "btnExportTxt")
                {
                    ExportToTxt("个人检查.txt", m_gridCheck);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridCheck.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.CheckService.Delete(pID);
                    m_gridCheck.RemoveRow(deleteRow);
                    m_gridCheck.Update();
                    m_gridCheck.Invalidate();
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
            if (colName == "colP2" || colName == "colP3")
            {
                m_selectStaffWindow = new SelectStaffWindow(Native);
                m_selectStaffWindow.Parent = this;
                m_selectStaffWindow.BindJobIdsToResultGrid(cell.Row.GetCell("colP2").GetString());
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
                CheckInfo checkInfo = DataCenter.CheckService.GetCheck(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP4")
                {
                    checkInfo.m_answer = cellValue;
                }
                else if (colName == "colP5")
                {
                    checkInfo.m_createDate = cellValue;
                }
                DataCenter.CheckService.Save(checkInfo);
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
                CheckService checkService = DataCenter.CheckService;
                List<GridRow> selectedRows = m_gridCheck.SelectedRows;
                int selectedRowsSize = selectedRows.Count;
                if (selectedRowsSize > 0)
                {
                    String newJobID = m_selectStaffWindow.GetSelectedJobIDs();
                    CheckInfo checkInfo = checkService.GetCheck(selectedRows[0].GetCell("colP1").GetString());
                    checkInfo.m_jobID = newJobID;
                    checkService.Save(checkInfo);
                    AddCheckToGrid(checkInfo);
                }
                m_selectStaffWindow = null;
            }
            return 0;
        }
    }
}
