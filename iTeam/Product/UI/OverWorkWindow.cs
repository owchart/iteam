/*****************************************************************************\
*                                                                             *
* OverWorkWindow.cs - Overwork window functions, types                      *
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
    /// 加班操作窗体
    /// </summary>
    public class OverWorkWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public OverWorkWindow(INativeBase native)
        {
            Load(native, "OverWorkWindow", "overWorkWindow");
            RegisterEvents(m_window);
            m_gridOverWorks = GetGrid("gridOverWorks");
            m_gridOverWorks.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            m_gridOverWorks.RegisterEvent(new GridCellMouseEvent(GridCellClick), EVENTID.GRIDCELLCLICK);
            BindOverWorks();
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridOverWorks;

        /// <summary>
        /// 选择员工窗体
        /// </summary>
        private SelectStaffWindow m_selectStaffWindow;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            OverWorkService overWorkService = DataCenter.OverWorkService;
            OverWorkInfo overWork = new OverWorkInfo();
            overWork.m_ID = overWorkService.GetNewID();
            overWorkService.Save(overWork);
            AddOverWorkToGrid(overWork);
            m_gridOverWorks.Update();
            if (m_gridOverWorks.VScrollBar != null)
            {
                m_gridOverWorks.VScrollBar.ScrollToEnd();
            }
            m_gridOverWorks.Invalidate();
        }

        /// <summary>
        /// 添加加班信息
        /// </summary>
        /// <param name="overWork">信息</param>
        public void AddOverWorkToGrid(OverWorkInfo overWork)
        {
            StaffService staffService = DataCenter.StaffService;
            StaffInfo staff = staffService.GetStaff(overWork.m_jobID);
            List<GridRow> rows = m_gridOverWorks.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == overWork.m_ID)
                {
                    findRow.GetCell("colP2").SetString(overWork.m_jobID);
                    findRow.GetCell("colP3").SetString(staffService.GetNamesByJobsID(overWork.m_jobID));
                    findRow.GetCell("colP4").SetString(overWork.m_content);
                    findRow.GetCell("colP5").SetString(overWork.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridOverWorks.AddRow(row);
            row.AddCell("colP1", new GridStringCell(overWork.m_ID));
            row.AddCell("colP2", new GridStringCell(overWork.m_jobID));
            row.AddCell("colP3", new GridStringCell(staffService.GetNamesByJobsID(overWork.m_jobID)));
            row.AddCell("colP4", new GridStringCell(overWork.m_content));
            row.AddCell("colP5", new GridStringCell(overWork.m_createDate));
            List<GridCell> cells = row.GetCells();
            int cellsSize = cells.Count;
            for (int j = 1; j < cellsSize; j++)
            {
                if (cells[j].Column.Name != "colP2" && cells[j].Column.Name != "colP3")
                {
                    cells[j].AllowEdit = true;
                }
            }
        }

        /// <summary>
        /// 绑定员工
        /// </summary>
        private void BindOverWorks()
        {
            m_gridOverWorks.CellEditMode = GridCellEditMode.DoubleClick;
            List<OverWorkInfo> overWorks = DataCenter.OverWorkService.m_overWorks;
            int overWorksSize = overWorks.Count;
            m_gridOverWorks.ClearRows();
            m_gridOverWorks.BeginUpdate();
            for (int i = 0; i < overWorksSize; i++)
            {
                OverWorkInfo overWork = overWorks[i];
                AddOverWorkToGrid(overWork);
            }
            m_gridOverWorks.EndUpdate();
            m_gridOverWorks.Invalidate();
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
                    ExportToExcel("加班记录.xls", m_gridOverWorks);
                }
                else if (name == "btnExportTxt")
                {
                    ExportToTxt("加班记录.txt", m_gridOverWorks);
                }
                else if (name == "btnExportPMTxt")
                {
                    List<GridRow> rows = m_gridOverWorks.m_rows;
                    int rowsSize = rows.Count;
                    Dictionary<String, int> works = new Dictionary<String, int>();
                    for (int i = 0; i < rowsSize; i++)
                    {
                        GridRow row = rows[i];
                        String members = row.GetCell(2).GetString();
                        String[] strs = members.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        int strsSize = strs.Length;
                        for (int j = 0; j < strsSize; j++)
                        {
                            if (works.ContainsKey(strs[j]))
                            {
                                works[strs[j]] = works[strs[j]] + 1;
                            }
                            else
                            {
                                works[strs[j]] = 1;
                            }
                        }
                    }
                    List<OverWorkData> overWorkDatas = new List<OverWorkData>();
                    foreach (String key in works.Keys)
                    {
                        OverWorkData data = new OverWorkData();
                        data.m_name = key;
                        data.m_times = works[key];
                        overWorkDatas.Add(data);
                    }
                    overWorkDatas.Sort(new OverWorkDataCompare());
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("总:" + rowsSize + "天");
                    foreach (OverWorkData overWorkData in overWorkDatas)
                    {
                        sb.AppendLine(overWorkData.m_name + ":" + overWorkData.m_times + "次");
                    }
                    DataCenter.ExportService.ExportHtmlToTxt("加班排名.txt", sb.ToString());
                }
            }
        }

        /// <summary>
        /// 加班数据
        /// </summary>
        public class OverWorkData
        {
            /// <summary>
            /// 姓名
            /// </summary>
            public String m_name;

            /// <summary>
            /// 次数
            /// </summary>
            public int m_times;
        }

        /// <summary>
        /// 加班数据比较
        /// </summary>
        public class OverWorkDataCompare : IComparer<OverWorkData>
        {
            public int Compare(OverWorkData x, OverWorkData y)
            {
                return y.m_times.CompareTo(x.m_times);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridOverWorks.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.OverWorkService.Delete(pID);
                    m_gridOverWorks.RemoveRow(deleteRow);
                    m_gridOverWorks.Update();
                    m_gridOverWorks.Invalidate();
                }
            }
        }

        /// <summary>
        /// 单元格点击方法
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="cell">单元格</param>
        /// <param name="mp">坐标</param>
        /// <param name="button">按钮</param>
        /// <param name="clicks">点击次数</param>
        /// <param name="delta">滚轮值</param>
        private void GridCellClick(object sender, GridCell cell, POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            String colName = cell.Column.Name;
            if (colName == "colP2" || colName == "colP3")
            {
                m_selectStaffWindow = new SelectStaffWindow(Native);
                m_selectStaffWindow.Parent = this;
                m_selectStaffWindow.BindJobIdsToResultGrid(cell.Row.GetCell("colP2").GetString());
                m_selectStaffWindow.ShowDialog();
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
                OverWorkInfo overWork = DataCenter.OverWorkService.GetOverWork(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP4")
                {
                    overWork.m_content = cellValue;
                }
                else if (colName == "colP5")
                {
                    overWork.m_createDate = cellValue;
                }
                DataCenter.OverWorkService.Save(overWork);
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

        /// <summary>
        /// 是否确认关闭
        /// </summary>
        /// <returns>不处理</returns>
        public override int Submit()
        {
            if (m_selectStaffWindow != null)
            {
                OverWorkService overWorkService = DataCenter.OverWorkService;
                List<GridRow> selectedRows = m_gridOverWorks.SelectedRows;
                int selectedRowsSize = selectedRows.Count;
                if (selectedRowsSize > 0)
                {
                    String newJobID = m_selectStaffWindow.GetSelectedJobIDs();
                    OverWorkInfo overWork = overWorkService.GetOverWork(selectedRows[0].GetCell("colP1").GetString());
                    overWork.m_jobID = newJobID;
                    overWorkService.Save(overWork);
                    AddOverWorkToGrid(overWork);
                }
                m_selectStaffWindow = null;
            }
            return 0;
        }
    }
}
