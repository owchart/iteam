/*****************************************************************************\
*                                                                             *
* PlanWindow.cs -  Plan functions, types, and definitions.                        *
*                                                                             *
*               Version 1.00  ★★★                                          *
*                                                                             *
*               Copyright (c) 2016-2016, iTeam. All rights reserved.         *
*               Created by Todd 2016/12/24.                                   *
*                                                                             *
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using OwLib;
using System.Windows.Forms;
using System.Threading;

namespace OwLib
{
    /// <summary>
    /// 行情系统
    /// </summary>
    public class PlanWindow : UIXmlEx, IDisposable
    {
        #region Lord 2016/12/24
        /// <summary>
        /// 计划任务表格
        /// </summary>
        private GridA m_gridPlan;

        /// <summary>
        /// 方法库
        /// </summary>
        private INativeBase m_native;

        /// <summary>
        /// 计划服务
        /// </summary>
        private PlanService m_planService = DataCenter.PlanService;

        /// <summary>
        /// 定时检查
        /// </summary>
        public void Check()
        {
            m_planService.OnTimer();
            String newLog = m_planService.GetNewLogs();
            if (newLog != null && newLog.Length > 0)
            {
                TextBoxA txtLog = GetTextBox("txtLog");
                txtLog.Text += newLog;
                txtLog.Invalidate();
            }
            Dictionary<int, GridColumn> columnsIndex = new Dictionary<int, GridColumn>();
            List<GridColumn> columns = m_gridPlan.GetColumns();
            int columnsSize = columns.Count;
            for (int i = 0; i < columnsSize; i++)
            {
                GridColumn column = columns[i];
                columnsIndex[CStr.ConvertStrToInt(column.Name.Substring(4))] = column;
            }
            Dictionary<String, String> pids = new Dictionary<String, String>();
            List<CPlan> plans = new List<CPlan>();
            DataCenter.PlanService.GetPlans(plans);
            int plansSize = plans.Count;
            for (int i = 0; i < plansSize; i++)
            {
                pids[plans[i].m_id] = "";
            }
            GridRow selectedRow = null;
            Dictionary<String, GridRow> rowsMap = new Dictionary<String, GridRow>();
            List<GridRow> rows = m_gridPlan.GetRows();
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow row = rows[i];
                String id = "";
                if (row.GetCell("colP1") != null)
                {
                    id = row.GetCell("colP1").GetString();
                }
                if (pids.ContainsKey(id))
                {
                    rowsMap[id] = row;
                }
                else
                {
                    m_gridPlan.RemoveRow(row);
                    row.Dispose();
                    rowsSize--;
                    i--;
                }
            }
            m_gridPlan.Update();
            m_gridPlan.BeginUpdate();
            for (int i = 0; i < plansSize; i++)
            {
                CPlan plan = plans[i];
                GridRow row = null;
                bool newData = false;
                if (rowsMap.ContainsKey(plan.m_id))
                {
                    row = rowsMap[plan.m_id];
                }
                else
                {
                    row = new GridRow();
                    row.Height = 30;
                    selectedRow = row;
                    m_gridPlan.AddRow(row);
                    newData = true;
                }
                foreach (int col in columnsIndex.Keys)
                {
                    GridCell cell = null;
                    GridColumn column = columnsIndex[col];
                    if (newData)
                    {
                        if (col == 5)
                        {
                            GridProgressCell progressCell = new GridProgressCell();
                            cell = progressCell;
                            row.AddCell(column.Index, cell);
                        }
                        else
                        {
                            cell = new GridStringCell();
                            if (col == 3)
                            {
                                cell.AllowEdit = true;
                            }
                            row.AddCell(column.Index, cell);
                        }
                    }
                    else
                    {
                        cell = row.GetCell(column.Index);
                    }
                    switch (col)
                    {
                        //ID
                        case 1:
                            cell.SetString(plan.m_id);
                            break;
                        //名称
                        case 2:
                            cell.SetString(plan.m_name);
                            break;
                        //进程
                        case 3:
                            cell.SetString(plan.m_command);
                            break;
                        //状态
                        case 4:
                            cell.SetString(plan.m_status);
                            GridCellStyle cellStyle = new GridCellStyle();
                            if (plan.m_status == "启动")
                            {
                                cellStyle.ForeColor = CDraw.GetPriceColor(1, 2);
                            }
                            else if (plan.m_status == "禁用")
                            {
                                cellStyle.ForeColor = CDraw.GetPriceColor(2, 1);
                            }
                            cell.Style = cellStyle;
                            break;
                        //下次执行时间
                        case 5:
                            GridProgressCell progressCell = cell as GridProgressCell;
                            if (plan.m_nextTime != 0)
                            {
                                DateTime nowDate = DateTime.Now;
                                long span = (long)plan.m_timeSpan * 1000 * 10000;
                                double rate = 100 - 100 * (plan.m_nextTime - nowDate.Ticks) / span;
                                if (rate < 0)
                                {
                                    rate = 100 - 100 * (double)(plan.m_nextTime - nowDate.Ticks) / (plan.m_nextTime - plan.m_createTime);
                                }
                                progressCell.Rate = rate;
                            }
                            else
                            {
                                progressCell.Rate = 0;
                            }
                            cell.SetString(new DateTime(plan.m_nextTime).ToString());
                            break;
                        //上次执行时间
                        case 6:
                            cell.SetString(new DateTime(plan.m_lastTime).ToString());
                            break;
                        //上次结果
                        case 7:
                            cell.SetString(plan.m_lastResult);
                            break;
                        //间隔
                        case 8:
                            cell.SetString(plan.m_timeSpan.ToString());
                            break;
                        //创建时间
                        case 9:
                            cell.SetString(new DateTime(plan.m_createTime).ToString());
                            break;
                    }
                }
            }
            //修正选中行
            if (selectedRow != null)
            {
                List<GridRow> selectedRows = new List<GridRow>();
                selectedRows.Add(selectedRow);
                m_gridPlan.SelectedRows = selectedRows;

            }
            m_gridPlan.EndUpdate();
            m_native.Invalidate();
            columnsIndex.Clear();
            pids.Clear();
            plans.Clear();
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridPlan.SelectedRows;
            if (selectedRows.Count > 0)
            {
                m_planService.RemoveService(selectedRows[0].GetCell("colP1").GetString());
            }
        }

        /// <summary>
        /// 获取选中的计划ID
        /// </summary>
        /// <returns>计划ID</returns>
        public String GetSelectedPlanID()
        {
            List<GridRow> selectedRows = m_gridPlan.SelectedRows;
            if (selectedRows.Count > 0)
            {
                GridRow row = selectedRows[0];
                GridCell cell = row.GetCell("colP1");
                if (cell != null)
                {
                    return cell.GetString();
                }
            }
            return "";
        }

        /// <summary>
        /// 加载XML
        /// </summary>
        /// <param name="xmlPath">XML路径</param>
        public override void Load(String xmlPath)
        {
            LoadFile(xmlPath, null);
            m_native = Native;
            m_gridPlan = GetGrid("gridPlan");
            RegisterEvents(m_native.GetControls()[0]);
        }

        /// 注册事件
        /// </summary>
        /// <param name="control">控件</param>
        private void RegisterEvents(ControlA control)
        {
            List<ControlA> controls = control.GetControls();
            int controlsSize = controls.Count;
            for (int i = 0; i < controlsSize; i++)
            {
                ControlA subControl = controls[i];
                GridColumn column = subControl as GridColumn;
                GridA grid = subControl as GridA;
                CheckBoxA checkBox = subControl as CheckBoxA;
                if (column != null)
                {
                    column.AllowDrag = true;
                    column.AllowResize = true;
                    column.BackColor = CDraw.PCOLORS_BACKCOLOR;
                    column.Font = new FONT("微软雅黑", 20, false, false, false);
                    column.ForeColor = CDraw.PCOLORS_FORECOLOR;
                }
                else if (grid != null)
                {
                    grid.GridLineColor = COLOR.CONTROLBORDER;
                    grid.RowStyle.HoveredBackColor = CDraw.PCOLORS_HOVEREDROWCOLOR;
                    grid.RowStyle.SelectedBackColor = CDraw.PCOLORS_SELECTEDROWCOLOR;
                    grid.RowStyle.SelectedForeColor = CDraw.PCOLORS_FORECOLOR4;
                    grid.RowStyle.Font = new FONT("微软雅黑", 20, false, false, false);
                    GridRowStyle alternateRowStyle = new GridRowStyle();
                    alternateRowStyle.BackColor = CDraw.PCOLORS_ALTERNATEROWCOLOR;
                    alternateRowStyle.HoveredBackColor = CDraw.PCOLORS_HOVEREDROWCOLOR;
                    alternateRowStyle.SelectedBackColor = CDraw.PCOLORS_SELECTEDROWCOLOR;
                    alternateRowStyle.SelectedForeColor = CDraw.PCOLORS_FORECOLOR4;
                    alternateRowStyle.Font = new FONT("微软雅黑", 20, false, false, false);
                    grid.AlternateRowStyle = alternateRowStyle;
                    grid.UseAnimation = true;
                }
                RegisterEvents(controls[i]);
            }
        }

        /// <summary>
        /// 选中计划
        /// </summary>
        public void SelectPlan()
        {
            List<GridRow> selectedRows = m_gridPlan.SelectedRows;
            if (selectedRows.Count > 0)
            {
                GridRow row = selectedRows[0];
                if (row.GetCells().Count > 0)
                {
                    String status = row.GetCell("colP4").GetString();
                    ButtonA btnStart = GetButton("btnStart");
                    if (status == "启动")
                    {
                        btnStart.Text = "禁用任务";
                        btnStart.ForeColor = CDraw.PCOLORS_UPCOLOR;
                    }
                    else
                    {
                        btnStart.Text = "启动任务";
                        btnStart.ForeColor = CDraw.PCOLORS_DOWNCOLOR;
                    }
                    m_native.Invalidate();
                }
            }
        }

        /// <summary>
        /// 启动所有任务
        /// </summary>
        public void Start()
        {
            List<GridRow> selectedRows = m_gridPlan.SelectedRows;
            if (selectedRows.Count > 0)
            {
                GridRow row = selectedRows[0];
                String id = row.GetCell("colP1").GetString();
                String status = row.GetCell("colP4").GetString();
                ButtonA btnStart = GetButton("btnStart");
                if (status == "启动")
                {
                    m_planService.StopService(id);
                    btnStart.Text = "启动任务";
                    btnStart.ForeColor = CDraw.PCOLORS_DOWNCOLOR;
                }
                else
                {
                    m_planService.StartService(id);
                    btnStart.Text = "禁用任务";
                    btnStart.ForeColor = CDraw.PCOLORS_UPCOLOR;
                }
                m_native.Invalidate();
            }
        }

        /// <summary>
        /// 启动所有任务
        /// </summary>
        public void StartAll()
        {
            List<GridRow> rows = m_gridPlan.GetRows();
            int rowsSize = rows.Count;
            if (rowsSize > 0)
            {
                for (int i = 0; i < rowsSize; i++)
                {
                    m_planService.StartService(rows[i].GetCell("colP1").GetString());
                }
            }
        }

        /// <summary>
        /// 禁止所有任务
        /// </summary>
        public void StopAll()
        {
            List<GridRow> rows = m_gridPlan.GetRows();
            int rowsSize = rows.Count;
            if (rowsSize > 0)
            {
                for (int i = 0; i < rowsSize; i++)
                {
                    m_planService.StopService(rows[i].GetCell("colP1").GetString());
                }
            }
        }
        #endregion
    }
}
