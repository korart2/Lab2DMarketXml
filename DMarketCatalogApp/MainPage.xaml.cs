using System.Text;
using DMarketCatalogEngine;

namespace DMarketCatalogApp;

public partial class MainPage : ContentPage
{
    private readonly CatalogController controller;
    private readonly SearchFilter filter;
    private bool isUpdatingUi;

    public MainPage()
    {
        InitializeComponent();
        controller = new CatalogController();
        filter = new SearchFilter();
    }

    public void openXmlBtnClick(string path)
    {
        try
        {
            Dictionary<string, List<string>> options = controller.loadXmlFile(path);
            filter.clear();
            showFilterOptions(options);

            List<CatalogItem> allItems = controller.search(filter);
            showResults(allItems);
            statusLabel.Text = $"Завантажено XML: {path} (усього елементів: {allItems.Count})";
        }
        catch (Exception ex)
        {
            showMessage($"Помилка читання XML-файлу:\n{ex.Message}");
        }
    }

    public void selectFilter(string attribute, string value)
    {
        filter.setCriterion(attribute, value);
    }

    public void searchBtnClick(StrategyType type)
    {
        if (string.IsNullOrWhiteSpace(controller.getXmlFilePath()))
        {
            showMessage("Спочатку відкрийте XML-файл бази даних.");
            return;
        }

        SyncFilterFromControls();
        controller.setStrategy(type);
        List<CatalogItem> items = controller.search(filter);
        showResults(items);

        statusLabel.Text = $"Пошук виконано методом {type}. Знайдено елементів: {items.Count}.";
    }

    public void transformBtnClick(string htmlPath)
    {
        if (string.IsNullOrWhiteSpace(controller.getXmlFilePath()))
        {
            showMessage("Спочатку відкрийте XML-файл для трансформації.");
            return;
        }

        try
        {
            if (controller.transformToHtml(htmlPath))
            {
                statusLabel.Text = $"HTML-звіт збережено: {htmlPath}";
                showMessage($"HTML-звіт успішно збережено:\n{htmlPath}");
            }
            else
            {
                showMessage("Не вдалося виконати трансформацію. Перевірте наявність файлів XML та XSL.");
            }
        }
        catch (Exception ex)
        {
            showMessage($"Помилка трансформації в HTML:\n{ex.Message}");
        }
    }

    public void clearBtnClick()
    {
        isUpdatingUi = true;
        try
        {
            keywordCheck.IsChecked = false;
            keywordEntry.Text = string.Empty;

            gameCheck.IsChecked = false;
            categoryCheck.IsChecked = false;
            rarityCheck.IsChecked = false;
            exteriorCheck.IsChecked = false;
            sellerCheck.IsChecked = false;

            gamePicker.SelectedIndex = -1;
            categoryPicker.SelectedIndex = -1;
            rarityPicker.SelectedIndex = -1;
            exteriorPicker.SelectedIndex = -1;
            sellerPicker.SelectedIndex = -1;
        }
        finally
        {
            isUpdatingUi = false;
        }

        filter.clear();
        showResults(new List<CatalogItem>());
        statusLabel.Text = "Критерії пошуку та поле виведення очищено.";
    }

    public async void helpBtnClick()
    {
        string helpText =
            "ДОВІДКА КОРИСТУВАЧА (Лабораторна робота №2)\n\n" +
            "Програма призначена для аналізу XML-каталогу торгових лотів ІС «DMarket» та його трансформації у формат HTML.\n\n" +
            "Як користуватися програмою:\n" +
            "1. Натисніть «Відкрити XML», щоб обрати файл бази даних (.xml). Значення атрибутів автоматично підвантажаться у списки фільтрів.\n" +
            "2. Оберіть метод пошуку (шаблон Стратегія): DOM, SAX або LINQ to XML.\n" +
            "3. Задайте ключове слово та/або оберіть значення атрибутів зі списків.\n" +
            "4. Натисніть «Пошук» для виведення знайдених предметів.\n" +
            "5. Натисніть «Трансформувати в HTML» для генерації HTML-звіту за допомогою XSLT.\n" +
            "6. Кнопка «Очистити (Clear)» скидає всі вибрані фільтри та очищує поле результатів.";

        await DisplayAlert("Довідка", helpText, "OK");
    }

    public void showFilterOptions(Dictionary<string, List<string>> options)
    {
        isUpdatingUi = true;
        try
        {
            keywordCheck.IsChecked = false;
            keywordEntry.Text = string.Empty;
            keywordCheck.IsEnabled = true;
            keywordEntry.IsEnabled = true;

            FillPicker(gamePicker, gameCheck, options, "Game");
            FillPicker(categoryPicker, categoryCheck, options, "Category");
            FillPicker(rarityPicker, rarityCheck, options, "Rarity");
            FillPicker(exteriorPicker, exteriorCheck, options, "Exterior");
            FillPicker(sellerPicker, sellerCheck, options, "Seller");
        }
        finally
        {
            isUpdatingUi = false;
        }
    }

    public void showResults(List<CatalogItem> items)
    {
        resultsHeaderLabel.Text = $"Результати пошуку (знайдено: {items.Count}):";

        if (items.Count == 0)
        {
            resultsEditor.Text = "За вказаними критеріями елементів не знайдено (або поле очищено).";
            return;
        }

        var sb = new StringBuilder();
        for (int i = 0; i < items.Count; i++)
        {
            sb.AppendLine($"{i + 1}. {items[i].getDisplayInfo()}");
            sb.AppendLine(new string('-', 55));
        }

        resultsEditor.Text = sb.ToString();
    }

