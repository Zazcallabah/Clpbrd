	using System;
	using System.Threading;
namespace Clpbrd
{
class Clipboard : StaHelper
{
    readonly string _data;

    public Clipboard( string data )
    {
        _data = data;
    }

    protected override void Work()
    {
        System.Windows.Clipboard.SetText( _data );
    }
}
	public abstract class StaHelper
{
    readonly ManualResetEvent _complete = new ManualResetEvent( false );    

    public void Go()
    {
        var thread = new Thread( new ThreadStart( DoWork ) )
        {
            IsBackground = true,
        };
        thread.SetApartmentState( ApartmentState.STA );
        thread.Start();
    }

    private void DoWork()
    {
        try
        {
            _complete.Reset();
            Work();
        }
        catch( Exception ex )
        {
                try
                {
                    Thread.Sleep( 1000 );
                    Work();
                }
                catch
                {
                }
        }
        finally
        {
            _complete.Set();
        }
    }

    protected abstract void Work();
}
}
