using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CryptoMessenger.Behaviors
{
	/// <summary>
	/// Class for listbox behaviors.
	/// </summary>
	class ListBoxBehavior
	{
		static readonly Dictionary<ListBox, Capture> Associations =
		   new Dictionary<ListBox, Capture>();

		public static readonly DependencyProperty ScrollOnNewItemProperty =
			DependencyProperty.RegisterAttached(
				"ScrollOnNewItem",
				typeof(bool),
				typeof(ListBoxBehavior),
				new UIPropertyMetadata(false, OnScrollOnNewItemChanged));

		public static bool GetScrollOnNewItem(DependencyObject obj)
		{
			return (bool)obj.GetValue(ScrollOnNewItemProperty);
		}

		public static void SetScrollOnNewItem(DependencyObject obj, bool value)
		{
			obj.SetValue(ScrollOnNewItemProperty, value);
		}

		public static void OnScrollOnNewItemChanged(DependencyObject d,	DependencyPropertyChangedEventArgs e)
		{
			var listBox = d as ListBox;
			if (listBox == null) return;

			bool oldValue = (bool)e.OldValue, newValue = (bool)e.NewValue;
			if (newValue == oldValue) return;

			if (newValue)
			{
				listBox.Loaded += ListBox_Loaded;
				listBox.Unloaded += ListBox_Unloaded;
				var itemsSourcePropertyDescriptor = TypeDescriptor.GetProperties(listBox)["ItemsSource"];
				itemsSourcePropertyDescriptor.AddValueChanged(listBox, ListBox_ItemsSourceChanged);
			}
			else
			{
				listBox.Loaded -= ListBox_Loaded;
				listBox.Unloaded -= ListBox_Unloaded;
				if (Associations.ContainsKey(listBox))
					Associations[listBox].Dispose();
				var itemsSourcePropertyDescriptor = TypeDescriptor.GetProperties(listBox)["ItemsSource"];
				itemsSourcePropertyDescriptor.RemoveValueChanged(listBox, ListBox_ItemsSourceChanged);
			}
		}

		static void ListBox_Loaded(object sender, RoutedEventArgs e)
		{
			var listBox = (ListBox)sender;
			var incc = listBox.Items as INotifyCollectionChanged;
			if (incc == null) return;
			listBox.Loaded -= ListBox_Loaded;
			Associations[listBox] = new Capture(listBox);
		}

		static void ListBox_Unloaded(object sender, RoutedEventArgs e)
		{
			var listBox = (ListBox)sender;
			if (Associations.ContainsKey(listBox))
				Associations[listBox].Dispose();
			listBox.Unloaded -= ListBox_Unloaded;
		}

		private static void ListBox_ItemsSourceChanged(object sender, EventArgs e)
		{
			var listBox = (ListBox)sender;
			if (Associations.ContainsKey(listBox))
				Associations[listBox].Dispose();
			Associations[listBox] = new Capture(listBox);
		}


		class Capture : IDisposable
		{
			private readonly ListBox listBox;
			private readonly INotifyCollectionChanged incc;

			public Capture(ListBox listBox)
			{
				this.listBox = listBox;
				incc = listBox.ItemsSource as INotifyCollectionChanged;
				if (incc != null)
				{
					incc.CollectionChanged += incc_CollectionChanged;

					if (listBox.Items.Count > 0)
					{
						var scrollViewer = GetDescendantByType(listBox, typeof(ScrollViewer)) as ScrollViewer;
						if (scrollViewer.ScrollableHeight == scrollViewer.VerticalOffset)
						{
							listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
							listBox.SelectedItem = listBox.Items[listBox.Items.Count - 1];
						}
					}
				}
			}

			private void incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				if (e.Action == NotifyCollectionChangedAction.Add)
				{
					var scrollViewer = GetDescendantByType(listBox, typeof(ScrollViewer)) as ScrollViewer;

					if (scrollViewer.ScrollableHeight == scrollViewer.VerticalOffset)
					{
						listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
						listBox.SelectedItem = listBox.Items[listBox.Items.Count - 1];
					}
				}
			}

			public Visual GetDescendantByType(Visual element, Type type)
			{
				if (element == null)
				{
					return null;
				}
				if (element.GetType() == type)
				{
					return element;
				}
				Visual foundElement = null;
				if (element is FrameworkElement)
				{
					(element as FrameworkElement).ApplyTemplate();
				}
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
				{
					Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
					foundElement = GetDescendantByType(visual, type);
					if (foundElement != null)
					{
						break;
					}
				}
				return foundElement;
			}

			public void Dispose()
			{
				if (incc != null)
					incc.CollectionChanged -= incc_CollectionChanged;
			}
		}
	}
}
