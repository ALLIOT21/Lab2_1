using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Lab2_1
{
    public partial class Form1 : Form
    {
        private readonly List<Control> RequiredControls;
        private readonly List<Person> Persons;

        private readonly PersonsSerializer _personsSerializer;

        public Form1()
        {
            InitializeComponent();
            RequiredControls = new List<Control>
            {
                textBoxSurname,
                textBoxName,
                textBoxPatronymic,
                dateTimePickerBirthDate,
                comboBoxGender,
                dateTimePickerIssueDate,
                textBoxIssuePlace
            };
            _personsSerializer = new PersonsSerializer();
            Persons = _personsSerializer.DeserializePersons();
            FillListViewPersons();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!ValidateRequiredControls())
            {
                return;
            }
            var newPerson = new Person
            {
                Id = Persons.Any() ? Persons.Max(p => p.Id) + 1 : 1,
                Surname = textBoxSurname.Text,
                Name = textBoxName.Text,
                Patronymic = textBoxPatronymic.Text,
                BirthDate = DateTime.Parse(dateTimePickerBirthDate.Text),
                IssueDate = DateTime.Parse(dateTimePickerIssueDate.Text),
                IssuePlace = textBoxIssuePlace.Text,
                Gender = GetGender(comboBoxGender.Text),
            };

            Persons.Add(newPerson);
            FillListViewPersons();
            RequiredControls.ForEach(c => c.Text = string.Empty);
        }

        private bool ValidateRequiredControls()
        {
            var isValid = true;
            foreach (var control in RequiredControls)
            {
                if (IsControlEmpty(control))
                {
                    isValid = false;
                    HighlightEmptyControl(control);
                }
            }
            return isValid;
        }

        private bool IsControlEmpty(Control control)
        {
            return string.IsNullOrWhiteSpace(control.Text);
        }

        private void HighlightEmptyControl(Control control)
        {
            control.ForeColor = Color.Red;
            control.Refresh();
        }

        private Gender GetGender(string value)
        {
            return GetValueFromDescription<Gender>(value);
        }

        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var genders = GetGenders();
            comboBoxGender.Items.AddRange(genders.ToArray<object>());
        }

        private List<string> GetGenders()
        {
            var genders = new List<string>();
            foreach (var item in Enum.GetValues(typeof(Gender)))
            {
                FieldInfo fi = item.GetType().GetField(item.ToString());
                DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                genders.Add(attributes.First().Description);
            }
            return genders;
        }

        private void Form1_Closed(object sender, FormClosedEventArgs e)
        {
            _personsSerializer.SerializePersons(Persons);
        }

        private void FillListViewPersons()
        {
            var items = new List<ListViewItem>();
            foreach (var person in Persons)
            {
                var item = new ListViewItem(person.Id.ToString());
                item.SubItems.Add(person.Surname);
                item.SubItems.Add(person.Name);
                item.SubItems.Add(person.Patronymic);
                item.SubItems.Add(person.BirthDate.ToShortDateString());
                item.SubItems.Add(person.Gender.GetDescription());
                item.SubItems.Add(person.IssueDate.ToShortDateString());
                item.SubItems.Add(person.IssuePlace);
                items.Add(item);
            }
            listViewPersons.Clear();
            listViewPersons.Items.AddRange(items.ToArray());
        }
    }
}
