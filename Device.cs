namespace sim;


class Device
{
    private readonly int port = 48323;
    private readonly string password;
    private readonly string ip;
    private readonly Comm comm;
    public Device(string ip = "localhost", string password = "")
    {
        this.password = password;
        this.ip = ip;
        comm = new Comm(this.ip, this.port, this.password);

    }

    public void EnableMachine(bool data)
    {
        comm.SendAndWait("enableMachine:" + (data ? "True" : "False"));

    }

    public State GetState()
    {
        return (State)Enum.Parse(typeof(State), comm.SendAndWait("getState:"));

    }

}