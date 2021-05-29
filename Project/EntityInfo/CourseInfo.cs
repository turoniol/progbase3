using EntitiesProcessingLib.Entities;
using EntitiesProcessingLib.Repositories;
using Terminal.Gui;

namespace UserInterface
{
    public class CourseInfo : Dialog
    {
        private CourseRepository _rep;
        private TextField _idView;
        private TextField _titleView;
        private TextField _authorIDView;
        private CheckBox _importedBox;

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
                CourseUpdate dlg = new CourseUpdate();
                long id = long.Parse(_idView.Text.ToString());
                dlg.SetCourse(_rep.GetCourse(id));
                Application.Run(dlg);

                if (dlg.canceled)
                {
                    return;
                }

                this.SetCourse(dlg.Course);
                _rep.Update(id, dlg.Course);
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

        public void SetRepository(CourseRepository rep)
        {
            _rep = rep;
        }

        public void SetCourse(Course d)
        {
            _idView.Text = d.ID.ToString();
            _titleView.Text = d.Title;
            _authorIDView.Text = d.Author.ID.ToString();
            _importedBox.Checked = d.IsImported;
        }

        private void OnToggled(bool obj)
        {
            _importedBox.Checked = obj;
        }
    }
}