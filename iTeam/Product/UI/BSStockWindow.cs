/*****************************************************************************\
*                                                                             *
* BSStockWindow.cs - BSStock window functions, types                      *
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
    public class BSStockWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public BSStockWindow(INativeBase native)
        {
            Load(native, "BSStockWindow", "bsStockWindow");
            RegisterEvents(m_window);
            m_gridBSStocks = GetGrid("gridBSStocks");
            m_gridBSStocks.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindCodes();
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridBSStocks;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            BSStockInfo bsStock = new BSStockInfo();
            bsStock.m_ID = DataCenter.BSStockService.GetNewID();
            DataCenter.BSStockService.Save(bsStock);
            AddCodeToGrid(bsStock);
            m_gridBSStocks.Update();
            if (m_gridBSStocks.VScrollBar != null)
            {
                m_gridBSStocks.VScrollBar.ScrollToEnd();
            }
            m_gridBSStocks.Invalidate();
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="bsStock">信息</param>
        public void AddCodeToGrid(BSStockInfo bsStock)
        {
            List<GridRow> rows = m_gridBSStocks.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == bsStock.m_ID)
                {
                    findRow.GetCell("colP2").SetString(bsStock.m_code);
                    findRow.GetCell("colP3").SetString(bsStock.m_name);
                    findRow.GetCell("colP4").SetString(bsStock.m_buyPrice);
                    findRow.GetCell("colP5").SetString(bsStock.m_sellPrice);
                    findRow.GetCell("colP6").SetString(bsStock.m_qty);
                    findRow.GetCell("colP7").SetString(bsStock.m_profit);
                    findRow.GetCell("colP8").SetString(bsStock.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridBSStocks.AddRow(row);
            row.AddCell("colP1", new GridStringCell(bsStock.m_ID));
            row.AddCell("colP2", new GridStringCell(bsStock.m_code));
            row.AddCell("colP3", new GridStringCell(bsStock.m_name));
            row.AddCell("colP4", new GridStringCell(bsStock.m_buyPrice));
            row.AddCell("colP5", new GridStringCell(bsStock.m_sellPrice));
            row.AddCell("colP6", new GridStringCell(bsStock.m_qty));
            row.AddCell("colP7", new GridStringCell(bsStock.m_profit));
            row.AddCell("colP8", new GridStringCell(bsStock.m_createDate));
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
            m_gridBSStocks.CellEditMode = GridCellEditMode.DoubleClick;
            List<BSStockInfo> codes = DataCenter.BSStockService.m_bsStocks;
            int codesSize = codes.Count;
            m_gridBSStocks.ClearRows();
            m_gridBSStocks.BeginUpdate();
            for (int i = 0; i < codesSize; i++)
            {
                BSStockInfo bsStock = codes[i];
                AddCodeToGrid(bsStock);
            }
            m_gridBSStocks.EndUpdate();
            m_gridBSStocks.Invalidate();
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
                    ExportToExcel("股票买卖.xls", m_gridBSStocks);
                }
                else if (name == "btnExportTxt")
                {
                    ExportToTxt("股票买卖.txt", m_gridBSStocks);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridBSStocks.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.BSStockService.Delete(pID);
                    m_gridBSStocks.RemoveRow(deleteRow);
                    m_gridBSStocks.Update();
                    m_gridBSStocks.Invalidate();
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
                BSStockInfo bsStock = DataCenter.BSStockService.GetBSStock(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    bsStock.m_code = cellValue;
                }
                else if (colName == "colP3")
                {
                    bsStock.m_name = cellValue;
                }
                else if (colName == "colP4")
                {
                    bsStock.m_buyPrice = cellValue;
                }
                else if (colName == "colP5")
                {
                    bsStock.m_sellPrice = cellValue;
                }
                else if (colName == "colP6")
                {
                    bsStock.m_qty = cellValue;
                }
                else if (colName == "colP7")
                {
                    bsStock.m_profit = cellValue;
                }
                else if (colName == "colP8")
                {
                    bsStock.m_createDate = cellValue;
                }
                DataCenter.BSStockService.Save(bsStock);
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
