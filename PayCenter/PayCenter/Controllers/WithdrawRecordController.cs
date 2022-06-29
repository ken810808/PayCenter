using Microsoft.AspNetCore.Mvc;
using PayCenter.Enums;
using PayCenter.MiddleWares;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PayCenter.Controllers
{
    [ApiController]
    public class WithdrawRecordController : Controller
    {
        private readonly ILogger<WithdrawRecordController> _logger;
        private readonly JsonSerializerOptions _options;

        public WithdrawRecordController(ILogger<WithdrawRecordController> logger, JsonSerializerOptions options)
        {
            _logger = logger;
            _options = options;
        }

        [HttpPost]
        [MiddlewareFilter(typeof(TokenAuthMiddlewarePipeline))]
        [Route("withdraw_record")]
        // 读取已审核通过（flag 等于1）的取款提案数据，为会员付款
        public ActionResult Post([FromBody] RequestInfo requestInfo)
        {
            try
            {
                _logger.LogDebug($"請求參數: {JsonSerializer.Serialize(requestInfo, _options)}");

                //var errorInfo = new ErrorInfo();
                //if (!Enum.TryParse(requestInfo.Flag, out WithDrawFlag withDrawFlag) ||
                //   !Int32.TryParse(requestInfo.PageNum, out int pageNum) ||
                //   !Int32.TryParse(requestInfo.PageSize, out int pageSize))
                //{
                //    errorInfo.Code = "BAD_REQUEST";
                //    errorInfo.Message = "BAD_REQUEST";
                //    _logger.LogDebug($"{nameof(StatusCodes.Status401Unauthorized)}, 返回參數: {JsonSerializer.Serialize(errorInfo, _options)}");

                //    return BadRequest(JsonSerializer.Serialize(errorInfo, _options));
                //}

                //if (requestInfo.Pid != "pid_123" ||
                //    withDrawFlag != WithDrawFlag.Approved)
                //{
                //    errorInfo.Code = "BAD_REQUEST";
                //    errorInfo.Message = "BAD_REQUEST";
                //    _logger.LogDebug($"{nameof(StatusCodes.Status401Unauthorized)}, 返回參數: {JsonSerializer.Serialize(errorInfo, _options)}");
                //    return BadRequest(JsonSerializer.Serialize(errorInfo, _options));
                //}

                var recordList = new List<Record>()
                {
                    new Record()
                    {

                    }
                };

                _logger.LogDebug($"{nameof(StatusCodes.Status200OK)}, 返回參數: {JsonSerializer.Serialize(recordList, _options)}");
                return Ok(JsonSerializer.Serialize(recordList, _options));
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
            /// 状态（1：已审核）
            /// </summary>
            [JsonPropertyName("flag")]
            public string Flag { get; set; }

            /// <summary>
            /// 页码
            /// </summary>
            [JsonPropertyName("pageNum")]
            public string PageNum { get; set; }

            /// <summary>
            /// 分页大小
            /// </summary>
            [JsonPropertyName("pageSize")]
            public string PageSize { get; set; }
        }

        public class Record
        {
            /// <summary>
            /// 取款金额
            /// </summary>
            [JsonPropertyName("amount")]
            public string Amount { get; set; }

            /// <summary>
            /// 银行账户名称
            /// </summary>
            [JsonPropertyName("bank_account_name")]
            public string BankAccountName { get; set; }

            /// <summary>
            /// 银行卡号
            /// </summary>
            [JsonPropertyName("bank_account_no")]
            public string BankAccountNo { get; set; }

            /// <summary>
            /// 账户类型(借记卡、贷记卡等)
            /// </summary>
            [JsonPropertyName("bank_account_type")]
            public string BankAccountType { get; set; }

            /// <summary>
            /// 银行所在省份
            /// </summary>
            [JsonPropertyName("bank_province")]
            public string BankProvince { get; set; }

            /// <summary>
            /// 银行所在城市
            /// </summary>
            [JsonPropertyName("bank_city")]
            public string BankCity { get; set; }

            /// <summary>
            /// 银行名称
            /// </summary>
            [JsonPropertyName("bank_name")]
            public string Bank_Name { get; set; }

            /// <summary>
            /// 支行名称
            /// </summary>
            [JsonPropertyName("branch_name")]
            public string BranchName { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [JsonPropertyName("created_date")]
            public string CreatedDate { get; set; }

            /// <summary>
            /// 货币(CNY)
            /// </summary>
            [JsonPropertyName("currency")]
            public string Currency { get; set; }

            /// <summary>
            /// 客户类别(1会员2代理)
            /// </summary>
            [JsonPropertyName("customer_type")]
            public string CustomerType { get; set; }

            /// <summary>
            /// 登录名称
            /// </summary>
            [JsonPropertyName("login_name")]
            public string LoginName { get; set; }

            /// <summary>
            /// 产品ID
            /// </summary>
            [JsonPropertyName("product_id")]
            public string ProductId { get; set; }

            /// <summary>
            /// 状态（1：待付款，2：已付款，-3：拒绝）
            /// </summary>
            [JsonPropertyName("flag")]
            public string Flag { get; set; }

            /// <summary>
            /// 批准日期
            /// </summary>
            [JsonPropertyName("processed_date")]
            public string ProcessedDate { get; set; }

            /// <summary>
            /// 备注信息
            /// </summary>
            [JsonPropertyName("remarks")]
            public string Remarks { get; set; } = string.Empty;

            /// <summary>
            /// 请求ID	(以产品id开头 18位,字母大小,不包含特殊字符)
            /// </summary>
            [JsonPropertyName("request_id")]
            public string RequestId { get; set; }

            /// <summary>
            /// 客户等级
            /// </summary>
            [JsonPropertyName("customer_level")]
            public string CustomerLevel { get; set; }

            /// <summary>
            /// 信用等级
            /// </summary>
            [JsonPropertyName("deposit_level")]
            public string DepositLevel { get; set; }

            /// <summary>
            /// 优先级
            /// </summary>
            [JsonPropertyName("priority_level")]
            public string PriorityLevel { get; set; }
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



