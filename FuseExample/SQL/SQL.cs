
namespace FuseExample
{


    internal static class SQL
    {
		private static System.Reflection.BindingFlags m_CaseSensitivity = 
			  System.Reflection.BindingFlags.Instance
			| System.Reflection.BindingFlags.Public
			| System.Reflection.BindingFlags.IgnoreCase
		;


		private static bool IsNullable(System.Type t)
		{
			if (t == null)
				return false;

			return t.IsGenericType && object.ReferenceEquals(t.GetGenericTypeDefinition(), typeof(System.Nullable<>));
		} // End Function IsNullable

		private static bool Log(System.Exception ex, System.Data.IDbCommand cmd)
		{
			System.Console.WriteLine(ex.Message);
			System.Console.WriteLine (cmd.CommandText);

			return true; // Rethrow
		}

		private static System.Reflection.MemberInfo GetMemberInfo(System.Type t, string strName)
		{
			System.Reflection.MemberInfo mi = t.GetField(strName, m_CaseSensitivity);

			if (mi == null)
				mi = t.GetProperty(strName, m_CaseSensitivity);

			return mi;
		} // End Function GetMemberInfo


		private static void SetMemberValue(object obj, System.Reflection.MemberInfo mi, object objValue)
		{
			if (mi is System.Reflection.FieldInfo)
			{
				System.Reflection.FieldInfo fi = (System.Reflection.FieldInfo)mi;
				fi.SetValue(obj, MyChangeType(objValue, fi.FieldType));
				return;
			}

			if (mi is System.Reflection.PropertyInfo)
			{
				System.Reflection.PropertyInfo pi = (System.Reflection.PropertyInfo)mi;
				pi.SetValue(obj, MyChangeType(objValue, pi.PropertyType), null);
				return;
			}

			// Else silently ignore value
		} // End Sub SetMemberValue


		private static object MyChangeType(object objVal, System.Type t)
		{
			if (objVal == null || object.ReferenceEquals(objVal, System.DBNull.Value))
			{
				return null;
			}

			//getbasetype
			System.Type tThisType = objVal.GetType();

			bool bNullable = IsNullable(t);
			if (bNullable)
			{
				t = System.Nullable.GetUnderlyingType(t);
			}

			if (object.ReferenceEquals(t, typeof(string)) && object.ReferenceEquals(tThisType, typeof(System.Guid)))
			{
				return objVal.ToString();
			}

			return System.Convert.ChangeType(objVal, t);
		} // End Function MyChangeType


		public static System.Data.IDataReader ExecuteReader(System.Data.IDbCommand cmd)
		{
			System.Data.IDataReader idr = null;

			lock (cmd)
			{
				System.Data.IDbConnection idbc = GetConnection();
				cmd.Connection = idbc;

				if (cmd.Connection.State != System.Data.ConnectionState.Open)
					cmd.Connection.Open();

				try
				{
					idr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
				}
				catch (System.Exception ex)
				{
					if (Log(ex, cmd))
						throw;
				}
			} // End Lock cmd

			return idr;
		} // End Function ExecuteReader


		public static System.Data.IDataReader ExecuteReader(string strSQL)
		{
			System.Data.IDataReader idr = null;

			using (System.Data.IDbCommand cmd = CreateCommand(strSQL))
			{
				idr = ExecuteReader(cmd);
			} // End Using cmd

			return idr;
		} // End Function ExecuteReader


