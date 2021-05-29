using System;
using EntitiesProcessingLib.Entities;
using EntitiesProcessingLib.Repositories;
using Terminal.Gui;

namespace UserInterface
{
    public class CourseUpdate : Dialog
    {
        public bool canceled;
        public Course Course {get; private set;}
        private TextField _idView;
        private TextField _titleView;
        private TextField _authorIDView;
        private CheckBox _importedBox;

        public CourseUpdate()
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
            Label title = new Label(1, 1 * yShift, "Title: ");
            Label authorID = new Label(1, 2 * yShift, "Author ID: ");
            Label imported = new Label(1, 3 * yShift, "Is imported: ");
            this.Add(id, title, authorID, imported);

            int xShift = 15;
            _idView = new TextField() {
                X = xShift, Y = 0 * yShift, Width = 50, ReadOnly = true,
            };
            _titleView = new TextField() {
                X = xShift, Y = 1 * yShift, Width = 50,
            };
            _authorIDView = new TextField() {
                X = xShift, Y = 2 * yShift, Width = 50,
            };
            _importedBox = new CheckBox() {
                X = xShift, Y = 3 * yShift,
            };
            this.Add(_idView, _titleView, _authorIDView, _importedBox);
        }

        private void OnCancel()
        {
            canceled = true;
            Course = null;
            Application.RequestStop();
        }

        private void OnApply()
        {
            canceled = false;
            Course = new Course {
                ID = long.Parse(_idView.Text.ToString()),
                Title = _titleView.Text.ToString(),
                Author = new User { ID = long.Parse(_authorIDView.Text.ToString())},
                IsImported = _importedBox.Checked,
            };
            Application.RequestStop();
        }

        public void SetCourse(Course l)
        {
            _idView.Text = l.ID.ToString();
            _titleView.Text = l.Title;
            _authorIDView.Text = l.Author.ID.ToString();
            _importedBox.Checked = l.IsImported;
        }
    }
}