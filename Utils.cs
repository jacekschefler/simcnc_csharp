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