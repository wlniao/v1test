using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace v1test
{
    public class RunTime
    {
        private static string hostIndex = "";
        private static string testUrl = null;
        private static int taskCount = -1;
        private static void ReadCfg()
        {
            if (taskCount < 0 && string.IsNullOrEmpty(testUrl))
            {
                testUrl = Wlniao.Config.GetSetting("TestUrl");
                taskCount = Wlniao.cvt.ToInt(Wlniao.Config.GetSetting("TaskCount"));
                BetweenTime = Wlniao.cvt.ToInt(Wlniao.Config.GetSetting("BetweenTime"));
                ActiveInterval = Wlniao.cvt.ToInt(Wlniao.Config.GetSetting("ActiveInterval"));
                CacheResult = Wlniao.cvt.ToBool(Wlniao.Config.GetSetting("CacheResult"));
                hostIndex = Wlniao.Config.GetEnvironment("HostIndex");
            }
        }

        public static Boolean IsStart = false;
        /// <summary>
        /// 任务间隔时间
        /// </summary>
        public static int BetweenTime = 0;
        /// <summary>
        /// 活跃间隔时间
        /// </summary>
        public static int ActiveInterval = 0;
        /// <summary>
        /// 测试Url
        /// </summary>
        public static string TestUrl
        {
            get
            {
                ReadCfg();
                return testUrl;
            }
        }
        /// <summary>
        /// 生成的任务数量
        /// </summary>
        public static int TaskCount
        {
            get
            {
                ReadCfg();
                return taskCount;
            }
            set
            {
                taskCount = value;
            }
        }
        /// <summary>
        /// 暂存测试结果
        /// </summary>
        public static Dictionary<String, Int64> TaskLogs = new Dictionary<String, Int64>();

        /// <summary>
        /// 是否要缓存结果
        /// </summary>
        public static Boolean CacheResult = false;
        
        /// <summary>
        /// 当前激活线程数量
        /// </summary>
        public static int ActiveCount = 0;


        private static string _lock = "lock";
        /// <summary>
        /// 主机编号
        /// </summary>
        public static string HostIndex
        {
            get
            {
                lock (_lock)
                {
                    if (string.IsNullOrEmpty(hostIndex))
                    {
                        hostIndex = new Random().Next(100000, 999999).ToString();
                    }
                    if (HostingList == null || HostingList.Length == 0)
                    {
                        var HostList = new List<String>();
                        var hosts = Wlniao.Cache.Get("v1test").Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var host in HostList)
                        {
                            if (Wlniao.strUtil.IsNumber(host))
                            {
                                HostList.Add(host);
                            }
                        }
                        if (!HostList.Contains(hostIndex))
                        {
                            HostList.Add(hostIndex);
                            var str = Wlniao.strUtil.Join("#", HostList.ToArray());
                            Wlniao.Cache.Set("v1test", str);
                        }
                        HostingList = HostList.ToArray();
                    }
                    return hostIndex;
                }
            }
        }

        /// <summary>
        /// 主机列表
        /// </summary>
        public static String[] HostingList = new String[] { };

        public static Int32 ResultCount = 0;
        public static Int32 ResultSuccess = 0;
        public static Int32 ResultError = 0;
        public static Double TotalUseTime = 0;

        /// <summary>
        /// 计算平均相应情况
        /// </summary>
        /// <returns></returns>
        public static void Average()
        {
            try
            {
                Double averageUseTime = 0;
                Double averageSuccess = 0;
                if (ResultCount > 0)
                {
                    if (TotalUseTime > 0)
                    {
                        averageUseTime = TotalUseTime / ResultCount;
                    }
                    if (ResultSuccess > 0)
                    {
                        averageSuccess = (ResultSuccess * 100f) / ResultCount;
                    }
                }
                Wlniao.Cache.Set(HostIndex + "AverageUseTime", averageUseTime.ToString("F2") + "ms");
                Wlniao.Cache.Set(HostIndex + "AverageSuccess", averageSuccess.ToString("F2") + "%");
            }
            catch { }
        }
        /// <summary>
        /// 获取全部测试主机的平均响应时间
        /// </summary>
        /// <returns></returns>
        public static Dictionary<String, String> GetAverageUseTimeList()
        {
            var dic = new Dictionary<String, String>();
            try
            {
                foreach (var hosting in HostingList)
                {
                    var val = Wlniao.Cache.Get(hosting + "AverageUseTime");
                    if (!string.IsNullOrEmpty(val))
                    {
                        dic.Add(hosting, val);
                    }
                }

            }
            catch { }
            if (!dic.ContainsKey(HostIndex))
            {
                Double averageUseTime = 0;
                if (ResultCount > 0)
                {
                    if (TotalUseTime > 0)
                    {
                        averageUseTime = TotalUseTime / ResultCount;
                    }
                }
                dic.Add(HostIndex, averageUseTime.ToString("F2") + "ms");
            }
            return dic;
        }
        /// <summary>
        /// 获取全部测试主机的平均响应成功率
        /// </summary>
        /// <returns></returns>
        public static Dictionary<String, String> GetAverageSuccessList()
        {
            var dic = new Dictionary<String, String>();
            try
            {
                foreach (var hosting in HostingList)
                {
                    var val = Wlniao.Cache.Get(hosting + "AverageSuccess");
                    if (!string.IsNullOrEmpty(val))
                    {
                        dic.Add(hosting, val);
                    }
                }
            }
            catch { }
            if (!dic.ContainsKey(HostIndex))
            {
                Double averageSuccess = 0;
                if (ResultCount > 0)
                {
                    if (ResultSuccess > 0)
                    {
                        averageSuccess = (ResultSuccess * 100f) / ResultCount;
                    }
                }
                dic.Add(HostIndex, averageSuccess.ToString("F2") + "%");
            }
            return dic;
        }
    }
}
