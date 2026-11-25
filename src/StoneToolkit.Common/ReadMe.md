# StoneToolkit.Common库介绍

## 概述
StoneToolkit.Common是一个基于.NET Standard 2.0的通用工具库，提供了C#应用程序开发中常用的基础功能组件。该库不包含UI相关代码，专注于提供跨平台的基础设施支持。

## 主要功能模块

### 1. **字节处理工具 (ByteUtilities)**
- **BoolConverter**: 布尔值与字节数组的转换
- **ByteArray**: 字节数组操作工具
- **ByteArrayBuilder**: 字节数组构建器
- **ByteConverter**: 字节转换基础类
- **ByteSegment/ByteSegmentCollection**: 字节段管理
- **HexConverter**: 十六进制转换
- **ObjectValueConverter**: 对象值转换器
- **UInt16Converter**: 无符号16位整数转换

### 2. **集合类 (Collections)**
- **SortedObservableCollection**: 排序的可观察集合
- **SortedObservableCollectionWithComparer**: 带比较器的排序可观察集合

### 3. **命令行解析 (CommandLineParsing)**
- **CommandLineParser**: 完整的命令行参数解析器
- **CommandLineDictionary**: 命令行参数字典
- **Command**: 命令定义基类
- **RequiredAttribute**: 必需参数标记
- **AdditionalPropertiesAttribute**: 附加属性标记
- **FileInfoConverter/DirectoryInfoConverter**: 文件/目录信息转换器

### 4. **组件模型 (ComponentModel)**
- **ObservableObject**: 实现INotifyPropertyChanged接口的基础类，支持属性变更通知

### 5. **配置管理 (Configuration)**
- **ExeConfiguration**: 应用程序配置文件管理
- **RecentStringsManager**: 最近使用字符串管理

### 6. **配置文件处理 (ConfigureFile)**
- **ConfigureFileBase**: 配置文件基类
- **SerializationHelper**: 序列化辅助工具

### 7. **进程间通信 (IPC)**
- **PipeServer/PipeClient**: 命名管道服务器/客户端
- **PipeServerInfoManager**: 管道服务器信息管理
- **FileTransferPipeServer**: 文件传输管道服务器
- **PipeServerInfoHelper**: 管道服务器信息辅助工具

### 8. **日志系统 (Logging)**
- **Logger**: 基于NLog的日志记录器封装
- **LoggerProvider**: 日志提供者
- **SystemLogger**: 系统日志记录器
- **MessageLogger**: 消息日志记录器
- **ThisMethodCallTarget**: 方法调用跟踪

### 9. **命名规则验证 (Naming)**
- **NamingRulesValidator**: 命名规则验证器
- **NameMap**: 名称映射
- **NamingRule**: 命名规则定义
- **ValidationResult**: 验证结果

### 10. **服务定位 (Services)**
- **ServiceLocator**: 简单的服务定位器

### 11. **定时器 (Timers)**
- **IntervalTrigger**: 间隔触发器
- **IntervalTriggerManager**: 间隔触发器管理器
- **TimerIntervalTrigger**: 定时器间隔触发器

### 12. **实用工具 (Utilities)**
- **ApplicationUtils**: 应用程序工具
- **EncryptionHelper**: 加密辅助工具
- **EnumTypeHelper**: 枚举类型辅助
- **FilePathUtils**: 文件路径工具（包含应用程序数据文件夹管理）
- **ProcessUtils**: 进程工具
- **ReflectionHelper**: 反射辅助工具
- **SingleInstanceManager**: 单实例管理器
- **StringHelper**: 字符串工具
- **XmlSerializerUtils**: XML序列化工具

## 技术特点

### **依赖项**
- NLog (5.3.4): 强大的日志记录框架
- Microsoft.Win32.Registry (5.0.0): Windows注册表访问
- System.Configuration.ConfigurationManager (5.0.0): 配置管理

### **架构设计**
1. **模块化设计**: 每个功能模块独立，便于维护和扩展
2. **接口驱动**: 使用标准.NET接口，易于测试和替换
3. **异步支持**: 在IPC等模块中提供异步操作支持
4. **事件驱动**: 使用事件机制实现松耦合

### **使用场景**
1. **桌面应用程序**: 提供配置管理、日志记录、进程间通信等基础设施
2. **命令行工具**: 提供强大的命令行参数解析功能
3. **服务程序**: 支持单实例管理、定时任务等
4. **跨平台应用**: 基于.NET Standard 2.0，支持多平台

## 典型应用示例

### 1. **属性变更通知**
```csharp
public class ViewModel : ObservableObject
{
    private string _stringProperty;

    public string StringProperty
    {
        get
        {
            return _stringProperty;
        }
        set
        {
            _stringProperty = value;
            RaisePropertyChanged(nameof(StringProperty));
        }
    }
}
```

