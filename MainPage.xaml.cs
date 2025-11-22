using System.Collections.ObjectModel;
using System.Text.Json;

namespace labka3
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Apartment> _apartments = new ObservableCollection<Apartment>();
        private List<Apartment> _allApartments = new List<Apartment>();

        public MainPage()
        {
            InitializeComponent();
            ApartmentsList.ItemsSource = _apartments;
        }

        private async void OnExitClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Вихід", "Чи точно ви хочете вийти з програми?", "Так", "Ні");
            if (answer)
            {
                Application.Current.Quit();
            }
        }

        private async void OnLoadClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Виберіть файл даних"
                });

                if (result != null)
                {
                    if (!result.FileName.EndsWith(".json") && !result.FileName.EndsWith(".txt"))
                    {
                        await DisplayAlert("Увага", "Файл може мати некоректний формат.", "OK");
                    }

                    string json = await File.ReadAllTextAsync(result.FullPath);

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        await DisplayAlert("Помилка", "Файл порожній.", "OK");
                        return;
                    }

                    var data = JsonSerializer.Deserialize<List<Apartment>>(json);

                    if (data != null)
                    {
                        _allApartments = data;
                        RefreshList();
                        StatusLabel.Text = $"Завантажено {data.Count} об'єктів.";
                    }
                    else
                    {
                        await DisplayAlert("Помилка", "Не вдалося прочитати дані.", "OK");
                    }
                }
            }
            catch (JsonException)
            {
                await DisplayAlert("Помилка читання", "Структура JSON пошкоджена.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Критична помилка", ex.Message, "OK");
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                string json = JsonSerializer.Serialize(_allApartments, new JsonSerializerOptions { WriteIndented = true });
                string fileName = Path.Combine(FileSystem.Current.AppDataDirectory, "apartments.json");
                await File.WriteAllTextAsync(fileName, json);

                await DisplayAlert("Успіх", $"Файл збережено:\n{fileName}", "OK");
                StatusLabel.Text = "Дані збережено успішно";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка збереження", ex.Message, "OK");
            }
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            var editPage = new EditPage();
            editPage.OnSaved += (newApartment) =>
            {
                _allApartments.Add(newApartment);
                RefreshList();
            };
            await Navigation.PushModalAsync(editPage);
        }

        private async void OnEditItemClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Apartment apartment)
            {
                var editPage = new EditPage(apartment);
                editPage.OnSaved += (updatedApartment) =>
                {
                    RefreshList();
                };
                await Navigation.PushModalAsync(editPage);
            }
        }

        private async void OnDeleteItemClicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button button && button.CommandParameter is Apartment apartment)
                {
                    bool confirm = await DisplayAlert("Видалення", "Видалити запис?", "Так", "Ні");
                    if (confirm)
                    {
                        _allApartments.Remove(apartment);
                        RefreshList();
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        private void OnSearchClicked(object sender, EventArgs e)
        {
            try
            {
                var filtered = _allApartments.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(SearchCity.Text))
                {
                    filtered = filtered.Where(x => x.City != null && x.City.Contains(SearchCity.Text, StringComparison.OrdinalIgnoreCase));
                }

                if (decimal.TryParse(SearchMinPrice.Text, out decimal minPrice))
                {
                    filtered = filtered.Where(x => x.Price >= minPrice);
                }

                if (int.TryParse(SearchMinRooms.Text, out int minRooms))
                {
                    filtered = filtered.Where(x => x.Rooms >= minRooms);
                }

                _apartments.Clear();
                foreach (var item in filtered)
                {
                    _apartments.Add(item);
                }
                StatusLabel.Text = $"Знайдено: {_apartments.Count}";
            }
            catch
            {
                StatusLabel.Text = "Помилка під час пошуку";
            }
        }

        private void OnResetFilterClicked(object sender, EventArgs e)
        {
            SearchCity.Text = string.Empty;
            SearchMinPrice.Text = string.Empty;
            SearchMinRooms.Text = string.Empty;
            RefreshList();
            StatusLabel.Text = "Фільтр скинуто";
        }

        private async void OnAboutClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AboutPage());
        }

        private void RefreshList()
        {
            _apartments.Clear();
            foreach (var item in _allApartments)
            {
                _apartments.Add(item);
            }
        }
    }
}