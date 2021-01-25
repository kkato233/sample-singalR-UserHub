using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Hubs
{
    public class UserHub : Hub
    {
        /// <summary>
        /// エンドポイントのURL
        /// </summary>
        public const string EndPoint_URL = "/UserHub";

        // メッセージ受信
        public const string SingalR_ReceiveMessage = "ReceiveMessage";

        // 処理終了メッセージ
        public const string SingalR_ReceiveFinishMessage = "ReceiveFinishMessage";

        /// <summary>
        /// メッセージ送信 (送信メッセージ)
        /// </summary>
        public const string SingalR_SendMessage = nameof(SendClientMessage);

        // 処理終了メッセージ
        public const string SingalR_SendFinishMessage = nameof(SendClientFinishMessage);


        /// <summary>
        /// 指定のグループにメッセージを送る
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendClientMessage(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync(SingalR_ReceiveMessage, message);
        }

        /// <summary>
        /// 指定のグループに終了通知を送る
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task SendClientFinishMessage(string groupName)
        {
            await Clients.Group(groupName).SendAsync(SingalR_ReceiveFinishMessage);
        }

        /// <summary>
        /// メッセージ受信のためにグループに参加する
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// グループから抜ける
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

    }
}
