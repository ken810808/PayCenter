using Microsoft.AspNetCore.Mvc;
using PayCenter.MiddleWares;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PayCenter.Controllers
{
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly JsonSerializerOptions _options;

        public CustomerController(ILogger<CustomerController> logger, JsonSerializerOptions options)
        {
            _logger = logger;
            _options = options;
        }

        [HttpGet]
        [MiddlewareFilter(typeof(TokenAuthMiddlewarePipeline))]
        [Route("customer")]
        // 会员通过在线支付存款时校验账号是否存在，并且是否为可存款类型账号。返回有效的会员
        public ActionResult Get([FromBody] RequestInfo requestInfo)
        {
            try
            {
                _logger.LogDebug($"請求參數: {JsonSerializer.Serialize(requestInfo, _options)}");

                //if (requestInfo.Pid != "pid_123")
                //{
                //    _logger.LogDebug($"{nameof(StatusCodes.Status401Unauthorized)}, 返回參數: {JsonSerializer.Serialize(requestInfo, _options)}");
                //    return BadRequest("");
                //}

                var customer = new Customer()
                {

                };

                _logger.LogDebug($"{nameof(StatusCodes.Status200OK)}, 返回參數: {JsonSerializer.Serialize(customer, _options)}");
                return Ok(JsonSerializer.Serialize(customer, _options));
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
            /// 会员账号
            /// </summary>
            [JsonPropertyName("login_name")]
            public string LoginName { get; set; }
        }

        public class Customer
        {
            /// <summary>
            /// 客户等级
            /// </summary>
            [JsonPropertyName("customer_level")]
            public string CustomerLevel { get; set; }

            /// <summary>
            /// 会员类别(1表示真钱)
            /// </summary>
            [JsonPropertyName("customer_type")]
            public string CustomerType { get; set; }

            /// <summary>
            /// 优先级
            /// </summary>
            [JsonPropertyName("priority_level")]
            public string PriorityLevel { get; set; }

            /// <summary>
            /// 信用等级
            /// </summary>
            [JsonPropertyName("deposit_level")]
            public string DepositLevel { get; set; }

            /// <summary>
            /// 登录名
            /// </summary>
            [JsonPropertyName("login_name")]
            public string LoginName { get; set; }

            /// <summary>
            /// 真实姓名（明文）
            /// </summary>
            [JsonPropertyName("real_name")]
            public string RealName { get; set; }
        }
    }
}


