﻿using HtmlAgilityPack;

namespace RankOne.Business.Models
{
    public class HtmlResultVm
    {
        public string Url { get; set; }
        public long ServerResponseTime { get; set; }
        public int Size { get; set; }
    }
}
