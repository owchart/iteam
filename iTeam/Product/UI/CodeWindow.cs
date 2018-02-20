/*****************************************************************************\
*                                                                             *
* CodeWindow.cs - Code window functions, types                      *
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
    /// 行动代号窗体
    /// </summary>
    public class CodeWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public CodeWindow(INativeBase native)
        {
            Load(native, "CodeWindow", "codeWindow");
            RegisterEvents(m_window);
            m_gridCodes = GetGrid("gridCodes");
            m_gridCodes.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindCodes();
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridCodes;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            CodeInfo code = new CodeInfo();
            code.m_ID = DataCenter.CodeService.GetNewID();
            DataCenter.CodeService.Save(code);
            AddCodeToGrid(code);
            m_gridCodes.Update();
            if (m_gridCodes.VScrollBar != null)
            {
                m_gridCodes.VScrollBar.ScrollToEnd();
            }
            m_gridCodes.Invalidate();
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="code">信息</param>
        public void AddCodeToGrid(CodeInfo code)
        {
            List<GridRow> rows = m_gridCodes.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == code.m_ID)
                {
                    findRow.GetCell("colP4").SetString(code.m_content);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridCodes.AddRow(row);
            row.AddCell("colP1", new GridStringCell(code.m_ID));
            row.AddCell("colP2", new GridStringCell(code.m_content));
            List<GridCell> cells = row.GetCells();
            int cellsSize = cells.Count;
            for (int j = 1; j < cellsSize; j++)
            {
                cells[j].AllowEdit = true;
            }
        }

        /// <summary>
        /// 绑定信息
        /// </summary>
        private void BindCodes()
        {
            m_gridCodes.CellEditMode = GridCellEditMode.DoubleClick;
            List<CodeInfo> codes = DataCenter.CodeService.m_codes;
            int codesSize = codes.Count;
            m_gridCodes.ClearRows();
            m_gridCodes.BeginUpdate();
            for (int i = 0; i < codesSize; i++)
            {
                CodeInfo code = codes[i];
                AddCodeToGrid(code);
            }
            m_gridCodes.EndUpdate();
            m_gridCodes.Invalidate();
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
                    ExportToExcel("行动代号.xls", m_gridCodes);
                }
                else if (name == "btnExportTxt")
                {
                    ExportToTxt("行动代号.txt", m_gridCodes);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridCodes.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.CodeService.Delete(pID);
                    m_gridCodes.RemoveRow(deleteRow);
                    m_gridCodes.Update();
                    m_gridCodes.Invalidate();
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
                CodeInfo code = DataCenter.CodeService.GetCode(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    code.m_content = cellValue;
                }
                DataCenter.CodeService.Save(code);
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
