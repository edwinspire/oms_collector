using System;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Threading;
using OpenMonitoringSystem; 
using System.Collections.Specialized;
using System.Data.SqlClient;

namespace OpenMonitoringSystem
{


	public class SQLServer:BaseReport
	{

		public string DataConexion = "Data Source=localhost;"+"Initial Catalog=UseDataBase;Integrated Security=true;";

		public SQLServer(){
			this.Name = "SQLServerJobs";
			//this.Table = "events_ping";
		}


		public override NameValueCollection Execute ()
		{

			NameValueCollection r = new NameValueCollection();
			r ["Status"] = "Unknown";
			//r ["Error"] = "-100";
			using (SqlConnection con = new SqlConnection(this.DataConexion))
			{
				con.Open();
				this.Jobs(con);

				//Bloque de instrucciones sobre la base de datos
			}


			return r;
		}

		public string Jobs(SqlConnection con){

			var textoCmd = "SELECT Nombre,Apellido FROM HUESPED;";

			SqlCommand cmd = new SqlCommand(textoCmd,con);

			SqlDataReader reader = cmd.ExecuteReader();
			try
			{
				while (reader.Read())
				{
					Console.WriteLine(String.Format(" {0},{1}",
					                                reader[0], reader[1]));
				}
			}
			catch (SqlException e)
			{
				Console.WriteLine(e.Message);
			}
			reader.Close();

			return "[]";
		}




	}

}

