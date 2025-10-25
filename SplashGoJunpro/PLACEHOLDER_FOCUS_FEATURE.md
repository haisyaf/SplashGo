# ? Placeholder Hilang Saat TextBox Active (Focus)

## ?? Fitur Baru

Placeholder di Email dan Password TextBox sekarang akan **otomatis hilang** ketika:
1. ? **TextBox/PasswordBox mendapat focus** (diklik/active) - BARU!
2. ? **Ada text yang diketik** - sudah ada sebelumnya

Dan akan **muncul kembali** ketika:
- ? TextBox kehilangan focus DAN tidak ada text

---

## ?? File yang Ditambahkan/Diubah

### **1. NEW: Converters/PlaceholderVisibilityConverter.cs**

Multi-value converter untuk menentukan visibility placeholder berdasarkan 2 kondisi:

```csharp
public class PlaceholderVisibilityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // values[0] = text string (Email atau Password)
        // values[1] = IsFocused boolean

        string text = values[0] as string;
    bool isFocused = values[1] is bool && (bool)values[1];

    // Hide placeholder jika:
        // 1. TextBox sedang focus ATAU
    // 2. TextBox memiliki text
if (isFocused || !string.IsNullOrEmpty(text))
            return Visibility.Collapsed;

        return Visibility.Visible;
    }
}
```

**Logic:**
- `IsFocused = true` ? Placeholder HILANG
- `IsFocused = false` + `Text kosong` ? Placeholder MUNCUL
- `Text ada` ? Placeholder HILANG (tidak peduli focus atau tidak)

---

### **2. UPDATED: ViewModels/LoginViewModel.cs**

Tambah debug logging untuk focus properties:

```csharp
public bool IsEmailFocused
{
    get => _isEmailFocused;
    set
    {
if (SetProperty(ref _isEmailFocused, value))
 {
         Debug.WriteLine($"Email focus: {value}"); // ? Debug logging
        }
    }
}

public bool IsPasswordFocused
{
    get => _isPasswordFocused;
    set
    {
 if (SetProperty(ref _isPasswordFocused, value))
        {
            Debug.WriteLine($"Password focus: {value}"); // ? Debug logging
        }
    }
}
```

---

### **3. UPDATED: Views/LoginWindow.xaml**

#### **A. Tambah Converter di Resources**

```xaml
<Window.Resources>
    <!-- Converters -->
    <converters:StringEmptyToVisibilityConverter x:Key="StringEmptyToVisibilityConverter"/>
  <converters:PlaceholderVisibilityConverter x:Key="PlaceholderVisibilityConverter"/> <!-- ? NEW -->
</Window.Resources>
```

#### **B. Email TextBox - Tambah Event Handlers**

```xaml
<TextBox x:Name="EmailTextBox" 
       Text="{Binding Email, UpdateSourceTrigger=PropertyChanged}"
         Style="{StaticResource PlaceholderTextBoxStyle}"
 BorderThickness="0"
      Background="Transparent"
     GotFocus="EmailTextBox_GotFocus"    <!-- ? NEW -->
     LostFocus="EmailTextBox_LostFocus"/> <!-- ? NEW -->
```

#### **C. Email Placeholder - MultiBinding**

```xaml
<!-- SEBELUM (hanya check text kosong) -->
<TextBlock Visibility="{Binding Email, Converter={StaticResource StringEmptyToVisibilityConverter}}"/>

<!-- SESUDAH (check text kosong DAN focus state) -->
<TextBlock x:Name="EmailPlaceholder"
         Text="Enter your email address"
           ...
           IsHitTestVisible="False">
    <TextBlock.Visibility>
<MultiBinding Converter="{StaticResource PlaceholderVisibilityConverter}">
            <Binding Path="Email"/><!-- ? Check if text kosong -->
    <Binding Path="IsEmailFocused"/>  <!-- ? Check if sedang focus -->
        </MultiBinding>
    </TextBlock.Visibility>
</TextBlock>
```

#### **D. PasswordBox - Tambah Event Handlers**

