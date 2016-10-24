using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Hardcodet.Wpf.TaskbarNotification;

namespace Clpbrd
{
	class Controller : IDisposable, INotifyPropertyChanged
	{
		readonly TaskbarIcon _icon;
		SimpleHTTPServer server;

		public Controller(TaskbarIcon icon)
		{
			_icon = icon;
			server = new SimpleHTTPServer();
		}

		public void Dispose()
		{
			server.Stop();
		}

		void FirePropChanged(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}


	public class SimpleHTTPServer
	{
		private Thread _serverThread;
		private HttpListener _listener;
		bool IsActive = true;

		public SimpleHTTPServer()
		{
			_serverThread = new Thread(this.Listen);
			_serverThread.Start();
		}

		public void Stop()
		{
			IsActive = false;
			_serverThread.Abort();
			_listener.Stop();
		}

		private static void WriteResponse(Stream stream)
		{
			Write(stream, "HTTP/1.0 200 OK\r\n");
			Write(stream, string.Join("\r\n", new[] { "Content-Type: text/html" }));
			Write(stream, "\r\n\r\n");
		}


		private static string Readline(Stream stream)
		{
			int next_char;
			string data = "";
			while (true)
			{
				next_char = stream.ReadByte();
				if (next_char == '\n') { break; }
				if (next_char == '\r') { continue; }
				if (next_char == -1)
				{
					Thread.Sleep(1); continue;
				};
				data += Convert.ToChar(next_char);
			}
			return data;
		}

		private static void Write(Stream stream, string text)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			stream.Write(bytes, 0, bytes.Length);
		}


		private void Listen()
		{


			System.Net.Sockets.TcpListener s = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, 18333);
			s.Start();

			while (IsActive)
			{
				try
				{


					using (System.Net.Sockets.TcpClient c = s.AcceptTcpClient())
					{

						Stream inputStream = c.GetStream();
						Stream outputStream = c.GetStream();
						int length = 0;
						string line;
						while ((line = Readline(inputStream)) != null)
						{
							if (line.Equals(""))
							{
								break;
							}
							if (line.StartsWith("Content-Length"))
							{
								length = Math.Max(Int32.Parse(line.Substring("Content-Length: ".Length)), 0);
							}
						}

						var buffer = new byte[Math.Min(1024 * 1024, length)];

						int n = inputStream.Read(buffer, 0, buffer.Length);

						var content = Encoding.Default.GetString(buffer);
						WriteResponse(outputStream);

						outputStream.Flush();
						outputStream.Close();
						outputStream = null;

						inputStream.Close();
						inputStream = null;


						new Clipboard(content).Go();

					}

				}
				catch (Exception ex)
				{

				}
				Thread.Sleep(1);
			}

		}
	}
}

