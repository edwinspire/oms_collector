using System;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Threading;
using OpenMonitoringSystem; 
using System.Collections.Specialized;

namespace OpenMonitoringSystem
{


	public class Ping:BaseReport
	{
		
		public string ToIP = "127.0.0.1";
		
		public int MaxTime = 800;
		
		public bool ReportOnlyExceedsMaxTime = true; // Reporta el ping solo si sobrepasa de valor de Maxtime

		public Ping(){
			this.Name = "Ping";
			//this.Table = "events_ping";
		}


		public override NameValueCollection Execute ()
		{

			NameValueCollection r = new NameValueCollection();
			r ["Status"] = "Unknown";
			//r ["Error"] = "-100";
			r ["ToIP"] = this.ToIP;
			r ["MaxTime"] = this.MaxTime.ToString();
			r ["roundtriptime"] = "-100";
			//r ["IdEquipment"] = this.IdEquipment.ToString();
			//r ["Identification"] = this.Identification;
			//r ["Table"] = this.Table;
			PingOptions options = new PingOptions ();

			// Use the default Ttl value which is 128, 
			// but change the fragmentation behavior.
			options.DontFragment = false;
			long RoundtripTime = -1; 

			
			var pingSender = new System.Net.NetworkInformation.Ping ();

			try{

				if (ToIP.Length == 0) {
				//	r ["Error"] = "1";
					r ["Status"] = "Error";
					this.LastExecReturn = 4;
				//	throw new ArgumentException ("Ping needs a host or IP Address.");
				} else {

					PingReply reply = pingSender.Send (this.ToIP, 5000);
					r ["Status"] = reply.Status.ToString();
					RoundtripTime = reply.RoundtripTime;
					Console.WriteLine ("Rep: {0}", reply.ToString ());

					r ["Ttl"] = reply.Options.Ttl.ToString();

					if(RoundtripTime > this.MaxTime){

						System.Threading.Thread.Sleep(2000);
						
						reply = pingSender.Send (this.ToIP, 5000);
						r ["Status"] = reply.Status.ToString();
						Console.WriteLine ("Rep 2: {0}", reply.ToString ());
						RoundtripTime = reply.RoundtripTime;
						r ["Ttl"] = reply.Options.Ttl.ToString();
					}

					this.LastExecReturn = 0;
				}

				r ["roundtriptime"] = RoundtripTime.ToString();
				if(!this.ReportOnlyExceedsMaxTime){
					this.SendReport = true;
				}else if(RoundtripTime > this.MaxTime && this.ReportOnlyExceedsMaxTime){
					this.SendReport = true;
				}else{
					this.SendReport = false;
				}

			}catch(System.ArgumentException E){
				//r ["Error"] = "2";
				r ["Status"] = "Error";
				Console.WriteLine (E.ToString());
				this.LastExecReturn = 1;
			}catch(System.Net.NetworkInformation.PingException E){
				//r ["Error"] = "2";
				r ["Status"] = "Error";
				Console.WriteLine (E.ToString());
				this.LastExecReturn = 2;
			}
			catch(System.NullReferenceException E){
				//r ["Error"] = "2";
				r ["Status"] = "Error";
				Console.WriteLine (E.ToString());
				this.LastExecReturn = 3;
			}


		
			return r;
		}




	}

}

