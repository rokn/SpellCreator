using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace MouldSpellCreator
{
	public partial class NotificationWindow
	{

		public NotificationWindow(string header, string description)
		{
			InitializeComponent();

			Header.Text = header;
			Description.Text = description;

			Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
			{
				var workingArea = Screen.PrimaryScreen.WorkingArea;
				var presentationSource = PresentationSource.FromVisual(this);

				if (presentationSource?.CompositionTarget == null) return;

				var transform = presentationSource.CompositionTarget.TransformFromDevice;
				var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));

				Left = corner.X - ActualWidth - 100;
				Top = corner.Y - ActualHeight;
			}));
		}

		private void OnDisapear(object sender, EventArgs e)
		{
			Close();
		}
	}
}