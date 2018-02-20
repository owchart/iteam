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
            StaffInfo staff = staffService.GetStaff(award.m_jobID);
            List<GridRow> rows = m_gridAwards.m_rows;
            int rowsSize = rows.Count;
            for (int i = 0; i < rowsSize; i++)
            {
                GridRow findRow = rows[i];
                if (findRow.GetCell("colP1").GetString() == award.m_ID)
                {
                    findRow.GetCell("colP2").SetString(award.m_jobID);
                    findRow.GetCell("colP3").SetString(staffService.GetNamesByJobsID(award.m_jobID));
                    findRow.GetCell("colP4").SetString(award.m_level);
                    findRow.GetCell("colP5").SetString(award.m_title);
                    findRow.GetCell("colP6").SetString(award.m_content);
                    findRow.GetCell("colP7").SetString(award.m_createDate);
                    return;
                }
            }
            GridRow row = new GridRow();
            m_gridAwards.AddRow(row);
            row.AddCell("colP1", new GridStringCell(award.m_ID));
            row.AddCell("colP2", new GridStringCell(award.m_jobID));
            row.AddCell("colP3", new GridStringCell(staffService.GetNamesByJobsID(award.m_jobID)));
            row.AddCell("colP4", new GridStringCell(award.m_level));
            row.AddCell("colP5", new GridStringCell(award.m_title));
            row.AddCell("colP6", new GridStringCell(award.m_content));
            row.AddCell("colP7", new GridStringCell(award.m_createDate));
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
                else if (name == "btnExportExcel")
                {
                    ExportToExcel("荣誉榜.xls", m_gridAwards);
                }
                else if (name == "btnExportTxt")
                {
                    ExportToTxt("荣誉榜.txt", m_gridAwards);
                }
                else if (name == "btnExportExcel2")
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add(new DataColumn("姓名"));
                    String[] otherColumns = new String[] { "次数", "积分", "产品", "研发", "奖励", "管理", "运维" };
                    foreach (String otherColumn in otherColumns)
                    {
                        DataColumn dataColumn = new DataColumn(otherColumn);
                        dataColumn.DataType = typeof(int);
                        dataTable.Columns.Add(dataColumn);
                    }
                    Dictionary<String, int> timesDic = new Dictionary<String, int>();
                    Dictionary<String, int> otherDic1 = new Dictionary<String, int>();
                    Dictionary<String, int> otherDic2 = new Dictionary<String, int>();
                    Dictionary<String, int> otherDic3 = new Dictionary<String, int>();
                    Dictionary<String, int> otherDic4 = new Dictionary<String, int>();
                    Dictionary<String, int> otherDic5 = new Dictionary<String, int>();
                    List<GridRow> rows = m_gridAwards.m_rows;
                    int rowsSize = rows.Count;
                    List<String> namesList = new List<String>();
                    for (int i = 0; i < rowsSize; i++)
                    {
                        GridRow row = rows[i];
                        String[] names = row.GetCell("colP3").GetString().Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        int namesSize = names.Length;
                        for (int j = 0; j < namesSize; j++)
                        {
                            String myName = names[j];
                            namesList.Add(myName);
                            if (timesDic.ContainsKey(myName))
                            {
                                timesDic[myName] = timesDic[myName] + 1;
                            }
                            else
                            {
                                timesDic[myName] = 1;
                            }
                            int score = 1;
                            String level = row.GetCell("colP4").GetString();
                            score = CStr.ConvertStrToInt(level.Substring(2));
                            if (level.IndexOf("产品") != -1)
                            {
                                if (otherDic1.ContainsKey(myName))
                                {
                                    otherDic1[myName] = otherDic1[myName] + score;
                                }
                                else
                                {
                                    otherDic1[myName] = score;
                                }
                            }
                            else if (level.IndexOf("研发") != -1)
                            {
                                if (otherDic2.ContainsKey(myName))
                                {
                                    otherDic2[myName] = otherDic2[myName] + score;
                                }
                                else
                                {
                                    otherDic2[myName] = score;
                                }
                            }
                            else if (level.IndexOf("奖励") != -1)
                            {
                                if (otherDic3.ContainsKey(myName))
                                {
                                    otherDic3[myName] = otherDic3[myName] + score;
                                }
                                else
                                {
                                    otherDic3[myName] = score;
                                }
                            }
                            else if (level.IndexOf("管理") != -1)
                            {
                                if (otherDic4.ContainsKey(myName))
                                {
                                    otherDic4[myName] = otherDic4[myName] + score;
                                }
                                else
                                {
                                    otherDic4[myName] = score;
                                }
                            }
                            else if (level.IndexOf("运维") != -1)
                            {
                                if (otherDic5.ContainsKey(myName))
                                {
                                    otherDic5[myName] = otherDic5[myName] + score;
                                }
                                else
                                {
                                    otherDic5[myName] = score;
                                }
                            }
                        }
                    }
                    List<AwardData> awardDatas = new List<AwardData>();
                    foreach (String key in timesDic.Keys)
                    {
                        AwardData data = new AwardData();
                        data.m_name = key;
                        data.m_times = timesDic[key];
                        if (otherDic1.ContainsKey(key))
                        {
                            data.m_other1 = otherDic1[key];
                            if (data.m_other1 < 0)
                            {
                                data.m_other1 = 0;
                            }
                        }
                        if (otherDic2.ContainsKey(key))
                        {
                            data.m_other2 = otherDic2[key];
                            if (data.m_other2 < 0)
                            {
                                data.m_other2 = 0;
                            }
                        }
                        if (otherDic3.ContainsKey(key))
                        {
                            data.m_other3 = otherDic3[key];
                            if (data.m_other3 < 0)
                            {
                                data.m_other3 = 0;
                            }
                        }
                        if (otherDic4.ContainsKey(key))
                        {
                            data.m_other4 = otherDic4[key];
                            if (data.m_other4 < 0)
                            {
                                data.m_other4 = 0;
                            }
                        }
                        if (otherDic5.ContainsKey(key))
                        {
                            data.m_other5 = otherDic5[key];
                            if (data.m_other5 < 0)
                            {
                                data.m_other5 = 0;
                            }
                        }
                        data.m_scores = data.m_other1 * 2 + data.m_other2 * 3 + data.m_other3 + data.m_other4 * 3 + data.m_other5;
                        awardDatas.Add(data);
                    }
                    awardDatas.Sort(new AwardDataCompare());
                    foreach (AwardData aData in awardDatas)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        aData.m_scores = aData.m_other1 + aData.m_other2 + aData.m_other3 + aData.m_other4 + aData.m_other5;
                        dataRow[0] = aData.m_name;
                        dataRow[1] = aData.m_times;
                        dataRow[2] = aData.m_scores;
                        dataRow[3] = aData.m_other1;
                        dataRow[4] = aData.m_other2;
                        dataRow[5] = aData.m_other3;
                        dataRow[6] = aData.m_other4;
                        dataRow[7] = aData.m_other5;
                        dataTable.Rows.Add(dataRow);
                    }
                    DataCenter.ExportService.ExportDataTableToExcel(dataTable, "荣誉总排名.xlsx");
                    dataTable.Dispose();
                }
                else if (name == "btnExportExcel3")
                {
                    Dictionary<String, int> timesDic = new Dictionary<String, int>();
                    Dictionary<String, int> otherDic1 = new Dictionary<String, int>();
                    Dictionary<String, int> otherDic2 = new Dictionary<String, int>();
                    Dictionary<String, int> otherDic3 = new Dictionary<String, int>();
                    Dictionary<String, int> otherDic4 = new Dictionary<String, int>();
                    Dictionary<String, int> otherDic5 = new Dictionary<String, int>();
                    List<GridRow> rows = m_gridAwards.m_rows;
                    int rowsSize = rows.Count;
                    List<String> namesList = new List<String>();
                    StringBuilder sb2 = new StringBuilder();
                    for (int i = 0; i < rowsSize; i++)
                    {
                        GridRow row = rows[i];
                        sb2.Append(row.GetCell("colP3").GetString() + "|" + row.GetCell("colP4").GetString() + "\r\n");
                        String[] names = row.GetCell("colP3").GetString().Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        int namesSize = names.Length;
                        for (int j = 0; j < namesSize; j++)
                        {
                            String myName = names[j];
                            namesList.Add(myName);
                            if (timesDic.ContainsKey(myName))
                            {
                                timesDic[myName] = timesDic[myName] + 1;
                            }
                            else
                            {
                                timesDic[myName] = 1;
                            }
                            int score = 1;
                            String level = row.GetCell("colP4").GetString();
                            score = CStr.ConvertStrToInt(level.Substring(2));
                            if (level.IndexOf("产品") != -1)
                            {
                                if (otherDic1.ContainsKey(myName))
                                {
                                    otherDic1[myName] = otherDic1[myName] + score;
                                }
                                else
                                {
                                    otherDic1[myName] = score;
                                }
                            }
                            else if (level.IndexOf("研发") != -1)
                            {
                                if (otherDic2.ContainsKey(myName))
                                {
                                    otherDic2[myName] = otherDic2[myName] + score;
                                }
                                else
                                {
                                    otherDic2[myName] = score;
                                }
                            }
                            else if (level.IndexOf("奖励") != -1)
                            {
                                if (otherDic3.ContainsKey(myName))
                                {
                                    otherDic3[myName] = otherDic3[myName] + score;
                                }
                                else
                                {
                                    otherDic3[myName] = score;
                                }
                            }
                            else if (level.IndexOf("管理") != -1)
                            {
                                if (otherDic4.ContainsKey(myName))
                                {
                                    otherDic4[myName] = otherDic4[myName] + score;
                                }
                                else
                                {
                                    otherDic4[myName] = score;
                                }
                            }
                            else if (level.IndexOf("运维") != -1)
                            {
                                if (otherDic5.ContainsKey(myName))
                                {
                                    otherDic5[myName] = otherDic5[myName] + score;
                                }
                                else
                                {
                                    otherDic5[myName] = score;
                                }
                            }
                        }
                    }
                    List<AwardData> awardDatas = new List<AwardData>();
                    foreach (String key in timesDic.Keys)
                    {
                        AwardData data = new AwardData();
                        data.m_name = key;
                        data.m_times = timesDic[key];
                        if (otherDic1.ContainsKey(key))
                        {
                            data.m_other1 = otherDic1[key];
                            if (data.m_other1 < 0)
                            {
                                data.m_other1 = 0;
                            }
                        }
                        if (otherDic2.ContainsKey(key))
                        {
                            data.m_other2 = otherDic2[key];
                            if (data.m_other2 < 0)
                            {
                                data.m_other2 = 0;
                            }
                        }
                        if (otherDic3.ContainsKey(key))
                        {
                            data.m_other3 = otherDic3[key];
                            if (data.m_other3 < 0)
                            {
                                data.m_other3 = 0;
                            }
                        }
                        if (otherDic4.ContainsKey(key))
                        {
                            data.m_other4 = otherDic4[key];
                            if (data.m_other4 < 0)
                            {
                                data.m_other4 = 0;
                            }
                        }
                        if (otherDic5.ContainsKey(key))
                        {
                            data.m_other5 = otherDic5[key];
                            if (data.m_other5 < 0)
                            {
                                data.m_other5 = 0;
                            }
                        }
                        data.m_scores = data.m_other1 * 2 + data.m_other2 * 3 + data.m_other3 + data.m_other4 * 3 + data.m_other5;
                        awardDatas.Add(data);
                    }
                    awardDatas.Sort(new AwardDataCompare());
                    int queue = 0, lastScore = 0; ;
                    StringBuilder sb = new StringBuilder();
                    int awardDatasSize = awardDatas.Count;
                    String strRank = "高阶督军,督军,将军,中将,勇士,百夫长,军团士兵,血卫士,石头守卫,一等军士长,高阶军士,中士,步兵,新兵";
                    String[] strRanks = strRank.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < awardDatasSize; i++)
                    {
                        AwardData aData = awardDatas[i];
                        aData.m_scores = aData.m_other1 + aData.m_other2 + aData.m_other3 + aData.m_other4 + aData.m_other5;
                        if (lastScore != aData.m_scores)
                        {
                            queue = i + 1;
                        }
                        sb.Append(queue.ToString() + ",");
                        int aLevel = GetLevel(aData.m_scores);
                        sb.Append(aData.m_name + " (" + strRanks[strRanks.Length - aLevel] + "),");
                        sb.Append(aData.m_scores.ToString() + ",");
                        sb.Append(aData.m_other1.ToString() + ",");
                        sb.Append(aData.m_other2.ToString() + ",");
                        sb.Append(aData.m_other3.ToString() + ",");
                        sb.Append(aData.m_other4.ToString() + ",");
                        sb.Append(aData.m_other5.ToString());
                        if (i != awardDatasSize - 1)
                        {
                            sb.Append(";");
                        }
                        lastScore = aData.m_scores;
                    }
                    File.WriteAllText(DataCenter.GetAppPath() + "\\Rank2.txt", sb2.ToString());
                    DataCenter.ExportService.ExportHtmlToTxt("Rank.txt", sb.ToString());
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
                if (colName == "colP4")
                {
                    award.m_level = cellValue;
                }
                else if (colName == "colP5")
                {
                    award.m_title = cellValue;
                }
                else if (colName == "colP6")
                {
                    award.m_content = cellValue;
                }
                else if (colName == "colP7")
                {
                    award.m_createDate = cellValue;
                }
                DataCenter.AwardService.Save(award);
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
                AwardService awardService = DataCenter.AwardService;
                List<GridRow> selectedRows = m_gridAwards.SelectedRows;
                int selectedRowsSize = selectedRows.Count;
                if (selectedRowsSize > 0)
                {
                    String newJobID = m_selectStaffWindow.GetSelectedJobIDs();
                    AwardInfo award = awardService.GetAward(selectedRows[0].GetCell("colP1").GetString());
                    award.m_jobID = newJobID;
                    awardService.Save(award);
                    AddAwardToGrid(award);
                }
                m_selectStaffWindow = null;
            }
            return 0;
        }
    }
}
