using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic; 

namespace booking_system
{
    public partial class Form1 : Form
    {
        // Основные элементы для доступа из разных методов
        private DataGridView dgvTables;
        private DataGridView dgvReservations;
        private DataGridView dgvMenu;
        private DataGridView dgvOrders; 

        // Поля ввода для поиска бронирования
        private TextBox txtSearchName;
        private TextBox txtSearchPhoneLast4;
        private ListBox lbSearchResults;
        
        // Элементы статистики для быстрого доступа
        private Label lblRevenue;
        private ListBox lbDishStats;


        public Form1()
        {
            // InitializeComponent(); 
            
            // Инициализация данных
            BookingSystem.InitializeTables();
            OrderingSystem.InitializeMenu();
            
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Система Управления Рестораном (C# Windows Forms)";
            this.BackColor = Color.FromArgb(43, 43, 43); 
            this.ForeColor = Color.White;
            this.MinimumSize = new Size(1200, 800);
            this.Size = new Size(1200, 800); 

            // ----------------------------------------------------
            // TabControl для разделения систем
            // ----------------------------------------------------
            TabControl mainTabControl = new TabControl { Dock = DockStyle.Fill };
            
            // --- Вкладка 1: Бронирование ---
            TabPage bookingPage = new TabPage("Система Бронирования")
                { BackColor = Color.FromArgb(50, 50, 50), ForeColor = Color.White };
            
            // --- Вкладка 2: Заказы и Меню ---
            TabPage ordersPage = new TabPage("Система Заказов и Меню")
                { BackColor = Color.FromArgb(50, 50, 50), ForeColor = Color.White };
            
            mainTabControl.Controls.Add(bookingPage);
            mainTabControl.Controls.Add(ordersPage);
            this.Controls.Add(mainTabControl);

            // ----------------------------------------------------
            // 1. НАСТРОЙКА ВКЛАДКИ БРОНИРОВАНИЯ
            // ----------------------------------------------------
            SetupBookingPage(bookingPage);

            // ----------------------------------------------------
            // 2. НАСТРОЙКА ВКЛАДКИ ЗАКАЗОВ
            // ----------------------------------------------------
            SetupOrderingPage(ordersPage);

            // Первичное заполнение таблиц
            UpdateTablesView();
            UpdateReservationsView();
            UpdateOrdersView();
            UpdateMenuView();
        }
        
        // =========================================================================
        // МЕТОДЫ НАСТРОЙКИ ВКЛАДОК
        // =========================================================================

        private void SetupBookingPage(TabPage bookingPage)
        {
            // Главный макет для бронирования: 3 строки (Кнопки, Столы, Бронирования)
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(10)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Кнопки
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50)); // Столы
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50)); // Бронирования
            bookingPage.Controls.Add(mainLayout);

            // --- Панель Кнопок (Row 0, Span 2) ---
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            mainLayout.Controls.Add(buttonPanel, 0, 0);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            AddButton(buttonPanel, "Создать Бронирование", BtnNewReservation_Click, Color.FromArgb(0, 120, 215));
            AddButton(buttonPanel, "Отменить Бронь", BtnCancelReservation_Click, Color.FromArgb(200, 50, 50));
            AddButton(buttonPanel, "Редактировать Стол", BtnEditTable_Click, Color.FromArgb(0, 150, 0)); // ИСПОЛЬЗУЕТ DIALOG
            AddButton(buttonPanel, "Показать Инфо Стола", BtnShowTableInfo_Click, Color.FromArgb(100, 100, 100)); // ИСПОЛЬЗУЕТ DIALOG
            AddButton(buttonPanel, "Обновить", (s, e) => { UpdateTablesView(); UpdateReservationsView(); }, Color.FromArgb(60, 60, 60));

            // --- Таблица Столов (Row 1, Col 0) ---
            dgvTables = CreateDataGridView();
            GroupBox tableGroup = new GroupBox { Text = "Столы", Dock = DockStyle.Fill, ForeColor = Color.White };
            tableGroup.Controls.Add(dgvTables);
            mainLayout.Controls.Add(tableGroup, 0, 1);

            // --- Таблица Бронирований (Row 1, Col 1) ---
            dgvReservations = CreateDataGridView();
            GroupBox resGroup = new GroupBox { Text = "Бронирования", Dock = DockStyle.Fill, ForeColor = Color.White };
            resGroup.Controls.Add(dgvReservations);
            mainLayout.Controls.Add(resGroup, 1, 1);
            
            // --- Панель Поиска (Row 2, Span 2) ---
            GroupBox searchGroup = new GroupBox { Text = "Поиск Бронирования", Dock = DockStyle.Fill, ForeColor = Color.White };
            mainLayout.Controls.Add(searchGroup, 0, 2);
            mainLayout.SetColumnSpan(searchGroup, 2);

            FlowLayoutPanel searchLayout = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(5) };
            searchGroup.Controls.Add(searchLayout);

            // Элементы поиска
            txtSearchName = new TextBox { Width = 150 };
            txtSearchPhoneLast4 = new TextBox { Width = 80, MaxLength = 4 };
            Button btnSearch = new Button { Text = "Найти", Width = 80, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSearch.Click += BtnSearchReservation_Click;
            
            searchLayout.Controls.Add(new Label { Text = "Имя:", AutoSize = true, Padding = new Padding(0, 5, 5, 0) });
            searchLayout.Controls.Add(txtSearchName);
            searchLayout.Controls.Add(new Label { Text = "Последние 4 цифры тел.:", AutoSize = true, Padding = new Padding(10, 5, 5, 0) });
            searchLayout.Controls.Add(txtSearchPhoneLast4);
            searchLayout.Controls.Add(btnSearch);

            lbSearchResults = new ListBox { Width = 400, Height = 100, BackColor = Color.FromArgb(70, 70, 70), ForeColor = Color.White };
            searchLayout.Controls.Add(lbSearchResults);
        }

        private void SetupOrderingPage(TabPage ordersPage)
        {
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(10)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // Меню
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // Заказы
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Кнопки
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50)); // Таблицы
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50)); // Статистика
            ordersPage.Controls.Add(mainLayout);

            // --- Панель Кнопок (Row 0, Span 2) ---
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            mainLayout.Controls.Add(buttonPanel, 0, 0);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            AddButton(buttonPanel, "Создать Заказ", BtnNewOrder_Click, Color.FromArgb(215, 120, 0));
            AddButton(buttonPanel, "Добавить Блюдо в Заказ", BtnAddDishToOrder_Click, Color.FromArgb(150, 150, 0)); // ИСПОЛЬЗУЕТ DIALOG
            AddButton(buttonPanel, "Закрыть Заказ", BtnCloseOrder_Click, Color.FromArgb(0, 150, 0)); // ИСПОЛЬЗУЕТ DIALOG
            AddButton(buttonPanel, "Вывести Чек", BtnPrintReceipt_Click, Color.FromArgb(100, 100, 100)); // ИСПОЛЬЗУЕТ DIALOG
            AddButton(buttonPanel, "Обновить", (s, e) => { UpdateOrdersView(); UpdateMenuView(); UpdateStatistics(); }, Color.FromArgb(60, 60, 60));


            // --- Таблица Меню (Row 1, Col 0) ---
            dgvMenu = CreateDataGridView();
            GroupBox menuGroup = new GroupBox { Text = "Меню", Dock = DockStyle.Fill, ForeColor = Color.White };
            menuGroup.Controls.Add(dgvMenu);
            mainLayout.Controls.Add(menuGroup, 0, 1);

            // --- Таблица Заказов (Row 1, Col 1) ---
            dgvOrders = CreateDataGridView();
            GroupBox ordersGroup = new GroupBox { Text = "Заказы (Активные и Закрытые)", Dock = DockStyle.Fill, ForeColor = Color.White };
            ordersGroup.Controls.Add(dgvOrders);
            mainLayout.Controls.Add(ordersGroup, 1, 1);
            
            // --- Панель Статистики (Row 2, Span 2) ---
            GroupBox statsGroup = new GroupBox { Text = "Статистика", Dock = DockStyle.Fill, ForeColor = Color.White };
            mainLayout.Controls.Add(statsGroup, 0, 2);
            mainLayout.SetColumnSpan(statsGroup, 2);

            TableLayoutPanel statsLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            statsGroup.Controls.Add(statsLayout);

            // Общая выручка
            lblRevenue = new Label { Text = "Общая выручка: N/A", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            statsLayout.Controls.Add(lblRevenue, 0, 0);

            // Заказы официанта
            FlowLayoutPanel waiterStats = new FlowLayoutPanel { Dock = DockStyle.Fill };
            statsLayout.Controls.Add(waiterStats, 1, 0);
            Label lblWaiter = new Label { Text = "Официант ID:", AutoSize = true };
            NumericUpDown nudWaiterID = new NumericUpDown { Minimum = 1, Maximum = 999, Value = 101, Width = 50 };
            Button btnWaiterStats = new Button { Text = "Показать заказы", BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            Label lblWaiterOrders = new Label { Text = "Заказов: N/A", AutoSize = true };
            waiterStats.Controls.AddRange(new Control[] { lblWaiter, nudWaiterID, btnWaiterStats, lblWaiterOrders });
            
            btnWaiterStats.Click += (s, e) => 
            {
                int waiterId = (int)nudWaiterID.Value;
                int count = OrderingSystem.CountClosedOrdersByWaiter(waiterId);
                lblWaiterOrders.Text = $"Заказов: {count}";
            };
            
            // Статистика по блюдам
            lbDishStats = new ListBox { Dock = DockStyle.Fill, BackColor = Color.FromArgb(70, 70, 70), ForeColor = Color.White };
            statsLayout.Controls.Add(lbDishStats, 2, 0);

            UpdateStatistics();
        }

        // Вспомогательный метод для создания DataGridView
        private DataGridView CreateDataGridView()
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                BackgroundColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.Black,
                GridColor = Color.FromArgb(100, 100, 100),
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect // Оставляем, для кнопок, которые не были переделаны (Отменить Бронь)
            };
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(70, 70, 70);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.EnableHeadersVisualStyles = false;
            
            return dgv;
        }

        // Вспомогательный метод для создания кнопок
        private void AddButton(FlowLayoutPanel panel, string text, EventHandler clickHandler, Color color)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(160, 30),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5, 5, 5, 5)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += clickHandler;
            panel.Controls.Add(btn);
        }
        
        // =========================================================================
        // МЕТОДЫ ОБНОВЛЕНИЯ UI
        // =========================================================================

        private void UpdateTablesView()
        {
            dgvTables.DataSource = BookingSystem.Tables
                .Select(t => new 
                { 
                    ID = t.ID, 
                    Расположение = t.Location.ToString().Replace("_", " "), 
                    Мест = t.Capacity, 
                    Статус = t.Schedule.Any(kv => kv.Value != 0) ? "Занят" : "Свободен"
                }).ToList();
        }

        private void UpdateReservationsView()
        {
            dgvReservations.DataSource = BookingSystem.Reservations
                .Where(r => r.AssignedTable != null && r.EndTime > DateTime.Now) // Только будущие/активные
                .Select(r => new
                {
                    ID = r.ID,
                    Клиент = r.ClientName,
                    Телефон = r.ClientPhone,
                    Стол = r.AssignedTable.ID,
                    Начало = r.StartTime.ToString("HH:mm"),
                    Окончание = r.EndTime.ToString("HH:mm")
                }).ToList();
        }

        private void UpdateMenuView()
        {
            dgvMenu.DataSource = OrderingSystem.Menu
                .Select(d => new
                {
                    ID = d.ID,
                    Название = d.Name,
                    Категория = d.Category.ToString().Replace("_", " "),
                    Цена = d.Price,
                    Вес = d.Weight,
                    Готовка = $"{d.CookingTimeMinutes} мин"
                }).ToList();
        }
        
        private void UpdateOrdersView()
        {
            dgvOrders.DataSource = OrderingSystem.Orders
                .OrderByDescending(o => o.ClosureTime.HasValue) // Закрытые вниз
                .Select(o => new
                {
                    ID = o.ID,
                    Стол = o.TableID,
                    Официант = o.WaiterID,
                    Время = o.AcceptanceTime.ToString("HH:mm"),
                    Статус = o.ClosureTime.HasValue ? "Закрыт" : "Активен",
                    Итог = o.TotalCost
                }).ToList();
        }
        
        private void UpdateStatistics()
        {
            if (lblRevenue == null || lbDishStats == null) return; 
            
            lblRevenue.Text = $"Общая выручка: {OrderingSystem.GetTotalRevenue():C}";
            
            lbDishStats.Items.Clear();
            lbDishStats.Items.Add("Статистика по блюдам (закрытые заказы):");
            foreach (var item in OrderingSystem.GetDishStatistics())
            {
                lbDishStats.Items.Add($"{item.Key}: {item.Value} шт.");
            }
        }
        
        // =========================================================================
        // ОБРАБОТЧИКИ СОБЫТИЙ: БРОНИРОВАНИЕ
        // =========================================================================

        private void BtnNewReservation_Click(object sender, EventArgs e)
        {
            string name = Interaction.InputBox("Имя клиента:", "Создание брони", "Иван");
            string phone = Interaction.InputBox("Номер телефона:", "Создание брони", "88005553535");
            string capacityStr = Interaction.InputBox("Кол-во мест:", "Создание брони", "4");
            
            if (!int.TryParse(capacityStr, out int capacity)) return;

            // Используем ближайший час для текущей даты, чтобы бронь была в будущем
            DateTime now = DateTime.Now;
            var startTime = now.Date.AddHours(now.Hour < 22 ? now.Hour + 1 : 12); 
            var endTime = startTime.AddHours(2);
            
            try
            {
                 var availableTable = BookingSystem.GetAvailableTables(startTime, endTime, capacity).FirstOrDefault();

                if (availableTable != null)
                {
                    BookingSystem.CreateReservation(name, phone, startTime, endTime, availableTable.ID);
                    MessageBox.Show($"Бронь для {name} на стол {availableTable.ID} создана ({startTime:HH:mm}-{endTime:HH:mm})!", "Успех");
                    UpdateTablesView();
                    UpdateReservationsView();
                }
                else
                {
                    MessageBox.Show("Нет свободных столов, удовлетворяющих условиям.", "Ошибка");
                }
            }
            catch (InvalidOperationException ex)
            {
                 MessageBox.Show(ex.Message, "Ошибка Бронирования");
            }
        }

        private void BtnCancelReservation_Click(object sender, EventArgs e)
        {
            // Здесь оставлено использование выделения строки в DGV Бронирований
            if (dgvReservations.SelectedRows.Count == 0)
            {
                 MessageBox.Show("Выберите бронь для отмены.", "Внимание");
                 return;
            }

            if (!int.TryParse(dgvReservations.SelectedRows[0].Cells["ID"].Value.ToString(), out int reservationId)) return;
            
            var res = BookingSystem.Reservations.FirstOrDefault(r => r.ID == reservationId);

            if (res != null && MessageBox.Show($"Отменить бронь ID {reservationId}?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                res.Cancel(); 
                BookingSystem.Reservations.Remove(res); 
                UpdateTablesView();
                UpdateReservationsView();
                MessageBox.Show($"Бронь ID {reservationId} отменена.", "Успех");
            }
        }
        
        private void BtnShowTableInfo_Click(object sender, EventArgs e)
        {
            // ИСПОЛЬЗУЕМ DIALOG ДЛЯ ВЫБОРА СТОЛА
            var tableData = BookingSystem.Tables
                .Select(t => new { ID = t.ID, Расположение = t.Location.ToString().Replace("_", " "), Мест = t.Capacity, Статус = t.Schedule.Any(kv => kv.Value != 0) ? "Занят" : "Свободен" })
                .ToList();

            // <-- SelectionDialog больше не подчеркивается, т.к. находится в той же папке/проекте
            using (var dialog = new SelectionDialog("Выберите стол для просмотра информации", tableData)) 
            {
                if (dialog.ShowDialog() == DialogResult.OK && dialog.SelectedId != -1)
                {
                    int tableId = dialog.SelectedId;
                    var table = BookingSystem.Tables.FirstOrDefault(t => t.ID == tableId);

                    if (table != null)
                    {
                        MessageBox.Show(table.GetDetailedInfo(), $"Подробная информация о столе ID {tableId}");
                    }
                }
            }
        }
        
        private void BtnEditTable_Click(object sender, EventArgs e)
        {
            // ИСПОЛЬЗУЕМ DIALOG ДЛЯ ВЫБОРА СТОЛА
             var tableData = BookingSystem.Tables
                .Select(t => new { ID = t.ID, Расположение = t.Location.ToString().Replace("_", " "), Мест = t.Capacity, Статус = t.Schedule.Any(kv => kv.Value != 0) ? "Занят" : "Свободен" })
                .ToList();

            using (var dialog = new SelectionDialog("Выберите стол для редактирования", tableData))
            {
                if (dialog.ShowDialog() == DialogResult.OK && dialog.SelectedId != -1)
                {
                    int tableId = dialog.SelectedId;
                    var table = BookingSystem.Tables.FirstOrDefault(t => t.ID == tableId);

                    if (table != null)
                    {
                        // <-- Interaction больше не подчеркивается, т.к. добавлена using Microsoft.VisualBasic.Interaction
                        string newCapacityStr = Interaction.InputBox($"Новая вместимость для стола {tableId}:", "Редактирование стола", table.Capacity.ToString());
                        if (int.TryParse(newCapacityStr, out int newCapacity) && newCapacity > 0)
                        {
                            if (BookingSystem.EditTableInfo(tableId, table.Location, newCapacity))
                            {
                                MessageBox.Show("Информация о столе обновлена.", "Успех");
                                UpdateTablesView();
                            }
                            else
                            {
                                MessageBox.Show("Невозможно редактировать: стол фигурирует в активном бронировании.", "Ошибка");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Некорректное значение вместимости.", "Ошибка Ввода");
                        }
                    }
                }
            }
        }
        
        private void BtnSearchReservation_Click(object sender, EventArgs e)
        {
            string name = txtSearchName.Text.Trim();
            string last4 = txtSearchPhoneLast4.Text.Trim();
            
            if (string.IsNullOrEmpty(name) || last4.Length != 4)
            {
                MessageBox.Show("Пожалуйста, введите имя и последние 4 цифры телефона.", "Ввод данных");
                return;
            }

            var results = BookingSystem.SearchReservations(last4, name);

            lbSearchResults.Items.Clear();
            if (results.Any())
            {
                foreach (var r in results)
                {
                    lbSearchResults.Items.Add($"ID {r.ID}, Стол {r.AssignedTable?.ID}, Время: {r.StartTime:HH:mm}-{r.EndTime:HH:mm}");
                }
            }
            else
            {
                lbSearchResults.Items.Add("Бронирования не найдены.");
            }
        }

        // =========================================================================
        // ОБРАБОТЧИКИ СОБЫТИЙ: ЗАКАЗЫ
        // =========================================================================

        private void BtnNewOrder_Click(object sender, EventArgs e)
        {
            string tableIdStr = Interaction.InputBox("ID стола для заказа:", "Новый Заказ", "1");
            string waiterIdStr = Interaction.InputBox("ID официанта:", "Новый Заказ", "101");

            if (int.TryParse(tableIdStr, out int tableId) && int.TryParse(waiterIdStr, out int waiterId))
            {
                // Проверка на активный заказ
                if (OrderingSystem.Orders.Any(o => o.TableID == tableId && !o.ClosureTime.HasValue))
                {
                    MessageBox.Show($"На столе {tableId} уже есть активный заказ.", "Ошибка Создания Заказа");
                    return;
                }
                
                var order = OrderingSystem.CreateOrder(tableId, waiterId);
                MessageBox.Show($"Создан новый заказ ID {order.ID} для стола {tableId}.", "Успех");
                UpdateOrdersView();
            }
            else
            {
                MessageBox.Show("Некорректный ввод ID стола или официанта.", "Ошибка Ввода");
            }
        }

        private void BtnAddDishToOrder_Click(object sender, EventArgs e)
        {
            // Убедимся, что блюдо выбрано в таблице меню
            if (dgvMenu.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, сначала выберите блюдо из таблицы Меню (слева).", "Внимание");
                return;
            }
            
            // 1. Создаем список АКТИВНЫХ заказов для выбора (ЧЕРЕЗ DIALOG)
            var activeOrdersData = OrderingSystem.Orders
                .Where(o => !o.ClosureTime.HasValue) 
                .Select(o => new { ID = o.ID, Стол = o.TableID, Официант = o.WaiterID, Итог = o.TotalCost })
                .ToList();

            if (!activeOrdersData.Any())
            {
                MessageBox.Show("Нет активных заказов, в которые можно добавить блюдо.", "Внимание");
                return;
            }
                
            using (var dialog = new SelectionDialog("Выберите АКТИВНЫЙ заказ для добавления блюда", activeOrdersData))
            {
                if (dialog.ShowDialog() == DialogResult.OK && dialog.SelectedId != -1)
                {
                    int orderId = dialog.SelectedId;
                    // Получаем ID блюда из выбранной строки DGV Menu
                    if (!int.TryParse(dgvMenu.SelectedRows[0].Cells["ID"].Value.ToString(), out int dishId)) return;
                    
                    var order = OrderingSystem.Orders.FirstOrDefault(o => o.ID == orderId);
                    var dish = OrderingSystem.Menu.FirstOrDefault(d => d.ID == dishId);

                    if (order != null && dish != null)
                    {
                         string quantityStr = Interaction.InputBox($"Количество {dish.Name}:", "Добавить Блюдо", "1");
                         if (int.TryParse(quantityStr, out int quantity) && quantity > 0)
                         {
                            order.AddItem(dish, quantity);
                            MessageBox.Show($"Добавлено {quantity}x {dish.Name} в заказ {orderId}. Итог: {order.TotalCost:C}", "Успех");
                            UpdateOrdersView(); 
                         }
                         else
                         {
                             MessageBox.Show("Некорректное количество.", "Ошибка Ввода");
                         }
                    }
                }
            }
        }

        private void BtnCloseOrder_Click(object sender, EventArgs e)
        {
            // 1. Создаем список АКТИВНЫХ заказов для выбора (ЧЕРЕЗ DIALOG)
            var activeOrdersData = OrderingSystem.Orders
                .Where(o => !o.ClosureTime.HasValue) 
                .Select(o => new { ID = o.ID, Стол = o.TableID, Официант = o.WaiterID, Итог = o.TotalCost })
                .ToList();

            if (!activeOrdersData.Any())
            {
                MessageBox.Show("Нет активных заказов для закрытия.", "Внимание");
                return;
            }
            
            using (var dialog = new SelectionDialog("Выберите АКТИВНЫЙ заказ для закрытия", activeOrdersData))
            {
                if (dialog.ShowDialog() == DialogResult.OK && dialog.SelectedId != -1)
                {
                    int orderId = dialog.SelectedId;
                    var order = OrderingSystem.Orders.FirstOrDefault(o => o.ID == orderId);

                    if (order != null && !order.ClosureTime.HasValue)
                    {
                        order.CloseOrder();
                        MessageBox.Show($"Заказ ID {orderId} закрыт. Итого: {order.TotalCost:C}", "Заказ закрыт");
                        UpdateOrdersView();
                        UpdateStatistics(); 
                    }
                }
            }
        }

        private void BtnPrintReceipt_Click(object sender, EventArgs e)
        {
            // 1. Создаем список ЗАКРЫТЫХ заказов для выбора чека (ЧЕРЕЗ DIALOG)
            var closedOrdersData = OrderingSystem.Orders
                .Where(o => o.ClosureTime.HasValue) 
                .Select(o => new { ID = o.ID, Стол = o.TableID, Официант = o.WaiterID, ВремяЗакрытия = o.ClosureTime.Value.ToString("HH:mm"), Итог = o.TotalCost })
                .ToList();

            if (!closedOrdersData.Any())
            {
                MessageBox.Show("Нет закрытых заказов для печати чека.", "Внимание");
                return;
            }

            using (var dialog = new SelectionDialog("Выберите ЗАКРЫТЫЙ заказ для вывода чека", closedOrdersData))
            {
                if (dialog.ShowDialog() == DialogResult.OK && dialog.SelectedId != -1)
                {
                    int orderId = dialog.SelectedId;
                    var order = OrderingSystem.Orders.FirstOrDefault(o => o.ID == orderId);

                    if (order != null && order.ClosureTime.HasValue)
                    {
                        // Важно: Вызов GenerateReceipt должен быть исправлен в файле Order.cs, если он не принимает аргументов.
                        // Используем имеющиеся TableID и WaiterID из объекта Order
                        string receipt = order.GenerateReceipt(); 
                        MessageBox.Show(receipt, $"Чек для заказа ID {orderId}");
                    }
                }
            }
        }
    }
}