namespace GINS_Task.Utility
{
    public class ConnectionString
    {
        private static string cnn = "Data Source=DURJOY; Initial Catalog=GINSTask; Integrated Security= true; TrustServerCertificate=true";

        public static string CNN { get { return cnn; } }
    }
}
