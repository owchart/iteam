/*******************************************************************************************\
*                                                                                           *
* CFunctionEx.cs -  Indicator functions, types, and definitions.                            *
*                                                                                           *
*               Version 1.00  ★★★                                                        *
*                                                                                           *
*               Copyright (c) 2016-2016, iTeam. All rights reserved.                    *
*               Created by Todd 2016/10/17.                                                  *
*                                                                                           *
********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;

namespace OwLib
{
    /// <summary>
    /// 提示方法
    /// </summary>
    public class CFunctionEx : CFunction
    {
        /// <summary>
        /// 创建方法
        /// </summary>
        /// <param name="indicator">指标</param>
        /// <param name="id">ID</param>
        /// <param name="name">名称</param>
        /// <param name="withParameters">是否有参数</param>
        public CFunctionEx(CIndicator indicator, int id, String name, UIXml xml)
        {
            m_indicator = indicator;
            m_ID = id;
            m_name = name;
            m_xml = xml;
        }

        /// <summary>
        /// 指标
        /// </summary>
        public CIndicator m_indicator;

        /// <summary>
        /// XML对象
        /// </summary>
        public UIXml m_xml;

        /// <summary>
        /// 方法字段
        /// </summary>
        private const String FUNCTIONS = "PLAN.DELETE,PLAN.START,PLAN.STARTALL,PLAN.STOPALL,PLAN.SELECT,PLAN.CREATE,PLAN.CHECK,PLAN.GETSELECTED,PLAN.MODIFY,PLAYSOUND";

        /// <summary>
        /// 开始索引
        /// </summary>
        private const int STARTINDEX = 1000000;

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="var">变量</param>
        /// <returns>结果</returns>
        public override double OnCalculate(CVariable var)
        {
            switch (var.m_functionID)
            {
                case STARTINDEX:
                    return PLAN_DELETE(var);
                case STARTINDEX + 1:
                    return PLAN_START(var);
                case STARTINDEX + 2:
                    return PLAN_STARTALL(var);
                case STARTINDEX + 3:
                    return PLAN_STOPALL(var);
                case STARTINDEX + 4:
                    return PLAN_SELECT(var);
                case STARTINDEX + 5:
                    return PLAN_CREATE(var);
                case STARTINDEX + 6:
                    return PLAN_CHECK(var);
                case STARTINDEX + 7:
                    return PLAN_GETSELECTED(var);
                case STARTINDEX + 8:
                    return PLAN_MODIFY(var);
                case STARTINDEX + 9:
                    return PLAYSOUND(var);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 创建指标
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="script">脚本</param>
        /// <param name="xml">XML</param>
        /// <returns>指标</returns>
        public static CIndicator CreateIndicator(String id, String script, UIXml xml)
        {
            CIndicator indicator = xml.Native.CreateIndicator();
            indicator.Name = id;
            CTable table = xml.Native.CreateTable();
            indicator.DataSource = table;
            CFunctionBase.AddFunctions(indicator);
            CFunctionUI.AddFunctions(indicator, xml);
            CFunctionWin.AddFunctions(indicator);
            CFunctionAjax.AddFunctions(indicator);
            int index = STARTINDEX;
            String[] functions = FUNCTIONS.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int functionsSize = functions.Length;
            for (int i = 0; i < functionsSize; i++)
            {
                indicator.AddFunction(new CFunctionEx(indicator, index + i, functions[i], xml));
            }
            indicator.Script = script;
            table.AddColumn(0);
            table.Set(0, 0, 0);
            indicator.OnCalculate(0);
            return indicator;
        }


        /// <summary>
        /// 检查计划
        /// </summary>
        /// <param name="var">变量</param>
        /// <returns>状态</returns>
        private double PLAN_CHECK(CVariable var)
        {
            PlanWindow planWindow = DataCenter.PlanWindow;
            planWindow.Check();
            return 1;
        }

        /// <summary>
        /// 删除计划
        /// </summary>
        /// <param name="var">变量</param>
        /// <returns>状态</returns>
        private double PLAN_DELETE(CVariable var)
        {
            if (m_indicator.Name != null && m_indicator.Name.Length > 0)
            {
                DataCenter.PlanService.RemoveService(m_indicator.Name);
            }
            else
            {
                PlanWindow planWindow = DataCenter.PlanWindow;
                planWindow.Delete();
            }
            return 1;
        }

        /// <summary>
        /// 获取选中的计划
        /// </summary>
        /// <param name="var">变量</param>
        /// <returns>状态</returns>
        private double PLAN_GETSELECTED(CVariable var)
        {
            PlanWindow planWindow = DataCenter.PlanWindow;
            String id = planWindow.GetSelectedPlanID();
            if (id != null && id.Length > 0)
            {
                CPlan plan = new CPlan();
                DataCenter.PlanService.GetPlan(id, ref plan);
                CVariable newVar = new CVariable(m_indicator);
                newVar.m_expression = "'" + id + "'";
                m_indicator.SetVariable(var.m_parameters[0], newVar);
                CVariable newVar2 = new CVariable(m_indicator);
                newVar2.m_expression = "'" + plan.m_name + "'";
                m_indicator.SetVariable(var.m_parameters[1], newVar2);
                CVariable newVar3 = new CVariable(m_indicator);
                newVar3.m_expression = "'" + new DateTime(plan.m_nextTime).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                m_indicator.SetVariable(var.m_parameters[2], newVar3);
                 CVariable newVar4 = new CVariable(m_indicator);
                newVar4.m_expression =  "'" + plan.m_member + "'";
                m_indicator.SetVariable(var.m_parameters[3], newVar4);
                CVariable newVar5 = new CVariable(m_indicator);
                newVar5.m_expression = "'" + plan.m_command + "'";
                m_indicator.SetVariable(var.m_parameters[4], newVar5);
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 修改计划
        /// </summary>
        /// <param name="var">变量</param>
        /// <returns>状态</returns>
        private double PLAN_MODIFY(CVariable var)
        {
            String id = m_indicator.GetText(var.m_parameters[0]);
            if (id != null && id.Length > 0)
            {
                CPlan plan = new CPlan();
                DataCenter.PlanService.GetPlan(id, ref plan);
                plan.m_name = m_indicator.GetText(var.m_parameters[1]);
                plan.m_nextTime = Convert.ToDateTime(m_indicator.GetText(var.m_parameters[2])).Ticks;
                plan.m_member = m_indicator.GetText(var.m_parameters[3]);
                plan.m_command = m_indicator.GetText(var.m_parameters[4]);
                DataCenter.PlanService.SavePlans();
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// 新的计划
        /// </summary>
        /// <param name="var">变量</param>
        /// <returns>状态</returns>
        private double PLAN_CREATE(CVariable var)
        {
            //拼装行对象
            CPlan plan = new CPlan();
            String id = System.Guid.NewGuid().ToString();
            String name = "";
            String sTime = "";
            String command = "";
            String member = "";
            int timeSpan = 60;
            String runImmediately = "否";
            if (var.m_parameters.Length == 1)
            {
                String fileName = Application.StartupPath + "\\" + var.m_parameters[0].m_expression.Replace("'", "");
                String text = "";
                CFileA.Read(fileName, ref text);
                int index = text.IndexOf("\r\n");
                name = text.Substring(0, index);
                text = text.Substring(index + 2);
                index = text.IndexOf("\r\n");
                sTime = text.Substring(index + 2);
                index = text.IndexOf("\r\n");
                member = text.Substring(0, index);
                text = text.Substring(index + 2);
                index = text.IndexOf("\r\n");
                command = text.Substring(index + 2);
            }
            else
            {
                name = m_indicator.GetText(var.m_parameters[0]);
                sTime = m_indicator.GetText(var.m_parameters[1]);
                member = m_indicator.GetText(var.m_parameters[2]);
                command = m_indicator.GetText(var.m_parameters[3]);
            }
            plan.m_id = id;
            plan.m_name = name;
            plan.m_member = member;
            plan.m_status = "启动";
            if (command != null && command.Length > 0)
            {
                plan.m_command = command;
            }
            plan.m_createTime = DateTime.Now.Ticks;
            plan.m_timeSpan = timeSpan;
            DateTime startTime = new DateTime(new TimeSpan(DateTime.Now.Ticks + (long)plan.m_timeSpan * 1000 * 10000).Ticks);
            if (sTime != null && sTime.Length > 0)
            {
                startTime = Convert.ToDateTime(sTime);
            }
            plan.m_startTime = startTime.Ticks;
            plan.m_runImmediately = runImmediately == "是";
            if (plan.m_runImmediately)
            {
                plan.m_startTime = DateTime.Now.Ticks;
            }
            else
            {
                if (startTime < DateTime.Now)
                {
                    plan.m_runImmediately = true;
                    plan.m_startTime = DateTime.Now.Ticks;
                }
                else
                {
                    plan.m_nextTime = plan.m_startTime;
                }
            }
            DataCenter.PlanService.NewService(plan);
            return 0;
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="var">变量</param>
        /// <returns>状态</returns>
        private double PLAN_START(CVariable var)
        {
            PlanWindow planWindow = DataCenter.PlanWindow;
            planWindow.Start();
            return 1;
        }

        /// <summary>
        /// 开始所有任务
        /// </summary>
        /// <param name="var">变量</param>
        /// <returns>状态</returns>
        private double PLAN_STARTALL(CVariable var)
        {
            PlanWindow planWindow = DataCenter.PlanWindow;
            planWindow.StartAll();
            return 1;
        }

        /// <summary>
        /// 结束所有任务
        /// </summary>
        /// <param name="var">变量</param>
        /// <returns>状态</returns>
        private double PLAN_STOPALL(CVariable var)
        {
            PlanWindow planWindow = DataCenter.PlanWindow;
            planWindow.StopAll();
            return 1;
        }

        /// <summary>
        /// 选中计划
        /// </summary>
        /// <param name="var">变量</param>
        /// <returns>状态</returns>
        private double PLAN_SELECT(CVariable var)
        {
            PlanWindow planWindow = DataCenter.PlanWindow;
            planWindow.SelectPlan();
            return 1;
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="var">变量</param>
        /// <returns>状态</returns>
        private double PLAYSOUND(CVariable var)
        {
            Sound.Play(m_indicator.GetText(var.m_parameters[0]));
            return 1;
        }
    }
}
