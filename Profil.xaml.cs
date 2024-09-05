using Microsoft.Maui.Controls;
using System;
using System.Data.SqlClient; // Assure-toi d'avoir la bonne référence pour ton SGBD

namespace AppFinal
{
    public partial class Profil : ContentPage
    {
        private string connectionString = "your_connection_string_here"; // Remplace par ta chaîne de connexion

        public Profil()
        {
            InitializeComponent();
        }

        private async void OnUpdatePasswordClicked(object sender, EventArgs e)
        {
            if (OldPasswordEntry.Text == null || NewPasswordEntry.Text == null || ConfirmPasswordEntry.Text == null)
            {
                UpdateMessage.Text = "Tous les champs doivent être remplis.";
                UpdateMessage.IsVisible = true;
                return;
            }

            if (NewPasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                UpdateMessage.Text = "Les mots de passe ne correspondent pas.";
                UpdateMessage.IsVisible = true;
                return;
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                UpdateMessage.Text = "Chaîne de connexion non définie.";
                UpdateMessage.IsVisible = true;
                return;
            }

            bool result = await UpdatePasswordAsync(OldPasswordEntry.Text, NewPasswordEntry.Text);

            if (result)
            {
                UpdateMessage.Text = "Mot de passe mis à jour avec succès.";
                UpdateMessage.TextColor = Color.Green;
            }
            else
            {
                UpdateMessage.Text = "Échec de la mise à jour du mot de passe.";
                UpdateMessage.TextColor = Color.Red;
            }

            UpdateMessage.IsVisible = true;
        }

        private async Task<bool> UpdatePasswordAsync(string oldPassword, string newPassword)
        {
            // Exemple de mise à jour du mot de passe dans la base de données
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand("UPDATE Users SET Password = @NewPassword WHERE Password = @OldPassword", conn);
                    cmd.Parameters.AddWithValue("@NewPassword", newPassword);
                    cmd.Parameters.AddWithValue("@OldPassword", oldPassword);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Gestion des erreurs
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
