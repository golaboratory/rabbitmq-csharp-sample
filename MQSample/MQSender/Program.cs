var producer = new Rahmen.Core.MessageQueue.Producer(
    hostname: "localhost",
    userName: "user",
    password: "password",
    virtualHost: "/",
    port: 5672
    );

var channel = "audit_log_test";

Console.WriteLine("MQサーバに送信する内容を入力してください");

while (true)
{

    var s = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(s))
    {
        Console.WriteLine("プログラムを終了します。");
        Console.ReadLine();
        return;
    }

    var msg = new MQEntity.Protocol()
    {
        Message = s,
        Timestamp = DateTime.Now,
        MachineName = Environment.MachineName,
    };


    var json = System.Text.Json.JsonSerializer.Serialize(msg);


    producer.Publish(json, channel);
    Console.WriteLine($" [x] Sent {msg}");

    Console.WriteLine("MQサーバに送信する内容を入力してください");
    Console.WriteLine(" Press [enter] to exit.");

}
