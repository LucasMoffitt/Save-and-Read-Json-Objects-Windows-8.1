using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SaveReadJsonObjectWin8.Annotations;

namespace SaveReadJsonObjectWin8
{
    public sealed partial class Main : INotifyPropertyChanged
    {
        public ObservableCollection<Person> MyTestPeople { get; set; }
        private readonly PersonStorage _personStorage = new PersonStorage();

        private Person _selectedPersonObject;
        public Person SelectedPersonObject
        {
            get
            {
                return _selectedPersonObject;
            }
            set
            {
                if (value == null)
                    return;

                if (_selectedPersonObject == value)
                    return;

                if (_selectedPersonObject != null)
                    Save(value);

                _selectedPersonObject = value;
                OnPropertyChanged();
            }
        }

        public Main()
        {
            InitializeComponent();
            LoadSavedFiles();
        }

        private async void LoadSavedFiles()
        {
            MyTestPeople = await _personStorage.All();
            OnPropertyChanged("MyTestPeople");
        }

        private async void Save(Person person)
        {
            await _personStorage.Save(person);
        }

        private void New_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MyTestPeople.Add(new Person());
        }

        private void Delete_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _personStorage.Delete(_selectedPersonObject.Id);
            _selectedPersonObject = null;
            SelectedPersonObject = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}