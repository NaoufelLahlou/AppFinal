using Microsoft.Maui.Controls;

namespace AppFinal
{
    public class TransactionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SomeViewCellTemplate { get; set; }
        public DataTemplate AnotherViewCellTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var transaction = item as TransactionModel;
            if (transaction.Amount > 0) // Exemple de condition pour sélectionner le modèle
            {
                return SomeViewCellTemplate;
            }
            else
            {
                return AnotherViewCellTemplate;
            }
        }
    }
}
