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
		public static readonly DependencyProperty StoryboardProperty =
			DependencyProperty.Register("Storyboard", typeof(Storyboard), typeof(WindowBehavior), new PropertyMetadata(default(Storyboard)));

		public Storyboard Storyboard
		{
			get { return (Storyboard)GetValue(StoryboardProperty); }
			set { SetValue(StoryboardProperty, value); }
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.Closing += onWindowClosing;
		}

		private void onWindowClosing(object sender, CancelEventArgs e)
		{
			if (Storyboard == null)
				return;

			e.Cancel = true;
			AssociatedObject.Closing -= onWindowClosing;

			Storyboard.Completed += (o, a) => AssociatedObject.Close();
			Storyboard.Begin(AssociatedObject);
		}
	}
}
