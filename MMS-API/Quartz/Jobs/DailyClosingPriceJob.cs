using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MMS_API.Service.Implement;
using Quartz;

namespace DFPS_API.Quartz.Jobs
{
    public class DailyClosingPriceJob : IJob
    {
        private ILogger<DailyClosingPriceJob> _logger;
        // 注入DI provider
        private readonly IServiceProvider _provider;
        public DailyClosingPriceJob(ILogger<DailyClosingPriceJob> logger, IServiceProvider provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _provider = provider;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation(String.Format(@"******   DailyClosingPriceJob was fired!!!!! ******"));
            // 建立一個新的作用域
            using (var scope = _provider.CreateScope())
            {
                // 解析你的作用域服務
                var stockService = scope.ServiceProvider.GetService<IStockService>();
                //20211207
                string today = DateTime.Now.ToString("yyyyMMdd");
                await stockService.GetSeDaily(today);
                string today2 = DateTime.Now.AddYears(-1911).ToString("yyy/MM/dd");
                await stockService.GetSe2Daily(today2);
                
            }
            
        }
    }
}