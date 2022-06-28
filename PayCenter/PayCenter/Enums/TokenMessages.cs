using System.ComponentModel;

namespace PayCenter.Enums
{
    public enum TokenMessages
    {
        [Description("账号错误")]
        invalidAccount = 0,

        [Description("密码错误")]
        invalidPwd = 1,

        [Description("pid错误")]
        invalidPid = 2,
    }
}
