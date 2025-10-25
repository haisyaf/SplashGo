# ?? Troubleshooting Guide - Login Window Issues

## ? Masalah yang Sudah Diperbaiki

### 1. **TextBox Tidak Bisa Mengetik**
**Penyebab:** Konflik BorderBrush antara Style dan inline property
**Solusi:**
- ? Menghapus `BorderBrush="#037ADE"` dari TextBox inline property
- ? Set `BorderBrush="Transparent"` dan `BorderThickness="0"` di Style
- ? Border styling dipindahkan ke parent Border element

### 2. **Tombol Sign In Selalu Disabled**
**Penyebab:** `CanExecuteSignIn` membutuhkan Email dan Password tidak kosong
**Solusi:**
- ? Pastikan binding `UpdateSourceTrigger=PropertyChanged` ada
- ? `RaiseCanExecuteChanged()` dipanggil saat Email/Password berubah
- ? Tambahkan trigger `IsEnabled` di button style untuk visual feedback

### 3. **Placeholder Tidak Hilang**
**Penyebab:** Converter binding tidak update dengan benar
**Solusi:**
- ? Gunakan `StringEmptyToVisibilityConverter`
- ? Binding dengan mode default (OneWay untuk Visibility)

---

## ?? Cara Debug Aplikasi

### **1. Buka Debug Output Window**
Di Visual Studio:
1. Tekan `Ctrl + Alt + O` atau
2. Menu: **View** ? **Output**
3. Pilih dropdown: **Debug**

### **2. Debug Messages yang Akan Muncul**

Saat aplikasi berjalan, Anda akan melihat:

```
LoginViewModel initialized
All commands initialized
```

Saat mengetik di Email TextBox:
```
Email changed: a
Email changed: ad
Email changed: admin@splashgo.com
```

Saat mengetik di Password:
```
Password changed: has value
Password changed: has value
```

Saat klik Sign In button:
```
ExecuteSignIn called
```

### **3. Periksa Binding Errors**
Jika ada binding error, akan muncul di Output window seperti:
```
System.Windows.Data Error: 40 : BindingExpression path error...
```

---

## ?? Cara Testing

### **Test 1: TextBox Input**
1. ? Klik di Email TextBox
2. ? Ketik beberapa karakter
3. ? Pastikan text muncul
4. ? Placeholder harus hilang saat ada text

**Expected Result:**
- Debug Output: `Email changed: [your input]`
- Placeholder text hilang
- Text berwarna #333 (hitam)

---

### **Test 2: PasswordBox Input**
1. ? Klik di PasswordBox
2. ? Ketik beberapa karakter
3. ? Placeholder harus hilang

**Expected Result:**
- Debug Output: `Password changed: has value`
- Placeholder hilang
- Bullet points (••••) muncul

---

### **Test 3: Sign In Button State**
**Scenario A: Fields Kosong**
- Email: *(kosong)*
- Password: *(kosong)*
- **Expected:** Button abu-abu/disabled (opacity 0.6)

**Scenario B: Hanya Email**
- Email: `admin@splashgo.com`
- Password: *(kosong)*
- **Expected:** Button tetap disabled

**Scenario C: Kedua Field Terisi**
- Email: `admin@splashgo.com`
- Password: `admin123`
- **Expected:** Button enabled (gradient hijau-biru)

---

### **Test 4: Sign In Functionality**
1. ? Isi Email: `admin@splashgo.com`
2. ? Isi Password: `admin123`
3. ? Klik Sign In button

**Expected Result:**
- Debug Output: `ExecuteSignIn called`
- MessageBox: "Login successful!"

**Test Invalid Credentials:**
- Email: `test@test.com`
- Password: `wrong`
- **Expected:** MessageBox: "Invalid email or password."

---

## ?? Common Issues & Solutions

### **Issue 1: Button Masih Disabled Padahal Fields Terisi**

**Diagnosis:**
```
1. Periksa Debug Output
2. Apakah "Email changed:" muncul saat mengetik?
3. Apakah "Password changed:" muncul saat mengetik?
```

**Solusi:**
- Jika TIDAK muncul ? Binding error
- Check XAML binding syntax
- Pastikan DataContext = LoginViewModel

---

### **Issue 2: TextBox Tidak Bisa Diklik/Focus**

**Diagnosis:**
- Apakah TextBox tertutup element lain?
- Apakah IsHitTestVisible=False di suatu element?

