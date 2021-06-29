//////////////////////////////// 
// 
//   Copyright 2021 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using CSETWeb_Api.BusinessManagers.Analysis;
using CSETWeb_Api.Controllers.newstuff;
using DataLayerCore.Manual;
using DataLayerCore.Model;
using Snickler.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSETWeb_Api.Controllers
{
    public class RRASummary
    {
        
        private CSET_Context context;        
        public RRASummary(CSET_Context context)
        {
            this.context = context;
        }

        internal List<usp_getRRASummaryOverall> getSummaryOverall(CSET_Context context, int assessmentId)
        {
            List<usp_getRRASummaryOverall> results = null;

            context.LoadStoredProc("[usp_getRRASummaryOverall]")
            .WithSqlParam("assessment_id", assessmentId)
            .ExecuteStoredProc((handler) =>
            {
                results = handler.ReadToList<usp_getRRASummaryOverall>().ToList();
            });
            return results;
        }

        internal List<usp_getRRASummary> getRRASummary(CSET_Context context, int assessmentId)
        {
            List<usp_getRRASummary> results = null;

            context.LoadStoredProc("[usp_getRRASummary]")
            .WithSqlParam("assessment_id", assessmentId)
            .ExecuteStoredProc((handler) =>
            {
                results = handler.ReadToList<usp_getRRASummary>().ToList();
            });
            return results;

        }

        internal List<usp_getRRASummaryByGoal> getRRASummaryByGoal(CSET_Context context, int assessmentId)
        {
            List<usp_getRRASummaryByGoal> results = null;

            context.LoadStoredProc("[usp_getRRASummaryByGoal]")
            .WithSqlParam("assessment_id", assessmentId)
            .ExecuteStoredProc((handler) =>
            {
                results = handler.ReadToList<usp_getRRASummaryByGoal>().ToList();
            });
            return results;

        }

        internal List<usp_getRRASummaryByGoalOverall> getRRASummaryByGoalOverall(CSET_Context context, int assessmentId)
        {
            List<usp_getRRASummaryByGoalOverall> results = null;

            context.LoadStoredProc("[usp_getRRASummaryByGoalOverall]")
            .WithSqlParam("assessment_id", assessmentId)
            .ExecuteStoredProc((handler) =>
            {
                results = handler.ReadToList<usp_getRRASummaryByGoalOverall>().ToList();
            });
            return results;
        }


        internal List<usp_getRRAQuestionsDetails> getRRAQuestions(CSET_Context context, int assessmentId)
        {
            List<usp_getRRAQuestionsDetails> results = null;

            context.LoadStoredProc("[usp_getRRAQuestionsDetails]")
            .WithSqlParam("assessment_id", assessmentId)
            .ExecuteStoredProc((handler) =>
            {
                results = handler.ReadToList<usp_getRRAQuestionsDetails>().ToList();
            });

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal List<string> GetQuestions(CSET_Context context, int assessmentId)
        {
            List<string> results = new List<string>();

            return results;
        }


        internal Dictionary<string, RRAQuestions> ProcessStructure(List<usp_getRRAQuestionsDetails> questions,
                                     List<usp_getRRASummaryByGoal> goalAnswers,
                                       List<usp_getRRASummaryByGoalOverall> goals)
        {
            /* I have three data sets that need to be grouped up
             * so create a dictionary of them all and then just assign the values.
             */
            Dictionary<string, usp_getRRASummaryByGoalOverall> top 
                = goals.ToDictionary(mc => mc.Title, mc => mc, StringComparer.OrdinalIgnoreCase);
            Dictionary<string, RRAQuestions> rvalue = new Dictionary<string, RRAQuestions>();
            foreach (var ans in top)
            {
                rvalue.Add(ans.Key, new RRAQuestions()
                {
                    goalName = ans.Key,
                    qc = ans.Value.qc,
                    Total = ans.Value.Total,
                    Percent = ans.Value.Percent
                });
            }

            foreach (var a in goalAnswers)
            {
                RRAQuestions tmp;
                if(!rvalue.TryGetValue(a.Title,out tmp))
                {
                    tmp = new RRAQuestions()
                    {
                        goalName = a.Title
                    };
                    rvalue.Add(a.Title, tmp);
                }
                
                switch (a.Answer_Text.ToUpper())
                {
                    case "Y":
                        tmp.YesCount = a.qc;
                        tmp.Percent = Convert.ToInt32(((Convert.ToDouble(a.qc) / Convert.ToDouble(rvalue[a.Title].Total)) * 100));
                        break;
                    case "N":
                        tmp.NoCount = a.qc;
                        break;
                    case "U":
                        tmp.UnAnsweredCount = a.qc;
                        break;
                }
                
            }

            foreach (var q in questions)
            {
                RRAQuestions tmp;
                if (!rvalue.TryGetValue(q.Title, out tmp))
                {
                    tmp = new RRAQuestions()
                    {
                        goalName = q.Title
                    };
                    rvalue.Add(q.Title, tmp);
                }
                tmp.goalQuestions.Add(q);
            }
            return rvalue;

        }
    }
}