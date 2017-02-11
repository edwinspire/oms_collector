using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;


namespace OpenMonitoringSystem
{


	public abstract class BaseReport{

		protected string Name = "Plugin";
		
		public string Receiver = "http://192.168.251.174/receiver_web.njs";
		private string config_file = "";
		
		public string ReportValidator = "*";
		
		public Int64 IdEquipment = -1;
		
		public int Interval = 30; //En minutos
		
		public DateTime LastExec = new DateTime();
		
		public int LastExecReturn = 999;
		
		public bool Reported = false;
		
		public bool Enabled = true;

		protected bool SendReport = true;

		public abstract NameValueCollection Execute();

		public string NamePlugin(){
			return this.Name;
		}

		public  void _Send(bool force = false){
		
			Console.WriteLine ("Iniciando proceso "+this.Name);
			var d = this.Execute ();

			if (this.SendReport || force) {

				Console.WriteLine ("Enviando reporte de "+this.Name);
				using (var  client =  new   WebClient ()) {

					if (this.Name.Length > 0 && this.Receiver.Length > 0 && this.ReportValidator.Length > 1 && this.IdEquipment > 0) {


						d.Add ("DeviceTime", DateTime.UtcNow.ToString ());
						d.Add ("PluginName", this.Name);
						d.Add ("IPDevice", GetLocalIPAddress ());
						d.Add ("report_validator", this.ReportValidator);
						d.Add ("idequipment", this.IdEquipment.ToString ());
						d.Add ("LastExecReturn", this.LastExecReturn.ToString ());

						try {
							var response = client.UploadValues (this.Receiver, d);

							var responseString = Encoding.Default.GetString (response);
							Console.WriteLine (responseString);
							this.Reported = true;

						} catch (System.Net.WebException e) {

							Console.WriteLine (e.Message);

							Console.WriteLine ("No se pudo conectar con " + this.Receiver);
							Console.WriteLine ("Aqui esta pendiente crear una base de datos local que permita guardar el evento que no se pudo enviar");

						}

		

					} else {
						Console.WriteLine ("Algun dato de conexion con el servidor no esta lleno");
					}

				}
			} else {
				Console.WriteLine ("No se considero necesario enviar el reporte de "+this.Name);
			}

			this.LastExec = DateTime.Now;
			this.Serialize ();

		//	return true;
		}

		public void Send(bool force = false){
		
			Console.WriteLine ("Iniciando proceso "+this.Name);
			//Creamos el delegado 
		//	ThreadStart delegado = new ThreadStart(); 
			//Creamos la instancia del hilo 
			Thread hilo = new Thread(() => this._Send(force)); 
			//Iniciamos el hilo 
			hilo.Start(); 
		}

		public static string Base64Encode(string plainText) {
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		public static string Base64Decode(string base64EncodedData) {
			var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}

		public static string GetLocalIPAddress()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					return ip.ToString();
				}
			}
			//throw new Exception("Local IP Address Not Found!");
			return "127.0.0.1";
		}

		public void Serialize(Boolean replace = true){
			this.get_config_path () ;

			//var config_name = "plugin_"+this.Name + ".xml";
			if (!File.Exists (this.config_file)) {
				save_serialized ();
			} else if(replace){
				save_serialized ();
			}

		}

		private void save_serialized(){
			File.Delete(this.config_file);
			using (FileStream fs = new FileStream(this.config_file, FileMode.CreateNew)) {        
				XmlSerializer serializer = new XmlSerializer (this.GetType ());
				serializer.Serialize (fs, this);	
			}
		}

		private void get_config_path(){
			this.config_file = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "plugin_"+this.Name + ".xml");
		}

		public object Deserialize(){
			this.get_config_path ();
		
			if (!File.Exists (this.config_file)) {
				Serialize ();
				return _deserialize ();

			} else {

				return _deserialize ();

			}

		}

		public object _deserialize(){
		
			using (FileStream fs = File.OpenRead(this.config_file)) {        
				XmlSerializer serializer = new XmlSerializer(this.GetType());
				return serializer.Deserialize(fs);	
			}

		}






	}





}

