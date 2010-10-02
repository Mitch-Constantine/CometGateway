using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspComet.Samples.Chat
{
    public class MessageSender
    {
        private IClientRepository repository;

        public MessageSender(IClientRepository repository)
        {
            this.repository = repository;
        }

        public void SendMessage()
        {
            foreach( var client in repository.WhereSubscribedTo("/telnet"))
            {
                var message = new Message()
                                      {
                                          channel = "/telnet",
                                          data = new {sender = "auto", message = DateTime.Now.ToString() + "<br>"}
                                      };
                client.Enqueue(message);
                client.FlushQueue();
            }
        }
    }
}