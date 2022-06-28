using System.ComponentModel;

namespace Payment.Enums
{
    public enum DepositType
    {
        [Description("人工存款")]
        ManualDeposit = 0,

        [Description("BQ存款")]
        BQDeposit = 1,
    }
}
