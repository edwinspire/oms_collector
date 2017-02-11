using System;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Threading;
using OpenMonitoringSystem; 
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;


namespace OpenMonitoringSystem
{

	public class DevUpTime:BaseReport
	{
		
		public TimeSpan LastUpTime = TimeSpan.FromHours(0);

		public DevUpTime(){
			this.Name = "DevUpTime";
			this.Interval = 45;
		}

		public override NameValueCollection Execute ()
		{
			NameValueCollection r = new NameValueCollection();

			if(this.UpTime.Hours != this.LastUpTime.Hours){
				this.LastUpTime = this.UpTime;
				this.SendReport = true;
				r ["uptime"] = this.LastUpTime.Hours.ToString();
			}else{
				this.SendReport = false;
			}

	this.LastExecReturn = 0; // Sin novedad
	return r;
}

		public static TimeSpan TickCount(){
			int result = Environment.TickCount & Int32.MaxValue;
			return  TimeSpan.FromMilliseconds(result);
		}

		public static TimeSpan SystemUpTime(){
			using (var uptime = new PerformanceCounter("System", "System Up Time")) {
				uptime.NextValue();       //Call this an extra time before reading its value
				return TimeSpan.FromSeconds(uptime.NextValue());
			}		
		}

		public static TimeSpan TickCountOrSystemUpTime(){

			if(TickCount() > SystemUpTime()){
				return TickCount ();

			}else{
				return SystemUpTime ();

			}
		}


		public TimeSpan UpTime {
			get {
				return TickCountOrSystemUpTime ();
			}
		}







}

}

