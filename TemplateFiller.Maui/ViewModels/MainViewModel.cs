using System.Collections.ObjectModel;
using System.Windows.Input;
using TemplateFiller.Core;
using TemplateFiller.Core.Models;
using TemplateFiller.Maui.Services;

namespace TemplateFiller.Maui.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		private IFileProcessor _fileProcessor;
		private IEmailSender _emailSender;
		private string _fullPath;

		private string _header;
		public string Header
		{
			get { return _header; }
			set { SetValue(ref _header, value); }
		}

		private bool _isLoaded;
		public bool IsLoaded
		{
			get { return _isLoaded; }
			set { SetValue(ref _isLoaded, value); }
		}

		private ObservableCollection<DataField> _dataFields;
		public ObservableCollection<DataField> DataFields
		{
			get { return _dataFields; }
			set { SetValue(ref _dataFields, value); }
		}

		public ICommand OpenFileCommand { get; private set; }
		public ICommand SaveFileCommand { get; private set; }
		public ICommand SendEmailCommand { get; private set; }

		public MainViewModel(IFileProcessor fileProcessor, IEmailSender emailSender)
		{
			Header = "Open file to start editing fields...";
			IsLoaded = false;
			DataFields = new ObservableCollection<DataField>();
			_fileProcessor = fileProcessor;
			_emailSender = emailSender;

			OpenFileCommand = new Command(ExecuteOpenFileCommand);
			SaveFileCommand = new Command(ExecuteSaveFileCommand);
			SendEmailCommand = new Command(ExecuteSendEmailCommand);
		}

		private async void ExecuteOpenFileCommand()
		{
			try
			{
				PickOptions options = new()
				{
					FileTypes = new FilePickerFileType(
						new Dictionary<DevicePlatform, IEnumerable<string>>
						{
							{DevicePlatform.WinUI, new[] { ".docx" } }
						})
				};
				var result = await FilePicker.Default.PickAsync(options);
				if (result != null)
				{
					if (result.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
					{
						_fileProcessor.LoadFile(result.FullPath);
						_fullPath = result.FullPath;
						Header = result.FileName;
						IsLoaded = true;
						DataFields = new ObservableCollection<DataField>(_fileProcessor.GetDataFields());
					}
					else
					{
						throw new Exception("Open .docx file.");
					}
				}
			}
			catch (Exception e)
			{
				await Application.Current.MainPage.DisplayAlert("Alert", e.Message, "OK");
			}
		}

		private async void ExecuteSaveFileCommand()
		{
			var docName = Header.Insert(Header.Length - 5,
				$" {DateTimeOffset.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss")}");
			var fileName = Path.Combine(Path.GetDirectoryName(_fullPath), docName);
			if (File.Exists(fileName))
			{
				await Application.Current.MainPage.DisplayAlert(
					"Alert",
					"File already exists.",
					"OK");
			}
			else
			{
				_fileProcessor.Save(DataFields.ToList(), fileName);
				await Application.Current.MainPage.DisplayAlert(
					"File saved",
					$"File name: \"{docName}\"",
					"OK");
			}
		}

		private async void ExecuteSendEmailCommand()
		{
			string fileName = Path.Combine(FileSystem.Current.AppDataDirectory, Header);
			try
			{
				_fileProcessor.Save(DataFields.ToList(), fileName);

				await _emailSender.SendEmailAsync(fileName);
			}
			catch (Exception e)
			{
				await Application.Current.MainPage.DisplayAlert(
					"Alert",
					e.Message,
					"OK");
			}
			finally
			{
				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}
			}
		}
	}
}
