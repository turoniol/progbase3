using System;
using EntitiesProcessingLib.Entities;
using EntitiesProcessingLib.Repositories;
using Terminal.Gui;

namespace UserInterface
{
    public class UserUpdate : Dialog
    {
        public bool canceled;
        public User User {get; private set;}
        private User _loginedUser;
        private TextField _idView;
        private TextField _loginView;
        private TextField _passwordView;
        private TextField _fullnameView;

        public UserUpdate()
        {
            this.Width = 60;
            this.Height = 14;
            Button okBtn = new Button("Ok");
            Button cancelBtn = new Button("Cancel");
            okBtn.Clicked += OnApply;
            cancelBtn.Clicked += OnCancel;

            this.AddButton(okBtn);
            this.AddButton(cancelBtn);

            int yShift = 2;
            Label id = new Label(1, 0 * yShift, "ID: ");
            Label login = new Label(1, 1 * yShift, "Login: ");
            Label password = new Label(1, 2 * yShift, "New password: ");
            Label fullname = new Label(1, 3 * yShift, "Fullname: ");
            this.Add(id, login, password, fullname);

            int xShift = 15;
            _idView = new TextField() {
                X = xShift, Y = 0 * yShift, Width = 50, ReadOnly = true,
            };
            _loginView = new TextField() {
                X = xShift, Y = 1 * yShift, Width = 50, ReadOnly = true,
            };
            _passwordView = new TextField() {
                X = xShift, Y = 2 * yShift, Width = 50, Secret = true,
            };
            _fullnameView = new TextField() {
                X = xShift, Y = 3 * yShift, Width = 50,
            };

            this.Add(_idView, _loginView, _passwordView, _fullnameView);
        }

        private void OnCancel()
        {
            canceled = true;
            User = null;
            Application.RequestStop();
        }

        private void OnApply()
        {
            canceled = false;
            User = new User {
                ID = long.Parse(_idView.Text.ToString()),
                Fullname = _fullnameView.Text.ToString(),
                Password = _passwordView.Text.ToString(),
                Login = _loginView.Text.ToString(),
            };
            Application.RequestStop();
        }

        public void SetUser(User l)
        {
            _loginedUser = l;
            _idView.Text = l.ID.ToString();
            _loginView.Text = l.Login;
            _fullnameView.Text = l.Fullname;
        }
    }
}