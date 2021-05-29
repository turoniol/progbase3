using Terminal.Gui;
using EntitiesProcessingLib.Entities;

namespace UserInterface
{
    public class UserCreatingDialog : Dialog
    {
        public User user { get; private set; }

        public bool canceled;

        private TextField _login;

        private TextField _password;

        private TextField _fullname;

        public UserCreatingDialog() : base("New user")
        {
            int yShift = 2;
            this.Height = 12;
            this.Width = 50;
            // buttons
            Button cancelBtn = new Button("Cancel");
            Button okBtn = new Button("Ok");
            cancelBtn.Clicked += OnCancel;
            okBtn.Clicked += OnApply;
            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            // labels
            Label loginLbl = new Label(2, 1 * yShift, "Login:");
            Label passwordLbl = new Label(2, 2 * yShift, "Password: ");
            Label fullnameLbl = new Label(2, 3 * yShift, "Fullname: ");
            this.Add(loginLbl, passwordLbl, fullnameLbl);

            // text fields
            _login = new TextField(14, 1 * yShift, 30, "");
            _password = new TextField(14, 2 * yShift, 30, "")
            {
                Secret = true,
            };
            _fullname = new TextField(14, 3 * yShift, 30, "");
            this.Add(_login, _password, _fullname);
        }

        private void OnCancel()
        {
            canceled = true;
            Application.RequestStop();
        }

        private void OnApply()
        {
            canceled = false;
            user = new User() 
            {
                Login = _login.Text.ToString(),
                Password = _password.Text.ToString(),
                Fullname = _fullname.Text.ToString(),
            };
            Application.RequestStop();
        }
    }
}