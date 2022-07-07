using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PayCenter.Controllers
{
    [ApiController]
    public class DigitalCoinController : Controller
    {
        private readonly ILogger<OnlinePaymentController> _logger;
        private readonly JsonSerializerOptions _options;

        public DigitalCoinController(ILogger<OnlinePaymentController> logger, JsonSerializerOptions options)
        {
            _logger = logger;
            _options = options;
        }

        readonly string createOrderrequestUrl = "https://uatpay.pay978.com/digitalCoin/createOrder";
        readonly string scalerQuoterequestUrl = "https://uatpay.pay978.com/rate/scalerQuote.do";

        /// <summary>
        /// 用于查询支付方式实付开启 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("digitalCoin/createOrder")]
        public ActionResult CreateOrder([FromBody] OrderModel orderModel)
        {
            try
            {
                _logger.LogDebug($"請求參數: {JsonSerializer.Serialize(orderModel, _options)}");

                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(orderModel.Currency), nameof(orderModel.Currency)); // VND 当币种为空时 则不参与
                        content.Add(new StringContent(orderModel.TranAmount), nameof(orderModel.TranAmount));
                        content.Add(new StringContent(orderModel.ProductId), nameof(orderModel.ProductId)); // V26
                        content.Add(new StringContent(orderModel.TrustLevel.ToString()), nameof(orderModel.TrustLevel)); // 1
                        content.Add(new StringContent(orderModel.StarLevel.ToString()), nameof(orderModel.StarLevel)); // 0
                        content.Add(new StringContent(orderModel.LoginName), nameof(orderModel.LoginName)); // qutestuser
                        content.Add(new StringContent(orderModel.PayType.ToString()), nameof(orderModel.PayType)); // 25
                        content.Add(new StringContent(orderModel.Sign), nameof(orderModel.Sign));  // 签名规则：会员账号+产品id+金额+币种+支付类型+key 进行MD5 结果赋值给sign                        
                        content.Add(new StringContent(orderModel.Protocol), nameof(orderModel.Protocol));  // 协议（USDT 则必填）
                        content.Add(new StringContent(orderModel.TerminalType), nameof(orderModel.TerminalType));  // PC

                        var response = client.PostAsync(createOrderrequestUrl, content).Result;
                        var result = response.Content.ReadAsStringAsync().Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogDebug($"{nameof(StatusCodes.Status400BadRequest)}, 返回參數: {result}");
                            return BadRequest(result);
                        }

                        _logger.LogDebug($"{nameof(StatusCodes.Status200OK)}, 返回參數: {result}");
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

        [HttpPost]
        [Route("rate/scalerQuote.do")]
        public ActionResult ScalerQuote([FromBody] ScalerModel scalerModel)
        {
            try
            {
                _logger.LogDebug($"請求參數: {JsonSerializer.Serialize(scalerModel, _options)}");

                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(scalerModel.ProCode), nameof(scalerModel.ProCode)); // V26
                        content.Add(new StringContent(scalerModel.LoginName), nameof(scalerModel.LoginName)); // qbtestuser
                        content.Add(new StringContent(scalerModel.Grade), nameof(scalerModel.Grade)); // 1
                        content.Add(new StringContent(scalerModel.CusLevel), nameof(scalerModel.CusLevel)); // 0
                        content.Add(new StringContent(scalerModel.Scur), nameof(scalerModel.Scur)); // VND
                        content.Add(new StringContent(scalerModel.Tcur), nameof(scalerModel.Tcur)); // USDT
                        content.Add(new StringContent(scalerModel.Samount), nameof(scalerModel.Samount)); //10000
                        content.Add(new StringContent(scalerModel.Sign), nameof(scalerModel.Sign));  // 签名MD5(loginName+proCode+grade+samount)小写
                        content.Add(new StringContent(scalerModel.TradeType), nameof(scalerModel.TradeType));  // 01

                        var response = client.PostAsync(scalerQuoterequestUrl, content).Result;
                        var result = response.Content.ReadAsStringAsync().Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogDebug($"{nameof(StatusCodes.Status400BadRequest)}, 返回參數: {result}");
                            return BadRequest(result);
                        }

                        _logger.LogDebug($"{nameof(StatusCodes.Status200OK)}, 返回參數: {result}");
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

        public class OrderModel
        {

            /// <summary>
            /// 币种
            /// </summary>
            [JsonPropertyName("currency")]
            public string Currency { get; set; }

            /// <summary>
            /// 金额（金额为0则不生成订单）
            /// </summary>
            [JsonPropertyName("tranAmount")]
            public string TranAmount { get; set; }

            /// <summary>
            /// 产品
            /// </summary>
            [JsonPropertyName("productId")]
            public string ProductId { get; set; }

            /// <summary>
            /// 信用等级
            /// </summary>
            [JsonPropertyName("trustLevel")]
            public int TrustLevel { get; set; }

            /// <summary>
            /// 会员星级
            /// </summary>
            [JsonPropertyName("starLevel")]
            public int StarLevel { get; set; }

            /// <summary>
            /// 登录名
            /// </summary>
            [JsonPropertyName("loginName")]
            public string LoginName { get; set; }

            /// <summary>
            /// 支付类型
            /// </summary>
            [JsonPropertyName("payType")]
            public int PayType { get; set; }

            /// <summary>
            /// 签名串
            /// </summary>
            [JsonPropertyName("sign")]
            public string Sign { get; set; }

            /// <summary>
            /// 终端ID
            /// </summary>
            [JsonPropertyName("terminalId")]
            public string? TerminalId { get; set; }

            /// <summary>
            /// 协议（USDT 则必填）
            /// </summary>
            [JsonPropertyName("protocol")]
            public string? Protocol { get; set; }

            /// <summary>
            /// 终端类型 （PC,MOBILE）
            /// </summary>
            [JsonPropertyName("terminalType")]
            public string? TerminalType { get; set; }

            /// <summary>
            /// 子产品
            /// </summary>
            [JsonPropertyName("childProductId")]
            public string? ChildProductId { get; set; }
        }

        public class ScalerModel
        {
            /// <summary>
            /// 产品编码
            /// </summary>
            [JsonPropertyName("proCode")]
            public string ProCode { get; set; }

            /// <summary>
            /// 登录名
            /// </summary>
            [JsonPropertyName("loginName")]
            public string LoginName { get; set; }

            /// <summary>
            /// 信用等级
            /// </summary>
            [JsonPropertyName("grade")]
            public string Grade { get; set; }

            /// <summary>
            /// 会员级别
            /// </summary>
            [JsonPropertyName("cusLevel")]
            public string CusLevel { get; set; }

            /// <summary>
            /// 原币种
            /// </summary>
            [JsonPropertyName("scur")]
            public string Scur { get; set; }

            /// <summary>
            /// 目标币种
            /// </summary>
            [JsonPropertyName("tcur")]
            public string Tcur { get; set; }

            /// <summary>
            /// 交易数量
            /// </summary>
            [JsonPropertyName("samount")]
            public string Samount { get; set; }

            /// <summary>
            /// 签名MD5(loginName+proCode+grade+samount)小写
            /// </summary>
            [JsonPropertyName("sign")]
            public string Sign { get; set; }

            /// <summary>
            /// 交易类型（存款: 01；取款: 02）默认01
            /// </summary>
            [JsonPropertyName("tradeType")]
            public string TradeType { get; set; }

            /// <summary>
            /// USDT协议，OMNI或者ERC20
            /// </summary>
            [JsonPropertyName("protocol")]
            public string? Protocol { get; set; }
        }

        class ResponseModel
        {
            /// <summary>
            /// 订单号码
            /// </summary>
            [JsonPropertyName("billNo")]
            public string BillNo { get; set; }

            /// <summary>
            /// 地址
            /// </summary>
            [JsonPropertyName("address")]
            public string Address { get; set; }

            /// <summary>
            /// 协议
            /// </summary>
            [JsonPropertyName("protocol")]
            public string Protocol { get; set; }

            /// <summary>
            /// 交易金额
            /// </summary>
            [JsonPropertyName("tranAmount")]
            public decimal TranAmount { get; set; }

            /// <summary>
            /// 币种
            /// </summary>
            [JsonPropertyName("currency")]
            public string Currency { get; set; }

            /// <summary>
            /// 地址签名
            /// </summary>
            [JsonPropertyName("crcCode")]
            public string CrcCode { get; set; }

            /// <summary>
            /// 标签（XRP会用）
            /// </summary>
            [JsonPropertyName("tag")]
            public string Tag { get; set; }

            /// <summary>
            /// 小金库APP下载地址
            /// </summary>
            [JsonPropertyName("downloadUrl")]
            public string DownloadUrl { get; set; }
        }
    }
}
