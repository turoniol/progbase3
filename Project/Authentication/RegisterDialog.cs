using Terminal.Gui;
using EntitiesProcessingLib.Entities;
using EntitiesProcessingLib.Repositories;
using EntitiesProcessingLib.Authentication;

namespace UserInterface
{
    public class RegisterDialog : Dialog 
    {
        public bool canceled;
        public User User {get; private set;}
        private UserRepository _rep;
        private TextField _login;
        private TextField _password;
        private TextField _secondPassword;
        private TextField _fullname;

        public RegisterDialog()
        {
            Width = 50;
            Height = 12;

            Button okBtn = new Button("Ok");
            Button cancelBtn = new Button("Cancel");

            okBtn.Clicked += OnApply;
            cancelBtn.Clicked += OnCancel;
            this.AddButton(okBtn);
            this.AddButton(cancelBtn);

            int xShift = 2, yShift = 2, fieldShift = 15;
            Label login = new Label(xShift,  0 * yShift, "Login: ");
            Label password = new Label(xShift, 1 * yShift, "Password: ");
            Label secondPassword = new Label(xShift, 2 * yShift, "Repeat pas: ");
            Label fullname = new Label(xShift, 3 * yShift, "Fullname: ");

            _login = new TextField(fieldShift, 0 * yShift, 30, "");
            _password = new TextField(fieldShift, 1 * yShift, 30, "") {
                Secret = true,
            };
            _secondPassword = new TextField(fieldShift, 2 * yShift, 30, "") {
                Secret = true,
            };
            _fullname = new TextField(fieldShift, 3 * yShift, 30, "");
            this.Add(login, password, secondPassword, fullname, _login, _password, _secondPassword, _fullname);
        }

        public void SetRepository(UserRepository rep)
        {
            this._rep = rep;
        }

        private void OnCancel()
        {
            int decision = MessageBox.Query("Canceling", "Cancel?", "Yes", "No");
            if (decision == 1)
            {
                return;
            }
            canceled = true;
            User = null;
            Application.RequestStop();
        }

        private void OnApply()
        {
            if (_login.Text.IsEmpty || _password.Text.IsEmpty || _fullname.Text.IsEmpty)
            {
                MessageBox.ErrorQuery("Error", "Some field is empty!", "Ok");
            }
            else if (!_password.Text.Equals(_secondPassword.Text))
            {
                MessageBox.ErrorQuery("Error", "Passwords aren't equal", "Ok");
            }
            else 
            {
                Authenticator auth = new Authenticator(_rep);
                User = auth.Register(new User {
                    Login = _login.Text.ToString(),
                    Password = _password.Text.ToString(),
                    Fullname = _fullname.Text.ToString(),
                });
                
                if (User == null)
                {
                    MessageBox.ErrorQuery("Error", "User with this login already exist!", "Ok");
                    return;
                }

                canceled = false;
                Application.RequestStop();
            }
        }
    }
}