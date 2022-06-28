using System.ComponentModel;

namespace PayCenter.Enums
{
    public enum NewFlag
    {
        [Description("批准")]
        Deposit = 2,

        [Description("拒绝")]
        Withdraw = -3,
    }
}
