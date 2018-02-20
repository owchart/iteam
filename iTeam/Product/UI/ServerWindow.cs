/*****************************************************************************\
*                                                                             *
* ServerWindow.cs - Server window functions, types                      *
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
    /// 服务器操作窗体
    /// </summary>
    public class ServerWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public ServerWindow(INativeBase native)
        {
            Load(native, "ServerWindow", "serverWindow");
            RegisterEvents(m_window);
            m_gridServers = GetGrid("gridServers");
            m_gridServers.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindServers();
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridServers;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            ServerInfo server = new ServerInfo();
            server.m_ID = DataCenter.ServerService.GetNewID();
            DataCenter.ServerService.Save(server);
            AddServerToGrid(server);
            m_gridServers.Update();
            if (m_gridServers.VScrollBar != null)
            {
                m_gridServers.VScrollBar.ScrollToEnd();
            }
            m_gridServers.Invalidate();
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="server">信息</param>
        public void AddServerToGrid(ServerInfo server)
        {
            List<GridRow> rows = m_gridServers.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == server.m_ID)
                {
                    findRow.GetCell("colP2").SetString(server.m_name);
                    findRow.GetCell("colP3").SetString(server.m_IP);
                    findRow.GetCell("colP4").SetString(server.m_userName);
                    findRow.GetCell("colP5").SetString(server.m_password);
                    findRow.GetCell("colP6").SetString(server.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridServers.AddRow(row);
            row.AddCell("colP1", new GridStringCell(server.m_ID));
            row.AddCell("colP2", new GridStringCell(server.m_name));
            row.AddCell("colP3", new GridStringCell(server.m_IP));
            row.AddCell("colP4", new GridStringCell(server.m_userName));
            row.AddCell("colP5", new GridStringCell(server.m_password));
            row.AddCell("colP6", new GridStringCell(server.m_createDate));
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
            m_gridServers.CellEditMode = GridCellEditMode.DoubleClick;
            List<ServerInfo> servers = DataCenter.ServerService.m_servers;
            int serversSize = servers.Count;
            m_gridServers.ClearRows();
            m_gridServers.BeginUpdate();
            for (int i = 0; i < serversSize; i++)
            {
                ServerInfo server = servers[i];
                AddServerToGrid(server);
            }
            m_gridServers.EndUpdate();
            m_gridServers.Invalidate();
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
                    ExportToExcel("服务器列表.xls", m_gridServers);
                }
                else if (name == "btnExportTxt")
                {
                    ExportToTxt("服务器列表.txt", m_gridServers);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridServers.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.ServerService.Delete(pID);
                    m_gridServers.RemoveRow(deleteRow);
                    m_gridServers.Update();
                    m_gridServers.Invalidate();
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
                ServerInfo server = DataCenter.ServerService.GerServer(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    server.m_name = cellValue;
                }
                else if (colName == "colP3")
                {
                    server.m_IP = cellValue;
                }
                else if (colName == "colP4")
                {
                    server.m_userName = cellValue;
                }
                else if (colName == "colP5")
                {
                    server.m_password = cellValue;
                }
                else if (colName == "colP6")
                {
                    server.m_createDate = cellValue;
                }
                DataCenter.ServerService.Save(server);
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
