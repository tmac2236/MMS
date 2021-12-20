using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MMS_API.Service.Implement;
using Quartz;

namespace DFPS_API.Quartz.Jobs
{
    public class ServicePoolJob : IJob
    {
        private ILogger<ServicePoolJob> _logger;
        // 注入DI provider
        private readonly IServiceProvider _provider;
        public ServicePoolJob(ILogger<ServicePoolJob> logger, IServiceProvider provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _provider = provider;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation(String.Format(@"******   ServicePoolJob was fired!!!!! ******"));
            // 建立一個新的作用域
            using (var scope = _provider.CreateScope())
            {
                // 解析你的作用域服務
                var stockService = scope.ServiceProvider.GetService<IStockService>();
                await stockService.DoUndoTaskByServicePool();
                
            }
            
        }
    }
}