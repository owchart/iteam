/*****************************************************************************\
*                                                                             *
* RemoteWindow.cs - Remote window functions, types                      *
*                                                                             *
*               Version 1.00  ★                                              *
*                                                                             *
*               Copyright (c) 2017-2017, iTeam. All rights reserved.      *
*               Created by Todd 2017/10/4.                          *
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
    /// 远程服务窗体
    /// </summary>
    public class RemoteWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public RemoteWindow(INativeBase native)
        {
            Load(native, "RemoteWindow", "remoteWindow");
            RegisterEvents(m_window);
            m_gridRemotes = GetGrid("gridRemotes");
            m_gridRemotes.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindServers();
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridRemotes;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            RemoteInfo remote = new RemoteInfo();
            remote.m_ID = DataCenter.RemoteService.GetNewID();
            DataCenter.RemoteService.Save(remote);
            AddRemoteToGrid(remote);
            m_gridRemotes.Update();
            if (m_gridRemotes.VScrollBar != null)
            {
                m_gridRemotes.VScrollBar.ScrollToEnd();
            }
            m_gridRemotes.Invalidate();
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="remote">信息</param>
        public void AddRemoteToGrid(RemoteInfo remote)
        {
            List<GridRow> rows = m_gridRemotes.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == remote.m_ID)
                {
                    findRow.GetCell("colP2").SetString(remote.m_name);
                    findRow.GetCell("colP3").SetString(remote.m_IP);
                    findRow.GetCell("colP4").SetString(remote.m_port);
                    findRow.GetCell("colP5").SetString(remote.m_userName);
                    findRow.GetCell("colP6").SetString(remote.m_password);
                    findRow.GetCell("colP7").SetString(remote.m_remarks);
                    findRow.GetCell("colP8").SetString(remote.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridRemotes.AddRow(row);
            row.AddCell("colP1", new GridStringCell(remote.m_ID));
            row.AddCell("colP2", new GridStringCell(remote.m_name));
            row.AddCell("colP3", new GridStringCell(remote.m_IP));
            row.AddCell("colP4", new GridStringCell(remote.m_port));
            row.AddCell("colP5", new GridStringCell(remote.m_userName));
            row.AddCell("colP6", new GridStringCell(remote.m_password));
            row.AddCell("colP7", new GridStringCell(remote.m_remarks));
            row.AddCell("colP8", new GridStringCell(remote.m_createDate));
            List<GridCell> cells = row.GetCells();
            int cellsSize = cells.Count;
            for (int j = 1; j < cellsSize; j++)
            {
                cells[j].AllowEdit = true;
            }
        }

        /// <summary>
        /// 绑定服务器
        /// </summary>
        private void BindServers()
        {
            m_gridRemotes.CellEditMode = GridCellEditMode.DoubleClick;
            List<RemoteInfo> remotes = DataCenter.RemoteService.m_remotes;
            int serversSize = remotes.Count;
            m_gridRemotes.ClearRows();
            m_gridRemotes.BeginUpdate();
            for (int i = 0; i < serversSize; i++)
            {
                RemoteInfo remote = remotes[i];
                AddRemoteToGrid(remote);
            }
            m_gridRemotes.EndUpdate();
            m_gridRemotes.Invalidate();
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
                    ExportToExcel("远程服务.xls", m_gridRemotes);
                }
                else if (name == "btnExportTxt")
                {
                    ExportToTxt("远程服务.txt", m_gridRemotes);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridRemotes.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.RemoteService.Delete(pID);
                    m_gridRemotes.RemoveRow(deleteRow);
                    m_gridRemotes.Update();
                    m_gridRemotes.Invalidate();
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
                RemoteInfo remote = DataCenter.RemoteService.GetRemote(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    remote.m_name = cellValue;
                }
                else if (colName == "colP3")
                {
                    remote.m_IP = cellValue;
                }
                else if (colName == "colP4")
                {
                    remote.m_port = cellValue;
                }
                else if (colName == "colP5")
                {
                    remote.m_userName = cellValue;
                }
                else if (colName == "colP6")
                {
                    remote.m_password = cellValue;
                }
                else if (colName == "colP7")
                {
                    remote.m_remarks = cellValue;
                }
                else if (colName == "colP8")
                {
                    remote.m_createDate = cellValue;
                }
                DataCenter.RemoteService.Save(remote);
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
