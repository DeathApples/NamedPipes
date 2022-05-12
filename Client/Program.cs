using System.Text;
using System.IO.Pipes;
using System.Security.Principal;

public class Client
{
    static string pipeName = "testpipe";

    static byte[] data;
    static NamedPipeClientStream pipeClient;

    public static void Main(string[] args)
    {
        Initialise();
        Send();
    }

    static void Initialise()
    {
        data = new byte[1024];
        pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);
    }

    static void Send()
    {
        pipeClient.Connect();

        Console.Write("Введите имя файла, который хотите сохранить на сервере: ");
        string filename = Console.ReadLine() ?? "";

        data[0] = (byte)filename.Length;
        Encoding.UTF8.GetBytes(filename, 0, filename.Length, data, 1);

        pipeClient.Write(data, 0, data.Length);

        using (var fstream = new FileStream(filename, FileMode.Open))
        {
            while (fstream.Read(data, 0, data.Length) > 0)
            {
                pipeClient.Write(data, 0, data.Length);
            }
        }

        pipeClient.Close();
    }
}
