/*****************************************************************************\
*                                                                             *
* CalendarWindow.cs - Calendar window functions, types                      *
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
    /// 日历窗体
    /// </summary>
    public class CalendarWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public CalendarWindow(INativeBase native)
        {
            Load(native, "CalendarWindow", "calendarWindow");
            RegisterEvents(m_window);
            m_calendar = FindControl("calendar") as CalendarA;
            m_calendar.RegisterEvent(new ControlEvent(SelectedTimeChanged), EVENTID.SELECTEDTIMECHANGED);
            m_calendar.TimeDiv.Height = 0;
            m_calendar.Update();
            DayDiv dayDiv = m_calendar.DayDiv;
            dayDiv.m_dayButtons.Clear();
            dayDiv.m_dayButtons_am.Clear();
            for (int i = 0; i < 42; i++)
            {
                DayButtonEx dayButton = new DayButtonEx(m_calendar);
                dayDiv.m_dayButtons.Add(dayButton);
                DayButtonEx dayButtonAm = new DayButtonEx(m_calendar);
                dayButtonAm.Visible = false;
                dayDiv.m_dayButtons_am.Add(dayButtonAm);
            }
            m_gridEvents = GetGrid("gridEvents");
            m_gridEvents.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindEvents();
        }

        /// <summary>
        /// 日历
        /// </summary>
        private CalendarA m_calendar;

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridEvents;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            CalendarService calendarService = DataCenter.CalendarService;
            EventInfo eventInfo = new EventInfo();
            calendarService.AddEvent(GetSelectedDay(), eventInfo);
            AddEventToGrid(eventInfo);
            m_gridEvents.Update();
            if (m_gridEvents.VScrollBar != null)
            {
                m_gridEvents.VScrollBar.ScrollToEnd();
            }
            Native.Invalidate();
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="eventInfo">事件</param>
        public void AddEventToGrid(EventInfo eventInfo)
        {
            List<GridRow> rows = m_gridEvents.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == eventInfo.m_eventID)
                {
                    findRow.GetCell("colP2").SetString(eventInfo.m_time);
                    findRow.GetCell("colP3").SetString(eventInfo.m_level);
                    findRow.GetCell("colP4").SetString(eventInfo.m_title);
                    findRow.GetCell("colP5").SetString(eventInfo.m_content);
                    findRow.GetCell("colP6").SetString(eventInfo.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridEvents.AddRow(row);
            row.AddCell("colP1", new GridStringCell(eventInfo.m_eventID));
            row.AddCell("colP2", new GridStringCell(eventInfo.m_time));
            row.AddCell("colP3", new GridStringCell(eventInfo.m_level));
            row.AddCell("colP4", new GridStringCell(eventInfo.m_title));
            row.AddCell("colP5", new GridStringCell(eventInfo.m_content));
            row.AddCell("colP6", new GridStringCell(eventInfo.m_createDate));
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
        private void BindEvents()
        {
            m_gridEvents.CellEditMode = GridCellEditMode.DoubleClick;
            List<EventInfo> events = DataCenter.CalendarService.GetEvents(GetSelectedDay());
            int eventsSize = events.Count;
            m_gridEvents.ClearRows();
            m_gridEvents.BeginUpdate();
            for (int i = 0; i < eventsSize; i++)
            {
                EventInfo eventInfo = events[i];
                AddEventToGrid(eventInfo);
            }
            m_gridEvents.EndUpdate();
            m_gridEvents.Invalidate();
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
                    ExportToExcel("日程安排.xls", m_gridEvents);
                }
                else if (name == "btnExportTxt")
                {
                    ExportToTxt("日程安排.txt", m_gridEvents);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridEvents.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String eventID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.CalendarService.RemoveEvent(GetSelectedDay(), eventID);
                    m_gridEvents.RemoveRow(deleteRow);
                    m_gridEvents.Update();
                }
            }
            Native.Invalidate();
        }

        /// <summary>
        /// 获取选中的日
        /// </summary>
        /// <returns>日期</returns>
        public String GetSelectedDay()
        {
            return String.Format("{0}{1}{2}", m_calendar.SelectedDay.Year, m_calendar.SelectedDay.Month, m_calendar.SelectedDay.Day);
        }

        // <summary>
        /// 单元格编辑结束事件
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="cell">单元格</param>
        private void GridCellEditEnd(object sender, GridCell cell)
        {
            if (cell != null)
            {
                EventInfo eventInfo = DataCenter.CalendarService.GetEvent(GetSelectedDay(), cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    eventInfo.m_time = cellValue;
                }
                else if (colName == "colP3")
                {
                    eventInfo.m_level = cellValue;
                }
                else if (colName == "colP4")
                {
                    eventInfo.m_title = cellValue;
                }
                else if (colName == "colP5")
                {
                    eventInfo.m_content = cellValue;
                }
                else if (colName == "colP6")
                {
                    eventInfo.m_createDate = cellValue;
                }
                DataCenter.CalendarService.ModifyEvent(GetSelectedDay(), eventInfo);
                Native.Invalidate();
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
        /// 选中日期改变事件
        /// </summary>
        /// <param name="sender">调用者</param>
        private void SelectedTimeChanged(object sender)
        {
            BindEvents();
        }
    }

    /// <summary>
    /// 日期按钮扩展
    /// </summary>
    public class DayButtonEx : DayButton
    {
        /// <summary>
        /// 创建按钮
        /// </summary>
        /// <param name="calendar"></param>
        public DayButtonEx(CalendarA calendar) : base(calendar)
        {
            
        }

        /// <summary>
        /// 获取要绘制的前景色
        /// </summary>
        /// <returns></returns>
        protected override long GetPaintingForeColor()
        {
            if (m_selected)
            {
                return COLOR.ARGB(255, 255, 255);
            }
            else
            {
                if (m_inThisMonth)
                {
                    return COLOR.CONTROLTEXT;
                }
                else
                {
                    return COLOR.DISABLEDCONTROLTEXT;
                }
            }
        }

        /// <summary>
        /// 重绘背景方法
        /// </summary>
        /// <param name="paint">绘图对象</param>
        /// <param name="clipRect">裁剪区域</param>
        public override void OnPaintBackGround(CPaint paint, RECT clipRect)
        {
            long backColor = GetPaintingBackColor();
            if (m_selected)
            {
                paint.FillRect(COLOR.ARGB(50, 105, 217), m_bounds);
            }
            else
            {
                if (m_inThisMonth)
                {
                    paint.FillGradientRect(CDraw.PCOLORS_BACKCOLOR, CDraw.PCOLORS_BACKCOLOR2, m_bounds, 0, 90);
                }
                else
                {
                    paint.FillRect(COLOR.CONTROL, m_bounds);
                }
            }
        }

        /// <summary>
        /// 重绘前景方法
        /// </summary>
        /// <param name="paint">绘图对象</param>
        /// <param name="clipRect">裁剪区域</param>
        public override void OnPaintForeground(CPaint paint, RECT clipRect)
        {
            if (m_day != null)
            {
                int width = m_bounds.right - m_bounds.left;
                int height = m_bounds.bottom - m_bounds.top;
                String dayStr = m_day.Day.ToString();
                FONT font = new FONT("微软雅黑", 18, false, false, false);
                SIZE textSize = paint.TextSize(dayStr, font);
                RECT tRect = new RECT();
                tRect.left = m_bounds.left + (width - textSize.cx) / 2;
                tRect.top = m_bounds.top + (height - textSize.cy) / 2;
                tRect.right = tRect.left + textSize.cx;
                tRect.bottom = tRect.top + textSize.cy;
                paint.DrawText(dayStr, GetPaintingForeColor(), font, tRect);
                String date = String.Format("{0}{1}{2}", m_day.Year, m_day.Month, m_day.Day);
                List<EventInfo> events = DataCenter.CalendarService.GetEvents(date);
                int eventsSize = events.Count;
                if (eventsSize > 0)
                {
                    String eventStr = eventsSize.ToString();
                    FONT eFont = new FONT("Arial", 16, false, false, true);
                    SIZE eSize = paint.TextSize(eventStr, eFont);
                    RECT eRect = new RECT();
                    eRect.left = m_bounds.left + (width - eSize.cx) / 2;
                    eRect.top = m_bounds.top + height - eSize.cy - 5;
                    eRect.right = eRect.left + eSize.cx;
                    eRect.bottom = eRect.top + eSize.cy;
                    RECT ellipseRect = new RECT();
                    ellipseRect.left = m_bounds.left + (width - (int)(eSize.cx * 1.5)) / 2;
                    ellipseRect.top = eRect.top + eSize.cy / 2 - 10;
                    ellipseRect.right = ellipseRect.left + (int)(eSize.cx * 1.5);
                    ellipseRect.bottom = ellipseRect.top + (int)(eSize.cx * 1.5);
                    paint.FillEllipse(COLOR.ARGB(255, 0, 0), ellipseRect);
                    paint.DrawText(eventsSize.ToString(), COLOR.ARGB(255, 255, 255), eFont, eRect);
                }
            }
        }
    }
}
