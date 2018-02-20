/*********************************************************************************\
*                                                                                 *
* JidianService.cs - Jidian service functions, types, and definitions.            *
*                                                                                 *
*               Version 6.00                                                      *
*                                                                                 *
*               Copyright (c) 2016-2016, Jidian. All rights reserved.             *
*               Created by Todd 2016/4/25.                                        *
*                                                                                 *
**********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace OwLib
{
    /// <summary>
    /// 项目基点
    /// </summary>
    public class ProjectJidian
    {
        private double m_avgLevel;

        /// <summary>
        /// 获取或设置平均等级
        /// </summary>
        public double AvgLevel
        {
            get { return m_avgLevel; }
            set { m_avgLevel = value; }
        }

        private int m_files;

        /// <summary>
        /// 获取或设置文件数
        /// </summary>
        public int Files
        {
            get { return m_files; }
            set { m_files = value; }
        }

        private List<FileJidian> m_fileJidians = new List<FileJidian>();

        /// <summary>
        /// 获取或设置文件基点
        /// </summary>
        public List<FileJidian> FileJidians
        {
            get { return m_fileJidians; }
            set { m_fileJidians = value; }
        }

        private int m_lines;

        /// <summary>
        /// 获取或设置代码量
        /// </summary>
        public int Lines
        {
            get { return m_lines; }
            set { m_lines = value; }
        }

        private int m_scores;

        /// <summary>
        /// 获取或设置分数
        /// </summary>
        public int Scores
        {
            get { return m_scores; }
            set { m_scores = value; }
        }

        /// <summary>
        /// 从字符串转换为对象
        /// </summary>
        /// <param name="str">字符串</param>
        public void FromString(String str)
        {
            String[] strs = str.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            m_files = CStr.ConvertStrToInt(strs[0]);
            m_lines = CStr.ConvertStrToInt(strs[1]);
            m_scores = CStr.ConvertStrToInt(strs[2]);
            m_avgLevel = CStr.ConvertStrToInt(strs[3]);
            int strsSize = strs.Length;
            for (int i = 4; i < strsSize; i++)
            {
                FileJidian fileJidian = new FileJidian();
                fileJidian.FromString(strs[i]);
                m_fileJidians.Add(fileJidian);
            }
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(m_files.ToString());
            sb.AppendLine(m_lines.ToString());
            sb.AppendLine(m_scores.ToString());
            sb.AppendLine(m_avgLevel.ToString());
            int jidianSize = m_fileJidians.Count;
            for (int i = 0; i < jidianSize; i++)
            {
                sb.AppendLine(m_fileJidians[i].ToString());
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// 文件基点
    /// </summary>
    public class FileJidian
    {
        private String m_codeType;

        /// <summary>
        /// 获取或设置代码类型
        /// </summary>
        public String CodeType
        {
            get { return m_codeType; }
            set { m_codeType = value; }
        }

        private int m_level;

        /// <summary>
        /// 获取或设置等级
        /// </summary>
        public int Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        private int m_lines;

        /// <summary>
        /// 获取或设置代码量
        /// </summary>
        public int Lines
        {
            get { return m_lines; }
            set { m_lines = value; }
        }

        private List<MemberJidian> m_memberJidians = new List<MemberJidian>();

        /// <summary>
        /// 获取或设置成员基点
        /// </summary>
        public List<MemberJidian> MemberJidians
        {
            get { return m_memberJidians; }
            set { m_memberJidians = value; }
        }

        private String m_path;

        /// <summary>
        /// 获取或设置代码路径
        /// </summary>
        public String Path
        {
            get { return m_path; }
            set { m_path = value; }
        }

        private String m_pID;

        /// <summary>
        /// 获取或设置所属项目编号
        /// </summary>
        public String PID
        {
            get { return m_pID; }
            set { m_pID = value; }
        }

        private int m_scores;

        /// <summary>
        /// 获取或设置分数
        /// </summary>
        public int Scores
        {
            get { return m_scores; }
            set { m_scores = value; }
        }

        /// <summary>
        /// 从字符串转换为对象
        /// </summary>
        /// <param name="str">字符串</param>
        public void FromString(String str)
        {
            String[] strs = str.Split(new String[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            m_path = strs[0];
            m_pID=strs[1];
            m_codeType = strs[2];
            m_lines = CStr.ConvertStrToInt(strs[3]);
            m_level = CStr.ConvertStrToInt(strs[4]);
            m_scores = CStr.ConvertStrToInt(strs[5]);
            int strsSize = strs.Length;
            for (int i = 6; i < strsSize; i++)
            {
                MemberJidian memberJidian = new MemberJidian();
                memberJidian.FromString(strs[i]);
                m_memberJidians.Add(memberJidian);
            }
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(m_path + "|");
            sb.Append(m_pID + "|");
            sb.Append(m_codeType + "|");
            sb.Append(m_lines.ToString() + "|");
            sb.Append(m_level.ToString() + "|");
            sb.Append(m_scores.ToString() + "|");
            int membersSize = m_memberJidians.Count;
            for (int i = 0; i < membersSize; i++)
            {
                sb.Append(m_memberJidians[i].ToString() + "|");
            }
            return sb.ToString();
        }
    }


    /// <summary>
    /// 成员基点
    /// </summary>
    public class MemberJidian
    {
        private int m_lines;

        /// <summary>
        /// 获取或设置代码量
        /// </summary>
        public int Lines
        {
            get { return m_lines; }
            set { m_lines = value; }
        }

        private String m_name;

        /// <summary>
        /// 获取或设置姓名
        /// </summary>
        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private double m_rates;

        /// <summary>
        /// 获取或设置占比
        /// </summary>
        public double Rates
        {
            get { return m_rates; }
            set { m_rates = value; }
        }

        /// <summary>
        /// 从字符串转换为对象
        /// </summary>
        /// <param name="str">字符串</param>
        public void FromString(String str)
        {
            String[] strs = str.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            m_name = strs[0];
            m_lines = CStr.ConvertStrToInt(strs[1]);
            m_rates = CStr.ConvertStrToDouble(strs[2]);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(m_name + ",");
            sb.Append(m_lines.ToString() + ",");
            sb.Append(m_rates.ToString());
            return sb.ToString();
        }
    }

    /// <summary>
    /// 基点服务
    /// </summary>
    public class JidianService
    {
        #region Lord 2016/04/25
        private String m_dir;

        /// <summary>
        /// 获取或设置目录
        /// </summary>
        public String Dir
        {
            get { return m_dir; }
            set { m_dir = value; }
        }

        /// <summary>
        /// 分析基点
        /// </summary>
        /// <param name="dir">目录</param>
        private void Analysis(String dir, ref ProjectJidian projectJidian)
        {
            List<String> dirs = new List<String>();
            CFileA.GetDirectories(dir, dirs);
            int dirsSize = dirs.Count;
            for (int i = 0; i < dirsSize; i++)
            {
                Analysis(dirs[i], ref projectJidian);
            }
            List<String> files = new List<String>();
            CFileA.GetFiles(dir, files);
            int filesSize = files.Count;
            for (int i = 0; i < filesSize; i++)
            {
                String file = files[i];
                String content = "";
                CFileA.Read(file, ref content);
                String[] strs = content.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                int strsSize = strs.Length;
                if (strsSize > 0)
                {
                    String fline = strs[0];
                    if (fline.StartsWith("//GAIA"))
                    {
                        FileJidian fileJidian = new FileJidian();
                        fileJidian.Lines = strsSize;
                        projectJidian.Lines += strsSize;
                        fileJidian.Path = file.Replace(m_dir + "\\", "");
                        String[] subStrs = fline.Split(new String[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        String[] subStrs2 = subStrs[0].Split(new String[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                        fileJidian.PID = subStrs2[1];
                        fileJidian.CodeType = subStrs2[2];
                        fileJidian.Level = Convert.ToInt32(subStrs2[3].Replace("S", ""));
                        fileJidian.Scores = fileJidian.Lines * fileJidian.Level;
                        projectJidian.Scores += fileJidian.Scores;
                        projectJidian.AvgLevel = (double)(projectJidian.Files * projectJidian.AvgLevel + fileJidian.Level) / (projectJidian.Files + 1);
                        projectJidian.Files++;
                        String[] sunStrs = subStrs[1].Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < sunStrs.Length; j++)
                        {
                            String[] csunStrs = sunStrs[j].Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            if (csunStrs.Length >= 2)
                            {
                                MemberJidian memberJidian = new MemberJidian();
                                memberJidian.Name = csunStrs[0];
                                memberJidian.Rates = (double)Convert.ToInt32(csunStrs[1].Replace("%", "")) / 100;
                                memberJidian.Lines = (int)(strsSize * memberJidian.Rates);
                                fileJidian.MemberJidians.Add(memberJidian);
                            }
                        }
                        projectJidian.FileJidians.Add(fileJidian);
                    }
                }
            }
            dirs.Clear();
            files.Clear();
        }

        /// <summary>
        /// 获取基点
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="projectJidian">项目基点</param>
        /// <returns></returns>
        public int GetJidian(ref ProjectJidian projectJidian)
        {
            Analysis(m_dir, ref projectJidian);
            return 1;
        }
        #endregion
    }
}
