/*****************************************************************************\
*                                                                             *
* SpeechWindow.cs - Speech window functions, types                      *
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
    /// 线索窗体
    /// </summary>
    public class SpeechWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public SpeechWindow(INativeBase native)
        {
            Load(native, "SpeechWindow", "speechWindow");
            RegisterEvents(m_window);
            m_gridSpeeches = GetGrid("gridSpeeches");
            m_gridSpeeches.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindSpeeches();
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridSpeeches;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            SpeechInfo speechInfo = new SpeechInfo();
            speechInfo.m_ID = DataCenter.SpeechService.GetNewID();
            DataCenter.SpeechService.Save(speechInfo);
            AddSpeechToGrid(speechInfo);
            m_gridSpeeches.Update();
            if (m_gridSpeeches.VScrollBar != null)
            {
                m_gridSpeeches.VScrollBar.ScrollToEnd();
            }
            m_gridSpeeches.Invalidate();
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="speechInfo">信息</param>
        public void AddSpeechToGrid(SpeechInfo speechInfo)
        {
            List<GridRow> rows = m_gridSpeeches.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == speechInfo.m_ID)
                {
                    findRow.GetCell("colP2").SetString(speechInfo.m_level);
                    findRow.GetCell("colP3").SetString(speechInfo.m_title);
                    findRow.GetCell("colP4").SetString(speechInfo.m_content);
                    findRow.GetCell("colP5").SetString(speechInfo.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridSpeeches.AddRow(row);
            row.AddCell("colP1", new GridStringCell(speechInfo.m_ID));
            row.AddCell("colP2", new GridStringCell(speechInfo.m_level));
            row.AddCell("colP3", new GridStringCell(speechInfo.m_title));
            row.AddCell("colP4", new GridStringCell(speechInfo.m_content));
            row.AddCell("colP5", new GridStringCell(speechInfo.m_createDate));
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
        private void BindSpeeches()
        {
            m_gridSpeeches.CellEditMode = GridCellEditMode.DoubleClick;
            List<SpeechInfo> speeches = DataCenter.SpeechService.m_speeches;
            int speechesSize = speeches.Count;
            m_gridSpeeches.ClearRows();
            m_gridSpeeches.BeginUpdate();
            for (int i = 0; i < speechesSize; i++)
            {
                SpeechInfo speechInfo = speeches[i];
                AddSpeechToGrid(speechInfo);
            }
            m_gridSpeeches.EndUpdate();
            m_gridSpeeches.Invalidate();
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
            List<GridRow> selectedRows = m_gridSpeeches.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.SpeechService.Delete(pID);
                    m_gridSpeeches.RemoveRow(deleteRow);
                    m_gridSpeeches.Update();
                    m_gridSpeeches.Invalidate();
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
                SpeechInfo speechInfo = DataCenter.SpeechService.GetSpeech(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    speechInfo.m_level = cellValue;
                }
                else if (colName == "colP3")
                {
                    speechInfo.m_title = cellValue;
                }
                else if (colName == "colP4")
                {
                    speechInfo.m_content = cellValue;
                }
                else if (colName == "colP5")
                {
                    speechInfo.m_createDate = cellValue;
                }
                DataCenter.SpeechService.Save(speechInfo);
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
