using System;
using System.IO;
using System.Text;
using  System.Collections.Specialized;
using System.Xml.Serialization;
using Npgsql;

namespace OpenMonitoringSystem
{

	public class EventReport
	{
		public static void Main ()
		{

			var o = new Octopus ();
			o = (Octopus)o.Deserialize ();
			o.Run();

		}
	}



	public class Octopus:BaseReport
	{
		public string pgServer = "127.0.0.1";
		public string pgUserId = "user";
		public string pgPassword = "pwd";
		public string pgDatabase = "oms";
		public override NameValueCollection Execute(){
			return new NameValueCollection ();
		}

		public Octopus(){
			this.Name = "Octopus";
					}


		public void Run()
		{
			// Connect to a PostgreSQL database
			NpgsqlConnection conn = new NpgsqlConnection("Server="+this.pgServer+";User Id="+this.pgUserId+"; Password="+this.pgPassword+";Database="+this.pgDatabase+";");
			conn.Open();

			// Define a query returning a single row result set
			NpgsqlCommand command = new NpgsqlCommand("SELECT idequipment, host(ip)::TEXT, report_validator, username, pwd FROM view_account_network_devices WHERE enabled = true AND monitored = true AND ip is not null;", conn);

			// Execute the query and obtain a result set
			NpgsqlDataReader dr = command.ExecuteReader();

			var ping = new OpenMonitoringSystem.Ping ();

			while (dr.Read()) {
			//	Console.Write("{0}\t{1} \n", dr[0], dr.GetString(1));
				// PING 
				ping.IdEquipment = (long)dr [0];
				ping.ToIP = (string)dr[1];
				ping.ReportValidator = (string)dr[2];
				ping.Send ();
			}

			conn.Close();
		}
	}


}