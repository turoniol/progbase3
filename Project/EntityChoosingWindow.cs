using Terminal.Gui;
using System;
using System.Collections.Generic;

namespace UserInterface
{
    public enum EntityType { Course, Lecture, User }
    
    public class EntityChoosingWindow : Window
    {
        public event Action ItemChanged;
        private RadioGroup _group;

        public EntityChoosingWindow() : base ("Choose")
        {
            Button cancelBtn = new Button("Cancel");
            Button okBtn = new Button("Ok");

            NStack.ustring[] items = new NStack.ustring[] {"Course", "Lecture", "User"};
            _group = new RadioGroup(1, 1, items);
            _group.SelectedItemChanged += OnItemChanged;

            this.Add(_group);
        }

        private void OnItemChanged(RadioGroup.SelectedItemChangedArgs obj)
        {
            ItemChanged?.Invoke();
        }

        public EntityType Selected()
        {
            return (EntityType) _group.SelectedItem;
        }
    }
}