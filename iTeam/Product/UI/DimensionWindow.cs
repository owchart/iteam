/*****************************************************************************\
*                                                                             *
* DimensionWindow.cs - Dimension window functions, types                      *
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
    /// 六维图窗体
    /// </summary>
    public class DimensionWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public DimensionWindow(INativeBase native)
        {
            Load(native, "DimensionWindow", "dimensionWindow");
            RegisterEvents(m_window);
            m_divDimemsion = FindControl("divDimemsion") as DimemsionDiv;
            m_divDememsion2 = FindControl("divDimemsion2") as DimemsionDiv;
            m_gridStaffs = GetGrid("gridStaffs");
            BindStaffs();
            m_gridStaffs.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            m_gridStaffs.RegisterEvent(new ControlEvent(GridSelectedRowsChanged), EVENTID.GRIDSELECTEDROWSCHANGED);

        }

        /// <summary>
        /// 六维图
        /// </summary>
        private DimemsionDiv m_divDimemsion;

        /// <summary>
        /// 第二个六维图
        /// </summary>
        private DimemsionDiv m_divDememsion2;

        /// <summary>
        /// 员工表格
        /// </summary>
        private GridA m_gridStaffs;

        /// <summary>
        /// 绑定员工
        /// </summary>
        private void BindStaffs()
        {
            m_gridStaffs.CellEditMode = GridCellEditMode.DoubleClick;
            List<StaffInfo> staffs = DataCenter.StaffService.GetAliveStaffs();
            int staffsSize = staffs.Count;
            Dictionary<String, DimensionInfo> dimensionsMap = new Dictionary<String, DimensionInfo>();
            List<DimensionInfo> dimensions = DataCenter.DimensionService.m_dimensions;
            int dimensionsSize = dimensions.Count;
            for (int i = 0; i < dimensionsSize; i++)
            {
                DimensionInfo dimension = dimensions[i];
                if (dimension.m_jobID != null && dimension.m_jobID.Length > 0)
                {
                    dimensionsMap[dimension.m_jobID] = dimension;
                }
            }
            m_gridStaffs.ClearRows();
            m_gridStaffs.BeginUpdate();
            for (int i = 0; i < staffsSize; i++)
            {
                StaffInfo staff = staffs[i];
                DimensionInfo dimension = null;
                if (dimensionsMap.ContainsKey(staff.m_jobID))
                {
                    dimension = dimensionsMap[staff.m_jobID];
                }
                else
                {
                    dimension = new DimensionInfo();
                    dimension.m_business = 100;
                    dimension.m_EQ = 100;
                    dimension.m_IQ = 100;
                    dimension.m_knowledge = 100;
                    dimension.m_lead = 100;
                    dimension.m_technology = 100;
                    dimension.m_jobID = staff.m_jobID;
                }
                GridRow row = new GridRow();
                m_gridStaffs.AddRow(row);
                row.AddCell("colF1", new GridStringCell(staff.m_jobID));
                row.AddCell("colF2", new GridStringCell(staff.m_name));
                row.AddCell("colF3", new GridIntCell(dimension.m_business));
                row.AddCell("colF4", new GridIntCell(dimension.m_EQ));
                row.AddCell("colF5", new GridIntCell(dimension.m_knowledge));
                row.AddCell("colF6", new GridIntCell(dimension.m_IQ));
                row.AddCell("colF7", new GridIntCell(dimension.m_lead));
                row.AddCell("colF8", new GridIntCell(dimension.m_technology));
                int avg = (dimension.m_business + dimension.m_EQ + dimension.m_knowledge + dimension.m_IQ + dimension.m_lead
                + dimension.m_technology) / 6;
                row.AddCell("colF9", new GridIntCell(avg));
                List<GridCell> cells = row.GetCells();
                int cellsSize = cells.Count;
                for (int j = 2; j < cellsSize - 1; j++)
                {
                    cells[j].AllowEdit = true;
                }
            }
            m_gridStaffs.EndUpdate();
            m_gridStaffs.Invalidate();
            dimensionsMap.Clear();
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
                DimensionService dimensionService = DataCenter.DimensionService;
                GridRow row = cell.Row;
                String jobID = row.GetCell("colF1").GetString();
                DimensionInfo dimension = dimensionService.GetDimension(jobID);
                dimension.m_jobID = jobID;
                dimension.m_business = row.GetCell("colF3").GetInt();
                dimension.m_EQ = row.GetCell("colF4").GetInt();
                dimension.m_knowledge = row.GetCell("colF5").GetInt();
                dimension.m_IQ = row.GetCell("colF6").GetInt();
                dimension.m_lead = row.GetCell("colF7").GetInt();
                dimension.m_technology = row.GetCell("colF8").GetInt();
                int avg = (dimension.m_business + dimension.m_EQ + dimension.m_knowledge + dimension.m_IQ + dimension.m_lead
               + dimension.m_technology) / 6;
                row.GetCell("colF9").SetInt(avg);
                dimensionService.Save(dimension);
                m_divDimemsion.Dimension = dimension;
                Native.Invalidate();
            }
        }

        /// <summary>
        /// 表格选中行改变事件
        /// </summary>
        /// <param name="sender">调用者</param>
        private void GridSelectedRowsChanged(object sender)
        {
            OnGridSelectedRowsChanged();
        }

        /// <summary>
        /// 表格选中行改变方法
        /// </summary>
        private void OnGridSelectedRowsChanged()
        {
            List<GridRow> selectedRows = m_gridStaffs.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                DimensionInfo oldDimension = m_divDimemsion.Dimension;
                String oldText = m_divDimemsion.Text;
                if (oldDimension != null)
                {
                    m_divDememsion2.Dimension = oldDimension;
                    m_divDememsion2.Text = oldText;
                }
                GridRow row = selectedRows[0];
                DimensionInfo dimension = new DimensionInfo();
                dimension.m_jobID = row.GetCell("colF1").GetString();
                dimension.m_business = row.GetCell("colF3").GetInt();
                dimension.m_EQ = row.GetCell("colF4").GetInt();
                dimension.m_knowledge = row.GetCell("colF5").GetInt();
                dimension.m_IQ = row.GetCell("colF6").GetInt();
                dimension.m_lead = row.GetCell("colF7").GetInt();
                dimension.m_technology = row.GetCell("colF8").GetInt();
                m_divDimemsion.Text = row.GetCell("colF2").GetString();
                m_divDimemsion.Dimension = dimension;
                Native.Invalidate();
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
