/*****************************************************************************\
*                                                                             *
* BeAttackedWindow.cs - BeAttacked window functions, types                      *
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
    /// 上级指示窗体
    /// </summary>
    public class BeAttackedWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public BeAttackedWindow(INativeBase native)
        {
            Load(native, "BeAttackedWindow", "beAttackedWindow");
            RegisterEvents(m_window);
            m_gridBeAttackeds = GetGrid("gridBeAttackeds");
            m_gridBeAttackeds.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindBeAttackeds();
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridBeAttackeds;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            BeAttackedInfo beAttacked = new BeAttackedInfo();
            beAttacked.m_ID = DataCenter.BeAttackedService.GetNewID();
            DataCenter.BeAttackedService.Save(beAttacked);
            AddBeAttackedToGrid(beAttacked);
            m_gridBeAttackeds.Update();
            if (m_gridBeAttackeds.VScrollBar != null)
            {
                m_gridBeAttackeds.VScrollBar.ScrollToEnd();
            }
            m_gridBeAttackeds.Invalidate();
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="beAttacked">信息</param>
        public void AddBeAttackedToGrid(BeAttackedInfo beAttacked)
        {
            List<GridRow> rows = m_gridBeAttackeds.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == beAttacked.m_ID)
                {
                    findRow.GetCell("colP2").SetString(beAttacked.m_level);
                    findRow.GetCell("colP3").SetString(beAttacked.m_title);
                    findRow.GetCell("colP4").SetString(beAttacked.m_content);
                    findRow.GetCell("colP5").SetString(beAttacked.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridBeAttackeds.AddRow(row);
            row.AddCell("colP1", new GridStringCell(beAttacked.m_ID));
            row.AddCell("colP2", new GridStringCell(beAttacked.m_level));
            row.AddCell("colP3", new GridStringCell(beAttacked.m_title));
            row.AddCell("colP4", new GridStringCell(beAttacked.m_content));
            row.AddCell("colP5", new GridStringCell(beAttacked.m_createDate));
            List<GridCell> cells = row.GetCells();
            int cellsSize = cells.Count;
            for (int j = 1; j < cellsSize; j++)
            {
                cells[j].AllowEdit = true;
            }
        }

        /// <summary>
        /// 绑定指示
        /// </summary>
        private void BindBeAttackeds()
        {
            m_gridBeAttackeds.CellEditMode = GridCellEditMode.DoubleClick;
            List<BeAttackedInfo> beAttackeds = DataCenter.BeAttackedService.m_beAttackeds;
            int beAttackedsSize = beAttackeds.Count;
            m_gridBeAttackeds.ClearRows();
            m_gridBeAttackeds.BeginUpdate();
            for (int i = 0; i < beAttackedsSize; i++)
            {
                BeAttackedInfo beAttacked = beAttackeds[i];
                AddBeAttackedToGrid(beAttacked);
            }
            m_gridBeAttackeds.EndUpdate();
            m_gridBeAttackeds.Invalidate();
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
            List<GridRow> selectedRows = m_gridBeAttackeds.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.BeAttackedService.Delete(pID);
                    m_gridBeAttackeds.RemoveRow(deleteRow);
                    m_gridBeAttackeds.Update();
                    m_gridBeAttackeds.Invalidate();
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
                BeAttackedInfo beAttacked = DataCenter.BeAttackedService.GetBeAttacked(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    beAttacked.m_level = cellValue;
                }
                else if (colName == "colP3")
                {
                    beAttacked.m_title = cellValue;
                }
                else if (colName == "colP4")
                {
                    beAttacked.m_content = cellValue;
                }
                else if (colName == "colP5")
                {
                    beAttacked.m_createDate = cellValue;
                }
                DataCenter.BeAttackedService.Save(beAttacked);
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
