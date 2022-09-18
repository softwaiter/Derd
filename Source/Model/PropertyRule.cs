using CodeM.Common.Tools;
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

        private void CheckPattern(object value)
        {
            if (value != null)
            {
                bool bRet = false;
                switch (Pattern)
                {
                    case RulePattern.Email:
                        bRet = Xmtool.Regex().IsEmail(value.ToString());
                        break;
                    case RulePattern.IP:
                        break;
                    case RulePattern.Url:
                        break;
                    case RulePattern.Mobile:
                        break;
                    case RulePattern.Telephone:
                        break;
                    case RulePattern.IDCard:
                        break;
                    default:
                        break;
                }

                if (!bRet)
                {
                    if (!string.IsNullOrWhiteSpace(Message))
                    {
                        throw new Exception(Message);
                    }
                    throw new Exception(string.Concat("不匹配的模式：", Pattern));
                }
            }
        }

        private void CheckRegex(object value)
        {
            if (value != null)
            {
                if (!Regex.IsMatch(value.ToString()))
                {
                    if (!string.IsNullOrWhiteSpace(Message))
                    {
                        throw new Exception(Message);
                    }
                    throw new Exception(string.Concat("不匹配的正则表达式：", Regex));
                }
            }
        }

        private void CheckValidation(Property prop, object value)
        {
            Processor.CallRuleProcessor(ValidationProcessor, prop, value);
        }

        public void Validate(Property prop, object value)
        {
            if (Pattern != RulePattern.Unset)
            {
                CheckPattern(value);
            }

            if (Regex != null)
            {
                CheckRegex(value);
            }

            if (!string.IsNullOrWhiteSpace(ValidationProcessor))
            {
                CheckValidation(prop, value);
            }
        }
    }
}
