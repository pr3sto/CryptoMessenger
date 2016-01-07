﻿using System.Windows.Forms;

namespace CryptoMessenger
{
	partial class MySplitContainer
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
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		// resize when drag

		//assign this to the SplitContainer's MouseDown event
		private void splitCont_MouseDown(object sender, MouseEventArgs e)
		{
			// This disables the normal move behavior
			((SplitContainer)sender).IsSplitterFixed = true;
		}

		//assign this to the SplitContainer's MouseUp event
		private void splitCont_MouseUp(object sender, MouseEventArgs e)
		{
			// This allows the splitter to be moved normally again
			((SplitContainer)sender).IsSplitterFixed = false;
		}

		//assign this to the SplitContainer's MouseMove event
		private void splitCont_MouseMove(object sender, MouseEventArgs e)
		{
			// Check to make sure the splitter won't be updated by the
			// normal move behavior also
			if (((SplitContainer)sender).IsSplitterFixed)
			{
				// Make sure that the button used to move the splitter
				// is the left mouse button
				if (e.Button.Equals(MouseButtons.Left))
				{
					// Checks to see if the splitter is aligned Vertically
					if (((SplitContainer)sender).Orientation.Equals(Orientation.Vertical))
					{
						// Only move the splitter if the mouse is within
						// the appropriate bounds
						if (e.X > 0 && e.X < ((SplitContainer)sender).Width)
						{
							// Move the splitter & force a visual refresh
							((SplitContainer)sender).SplitterDistance = e.X;
							((SplitContainer)sender).Refresh();
						}
					}
					// If it isn't aligned vertically then it must be
					// horizontal
					else
					{
						// Only move the splitter if the mouse is within
						// the appropriate bounds
						if (e.Y > 0 && e.Y < ((SplitContainer)sender).Height)
						{
							// Move the splitter & force a visual refresh
							((SplitContainer)sender).SplitterDistance = e.Y;
							((SplitContainer)sender).Refresh();
						}
					}
				}
				// If a button other than left is pressed or no button
				// at all
				else
				{
					// This allows the splitter to be moved normally again
					((SplitContainer)sender).IsSplitterFixed = false;
				}
			}
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitCont_MouseDown);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitCont_MouseUp);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.splitCont_MouseMove);
		}

		#endregion
	}
}
