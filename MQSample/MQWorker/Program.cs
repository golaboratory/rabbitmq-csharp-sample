using System.Text;


using var client = new Rahmen.Core.MessageQueue.Client(
    hostname: "localhost",
    userName: "user",
    password: "password",
    virtualHost: "/",
    port: 5672
    );

var channel = "audit_log_test";

client.RecieveStart(channel, (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
});


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();