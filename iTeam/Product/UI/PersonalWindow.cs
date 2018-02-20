/*****************************************************************************\
*                                                                             *
* PersonalWindow.cs - Personal window functions, types                      *
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
    /// 个人信息操作窗体
    /// </summary>
    public class PersonalWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public PersonalWindow(INativeBase native)
        {
            Load(native, "PersonalWindow", "personalWindow");
            RegisterEvents(m_window);
            m_gridPersonals = GetGrid("gridPersonals");
            m_gridPersonals.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            BindStaffs();
        }

        /// <summary>
        /// 信息表格
        /// </summary>
        private GridA m_gridPersonals;

        /// <summary>
        /// 绑定员工
        /// </summary>
        private void BindStaffs()
        {
            m_gridPersonals.CellEditMode = GridCellEditMode.DoubleClick;
            List<StaffInfo> staffs = DataCenter.StaffService.m_staffs;
            int staffsSize = staffs.Count;
            Dictionary<String, PersonalInfo> personalsMap = new Dictionary<String, PersonalInfo>();
            List<PersonalInfo> personals = DataCenter.PersonalService.m_personals;
            int personalsSize = personals.Count;
            for (int i = 0; i < personalsSize; i++)
            {
                PersonalInfo personal = personals[i];
                if (personal.m_jobID != null && personal.m_jobID.Length > 0)
                {
                    personalsMap[personal.m_jobID] = personal;
                }
            }
            m_gridPersonals.ClearRows();
            m_gridPersonals.BeginUpdate();
            for (int i = 0; i < staffsSize; i++)
            {
                StaffInfo staff = staffs[i];
                PersonalInfo personal = null;
                if (personalsMap.ContainsKey(staff.m_jobID))
                {
                    personal = personalsMap[staff.m_jobID];
                }
                else
                {
                    personal = new PersonalInfo();
                }
                GridRow row = new GridRow();
                m_gridPersonals.AddRow(row);
                row.AddCell("colS1", new GridStringCell(staff.m_jobID));
                row.AddCell("colS2", new GridStringCell(staff.m_name));
                row.AddCell("colS3", new GridStringCell(personal.m_marry));
                row.AddCell("colS4", new GridStringCell(personal.m_loan));
                row.AddCell("colS5", new GridStringCell(personal.m_house));
                row.AddCell("colS6", new GridStringCell(personal.m_parent));
                row.AddCell("colS7", new GridStringCell(personal.m_body));
                row.AddCell("colS8", new GridStringCell(personal.m_sb));
                row.AddCell("colS9", new GridStringCell(personal.m_eat));
                row.AddCell("colS10", new GridStringCell(personal.m_teach));
                row.AddCell("colS11", new GridStringCell(personal.m_pay));
                row.AddCell("colS12", new GridStringCell(personal.m_dress));
                row.AddCell("colS13", new GridStringCell(personal.m_cross));
                row.AddCell("colS14", new GridStringCell(personal.m_game));
                row.AddCell("colS15", new GridStringCell(personal.m_sports));
                row.AddCell("colS16", new GridStringCell(personal.m_pet));
                row.AddCell("colS17", new GridStringCell(personal.m_lead));
                row.AddCell("colS18", new GridStringCell(personal.m_gay));
                row.AddCell("colS19", new GridStringCell(personal.m_preference));
                row.AddCell("colS20", new GridStringCell(personal.m_oldWork));
                row.AddCell("colS21", new GridStringCell(personal.m_drink));
                row.AddCell("colS22", new GridStringCell(personal.m_smoke));
                List<GridCell> cells = row.GetCells();
                int cellsSize = cells.Count;
                for (int j = 2; j < cellsSize; j++)
                {
                    cells[j].AllowEdit = true;
                }
            }
            m_gridPersonals.EndUpdate();
            m_gridPersonals.Invalidate();
            personalsMap.Clear();
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
                PersonalInfo personal = DataCenter.PersonalService.GerPersonal(cell.Row.GetCell("colS1").GetString());
                personal.m_jobID = cell.Row.GetCell("colS1").GetString();
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colS3")
                {
                    personal.m_marry = cellValue;
                }
                else if (colName == "colS4")
                {
                    personal.m_loan = cellValue;
                }
                else if (colName == "colS5")
                {
                    personal.m_house = cellValue;
                }
                else if (colName == "colS6")
                {
                    personal.m_parent = cellValue;
                }
                else if (colName == "colS7")
                {
                    personal.m_body = cellValue;
                }
                else if (colName == "colS8")
                {
                    personal.m_sb = cellValue;
                }
                else if (colName == "colS9")
                {
                    personal.m_eat = cellValue;
                }
                else if (colName == "colS10")
                {
                    personal.m_teach = cellValue;
                }
                else if (colName == "colS11")
                {
                    personal.m_pay = cellValue;
                }
                else if (colName == "colS12")
                {
                    personal.m_dress = cellValue;
                }
                else if (colName == "colS13")
                {
                    personal.m_cross = cellValue;
                }
                else if (colName == "colS14")
                {
                    personal.m_game = cellValue;
                }
                else if (colName == "colS15")
                {
                    personal.m_sports = cellValue;
                }
                else if (colName == "colS16")
                {
                    personal.m_pet = cellValue;
                }
                else if (colName == "colS17")
                {
                    personal.m_lead = cellValue;
                }
                else if (colName == "colS18")
                {
                    personal.m_gay = cellValue;
                }
                else if (colName == "colS19")
                {
                    personal.m_preference = cellValue;
                }
                else if (colName == "colS20")
                {
                    personal.m_oldWork = cellValue;
                }
                else if (colName == "colS21")
                {
                    personal.m_drink = cellValue;
                }
                else if (colName == "colS22")
                {
                    personal.m_smoke = cellValue;
                }
                DataCenter.PersonalService.Save(personal);
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
