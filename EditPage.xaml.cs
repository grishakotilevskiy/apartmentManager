namespace labka3
{
    public partial class EditPage : ContentPage
    {
        private Apartment _apartment;
        public event Action<Apartment> OnSaved;

        public EditPage(Apartment apartment = null)
        {
            InitializeComponent();
            if (apartment != null)
            {
                _apartment = apartment;
                EntryCity.Text = apartment.City;
                EntryStreet.Text = apartment.Street;
                EntryRooms.Text = apartment.Rooms.ToString();
                EntryFloor.Text = apartment.Floor.ToString();
                EntryArea.Text = apartment.Area.ToString();
                EntryPrice.Text = apartment.Price.ToString();
                CheckRenovated.IsChecked = apartment.IsRenovated;
            }
            else
            {
                _apartment = new Apartment();
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // ВАЛІДАЦІЯ (Захист від дурня)
            if (string.IsNullOrWhiteSpace(EntryCity.Text) || string.IsNullOrWhiteSpace(EntryStreet.Text))
            {
                await DisplayAlert("Помилка", "Заповніть Місто та Вулицю.", "OK");
                return;
            }

            if (!int.TryParse(EntryRooms.Text, out int rooms) || rooms <= 0)
            {
                await DisplayAlert("Помилка", "Кількість кімнат: введіть ціле число > 0.", "OK");
                return;
            }

            if (!int.TryParse(EntryFloor.Text, out int floor))
            {
                await DisplayAlert("Помилка", "Поверх: введіть коректне число.", "OK");
                return;
            }

            if (!double.TryParse(EntryArea.Text, out double area) || area <= 0)
            {
                await DisplayAlert("Помилка", "Площа: введіть число > 0.", "OK");
                return;
            }

            if (!decimal.TryParse(EntryPrice.Text, out decimal price) || price < 0)
            {
                await DisplayAlert("Помилка", "Ціна: введіть додатне число.", "OK");
                return;
            }

            _apartment.City = EntryCity.Text.Trim();
            _apartment.Street = EntryStreet.Text.Trim();
            _apartment.Rooms = rooms;
            _apartment.Floor = floor;
            _apartment.Area = area;
            _apartment.Price = price;
            _apartment.IsRenovated = CheckRenovated.IsChecked;

            OnSaved?.Invoke(_apartment);
            await Navigation.PopModalAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}