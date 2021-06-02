using ServiceLib;
using EntitiesProcessingLib.Entities;
using Terminal.Gui;

namespace UserInterface
{
    public class CourseInfo : Dialog
    {
        private User _loginedUser;        
        private RemoteService _service;
        private TextField _idView;
        private TextField _titleView;
        private TextField _authorIDView;
        private CheckBox _importedBox;
        private CheckBox _isOpen;
        private Button _deleteBtn;
        private Button _updateBtn;
        private Button _subscribeBtn;
        private Button _unsubscribeBtn;

        public CourseInfo()
        {
            this.Width = 60;
            this.Height = 16;
            Button okBtn = new Button("Ok");
            okBtn.Clicked += Application.RequestStop;
            this.AddButton(okBtn);

            int yShift = 2;
            Label id = new Label(1, 0 * yShift, "ID: ");
            Label title = new Label(1, 1 * yShift, "Title: ");
            Label authorID = new Label(1, 2 * yShift, "Author ID: ");
            Label imported = new Label(1, 3 * yShift, "Is imported: ");
            Label open = new Label(1, 4 * yShift, "Is open:");
            this.Add(id, title, authorID, imported, open);

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
                X = xShift, Y = 3 * yShift,
            };
            _isOpen = new CheckBox() {
                X = xShift, Y = 4 * yShift,
            };
            _importedBox.Toggled += OnToggledImported;
            _isOpen.Toggled += OnToggledOpen;
            this.Add(_idView, _titleView, _authorIDView, _importedBox, _isOpen);

            _deleteBtn = new Button("Delete") {
                X = 1, Y = 5 * yShift,
            };
            _updateBtn = new Button("Update") {
                X = 15, Y = 5 * yShift,
            };
            _subscribeBtn = new Button("Subscribe") {
                X = 1, Y = 5 * yShift,
            };
            _unsubscribeBtn = new Button("Unsubscribe") {
                X = 15, Y = 5 * yShift, Visible = false,
            };
            _deleteBtn.Clicked += OnDelete;
            _updateBtn.Clicked += OnUpdate;
            _subscribeBtn.Clicked += OnSubscribe;
            _unsubscribeBtn.Clicked += OnUnsubscribe;

            this.Add(_deleteBtn, _updateBtn, _subscribeBtn, _unsubscribeBtn);
        }

        private void OnUnsubscribe()
        {
            var sub = _service.GetSubscription(new Subscription {
                userID = _loginedUser.ID, 
                courseID = long.Parse(_idView.Text.ToString()),
            });
            _service.DeleteSubscription(sub.id);
            SetVisibility();
        }

        private void OnSubscribe()
        {
            _service.Insert(new Subscription {
                userID = _loginedUser.ID, 
                courseID = long.Parse(_idView.Text.ToString()),
            });
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
                dlg.SetCourse(_service.GetCourse(id));
                Application.Run(dlg);

                if (dlg.canceled)
                {
                    return;
                }

                this.SetCourse(dlg.Course);
                _service.Update(id, dlg.Course);
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

            long courseId = long.Parse(_idView.Text.ToString());
            var sub = _service.GetSubscription(new Subscription
            {
                userID = _loginedUser.ID,
                courseID = courseId,
            });
            if (sub != null)
            {
                _service.DeleteSubscription(sub.id);
            }

            _service.DeleteCourse(courseId);
            _service.DeleteLectures(courseId);
            Application.RequestStop();
        }

        public void SetService(RemoteService service)
        {
            _service = service;
        }

        public void SetCourse(Course d)
        {
            _idView.Text = d.ID.ToString();
            _titleView.Text = d.Title;
            _authorIDView.Text = d.Author.ID.ToString();
            _importedBox.Checked = d.IsImported;
            _isOpen.Checked = d.CanSubcribe;

            SetVisibility();
        }

        private void SetVisibility()
        {
            bool isAuthor = _loginedUser.ID == long.Parse(_authorIDView.Text.ToString());
            _deleteBtn.Visible = isAuthor;
            _updateBtn.Visible = isAuthor;
            _subscribeBtn.Visible = !isAuthor && _isOpen.Checked;
            _unsubscribeBtn.Visible = false;

            var sub = _service.GetSubscription(new Subscription {
                userID = _loginedUser.ID, 
                courseID = long.Parse(_idView.Text.ToString()),
            });

            if (sub != null)
            {
                _subscribeBtn.Visible = false;
                _unsubscribeBtn.Visible = true;
            }
        }

        private void OnToggledImported(bool obj)
        {
            _importedBox.Checked = obj;
        }

        private void OnToggledOpen(bool obj)
        {
            _isOpen.Checked = obj;
        }
    }
}