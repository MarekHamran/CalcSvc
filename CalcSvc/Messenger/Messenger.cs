
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;

namespace CalcSvc.Messenger
{
	public class Messenger : IMessenger
	{
		private static readonly string queueName = "calcSvc";
		private static readonly string exchangeName = "calcExchange";
		private static readonly string routeKey = "calc";


		private Consumer? _consumer;

		private IChannel? _channel;

		private async Task<IChannel> CreateChannel()
		{
			ConnectionFactory factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
			IConnection connection = await factory.CreateConnectionAsync();
			return await connection.CreateChannelAsync();
		}

		private async Task CreateConsumer()
		{
			IChannel channel = await CreateChannel();
			_consumer = new Consumer(channel);
			await channel.BasicConsumeAsync(queueName, true, _consumer);
		}


		public async Task<string?> GetMessageAsync(CancellationToken cancellationToken)
		{
			if (_consumer == null || _consumer.Channel.IsClosed) await CreateConsumer();

			return await _consumer?.GetValueAsync(cancellationToken);
		}

		public async Task SendMessageAsync(string message)
		{
			if (_channel == null || _channel.IsClosed)
			{
				_channel = await CreateChannel();
				await _channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);
				await _channel.QueueDeclareAsync(queueName, durable: false, exclusive: false);
				await _channel.QueueBindAsync(queueName, exchangeName, routeKey);
			}

			byte[] data = Encoding.UTF8.GetBytes(message);

			await _channel.BasicPublishAsync(exchangeName, routeKey, mandatory: true, data);
		}

		private class Consumer : AsyncDefaultBasicConsumer
		{
			public Consumer(IChannel channel) : base(channel)
			{
			}


			public async Task<string?> GetValueAsync(CancellationToken cancellationToken)
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					if (Values.TryDequeue(out string? value))
						return value;
					else
						await Task.Delay(100);
				}
				return null;
			}

			private ConcurrentQueue<string> Values { get; set; } = new ConcurrentQueue<string>();

			public override Task HandleBasicDeliverAsync(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IReadOnlyBasicProperties properties, ReadOnlyMemory<byte> body, CancellationToken cancellationToken = default)
			{
				string value = Encoding.UTF8.GetString(body.ToArray());
				Values.Enqueue(value);
				return base.HandleBasicDeliverAsync(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body, cancellationToken);
			}
		}
	}
}
