/**************************************************************************************\
*                                                                                      *
* SecurityService.cs -  Security service functions, types, and definitions.       *
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
using System.Net;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace OwLib
{
    /// <summary>
    /// ��Ʊ��Ϣ
    /// </summary>
    public class Security
    {
        #region Lord 2016/4/20
        /// <summary>
        /// �������̾���
        /// </summary>
        public Security()
        {
        }

        /// <summary>
        /// ��Ʊ����
        /// </summary>
        public String m_code = "";

        /// <summary>
        /// ��Ʊ����
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// ƴ��
        /// </summary>
        public String m_pingyin = "";

        /// <summary>
        /// ״̬
        /// </summary>
        public int m_status;

        /// <summary>
        /// �г�����
        /// </summary>
        public int m_type;
        #endregion
    }

    /// <summary>
    /// ��Ʊʵʱ����
    /// </summary>
    public class SecurityLatestData
    {
        #region Lord 2016/3/5
        /// <summary>
        /// �ɽ���
        /// </summary>
        public double m_amount;

        /// <summary>
        /// ��һ��
        /// </summary>
        public int m_buyVolume1;

        /// <summary>
        /// �����
        /// </summary>
        public int m_buyVolume2;

        /// <summary>
        /// ������
        /// </summary>
        public int m_buyVolume3;

        /// <summary>
        /// ������
        /// </summary>
        public int m_buyVolume4;

        /// <summary>
        /// ������
        /// </summary>
        public int m_buyVolume5;

        /// <summary>
        /// ��һ��
        /// </summary>
        public float m_buyPrice1;

        /// <summary>
        /// �����
        /// </summary>
        public float m_buyPrice2;

        /// <summary>
        /// ������
        /// </summary>
        public float m_buyPrice3;

        /// <summary>
        /// ���ļ�
        /// </summary>
        public float m_buyPrice4;

        /// <summary>
        /// �����
        /// </summary>
        public float m_buyPrice5;

        /// <summary>
        /// ��ǰ�۸�
        /// </summary>
        public float m_close;

        /// <summary>
        /// ���ڼ�ʱ��
        /// </summary>
        public long m_date;

        /// <summary>
        /// ��߼�
        /// </summary>
        public float m_high;

        /// <summary>
        /// ���̳ɽ���
        /// </summary>
        public int m_innerVol;

        /// <summary>
        /// �������̼�
        /// </summary>
        public float m_lastClose;

        /// <summary>
        /// ��ͼ�
        /// </summary>
        public float m_low;

        /// <summary>
        /// ���̼�
        /// </summary>
        public float m_open;

        /// <summary>
        /// �ڻ��ֲ���
        /// </summary>
        public double m_openInterest;

        /// <summary>
        /// ���̳ɽ���
        /// </summary>
        public int m_outerVol;

        /// <summary>
        /// ��Ʊ����
        /// </summary>
        public String m_securityCode = "";

        /// <summary>
        /// ��Ʊ����
        /// </summary>
        public String m_securityName = "";

        /// <summary>
        /// ��һ��
        /// </summary>
        public int m_sellVolume1;

        /// <summary>
        /// ������
        /// </summary>
        public int m_sellVolume2;

        /// <summary>
        /// ������
        /// </summary>
        public int m_sellVolume3;

        /// <summary>
        /// ������
        /// </summary>
        public int m_sellVolume4;

        /// <summary>
        /// ������
        /// </summary>
        public int m_sellVolume5;

        /// <summary>
        /// ��һ��
        /// </summary>
        public float m_sellPrice1;

        /// <summary>
        /// ������
        /// </summary>
        public float m_sellPrice2;

        /// <summary>
        /// ������
        /// </summary>
        public float m_sellPrice3;

        /// <summary>
        /// ���ļ�
        /// </summary>
        public float m_sellPrice4;

        /// <summary>
        /// �����
        /// </summary>
        public float m_sellPrice5;

        /// <summary>
        /// �ڻ������
        /// </summary>
        public float m_settlePrice;

        /// <summary>
        /// ������
        /// </summary>
        public float m_turnoverRate;

        /// <summary>
        /// �ɽ���
        /// </summary>
        public double m_volume;

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="data">����</param>
        public void Copy(SecurityLatestData data)
        {
            if (data == null) return;
            m_amount = data.m_amount;
            m_buyVolume1 = data.m_buyVolume1;
            m_buyVolume2 = data.m_buyVolume2;
            m_buyVolume3 = data.m_buyVolume3;
            m_buyVolume4 = data.m_buyVolume4;
            m_buyVolume5 = data.m_buyVolume5;
            m_buyPrice1 = data.m_buyPrice1;
            m_buyPrice2 = data.m_buyPrice2;
            m_buyPrice3 = data.m_buyPrice3;
            m_buyPrice4 = data.m_buyPrice4;
            m_buyPrice5 = data.m_buyPrice5;
            m_close = data.m_close;
            m_date = data.m_date;
            m_high = data.m_high;
            m_innerVol = data.m_innerVol;
            m_lastClose = data.m_lastClose;
            m_low = data.m_low;
            m_open = data.m_open;
            m_openInterest = data.m_openInterest;
            m_outerVol = data.m_outerVol;
            m_securityCode = data.m_securityCode;
            m_securityName = data.m_securityName;
            m_sellVolume1 = data.m_sellVolume1;
            m_sellVolume2 = data.m_sellVolume2;
            m_sellVolume3 = data.m_sellVolume3;
            m_sellVolume4 = data.m_sellVolume4;
            m_sellVolume5 = data.m_sellVolume5;
            m_sellPrice1 = data.m_sellPrice1;
            m_sellPrice2 = data.m_sellPrice2;
            m_sellPrice3 = data.m_sellPrice3;
            m_sellPrice4 = data.m_sellPrice4;
            m_sellPrice5 = data.m_sellPrice5;
            m_settlePrice = data.m_settlePrice;
            m_turnoverRate = data.m_turnoverRate;
            m_volume = data.m_volume;
        }

        /// <summary>
        /// �Ƚ��Ƿ���ͬ
        /// </summary>
        /// <param name="data">����</param>
        /// <returns>�Ƿ���ͬ</returns>
        public bool Equal(SecurityLatestData data)
        {
            if (data == null) return false;
            if (m_amount == data.m_amount
            && m_buyVolume1 == data.m_buyVolume1
            && m_buyVolume2 == data.m_buyVolume2
            && m_buyVolume3 == data.m_buyVolume3
            && m_buyVolume4 == data.m_buyVolume4
            && m_buyVolume5 == data.m_buyVolume5
            && m_buyPrice1 == data.m_buyPrice1
            && m_buyPrice2 == data.m_buyPrice2
            && m_buyPrice3 == data.m_buyPrice3
            && m_buyPrice4 == data.m_buyPrice4
            && m_buyPrice5 == data.m_buyPrice5
            && m_close == data.m_close
            && m_date == data.m_date
            && m_high == data.m_high
            && m_innerVol == data.m_innerVol
            && m_lastClose == data.m_lastClose
            && m_low == data.m_low
            && m_open == data.m_open
            && m_openInterest == data.m_openInterest
            && m_outerVol == data.m_outerVol
            && m_securityCode == data.m_securityCode
            && m_securityName == data.m_securityName
            && m_sellVolume1 == data.m_sellVolume1
            && m_sellVolume2 == data.m_sellVolume2
            && m_sellVolume3 == data.m_sellVolume3
            && m_sellVolume4 == data.m_sellVolume4
            && m_sellVolume5 == data.m_sellVolume5
            && m_sellPrice1 == data.m_sellPrice1
            && m_sellPrice2 == data.m_sellPrice2
            && m_sellPrice3 == data.m_sellPrice3
            && m_sellPrice4 == data.m_sellPrice4
            && m_sellPrice5 == data.m_sellPrice5
            && m_settlePrice == data.m_settlePrice
            && m_turnoverRate == data.m_turnoverRate
            && m_volume == data.m_volume)
            {
                return true;
            }
            return false;
        }
        #endregion
    }

    /// <summary>
    /// ��Ʊ����
    /// </summary>
    public class SecurityService
    {
        #region Lord 2015/11/14
        /// <summary>
        /// ��������
        /// </summary>
        private static Dictionary<String, SecurityLatestData> m_latestDatas = new Dictionary<String, SecurityLatestData>();

        /// <summary>
        /// �������
        /// </summary>
        private static Random m_rd = new Random();

        /// <summary>
        /// ��Ʊ�б�
        /// </summary>
        private static List<Security> m_securities = new List<Security>();

        /// <summary>
        /// �����ѡ��Ա
        /// </summary>
        /// <returns>��Ա</returns>
        public static String GetAutoMember()
        {
            StaffService staffService = DataCenter.StaffService;
            List<StaffInfo> staffs = staffService.m_staffs;
            int staffsSize = staffs.Count;
            if (staffsSize > 0)
            {
                List<StaffInfo> selectStaffs = new List<StaffInfo>();
                for (int i = 0; i < staffsSize; i++)
                {
                    if (staffs[i].m_canSelect == "��")
                    {
                        selectStaffs.Add(staffs[i]);
                    }
                }
                int selectStaffsSize = selectStaffs.Count;
                return selectStaffs[m_rd.Next(0, selectStaffsSize)].m_name;
            }
            return "";
        }

        /// <summary>
        /// ��ȡ�Զ���Ʊ
        /// </summary>
        /// <returns>��Ʊ</returns>
        public static Security GetAutoSecurity()
        {
            lock (m_securities)
            {
                int count = m_securities.Count;
                if (count > 0)
                {
                    return m_securities[m_rd.Next(0, count)];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// �����ַ�����ȡ���˵���������
        /// </summary>
        /// <param name="str">�����ַ���</param>
        /// <param name="formatType">��ʽ</param>
        /// <param name="data">��������</param>
        /// <returns>״̬</returns>
        public static int GetLatestDataBySinaStr(String str, int formatType, ref SecurityLatestData data)
        {
            //��������
            String date = "";
            String[] strs2 = str.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int strLen2 = strs2.Length;
            bool szIndex = false;
            for (int j = 0; j < strLen2; j++)
            {
                String str2 = strs2[j];
                switch (j)
                {
                    case 0:
                        data.m_securityCode = CStrA.ConvertSinaCodeToDBCode(str2);
                        if (data.m_securityCode.StartsWith("399"))
                        {
                            szIndex = true;
                        }
                        break;
                    case 1:
                        {
                            float value = 0;
                            float.TryParse(str2, out value);
                            data.m_open = value;
                            break;
                        }
                    case 2:
                        {
                            float value = 0;
                            float.TryParse(str2, out value);
                            data.m_lastClose = value;
                            break;
                        }
                    case 3:
                        {
                            float value = 0;
                            float.TryParse(str2, out value);
                            data.m_close = value;
                            break;
                        }
                    case 4:
                        {
                            float value = 0;
                            float.TryParse(str2, out value);
                            data.m_high = value;
                            break;
                        }
                    case 5:
                        {
                            float value = 0;
                            float.TryParse(str2, out value);
                            data.m_low = value;
                            break;
                        }
                    case 8:
                        {
                            double lValue = 0;
                            double.TryParse(str2, out lValue);
                            data.m_volume = lValue;
                            if (szIndex)
                            {
                                data.m_volume /= 100;
                            }
                            break;
                        }
                    case 9:
                        {
                            double lValue = 0;
                            double.TryParse(str2, out lValue);
                            data.m_amount = lValue;
                            break;
                        }
                    case 10:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_buyVolume1 = (int)value;
                            }
                            break;
                        }
                    case 11:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_buyPrice1 = value;
                            }
                            break;
                        }
                    case 12:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_buyVolume2 = (int)value;
                            }
                            break;
                        }
                    case 13:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_buyPrice2 = value;
                            }
                            break;
                        }
                    case 14:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_buyVolume3 = (int)value;
                            }
                            break;
                        }
                    case 15:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_buyPrice3 = value;
                            }
                            break;
                        }
                    case 16:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_buyVolume4 = (int)value;
                            }
                            break;
                        }
                    case 17:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_buyPrice4 = value;
                            }
                            break;
                        }
                    case 18:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_buyVolume5 = (int)value;
                            }
                            break;
                        }
                    case 19:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_buyPrice5 = value;
                            }
                            break;
                        }
                    case 20:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_sellVolume1 = (int)value;
                            }
                            break;
                        }
                    case 21:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_sellPrice1 = value;
                            }
                            break;
                        }
                    case 22:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_sellVolume2 = (int)value;
                            }
                            break;
                        }
                    case 23:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_sellPrice2 = value;
                            }
                            break;
                        }
                    case 24:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_sellVolume3 = (int)value;
                            }
                            break;
                        }
                    case 25:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_sellPrice3 = value;
                            }
                            break;
                        }
                    case 26:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_sellVolume4 = (int)value;
                            }
                            break;
                        }
                    case 27:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_sellPrice4 = value;
                            }
                            break;
                        }
                    case 28:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_sellVolume5 = (int)value;
                            }
                            break;
                        }
                    case 29:
                        {
                            if (formatType == 0)
                            {
                                float value = 0;
                                float.TryParse(str2, out value);
                                data.m_sellPrice5 = value;
                            }
                            break;
                        }
                    case 30:
                        date = str2;
                        break;
                    case 31:
                        date += " " + str2;
                        break;
                }
            }
            //��ȡʱ��
            if (date != null && date.Length > 0)
            {
                DateTime dateTime = Convert.ToDateTime(date);
                data.m_date = (long)CStrA.M129(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0);
            }
            //�۸�����
            if (data.m_close != 0)
            {
                if (data.m_open == 0)
                {
                    data.m_open = data.m_close;
                }
                if (data.m_high == 0)
                {
                    data.m_high = data.m_close;
                }
                if (data.m_low == 0)
                {
                    data.m_low = data.m_close;
                }
            }
            return 0;
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="code">����</param>
        /// <param name="latestData">��������</param>
        /// <returns>״̬</returns>
        public static int GetLatestData(String code, ref SecurityLatestData latestData)
        {
            int state = 0;
            lock (m_latestDatas)
            {
                if (m_latestDatas.ContainsKey(code))
                {
                    latestData.Copy(m_latestDatas[code]);
                    state = 1;
                }
            }
            return state;
        }

        /// <summary>
        /// ��ȡ��ҳ����
        /// </summary>
        /// <param name="url">��ַ</param>
        /// <returns>ҳ��Դ��</returns>
        public static String GetHttpWebRequest(String url)
        {
            String content = "";
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader streamReader = null;
            Stream resStream = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = 10000;
                response = (HttpWebResponse)request.GetResponse();
                resStream = response.GetResponseStream();
                streamReader = new StreamReader(resStream, System.Text.Encoding.GetEncoding("UTF-8"));
                content = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (resStream != null)
                {
                    resStream.Close();
                    resStream.Dispose();
                }
                if (streamReader != null)
                {
                    streamReader.Close();
                    streamReader.Dispose();
                }
            }
            return content;
        }

        /// <summary>
        /// �����ַ�����ȡ������������
        /// </summary>
        /// <param name="str">�����ַ���</param>
        /// <param name="formatType">��ʽ</param>
        /// <param name="datas">��������</param>
        /// <returns>״̬</returns>
        public static int GetLatestDatasBySinaStr(String str, int formatType, List<SecurityLatestData> datas)
        {
            String[] strs = str.Split(new String[] { ";\n" }, StringSplitOptions.RemoveEmptyEntries);
            int strLen = strs.Length;
            for (int i = 0; i < strLen; i++)
            {
                SecurityLatestData latestData = new SecurityLatestData();
                String dataStr = strs[i];
                GetLatestDataBySinaStr(strs[i], formatType, ref latestData);
                if (latestData.m_date > 0)
                {
                    datas.Add(latestData);
                }
            }
            return 1;
        }

        /// <summary>
        /// ���ݹ�Ʊ�����ȡ������������
        /// </summary>
        /// <param name="codes">��Ʊ�����б�</param>
        /// <returns>�ַ���</returns>
        public static String GetSinaLatestDatasStrByCodes(String codes)
        {
            String[] strs = codes.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int strLen = strs.Length;
            List<String> sinaCodes = new List<String>();
            List<String> dcCodes = new List<String>();
            for (int i = 0; i < strLen; i++)
            {
                String postCode = strs[i];
                sinaCodes.Add(postCode);
            }
            String requestCode = "";
            int sinaCodesSize = sinaCodes.Count;
            for (int i = 0; i < sinaCodesSize; i++)
            {
                String postCode = strs[i];
                requestCode += CStrA.ConvertDBCodeToSinaCode(postCode);
                if (i != strLen - 1)
                {
                    requestCode += ",";
                }
            }
            String result = "";
            if (sinaCodesSize > 0)
            {
                String url = "http://hq.sinajs.cn/list=" + requestCode;
                result = GetHttpWebRequest(url);
            }
            return result;
        }

        /// <summary>
        /// ��ʼ����
        /// </summary>
        public static void Start()
        {
            Thread thread = new Thread(new ThreadStart(StartWork));
            thread.Start();
        }

        /// <summary>
        /// ��ʼ����
        /// </summary>
        private static void StartWork()
        {
            Dictionary<String, String> m_codesMap = new Dictionary<String, String>();
            String codes = "";
            while (true)
            {
                if (m_securities.Count == 0)
                {
                    String codesStr = "";
                    CFileA.Read(DataCenter.GetAppPath() + "\\codes.txt", ref codesStr);
                    String[] strs = codesStr.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < strs.Length; i++)
                    {
                        String[] subStrs = strs[i].Split(',');
                        Security security = new Security();
                        security.m_code = subStrs[0];
                        security.m_name = subStrs[1];
                        lock (m_securities)
                        {
                            m_securities.Add(security);
                        }
                        m_codesMap[security.m_code] = security.m_name;
                        codes += security.m_code;
                        codes += ",";
                        if (!security.m_code.StartsWith("A"))
                        {
                            sb.Append(security.m_code + "," + security.m_name + "\r\n");
                        }
                    }
                    CFileA.Write(DataCenter.GetAppPath() + "\\codes.txt", sb.ToString());
                }
                if (codes != null && codes.Length > 0)
                {
                    if (codes.EndsWith(","))
                    {
                        codes.Remove(codes.Length - 1);
                    }
                    String[] strCodes = codes.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    int codesSize = strCodes.Length;
                    String latestCodes = "";
                    for (int i = 0; i < codesSize; i++)
                    {
                        latestCodes += strCodes[i];
                        if (i == codesSize - 1 || (i > 0 && i % 50 == 0))
                        {
                            String latestDatasResult = GetSinaLatestDatasStrByCodes(latestCodes);
                            if (latestDatasResult != null && latestDatasResult.Length > 0)
                            {
                                List<SecurityLatestData> latestDatas = new List<SecurityLatestData>();
                                GetLatestDatasBySinaStr(latestDatasResult, 0, latestDatas);
                                int latestDatasSize = latestDatas.Count;
                                for (int j = 0; j < latestDatasSize; j++)
                                {
                                    SecurityLatestData latestData = latestDatas[j];
                                    if (latestData.m_close == 0)
                                    {
                                        latestData.m_close = latestData.m_buyPrice1;
                                    }
                                    if (latestData.m_close == 0)
                                    {
                                        latestData.m_close = latestData.m_sellPrice1;
                                    }
                                    lock (m_latestDatas)
                                    {
                                        m_latestDatas[latestData.m_securityCode] = latestData;
                                    }
                                }
                                latestDatas.Clear();
                            }
                            latestCodes = "";
                        }
                        else
                        {
                            latestCodes += ",";
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        #endregion
    }
}
