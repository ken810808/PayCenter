using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PayCenter.Controllers
{
    [ApiController]
    public class ValidateOrdersController : Controller
    {
        private readonly ILogger<ValidateOrdersController> _logger;
        private readonly JsonSerializerOptions _options;
        public ValidateOrdersController(ILogger<ValidateOrdersController> logger, JsonSerializerOptions options)
        {
            _logger = logger;
            _options = options;
        }

        readonly string requestUrl = "https://uatpay.pay978.com/validateOrders.do";

        /// <summary>
        /// 根据订单号查询订单信息接口
        /// </summary>
        [HttpPost]
        [Route("ValidateOrders")]
        public ActionResult ValidateOrders([FromBody] RequestInfo requestInfo)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(requestInfo.billnos), nameof(requestInfo.billnos));
                        content.Add(new StringContent(requestInfo.key), nameof(requestInfo.key)); // 12345 +  md5(gw_netpay_123qwe+ billnos)
                        _logger.LogDebug($"{"請求參數"}: {JsonSerializer.Serialize(requestInfo, _options)}");

                        var response = client.PostAsync(requestUrl, content).Result;
                        var result = response.Content.ReadAsStringAsync().Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogDebug($"{nameof(StatusCodes.Status400BadRequest)}, {result}");
                            return BadRequest(result);
                        }

                        _logger.LogDebug($"{nameof(StatusCodes.Status200OK)}, {result}");
                        return Ok(result);
                    }
                }
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
            /// 订单号，多个用;隔开
            /// </summary>
            public string billnos { get; set; }

            /// <summary>
            /// 12345 +  md5(gw_netpay_123qwe+ billnos)
            /// </summary>
            public string key { get; set; }
        }

        public class ResponseInfo
        {
            /// <summary>
            /// 订单号
            /// </summary>
            [JsonPropertyName("billno")]
            public string Billno { get; set; }

            /// <summary>
            /// 公司虚拟币钱包收款地址
            /// </summary>
            [JsonPropertyName("walletAddress")]
            public string WalletAddress { get; set; }

            /// <summary>
            /// 块hash，目前只有小金库渠道返回
            /// </summary>
            [JsonPropertyName("blockHash")]
            public string BlockHash { get; set; }
            /// <summary>
            /// 客人转币的钱包地址，目前只有小金库渠道返回
            /// </summary>
            [JsonPropertyName("depositFromAddress")]
            public string DepositFromAddress { get; set; }

            /// <summary>
            /// 产品
            /// </summary>
            [JsonPropertyName("product")]
            public string Product { get; set; }

            /// <summary>
            /// 登录名
            /// </summary>
            [JsonPropertyName("loginname")]
            public string Loginname { get; set; }

            /// <summary>
            /// 金额
            /// </summary>
            [JsonPropertyName("amount")]
            public string Amount { get; set; }

            /// <summary>
            /// 订单币种
            /// </summary>
            [JsonPropertyName("currency")]
            public string Currency { get; set; }

            /// <summary>
            /// 渠道名称
            /// </summary>
            [JsonPropertyName("payment")]
            public string Payment { get; set; }

            /// <summary>
            /// 商户号
            /// </summary>
            [JsonPropertyName("merno")]
            public string Merno { get; set; }

            /// <summary>
            /// 状态[0 成功 1处理中 2 失败 3 取消]
            /// </summary>
            [JsonPropertyName("flag")]
            public string Flag { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [JsonPropertyName("jointime")]
            public string Jointime { get; set; }

            /// <summary>
            /// IP地址
            /// </summary>
            [JsonPropertyName("ip")]
            public string Ip { get; set; }

            /// <summary>
            /// 资金系统订单金额
            /// </summary>
            [JsonPropertyName("orderAmount")]
            public string OrderAmount { get; set; }

            /// <summary>
            /// 上分金额
            /// </summary>
            [JsonPropertyName("refAmount")]
            public string RefAmount { get; set; }

            /// <summary>
            /// 上分币种
            /// </summary>
            [JsonPropertyName("refCurrency")]
            public string RefCurrency { get; set; }

            /// <summary>
            /// 汇率
            /// </summary>
            [JsonPropertyName("refRate")]
            public string RefRate { get; set; }

            /// <summary>
            /// 上分异常标志  1异常 0未校验 2正常
            /// </summary>
            [JsonPropertyName("addScourceWrongFlag")]
            public string AddScourceWrongFlag { get; set; }

            /// <summary>
            /// 0 非买币， 1 买币
            /// </summary>
            [JsonPropertyName("isOtcBuy")]
            public string IsOtcBuy { get; set; }

            /// <summary>
            /// 确认状态（0未确认 1 成功 2确认中 3确认失败）
            /// </summary>
            [JsonPropertyName("confirmStatus")]
            public string ConfirmStatus { get; set; }

            /// <summary>
            /// 确认数
            /// </summary>
            [JsonPropertyName("confirmNumber")]
            public string ConfirmNumber { get; set; }

            /// <summary>
            /// 订单类型 1 需要确认的订单 0 普通订单
            /// </summary>
            [JsonPropertyName("orderType")]
            public string OrderType { get; set; }
        }
    }
}
