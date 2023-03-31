public class ClientInfo
{
    private static readonly int mainVersion = 0;
    private static readonly int subVersion = 1;

    public static string ClientVersion
    {
        get
        {
            var versionString = string.Format("{0}.{1:D2}", mainVersion, subVersion);
            return versionString;
        }
    }
}
