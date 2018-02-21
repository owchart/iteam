/**************************************************************************************\
*                                                                                      *
* ServerService.cs -  Server service functions, types, and definitions.       *
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
using Newtonsoft.Json;

namespace OwLib
{
    /// <summary>
    /// 训练信息
    /// </summary>
    public class TrainingInfo : BaseInfo
    {
        /// <summary>
        /// 训练1
        /// </summary>
        public String m_training1 = "0";

        /// <summary>
        /// 训练2
        /// </summary>
        public String m_training2 = "0";

        /// <summary>
        /// 训练3
        /// </summary>
        public String m_training3 = "0";

        /// <summary>
        /// 训练4
        /// </summary>
        public String m_training4 = "0";

        /// <summary>
        /// 训练5
        /// </summary>
        public String m_training5 = "0";

        /// <summary>
        /// 训练6
        /// </summary>
        public String m_training6 = "0";

        /// <summary>
        /// 训练7
        /// </summary>
        public String m_training7 = "0";

        /// <summary>
        /// 训练8
        /// </summary>
        public String m_training8 = "0";

        /// <summary>
        /// 训练9
        /// </summary>
        public String m_training9 = "0";

        /// <summary>
        /// 训练10
        /// </summary>
        public String m_training10 = "0";
    }

    /// <summary>
    /// 训练服务
    /// </summary>
    public class TrainingService
    {
        /// <summary>
        /// 创建服务
        /// </summary>
        public TrainingService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("TRAINING", ref cookie) > 0)
            {
                try
                {
                    m_trainings = JsonConvert.DeserializeObject<List<TrainingInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_trainings == null)
                {
                    try
                    {
                        m_trainings = JsonConvert.DeserializeObject<List<TrainingInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 训练信息
        /// </summary>
        public List<TrainingInfo> m_trainings = new List<TrainingInfo>();

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int trainingsSize = m_trainings.Count;
            for (int i = 0; i < trainingsSize; i++)
            {
                if (m_trainings[i].m_ID == id)
                {
                    m_trainings.RemoveAt(i);
                    Save();
                    break;
                }
            }
        }

        /// <summary>
        /// 获取新的ID
        /// </summary>
        /// <returns>新ID</returns>
        public String GetNewID()
        {
            List<int> ids = new List<int>();
            int trainingsSize = m_trainings.Count;
            for (int i = 0; i < trainingsSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_trainings[i].m_ID));
            }
            ids.Sort();
            int idsSize = ids.Count;
            if (idsSize > 0)
            {
                return CStr.ConvertIntToStr(ids[idsSize - 1] + 1);
            }
            else
            {
                return "1";
            }
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>信息</returns>
        public TrainingInfo GetTraining(String id)
        {
            int trainingsSize = m_trainings.Count;
            for (int i = 0; i < trainingsSize; i++)
            {
                if (m_trainings[i].m_ID == id)
                {
                    return m_trainings[i];
                }
            }
            return new TrainingInfo();
        }


        /// <summary>
        /// 保存信息
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "TRAINING";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_trainings));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="training">信息</param>
        public void Save(TrainingInfo training)
        {
            bool modify = false;
            int trainingsSize = m_trainings.Count;
            for (int i = 0; i < trainingsSize; i++)
            {
                if (m_trainings[i].m_ID == training.m_ID)
                {
                    m_trainings[i] = training;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_trainings.Add(training);
            }
            Save();
        }
    }
}
