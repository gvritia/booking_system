using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq; // Добавляем System.Linq

public class SelectionDialog : Form
{
    private DataGridView dgv;
    public int SelectedId { get; private set; } = -1;

    /// <summary>
    /// Создает диалоговое окно для выбора элемента из списка.
    /// </summary>
    /// <param name="title">Заголовок окна.</param>
    /// <param name="dataSource">Список объектов (анонимных типов, List<Table> и т.д.).</param>
    public SelectionDialog(string title, object dataSource)
    {
        Text = title;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(600, 450); 
        BackColor = Color.FromArgb(43, 43, 43); 

        dgv = new DataGridView
        {
            Dock = DockStyle.Top,
            Height = 350,
            DataSource = dataSource,
            AutoGenerateColumns = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor = Color.FromArgb(70, 70, 70),
            ForeColor = Color.Black,
            GridColor = Color.FromArgb(100, 100, 100),
            BorderStyle = BorderStyle.None
        };
        
        // Настройка стилей для DataGridView
        dgv.DefaultCellStyle.BackColor = Color.FromArgb(70, 70, 70);
        dgv.DefaultCellStyle.ForeColor = Color.White;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgv.EnableHeadersVisualStyles = false;
        
        dgv.DataBindingComplete += (s, e) => 
        {
            dgv.ClearSelection();
            if (dgv.Rows.Count > 0)
            {
                dgv.Rows[0].Selected = true; // Выбираем первую строку по умолчанию
            }
        };

        Button btnOk = new Button 
        { 
            Text = "Выбрать", 
            DialogResult = DialogResult.OK, 
            Location = new Point(Size.Width - 180, dgv.Height + 30), 
            Width = 150,
            BackColor = Color.FromArgb(0, 120, 215), 
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat 
        };
        Button btnCancel = new Button 
        { 
            Text = "Отмена", 
            DialogResult = DialogResult.Cancel, 
            Location = new Point(Size.Width - 350, dgv.Height + 30), 
            Width = 150,
            BackColor = Color.FromArgb(200, 50, 50), 
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        
        btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

        btnOk.Click += (sender, e) =>
        {
            if (dgv.SelectedRows.Count > 0)
            {
                var selectedRow = dgv.SelectedRows[0];
                // Важно: проверяем, что колонка "ID" существует и имеет целое число
                if (selectedRow.Cells.Cast<DataGridViewCell>().Any(c => c.OwningColumn.Name == "ID") && 
                    selectedRow.Cells["ID"].Value is int id)
                {
                    SelectedId = id;
                }
                else
                {
                    MessageBox.Show("Не удалось определить ID выбранного элемента. Убедитесь, что в списке есть колонка 'ID'.", "Ошибка Выбора", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None; 
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None; 
            }
        };

        Controls.Add(dgv);
        Controls.Add(btnOk);
        Controls.Add(btnCancel);
    }
}