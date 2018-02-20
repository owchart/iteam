/*****************************************************************************\
*                                                                             *
* SelectStaffWindow.cs - Select staff window functions, types                      *
*                                                                             *
*               Version 1.00  ★                                              *
*                                                                             *
*               Copyright (c) 2017-2017, iTeam. All rights reserved.      *
*               Created by Todd 2017/6/19.                          *
*                                                                             *
******************************************************************************/

using Newtonsoft.Json;
using OwLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace OwLib
{
    /// <summary>
    /// 选择员工窗体
    /// </summary>
    public class SelectStaffWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public SelectStaffWindow(INativeBase native)
        {
            Load(native, "SelectStaffWindow", "selectStaffWindow");
            RegisterEvents(m_window);
            m_gridResult = GetGrid("gridResult");
            m_gridStaffs = GetGrid("gridStaffs");
            BindStaffs();
        }

        /// <summary>
        /// 结果表格
        /// </summary>
        private GridA m_gridResult;

        /// <summary>
        /// 成员表格
        /// </summary>
        private GridA m_gridStaffs;

        /// <summary>
        /// 添加成员到结果表格
        /// </summary>
        /// <param name="staffs">成员</param>
        public void AddToResultGrid(List<StaffInfo> staffs)
        {
            Dictionary<String, String> existsIds = new Dictionary<String, String>();
            List<GridRow> rows = m_gridResult.GetRows();
            int rowSize = rows.Count;
            for (int i = 0; i < rowSize; i++)
            {
                GridRow row = rows[i];
                String id = row.GetCell(0).GetString();
                existsIds[id] = "";
            }
            int staffsSize = staffs.Count;
            m_gridResult.BeginUpdate();
            for (int i = 0; i < staffsSize; i++)
            {
                StaffInfo staff = staffs[i];
                if (!existsIds.ContainsKey(staff.m_jobID))
                {
                    GridRow row = new GridRow();
                    m_gridResult.AddRow(row);
                    row.AddCell(0, new GridStringCell(staff.m_jobID));
                    row.AddCell(1, new GridStringCell(staff.m_name));
                    existsIds[staff.m_jobID] = "";
                }
            }
            m_gridResult.EndUpdate();
            m_gridResult.Invalidate();
        }

        /// <summary>
        /// 绑定员工
        /// </summary>
        public void BindStaffs()
        {
            m_gridStaffs.BeginUpdate();
            List<StaffInfo> staffs = DataCenter.StaffService.GetAliveStaffs();
            int staffsSize = staffs.Count;
            for (int i = 0; i < staffsSize; i++)
            {
                StaffInfo staff = staffs[i];
                GridRow row = new GridRow();
                m_gridStaffs.AddRow(row);
                row.AddCell(0, new GridStringCell(staff.m_jobID));
                row.AddCell(1, new GridStringCell(staff.m_name));
            }
            m_gridStaffs.EndUpdate();
            m_gridStaffs.Invalidate();
        }

        /// <summary>
        /// 绑定员工到结果表格
        /// </summary>
        public void BindJobIdsToResultGrid(String jobIDs)
        {
            m_gridResult.BeginUpdate();
            String[] strs = jobIDs.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int strsSize = strs.Length;
            for (int i = 0; i < strsSize; i++)
            {
                StaffInfo staff = DataCenter.StaffService.GetStaff(strs[i]);
                GridRow row = new GridRow();
                m_gridResult.AddRow(row);
                row.AddCell(0, new GridStringCell(staff.m_jobID));
                row.AddCell(1, new GridStringCell(staff.m_name));
            }
            m_gridResult.EndUpdate();
            m_gridResult.Invalidate();
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
                if (name == "btnCancel")
                {
                    Close();
                }
                else if (name == "btnSelect")
                {
                    Select();
                }
                else if (name == "btnSelectAll")
                {
                    SelectAll();
                }
                else if (name == "btnSubmit")
                {
                    if (m_parent != null)
                    {
                        if (m_parent.Submit() == 0)
                        {
                            Close();
                        }
                    }
                }
                else if (name == "btnUnSelect")
                {
                    UnSelect();
                }
                else if (name == "btnUnSelectAll")
                {
                    UnSelectAll();
                }
            }
        }

        /// <summary>
        /// 获取选中的ID
        /// </summary>
        public String GetSelectedJobIDs()
        {
            String jobIDs = "";
            List<GridRow> rows = m_gridResult.GetRows();
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                jobIDs += rows[i].GetCell(0).GetString();
                if (i != rowsSize - 1)
                {
                    jobIDs += ",";
                }
            }
            return jobIDs;
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
        /// 从表格中删除员工
        /// </summary>
        /// <param name="staffs">员工列表</param>
        public void RemoveFromResultGrid(List<StaffInfo> staffs)
        {
            Dictionary<String, StaffInfo> existsIds = new Dictionary<String, StaffInfo>();
            int staffsSize = staffs.Count;
            for (int i = 0; i < staffsSize; i++)
            {
                StaffInfo staff = staffs[i];
                existsIds[staff.m_jobID] = staff;
            }
            int rowSize = m_gridResult.GetRows().Count;
            m_gridResult.BeginUpdate();
            for (int i = 0; i < rowSize; i++)
            {
                GridRow row = m_gridResult.GetRow(i);
                StaffInfo staffInfo = DataCenter.StaffService.GetStaff(row.GetCell(0).GetString());
                if (existsIds.ContainsKey(staffInfo.m_jobID))
                {
                    m_gridResult.RemoveRow(row);
                    i--;
                    rowSize--;
                }
            }
            m_gridResult.EndUpdate();
            m_gridResult.Invalidate();
        }

        /// <summary>
        /// 选中股票
        /// </summary>
        public void Select()
        {
            List<GridRow> selectedRows = m_gridStaffs.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                List<StaffInfo> staffs = new List<StaffInfo>();
                for (int i = 0; i < selectedRowsSize; i++)
                {
                    GridRow row = selectedRows[i];
                    staffs.Add(DataCenter.StaffService.GetStaff(row.GetCell(0).GetString()));
                }
                AddToResultGrid(staffs);
            }
        }

        /// <summary>
        /// 选中全部
        /// </summary>
        public void SelectAll()
        {
            List<GridRow> rows = m_gridStaffs.GetRows();
            int rowSize = rows.Count;
            if (rowSize > 0)
            {
                List<StaffInfo> staffs = new List<StaffInfo>();
                for (int i = 0; i < rowSize; i++)
                {
                    GridRow row = rows[i];
                    staffs.Add(DataCenter.StaffService.GetStaff(row.GetCell(0).GetString()));
                }
                AddToResultGrid(staffs);
            }
        }

        /// <summary>
        /// 取消选中所有
        /// </summary>
        public void UnSelectAll()
        {
            List<GridRow> rows = m_gridResult.GetRows();
            int rowSize = rows.Count;
            if (rowSize > 0)
            {
                List<StaffInfo> staffs = new List<StaffInfo>();
                for (int i = 0; i < rowSize; i++)
                {
                    GridRow row = rows[i];
                    staffs.Add(DataCenter.StaffService.GetStaff(row.GetCell(0).GetString()));
                }
                RemoveFromResultGrid(staffs);
            }
        }

        /// <summary>
        /// 取消选中
        /// </summary>
        public void UnSelect()
        {
            List<GridRow> selectedRows = m_gridResult.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                List<StaffInfo> staffs = new List<StaffInfo>();
                for (int i = 0; i < selectedRowsSize; i++)
                {
                    GridRow row = selectedRows[i];
                    staffs.Add(DataCenter.StaffService.GetStaff(row.GetCell(0).GetString()));
                }
                RemoveFromResultGrid(staffs);
            }
        }
    }
}
