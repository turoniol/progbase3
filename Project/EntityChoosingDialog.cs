using Terminal.Gui;
using System.Collections.Generic;

namespace UserInterface
{
    public enum EntityType { User, Course, Lecture }

    public class EntityChoosingDialog : Dialog
    {
        public bool canceled;

        private RadioGroup _group;

        public EntityType Entity { get; private set; }

        public EntityChoosingDialog() : base ("Choose an entity")
        {
            this.Width = 30;
            this.Height = 10;

            Button cancelBtn = new Button("Cancel");
            Button okBtn = new Button("Ok");

            NStack.ustring[] items = new NStack.ustring[] {"User", "Course", "Lecture"};
            _group = new RadioGroup(1, 1, items);

            cancelBtn.Clicked += OnCancel;
            okBtn.Clicked += OnApply;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);
            this.Add(_group);
        }

        private void OnCancel()
        {
            canceled = true;
            Application.RequestStop();
        }

        private void OnApply()
        {
            canceled = false;
            Entity = (EntityType) _group.SelectedItem;
            Application.RequestStop();
        }
    }
}