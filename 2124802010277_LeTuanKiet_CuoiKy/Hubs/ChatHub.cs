using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Hubs
{
    [HubName("Chat")]
    public class ChatHub : Hub
    {
        DataTiengAnhEntities db = new DataTiengAnhEntities();
        public void Hello()
        {
            Clients.All.hello();
        }
        public async Task JoinRoom(string roomName)
        {
            await Groups.Add(Context.ConnectionId, roomName);
        }
        public void Connect(string name)
        {
            Clients.Caller.name(name);
        }
        public void Message(string name, string message,string IdRoom, string IdUser,string Img)
        {
            Message m = new Message();
            m.IdNguoiDung =int.Parse(IdUser);
            m.NoiDung = message;
            m.Gio = DateTime.Now;
            m.RoomId = int.Parse(IdRoom);
            db.Messages.Add(m);
            db.SaveChanges();
            Clients.Group(IdRoom).message(name, message,IdUser,Img);
        }
    }
}