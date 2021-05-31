using System;
using EntitiesProcessingLib.Entities;
using EntitiesProcessingLib.Repositories;
using Terminal.Gui;

namespace UserInterface
{
    public class CourseInfo : Dialog
    {
        private User _loginedUser;
        private CourseRepository _courseRep;
        private SubscriptionRepository _subscriptionRep;
        private TextField _idView;
        private TextField _titleView;
        private TextField _authorIDView;
        private CheckBox _importedBox;
        private Button _deleteBtn;
        private Button _updateBtn;
        private Button _subscribeBtn;
        private Button _unsubscribeBtn;

        public CourseInfo()
        {
            this.Width = 60;
            this.Height = 14;
            Button okBtn = new Button("Ok");
            okBtn.Clicked += Application.RequestStop;
            this.AddButton(okBtn);

            int yShift = 2;
            Label id = new Label(1, 0 * yShift, "ID: ");
            Label title = new Label(1, 1 * yShift, "Title: ");
            Label authorID = new Label(1, 2 * yShift, "Author ID: ");
            Label imported = new Label(1, 3 * yShift, "Is imported: ");
            this.Add(id, title, authorID, imported);

            int xShift = 15;
            _idView = new TextField() {
                X = xShift, Y = 0 * yShift, ReadOnly = true, Width = 50,
            };
            _titleView = new TextField() {
                X = xShift, Y = 1 * yShift, ReadOnly = true, Width = 50,
            };
            _authorIDView = new TextField() {
                X = xShift, Y = 2 * yShift, ReadOnly = true, Width = 50,
            };
            _importedBox = new CheckBox() {
                X = xShift, Y = 3* yShift,
            };
            _importedBox.Toggled += OnToggled;
            this.Add(_idView, _titleView, _authorIDView, _importedBox);

            _deleteBtn = new Button("Delete") {
                X = 1, Y = 4 * yShift,
            };
            _updateBtn = new Button("Update") {
                X = 15, Y = 4 * yShift,
            };
            _subscribeBtn = new Button("Subscribe") {
                X = 1, Y = 4 * yShift,
            };
            _unsubscribeBtn = new Button("Unsubscribe") {
                X = 15, Y = 4 * yShift, Visible = false,
            };
            _deleteBtn.Clicked += OnDelete;
            _updateBtn.Clicked += OnUpdate;
            _subscribeBtn.Clicked += OnSubscribe;
            _unsubscribeBtn.Clicked += OnUnsubscribe;

            this.Add(_deleteBtn, _updateBtn, _subscribeBtn, _unsubscribeBtn);
        }

        private void OnUnsubscribe()
        {
            var sub = _subscriptionRep.GetSubscription(_loginedUser.ID, long.Parse(_idView.Text.ToString()));
            _subscriptionRep.Delete(sub.id);
            SetVisibility();
        }

        private void OnSubscribe()
        {
            _subscriptionRep.Insert(_loginedUser.ID, long.Parse(_idView.Text.ToString()));
            SetVisibility();
        }

        public void LoginUser(User user)
        {
            _loginedUser = user;
        }

        private void OnUpdate()
        {
            
            try 
            {
                CourseUpdate dlg = new CourseUpdate();
                long id = long.Parse(_idView.Text.ToString());
                dlg.SetCourse(_courseRep.GetCourse(id));
                Application.Run(dlg);

                if (dlg.canceled)
                {
                    return;
                }

                this.SetCourse(dlg.Course);
                _courseRep.Update(id, dlg.Course);
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

            _courseRep.Delete(long.Parse(_idView.Text.ToString()));
            Application.RequestStop();
        }

        public void SetRepository(CourseRepository courseRep, SubscriptionRepository subRep)
        {
            _courseRep = courseRep;
            _subscriptionRep = subRep;
        }

        public void SetCourse(Course d)
        {
            _idView.Text = d.ID.ToString();
            _titleView.Text = d.Title;
            _authorIDView.Text = d.Author.ID.ToString();
            _importedBox.Checked = d.IsImported;

            SetVisibility();
        }

        private void SetVisibility()
        {
            bool isAuthor = _loginedUser.ID == long.Parse(_authorIDView.Text.ToString());
            _deleteBtn.Visible = isAuthor;
            _updateBtn.Visible = isAuthor;
            _subscribeBtn.Visible = !isAuthor;
            _unsubscribeBtn.Visible = false;

            if (_subscriptionRep.GetSubscription(_loginedUser.ID, long.Parse(_idView.Text.ToString())) != null)
            {
                _subscribeBtn.Visible = false;
                _unsubscribeBtn.Visible = true;
            }
        }

        private void OnToggled(bool obj)
        {
            _importedBox.Checked = obj;
        }
    }
}