using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MySql.Data.MySqlClient;
using System.Data;

namespace AppFinal
{
    public partial class HomePageAdmin : ContentPage
    {
        public HomePageAdmin()
        {
            InitializeComponent();
            LoadUsers();
        }

        private async void LoadUsers()
        {
            string connectionString = "Server=localhost;Database=projet1;User Id=root;Password=;";
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT username FROM users";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                UserPicker.Items.Add(reader.GetString("username"));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erreur", $"Erreur lors du chargement des utilisateurs : {ex.Message}", "OK");
                }
            }
        }

        private async void OnGeneratePdfClicked(object sender, EventArgs e)
        {
            var selectedUser = UserPicker.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedUser))
            {
                ErrorMessage.Text = "Veuillez sélectionner un utilisateur.";
                ErrorMessage.IsVisible = true;
                return;
            }

            var startDate = StartDatePicker.Date;
            var endDate = EndDatePicker.Date;

            if (startDate > endDate)
            {
                ErrorMessage.Text = "La date de début ne peut pas être après la date de fin.";
                ErrorMessage.IsVisible = true;
                return;
            }

            var transactions = await GetTransactions(selectedUser, startDate, endDate);
            if (transactions.Count == 0)
            {
                ErrorMessage.Text = "Aucune transaction trouvée pour la période sélectionnée.";
                ErrorMessage.IsVisible = true;
                return;
            }

            GeneratePdf(transactions);
            await DisplayAlert("Succès", "Le PDF a été généré avec succès.", "OK");
        }

        private async Task<List<Transaction>> GetTransactions(string user, DateTime startDate, DateTime endDate)
        {
            var transactions = new List<Transaction>();
            string connectionString = "Server=localhost;Database=projet1;User Id=root;Password=;";
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT id, amount FROM transactions WHERE username = @user AND transaction_date BETWEEN @startDate AND @endDate";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user", user);
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", endDate);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                transactions.Add(new Transaction
                                {
                                    Id = reader.GetInt32("id"),
                                    Amount = reader.GetDecimal("amount")
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erreur", $"Erreur lors du chargement des transactions : {ex.Message}", "OK");
                }
            }
            return transactions;
        }

        private void GeneratePdf(List<Transaction> transactions)
        {
            var doc = new Document();
            var pdfPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Transactions.pdf");
            using (var stream = new FileStream(pdfPath, FileMode.Create))
            {
                PdfWriter.GetInstance(doc, stream);
                doc.Open();
                doc.Add(new Paragraph("Transactions"));
                foreach (var transaction in transactions)
                {
                    doc.Add(new Paragraph($"ID : {transaction.Id}, Montant : {transaction.Amount}"));
                }
                doc.Close();
            }
        }
    }

    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
    }
}
