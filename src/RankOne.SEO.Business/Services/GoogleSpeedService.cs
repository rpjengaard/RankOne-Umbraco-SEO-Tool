using System;
using System.Net;
using System.Web;
using Google.Apis.Pagespeedonline.v2;
using Google.Apis.Pagespeedonline.v2.Data;
using Google.Apis.Services;
using ImageProcessor.Imaging;
using RankOne.Business.Models;

namespace RankOne.Business.Services
{
    public class GoogleSpeedService
    {
        private PageAnalysis _webpage;

        public GoogleSpeedService(PageAnalysis webpage)
        {
            _webpage = webpage;
        }

        public Analysis GetAnalysis()
        {
            var analysis = new Analysis();
            try
            {
                var service = new PagespeedonlineService(new BaseClientService.Initializer()
                {
                    ApplicationName = "RankOne SEO Toolkit",
                });
                var pageSpeed = service.Pagespeedapi.Runpagespeed(_webpage.Url);
                pageSpeed.Locale = "nl_nl";

                var result = pageSpeed.Execute();

                if (result.RuleGroups.ContainsKey("Speed") && result.RuleGroups["SPEED"].Score.HasValue)
                {
                    analysis.Score = result.RuleGroups["SPEED"].Score.Value;
                }

                foreach (var ruleResult in result.FormattedResults.RuleResults)
                {
                    var analyzeResult = new GoogleAnalyzeResult();
                    analyzeResult.Alias = ruleResult.Key.ToLower();
                    analyzeResult.Name = ruleResult.Value.LocalizedRuleName;
                    analyzeResult.Description = GetText(ruleResult.Value.Summary);

                    var type = ResultType.Success;
                    if (ruleResult.Value.RuleImpact.HasValue)
                    {
                        var value = ruleResult.Value.RuleImpact.Value;

                        if (value > 0 && value <= 3)
                        {
                            type = ResultType.Hint;
                        }
                        else if (value > 3 && value <= 7)
                        {
                            type = ResultType.Warning;
                        }
                        else if (value > 7)
                        {
                            type = ResultType.Hint;
                        }

                        foreach (var urlBlock in ruleResult.Value.UrlBlocks)
                        {
                            foreach (var url in urlBlock.Urls)
                            {
                                var text = GetText(url.Result);

                                analyzeResult.ResultRules.Add(new ResultRule
                                {
                                    Type = type,
                                    Text = text
                                });

                                analysis.Results.Add(analyzeResult);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return analysis;
        }

        private string GetText(PagespeedApiFormatStringV2 summary)
        {
            var text = summary.Format;
            foreach (var arg in summary.Args)
            {
                if (arg.Key == "LINK")
                {
                    var linkHtml = "<a href=\"" + arg.Value + "\" target=\"_blank\">";
                    text = text.Replace("{{BEGIN_LINK}}", linkHtml);
                    text = text.Replace("{{END_LINK}}", "</a>");
                }
                else
                {
                    text = text.Replace("{{" + arg.Key + "}}", arg.Value);
                }
            }
            return text;
        }
    }
}
