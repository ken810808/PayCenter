using System.ComponentModel;

namespace PayCenter.Enums
{
    public enum CustomerType
    {
        [Description("会员")]
        ManualDeposit = 1,

        [Description("代理")]
        BQDeposit = 2,
    }
}
