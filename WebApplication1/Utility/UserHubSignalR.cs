using System;
using System.Threading.Tasks;
using WebApplication1.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;

namespace WebApplication1.Utility
{
    /// <summary>
    /// SignalR の クライアントライブラリを サーバー側で実装する。
    /// 
    /// サーバーで遅い処理をバックグラウンドで行いその結果をユーザに通知する。
    /// </summary>
    public class UserHubSignalR
    {

        /// <summary>
        /// URLを指定して 通知インスタンスを作成する
        /// </summary>
        /// <param name="channelURL"></param>
        /// <param name="userGroup"></param>
        public UserHubSignalR(HttpRequest request, string userGroup)
        {
            string channelURL = request.Scheme + "://" + request.Host + UserHub.EndPoint_URL;

            _userGroup = userGroup;
            _channelURL = channelURL;
        }
        private string _channelURL;
        private string _userGroup;
        private HubConnection _connection;
        private bool _connected = false;

        /// <summary>
        /// ユーザに通知メッセージを送る
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(string msg)
        {
            await SendHubFunctionAsync(UserHub.SingalR_SendMessage, msg);
        }
        /// <summary>
        /// 処理終了したことを通知する
        /// </summary>
        /// <returns></returns>
        public async Task SendClientFinishMessageAsync()
        {
            await SendHubFunctionAsync(UserHub.SingalR_SendFinishMessage);
        }

        /// <summary>
        /// 接続を破棄
        /// </summary>
        /// <returns></returns>
        public async Task DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        private async Task CheckConnect()
        {
            if (!_connected)
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl(_channelURL)
                    .Build();

                await _connection.StartAsync();

                _connected = true;
            }
        }


        private async Task SendHubFunctionAsync(string functionName, string msg)
        {
            try
            {
                await CheckConnect();

                await _connection.InvokeAsync(functionName, _userGroup, msg);
            }
            catch (Exception ex)
            {
                // 全てのエラーを無視して 例外情報として記録しておく
                _connected = false;
                _lastException = ex;
            }
        }
        private async Task SendHubFunctionAsync(string functionName)
        {
            try
            {
                await CheckConnect();

                await _connection.InvokeAsync(functionName, _userGroup);
            }
            catch (Exception ex)
            {
                // 全てのエラーを無視して 例外情報として記録しておく
                _connected = false;
                _lastException = ex;
            }
        }

        Exception _lastException;

        /// <summary>
        /// 最後に発生したエラーを返す
        /// </summary>
        /// <returns></returns>
        public Exception GetLastException()
        {
            return _lastException;
        }
    }
}
