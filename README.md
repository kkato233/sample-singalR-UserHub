# sample-singalR-UserHub
ログインしているユーザに 通知メッセージを表示するサンプル

ログインしているユーザの ユーザID をグループ名にすることで 
特定のユーザにメッセージを送信する。


### SignalR 用の サーバー側の処理

```
Hubs\UserHub.cs 
     SingalR の サーバー側の処理が実装されている。
```

```
Startup.cs ファイルに SignalR の動作定義追加
            // SignalR 定義追加
            services.AddSignalR();

                // SignalR の エンドポイント追加
                endpoints.MapHub<UserHub>(UserHub.EndPoint_URL);
```

実装部分詳細

https://github.com/kkato233/sample-singalR-UserHub/commit/59830b205acc6f72f2d4c0992934ae715d99aa2c#diff-21b431d8d52b7b53ee9e5214d52c03cc34392d5ee71f5aac292d4fad90487715

### クライアント側で SignalR による リアルタイム送信の処理追加

クライアント側のライブラリとして
```
libman.json 

"defaultProvider": "cdnjs",
  "libraries": [
    {
      "library": "microsoft-signalr@5.0.2",
      "destination": "wwwroot/lib/microsoft-signalr/"
    }
```
を追加

SignalR の WEB サイト側の処理を記述する。

``` js
@section Scripts {
    <script src="~/lib/microsoft-signalr/signalr.min.js"></script>
    <script type="text/javascript">
        // SignalR 接続先
        var connection = new signalR.HubConnectionBuilder().withUrl("/UserHub").build();
        // メッセージ受信時の処理
        connection.on("ReceiveMessage", function (message) {
            // 受信したメッセージの表示
            console.log("ReceiveMessage:" + message);
            $("<li />").appendTo("#messagesList").text(message);
        });
        // 終了メッセージ受信時の処理
        connection.on("ReceiveFinishMessage", function () {
            console.log("ReceiveFinishMessage");
        });
        var userGroup = $("#userGroup").val();
        // SignalR 受信処理を開始
        connection.start().then(function () {
            if (userGroup) {
                // グループに参加
                connection.invoke("AddToGroup", userGroup).catch(function (err) {
                    return console.error(err.toString());
                });
            }
        }).catch(function (err) {
            return console.error(err.toString());
        });
        // クライアントによる 情報の通知
        if (userGroup) {
            $("#sendButton").click(function () {
                var message = $("#messageInput").val();
                connection.invoke("SendClientMessage", userGroup, message).catch(function (err) {
                    return console.error(err.toString());
                });
            });
        } else {
            $("#sendButton").prop("disabled", true);
        }
    </script>
```

ボタンと メッセージ表示領域を追加する。

### サーバー側で バックグランドに実行する処理の追加

```
Services\BackgroundTaskQueue.cs
Services\QueuedHostedService.cs
```

を追加

Startup.cs に バックグランド用のタスクを サービスに登録する。
```
            // バックグランドタスク用
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
```

ページの処理で 下記のように コードを追加して
バックグランドで処理を実行できるようにする。

```

public IBackgroundTaskQueue Queue { get; }
        public HomeController(
            IBackgroundTaskQueue queue,
            ILogger<HomeController> logger)
        {
            _logger = logger;
            Queue = queue;
        }
        
        
        [HttpPost,ActionName("Index")]
        public IActionResult IndexPost()
        {
            // 遅い処理をバックグラウンドで実行させる
            Queue.QueueBackgroundWorkItem(async token =>
            {
                await Task.Delay(1000);
                Debug.WriteLine("A");
                await Task.Delay(1000);
                Debug.WriteLine("B");
            });

            return View();
        }

```

### toast 表示を行う

クライアント側のライブラリとして toast.js を使って ユーザに通知するメッセージを トースト形式とする。


