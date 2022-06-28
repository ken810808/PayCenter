using System.ComponentModel;

namespace PayCenter.Enums
{
    public enum RequestType
    {
        [Description("存款")]
        Refused = 1,

        [Description("取款")]
        Waitting = 2,

    }
}
