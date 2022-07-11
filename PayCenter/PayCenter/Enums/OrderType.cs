using System.ComponentModel;

namespace PayCenter.Enums
{
    public enum OrderType
    {
        [Description("网银PC端快捷支付")]
        FastPay = 18,

        [Description("虚拟币支付")]
        DigitalCoin = 25,
    }
}
