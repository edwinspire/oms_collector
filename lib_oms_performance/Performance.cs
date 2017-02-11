using System;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Threading;
using OpenMonitoringSystem; 
using System.Collections.Specialized;
using System.Diagnostics;


namespace OpenMonitoringSystem
{

	public class Performance:BaseReport
	{

		private PerformanceCounter theCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"); 
		
		public int MaxPercentage = 95;
		
		public bool ReportOnlyIfMoreThanMaxPercentage = true;
		private int LastPercentage = 0; 
	
		public Performance(){
			this.Name = "Performance";
			theCPUCounter.NextValue ();
			this.Interval = 2;
		}


		public override NameValueCollection Execute ()
		{

			NameValueCollection r = new NameValueCollection();
			var process = theCPUCounter.NextValue ();
			r ["Status"] = "Unknown";
			r ["Processor"] = process.ToString();
			//this.theCPUCounter.NextValue();
			//Console.WriteLine("NextValue() = " + theCPUCounter.NextValue().ToString());
			if(process >= this.MaxPercentage && process >= this.LastPercentage){
				this.SendReport = true;
				r ["Status"] = "Warning";
			}else{
				r ["Status"] = "OK";
				this.SendReport = false;
			}

			if(!this.ReportOnlyIfMoreThanMaxPercentage){
				this.SendReport = true;
			}

			return r;
		}




	}

}

