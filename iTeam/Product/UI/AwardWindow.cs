/*****************************************************************************\
*                                                                             *
* AwardWindow.cs - Award window functions, types                      *
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
using System.Data;
using System.IO;

namespace OwLib
{
    /// <summary>
    /// 嘉奖操作窗体
    /// </summary>
    public class AwardWindow : WindowXmlEx
    {
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="native">方法库</param>
        public AwardWindow(INativeBase native)
        {
            Load(native, "AwardWindow", "awardWindow");
            RegisterEvents(m_window);
            m_gridAwards = GetGrid("gridAwards");
            m_gridAwards.RegisterEvent(new GridCellEvent(GridCellEditEnd), EVENTID.GRIDCELLEDITEND);
            m_gridAwards.RegisterEvent(new GridCellMouseEvent(GridCellClick), EVENTID.GRIDCELLCLICK);
            BindAwards();
        }

        /// <summary>
        /// 表格
        /// </summary>
        private GridA m_gridAwards;

        /// <summary>
        /// 选择员工窗体
        /// </summary>
        private SelectStaffWindow m_selectStaffWindow;

        /// <summary>
        /// 添加
        /// </summary>
        public void Add()
        {
            AwardService awardService = DataCenter.AwardService;
            AwardInfo award = new AwardInfo();
            award.m_ID = awardService.GetNewID();
            awardService.Save(award);
            AddAwardToGrid(award);
            m_gridAwards.Update();
            if (m_gridAwards.VScrollBar != null)
            {
                m_gridAwards.VScrollBar.ScrollToEnd();
            }
            m_gridAwards.Invalidate();
        }

        /// <summary>
        /// 添加嘉奖
        /// </summary>
        /// <param name="award">嘉奖</param>
        public void AddAwardToGrid(AwardInfo award)
        {
            StaffService staffService = DataCenter.StaffService;
            StaffInfo staff = staffService.GetStaff(award.m_name);
            List<GridRow> rows = m_gridAwards.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == award.m_ID)
                {
                    findRow.GetCell("colP2").SetString(award.m_name);
                    findRow.GetCell("colP3").SetString(award.m_title);
                    findRow.GetCell("colP4").SetString(award.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridAwards.AddRow(row);
            row.AddCell("colP1", new GridStringCell(award.m_ID));
            row.AddCell("colP2", new GridStringCell(award.m_name));
            row.AddCell("colP3", new GridStringCell(award.m_title));
            row.AddCell("colP4", new GridStringCell(award.m_createDate));
            List<GridCell> cells = row.GetCells();
            int cellsSize = cells.Count;
            for (int j = 1; j < cellsSize; j++)
            {
                cells[j].AllowEdit = true;
            }
        }

        /// <summary>
        /// 绑定员工
        /// </summary>
        private void BindAwards()
        {
            m_gridAwards.CellEditMode = GridCellEditMode.DoubleClick;
            List<AwardInfo> awards = DataCenter.AwardService.m_awards;
            int awardsSize = awards.Count;
            m_gridAwards.ClearRows();
            m_gridAwards.BeginUpdate();
            for (int i = 0; i < awardsSize; i++)
            {
                AwardInfo award = awards[i];
                AddAwardToGrid(award);
            }
            m_gridAwards.EndUpdate();
            m_gridAwards.Invalidate();
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
                else if (name == "btnImport")
                {
                    Import();
                }
            }
        }

        /// <summary>
        /// 加班数据
        /// </summary>
        public class AwardData
        {
            /// <summary>
            /// 姓名
            /// </summary>
            public String m_name;

            public int m_other1;
            public int m_other2;
            public int m_other3;
            public int m_other4;
            public int m_other5;

            /// <summary>
            /// 分数
            /// </summary>
            public int m_scores;

            /// <summary>
            /// 次数
            /// </summary>
            public int m_times;
        }

        /// <summary>
        /// 加班数据比较
        /// </summary>
        public class AwardDataCompare : IComparer<AwardData>
        {
            public int Compare(AwardData x, AwardData y)
            {
                return y.m_scores.CompareTo(x.m_scores);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            List<GridRow> selectedRows = m_gridAwards.SelectedRows;
            int selectedRowsSize = selectedRows.Count;
            if (selectedRowsSize > 0)
            {
                if (DialogResult.Yes == MessageBox.Show("是否确认删除该条信息?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    GridRow deleteRow = selectedRows[0];
                    String pID = deleteRow.GetCell("colP1").GetString();
                    DataCenter.AwardService.Delete(pID);
                    m_gridAwards.RemoveRow(deleteRow);
                    m_gridAwards.Update();
                    m_gridAwards.Invalidate();
                }
            }
        }

        /// <summary>
        /// 获取当前级别
        /// </summary>
        /// <param name="score">分数</param>
        /// <returns>级别</returns>
        private int GetLevel(int score)
        {
            if (score < 10)
            {
                return 1;
            }
            else if (score < 25)
            {
                return 2;
            }
            else if (score < 50)
            {
                return 3;
            }
            else if (score < 80)
            {
                return 4;
            }
            else if (score < 130)
            {
                return 5;
            }
            else if (score < 200)
            {
                return 6;
            }
            else if (score < 300)
            {
                return 7;
            }
            else if (score < 500)
            {
                return 8;
            }
            else if (score < 800)
            {
                return 9;
            }
            else if (score < 1300)
            {
                return 10;
            }
            else if (score < 2000)
            {
                return 11;
            }
            else if (score < 3000)
            {
                return 12;
            }
            else if (score < 5000)
            {
                return 13;
            }
            else
            {
                return 14;
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
                AwardInfo award = DataCenter.AwardService.GetAward(cell.Row.GetCell("colP1").GetString());
                String colName = cell.Column.Name;
                String cellValue = cell.GetString();
                if (colName == "colP2")
                {
                    award.m_name = cellValue;
                }
                else if (colName == "colP3")
                {
                    award.m_title = cellValue;
                }
                else if (colName == "colP4")
                {
                    award.m_createDate = cellValue;
                }
                DataCenter.AwardService.Save(award);
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        private void Import()
        {
            String copyData = Clipboard.GetText();
            String[] strs = copyData.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            DataCenter.AwardService.m_awards.Clear();
            int strsSize = strs.Length;
            for (int i = 0; i < strsSize; i++)
            {
                String[] strs2 = strs[i].Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (strs2.Length >= 3)
                {
                    try
                    {
                        AwardInfo awardInfo = new AwardInfo();
                        awardInfo.m_ID = System.Guid.NewGuid().ToString();
                        awardInfo.m_createDate = strs2[strs2.Length - 1];
                        awardInfo.m_name = strs2[strs2.Length - 2];
                        awardInfo.m_title = strs2[0];
                        DataCenter.AwardService.m_awards.Add(awardInfo);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            DataCenter.AwardService.Save();
            Console.WriteLine("1");
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
                AwardService awardService = DataCenter.AwardService;
                List<GridRow> selectedRows = m_gridAwards.SelectedRows;
                int selectedRowsSize = selectedRows.Count;
                if (selectedRowsSize > 0)
                {
                    String newJobID = m_selectStaffWindow.GetSelectedJobIDs();
                    AwardInfo award = awardService.GetAward(selectedRows[0].GetCell("colP1").GetString());
                    award.m_name = newJobID;
                    awardService.Save(award);
                    AddAwardToGrid(award);
                }
                m_selectStaffWindow = null;
            }
            return 0;
        }
    }
}
