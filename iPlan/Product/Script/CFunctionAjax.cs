using System;
using System.Collections.Generic;
using System.Text;
using OwLib;
using System.Threading;
using node.gs;

namespace OwLib
{
    /// <summary>
    /// Ajax请求信息
    /// </summary>
    public class AjaxRequest
    {
        /// <summary>
        /// 创建请求
        /// </summary>
        public AjaxRequest()
        {
        }

        /// <summary>
        /// 创建请求
        /// </summary>
        /// <param name="indicator">指标</param>
        /// <param name="type">类型</param>
        /// <param name="url">地址</param>
        /// <param name="dataType">数据类型</param>
        /// <param name="callBack">回调</param>
        public AjaxRequest(CIndicator indicator, String type, String url, String dataType, String callBack)
        {
            m_indicator = indicator;
            m_url = url;
            m_type = type;
            m_dataType = dataType;
            m_callBack = callBack;
        }

        /// <summary>
        /// 回调函数
        /// </summary>
        public String m_callBack = "";

        /// <summary>
        /// 数据类型
        /// </summary>
        public String m_dataType = "";

        /// <summary>
        /// 指标
        /// </summary>
        public CIndicator m_indicator;

        /// <summary>
        /// 类型
        /// </summary>
        public String m_type = "";

        /// <summary>
        /// 地址
        /// </summary>
        public String m_url = "";
    }
    
    /// <summary>
    /// Ajax响应信息
    /// </summary>
    public class AjaxResponse
    {
        /// <summary>
        /// 创建响应
        /// </summary>
        public AjaxResponse()
        {
        }

        /// <summary>
        /// 创建请求
        /// </summary>
        /// <param name="indicator">指标</param>
        /// <param name="type">类型</param>
        /// <param name="url">地址</param>
        /// <param name="dataType">数据类型</param>
        /// <param name="callBack">回调</param>
        /// <param name="resStr">回调字符串</param>
        public AjaxResponse(CIndicator indicator, String type, String url, String dataType, String callBack, String resStr)
        {
            m_indicator = indicator;
            m_url = url;
            m_type = type;
            m_dataType = dataType;
            m_callBack = callBack;
            m_resStr = resStr;
        }

        /// <summary>
        /// 回调函数
        /// </summary>
        public String m_callBack = "";

        /// <summary>
        /// 数据类型
        /// </summary>
        public String m_dataType = "";

        /// <summary>
        /// 指标
        /// </summary>
        public CIndicator m_indicator;

        /// <summary>
        /// 响应字符串
        /// </summary>
        public String m_resStr = "";

        /// <summary>
        /// 类型
        /// </summary>
        public String m_type = "";

        /// <summary>
        /// 地址
        /// </summary>
        public String m_url = "";
    }

    /// <summary>
    /// Ajax库
    /// </summary>
    public class CFunctionAjax : CFunction
    {
         /// <summary>
        /// 创建方法
        /// </summary>
        /// <param name="indicator">指标</param>
        /// <param name="id">ID</param>
        /// <param name="name">名称</param>
        public CFunctionAjax(CIndicator indicator, int id, String name)
        {
            m_indicator = indicator;
            m_ID = id;
            m_name = name;
        }

        /// <summary>
        /// 指标
        /// </summary>
        public CIndicator m_indicator;

        /// <summary>
        /// 调用控件
        /// </summary>
        private static ControlA m_listener;

        /// <summary>
        /// AJAX请求
        /// </summary>
        public static List<AjaxRequest> m_requests;

        /// <summary>
        /// 方法
        /// </summary>
        private static String FUNCTIONS = "AJAX";

        /// <summary>
        /// 前缀
        /// </summary>
        private static String PREFIX = "$.";

        /// <summary>
        /// 开始索引
        /// </summary>
        private const int STARTINDEX = 30000;

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="var">变量</param>
        /// <returns>结果</returns>
        public override double OnCalculate(CVariable var)
        {
            switch (var.m_functionID)
            {
                case STARTINDEX + 0:
                    return AJAX(var);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 添加方法
        /// </summary>
        /// <param name="indicator">方法库</param>
        /// <returns>指标</returns>
        public static void AddFunctions(CIndicator indicator)
        {
            String[] functions = FUNCTIONS.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int functionsSize = functions.Length;
            for (int i = 0; i < functionsSize; i++)
            {
                indicator.AddFunction(new CFunctionAjax(indicator, STARTINDEX + i, PREFIX + functions[i]));
            }
            if (m_requests == null)
            {
                m_requests = new List<AjaxRequest>();
                Thread thread = new Thread(new ThreadStart(AjaxHandle));
                thread.Start();
            }
        }

        /// <summary>
        /// Ajax处理类
        /// </summary>
        public static void AjaxHandle()
        {
            while (true)
            {
                AjaxRequest request = null;
                if (m_requests != null)
                {
                    lock (m_requests)
                    {
                        int requestsSize = m_requests.Count;
                        if (requestsSize > 0)
                        {
                            request = m_requests[requestsSize - 1];
                            m_requests.RemoveAt(requestsSize - 1);
                        }
                    }
                }
                if (request != null)
                {
                    String resStr = "";
                    String lowerType = request.m_type.ToLower();
                    if (lowerType == "get")
                    {
                        resStr = HttpGetService.Get(request.m_url);
                    }
                    else if (lowerType == "post")
                    {
                        resStr = HttpPostService.Post(request.m_url);
                    }
                    if (m_listener != null)
                    {
                        m_listener.Invoke(new AjaxResponse(request.m_indicator, request.m_type, request.m_url, request.m_dataType, request.m_callBack, resStr));
                    }
                    else
                    {
                        request.m_indicator.CallFunction(request.m_callBack + "('" + resStr + "');");
                    }
                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="args">参数</param>
        private static void Invoke(object sender, object args)
        {
            AjaxResponse response = args as AjaxResponse;
            if (response != null)
            {
                response.m_indicator.CallFunction(response.m_callBack + "('" + response.m_resStr + "');");
            }
        }

        /// <summary>
        /// 设置调用控件
        /// </summary>
        /// <param name="control">控件</param>
        public static void SetListener(ControlA control)
        {
            m_listener = control;
            m_listener.RegisterEvent(new ControlInvokeEvent(Invoke), EVENTID.INVOKE);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Ajax请求数据
        /// </summary>
        /// <param name="var">变量</param>
        /// <returns>状态</returns>
        public double AJAX(CVariable var)
        {
            lock (m_requests)
            {
                Dictionary<String, String> parameters = new Dictionary<String, String>();
                int parametersLen = var.m_parameters.Length;
                AjaxRequest request = new AjaxRequest();
                request.m_indicator = m_indicator;
                for (int i = 0; i < parametersLen; i++)
                {
                    String parameter = m_indicator.GetText(var.m_parameters[i]);
                    int idx = parameter.IndexOf(':');
                    String name = parameter.Substring(0, idx).ToLower();
                    String value = parameter.Substring(idx + 1);
                    if (name == "type")
                    {
                        request.m_type = value;
                    }
                    else if (name == "url")
                    {
                        request.m_url = value;
                    }
                    else if (name == "datatype")
                    {
                        request.m_dataType = value;
                    }
                    else if (name == "success")
                    {
                        request.m_callBack = value;
                    }
                }
                m_requests.Add(request);

            }
            return 1;
        }
    }
}
