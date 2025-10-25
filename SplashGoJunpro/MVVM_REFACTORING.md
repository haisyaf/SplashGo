# Refactoring ke MVVM Pattern - SplashGo

## ?? Ringkasan Perubahan

Proyek telah berhasil di-refactor dari **Code-Behind pattern** ke **MVVM (Model-View-ViewModel) pattern** yang proper.

---

## ??? Struktur Proyek (Sebelum vs Sesudah)

### ? **SEBELUM** (Tidak MVVM)
```
SplashGoJunpro/
??? Models/          ? Ada
?   ??? User.cs
?   ??? Admin.cs
?   ??? ...
??? Views/      ? Ada
?   ??? LoginWindow.xaml
?   ??? LoginWindow.xaml.cs (? PENUH LOGIC BISNIS)
??? ViewModels/   ? TIDAK ADA
```

### ? **SESUDAH** (MVVM Proper)
```
SplashGoJunpro/
??? Models/     ? (Business entities)
?   ??? User.cs
?   ??? Admin.cs
?   ??? Tourist.cs
?   ??? Booking.cs
?   ??? Destination.cs
?
??? Views/               ? (UI only)
?   ??? LoginWindow.xaml (Data Binding)
?   ??? LoginWindow.xaml.cs (UI logic only)
?
??? ViewModels/          ? BARU! (Business logic)
?   ??? ViewModelBase.cs
?   ??? LoginViewModel.cs
?
??? Commands/      ? BARU! (ICommand implementation)
?   ??? RelayCommand.cs
?
??? Helpers/             ? BARU! (Attached properties)
?   ??? PasswordBoxHelper.cs
?
??? Converters/          ? BARU! (Value converters)
    ??? BooleanToVisibilityConverter.cs
```

---

## ?? File Baru yang Ditambahkan

### 1. **ViewModels/ViewModelBase.cs**
Base class untuk semua ViewModel yang mengimplementasi `INotifyPropertyChanged`.

**Fitur:**
- ? `OnPropertyChanged()` - Notify UI saat property berubah
- ? `SetProperty<T>()` - Helper method untuk set property dengan auto-notification

### 2. **ViewModels/LoginViewModel.cs**
ViewModel untuk LoginWindow yang mengelola semua business logic.

**Properties:**
- `Email` - Binding untuk input email
- `Password` - Binding untuk input password
- `RememberMe` - Binding untuk checkbox
- `IsEmailFocused` - State focus email
- `IsPasswordFocused` - State focus password

**Commands:**
- `SignInCommand` - Command untuk login
- `ForgotPasswordCommand` - Command untuk forgot password
- `GoogleSignInCommand` - Command untuk Google sign-in
- `SignUpCommand` - Command untuk navigasi ke sign up
- `CloseCommand` - Command untuk close window

### 3. **Commands/RelayCommand.cs**
Implementasi `ICommand` untuk MVVM pattern.

**Fitur:**
- ? Support Execute action
- ? Support CanExecute validation
- ? Auto refresh dengan `CommandManager`

### 4. **Helpers/PasswordBoxHelper.cs**
Attached property helper untuk binding PasswordBox (workaround karena PasswordBox tidak support direct binding).

**Usage:**
```xaml
<PasswordBox helpers:PasswordBoxHelper.Attach="True"
        helpers:PasswordBoxHelper.Password="{Binding Password, Mode=TwoWay}"/>
```

### 5. **Converters/BooleanToVisibilityConverter.cs**
Value converters untuk konversi data binding.

**Converters:**
- `InverseBooleanToVisibilityConverter` - Bool ke Visibility (inverted)
- `StringEmptyToVisibilityConverter` - String kosong ke Visibility

---

## ?? Perubahan pada File Existing

### **LoginWindow.xaml.cs**

#### ? **SEBELUM** (258 baris dengan banyak business logic)
```csharp
public partial class LoginWindow : Window
{
    public LoginWindow() { InitializeComponent(); }
    
    private void SignIn_Click(object sender, RoutedEventArgs e)
    {
        string email = EmailTextBox.Text.Trim();
        string password = PasswordBox.Password;
        
     // Validasi...
        if (string.IsNullOrEmpty(email)) { ... }
      if (!IsValidEmail(email)) { ... }
        
     // Logic autentikasi...
        if (email == "admin@splashgo.com" && password == "admin123") { ... }
    }
    
    private void EmailTextBox_GotFocus(...) { ... }
    private void EmailTextBox_LostFocus(...) { ... }
    private void PasswordBox_PasswordChanged(...) { ... }
    // ... 15+ event handlers lainnya
}
```

#### ? **SESUDAH** (20 baris, hanya UI logic)
```csharp
public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
DataContext = new LoginViewModel(); // ? Set ViewModel
    }

 // ? Hanya UI-specific logic (drag window)
    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
       this.DragMove();
    }
}
```

### **LoginWindow.xaml**

#### ? **SEBELUM** (Event handlers)
```xaml
<TextBox x:Name="EmailTextBox"
         GotFocus="EmailTextBox_GotFocus"
  LostFocus="EmailTextBox_LostFocus"/>

<Button Content="Sign in" Click="SignIn_Click"/>
```

