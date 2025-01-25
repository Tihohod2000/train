using System;
using System.Windows.Forms;
using Grpc.Net.Client;
using GrpcStationService;
using Google.Protobuf.WellKnownTypes; // Для работы с Timestamp
using System.Threading.Tasks;
using Grpc.Core;
using System.Runtime.InteropServices.ComTypes;

namespace GrpcWinFormsClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnFetchData_Click_1(object sender, EventArgs e)
        {
            // Получаем введенные данные
            

            /* // Проверяем корректность дат
             if (!DateTime.TryParse(dateStart, out DateTime startDate) ||
                 !DateTime.TryParse(dateEnd, out DateTime endDate))
             {
                 MessageBox.Show("Пожалуйста, введите корректные даты в формате YYYY-MM-DD.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 return;
             }*/

            // Преобразуем DateTime в Timestamp
            try
            {
                // Создаём канал и клиента
                using var channel = GrpcChannel.ForAddress("http://localhost:5033");
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

                Console.WriteLine("Ответ: " + response);

                // Обрабатываем ответ
                txtResponse.Clear();
                foreach (var wagon in response.Wagons)
                {
                    txtResponse.AppendText($"Инвентарный номер: {wagon.InventoryNumber}\n");
                    txtResponse.AppendText($"Время прибытия: {wagon.ArrivalTime}\n");
                    txtResponse.AppendText($"Время отправления: {wagon.DepartureTime}\n");
                    txtResponse.AppendText("\n");
                }
            }
            catch (RpcException ex)
            {
                MessageBox.Show($"Ошибка: {ex.Status.Detail}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
