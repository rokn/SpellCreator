using System.ComponentModel;
using System.Windows;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using MouldSpellCreator.Annotations;

namespace MouldSpellCreator
{
	/// <summary>
	/// Interaction logic for CreateNewSpellDialog.xaml
	/// </summary>
	public partial class CreateNewSpellDialog : Window, INotifyPropertyChanged
	{
		private string _filePath;
		private string _spellName;

		public string FilePath
		{
			get { return _filePath; }
			set
			{
				_filePath = value; 
				OnPropertyChanged();
			}
		}

		public string SpellName
		{
			get { return _spellName; }
			set
			{
				_spellName = value;
				OnPropertyChanged();
			}
		}

		public bool Success { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public CreateNewSpellDialog()
		{
			InitializeComponent();

			FilePath = "";
			SpellName = "";

			DataContext = this;
			Success = false;
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void BrowseClick(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog()
			{
				AddExtension = true,
				CheckFileExists = true,
				Filter = "Spell files (*.spell)|*.spell",
				CreatePrompt = true,
			};

			dialog.ShowDialog();

			if (File.Exists(dialog.FileName))
			{
				FilePath = dialog.FileName;
			}
		}


		private void CancelClick(object sender, RoutedEventArgs e)
		{
			this.Close();
			Success = false;
		}

		private void CreateClick(object sender, RoutedEventArgs e)
		{
			if (File.Exists(FilePath) && !string.IsNullOrEmpty(SpellName))
			{
				Success = true;
				NotificationWindow notify = new NotificationWindow("New spell created", "Successfully created a new spell named: " + SpellName);
				notify.Show();
				this.Close();
			}
		}
	}
}
