using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using thread.Commands;

namespace thread.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {

        private ObservableCollection<string> _wordsList = new ObservableCollection<string>();

        public ObservableCollection<string> WordsList
        {
            get { return _wordsList; }
            set { _wordsList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _wordsMixList = new ObservableCollection<string>();

        public ObservableCollection<string> WordsMixList
        {
            get { return _wordsMixList; }
            set { _wordsMixList = value; OnPropertyChanged(); }
        }
        
        private string _word;
        
        public string Words
        {
            get { return _word; }
            set { _word = value; OnPropertyChanged(); }
        }


        public static bool IsPlay { get; set; }

        public static bool IsResume { get; set; }


        public RelayCommand EnterCommand { get; set; }
        public RelayCommand PlayCommand { get; set; }
        public RelayCommand PauseCommand { get; set; }
        public RelayCommand ResumeCommand { get; set; }
        Thread thread;

        public void Load()
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                thread = new Thread(() =>
                {
                    int maxIndex = WordsList.Count - 1;
                    for (int i = maxIndex; i >= 0; i--)
                    {
                        if (IsPlay)
                        {
                            Thread.Sleep(1000);
                            timer_Tick(i);
                        }
                    }
                });
                thread.Start();
            });
        }

        public MainWindowViewModel()
        {
            EnterCommand = new RelayCommand((obj) =>
            {
                WordsList.Add(Words);
                if (IsPlay)
                {
                    Load();
                }
            });

            PlayCommand = new RelayCommand((obj) =>
            {
                if (!IsResume)
                {
                    IsPlay = true;
                    IsResume = true;
                    Load();
                }
                else
                {
                    MessageBox.Show("You pressed the Play button without including the words or second time", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });


            ResumeCommand = new RelayCommand((obj) =>
            {
                if (IsResume)
                {
                    IsPlay = true;

                    if (IsPlay)
                    {
                        Load();
                    }
                }
                else
                {
                    MessageBox.Show("You pressed the Resume button before Play button or without including the words", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });

            PauseCommand = new RelayCommand((obj) =>
            {

                IsPlay = false;

            });
        }
        static string sha256(string randomString)
        {
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString($"x2");
            }
            return hash;
        }

        private void timer_Tick(int index)
        {
            if (IsPlay)
            {
                App.Current.Dispatcher.Invoke((System.Action)delegate
                {
                    try
                    {
                        var mix = sha256(WordsList[index]);
                        WordsMixList.Add(mix);
                        WordsList.Remove(WordsList[index]);

                    }
                    catch (Exception)
                    {
                    }
                });
            }
        }
    }
}
