using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace AppFinal
{
    public partial class Profil : ContentPage
    {
        private int userId;

        public Profil(int userId)
        {
            InitializeComponent();
            this.userId = userId;
        }

        private async void OnChangePasswordClicked(object sender, EventArgs e)
        {
            var oldPassword = OldPasswordEntry.Text;
            var newPassword = NewPasswordEntry.Text;
            var confirmPassword = ConfirmPasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                await DisplayAlert("Erreur", "Veuillez remplir tous les champs.", "OK");
                return;
            }

            if (newPassword != confirmPassword)
            {
                await DisplayAlert("Erreur", "Les nouveaux mots de passe ne correspondent pas.", "OK");
                return;
            }

            var isUpdated = await UpdatePasswordAsync(oldPassword, newPassword);
            if (isUpdated)
            {
                await DisplayAlert("Succès", "Mot de passe changé avec succès.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Erreur", "L'ancien mot de passe est incorrect.", "OK");
            }
        }

        private async Task<bool> UpdatePasswordAsync(string oldPassword, string newPassword)
        {
            string connectionString = "Server=localhost;Database=projet1;User Id=root;Password=;";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "UPDATE users SET password = @newPassword WHERE id = @userId AND password = @oldPassword";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@newPassword", newPassword);
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@oldPassword", oldPassword);
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", ex.Message, "OK");
                return false;
            }
        }
    }
}
