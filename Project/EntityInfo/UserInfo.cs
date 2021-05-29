using EntitiesProcessingLib.Entities;
using EntitiesProcessingLib.Repositories;
using Terminal.Gui;

namespace UserInterface
{
    public class UserInfo : Dialog
    {
        private UserRepository _rep;
        private TextField _idView;
        private TextField _loginView;
        private TextField _passwordView;
        private TextField _fullnameView;

        public UserInfo()
        {
            this.Width = 60;
            this.Height = 14;
            Button okBtn = new Button("Ok");
            okBtn.Clicked += Application.RequestStop;
            this.AddButton(okBtn);

            int yShift = 2;
            Label id = new Label(1, 0 * yShift, "ID: ");
            Label login = new Label(1, 1 * yShift, "Login: ");
            Label password = new  Label(1, 2 * yShift, "Password: ");
            Label fullname = new Label(1, 3 * yShift, "Fullname: ");
            this.Add(id, login, password, fullname);

            int xShift = 15;
            _idView = new TextField() {
                X = xShift, Y = 0 * yShift, ReadOnly = true, Width = 50,
            };
            _loginView = new TextField() {
                X = xShift, Y = 1 * yShift, ReadOnly = true, Width = 50,
            };
            _passwordView = new TextField() {
                X = xShift, Y = 2 * yShift, ReadOnly = true, Width = 50,
            };
            _fullnameView = new TextField() {
                X = xShift, Y = 3 * yShift, ReadOnly = true, Width = 50,
            };

            this.Add(_idView, _loginView, _passwordView, _fullnameView);

            Button deleteBtn = new Button("Delete") {
                X = 1, Y = 4 * yShift,
            };
            Button updateBtn = new Button("Update") {
                X = 15, Y = 4 * yShift,
            };
            deleteBtn.Clicked += OnDelete;
            updateBtn.Clicked += OnUpdate;
            this.Add(deleteBtn, updateBtn);
        }

        private void OnUpdate()
        {
            
            try 
            {
                UserUpdate dlg = new UserUpdate();
                long id = long.Parse(_idView.Text.ToString());
                dlg.SetUser(_rep.GetUser(id));
                Application.Run(dlg);

                if (dlg.canceled)
                {
                    return;
                }

                this.SetUser(dlg.User);
                _rep.Update(id, dlg.User);
            }
            catch
            {
                MessageBox.ErrorQuery("Error", "Some fields is empty!", "Ok");
            }
        }

        private void OnDelete()
        {
            int decision = MessageBox.Query("Deleting", "Do you reaaly want to delete?", "Yes", "No");
            if (decision != 0)
            {
                return;
            }

            _rep.Delete(long.Parse(_idView.Text.ToString()));
            Application.RequestStop();
        }

        public void SetRepository(UserRepository rep)
        {
            _rep = rep;
        }

        public void SetUser(User l)
        {
            _idView.Text = l.ID.ToString();
            _loginView.Text = l.Login;
            _passwordView.Text = l.Password;
            _fullnameView.Text = l.Fullname;
        }
    }
}