    public async void showMessage(string message)
    {
        await DisplayAlert("Повідомлення", message, "OK");
    }

    public async void close()
    {
        bool confirm = await DisplayAlert(
            "Підтвердження виходу",
            "Чи дійсно ви хочете завершити роботу з програмою?",
            "Так",
            "Ні");

        if (confirm)
        {
            Application.Current?.Quit();
        }
    }

    private static void FillPicker(Picker picker, CheckBox checkBox, Dictionary<string, List<string>> options, string attributeName)
    {
        picker.SelectedIndex = -1;
        picker.ItemsSource = null;
        checkBox.IsChecked = false;

        if (options.TryGetValue(attributeName, out List<string>? values) && values.Count > 0)
        {
            picker.ItemsSource = values;
            picker.IsEnabled = true;
            checkBox.IsEnabled = true;
        }
        else
        {
            picker.IsEnabled = false;
            checkBox.IsEnabled = false;
        }
    }

    private void SyncFilterFromControls()
    {
        filter.clear();

        if (keywordCheck.IsChecked && !string.IsNullOrWhiteSpace(keywordEntry.Text))
        {
            selectFilter("Keyword", keywordEntry.Text);
        }

        ApplySingleControlToFilter(gameCheck, gamePicker, "Game");
        ApplySingleControlToFilter(categoryCheck, categoryPicker, "Category");
        ApplySingleControlToFilter(rarityCheck, rarityPicker, "Rarity");
        ApplySingleControlToFilter(exteriorCheck, exteriorPicker, "Exterior");
        ApplySingleControlToFilter(sellerCheck, sellerPicker, "Seller");
    }

    private void ApplySingleControlToFilter(CheckBox checkBox, Picker picker, string attributeName)
    {
        if (checkBox.IsChecked && picker.SelectedIndex >= 0 && picker.SelectedItem is string val)
        {
            selectFilter(attributeName, val);
        }
    }

    private StrategyType GetSelectedStrategyType()
    {
        if (domRadio.IsChecked) return StrategyType.DOM;
        if (saxRadio.IsChecked) return StrategyType.SAX;
        return StrategyType.LINQ;
    }

    private void OnKeywordTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (isUpdatingUi) return;
        keywordCheck.IsChecked = !string.IsNullOrWhiteSpace(e.NewTextValue);
        SyncFilterFromControls();
    }

    private void OnFilterControlChanged(object? sender, EventArgs e)
    {
        if (isUpdatingUi) return;

        if (sender == gamePicker && gamePicker.SelectedIndex >= 0) gameCheck.IsChecked = true;
        if (sender == categoryPicker && categoryPicker.SelectedIndex >= 0) categoryCheck.IsChecked = true;
        if (sender == rarityPicker && rarityPicker.SelectedIndex >= 0) rarityCheck.IsChecked = true;
        if (sender == exteriorPicker && exteriorPicker.SelectedIndex >= 0) exteriorCheck.IsChecked = true;
        if (sender == sellerPicker && sellerPicker.SelectedIndex >= 0) sellerCheck.IsChecked = true;

        SyncFilterFromControls();
    }

    private void OnSearchClicked(object? sender, EventArgs e) => searchBtnClick(GetSelectedStrategyType());
    private void OnClearClicked(object? sender, EventArgs e) => clearBtnClick();
    private void OnHelpClicked(object? sender, EventArgs e) => helpBtnClick();
    private void OnExitClicked(object? sender, EventArgs e) => close();

    private async void OnAboutClicked(object? sender, EventArgs e)
    {
        string aboutText =
            "Лабораторна робота №2\n" +
            "«Обробка XML-документів» (ІС Маркетплейс DMarket)\n\n" +
            "Виконав: Корпачов Артем Андрійович, група К-27\n" +
            "ФКНК КНУ імені Тараса Шевченка\n\n" +
            "Реалізовано:\n" +
            "• Патерн проєктування Strategy (ISearchStrategy)\n" +
            "• Пошук через DOM (XPath), SAX (XmlReader) та LINQ to XML (XDocument)\n" +
            "• Динамічне завантаження атрибутів із вибраного XML-файлу\n" +
            "• XSLT-трансформація у формат HTML.";

        await DisplayAlert("Про програму", aboutText, "OK");
    }

    private async void OnOpenXmlClicked(object? sender, EventArgs e)
    {
        var xmlFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, new[] { ".xml" } }
        });

        PickOptions options = new()
        {
            PickerTitle = "Виберіть XML-файл каталогу DMarket",
            FileTypes = xmlFileType
        };

        FileResult? result = await FilePicker.Default.PickAsync(options);
        if (result != null)
        {
            openXmlBtnClick(result.FullPath);
        }
    }

    private async void OnTransformClicked(object? sender, EventArgs e)
    {
#if WINDOWS
        var savePicker = new Windows.Storage.Pickers.FileSavePicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "dmarket_report"
        };
        savePicker.FileTypeChoices.Add("HTML сторінка", new List<string> { ".html" });

        if (Application.Current?.Windows.FirstOrDefault()?.Handler?.PlatformView is MauiWinUIWindow window)
        {
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, window.WindowHandle);
        }

        Windows.Storage.StorageFile? file = await savePicker.PickSaveFileAsync();
        if (file != null)
        {
            transformBtnClick(file.Path);
        }
#else
        string defaultHtml = Path.Combine(FileSystem.Current.AppDataDirectory, "dmarket_report.html");
        transformBtnClick(defaultHtml);
#endif
    }
}