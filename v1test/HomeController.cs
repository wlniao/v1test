using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace v1test
{
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            var _html = "<!DOCTYPE html>";
            _html += "<html>";
            _html += "<head>";
            _html += "<title>Wlniao Test Project</title>";
            _html += "<style>";
            _html += "  h3,h4{font-style:normal;font-weight:normal;line-height:1rem;}";
            _html += "  em{display:inline-block;min-width:168px;font-style:normal;}";
            _html += "</style>";
            _html += "</head>";
            _html += "<body>";
            try
            {
                _html += "<h1>Test host:" + RunTime.HostIndex + "</h1>";
                if (RunTime.IsStart)
                {
                    _html += "<h3><em>TestUrl</em>" + RunTime.TestUrl + "</h3>";
                    _html += "<h3><em>TaskCount</em>" + RunTime.TaskCount + "</h3>";
                    _html += "<h3><em>Active Count</em>" + RunTime.ActiveCount + "</h3>";
                    _html += "<h3><em>Test times</em>" + RunTime.ResultCount + "</h3>";
                    _html += "<h3><em>Success times</em>" + RunTime.ResultSuccess + "</h3>";
                    _html += "<h3><em>Error times</em>" + RunTime.ResultError + "</h3>";
                    RunTime.Average();
                    var usetime = RunTime.GetAverageUseTimeList();
                    if (usetime != null && usetime.Count > 0)
                    {
                        _html += "<h3><em>Average Use Time</em>:</h3>";
                        foreach (var item in usetime)
                        {
                            if (item.Key == RunTime.HostIndex)
                            {
                                _html += "<h4 style=\"color:green;\"><em>Host-" + item.Key + "</em>" + item.Value + "</h4>";
                            }
                            else
                            {
                                _html += "<h4><em>Host-" + item.Key + "</em>" + item.Value + "</h4>";
                            }
                        }
                    }
                    var success = RunTime.GetAverageSuccessList();
                    if (success != null && success.Count > 0)
                    {
                        _html += "<h3><em>Average Success</em>:</h3>";
                        foreach (var item in success)
                        {
                            if (item.Key == RunTime.HostIndex)
                            {
                                _html += "<h4 style=\"color:green;\"><em>Host-" + item.Key + "</em>" + item.Value + "</h4>";
                            }
                            else
                            {
                                _html += "<h4><em>Host-" + item.Key + "</em>" + item.Value + "</h4>";
                            }
                        }
                    }
                    _html += "<h3><em>Response Time</em>" + Wlniao.DateTools.GetNow().ToString("HH:mm:ss") + "</h3>";
                }
                else
                {
                    _html += "<p>Please <a href=\"/start\" _taget=\"_blank\">click here</a> to start the test</p>";
                }
            }
            catch (Exception ex)
            {
                _html += ex.Message;
            }
            _html += "</body>";
            _html += "</html>";
            return Content(_html, "text/html");
        }
        [Route("start")]
        public IActionResult Start()
        {
            if (RunTime.IsStart)
            {
                return Content("Has been started", "text/html");
            }
            else
            {
                TestTask.CreateTask();
                return Content("Start Success", "text/html");
            }
        }
        [Route("clear")]
        public IActionResult Clear()
        {
            if (Wlniao.Cache.Exists("v1test"))
            {
                if (Wlniao.Cache.Del("v1test"))
                {
                    RunTime.HostingList = new string[] { };
                    return Content("Clear Success", "text/html");
                }
                else
                {
                    return Content("Clear Error", "text/html");
                }
            }
            else
            {
                return Content("Cache not Exists", "text/html");
            }
        }
    }
}
