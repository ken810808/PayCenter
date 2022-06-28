using System.ComponentModel;

namespace PayCenter.Enums
{
    public enum UserType
    {
        [Description("会员")]
        C = 0,

        [Description("客服")]
        U = 1,

        [Description("资金")]
        Z = 2,
    }
}
