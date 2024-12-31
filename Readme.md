# Rahmen.Core.MessageQueue

## 実行手順

1. MQSample/MQSender をビルドする
1. MQSample/MQWorker をビルドする
1. docker compose にてMQサーバの立ち上げ
1. MQWorkerの起動（ダブルクリック）
1. MQSenderの起動（ダブルクリック）
1. MQSenderに送信文字列を入力後Enter押下


## サーバの起動

Readme.mdと同階層にて、下記のコマンドを実行

```
docker compose up rabbitmq
```