		public static T GetClass<T>(System.Data.IDbCommand cmd, T tThisClassInstance)
		{
			System.Type t = typeof(T);

			lock (cmd)
			{
				using (System.Data.IDataReader idr = ExecuteReader(cmd))
				{

					lock (idr)
					{

						bool bHasNoRows = true;

						while (idr.Read())
						{
							bHasNoRows = false;

							for (int i = 0; i < idr.FieldCount; ++i)
							{
								string strName = idr.GetName(i);
								object objVal = idr.GetValue(i);

								System.Reflection.MemberInfo mi = GetMemberInfo(t, strName);
								SetMemberValue(tThisClassInstance, mi, objVal);

								/*
                                System.Reflection.FieldInfo fi = t.GetField(strName, m_CaseSensitivity);
                                if (fi != null)
                                    fi.SetValue(tThisClassInstance, MyChangeType(objVal, fi.FieldType));
                                else
                                {
                                    System.Reflection.PropertyInfo pi = t.GetProperty(strName, m_CaseSensitivity);
                                    if (pi != null)
                                        pi.SetValue(tThisClassInstance, MyChangeType(objVal, pi.PropertyType), null);

                                } // End else of if (fi != null)
                                */
							} // Next i

							break;
						} // Whend

						idr.Close();

						if (bHasNoRows)
							tThisClassInstance = default(T);

					} // End Lock idr

				} // End Using idr

			} // End lock cmd

			return tThisClassInstance;
		} // End Function GetClass


		public static T GetClass<T>(System.Data.IDbCommand cmd)
		{
			T tThisClassInstance = System.Activator.CreateInstance<T>();
			return GetClass<T>(cmd, tThisClassInstance);
		}


		public static T GetClass<T>(string strSQL)
		{
			T tReturnClassInstance = default(T);

			using (System.Data.IDbCommand cmd = CreateCommand(strSQL))
			{
				tReturnClassInstance = GetClass<T>(cmd);
			} // End Using cmd

			return tReturnClassInstance;
		} // End Function GetClass









		private static System.Data.Common.DbProviderFactory GetFactory<T>()
        {
            System.Type t = typeof(T);
            return GetFactory(t);
        } // End Function GetFactory


		private static System.Data.Common.DbProviderFactory GetFactory(string assemblyType)
        {
#if TARGET_JVM // case insensitive GetType is not supported
			System.Type type = System.Type.GetType (assemblyType, false);
#else
            System.Type type = System.Type.GetType(assemblyType, false, true);
#endif

            return GetFactory(type);
        } // End Function GetFactory


		private static System.Data.Common.DbProviderFactory GetFactory(System.Type type)
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


		private static System.Data.Common.DbProviderFactory GetFactory()
        {
            return GetFactory(typeof(Npgsql.NpgsqlFactory));
            // return GetFactory(typeof(MySql.Data.MySqlClient.MySqlClientFactory));
            // return GetFactory(typeof(System.Data.SqlClient.SqlClientFactory));
        }

		private static System.Data.Common.DbProviderFactory factory = GetFactory();

		public static System.Data.IDbCommand CreateCommand(string strSQL)
		{
			System.Data.IDbCommand cmd = factory.CreateCommand ();
			cmd.CommandText = strSQL;

			return cmd;
		}



		private static string GetPgConnectionString()
        {
            Npgsql.NpgsqlConnectionStringBuilder csb = new Npgsql.NpgsqlConnectionStringBuilder();
            csb.Host = "127.0.0.1";
            csb.Database = "testdb";

            csb.UserName = "pgfuse";
            csb.Password = "p";

            // PORT=5432;TIMEOUT=15;POOLING=True;MINPOOLSIZE=1;MAXPOOLSIZE=20;COMMANDTIMEOUT=20;COMPATIBLE=2.2.0.0;HOST=127.0.0.1;DATABASE=testdb;USER ID=postgres;PASSWORD=Inspiron1300
            return csb.ConnectionString;
        }


		private static string GetMySQLConnectionString()
        {
            MySql.Data.MySqlClient.MySqlConnectionStringBuilder csb = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();
            csb.Server = "127.0.0.1";
            csb.Database = "testdb";

            csb.UserID = "root";
            csb.Password = "Inspiron1300";

            return csb.ConnectionString;
        }


		private static string GetMSSQLConnectionString()
        {
            System.Data.SqlClient.SqlConnectionStringBuilder csb =
                new System.Data.SqlClient.SqlConnectionStringBuilder();
            csb.DataSource = "127.0.0.1";
            csb.InitialCatalog = "testdb";

            csb.UserID = "root";
            csb.Password = "Inspiron1300";

            return csb.ConnectionString;
        }


		private static System.Data.Common.DbConnection GetConnection()
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
