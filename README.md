# cmd-parser 项目说明

## 项目概述
`cmd-parser` 是一个命令解析器项目，其主要功能是对命令进行解析和执行。通过该项目，可以将输入的命令转换为可执行的操作，同时支持对命令中的参数进行解析和处理。

### 支持功能
1. 支持指令嵌套, 嵌套指令形式: `%"...指令字符串..."`
2. 支持参数为数组类型或者字典类型. 数组形式: `[element1,element2,element3]`,
字典形式`{key1=value1,key2=value2,key3=value3}` 或者 `{key1:value1,key2:value2,key3:value3}`
3. 支持参数为字典, 数组混合嵌套类型
4. 支持对参数值反序列化所需的类型, 支持的反序列化类型: 数组, 集合, 字典, 一般对象
5. 支持指令注册模块和指令执行模块的分离

## 项目结构
项目主要包含以下几个部分：
- **Define 目录**：包含各种定义文件，如命令定义（`CommandDefine.cs`）、指令集定义（`CommandSet.cs`）、选项参数定义（`KvParamDefine.cs`）等，用于定义命令解析过程中所需的各种常量、类型和接口。
- **SubParsers 目录**：包含子解析器相关代码，例如 `CommandParser.cs`，负责对具体命令进行解析。
- **Value 目录**：包含与值解析相关的代码，如 `ValueParseForObject.cs` 和 `ValueParseForNumber.cs`，用于将输入的值转换为相应的数据类型。
- **其他文件**：如 `ParserBuilder.cs` 用于构建解析器，`CommandExecuter.cs` 用于执行解析后的命令，`ParserBase.cs` 作为解析器的基类，`ParseTask.cs` 表示指令解析任务等。

## 主要功能模块

### 命令解析
- **命令定义**：在 `Define` 目录下的文件中定义了各种命令及其参数的格式和规则。
- **命令解析器**：`SubParsers` 目录中的代码负责将输入的命令字符串解析为内部可识别的命令对象。

### 值解析
- **对象解析**：`ValueParseForObject.cs` 实现了将输入值解析为一般对象的功能，通过反射机制获取对象的属性和字段，并将输入值转换为对应类型后进行赋值。
- **数值解析**：`ValueParseForNumber.cs` 负责处理数值类型的解析。

### 执行器
- `CommandExecuter.cs` 负责根据解析后的命令对象执行相应的操作。

## 调试信息
在项目中，可以通过在文件首行添加宏定义 `"TEST"` 来打开对应调试信息显示。目前在以下文件中存在调试信息定义：
1. `CommandDefine.cs`
2. `Args.cs`
4. `ParseTask.cs`
5. `ValueParse.cs`

## 如何使用?
确保你已经安装了 .NET 7.0 开发环境
### 1. 定义虚拟控制台对象,
它将负责对指令字符串的解析和执行, 同时允许你向它注册所需的指令
``` C#
VisualConsole console = new VisualConsole();
```
### 2. 向控制台对象注册所需的指令
2.1 直接通过 `AddCommands()` 方法注册指令
``` C#
//这将返回一个指令定义对象
CommandDefine cmddf = console.AddCommands("command_name","command_description");
//给指令内定义所需的参数:
//添加位置参数, 根据需要配置参数属性
//参数定义各字段默认值请查看对应的类文件
cmddf.AddParam(new PosParamDefine{
	name = "message",//参数名称
	description = "消息",//参数说明文本
	isRequire = false,//是否必填
	defaultValue = "",//缺省时的默认值
	type="string",//类型说明文本
});
//添加键值对参数, 更具需要配置参数属性
cmddf.AddParam(new KvParamDefine
{
    name = "repeat",//参数名称, 使用时为 --repeat
    epithet = "r",//参数别称,   使用时为 -r
    isMark = true,//是否为标记型参数, 如果是的话, 允许不填入值
    markValue = 2,//参数出现但是未填写值时的默认值
    defaultValue = 1,//参数缺省时的默认值
    isRequire = false,//是否必填
    type = "int",//类型说明文本
    description = "重复次数"//参数说明文本
});
//给指令定义执行委托:
cmddf.execute = (ce,args)=>{
    //ce 是指令执行器对象
    //args 是解析出的参数组

    //通过索引器获取所需的参数值
    //参数值会在解析时自动补全, 无需在此处进行缺省判断
    var message = args["message"].String;
    var count = args["repeat"].Int;
    //通过 Value.String, Value.Int 可将Value容器内的值转换为所需的类型并获取

    while (count > 0)
    {
        Console.WriteLine(message);
        count--;
    }

    //最后需要返回一个 Value, 如果该指令作为其他指令的嵌套指令的话
    //那么该返回值将会作为上一层的参数值
    return new Value(message);
}
```
2.2 通过 `RegisterAllSet()` 注册当前程序集内所有指令

**注册执行部分**:
``` C#
console.RegisterAllSet();
```

**指令定义部分**:
方式一: 通过 `ICommanSet` 接口定义指令集
``` C#
//需要实现 `ICommanSet`, 并使用 `[CommandRegister]` 修饰
[CommandRegister]
class MyCommandSet:ICommandSet
{
    //ICommanSet 接口要求实现该方法
    public void RegisteredCommands(CommandSet set)
    {
        //在这里完成指令添加
        CommandDefine cmddf = set.AddCommand("commandName","commandDescription");
        //....
    }
}
```
方法二: 通过`[CommandRegister]`方法定义指令集
``` C#
class MyClass{
    //在一般类型下, 可以使用 [CommandRegister] 对方法进行修饰, 也能够达成相同的效果
    [CommandRegister]
    private void Register(CommandSet set)
    {
        //在这里完成指令添加
    }
    //公开,私有,静态,实例方法都可以
    //但是参数个数和类型必须保持和 RegisteredCommands(CommandSet set) 相同
    public static void Register2(CommandSet set)
    {
        //在这里完成指令添加
    }
}
```
### 3. 解析并执行指令
``` C#
string input = Console.ReadLine() + string.Empty;
console.ParseAndRun(input);
```
### 更多样例
更多用法和样例查看项目下 Sample 目录, 更改 Program.cs 内的样例索引, 运行以查看不同样例的执行效果







## 许可证
[说明项目使用的许可证，例如 MIT 等，如果没有可以写暂未确定]