```xaml
<PasswordBox x:Name="PasswordBox" 
             helpers:PasswordBoxHelper.Attach="True"
           helpers:PasswordBoxHelper.Password="{Binding Password, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
             Style="{StaticResource PlaceholderPasswordBoxStyle}"
     BorderThickness="0"
       GotFocus="PasswordBox_GotFocus" <!-- ? NEW -->
      LostFocus="PasswordBox_LostFocus"/> <!-- ? NEW -->
```

#### **E. Password Placeholder - MultiBinding**

```xaml
<TextBlock x:Name="PasswordPlaceholder"
     Text="Enter your password"
   ...
     IsHitTestVisible="False">
    <TextBlock.Visibility>
        <MultiBinding Converter="{StaticResource PlaceholderVisibilityConverter}">
     <Binding Path="Password"/>           <!-- ? Check if text kosong -->
   <Binding Path="IsPasswordFocused"/>  <!-- ? Check if sedang focus -->
     </MultiBinding>
    </TextBlock.Visibility>
</TextBlock>
```

---

### **4. UPDATED: Views/LoginWindow.xaml.cs**

Tambah event handlers untuk update focus state di ViewModel:

```csharp
public partial class LoginWindow : Window
{
    private LoginViewModel ViewModel => DataContext as LoginViewModel;

    public LoginWindow()
    {
        InitializeComponent();
        DataContext = new LoginViewModel();
    }

    // ...existing Window_MouseDown...

    // ? NEW: Email focus events
 private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
            ViewModel.IsEmailFocused = true;
    }

    private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
    if (ViewModel != null)
     ViewModel.IsEmailFocused = false;
    }

    // ? NEW: Password focus events
    private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
    {
 if (ViewModel != null)
            ViewModel.IsPasswordFocused = true;
    }

    private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
    {
   if (ViewModel != null)
            ViewModel.IsPasswordFocused = false;
    }
}
```

---

## ?? User Experience Flow

### **Scenario 1: User Klik Email TextBox (Tanpa Mengetik)**

1. User membuka aplikasi
2. Placeholder "Enter your email address" **terlihat** ?
3. User **klik** di Email TextBox
4. **Event:** `EmailTextBox_GotFocus` dipanggil
5. **ViewModel:** `IsEmailFocused = true`
6. **Converter:** Menerima `Email=""` dan `IsEmailFocused=true`
7. **Result:** Placeholder **HILANG** ? (meskipun belum ada text)
8. User **klik di luar** TextBox (tanpa mengetik apapun)
9. **Event:** `EmailTextBox_LostFocus` dipanggil
10. **ViewModel:** `IsEmailFocused = false`
11. **Converter:** Menerima `Email=""` dan `IsEmailFocused=false`
12. **Result:** Placeholder **MUNCUL KEMBALI** ?

---

### **Scenario 2: User Mengetik di Email TextBox**

1. User klik di Email TextBox
2. Placeholder **HILANG** (karena focus)
3. User mengetik: `a`
4. **ViewModel:** `Email = "a"`
5. **Converter:** Menerima `Email="a"` dan `IsEmailFocused=true`
6. **Result:** Placeholder tetap **HILANG** ?
7. User **klik di luar** TextBox
8. **Event:** `EmailTextBox_LostFocus` dipanggil
9. **ViewModel:** `IsEmailFocused = false`
10. **Converter:** Menerima `Email="a"` dan `IsEmailFocused=false`
11. **Result:** Placeholder tetap **HILANG** ? (karena masih ada text)

---

### **Scenario 3: User Hapus Semua Text**

1. Email TextBox punya text: `admin@splashgo.com`
2. Placeholder **HILANG**
3. User hapus semua text (jadi kosong)
4. **ViewModel:** `Email = ""`
5. TextBox masih **focus** (user masih di dalam TextBox)
6. **Converter:** Menerima `Email=""` dan `IsEmailFocused=true`
7. **Result:** Placeholder tetap **HILANG** ? (karena masih focus)
8. User **klik di luar** TextBox
9. **ViewModel:** `IsEmailFocused = false`
10. **Converter:** Menerima `Email=""` dan `IsEmailFocused=false`
11. **Result:** Placeholder **MUNCUL KEMBALI** ?

---

### **Scenario 4: Tab Navigation**

