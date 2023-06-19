namespace TiMonSys
{
    public sealed class ContextManager
    {
        #region Синглтон (общедоступный экземпляр класса на все приложение, потокобезопасно)

        static readonly ContextManager instance = new ContextManager();
        static ContextManager() { }
        private ContextManager() { }

        /// <summary>
        /// Единственный экземпляр менеджера контекста данных (потокобезопасно)
        /// </summary>
        public static ContextManager Instance
        {
            get { return instance; }
        } 

        #endregion Синглтон

        private string _connectionString = "Data Source=";
        private const string EDMX_MODEL = "ModelDB";
        private const string PROVIDER = "System.Data.SqlServerCe.3.5";


        public DBEntities Database { get; private set; }
    

        public DBEntities CreateDataContext()
        {
           var context =  string.IsNullOrEmpty(_connectionString) ?
                new DBEntities() 
               
                :
                new DBEntities(GetDataContextConnectionString(
                                    edmxName: EDMX_MODEL,
                                    provider: PROVIDER,
                                    providerConnectionString: _connectionString
                                ));
          
           return context;
        }

     
        public bool Connect(string sqlConnectionString)
        {
          
            var isReady = false;
            if (string.IsNullOrEmpty(sqlConnectionString)) return false;

            sqlConnectionString =string.Format("Data Source={0};", sqlConnectionString);
            
            using (var connection = new System.Data.SqlServerCe.SqlCeConnection())
            {
                try
                {
                    connection.ConnectionString = sqlConnectionString;
                    connection.Open();
                    connection.Close();
                    _connectionString = sqlConnectionString;
                    Database = new DBEntities(
                        GetDataContextConnectionString(
                            edmxName: EDMX_MODEL,
                            provider: PROVIDER,
                            providerConnectionString: _connectionString
                        ));
                    isReady = true;
                }
                catch (System.Data.SqlClient.SqlException)
                {
                   
                }
            }
            return isReady;
        }

        public void ResetDataContext()
        {
            Database = CreateDataContext();
        }

     
       /// <summary>
        /// Строка подключения для контекста данных Entity Framework
        /// </summary>
        private static string GetDataContextConnectionString(string edmxName, string provider, string providerConnectionString)
        {
            string res = string.Format(
                 "provider connection string=\"{2}\"; " +
                "provider={1};" +
                "metadata =res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl"
               
                ,
                edmxName, provider, providerConnectionString);
            return res;

        }
    }
}
