using System.Text;

// クライアントのインスタンスを作成します
using var client = new Rahmen.Core.MessageQueue.Client(
    hostname: "localhost",
    userName: "user",
    password: "password",
    virtualHost: "/",
    port: 5672
);

// キューの名前を設定します
var channel = "audit_log_test";

// メッセージの受信を開始します
client.RecieveStart(channel, (model, ea) =>
{
    // メッセージの内容を取得します
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
});

Console.WriteLine(" Press [enter] to exit.");

// 終了しないように、ReadLineで待機します
Console.ReadLine();