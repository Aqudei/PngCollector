using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using PngCollector.Models;

namespace PngCollector.ViewModels
{
    class ShellViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private string _workingDirectory;
        private int _imagePerFolder;
        private bool _isBusy;
        public BindableCollection<Folder> Folders { get; set; } = new BindableCollection<Folder>();
        public BindableCollection<PicFile> PicFiles { get; set; } = new BindableCollection<PicFile>();
        public Folder SelectedFolder { get; set; }

        public int ImagePerFolder
        {
            get => _imagePerFolder;
            set => Set(ref _imagePerFolder, value);
        }

        public string WorkingDirectory
        {
            get => _workingDirectory;
            set => Set(ref _workingDirectory, value);
        }

        public void RemoveFolder()
        {
            if (SelectedFolder == null)
                return;

            using (var db = new Database())
            {
                var toDelete = db.Folders.FirstOrDefault(folder => folder.Path == SelectedFolder.Path);
                if (toDelete == null)
                    return;
                db.Folders.Remove(toDelete);
                db.SaveChanges();
                Folders.Remove(SelectedFolder);
            }
        }

        private string GenerateFolderNameBaseOnDateTime()
        {
            return DateTime.Now.ToString().Replace("/", "_")
                .Replace(":", "_")
                .Replace(" ", "_");
        }

        public async void Collect()
        {
            IsBusy = true;

            await Task.Run(() =>
            {
                var counter = 0;
                var currentFolderName = Path.Combine(WorkingDirectory, GenerateFolderNameBaseOnDateTime());
                using (var db = new Database())
                {
                    foreach (var folder in Folders)
                    {
                        var files = Directory.GetFiles(folder.Path, "*.png", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            if (counter == ImagePerFolder)
                            {
                                currentFolderName = Path.Combine(WorkingDirectory, GenerateFolderNameBaseOnDateTime());
                                counter = 0;
                            }

                            Directory.CreateDirectory(currentFolderName);
                            var destinationFileName = Path.Combine(currentFolderName, Guid.NewGuid() + "." + Path.GetFileName(file));
                            File.Copy(file, destinationFileName);

                            db.PicFiles.Add(new PicFile
                            {
                                CurrentLocation = destinationFileName,
                                OriginalLocation = file
                            });
                            db.SaveChanges();
                            counter++;
                        }
                    }
                }
            });
            IsBusy = false;
            Process.Start("explorer", WorkingDirectory);
        }

        public bool CanCollect => !IsBusy;
        public bool CanDistribute => !IsBusy;

        public void AddFolder()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false
            };
            var result = dialog.ShowDialog();
            if (result != CommonFileDialogResult.Ok)
                return;

            if (string.IsNullOrWhiteSpace(dialog.FileName))
                return;

            using (var db = new Database())
            {
                try
                {
                    var folder = db.Folders.Add(new Folder
                    {
                        Path = dialog.FileName
                    });

                    db.SaveChanges();
                    Folders.Add(folder);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }

        protected override void OnDeactivate(bool close)
        {
            Properties.Settings.Default.ImagePerFolder = ImagePerFolder;
            Properties.Settings.Default.WorkingDirectory = WorkingDirectory;
            Properties.Settings.Default.Save();
        }

        public void SelectWorkingDirectory()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false
            };
            var result = dialog.ShowDialog();
            if (result != CommonFileDialogResult.Ok)
                return;

            WorkingDirectory = dialog.FileName;
            Properties.Settings.Default.WorkingDirectory = dialog.FileName;
            Properties.Settings.Default.Save();
        }

        public async void Distribute()
        {
            IsBusy = true;
            await Task.Run(() =>
            {
                var pngs = Directory.GetFiles(WorkingDirectory, "*.png", SearchOption.AllDirectories);
                using (var db = new Database())
                {
                    foreach (var png in pngs)
                    {
                        var picFile = db.PicFiles.FirstOrDefault(file => file.CurrentLocation == png);
                        if (picFile != null)
                        {
                            File.Copy(png, picFile.OriginalLocation);
                        }
                    }
                }

                var directories = Directory.GetDirectories(WorkingDirectory);
                foreach (var directory in directories)
                {
                    Directory.Delete(directory, true);
                }
            });
            IsBusy = false;
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                Set(ref _isBusy, value);
                NotifyOfPropertyChange(nameof(CanCollect));
                NotifyOfPropertyChange(nameof(CanDistribute));
            }
        }

        public ShellViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
            WorkingDirectory = Properties.Settings.Default.WorkingDirectory;
            ImagePerFolder = Properties.Settings.Default.ImagePerFolder;

            IsBusy = true;
            Task.Run(() =>
            {
                using (var db = new Database())
                {
                    foreach (var folder in db.Folders)
                    {
                        Execute.OnUIThread(() => Folders.Add(folder));
                    }

                    foreach (var picFile in db.PicFiles)
                    {
                        Execute.OnUIThread(() => PicFiles.Add(picFile));
                    }
                }

                IsBusy = false;
            });
        }
    }
}
