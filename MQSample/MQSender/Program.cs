// プロデューサーのインスタンスを作成します
var producer = new Rahmen.Core.MessageQueue.Producer(
    hostname: "localhost",
    userName: "user",
    password: "password",
    virtualHost: "/",
    port: 5672
);

// キューの名前を設定します
var channel = "audit_log_test";

Console.WriteLine("MQサーバに送信する内容を入力してください");

while (true)
{
    // ユーザーからの入力を読み取ります
    var s = Console.ReadLine();

    // 入力が空の場合、プログラムを終了します
    if (string.IsNullOrWhiteSpace(s))
    {
        Console.WriteLine("プログラムを終了します。");
        Console.ReadLine();
        return;
    }

    // メッセージエンティティを作成します
    var msg = new MQEntity.Protocol()
    {
        Message = s,
        Timestamp = DateTime.Now,
        MachineName = Environment.MachineName,
    };

    // メッセージをJSON形式にシリアライズします
    var json = System.Text.Json.JsonSerializer.Serialize(msg);

    // メッセージをキューに送信します
    producer.Publish(json, channel);
    Console.WriteLine($" [x] Sent {msg}");

    Console.WriteLine("MQサーバに送信する内容を入力してください");
    Console.WriteLine(" Press [enter] to exit.");

}
