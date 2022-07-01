using Microsoft.AspNetCore.Mvc;
using PayCenter.MiddleWares;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PayCenter.Controllers
{
    [ApiController]
    public class ApproveController : Controller
    {
        private readonly ILogger<ApproveController> _logger;
        private readonly JsonSerializerOptions _options;

        public ApproveController(ILogger<ApproveController> logger, JsonSerializerOptions options)
        {
            _logger = logger;
            _options = options;
        }

        /// <summary>
        /// 通知业务系统批准或拒绝存取款提案
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        [HttpPut]
        [MiddlewareFilter(typeof(MiddlewarePipeline))]
        [Route("approve")]
        public ActionResult Put([FromBody] RequestInfo requestInfo)
        {
            try
            {
                _logger.LogDebug($"請求參數: {JsonSerializer.Serialize(requestInfo, _options)}");

                //var errorInfo = new ErrorInfo();
                //if (!Enum.TryParse(requestInfo.RequestType, out RequestType requestType) ||
                //    !Enum.TryParse(requestInfo.UserType, out UserType userType) ||
                //    !Enum.TryParse(requestInfo.OldFlag, out OldFlag oldFlag) ||
                //    !Enum.TryParse(requestInfo.NewFlag, out NewFlag newFlag))
                //{
                //    errorInfo.Code = "BAD_REQUEST";
                //    errorInfo.Message = "BAD_REQUEST";
                //    _logger.LogDebug($"{nameof(StatusCodes.Status401Unauthorized)}, 返回參數: {JsonSerializer.Serialize(errorInfo, _options)}");
                //    return BadRequest(JsonSerializer.Serialize(errorInfo, _options));
                //}

                //if (requestInfo.Pid != "pid_123" ||
                //    //requestInfo.RequestId != "123.............." || // TODO: 请求ID(以产品id开头 18位,字母大小,不包含特殊字符)
                //    !Enum.IsDefined(typeof(RequestType), requestType) ||
                //    !Enum.IsDefined(typeof(OldFlag), oldFlag) ||
                //    !Enum.IsDefined(typeof(NewFlag), newFlag) ||
                //    requestInfo.ApproveBy != "approver_123" ||
                //    !Enum.IsDefined(typeof(UserType), userType) ||
                //    requestInfo.IpAddress != "0.0.0.0")
                //{
                //    errorInfo.Code = "BAD_REQUEST";
                //    errorInfo.Message = "BAD_REQUEST";
                //    _logger.LogDebug($"{nameof(StatusCodes.Status401Unauthorized)}, 返回參數: {JsonSerializer.Serialize(errorInfo, _options)}");
                //    return BadRequest(JsonSerializer.Serialize(errorInfo, _options));
                //}

                _logger.LogDebug($"{nameof(StatusCodes.Status200OK)}");
                return Ok("");
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
            /// 提案ID
            /// </summary>
            [JsonPropertyName("request_id")]
            public string RequestId { get; set; }

            /// <summary>
            /// 请求类型01存款02取款
            /// </summary>
            [JsonPropertyName("request_type")]
            public string RequestType { get; set; }

            /// <summary>
            /// 旧状态（存款为0,取款为1）
            /// </summary>
            [JsonPropertyName("old_flag")]
            public string OldFlag { get; set; }

            /// <summary>
            /// 新状态（批准为2，拒绝为-3）
            /// </summary>
            [JsonPropertyName("new_flag")]
            public string NewFlag { get; set; }

            /// <summary>
            /// 审批人
            /// </summary>
            [JsonPropertyName("approve_by")]
            public string ApproveBy { get; set; }

            /// <summary>
            /// C会员U客服Z资金
            /// </summary>
            [JsonPropertyName("user_type")]
            public string UserType { get; set; }

            /// <summary>
            /// 备注信息
            /// </summary>
            [JsonPropertyName("remarks")]
            public string? Remarks { get; set; }

            /// <summary>
            /// IP
            /// </summary>
            [JsonPropertyName("ip_address")]
            public string IpAddress { get; set; }
        }

        public class ErrorInfo
        {
            [JsonPropertyName("code")]
            public string Code { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }
        }
    }
}
