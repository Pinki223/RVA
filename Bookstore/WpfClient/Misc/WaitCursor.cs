using System;
using System.Windows.Input;

namespace WpfClient.Classes
{
	class WaitCursor : IDisposable
	{
		public WaitCursor()
		{
			Mouse.OverrideCursor = Cursors.Wait;
		}

		public void Dispose()
		{
			Mouse.OverrideCursor = Cursors.Arrow;
		}
	}
}
