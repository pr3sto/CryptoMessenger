using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

using TheArtOfDev.HtmlRenderer.WinForms;

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
		/// Parameters for minimizing form.
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
			// interface fonts
			appNameLabel.Font = loginForm.NeueFont15;
			loadingLabel.Font = loginForm.NeueFont15;
			friendsLabel.Font = loginForm.NeueFont15;
			friendsListBox.Font = loginForm.NeueFont15;
			removeFriendButton.Font = loginForm.NeueFont15;
			searchLabel.Font = loginForm.NeueFont10;
			allUsersListBox.Font = loginForm.NeueFont15;
			addFriendButton.Font = loginForm.NeueFont15;
			friendshipRequestsLabel.Font = loginForm.NeueFont10;
			incomeFriendshipRequestsLabel.Font = loginForm.NeueFont15;
			outcomeFriendshipRequestsLabel.Font = loginForm.NeueFont15;
			incomeFriendshipRequestsListBox.Font = loginForm.NeueFont15;
			outcomeFriendshipRequestsListBox.Font = loginForm.NeueFont15;
			cancelFriendshipRequestButton.Font = loginForm.NeueFont15;
			acceptFriendshipButton.Font = loginForm.NeueFont15;
			rejectFriendshipButton.Font = loginForm.NeueFont15;
			replyTextfield.Font = loginForm.NeueFont15;
			sendReplyButton.Font = loginForm.NeueFont15;
			activeTalkLabel.Font = loginForm.NeueFont15;

			// load interface
			ResizeContactLabels();
			UpdateUserPanel(friendsLabel, EventArgs.Empty);
			// center alignment
			Point tmp = incomeFriendshipRequestsLabel.Location;
			tmp.X = (friendshipRequestsPanel.Width - incomeFriendshipRequestsLabel.Width) / 2;
			incomeFriendshipRequestsLabel.Location = tmp;
			tmp = outcomeFriendshipRequestsLabel.Location;
			tmp.X = (friendshipRequestsPanel.Width - outcomeFriendshipRequestsLabel.Width) / 2;
			outcomeFriendshipRequestsLabel.Location = tmp;

			this.searchLabel.Click += new System.EventHandler(this.UpdateUserPanel);
			this.friendsLabel.Click += new System.EventHandler(this.UpdateUserPanel);
			this.friendshipRequestsLabel.Click += new System.EventHandler(this.UpdateUserPanel);

			ActiveControl = sendReplyButton;

			Text = login;
			appNameLabel.Text = login;
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
			Rectangle rect = usersPanel.ClientRectangle;
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
			Rectangle rect = activeTalkLabelPanel.ClientRectangle;
			e.Graphics.DrawLine(new Pen(Properties.Settings.Default.MainFirstColor),
				new Point(1, rect.Height - 1), new Point(rect.Width - 2, rect.Height - 1));
		}
		private void friendsTitlePanel_Paint(object sender, PaintEventArgs e)
		{
			Rectangle rect = usersLabelsPanel.ClientRectangle;
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
			Point location = activeTalkLabel.Location;
			location.X = (activeTalkLabelPanel.Width - activeTalkLabel.Width) / 2;
			activeTalkLabel.Location = location;
		}

		// resize button and textbox when split control change size
		private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
		{
			sendReplyButton.Height = splitContainer.Panel2.Height - 2;
			replyTextfield.Height = splitContainer.Panel2.Height - 10;
		}

		// resize textbox to fit content
		private void replyTextfield_TextChanged(object sender, EventArgs e)
		{
			Size size = TextRenderer.MeasureText(replyTextfield.Text, replyTextfield.Font);
			size.Width += 10;

			if (size.Width > replyTextfield.Width)
			{
				int remainder;
				int quotient = Math.DivRem(size.Width, replyTextfield.Width, out remainder);
				int k = remainder == 0 ? quotient : quotient + 1;
				int height = size.Height * k;

				int x = (replyTextfield.Height - height) / size.Height;

				if (replyTextfield.Height < height | (replyTextfield.Height > height & x <= 1))
				{
					int new_spl_dist = splitContainer.Height - height - 25;

					if (new_spl_dist > splitContainer.Panel1MinSize)
					{
						splitContainer.SplitterDistance = new_spl_dist;
						replyTextfield.Height = height;
					}
				}
			}
		}

		#endregion

		#region Hover users panels labels

		private void userPanelsLabel_MouseEnter(object sender, EventArgs e)
		{
			Label label = (Label)sender;
			label.ForeColor = Color.Black;
		}

		private void userPanelsLabel_MouseLeave(object sender, EventArgs e)
		{
			Label label = (Label)sender;
			label.ForeColor = SystemColors.GrayText;
		}

		#endregion

		#region Switch between income and outcome requests

		private void incomeRequestsListBox_Enter(object sender, EventArgs e)
		{
			outcomeFriendshipRequestsListBox.ClearSelected();
			cancelFriendshipRequestButton.Visible = false;
			acceptFriendshipButton.Visible = true;
			rejectFriendshipButton.Visible = true;
		}

		private void outcomeRequestsListBox_Enter(object sender, EventArgs e)
		{
			incomeFriendshipRequestsListBox.ClearSelected();
			acceptFriendshipButton.Visible = false;
			rejectFriendshipButton.Visible = false;
			cancelFriendshipRequestButton.Visible = true;
		}

		#endregion

		#region Switch friends / all / requests

		// show friendship requests
		private void friendRequestTitle_Click(object sender, EventArgs e)
		{
			if (selectedPanel == UsersPanels.REQUESTS) return;

			friendshipRequestsLabel.Font = loginForm.NeueFont15;
			friendsLabel.Font = loginForm.NeueFont10;
			searchLabel.Font = loginForm.NeueFont10;

			ResizeContactLabels();
			DisableSendReplies();

			selectedPanel = UsersPanels.REQUESTS;
		}

		// show friends
		private void friendsTitle_Click(object sender, EventArgs e)
		{
			if (selectedPanel == UsersPanels.FRIENDS) return;

			friendshipRequestsLabel.Font = loginForm.NeueFont10;
			friendsLabel.Font = loginForm.NeueFont15;
			searchLabel.Font = loginForm.NeueFont10;

			ResizeContactLabels();

			selectedPanel = UsersPanels.FRIENDS;
		}

		// show all users
		private void searchTitle_Click(object sender, EventArgs e)
		{
			if (selectedPanel == UsersPanels.SEARCH) return;

			friendshipRequestsLabel.Font = loginForm.NeueFont10;
			friendsLabel.Font = loginForm.NeueFont10;
			searchLabel.Font = loginForm.NeueFont15;

			ResizeContactLabels();
			DisableSendReplies();

			selectedPanel = UsersPanels.SEARCH;
		}

		// resize labels
		private void ResizeContactLabels()
		{
			Point tmp = new Point();
			tmp.X = 0;
			tmp.Y = usersLabelsPanel.Height / 2 - friendshipRequestsLabel.Height / 2;
			friendshipRequestsLabel.Location = tmp;

			tmp.X = (usersLabelsPanel.Width - friendsLabel.Width) / 2;
			tmp.Y = usersLabelsPanel.Height / 2 - friendsLabel.Height / 2;
			friendsLabel.Location = tmp;

			tmp.X = usersLabelsPanel.Width - searchLabel.Width;
			tmp.Y = usersLabelsPanel.Height / 2 - searchLabel.Height / 2;
			searchLabel.Location = tmp;
		}

		#endregion

		#region Listboxes selected item changed

		private async void AllUsersListBoxSelectedChanged(object sender, EventArgs e)
		{
			// show this text for some time
			if ("ЗАЯВКА ОТПРАВЛЕНА".Equals(addFriendButton.Text))
				await Task.Delay(TimeSpan.FromMilliseconds(800));

			if (allUsersListBox.SelectedItem == null)
			{
				addFriendButton.Enabled = false;
				addFriendButton.Text = "CRYPTO MESSENGER";
			}
			else
			{
				addFriendButton.Enabled = true;
				addFriendButton.Text = "ДОБАВИТЬ В ДРУЗЬЯ";
			}
		}

		private async void FriendsListBoxSelectedChanged(object sender, EventArgs e)
		{
			// show this text for some time
			if ("ПОЛЬЗОВАТЕЛЬ УДАЛЕН".Equals(removeFriendButton.Text))
				await Task.Delay(TimeSpan.FromMilliseconds(800));

			if (friendsListBox.SelectedItem == null)
			{
				removeFriendButton.Enabled = false;
				removeFriendButton.Text = "CRYPTO MESSENGER";
				DisableSendReplies();
			}
			else
			{
				removeFriendButton.Enabled = true;
				removeFriendButton.Text = "УДАЛИТЬ ИЗ ДРУЗЕЙ";
				EnabledSendReplies();

				GetOrShowConversation(activeTalkLabel.Text);
			}
		}

		private async void OutcomeRequestsListBoxesSelectedChanged(object sender, EventArgs e)
		{
			// show this text for some time
			if ("ЗАЯВКА ОТМЕНЕНА".Equals(cancelFriendshipRequestButton.Text) ||
				"ЗАЯВКА ПРИНЯТА".Equals(cancelFriendshipRequestButton.Text) ||
				"ЗАЯВКА ОТКЛОНЕНА".Equals(cancelFriendshipRequestButton.Text))
				await Task.Delay(TimeSpan.FromMilliseconds(800));

			if (outcomeFriendshipRequestsListBox.SelectedItem == null)
			{
				cancelFriendshipRequestButton.Enabled = false;
				cancelFriendshipRequestButton.Text = "CRYPTO MESSENGER";
			}
			else
			{
				cancelFriendshipRequestButton.Enabled = true;
				cancelFriendshipRequestButton.Text = "ОТМЕНИТЬ ЗАЯВКУ";
			}
		}

		#endregion

		#region Enable/Disable send replyes

		private void EnabledSendReplies()
		{
			activeTalkLabel.Text = friendsListBox.SelectedItem.ToString();

			sendReplyButton.Enabled = true;
			replyTextfield.Enabled = true;
		}

		private void DisableSendReplies()
		{
			replyTextfield.Text = "";
			replyTextfield_Leave(replyTextfield, EventArgs.Empty);

			activeTalkLabel.Text = "ДИАЛОГ";

			sendReplyButton.Enabled = false;
			replyTextfield.Enabled = false;
		}

		#endregion

		#region Reply placeholder

		private bool IsPlaceholderWrited = true;

		// add placeholder
		private void replyTextfield_Leave(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(replyTextfield.Text))
			{
				replyTextfield.ForeColor = SystemColors.InactiveCaption;
				replyTextfield.Text = "ВВЕДИТЕ СООБЩЕНИЕ...";

				IsPlaceholderWrited = true;
				CanSendReply = false;
			}
		}
		// remove placeholder
		private void replyTextfield_Enter(object sender, EventArgs e)
		{
			if (IsPlaceholderWrited)
			{
				replyTextfield.ForeColor = Color.Black;
				replyTextfield.Text = "";

				IsPlaceholderWrited = false;
				CanSendReply = true;
			}
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
			ListBox listBox = (ListBox)sender;

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
				e.Graphics.DrawString(listBox.Items[e.Index].ToString(), e.Font, Brushes.White, bounds, StringFormat.GenericDefault);
			}
			else
			{
				// Draw the background of the ListBox control for each item.
				e.DrawBackground();
				// Draw the current item text
				Rectangle bounds = e.Bounds;
				bounds.X += 10;
				e.Graphics.DrawString(listBox.Items[e.Index].ToString(), e.Font, Brushes.Black, bounds, StringFormat.GenericDefault);
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
			this.appNameLabel = new System.Windows.Forms.Label();
			this.usersPanel = new System.Windows.Forms.Panel();
			this.allUsersPanel = new System.Windows.Forms.Panel();
			this.allUsersListBox = new System.Windows.Forms.ListBox();
			this.addFriendButton = new CryptoMessenger.GUI.MyButton();
			this.friendsPanel = new System.Windows.Forms.Panel();
			this.friendsListBox = new System.Windows.Forms.ListBox();
			this.removeFriendButton = new CryptoMessenger.GUI.MyButton();
			this.friendshipRequestsPanel = new System.Windows.Forms.Panel();
			this.outcomeFriendshipRequestsListBox = new System.Windows.Forms.ListBox();
			this.incomeFriendshipRequestsListBox = new System.Windows.Forms.ListBox();
			this.cancelFriendshipRequestButton = new CryptoMessenger.GUI.MyButton();
			this.acceptFriendshipButton = new CryptoMessenger.GUI.MyButton();
			this.rejectFriendshipButton = new CryptoMessenger.GUI.MyButton();
			this.outcomeFriendshipRequestsLabel = new System.Windows.Forms.Label();
			this.incomeFriendshipRequestsLabel = new System.Windows.Forms.Label();
			this.usersLabelsPanel = new System.Windows.Forms.Panel();
			this.friendshipRequestsLabel = new System.Windows.Forms.Label();
			this.searchLabel = new System.Windows.Forms.Label();
			this.friendsLabel = new System.Windows.Forms.Label();
			this.loadingLabel = new System.Windows.Forms.Label();
			this.mainFormPanel = new System.Windows.Forms.Panel();
			this.splitContainer = new CryptoMessenger.GUI.MySplitContainer();
			this.conversationHtmlPanel = new HtmlPanel();
			this.activeTalkLabelPanel = new System.Windows.Forms.Panel();
			this.activeTalkLabel = new System.Windows.Forms.Label();
			this.replyTextfield = new System.Windows.Forms.TextBox();
			this.sendReplyButton = new CryptoMessenger.GUI.MyButton();
			this.topPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.iconBox)).BeginInit();
			this.usersPanel.SuspendLayout();
			this.allUsersPanel.SuspendLayout();
			this.friendsPanel.SuspendLayout();
			this.friendshipRequestsPanel.SuspendLayout();
			this.usersLabelsPanel.SuspendLayout();
			this.mainFormPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.activeTalkLabelPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// topPanel
			// 
			this.topPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(10)))), ((int)(((byte)(90)))));
			this.topPanel.Controls.Add(this.iconBox);
			this.topPanel.Controls.Add(this.closeButton);
			this.topPanel.Controls.Add(this.minimizeButton);
			this.topPanel.Controls.Add(this.appNameLabel);
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
			this.iconBox.TabIndex = 0;
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
			this.closeButton.TabIndex = 0;
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
			this.minimizeButton.TabIndex = 0;
			this.minimizeButton.TabStop = false;
			this.minimizeButton.UseVisualStyleBackColor = false;
			this.minimizeButton.Click += new System.EventHandler(this.minimizeButton_Click);
			this.minimizeButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.minimize_MouseDown);
			this.minimizeButton.MouseEnter += new System.EventHandler(this.minimizeButton_MouseEnter);
			this.minimizeButton.MouseLeave += new System.EventHandler(this.minimizeButton_MouseLeave);
			// 
			// appNameLabel
			// 
			this.appNameLabel.AutoSize = true;
			this.appNameLabel.BackColor = System.Drawing.Color.Transparent;
			this.appNameLabel.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.appNameLabel.ForeColor = System.Drawing.Color.White;
			this.appNameLabel.Location = new System.Drawing.Point(40, 5);
			this.appNameLabel.Name = "appNameLabel";
			this.appNameLabel.Size = new System.Drawing.Size(54, 21);
			this.appNameLabel.TabIndex = 0;
			this.appNameLabel.Text = "LOGIN";
			this.appNameLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseDown);
			this.appNameLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseMove);
			this.appNameLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.moveForm_MouseUp);
			// 
			// usersPanel
			// 
			this.usersPanel.BackColor = System.Drawing.Color.White;
			this.usersPanel.Controls.Add(this.allUsersPanel);
			this.usersPanel.Controls.Add(this.friendsPanel);
			this.usersPanel.Controls.Add(this.friendshipRequestsPanel);
			this.usersPanel.Controls.Add(this.usersLabelsPanel);
			this.usersPanel.Controls.Add(this.loadingLabel);
			this.usersPanel.Location = new System.Drawing.Point(10, 45);
			this.usersPanel.Name = "usersPanel";
			this.usersPanel.Size = new System.Drawing.Size(200, 445);
			this.usersPanel.TabIndex = 0;
			this.usersPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.contactsPanel_Paint);
			// 
			// allUsersPanel
			// 
			this.allUsersPanel.Controls.Add(this.allUsersListBox);
			this.allUsersPanel.Controls.Add(this.addFriendButton);
			this.allUsersPanel.Location = new System.Drawing.Point(1, 30);
			this.allUsersPanel.Name = "allUsersPanel";
			this.allUsersPanel.Size = new System.Drawing.Size(198, 414);
			this.allUsersPanel.TabIndex = 0;
			this.allUsersPanel.Visible = false;
			// 
			// allUsersListBox
			// 
			this.allUsersListBox.BackColor = System.Drawing.SystemColors.Window;
			this.allUsersListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.allUsersListBox.Cursor = System.Windows.Forms.Cursors.Hand;
			this.allUsersListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.allUsersListBox.FormattingEnabled = true;
			this.allUsersListBox.Location = new System.Drawing.Point(0, 0);
			this.allUsersListBox.Name = "allUsersListBox";
			this.allUsersListBox.Size = new System.Drawing.Size(198, 380);
			this.allUsersListBox.TabIndex = 0;
			this.allUsersListBox.TabStop = false;
			this.allUsersListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_DrawItem);
			this.allUsersListBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox_MeasureItem);
			this.allUsersListBox.SelectedIndexChanged += new System.EventHandler(this.AllUsersListBoxSelectedChanged);
			// 
			// addFriendButton
			// 
			this.addFriendButton.BackColor = global::CryptoMessenger.Properties.Settings.Default.MainFirstColor;
			this.addFriendButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.addFriendButton.Enabled = false;
			this.addFriendButton.FlatAppearance.BorderSize = 0;
			this.addFriendButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(0)))), ((int)(((byte)(75)))));
			this.addFriendButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(25)))), ((int)(((byte)(105)))));
			this.addFriendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.addFriendButton.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.addFriendButton.ForeColor = System.Drawing.Color.White;
			this.addFriendButton.Location = new System.Drawing.Point(0, 384);
			this.addFriendButton.Name = "addFriendButton";
			this.addFriendButton.Size = new System.Drawing.Size(198, 30);
			this.addFriendButton.TabIndex = 0;
			this.addFriendButton.TabStop = false;
			this.addFriendButton.Text = "CRYPTO MESSENGER";
			this.addFriendButton.UseVisualStyleBackColor = true;
			this.addFriendButton.Click += new System.EventHandler(this.addFriendButton_Click);
			// 
			// friendsPanel
			// 
			this.friendsPanel.Controls.Add(this.friendsListBox);
			this.friendsPanel.Controls.Add(this.removeFriendButton);
			this.friendsPanel.Location = new System.Drawing.Point(1, 30);
			this.friendsPanel.Name = "friendsPanel";
			this.friendsPanel.Size = new System.Drawing.Size(198, 414);
			this.friendsPanel.TabIndex = 0;
			// 
			// friendsListBox
			// 
			this.friendsListBox.BackColor = System.Drawing.SystemColors.Window;
			this.friendsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.friendsListBox.Cursor = System.Windows.Forms.Cursors.Hand;
			this.friendsListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.friendsListBox.FormattingEnabled = true;
			this.friendsListBox.Location = new System.Drawing.Point(0, 0);
			this.friendsListBox.Name = "friendsListBox";
			this.friendsListBox.Size = new System.Drawing.Size(198, 380);
			this.friendsListBox.TabIndex = 0;
			this.friendsListBox.TabStop = false;
			this.friendsListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_DrawItem);
			this.friendsListBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox_MeasureItem);
			this.friendsListBox.SelectedIndexChanged += new System.EventHandler(this.FriendsListBoxSelectedChanged);
			// 
			// removeFriendButton
			// 
			this.removeFriendButton.BackColor = global::CryptoMessenger.Properties.Settings.Default.MainFirstColor;
			this.removeFriendButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.removeFriendButton.Enabled = false;
			this.removeFriendButton.FlatAppearance.BorderSize = 0;
			this.removeFriendButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(0)))), ((int)(((byte)(75)))));
			this.removeFriendButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(25)))), ((int)(((byte)(105)))));
			this.removeFriendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.removeFriendButton.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.removeFriendButton.ForeColor = System.Drawing.Color.White;
			this.removeFriendButton.Location = new System.Drawing.Point(0, 384);
			this.removeFriendButton.Name = "removeFriendButton";
			this.removeFriendButton.Size = new System.Drawing.Size(198, 30);
			this.removeFriendButton.TabIndex = 0;
			this.removeFriendButton.TabStop = false;
			this.removeFriendButton.Text = "CRYPTO MESSENGER";
			this.removeFriendButton.UseVisualStyleBackColor = true;
			this.removeFriendButton.Click += new System.EventHandler(this.removeFriendButton_Click);
			// 
			// friendshipRequestsPanel
			// 
			this.friendshipRequestsPanel.Controls.Add(this.outcomeFriendshipRequestsListBox);
			this.friendshipRequestsPanel.Controls.Add(this.incomeFriendshipRequestsListBox);
			this.friendshipRequestsPanel.Controls.Add(this.cancelFriendshipRequestButton);
			this.friendshipRequestsPanel.Controls.Add(this.acceptFriendshipButton);
			this.friendshipRequestsPanel.Controls.Add(this.rejectFriendshipButton);
			this.friendshipRequestsPanel.Controls.Add(this.outcomeFriendshipRequestsLabel);
			this.friendshipRequestsPanel.Controls.Add(this.incomeFriendshipRequestsLabel);
			this.friendshipRequestsPanel.Location = new System.Drawing.Point(1, 30);
			this.friendshipRequestsPanel.Name = "friendshipRequestsPanel";
			this.friendshipRequestsPanel.Size = new System.Drawing.Size(198, 414);
			this.friendshipRequestsPanel.TabIndex = 0;
			this.friendshipRequestsPanel.Visible = false;
			// 
			// outcomeFriendshipRequestsListBox
			// 
			this.outcomeFriendshipRequestsListBox.BackColor = System.Drawing.SystemColors.Window;
			this.outcomeFriendshipRequestsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.outcomeFriendshipRequestsListBox.Cursor = System.Windows.Forms.Cursors.Hand;
			this.outcomeFriendshipRequestsListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.outcomeFriendshipRequestsListBox.FormattingEnabled = true;
			this.outcomeFriendshipRequestsListBox.Location = new System.Drawing.Point(0, 216);
			this.outcomeFriendshipRequestsListBox.Name = "outcomeFriendshipRequestsListBox";
			this.outcomeFriendshipRequestsListBox.Size = new System.Drawing.Size(198, 164);
			this.outcomeFriendshipRequestsListBox.TabIndex = 0;
			this.outcomeFriendshipRequestsListBox.TabStop = false;
			this.outcomeFriendshipRequestsListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_DrawItem);
			this.outcomeFriendshipRequestsListBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox_MeasureItem);
			this.outcomeFriendshipRequestsListBox.SelectedIndexChanged += new System.EventHandler(this.OutcomeRequestsListBoxesSelectedChanged);
			this.outcomeFriendshipRequestsListBox.Enter += new System.EventHandler(this.outcomeRequestsListBox_Enter);
			// 
			// incomeFriendshipRequestsListBox
			// 
			this.incomeFriendshipRequestsListBox.BackColor = System.Drawing.SystemColors.Window;
			this.incomeFriendshipRequestsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.incomeFriendshipRequestsListBox.Cursor = System.Windows.Forms.Cursors.Hand;
			this.incomeFriendshipRequestsListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.incomeFriendshipRequestsListBox.FormattingEnabled = true;
			this.incomeFriendshipRequestsListBox.Location = new System.Drawing.Point(0, 26);
			this.incomeFriendshipRequestsListBox.Name = "incomeFriendshipRequestsListBox";
			this.incomeFriendshipRequestsListBox.Size = new System.Drawing.Size(198, 164);
			this.incomeFriendshipRequestsListBox.TabIndex = 0;
			this.incomeFriendshipRequestsListBox.TabStop = false;
			this.incomeFriendshipRequestsListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_DrawItem);
			this.incomeFriendshipRequestsListBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox_MeasureItem);
			this.incomeFriendshipRequestsListBox.Enter += new System.EventHandler(this.incomeRequestsListBox_Enter);
			// 
			// cancelFriendshipRequestButton
			// 
			this.cancelFriendshipRequestButton.BackColor = global::CryptoMessenger.Properties.Settings.Default.MainFirstColor;
			this.cancelFriendshipRequestButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.cancelFriendshipRequestButton.Enabled = false;
			this.cancelFriendshipRequestButton.FlatAppearance.BorderSize = 0;
			this.cancelFriendshipRequestButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(0)))), ((int)(((byte)(75)))));
			this.cancelFriendshipRequestButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(25)))), ((int)(((byte)(105)))));
			this.cancelFriendshipRequestButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cancelFriendshipRequestButton.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cancelFriendshipRequestButton.ForeColor = System.Drawing.Color.White;
			this.cancelFriendshipRequestButton.Location = new System.Drawing.Point(0, 384);
			this.cancelFriendshipRequestButton.Name = "cancelFriendshipRequestButton";
			this.cancelFriendshipRequestButton.Size = new System.Drawing.Size(198, 30);
			this.cancelFriendshipRequestButton.TabIndex = 0;
			this.cancelFriendshipRequestButton.TabStop = false;
			this.cancelFriendshipRequestButton.Text = "CRYPTO MESSENGER";
			this.cancelFriendshipRequestButton.UseVisualStyleBackColor = true;
			this.cancelFriendshipRequestButton.Click += new System.EventHandler(this.cancelFriendshipRequestButton_Click);
			// 
			// acceptFriendshipButton
			// 
			this.acceptFriendshipButton.BackColor = global::CryptoMessenger.Properties.Settings.Default.MainFirstColor;
			this.acceptFriendshipButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.acceptFriendshipButton.FlatAppearance.BorderSize = 0;
			this.acceptFriendshipButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(0)))), ((int)(((byte)(75)))));
			this.acceptFriendshipButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(25)))), ((int)(((byte)(105)))));
			this.acceptFriendshipButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.acceptFriendshipButton.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.acceptFriendshipButton.ForeColor = System.Drawing.Color.White;
			this.acceptFriendshipButton.Location = new System.Drawing.Point(0, 384);
			this.acceptFriendshipButton.Name = "acceptFriendshipButton";
			this.acceptFriendshipButton.Size = new System.Drawing.Size(98, 30);
			this.acceptFriendshipButton.TabIndex = 0;
			this.acceptFriendshipButton.TabStop = false;
			this.acceptFriendshipButton.Text = "ПРИНЯТЬ";
			this.acceptFriendshipButton.UseVisualStyleBackColor = true;
			this.acceptFriendshipButton.Visible = false;
			this.acceptFriendshipButton.Click += new System.EventHandler(this.acceptFriendshipButton_Click);
			// 
			// rejectFriendshipButton
			// 
			this.rejectFriendshipButton.BackColor = global::CryptoMessenger.Properties.Settings.Default.MainFirstColor;
			this.rejectFriendshipButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.rejectFriendshipButton.FlatAppearance.BorderSize = 0;
			this.rejectFriendshipButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(0)))), ((int)(((byte)(75)))));
			this.rejectFriendshipButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(25)))), ((int)(((byte)(105)))));
			this.rejectFriendshipButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.rejectFriendshipButton.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.rejectFriendshipButton.ForeColor = System.Drawing.Color.White;
			this.rejectFriendshipButton.Location = new System.Drawing.Point(100, 384);
			this.rejectFriendshipButton.Name = "rejectFriendshipButton";
			this.rejectFriendshipButton.Size = new System.Drawing.Size(98, 30);
			this.rejectFriendshipButton.TabIndex = 0;
			this.rejectFriendshipButton.TabStop = false;
			this.rejectFriendshipButton.Text = "ОТКЛОНИТЬ";
			this.rejectFriendshipButton.UseVisualStyleBackColor = true;
			this.rejectFriendshipButton.Visible = false;
			this.rejectFriendshipButton.Click += new System.EventHandler(this.rejectFriendshipButton_Click);
			// 
			// outcomeFriendshipRequestsLabel
			// 
			this.outcomeFriendshipRequestsLabel.AutoSize = true;
			this.outcomeFriendshipRequestsLabel.BackColor = System.Drawing.Color.Transparent;
			this.outcomeFriendshipRequestsLabel.Font = new System.Drawing.Font("Roboto Light", 12F);
			this.outcomeFriendshipRequestsLabel.ForeColor = System.Drawing.SystemColors.GrayText;
			this.outcomeFriendshipRequestsLabel.Location = new System.Drawing.Point(0, 190);
			this.outcomeFriendshipRequestsLabel.Name = "outcomeFriendshipRequestsLabel";
			this.outcomeFriendshipRequestsLabel.Size = new System.Drawing.Size(124, 21);
			this.outcomeFriendshipRequestsLabel.TabIndex = 0;
			this.outcomeFriendshipRequestsLabel.Text = "ВАШИ ЗАЯВКИ:";
			// 
			// incomeFriendshipRequestsLabel
			// 
			this.incomeFriendshipRequestsLabel.AutoSize = true;
			this.incomeFriendshipRequestsLabel.BackColor = System.Drawing.Color.Transparent;
			this.incomeFriendshipRequestsLabel.Font = new System.Drawing.Font("Roboto Light", 12F);
			this.incomeFriendshipRequestsLabel.ForeColor = System.Drawing.SystemColors.GrayText;
			this.incomeFriendshipRequestsLabel.Location = new System.Drawing.Point(0, 0);
			this.incomeFriendshipRequestsLabel.Name = "incomeFriendshipRequestsLabel";
			this.incomeFriendshipRequestsLabel.Size = new System.Drawing.Size(112, 21);
			this.incomeFriendshipRequestsLabel.TabIndex = 0;
			this.incomeFriendshipRequestsLabel.Text = "ЗАЯВКИ ВАМ:";
			// 
			// usersLabelsPanel
			// 
			this.usersLabelsPanel.BackColor = System.Drawing.Color.Transparent;
			this.usersLabelsPanel.Controls.Add(this.friendshipRequestsLabel);
			this.usersLabelsPanel.Controls.Add(this.searchLabel);
			this.usersLabelsPanel.Controls.Add(this.friendsLabel);
			this.usersLabelsPanel.Location = new System.Drawing.Point(0, 0);
			this.usersLabelsPanel.Name = "usersLabelsPanel";
			this.usersLabelsPanel.Size = new System.Drawing.Size(200, 25);
			this.usersLabelsPanel.TabIndex = 0;
			this.usersLabelsPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.friendsTitlePanel_Paint);
			// 
			// friendshipRequestsLabel
			// 
			this.friendshipRequestsLabel.AutoSize = true;
			this.friendshipRequestsLabel.BackColor = System.Drawing.Color.Transparent;
			this.friendshipRequestsLabel.Cursor = System.Windows.Forms.Cursors.Hand;
			this.friendshipRequestsLabel.Font = new System.Drawing.Font("Roboto Light", 12F);
			this.friendshipRequestsLabel.ForeColor = System.Drawing.SystemColors.GrayText;
			this.friendshipRequestsLabel.Location = new System.Drawing.Point(3, 0);
			this.friendshipRequestsLabel.Name = "friendshipRequestsLabel";
			this.friendshipRequestsLabel.Size = new System.Drawing.Size(71, 21);
			this.friendshipRequestsLabel.TabIndex = 0;
			this.friendshipRequestsLabel.Text = "ЗАЯВКИ";
			this.friendshipRequestsLabel.Click += new System.EventHandler(this.friendRequestTitle_Click);
			this.friendshipRequestsLabel.MouseEnter += new System.EventHandler(this.userPanelsLabel_MouseEnter);
			this.friendshipRequestsLabel.MouseLeave += new System.EventHandler(this.userPanelsLabel_MouseLeave);
			// 
			// searchLabel
			// 
			this.searchLabel.AutoSize = true;
			this.searchLabel.BackColor = System.Drawing.Color.Transparent;
			this.searchLabel.Cursor = System.Windows.Forms.Cursors.Hand;
			this.searchLabel.Font = new System.Drawing.Font("Roboto Light", 12F);
			this.searchLabel.ForeColor = System.Drawing.SystemColors.GrayText;
			this.searchLabel.Location = new System.Drawing.Point(141, 0);
			this.searchLabel.Name = "searchLabel";
			this.searchLabel.Size = new System.Drawing.Size(63, 21);
			this.searchLabel.TabIndex = 0;
			this.searchLabel.Text = "ПОИСК";
			this.searchLabel.Click += new System.EventHandler(this.searchTitle_Click);
			this.searchLabel.MouseEnter += new System.EventHandler(this.userPanelsLabel_MouseEnter);
			this.searchLabel.MouseLeave += new System.EventHandler(this.userPanelsLabel_MouseLeave);
			// 
			// friendsLabel
			// 
			this.friendsLabel.AutoSize = true;
			this.friendsLabel.BackColor = System.Drawing.Color.Transparent;
			this.friendsLabel.Cursor = System.Windows.Forms.Cursors.Hand;
			this.friendsLabel.Font = new System.Drawing.Font("Roboto Light", 12F);
			this.friendsLabel.ForeColor = System.Drawing.SystemColors.GrayText;
			this.friendsLabel.Location = new System.Drawing.Point(64, 0);
			this.friendsLabel.Name = "friendsLabel";
			this.friendsLabel.Size = new System.Drawing.Size(72, 21);
			this.friendsLabel.TabIndex = 0;
			this.friendsLabel.Text = "ДРУЗЬЯ";
			this.friendsLabel.Click += new System.EventHandler(this.friendsTitle_Click);
			this.friendsLabel.MouseEnter += new System.EventHandler(this.userPanelsLabel_MouseEnter);
			this.friendsLabel.MouseLeave += new System.EventHandler(this.userPanelsLabel_MouseLeave);
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
			this.loadingLabel.TabIndex = 0;
			this.loadingLabel.Text = "ЗАГРУЗКА...";
			this.loadingLabel.Visible = false;
			// 
			// mainFormPanel
			// 
			this.mainFormPanel.Controls.Add(this.splitContainer);
			this.mainFormPanel.Controls.Add(this.topPanel);
			this.mainFormPanel.Controls.Add(this.usersPanel);
			this.mainFormPanel.Location = new System.Drawing.Point(0, 0);
			this.mainFormPanel.Name = "mainFormPanel";
			this.mainFormPanel.Size = new System.Drawing.Size(700, 500);
			this.mainFormPanel.TabIndex = 0;
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
			this.splitContainer.Panel1.Controls.Add(this.conversationHtmlPanel);
			this.splitContainer.Panel1.Controls.Add(this.activeTalkLabelPanel);
			this.splitContainer.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer_Panel1_Paint);
			this.splitContainer.Panel1MinSize = 150;
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.replyTextfield);
			this.splitContainer.Panel2.Controls.Add(this.sendReplyButton);
			this.splitContainer.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer_Panel2_Paint);
			this.splitContainer.Panel2MinSize = 60;
			this.splitContainer.Size = new System.Drawing.Size(470, 445);
			this.splitContainer.SplitterDistance = 376;
			this.splitContainer.SplitterWidth = 9;
			this.splitContainer.TabIndex = 0;
			this.splitContainer.TabStop = false;
			this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
			this.splitContainer.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer_Paint);
			// 
			// conversationHtmlPanel
			// 
			this.conversationHtmlPanel.AutoScroll = true;
			this.conversationHtmlPanel.AutoScrollMinSize = new System.Drawing.Size(450, 0);
			this.conversationHtmlPanel.BackColor = System.Drawing.SystemColors.Window;
			this.conversationHtmlPanel.Location = new System.Drawing.Point(10, 35);
			this.conversationHtmlPanel.Name = "conversationHtmlPanel";
			this.conversationHtmlPanel.Size = new System.Drawing.Size(450, 330);
			this.conversationHtmlPanel.TabIndex = 0;
			// 
			// activeTalkLabelPanel
			// 
			this.activeTalkLabelPanel.BackColor = System.Drawing.Color.Transparent;
			this.activeTalkLabelPanel.Controls.Add(this.activeTalkLabel);
			this.activeTalkLabelPanel.Location = new System.Drawing.Point(0, 0);
			this.activeTalkLabelPanel.Name = "activeTalkLabelPanel";
			this.activeTalkLabelPanel.Size = new System.Drawing.Size(470, 25);
			this.activeTalkLabelPanel.TabIndex = 0;
			this.activeTalkLabelPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.activeTalkTitlePanel_Paint);
			// 
			// activeTalkLabel
			// 
			this.activeTalkLabel.AutoSize = true;
			this.activeTalkLabel.BackColor = System.Drawing.Color.Transparent;
			this.activeTalkLabel.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.activeTalkLabel.ForeColor = System.Drawing.SystemColors.GrayText;
			this.activeTalkLabel.Location = new System.Drawing.Point(195, 0);
			this.activeTalkLabel.Name = "activeTalkLabel";
			this.activeTalkLabel.Size = new System.Drawing.Size(74, 21);
			this.activeTalkLabel.TabIndex = 0;
			this.activeTalkLabel.Text = "ДИАЛОГ";
			this.activeTalkLabel.SizeChanged += new System.EventHandler(this.activeTalkTitle_SizeChanged);
			// 
			// replyTextfield
			// 
			this.replyTextfield.BackColor = System.Drawing.Color.White;
			this.replyTextfield.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.replyTextfield.Enabled = false;
			this.replyTextfield.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.replyTextfield.ForeColor = System.Drawing.SystemColors.GrayText;
			this.replyTextfield.Location = new System.Drawing.Point(10, 5);
			this.replyTextfield.Multiline = true;
			this.replyTextfield.Name = "replyTextfield";
			this.replyTextfield.Size = new System.Drawing.Size(392, 50);
			this.replyTextfield.TabIndex = 0;
			this.replyTextfield.TabStop = false;
			this.replyTextfield.Text = "ВВЕДИТЕ СООБЩЕНИЕ...";
			this.replyTextfield.TextChanged += new System.EventHandler(this.replyTextfield_TextChanged);
			this.replyTextfield.Enter += new System.EventHandler(this.replyTextfield_Enter);
			this.replyTextfield.Leave += new System.EventHandler(this.replyTextfield_Leave);
			// 
			// sendReplyButton
			// 
			this.sendReplyButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(10)))), ((int)(((byte)(90)))));
			this.sendReplyButton.BackgroundImage = global::CryptoMessenger.Properties.Resources.send;
			this.sendReplyButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.sendReplyButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.sendReplyButton.Enabled = false;
			this.sendReplyButton.FlatAppearance.BorderSize = 0;
			this.sendReplyButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(0)))), ((int)(((byte)(75)))));
			this.sendReplyButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(25)))), ((int)(((byte)(105)))));
			this.sendReplyButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.sendReplyButton.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.sendReplyButton.ForeColor = System.Drawing.Color.White;
			this.sendReplyButton.Location = new System.Drawing.Point(412, 1);
			this.sendReplyButton.Name = "sendReplyButton";
			this.sendReplyButton.Size = new System.Drawing.Size(58, 58);
			this.sendReplyButton.TabIndex = 0;
			this.sendReplyButton.TabStop = false;
			this.sendReplyButton.UseVisualStyleBackColor = true;
			this.sendReplyButton.Click += new System.EventHandler(this.sendReplyButton_Click);
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
			this.usersPanel.ResumeLayout(false);
			this.usersPanel.PerformLayout();
			this.allUsersPanel.ResumeLayout(false);
			this.friendsPanel.ResumeLayout(false);
			this.friendshipRequestsPanel.ResumeLayout(false);
			this.friendshipRequestsPanel.PerformLayout();
			this.usersLabelsPanel.ResumeLayout(false);
			this.usersLabelsPanel.PerformLayout();
			this.mainFormPanel.ResumeLayout(false);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.activeTalkLabelPanel.ResumeLayout(false);
			this.activeTalkLabelPanel.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private Panel topPanel;
        private Label appNameLabel;
        private MyButton minimizeButton;
        private MyButton closeButton;
        private Panel usersPanel; 
        private TextBox replyTextfield;
        private MyButton sendReplyButton;
        private Label activeTalkLabel;
        private Panel activeTalkLabelPanel;
        private Panel mainFormPanel;
		private PictureBox iconBox;
		private MySplitContainer splitContainer;
		private Panel usersLabelsPanel; 
		private Label friendsLabel;
		private ListBox friendsListBox;
		private Label searchLabel;
		private Label loadingLabel;
		private MyButton removeFriendButton;
		private Panel friendsPanel;
		private Panel friendshipRequestsPanel;
		private Panel allUsersPanel;
		private ListBox allUsersListBox;
		private MyButton addFriendButton;
		private Label friendshipRequestsLabel;
		private Label outcomeFriendshipRequestsLabel;
		private Label incomeFriendshipRequestsLabel;
		private ListBox outcomeFriendshipRequestsListBox;
		private ListBox incomeFriendshipRequestsListBox;
		private MyButton cancelFriendshipRequestButton;
		private MyButton acceptFriendshipButton;
		private MyButton rejectFriendshipButton;
		private HtmlPanel conversationHtmlPanel;
	}
}