**Solusi:**
```xaml
<!-- Pastikan placeholder tidak block input -->
<TextBlock IsHitTestVisible="False" .../>
```

---

### **Issue 3: Placeholder Tidak Hilang**

**Diagnosis:**
- Check converter di Resources
- Check binding syntax

**Solusi:**
```xaml
<!-- Harus ada converter di Resources -->
<Window.Resources>
    <converters:StringEmptyToVisibilityConverter x:Key="StringEmptyToVisibilityConverter"/>
</Window.Resources>

<!-- Binding harus reference converter -->
Visibility="{Binding Email, Converter={StaticResource StringEmptyToVisibilityConverter}}"
```

---

### **Issue 4: Command Tidak Jalan Saat Klik Button**

**Diagnosis:**
1. Periksa Debug Output saat klik button
2. Apakah "ExecuteSignIn called" muncul?

**Jika YA:** Command jalan, cek logic di dalam ExecuteSignIn

**Jika TIDAK:**
- Periksa binding: `Command="{Binding SignInCommand}"`
- Periksa DataContext sudah di-set
- Periksa command di-initialize di constructor

---

## ?? Checklist Troubleshooting

Jika ada masalah, cek satu per satu:

### ? **ViewModel Setup**
- [ ] LoginViewModel di-initialize di LoginWindow constructor
- [ ] DataContext di-set ke LoginViewModel instance
- [ ] Semua commands di-initialize di constructor
- [ ] Properties implement SetProperty dengan notification

### ? **XAML Binding**
- [ ] Namespace helpers dan converters sudah ditambahkan
- [ ] Converter ada di Window.Resources
- [ ] TextBox: `Text="{Binding Email, UpdateSourceTrigger=PropertyChanged}"`
- [ ] PasswordBox: `helpers:PasswordBoxHelper.Attach="True"`
- [ ] Button: `Command="{Binding SignInCommand}"`

### ? **Styling**
- [ ] TextBox Style tidak ada konflik BorderBrush
- [ ] Background="Transparent" di Style
- [ ] Parent Border handle border styling
- [ ] Button Style ada trigger IsEnabled

### ? **Converter**
- [ ] StringEmptyToVisibilityConverter exist di Converters folder
- [ ] Converter registered di Resources dengan x:Key
- [ ] Binding reference StaticResource dengan key yang benar

---

## ?? Quick Fix Commands

### **Reset DataContext (di LoginWindow.xaml.cs)**
```csharp
public LoginWindow()
{
    InitializeComponent();
    DataContext = new LoginViewModel(); // ? Set ViewModel
}
```

### **Force Refresh Button State (di ViewModel)**
```csharp
public string Email
{
 get => _email;
    set
    {
  if (SetProperty(ref _email, value))
        {
            (SignInCommand as RelayCommand)?.RaiseCanExecuteChanged(); // ? Force update
 }
    }
}
```

### **Verify Binding at Runtime (Add to ViewModel)**
```csharp
public LoginViewModel()
{
    // ... existing code ...
    
 // Test binding
    Email = "test@test.com"; // Should trigger "Email changed" in debug
    Password = "test123";   // Should trigger "Password changed"
    
    // Clear after test
    Email = "";
    Password = "";
}
```

---

## ?? Performance Tips

### **UpdateSourceTrigger=PropertyChanged**
**Kelebihan:**
- ? Real-time update
- ? CanExecute langsung update

**Kekurangan:**
- ?? Trigger setiap keystroke
- ?? Bisa lambat jika ada heavy validation

**Alternatif untuk production:**
```xaml
<!-- Update saat lost focus -->
Text="{Binding Email, UpdateSourceTrigger=LostFocus}"

<!-- Atau kombinasi -->
Text="{Binding Email, UpdateSourceTrigger=PropertyChanged, Delay=300}"
```

---

## ?? Jika Masih Ada Masalah

### **Steps to Report Issue:**
1. Screenshot window dengan issue
2. Copy paste error dari Debug Output
3. Share XAML snippet yang bermasalah
4. Explain expected vs actual behavior

### **Info yang Dibutuhkan:**
- [ ] Apa yang dilakukan (step by step)
- [ ] Apa yang diharapkan terjadi
- [ ] Apa yang sebenarnya terjadi
- [ ] Error messages dari Debug Output
- [ ] Screenshot jika ada visual issue

---

**Status:** ? All known issues fixed!
**Build:** ? Successful
**Ready for:** Testing & Development
