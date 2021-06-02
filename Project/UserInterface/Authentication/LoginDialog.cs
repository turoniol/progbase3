using Terminal.Gui;
using EntitiesProcessingLib.Entities;
using ServiceLib;

namespace UserInterface
{
    public class LoginDialog : Dialog 
    {
        public bool canceled;
        public User User {get; private set;}
        private RemoteService _service;
        private TextField _login;
        private TextField _password;

        public LoginDialog()
        {
            Width = 50;
            Height = 8;

            Button okBtn = new Button("Ok");
            Button cancelBtn = new Button("Cancel");

            okBtn.Clicked += OnApply;
            cancelBtn.Clicked += OnCancel;
            this.AddButton(okBtn);
            this.AddButton(cancelBtn);

            int xShift = 2, yShift = 2, fieldShift = 15;
            Label login = new Label(xShift,  0 * yShift, "Login: ");
            Label password = new Label(xShift, 1 * yShift, "Password: ");

            _login = new TextField(fieldShift, 0 * yShift, 30, "");
            _password = new TextField(fieldShift, 1 * yShift, 30, "") {
                Secret = true,
            };
            this.Add(login, password, _login, _password);
        }

        public void SetService(RemoteService service)
        {
            this._service = service;
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
            if (_login.Text.IsEmpty || _password.Text.IsEmpty)
            {
                MessageBox.ErrorQuery("Error", "Some field is empty!", "Ok");
            }
            else 
            {
                User = _service.Login(new User {
                    Login = _login.Text.ToString(),
                    Password = _password.Text.ToString(),
                });

                if (User == null)
                {
                    MessageBox.ErrorQuery("Error", "Invalid login or password!", "Ok");
                    return;
                }

                canceled = false;
                Application.RequestStop();
            }
        }
    }
}