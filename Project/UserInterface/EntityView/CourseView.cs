using Terminal.Gui;
using EntitiesProcessingLib.Repositories;
using EntitiesProcessingLib.Entities;
using ServiceLib;
using NStack;

namespace UserInterface
{
    public class CourseView : Window
    {
        private int _pageSize = 15;
        private int _page = 1;
        private long _authorID = -1;
        private int _totalPages;
        private User _loginedUser;
        private ListView _view;
        private RemoteService _service;
        private Label _pageLabel;
        private Button _prevPage;
        private Button _nextPage;
        private CheckBox _onlyUser;
        private TextField _searchField;

        public CourseView()
        {
            FrameView frame = new FrameView
            {
                Title = "Courses",
                X = 0,
                Y = 4, 
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            Label onlyUser = new Label("I'm the author") {
                X = Pos.Percent(70), Y = 3,
            };
            _view = new ListView
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            _onlyUser = new CheckBox() {
                X = Pos.Percent(70) + "I'm the author".Length + 1, Y = 3,
            };
            _searchField = new TextField() {
                X = 2, Y = 3, Width = 40,
            };
            frame.Add(_view);
            this.Add(frame);

            _prevPage = new Button(2, 2, "Prev");
            _nextPage = new Button(20, 2, "Next");
            _pageLabel = new Label(12, 2, "??? / ???");

            _prevPage.Clicked += OnPrev;
            _nextPage.Clicked += OnNext;
            _view.OpenSelectedItem += OnItemOpen;
            _onlyUser.Toggled += OnToggled;
            _searchField.TextChanged += OnSearch;
            
            this.Add(onlyUser, _onlyUser, _searchField);
            this.Add(_prevPage, _pageLabel, _nextPage);
        }

        private void OnSearch(ustring obj)
        {
            UpdateView();
        }

        public void LoginUser(User user)
        {
            _loginedUser = user;
            OnToggled(true);
        }

        private void OnToggled(bool previous)
        {
            _authorID = _onlyUser.Checked ? _loginedUser.ID : -1;
            if (_service != null)
            {
                UpdateView();
            }
        }

        private void OnItemOpen(ListViewItemEventArgs obj)
        {
            Course selected = (Course) obj.Value;
            CourseInfo dlg = new CourseInfo();
            dlg.LoginUser(_loginedUser);
            dlg.SetService(_service);
            dlg.SetCourse(selected);
            Application.Top.Add(dlg);
            Application.Run(dlg);
            Application.Top.Remove(dlg);
            UpdateView();
        }

        public void OnNext()
        {
            _page += 1;
            UpdateView();
        }

        public void OnPrev()
        {
            _page -= 1;
            UpdateView();
        }

        public void SetService(RemoteService service)
        {
            _service = service;
            UpdateView();
        }

        public void UpdateView()
        {
            string name = _searchField.Text.ToString();
            _totalPages = _service.GetTotalPagesCountCourse(_pageSize, _authorID, name);
            
            _page = _page > _totalPages ? _totalPages : _page;
            _page = _page == 0 ? 1 : _page;
            
            _prevPage.Visible = (_page != 1);
            _nextPage.Visible = (_page != _totalPages);

            _view.SetSource(_service.GetPageCourse(_page, _pageSize, name, _authorID));
            _pageLabel.Text = $"{_page} / {_totalPages}";
            
            if (_totalPages == 0)
            {
                _prevPage.Visible = false;
                _nextPage.Visible = false;
                _pageLabel.Text = "0 / 0";
            }
        }
    }
}