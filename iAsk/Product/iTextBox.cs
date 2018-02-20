/*****************************************************************************\
*                                                                             *
* TextBox.cs -  Text box functions, types, and definitions                    *
*                                                                             *
*               Version 4.00 ������                                       *
*                                                                             *
*               Copyright (c) 2016-2016, Lord's text box. All rights reserved.*
*               Checked 2016/9/25 by Lord.                                    *
*                                                                             *
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OwLib
{
    /// <summary>
    /// �ı���ؼ�
    /// </summary>
    public class iTextBox : DivA
    {
        #region Lord 2016/5/27
        /// <summary>
        /// �����ؼ�
        /// </summary>
        public iTextBox()
        {
            BorderColor = COLOR.ARGB(122, 156, 40);
            BackColor = COLOR.ARGB(150, 0, 0, 0);
            ForeColor = COLOR.ARGB(255, 255, 255);
            this.Cursor = CursorsA.IBeam;
            SIZE size = new SIZE(100, 20);
            Size = size;
            TabStop = false;
        }

        /// <summary>
        /// �����Ƿ���
        /// </summary>
        protected bool m_isKeyDown;

        /// <summary>
        /// �Ƿ���갴��
        /// </summary>
        protected bool m_isMouseDown;

        /// <summary>
        /// ����ƫ����
        /// </summary>
        protected int m_offsetX = 0;

        /// <summary>
        /// �����
        /// </summary>
        private Random m_random = new Random();

        /// <summary>
        /// ���־��η�Χ
        /// </summary>
        protected List<RECTF> m_ranges = new List<RECTF>();

        /// <summary>
        /// ����ջ
        /// </summary>
        protected Stack<String> m_redoStack = new Stack<String>();

        /// <summary>
        /// �Ƿ���ʾ���
        /// </summary>
        protected bool m_showCursor = false;

        /// <summary>
        /// ��ʼ�ƶ�������
        /// </summary>
        protected int m_startMovingIndex = -1;

        /// <summary>
        /// �����ƶ�������
        /// </summary>
        protected int m_stopMovingIndex = -1;

        /// <summary>
        /// �����Ƿ�ı�
        /// </summary>
        protected bool m_textChanged = false;

        /// <summary>
        /// �����˸Ƶ��
        /// </summary>
        protected int TICK = 500;

        /// <summary>
        /// ���ID
        /// </summary>
        private int m_timerID = GetNewTimerID();

        /// <summary>
        /// ����ջ
        /// </summary>
        protected Stack<String> m_undoStack = new Stack<String>();

        /// <summary>
        /// ���ִ�С
        /// </summary>
        protected List<SIZEF> m_wordsSize = new List<SIZEF>();

        /// <summary>
        /// ��ȡ����
        /// </summary>
        public virtual int LinesCount
        {
            get { return m_lines.Count; }
        }

        protected int m_lineHeight = 20;

        /// <summary>
        /// ��ȡ�������и�
        /// </summary>
        public virtual int LineHeight
        {
            get { return m_lineHeight; }
            set { m_lineHeight = value; }
        }

        protected List<WordLineA> m_lines = new List<WordLineA>();

        /// <summary>
        /// ��ȡ����
        /// </summary>
        public virtual List<WordLineA> Lines
        {
            get { return m_lines; }
        }

        private MainFrame m_mainFrame;

        /// <summary>
        /// ��ȡ������������
        /// </summary>
        public MainFrame MainFrame
        {
            get { return m_mainFrame; }
            set { m_mainFrame = value; }
        }

        protected bool m_multiline = false;

        /// <summary>
        /// ��ȡ�������Ƿ������ʾ
        /// </summary>
        public virtual bool Multiline
        {
            get { return m_multiline; }
            set
            {
                if (m_multiline != value)
                {
                    m_multiline = value;
                    m_textChanged = true;
                }
                ShowVScrollBar = m_multiline;
            }
        }

        protected char m_passwordChar;

        /// <summary>
        /// ��ȡ�����������ַ�
        /// </summary>
        public virtual char PasswordChar
        {
            get { return m_passwordChar; }
            set
            {
                m_passwordChar = value;
                m_textChanged = true;
            }
        }

        protected bool m_readOnly = false;

        /// <summary>
        /// ��ȡ�������Ƿ�ֻ��
        /// </summary>
        public virtual bool ReadOnly
        {
            get { return m_readOnly; }
            set { m_readOnly = value; }
        }

        protected bool m_rightToLeft;

        /// <summary>
        /// ��ȡ�������Ƿ�����������
        /// </summary>
        public virtual bool RightToLeft
        {
            get { return m_rightToLeft; }
            set
            {
                m_rightToLeft = value;
                m_textChanged = true;
            }
        }

        /// <summary>
        /// ��ȡѡ�е�����
        /// </summary>
        public virtual String SelectionText
        {
            get
            {
                String text = Text;
                if (text == null)
                {
                    text = "";
                }
                int textLength = text.Length;
                if (textLength > 0 && m_selectionStart != textLength)
                {
                    String selectedText = text.Substring(m_selectionStart, m_selectionLength);
                    return selectedText;
                }
                return String.Empty;
            }
        }

        protected long m_selectionBackColor = COLOR.ARGB(10, 36, 106);

        /// <summary>
        /// ��ȡ������ѡ�еı���ɫ
        /// </summary>
        public virtual long SelectionBackColor
        {
            get { return m_selectionBackColor; }
            set { m_selectionBackColor = value; }
        }

        protected long m_selectionForeColor = COLOR.ARGB(255, 255, 255);

        /// <summary>
        /// ��ȡ������ѡ�е�ǰ��ɫ
        /// </summary>
        public virtual long SelectionForeColor
        {
            get { return m_selectionForeColor; }
            set { m_selectionForeColor = value; }
        }

        protected int m_selectionLength;

        /// <summary>
        /// ��ȡ������ѡ�г���
        /// </summary>
        public virtual int SelectionLength
        {
            get { return m_selectionLength; }
            set { m_selectionLength = value; }
        }

        protected int m_selectionStart = -1;

        /// <summary>
        /// ��ȡ������ѡ�п�ʼλ��
        /// </summary>
        public virtual int SelectionStart
        {
            get { return m_selectionStart; }
            set
            {
                m_selectionStart = value;
                if (m_selectionStart > Text.Length)
                {
                    m_selectionStart = Text.Length;
                }
            }
        }

        protected String m_tempText;

        /// <summary>
        /// ��ȡ��������ʱ����
        /// </summary>
        public virtual String TempText
        {
            get { return m_tempText; }
            set { m_tempText = value; }
        }

        protected long m_tempTextForeColor = COLOR.DISABLEDCONTROLTEXT;

        /// <summary>
        /// ��ȡ��������ʱ���ֵ���ɫ
        /// </summary>
        public virtual long TempTextForeColor
        {
            get { return m_tempTextForeColor; }
            set { m_tempTextForeColor = value; }
        }

        protected HorizontalAlignA m_textAlign = HorizontalAlignA.Left;

        /// <summary>
        /// ��ȡ���������ݵĺ���������ʽ
        /// </summary>
        public virtual HorizontalAlignA TextAlign
        {
            get { return m_textAlign; }
            set { m_textAlign = value; }
        }

        protected bool m_wordWrap = false;

        /// <summary>
        /// ��ȡ�����ö��б༭�ؼ��Ƿ���������
        /// </summary>
        public virtual bool WordWrap
        {
            get { return m_wordWrap; }
            set
            {
                if (m_wordWrap != value)
                {
                    m_wordWrap = value;
                    m_textChanged = true;
                }
                ShowHScrollBar = !m_wordWrap;
            }
        }

        /// <summary>
        /// �ж��Ƿ�����ظ�
        /// </summary>
        /// <returns>�Ƿ�����ظ�</returns>
        public bool CanRedo()
        {
            if (m_redoStack.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// �ж��Ƿ���Գ���
        /// </summary>
        /// <returns>�Ƿ���Գ���</returns>
        public bool CanUndo()
        {
            if (m_undoStack.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// ����
        /// </summary>
        public void ClearRedoUndo()
        {
            m_undoStack.Clear();
            m_redoStack.Clear();
        }

        /// <summary>
        /// ��������ƶ�
        /// </summary>
        public void CursorDown()
        {
            ControlHost host = Native.Host;
            int scol = -1, srow = -1;
            int rangeSize = m_ranges.Count;
            int start = m_selectionStart + m_selectionLength < rangeSize - 1 ? m_selectionStart + m_selectionLength : rangeSize - 1;
            if (host.IsKeyPress(0x10))
            {
                start = m_stopMovingIndex;
            }
            else
            {
                if (m_selectionLength > 0)
                {
                    m_selectionLength = 1;
                }
            }
            int lineCount = m_lines.Count;
            bool check = false;
            for (int i = 0; i < lineCount; i++)
            {
                WordLineA line = m_lines[i];
                for (int j = line.m_start; j <= line.m_end; j++)
                {
                    if (j >= start && j < rangeSize)
                    {
                        int col = j - line.m_start;
                        if (j == start)
                        {
                            if (i != 0 && j == line.m_start)
                            {
                                check = true;
                                srow = i - 1;
                                scol = line.m_end + 1;
                            }
                            else
                            {
                                if (i != lineCount - 1)
                                {
                                    check = true;
                                    int idx = j - line.m_start;
                                    scol = m_lines[i + 1].m_start + idx + 1;
                                    srow = i;
                                    continue;
                                }
                            }
                        }
                        if (check)
                        {
                            if (i == srow + 1)
                            {
                                if (host.IsKeyPress(0x10))
                                {
                                    SetMovingIndex(m_startMovingIndex, j);
                                }
                                else
                                {
                                    if (scol > line.m_end)
                                    {
                                        scol = line.m_end + 1;
                                    }
                                    m_selectionStart = scol;
                                    m_selectionLength = 0;
                                    m_startMovingIndex = m_selectionStart;
                                    m_stopMovingIndex = m_selectionStart;
                                }
                                m_showCursor = true;
                                StartTimer(m_timerID, TICK);
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ����ƶ������Ҷ�
        /// </summary>
        public void CursorEnd()
        {
            ControlHost host = Native.Host;
            int rangeSize = m_ranges.Count;
            int start = m_selectionStart + m_selectionLength < rangeSize - 1 ? m_selectionStart + m_selectionLength : rangeSize - 1;
            if (host.IsKeyPress(0x10))
            {
                start = m_stopMovingIndex;
            }
            int lineCount = m_lines.Count;
            for (int i = 0; i < lineCount; i++)
            {
                WordLineA line = m_lines[i];
                for (int j = line.m_start; j <= line.m_end; j++)
                {
                    if (j == start)
                    {
                        if (j == line.m_start && i > 0)
                        {
                            line = m_lines[i - 1];
                        }
                        if (host.IsKeyPress(0x10))
                        {
                            SetMovingIndex(m_startMovingIndex, line.m_end + 1);
                        }
                        else
                        {
                            m_selectionStart = line.m_end + 1;
                            m_selectionLength = 0;
                            m_startMovingIndex = m_selectionStart;
                            m_stopMovingIndex = m_selectionStart;
                        }
                        m_showCursor = true;
                        StartTimer(m_timerID, TICK);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// ����ƶ��������
        /// </summary>
        public void CursorHome()
        {
            ControlHost host = Native.Host;
            int rangeSize = m_ranges.Count;
            int start = m_selectionStart < rangeSize - 1 ? m_selectionStart : rangeSize - 1;
            if (host.IsKeyPress(0x10))
            {
                start = m_stopMovingIndex;
            }
            int lineCount = m_lines.Count;
            for (int i = 0; i < lineCount; i++)
            {
                WordLineA line = m_lines[i];
                for (int j = line.m_start; j <= line.m_end; j++)
                {
                    if (j == start)
                    {
                        if (j == line.m_start && i > 0)
                        {
                            line = m_lines[i - 1];
                        }
                        if (host.IsKeyPress(0x10))
                        {
                            SetMovingIndex(m_startMovingIndex, line.m_start + 1);
                        }
                        else
                        {
                            m_selectionStart = line.m_start + 1;
                            m_selectionLength = 0;
                            m_startMovingIndex = m_selectionStart;
                            m_stopMovingIndex = m_selectionStart;
                        }
                        m_showCursor = true;
                        StartTimer(m_timerID, TICK);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// ��������ƶ�
        /// </summary>
        public void CursorLeft()
        {
            ControlHost host = Native.Host;
            if (host.IsKeyPress(0x10))
            {
                SetMovingIndex(m_startMovingIndex, m_stopMovingIndex - 1);
            }
            else
            {
                if (m_selectionStart > 0)
                {
                    m_selectionStart -= 1;
                }
                m_selectionLength = 0;
                m_startMovingIndex = m_selectionStart;
                m_stopMovingIndex = m_selectionStart;
            }
        }

        /// <summary>
        /// ��������ƶ�
        /// </summary>
        public void CursorRight()
        {
            ControlHost host = Native.Host;
            if (host.IsKeyPress(0x10))
            {
                SetMovingIndex(m_startMovingIndex, m_stopMovingIndex + 1);
            }
            else
            {
                int rangeSize = m_ranges.Count;
                int start = m_selectionStart + m_selectionLength < rangeSize - 1 ? m_selectionStart + m_selectionLength : rangeSize - 1;
                if (start < rangeSize)
                {
                    m_selectionStart = start + 1;
                }
                m_selectionLength = 0;
                m_startMovingIndex = m_selectionStart;
                m_stopMovingIndex = m_selectionStart;
            }
        }

        /// <summary>
        /// ��������ƶ�
        /// </summary>
        public void CursorUp()
        {
            ControlHost host = Native.Host;
            int scol = -1, srow = -1;
            int rangeSize = m_ranges.Count;
            int start = m_selectionStart < rangeSize - 1 ? m_selectionStart : rangeSize - 1;
            if (host.IsKeyPress(0x10))
            {
                start = m_stopMovingIndex;
            }
            else
            {
                if (m_selectionLength > 0)
                {
                    m_selectionLength = 1;
                }
            }
            int lineCount = m_lines.Count;
            bool check = false;
            for (int i = lineCount - 1; i >= 0; i--)
            {
                WordLineA line = m_lines[i];
                for (int j = line.m_end; j >= line.m_start; j--)
                {
                    if (j >= 0 && j <= start)
                    {
                        int col = j - line.m_start;
                        if (i != 0 && j == start)
                        {
                            check = true;
                            if (i != lineCount - 1 && j == line.m_start)
                            {
                                srow = i;
                                scol = m_lines[i - 1].m_start;
                            }
                            else
                            {
                                int idx = j - line.m_start;
                                scol = m_lines[i - 1].m_start + idx - 1;
                                if (scol < 0)
                                {
                                    scol = 0;
                                }
                                srow = i;
                            }
                            continue;
                        }
                        if (check)
                        {
                            if (i == srow - 1 && col <= scol)
                            {
                                if (host.IsKeyPress(0x10))
                                {
                                    SetMovingIndex(m_startMovingIndex, j);
                                }
                                else
                                {
                                    if (scol > line.m_end)
                                    {
                                        scol = line.m_end + 1;
                                    }
                                    m_selectionStart = scol;
                                    m_selectionLength = 0;
                                    m_startMovingIndex = m_selectionStart;
                                    m_stopMovingIndex = m_selectionStart;
                                }
                                m_showCursor = true;
                                StartTimer(m_timerID, TICK);
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ɾ���ַ�
        /// </summary>
        public virtual void DeleteWord()
        {
            try
            {
                String text = Text;
                if (text == null)
                {
                    text = "";
                }
                int textLength = text.Length;
                if (textLength > 0)
                {
                    int oldLines = m_lines.Count;
                    String left = "", right = "";
                    int rightIndex = -1;
                    if (m_selectionLength > 0)
                    {
                        left = m_selectionStart > 0 ? text.Substring(0, m_selectionStart) : "";
                        rightIndex = m_selectionStart + m_selectionLength;
                    }
                    else
                    {
                        left = m_selectionStart > 0 ? text.Substring(0, m_selectionStart - 1) : "";
                        rightIndex = m_selectionStart + m_selectionLength;
                        if (m_selectionStart > 0)
                        {
                            m_selectionStart -= 1;
                        }
                    }
                    if (rightIndex < textLength)
                    {
                        right = text.Substring(rightIndex);
                    }
                    String newText = left + right;
                    m_text = newText;
                    textLength = newText.Length;
                    if (textLength == 0)
                    {
                        m_selectionStart = 0;
                    }
                    else
                    {
                        if (m_selectionStart > textLength)
                        {
                            m_selectionStart = textLength;
                        }
                    }
                    m_selectionLength = 0;
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// ���ٷ���
        /// </summary>
        public override void Dispose()
        {
            if (!IsDisposed)
            {
                StopTimer(m_timerID);
                m_lines.Clear();
                m_ranges.Clear();
                m_redoStack.Clear();
                m_undoStack.Clear();
                m_wordsSize.Clear();
            }
            base.Dispose();
        }

        /// <summary>
        /// ��ȡ���ݵĸ߶�
        /// </summary>
        /// <returns>�߶�</returns>
        public override int GetContentHeight()
        {
            int hmax = base.GetContentHeight();
            int cheight = 0;
            int rangeSize = m_ranges.Count;
            for (int i = 0; i < rangeSize; i++)
            {
                if (cheight < m_ranges[i].bottom)
                {
                    cheight = (int)m_ranges[i].bottom;
                }
            }
            return hmax > cheight ? hmax : cheight;
        }

        /// <summary>
        /// ��ȡ���ݵĿ��
        /// </summary>
        /// <returns>���</returns>
        public override int GetContentWidth()
        {
            int wmax = base.GetContentWidth();
            int cwidth = 0;
            int rangeSize = m_ranges.Count;
            for (int i = 0; i < rangeSize; i++)
            {
                if (cwidth < m_ranges[i].right)
                {
                    cwidth = (int)m_ranges[i].right;
                }
            }
            return wmax > cwidth ? wmax : cwidth;
        }

        /// <summary>
        /// ��ȡ�ؼ�����
        /// </summary>
        /// <returns>�ؼ�����</returns>
        public override String GetControlType()
        {
            return "TextBox";
        }

        /// <summary>
        /// ��ȡ����ֵ
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="value">��������ֵ</param>
        /// <param name="type">������������</param>
        public override void GetProperty(String name, ref String value, ref String type)
        {
            if (name == "lineheight")
            {
                type = "int";
                value = CStr.ConvertIntToStr(LineHeight);
            }
            else if (name == "multiline")
            {
                type = "bool";
                value = CStr.ConvertBoolToStr(Multiline);
            }
            else if (name == "passwordchar")
            {
                type = "text";
                value = PasswordChar.ToString();
            }
            else if (name == "readonly")
            {
                type = "bool";
                value = CStr.ConvertBoolToStr(ReadOnly);
            }
            else if (name == "righttoleft")
            {
                type = "bool";
                value = CStr.ConvertBoolToStr(RightToLeft);
            }
            else if (name == "selectionbackcolor")
            {
                type = "color";
                value = CStr.ConvertColorToStr(SelectionBackColor);
            }
            else if (name == "selectionforecolor")
            {
                type = "color";
                value = CStr.ConvertColorToStr(SelectionForeColor);
            }
            else if (name == "temptext")
            {
                type = "text";
                value = TempText;
            }
            else if (name == "temptextforecolor")
            {
                type = "color";
                value = CStr.ConvertColorToStr(TempTextForeColor);
            }
            else if (name == "textalign")
            {
                type = "enum:HorizontalAlignA";
                value = CStr.ConvertHorizontalAlignToStr(TextAlign);
            }
            else if (name == "wordwrap")
            {
                type = "bool";
                value = CStr.ConvertBoolToStr(WordWrap);
            }
            else
            {
                base.GetProperty(name, ref value, ref type);
            }
        }

        /// <summary>
        /// ��ȡ���������б�
        /// </summary>
        /// <returns>���������б�</returns>
        public override List<String> GetPropertyNames()
        {
            List<String> propertyNames = base.GetPropertyNames();
            propertyNames.AddRange(new String[] { "LineHeight", "Multiline", "PasswordChar", "ReadOnly", "RightToLeft", "SelectionBackColor", "SelectionForeColor", "TempText", "TempTextForeColor", "TextAlign", "WordWrap" });
            return propertyNames;
        }

        private bool M138(int indexTop, int indexBottom, int cell, int floor, int lineHeight, double visiblePercent)
        {
            if (indexTop < cell)
            {
                indexTop = cell;
            }
            else if (indexTop > floor)
            {
                indexTop = floor;
            }
            if (indexBottom < cell)
            {
                indexBottom = cell;
            }
            else if (indexBottom > floor)
            {
                indexBottom = floor;
            }
            if (indexBottom - indexTop > lineHeight * visiblePercent)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// �ж��ַ������������Ƿ�ɼ�
        /// </summary>
        /// <param name="index">�ַ�����</param>
        /// <param name="visiblePercent">�ɼ��ٷֱ�</param>
        /// <returns>�Ƿ�ɼ�</returns>
        public bool IsLineVisible(int index, double visiblePercent)
        {
            int rangeSize = m_ranges.Count;
            if (rangeSize > 0)
            {
                if (index >= 0 && index < rangeSize)
                {
                    int top = 0, scrollV = 0, sch = 0;
                    HScrollBarA hScrollBar = HScrollBar;
                    VScrollBarA vScrollBar = VScrollBar;
                    if (hScrollBar != null && hScrollBar.Visible)
                    {
                        sch = hScrollBar.Height;
                    }
                    if (vScrollBar != null && vScrollBar.Visible)
                    {
                        scrollV = -vScrollBar.Pos;
                    }
                    top = scrollV;
                    int cell = 1;
                    int floor = Height - cell - sch - 1;
                    RECTF indexRect = m_ranges[index];
                    int indexTop = (int)indexRect.top + scrollV;
                    int indexBottom = (int)indexRect.bottom + scrollV;
                    return M138(indexTop, indexBottom, cell, floor, m_lineHeight, visiblePercent);
                }
            }
            return false;
        }


        /// <summary>
        /// �����ַ�
        /// </summary>
        /// <param name="str">�ַ���</param>
        public virtual void InsertWord(String str)
        {
            try
            {
                String text = Text;
                if (text == null)
                {
                    text = "";
                }
                if (text.Length == 0 || m_selectionStart == text.Length)
                {
                    text = text + str;
                    m_text = text;
                    m_selectionStart = text.Length;
                }
                else
                {
                    int sIndex = m_selectionStart > text.Length ? text.Length : m_selectionStart;
                    String left = sIndex > 0 ? text.Substring(0, sIndex) : "";
                    String right = "";
                    int rightIndex = m_selectionStart + (m_selectionLength == 0 ? 0 : m_selectionLength);
                    if (rightIndex < text.Length)
                    {
                        right = text.Substring(rightIndex);
                    }
                    text = left + str + right;
                    m_text = text;
                    m_selectionStart += str.Length;
                    if (m_selectionStart > text.Length)
                    {
                        m_selectionStart = text.Length;
                    }
                    m_selectionLength = 0;
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// �ı����뷽��
        /// </summary>
        /// <param name="ch">����</param>
        public override void OnChar(char ch)
        {
            if (!m_readOnly)
            {
                base.OnChar(ch);
                ControlHost host = Native.Host;
                if (!host.IsKeyPress(0x11))
                {
                    int oldLines = m_lines.Count;
                    if (ch != 8 || (!m_multiline && ch == 13))
                    {
                        String redotext = Text;
                        InsertWord(ch.ToString());
                        OnTextChanged();
                        if (m_textChanged)
                        {
                            if (redotext != null)
                            {
                                m_undoStack.Push(redotext);
                            }
                        }
                    }
                    Invalidate();
                    int newLines = m_lines.Count;
                    if (newLines != oldLines)
                    {
                        VScrollBarA vScrollBar = VScrollBar;
                        if (vScrollBar != null)
                        {
                            vScrollBar.Pos += m_lineHeight * (newLines - oldLines);
                            Invalidate();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        public override void OnCopy()
        {
            String selectionText = SelectionText;
            if (selectionText != null && selectionText.Length > 0)
            {
                ControlHost host = Native.Host;
                host.Copy(selectionText);
                base.OnCopy();
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        public override void OnCut()
        {
            if (!m_readOnly)
            {
                OnCopy();
                int oldLines = m_lines.Count;
                String redotext = Text;
                DeleteWord();
                OnTextChanged();
                if (m_textChanged)
                {
                    if (redotext != null)
                    {
                        m_undoStack.Push(redotext);
                    }
                }
                Invalidate();
                int newLines = m_lines.Count;
                if (newLines != oldLines)
                {
                    VScrollBarA vScrollBar = VScrollBar;
                    if (vScrollBar != null)
                    {
                        vScrollBar.Pos += m_lineHeight * (newLines - oldLines);
                        Invalidate();
                    }
                }
                base.OnCut();
            }
        }

        /// <summary>
        /// ��ȡ���㷽��
        /// </summary>
        public override void OnGotFocus()
        {
            base.OnGotFocus();
            m_showCursor = true;
            Invalidate();
            StartTimer(m_timerID, TICK);
        }

        /// <summary>
        /// ���̷���
        /// </summary>
        /// <param name="key">����</param>
        public override void OnKeyDown(char key)
        {
            m_isKeyDown = true;
            ControlHost host = Native.Host;
            if (host.IsKeyPress(0x11))
            {
                base.OnKeyDown(key);
                //ȫѡ
                if (key == 65)
                {
                    SelectAll();
                }
                //����
                else if (key == 89)
                {
                    Redo();
                }
                //����
                else if (key == 90)
                {
                    Undo();
                }
            }
            else
            {
                if (key >= 35 && key <= 40)
                {
                    if (key == 38 || key == 40)
                    {
                        CallKeyEvents(EVENTID.KEYDOWN, key);
                        if (m_lines.Count > 1)
                        {
                            int offset = 0;
                            //��������ƶ�
                            if (key == 38)
                            {
                                CursorUp();
                                if (!IsLineVisible(m_stopMovingIndex, 0.6))
                                {
                                    offset = -m_lineHeight;
                                }
                            }
                            //��������ƶ�
                            else if (key == 40)
                            {
                                CursorDown();
                                if (!IsLineVisible(m_stopMovingIndex, 0.6))
                                {
                                    offset = m_lineHeight;
                                }
                            }
                            VScrollBarA vScrollBar = VScrollBar;
                            if (vScrollBar != null && vScrollBar.Visible)
                            {
                                vScrollBar.Pos += offset;
                                vScrollBar.Update();
                            }
                        }
                    }
                    else
                    {
                        base.OnKeyDown(key);
                        //����ƶ������Ҷ�
                        if (key == 35)
                        {
                            CursorEnd();
                        }
                        //����ƶ��������
                        else if (key == 36)
                        {
                            CursorHome();
                        }
                        //��������ƶ�
                        else if (key == 37)
                        {
                            CursorLeft();
                        }
                        //��������ƶ�
                        else if (key == 39)
                        {
                            CursorRight();
                        }
                    }
                }
                else
                {
                    base.OnKeyDown(key);
                    //ȡ������
                    if (key == 27)
                    {
                        Focused = false;
                    }
                    //ɾ��
                    else if (key == 8 || key == 46)
                    {
                        if (!m_readOnly)
                        {
                            int oldLines = m_lines.Count;
                            String redotext = Text;
                            DeleteWord();
                            OnTextChanged();
                            if (m_textChanged)
                            {
                                if (redotext != null)
                                {
                                    m_undoStack.Push(redotext);
                                }
                            }
                            Invalidate();
                            int newLines = m_lines.Count;
                            if (newLines != oldLines)
                            {
                                VScrollBarA vScrollBar = VScrollBar;
                                if (vScrollBar != null)
                                {
                                    vScrollBar.Pos += m_lineHeight * (newLines - oldLines);
                                    Invalidate();
                                }
                            }
                        }
                    }
                }
            }
            Invalidate();
        }

        /// <summary>
        /// ����̧�𷽷�
        /// </summary>
        /// <param name="key">����</param>
        public override void OnKeyUp(char key)
        {
            base.OnKeyUp(key);
            ControlHost host = Native.Host;
            if (!host.IsKeyPress(0x10) && !m_isMouseDown)
            {
                m_startMovingIndex = m_selectionStart;
                m_stopMovingIndex = m_selectionStart;
            }
            m_isKeyDown = false;
        }

        /// <summary>
        /// ��ʧ���㷽��
        /// </summary>
        public override void OnLostFocus()
        {
            base.OnLostFocus();
            StopTimer(m_timerID);
            m_isKeyDown = false;
            m_showCursor = false;
            m_selectionLength = 0;
            Invalidate();
        }

        /// <summary>
        /// ��갴�·���
        /// </summary>
        /// <param name="mp">�������</param>
        /// <param name="button">��ť</param>
        /// <param name="clicks">�������</param>
        /// <param name="delta">����ֵ</param>
        public override void OnMouseDown(POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            base.OnMouseDown(mp, button, clicks, delta);
            if (button == MouseButtonsA.Left)
            {
                //����
                if (clicks == 1)
                {
                    int sIndex = -1;
                    int linesCount = m_lines.Count;
                    m_selectionLength = 0;
                    m_startMovingIndex = -1;
                    m_stopMovingIndex = -1;
                    if (linesCount > 0)
                    {
                        HScrollBarA hScrollBar = HScrollBar;
                        VScrollBarA vScrollBar = VScrollBar;
                        int scrollH = (hScrollBar != null && hScrollBar.Visible) ? hScrollBar.Pos : 0;
                        int scrollV = (vScrollBar != null && vScrollBar.Visible) ? vScrollBar.Pos : 0;
                        scrollH += m_offsetX;
                        int x = mp.x + scrollH, y = mp.y + scrollV;
                        RECTF lastRange = new RECTF();
                        int rangeSize = m_ranges.Count;
                        if (rangeSize > 0)
                        {
                            lastRange = m_ranges[rangeSize - 1];
                        }
                        for (int i = 0; i < linesCount; i++)
                        {
                            WordLineA line = m_lines[i];
                            for (int j = line.m_start; j <= line.m_end; j++)
                            {
                                RECTF rect = m_ranges[j];
                                if (i == linesCount - 1)
                                {
                                    rect.bottom += 3;
                                }
                                if (y >= rect.top && y <= rect.bottom)
                                {
                                    float sub = (rect.right - rect.left) / 2;
                                    if ((x >= rect.left - sub && x <= rect.right - sub)
                                        || (j == line.m_start && x <= rect.left + sub)
                                        || (j == line.m_end && x >= rect.right - sub))
                                    {
                                        if (j == line.m_end && x >= rect.right - sub)
                                        {
                                            sIndex = j + 1;
                                        }
                                        else
                                        {
                                            sIndex = j;
                                        }
                                        break;
                                    }
                                }
                            }
                            if (sIndex != -1)
                            {
                                break;
                            }
                        }
                        if (sIndex == -1)
                        {
                            if ((y >= lastRange.top && y <= lastRange.bottom && x > lastRange.right) || (y >= lastRange.bottom))
                            {
                                sIndex = rangeSize;
                            }
                        }
                    }
                    if (sIndex != -1)
                    {
                        m_selectionStart = sIndex;
                    }
                    else
                    {
                        m_selectionStart = 0;
                    }
                    m_startMovingIndex = m_selectionStart;
                    m_stopMovingIndex = m_selectionStart;
                }
                //˫��
                else if (clicks == 2)
                {
                    if (!m_multiline)
                    {
                        SelectAll();
                    }
                }
            }
            m_isMouseDown = true;
            m_showCursor = true;
            StartTimer(m_timerID, TICK);
            Invalidate();
        }

        /// <summary>
        /// ����ƶ�����
        /// </summary>
        /// <param name="mp">�������</param>
        /// <param name="button">��ť</param>
        /// <param name="clicks">�������</param>
        /// <param name="delta">����ֵ</param>
        public override void OnMouseMove(POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            base.OnMouseMove(mp, button, clicks, delta);
            if (m_isMouseDown)
            {
                int linesCount = m_lines.Count;
                if (linesCount > 0)
                {
                    int eIndex = -1;
                    HScrollBarA hScrollBar = HScrollBar;
                    VScrollBarA vScrollBar = VScrollBar;
                    int scrollH = (hScrollBar != null && hScrollBar.Visible) ? hScrollBar.Pos : 0;
                    int scrollV = (vScrollBar != null && vScrollBar.Visible) ? vScrollBar.Pos : 0;
                    scrollH += m_offsetX;
                    POINT point = mp;
                    if (point.x < 0)
                    {
                        point.x = 0;
                    }
                    if (point.y < 0)
                    {
                        point.y = 0;
                    }
                    int x = point.x + scrollH, y = point.y + scrollV;
                    RECTF lastRange = new RECTF();
                    int rangeSize = m_ranges.Count;
                    if (rangeSize > 0)
                    {
                        lastRange = m_ranges[rangeSize - 1];
                    }
                    for (int i = 0; i < linesCount; i++)
                    {
                        WordLineA line = m_lines[i];
                        for (int j = line.m_start; j <= line.m_end; j++)
                        {
                            RECTF rect = m_ranges[j];
                            if (i == linesCount - 1)
                            {
                                rect.bottom += 3;
                            }
                            if (eIndex == -1)
                            {
                                if (y >= rect.top && y <= rect.bottom)
                                {
                                    float sub = (rect.right - rect.left) / 2;
                                    if ((x >= rect.left - sub && x <= rect.right - sub)
                                        || (j == line.m_start && x <= rect.left + sub)
                                        || (j == line.m_end && x >= rect.right - sub))
                                    {
                                        if (j == line.m_end && x >= rect.right - sub)
                                        {
                                            eIndex = j + 1;
                                        }
                                        else
                                        {
                                            eIndex = j;
                                        }
                                    }
                                }
                            }
                        }
                        if (eIndex != -1)
                        {
                            break;
                        }
                    }
                    if (eIndex != -1)
                    {
                        m_stopMovingIndex = eIndex;
                    }
                    if (m_startMovingIndex == m_stopMovingIndex)
                    {
                        m_selectionStart = m_startMovingIndex;
                        m_selectionLength = 0;
                    }
                    else
                    {
                        m_selectionStart = m_startMovingIndex < m_stopMovingIndex ? m_startMovingIndex : m_stopMovingIndex;
                        m_selectionLength = Math.Abs(m_startMovingIndex - m_stopMovingIndex);
                    }
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// ���̧�𷽷�
        /// </summary>
        /// <param name="mp">�������</param>
        /// <param name="button">��ť</param>
        /// <param name="clicks">�������</param>
        /// <param name="delta">����ֵ</param>
        public override void OnMouseUp(POINT mp, MouseButtonsA button, int clicks, int delta)
        {
            m_isMouseDown = false;
            base.OnMouseUp(mp, button, clicks, delta);
        }

        /// <summary>
        /// ���Ʊ�������
        /// </summary>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clipRect">�ü�����</param>
        public override void OnPaintBackground(CPaint paint, RECT clipRect)
        {
            int width = Width, height = Height;
            RECT drawRect = new RECT(0, 0, width, height);
            paint.FillRoundRect(GetPaintingBackColor(), drawRect, 8);
        }

        /// <summary>
        /// ���Ʊ��߷���
        /// </summary>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clipRect">�ü�����</param>
        public override void OnPaintBorder(CPaint paint, RECT clipRect)
        {
            int width = Width, height = Height;
            RECT drawRect = new RECT(0, 0, width, height);
            paint.DrawRoundRect(GetPaintingBorderColor(), 1, 0, drawRect, 8);
        }

        /// <summary>
        /// �ػ�ǰ������
        /// </summary>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clipRect">�ü�����</param>
        public override void OnPaintForeground(CPaint paint, RECT clipRect)
        {
            int width = Width, height = Height;
            if (width > 0 && height > 0)
            {
                int lineHeight = m_multiline ? m_lineHeight : height;
                PADDING padding = Padding;
                ControlHost host = Native.Host;
                RECT rect = new RECT(0, 0, width, height);
                FONT font = Font;
                SIZEF tSize = paint.TextSizeF(" ", font);
                HScrollBarA hScrollBar = HScrollBar;
                VScrollBarA vScrollBar = VScrollBar;
                int vWidth = (vScrollBar != null && vScrollBar.Visible) ? (vScrollBar.Width - padding.left - padding.right) : 0;
                int scrollH = ((hScrollBar != null && hScrollBar.Visible) ? hScrollBar.Pos : 0);
                int scrollV = ((vScrollBar != null && vScrollBar.Visible) ? vScrollBar.Pos : 0);
                float strX = padding.left + 1;
                //��������
                String text = Text;
                if (text == null)
                {
                    text = "";
                }
                int length = text.Length;
                SIZEF bSize = paint.TextSizeF("0", font);
                if (m_textChanged)
                {
                    int line = 0, count = 0;
                    //��ȡ��������
                    m_textChanged = !m_textChanged;
                    m_lines.Clear();
                    m_ranges.Clear();
                    m_wordsSize.Clear();
                    for (int i = 0; i < length; i++)
                    {
                        if (i == 0)
                        {
                            count = 0;
                            line++;
                            strX = padding.left + 1;
                            m_lines.Add(new WordLineA(i, i));
                        }
                        char ch = text[i];
                        String dch = ch.ToString();
                        //�Ʊ��
                        if (ch == 9)
                        {
                            int addCount = 4 - count % 4;
                            tSize.cx = bSize.cx * addCount;
                            tSize.cy = bSize.cy;
                            count += addCount;
                        }
                        else
                        {
                            //�����滻
                            if (m_passwordChar != 0)
                            {
                                dch = m_passwordChar.ToString();
                            }
                            tSize = paint.TextSizeF(dch, font);
                            if (ch == 10)
                            {
                                tSize.cx = 0;
                            }
                            count++;
                        }
                        //�ж��Ƿ����
                        if (m_multiline)
                        {
                            bool isNextLine = false;
                            bool isWordWrap = false;
                            if (ch == 13)
                            {
                                isNextLine = true;
                            }
                            else
                            {
                                //�Ƿ��Զ�����
                                if (m_wordWrap)
                                {
                                    if (strX + tSize.cx > width - vWidth)
                                    {
                                        isNextLine = true;
                                        isWordWrap = true;
                                    }
                                }
                            }
                            //����
                            if (isNextLine)
                            {
                                count = 0;
                                line++;
                                strX = padding.left + 1;
                                m_lines.Add(new WordLineA(i, i));
                                if (!isWordWrap)
                                {
                                    tSize.cx = 0;
                                }
                            }
                            else
                            {
                                m_lines[line - 1] = new WordLineA(m_lines[line - 1].m_start, i);
                            }
                        }
                        else
                        {
                            m_lines[line - 1] = new WordLineA(m_lines[line - 1].m_start, i);
                        }
                        if (ch > 1000)
                        {
                            tSize.cx += 1; //MODIFY
                        }
                        //��������
                        RECTF rangeRect = new RECTF(strX, padding.top + (line - 1) * lineHeight, strX + tSize.cx, padding.top + line * lineHeight);
                        m_ranges.Add(rangeRect);
                        m_wordsSize.Add(tSize);
                        strX = rangeRect.right;
                    }
                    //��������
                    if (m_rightToLeft)
                    {
                        int lcount = m_lines.Count;
                        for (int i = 0; i < lcount; i++)
                        {
                            WordLineA ln = m_lines[i];
                            float lw = width - vWidth - (m_ranges[ln.m_end].right - m_ranges[ln.m_start].left) - 2;
                            if (lw > 0)
                            {
                                for (int j = ln.m_start; j <= ln.m_end; j++)
                                {
                                    RECTF rangeRect = m_ranges[j];
                                    rangeRect.left += lw;
                                    rangeRect.right += lw;
                                    m_ranges[j] = rangeRect;
                                }
                            }
                        }
                    }
                    Update();
                }
                scrollH += m_offsetX;
                RECT tempRect = new RECT();
                int rangesSize = m_ranges.Count;
                int offsetX = m_offsetX; //MODIFY
                //�жϵ�ǰ�����Ƿ�ɼ�
                if (!m_multiline)
                {
                    RECTF firstRange = new RECTF();
                    RECTF lastRange = new RECTF();
                    if (rangesSize > 0)
                    {
                        firstRange = m_ranges[0];
                        lastRange = m_ranges[rangesSize - 1];
                    }
                    scrollH -= offsetX;
                    //����
                    if (m_textAlign == HorizontalAlignA.Center)
                    {
                        offsetX = -(int)(width - padding.right - (lastRange.right - firstRange.left)) / 2;
                    }
                    //Զ��
                    else if (m_textAlign == HorizontalAlignA.Right)
                    {
                        offsetX = -(int)(width - padding.right - (lastRange.right - firstRange.left) - 3);
                    }
                    //����
                    else
                    {
                        //��ʾ�����߽�
                        if (lastRange.right > width - padding.right)
                        {
                            //��ȡ���ǿɼ�������
                            int alwaysVisibleIndex = m_selectionStart + m_selectionLength;
                            if (m_startMovingIndex != -1)
                            {
                                alwaysVisibleIndex = m_startMovingIndex;
                            }
                            if (m_stopMovingIndex != -1)
                            {
                                alwaysVisibleIndex = m_stopMovingIndex;
                            }
                            if (alwaysVisibleIndex > rangesSize - 1)
                            {
                                alwaysVisibleIndex = rangesSize - 1;
                            }
                            if (alwaysVisibleIndex != -1)
                            {
                                RECTF alwaysVisibleRange = m_ranges[alwaysVisibleIndex];
                                int cw = width - padding.left - padding.right;
                                if (alwaysVisibleRange.left - offsetX > cw - 10)
                                {
                                    offsetX = (int)alwaysVisibleRange.right - cw + 3;
                                    if (offsetX < 0)
                                    {
                                        offsetX = 0;
                                    }
                                }
                                else if (alwaysVisibleRange.left - offsetX < 10)
                                {
                                    offsetX -= (int)bSize.cx * 4;
                                    if (offsetX < 0)
                                    {
                                        offsetX = 0;
                                    }
                                }
                                if (offsetX > lastRange.right - cw)
                                {
                                    offsetX = (int)lastRange.right - cw + 3;
                                }
                            }
                        }
                        //��ʾδ�����߽�
                        else
                        {
                            if (m_textAlign == HorizontalAlignA.Right)
                            {
                                offsetX = -(int)(width - padding.right - (lastRange.right - firstRange.left) - 3);
                            }
                            else
                            {
                                offsetX = 0;
                            }
                        }
                    }
                    m_offsetX = offsetX;
                    scrollH += m_offsetX;
                }
                int lineCount = m_lines.Count;
                //���ƾ��κ��ַ�
                List<RECTF> selectedRanges = new List<RECTF>();
                List<RECT> selectedWordsRanges = new List<RECT>();
                List<char> selectedWords = new List<char>();
                for (int i = 0; i < lineCount; i++)
                {
                    WordLineA line = m_lines[i];
                    for (int j = line.m_start; j <= line.m_end; j++)
                    {
                        char ch = text[j];
                        if (ch != 9)
                        {
                            //�����滻
                            if (m_passwordChar > 0)
                            {
                                ch = m_passwordChar;
                            }
                        }
                        //��ȡ��������
                        RECTF rangeRect = m_ranges[j];
                        rangeRect.left -= scrollH;
                        rangeRect.top -= scrollV;
                        rangeRect.right -= scrollH;
                        rangeRect.bottom -= scrollV;
                        RECT rRect = new RECT(rangeRect.left, rangeRect.top + (lineHeight - m_wordsSize[j].cy) / 2,
                            rangeRect.right, rangeRect.top + (lineHeight + m_wordsSize[j].cy) / 2);
                        if (rRect.right == rRect.left)
                        {
                            rRect.right = rRect.left + 1;
                        }
                        //��������
                        if (host.GetIntersectRect(ref tempRect, ref rRect, ref rect) > 0)
                        {
                            if (m_selectionLength > 0)
                            {
                                if (j >= m_selectionStart && j < m_selectionStart + m_selectionLength)
                                {
                                    selectedWordsRanges.Add(rRect);
                                    selectedRanges.Add(rangeRect);
                                    selectedWords.Add(ch);
                                    continue;
                                }
                            }
                            paint.DrawText(ch.ToString(), GetPaintingForeColor(), font, rRect);
                        }
                    }
                }
                //����ѡ������
                int selectedRangesSize = selectedRanges.Count;
                if (selectedRangesSize > 0)
                {
                    int sIndex = 0;
                    float right = 0;
                    for (int i = 0; i < selectedRangesSize; i++)
                    {
                        RECTF rRect = selectedRanges[i];
                        RECTF sRect = selectedRanges[sIndex];
                        bool newLine = rRect.top != sRect.top;
                        if (newLine || i == selectedRangesSize - 1)
                        {
                            int eIndex = (i == selectedRangesSize - 1) ? i : i - 1;
                            RECTF eRect = selectedRanges[eIndex];
                            RECT unionRect = new RECT(sRect.left, sRect.top, eRect.right + 1, sRect.bottom + 1);
                            if (newLine)
                            {
                                unionRect.right = (int)right;
                            }
                            paint.FillRect(m_selectionBackColor, unionRect);
                            for (int j = sIndex; j <= eIndex; j++)
                            {
                                paint.DrawText(selectedWords[j].ToString(), m_selectionForeColor, font, selectedWordsRanges[j]);
                            }
                            sIndex = i;
                        }
                        right = rRect.right;
                    }
                    selectedRanges.Clear();
                    selectedWords.Clear();
                    selectedWordsRanges.Clear();
                }
                //���ƹ��
                if (Focused && !m_readOnly && m_selectionLength == 0)
                {
                    int index = m_selectionStart;
                    if (index < 0)
                    {
                        index = 0;
                    }
                    if (index > length)
                    {
                        index = length;
                    }
                    //��ȡ����λ��
                    int cursorX = offsetX; //MODIFY
                    int cursorY = 0;
                    if (length > 0)
                    {
                        if (index == 0)
                        {
                            if (rangesSize > 0) //MODIFY
                            {
                                cursorX = (int)m_ranges[0].left;
                                cursorY = (int)m_ranges[0].top;
                            }
                        }
                        else
                        {
                            cursorX = (int)Math.Ceiling(m_ranges[index - 1].right) + 2;
                            cursorY = (int)Math.Ceiling(m_ranges[index - 1].top);
                        }
                        cursorY += lineHeight / 2 - (int)tSize.cy / 2;
                    }
                    else
                    {
                        cursorY = lineHeight / 2 - (int)tSize.cy / 2;
                    }
                    //������˸���
                    RECT cRect = new RECT(cursorX - scrollH, cursorY - scrollV, cursorX - scrollH + 4, cursorY + tSize.cy - scrollV);
                    paint.FillRect(COLOR.ARGB(255,0,0), cRect);
                }
                else
                {
                    if (!Focused && text.Length == 0)
                    {
                        if (m_tempText != null && m_tempText.Length > 0)
                        {
                            SIZE pSize = paint.TextSize(m_tempText, font);
                            RECT pRect = new RECT();
                            pRect.left = padding.left;
                            pRect.top = (lineHeight - pSize.cy) / 2;
                            pRect.right = pRect.left + pSize.cx;
                            pRect.bottom = pRect.top + pSize.cy;
                            paint.DrawText(m_tempText, m_tempTextForeColor, font, pRect);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ճ������
        /// </summary>
        public override void OnPaste()
        {
            if (!m_readOnly)
            {
                ControlHost host = Native.Host;
                String insert = host.Paste();
                if (insert != null && insert.Length > 0)
                {
                    int oldLines = m_lines.Count;
                    String redotext = Text;
                    InsertWord(insert);
                    OnTextChanged();
                    if (m_textChanged)
                    {
                        if (redotext != null)
                        {
                            m_undoStack.Push(redotext);
                        }
                    }
                    Invalidate();
                    int newLines = m_lines.Count;
                    if (newLines != oldLines)
                    {
                        VScrollBarA vScrollBar = VScrollBar;
                        if (vScrollBar != null)
                        {
                            vScrollBar.Pos += m_lineHeight * (newLines - oldLines);
                            Invalidate();
                        }
                    }
                    Update();
                    base.OnPaste();
                }
            }
        }

        /// <summary>
        /// ��ʹ��TAB�л�����
        /// </summary>
        public override void OnTabStop()
        {
            base.OnTabStop();
            if (!m_multiline)
            {
                if (Text != null)
                {
                    int textSize = Text.Length;
                    if (textSize > 0)
                    {
                        m_selectionStart = 0;
                        m_selectionLength = textSize;
                        OnTimer(m_timerID);
                    }
                }
            }
        }

        /// <summary>
        /// ���ֳߴ�ı��¼�
        /// </summary>
        public override void OnSizeChanged()
        {
            base.OnSizeChanged();
            if (m_wordWrap)
            {
                m_textChanged = true;
                Invalidate();
            }
        }

        /// <summary>
        /// ���ָı䷽��
        /// </summary>
        public override void OnTextChanged()
        {
            m_textChanged = true;
            base.OnTextChanged();
        }

        /// <summary>
        /// ���ص�����
        /// </summary>
        /// <param name="timerID">���ID</param>
        public override void OnTimer(int timerID)
        {
            base.OnTimer(timerID);
            if (m_timerID == timerID)
            {
                if (Visible && Focused && !m_textChanged)
                {
                    m_showCursor = !m_showCursor;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// �ظ�
        /// </summary>
        /// <returns>�ظ�����</returns>
        public void Redo()
        {
            if (CanRedo())
            {
                m_undoStack.Push(Text);
                Text = m_redoStack.Pop();
            }
        }

        /// <summary>
        /// ȫѡ
        /// </summary>
        public void SelectAll()
        {
            m_selectionStart = 0;
            if (Text != null)
            {
                m_selectionLength = Text.Length;
            }
            else
            {
                m_selectionLength = 0;
            }
        }

        /// <summary>
        /// �����ƶ�����
        /// </summary>
        /// <param name="sIndex">��ʼ����</param>
        /// <param name="eIndex">��������</param>
        private void SetMovingIndex(int sIndex, int eIndex)
        {
            int textSize = Text.Length;
            if (textSize > 0)
            {
                if (sIndex < 0)
                {
                    sIndex = 0;
                }
                if (sIndex > textSize)
                {
                    sIndex = textSize;
                }
                if (eIndex < 0)
                {
                    eIndex = 0;
                }
                if (eIndex > textSize)
                {
                    eIndex = textSize;
                }
                m_startMovingIndex = sIndex;
                m_stopMovingIndex = eIndex;
                m_selectionStart = Math.Min(m_startMovingIndex, m_stopMovingIndex);
                m_selectionLength = Math.Abs(m_startMovingIndex - m_stopMovingIndex);
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="value">����ֵ</param>
        public override void SetProperty(String name, String value)
        {
            if (name == "lineheight")
            {
                LineHeight = CStr.ConvertStrToInt(value);
            }
            else if (name == "multiline")
            {
                Multiline = CStr.ConvertStrToBool(value);
            }
            else if (name == "passwordchar")
            {
                PasswordChar = Convert.ToChar(value);
            }
            else if (name == "readonly")
            {
                ReadOnly = CStr.ConvertStrToBool(value);
            }
            else if (name == "righttoleft")
            {
                RightToLeft = CStr.ConvertStrToBool(value);
            }
            else if (name == "selectionbackcolor")
            {
                SelectionBackColor = CStr.ConvertStrToColor(value);
            }
            else if (name == "selectionforecolor")
            {
                SelectionForeColor = CStr.ConvertStrToColor(value);
            }
            else if (name == "temptext")
            {
                TempText = value;
            }
            else if (name == "temptextforecolor")
            {
                TempTextForeColor = CStr.ConvertStrToColor(value);
            }
            else if (name == "textalign")
            {
                TextAlign = CStr.ConvertStrToHorizontalAlign(value);
            }
            else if (name == "wordwrap")
            {
                WordWrap = CStr.ConvertStrToBool(value);
            }
            else
            {
                base.SetProperty(name, value);
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <returns>��������</returns>
        public void Undo()
        {
            if (CanUndo())
            {
                if (Text != null)
                {
                    m_redoStack.Push(Text);
                }
                Text = m_undoStack.Pop();
            }
        }

        /// <summary>
        /// ���²��ַ���
        /// </summary>
        public override void Update()
        {
            INativeBase native = Native;
            if (native != null)
            {
                VScrollBarA vScrollBar = VScrollBar;
                if (vScrollBar != null)
                {
                    vScrollBar.LineSize = m_lineHeight;
                }
            }
            base.Update();
        }
        #endregion
    }
}
