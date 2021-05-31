using Terminal.Gui;
using EntitiesProcessingLib.Repositories;
using EntitiesProcessingLib.Entities;
using System;

namespace UserInterface
{
    public class AuthenticationWindow : Window 
    {
        private UserRepository _rep;
        public User User {get; private set;}

        public AuthenticationWindow(UserRepository rep)
        {
            _rep = rep;

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
            dlg.SetRepository(_rep);
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
            dlg.SetRepository(_rep);
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