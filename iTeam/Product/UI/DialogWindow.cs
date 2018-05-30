/*****************************************************************************\
*                                                                             *
* LevelWindow.cs - Level window functions, types                      *
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
    /// 级别窗体
    /// </summary>
    public class DialogWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public DialogWindow(INativeBase native)
        {
            Load(native, "DialogWindow", "dialogWindow");
            RegisterEvents(m_window);
            m_divDialogs = GetDiv("divDialogs");
            m_divDialogs.RegisterEvent(new ControlPaintEvent(PaintDiv), EVENTID.PAINT);
            m_gridDialogs = GetGrid("gridDialogs");
            m_gridDialogs.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindDialogs();

            m_chartLevels = GetChart("chartLevels");
            m_chartLevels.HScalePixel = 20;
            m_chartLevels.HScaleFieldText = "日期";
            CTable dataSource = m_chartLevels.DataSource;
            dataSource.AddColumn(0);
            dataSource.AddColumn(1);
            CDiv div = m_chartLevels.AddDiv();
            div.BackColor = COLOR.ARGB(255, 255, 255);
            div.LeftVScale.ScaleColor = COLOR.ARGB(50, 105, 217);
            div.HScale.ScaleColor = COLOR.ARGB(50, 105, 217);
            div.HGrid.GridColor = COLOR.ARGB(50, 105, 217);
            div.HGrid.LineStyle = 2;
            PolylineShape ps = new PolylineShape();
            ps.FieldName = 0;
            ps.FieldText = "接待量";
            ps.FillColor = COLOR.ARGB(100, 50, 105, 217);
            ps.Color = COLOR.ARGB(50, 105, 217);
            div.AddShape(ps);
            PolylineShape psBottom = new PolylineShape();
            psBottom.FieldName = 1;
            div.AddShape(psBottom);
            BindChart();
        }

        private ChartA m_chartLevels;

        private DivA m_divDialogs;

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridDialogs;

        /// <summary>
        /// 系统颜色
        /// </summary>
        private long[] m_sysColors = new long[] { COLOR.ARGB(255,255,0), COLOR.ARGB(255, 0, 255),
            COLOR.ARGB(0, 255, 0), COLOR.ARGB(82, 255, 255), COLOR.ARGB(255, 82, 82) };

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            DialogInfo dialog = new DialogInfo();
            dialog.m_ID = DataCenter.DialogService.GetNewID();
            dialog.m_date = DateTime.Now.ToString("yyyyMMdd");
            DataCenter.DialogService.Save(dialog);
            AddDialogToGrid(dialog);
            m_gridDialogs.Update();
            if (m_gridDialogs.VScrollBar != null)
            {
                m_gridDialogs.VScrollBar.ScrollToEnd();
            }
            m_gridDialogs.Invalidate();
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="level">信息</param>
        public void AddDialogToGrid(DialogInfo level)
        {
            List<GridRow> rows = m_gridDialogs.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == level.m_ID)
                {
                    findRow.GetCell("colP2").SetString(level.m_name);
                    findRow.GetCell("colP3").SetString(level.m_date);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridDialogs.AddRow(row);
            row.AddCell("colP1", new GridStringCell(level.m_ID));
            row.AddCell("colP2", new GridStringCell(level.m_name));
            row.AddCell("colP3", new GridStringCell(level.m_date));
            List<GridCell> cells = row.GetCells();
            int cellsSize = cells.Count;
            for (int j = 1; j < cellsSize; j++)
            {
                cells[j].AllowEdit = true;
            }
        }

        /// <summary>
        /// 绑定图形
        /// </summary>
        private void BindChart()
        {
            m_chartLevels.Clear();
            CTable dataSource = m_chartLevels.DataSource;
            List<DialogInfo> dialogs = DataCenter.DialogService.m_dialogs;
            Dictionary<String, int> dialogsMap = new Dictionary<String, int>();
            int dialogsSize = dialogs.Count;
            for (int i = 0; i < dialogsSize; i++)
            {
                if (dialogs[i].m_date.Length == 8)
                {
                    if (dialogsMap.ContainsKey(dialogs[i].m_date))
                    {
                        dialogsMap[dialogs[i].m_date] = dialogsMap[dialogs[i].m_date] + 1;
                    }
                    else
                    {
                        dialogsMap[dialogs[i].m_date] = 1;
                    }

                }
            }
            List<DialogData> datas = new List<DialogData>();
            foreach (String key in dialogsMap.Keys)
            {
                DialogData data = new DialogData();
                data.m_name = key;
                data.m_times = dialogsMap[key];
                datas.Add(data);
            }
            datas.Sort(new DialogDataCompare2());
            int datasSize = datas.Count;
            for (int i = 0; i < datasSize; i++)
            {
                DialogData data = datas[i];
                int year = CStr.ConvertStrToInt(data.m_name.Substring(0, 4));
                int month = CStr.ConvertStrToInt(data.m_name.Substring(4, 2));
                int day = CStr.ConvertStrToInt(data.m_name.Substring(6, 2));
                DateTime dt = new DateTime(year, month, day);
                double pk = CStr.ConvertDateToNum(dt);
                dataSource.Set(pk, 0, data.m_times);
                dataSource.Set2(i, 1, 0);
            }
            m_chartLevels.Update();
            m_chartLevels.Invalidate();
        }

        /// <summary>
        /// 绑定指示
        /// </summary>
        private void BindDialogs()
        {
            m_gridDialogs.CellEditMode = GridCellEditMode.DoubleClick;
            List<DialogInfo> dialogs = DataCenter.DialogService.m_dialogs;
            int dialogsSize = dialogs.Count;
            m_gridDialogs.ClearRows();
            m_gridDialogs.BeginUpdate();
            for (int i = 0; i < dialogsSize; i++)
            {
                DialogInfo dialog = dialogs[i];
                dialog.m_date = dialog.m_date.Replace("-", "");
                AddDialogToGrid(dialog);
            }
            m_gridDialogs.EndUpdate();
            if (m_gridDialogs.VScrollBar != null && m_gridDialogs.VScrollBar.Visible)
            {
                m_gridDialogs.VScrollBar.ScrollToEnd();
            }
            m_gridDialogs.Invalidate();
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
            List<GridRow> selectedRows = m_gridDialogs.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.DialogService.Delete(pID);
                    m_gridDialogs.RemoveRow(deleteRow);
                    m_gridDialogs.Update();
                    m_gridDialogs.Invalidate();
                    m_divDialogs.Invalidate();
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
                DialogInfo dialog = DataCenter.DialogService.GetDialog(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    dialog.m_name = cellValue;
                }
                else if (colName == "colP3")
                {
                    dialog.m_date = cellValue;
                }
                DataCenter.DialogService.Save(dialog);
                BindChart();
                m_divDialogs.Invalidate();
            }
        }

        /// <summary>
        /// 加班数据
        /// </summary>
        public class DialogData
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
        public class DialogDataCompare : IComparer<DialogData>
        {
            public int Compare(DialogData x, DialogData y)
            {
                return y.m_times.CompareTo(x.m_times);
            }
        }

        /// <summary>
        /// 加班数据比较
        /// </summary>
        public class DialogDataCompare2 : IComparer<DialogData>
        {
            public int Compare(DialogData x, DialogData y)
            {
                return x.m_name.CompareTo(y.m_name);
            }
        }

        /// <summary>
        /// 绘图方法
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="paint">绘图</param>
        /// <param name="clipRect">裁剪区域</param>
        private void PaintDiv(object sender, CPaint paint, RECT clipRect)
        {
            List<DialogInfo> dialogs = DataCenter.DialogService.m_dialogs;
            int dialogsSize = dialogs.Count;
            Dictionary<String, int> names = new Dictionary<String, int>();
            for (int i = 0; i < dialogsSize; i++)
            {
                DialogInfo dialog = dialogs[i];
                if (names.ContainsKey(dialog.m_name))
                {
                    names[dialog.m_name] = names[dialog.m_name] + 1;
                }
                else
                {
                    names[dialog.m_name] = 1;
                }
            }
            List<DialogData> datas = new List<DialogData>();
            foreach (String key in names.Keys)
            {
                DialogData data = new DialogData();
                data.m_name = key;
                data.m_times = names[key];
                datas.Add(data);
            }
            datas.Sort(new DialogDataCompare());
            int width = m_divDialogs.Width, height = m_divDialogs.Height;
            int datasSize = datas.Count;
            if (datasSize > 0)
            {
                int paddingLeft = 50, paddingRight = 50, paddingTop = 20, paddingBottom = 20;
                int top = paddingTop;
                int pSize = (height - paddingTop - paddingBottom) / datasSize;
                double max = 0;
                for (int i = 0; i < datasSize; i++)
                {
                    DialogData data = datas[i];
                    if (i == 0)
                    {
                        max = data.m_times;
                    }
                    int wSize = (int)((width - paddingLeft - paddingRight) * data.m_times / max);
                    CDraw.DrawText(paint, data.m_name, COLOR.ARGB(0, 0, 0), m_divDialogs.Font, 5, top);
                    paint.FillGradientRect(m_sysColors[i % m_sysColors.Length], m_sysColors[(i + 1) % m_sysColors.Length], new RECT(paddingLeft, top + 2, paddingLeft + wSize, top + 12), 2, 0);
                    FONT font = new FONT("微软雅黑", 14, false, false, false);
                    CDraw.DrawText(paint, data.m_times.ToString(), COLOR.ARGB(0, 0, 0), font, paddingLeft + wSize, top);
                    top += pSize;
                }
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
