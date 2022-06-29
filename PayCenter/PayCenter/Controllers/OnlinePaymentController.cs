using Microsoft.AspNetCore.Mvc;
using PayCenter.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PayCenter.Controllers
{
    [ApiController]
    public class OnlinePaymentController : Controller
    {
        private readonly ILogger<OnlinePaymentController> _logger;
        private readonly JsonSerializerOptions _options;

        public OnlinePaymentController(ILogger<OnlinePaymentController> logger, JsonSerializerOptions options)
        {
            _logger = logger;
            _options = options;
        }

        readonly string firstRequestUrl = "https://uatpay.pay978.com/OnlinePaymentFirst.do";
        readonly string secondRequestUrl = "https://uatpay.pay978.com/OnlinePaymentSecond.do";
        //readonly string thirdRequestUrl = "https://uatpay.pay978.com/OnlinePaymentDispatch.do"; // 文檔提供
        readonly string thirdRequestUrl = "https://uatpay.pay978.com/OnlinePaymentThird.do";  // 第二步返回，結果一致
        readonly string defaultString = "V26123456";

        /// <summary>
        /// 在线支付获取支付ID及银行列表接口
        /// </summary>
        [HttpPost]
        [Route("OnlinePaymentFirst")]
        public ActionResult OnlinePaymentFirst([FromBody] OnlinePaymentFirstModel firstModel)
        {
            try
            {
                _logger.LogDebug($"請求參數: {JsonSerializer.Serialize(firstModel, _options)}");

                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(firstModel.newaccount), nameof(firstModel.newaccount)); // 0
                        content.Add(new StringContent(firstModel.loginname), nameof(firstModel.loginname)); // 123456789
                        content.Add(new StringContent(firstModel.product), nameof(firstModel.product)); // V26
                        content.Add(new StringContent(firstModel.grade), nameof(firstModel.grade)); // 1
                        content.Add(new StringContent(firstModel.currency), nameof(firstModel.currency)); // VND
                        content.Add(new StringContent(firstModel.amount), nameof(firstModel.amount)); // 10000
                        content.Add(new StringContent(firstModel.cuslevel), nameof(firstModel.cuslevel)); // 0
                        content.Add(new StringContent(firstModel.keycode), nameof(firstModel.keycode));  // 数据签名=md5(loginname + product + newaccount + grade +预定字符串, 8C16771746843D4764C63A0F9D0DAB42
                        content.Add(new StringContent(firstModel.type), nameof(firstModel.type)); // 18

                        var response = client.PostAsync(firstRequestUrl, content).Result;
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

        /// <summary>
        /// 在线支付获取实际支付接口
        /// </summary>
        [HttpPost]
        [Route("OnlinePaymentSecond")]
        public ActionResult OnlinePaymentSecond([FromBody] OnlinePaymentSecondModel secondModel)
        {
            try
            {
                _logger.LogDebug($"請求參數: {JsonSerializer.Serialize(secondModel, _options)}");

                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(secondModel.newaccount), nameof(secondModel.newaccount)); // 0
                        content.Add(new StringContent(secondModel.loginname), nameof(secondModel.loginname)); // 123456789
                        content.Add(new StringContent(secondModel.grade), nameof(secondModel.grade)); // 1
                        content.Add(new StringContent(secondModel.product), nameof(secondModel.product)); // V26
                        content.Add(new StringContent(secondModel.currency), nameof(secondModel.currency)); // VND
                        content.Add(new StringContent(secondModel.amount), nameof(secondModel.amount)); // 10000
                        content.Add(new StringContent(secondModel.backurl), nameof(secondModel.backurl));
                        content.Add(new StringContent(secondModel.payid), nameof(secondModel.payid)); // 101662
                        content.Add(new StringContent(secondModel.name), nameof(secondModel.name));
                        content.Add(new StringContent(secondModel.pwd), nameof(secondModel.pwd));
                        content.Add(new StringContent(secondModel.email), nameof(secondModel.email));
                        content.Add(new StringContent(secondModel.bankCode), nameof(secondModel.bankCode));
                        content.Add(new StringContent(secondModel.ip), nameof(secondModel.ip));
                        content.Add(new StringContent(secondModel.keycode), nameof(secondModel.keycode));  // 数据签名=md5(loginname + newaccount + product + amount + grade +预定字符串, 9db50407d8b8c7ace90468abe6b7eb62
                        content.Add(new StringContent(secondModel.cuslevel), nameof(secondModel.cuslevel)); // 0

                        var response = client.PostAsync(secondRequestUrl, content).Result;
                        var result = response.Content.ReadAsStringAsync().Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogDebug($"{nameof(StatusCodes.Status400BadRequest)}, 返回參數: {result}");
                            return BadRequest(result);
                        }

                        var secondResponse = JsonSerializer.Deserialize<SecondResponse>(result, _options);
                        _logger.LogDebug($"第三段請求keycode: {Md5Helper.ToMd5(secondModel.loginname + secondModel.amount + secondModel.product + secondModel.currency + secondResponse.billno + defaultString)}");
                        Console.WriteLine($"第三段請求keycode: {Md5Helper.ToMd5(secondModel.loginname + secondModel.amount + secondModel.product + secondModel.currency + secondResponse.billno + defaultString)}");

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

        /// <summary>
        /// 在线支付提交接口
        /// </summary>
        [HttpPost]
        [Route("OnlinePaymentThird")]
        public ActionResult OnlinePaymentThird([FromBody] OnlinePaymentThirdModel thirdModel)
        {
            try
            {
                _logger.LogDebug($"請求參數: {JsonSerializer.Serialize(thirdModel, _options)}");

                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(thirdModel.product), nameof(thirdModel.product)); // V26
                        content.Add(new StringContent(thirdModel.loginname), nameof(thirdModel.loginname)); // 123456789
                        content.Add(new StringContent(thirdModel.amount), nameof(thirdModel.amount)); // 10000
                        content.Add(new StringContent(thirdModel.currency), nameof(thirdModel.currency)); // VND
                        content.Add(new StringContent(thirdModel.billno), nameof(thirdModel.billno));
                        content.Add(new StringContent(thirdModel.keycode), nameof(thirdModel.keycode)); // 数据签名=md5(loginname + amount + product+ currency + billno +预定字符串)
                        content.Add(new StringContent(thirdModel.customerType), nameof(thirdModel.customerType)); //1

                        var response = client.PostAsync(thirdRequestUrl, content).Result;
                        var result = response.Content.ReadAsStringAsync().Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogDebug($"{nameof(StatusCodes.Status400BadRequest)}, 返回參數: {result}");
                            return BadRequest(result);
                        }

                        _logger.LogDebug($"ValidateOrders key: {12345 + Md5Helper.ToMd5("gw_netpay_123qwe" + thirdModel.billno)}");
                        Console.WriteLine($"ValidateOrders key: {12345 + Md5Helper.ToMd5("gw_netpay_123qwe" + thirdModel.billno)}");

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

        public class OnlinePaymentFirstModel
        {
            /// <summary>
            /// 0 充值
            /// </summary>
            public string newaccount { get; set; }

            /// <summary>
            /// 登陆名
            /// </summary>
            public string loginname { get; set; }

            /// <summary>
            /// 产品ID
            /// </summary>
            public string product { get; set; }

            /// <summary>
            /// 信用等级
            /// </summary>
            public string grade { get; set; }

            /// <summary>
            /// 币种
            /// </summary>
            public string currency { get; set; }

            /// <summary>
            /// 支付渠道名称 
            /// </summary>
            public string? payment { get; set; }

            /// <summary>
            /// 默认COMPUTER  COMPUTER(PC终端) MOBILE(手机终端)
            /// </summary>
            public string? terminal { get; set; } = "COMPUTER";

            /// <summary>
            /// 默认为1 18 网银PC端快捷支付
            /// </summary>
            public string? type { get; set; } = "1";

            /// <summary>
            /// 数据签名=md5(loginname + product + newaccount + grade +预定字符串)
            /// </summary>
            public string keycode { get; set; } = string.Empty;

            /// <summary>
            /// C07接入baokim、all2pay网银渠道使用必传参数：
            /// </summary>
            public string? bankType { get; set; }

            /// <summary>
            /// 接入BQ扫码方式时,此参数必传; 优先使用众鑫支付宝WAP请传入
            /// </summary>
            public string? amount { get; set; }

            /// <summary>
            /// 结算部测试专用参数，根据商户号来查询开启状态的商户
            /// </summary>
            public string? merno { get; set; }

            /// <summary>
            /// 会员星级必传
            /// </summary>
            public string cuslevel { get; set; }

            /// <summary>
            /// 指定域名(或约定密钥)，指定将启用商户配置的域名内容对指定域名过滤。
            /// </summary>
            public string? frontDomain { get; set; }

            /// <summary>
            /// 请求ID，前后端定位问题标记
            /// </summary>
            public string? requestid { get; set; }
        }

        public class OnlinePaymentSecondModel
        {
            /// <summary>
            /// 账号类型
            /// </summary>
            public string newaccount { get; set; }

            /// <summary>
            /// 产品ID
            /// </summary>
            public string product { get; set; }

            /// <summary>
            /// 金额 V币支付时，必须保证人民币换算后的V币金额是整数，否则订单会出错
            /// </summary>
            public string amount { get; set; }

            /// <summary>
            /// 登陆名
            /// </summary>
            public string loginname { get; set; }

            /// <summary>
            /// 回调域名
            /// </summary>
            public string backurl { get; set; }

            /// <summary>
            /// 备忘 （A98产品，在该字段将每笔订单的回调地址传过来）
            /// </summary>
            public string? memo { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            public string? remark { get; set; }

            /// <summary>
            /// 电话，C07 越南站nganluong\all2pay网银支付必填
            /// </summary>
            public string? phone { get; set; }

            /// <summary>
            /// 信用等级
            /// </summary>
            public string grade { get; set; }

            /// <summary>
            /// 姓名，C07 越南站nganluong\all2pay网银支付必填
            /// </summary>
            public string name { get; set; }

            /// <summary>
            /// 币种
            /// </summary>
            public string currency { get; set; }

            /// <summary>
            /// 支付id
            /// </summary>
            public string payid { get; set; }

            /// <summary>
            /// 密码
            /// </summary>
            public string pwd { get; set; }

            /// <summary>
            /// 电子邮件，C07 越南站nganluong\all2pay网银支付必填
            /// </summary>
            public string email { get; set; }

            /// <summary>
            /// 银行编号
            /// </summary>
            public string bankCode { get; set; }

            /// <summary>
            /// 玩家真实外网IP
            /// </summary>
            public string ip { get; set; }

            /// <summary>
            /// PC端充值则传COMPUTER，移动端充值则传MOBILE
            /// </summary>
            public string? terminal { get; set; }

            /// <summary>
            /// 支付调用域名，如果终端为手机则必填，V币支付和BTC支付时此地址必填, C07的所有支付渠道都必填该参数（意指支付成功后跳转回的页面）
            /// </summary>
            public string? requestdomain { get; set; }

            /// <summary>
            /// 站点ID(凯发体育站)
            /// </summary>
            public string? siteId { get; set; }

            /// <summary>
            /// 0=Office,1=PC,2=mobile_html5，3=手机android app ,4=手机IOS app,5=agin app苹果设备，6=agqj app手机客户端，7=ag-keno手机客户端，8=网站登陆器，9=优惠系统
            /// </summary>
            public string? endPoint { get; set; }

            /// <summary>
            /// 数据签名=md5(loginname + newaccount + product + amount + grade +预定字符串)
            /// </summary>
            public string keycode { get; set; }

            /// <summary>
            /// heytpaymobile专用必填参数，表示快捷支付银行卡号，
            /// </summary>
            public string? cardNo { get; set; }

            /// <summary>
            /// heytpaymobile专用必填参数，表示快捷支付银行卡账户名
            /// </summary>
            public string? cardName { get; set; }

            /// <summary>
            /// heytpaymobile专用必填参数，表示身份证号
            /// </summary>
            public string? idCardNo { get; set; }

            /// <summary>
            /// heytpaymobile专用必填参数，表示银行预留手机号
            /// </summary>
            public string? mobile { get; set; }

            /// <summary>
            /// 会员星级必传
            /// </summary>
            public string cuslevel { get; set; }

            /// <summary>
            /// ngamluong网银支付必传,  NL:电子钱包; ATM_ONLINE: 网上ATM转帐; QRCODE:扫码方式
            /// </summary>
            public string? paymentmethod { get; set; }

            /// <summary>
            /// 使用BtcRateCheck.do接口需要把交易比特币兑人民币汇率传入该字段 1TC=N RMB
            /// </summary>
            public string? tradeRate { get; set; }

            /// <summary>
            /// 使用BtcRateCheck.do接口需要把交易的比特币数量传入该字段
            /// </summary>
            public string? tradeAmount { get; set; }

            /// <summary>
            /// 提交存款请求时访问跳转的域名地址,前端传入
            /// </summary>
            public string? redirectdomain { get; set; }

            /// <summary>
            /// 订单手续费费率(外部产品专用)
            /// </summary>
            public string? handleAmount { get; set; }

            /// <summary>
            /// 请求ID，前后端定位标记
            /// </summary>
            public string? requestid { get; set; }
        }

        public class OnlinePaymentThirdModel
        {
            /// <summary>
            /// 产品ID
            /// </summary>
            public string product { get; set; }

            /// <summary>
            /// 订单号
            /// </summary>
            public string billno { get; set; }

            /// <summary>
            /// 金额
            /// </summary>
            public string amount { get; set; }

            /// <summary>
            /// 登陆名
            /// </summary>
            public string loginname { get; set; }

            /// <summary>
            /// 币种
            /// </summary>
            public string currency { get; set; }

            /// <summary>
            /// 语言默认为简体中文  zn 简体中文 tw 繁体中文 vn 越南语 en 英语
            /// </summary>
            public string? language { get; set; }

            /// <summary>
            /// 数据签名=md5(loginname + newaccount + billNo + 预定字符串)
            /// </summary>
            public string keycode { get; set; }

            /// <summary>
            /// C07接入baokim网银渠道使用必传参数：
            /// 付款人姓名
            /// </summary>
            public string? payername { get; set; }

            /// <summary>
            /// C07接入baokim网银渠道使用必传参数：
            /// 付款人邮箱
            /// </summary>
            public string? payeremail { get; set; }

            /// <summary>
            /// C07接入baokim网银渠道使用必传参数：
            /// 付款人电话号码
            /// </summary>
            public string? payerphoneno { get; set; }

            /// <summary>
            /// 值固定为1
            /// </summary>
            public string customerType { get; set; } = "1";

            /// <summary>
            /// 值固定为1
            /// </summary>
            public string? dympwd { get; set; }

            /// <summary>
            /// 值固定为1
            /// </summary>
            public string? requestid { get; set; }
        }

        // TODO: ENUM
        public class FirstResponse
        {
            /// <summary>
            /// 0为成功 1为支付方式关闭 2 参数验证失败
            /// </summary>
            [JsonPropertyName("status")]
            public int Status { get; set; }

            /// /// <summary>
            /// 支付id
            /// </summary>
            [JsonPropertyName("payid")]
            public int Payid { get; set; }

            /// <summary>
            /// 错误信息
            /// </summary>
            [JsonPropertyName("message")]
            public string Message { get; set; }

            /// <summary>
            /// 该渠道最小金额
            /// </summary>
            [JsonPropertyName("minamount")]
            public long Minamount { get; set; }

            /// <summary>
            /// 该渠道最大金额
            /// </summary>
            [JsonPropertyName("maxamount")]
            public long Maxamount { get; set; }

            /// <summary>
            /// 渠道编码,如果该渠道编码为heytpaymobile,请走和易通手机网银快捷支付流程
            /// </summary>
            [JsonPropertyName("paymentcode")]
            public string Paymentcode { get; set; }

            /// <summary>
            /// 只有当返回的paymentcode为“xinfuqrcode”，并且type=9即支付宝wap时，才会返回该字段，取值说明：true（此时前端应提供固定充值金额给客户选择），false（任意充值金额）
            /// </summary>
            [JsonPropertyName("isBigAmount")]
            public bool IsBigAmount { get; set; }

            /// <summary>
            /// 100030 要提交code, pab 在线支付银行编码 
            /// 如果bankList 为空则选择银行是在第三方选择，有值则显示， 如果为空时，在页面做相应的提示玩家信息。谢谢！
            /// </summary>
            [JsonPropertyName("bankList")]
            public List<object> BankList { get; set; }

            /// <summary>
            /// V币兑人民币汇率，比如1 V币=0.45人民币，那vcoinrate就是0.45.
            /// </summary>
            [JsonPropertyName("vcoinrate")]
            public string Vcoinrate { get; set; }

            /// <summary>
            /// 返回玩家最近一个月内成功存款的银行类型列表，依逗号隔开
            /// </summary>
            [JsonPropertyName("bankcode")]

            public string Bankcode { get; set; }

            /// <summary>
            /// 如果此参数有值表示玩家只能从金额列表中选择金额，不能输入，如果为空表示可以输入任意金额。例如： 100,200,500,1000
            /// </summary>
            [JsonPropertyName("amountList")]
            public string AmountList { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonPropertyName("customerFeeRate")]
            public int CustomerFeeRate { get; set; }
        }

        // TODO: ENUM
        public class SecondResponse
        {
            /// <summary>
            /// 0为成功 1为支付方式关闭 2 参数验证失败 9 其他异常
            /// </summary>
            public int status { get; set; }

            /// <summary>
            /// 错误信息
            /// </summary>
            public string message { get; set; }

            /// <summary>
            /// 订单号
            /// </summary>
            public string billno { get; set; }

            /// <summary>
            /// 支付渠道编码 如果是 reapalmobile参见【融宝在线支付提交】,其他参见【在线支付提交接口】
            /// </summary>
            public string paycode { get; set; }

            /// <summary>
            /// 订单金额
            /// </summary>
            public string amount { get; set; }

            /// <summary>
            /// 第三步调用地址
            /// </summary>
            public string url { get; set; }

            /// <summary>
            /// 订单日期
            /// </summary>
            public string billdate { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            public string memo { get; set; }

            /// <summary>
            /// 备用调用域名数组列表，如果为空请使用url
            /// </summary>
            public List<string> urlList { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonPropertyName("customerFeeRate")]
            public int CustomerFeeRate { get; set; }
        }

        // TODO: ENUM
        public class ThirdResponde
        {
            /// <summary>
            /// 0为成功 1为支付方式关闭 2 参数验证失败 9 其他异常
            /// </summary>
            [JsonPropertyName("status")]
            public int Status { get; set; }

            /// <summary>
            /// 订单号
            /// </summary>
            [JsonPropertyName("billno")]
            public string Billno { get; set; }

            /// <summary>
            /// 订单状态有
            /// </summary>
            [JsonPropertyName("billstatus")]
            public string Billstatus { get; set; }

            /// <summary>
            /// 订单提交的卡号，依逗号分隔
            /// </summary>
            [JsonPropertyName("cardNos")]
            public string CardNos { get; set; }

            /// <summary>
            /// 卡支付状态
            /// </summary>
            [JsonPropertyName("cardStatus")]
            public string CardStatus { get; set; }

            /// <summary>
            /// 错误信息
            /// </summary>
            [JsonPropertyName("message")]
            public string Message { get; set; }
        }
    }
}
