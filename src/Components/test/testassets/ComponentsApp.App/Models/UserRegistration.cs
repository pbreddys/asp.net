using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ComponentsApp.App.Models
{
    public class UserRegistration : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string _userName;
        bool _acceptsTerms;

        [Required]
        [StringLength(6)]
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You gotta tick the box!")]
        public bool AcceptsTerms
        {
            get => _acceptsTerms;
            set => SetProperty(ref _acceptsTerms, value);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
