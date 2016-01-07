using System;
using System.Drawing;
using System.Windows.Forms;

namespace CryptoMessenger
{
    partial class LoginForm
    {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
				if (components != null)
					components.Dispose();
				if (shadow != null)
					shadow.Dispose();
				if (fonts != null)
					fonts.Dispose();
				if (NeueFont15 != null)
					NeueFont15.Dispose();
				if (NeueFont10 != null)
					NeueFont10.Dispose();
			}
            base.Dispose(disposing);
        }
		
		/// <summary>
		/// Parameters for minimazing form.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.Style |= 0x20000;  // WS_MINIMIZEBOX
				cp.ClassStyle |= 0x8; // CS_DBLCLKS
				return cp;
			}
		}

		/// <summary>
		/// Change color when form lost and get focus.
		/// </summary>
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x1C) // 0x1C - WM_ACTIVATEAPP
			{
				if (m.WParam != IntPtr.Zero)
				{
					topPanel.BackColor = Properties.Settings.Default.LoginFirstColor;
					loginFormPanelBorderColor = Properties.Settings.Default.LoginFirstColor;
					Refresh();
					if (Visible) shadow.Show();
				}
				else
				{
					topPanel.BackColor = Properties.Settings.Default.LoginSecondColor;
					loginFormPanelBorderColor = Properties.Settings.Default.LoginSecondColor;
					Refresh();
					shadow.Hide();
				}
			}

			base.WndProc(ref m);
		}

		#region Loading form

		private void LoginForm_Load(object sender, EventArgs e)
		{
			appName.Font = NeueFont15;
			loginButton.Font = NeueFont15;
			registerButton.Font = NeueFont15;
			userName.Font = NeueFont15;
			userPassword.Font = NeueFont15;
			incorrectName.Font = NeueFont10;
			incorrectPassword.Font = NeueFont10;
			showPasswordCheckBox.Font = NeueFont10;

			ActiveControl = loginButton;

			NativeMethods.SendMessage(userName.Handle, NativeMethods.EM_SETCUEBANNER, IntPtr.Zero, "ИМЯ");
			NativeMethods.SendMessage(userPassword.Handle, NativeMethods.EM_SETCUEBANNER, IntPtr.Zero, "ПАРОЛЬ");
			userPassword.PasswordChar = '*';
		}

		#endregion

		#region Panels borders

		// panels borders colors
		private Color loginFormPanelBorderColor = Properties.Settings.Default.LoginFirstColor;
		private Color namePanelBorderColor = Properties.Settings.Default.PanelBorderColor;
		private Color passPanelBorderColor = Properties.Settings.Default.PanelBorderColor;
		// paint borders
		private void loginFormPanel_Paint(object sender, PaintEventArgs e)
		{
			Rectangle rect = loginFormPanel.ClientRectangle;
			rect.Width--;
			rect.Height--;
			e.Graphics.DrawRectangle(new Pen(loginFormPanelBorderColor), rect);
		}
		private void userNamePanel_Paint(object sender, PaintEventArgs e)
		{
			Rectangle rect = userNamePanel.ClientRectangle;
			rect.Width--;
			rect.Height--;
			e.Graphics.DrawRectangle(new Pen(namePanelBorderColor), rect);
		}
		private void userPasswordPanel_Paint(object sender, PaintEventArgs e)
		{
			Rectangle rect = userPasswordPanel.ClientRectangle;
			rect.Width--;
			rect.Height--;
			e.Graphics.DrawRectangle(new Pen(passPanelBorderColor), rect);
		}

		#endregion

		#region Move form 
		
		private bool moveForm;
		private int deltaX;
		private int deltaY;

		private void moveForm_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) moveForm = true;
			deltaX = e.X;
			deltaY = e.Y;
		}
		private void moveForm_MouseUp(object sender, MouseEventArgs e)
		{
			moveForm = false;
		}
		private void moveForm_MouseMove(object sender, MouseEventArgs e)
		{
			if (sender is Panel)
			{
				Panel p = (Panel)sender;
				if (moveForm)
					SetDesktopLocation(MousePosition.X - deltaX - p.Location.X, MousePosition.Y - deltaY - p.Location.Y);
			}
			else if (sender is Label)
			{
				Label l = (Label)sender;
				if (moveForm)
					SetDesktopLocation(MousePosition.X - deltaX - l.Location.X, MousePosition.Y - deltaY - l.Location.Y);
			}
			else if (sender is PictureBox)
			{
				PictureBox pb = (PictureBox)sender;
				if (moveForm)
					SetDesktopLocation(MousePosition.X - deltaX - pb.Location.X, MousePosition.Y - deltaY - pb.Location.Y);
			}
		}

		#endregion

		#region Close button action and style
		
		private void closeButton_Click(object sender, EventArgs e)
		{
			Close();
		}
		private void closeButton_MouseEnter(object sender, EventArgs e)
		{
			closeButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
			closeButton.Image = Properties.Resources.close_button_hover;
		}
		private void closeButton_MouseLeave(object sender, EventArgs e)
		{
			closeButton.Image = Properties.Resources.close_button_normal;
		}
		private void closeButton_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				closeButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
				closeButton.Image = Properties.Resources.close_button_pressed;
			}
		}

		#endregion

		#region Minimize button action and style
		
		private void minimizeButton_Click(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Minimized;
		}
		private void minimizeButton_MouseEnter(object sender, EventArgs e)
		{
			minimizeButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
			minimizeButton.Image = Properties.Resources.minimize_button_hover;
		}
		private void minimizeButton_MouseLeave(object sender, EventArgs e)
		{
			minimizeButton.Image = Properties.Resources.minimize_button_normal;
		}
		private void minimize_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				minimizeButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
				minimizeButton.Image = Properties.Resources.minimize_button_pressed;
			}
		}

		#endregion

		#region Focus user ad pass fields

		// delete warning when focus textbox
		private void userName_Enter(object sender, EventArgs e)
		{
			namePanelBorderColor = Properties.Settings.Default.PanelBorderColor;
			userNamePanel.Refresh();

			incorrectName.Text = null;
		}

		// delete warning when focus textbox
		private void userPassword_Enter(object sender, EventArgs e)
		{
			passPanelBorderColor = Properties.Settings.Default.PanelBorderColor;
			userPasswordPanel.Refresh();

			incorrectPassword.Text = null;
		}

		#endregion

		#region Show / Hide password

		// show/hide password
		private void showPasswordCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (showPasswordCheckBox.Checked)
				userPassword.PasswordChar = '*';
			else
				userPassword.PasswordChar = '\0';
		}

		#endregion

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
			this.topPanel = new System.Windows.Forms.Panel();
			this.iconBox = new System.Windows.Forms.PictureBox();
			this.appName = new System.Windows.Forms.Label();
			this.minimizeButton = new CryptoMessenger.MyButton();
			this.closeButton = new CryptoMessenger.MyButton();
			this.incorrectName = new System.Windows.Forms.Label();
			this.incorrectPassword = new System.Windows.Forms.Label();
			this.showPasswordCheckBox = new System.Windows.Forms.CheckBox();
			this.userNamePanel = new System.Windows.Forms.Panel();
			this.userName = new System.Windows.Forms.TextBox();
			this.loginFormPanel = new System.Windows.Forms.Panel();
			this.userPasswordPanel = new System.Windows.Forms.Panel();
			this.userPassword = new System.Windows.Forms.TextBox();
			this.registerButton = new CryptoMessenger.MyButton();
			this.loginButton = new CryptoMessenger.MyButton();
			this.topPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.iconBox)).BeginInit();
			this.userNamePanel.SuspendLayout();
			this.loginFormPanel.SuspendLayout();
			this.userPasswordPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// topPanel
			// 
			this.topPanel.BackColor = global::CryptoMessenger.Properties.Settings.Default.LoginFirstColor;
			this.topPanel.Controls.Add(this.iconBox);
			this.topPanel.Controls.Add(this.appName);
			this.topPanel.Controls.Add(this.minimizeButton);
			this.topPanel.Controls.Add(this.closeButton);
			this.topPanel.Location = new System.Drawing.Point(0, 0);
			this.topPanel.Name = "topPanel";
			this.topPanel.Size = new System.Drawing.Size(300, 35);
			this.topPanel.TabIndex = 4;
			this.topPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseDown);
			this.topPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseMove);
			this.topPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseUp);
			// 
			// iconBox
			// 
			this.iconBox.BackgroundImage = global::CryptoMessenger.Properties.Resources.icon1;
			this.iconBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.iconBox.Location = new System.Drawing.Point(10, 5);
			this.iconBox.Name = "iconBox";
			this.iconBox.Size = new System.Drawing.Size(25, 25);
			this.iconBox.TabIndex = 9;
			this.iconBox.TabStop = false;
			this.iconBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseDown);
			this.iconBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseMove);
			this.iconBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseUp);
			// 
			// appName
			// 
			this.appName.AutoSize = true;
			this.appName.BackColor = System.Drawing.Color.Transparent;
			this.appName.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.appName.ForeColor = System.Drawing.Color.White;
			this.appName.Location = new System.Drawing.Point(40, 5);
			this.appName.Name = "appName";
			this.appName.Size = new System.Drawing.Size(169, 21);
			this.appName.TabIndex = 5;
			this.appName.Text = "CRYPTO MESSENGER";
			this.appName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseDown);
			this.appName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseMove);
			this.appName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseUp);
			// 
			// minimizeButton
			// 
			this.minimizeButton.BackColor = System.Drawing.Color.Transparent;
			this.minimizeButton.FlatAppearance.BorderSize = 0;
			this.minimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.minimizeButton.Image = ((System.Drawing.Image)(resources.GetObject("minimizeButton.Image")));
			this.minimizeButton.Location = new System.Drawing.Point(240, 2);
			this.minimizeButton.Name = "minimizeButton";
			this.minimizeButton.Size = new System.Drawing.Size(20, 31);
			this.minimizeButton.TabIndex = 2;
			this.minimizeButton.TabStop = false;
			this.minimizeButton.UseVisualStyleBackColor = false;
			this.minimizeButton.Click += new System.EventHandler(this.minimizeButton_Click);
			this.minimizeButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.minimize_MouseDown);
			this.minimizeButton.MouseEnter += new System.EventHandler(this.minimizeButton_MouseEnter);
			this.minimizeButton.MouseLeave += new System.EventHandler(this.minimizeButton_MouseLeave);
			// 
			// closeButton
			// 
			this.closeButton.BackColor = System.Drawing.Color.Transparent;
			this.closeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.closeButton.FlatAppearance.BorderSize = 0;
			this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.closeButton.Image = ((System.Drawing.Image)(resources.GetObject("closeButton.Image")));
			this.closeButton.Location = new System.Drawing.Point(270, 2);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(20, 31);
			this.closeButton.TabIndex = 3;
			this.closeButton.TabStop = false;
			this.closeButton.UseVisualStyleBackColor = false;
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			this.closeButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.closeButton_MouseDown);
			this.closeButton.MouseEnter += new System.EventHandler(this.closeButton_MouseEnter);
			this.closeButton.MouseLeave += new System.EventHandler(this.closeButton_MouseLeave);
			// 
			// incorrectName
			// 
			this.incorrectName.AutoSize = true;
			this.incorrectName.BackColor = System.Drawing.Color.Transparent;
			this.incorrectName.Font = new System.Drawing.Font("Roboto Light", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.incorrectName.Location = new System.Drawing.Point(47, 49);
			this.incorrectName.Name = "incorrectName";
			this.incorrectName.Size = new System.Drawing.Size(0, 16);
			this.incorrectName.TabIndex = 2;
			// 
			// incorrectPassword
			// 
			this.incorrectPassword.AutoSize = true;
			this.incorrectPassword.BackColor = System.Drawing.Color.Transparent;
			this.incorrectPassword.Font = new System.Drawing.Font("Roboto Light", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.incorrectPassword.Location = new System.Drawing.Point(47, 109);
			this.incorrectPassword.Name = "incorrectPassword";
			this.incorrectPassword.Size = new System.Drawing.Size(0, 16);
			this.incorrectPassword.TabIndex = 8;
			// 
			// showPasswordCheckBox
			// 
			this.showPasswordCheckBox.AutoSize = true;
			this.showPasswordCheckBox.BackColor = System.Drawing.Color.Transparent;
			this.showPasswordCheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.showPasswordCheckBox.Checked = true;
			this.showPasswordCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.showPasswordCheckBox.Cursor = System.Windows.Forms.Cursors.Default;
			this.showPasswordCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.showPasswordCheckBox.Font = new System.Drawing.Font("Roboto Light", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.showPasswordCheckBox.ForeColor = System.Drawing.SystemColors.GrayText;
			this.showPasswordCheckBox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.showPasswordCheckBox.Location = new System.Drawing.Point(50, 171);
			this.showPasswordCheckBox.Name = "showPasswordCheckBox";
			this.showPasswordCheckBox.Size = new System.Drawing.Size(135, 20);
			this.showPasswordCheckBox.TabIndex = 3;
			this.showPasswordCheckBox.Text = "СКРЫВАТЬ ПАРОЛЬ";
			this.showPasswordCheckBox.UseVisualStyleBackColor = false;
			this.showPasswordCheckBox.CheckedChanged += new System.EventHandler(this.showPasswordCheckBox_CheckedChanged);
			// 
			// userNamePanel
			// 
			this.userNamePanel.BackColor = System.Drawing.Color.White;
			this.userNamePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.userNamePanel.Controls.Add(this.userName);
			this.userNamePanel.Location = new System.Drawing.Point(50, 65);
			this.userNamePanel.Name = "userNamePanel";
			this.userNamePanel.Size = new System.Drawing.Size(200, 40);
			this.userNamePanel.TabIndex = 5;
			this.userNamePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.userNamePanel_Paint);
			// 
			// userName
			// 
			this.userName.BackColor = System.Drawing.Color.White;
			this.userName.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.userName.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.userName.Location = new System.Drawing.Point(9, 9);
			this.userName.Name = "userName";
			this.userName.Size = new System.Drawing.Size(182, 22);
			this.userName.TabIndex = 0;
			this.userName.Enter += new System.EventHandler(this.userName_Enter);
			// 
			// loginFormPanel
			// 
			this.loginFormPanel.Controls.Add(this.topPanel);
			this.loginFormPanel.Controls.Add(this.incorrectPassword);
			this.loginFormPanel.Controls.Add(this.showPasswordCheckBox);
			this.loginFormPanel.Controls.Add(this.incorrectName);
			this.loginFormPanel.Controls.Add(this.userNamePanel);
			this.loginFormPanel.Controls.Add(this.userPasswordPanel);
			this.loginFormPanel.Controls.Add(this.registerButton);
			this.loginFormPanel.Controls.Add(this.loginButton);
			this.loginFormPanel.Location = new System.Drawing.Point(0, 0);
			this.loginFormPanel.Name = "loginFormPanel";
			this.loginFormPanel.Size = new System.Drawing.Size(300, 400);
			this.loginFormPanel.TabIndex = 9;
			this.loginFormPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.loginFormPanel_Paint);
			// 
			// userPasswordPanel
			// 
			this.userPasswordPanel.BackColor = System.Drawing.Color.White;
			this.userPasswordPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.userPasswordPanel.Controls.Add(this.userPassword);
			this.userPasswordPanel.Location = new System.Drawing.Point(50, 125);
			this.userPasswordPanel.Name = "userPasswordPanel";
			this.userPasswordPanel.Size = new System.Drawing.Size(200, 40);
			this.userPasswordPanel.TabIndex = 6;
			this.userPasswordPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.userPasswordPanel_Paint);
			// 
			// userPassword
			// 
			this.userPassword.BackColor = System.Drawing.Color.White;
			this.userPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.userPassword.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.userPassword.Location = new System.Drawing.Point(9, 9);
			this.userPassword.Name = "userPassword";
			this.userPassword.PasswordChar = '*';
			this.userPassword.Size = new System.Drawing.Size(182, 22);
			this.userPassword.TabIndex = 1;
			this.userPassword.Enter += new System.EventHandler(this.userPassword_Enter);
			// 
			// registerButton
			// 
			this.registerButton.BackColor = global::CryptoMessenger.Properties.Settings.Default.LoginFirstColor;
			this.registerButton.BackgroundImage = global::CryptoMessenger.Properties.Resources.register;
			this.registerButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.registerButton.Cursor = System.Windows.Forms.Cursors.Default;
			this.registerButton.FlatAppearance.BorderSize = 0;
			this.registerButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(160)))));
			this.registerButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(225)))), ((int)(((byte)(190)))));
			this.registerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.registerButton.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.registerButton.ForeColor = System.Drawing.Color.White;
			this.registerButton.Location = new System.Drawing.Point(50, 300);
			this.registerButton.Name = "registerButton";
			this.registerButton.Size = new System.Drawing.Size(200, 50);
			this.registerButton.TabIndex = 5;
			this.registerButton.TabStop = false;
			this.registerButton.Text = "РЕГИСТРАЦИЯ";
			this.registerButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.registerButton.UseVisualStyleBackColor = false;
			this.registerButton.Click += new System.EventHandler(this.registerButton_Click);
			// 
			// loginButton
			// 
			this.loginButton.BackColor = global::CryptoMessenger.Properties.Settings.Default.LoginFirstColor;
			this.loginButton.BackgroundImage = global::CryptoMessenger.Properties.Resources.login;
			this.loginButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.loginButton.Cursor = System.Windows.Forms.Cursors.Default;
			this.loginButton.FlatAppearance.BorderSize = 0;
			this.loginButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(200)))), ((int)(((byte)(160)))));
			this.loginButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(225)))), ((int)(((byte)(190)))));
			this.loginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.loginButton.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.loginButton.ForeColor = System.Drawing.Color.White;
			this.loginButton.Location = new System.Drawing.Point(50, 230);
			this.loginButton.Name = "loginButton";
			this.loginButton.Size = new System.Drawing.Size(200, 50);
			this.loginButton.TabIndex = 4;
			this.loginButton.TabStop = false;
			this.loginButton.Text = "ВХОД";
			this.loginButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.loginButton.UseVisualStyleBackColor = false;
			this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
			// 
			// LoginForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = global::CryptoMessenger.Properties.Settings.Default.BackgroundWhiteColor;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.ClientSize = new System.Drawing.Size(300, 400);
			this.Controls.Add(this.loginFormPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = global::CryptoMessenger.Properties.Resources.icon;
			this.KeyPreview = true;
			this.Name = "LoginForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Crypto Messenger";
			this.Load += new System.EventHandler(this.LoginForm_Load);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LoginForm_KeyPress);
			this.topPanel.ResumeLayout(false);
			this.topPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.iconBox)).EndInit();
			this.userNamePanel.ResumeLayout(false);
			this.userNamePanel.PerformLayout();
			this.loginFormPanel.ResumeLayout(false);
			this.loginFormPanel.PerformLayout();
			this.userPasswordPanel.ResumeLayout(false);
			this.userPasswordPanel.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private TextBox userName;
        private TextBox userPassword;
        private MyButton minimizeButton;
        private MyButton closeButton;
        private Panel topPanel;
        private MyButton loginButton;
        private MyButton registerButton;
        private Label appName;
        private Panel userNamePanel;
        private Panel userPasswordPanel;
        private Label incorrectName;
        private Label incorrectPassword;
        private CheckBox showPasswordCheckBox;
        private Panel loginFormPanel;
		private PictureBox iconBox;
	}
}
