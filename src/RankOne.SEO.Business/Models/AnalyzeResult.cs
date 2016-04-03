﻿using System.Collections.Generic;

namespace RankOne.Business.Models
{
    public class AnalyzeResult
    {
        public AnalyzeResult()
        {
            ResultRules = new List<ResultRule>();
        }

        public string Title { get; set; }
        public List<ResultRule> ResultRules { get; set; }
    }
}