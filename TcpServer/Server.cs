using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BookLibrary;
using Newtonsoft.Json;

namespace TcpServer
{
    public class Server
    {
        private static List<Book> bookList = new List<Book>
        {
            new Book("I offentlighedens tjeneste",400,"9788793772137","Politiken"),
            new Book("Simones perlebog",100,"9788793825000","Simone"),
            new Book("Ternet Ninja",272,"9788763859646","Anders Matthesen"),
            new Book("De smaa synger",127,"9788714195519","Benita Haastrup"),
            new Book("Funny Face",25,"9978814155520","Funny Face")
        };

        public void Start()
        {
            TcpListener tcpServer = new TcpListener(IPAddress.Loopback, 4646);// Loopback er ens egen maskine
            tcpServer.Start();

            while (true)
            {
                TcpClient socket = tcpServer.AcceptTcpClient(); // Denne venter hårdt på en client

                // Starter ny tråd
                Task.Run(() =>
                {
                        // Indsætter metoder (delegate) som lambda
                        TcpClient tmpSocket = socket;
                    Run(tmpSocket);

                });

            }
        }

        public void Run(TcpClient socket)
        {
            using (StreamReader sr = new StreamReader(socket.GetStream()))
            using (StreamWriter sw = new StreamWriter(socket.GetStream()))
            {
                    string lineOne = sr.ReadLine();
                    string lineTwo;

                    switch (lineOne)
                    {
                        case "HentAlle":
                            // json list
                            if (sr.ReadLine().Equals(""))
                            {
                                sw.WriteLine(JsonConvert.SerializeObject(bookList));
                                sw.Flush();
                            }
                            break;
                        case "Hent":
                            lineTwo = sr.ReadLine();
                            // json object
                            sw.WriteLine(JsonConvert.SerializeObject(bookList.First(b => b.ISbn == lineTwo)));
                            sw.Flush();
                            break;
                        case "Gem":
                            // enter book as json
                            lineTwo = sr.ReadLine();
                            // save json object
                            bookList.Add(JsonConvert.DeserializeObject<Book>(lineTwo));
                            sw.Flush();
                            break;
                    }
            }
            /// Bogobjekter i json skal skrives i dette format:
            /// {"Title":"I offentlighedens tjeneste","Author":"Politiken","NumberOfPages":400,"ISbn":"9788793772137"}


            socket?.Close();
        }
    }
}
