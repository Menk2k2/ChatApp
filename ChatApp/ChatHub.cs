using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ChatApp.Models;
using System.Data.Entity;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace ChatApp
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        private ChatAppEntities _context = new ChatAppEntities();

        // Phương thức để gửi tin nhắn từ client này đến tất cả các client khác
        public async Task SendMessage(int senderId, int receiverID, int chanelID, string message) //, string connectionId
        {
            try
            {
                var senderUser = await _context.tblEmployees.FindAsync(senderId);
                var receiverUser = await _context.tblEmployees.FindAsync(receiverID);
                var chanel = await _context.Channels.FindAsync(chanelID);
                var mess = new Message();
                mess.SenderId = senderUser.Id;
                if (receiverID > 0) mess.ReceiverID = receiverUser.Id;
                if (chanelID > 0) mess.ChannelId = chanel.Id;
                mess.Content = message;
                mess.DateSent = DateTime.Now;
                _context.Messages.Add(mess);
                await _context.SaveChangesAsync();

                // Gọi phương thức addMessageToPage trên tất cả các client để cập nhật tin nhắn mới
                Clients.All.addMessageToPage(senderUser.Name, message, receiverID, senderId, chanelID); //.Client(receiverUser.Connection_Id)
            }
            catch (Exception ex)
            {
                Clients.All.addMessageToPage(1, "Đã xảy ra lỗi: " + ex.ToString());
            }

        }

        public async Task CreateGroup(string groupName, List<int> listAdd, int userCreateId, int chanelId = 0)
        {
            try
            {
                if(chanelId > 0) {
                    var chanel = await _context.Channels.FindAsync(chanelId);
                    if(chanel != null)
                    {
                        chanel.Name = groupName;
                        _context.Entry(chanel).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        var deleteQuery = string.Format("DELETE FROM ChannelUsers where ChannelId = {0}", chanelId);
                        var result = await _context.Database.ExecuteSqlCommandAsync(deleteQuery);

                        foreach (var item in listAdd)
                        {
                            await AddUserToGroup(chanel.Id, item, userCreateId);
                        }

                        Clients.All.pushMessage(0, "Cập nhật nhóm thành công");
                    }
                    else
                    {
                        Clients.All.pushMessage(1, "Nhóm không tồn tại!");
                    }
                }else
                {
                    var group = new Channel { Name = groupName, CreateDate = DateTime.Now };
                    _context.Channels.Add(group);
                    await _context.SaveChangesAsync();

                    foreach (var item in listAdd)
                    {
                        await AddUserToGroup(group.Id, item, userCreateId);
                    }

                    Clients.All.pushMessage(0, "Tạo mới nhóm thành công");
                }
            }
            catch (Exception ex)
            {
                Clients.All.pushMessage(1, "Đã xảy ra lỗi trong quá trình tạo mới nhóm: " + ex.ToString());
            }
        }

        public async Task AddUserToGroup(int groupId, int userId, int userCreateId)
        {
            try
            {
                var group = await _context.Channels.FindAsync(groupId);
                var user = await _context.tblEmployees.FindAsync(userId);
                if (group != null && user != null)
                {
                    var groupMember = new ChannelUser { ChannelId = group.Id, UserId = user.Id };
                    if (userId == userCreateId)
                    {
                        groupMember = new ChannelUser { ChannelId = group.Id, UserId = user.Id, IsAdmin = true };
                    }
                    _context.ChannelUsers.Add(groupMember);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
            }

        }

        //Kết nối tới server
        public override Task OnConnected()
        {
            ConnectedUser.connections.Add(Context.ConnectionId);
            return base.OnConnected();
        }

        // Ngắt kết nối đến server
        public override Task OnDisconnected(bool stopCalled)
        {
            ConnectedUser.connections.Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
    }

    public static class ConnectedUser
    {
        public static List<string> connections = new List<string>();
    }
}
