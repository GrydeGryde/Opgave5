using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using opgave1;

namespace ObligatoriskOpgave5
{
    class Program
    {
        private static List<FootballPlayer> Players = new List<FootballPlayer>();

        static void Main(string[] args)
        {

            Console.WriteLine("3. Semester: Obligatorisk Opgave 5 TCP Server");
            TcpListener listener = new TcpListener(2121);
            listener.Start();
            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();
                Task.Run(() => TaskMetode(socket));
            }
        }

        public static void TaskMetode(TcpClient socket)

        {
            
            Console.WriteLine("En klient er kommet ind");
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);
            while (true)
            {
                string message = reader.ReadLine();
                if (message.StartsWith("HentAlle", StringComparison.InvariantCultureIgnoreCase))
                {
                    reader.ReadLine();
                    foreach (FootballPlayer player in Players)
                    {
                        writer.WriteLine(JsonSerializer.Serialize(player));
                    }
                }

                else if (message.StartsWith("HentID", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Skriv det ID du vil søge efter");
                    int id = Convert.ToInt32(reader.ReadLine());
                    FootballPlayer playertoFind = Players.Find(player => player.ID == id);
                    writer.WriteLine(JsonSerializer.Serialize(playertoFind));
                }

                else if (message.StartsWith("Gem", StringComparison.InvariantCultureIgnoreCase))
                {
                    //jsonfilen er Casesensitive: eksempel at indsætte er nednstående:
                    //{"ID": 1,"Name": "Ronaldo","Price": 1000,"ShirtNumber": 8}
                    Console.WriteLine("Gem objektet (ID, Name, Price, ShirtNumber) som json husk de store bogstaver");
                    string jsonplayer = reader.ReadLine();
                    FootballPlayer deserializedplayer = JsonSerializer.Deserialize<FootballPlayer>(jsonplayer);
                    Players.Add(deserializedplayer);
                }

                writer.Flush();
                if (message.ToLower() == "stop")
                {
                    socket.Close();
                    break;
                }
            }
        }
    }
}
