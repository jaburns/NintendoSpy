using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

using RetroSpy.Readers;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Reflection;

namespace RetroSpy
{
    public partial class AddRemoveWindow : Window
    {
        private List<string> excluded;
        IReadOnlyList<InputSource> _allSources;
        List<string> originalExcludedList;

        public AddRemoveWindow(IReadOnlyList<InputSource> allSources, List<string> excludedList)
        {
            originalExcludedList = excludedList;
            excluded = new List<string>(excludedList);
            _allSources = allSources;

            InitializeComponent();

            populateBoxes();
        }

        private void populateBoxes()
        {
            Included.Items.Clear();
            Excluded.Items.Clear();
            foreach(var source in _allSources)
            {
                if (excluded.Contains(source.Name))
                {
                    Excluded.Items.Add(source.Name);
                }
                else
                {
                    Included.Items.Add(source.Name);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            originalExcludedList.Clear();
            foreach(var source in excluded)
            {
                originalExcludedList.Add(source);
            }
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (Included.SelectedIndex != -1)
            {
                var selectedIndex = Included.SelectedIndex;
                var item = Included.SelectedItem;

                if (Included.SelectedIndex == Included.Items.Count - 1 && Included.Items.Count > 1)
                    selectedIndex = selectedIndex - 1;
                else if (Included.Items.Count == 1)
                    selectedIndex = -1;

                excluded.Add((string)Included.Items[Included.SelectedIndex]);

                populateBoxes();
                Included.SelectedIndex = selectedIndex;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (Excluded.SelectedIndex != -1)
            {
                var selectedIndex = Excluded.SelectedIndex;
                var item = Excluded.SelectedItem;
                if (Excluded.SelectedIndex == Excluded.Items.Count - 1 && Excluded.Items.Count > 1)
                    selectedIndex = selectedIndex - 1;
                else if (Excluded.Items.Count == 1)
                    selectedIndex = -1;

                excluded.Remove((string)Excluded.Items[Excluded.SelectedIndex]);

                populateBoxes();
                Excluded.SelectedIndex = selectedIndex;
            }
        }
    }
}

