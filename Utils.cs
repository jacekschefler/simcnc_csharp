namespace sim;

class Utils
{
    static public string EncodeTo64(string toEncode)

    {

        byte[] toEncodeAsBytes

              = System.Text.Encoding.UTF8.GetBytes(toEncode);

        string returnValue

              = Convert.ToBase64String(toEncodeAsBytes);

        return returnValue;

    }

}


enum State
{
    EStop = 0,
    Idle = 1,
    Homing = 2,
    Trajectory = 3,
    JOG = 4,
    MDI = 5,
    MPG = 6
}


public abstract record ResultA
{
    ResultA(){ }
    public sealed record IntResult(int Result): ResultA;
    public sealed record StringResult(string Result): ResultA;
    public sealed record BoolResult(bool Result): ResultA;
    public sealed record DoubleResult(double Result): ResultA;


}

