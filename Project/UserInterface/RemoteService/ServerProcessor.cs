using System.Xml.Serialization;
using System.Text;
using System.IO;
using System.Net.Sockets;
using Terminal.Gui;

namespace ServiceLib
{
    public static class ServerProcessor
    {
        public static T SendRequest<T>(Socket sender, string request)
        {
            string msgSent = PrepareAnswer(request);

            byte[] msg = Encoding.ASCII.GetBytes(msgSent);

            try
            {
                int bytesSent = sender.Send(msg);
            }
            catch (System.Exception ex)
            {
                MessageBox.ErrorQuery("Error", ex.Message, "Ok");
            }

            return ParseEntity<T>(ReadAllMessage(sender));
        }

        public static string AddAgrument<T>(string message, T argument)
        {
            return message + "<ARG>" + EntityToXml(argument);
        }

        private static string ReadAllMessage(Socket sender)
        {
            string data = string.Empty;
            byte[] bytes = new byte[1024];

            int bytesRec = 0;
            try
            {
                bytesRec = sender.Receive(bytes);
                string message = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                string[] splited = message.Split("<DEL>", 2);
                int count = int.Parse(splited[0]);

                data += splited[1];

                while (count > data.Length)
                {
                    int nBytes = sender.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, nBytes);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.ErrorQuery("Error", ex.Message, "Ok");
                data = "Error";
            }

            return data;
        }

        private static string PrepareAnswer(string input)
        {
            return input.Length + "<DEL>" + input;
        }

        private static string EntityToXml<T>(T entity)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            StringWriter writer = new StringWriter();
            ser.Serialize(writer, entity);

            string result = writer.ToString();
            writer.Close();

            return result;
        }

        public static T ParseEntity<T>(string xmlData)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(xmlData);

            T result = (T)ser.Deserialize(reader);
            reader.Close();

            return result;
        }
    }
}