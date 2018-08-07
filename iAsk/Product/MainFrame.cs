/*****************************************************************************\
*                                                                             *
* MainFrame.cs -  MainFrame functions, types, and definitions.                *
*                                                                             *
*               Version 1.00  ★★★                                          *
*                                                                             *
*               Copyright (c) 2016-2016, iTeam. All rights reserved.      *
*               Created by Lord 2016/12/24.                                   *
*                                                                             *
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using OwLib;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using node.gs;

namespace OwLib
{
    /// <summary>
    /// 管理系统
    /// </summary>
    public class MainFrame : UIXmlEx, IDisposable
    {
        /// <summary>
        /// 创建行情系统
        /// </summary>
        public MainFrame()
        {
        }

        /// <summary>
        /// 回答
        /// </summary>
        private Dictionary<String, String> m_answers = new Dictionary<String, String>();

        /// <summary>
        /// 开始按钮
        /// </summary>
        private ButtonA m_btnStart;

        /// <summary>
        /// 子弹时间
        /// </summary>
        private DateTime m_bulletTime = DateTime.Now;

        /// <summary>
        /// 已做数量
        /// </summary>
        private int m_count = 0;

        /// <summary>
        /// 当前考试题目
        /// </summary>
        public QuestionInfo m_currentQuestion;

        /// <summary>
        /// 当前时间
        /// </summary>
        public double m_currentTick;

        /// <summary>
        /// 考试时间
        /// </summary>
        private int m_examMinute = 15;

        /// <summary>
        /// 首次执行时间
        /// </summary>
        private DateTime m_firstTime = DateTime.Now;

        /// <summary>
        /// IP地址
        /// </summary>
        private String m_ip = "47.100.16.237";

        /// <summary>
        /// 是否比赛
        /// </summary>
        private bool m_isMatch = false;

        /// <summary>
        /// 上次时间
        /// </summary>
        private DateTime m_lastTime = DateTime.Now;

        /// <summary>
        /// 警告框
        /// </summary>
        private LabelA m_lblAlarm;

        /// <summary>
        /// 模式框
        /// </summary>
        private LabelA m_lblMode;

        /// <summary>
        /// 倒计时
        /// </summary>
        private LabelA m_lblTime;

        /// <summary>
        /// 类型框
        /// </summary>
        private LabelA m_lblType;

        /// <summary>
        /// 是否结束
        /// </summary>
        private bool m_isOver;

        /// <summary>
        /// 老问题
        /// </summary>
        private List<QuestionInfo> m_oldQuestions = new List<QuestionInfo>();

        /// <summary>
        /// 考试题目
        /// </summary>
        private List<QuestionInfo> m_questions = new List<QuestionInfo>();

        /// <summary>
        /// 随机种子
        /// </summary>
        private Random m_rd = new Random();

        /// <summary>
        /// 阴影时间
        /// </summary>
        private DateTime m_shadowTime = DateTime.Now;

        /// <summary>
        /// 背景图
        /// </summary>
        private Sky m_sky;

        /// <summary>
        /// 自增数
        /// </summary>
        private int m_tick;

        /// <summary>
        /// 总时间
        /// </summary>
        private double m_totalTick;

        /// <summary>
        /// 回答文本框
        /// </summary>
        public iTextBox m_txtAnswer;

        /// <summary>
        /// 问题文本框
        /// </summary>
        private iTextBox m_txtQuestion;

        /// <summary>
        /// 弹幕窗体
        /// </summary>
        private BarrageDiv m_barrageDiv;

        private int m_mode;

        /// <summary>
        /// 获取或设置模式
        /// </summary>
        public int Mode
        {
            get { return m_mode; }
            set { m_mode = value; }
        }

        /// <summary>
        /// 添加弹幕
        /// </summary>
        /// <param name="text">文字</param>
        /// <param name="mode">模式</param>
        /// <param name="speed">速度</param>
        public void AddBarrage(String text, int mode, int speed)
        {
            Barrage barrage = new Barrage();
            barrage.Text = text;
            barrage.Speed = speed;
            barrage.Mode = mode;
            m_barrageDiv.AddBarrage(barrage);
        }

        /// <summary>
        /// 躲子弹失败
        /// </summary>
        public void BulletLose()
        {
            for (int i = 0; i < 50; i++)
            {
                AddBarrage("BANG!!!", 0, 4 + i);
            }
            ChangeQuestion();
        }

        /// <summary>
        /// 触发秒表事件
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="timerID">秒表ID</param>
        private void CallTimerEvent(object sender, int timerID)
        {
            if (m_isOver)
            {
                return;
            }
            if (m_sky != null)
            {
                if (m_currentQuestion != null && m_mode >= 5 && m_currentQuestion.m_type != "极限")
                {
                    TimeSpan ts = DateTime.Now - m_bulletTime;
                    int bollSeconds = 30, totalSeconds = (int)ts.TotalSeconds;
                    m_sky.Text = "";
                    if (totalSeconds >= bollSeconds)
                    {
                        Sound.Play("sound\\attbomb.wav");
                        m_sky.CreateBullets(6);
                        m_bulletTime = DateTime.Now;
                    }
                    else if (totalSeconds == bollSeconds - 1)
                    {
                        Sound.Play("sound\\1.wav");
                        m_sky.Text = "1";
                    }
                    else if (totalSeconds == bollSeconds - 2)
                    {
                        Sound.Play("sound\\2.wav");
                        m_sky.Text = "2";
                    }
                    else if (totalSeconds == bollSeconds - 3)
                    {
                        Sound.Play("sound\\3.wav");
                        m_sky.Text = "3";
                    }
                    else if (totalSeconds == bollSeconds - 4)
                    {
                        Sound.Play("sound\\bomb.wav");
                        m_sky.Text = "准备炸弹";
                    }
                    TimeSpan ts2 = DateTime.Now - m_shadowTime;
                    int shadownTime = 1, totalSeconds2 = (int)ts2.TotalSeconds;
                    if (totalSeconds2 >= shadownTime)
                    {

                        m_shadowTime = DateTime.Now;
                    }
                    m_sky.OnTimer(timerID);
                }
                if (m_currentQuestion != null && (m_currentQuestion.m_type == "记忆" || m_currentQuestion.m_type == "算数"))
                {
                    if (m_currentQuestion.m_answer == m_txtAnswer.Text)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            AddBarrage("回答正确", 0, 4 + i);
                        }
                        ChangeQuestion();
                    }
                }
            }
            double m_oldCurrentTick = m_currentTick;
            if (m_currentTick > 0)
            {
                TimeSpan ts = DateTime.Now - m_lastTime;
                bool bulletIsClick = true;
                foreach (Bullet bullet in m_sky.m_bullets)
                {
                    if (!bullet.IsClick)
                    {
                        bulletIsClick = false;
                    }
                }
                if (bulletIsClick)
                {
                    m_currentTick -= (double)ts.Milliseconds / 1000;
                }
                if (m_currentTick <= 0)
                {
                    if (m_questions != null)
                    {
                        m_answers[m_currentQuestion.m_title] = m_txtAnswer.Text;
                    }
                    if (m_currentQuestion != null && m_currentQuestion.m_type == "极限")
                    {
                        Native.ExportToImage(DataCenter.GetAppPath() + "\\成绩截图.jpg");
                    }
                    m_lblTime.Text = "时间到了";
                    m_txtAnswer.Text = m_currentQuestion.m_type;
                    m_currentTick = 0;
                    ChangeQuestion();
                }
                else
                {
                    if (m_currentQuestion.m_type == "记忆")
                    {
                        if (m_totalTick - m_currentTick > m_totalTick / 2)
                        {
                            m_txtQuestion.Text = "不可见";
                            m_txtAnswer.ReadOnly = false;
                            if (m_txtAnswer.Text == "请迅速记忆上面这串数字,在此文字消失时打出刚才那串数字")
                            {
                                m_txtAnswer.Text = "";
                            }
                        }
                        if (m_currentTick <= 2)
                        {
                            m_txtQuestion.Text = m_currentQuestion.m_answer;
                        }
                    }
                    double finishTime = (double)((TimeSpan)(DateTime.Now - m_firstTime)).TotalMilliseconds / 1000;
                    m_lblTime.Text = "还剩" + m_currentTick.ToString("0.00") + "秒 已用时" + finishTime.ToString("0.00") + "秒";
                    if (finishTime > 60 * m_examMinute)
                    {
                        m_answers[m_currentQuestion.m_title] = m_txtAnswer.Text;
                        String file = DataCenter.GetAppPath() + "\\Result.txt";
                        StringBuilder sb = new StringBuilder();
                        int index = 1;
                        foreach (String question in m_answers.Keys)
                        {
                            sb.AppendLine(index.ToString() + "." + question);
                            sb.AppendLine(m_answers[question]);
                            index++;
                        }
                        CFileA.Write(file, sb.ToString());
                        String examName = "";
                        CFileA.Read(DataCenter.GetAppPath() + "\\WriteYourName.txt", ref examName);
                        String url = "http://" + m_ip + ":10009/sendresult?name=" + examName;
                        HttpPostService postService = new HttpPostService();
                        postService.Post(url, sb.ToString());
                        m_txtAnswer.Text = "考试时间到,请等待考试结果!";
                        Native.Invalidate();
                        m_isOver = true;
                    }
                }
            }
            if (m_mode == 5)
            {
                if (m_currentQuestion.m_type == "打字" || m_currentQuestion.m_type == "极限")
                {
                    if (m_tick % 5 == 0)
                    {
                        AddBarrage(m_rd.Next(0, 2) == 0 ? "111" : "222", 2, m_rd.Next(3, 20));
                    }
                }
            }
            m_lastTime = DateTime.Now;
            m_tick++;
            if (m_tick > 10000)
            {
                m_tick = 0;
            }
            Native.Invalidate();
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="mp">坐标</param>
        /// <param name="button">按钮</param>
        /// <param name="clicks">点击次数</param>
        /// <param name="delta">滚轮值/param>
        private void ClickEvent(object sender, POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            if (button == MouseButtonsA.Left && clicks == 1)
            {
                ControlA control = sender as ControlA;
                String name = control.Name;
                if (name == "btnStart")
                {
                    ChangeQuestion();
                }
                else if (name == "choose1" || name == "choose2" || name == "choose3" || name == "choose4"
                    || name == "choose5" || name == "choose6")
                {
                    ButtonA choose1 = GetButton("choose1");
                    ButtonA choose2 = GetButton("choose2");
                    ButtonA choose3 = GetButton("choose3");
                    ButtonA choose4 = GetButton("choose4");
                    ButtonA choose5 = GetButton("choose5");
                    ButtonA choose6 = GetButton("choose6");
                    choose1.Opacity = 0.5f;
                    choose2.Opacity = 0.5f;
                    choose3.Opacity = 0.5f;
                    choose4.Opacity = 0.5f;
                    choose5.Opacity = 0.5f;
                    choose6.Opacity = 0.5f;
                    control.Opacity = 1;
                    if (choose1.Opacity == 1)
                    {
                        m_lblMode.Text = "初级";
                        AddBarrage("选择初级难度", 1, 5);
                    }
                    else if (choose2.Opacity == 1)
                    {
                        m_lblMode.Text = "中级";
                        AddBarrage("选择中级难度",1, 5);
                    }
                    else if (choose3.Opacity == 1)
                    {
                        m_lblMode.Text = "高级";
                        AddBarrage("选择高级难度", 1, 5);
                    }
                    else if (choose4.Opacity == 1)
                    {
                        m_lblMode.Text = "英雄";
                        AddBarrage("选择英雄难度", 1, 5);
                    }
                    else if (choose5.Opacity == 1)
                    {
                        m_lblMode.Text = "史诗";
                        AddBarrage("选择史诗难度", 1, 5);
                    }
                    else if (choose6.Opacity == 1)
                    {
                        m_lblMode.Text = "传说";
                        AddBarrage("选择史诗难度", 1, 5);
                    }
                }
            }
        }

        /// <summary>
        /// 切换问题
        /// </summary>
        public void ChangeQuestion()
        {
            if (m_currentQuestion != null)
            {
                m_answers[m_currentQuestion.m_title] = m_txtAnswer.Text;
            }
            if (m_currentQuestion != null && (m_currentQuestion.m_type == "记忆" || m_currentQuestion.m_type == "算数"))
            {
                if (m_currentQuestion.m_answer != m_txtAnswer.Text)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        AddBarrage("回答错误", 0, 4 + i);
                    }
                }
            }
            m_txtAnswer.ReadOnly = false;
            if (m_btnStart.Text == "开始")
            {
                Thread thread = new Thread(new ThreadStart(CheckResult));
                thread.Start();
                //加载问题
                String file = DataCenter.GetAppPath() + "\\Exam.txt";
                if (GetRadioButton("rbExamC").Checked)
                {
                    file = DataCenter.GetAppPath() + "\\Exam_cplusplus.txt";
                }
                else if (GetRadioButton("rbExamJava").Checked)
                {
                    file = DataCenter.GetAppPath() + "\\Exam_Java.txt";
                }
                String content = "";
                CFileA.Read(file, ref content);
                String[] strs = content.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                int strsSize = strs.Length;
                for (int i = 0; i < strsSize; i++)
                {
                    String str = strs[i];
                    QuestionInfo question = new QuestionInfo();
                    int idx = str.IndexOf(",");
                    question.m_type = str.Substring(0, idx);
                    int nIdx = str.IndexOf(",", idx + 1);
                    question.m_interval = Convert.ToInt32(str.Substring(idx + 1, nIdx - idx - 1));
                    question.m_title = str.Substring(nIdx + 1);
                    m_oldQuestions.Add(question);
                }
                m_bulletTime = DateTime.Now;
                for (int i = 0; i < 10; i++)
                {
                    AddBarrage("开始答题", 0, 4 + i);
                }
                GetDiv("divMode").Enabled = false;
                GetDiv("divMode").CanFocus = false;
                switch (m_lblMode.Text)
                {
                    case "初级":
                        m_mode = 0;
                        break;
                    case "中级":
                        m_mode = 1;
                        break;
                    case "高级":
                        m_mode = 2;
                        break;
                    case "英雄":
                        m_mode = 3;
                        break;
                    case "史诗":
                        m_mode = 4;
                        break;
                    case "传说":
                        m_mode = 5;
                        break;
                }
                if (m_mode == 5)
                {
                    GetButton("btnTimer").BackImage = "skull.jpg";
                    GetButton("btnAnswer").BackImage = "me.jpg";
                    GetButton("btnQuestion").BackImage = "you.jpg";
                    ButtonA choose1 = GetButton("choose1");
                    ButtonA choose2 = GetButton("choose2");
                    ButtonA choose3 = GetButton("choose3");
                    ButtonA choose4 = GetButton("choose4");
                    ButtonA choose5 = GetButton("choose5");
                    ButtonA choose6 = GetButton("choose6");
                    choose1.BackImage = "a1.jpg";
                    choose2.BackImage = "a2.jpg";
                    choose3.BackImage = "a3.jpg";
                    choose4.BackImage = "a4.jpg";
                    choose5.BackImage = "a5.jpg";
                    choose6.BackImage = "a6.jpg";
                    for (int i = 0; i < 20; i++)
                    {
                        CometA comet = new CometA();
                        Native.AddControl(comet);
                    }
                    m_txtAnswer.BorderColor = COLOR.ARGB(200, 0, 0);
                    m_txtQuestion.BorderColor = COLOR.ARGB(200, 0, 0);
                }
                m_firstTime = DateTime.Now;
                bool lastState = true;
                if (m_isMatch)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        QuestionInfo codingQuestion = new QuestionInfo();
                        codingQuestion.m_type = "极限";
                        codingQuestion.m_interval = 180;
                        codingQuestion.m_title = "以你最快的速度连续输出for(int i = 0; i < 100; i++)(前任select * from冠军是吴思杰，目前30次为合格)";
                        m_questions.Add(codingQuestion);
                    }
                }
                else
                {
                    while (m_oldQuestions.Count > 0)
                    {
                        QuestionInfo question = m_oldQuestions[m_rd.Next(0, m_oldQuestions.Count)];
                        if (lastState)
                        {
                            if (question.m_type == "打字")
                            {
                                m_questions.Add(question);
                                m_oldQuestions.Remove(question);
                                int random = m_rd.Next(0, 5);
                                //加减运算
                                //if (random == 0)
                                //{
                                //    QuestionInfo addSubQuestion = new QuestionInfo();
                                //    addSubQuestion.m_type = "算数";
                                //    addSubQuestion.m_interval = 30;
                                //    int count = 5;
                                //    String num1 = GetRandomNum(count);
                                //    String num2 = GetRandomNum(count);
                                //    String op = "+";
                                //    addSubQuestion.m_title = num1 + op + num2;
                                //    addSubQuestion.m_answer = (CStr.ConvertStrToInt(num1) + CStr.ConvertStrToInt(num2)).ToString();
                                //    m_questions.Add(addSubQuestion);
                                //}
                                ////记忆考察
                                //else if (m_mode != 5 && random == 1)
                                //{
                                //    QuestionInfo memoryQuestion = new QuestionInfo();
                                //    memoryQuestion.m_type = "记忆";
                                //    memoryQuestion.m_interval = 30;
                                //    int count = 11;
                                //    String num = GetRandomNum(count);
                                //    memoryQuestion.m_title = num;
                                //    memoryQuestion.m_answer = num;
                                //    m_questions.Add(memoryQuestion);
                                //}
                                //if (m_questions.Count >= 6 && coding)
                                //{
                                //    QuestionInfo codingQuestion = new QuestionInfo();
                                //    codingQuestion.m_type = "极限";
                                //    codingQuestion.m_interval = 180;
                                //    codingQuestion.m_title = "以你最快的速度连续输出for(int i = 0; i < 100; i++)(前任select * from冠军是吴思杰，目前30次为合格)";
                                //    m_questions.Add(codingQuestion);
                                //    coding = false;
                                //}
                                lastState = false;
                            }
                        }
                        else
                        {
                            if (question.m_type == "口述")
                            {
                                m_questions.Add(question);
                                m_oldQuestions.Remove(question);
                                lastState = true;

                            }
                        }
                        bool noDZ = true, noKS = true;
                        int oldQuestionsSize = m_oldQuestions.Count;
                        for (int i = 0; i < oldQuestionsSize; i++)
                        {
                            if (m_oldQuestions[i].m_type == "口述")
                            {
                                noKS = false;
                            }
                            else if (m_oldQuestions[i].m_type == "打字")
                            {
                                noDZ = false;
                            }
                        }
                        if (noDZ || noKS)
                        {
                            break;
                        }
                    }
                }
            }
            m_btnStart.Text = "下一题";
            if (m_questions.Count > 0)
            {
                m_currentQuestion = m_questions[0];
                if (m_currentQuestion.m_type != "口述")
                {
                    m_txtAnswer.Text = "";
                }
                else
                {
                    m_txtAnswer.Text = "请口述";
                    m_txtAnswer.Text = "";
                }
                m_currentTick = m_currentQuestion.m_interval;
                if (m_currentQuestion.m_type == "口述")
                {
                    switch (m_mode)
                    {
                        case 0:
                            m_currentTick *= 2;
                            break;
                        case 1:
                            break;
                        case 2:
                            m_currentTick = m_currentQuestion.m_interval * 2/ 3;
                            break;
                        case 3:
                            m_currentTick = m_currentQuestion.m_interval / 2;
                            break;
                        case 4:
                            m_currentTick = m_currentQuestion.m_interval / 3;
                            break;
                        case 5:
                            m_currentTick = 2;
                            m_txtAnswer.Text = "请在打字题中回答";
                            break;
                    }
                }
                else if (m_currentQuestion.m_type == "打字")
                {
                    switch (m_mode)
                    {
                        case 0:
                            m_currentTick *= 2;
                            break;
                        case 1:
                            break;
                        case 2:
                            m_currentTick = m_currentQuestion.m_interval * 2 / 3;
                            break;
                        case 3:
                            m_currentTick = m_currentQuestion.m_interval / 2;
                            break;
                        case 4:
                            m_currentTick = m_currentQuestion.m_interval / 3;
                            break;
                        case 5:
                            m_currentTick = m_currentQuestion.m_interval / 3;
                            break;
                    }
                }
                else if (m_currentQuestion.m_type == "算数")
                {
                    switch (m_mode)
                    {
                        case 2:
                            m_currentTick = 25;
                            break;
                        case 3:
                            m_currentTick = 20;
                            break;
                        case 4:
                            m_currentTick = 15;
                            break;
                        case 5:
                            m_currentTick = 10;
                            break;
                    }
                }
                else if (m_currentQuestion.m_type == "记忆")
                {
                    m_txtAnswer.Text = "请迅速记忆上面这串数字,在此文字消失时打出刚才那串数字";
                    m_txtAnswer.ReadOnly = true;
                    switch (m_mode)
                    {
                        case 0:
                            m_currentTick = 40;
                            break;
                        case 1:
                            m_currentTick = 30;
                            break;
                        case 2:
                            m_currentTick = 28;
                            break;
                        case 3:
                            m_currentTick = 26;
                            break;
                        case 4:
                            m_currentTick = 24;
                            break;
                        case 5:
                            m_currentTick = 20;
                            break;
                    }
                }
                m_count++;
                m_lblAlarm.Text = "已做" + m_count.ToString() + "题";
                m_totalTick = m_currentTick;
                double finishTime = (double)((TimeSpan)(DateTime.Now - m_firstTime)).TotalMilliseconds / 1000;
                m_lblTime.Text = "还剩" + m_totalTick.ToString("0.00") + "秒 已用时" + finishTime.ToString("0.00") + "秒";
                m_questions.Remove(m_currentQuestion);
                m_txtQuestion.Text = m_currentQuestion.m_title;
                if (m_currentQuestion.m_type == "口述")
                {
                    m_lblType.Text = "题型:打字,限时" + m_currentTick.ToString() + "秒";
                }
                else
                {
                    m_lblType.Text = "题型:" + m_currentQuestion.m_type + ",限时" + m_currentTick.ToString() + "秒";
                }
            }
        }

        /// <summary>
        /// 退出程序方法
        /// </summary>
        public void CheckResult()
        {
            String examName = "";
            CFileA.Read(DataCenter.GetAppPath() + "\\WriteYourName.txt", ref examName);
            while (true)
            {
                if (m_isOver)
                {
                    String url = "http://" + m_ip + ":10009/getresult?name=" + examName;
                    String examResult = HttpGetService.Get(url);
                    if (examResult.StartsWith("tongguo"))
                    {
                        m_txtAnswer.Text = "机试成绩合格，等待进一步面谈!";
                        for (int i = 0; i < 5; i++)
                        {
                            AddBarrage("机试成绩合格，等待进一步面谈!", 0, 4 + i);
                        }
                        break;
                    }
                    else if (examResult.StartsWith("butongguo"))
                    {
                        m_txtAnswer.Text = "非常抱歉，机试成绩不合格，谢谢参与!";
                        for (int i = 0; i < 5; i++)
                        {
                            AddBarrage("非常抱歉，机试成绩不合格，谢谢参与!", 0, 4 + i);
                        }
                        break;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="count">字数</param>
        /// <returns>字符</returns>
        public String GetRandomNum(int count)
        {
            String strNum = "";
            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    strNum += m_rd.Next(1, 10).ToString();
                }
                else
                {
                    strNum += m_rd.Next(0, 10).ToString();
                }
            }
            return strNum;
        }

        /// <summary>
        /// 是否有窗体显示
        /// </summary>
        /// <returns>是否显示</returns>
        public bool IsWindowShowing()
        {
            List<ControlA> controls = Native.GetControls();
            int controlsSize = controls.Count;
            for (int i = 0; i < controlsSize; i++)
            {
                WindowFrameA frame = controls[i] as WindowFrameA;
                if (frame != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 秒表ID
        /// </summary>
        private int m_timerID = ControlA.GetNewTimerID();

        /// <summary>
        /// 加载XML
        /// </summary>
        /// <param name="xmlPath">XML路径</param>
        public override void Load(String xmlPath)
        {
            LoadFile(xmlPath, null);
            DataCenter.MainUI = this;
            ControlA control = Native.GetControls()[0];
            RegisterEvents(control);
            control.RegisterEvent(new ControlTimerEvent(CallTimerEvent), EVENTID.TIMER);
            control.StartTimer(m_timerID, 10);
            control.RegisterEvent(new ControlPaintEvent(PaintDiv), EVENTID.PAINT);
            m_barrageDiv = new BarrageDiv();
            m_barrageDiv.Dock = DockStyleA.Fill;
            Native.AddControl(m_barrageDiv);
            m_sky = new Sky();
            m_sky.Dock = DockStyleA.Fill;
            Native.AddControl(m_sky);
            m_sky.MainFrame = this;
            m_txtAnswer = FindControl("txtAnswer") as iTextBox;
            m_txtAnswer.MainFrame = this;
            m_txtQuestion = FindControl("txtQuestion") as iTextBox;
            m_lblAlarm = GetLabel("lblAlarm");
            m_lblTime = GetLabel("lblTime");
            m_btnStart = GetButton("btnStart");
            m_lblType = GetLabel("lblType");
            m_lblMode = GetLabel("lblMode");
            (m_btnStart as RuningButton).MainFrame = this;
        }

        /// <summary>
        /// 重绘层
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="paint">绘图对象</param>
        /// <param name="clicpRect">裁剪区域</param>
        private void PaintDiv(Object sender, CPaint paint, RECT clicpRect)
        {
            DivA div = sender as DivA;
            int width = div.Width, height = div.Height;
            RECT drawRect = new RECT(0,0,width,height);
            if (m_mode == 5)
            {
                paint.FillGradientRect(COLOR.ARGB(200, 255, 40, 24), COLOR.ARGB(200, 255, 255, 40), drawRect, 0, m_tick % 360);
            }
            else
            {
                paint.FillGradientRect(COLOR.ARGB(200, 90, 120, 24), COLOR.ARGB(200, 122, 156, 40), drawRect, 0, 0);
            }
            RECT bounds = m_lblTime.Bounds;
            RECT fullBloodRect = new RECT(bounds.left, bounds.bottom, bounds.left + 400, bounds.bottom + 15);
            paint.FillRect(COLOR.ARGB(255, 0, 0), fullBloodRect);
            int bloodWidth = (int)(m_currentTick / m_totalTick * 400);
            RECT bloodRect = new RECT(bounds.left, bounds.bottom, bounds.left + bloodWidth, bounds.bottom + 15);
            if (bloodWidth < 40)
            {
                paint.FillRect(COLOR.ARGB(255, 150, 0), bloodRect);
            }
            else if (bloodWidth < 120)
            {
                paint.FillRect(COLOR.ARGB(255, 200, 0), bloodRect);
            }
            else
            {
                paint.FillRect(COLOR.ARGB(255, 255, 0), bloodRect);
            }
            if (m_currentQuestion != null && m_currentQuestion.m_type == "极限")
            {
                String[] strs = m_txtAnswer.Text.Split(new String[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
                int cCount = 0;
                foreach (String str in strs)
                {
                    if (str == "for(int i = 0; i < 100; i++)")
                    {
                        cCount++;
                    }
                }
                String strLine = cCount.ToString();
                FONT tFont = new FONT("微软雅黑", 100, true, false, true);
                SIZE tSize = paint.TextSize(strLine, tFont);
                RECT tRect = new RECT();
                tRect.left = (width - tSize.cx) / 2;
                tRect.top = (height - tSize.cy) / 2;
                tRect.right = tRect.left + tSize.cx;
                tRect.bottom = tRect.top + tSize.cy;
                paint.DrawText(strLine, COLOR.ARGB(200, 255, 255, 255), tFont, tRect);
            }
        }

        /// 注册事件
        /// </summary>
        /// <param name="control">控件</param>
        private void RegisterEvents(ControlA control)
        {
            ControlMouseEvent clickButtonEvent = new ControlMouseEvent(ClickEvent);
            List<ControlA> controls = control.GetControls();
            int controlsSize = controls.Count;
            for (int i = 0; i < controlsSize; i++)
            {
                ControlA subControl = controls[i];
                ButtonA button = subControl as ButtonA;
                if (button != null)
                {
                    button.RegisterEvent(clickButtonEvent, EVENTID.CLICK);
                }
                RegisterEvents(controls[i]);
            }
        }

        /// <summary>
        /// 显示提示窗口
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="caption">标题</param>
        /// <param name="uType">格式</param>
        /// <returns>结果</returns>
        public int ShowMessageBox(String text, String caption, int uType)
        {
            MessageBox.Show(text, caption);
            return 1;
        }


        /// <summary>
        /// 消息监听
        /// </summary>
        /// <param name="m">消息</param>
        public virtual void WndProc(ref Message m)
        {
            if (m.Msg == 0x0201)
            {
                if (m_sky != null)
                {
                    m_sky.OnMouseDown(Native.MousePoint, MouseButtonsA.Left, 1, 0);
                }
            }
            else if (m.Msg == 0x0202)
            {
                m_txtAnswer.Focus();
            }
        }
    }

    /// <summary>
    /// 提问信息
    /// </summary>
    public class QuestionInfo
    {
        /// <summary>
        /// 答案
        /// </summary>
        public String m_answer;

        /// <summary>
        /// 间隔
        /// </summary>
        public int m_interval;

        /// <summary>
        /// 标题
        /// </summary>
        public String m_title;

        /// <summary>
        /// 类型
        /// </summary>
        public String m_type;
    }
}
