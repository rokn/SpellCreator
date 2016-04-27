using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MouldSpellCreator
{
//	public class InstructionListBox<T> : DragAndDropListBox<T>
//		where T : class
//	{
//		protected override void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
//		{
//			var point = e.GetPosition(null);
//			var diff = _dragStartPoint - point;
//			if(e.LeftButton == MouseButtonState.Pressed &&
//				(Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
//				 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
//			{
//				var lb = sender as ListBox;
//				var lbi = FindVisualParent<ListBoxItem>(((DependencyObject)e.OriginalSource));
//				if(lbi != null)
//				{
//					DragDrop.DoDragDrop(lbi, lbi.DataContext, DragDropEffects.Copy);
//				}
//			}
//		}
//	}
//
//	public class DragAndDropListBox<T> : ListBox
//		where T : class
//	{
//		protected Point _dragStartPoint;
//		protected bool _dragging;
//
//		public DragAndDropListBox()
//		{
//			_dragging = true;
//			
//			PreviewMouseMove += ListBox_PreviewMouseMove;
//
//			var style = new Style(typeof (ListBoxItem));
//
//			style.Setters.Add(new Setter(AllowDropProperty, true));
//
//			style.Setters.Add(
//				new EventSetter(
//					PreviewMouseLeftButtonDownEvent,
//					new MouseButtonEventHandler(ListBoxItem_PreviewMouseLeftButtonDown)));
//
//			style.Setters.Add(
//				new EventSetter(
//					DropEvent,
//					new DragEventHandler(ListBoxItem_Drop)));
//
//			ItemContainerStyle = style;
//		}
//
//		protected P FindVisualParent<P>(DependencyObject child)
//			where P : DependencyObject
//		{
//			var parentObject = VisualTreeHelper.GetParent(child);
//			if (parentObject == null)
//				return null;
//
//			var parent = parentObject as P;
//			if (parent != null)
//				return parent;
//
//			return FindVisualParent<P>(parentObject);
//		}
//
//		protected virtual void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
//		{
//			var point = e.GetPosition(null);
//			var diff = _dragStartPoint - point;
//			if (e.LeftButton == MouseButtonState.Pressed &&
//			    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
//			     Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
//			{
//				var lb = sender as ListBox;
//				var lbi = FindVisualParent<ListBoxItem>(((DependencyObject) e.OriginalSource));
//				if (lbi != null)
//				{
//					_dragging = true;
//					DragDrop.DoDragDrop(lbi, lbi.DataContext, DragDropEffects.Move);
//				}
//			}
//		}
//
//		private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
//		{
//			_dragStartPoint = e.GetPosition(null);
//		}
//
//		private void ListBoxItem_Drop(object sender, DragEventArgs e)
//		{
//			var source = e.Data.GetData(typeof(T)) as T;
//			var target = ((ListBoxItem)(sender)).DataContext as T;
//
//			if (source == null || target == null) return;
//
//			var targetIndex = Items.IndexOf(target);
//
//			if (_dragging)
//			{
//				_dragging = false;
//				var sourceIndex = Items.IndexOf(source);
//				Move(source, sourceIndex, targetIndex);
//			}
//			else
//			{
//				var items = DataContext as IList<T>;
//				items?.Insert(targetIndex, source);
//			}
//		}
//
//		private void Move(T source, int sourceIndex, int targetIndex)
//		{
//			if (sourceIndex < targetIndex)
//			{
//				var items = DataContext as IList<T>;
//				if (items != null)
//				{
//					items.Insert(targetIndex + 1, source);
//					items.RemoveAt(sourceIndex);
//				}
//			}
//			else
//			{
//				var items = DataContext as IList<T>;
//				if (items != null)
//				{
//					var removeIndex = sourceIndex + 1;
//					if (items.Count + 1 > removeIndex)
//					{
//						items.Insert(targetIndex, source);
//						items.RemoveAt(removeIndex);
//					}
//				}
//			}
//		}
//	}

	

//	public class CurrentInstructionsListBox : DragAndDropListBox<Instruction>
//	{
//	}
//
//	public class AllInstructionsListBox : InstructionListBox<Instruction>
//	{
//	}

	public partial class MainWindow
	{
		private readonly ViewModel _viewModel;

		public MainWindow()
		{
			InitializeComponent();

			if (Properties.Settings.Default.LastFullscreen)
			{
				WindowState = WindowState.Maximized;
			}

			SizeChanged += OnResize;
			LocationChanged += WindowLocationChanged;

			_viewModel = new ViewModel();

			_viewModel.OpenProject();

			DataContext = _viewModel;
		}

		private void WindowLocationChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.LastPosition = new Point((int)Application.Current.MainWindow.Left, (int)Application.Current.MainWindow.Top);
			Properties.Settings.Default.Save();
		}

		private void OnResize(object sender, SizeChangedEventArgs e)
		{
			Properties.Settings.Default.LastFullscreen = WindowState == WindowState.Maximized;
			Properties.Settings.Default.LastSize = e.NewSize;
			Properties.Settings.Default.Save();
		}

		private static void UpdateScrollBar(DependencyObject listBox)
		{
			if (listBox == null) return;

			var border = (Border)VisualTreeHelper.GetChild(listBox, 0);
			var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
			scrollViewer.ScrollToBottom();
		}

		private void AddFromInstructionList()
		{
			Instruction instr = InstructionsList.SelectedItem as Instruction;

			if (instr != null)
			{
				_viewModel.AddInstruction(instr);
				UpdateScrollBar(CurrInstructions);
			}
		}

		private void RemoveFromInstructions()
		{
			if (CurrInstructions.SelectedIndex >= 0)
			{
				var oldIndex = CurrInstructions.SelectedIndex;
				_viewModel.RemoveInstruction(CurrInstructions.SelectedIndex);
				CurrInstructions.SelectedIndex = Math.Max(oldIndex - 1, 0);
			}
		}

		private void InstructionListDoubleClick(object sender, MouseButtonEventArgs e)
		{
			AddFromInstructionList();
		}

		private void InstructionsList_OnKeyDown(object sender, KeyEventArgs e)
		{
			switch(e.Key)
			{
				case Key.Enter:
					AddFromInstructionList();
					break;
			}
		}

		private void CurrInstructions_OnKeyDown(object sender, KeyEventArgs e)
		{
			switch(e.Key)
			{
				case Key.Delete:
					RemoveFromInstructions();
					break;
			}
		}

		private void SaveProject(object sender, ExecutedRoutedEventArgs e)
		{
			_viewModel.SaveProject();
			NotificationWindow notify = new NotificationWindow("Save successfull", "");
			notify.Show();
			Focus();
		}

		private void CreateSpellClick(object sender, RoutedEventArgs e)
		{
			_viewModel.CreateNewSpell();
		}

		private void MoveInstructionClick(object sender, RoutedEventArgs e)
		{
			if (CurrInstructions.SelectedIndex >= 0)
			{
				int newIndex = _viewModel.MoveInstruction(((Button) sender).Equals(UpButton), CurrInstructions.SelectedIndex);
				CurrInstructions.SelectedIndex = newIndex;
				CurrInstructions.Focus();
			}
		}

		private void DeleteSpellClick(object sender, RoutedEventArgs e)
		{
			_viewModel.DeleteSpell();
		}
	}

	public static class Commands
	{
		public static readonly RoutedUICommand SaveCommand = new RoutedUICommand("Save command", "SaveComand", typeof(MainWindow));
	}
}