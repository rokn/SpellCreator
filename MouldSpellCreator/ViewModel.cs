using System;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace MouldSpellCreator
{
	public class ViewModel : INotifyPropertyChanged //  az sym toni bonboni \;d ;d ;d d; ;d
	{
		private Spell _selectedSpell; 
		private Project _openedProject;
		private Instruction _selectedInstruction;

		public ObservableCollection<Instruction> AllInstructions => Instruction.GetAllInstructions(); 

		public event PropertyChangedEventHandler PropertyChanged; 

		public Spell SelectedSpell 
		{
			
			get
			{
				return _selectedSpell; 
			}

			set
			{
				_selectedSpell = value; 
				OnPropertyChanged(); 
			}
		}

		public Instruction SelectedInstruction
		{
			get { return _selectedInstruction; }
			set
			{
				_selectedInstruction = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<Spell> Spells
		{
			get { return OpenedProject?.Spells; }

			set
			{
				if (OpenedProject == null) return;

				OpenedProject.Spells = value; 
				OnPropertyChanged(); 
			}
		}
		
		public Project OpenedProject
		{
			get { return _openedProject; }
			private set
			{
				_openedProject = value;
				OnPropertyChanged();
			}
		}

		public int StackSize
		{
			get
			{
				int stackSize = 0;

				foreach (var instruction in SelectedSpell.Instructions)
				{
					stackSize -= instruction.Inputs;
					stackSize += instruction.Outputs;
				}

				return stackSize;
			}
		}
		
		public void OpenProject()
		{
			
			var filename = Properties.Settings.Default.LastUsedFile;

			if (!File.Exists(filename))
			{
				OpenFileDialog dialog = new OpenFileDialog
				{
					CheckFileExists = true,
					Filter = "Spell creator files (*.spellproj)|*.spellproj"
				};

				dialog.ShowDialog();
				
				filename = dialog.FileName;

				if (!File.Exists(filename))
				{
					MessageBox.Show("Please select a valid file");
					Environment.Exit(1);
				}

				Properties.Settings.Default.LastUsedFile = filename;
				Properties.Settings.Default.Save();
			}
			
			OpenedProject = new Project(filename);

			SaveProject();

			if(OpenedProject?.Spells.Count > 0)
				SelectedSpell = OpenedProject.Spells[0];
		}

		public void AddInstruction(Instruction instruction)
		{
			SelectedSpell.Instructions.Add(instruction);
			OnPropertyChanged("StackSize");
		}
		
		public void RemoveInstruction(int index)
		{
			SelectedSpell.Instructions.RemoveAt(index);
			OnPropertyChanged("StackSize");
		}

		public void SaveProject()
		{
			_openedProject.Save();
		}
		
		public void CreateNewSpell()
		{
			var dialog = new CreateNewSpellDialog();
			dialog.ShowDialog();
			if (dialog.Success)
			{
				OpenedProject.AddSpell(dialog.FilePath, dialog.SpellName);
				SaveProject();
			}
		}

		public int MoveInstruction(bool up, int index)
		{
			int newIndex = index;

			if (up)
			{
				if (index == 0) return newIndex;
				_selectedSpell.Instructions.Swap(index, index - 1);
				newIndex--;
			}
			else
			{
				if(index >= _selectedSpell.Instructions.Count - 1) return newIndex;

				_selectedSpell.Instructions.Swap(index, index + 1);
				newIndex++;
			}

			return newIndex;
		}

		public void DeleteSpell()
		{
			var result = MessageBox.Show("Are you sure you want to delete \""+ SelectedSpell.FriendlyName +"\"?", "Warning!", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes)
			{
				var deleteFileResult = MessageBox.Show("Do you want to delete the file on the system?", "Warning!", MessageBoxButton.YesNo);

				if (deleteFileResult == MessageBoxResult.Yes)
				{
					File.Delete(SelectedSpell.Filename);
					File.Delete(SelectedSpell.AltFilename);
				}

				OpenedProject.Spells.Remove(SelectedSpell);

				if (OpenedProject.Spells.Count > 0)
				{
					SelectedSpell = OpenedProject.Spells[0];
				}

				SaveProject();
			}
		}
 	
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
