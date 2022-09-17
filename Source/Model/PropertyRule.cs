using System;
using System.Text.RegularExpressions;

namespace CodeM.Common.Orm
{
    public enum RulePattern
    {
        Unset = 0,
        Email = 1,
        IP = 2,
        Url = 3,
        Mobile = 4,
        Telephone = 5,
        IDCard = 6
    }

    [Serializable]
    public class PropertyRule
    {
        public RulePattern Pattern { get; set; } = RulePattern.Unset;

        public Regex Regex { get; set; } = null;

        public string ValidationProcessor { get; set; } = string.Empty;

        public string Message { get; set; } = String.Empty;
    }
}
