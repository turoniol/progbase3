using Terminal.Gui;
using System;
using EntitiesProcessingLib.Entities;
using NStack;
using ServiceLib;

namespace UserInterface
{
    public class UserView : Window
    {
        public event Action UserDeleted;
        private int _pageSize = 15;
        private int _totalPages;
        private int _page = 1;
        private User _loginedUser;
        private ListView _view;
        private RemoteService _service;
        private Label _pageLabel;
        private Button _prevPage;
        private Button _nextPage;
        private TextField _searchField;

        public UserView()
        {
            FrameView frame = new FrameView
            {
                Title = "Users",
                X = 0,
                Y = 4, 
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            _view = new ListView
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            frame.Add(_view);
            this.Add(frame);

            _prevPage = new Button(2, 2, "Prev");
            _nextPage = new Button(20, 2, "Next");
            _pageLabel = new Label(12, 2, "??? / ???");
            _searchField = new TextField() {
                X = 2, Y = 3, Width = 40,
            };

            _prevPage.Clicked += OnPrev;
            _nextPage.Clicked += OnNext;
            _view.OpenSelectedItem += OnItemOpen;
            _searchField.TextChanged += OnSearch;

            this.Add(_prevPage, _pageLabel, _nextPage, _searchField);
        }

        public void LoginUser(User user)
        {
            _loginedUser = user;
        }

        private void OnSearch(ustring obj)
        {
            UpdateView();
        }

        private void OnItemOpen(ListViewItemEventArgs obj)
        {
            User selected = (User) obj.Value;
            UserInfo dlg = new UserInfo();
            dlg.LoginUser(_loginedUser);
            dlg.SetUser(selected);
            dlg.SetService(_service);
            Application.Run(dlg);

            if (dlg.deleted)
            {
                UserDeleted?.Invoke();
            }

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
            _totalPages = _service.GetTotalPagesCountUser(_pageSize, name);
            
            _page = _page > _totalPages ? _totalPages : _page;
            _page = _page == 0 ? 1 : _page;
            
            _prevPage.Visible = (_page != 1);
            _nextPage.Visible = (_page != _totalPages);

            if (_totalPages == 0)
            {
                _prevPage.Visible = false;
                _nextPage.Visible = false;
            }

            _view.SetSource(_service.GetPageUser(_page, _pageSize, name));
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