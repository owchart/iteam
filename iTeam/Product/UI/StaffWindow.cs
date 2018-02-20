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
    /// 员工操作窗体
    /// </summary>
    public class StaffWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public StaffWindow(INativeBase native)
        {
            Load(native, "StaffWindow", "staffWindow");
            RegisterEvents(m_window);
            m_gridStaffs = GetGrid("gridStaffs");
            m_gridStaffs.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindStaffs();
        }

        /// <summary>
        /// 员工表格
        /// </summary>
        private GridA m_gridStaffs;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            StaffInfo staff = new StaffInfo();
            staff.m_jobID = DataCenter.StaffService.GetNewJobID();
            DataCenter.StaffService.Save(staff);
            AddStaffToGrid(staff);
            m_gridStaffs.Update();
            if (m_gridStaffs.VScrollBar != null)
            {
                m_gridStaffs.VScrollBar.ScrollToEnd();
            }
            m_gridStaffs.Invalidate();
        }

        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="staff">员工</param>
        public void AddStaffToGrid(StaffInfo staff)
        {
            List<GridRow> rows = m_gridStaffs.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colS1").GetString() == staff.m_jobID)
                {
                    findRow.GetCell("colS2").SetString(staff.m_name);
                    findRow.GetCell("colS3").SetString(staff.m_sex);
                    findRow.GetCell("colS4").SetString(staff.m_state);
                    findRow.GetCell("colS5").SetString(staff.m_education);
                    findRow.GetCell("colS6").SetString(staff.m_degree);
                    findRow.GetCell("colS7").SetString(staff.m_birthDay);
                    findRow.GetCell("colS8").SetString(staff.m_entryDay);
                    findRow.GetCell("colS9").SetString(staff.m_canSelect);
                    findRow.GetCell("colS10").SetString(staff.m_isManager);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridStaffs.AddRow(row);
            row.AddCell("colS1", new GridStringCell(staff.m_jobID));
            row.AddCell("colS2", new GridStringCell(staff.m_name));
            row.AddCell("colS3", new GridStringCell(staff.m_sex));
            row.AddCell("colS4", new GridStringCell(staff.m_state));
            row.AddCell("colS5", new GridStringCell(staff.m_education));
            row.AddCell("colS6", new GridStringCell(staff.m_degree));
            row.AddCell("colS7", new GridStringCell(staff.m_birthDay));
            row.AddCell("colS8", new GridStringCell(staff.m_entryDay));
            row.AddCell("colS9", new GridStringCell(staff.m_canSelect));
            row.AddCell("colS10", new GridStringCell(staff.m_isManager));
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
        private void BindStaffs()
        {
            m_gridStaffs.CellEditMode = GridCellEditMode.DoubleClick;
            List<StaffInfo> staffs = DataCenter.StaffService.m_staffs;
            int staffsSize = staffs.Count;
            m_gridStaffs.ClearRows();
            m_gridStaffs.BeginUpdate();
            for (int i = 0; i < staffsSize; i++)
            {
                StaffInfo staff = staffs[i];
                AddStaffToGrid(staff);
            }
            m_gridStaffs.EndUpdate();
            m_gridStaffs.Invalidate();
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
                    ExportToExcel("员工列表.xls", m_gridStaffs);
                }
                else if (name == "btnExportTxt")
                {
                    ExportToTxt("员工列表.txt", m_gridStaffs);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridStaffs.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String jobID = deleteRow.GetCell("colS1").GetString();
                    DataCenter.StaffService.Delete(jobID);
                    m_gridStaffs.RemoveRow(deleteRow);
                    m_gridStaffs.Update();
                    m_gridStaffs.Invalidate();
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
                StaffInfo staff = DataCenter.StaffService.GetStaff(cell.Row.GetCell("colS1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colS2")
                {
                    staff.m_name = cellValue;
                }
                else if (colName == "colS3")
                {
                    staff.m_sex = cellValue;
                }
                else if (colName == "colS4")
                {
                    staff.m_state = cellValue;
                }
                else if (colName == "colS5")
                {
                    staff.m_education = cellValue;
                }
                else if (colName == "colS6")
                {
                    staff.m_degree = cellValue;
                }
                else if (colName == "colS7")
                {
                    staff.m_birthDay = cellValue;
                }
                else if (colName == "colS8")
                {
                    staff.m_entryDay = cellValue;
                }
                else if (colName == "colS9")
                {
                    staff.m_canSelect = cellValue;
                }
                else if (colName == "colS10")
                {
                    staff.m_isManager = cellValue;
                }
                DataCenter.StaffService.Save(staff);
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
