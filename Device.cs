using static System.Runtime.InteropServices.JavaScript.JSType;

namespace sim;


class Device
{
    private readonly int port;
    private readonly string password;
    private readonly string ip;
    public  Comm Comm { get; }
    public Device(string ip = "localhost", string password = "", int port = 48323)
    {
        this.password = password;
        this.ip = ip;
        this.port = port;
        Comm = new Comm(this.ip, this.port, this.password);

    }

    public void EnableMachine(bool data)
    {
        Comm.SendAndWait("enableMachine:" + (data ? "True" : "False"));

    }

    public State GetState()
    {
        return (State)Enum.Parse(typeof(State), Comm.SendAndWait("getState:"));

    }

    public bool IsGcodeFileLoaded()
    {
        return Comm.SendAndWait("isGcodeFileLoaded:") == "True";
    }

    public void OpenGcodeFile(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new ArgumentException($"Element „{nameof(data)}” can not be null or empty.", nameof(data));
        }

        Comm.SendAndWait("openGCodeFile:" + data);
    }

    public void CloseGcodeFile()
    {
        Comm.SendAndWait("closeGCodeFile:");
    }
}