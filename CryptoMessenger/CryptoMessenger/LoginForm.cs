using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CryptoMessenger
{
    public partial class LoginForm : Form
    {
        // parameters
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int CS_DBLCLKS = 0x8;
        private const int CS_DROPSHADOW = 0x20000;
        protected override CreateParams CreateParams
        {
            get
            {  
                CreateParams cp = base.CreateParams;
                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        // for moving form
        bool moveForm;
        int deltaX;
        int deltaY;
        
        // font
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
           IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        private PrivateFontCollection fonts = new PrivateFontCollection();
        private Font RobotoLightFont12;
        private Font RobotoLightFont9;

        // for textbox
        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, 
            [MarshalAs(UnmanagedType.LPWStr)]string lParam);


        public LoginForm()
        {
            InitializeComponent();

            // creat font
            byte[] fontData = Properties.Resources.Roboto_Light;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, Properties.Resources.Roboto_Light.Length);
            AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.Roboto_Light.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);

            RobotoLightFont12 = new Font(fonts.Families[0], 12.0F);
            RobotoLightFont9 = new Font(fonts.Families[0], 9.0F);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.label1.Font = RobotoLightFont12;
            this.button3.Font = RobotoLightFont12;
            this.button4.Font = RobotoLightFont12;
            this.textBox1.Font = RobotoLightFont12;
            this.textBox2.Font = RobotoLightFont12;
            this.label2.Font = RobotoLightFont9;
            this.label3.Font = RobotoLightFont9;
            this.label4.Font = RobotoLightFont9;
            this.checkBox1.Font = RobotoLightFont9;

            this.ActiveControl = button3;

            SendMessage(textBox1.Handle, EM_SETCUEBANNER, 0, "Имя");
            SendMessage(textBox2.Handle, EM_SETCUEBANNER, 0, "Пароль");
            this.textBox2.PasswordChar = '*';
        }

        // moving form
        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            moveForm = true;
            deltaX = e.X;
            deltaY = e.Y;
        }
        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            moveForm = false;
        }    
        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveForm)
                SetDesktopLocation(MousePosition.X - deltaX - label1.Location.X, MousePosition.Y - deltaY - label1.Location.Y);
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            moveForm = true;
            deltaX = e.X;
            deltaY = e.Y;
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            moveForm = false;
        }
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveForm)
                SetDesktopLocation(MousePosition.X - deltaX, MousePosition.Y - deltaY);
        }


        // close
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // minimize
        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }


        // hover close button
        private void button2_MouseEnter(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            b.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button2.Image = CryptoMessenger.Properties.Resources.close_button_hover;
        }
        // un-hover close button
        private void button2_MouseLeave(object sender, EventArgs e)
        {
            this.button2.Image = CryptoMessenger.Properties.Resources.close_button_normal;
        }
        // press close button
        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            b.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button2.Image = CryptoMessenger.Properties.Resources.close_button_pressed;
        }

        // hover minimize button
        private void button1_MouseEnter(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            b.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button1.Image = CryptoMessenger.Properties.Resources.minimize_button_hover;
        }
        // un-hover minimize button
        private void button1_MouseLeave(object sender, EventArgs e)
        {
            this.button1.Image = CryptoMessenger.Properties.Resources.minimize_button_normal;
        }
        // press minimize button
        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            b.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button1.Image = CryptoMessenger.Properties.Resources.minimize_button_pressed;
        }

        // hover login button
        private void button3_MouseEnter(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            b.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button3.BackgroundImage = CryptoMessenger.Properties.Resources.login_button_hover;
        }
        // un-hover login button
        private void button3_MouseLeave(object sender, EventArgs e)
        {
            this.button3.BackgroundImage = CryptoMessenger.Properties.Resources.login_button_normal;
        }
        // press login button
        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            b.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button3.BackgroundImage = CryptoMessenger.Properties.Resources.login_button_pressed;
        }
        // release login button
        private void button3_MouseUp(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            b.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button3.BackgroundImage = CryptoMessenger.Properties.Resources.login_button_hover;
        }

        // hover register button
        private void button4_MouseEnter(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            b.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button4.BackgroundImage = CryptoMessenger.Properties.Resources.register_button_hover;
        }
        // un-hover register button
        private void button4_MouseLeave(object sender, EventArgs e)
        {
            this.button4.BackgroundImage = CryptoMessenger.Properties.Resources.register_button_normal;
        }
        // press register button
        private void button4_MouseDown(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            b.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button4.BackgroundImage = CryptoMessenger.Properties.Resources.register_button_pressed;
        }
        // release register button
        private void button4_MouseUp(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            b.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button4.BackgroundImage = CryptoMessenger.Properties.Resources.register_button_hover;
        }


        // login
        private void button3_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Equals(""))
            {
                this.panel2.BackColor = Color.Transparent;
                this.panel2.BackgroundImage = global::CryptoMessenger.Properties.Resources.warninig;
                this.label3.ForeColor = System.Drawing.Color.Red;
                this.label3.Text = "Имя не указано";
            }
            if (this.textBox2.Text.Equals(""))
            {
                this.panel3.BackColor = Color.Transparent;
                this.panel3.BackgroundImage = global::CryptoMessenger.Properties.Resources.warninig;
                this.label4.ForeColor = System.Drawing.Color.Red;
                this.label4.Text = "Пароль не указан";
            }
        }

        // register
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Equals(""))
            {
                this.panel2.BackColor = Color.Transparent;
                this.panel2.BackgroundImage = global::CryptoMessenger.Properties.Resources.warninig;
                this.label3.ForeColor = System.Drawing.Color.Red;
                this.label3.Text = "Имя не указано";
            }
            if (this.textBox2.Text.Equals(""))
            {
                this.panel3.BackColor = Color.Transparent;
                this.panel3.BackgroundImage = global::CryptoMessenger.Properties.Resources.warninig;
                this.label4.ForeColor = System.Drawing.Color.Red;
                this.label4.Text = "Пароль не указан";
            }
        }


        // delete warnings
        private void textBox1_Enter(object sender, EventArgs e)
        {
            this.panel2.BackColor = Color.White;
            this.panel2.BackgroundImage = null;
            this.label3.Text = null;
        }

        // delete warnings
        private void textBox2_Enter(object sender, EventArgs e)
        {
            this.panel3.BackColor = Color.White;
            this.panel3.BackgroundImage = null;
            this.label4.Text = null;
        }

        // show/hide password
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                this.textBox2.PasswordChar = '*';
            else
                this.textBox2.PasswordChar = '\0';
        }
    }
}
