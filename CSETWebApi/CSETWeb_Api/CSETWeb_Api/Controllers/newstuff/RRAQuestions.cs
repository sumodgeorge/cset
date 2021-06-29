using DataLayerCore.Manual;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSETWeb_Api.Controllers.newstuff
{
    public class RRAQuestions
    {   
        public RRAQuestions()
        {
            goalQuestions = new List<usp_getRRAQuestionsDetails>();
        }

        public string goalName { get; set; }        
        public int? qc { get; set; }
        public int? Total { get; set; }
        public int? Percent { get; set; }
        public int? YesCount { get; set; }
        public int? NoCount { get; set; }
        public int? UnAnsweredCount { get; set; }
        public List<usp_getRRAQuestionsDetails> goalQuestions { get; set; }
    }
}