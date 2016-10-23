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

		public Controller( TaskbarIcon icon )
		{
			_icon = icon;
			server =new SimpleHTTPServer();
		}

		public void Dispose()
		{
		server.Stop();
		}

		void FirePropChanged( string property )
		{
			if( PropertyChanged != null )
				PropertyChanged( this, new PropertyChangedEventArgs( property ) );
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public ImageSource CurrentIconState
		{
			get
			{
				return new BitmapImage( new Uri( @"pack://application:,,,/IrcNotify;component/Resources/chat.ico" ) );
			}
		}
	}

	
	public class SimpleHTTPServer
	{
		private Thread _serverThread;
		private HttpListener _listener;

		public SimpleHTTPServer()
		{
			_serverThread = new Thread(this.Listen);
			_serverThread.Start();
		}
 
		public void Stop()
		{
			_serverThread.Abort();
			_listener.Stop();
		}
 
		private void Listen()
		{
			_listener = new HttpListener();
			_listener.Prefixes.Add("http://+:18222/");


			_listener.Start();
			while (true)
			{
				try
				{
					HttpListenerContext context = _listener.GetContext();
					var str = new StreamReader(context.Request.InputStream,Encoding.Default).ReadToEnd();
					new  Clipboard(str).Go();
					 context.Response.StatusCode = 200;
					    context.Response.KeepAlive = false;
						    context.Response.Close();
				}
				catch (Exception ex)
				{
 
				}
			}
		}
	}
}

