using System.Windows.Forms;

namespace CryptoMessenger.GUI
{
    public partial class MyButton : Button
    {
        protected override bool ShowFocusCues
        {
            get { return false; }
        }

        public MyButton()
        {
            InitializeComponent();
        }

        public override void NotifyDefault(bool value)
        {
            base.NotifyDefault(false);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
			base.OnPaint(pe);
		}
	}
}