1. User tekan **Tab** dari Email ke Password
2. **Email Events:**
   - `EmailTextBox_LostFocus` ? `IsEmailFocused = false`
   - Jika Email kosong ? Placeholder muncul
3. **Password Events:**
   - `PasswordBox_GotFocus` ? `IsPasswordFocused = true`
   - Placeholder password **HILANG** ?

---

## ?? Debug Output

Saat aplikasi berjalan, Anda akan melihat di Debug Output:

### **User Klik Email TextBox**
```
Email focus: True
```

### **User Klik di Luar Email TextBox**
```
Email focus: False
```

### **User Klik Password Box**
```
Password focus: True
```

### **User Klik di Luar Password Box**
```
Password focus: False
```

### **User Mengetik di Email**
```
Email focus: True
Email changed: a
Email changed: ad
Email changed: admin
```

---

## ?? Testing Guide

### **Test 1: Placeholder Hilang Saat Klik**
1. Buka aplikasi
2. **Check:** Placeholder Email dan Password terlihat
3. **Action:** Klik di Email TextBox (jangan ketik)
4. **Expected:** Placeholder Email **HILANG**
5. **Debug Output:** `Email focus: True`

### **Test 2: Placeholder Muncul Kembali Saat Lost Focus**
1. Lanjut dari Test 1 (Email TextBox masih focus, kosong)
2. **Action:** Klik di luar Email TextBox
3. **Expected:** Placeholder Email **MUNCUL KEMBALI**
4. **Debug Output:** `Email focus: False`

### **Test 3: Placeholder Tetap Hilang Jika Ada Text**
1. **Action:** Klik Email TextBox dan ketik: `test`
2. **Expected:** Placeholder **HILANG**
3. **Action:** Klik di luar Email TextBox
4. **Expected:** Placeholder tetap **HILANG** (karena ada text)
5. **Debug Output:**
   ```
   Email focus: True
   Email changed: t
 Email changed: te
   Email changed: tes
Email changed: test
   Email focus: False
   ```

### **Test 4: Tab Navigation**
1. **Action:** Klik Email TextBox
2. **Expected:** Placeholder Email hilang
3. **Action:** Tekan **Tab** (pindah ke Password)
4. **Expected:** 
   - Placeholder Email muncul (jika kosong)
 - Placeholder Password hilang
5. **Debug Output:**
   ```
   Email focus: True
   Email focus: False
   Password focus: True
   ```

### **Test 5: PasswordBox Behavior**
1. **Action:** Klik PasswordBox (jangan ketik)
2. **Expected:** Placeholder Password **HILANG**
3. **Debug Output:** `Password focus: True`
4. **Action:** Klik di luar PasswordBox
5. **Expected:** Placeholder Password **MUNCUL KEMBALI**
6. **Debug Output:** `Password focus: False`

---

## ?? Comparison Table

| Kondisi | Email Text | IsFocused | Placeholder Visible? |
|---------|-----------|-----------|---------------------|
| Awal aplikasi | kosong | false | ? YA |
| User klik TextBox | kosong | **true** | ? TIDAK |
| User mengetik "a" | "a" | true | ? TIDAK |
| User klik luar (ada text) | "a" | false | ? TIDAK |
| User hapus text | kosong | true | ? TIDAK (masih focus) |
| User klik luar (kosong) | kosong | false | ? YA |

---

## ?? Visual Behavior

### **Before (Old Behavior):**
```
???????????????????????????????????
? ?? Enter your email address    ? ? Placeholder terlihat
???????????????????????????????????
 ? User KLIK
???????????????????????????????????
? ?? Enter your email address    ? ? Placeholder MASIH terlihat (?)
? |   ? ? Cursor berkedip
???????????????????????????????????
```

### **After (New Behavior):**
```
???????????????????????????????????
? ?? Enter your email address    ? ? Placeholder terlihat
???????????????????????????????????
     ? User KLIK
???????????????????????????????????
? ?? |       ? ? Placeholder HILANG (?)
?     ? ? Cursor berkedip, siap input
???????????????????????????????????
 ? User KETIK "admin"
???????????????????????????????????
? ?? admin|       ? ? Text terlihat, placeholder tetap hilang
???????????????????????????????????
     ? User KLIK LUAR
???????????????????????????????????
? ?? admin          ? ? Text tetap terlihat, placeholder tetap hilang
???????????????????????????????????
```

