using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/// <summary>
/// メッセージキューを管理する名前空間
/// </summary>
namespace Rahmen.Core.MessageQueue
{
    /// <summary>
    /// メッセージキュークライアントクラス
    /// </summary>
    public class Client:IDisposable
    {

        public string Hostname { get; private set; }

        public string UserName { get; private set; }

        public string Password { get; private set; }

        public string VirtualHost { get; private set; }

        public int Port { get; private set; }

        private IConnection _connection;
        
        private IChannel _channel;



        public Client(string hostname)
        {
            Hostname = hostname;
            Port = AmqpTcpEndpoint.UseDefaultPort;
            UserName = ConnectionFactory.DefaultUser;
            Password = ConnectionFactory.DefaultPass;
            VirtualHost = ConnectionFactory.DefaultVHost;
        }


        public Client(string hostname, string userName, string password, string virtualHost, int port)
        {
            Hostname = hostname;
            UserName = userName;
            Password = password;
            VirtualHost = virtualHost;
            Port = port;
        }


        /// <summary>
        /// メッセージキューの受信を開始する
        /// </summary>
        /// <param name="queueName">接続するキュー名</param>
        /// <param name="connectionUrl">接続先</param>
        /// <param name="func">コールバック</param>
        public async void RecieveStart(string queueName, Action<object, BasicDeliverEventArgs> func)
        {
            var factory = new ConnectionFactory()
            {
                HostName = this.Hostname,
                UserName = this.UserName,
                Password = this.Password,
                VirtualHost = this.VirtualHost,
                Port = this.Port,
            };
            this._connection = await factory.CreateConnectionAsync();
            this._channel = await this._connection.CreateChannelAsync();


            await this._channel.QueueDeclareAsync(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(this._channel);
            consumer.ReceivedAsync += (model, ea) =>
            {
                func(model, ea);
                return Task.CompletedTask;
            };

            await this._channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

        }

        public void Dispose()
        {
            this._channel?.Dispose();
            this._connection?.Dispose();
        }
    }
}
