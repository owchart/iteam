/*****************************************************************************\
*                                                                             *
* PlanWindow.cs -  Plan functions, types, and definitions.                        *
*                                                                             *
*               Version 1.00  ����                                          *
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
    /// ����ϵͳ
    /// </summary>
    public class PlanWindow : UIXmlEx, IDisposable
    {
        #region Lord 2016/12/24
        /// <summary>
        /// �ƻ�������
        /// </summary>
        private GridA m_gridPlan;

        /// <summary>
        /// ������
        /// </summary>
        private INativeBase m_native;

        /// <summary>
        /// �ƻ�����
        /// </summary>
        private PlanService m_planService = DataCenter.PlanService;

        /// <summary>
        /// ��ʱ���
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
                        //����
                        case 2:
                            cell.SetString(plan.m_name);
                            break;
                        //����
                        case 3:
                            cell.SetString(plan.m_command);
                            break;
                        //״̬
                        case 4:
                            cell.SetString(plan.m_status);
                            GridCellStyle cellStyle = new GridCellStyle();
                            if (plan.m_status == "����")
                            {
                                cellStyle.ForeColor = CDraw.GetPriceColor(1, 2);
                            }
                            else if (plan.m_status == "����")
                            {
                                cellStyle.ForeColor = CDraw.GetPriceColor(2, 1);
                            }
                            cell.Style = cellStyle;
                            break;
                        //�´�ִ��ʱ��
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
                        //�ϴ�ִ��ʱ��
                        case 6:
                            cell.SetString(new DateTime(plan.m_lastTime).ToString());
                            break;
                        //�ϴν��
                        case 7:
                            cell.SetString(plan.m_lastResult);
                            break;
                        //���
                        case 8:
                            cell.SetString(plan.m_timeSpan.ToString());
                            break;
                        //����ʱ��
                        case 9:
                            cell.SetString(new DateTime(plan.m_createTime).ToString());
                            break;
                    }
                }
            }
            //����ѡ����
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
        /// ɾ������
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
        /// ��ȡѡ�еļƻ�ID
        /// </summary>
        /// <returns>�ƻ�ID</returns>
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
        /// ����XML
        /// </summary>
        /// <param name="xmlPath">XML·��</param>
        public override void Load(String xmlPath)
        {
            LoadFile(xmlPath, null);
            m_native = Native;
            m_gridPlan = GetGrid("gridPlan");
            RegisterEvents(m_native.GetControls()[0]);
        }

        /// ע���¼�
        /// </summary>
        /// <param name="control">�ؼ�</param>
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
                    column.Font = new FONT("΢���ź�", 20, false, false, false);
                    column.ForeColor = CDraw.PCOLORS_FORECOLOR;
                }
                else if (grid != null)
                {
                    grid.GridLineColor = COLOR.CONTROLBORDER;
                    grid.RowStyle.HoveredBackColor = CDraw.PCOLORS_HOVEREDROWCOLOR;
                    grid.RowStyle.SelectedBackColor = CDraw.PCOLORS_SELECTEDROWCOLOR;
                    grid.RowStyle.SelectedForeColor = CDraw.PCOLORS_FORECOLOR4;
                    grid.RowStyle.Font = new FONT("΢���ź�", 20, false, false, false);
                    GridRowStyle alternateRowStyle = new GridRowStyle();
                    alternateRowStyle.BackColor = CDraw.PCOLORS_ALTERNATEROWCOLOR;
                    alternateRowStyle.HoveredBackColor = CDraw.PCOLORS_HOVEREDROWCOLOR;
                    alternateRowStyle.SelectedBackColor = CDraw.PCOLORS_SELECTEDROWCOLOR;
                    alternateRowStyle.SelectedForeColor = CDraw.PCOLORS_FORECOLOR4;
                    alternateRowStyle.Font = new FONT("΢���ź�", 20, false, false, false);
                    grid.AlternateRowStyle = alternateRowStyle;
                    grid.UseAnimation = true;
                }
                RegisterEvents(controls[i]);
            }
        }

        /// <summary>
        /// ѡ�мƻ�
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
                    if (status == "����")
                    {
                        btnStart.Text = "��������";
                        btnStart.ForeColor = CDraw.PCOLORS_UPCOLOR;
                    }
                    else
                    {
                        btnStart.Text = "��������";
                        btnStart.ForeColor = CDraw.PCOLORS_DOWNCOLOR;
                    }
                    m_native.Invalidate();
                }
            }
        }

        /// <summary>
        /// ������������
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
                if (status == "����")
                {
                    m_planService.StopService(id);
                    btnStart.Text = "��������";
                    btnStart.ForeColor = CDraw.PCOLORS_DOWNCOLOR;
                }
                else
                {
                    m_planService.StartService(id);
                    btnStart.Text = "��������";
                    btnStart.ForeColor = CDraw.PCOLORS_UPCOLOR;
                }
                m_native.Invalidate();
            }
        }

        /// <summary>
        /// ������������
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
        /// ��ֹ��������
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
