using Microsoft.AspNetCore.Mvc;
using PayCenter.Enums;
using PayCenter.MiddleWares;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PayCenter.Controllers
{
    [ApiController]
    public class PayController : Controller
    {
        private readonly ILogger<PayController> _logger;
        private readonly JsonSerializerOptions _options;

        public PayController(ILogger<PayController> logger, JsonSerializerOptions options)
        {
            _logger = logger;
            _options = options;
        }

        [HttpPut]
        [MiddlewareFilter(typeof(TokenAuthMiddlewarePipeline))]
        [Route("pay")]
        //当会员通过在线支付存款成功后，通过该接口给会员加额度
        public ActionResult Put([FromBody] RequestInfo requestInfo)
        {
            try
            {
                _logger.LogDebug($"請求參數: {JsonSerializer.Serialize(requestInfo, _options)}");

                //var errorInfo = new ErrorInfo();
                //if (!Int32.TryParse(requestInfo.Amount, out int amount) ||
                //    !Int32.TryParse(requestInfo.Billno, out int billno) ||
                //    !Int32.TryParse(requestInfo.ProductId, out int productId) ||
                //    !Int32.TryParse(requestInfo.MerchantNo, out int merchantNo) ||
                //    !Enum.TryParse(requestInfo.OrderType, out OrderType orderType))
                //{
                //    errorInfo.Code = "BAD_REQUEST";
                //    errorInfo.Message = "BAD_REQUEST";
                //    _logger.LogDebug($"{nameof(StatusCodes.Status401Unauthorized)}, 返回參數: {JsonSerializer.Serialize(errorInfo, _options)}");
                //    return BadRequest(JsonSerializer.Serialize(errorInfo, _options));
                //}


                //if (requestInfo.Currency != "VND" ||
                //    orderType != OrderType.FastPay
                //    )
                //{
                //    errorInfo.Code = "BAD_REQUEST";
                //    errorInfo.Message = "BAD_REQUEST";
                //    _logger.LogDebug($"{nameof(StatusCodes.Status401Unauthorized)}, 返回參數: {JsonSerializer.Serialize(errorInfo, _options)}");
                //    return BadRequest(JsonSerializer.Serialize(errorInfo, _options));
                //}

                var successInfo = new SuccessInfo
                {
                    Status = "true",
                    Message = ""
                };

                _logger.LogDebug($"{nameof(StatusCodes.Status200OK)}, 返回參數: {JsonSerializer.Serialize(successInfo, _options)}");
                return Ok(JsonSerializer.Serialize(successInfo, _options));
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
            /// 总额
            /// </summary>
            [JsonPropertyName("amount")]
            public string Amount { get; set; }

            /// <summary>
            /// billno
            /// </summary>
            [JsonPropertyName("billno")]
            public string Billno { get; set; }

            /// <summary>
            /// 创建人
            /// </summary>
            [JsonPropertyName("created_by")]
            public string CreatedBy { get; set; }

            /// <summary>
            /// 登录名
            /// </summary>
            [JsonPropertyName("login_name")]
            public string LoginName { get; set; }

            /// <summary>
            /// 产品ID
            /// </summary>
            [JsonPropertyName("product_id")]
            public string ProductId { get; set; }

            /// <summary>
            /// IP
            /// </summary>
            [JsonPropertyName("ip_address")]
            public string IpAddress { get; set; }

            /// <summary>
            /// 备注信息
            /// </summary>
            [JsonPropertyName("remarks")]
            public string? Remarks { get; set; }

            /// <summary>
            /// CNY
            /// </summary>
            [JsonPropertyName("currency")]
            public string Currency { get; set; }

            /// <summary>
            ///  渠道
            /// </summary>
            [JsonPropertyName("channel")]
            public string Channel { get; set; }

            /// <summary>
            ///  商户号
            /// </summary>
            [JsonPropertyName("merchant_no")]
            public string MerchantNo { get; set; }

            /// <summary>
            /// 渠道支类型 
            /// </summary>
            [JsonPropertyName("sub_trans_code")]
            public string SubTransCode { get; set; }

            /// <summary>
            /// 订单完成时间
            /// </summary>
            [JsonPropertyName("finishTime")]
            public string FinishTime { get; set; }

            /// <summary>
            /// 手续费
            /// </summary>
            [JsonPropertyName("handleAmount")]
            public string? HandleAmount { get; set; }

            /// <summary>
            /// 订单类型
            /// </summary>
            [JsonPropertyName("orderType")]
            public string OrderType { get; set; }

            /// <summary>
            /// 虚拟币汇率（目前只有比特币）
            /// </summary>
            [JsonPropertyName("exchangeRate")]
            public string? ExchangeRate { get; set; }

            /// <summary>
            /// 虚拟币汇率（目前只有比特币）
            /// </summary>
            [JsonPropertyName("vcAmount")]
            public string? VcAmount { get; set; }

            /// <summary>
            /// BQ补单订单的旧订单号
            /// </summary>
            [JsonPropertyName("oldbillno")]
            public string? Oldbillno { get; set; }

            /// <summary>
            /// 虚拟币订单的公司收款钱包地址
            /// </summary>
            [JsonPropertyName("compBtcAddr")]
            public string? CompBtcAddr { get; set; }
        }

        public class SuccessInfo
        {
            [JsonPropertyName("status")]
            public string Status { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }
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