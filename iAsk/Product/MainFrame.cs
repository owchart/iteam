/*****************************************************************************\
*                                                                             *
* MainFrame.cs -  MainFrame functions, types, and definitions.                *
*                                                                             *
*               Version 1.00  ����                                          *
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
    /// ����ϵͳ
    /// </summary>
    public class MainFrame : UIXmlEx, IDisposable
    {
        /// <summary>
        /// ��������ϵͳ
        /// </summary>
        public MainFrame()
        {
        }

        /// <summary>
        /// �ش�
        /// </summary>
        private Dictionary<String, String> m_answers = new Dictionary<String, String>();

        /// <summary>
        /// ��ʼ��ť
        /// </summary>
        private ButtonA m_btnStart;

        /// <summary>
        /// �ӵ�ʱ��
        /// </summary>
        private DateTime m_bulletTime = DateTime.Now;

        /// <summary>
        /// ��������
        /// </summary>
        private int m_count = 0;

        /// <summary>
        /// ��ǰ������Ŀ
        /// </summary>
        public QuestionInfo m_currentQuestion;

        /// <summary>
        /// ��ǰʱ��
        /// </summary>
        public double m_currentTick;

        /// <summary>
        /// ����ʱ��
        /// </summary>
        private int m_examMinute = 15;

        /// <summary>
        /// �״�ִ��ʱ��
        /// </summary>
        private DateTime m_firstTime = DateTime.Now;

        /// <summary>
        /// IP��ַ
        /// </summary>
        private String m_ip = "47.100.16.237";

        /// <summary>
        /// �Ƿ����
        /// </summary>
        private bool m_isMatch = false;

        /// <summary>
        /// �ϴ�ʱ��
        /// </summary>
        private DateTime m_lastTime = DateTime.Now;

        /// <summary>
        /// �����
        /// </summary>
        private LabelA m_lblAlarm;

        /// <summary>
        /// ģʽ��
        /// </summary>
        private LabelA m_lblMode;

        /// <summary>
        /// ����ʱ
        /// </summary>
        private LabelA m_lblTime;

        /// <summary>
        /// ���Ϳ�
        /// </summary>
        private LabelA m_lblType;

        /// <summary>
        /// �Ƿ����
        /// </summary>
        private bool m_isOver;

        /// <summary>
        /// ������
        /// </summary>
        private List<QuestionInfo> m_oldQuestions = new List<QuestionInfo>();

        /// <summary>
        /// ������Ŀ
        /// </summary>
        private List<QuestionInfo> m_questions = new List<QuestionInfo>();

        /// <summary>
        /// �������
        /// </summary>
        private Random m_rd = new Random();

        /// <summary>
        /// ��Ӱʱ��
        /// </summary>
        private DateTime m_shadowTime = DateTime.Now;

        /// <summary>
        /// ����ͼ
        /// </summary>
        private Sky m_sky;

        /// <summary>
        /// ������
        /// </summary>
        private int m_tick;

        /// <summary>
        /// ��ʱ��
        /// </summary>
        private double m_totalTick;

        /// <summary>
        /// �ش��ı���
        /// </summary>
        public iTextBox m_txtAnswer;

        /// <summary>
        /// �����ı���
        /// </summary>
        private iTextBox m_txtQuestion;

        /// <summary>
        /// ��Ļ����
        /// </summary>
        private BarrageDiv m_barrageDiv;

        private int m_mode;

        /// <summary>
        /// ��ȡ������ģʽ
        /// </summary>
        public int Mode
        {
            get { return m_mode; }
            set { m_mode = value; }
        }

        /// <summary>
        /// ��ӵ�Ļ
        /// </summary>
        /// <param name="text">����</param>
        /// <param name="mode">ģʽ</param>
        /// <param name="speed">�ٶ�</param>
        public void AddBarrage(String text, int mode, int speed)
        {
            Barrage barrage = new Barrage();
            barrage.Text = text;
            barrage.Speed = speed;
            barrage.Mode = mode;
            m_barrageDiv.AddBarrage(barrage);
        }

        /// <summary>
        /// ���ӵ�ʧ��
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
        /// ��������¼�
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="timerID">���ID</param>
        private void CallTimerEvent(object sender, int timerID)
        {
            if (m_isOver)
            {
                return;
            }
            if (m_sky != null)
            {
                if (m_currentQuestion != null && m_mode >= 5 && m_currentQuestion.m_type != "����")
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
                        m_sky.Text = "׼��ը��";
                    }
                    TimeSpan ts2 = DateTime.Now - m_shadowTime;
                    int shadownTime = 1, totalSeconds2 = (int)ts2.TotalSeconds;
                    if (totalSeconds2 >= shadownTime)
                    {

                        m_shadowTime = DateTime.Now;
                    }
                    m_sky.OnTimer(timerID);
                }
                if (m_currentQuestion != null && (m_currentQuestion.m_type == "����" || m_currentQuestion.m_type == "����"))
                {
                    if (m_currentQuestion.m_answer == m_txtAnswer.Text)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            AddBarrage("�ش���ȷ", 0, 4 + i);
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
                    if (m_currentQuestion != null && m_currentQuestion.m_type == "����")
                    {
                        Native.ExportToImage(DataCenter.GetAppPath() + "\\�ɼ���ͼ.jpg");
                    }
                    m_lblTime.Text = "ʱ�䵽��";
                    m_txtAnswer.Text = m_currentQuestion.m_type;
                    m_currentTick = 0;
                    ChangeQuestion();
                }
                else
                {
                    if (m_currentQuestion.m_type == "����")
                    {
                        if (m_totalTick - m_currentTick > m_totalTick / 2)
                        {
                            m_txtQuestion.Text = "���ɼ�";
                            m_txtAnswer.ReadOnly = false;
                            if (m_txtAnswer.Text == "��Ѹ�ټ��������⴮����,�ڴ�������ʧʱ����ղ��Ǵ�����")
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
                    m_lblTime.Text = "��ʣ" + m_currentTick.ToString("0.00") + "�� ����ʱ" + finishTime.ToString("0.00") + "��";
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
                        m_txtAnswer.Text = "����ʱ�䵽,��ȴ����Խ��!";
                        Native.Invalidate();
                        m_isOver = true;
                    }
                }
            }
            if (m_mode == 5)
            {
                if (m_currentQuestion.m_type == "����" || m_currentQuestion.m_type == "����")
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
        /// ����¼�
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="mp">����</param>
        /// <param name="button">��ť</param>
        /// <param name="clicks">�������</param>
        /// <param name="delta">����ֵ/param>
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
                        m_lblMode.Text = "����";
                        AddBarrage("ѡ������Ѷ�", 1, 5);
                    }
                    else if (choose2.Opacity == 1)
                    {
                        m_lblMode.Text = "�м�";
                        AddBarrage("ѡ���м��Ѷ�",1, 5);
                    }
                    else if (choose3.Opacity == 1)
                    {
                        m_lblMode.Text = "�߼�";
                        AddBarrage("ѡ��߼��Ѷ�", 1, 5);
                    }
                    else if (choose4.Opacity == 1)
                    {
                        m_lblMode.Text = "Ӣ��";
                        AddBarrage("ѡ��Ӣ���Ѷ�", 1, 5);
                    }
                    else if (choose5.Opacity == 1)
                    {
                        m_lblMode.Text = "ʷʫ";
                        AddBarrage("ѡ��ʷʫ�Ѷ�", 1, 5);
                    }
                    else if (choose6.Opacity == 1)
                    {
                        m_lblMode.Text = "��˵";
                        AddBarrage("ѡ��ʷʫ�Ѷ�", 1, 5);
                    }
                }
            }
        }

        /// <summary>
        /// �л�����
        /// </summary>
        public void ChangeQuestion()
        {
            if (m_currentQuestion != null)
            {
                m_answers[m_currentQuestion.m_title] = m_txtAnswer.Text;
            }
            if (m_currentQuestion != null && (m_currentQuestion.m_type == "����" || m_currentQuestion.m_type == "����"))
            {
                if (m_currentQuestion.m_answer != m_txtAnswer.Text)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        AddBarrage("�ش����", 0, 4 + i);
                    }
                }
            }
            m_txtAnswer.ReadOnly = false;
            if (m_btnStart.Text == "��ʼ")
            {
                Thread thread = new Thread(new ThreadStart(CheckResult));
                thread.Start();
                //��������
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
                    AddBarrage("��ʼ����", 0, 4 + i);
                }
                GetDiv("divMode").Enabled = false;
                GetDiv("divMode").CanFocus = false;
                switch (m_lblMode.Text)
                {
                    case "����":
                        m_mode = 0;
                        break;
                    case "�м�":
                        m_mode = 1;
                        break;
                    case "�߼�":
                        m_mode = 2;
                        break;
                    case "Ӣ��":
                        m_mode = 3;
                        break;
                    case "ʷʫ":
                        m_mode = 4;
                        break;
                    case "��˵":
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
                        codingQuestion.m_type = "����";
                        codingQuestion.m_interval = 180;
                        codingQuestion.m_title = "���������ٶ��������for(int i = 0; i < 100; i++)(ǰ��select * from�ھ�����˼�ܣ�Ŀǰ30��Ϊ�ϸ�)";
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
                            if (question.m_type == "����")
                            {
                                m_questions.Add(question);
                                m_oldQuestions.Remove(question);
                                int random = m_rd.Next(0, 5);
                                //�Ӽ�����
                                //if (random == 0)
                                //{
                                //    QuestionInfo addSubQuestion = new QuestionInfo();
                                //    addSubQuestion.m_type = "����";
                                //    addSubQuestion.m_interval = 30;
                                //    int count = 5;
                                //    String num1 = GetRandomNum(count);
                                //    String num2 = GetRandomNum(count);
                                //    String op = "+";
                                //    addSubQuestion.m_title = num1 + op + num2;
                                //    addSubQuestion.m_answer = (CStr.ConvertStrToInt(num1) + CStr.ConvertStrToInt(num2)).ToString();
                                //    m_questions.Add(addSubQuestion);
                                //}
                                ////���俼��
                                //else if (m_mode != 5 && random == 1)
                                //{
                                //    QuestionInfo memoryQuestion = new QuestionInfo();
                                //    memoryQuestion.m_type = "����";
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
                                //    codingQuestion.m_type = "����";
                                //    codingQuestion.m_interval = 180;
                                //    codingQuestion.m_title = "���������ٶ��������for(int i = 0; i < 100; i++)(ǰ��select * from�ھ�����˼�ܣ�Ŀǰ30��Ϊ�ϸ�)";
                                //    m_questions.Add(codingQuestion);
                                //    coding = false;
                                //}
                                lastState = false;
                            }
                        }
                        else
                        {
                            if (question.m_type == "����")
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
                            if (m_oldQuestions[i].m_type == "����")
                            {
                                noKS = false;
                            }
                            else if (m_oldQuestions[i].m_type == "����")
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
            m_btnStart.Text = "��һ��";
            if (m_questions.Count > 0)
            {
                m_currentQuestion = m_questions[0];
                if (m_currentQuestion.m_type != "����")
                {
                    m_txtAnswer.Text = "";
                }
                else
                {
                    m_txtAnswer.Text = "�����";
                    m_txtAnswer.Text = "";
                }
                m_currentTick = m_currentQuestion.m_interval;
                if (m_currentQuestion.m_type == "����")
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
                            m_txtAnswer.Text = "���ڴ������лش�";
                            break;
                    }
                }
                else if (m_currentQuestion.m_type == "����")
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
                else if (m_currentQuestion.m_type == "����")
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
                else if (m_currentQuestion.m_type == "����")
                {
                    m_txtAnswer.Text = "��Ѹ�ټ��������⴮����,�ڴ�������ʧʱ����ղ��Ǵ�����";
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
                m_lblAlarm.Text = "����" + m_count.ToString() + "��";
                m_totalTick = m_currentTick;
                double finishTime = (double)((TimeSpan)(DateTime.Now - m_firstTime)).TotalMilliseconds / 1000;
                m_lblTime.Text = "��ʣ" + m_totalTick.ToString("0.00") + "�� ����ʱ" + finishTime.ToString("0.00") + "��";
                m_questions.Remove(m_currentQuestion);
                m_txtQuestion.Text = m_currentQuestion.m_title;
                if (m_currentQuestion.m_type == "����")
                {
                    m_lblType.Text = "����:����,��ʱ" + m_currentTick.ToString() + "��";
                }
                else
                {
                    m_lblType.Text = "����:" + m_currentQuestion.m_type + ",��ʱ" + m_currentTick.ToString() + "��";
                }
            }
        }

        /// <summary>
        /// �˳����򷽷�
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
                        m_txtAnswer.Text = "���Գɼ��ϸ񣬵ȴ���һ����̸!";
                        for (int i = 0; i < 5; i++)
                        {
                            AddBarrage("���Գɼ��ϸ񣬵ȴ���һ����̸!", 0, 4 + i);
                        }
                        break;
                    }
                    else if (examResult.StartsWith("butongguo"))
                    {
                        m_txtAnswer.Text = "�ǳ���Ǹ�����Գɼ����ϸ�лл����!";
                        for (int i = 0; i < 5; i++)
                        {
                            AddBarrage("�ǳ���Ǹ�����Գɼ����ϸ�лл����!", 0, 4 + i);
                        }
                        break;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// ��ȡ�����
        /// </summary>
        /// <param name="count">����</param>
        /// <returns>�ַ�</returns>
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
        /// �Ƿ��д�����ʾ
        /// </summary>
        /// <returns>�Ƿ���ʾ</returns>
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
        /// ���ID
        /// </summary>
        private int m_timerID = ControlA.GetNewTimerID();

        /// <summary>
        /// ����XML
        /// </summary>
        /// <param name="xmlPath">XML·��</param>
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
        /// �ػ��
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clicpRect">�ü�����</param>
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
            if (m_currentQuestion != null && m_currentQuestion.m_type == "����")
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
                FONT tFont = new FONT("΢���ź�", 100, true, false, true);
                SIZE tSize = paint.TextSize(strLine, tFont);
                RECT tRect = new RECT();
                tRect.left = (width - tSize.cx) / 2;
                tRect.top = (height - tSize.cy) / 2;
                tRect.right = tRect.left + tSize.cx;
                tRect.bottom = tRect.top + tSize.cy;
                paint.DrawText(strLine, COLOR.ARGB(200, 255, 255, 255), tFont, tRect);
            }
        }

        /// ע���¼�
        /// </summary>
        /// <param name="control">�ؼ�</param>
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
        /// ��ʾ��ʾ����
        /// </summary>
        /// <param name="text">�ı�</param>
        /// <param name="caption">����</param>
        /// <param name="uType">��ʽ</param>
        /// <returns>���</returns>
        public int ShowMessageBox(String text, String caption, int uType)
        {
            MessageBox.Show(text, caption);
            return 1;
        }


        /// <summary>
        /// ��Ϣ����
        /// </summary>
        /// <param name="m">��Ϣ</param>
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
    /// ������Ϣ
    /// </summary>
    public class QuestionInfo
    {
        /// <summary>
        /// ��
        /// </summary>
        public String m_answer;

        /// <summary>
        /// ���
        /// </summary>
        public int m_interval;

        /// <summary>
        /// ����
        /// </summary>
        public String m_title;

        /// <summary>
        /// ����
        /// </summary>
        public String m_type;
    }
}
