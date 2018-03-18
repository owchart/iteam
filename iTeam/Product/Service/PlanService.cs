/**************************************************************************************\
*                                                                                      *
* PlanService.cs -  Plan service functions, types, and definitions.       *
*                                                                                      *
*               Version 1.00 ★                                                        *
*                                                                                      *
*               Copyright (c) 2016-2016, iTeam. All rights reserved.               *
*               Created by Todd.                                                 *
*                                                                                      *
***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace OwLib
{
    /// <summary>
    /// 计划
    /// </summary>
    public class CPlan
    {
        /// <summary>
        /// 命令
        /// </summary>
        public String m_command = "无";

        /// <summary>
        /// 创建时间
        /// </summary>
        public long m_createTime;

        /// <summary>
        /// ID
        /// </summary>
        public String m_id = "无";

        /// <summary>
        /// 上次结果
        /// </summary>
        public String m_lastResult = "无";

        /// <summary>
        /// 上次结束时间
        /// </summary>
        public long m_lastTime;

        /// <summary>
        /// 成员
        /// </summary>
        public String m_member = "无";

        /// <summary>
        /// 名称
        /// </summary>
        public String m_name = "无";

        /// <summary>
        /// 下次执行时间
        /// </summary>
        public long m_nextTime;

        /// <summary>
        /// 是否立即执行
        /// </summary>
        public bool m_runImmediately;

        /// <summary>
        /// 开始执行
        /// </summary>
        public long m_startTime;

        /// <summary>
        /// 状态
        /// </summary>
        public String m_status = "启动";

        /// <summary>
        /// 间隔
        /// </summary>
        public int m_timeSpan = 60;

        /// <summary>
        /// 从字符串解析
        /// </summary>
        /// <param name="str">字符串</param>
        public void FromString(String str)
        {
            String[] strs = str.Split(new String[] { "☼" }, StringSplitOptions.RemoveEmptyEntries);
            int strsSize = strs.Length;
            if (strsSize >= 11)
            {
                m_id = strs[0];
                m_name = strs[1];
                m_command = strs[2];
                m_status = strs[3];
                m_lastResult = strs[4];
                m_member = strs[5];
                m_createTime = Convert.ToInt64(strs[6]);
                m_startTime = Convert.ToInt64(strs[7]);
                m_lastTime = Convert.ToInt64(strs[8]);
                m_nextTime = Convert.ToInt64(strs[9]);
                m_timeSpan = Convert.ToInt32(strs[10]);
                m_runImmediately = strs[11] == "1";
            }
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override String ToString()
        {
            return String.Format("{0}☼{1}☼{2}☼{3}☼{4}☼{5}☼{6}☼{7}☼{8}☼{9}☼{10}☼{11}",
                m_id,
                m_name,
                m_command,
                m_status,
                m_lastResult,
                m_member, 
                m_createTime,
                m_startTime,
                m_lastTime,
                m_nextTime,
                m_timeSpan,
                m_runImmediately);
        }
    }

    /// <summary>
    /// 任务计划的处理类
    /// </summary>
    public class PlanService
    {
        #region Lord 2012/5/3
        /// <summary>
        /// 静态构造函数
        /// </summary>
        public PlanService()
        {
            Init();
        }

        /// <summary>
        /// 日志集合
        /// </summary>
        private List<String> m_logs = new List<String>();

        /// <summary>
        /// 计划列表
        /// </summary>
        private Dictionary<String, CPlan> m_plans = new Dictionary<String, CPlan>();

        /// <summary>
        /// 获取新的日志
        /// </summary>
        /// <returns>日志</returns>
        public String GetNewLogs()
        {
            String log = "";
            lock (m_logs)
            {
                int logsSize = m_logs.Count;
                for (int i = 0; i < logsSize; i++)
                {
                    log += m_logs[i];
                }
                m_logs.Clear();
            }
            return log;
        }

        /// <summary>
        /// 获取计划
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="plan">计划</param>
        /// <returns>状态</returns>
        public int GetPlan(String id, ref CPlan plan)
        {
            int state = 0;
            lock (m_plans)
            {
                if (m_plans.ContainsKey(id))
                {
                    plan = m_plans[id];
                    state = 1;
                }
            }
            return state;
        }

        /// <summary>
        /// 获取所有的计划
        /// </summary>
        /// <param name="plans">计划列表</param>
        /// <returns>状态</returns>
        public int GetPlans(List<CPlan> plans)
        {
            lock (m_plans)
            {
                foreach (CPlan plan in m_plans.Values)
                {
                    plans.Add(plan);
                }
            }
            return 1;
        }

        /// <summary>
        /// 记录Log
        /// </summary>
        /// <param name="text">日志</param>
        public void Log(String text)
        {
            lock (m_logs)
            {
                m_logs.Add(String.Format("{0} {1}\r\n", DateTime.Now.ToString(), text));
            }
        }

        /// <summary>
        /// 初始化方法
        /// </summary>
        public void Init()
        {
            UserCookie cookie = new UserCookie();
            if (DataCenter.UserCookieService.GetCookie("PLANS", ref cookie) > 0)
            {
                String[] strs = cookie.m_value.Split(new String[] { "☀" }, StringSplitOptions.RemoveEmptyEntries);
                int strsSize = strs.Length;
                for (int i = 0; i < strsSize; i++)
                {
                    CPlan plan = new CPlan();
                    plan.FromString(strs[i]);
                    if (plan.m_id.Length > 0)
                    {
                        m_plans[plan.m_id] = plan;
                    }
                }
            }
        }

        /// <summary>
        /// 秒表事件
        /// </summary>
        public void OnTimer()
        {
            DateTime now = DateTime.Now;
            long nowTicks = now.Ticks;
            List<CPlan> plans = new List<CPlan>();
            GetPlans(plans);
            int plansSize = plans.Count;
            for(int i = 0; i < plansSize;i++)
            {
                CPlan plan = plans[i];
                if (plan.m_runImmediately)
                {
                    if (plan.m_status == "启动")
                    {
                        //立即执行的任务
                        plan.m_runImmediately = false;
                        plan.m_lastTime = nowTicks;
                        plan.m_lastResult = "无";
                        plan.m_nextTime = nowTicks + (long)plan.m_timeSpan * 1000 * 10000;
                        //StartRunPlan(plan);
                        SavePlans();
                    }
                }
                else
                {
                    if (plan.m_nextTime != 0)
                    {
                        long timeSpan = (long)plan.m_timeSpan * 1000 * 10000;
                        if (plan.m_nextTime < nowTicks)
                        {
                            //满足执行的情况
                            if (plan.m_nextTime + timeSpan > nowTicks)
                            {
                                if (plan.m_status == "启动")
                                {
                                    plan.m_status = "禁止";
                                    //StartRunPlan(plan);
                                    plan.m_lastTime = plan.m_nextTime;
                                    plan.m_lastResult = "无";
                                    SavePlans();
                                    Console.Beep(1000, 500);
                                    Console.Beep(1000, 500);
                                    Console.Beep(1000, 500);
                                }
                                //plan.m_nextTime += timeSpan;
                            }
                            else
                            {
                                //计算出最近的执行时间
                                long newDate = plan.m_nextTime + timeSpan;
                                while (newDate < nowTicks)
                                {
                                    newDate += timeSpan;
                                }
                                plan.m_nextTime = newDate;
                            }
                        }
                    }
                }
            }
            plans.Clear();
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void SavePlans()
        {
            String str = "";
            List<CPlan> plans = new List<CPlan>();
            GetPlans(plans);
            int plansSize = plans.Count;
            for (int i = 0; i < plansSize; i++)
            {
                CPlan plan = plans[i];
                str += plan.ToString();
                str += "☀";
            }
            plans.Clear();
            UserCookie cookie = new UserCookie();
            cookie.m_key = "PLANS";
            cookie.m_value = str;
            DataCenter.UserCookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 设置最后结果
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="result">结果</param>
        public void SetLastResult(String id, String result)
        {
            CPlan plan = new CPlan();
            if (GetPlan(id, ref plan) > 0)
            {
                plan.m_lastResult = result;
            }
        }

        /// <summary>
        /// 异步运行计划
        /// </summary>
        /// <param name="parameter">参数</param>
        public void Run(object parameter)
        {
            //获取参数
            object[] parameters = parameter as object[];
            String id = parameters[0].ToString();
            String commandString = parameters[1].ToString();
            SetLastResult(id, "正在执行");
            try
            {
                UIXml uiXml = new UIXml();
                uiXml.CreateNative();
                CIndicator indicator = CFunctionEx.CreateIndicator(id, commandString, uiXml);
                indicator.Dispose();
                uiXml.Dispose();
            }
            catch (Exception ex)
            {
                SetLastResult(id, "失败:" + ex.Message + "\r\n" + ex.StackTrace);
                return;
            }
            SetLastResult(id, "成功");
        }

        /// <summary>
        /// 开始准备运行计划
        /// </summary>
        /// <param name="plan">计划对象</param>
        public void StartRunPlan(CPlan plan)
        {
            object[] parameters = new object[3];
            parameters[0] = plan.m_id;
            parameters[1] = plan.m_command;
            Log("正在执行计划:" + plan.m_name);
            //启动线程运行
            Thread planThread = new Thread(new ParameterizedThreadStart(Run));
            planThread.IsBackground = true;
            planThread.Start(parameters);
        }

        /// <summary>
        /// 创建服务
        /// </summary>
        /// <param name="plan">计划</param>
        public void NewService(CPlan plan)
        {
            Log("创建计划:" + plan.m_name);
            lock (m_plans)
            {
                m_plans[plan.m_id] = plan;
            }
            SavePlans();
        }

        /// <summary>
        /// 启动计划
        /// </summary>
        public void StartService(String id)
        {
            CPlan plan = new CPlan();
            if (GetPlan(id, ref plan) > 0)
            {
                plan.m_status = "启动";
                Log("启动计划:" + plan.m_name);
                SavePlans();
            }
        }

        /// <summary>
        /// 禁用服务
        /// </summary>
        /// <param name="id">ID</param>
        public void StopService(String id)
        {
            CPlan plan = new CPlan();
            if (GetPlan(id, ref plan) > 0)
            {
                plan.m_status = "禁用";
                Log("禁用计划:" + plan.m_name);
                SavePlans();
            }
        }

        /// <summary>
        /// 删除任务　
        /// </summary>
        /// <param name="id">ID</param>
        public void RemoveService(String id)
        {
            CPlan plan = new CPlan();
            if (GetPlan(id, ref plan) > 0)
            {
                lock (m_plans)
                {
                    m_plans.Remove(id);
                }
                Log("删除计划" + plan.m_name);
                SavePlans();
            }
        }
        #endregion
    }
}
