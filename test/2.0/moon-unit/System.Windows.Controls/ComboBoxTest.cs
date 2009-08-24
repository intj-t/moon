//
// ComboBox Unit Tests
//
// Contact:
//   Moonlight List (moonlight-list@lists.ximian.com)
//
// Copyright 2009 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Shapes;
using Mono.Moonlight.UnitTesting;
using System.Collections.Specialized;
using Microsoft.Silverlight.Testing;
using System.Windows.Media;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace MoonTest.System.Windows.Controls {

	public struct Value {
		public string MethodName;
		public object [] MethodParams;
		public object ReturnValue;
	}

	public class FakeComboBox : ComboBox {
		public List<Value> methods = new List<Value> ();

		public bool CallBaseOnDropDown { get; set; }
		public bool CallBaseOnItemsChanged { get; set; }
		public DependencyObject ContainerItem {
			get; set;
		}
		public Popup TemplatePopup {
			get { return (Popup) GetTemplateChild ("Popup"); }
		}
		
		public FakeComboBox ()
		{
			CallBaseOnItemsChanged = true;
			CallBaseOnDropDown = true;
			this.SelectionChanged += (o, e) => methods.Add (new Value { MethodName = "SelectionChangedEvent", ReturnValue = e });
			this.DropDownClosed += delegate { methods.Add (new Value { MethodName = "DropDownClosedEvent" }); };
			this.DropDownOpened += delegate { methods.Add (new Value { MethodName = "DropDownOpenedEvent" }); };
		}

		protected override Size ArrangeOverride (Size arrangeBounds)
		{
			methods.Add (new Value { MethodParams = new object [] { arrangeBounds }, ReturnValue = base.ArrangeOverride (arrangeBounds) });
			return (Size) methods.Last ().ReturnValue;
		}

		public void ClearContainerForItemOverride_ (global::System.Windows.DependencyObject element, object item)
		{
			ClearContainerForItemOverride (element, item);
		}

		protected override void ClearContainerForItemOverride (global::System.Windows.DependencyObject element, object item)
		{
			methods.Add (new Value { MethodName = "ClearContainerForItemOverride", MethodParams = new object [] { element, item } });
			base.ClearContainerForItemOverride (element, item);
		}

		protected override global::System.Windows.DependencyObject GetContainerForItemOverride ()
		{
			if (ContainerItem != null)
				return ContainerItem;
			
			methods.Add (new Value { ReturnValue = base.GetContainerForItemOverride () });
			return (DependencyObject) methods.Last ().ReturnValue;
		}
		
		public bool IsItemItsOwnContainerOverride_ (object item)
		{
			return IsItemItsOwnContainerOverride (item);
		}

		protected override bool IsItemItsOwnContainerOverride (object item)
		{
			return base.IsItemItsOwnContainerOverride (item);
		}

		protected override global::System.Windows.Size MeasureOverride (global::System.Windows.Size availableSize)
		{
			return base.MeasureOverride (availableSize);
		}

		public override void OnApplyTemplate ()
		{
			base.OnApplyTemplate ();
		}

		protected override void OnDropDownClosed (EventArgs e)
		{
			if (!CallBaseOnDropDown)
				return;
			methods.Add (new Value { MethodName = "OnDropDownClosed", MethodParams = new object [] { e } });
			base.OnDropDownClosed (e);
		}

		protected override void OnDropDownOpened (EventArgs e)
		{
			if (!CallBaseOnDropDown)
				return;
			methods.Add (new Value { MethodName = "OnDropDownOpened", MethodParams = new object [] { e } });
			base.OnDropDownOpened (e);
		}

		protected override void OnItemsChanged (NotifyCollectionChangedEventArgs e)
		{
			methods.Add (new Value { MethodName = "OnItemsChanged", MethodParams = new object [] { e } });

			if (CallBaseOnItemsChanged)
				base.OnItemsChanged (e);
		}

		public void PrepareContainerForItemOverride_ (DependencyObject element, object item)
		{
			PrepareContainerForItemOverride (element, item);
		}

		protected override void PrepareContainerForItemOverride (global::System.Windows.DependencyObject element, object item)
		{
			methods.Add (new Value { MethodName = "PrepareContainerForItemOverride", MethodParams = new object [] { element, item } });
			base.PrepareContainerForItemOverride (element, item);
		}
	}

	[TestClass]
	public partial class ComboBoxTest : SilverlightTest {

		[TestMethod]
		public void AddTest2 ()
		{
			FakeComboBox c = new FakeComboBox ();
			Assert.AreEqual (-1, c.SelectedIndex);

			c.Items.Add (new object ());
			Assert.AreEqual (-1, c.SelectedIndex);

			c.SelectedIndex = 0;
			c.Items.Add (new object ());
			Assert.AreEqual (0, c.SelectedIndex);
		}
		
		[TestMethod]
		public void ClearTest ()
		{
			FakeComboBox c = new FakeComboBox ();
			c.Items.Add (new object ());
			c.Items.Add (new object ());
			c.Items.Add (new object ());

			c.SelectedIndex = 0;
			c.methods.Clear ();

			// What happens when we clear the items
			c.Items.Clear ();
			Assert.IsNull (c.SelectedItem, "#1");
			Assert.AreEqual (-1, c.SelectedIndex, "#2");
			Assert.AreEqual (2, c.methods.Count);
			Assert.AreEqual ("OnItemsChanged", c.methods [0].MethodName, "#3");
			Assert.AreEqual ("SelectionChangedEvent", c.methods [1].MethodName, "#4");
		}

		[TestMethod]
		public void DefaultValues ()
		{
			ComboBox b = new ComboBox ();
			Assert.IsFalse (b.IsDropDownOpen, "#1");
			Assert.IsFalse (b.IsEditable, "#2");
			Assert.IsFalse (b.IsSelectionBoxHighlighted, "#3");
			Assert.IsNull (b.ItemContainerStyle, "#4");
			Assert.AreEqual (double.PositiveInfinity, b.MaxDropDownHeight, "#5");
			Assert.IsNull (b.SelectionBoxItem, "#6");
			Assert.IsNull (b.SelectionBoxItemTemplate, "#7");
			Assert.AreEqual (-1, b.SelectedIndex, "#8");
			
			Assert.AreEqual (ScrollViewer.GetHorizontalScrollBarVisibility (b), ScrollBarVisibility.Auto, "Horizontal Scroll Vis"); // Fails in Silverlight 3
			Assert.AreEqual (ScrollViewer.GetVerticalScrollBarVisibility (b), ScrollBarVisibility.Auto, "Vertical Scroll Vis");
		}

		[TestMethod]
		public void InsertTest ()
		{
			object orig = new object ();
			FakeComboBox c = new FakeComboBox ();
			c.Items.Add (orig);
			c.SelectedIndex = 0;
			c.methods.Clear ();

			c.Items.Insert (0, new object ());

			// WTF? Why is there a remove, then add, then replace? Surely this is just a replace...
			Assert.AreEqual (1, c.methods.Count, "#1");
			Assert.AreEqual ("OnItemsChanged", c.methods [0].MethodName, "#2");
			Assert.AreEqual (NotifyCollectionChangedAction.Add, ((NotifyCollectionChangedEventArgs) c.methods [0].MethodParams [0]).Action, "#3");

			Assert.AreEqual (1, c.SelectedIndex, "#8");
			Assert.AreEqual (orig, c.SelectedItem, "#9");
		}
		
		[TestMethod]
		public void InvalidValues ()
		{
			ComboBox b = new ComboBox ();
			b.MaxDropDownHeight = -1;
			Assert.AreEqual (-1.0, b.MaxDropDownHeight, "#1");
			b.MaxDropDownHeight = -11000.0;
			Assert.AreEqual (-11000.0, b.MaxDropDownHeight, "#2");
			b.MaxDropDownHeight = 0;
			Assert.AreEqual (0, b.MaxDropDownHeight, "#3");
			Assert.Throws<ArgumentException> (delegate {
				b.Items.Add (null);
			});
			b.Items.Add (new Rectangle ());
			Assert.IsNull (b.SelectedItem);
			b.Items.Add ("This is a string");
			b.Items.Add (new ComboBoxItem { Content = "Yeah" });

			Assert.AreEqual (-1, b.SelectedIndex, "#4");
			b.SelectedIndex = 0;
			Assert.IsTrue (b.SelectedItem is Rectangle, "#5");
			b.SelectedItem = new object ();
			Assert.IsTrue (b.SelectedItem is Rectangle, "#6");
			b.SelectedItem = b.Items [1];
			Assert.AreEqual (1, b.SelectedIndex, "#7");
		}

		[TestMethod]
		public void TestOverrides ()
		{
			FakeComboBox b = new FakeComboBox ();
			object o = new object ();
			b.Items.Add (o);
			Assert.AreEqual (0, b.Items.IndexOf (b.Items [0]), "#0");
			Assert.AreEqual (1, b.methods.Count, "#1");
			Assert.AreEqual ("OnItemsChanged", b.methods [0].MethodName, "#2");
			b.IsDropDownOpen = true;
			Assert.AreEqual ("OnDropDownOpened", b.methods [1].MethodName, "#3");
			Assert.AreEqual ("DropDownOpenedEvent", b.methods [2].MethodName, "#4");
			b.IsDropDownOpen = false;
			Assert.AreEqual ("OnDropDownClosed", b.methods [3].MethodName, "#5");
			Assert.AreEqual ("DropDownClosedEvent", b.methods [4].MethodName, "#6");
			b.SelectedItem = new object ();
			Assert.AreEqual (5, b.methods.Count, "#7");
			b.SelectedItem = b.Items [0];
			Assert.AreEqual (6, b.methods.Count, "#8");
			Assert.AreEqual ("SelectionChangedEvent", b.methods [5].MethodName);
			b.SelectedIndex = -1;
			Assert.AreEqual (null, b.SelectedItem, "#9");
			Assert.AreEqual (7, b.methods.Count, "#10");
			Assert.AreEqual ("SelectionChangedEvent", b.methods [6].MethodName, "#11");
			b.SelectedItem = b.Items [0];
			b.methods.Clear ();
			b.Items.RemoveAt (0);
			Assert.AreEqual (1, b.methods.Count, "#10"); // Fails in Silverlight 3
			Assert.AreEqual ("OnItemsChanged", b.methods [0].MethodName, "#11");
			Assert.AreEqual (o, b.SelectedItem, "#12");
			Assert.AreEqual (0, b.SelectedIndex, "#13");
		}

		[TestMethod]
		public void AddTest ()
		{
			FakeComboBox box = new FakeComboBox ();
			box.Items.Add ("blah");
			Assert.AreEqual (1, box.methods.Count, "#1");
			Assert.AreEqual ("OnItemsChanged", box.methods [0].MethodName, "#2");
			NotifyCollectionChangedEventArgs e = (NotifyCollectionChangedEventArgs) box.methods [0].MethodParams[0];
			Assert.AreEqual (null, e.OldItems, "#3");
			Assert.AreEqual (-1, e.OldStartingIndex, "#4");
			Assert.IsNotNull (e.NewItems, "#5");
			Assert.AreEqual (1, e.NewItems.Count, "#6");
			Assert.AreEqual ("blah", e.NewItems [0], "#7");
			Assert.AreEqual (0, e.NewStartingIndex, "#8");
		}
		
		[TestMethod]
		public void ClearContainerForItemOverride ()
		{
			FakeComboBox ic = new FakeComboBox ();
			Assert.Throws<NullReferenceException> (() => ic.ClearContainerForItemOverride_ (null, null));
			Assert.Throws<NullReferenceException> (() => ic.ClearContainerForItemOverride_ (null, new object ()));
			Assert.Throws<InvalidCastException> (() => ic.ClearContainerForItemOverride_ (ic, null));
		}

		[TestMethod]
		public void ClearContainerForItemOverride2 ()
		{
			// Fails in Silverlight 3
			FakeComboBox ic = new FakeComboBox ();
			ListBoxItem item = new ListBoxItem ();
			item.Content = new object ();
			item.ContentTemplate = new DataTemplate ();
			item.Style = new Style (typeof (ListBoxItem));
			ic.ClearContainerForItemOverride_ (item, item);
			Assert.IsNull (item.Content);
			Assert.IsNotNull (item.Style);
			Assert.IsNotNull (item.ContentTemplate);
			ic.ClearContainerForItemOverride_ (item, null);
		}

		[TestMethod]
		public void ClearContainerForItemOverride3 ()
		{
			// Fails in Silverlight 3
			FakeComboBox ic = new FakeComboBox { ItemContainerStyle = new Style (typeof (ListBoxItem)) };
			ListBoxItem item = new ListBoxItem ();
			item.Content = new object ();
			item.ContentTemplate = new DataTemplate ();
			item.Style = ic.ItemContainerStyle;
			ic.ClearContainerForItemOverride_ (item, item);
			Assert.IsNull (item.Content);
			Assert.IsNotNull (item.Style);
			Assert.IsNotNull (item.ContentTemplate);
			ic.ClearContainerForItemOverride_ (item, null);
		}

		[TestMethod]
		public void ClearContainerForItemOverride4 ()
		{
			FakeComboBox box = new FakeComboBox ();

			ListBoxItem listItem = new ListBoxItem { Content = "Content", IsSelected = true };
			ComboBoxItem comboItem = new ComboBoxItem { Content = "Content", IsSelected = true };

			Assert.Throws<NullReferenceException> (() => box.ClearContainerForItemOverride_ (null, null), "#1");
			Assert.Throws<InvalidCastException> (() => box.ClearContainerForItemOverride_ (new Rectangle (), null), "#2");

			box.ClearContainerForItemOverride_ (listItem, null);
			box.ClearContainerForItemOverride_ (comboItem, null);

			Assert.IsNull (listItem.Content, "#3");
			Assert.IsNull (comboItem.Content, "#4");

			Assert.IsFalse (listItem.IsSelected, "#5"); // Fails in Silverlight 3
			Assert.IsFalse (comboItem.IsSelected, "#6");
		}

		[TestMethod]
		[Asynchronous]
		[Ignore ("Throws an internal exception of type MS.Internal.WrappedException. Can/should we replicate this?")]
		public void ContainerItemTest ()
		{
			FakeComboBox box = new FakeComboBox ();
			box.ApplyTemplate ();
			box.ContainerItem = new Storyboard ();
			CreateAsyncTest (box,
				() => box.IsDropDownOpen = true,
				() => Assert.Throws<Exception> (() => box.Items.Add (new object ()))
			);
		}

		class ConcreteFrameworkElement : FrameworkElement { }

		[TestMethod]
		[Asynchronous]
		public void ContainerItemTest2 ()
		{
			FakeComboBox box = new FakeComboBox ();
			box.ApplyTemplate ();
			box.ContainerItem = new ConcreteFrameworkElement (); 
			CreateAsyncTest (box,
				() => box.IsDropDownOpen = true,
				() => Assert.Throws<InvalidCastException> (() => box.Items.Add (new object ()))
			);
		}

		[TestMethod]
		[Asynchronous]
		public void ContainerItemTest3 ()
		{
			FakeComboBox box = new FakeComboBox ();
			box.ApplyTemplate ();
			box.ContainerItem = new ListBoxItem ();
			CreateAsyncTest (box,
				() => box.IsDropDownOpen = true,
				() => box.Items.Add (new object ())
			);
		}

		[TestMethod]
		[Asynchronous]
		public void ContainerItemTest4 ()
		{
			FakeComboBox box = new FakeComboBox ();
			box.ApplyTemplate ();
			box.ContainerItem = new Rectangle ();
			CreateAsyncTest (box,
				() => box.IsDropDownOpen = true,
				() => Assert.Throws<InvalidCastException> (() => box.Items.Add (new object ()))
			);
		}

		[TestMethod]
		[Asynchronous]
		[Ignore ("Throws an internal exception of type MS.Internal.WrappedException. Can/should we replicate this?")]
		public void ContainerItemTest5 ()
		{
			FakeComboBox box = new FakeComboBox ();
			box.ApplyTemplate ();
			box.ContainerItem = new ComboBoxItem ();
			CreateAsyncTest (box,
				() => box.IsDropDownOpen = true,
				() => {
					box.Items.Add (new object ()); },
				() => {
					box.Items.Add (new object ()); }
			);
		}

		[TestMethod]
		[Asynchronous]
		public void ItemParentTest ()
		{
			ComboBox box = new ComboBox ();
			ComboBoxItem item = new ComboBoxItem { Content = "Just a string" };
			box.Items.Add (item);
			Assert.IsNull (VisualTreeHelper.GetParent (item), "#1");
			Assert.AreSame (box, item.Parent, "#2");
			CreateAsyncTest (box,
				() => {
					Assert.IsNull (VisualTreeHelper.GetParent (item), "#3");
					Assert.AreSame (box, item.Parent, "#4");
				}
			);
		}

		[TestMethod]
		[Asynchronous]
		public void ItemParentTest2 ()
		{
			bool loaded = false;
			Rectangle content = new Rectangle { Width = 100, Height = 100, Fill = new SolidColorBrush (Colors.Green) };
			ComboBoxItem item = new ComboBoxItem { Content = content };
			ComboBox box = new ComboBox ();
			box.Items.Add (item);
			box.Loaded += (o, e) => loaded = true;

			TestPanel.Children.Add (box);

			Assert.IsNull (VisualTreeHelper.GetParent (item), "#1");
			Assert.AreSame (box, item.Parent, "#2");
			Assert.AreEqual (item, content.Parent, "#3");
			Assert.IsNull (VisualTreeHelper.GetParent (content), "#4");
			EnqueueConditional (() => loaded, "#5");
			Enqueue (() => Assert.IsNull (VisualTreeHelper.GetParent (item), "#6"));
			Enqueue (() => Assert.AreSame (box, item.Parent, "#7"));
			Enqueue (() => {
				Assert.IsNull (VisualTreeHelper.GetParent (content), "#8");
				Assert.AreEqual (item, content.Parent, "#9");
				Assert.AreSame (box, item.Parent, "#10");
			});
			EnqueueTestComplete ();
		}
		
		[TestMethod]
		[Asynchronous]
		[MoonlightBug]
		public void ItemParentTest2b ()
		{
			bool loaded = false;
			Rectangle content = new Rectangle { Width = 100, Height = 100, Fill = new SolidColorBrush (Colors.Green) };
			ComboBoxItem item = new ComboBoxItem { Content = content };
			ComboBox box = new ComboBox ();
			box.Items.Add (item);
			box.SelectedIndex = 0;
			box.Loaded += (o, e) => loaded = true;
			
			TestPanel.Children.Add (box);

			Assert.IsNull (VisualTreeHelper.GetParent (item), "#1");
			Assert.AreSame (box, item.Parent, "#2");
			Assert.AreEqual (item, content.Parent, "#3");
			Assert.IsNull (VisualTreeHelper.GetParent (content), "#4");
			EnqueueConditional (() => loaded, "#5");
			Enqueue (() => Assert.IsNull (VisualTreeHelper.GetParent (item), "#6"));
			Enqueue (() => Assert.AreSame (box, item.Parent, "#7"));
			Enqueue (() => {
				Assert.IsNull (VisualTreeHelper.GetParent (item), "#11");
				Assert.IsNotNull (VisualTreeHelper.GetParent (content), "#8");
				Assert.IsInstanceOfType<ContentPresenter> (VisualTreeHelper.GetParent (content), "#8b");
				Assert.AreEqual (item, content.Parent, "#9"); // Fails in Silverlight 3
				Assert.AreSame (box, item.Parent, "#10");
				box.SelectedItem = null;
			});
			Enqueue (() => {
				Assert.IsNull (VisualTreeHelper.GetParent (item), "#11");
				Assert.IsNull (VisualTreeHelper.GetParent (content), "#12");
			});
			EnqueueTestComplete ();
		}

		[TestMethod]
		[Asynchronous]
		public void ItemParentTest3 ()
		{
			bool loaded = false;
			bool opened = false;
			Rectangle content = new Rectangle { Width = 100, Height = 100, Fill = new SolidColorBrush (Colors.Green) };
			ComboBoxItem item = new ComboBoxItem { Content = content, Name = "Item" };
			ComboBox box = new ComboBox ();
			box.Items.Add (item);
			box.DropDownOpened += (o, e) => opened = true;
			box.DropDownClosed += (o, e) => opened = false;
			box.Loaded += (o, e) => loaded = true;

			TestPanel.Children.Add (box);
			EnqueueConditional (() => loaded, "#3");
			Enqueue (() => box.IsDropDownOpen = true);
			EnqueueConditional (() => opened);
			Enqueue (() => {
				AssertItemHasPresenter (box, item);
				Assert.AreEqual (0, VisualTreeHelper.GetChildrenCount ((Rectangle) item.Content));
			});
			Enqueue (() => Assert.AreSame (box, item.Parent, "#5"));
			EnqueueTestComplete ();
		}

		void AssertItemHasPresenter (ComboBox box, ComboBoxItem item)
		{
			Assert.AreEqual (box, item.Parent, "#a");
			Assert.VisualParent (item, new VisualNode<StackPanel>("#b"));
			Assert.VisualChildren (item,
				new VisualNode<Grid> ("#c",
					new VisualNode<Rectangle> ("#c1"),
					new VisualNode<Rectangle> ("#c2"),
					new VisualNode<ContentPresenter> ("#c3",
						new VisualNode<Rectangle> ("#e", (r) => Assert.AreSame (r, item.Content))
					),
					new VisualNode<Rectangle> ("#c4")
				)
			);
		}

		[TestMethod]
		[Asynchronous]
		public void ItemParentTest4 ()
		{
			bool loaded = false;
			bool opened = false;
			Rectangle content = new Rectangle { Width = 100, Height = 100, Fill = new SolidColorBrush (Colors.Green) };
			ComboBoxItem item = new ComboBoxItem { Content = content };
			ComboBox box = new ComboBox ();
			box.Items.Add (item);
			box.DropDownOpened += (o, e) => opened = true;
			box.DropDownClosed += (o, e) => opened = false;
			box.Loaded += (o, e) => loaded = true;

			TestPanel.Children.Add (box);

			Assert.IsNull (VisualTreeHelper.GetParent (item), "#1");
			Assert.AreSame (box, item.Parent, "#2");

			EnqueueConditional (() => loaded, "#3");
			Enqueue (() => Assert.IsNull (VisualTreeHelper.GetParent (item), "#4"));
			Enqueue (() => Assert.AreSame (box, item.Parent, "#5"));
			EnqueueTestComplete ();
		}

		[TestMethod]
		[Asynchronous]
		public void ItemParentTest5 ()
		{
			bool loaded = false;
			bool opened = false;
			Rectangle content = new Rectangle {
				Fill = new SolidColorBrush (Colors.Brown),
				Width = 100,
				Height = 50
			};
			FakeComboBox box = new FakeComboBox { Width = 50, Height = 50 };
			ComboBoxItem item = new ComboBoxItem { Content = content };
			StringBuilder sb = new StringBuilder ();
			box.DropDownOpened += delegate { opened = true; };
			box.DropDownClosed += delegate { opened = false; };
			box.Items.Add (item);
			box.SelectedIndex = 0;
			box.Loaded += delegate { loaded = true; };
			TestPanel.Children.Add (box);

			
			EnqueueConditional (() => loaded);
			Enqueue (() => box.ApplyTemplate ());
			Enqueue (() => box.IsDropDownOpen = true);
			EnqueueConditional (() => opened);
			Enqueue (() => Assert.IsNotNull (VisualTreeHelper.GetParent (item), "#0"));
			Enqueue (() => Assert.IsInstanceOfType<StackPanel> (VisualTreeHelper.GetParent (item), "#1"));
			Enqueue (() => Assert.AreSame (box, item.Parent, "#2"));
			Enqueue (() => Assert.AreEqual (content, item.Content, "#2b"));
			Enqueue (() => Assert.AreEqual (item, content.Parent, "2c"));
			Enqueue (() => Assert.IsInstanceOfType<ContentPresenter> (VisualTreeHelper.GetParent ((Rectangle) item.Content), "#3"));
			Enqueue (() => Assert.AreSame (item, ((Rectangle) item.Content).Parent, "#4"));

			Enqueue (() => box.IsDropDownOpen = false);
			EnqueueConditional (() => !opened, "#5");
			
			Enqueue (() => Assert.IsNotNull (VisualTreeHelper.GetParent (item), "#6"));
			Enqueue (() => Assert.IsInstanceOfType<StackPanel> (VisualTreeHelper.GetParent (item), "#6b"));
			Enqueue (() => Assert.AreSame (box, item.Parent, "#7"));
			Enqueue (() => Assert.IsNull (item.Content, "#8"));
			EnqueueTestComplete ();
		}

		[TestMethod]
		public void ItemsChangedTest ()
		{
			// Are SelectedItem and SelectedIndex updated in the base method or before it's invoked?
			FakeComboBox c = new FakeComboBox { CallBaseOnItemsChanged = false };
			c.Items.Add (new object ());
			c.methods.Clear ();

			c.SelectedItem = c.Items [0];
			Assert.AreEqual (0, c.SelectedIndex, "#1");
			Assert.AreEqual (c.Items[0], c.SelectedItem, "#2");

			c.Items.Insert (0, new object ());
			Assert.AreEqual (0, c.SelectedIndex, "#3");
			Assert.AreEqual (c.Items[1], c.SelectedItem, "#4");
		}

		[TestMethod]
		[Asynchronous]
		[MoonlightBug ("Failing because we don't create the ListBoxItems until the popup is opened for the first time")]
		public void ItemTemplateTest ()
		{
			ComboBox box = (ComboBox) XamlReader.Load(@"
<ComboBox	xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
			xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
	<ComboBox.ItemTemplate>
		<DataTemplate>
			<TextBlock Text=""{Binding Name}"" />
		</DataTemplate>
	</ComboBox.ItemTemplate>
</ComboBox>");
			box.Items.Add (new object());
			box.SelectedItem = box.Items[0];
			
			// Capture the contentpresenter from the ComboBoxs visual tree
			// and check that it's using the ComboBox DataTemplate as its
			// ContentTemplate.
			CreateAsyncTest (box, () => {
				Assert.AreEqual (box.Items [0], box.SelectionBoxItem, "#1");
				Assert.AreEqual (box.ItemTemplate, box.SelectionBoxItemTemplate, "#2");
				
				ContentPresenter presenter = null;
				Assert.VisualChildren (box,
					new VisualNode<Grid> ("#a",
						new VisualNode<Border> ("#b",
							new VisualNode<Grid> ("#c",
								new VisualNode<ToggleButton> ("#d", (VisualNode[]) null),
								new VisualNode<ContentPresenter> ("#e", p => presenter = p, null)
							)
						),
						new VisualNode<Rectangle> ("#f"),
						new VisualNode<Rectangle> ("#g"),
						new VisualNode<Popup> ("#h")
					)
				);

				Assert.IsInstanceOfType<DataTemplate> (presenter.ReadLocalValue (ContentPresenter.ContentTemplateProperty), "#2");
				Assert.AreEqual (box.ItemTemplate, presenter.ContentTemplate, "#3");
			});
		}
		
		[TestMethod]
		[Asynchronous]
		public void FocusTest ()
		{
			ComboBox box = new ComboBox ();
			Assert.IsFalse (ComboBox.GetIsSelectionActive (box));
			CreateAsyncTest (box,
				() => Assert.IsTrue (box.Focus (), "#1"),
				() => {
					Assert.IsFalse (ComboBox.GetIsSelectionActive (box), "#2");
					box.Items.Add ("string");
					box.SelectedItem = box.Items [0];
				},
				() => Assert.IsFalse (ComboBox.GetIsSelectionActive (box), "#3")
			);
		}
		
		[TestMethod]
		[Asynchronous]
		public void OnDropDownMethodsTest ()
		{
			bool opened = false;
			FakeComboBox box = new FakeComboBox { CallBaseOnDropDown = false };
			TestPanel.Children.Add (box);

			box.DropDownOpened += delegate { opened = true; };
			box.ApplyTemplate ();
			Enqueue (() => {
				box.IsDropDownOpen = true;
			});
			Enqueue (() => {
				try {
					Assert.IsFalse (opened, "#1");
					Assert.IsTrue (box.TemplatePopup.IsOpen, "#2");
				} finally {
					box.IsDropDownOpen = false;
				}
			});
			EnqueueTestComplete ();
		}

		[TestMethod]
		public void OwnContainerTest ()
		{
			FakeComboBox box = new FakeComboBox ();
			Assert.IsFalse (box.IsItemItsOwnContainerOverride_ (null), "#1");
			Assert.IsFalse (box.IsItemItsOwnContainerOverride_ (new object ()), "#2");
			Assert.IsFalse (box.IsItemItsOwnContainerOverride_ (new ListBoxItem ()), "#3");
			Assert.IsTrue (box.IsItemItsOwnContainerOverride_ (new ComboBoxItem ()), "#4");
			Assert.IsFalse (box.IsItemItsOwnContainerOverride_ (new Rectangle ()), "#5");
		}
		
		[TestMethod]
		public void PrepareContainerForItemOverrideTest ()
		{
			FakeComboBox box = new FakeComboBox ();
			Assert.Throws <NullReferenceException> (() => box.PrepareContainerForItemOverride_ (null, null));
		}

		[TestMethod]
		public void PrepareContainerForItemOverrideTest2 ()
		{
			FakeComboBox box = new FakeComboBox ();
			Assert.Throws<InvalidCastException> (() => box.PrepareContainerForItemOverride_ (new Rectangle (), null));
		}

		[TestMethod]
		[MoonlightBug]
		public void PrepareContainerForItemOverrideTest3 ()
		{
			FakeComboBox box = new FakeComboBox ();
			ComboBoxItem item = new ComboBoxItem ();
			Assert.IsNull (item.Style, "#1");
			Assert.IsNull (item.Content, "#2");
			Assert.IsNull (item.ContentTemplate, "#3");
			box.PrepareContainerForItemOverride_ (item, null);
			Assert.IsNull (item.Style, "#4");
			Assert.IsNull (item.Content, "#5");
			Assert.IsNotNull (item.ContentTemplate, "#6");
		}

		[TestMethod]
		[MoonlightBug]
		public void PrepareContainerForItemOverrideTest4 ()
		{
			FakeComboBox box = new FakeComboBox { ItemContainerStyle = new Style (typeof (ListBoxItem)) };
			box.ItemContainerStyle.Setters.Add (new Setter { Property = Canvas.LeftProperty, Value = 10.5 });
			ComboBoxItem item = new ComboBoxItem ();
			Assert.IsNull (item.Style, "#1");
			Assert.IsNull (item.Content, "#2");
			Assert.IsNull (item.ContentTemplate, "#3");

			box.PrepareContainerForItemOverride_ (item, null);

			Assert.AreSame (box.ItemContainerStyle, item.Style, "#4");
			Assert.IsNull (item.Content, "#5");
			Assert.IsNotNull (item.ContentTemplate, "#6");
		}

		[TestMethod]
		public void PrepareContainerForItemOverrideTest5 ()
		{
			FakeComboBox box = new FakeComboBox ();
			ComboBoxItem item = new ComboBoxItem ();
			box.PrepareContainerForItemOverride_ (item, item);
			Assert.IsNull (item.Content);
		}

		[TestMethod]
		public void PrepareContainerForItemOverrideTest6 ()
		{
			Rectangle rect = new Rectangle ();
			FakeComboBox box = new FakeComboBox ();
			ComboBoxItem item = new ComboBoxItem ();
			Assert.IsNull (item.Content, "#1");
			box.PrepareContainerForItemOverride_ (item, rect);
			Assert.AreSame (item.Content, rect, "#2");
		}

		[TestMethod]
		public void PrepareContainerForItemOverrideTest7 ()
		{
			Rectangle rect = new Rectangle ();
			FakeComboBox box = new FakeComboBox ();
			box.Items.Add (rect);
			ComboBoxItem item = new ComboBoxItem ();
			Assert.Throws<InvalidOperationException> (() => box.PrepareContainerForItemOverride_ (item, rect), "#2");
		}
		
		[TestMethod]
		public void RemoveTest ()
		{
			object orig = new object ();
			FakeComboBox c = new FakeComboBox ();
			c.Items.Add (orig);
			c.Items.Add (new object ());
			c.SelectedIndex = 0;
			c.methods.Clear ();

			c.Items.RemoveAt (0);

			// WTF? Why is there a remove, then add, then replace? Surely this is just a replace...
			Assert.AreEqual (1, c.methods.Count, "#1"); // Fails in Silverlight 3
			Assert.AreEqual ("OnItemsChanged", c.methods [0].MethodName, "#2");
			Assert.AreEqual (NotifyCollectionChangedAction.Remove, ((NotifyCollectionChangedEventArgs) c.methods [0].MethodParams [0]).Action, "#3");

			// '1' was never a valid index. How the **** is this happening?
			Assert.AreEqual (0, c.SelectedIndex, "#8");
			Assert.AreEqual (orig, c.SelectedItem, "#9");

			c.Items.RemoveAt (0);
			Assert.AreEqual (0, c.SelectedIndex, "#10");
			Assert.AreEqual (orig, c.SelectedItem, "#11");
		}

		[TestMethod]
		public void RemoveTest2 ()
		{
			object orig = new object ();
			FakeComboBox c = new FakeComboBox ();
			c.Items.Add (orig);
			c.Items.Add (new object ());
			c.Items.Add (new object ());
			c.SelectedIndex = 0;
			c.methods.Clear ();

			c.Items.RemoveAt (0);

			// WTF? Why is there a remove, then add, then replace? Surely this is just a replace...
			Assert.AreEqual (1, c.methods.Count, "#1"); // Fails in Silverlight 3
			Assert.AreEqual ("OnItemsChanged", c.methods [0].MethodName, "#2");
			Assert.AreEqual (NotifyCollectionChangedAction.Remove, ((NotifyCollectionChangedEventArgs) c.methods [0].MethodParams [0]).Action, "#3");

			// '1' was never a valid index. How the **** is this happening?
			Assert.AreEqual (0, c.SelectedIndex, "#8");
			Assert.AreEqual (orig, c.SelectedItem, "#9");
		}
		
		[TestMethod]
		public void RemoveTest3 ()
		{
			Rectangle orig = new Rectangle ();
			FakeComboBox c = new FakeComboBox ();
			c.Items.Add (orig);
			c.SelectedIndex = 0;

			c.Items.RemoveAt (0);
			Assert.AreEqual (0, c.SelectedIndex, "#10"); // Fails in Silverlight 3
			Assert.AreEqual (orig, c.SelectedItem, "#11");

			Assert.IsNull (orig.Parent);
			Assert.IsNull (VisualTreeHelper.GetParent (orig));
		}

		[TestMethod]
		public void ReplaceTest ()
		{
			object orig = new object ();
			FakeComboBox c = new FakeComboBox ();
			c.Items.Add (orig);
			c.SelectedIndex = 0;
			c.methods.Clear ();

			c.Items[0] = new object ();

			// WTF? Why is there a remove, then add, then replace? Surely this is just a replace...
			Assert.AreEqual (3, c.methods.Count, "#1"); // Fails in Silverlight 3
			Assert.AreEqual ("OnItemsChanged", c.methods [0].MethodName, "#2");
			Assert.AreEqual (NotifyCollectionChangedAction.Remove, ((NotifyCollectionChangedEventArgs) c.methods [0].MethodParams[0]).Action, "#3");

			Assert.AreEqual ("OnItemsChanged", c.methods [1].MethodName, "#4");
			Assert.AreEqual (NotifyCollectionChangedAction.Add, ((NotifyCollectionChangedEventArgs) c.methods [1].MethodParams [0]).Action, "#5");

			Assert.AreEqual ("OnItemsChanged", c.methods [2].MethodName, "#6");
			Assert.AreEqual (NotifyCollectionChangedAction.Replace, ((NotifyCollectionChangedEventArgs) c.methods [2].MethodParams [0]).Action, "#7");

			// '1' was never a valid index. How the **** is this happening?
			Assert.AreEqual (1, c.SelectedIndex, "#8");
			Assert.AreEqual (orig, c.SelectedItem, "#9");
		}
		
		[TestMethod]
		public void SelectedItemTest ()
		{
			FakeComboBox box = new FakeComboBox ();
			Assert.AreEqual (-1, box.SelectedIndex, "#1");
			Assert.AreEqual (null, box.SelectedItem, "#2");

			box.SelectedItem = new object ();
			Assert.AreEqual (-1, box.SelectedIndex, "#3");
			Assert.AreEqual (null, box.SelectedItem, "#4");

			object a = new object ();
			object b = new object ();
			object c = new object ();
			box.Items.Add (a);
			box.Items.Add (b);
			box.Items.Add (c);
			box.SelectedItem = new object ();
			Assert.AreEqual (-1, box.SelectedIndex, "#5");
			Assert.AreEqual (null, box.SelectedItem, "#6");

			box.SelectedItem = a;
			Assert.AreEqual (0, box.SelectedIndex, "#7");
			Assert.AreEqual (a, box.SelectedItem, "#8");

			box.SelectedIndex = -1;
			Assert.AreEqual (-1, box.SelectedIndex, "#9");
			Assert.AreEqual (null, box.SelectedItem, "#10");

			box.SelectedItem = a;
			box.SelectedItem = b;
			Assert.AreEqual (1, box.SelectedIndex, "#11");
			Assert.AreEqual (b, box.SelectedItem, "#12");
			Assert.AreEqual (7, box.methods.Count, "#13");
			int i = 0;
			Assert.AreEqual ("OnItemsChanged", box.methods [i++].MethodName, "#14." + i);
			Assert.AreEqual ("OnItemsChanged", box.methods [i++].MethodName, "#14." + i);
			Assert.AreEqual ("OnItemsChanged", box.methods [i++].MethodName, "#14." + i);
			Assert.AreEqual ("SelectionChangedEvent", box.methods [i++].MethodName, "#14." + i);
			Assert.AreEqual ("SelectionChangedEvent", box.methods [i++].MethodName, "#14." + i);
			Assert.AreEqual ("SelectionChangedEvent", box.methods [i++].MethodName, "#14." + i);
			Assert.AreEqual ("SelectionChangedEvent", box.methods [i++].MethodName, "#14." + i);
		}
		
		[TestMethod]
		public void SelectedItemTest2 ()
		{
			FakeComboBox box = new FakeComboBox ();
			object o = new object ();
			box.Items.Add (o);
			box.SelectedItem = o;
			box.methods.Clear ();
			box.SelectedItem = null;
			Assert.AreEqual (1, box.methods.Count, "#1");
			Assert.AreEqual ("SelectionChangedEvent", box.methods [0].MethodName, "#2");
			Assert.AreEqual (null, box.SelectedItem, "#3");
			Assert.AreEqual (-1, box.SelectedIndex, "#4");
		}
		
		[TestMethod]
		public void SelectedItemTest3 ()
		{
			FakeComboBox box = new FakeComboBox ();
			object o = new object ();
			box.Items.Add (o);
			box.SelectedItem = o;
			box.methods.Clear ();
			box.SelectedIndex = -1;
			Assert.AreEqual (1, box.methods.Count, "#1");
			Assert.AreEqual ("SelectionChangedEvent", box.methods [0].MethodName, "#2");
			Assert.AreEqual (null, box.SelectedItem, "#3");
			Assert.AreEqual (-1, box.SelectedIndex, "#4");
			SelectionChangedEventArgs e = (SelectionChangedEventArgs)box.methods[0].ReturnValue;
			Assert.AreEqual (o, e.RemovedItems [0], "#5");
			Assert.AreEqual (0, e.AddedItems.Count, "#6");
		}
		
        [TestMethod]
        public void SelectedItemTest4 ()
        {
            bool changed = false;
            ComboBox box = new ComboBox();
            box.SelectionChanged += delegate { changed = true; };
            box.Items.Add(new object());
            Assert.IsFalse(changed, "#1");
            box.SelectedItem = box.Items[0];
            Assert.IsTrue(changed, "#2");
        }

		[TestMethod]
		[Asynchronous]
		public void SelectThenClear ()
		{
			FakeComboBox box = new FakeComboBox ();
			CreateAsyncTest (box,
				() => box.ApplyTemplate (),
				() => {
					box.Items.Add (new object ());
					box.Items.Add (new object ());
					box.Items.Add (new object ());

				},
				() => { box.IsDropDownOpen = true; },
				() => { box.SelectedItem = box.Items [0]; },
				() => {
					box.methods.Clear ();
					box.Items.Clear ();
					Assert.AreEqual ("ClearContainerForItemOverride", box.methods [0].MethodName, "#1");
					Assert.AreEqual ("ClearContainerForItemOverride", box.methods [1].MethodName, "#2");
					Assert.AreEqual ("ClearContainerForItemOverride", box.methods [2].MethodName, "#3");
					Assert.AreEqual ("SelectionChangedEvent", box.methods [3].MethodName, "#4"); // Fails in Silverlight 3
					Assert.AreEqual ("OnItemsChanged", box.methods [4].MethodName, "#5");
					Assert.IsNull (box.SelectedItem, "#1");

				}
			);
		}

		[TestMethod]
		[Asynchronous]
		public void SelectThenClear2 ()
		{
			FakeComboBox box = new FakeComboBox ();
			CreateAsyncTest (box,
				() => box.ApplyTemplate (),
				() => {
					box.Items.Add (new object ());
					box.Items.Add (new object ());
					box.Items.Add (new object ());
				},
				// Check to see if the items are cleared in the reverse order they were added in
				() => { box.IsDropDownOpen = true; },
				() => { box.SelectedItem = box.Items.Last (); },
				() => {
					box.methods.Clear ();
					box.Items.Clear ();
					Assert.AreEqual ("ClearContainerForItemOverride", box.methods [0].MethodName, "#1");
					Assert.AreEqual ("SelectionChangedEvent", box.methods [1].MethodName, "#4"); // Fails in Silverlight 3
					Assert.AreEqual ("ClearContainerForItemOverride", box.methods [2].MethodName, "#2");
					Assert.AreEqual ("ClearContainerForItemOverride", box.methods [3].MethodName, "#3");

				}
			);
		}
		
		[TestMethod]
		[MoonlightBug ("This hits a corner case involving the default style not being applied by moonlight but it is on silverlight")]
		public void TemplateClosesDropdown ()
		{
			ComboBox box = new ComboBox ();
			box.IsDropDownOpen = true;
			Assert.IsTrue (box.ApplyTemplate (), "#1");
			Assert.IsFalse (box.IsDropDownOpen, "#2");
		}
	}
}
