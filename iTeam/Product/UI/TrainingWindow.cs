/*****************************************************************************\
*                                                                             *
* TrainingWindow.cs - Training window functions, types                      *
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
    public class TrainingWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public TrainingWindow(INativeBase native)
        {
            Load(native, "TrainingWindow", "trainingWindow");
            RegisterEvents(m_window);
            m_gridTrainings = GetGrid("gridTrainings");
            m_gridTrainings.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindTrainings();
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridTrainings;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            TrainingInfo training = new TrainingInfo();
            training.m_ID = DataCenter.TrainingService.GetNewID();
            DataCenter.TrainingService.Save(training);
            AddTrainingsToGrid(training);
            m_gridTrainings.Update();
            if (m_gridTrainings.VScrollBar != null)
            {
                m_gridTrainings.VScrollBar.ScrollToEnd();
            }
            m_gridTrainings.Invalidate();
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="training">信息</param>
        public void AddTrainingsToGrid(TrainingInfo training)
        {
            List<GridRow> rows = m_gridTrainings.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == training.m_ID)
                {
                    findRow.GetCell("colP2").SetString(training.m_training1);
                    findRow.GetCell("colP3").SetString(training.m_training2);
                    findRow.GetCell("colP4").SetString(training.m_training3);
                    findRow.GetCell("colP5").SetString(training.m_training4);
                    findRow.GetCell("colP6").SetString(training.m_training5);
                    findRow.GetCell("colP7").SetString(training.m_training6);
                    findRow.GetCell("colP8").SetString(training.m_training7);
                    findRow.GetCell("colP9").SetString(training.m_training8);
                    findRow.GetCell("colP10").SetString(training.m_training9);
                    findRow.GetCell("colP11").SetString(training.m_training10);
                    findRow.GetCell("colP12").SetString(training.m_training11);
                    findRow.GetCell("colP13").SetString(training.m_training12);
                    findRow.GetCell("colP14").SetString(training.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridTrainings.AddRow(row);
            row.AddCell("colP1", new GridStringCell(training.m_ID));
            row.AddCell("colP2", new GridStringCell(training.m_training1));
            row.AddCell("colP3", new GridStringCell(training.m_training2));
            row.AddCell("colP4", new GridStringCell(training.m_training3));
            row.AddCell("colP5", new GridStringCell(training.m_training4));
            row.AddCell("colP6", new GridStringCell(training.m_training5));
            row.AddCell("colP7", new GridStringCell(training.m_training6));
            row.AddCell("colP8", new GridStringCell(training.m_training7));
            row.AddCell("colP9", new GridStringCell(training.m_training8));
            row.AddCell("colP10", new GridStringCell(training.m_training9));
            row.AddCell("colP11", new GridStringCell(training.m_training10));
            row.AddCell("colP12", new GridStringCell(training.m_training11));
            row.AddCell("colP13", new GridStringCell(training.m_training12));
            row.AddCell("colP14", new GridStringCell(training.m_createDate));
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
        private void BindTrainings()
        {
            m_gridTrainings.CellEditMode = GridCellEditMode.DoubleClick;
            List<TrainingInfo> servers = DataCenter.TrainingService.m_trainings;
            int serversSize = servers.Count;
            m_gridTrainings.ClearRows();
            m_gridTrainings.BeginUpdate();
            for (int i = 0; i < serversSize; i++)
            {
                TrainingInfo training = servers[i];
                AddTrainingsToGrid(training);
            }
            m_gridTrainings.EndUpdate();
            m_gridTrainings.Invalidate();
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
                    ExportToExcel("个人训练.xls", m_gridTrainings);
                }
                else if (name == "btnExportTxt")
                {
                    ExportToTxt("个人训练.txt", m_gridTrainings);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridTrainings.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.TrainingService.Delete(pID);
                    m_gridTrainings.RemoveRow(deleteRow);
                    m_gridTrainings.Update();
                    m_gridTrainings.Invalidate();
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
                TrainingInfo training = DataCenter.TrainingService.GetTraining(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    training.m_training1 = cellValue;
                }
                else if (colName == "colP3")
                {
                    training.m_training2 = cellValue;
                }
                else if (colName == "colP4")
                {
                    training.m_training3 = cellValue;
                }
                else if (colName == "colP5")
                {
                    training.m_training4 = cellValue;
                }
                else if (colName == "colP6")
                {
                    training.m_training5 = cellValue;
                }
                else if (colName == "colP7")
                {
                    training.m_training6 = cellValue;
                }
                else if (colName == "colP8")
                {
                    training.m_training7 = cellValue;
                }
                else if (colName == "colP9")
                {
                    training.m_training8 = cellValue;
                }
                else if (colName == "colP10")
                {
                    training.m_training9 = cellValue;
                }
                else if (colName == "colP11")
                {
                    training.m_training10 = cellValue;
                }
                else if (colName == "colP12")
                {
                    training.m_training11 = cellValue;
                }
                else if (colName == "colP13")
                {
                    training.m_training12 = cellValue;
                }
                else if (colName == "colP14")
                {
                    training.m_createDate = cellValue;
                }
                DataCenter.TrainingService.Save(training);
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
