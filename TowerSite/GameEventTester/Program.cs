using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEventTester
{
    class Program
    {


        static void Main(string[] args)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            Uri uri = new Uri("https://localhost:44300/odata");
            GameEventService.Container container = new GameEventService.Container(uri);
            container.SendingRequest2 += (s, e) =>
            {
                Console.WriteLine("{0} {1}", e.RequestMessage.Method, e.RequestMessage.Url);
            };

            bool done = false;
            bool loggedIn = false;
            char[] delims = { ' ', ':' };
            String line;
            while (!done)
            {
                line = Console.ReadLine();
                string[] lARgs = line.Split(delims);

                switch (lARgs[0].ToLower())
                {
                    case "quit":
                    case "exit":
                        done = true;
                        break;
                    case "login":
                        if (System.Web.Security.Membership.ValidateUser(lARgs[1], lARgs[2]))
                        {
                            loggedIn = true;
                            Console.WriteLine("Login successful");
                        }
                        else
                        {
                            loggedIn = false;
                            Console.WriteLine("Login failed");
                        }
                        break;
                    case "listall":
                        ListAllEvents(container);
                        break;
                    case "add":
                        GameEventService.GameEventModel ge = new GameEventService.GameEventModel();
                        ge.SessionId = int.Parse(lARgs[1]);
                        ge.Timestamp = DateTime.Now;
                        ge.Type = lARgs[2];
                        ge.Data = lARgs[3];
                        AddEvent(container, ge);
                        break;
                }
            }
        }
        static void DisplayEvent(GameEventService.GameEventModel ge)
        {
            Console.WriteLine("{0} {1} {2} {3}", ge.SessionId, ge.Timestamp, ge.Type, ge.Data);
        }
        static void ListAllEvents(GameEventService.Container container)
        {
            foreach (var p in container.GameEvent)
            {
                DisplayEvent(p);
            }
        }
        static void AddEvent(GameEventService.Container container, GameEventService.GameEventModel ge)
        {
            container.AddToGameEvent(ge);
            var serviceResponse = container.SaveChanges();
            foreach (var operationResponse in serviceResponse)
            {
                Console.WriteLine(operationResponse.StatusCode);
            }
        }
    }
}
