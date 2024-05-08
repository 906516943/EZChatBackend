
using StackExchange.Redis;

namespace ChatHistory.Subscribers
{
    public class ChatMessageSubscriberService : IHostedService
    {
        private IConnectionMultiplexer _connection;

        public ChatMessageSubscriberService(IConnectionMultiplexer connection) 
        {
            _connection = connection;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var pubsub = _connection.GetSubscriber();

            pubsub.Subscribe("chat-message", (channel, message) => { });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            
        }
    }
}
