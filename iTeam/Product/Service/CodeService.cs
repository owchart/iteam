/**************************************************************************************\
*                                                                                      *
* CodeService.cs -  Code service functions, types, and definitions.       *
*                                                                                      *
*               Version 1.00 ��                                                        *
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
    /// ������Ϣ
    /// </summary>
    public class CodeInfo : BaseInfo
    {
        /// <summary>
        /// ����
        /// </summary>
        public String m_content = "";
    }

    /// <summary>
    /// �ж����ŷ���
    /// </summary>
    public class CodeService
    {
        /// <summary>
        /// ��������
        /// </summary>
        public CodeService()
        {
            UserCookie cookie = new UserCookie();
            UserCookieService cookieService = DataCenter.UserCookieService;
            if (cookieService.GetCookie("CODE", ref cookie) > 0)
            {
                try
                {
                    m_codes = JsonConvert.DeserializeObject<List<CodeInfo>>(AESHelper.Decrypt(cookie.m_value));
                }
                catch (Exception ex)
                {
                }
                if (m_codes == null)
                {
                    try
                    {
                        m_codes = JsonConvert.DeserializeObject<List<CodeInfo>>(cookie.m_value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// ָʾ��Ϣ
        /// </summary>
        public List<CodeInfo> m_codes = new List<CodeInfo>();

        /// <summary>
        /// ɾ����Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        public void Delete(String id)
        {
            int codesSize = m_codes.Count;
            for (int i = 0; i < codesSize; i++)
            {
                if (m_codes[i].m_ID == id)
                {
                    m_codes.RemoveAt(i);
                    Save();
                    break;
                }
            }
        }

        /// <summary>
        /// ��ȡ�µ�ID
        /// </summary>
        /// <returns>��ID</returns>
        public String GetNewID()
        {
            List<int> ids = new List<int>();
            int codesSize = m_codes.Count;
            for (int i = 0; i < codesSize; i++)
            {
                ids.Add(CStr.ConvertStrToInt(m_codes[i].m_ID));
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
        /// ��ȡ��Ϣ
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>��Ϣ</returns>
        public CodeInfo GetCode(String id)
        {
            int codesSize = m_codes.Count;
            for (int i = 0; i < codesSize; i++)
            {
                if (m_codes[i].m_ID == id)
                {
                    return m_codes[i];
                }
            }
            return new CodeInfo();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public void Save()
        {
            UserCookie cookie = new UserCookie();
            cookie.m_key = "CODE";
            cookie.m_value = AESHelper.Encrypt(JsonConvert.SerializeObject(m_codes));
            UserCookieService cookieService = DataCenter.UserCookieService;
            cookieService.AddCookie(cookie);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="code">��Ϣ</param>
        public void Save(CodeInfo code)
        {
            bool modify = false;
            int codesSize = m_codes.Count;
            for (int i = 0; i < codesSize; i++)
            {
                if (m_codes[i].m_ID == code.m_ID)
                {
                    m_codes[i] = code;
                    modify = true;
                    break;
                }
            }
            if (!modify)
            {
                m_codes.Add(code);
            }
            Save();
        }
    }
}
