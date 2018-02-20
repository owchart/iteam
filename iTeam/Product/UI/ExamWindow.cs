/*****************************************************************************\
*                                                                             *
* ReportWindow.cs - Report window functions, types                      *
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
    /// 汇报窗体
    /// </summary>
    public class ExamWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public ExamWindow(INativeBase native)
        {
            Load(native, "ExamWindow", "examWindow");
            RegisterEvents(m_window);
            m_gridExams = GetGrid("gridExams");
            m_gridExams.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindMasters();
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridExams;

        /// <summary>
        /// 随机数
        /// </summary>
        private Random m_rd = new Random();

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            ExamInfo exam = new ExamInfo();
            exam.m_ID = DataCenter.ExamService.GetNewID();
            DataCenter.ExamService.Save(exam);
            AddExamToGrid(exam);
            m_gridExams.Update();
            if (m_gridExams.VScrollBar != null)
            {
                m_gridExams.VScrollBar.ScrollToEnd();
            }
            m_gridExams.Invalidate();
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="report">信息</param>
        public void AddExamToGrid(ExamInfo report)
        {
            List<GridRow> rows = m_gridExams.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == report.m_ID)
                {
                    findRow.GetCell("colP2").SetString(report.m_title);
                    findRow.GetCell("colP3").SetString(report.m_type);
                    findRow.GetCell("colP4").SetString(report.m_interval);
                    findRow.GetCell("colP5").SetString(report.m_answer);
                    findRow.GetCell("colP6").SetString(report.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridExams.AddRow(row);
            row.AddCell("colP1", new GridStringCell(report.m_ID));
            row.AddCell("colP2", new GridStringCell(report.m_title));
            row.AddCell("colP3", new GridStringCell(report.m_type));
            row.AddCell("colP4", new GridStringCell(report.m_interval));
            row.AddCell("colP5", new GridStringCell(report.m_answer));
            row.AddCell("colP6", new GridStringCell(report.m_createDate));
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
        private void BindMasters()
        {
            m_gridExams.CellEditMode = GridCellEditMode.DoubleClick;
            List<ExamInfo> masters = DataCenter.ExamService.m_exams;
            int mastersSize = masters.Count;
            m_gridExams.ClearRows();
            m_gridExams.BeginUpdate();
            for (int i = 0; i < mastersSize; i++)
            {
                ExamInfo report = masters[i];
                AddExamToGrid(report);
            }
            m_gridExams.EndUpdate();
            m_gridExams.Invalidate();
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
                else if (name == "btnExportTxt")
                {
                    StringBuilder html = new StringBuilder();
                    List<GridRow> rows = m_gridExams.m_rows;
                    int rowsSize = rows.Count;
                    List<String> titles = new List<String>();
                    for (int i = 0; i < rowsSize; i++)
                    {
                        titles.Add(rows[i].GetCell("colP3").GetString() +"," + rows[i].GetCell("colP4").GetString()+ "," + rows[i].GetCell("colP2").GetString());
                    }
                    int titlesSize = titles.Count;
                    for (int i = 0; i < titlesSize; i++)
                    {
                        html.Append(titles[i]);
                        if (i != titlesSize - 1)
                        {
                            html.Append("\r\n");
                        }
                    }
                    DataCenter.ExportService.ExportHtmlToTxt("Exam.txt", html.ToString());
                }
                else if (name == "btnExportTxt2")
                {
                    StringBuilder html = new StringBuilder();
                    List<GridRow> rows = m_gridExams.m_rows;
                    int rowsSize = rows.Count;
                    List<String> titles = new List<String>();
                    for (int i = 0; i < rowsSize; i++)
                    {
                        titles.Add((i+1).ToString() + rows[i].GetCell("colP2").GetString() + "\r\n答:" + rows[i].GetCell("colP5").GetString() + "\r\n");
                    }
                    int titlesSize = titles.Count;
                    for (int i = 0; i < titlesSize; i++)
                    {
                        html.Append(titles[i]);
                    }
                    DataCenter.ExportService.ExportHtmlToTxt("Answer.txt", html.ToString());
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridExams.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                GridRow deleteRow = selectedRows[0];
                String pID = deleteRow.GetCell("colP1").GetString();
                DataCenter.ExamService.Delete(pID);
                m_gridExams.RemoveRow(deleteRow);
                m_gridExams.Update();
                m_gridExams.Invalidate();
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
                ExamInfo report = DataCenter.ExamService.GetExam(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    report.m_title = cellValue;
                }
                else if (colName == "colP3")
                {
                    report.m_type = cellValue;
                }
                else if (colName == "colP4")
                {
                    report.m_interval = cellValue;
                }
                else if (colName == "colP5")
                {
                    report.m_answer = cellValue;
                }
                else if (colName == "colP6")
                {
                    report.m_createDate = cellValue;
                }
                DataCenter.ExamService.Save(report);
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
