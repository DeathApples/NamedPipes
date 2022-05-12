using System.Text;
using System.IO.Pipes;

class Server
{
    static string pipeName = "testpipe";

    static byte[] data;
    static StringBuilder filename;
    static NamedPipeServerStream pipeServer;

    static void Main()
    {
        Initialise();
        Listening();
    }

    static void Initialise()
    {
        data = new byte[1024];
        filename = new StringBuilder();
        pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut);
    }

    static void Listening()
    {
        int size;
        pipeServer.WaitForConnection();

        try
        {
            pipeServer.Read(data, 0, data.Length);
            filename.Append(Encoding.UTF8.GetString(data, 1, data[0]));

            Console.WriteLine(filename);

            using (FileStream fstream = new FileStream(filename.ToString(), FileMode.OpenOrCreate))
            {
                do
                {
                    size = pipeServer.Read(data);
                    fstream.Write(data, 0, size);

                } while (pipeServer.IsConnected);
            }

            filename.Clear();

        }
        catch (IOException e)
        {
            Console.WriteLine($"ERROR: {e.Message}");
        }

        pipeServer.Close();
    }
}
