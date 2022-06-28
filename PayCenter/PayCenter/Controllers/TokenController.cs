using Microsoft.AspNetCore.Mvc;
using PayCenter.Enums;
using PayCenter.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PayCenter.Controllers
{
    [ApiController]
    public class TokenController : Controller
    {
        private readonly ILogger<TokenController> _logger;
        private readonly JsonSerializerOptions _options;
        public TokenController(ILogger<TokenController> logger, JsonSerializerOptions options)
        {
            _logger = logger;
            _options = options;
        }

        [HttpGet]
        [Route("Test")]
        public ActionResult Test()
        {
            _logger.LogDebug($"{nameof(StatusCodes.Status200OK)},測試接口正常");
            return Ok("測試接口正常");
        }


        [HttpPost]
        [Route("token")]
        // 为其他接口调用取得Token，将token放入header(Authorization)裡面，校验调用者的身份是否合法
        public ActionResult Post([FromBody] RequestInfo requestParam)
        {
            try
            {
                var errorInfo = new ErrorInfo();
                if (requestParam.Pid != "V26")
                {
                    errorInfo.Code = "BAD_REQUEST";
                    errorInfo.Message = EnumHelper.GetEnumDescription(TokenMessages.invalidPid);
                    _logger.LogDebug($"{nameof(StatusCodes.Status401Unauthorized)}, {JsonSerializer.Serialize(errorInfo, _options)}");
                    return BadRequest(JsonSerializer.Serialize(errorInfo, _options));
                }

                if (requestParam.Account != "qbtestuser")
                {
                    errorInfo.Code = "BAD_REQUEST";
                    errorInfo.Message = EnumHelper.GetEnumDescription(TokenMessages.invalidAccount);
                    _logger.LogDebug($"{nameof(StatusCodes.Status401Unauthorized)}, {JsonSerializer.Serialize(errorInfo, _options)}");
                    return BadRequest(JsonSerializer.Serialize(errorInfo, _options));
                }

                if (requestParam.Pwd != "qbtest123456")
                {
                    errorInfo.Code = "BAD_REQUEST";
                    errorInfo.Message = EnumHelper.GetEnumDescription(TokenMessages.invalidPwd);
                    _logger.LogDebug($"{nameof(StatusCodes.Status401Unauthorized)}, {JsonSerializer.Serialize(errorInfo, _options)}");
                    return BadRequest(JsonSerializer.Serialize(errorInfo, _options));
                }

                var tokenInfo = new TokenInfo
                {
                    Ip = "1.1.1.1",
                    Iat = "10",
                    Exp = "10000",
                    Token = "token_123"
                };

                _logger.LogDebug($"{nameof(StatusCodes.Status200OK)}, {JsonSerializer.Serialize(tokenInfo, _options)}");
                return Ok(JsonSerializer.Serialize(tokenInfo, _options));
            }

            catch (Exception ex)
            {
                _logger.LogError($"{ex.StackTrace}");
                return BadRequest();
            }
        }

        public class RequestInfo
        {
            /// <summary>
            /// 产品ID
            /// </summary>
            [JsonPropertyName("pid")]
            public string Pid { get; set; }

            /// <summary>
            /// 业务系统分配的账号
            /// </summary>
            [JsonPropertyName("account")]
            public string Account { get; set; }

            /// <summary>
            /// 业务系统分配的秘钥
            /// </summary>
            [JsonPropertyName("pwd")]
            public string Pwd { get; set; }
        }

        public class TokenInfo
        {
            /// <summary>
            /// IP
            /// </summary>
            [JsonPropertyName("ip")]
            public string Ip { get; set; }

            /// <summary>
            /// Token生效时间毫秒数
            /// </summary>
            [JsonPropertyName("iat")]
            public string Iat { get; set; }

            /// <summary>
            /// Token过期时间毫秒数
            /// </summary>
            [JsonPropertyName("exp")]
            public string Exp { get; set; }

            /// <summary>
            /// Token 其他接口调用使用
            /// </summary>
            [JsonPropertyName("token")]
            public string Token { get; set; }
        }

        public class ErrorInfo
        {
            /// <summary>
            /// 错误CODE
            /// </summary>
            [JsonPropertyName("code")]
            public string Code { get; set; }

            /// <summary>
            /// invalid account：账号错误
            /// invalid pwd：密码错误
            /// Invalid pid： pid错误
            /// </summary>
            [JsonPropertyName("message")]
            public string Message { get; set; }
        }
    }
}
