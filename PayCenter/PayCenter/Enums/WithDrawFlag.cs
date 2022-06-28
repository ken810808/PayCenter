using System.ComponentModel;

namespace PayCenter.Enums
{
    public enum WithDrawFlag
    {
        [Description("拒绝")]
        Refused = -3,

        [Description("待付款")]
        Waitting = 1,

        [Description("已付款")]
        Approved = 2,
    }
}
