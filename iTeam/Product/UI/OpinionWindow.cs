/*****************************************************************************\
*                                                                             *
* OpinionWindow.cs - Opinion window functions, types                      *
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
    /// 意见窗体
    /// </summary>
    public class OpinionWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public OpinionWindow(INativeBase native)
        {
            Load(native, "OpinionWindow", "opinionWindow");
            RegisterEvents(m_window);
            m_gridOpinions = GetGrid("gridOpinions");
            m_gridOpinions.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindClues();
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridOpinions;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            OpinionInfo opinion = new OpinionInfo();
            opinion.m_ID = DataCenter.OpinionService.GetNewID();
            DataCenter.OpinionService.Save(opinion);
            AddOpinionToGrid(opinion);
            m_gridOpinions.Update();
            if (m_gridOpinions.VScrollBar != null)
            {
                m_gridOpinions.VScrollBar.ScrollToEnd();
            }
            m_gridOpinions.Invalidate();
        }

        /// <summary>
        /// 添加线索
        /// </summary>
        /// <param name="opinion">线索</param>
        public void AddOpinionToGrid(OpinionInfo opinion)
        {
            List<GridRow> rows = m_gridOpinions.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == opinion.m_ID)
                {
                    findRow.GetCell("colP2").SetString(opinion.m_level);
                    findRow.GetCell("colP3").SetString(opinion.m_title);
                    findRow.GetCell("colP4").SetString(opinion.m_content);
                    findRow.GetCell("colP5").SetString(opinion.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridOpinions.AddRow(row);
            row.AddCell("colP1", new GridStringCell(opinion.m_ID));
            row.AddCell("colP2", new GridStringCell(opinion.m_level));
            row.AddCell("colP3", new GridStringCell(opinion.m_title));
            row.AddCell("colP4", new GridStringCell(opinion.m_content));
            row.AddCell("colP5", new GridStringCell(opinion.m_createDate));
            List<GridCell> cells = row.GetCells();
            int cellsSize = cells.Count;
            for (int j = 1; j < cellsSize; j++)
            {
                cells[j].AllowEdit = true;
            }
        }

        /// <summary>
        /// 绑定线索
        /// </summary>
        private void BindClues()
        {
            m_gridOpinions.CellEditMode = GridCellEditMode.DoubleClick;
            List<OpinionInfo> clues = DataCenter.OpinionService.m_opinions;
            int opinionsSize = clues.Count;
            m_gridOpinions.ClearRows();
            m_gridOpinions.BeginUpdate();
            for (int i = 0; i < opinionsSize; i++)
            {
                OpinionInfo opinion = clues[i];
                AddOpinionToGrid(opinion);
            }
            m_gridOpinions.EndUpdate();
            m_gridOpinions.Invalidate();
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
            List<GridRow> selectedRows = m_gridOpinions.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.OpinionService.Delete(pID);
                    m_gridOpinions.RemoveRow(deleteRow);
                    m_gridOpinions.Update();
                    m_gridOpinions.Invalidate();
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
                OpinionInfo opinion = DataCenter.OpinionService.GetOpinion(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    opinion.m_level = cellValue;
                }
                else if (colName == "colP3")
                {
                    opinion.m_title = cellValue;
                }
                else if (colName == "colP4")
                {
                    opinion.m_content = cellValue;
                }
                else if (colName == "colP5")
                {
                    opinion.m_createDate = cellValue;
                }
                DataCenter.OpinionService.Save(opinion);
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
