using EntitiesProcessingLib.Entities;
using EntitiesProcessingLib.Repositories;
using Terminal.Gui;

namespace UserInterface
{
    public class LectureInfo : Dialog
    {
        private LectureRepository _rep;
        private TextField _idView;
        private TextField _themeView;
        private TextField _courseIDView;
        private CheckBox _importedBox;

        public LectureInfo()
        {
            this.Width = 60;
            this.Height = 14;
            Button okBtn = new Button("Ok");
            okBtn.Clicked += Application.RequestStop;
            this.AddButton(okBtn);

            int yShift = 2;
            Label id = new Label(1, 0 * yShift, "ID: ");
            Label theme = new Label(1, 1 * yShift, "Theme: ");
            Label courseID = new Label(1, 2 * yShift, "Course ID: ");
            Label imported = new Label(1, 3 * yShift, "Is imported: ");
            this.Add(id, theme, courseID, imported);

            int xShift = 15;
            _idView = new TextField() {
                X = xShift, Y = 0 * yShift, ReadOnly = true, Width = 50,
            };
            _themeView = new TextField() {
                X = xShift, Y = 1 * yShift, ReadOnly = true, Width = 50,
            };
            _courseIDView = new TextField() {
                X = xShift, Y = 2 * yShift, ReadOnly = true, Width = 50,
            };
            _importedBox = new CheckBox() {
                X = xShift, Y = 3* yShift,
            };
            _importedBox.Toggled += OnToggled;
            this.Add(_idView, _themeView, _courseIDView, _importedBox);

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
                LectureUpdate dlg = new LectureUpdate();
                long id = long.Parse(_idView.Text.ToString());
                dlg.SetLecture(_rep.GetLecture(id));
                Application.Run(dlg);

                if (dlg.canceled)
                {
                    return;
                }

                this.SetLecture(dlg.Lecture);
                _rep.Update(id, dlg.Lecture);
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

        public void SetRepository(LectureRepository rep)
        {
            _rep = rep;
        }

        public void SetLecture(Lecture l)
        {
            _idView.Text = l.ID.ToString();
            _themeView.Text = l.Theme;
            _courseIDView.Text = l.Course.ID.ToString();
            _importedBox.Checked = l.IsImported;
        }

        private void OnToggled(bool obj)
        {
            _importedBox.Checked = obj;
        }
    }
}