using Sonic.ViewModels;

namespace Sonic.Models
{
    public class UserProfile : BindableBase
    {
        private string _name = "Sonic User";
        private int _age = 18;
        private string _email = "sonic@example.com";

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public int Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
    }
}
