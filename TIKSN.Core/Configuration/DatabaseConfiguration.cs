namespace TIKSN.Configuration
{
    public class DatabaseConfiguration
    {
        public DatabaseConfiguration(string databasePath)
        {
            DatabasePath = databasePath;
        }

        public string DatabasePath { get; private set; }
    }
}