#### ? **SESUDAH** (Data Binding & Commands)
```xaml
<!-- ? Namespace untuk helpers dan converters -->
xmlns:helpers="clr-namespace:SplashGoJunpro.Helpers"
xmlns:converters="clr-namespace:SplashGoJunpro.Converters"

<!-- ? Data Binding -->
<TextBox Text="{Binding Email, UpdateSourceTrigger=PropertyChanged}"/>

<PasswordBox helpers:PasswordBoxHelper.Attach="True"
           helpers:PasswordBoxHelper.Password="{Binding Password, Mode=TwoWay}"/>

<!-- ? Command Binding -->
<Button Content="Sign in" Command="{Binding SignInCommand}"/>

<!-- ? Converter untuk visibility -->
<TextBlock Visibility="{Binding Email, Converter={StaticResource StringEmptyToVisibilityConverter}}"/>
```

---

## ? Keuntungan MVVM Pattern

### 1. **Separation of Concerns**
- ? View hanya handle UI
- ? ViewModel handle business logic
- ? Model handle data entities

### 2. **Testability**
- ? ViewModel bisa di-unit test tanpa UI
- ? Tidak perlu instance Window untuk test logic

### 3. **Maintainability**
- ? Code lebih terorganisir
- ? Mudah di-maintain dan di-extend
- ? Logic bisnis terpisah dari UI

### 4. **Reusability**
- ? ViewModel bisa digunakan di multiple Views
- ? Commands bisa di-reuse
- ? Converters bisa di-share

### 5. **Data Binding**
- ? Two-way binding otomatis
- ? UI update otomatis saat data berubah
- ? Tidak perlu manual update UI elements

---

## ?? Best Practices yang Diimplementasi

### ? **INotifyPropertyChanged**
Semua properties di ViewModel implement notification untuk auto-update UI.

### ? **ICommand Pattern**
Semua user actions menggunakan Commands bukan event handlers.

### ? **Attached Properties**
Workaround untuk controls yang tidak support binding (PasswordBox).

### ? **Value Converters**
Convert data types untuk binding (bool to visibility, etc).

### ? **UpdateSourceTrigger**
Set `PropertyChanged` untuk instant update.

### ? **CanExecute Validation**
SignInCommand disable otomatis jika email/password kosong.

---

## ?? Cara Penggunaan

### **Membuat ViewModel Baru**
```csharp
public class MyViewModel : ViewModelBase
{
    private string _name;
    
  public string Name
  {
        get => _name;
        set => SetProperty(ref _name, value);
    }
 
    public ICommand SaveCommand { get; }
    
    public MyViewModel()
    {
        SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
    }
    
    private bool CanExecuteSave(object param) => !string.IsNullOrEmpty(Name);
    
    private void ExecuteSave(object param)
    {
        // Logic save
    }
}
```

### **Binding di XAML**
```xaml
<!-- Set DataContext di code-behind atau XAML -->
<Window DataContext="{StaticResource MyViewModel}">
  
    <!-- Two-way binding -->
    <TextBox Text="{Binding Name, UpdateSourceTrigger=PropertyChanged}"/>
    
  <!-- Command binding -->
    <Button Content="Save" Command="{Binding SaveCommand}"/>
    
</Window>
```

---

## ?? TODO untuk Development Selanjutnya

### 1. **Service Layer**
```csharp
// Buat authentication service
public interface IAuthenticationService
{
    Task<User> LoginAsync(string email, string password);
    Task LogoutAsync();
}
```

### 2. **Navigation Service**
```csharp
// Service untuk navigasi antar windows
public interface INavigationService
{
    void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
    void GoBack();
}
```

### 3. **Dependency Injection**
```csharp
// Gunakan DI container (SimpleInjector, Autofac, dll)
public LoginViewModel(IAuthenticationService authService, 
              INavigationService navService)
{
    _authService = authService;
    _navService = navService;
}
```

### 4. **Validation**
```csharp
// Implement IDataErrorInfo atau INotifyDataErrorInfo
public class LoginViewModel : ViewModelBase, IDataErrorInfo
{
    public string this[string propertyName]
    {
        get
        {
 if (propertyName == nameof(Email))
            {
     if (string.IsNullOrEmpty(Email))
     return "Email is required";
if (!IsValidEmail(Email))
      return "Invalid email format";
          }
     return null;
    }
    }
}
```

### 5. **Async Commands**
```csharp
// Buat AsyncRelayCommand untuk async operations
public class AsyncRelayCommand : ICommand
{
    private readonly Func<object, Task> _executeAsync;
    // ...
}
```

---

## ?? Referensi

- [Microsoft MVVM Pattern](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/data-binding-overview)
- [INotifyPropertyChanged](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged)
- [ICommand Interface](https://learn.microsoft.com/en-us/dotnet/api/system.windows.input.icommand)
- [WPF Data Binding](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/)

---

## ? Checklist MVVM Implementation

- [x] ViewModelBase dengan INotifyPropertyChanged
- [x] RelayCommand implementation
- [x] LoginViewModel dengan properties & commands
- [x] Data Binding di XAML
- [x] PasswordBoxHelper untuk PasswordBox binding
- [x] Value Converters
- [x] Remove business logic dari code-behind
- [x] Command binding untuk semua user actions
- [ ] Service layer (untuk future development)
- [ ] Navigation service
- [ ] Dependency Injection
- [ ] Validation framework
- [ ] Async commands

---

**Status:** ? **MVVM Pattern Successfully Implemented!**

Proyek sekarang mengikuti MVVM pattern yang proper dan siap untuk development lebih lanjut.
