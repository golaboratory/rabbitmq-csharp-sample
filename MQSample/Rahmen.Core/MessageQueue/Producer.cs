﻿using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace Rahmen.Core.MessageQueue
{
    /// <summary>
    /// メッセージキュープロデューサークラス
    /// </summary>
    public class Producer
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

        /// <summary>
        /// 指定されたホスト名で新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="hostname">ホスト名</param>
        public Producer(string hostname)
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
        public Producer(string hostname, string userName, string password, string virtualHost, int port)
        {
            Hostname = hostname;
            UserName = userName;
            Password = password;
            VirtualHost = virtualHost;
            Port = port;
        }

        /// <summary>
        /// キューを公開する
        /// </summary>
        /// <param name="message">送信データ</param>
        /// <param name="queueName">公開するキュー名</param>
        public async void Publish(string message, string queueName)
        {
            var factory = new ConnectionFactory()
            {
                HostName = this.Hostname,
                UserName = this.UserName,
                Password = this.Password,
                VirtualHost = this.VirtualHost,
                Port = this.Port,
            };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                body: body);
        }
    }
}
