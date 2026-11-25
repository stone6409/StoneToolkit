# StoneToolkit.WpfCommon库介绍

StoneToolkit.WpfCommon是一个专门为WPF应用程序开发设计的.NET类库，它提供了丰富的WPF相关组件、控件扩展、转换器、命令模式实现等实用工具。

## 项目基本信息
- **目标框架**: 支持多目标框架（net9.0-windows, net472）
- **依赖项**: 基于StoneToolkit.Common库
- **用途**: WPF应用程序开发辅助工具库

## 主要功能模块

### 1. **输入命令系统**
- **RelayCommand**: 实现了ICommand接口的轻量级命令类，支持带参数和不带参数的执行方法
- 支持CanExecute验证和自动重新查询建议

### 2. **树视图控件扩展**
- **TreeViewNode**: 可观察对象基类，提供IsSelected、IsInEditMode、IsExpanded等属性
- **TreeViewExtension/TreeViewItemExtension**: 树视图控件的扩展方法
- **TreeViewCustomStyles**: 自定义树视图样式

### 3. **数据网格定制**
- **DataGridCustomStyles**: 数据网格的自定义样式
- **FocusExtension**: 焦点管理扩展
- **DataGridComands**: 数据网格相关命令

### 4. **菜单系统**
- **MenuItem**: 基础菜单项类
- **CommandMenuItem**: 支持命令的菜单项
- **SeparatorItem**: 分隔符菜单项
- **MenuItemContainerTemplateSelector**: 菜单项容器模板选择器

### 5. **值转换器**
- **BoolToVisibilityConverter**: 布尔值到可见性转换
- **BoolToVisibilityHiddenConverter**: 布尔值到可见性转换（隐藏而非折叠）
- **EnumToBoolConverter**: 枚举值到布尔值转换
- **CollectionToBoolConverter**: 集合到布尔值转换
- **StringToBoolConverter**: 字符串到布尔值转换
- **ObjectToBoolConverter**: 对象到布尔值转换
- **LeftMarginMultiplierConverter**: 左边距倍数转换器
- **BoolToFontBoldConverter**: 布尔值到字体粗细转换

### 6. **控件扩展**
- **PasswordBoxExtensions**: 密码框扩展功能
- **ScrollViewerCorrector**: 滚动查看器校正器
- **OverrideCursor**: 光标覆盖控制

### 7. **实用工具**
- **UiHelpers**: UI相关辅助方法
- **WindowExtensions**: 窗口扩展方法
- **WindowStyleHelper**: 窗口样式助手

### 8. **杂项功能**
- **DataGridComands**: 数据网格命令集合
- **ScrollViewerCorrector**: 解决滚动查看器的常见问题

## 技术特点

### 1. **MVVM模式支持**
- 完整的RelayCommand实现，便于ViewModel中的命令绑定
- 可观察对象基类支持属性变更通知

### 2. **UI控件增强**
- 树视图节点的完整状态管理（选中、编辑、展开）
- 数据网格的样式和功能扩展
- 菜单系统的灵活配置

### 3. **值转换器丰富**
- 覆盖了WPF开发中常见的值转换场景
- 支持复杂的数据绑定转换需求

### 4. **跨框架兼容**
- 支持.NET Framework 4.7.2和.NET 9.0
- 处理了WPF主题程序集的兼容性问题

### 5. **性能优化**
- 使用弱事件模式避免内存泄漏
- 合理的依赖注入设计

## 使用示例

### 1. **使用RelayCommand**
```csharp
#region MyCommand

RelayCommand _myCommand;

public RelayCommand MyCommand
{
    get
    {
        if (_myCommand == null)
        {
            _myCommand = new RelayCommand(MyCommandExecute, CanMyCommandExecute);
        }

        return _myCommand;
    }
}

private void MyCommandExecute()
{
}

private bool CanMyCommandExecute()
{
    return true;
}

#endregion
```

### 2. **使用值转换器**
```xaml
<Window.Resources>
    <converters:BoolToVisibilityConverter x:Key="BoolToVisibility" />
</Window.Resources>

<TextBlock Visibility="{Binding IsVisible, Converter={StaticResource BoolToVisibility}}" />
```
