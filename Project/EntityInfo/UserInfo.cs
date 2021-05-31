using EntitiesProcessingLib.Entities;
using EntitiesProcessingLib.Repositories;
using EntitiesProcessingLib.Authentication;
using Terminal.Gui;

namespace UserInterface
{
    public class UserInfo : Dialog
    {
        private User _loginedUser;
        private UserRepository _rep;
        private TextField _idView;
        private TextField _loginView;
        private TextField _fullnameView;
        private Button _deleteBtn;
        private Button _updateBtn;

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
            Label fullname = new Label(1, 2 * yShift, "Fullname: ");
            this.Add(id, login, fullname);

            int xShift = 15;
            _idView = new TextField() {
                X = xShift, Y = 0 * yShift, ReadOnly = true, Width = 50,
            };
            _loginView = new TextField() {
                X = xShift, Y = 1 * yShift, ReadOnly = true, Width = 50,
            };
            _fullnameView = new TextField() {
                X = xShift, Y = 2 * yShift, ReadOnly = true, Width = 50,
            };

            this.Add(_idView, _loginView, _fullnameView);

            _deleteBtn = new Button("Delete") {
                X = 1, Y = 4 * yShift,
            };
            _updateBtn = new Button("Update") {
                X = 15, Y = 4 * yShift,
            };
            _deleteBtn.Clicked += OnDelete;
            _updateBtn.Clicked += OnUpdate;
            this.Add(_deleteBtn, _updateBtn);
        }

        public void LoginUser(User user)
        {
            _loginedUser = user;
        }

        private void OnUpdate()
        {
            try 
            {
                UserUpdate dlg = new UserUpdate();
                dlg.SetUser(_loginedUser);
                Application.Run(dlg);

                if (dlg.canceled)
                {
                    return;
                }

                this.SetUser(dlg.User);
                Authenticator auth = new Authenticator(_rep);
                auth.UpdateUser(dlg.User);
            }
            catch
            {
                MessageBox.ErrorQuery("Error", "Some field is empty!", "Ok");
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
            _fullnameView.Text = l.Fullname;

            _deleteBtn.Visible = l.ID == _loginedUser.ID;
            _updateBtn.Visible = l.ID == _loginedUser.ID;
        }
    }
}