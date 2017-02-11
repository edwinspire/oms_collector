using System;
using OpenMonitoringSystem;
using System.Collections.Specialized;


namespace OpenMonitoringSystem
{
	public class MainClass
	{
		public static void Main (string[] args)
		{


			var report = new StartMachine ();
			report = (StartMachine)report.Deserialize ();
			report.Send ();

		}


		public class StartMachine:BaseReport
		{

			public StartMachine(){
				this.Name = "StartMachine";
				//this.Table = "events_ping";
			}


			public override NameValueCollection Execute ()
			{
				NameValueCollection r = new NameValueCollection();
				r ["Status"] = "OK";
				return r;
			}


		}



	}







}
