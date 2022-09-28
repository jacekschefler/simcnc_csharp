using System.Globalization;
using System.IO.Hashing;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace sim;
public class Comm
{
    private static int _instances = 0;
    private readonly int instance_number;
    private readonly int appNr = 0;
    private readonly string startActSrc = "externalProgram 0";

    private readonly string password;

    private readonly Socket socket;


    int len;
    string lenStr = string.Empty;
    bool lenReady;
    ulong crc;
    string crcStr = string.Empty;
    bool crcReady;
    string datagram = string.Empty;
    public Comm(string ip, int port, string password)
    {
        _instances++;
        instance_number = _instances;
        this.password = password;
        socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(ip, port);
        ClearData();
        SendConnectionRequest();



    }
    private void ClearData()
    {
        len = 0;
        lenStr = "";
        lenReady = false;
        crc = 0;
        crcStr = "";
        crcReady = false;
        datagram = "";

    }
    private void SendConnectionRequest()
    {
        var request = "connectionRequest:" + instance_number.ToString() + ":" + appNr.ToString() + ":" + startActSrc + ":" + Utils.EncodeTo64(password);
        Send(request);
    }

    private void Send(string request)
    {
        string msg = PrepareMsg(request);
        socket.Send(Encoding.UTF8.GetBytes(msg));
    }

    private static string PrepareMsg(string request)
    {
        return AddCRC(AddLen(request));
    }

    private static string AddLen(string request)
    {
        return request.Length.ToString() + ":" + request;
    }

    private static string AddCRC(string value)
    {
        return CalcCRC32(Encoding.UTF8.GetBytes(value)) + value;
    }

    private static string CalcCRC32(byte[] bytes)
    {
        var crcVal = BitConverter.ToInt32(Crc32.Hash(bytes), 0) & 0xFFFFFFFF;
        return string.Format("{0:X8}", crcVal);
    }

    private bool CheckCRC(byte[] s)
    {
        var result = Crc32.Hash(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(s).Length.ToString() + ":" + Encoding.UTF8.GetString(s)));
        var v = BitConverter.ToUInt32(result, 0);
        //Console.WriteLine(v);
        //Console.WriteLine(crc);
        //Console.WriteLine(crc & 0xffffffff);

        return v == (crc & 0xffffffff);
    }

    bool FillData(string msg)
    {
        if (!crcReady)
        {
            var crcRemain = 8 - crcStr.Length;
            crcStr += msg[..crcRemain];
            msg = msg[crcRemain..];
            if (crcStr.Length == 8)
            {
                crcReady = true;

                var result = ulong.TryParse(crcStr, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out crc);
                if (!result)
                {
                    Console.WriteLine(crcStr);
                    Console.WriteLine("Incorrect datagram(crc is not int");
                    Environment.Exit(1);
                }
            }
            else
            {
                return false;
            }
        }
        if (msg.Length == 0)
        {
            return false;
        }

        var ready = false;

        if (!lenReady)
        {
            var colonPos = msg.IndexOf(":");
            if (colonPos < 0)
            {
                lenStr += lenStr + msg;
                msg = "";
            }
            else
            {
                lenStr += msg[..colonPos];
                msg = msg[(colonPos + 1)..];
                lenReady = true;
            }
            var result = int.TryParse(lenStr, out var length);
            if (!result)
            {
                Console.WriteLine("Incorrect datagram, len is not int");
                Environment.Exit(1);
            }
            if (!lenReady)
            {
                return false;
            }

            len = length;

            if (msg.Length == 0)
            {
                return false;
            }

        }

        if (lenReady)
        {
            var datagramRemain = len - datagram.Length;
            datagram += msg[..datagramRemain];
            _ = msg[datagramRemain..];
            if (datagram.Length == len)
            {
                ready = true;
            }

        }
        return ready;
    }

    private void GetReceivedData()
    {
        while (true)
        {
            var buffer = new byte[1024];
            socket.Receive(buffer);
            var msg = Encoding.UTF8.GetString(buffer);
            if (FillData(msg))
            {
                break;
            }
        }
    }
    public string SendAndWait(string msg)
    {
        Send(msg);
        ClearData();
        GetReceivedData();
        var data = Encoding.UTF8.GetBytes(datagram);
        if (CheckCRC(data) == false)
        {
            Console.WriteLine("CRC error");
            Environment.Exit(1);
        }
        var res = Encoding.UTF8.GetString(data).Split(":");
        if (res.Length >= 2 && res[0] == "commandExecuted" && res[1] == "ok")
        {
            return string.Join(":", res[2..]);
        }
        else
        {
            throw new Exception(string.Join(":", res[1..]));
        }
    }
}
