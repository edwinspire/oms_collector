using System;
using System.IO;
using System.Text;
using OpenMonitoringSystem; 
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;


namespace OpenMonitoringSystem
{


	public class DriveInfoSize:BaseReport
	{

		
		public int MinPercentage = 10;
		public bool ReportOnlyIfLessThanMinPercentage = true;
		public int ReportPercentageChange = 10;
		protected Dictionary<string, long> LastAvailableFreeSpace = new Dictionary<string, long>();

		public DriveInfoSize(){
			this.Name = "DriveInfoSize";
		}


		public override NameValueCollection Execute ()
		{

			var jsondriver = new StringBuilder ();
			NameValueCollection r = new NameValueCollection();
			r ["Status"] = "OK";
			//r ["Error"] = "-100";
			DriveInfo[] allDrives = DriveInfo.GetDrives();
			var drivers = new List<string>();
			var Percentaje = 199.99;
			var LastPercentaje = 0.0;
			var PercentageChange = 0.0;
			this.SendReport = false;

			foreach (DriveInfo d in allDrives)
			{

				if (d.IsReady == true)
				{

					Percentaje = (100 * d.AvailableFreeSpace / d.TotalSize);

					if (this.LastAvailableFreeSpace.ContainsKey (d.Name)) {
						LastPercentaje = (100 * this.LastAvailableFreeSpace[d.Name] / d.TotalSize);
					} else {
						LastPercentaje = 0;
					}

					PercentageChange = Math.Abs(Percentaje - LastPercentaje);

					jsondriver.Append ("{\"Driver\": ");
					jsondriver.AppendFormat ("\"{0}\", \"Type\": \"{1}\", \"Label\": \"{2}\", \"Format\": \"{3}\", \"Free\": \"{4}\", \"TotalSize\": \"{5}\"", d.Name, d.DriveType, d.VolumeLabel, d.DriveFormat, (d.AvailableFreeSpace/1000000).ToString(), (d.TotalSize/1000000).ToString());
					jsondriver.Append ("}");
					drivers.Add (jsondriver.ToString());
					jsondriver.Clear ();
					this.LastAvailableFreeSpace[d.Name] = d.AvailableFreeSpace;

					if(Percentaje <= this.MinPercentage || PercentageChange >= this.ReportPercentageChange){
						this.SendReport = true;
					}

				}
			}

			jsondriver.Clear ();
			jsondriver.AppendFormat ("[{0}]", String.Join(",", drivers));
			r ["Drivers"] = jsondriver.ToString ();

			return r;
		}




	}

}

