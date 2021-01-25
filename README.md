# sample-singalR-UserHub
ログインしているユーザに 通知メッセージを表示するサンプル

## SignalR 用の サーバー側の処理

```
Hubs\UserHub.cs 追加
	SignalR の サーバー側の処理
```

```
Startup.cs
            // SignalR 定義追加
            services.AddSignalR();

                // SignalR の エンドポイント追加
                endpoints.MapHub<UserHub>(UserHub.EndPoint_URL);
```


