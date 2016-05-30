using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace CryptoMessenger.Behaviors
{
	/// <summary>
	/// Class for window behaviors.
	/// </summary>
	public class WindowBehavior : Behavior<Window>
	{
		public static readonly DependencyProperty ClosingStoryboardProperty =
			DependencyProperty.Register("ClosingStoryboard", typeof(Storyboard), typeof(WindowBehavior), new PropertyMetadata(default(Storyboard)));

		public Storyboard ClosingStoryboard
		{
			get { return (Storyboard)GetValue(ClosingStoryboardProperty); }
			set { SetValue(ClosingStoryboardProperty, value); }
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.Closing += onWindowClosing;
		}

		private void onWindowClosing(object sender, CancelEventArgs e)
		{
			if (ClosingStoryboard == null)
				return;

			e.Cancel = true;
			AssociatedObject.Closing -= onWindowClosing;

			ClosingStoryboard.Completed += (o, a) => AssociatedObject.Close();
			ClosingStoryboard.Begin(AssociatedObject);
		}
	}
}
