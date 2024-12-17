
using RabbitMQ.Client;

namespace CalcSvc
{
	public class MQReader : BackgroundService
	{
		private ILogger _logger;
		private IMessenger _messenger;

		public MQReader(ILogger<MQReader> logger, IMessenger messenger)
		{
			_logger = logger;
			_messenger = messenger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{

			while (! stoppingToken.IsCancellationRequested)
			{
				try
				{
					string? value = await _messenger.GetMessageAsync(stoppingToken);
					if (value != null)
						Console.WriteLine(value);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					_logger.LogError(ex, "Error getting message");
					await Task.Delay(1000);
				}
			}

			return;
		}
	}
}
