using System;
using System.Windows.Forms;
using GrpcStationService;
using Google.Protobuf.WellKnownTypes; // Для работы с Timestamp
using System.Threading.Tasks;
using Grpc.Core;
using System.Linq;

namespace GrpcWinFormsClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Добавление столбцов в DataGridView
            /*dataGridView.Columns.Add("inventoryNumber", "Инвентарный номер");
            dataGridView.Columns.Add("arrivalTime", "Время прибытия");
            dataGridView.Columns.Add("departureTime", "Время отправления");*/
        }

        private async void btnFetchData_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Создаём канал и клиента
                var channel = new Channel("localhost:5000", ChannelCredentials.Insecure);
                var client = new WagonService.WagonServiceClient(channel);

                string dateStart = txtStartDate.Text;
                string dateEnd = txtEndDate.Text;

                var startDate = DateTime.Parse(dateStart).ToUniversalTime();
                var endDate = DateTime.Parse(dateEnd).ToUniversalTime();

                // Преобразуем DateTime в Timestamp
                var startTimestamp = Timestamp.FromDateTime(startDate);
                var endTimestamp = Timestamp.FromDateTime(endDate);

                // Создаём запрос
                var wagonRequest = new WagonRequest
                {
                    DateStart = startTimestamp,
                    DateEnd = endTimestamp
                };

                // Отправляем запрос
                var response = await client.GetWagonsByDateAsync(wagonRequest).ConfigureAwait(false);

                this.Invoke((MethodInvoker)delegate
                {
                    dataGridView.Rows.Clear(); // Очищаем таблицу перед добавлением новых данных

                    // Преобразуем данные в массив строк для DataGridView
                    var rows = response.Wagons.Select(wagon =>
                    {
                        // Преобразуем строки в DateTime
                        DateTime.TryParse(wagon.ArrivalTime, out var arrivalDateTime);
                        DateTime.TryParse(wagon.DepartureTime, out var departureDateTime);

                        // Если время невалидное, задаем минимальные значения
                        var arrivalTimeStr = arrivalDateTime != DateTime.MinValue
                            ? arrivalDateTime.ToString("yyyy-MM-dd HH:mm:ss")
                            : "Invalid Time";

                        var departureTimeStr = departureDateTime != DateTime.MinValue
                            ? departureDateTime.ToString("yyyy-MM-dd HH:mm:ss")
                            : "Invalid Time";

                        return new object[]
                        {
                            wagon.InventoryNumber,
                            arrivalTimeStr,
                            departureTimeStr
                        };
                    }).ToArray();

                    // Добавляем строки в DataGridView
                    dataGridView.Rows.AddRange(rows.Select(row => new DataGridViewRow 
                    {
                        Cells =
                        {
                            new DataGridViewTextBoxCell { Value = row[0] },
                            new DataGridViewTextBoxCell { Value = row[1] },
                            new DataGridViewTextBoxCell { Value = row[2] }
                        }
                    }).ToArray());
                });
            }
            catch (RpcException ex)
            {
                MessageBox.Show($"Ошибка: {ex.Status.Detail}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
