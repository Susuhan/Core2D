﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Core2D.Data;
using DocumentFormat.OpenXml.InkML;
using System.ComponentModel;
using System.Reactive;

namespace Core2D.UI.Views.Data
{
    /// <summary>
    /// Interaction logic for <see cref="RecordsControl"/> xaml.
    /// </summary>
    public class RecordsControl : UserControl
    {
        private DataGrid _rowsDataGrid;
        private IDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordsControl"/> class.
        /// </summary>
        public RecordsControl()
        {
            InitializeComponent();

            _rowsDataGrid = this.FindControl<DataGrid>("rowsDataGrid");

            DataContextChanged += RecordsControl_DataContextChanged;
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void RecordsControl_DataContextChanged(object sender, System.EventArgs e)
        {
            if (_database != null)
            {
                _database.PropertyChanged -= Database_PropertyChanged;
                _database = null;
            }

            ResetColumns();

            if (DataContext is IDatabase database)
            {
                _database = database;
                _database.PropertyChanged += Database_PropertyChanged;
                CreateColumns();
            }
        }

        private void Database_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Columns")
            {
                if (_database != null)
                {
                    ResetColumns();
                    CreateColumns();
                }
            }
        }

        private void Column_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                if (_database != null)
                {
                    UpdateHeaders();
                }
            }
        }

        private void CreateColumns()
        {
            for (int i = 0; i < _database.Columns.Length; i++)
            {
                var column = _database.Columns[i];
                var dataGridTextColumn = new DataGridTextColumn()
                {
                    Header = $"{column.Name}",
                    Binding = new Binding($"Values[{i}].Content"),
                    IsReadOnly = true
                };
                column.PropertyChanged += Column_PropertyChanged;
                _rowsDataGrid.Columns.Add(dataGridTextColumn);
            }
        }

        private void ResetColumns()
        {
            _rowsDataGrid.Columns.Clear();
        }

        private void UpdateHeaders()
        {
            for (int i = 0; i < _database.Columns.Length; i++)
            {
                var column = _database.Columns[i];
                _rowsDataGrid.Columns[i].Header = column.Name;
            }
        }
    }
}
