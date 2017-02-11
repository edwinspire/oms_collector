using System;
using OpenMonitoringSystem;
using System.Collections.Specialized;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;


namespace OpenMonitoringSystem
{
	public class MainClass
	{
		public static void Main (string[] args)
		{

			var report = new DeleteBackup ();
			report = (DeleteBackup)report.Deserialize ();
			report.Send ();

		}


		public class DeleteBackup:BaseReport
		{
			public string Directorio = "k:/backup";
			public int Dias = 3;
			public string Patron = "*.bak";
			public bool IncluirFechaModificacion = true;

			public DeleteBackup(){
				this.Name = "DeleteBackup";
				//this.Table = "events_ping";
			}

			public override NameValueCollection Execute ()
			{
				NameValueCollection r = new NameValueCollection();
				var listadriver = new List <string>();
				this.DeleteSelection (this.Directorio);
				var data_drivers = new StringBuilder ();
				DriveInfo[] drives = DriveInfo.GetDrives();

				if(drives.Length > 0){

					
					foreach (DriveInfo d in drives)
					{

						data_drivers.Clear ();
						if (d.IsReady == true) {
							//data_drivers.Append ("{")¬;

							data_drivers.AppendFormat ("Driver:'{0}', Type:'{1}', TotalSize:'{2}', AvailableFreeSpace:'{3}'", d.Name, d.DriveType.ToString (), Base64Encode ((d.TotalSize / 1000000).ToString ()), Base64Encode ((d.AvailableFreeSpace / 1000000).ToString ()));					
							//data_drivers.Append ("}");
							listadriver.Add (data_drivers.ToString ());
						}

					}

					this.LastExecReturn = 0; // Sin novedad
				}else{

					this.LastExecReturn = 1; // No hay drivers en la maquina
				}

				r ["Drivers"] = string.Join("|| ", listadriver.ToArray());


				return r;
			}

			private void DeleteSelection(string dir) 
			{

				Console.WriteLine (GetLocalIPAddress());
				try	
				{
					Console.WriteLine("Busca archivos en el Directorio "+dir+" con el Patron "+this.Patron+" con antiguedad de "+this.Dias.ToString()+" dias.");

					foreach (string d in Directory.GetDirectories(dir)) 
					{
						foreach (string f in Directory.GetFiles(d, this.Patron)) 
						{
							if(File.GetCreationTime(f).AddDays(this.Dias) < DateTime.Now){
								this.Delete(f);
							}else if(this.IncluirFechaModificacion && File.GetLastWriteTime(f).AddDays(this.Dias) < DateTime.Now){
								this.Delete(f);
							}


						}
						this.DeleteSelection(d);
					}
				}
				catch (System.Exception excpt) 
				{
					Console.WriteLine(excpt.Message);
				}
			}

			private void Delete(string file){
				File.Delete(file);
				Console.WriteLine("Borrado "+file);

			}



			/*
			public static string GetMACAddress2()
			{
				NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
				String sMacAddress = string.Empty;
				foreach (NetworkInterface adapter in nics)
				{
					if (sMacAddress == String.Empty)// only return MAC Address from first card  
					{
						//IPInterfaceProperties properties = adapter.GetIPProperties(); Line is not required
						sMacAddress = adapter.GetPhysicalAddress().ToString();
					}
				} return sMacAddress;
			}
			*/


		}



	}







}
