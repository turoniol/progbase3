using Terminal.Gui;
using EntitiesProcessingLib.Entities;
using ServiceLib;

namespace UserInterface
{
    public class AuthenticationWindow : Window 
    {
        private RemoteService _service;
        public User User {get; private set;}

        public AuthenticationWindow(RemoteService service)
        {
            _service = service;

            int width = 20, height = 8;
            Title = "Authentication";
            Width = width;
            Height = height;
            X = Pos.Center();
            Y = Pos.Center();

            int yShift = 2, xShift = 2;
            Button loginBtn = new Button(xShift, 0 * yShift + 1, "Sign in");
            Button registerBtn = new Button(xShift, 1 * yShift + 1, "Sign up");

            loginBtn.Clicked += OnLogin;
            registerBtn.Clicked += OnRegister;

            this.Add(loginBtn, registerBtn);
        }

        private void OnRegister()
        {
            RegisterDialog dlg = new RegisterDialog();
            dlg.SetService(_service);
            Application.Run(dlg);

            if (dlg.canceled)
            {
                return;
            }

            User = dlg.User;
            MessageBox.Query("Message", "Succesfull!", "Ok");
            Application.RequestStop();
        }

        private void OnLogin()
        {
            LoginDialog dlg = new LoginDialog();
            dlg.SetService(_service);
            Application.Run(dlg);

            if (dlg.canceled)
            {
                return;
            }

            User = dlg.User;
            MessageBox.Query("Message", "Succesfull!", "Ok");
            Application.RequestStop();
        }
    }
}