using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using ServiceLib;

namespace Server
{
    class Program
    {
        static string dataBaseFilePath = "./../../data/data.db";

        static void Main()
        {
            IPAddress ipAddress = IPAddress.Loopback;
            int port = 3000;
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {

                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

                listener.Bind(localEndPoint);
                listener.Listen();

                while (true)
                {
                    Console.WriteLine($"Waiting for a connection on port {port}");

                    Socket handler = listener.Accept();

                    Console.WriteLine($"User [{handler.RemoteEndPoint}] connected");
                    Thread thread = new Thread(StartNewThread);
                    thread.Start(handler);
                }
            }
            catch
            {
                Console.Error.WriteLine($"Can't start a server on port {port}");
            }
            listener.Close();
        }

        static void StartNewThread(object obj)
        {
            Socket socket = (Socket)obj;
            ProcessClient(socket);
            Console.WriteLine($"Connection with [{socket.RemoteEndPoint}] closed");
            socket.Close();
        }

        static string PrepareAnswer(string input)
        {
            return input.Length + "<DEL>" + input;
        }

        static string ReadAllMessage(Socket sender)
        {
            string data = string.Empty;
            byte[] bytes = new byte[1024];

            int bytesRec = sender.Receive(bytes);
            string message = Encoding.ASCII.GetString(bytes, 0, bytesRec);
            string[] splited = message.Split("<DEL>", 2);
            int count = int.Parse(splited[0]);

            data += splited[1];

            while (count > data.Length)
            {
                int nBytes = sender.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, nBytes);
            }

            return data;
        }

        static void ProcessClient(Socket handler)
        {
            string data = string.Empty;

            while (true)
            {
                try
                {
                    data = ReadAllMessage(handler);

                    Console.WriteLine("Text received : {0}", data);

                    CommandParser parser = new CommandParser(new Service(
                        new EntitiesProcessingLib.Repositories.UserRepository(dataBaseFilePath),
                        new EntitiesProcessingLib.Repositories.CourseRepository(dataBaseFilePath),
                        new EntitiesProcessingLib.Repositories.LectureRepository(dataBaseFilePath),
                        new EntitiesProcessingLib.Repositories.SubscriptionRepository(dataBaseFilePath)
                    ));

                    string answer = string.Empty;
                    try
                    {
                        answer = parser.ParseCommand(data);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                        break;
                    }

                    byte[] msg = Encoding.ASCII.GetBytes(PrepareAnswer(answer));

                    Console.WriteLine("Sent: {0}", answer);

                    data = "";
                    handler.Send(msg);
                }
                catch
                {
                    break;
                }
            }
        }
    }
}