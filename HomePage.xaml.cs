using Microsoft.Maui.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;

namespace AppFinal
{
    public partial class HomePage : ContentPage
    {
        public ObservableCollection<TransactionModel> Transactions { get; set; }
        private int userId;

        public HomePage(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            Transactions = new ObservableCollection<TransactionModel>();
            BindingContext = this;
            LoadTransactions(userId);
            LoadLatestBalance(userId);
        }

        private async void LoadTransactions(int userId)
        {
            string connectionString = "Server=localhost;Database=projet1;User Id=root;Password=;";
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT amount, final_balance, transaction_date FROM transactions WHERE user_id = @userId ORDER BY transaction_date";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                DateTime transactionDateTime = reader.GetDateTime("transaction_date");
                                Transactions.Add(new TransactionModel
                                {
                                    Amount = reader.GetDecimal("amount"),
                                    FinalBalance = reader.GetDecimal("final_balance"),
                                    TransactionDateTime = transactionDateTime
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
        }

        private async void LoadLatestBalance(int userId)
        {
            string connectionString = "Server=localhost;Database=projet1;User Id=root;Password=;";
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT final_balance FROM transactions WHERE user_id = @userId ORDER BY transaction_date DESC LIMIT 1";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        var result = await command.ExecuteScalarAsync();
                        if (result != null)
                        {
                            decimal latestBalance = Convert.ToDecimal(result);
                            BalanceLabel.Text = $"{latestBalance:C}";
                        }
                        else
                        {
                            BalanceLabel.Text = "N/A";
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erreur", $"Erreur lors du chargement du solde restant : {ex.Message}", "OK");
                }
            }
        }

        private async void OnProfileImageTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Profil(userId));
        }
    }
}
