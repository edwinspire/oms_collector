using System;
using OpenMonitoringSystem;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Threading;
using System.Diagnostics;



namespace OpenMonitoringSystem
{


	public class MainClass
	{
		public static void Main (string[] args)
		{

			Console.WriteLine ("**********************************");
			Console.WriteLine ("* OPEN MONITORING SYSTEMS CLIENT *");
			Console.WriteLine ("**********************************");


			var os = Environment.OSVersion;
			Console.WriteLine("Current OS Information:\n");
			Console.WriteLine("Platform: {0:G}", os.Platform);
			Console.WriteLine("Version String: {0}", os.VersionString);
			Console.WriteLine("Version Information:");
			Console.WriteLine("   Major: {0}", os.Version.Major);
			Console.WriteLine("   Minor: {0}", os.Version.Minor);
			Console.WriteLine("Service Pack: '{0}'", os.ServicePack);


			Console.WriteLine ("Cargando Plugins...");
			//var ListPlugins = new ArrayList();
			var c = new Client ();
			c = (Client)c.Deserialize ();
			c.LoadPlugins ();

			if(DevUpTime.TickCountOrSystemUpTime().TotalMinutes < 2){
				Console.WriteLine ("Reportando encendido de dispositivo");
				Thread.Sleep (30000);
				c.Send ();
			}

	
			Console.WriteLine("Iniciando procesos");

			c.Listen ();

			while(true){

				c = (Client)c.Deserialize ();

				if(c.Pause > 0){
					Thread.Sleep (c.Pause*1000);
					c.Pause = 0;
				}

				if(c.ReloadConfigFiles){

					c.LoadPlugins ();
					c.ReloadConfigFiles = false;
				}

				c.Serialize();

				foreach(BaseReport plug in c.ListPlugins){

					if(plug.LastExec.AddMinutes(plug.Interval) < DateTime.Now && plug.Enabled){
						plug.Send ();
					}

				}
			//	Console.WriteLine (" <<<<<<<<< Esperamos 30 segundos para la siguiente ronda >>>>>>>>>");
				Thread.Sleep (c.Interval*30000);
			}


		}


		public class Client:BaseReport{

			
			public string Server = "192.168.238.66";
			
			public int Pause = 0; //En segundos
			
			public bool ReloadConfigFiles = false;
			
			public bool UniqueConfig = true;
			
			public int ListenPort = 54321;
			
			public ArrayList ListPlugins = new ArrayList();


			public Client(){
				this.Name = "Client";
				this.Interval = 1;

			}


			public void Listen(){

				//Creamos el delegado 
				ThreadStart delegado = new ThreadStart(this.HttpServer); 
				//Creamos la instancia del hilo 
				Thread hilo = new Thread(delegado); 
				//Iniciamos el hilo 
				hilo.Start(); 
			}

			public void HttpServer(){
			
				HttpListener listener = new HttpListener();
				listener.Prefixes.Add("http://*:"+this.ListenPort.ToString()+"/");

				listener.Start();

				while(true){

					IAsyncResult result = listener.BeginGetContext(new AsyncCallback(this.ListenerCallback),listener);
					Console.WriteLine("Waiting for request to be processed asyncronously.");
					result.AsyncWaitHandle.WaitOne();
					Console.WriteLine("Request processed asyncronously.");
				}
			
			}

			public void ListenerCallback(IAsyncResult result)
			{

				Console.WriteLine ("ha ingresado una nueva solicitud");
				HttpListener listener = (HttpListener) result.AsyncState;
				// Call EndGetContext to complete the asynchronous operation.
				HttpListenerContext context = listener.EndGetContext(result);
				HttpListenerRequest request = context.Request;

				foreach(string key in request.QueryString ){
				//	Console.WriteLine(key+" => "+request.QueryString[key]);
					if(key == "RunPlugin"){

						foreach(BaseReport plug in this.ListPlugins){
							if (plug.NamePlugin() == request.QueryString[key]) {
								plug.Send (true);
								Console.WriteLine ("* Ha solicitado carga remota del plugin "+plug.NamePlugin());
							} else {
								Console.WriteLine ("No ha solicitado carga remota del plugin "+plug.NamePlugin());
							}
						}

					}

				}
				// Obtain a response object.
				HttpListenerResponse response = context.Response;
				// Construct a response.
				string responseString = "<HTML><BODY> Open Monitoring System!</BODY></HTML>";
				byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
				// Get a response stream and write the response to it.
				response.ContentLength64 = buffer.Length;
				System.IO.Stream output = response.OutputStream;
				output.Write(buffer,0,buffer.Length);
				// You must close the output stream.
				output.Close();

			}

			public override NameValueCollection Execute ()
			{
				NameValueCollection r = new NameValueCollection();
				r ["Status"] = "OK";
				this.LastExecReturn = 0;
				return r;
			}

			public void LoadPlugins(){
			

				//var ListPlugins = new ArrayList();
				this.ListPlugins.Clear ();

				var deletefile = new DeleteFiles ();
				deletefile = (DeleteFiles)deletefile.Deserialize ();
				this.ListPlugins.Add (deletefile);

				var ping = new Ping ();
				ping = (Ping)ping.Deserialize ();
				this.ListPlugins.Add (ping);

				var uptime = new DevUpTime ();
				uptime = (DevUpTime)uptime.Deserialize ();
				this.ListPlugins.Add (uptime);

				var dinfo = new DriveInfoSize ();
				dinfo = (DriveInfoSize)dinfo.Deserialize ();
				this.ListPlugins.Add (dinfo);

				var perf = new Performance ();
				perf = (Performance)perf.Deserialize ();
				this.ListPlugins.Add (perf);


				var sqlserver = new SQLServer ();
				sqlserver = (SQLServer)sqlserver.Deserialize ();
				this.ListPlugins.Add (sqlserver);



				if(this.UniqueConfig){

					foreach(BaseReport plug in this.ListPlugins){
						plug.Receiver = this.Receiver;
						plug.ReportValidator = this.ReportValidator;
						plug.IdEquipment = this.IdEquipment;
					}

				}


			}


		}





	}












}
