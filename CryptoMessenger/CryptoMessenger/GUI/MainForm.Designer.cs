using System;
using System.Drawing;
using System.Windows.Forms;

namespace CryptoMessenger.GUI
{
    partial class MainForm
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
				if (m.WParam != System.IntPtr.Zero)
				{
					topPanel.BackColor = Properties.Settings.Default.MainFirstColor;
					mainFormPanelBorderColor = Properties.Settings.Default.MainFirstColor;
					Refresh();
					if (Visible) shadow.Show();
				}
				else
				{
					topPanel.BackColor = Properties.Settings.Default.MainSecondColor;
					mainFormPanelBorderColor = Properties.Settings.Default.MainSecondColor;
					Refresh();
					shadow.Hide();
				}
			}

			base.WndProc(ref m);
		}

		#region Loading form

		private void MainForm_Load(object sender, EventArgs e)
		{
			appName.Font = loginForm.NeueFont15;
			message.Font = loginForm.NeueFont15;
			sendButton.Font = loginForm.NeueFont15;
			friendsTitle.Font = loginForm.NeueFont15;
			searchTitle.Font = loginForm.NeueFont15;
			loadingLabel.Font = loginForm.NeueFont15;
			activeTalkTitle.Font = loginForm.NeueFont15;
			usersListBox.Font = loginForm.NeueFont15;

			ActiveControl = sendButton;

			Text = login;
			appName.Text = login;
		}

		#endregion

		#region Panels borders

		// panel border color
		private Color mainFormPanelBorderColor = Properties.Settings.Default.MainFirstColor;

		private void mainFormPanel_Paint(object sender, PaintEventArgs e)
		{
			Rectangle rect = mainFormPanel.ClientRectangle;
			rect.Width--;
			rect.Height--;
			e.Graphics.DrawRectangle(new Pen(mainFormPanelBorderColor), rect);
		}
		private void contactsPanel_Paint(object sender, PaintEventArgs e)
		{
			Rectangle rect = contactsPanel.ClientRectangle;
			rect.Width--;
			rect.Height--;
			e.Graphics.DrawRectangle(new Pen(Properties.Settings.Default.PanelBorderColor), rect);
		}
		private void splitContainer_Paint(object sender, PaintEventArgs e)
		{
			Rectangle rect = splitContainer.ClientRectangle;
			rect.Width--;
			rect.Height--;
			e.Graphics.DrawRectangle(new Pen(Properties.Settings.Default.PanelBorderColor), rect);

			splitContainer.SplitterWidth = 9;

			int left = splitContainer.Width / 2 - 10;
			int right = splitContainer.Width / 2 + 10;
			int line1 = splitContainer.SplitterDistance + 2;
			int line2 = splitContainer.SplitterDistance + 4;
			int line3 = splitContainer.SplitterDistance + 6;
			e.Graphics.DrawLine(new Pen(Properties.Settings.Default.MainFirstColor), left, line1, right, line1);
			e.Graphics.DrawLine(new Pen(Properties.Settings.Default.MainFirstColor), left, line2, right, line2);
			e.Graphics.DrawLine(new Pen(Properties.Settings.Default.MainFirstColor), left, line3, right, line3);
		}
		private void splitContainer_Panel1_Paint(object sender, PaintEventArgs e)
		{
			Rectangle rect = splitContainer.Panel1.ClientRectangle;
			rect.Width--;
			rect.Height--;
			e.Graphics.DrawRectangle(new Pen(Properties.Settings.Default.PanelBorderColor), rect);
		}
		private void splitContainer_Panel2_Paint(object sender, PaintEventArgs e)
		{
			Rectangle rect = splitContainer.Panel2.ClientRectangle;
			rect.Width--;
			rect.Height--;
			e.Graphics.DrawRectangle(new Pen(Properties.Settings.Default.PanelBorderColor), rect);
		}
		private void activeTalkTitlePanel_Paint(object sender, PaintEventArgs e)
		{
			Rectangle rect = activeTalkTitlePanel.ClientRectangle;
			e.Graphics.DrawLine(new Pen(Properties.Settings.Default.MainFirstColor),
				new Point(1, rect.Height - 1), new Point(rect.Width - 2, rect.Height - 1));
		}
		private void friendsTitlePanel_Paint(object sender, PaintEventArgs e)
		{
			Rectangle rect = friendsTitlePanel.ClientRectangle;
			e.Graphics.DrawLine(new Pen(Properties.Settings.Default.MainFirstColor),
				new Point(1, rect.Height - 1), new Point(rect.Width - 2, rect.Height - 1));
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

		#region Resize interface components when need

		// resize label when text changed (center alignment)
		private void activeTalkTitle_SizeChanged(object sender, EventArgs e)
		{
			Point location = activeTalkTitle.Location;
			location.X = (activeTalkTitlePanel.Width - activeTalkTitle.Width) / 2;
			activeTalkTitle.Location = location;
		}

		// resize button and textbox when split control change size
		private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
		{
			sendButton.Height = splitContainer.Panel2.Height - 2;
			message.Height = splitContainer.Panel2.Height - 10;
		}

		// resize textbox to fit content
		private void message_TextChanged(object sender, EventArgs e)
		{
			Size size = TextRenderer.MeasureText(message.Text, message.Font);
			size.Width += 10;

			if (size.Width > message.Width)
			{
				int remainder;
				int quotient = Math.DivRem(size.Width, message.Width, out remainder);
				int k = remainder == 0 ? quotient : quotient + 1;
				int height = size.Height * k;

				int x = (message.Height - height) / size.Height;

				if (message.Height < height | (message.Height > height & x <= 1))
				{
					int new_spl_dist = splitContainer.Height - height - 25;

					if (new_spl_dist > splitContainer.Panel1MinSize)
					{
						splitContainer.SplitterDistance = new_spl_dist;
						message.Height = height;
					}
				}
			}
		}

		#endregion

		#region Message placeholder

		// add placeholder
		private void message_Leave(object sender, EventArgs e)
		{
			if (message.Text.Equals(""))
			{
				message.Text = "ВВЕДИТЕ СООБЩЕНИЕ...";
				message.ForeColor = SystemColors.InactiveCaption;
			}
		}
		// remove placeholder
		private void message_Enter(object sender, EventArgs e)
		{
			if (message.Text.Equals("ВВЕДИТЕ СООБЩЕНИЕ..."))
			{
				message.Text = "";
				message.ForeColor = Color.Black;
			}
		}

		#endregion

		#region Switch friends / all

		// show chat with selected user
		private bool showChat;

		// show friends
		private void friendsTitle_Click(object sender, EventArgs e)
		{
			showChat = true;

			Point tmp = friendsTitle.Location;
			tmp.X = (friendsTitlePanel.Width - friendsTitle.Width) / 2;
			friendsTitle.Location = tmp;

			tmp = searchTitle.Location;
			tmp.X = friendsTitlePanel.Width - searchTitle.Width;
			searchTitle.Location = tmp;
		}

		// show all users
		private void searchTitle_Click(object sender, EventArgs e)
		{
			showChat = false;

			Point tmp = searchTitle.Location;
			tmp.X = (friendsTitlePanel.Width - searchTitle.Width) / 2;
			searchTitle.Location = tmp;

			tmp = friendsTitle.Location;
			tmp.X = 0;
			friendsTitle.Location = tmp;
		}

		#endregion

		#region Custom listbox style

		// Custom item measure of listbox.
		private void listBox_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			ListBox listBox = (ListBox)sender;
			e.ItemHeight = listBox.Font.Height;
		}

		// Custom item draw of listbox.
		private void listBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0) return;
			//if the item state is selected them change the back color 
			if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
			{
				e = new DrawItemEventArgs(e.Graphics,
										  e.Font,
										  e.Bounds,
										  e.Index,
										  e.State ^ DrawItemState.Selected,
										  e.ForeColor,
										  Properties.Settings.Default.MainSecondColor);

				// Draw the background of the ListBox control for each item.
				e.DrawBackground();
				// Draw the current item text
				Rectangle bounds = e.Bounds;
				bounds.X += 10;
				e.Graphics.DrawString(this.usersListBox.Items[e.Index].ToString(), e.Font, Brushes.White, bounds, StringFormat.GenericDefault);
			}
			else
			{
				// Draw the background of the ListBox control for each item.
				e.DrawBackground();
				// Draw the current item text
				Rectangle bounds = e.Bounds;
				bounds.X += 10;
				e.Graphics.DrawString(this.usersListBox.Items[e.Index].ToString(), e.Font, Brushes.Black, bounds, StringFormat.GenericDefault);
			}
		}

		#endregion

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
        {
			this.topPanel = new System.Windows.Forms.Panel();
			this.iconBox = new System.Windows.Forms.PictureBox();
			this.closeButton = new CryptoMessenger.GUI.MyButton();
			this.minimizeButton = new CryptoMessenger.GUI.MyButton();
			this.appName = new System.Windows.Forms.Label();
			this.contactsPanel = new System.Windows.Forms.Panel();
			this.usersListBox = new System.Windows.Forms.ListBox();
			this.friendsTitlePanel = new System.Windows.Forms.Panel();
			this.searchTitle = new System.Windows.Forms.Label();
			this.friendsTitle = new System.Windows.Forms.Label();
			this.loadingLabel = new System.Windows.Forms.Label();
			this.mainFormPanel = new System.Windows.Forms.Panel();
			this.splitContainer = new CryptoMessenger.GUI.MySplitContainer();
			this.activeTalkTitlePanel = new System.Windows.Forms.Panel();
			this.activeTalkTitle = new System.Windows.Forms.Label();
			this.message = new System.Windows.Forms.TextBox();
			this.sendButton = new CryptoMessenger.GUI.MyButton();
			this.topPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.iconBox)).BeginInit();
			this.contactsPanel.SuspendLayout();
			this.friendsTitlePanel.SuspendLayout();
			this.mainFormPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.activeTalkTitlePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// topPanel
			// 
			this.topPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(10)))), ((int)(((byte)(90)))));
			this.topPanel.Controls.Add(this.iconBox);
			this.topPanel.Controls.Add(this.closeButton);
			this.topPanel.Controls.Add(this.minimizeButton);
			this.topPanel.Controls.Add(this.appName);
			this.topPanel.Location = new System.Drawing.Point(0, 0);
			this.topPanel.Name = "topPanel";
			this.topPanel.Size = new System.Drawing.Size(700, 35);
			this.topPanel.TabIndex = 0;
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
			this.iconBox.TabIndex = 10;
			this.iconBox.TabStop = false;
			this.iconBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseDown);
			this.iconBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseMove);
			this.iconBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseUp);
			// 
			// closeButton
			// 
			this.closeButton.BackColor = System.Drawing.Color.Transparent;
			this.closeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.closeButton.FlatAppearance.BorderSize = 0;
			this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.closeButton.Image = global::CryptoMessenger.Properties.Resources.close_button_normal;
			this.closeButton.Location = new System.Drawing.Point(670, 2);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(20, 31);
			this.closeButton.TabIndex = 4;
			this.closeButton.TabStop = false;
			this.closeButton.UseVisualStyleBackColor = false;
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			this.closeButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.closeButton_MouseDown);
			this.closeButton.MouseEnter += new System.EventHandler(this.closeButton_MouseEnter);
			this.closeButton.MouseLeave += new System.EventHandler(this.closeButton_MouseLeave);
			// 
			// minimizeButton
			// 
			this.minimizeButton.BackColor = System.Drawing.Color.Transparent;
			this.minimizeButton.FlatAppearance.BorderSize = 0;
			this.minimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.minimizeButton.Image = global::CryptoMessenger.Properties.Resources.minimize_button_normal;
			this.minimizeButton.Location = new System.Drawing.Point(640, 2);
			this.minimizeButton.Name = "minimizeButton";
			this.minimizeButton.Size = new System.Drawing.Size(20, 31);
			this.minimizeButton.TabIndex = 3;
			this.minimizeButton.TabStop = false;
			this.minimizeButton.UseVisualStyleBackColor = false;
			this.minimizeButton.Click += new System.EventHandler(this.minimizeButton_Click);
			this.minimizeButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.minimize_MouseDown);
			this.minimizeButton.MouseEnter += new System.EventHandler(this.minimizeButton_MouseEnter);
			this.minimizeButton.MouseLeave += new System.EventHandler(this.minimizeButton_MouseLeave);
			// 
			// appName
			// 
			this.appName.AutoSize = true;
			this.appName.BackColor = System.Drawing.Color.Transparent;
			this.appName.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.appName.ForeColor = System.Drawing.Color.White;
			this.appName.Location = new System.Drawing.Point(40, 5);
			this.appName.Name = "appName";
			this.appName.Size = new System.Drawing.Size(54, 21);
			this.appName.TabIndex = 1;
			this.appName.Text = "LOGIN";
			this.appName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseDown);
			this.appName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseMove);
			this.appName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseUp);
			// 
			// contactsPanel
			// 
			this.contactsPanel.BackColor = System.Drawing.Color.White;
			this.contactsPanel.Controls.Add(this.usersListBox);
			this.contactsPanel.Controls.Add(this.friendsTitlePanel);
			this.contactsPanel.Controls.Add(this.loadingLabel);
			this.contactsPanel.Location = new System.Drawing.Point(10, 45);
			this.contactsPanel.Name = "contactsPanel";
			this.contactsPanel.Size = new System.Drawing.Size(200, 445);
			this.contactsPanel.TabIndex = 1;
			this.contactsPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.contactsPanel_Paint);
			// 
			// usersListBox
			// 
			this.usersListBox.BackColor = System.Drawing.SystemColors.Window;
			this.usersListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.usersListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.usersListBox.FormattingEnabled = true;
			this.usersListBox.Location = new System.Drawing.Point(1, 35);
			this.usersListBox.Name = "usersListBox";
			this.usersListBox.Size = new System.Drawing.Size(198, 400);
			this.usersListBox.TabIndex = 2;
			this.usersListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_DrawItem);
			this.usersListBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox_MeasureItem);
			this.usersListBox.SelectedIndexChanged += new System.EventHandler(this.friendsListBox_SelectedIndexChanged);
			// 
			// friendsTitlePanel
			// 
			this.friendsTitlePanel.BackColor = System.Drawing.Color.Transparent;
			this.friendsTitlePanel.Controls.Add(this.searchTitle);
			this.friendsTitlePanel.Controls.Add(this.friendsTitle);
			this.friendsTitlePanel.Location = new System.Drawing.Point(0, 0);
			this.friendsTitlePanel.Name = "friendsTitlePanel";
			this.friendsTitlePanel.Size = new System.Drawing.Size(200, 25);
			this.friendsTitlePanel.TabIndex = 2;
			this.friendsTitlePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.friendsTitlePanel_Paint);
			// 
			// searchTitle
			// 
			this.searchTitle.AutoSize = true;
			this.searchTitle.BackColor = System.Drawing.Color.Transparent;
			this.searchTitle.Font = new System.Drawing.Font("Roboto Light", 12F);
			this.searchTitle.ForeColor = System.Drawing.SystemColors.GrayText;
			this.searchTitle.Location = new System.Drawing.Point(150, 0);
			this.searchTitle.Name = "searchTitle";
			this.searchTitle.Size = new System.Drawing.Size(63, 21);
			this.searchTitle.TabIndex = 12;
			this.searchTitle.Text = "ПОИСК";
			this.searchTitle.Click += new System.EventHandler(this.searchTitle_Click);
			this.searchTitle.Click += new System.EventHandler(this.Click_ShowUsers);
			// 
			// friendsTitle
			// 
			this.friendsTitle.AutoSize = true;
			this.friendsTitle.BackColor = System.Drawing.Color.Transparent;
			this.friendsTitle.Font = new System.Drawing.Font("Roboto Light", 12F);
			this.friendsTitle.ForeColor = System.Drawing.SystemColors.GrayText;
			this.friendsTitle.Location = new System.Drawing.Point(64, 0);
			this.friendsTitle.Name = "friendsTitle";
			this.friendsTitle.Size = new System.Drawing.Size(72, 21);
			this.friendsTitle.TabIndex = 2;
			this.friendsTitle.Text = "ДРУЗЬЯ";
			this.friendsTitle.Click += new System.EventHandler(this.friendsTitle_Click);
			this.friendsTitle.Click += new System.EventHandler(this.Click_ShowUsers);
			// 
			// loadingLabel
			// 
			this.loadingLabel.AutoSize = true;
			this.loadingLabel.BackColor = System.Drawing.Color.Transparent;
			this.loadingLabel.Font = new System.Drawing.Font("Roboto Light", 12F);
			this.loadingLabel.ForeColor = System.Drawing.SystemColors.GrayText;
			this.loadingLabel.Location = new System.Drawing.Point(65, 200);
			this.loadingLabel.Name = "loadingLabel";
			this.loadingLabel.Size = new System.Drawing.Size(101, 21);
			this.loadingLabel.TabIndex = 2;
			this.loadingLabel.Text = "ЗАГРУЗКА...";
			this.loadingLabel.Visible = false;
			// 
			// mainFormPanel
			// 
			this.mainFormPanel.Controls.Add(this.splitContainer);
			this.mainFormPanel.Controls.Add(this.topPanel);
			this.mainFormPanel.Controls.Add(this.contactsPanel);
			this.mainFormPanel.Location = new System.Drawing.Point(0, 0);
			this.mainFormPanel.Name = "mainFormPanel";
			this.mainFormPanel.Size = new System.Drawing.Size(700, 500);
			this.mainFormPanel.TabIndex = 11;
			this.mainFormPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mainFormPanel_Paint);
			// 
			// splitContainer
			// 
			this.splitContainer.BackColor = System.Drawing.Color.White;
			this.splitContainer.Location = new System.Drawing.Point(220, 45);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.activeTalkTitlePanel);
			this.splitContainer.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer_Panel1_Paint);
			this.splitContainer.Panel1MinSize = 150;
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.message);
			this.splitContainer.Panel2.Controls.Add(this.sendButton);
			this.splitContainer.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer_Panel2_Paint);
			this.splitContainer.Panel2MinSize = 60;
			this.splitContainer.Size = new System.Drawing.Size(470, 445);
			this.splitContainer.SplitterDistance = 376;
			this.splitContainer.SplitterWidth = 9;
			this.splitContainer.TabIndex = 2;
			this.splitContainer.TabStop = false;
			this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
			this.splitContainer.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer_Paint);
			// 
			// activeTalkTitlePanel
			// 
			this.activeTalkTitlePanel.BackColor = System.Drawing.Color.Transparent;
			this.activeTalkTitlePanel.Controls.Add(this.activeTalkTitle);
			this.activeTalkTitlePanel.Location = new System.Drawing.Point(0, 0);
			this.activeTalkTitlePanel.Name = "activeTalkTitlePanel";
			this.activeTalkTitlePanel.Size = new System.Drawing.Size(470, 25);
			this.activeTalkTitlePanel.TabIndex = 1;
			this.activeTalkTitlePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.activeTalkTitlePanel_Paint);
			// 
			// activeTalkTitle
			// 
			this.activeTalkTitle.AutoSize = true;
			this.activeTalkTitle.BackColor = System.Drawing.Color.Transparent;
			this.activeTalkTitle.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.activeTalkTitle.ForeColor = System.Drawing.SystemColors.GrayText;
			this.activeTalkTitle.Location = new System.Drawing.Point(195, 0);
			this.activeTalkTitle.Name = "activeTalkTitle";
			this.activeTalkTitle.Size = new System.Drawing.Size(74, 21);
			this.activeTalkTitle.TabIndex = 0;
			this.activeTalkTitle.Text = "ДИАЛОГ";
			this.activeTalkTitle.SizeChanged += new System.EventHandler(this.activeTalkTitle_SizeChanged);
			// 
			// message
			// 
			this.message.BackColor = System.Drawing.Color.White;
			this.message.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.message.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.message.ForeColor = System.Drawing.SystemColors.GrayText;
			this.message.Location = new System.Drawing.Point(10, 5);
			this.message.Multiline = true;
			this.message.Name = "message";
			this.message.Size = new System.Drawing.Size(392, 50);
			this.message.TabIndex = 0;
			this.message.Text = "ВВЕДИТЕ СООБЩЕНИЕ...";
			this.message.TextChanged += new System.EventHandler(this.message_TextChanged);
			this.message.Enter += new System.EventHandler(this.message_Enter);
			this.message.Leave += new System.EventHandler(this.message_Leave);
			// 
			// sendButton
			// 
			this.sendButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(10)))), ((int)(((byte)(90)))));
			this.sendButton.BackgroundImage = global::CryptoMessenger.Properties.Resources.send;
			this.sendButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.sendButton.Cursor = System.Windows.Forms.Cursors.Default;
			this.sendButton.FlatAppearance.BorderSize = 0;
			this.sendButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(0)))), ((int)(((byte)(75)))));
			this.sendButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(25)))), ((int)(((byte)(105)))));
			this.sendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.sendButton.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.sendButton.ForeColor = System.Drawing.Color.White;
			this.sendButton.Location = new System.Drawing.Point(412, 1);
			this.sendButton.Name = "sendButton";
			this.sendButton.Size = new System.Drawing.Size(58, 58);
			this.sendButton.TabIndex = 1;
			this.sendButton.TabStop = false;
			this.sendButton.UseVisualStyleBackColor = true;
			this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(252)))), ((int)(((byte)(248)))));
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.ClientSize = new System.Drawing.Size(700, 500);
			this.Controls.Add(this.mainFormPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = global::CryptoMessenger.Properties.Resources.icon;
			this.KeyPreview = true;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MainForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
			this.topPanel.ResumeLayout(false);
			this.topPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.iconBox)).EndInit();
			this.contactsPanel.ResumeLayout(false);
			this.contactsPanel.PerformLayout();
			this.friendsTitlePanel.ResumeLayout(false);
			this.friendsTitlePanel.PerformLayout();
			this.mainFormPanel.ResumeLayout(false);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.activeTalkTitlePanel.ResumeLayout(false);
			this.activeTalkTitlePanel.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private Panel topPanel;
        private Label appName;
        private MyButton minimizeButton;
        private MyButton closeButton;
        private Panel contactsPanel;
        private TextBox message;
        private MyButton sendButton;
        private Label activeTalkTitle;
        private Panel activeTalkTitlePanel;
        private Panel mainFormPanel;
		private PictureBox iconBox;
		private MySplitContainer splitContainer;
		private Panel friendsTitlePanel;
		private Label friendsTitle;
		private ListBox usersListBox;
		private Label searchTitle;
		private Label loadingLabel;
	}
}
