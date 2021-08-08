using AppQueueClient.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppQueueClient
{
    public partial class FrmMain : Form
    {
        private readonly string _baseUri = "https://localhost:44329/api/";
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnVendas_Click(object sender, EventArgs e)
        {
            var sales = SalesFake.ListSalesFake(10000);

            foreach (var item in sales)
            {
                dgvMain.Rows.Add(
                    item.Cardnumber,
                    item.Cvv,
                    item.Name,
                    item.Amount,
                    item.Transaction,
                    false);

                ConsumeApi(item, dgvMain.Rows.Count - 1);
                dgvMain.Refresh();
            }
        }

        private void ConsumeApi(Sales sales, int index)
        {
            var salesReq = JsonConvert.SerializeObject(sales, Formatting.Indented);
            HttpContent content = new StringContent(salesReq, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();

            Task taskUpload = httpClient.PostAsync(_baseUri + "payments", content).ContinueWith(task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    var response = task.Result;

                    if (response.IsSuccessStatusCode)
                        dgvMain["PROCESSED", index].Value = true;
                }
            });
        }
    }
}
