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


namespace OpenMonitoringSystem
{


	public class DeleteFiles:BaseReport
	{
		
		public string Directorio = "k:/backup";
		
		public int Dias = 3;
		
		public string Patron = "*.bak";
		
		public bool IncluirFechaModificacion = true;

		public DeleteFiles(){
			this.Name = "DeleteFiles";
			this.Interval = 120;
			this.SendReport = false;
		}

		public override NameValueCollection Execute ()
		{
			NameValueCollection r = new NameValueCollection();
			//var listadriver = new List <string>();
			this.DeleteSelection (this.Directorio);
	
			this.LastExecReturn = 0; // Sin novedad
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


	}

}

