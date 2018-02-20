/*****************************************************************************\
*                                                                             *
* BusinessCard.cs - Business card window functions, types                      *
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
    public class BusinessCardWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public BusinessCardWindow(INativeBase native)
        {
            Load(native, "BusinessCardWindow", "businessCardWindow");
            RegisterEvents(m_window);
            m_gridBusinessCards = GetGrid("gridBusinessCards");
            m_gridBusinessCards.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindCards();
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridBusinessCards;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            BusinessCardInfo card = new BusinessCardInfo();
            card.m_ID = DataCenter.BusinessCardService.GetNewID();
            DataCenter.BusinessCardService.Save(card);
            AddCardToGrid(card);
            m_gridBusinessCards.Update();
            if (m_gridBusinessCards.VScrollBar != null)
            {
                m_gridBusinessCards.VScrollBar.ScrollToEnd();
            }
            m_gridBusinessCards.Invalidate();
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="card">信息</param>
        public void AddCardToGrid(BusinessCardInfo card)
        {
            List<GridRow> rows = m_gridBusinessCards.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == card.m_ID)
                {
                    findRow.GetCell("colP2").SetString(card.m_name);
                    findRow.GetCell("colP3").SetString(card.m_phone);
                    findRow.GetCell("colP4").SetString(card.m_content);
                    findRow.GetCell("colP5").SetString(card.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridBusinessCards.AddRow(row);
            row.AddCell("colP1", new GridStringCell(card.m_ID));
            row.AddCell("colP2", new GridStringCell(card.m_name));
            row.AddCell("colP3", new GridStringCell(card.m_phone));
            row.AddCell("colP4", new GridStringCell(card.m_content));
            row.AddCell("colP5", new GridStringCell(card.m_createDate));
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
        private void BindCards()
        {
            m_gridBusinessCards.CellEditMode = GridCellEditMode.DoubleClick;
            List<BusinessCardInfo> cards = DataCenter.BusinessCardService.m_businessCards;
            int cardsSize = cards.Count;
            m_gridBusinessCards.ClearRows();
            m_gridBusinessCards.BeginUpdate();
            for (int i = 0; i < cardsSize; i++)
            {
                BusinessCardInfo card = cards[i];
                AddCardToGrid(card);
            }
            m_gridBusinessCards.EndUpdate();
            m_gridBusinessCards.Invalidate();
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
            List<GridRow> selectedRows = m_gridBusinessCards.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.BusinessCardService.Delete(pID);
                    m_gridBusinessCards.RemoveRow(deleteRow);
                    m_gridBusinessCards.Update();
                    m_gridBusinessCards.Invalidate();
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
                BusinessCardInfo card = DataCenter.BusinessCardService.GetCard(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    card.m_name = cellValue;
                }
                else if (colName == "colP3")
                {
                    card.m_phone = cellValue;
                }
                else if (colName == "colP4")
                {
                    card.m_content = cellValue;
                }
                else if (colName == "colP5")
                {
                    card.m_createDate = cellValue;
                }
                DataCenter.BusinessCardService.Save(card);
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
