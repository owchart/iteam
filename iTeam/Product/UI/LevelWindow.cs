/*****************************************************************************\
*                                                                             *
* LevelWindow.cs - Level window functions, types                      *
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
    /// 级别窗体
    /// </summary>
    public class LevelWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public LevelWindow(INativeBase native)
        {
            Load(native, "LevelWindow", "levelWindow");
            RegisterEvents(m_window);
            m_chartLevels = GetChart("chartLevels");
            m_chartLevels.HScalePixel = 20;
            m_chartLevels.HScaleFieldText = "日期";
            CTable dataSource = m_chartLevels.DataSource;
            dataSource.AddColumn(0);
            dataSource.AddColumn(1);
            dataSource.AddColumn(2);
            dataSource.AddColumn(3);
            dataSource.AddColumn(4);
            CDiv div = m_chartLevels.AddDiv();
            div.BackColor = COLOR.ARGB(255, 255, 255);
            div.LeftVScale.ScaleColor = COLOR.ARGB(50, 105, 217);
            div.HScale.ScaleColor = COLOR.ARGB(50, 105, 217);
            div.HGrid.GridColor = COLOR.ARGB(50, 105, 217);
            div.HGrid.LineStyle = 2;
            PolylineShape ps = new PolylineShape();
            ps.FieldName = 0;
            ps.FieldText = "开发";
            ps.FillColor = COLOR.ARGB(100, 50, 105, 217);
            ps.Color = COLOR.ARGB(50, 105, 217);
            div.AddShape(ps);
            PolylineShape ps2 = new PolylineShape();
            ps2.FieldName = 3;
            ps2.FieldText = "研发";
            ps2.FillColor = COLOR.ARGB(100, 255, 80, 80);
            ps2.Color = COLOR.ARGB(255, 80, 80);
            div.AddShape(ps2);
            PolylineShape ps3 = new PolylineShape();
            ps3.FieldName = 4;
            ps3.FieldText = "规范";
            ps3.FillColor = COLOR.ARGB(100, 80, 255, 80);
            ps3.Color = COLOR.ARGB(80, 255, 80);
            div.AddShape(ps3);
            PolylineShape psTop = new PolylineShape();
            psTop.FieldName = 1;
            div.AddShape(psTop);
            PolylineShape psBottom = new PolylineShape();
            psBottom.FieldName = 2;
            div.AddShape(psBottom);
            m_gridLevels = GetGrid("gridLevels");
            m_gridLevels.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindLevels();
            BindChart();
        }

        private ChartA m_chartLevels;

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridLevels;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            LevelInfo level = new LevelInfo();
            level.m_ID = DataCenter.LevelService.GetNewID();
            DataCenter.LevelService.Save(level);
            AddLevelToGrid(level);
            m_gridLevels.Update();
            if (m_gridLevels.VScrollBar != null)
            {
                m_gridLevels.VScrollBar.ScrollToEnd();
            }
            m_gridLevels.Invalidate();
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="level">信息</param>
        public void AddLevelToGrid(LevelInfo level)
        {
            List<GridRow> rows = m_gridLevels.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == level.m_ID)
                {
                    findRow.GetCell("colP2").SetString(level.m_date);
                    findRow.GetCell("colP3").SetString(level.m_level);
                    findRow.GetCell("colP4").SetString(level.m_level2);
                    findRow.GetCell("colP5").SetString(level.m_level3);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridLevels.AddRow(row);
            row.AddCell("colP1", new GridStringCell(level.m_ID));
            row.AddCell("colP2", new GridStringCell(level.m_date));
            row.AddCell("colP3", new GridStringCell(level.m_level));
            row.AddCell("colP4", new GridStringCell(level.m_level2));
            row.AddCell("colP5", new GridStringCell(level.m_level3));
            List<GridCell> cells = row.GetCells();
            int cellsSize = cells.Count;
            for (int j = 1; j < cellsSize; j++)
            {
                cells[j].AllowEdit = true;
            }
        }

        /// <summary>
        /// 绑定图形
        /// </summary>
        private void BindChart()
        {
            m_chartLevels.Clear();
            CTable dataSource = m_chartLevels.DataSource;
            List<LevelInfo> levels = DataCenter.LevelService.m_levels;
            int levelsSize = levels.Count;
            for (int i = 0; i < levelsSize; i++)
            {
                if (levels[i].m_date.Length == 8)
                {
                    int year = CStr.ConvertStrToInt(levels[i].m_date.Substring(0, 4));
                    int month = CStr.ConvertStrToInt(levels[i].m_date.Substring(4, 2));
                    int day = CStr.ConvertStrToInt(levels[i].m_date.Substring(6, 2));
                    DateTime dt = new DateTime(year, month, day);
                    double pk = CStr.ConvertDateToNum(dt);
                    dataSource.Set(pk, 0, CStr.ConvertStrToDouble(levels[i].m_level));
                    dataSource.Set2(i, 1, 10);
                    dataSource.Set2(i, 2, 0);
                    dataSource.Set2(i, 3, CStr.ConvertStrToDouble(levels[i].m_level2));
                    dataSource.Set2(i, 4, CStr.ConvertStrToDouble(levels[i].m_level3));
                }
            }
            m_chartLevels.Update();
            m_chartLevels.Invalidate();
        }

        /// <summary>
        /// 绑定指示
        /// </summary>
        private void BindLevels()
        {
            m_gridLevels.CellEditMode = GridCellEditMode.DoubleClick;
            List<LevelInfo> levels = DataCenter.LevelService.m_levels;
            int levelsSize = levels.Count;
            m_gridLevels.ClearRows();
            m_gridLevels.BeginUpdate();
            for (int i = 0; i < levelsSize; i++)
            {
                LevelInfo level = levels[i];
                AddLevelToGrid(level);
            }
            m_gridLevels.EndUpdate();
            m_gridLevels.Invalidate();
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
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridLevels.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.MasterService.Delete(pID);
                    m_gridLevels.RemoveRow(deleteRow);
                    m_gridLevels.Update();
                    m_gridLevels.Invalidate();
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
                LevelInfo level = DataCenter.LevelService.GetLevel(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    level.m_date = cellValue;
                }
                else if (colName == "colP3")
                {
                    level.m_level = cellValue;
                }
                else if (colName == "colP4")
                {
                    level.m_level2 = cellValue;
                }
                else if (colName == "colP5")
                {
                    level.m_level3 = cellValue;
                }
                DataCenter.LevelService.Save(level);
                BindChart();
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