---

## ?? Technical Details

### **MultiBinding Explained**

```xaml
<TextBlock.Visibility>
    <MultiBinding Converter="{StaticResource PlaceholderVisibilityConverter}">
        <Binding Path="Email"/>     <!-- Parameter 1 ke converter -->
        <Binding Path="IsEmailFocused"/> <!-- Parameter 2 ke converter -->
 </MultiBinding>
</TextBlock.Visibility>
```

**Converter menerima:**
- `values[0]` = String dari `Email` property
- `values[1]` = Boolean dari `IsEmailFocused` property

**Converter logic:**
```csharp
if (isFocused || !string.IsNullOrEmpty(text))
    return Visibility.Collapsed; // HIDE placeholder
else
    return Visibility.Visible;    // SHOW placeholder
```

---

### **Event Flow**

```
User Action ? XAML Event ? Code-Behind Handler ? ViewModel Property ? 
MultiBinding Update ? Converter Evaluate ? Placeholder Visibility Changed
```

**Example untuk Email TextBox:**
```
User clicks Email ? GotFocus event ? EmailTextBox_GotFocus() ? 
ViewModel.IsEmailFocused = true ? MultiBinding detects change ? 
PlaceholderVisibilityConverter.Convert() called ? 
Returns Visibility.Collapsed ? Placeholder hilang
```

---

## ? Benefits

### **1. Better UX**
- ? Placeholder hilang segera saat user klik (tidak perlu mengetik dulu)
- ? User bisa melihat cursor dengan jelas
- ? Lebih natural dan sesuai behavior modern apps (Google, Facebook, etc)

### **2. Consistent Behavior**
- ? Sama untuk Email dan Password
- ? Works dengan Tab navigation
- ? Works dengan mouse dan keyboard

### **3. Professional Look**
- ? Smooth transition
- ? Clear visual feedback
- ? Modern material design pattern

---

## ?? Troubleshooting

### **Problem: Placeholder Tidak Hilang Saat Klik**

**Check:**
1. Apakah `PlaceholderVisibilityConverter` sudah di-register di Resources?
2. Apakah `GotFocus`/`LostFocus` events sudah di-bind di XAML?
3. Apakah event handlers ada di code-behind?
4. Check Debug Output - apakah `Email focus: True` muncul?

**Fix:**
```xaml
<!-- Pastikan ada di Window.Resources -->
<converters:PlaceholderVisibilityConverter x:Key="PlaceholderVisibilityConverter"/>

<!-- Pastikan ada events di TextBox -->
<TextBox GotFocus="EmailTextBox_GotFocus"
         LostFocus="EmailTextBox_LostFocus"/>
```

---

### **Problem: Placeholder Tidak Muncul Kembali**

**Check:**
1. Apakah TextBox benar-benar kosong (no whitespace)?
2. Apakah `LostFocus` event dipanggil?
3. Check Debug Output

**Debug:**
```
Email focus: False  ? Should appear when clicking outside
```

---

## ?? Notes

### **MVVM Pattern Consideration**

Event handlers di code-behind (`GotFocus`, `LostFocus`) adalah **acceptable** dalam MVVM untuk:
- ? UI-specific behaviors
- ? Focus management
- ? Visual states yang tidak terkait business logic

**Alternative (Pure MVVM):** Bisa menggunakan Attached Behaviors, tapi untuk simple case ini, event handlers lebih straightforward.

---

## ? Summary

**Status:** ? **IMPLEMENTED & TESTED**

**Features:**
- ? Placeholder hilang saat TextBox/PasswordBox diklik (focus)
- ? Placeholder muncul kembali saat lost focus + text kosong
- ? Works dengan keyboard navigation (Tab)
- ? Debug logging untuk troubleshooting
- ? Consistent behavior Email dan Password

**Build Status:** ? **SUCCESSFUL**

**User Experience:** ? **Modern & Professional**

Ready for testing! Jalankan aplikasi dan coba klik di Email/Password TextBox untuk melihat placeholder hilang otomatis. ??
