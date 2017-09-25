using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wlniao;
namespace v1test
{
    public class TestTask
    {
        private static string url = "";
        /// <summary>
        /// 测试方法
        /// </summary>
        private static void TestMethod()
        {
            var startTime = DateTime.Now;
            var useTime = new TimeSpan(0);
            var testKey = startTime.Ticks.ToString() + "-" + startTime.ToString("yyyyMMddHHmmss");
            if (!RunTime.TaskLogs.ContainsKey(testKey))
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    var testRlt = "";
                    var statusCode = "OK";
                    var reqest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
                    reqest.Headers.Date = DateTime.UtcNow;
                    reqest.Headers.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("Wlniao-v1test", "beta"));
                    try
                    {
                        client.SendAsync(reqest).ContinueWith((requestTask) =>
                        {
                            try
                            {
                                var response = requestTask.Result;
                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    response.Content.ReadAsStringAsync().ContinueWith((readTask) =>
                                    {
                                        testRlt = readTask.Result;
                                        useTime = DateTime.Now - startTime;
                                        RunTime.ResultSuccess = RunTime.ResultSuccess + 1;
                                        RunTime.ResultCount = RunTime.ResultCount + 1;
                                        RunTime.TotalUseTime += useTime.TotalMilliseconds;
                                    }).Wait();
                                }
                                else
                                {
                                    response.Content.ReadAsStringAsync().ContinueWith((readTask) =>
                                    {
                                        testRlt = readTask.Result;
                                        useTime = DateTime.Now - startTime;
                                        statusCode = response.StatusCode.ToString();
                                        RunTime.ResultError = RunTime.ResultError + 1;
                                        RunTime.ResultCount = RunTime.ResultCount + 1;
                                        RunTime.TotalUseTime += useTime.TotalMilliseconds;
                                    }).Wait();
                                }
                            }
                            catch (Exception ex)
                            {
                                testRlt = testRlt.PadLeft(16, ' ');
                                useTime = DateTime.Now - startTime;
                                statusCode = "Exception";
                                RunTime.ResultError = RunTime.ResultError + 1;
                                RunTime.ResultCount = RunTime.ResultCount + 1;
                                RunTime.TotalUseTime += useTime.TotalMilliseconds;
                            }
                        }).Wait();
                    }
                    catch
                    {
                        testRlt = testRlt.PadLeft(16, ' ');
                        useTime = DateTime.Now - startTime;
                        statusCode = "Exception";
                        RunTime.ResultError = RunTime.ResultError + 1;
                        RunTime.ResultCount = RunTime.ResultCount + 1;
                        RunTime.TotalUseTime += useTime.TotalMilliseconds;
                    }
                    if (RunTime.CacheResult)
                    {
                        RunTime.TaskLogs.Add(testKey, (Int64)useTime.TotalMilliseconds);
                    }
                    Console.WriteLine(RunTime.ActiveCount.ToString().PadLeft(4, ' ') + "\t" + testKey + "\t" + statusCode.PadLeft(8, ' ') + "\t" + (string.IsNullOrEmpty(testRlt) ? "" : Wlniao.Encryptor.Md5Encryptor16(testRlt)) + "\t" + useTime.TotalMilliseconds.ToString("F2") + "ms");
                }
            }
        }

        private static async void NewThread()
        {
            RunTime.ActiveCount = RunTime.ActiveCount + 1;
            while (true)
            {
                try
                {
                    TestMethod();
                    if (RunTime.ActiveInterval > 0)
                    {
                        await Task.Delay(RunTime.ActiveInterval * 1000);
                    }
                    else
                    {
                        await Task.Delay(1000);
                    }
                }
                catch
                {
                    await Task.Delay(1000);
                }
            }
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        public static async void CreateTask()
        {
            if (!RunTime.IsStart)
            {
                RunTime.IsStart = true;
                if (string.IsNullOrEmpty(RunTime.TestUrl))
                {
                    Config.SetConfigs("TestUrl", "");
                    if (RunTime.TaskCount < 0)
                    {
                        RunTime.TaskCount = 1000;
                        RunTime.BetweenTime = 1;
                        RunTime.ActiveInterval = 15;
                        Config.SetConfigs("TaskCount", RunTime.TaskCount.ToString());
                        Config.SetConfigs("BetweenTime", RunTime.BetweenTime.ToString());
                        Config.SetConfigs("ActiveInterval", RunTime.ActiveInterval.ToString());
                        Config.SetConfigs("CacheResult", "false");
                    }
                    Console.WriteLine("\r\nNot set config key TestUrl!");
                }
                else if (RunTime.TaskCount <= 0)
                {
                    Console.WriteLine("\r\nNot set config key TaskCount!");
                }
                else
                {
                    url = RunTime.TestUrl;
                    Console.WriteLine("\r\nTest Url=" + url + "");
                    Console.WriteLine("\r\n\r\nActive\tTask Key\t\t\t\tStatus\t\tHash\t\t\tUseTime");
                    while (RunTime.ActiveCount < RunTime.TaskCount)
                    {
                        NewThread();
                        if (RunTime.BetweenTime > 0)
                        {
                            await Task.Delay(RunTime.BetweenTime * 1000);
                        }
                    }
                }
                Console.WriteLine("\r\n");
            }
        }

        /// <summary>
        /// 启动测试
        /// </summary>
        public static async void Start()
        {
            try
            {
                await Task.Delay(5000);
                using (var client = new System.Net.Http.HttpClient())
                {
                    var reqest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, "http://localhost:" + Wlniao.XCore.ListenPort + "/start");
                    reqest.Headers.Date = DateTime.UtcNow;
                    client.SendAsync(reqest).ContinueWith((requestTask) =>
                    {
                        var response = requestTask.Result;
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            response.Content.ReadAsStringAsync().ContinueWith((readTask) =>
                            {
                                var testRlt = readTask.Result;
                            }).Wait();
                        }
                        else
                        {
                            response.Content.ReadAsStringAsync().ContinueWith((readTask) =>
                            {
                                var testRlt = readTask.Result;
                            }).Wait();
                        }
                    }).Wait();
                }
            }
            catch { }
        }
    }
}