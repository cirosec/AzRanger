using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzRanger.Models.SharePoint
{
    public class SPOSearchQueryResult
    {
        [JsonPropertyName("@odata.context")]
        public string odatacontext { get; set; }
        public int ElapsedTime { get; set; }
        public SPOSearchQueryResultPrimaryqueryresult PrimaryQueryResult { get; set; }
        public SPOSearchQueryResultProperty2[] Properties { get; set; }
        public object[] SecondaryQueryResults { get; set; }
        public object SpellingSuggestion { get; set; }
        public object[] TriggeredRules { get; set; }
    }

    public class SPOSearchQueryResultPrimaryqueryresult
    {
        public object[] CustomResults { get; set; }
        public string QueryId { get; set; }
        public string QueryRuleId { get; set; }
        public object RefinementResults { get; set; }
        public SPOSearchQueryResultRelevantresults RelevantResults { get; set; }
        public object SpecialTermResults { get; set; }
    }

    public class SPOSearchQueryResultRelevantresults
    {
        public object GroupTemplateId { get; set; }
        public object ItemTemplateId { get; set; }
        public SPOSearchQueryResultProperty1[] Properties { get; set; }
        public object ResultTitle { get; set; }
        public object ResultTitleUrl { get; set; }
        public int RowCount { get; set; }
        public SPOSearchQueryResultTable Table { get; set; }
        public int TotalRows { get; set; }
        public int TotalRowsIncludingDuplicates { get; set; }
    }

    public class SPOSearchQueryResultTable
    {
        public SPOSearchQueryResultRow[] Rows { get; set; }
    }

    public class SPOSearchQueryResultRow
    {
        public SPOSearchQueryResultCell[] Cells { get; set; }
    }

    public class SPOSearchQueryResultCell
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
    }

    public class SPOSearchQueryResultProperty1
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
    }

    public class SPOSearchQueryResultProperty2
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
    }

}
