using System.ComponentModel;

namespace PayCenter.Enums
{
    public enum DepositFlag
    {
        [Description("拒绝")]
        Refused = -3,

        [Description("等待")]
        Waitting = 0,

        [Description("批准")]
        Approved = 2,
    }
}
