using System.Windows.Forms;

namespace CryptoMessenger.GUI
{
	public partial class MySplitContainer : SplitContainer
	{
		public MySplitContainer()
		{
			InitializeComponent();
		}


		// remove cues border
		private bool _painting;
		public override bool Focused
		{
			get { return _painting ? false : base.Focused; }
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			_painting = true;

			try
			{
				base.OnPaint(e);
			}
			finally
			{
				_painting = false;
			}
		}
	}
}
