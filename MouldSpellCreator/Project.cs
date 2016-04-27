using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace MouldSpellCreator
{
	public class Project
	{
		public string Name { get; set; }

		public string ProjectFolder { get; set; }

		public string Filename { get; set; }

		public ObservableCollection<Spell> Spells { get; set; }

		public string GetBytecodeFilename => ProjectFolder + @"\src\Spells\Bytecode.h";

		public Project(string filename)
		{
			var info = new FileInfo(filename);
			ProjectFolder = info.DirectoryName;
			Filename = filename;

			Instruction.ParseInstructions(GetBytecodeFilename);

			Spells = new ObservableCollection<Spell>();

			var doc = new XmlDocument();
			doc.Load(filename);
			XmlNode root = doc.DocumentElement;
			if (root?.Attributes != null) Name = root.Attributes["Name"].Value;


			if (root == null) return;

			foreach (XmlNode childNode in root.ChildNodes)
			{
				switch(childNode.Name)
				{
					case "Spells":
						foreach (XmlNode node in childNode.ChildNodes)
						{
							if (node.Attributes != null)
							{
								AddSpell(ProjectFolder + node.Attributes["Src"].Value, node.Attributes["Name"].Value);
							}
						}

						break;
				}
			}
		}

		public void AddSpell(string filename, string name)
		{
			string spellAltFile = ProjectFolder + "\\Debug" + filename.Remove(0, ProjectFolder.Length);

			Spells.Add(new Spell(filename, name, spellAltFile));
		}

		public void Save()
		{
			var doc = new XmlDocument();
			var root = doc.CreateNode(XmlNodeType.Element, "Project", "");
			var nameAttr = doc.CreateAttribute("Name");
			nameAttr.Value = Name;
			root.Attributes.Append(nameAttr);

			var spellsNode = doc.CreateNode(XmlNodeType.Element, "Spells", "");

			foreach (var spell in Spells)
			{
				var spellNode = doc.CreateNode(XmlNodeType.Element, "Spell", "");
				var spellName = doc.CreateAttribute("Name");
				spellName.Value = spell.FriendlyName;
				spellNode.Attributes.Append(spellName);

				var spellSrc = doc.CreateAttribute("Src");
				spellSrc.Value = spell.Filename.Remove(0, ProjectFolder.Length);
				spellNode.Attributes.Append(spellSrc);

				spellsNode.AppendChild(spellNode);

				spell.Save();
			}

			root.AppendChild(spellsNode);

			doc.AppendChild(root);
			doc.Save(Filename);
		}
	}

	public class Spell
	{
		public string Filename { get; set; }
		public string AltFilename { get; set; }
		public string FriendlyName { get; set; }
		public ObservableCollection<Instruction> Instructions { get; set; }

		public Spell(string filename, string friendlyName, string altFilename)
		{
			Filename = filename;
			FriendlyName = friendlyName;
			AltFilename = altFilename;
			Instructions = LoadInstructions();
		}

		public void Save()
		{
			using(StreamWriter writer = new StreamWriter(Filename, false))
			{
				WriteToStream(writer);
			}

			using(StreamWriter writer = new StreamWriter(AltFilename, false))
			{
				WriteToStream(writer);
			}
		}

		private void WriteToStream(StreamWriter stream)
		{
			for(int i = 0; i < Instructions.Count; i++)
			{
				if(i < Instructions.Count - 1)
					stream.Write(Instructions[i].Code + " ");
				else
					stream.Write(Instructions[i].Code);
			}

			stream.WriteLine();
		}

		private ObservableCollection<Instruction> LoadInstructions()
		{
			var instructions = new ObservableCollection<Instruction>();

			using (var reader = new StreamReader(Filename))
			{
				var readLine = reader.ReadLine();

				if (readLine == null) return instructions; // This should not happen.

				var instrStrings = readLine.Split();

				for(int i = 0; i < instrStrings.Length; i++)
				{
					if (!string.IsNullOrEmpty(instrStrings[i]))
					{
						var instr = Instruction.GetInstruction(int.Parse(instrStrings[i]));

						instructions.Add(instr);

						if (instr.FriendlyName == "Literal")
						{
							instructions.Add(Instruction.GetLiteral(int.Parse(instrStrings[++i])));
						}
					}
				}
			}

			return instructions;
		}
	}

	public class Instruction
	{
		private static List<Instruction> _instructions;

		public string FriendlyName { get; private set; }
		public int Code { get; private set; }
		public bool IsLiteral { get; private set; }
		public string Description { get; private set; }
		public int Inputs { get; private set; }
		public int Outputs { get; private set; }

		public Instruction()
		{
			IsLiteral = false;
		}

		public static void ParseInstructions(string filename)
		{
			_instructions = new List<Instruction>();

			using (StreamReader reader = new StreamReader(filename))
			{
				string line;
				bool parsing = false;

				while ((line = reader.ReadLine()) != null)
				{
					line = Regex.Replace(line, @"\s+", "");

					if (!parsing)
					{
						if (line == "enumByteCode{")
						{
							parsing = true;
						}
					}
					else
					{
						if (line == "I_END")
						{
							break;
						}
						else
						{
							var instr = new Instruction();
							var splitted = line.Split('=');
							instr.FriendlyName = ConvertToFriendlyName(splitted[0]);
							instr.Code = int.Parse(splitted[1].Split(',')[0]);

							line = reader.ReadLine();
							line = Regex.Replace(line, @"\t", "");
							instr.Description = line.Remove(0,2);//Descr

							line = reader.ReadLine();
							line = Regex.Replace(line, @"\s+", "");
							instr.Inputs = int.Parse(line.Remove(0, 2));

							line = reader.ReadLine();
							line = Regex.Replace(line, @"\s+", "");
							instr.Outputs = int.Parse(line.Remove(0, 2));

							_instructions.Add(instr);
						}
					}
				}
			}
		}

		public static string ConvertToFriendlyName(string name)
		{
			StringBuilder friendlyName = new StringBuilder();
			string[] splitted = name.Split('_');

			for (int i = 1; i < splitted.Length; i++)
			{
				splitted[i] = splitted[i].ToLower();
				friendlyName.Append(splitted[i]);
				friendlyName.Append(' ');
			}

			friendlyName.Remove(friendlyName.Length - 1, 1);


			return ToTitleCase(friendlyName.ToString());
		}

		public static string ToTitleCase(string str)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
		}

		public static Instruction GetInstruction(int code)
		{
			//Return the correct instruction or just the literal if no instruction is found
			return _instructions.FirstOrDefault(instr => instr.Code == code) ?? new Instruction {Code = code};
		}

		public static Instruction GetLiteral(int literal)
		{
			Instruction instr = new Instruction();
			instr.Code = literal;
			instr.FriendlyName = literal.ToString();
			instr.IsLiteral = true;
			return instr;
		}

		public static ObservableCollection<Instruction> GetAllInstructions()
		{
			return new ObservableCollection<Instruction>(_instructions);
		}
	}
}
