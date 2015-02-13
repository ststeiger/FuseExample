using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuseExample
{


    internal class SQL
    {



        protected static System.Data.Common.DbProviderFactory GetFactory<T>()
        {
            System.Type t = typeof(T);
            return GetFactory(t);
        } // End Function GetFactory


        protected static System.Data.Common.DbProviderFactory GetFactory(string assemblyType)
        {
#if TARGET_JVM // case insensitive GetType is not supported
			System.Type type = System.Type.GetType (assemblyType, false);
#else
            System.Type type = System.Type.GetType(assemblyType, false, true);
#endif

            return GetFactory(type);
        } // End Function GetFactory


        protected static System.Data.Common.DbProviderFactory GetFactory(System.Type type)
        {
            if (type != null && type.IsSubclassOf(typeof(System.Data.Common.DbProviderFactory)))
            {
                // Provider factories are singletons with Instance field having
                // the sole instance
                System.Reflection.FieldInfo field = type.GetField("Instance"
                    , System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static
                );

                if (field != null)
                {
                    return (System.Data.Common.DbProviderFactory)field.GetValue(null);
                    //return field.GetValue(null) as DbProviderFactory;
                } // End if (field != null)

            } // End if (type != null && type.IsSubclassOf(typeof(System.Data.Common.DbProviderFactory)))

            throw new System.Exception("DataProvider is missing!");
            // throw new System.Configuration.ConfigurationErrorsException("DataProvider is missing!");
            //throw new System.Configuration.ConfigurationException("DataProvider is missing!");
        } // End Function GetFactory


        protected static System.Data.Common.DbProviderFactory GetFactory()
        {
            return GetFactory(typeof(Npgsql.NpgsqlFactory));
            // return GetFactory(typeof(MySql.Data.MySqlClient.MySqlClientFactory));
            // return GetFactory(typeof(System.Data.SqlClient.SqlClientFactory));
        }

        protected static System.Data.Common.DbProviderFactory factory = GetFactory();


        protected static string GetPgConnectionString()
        {
            Npgsql.NpgsqlConnectionStringBuilder csb = new Npgsql.NpgsqlConnectionStringBuilder();
            csb.Host = "127.0.0.1";
            csb.Database = "testdb";

            csb.UserName = "pgfuse";
            csb.Password = "p";

            // PORT=5432;TIMEOUT=15;POOLING=True;MINPOOLSIZE=1;MAXPOOLSIZE=20;COMMANDTIMEOUT=20;COMPATIBLE=2.2.0.0;HOST=127.0.0.1;DATABASE=testdb;USER ID=postgres;PASSWORD=Inspiron1300
            return csb.ConnectionString;
        }


        protected static string GetMySQLConnectionString()
        {
            MySql.Data.MySqlClient.MySqlConnectionStringBuilder csb = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();
            csb.Server = "127.0.0.1";
            csb.Database = "testdb";

            csb.UserID = "root";
            csb.Password = "Inspiron1300";

            return csb.ConnectionString;
        }


        protected static string GetMSSQLConnectionString()
        {
            System.Data.SqlClient.SqlConnectionStringBuilder csb =
                new System.Data.SqlClient.SqlConnectionStringBuilder();
            csb.DataSource = "127.0.0.1";
            csb.InitialCatalog = "testdb";

            csb.UserID = "root";
            csb.Password = "Inspiron1300";

            return csb.ConnectionString;
        }


        protected static System.Data.Common.DbConnection GetConnection()
        {
            System.Data.Common.DbConnection con = factory.CreateConnection();

            if (factory is MySql.Data.MySqlClient.MySqlClientFactory)
                con.ConnectionString = GetMySQLConnectionString();
            else if (factory is Npgsql.NpgsqlFactory)
                con.ConnectionString = GetPgConnectionString();
            else if (factory is System.Data.SqlClient.SqlClientFactory)
                con.ConnectionString = GetMSSQLConnectionString();
            else
            {
                System.Console.WriteLine("GetConnectionString for your SQL-factory is not implemented !");
                throw new System.NotImplementedException("GetConnectionString for your SQL-factory");
            }

            return con;
        }


		public static T ExecuteScalar<T>(string strSQL)
		{
			object obj = ExecuteScalar (strSQL);

			if(typeof(T) == typeof(int))
			{
				int i = System.Convert.ToInt32(obj);
				return (T)(object)i;
			}
			else if(typeof(T) == typeof(long))
			{
				long i = System.Convert.ToInt64(obj);
				return (T)(object)i;
			}

			return (T) obj;
		}


		public static int ExecuteNonQuery(string strSQL)
		{
			int iRet = 0;

			using (System.Data.Common.DbConnection con = GetConnection())
			{
				using (System.Data.Common.DbCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = strSQL;
					if (con.State != System.Data.ConnectionState.Open)
						con.Open();

					iRet = cmd.ExecuteNonQuery();

					if (con.State != System.Data.ConnectionState.Closed)
						con.Close();
				} // End Using cmd

			} // End using con 

			return iRet;
		}


        // protected static object objLock = new object();

        public static object ExecuteScalar(string strSQL)
        {
            object obj = null;

            using (System.Data.Common.DbConnection con = GetConnection())
            {
                using (System.Data.Common.DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();

                    obj = cmd.ExecuteScalar();

                    if (con.State != System.Data.ConnectionState.Closed)
                        con.Close();
                } // End Using cmd

            } // End using con 

            return obj;
        }


        public static System.Data.DataTable GetDataTable(string strSQL)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            using (System.Data.Common.DbConnection con = GetConnection())
            {

                using (System.Data.Common.DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = strSQL;

                    using (System.Data.Common.DbDataAdapter da = factory.CreateDataAdapter())
                    {
                        da.SelectCommand = cmd;
                        da.Fill(dt);
                    } // End Using da

                } // End Using cmd
            } // End Using con

            return dt;
        }


    } // End internal class SQL


} // End Namespace FuseExample
