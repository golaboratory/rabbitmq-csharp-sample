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
    public class Client : IDisposable
    {
        /// <summary>
        /// ホスト名を取得します。
        /// </summary>
        public string Hostname { get; private set; }

        /// <summary>
        /// ユーザー名を取得します。
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// パスワードを取得します。
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// 仮想ホストを取得します。
        /// </summary>
        public string VirtualHost { get; private set; }

        /// <summary>
        /// ポート番号を取得します。
        /// </summary>
        public int Port { get; private set; }

        private IConnection? _connection = null;
        private IChannel? _channel = null;

        /// <summary>
        /// 指定されたホスト名で新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="hostname">ホスト名</param>
        public Client(string hostname)
        {
            Hostname = hostname;
            Port = AmqpTcpEndpoint.UseDefaultPort;
            UserName = ConnectionFactory.DefaultUser;
            Password = ConnectionFactory.DefaultPass;
            VirtualHost = ConnectionFactory.DefaultVHost;
        }

        /// <summary>
        /// 指定されたパラメータで新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="hostname">ホスト名</param>
        /// <param name="userName">ユーザー名</param>
        /// <param name="password">パスワード</param>
        /// <param name="virtualHost">仮想ホスト</param>
        /// <param name="port">ポート番号</param>
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

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            this._channel?.Dispose();
            this._connection?.Dispose();
        }
    }
}
