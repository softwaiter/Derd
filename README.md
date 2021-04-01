# netcoreORM
.NetCore轻量级ORM框架

## 一、概述

​		nercoreORM是一个基于.net core开发的跨平台轻量级数据库操作类库，全名称为CodeM.Common.Orm。netcoreORM模型定义文件基于XML文件格式，模型管理基于目录自动分类；数据库类型支持Sqlite、MySql、Oracle、Sqlserver、Postgresql等，数据库配置文件和模型定义一样基于目录划分，并支持基于目录层级的继承能力；数据操作采用链式方式，简单易用。

#### 特点列表：

* XML格式定义模型

* 索引约束定义

* 关联外键定义

* 数据库事务支持

* 查询方法支持链式操作

* 保存前字段值自定义加工，支持动态函数处理器

* 查询后字段值自定义处理，支持动态函数处理器

* 新建数据保存时属性是否参与保存可设置

* 更新数据保存时属性是否参与保存可设置

* 默认值设置支持自定义，支持动态函数处理器

* 查询返回dynamic动态对象，默认关联模型定义属性



## 二、快速入门

### 2.1 安装

#### Package Manager

```shell
Install-Package CodeM.Common.Orm -Version 1.1.12
```



#### .NET CLI

```shell
dotnet add package CodeM.Common.Orm --version 1.1.12
```



#### PackageReference

```xml
<PackageReference Include="CodeM.Common.Orm" Version="1.1.12" />
```



#### Paket CLI

```shell
paket add CodeM.Common.Orm --version 1.1.12
```



### 2.2 数据库依赖

根据数据库连接所配置的数据库类型，需要安装相应的数据库依赖包，具体对应关系如下：

| 数据库库类型 | 依赖包                        |
| ------------ | ----------------------------- |
| Sqlite       | System.Data.SQLite            |
| Mysql        | MySql.Data                    |
| Oracle       | Oracle.ManagedDataAccess.Core |
| SqlServer    | Microsoft.Data.SqlClient      |

使用时，可根据需要添加其中的一项或多项依赖。

*注：其他未列出的实现了微软数据提供者接口的数据库理论上应支持，尚未实际验证。



## 三、数据库连接

所有的模型定义按照存储目录划分，模型定义根目录通过OrmUtils.ModelPath属性进行指定，默认为当前程序的根目录下的models子目录。

数据库连接和模型存储目录相对应，模型存储根目录及其下面的子目录都可以通过数据库配置文件设置自己的数据库连接，如果当前目录没有数据库连接配置文件，将继续向上寻找父目录的数据库连接配置文件，直到找到为止；基于此设计逻辑，根目录必须有数据库连接配置文件。

数据库连接配置文件命必须为.connection.xml，格式如下：

```xml
<?xml version="1.0" encoding="utf-8" ?>
<connection>
    <dialect>sqlite</dialect>	<!--数据库类型，目前支持sqlite、mysql、oracle、sqlserver-->
    <host>test.db</host>	<!--数据库文件地址或IP-->
    <port></port>	<!--数据库端口，为空即不设置，按数据库默认端口-->
    <user></user>	<!--数据库账户名-->
    <password></password>	<!--数据库账户密码-->
    <database></database>	<!--数据库名称-->
    <pool max="100" min="0">true</pool>	<!--数据库连接池设置，max=最大数量，min=最小数量-->
</connection>
```

*注：dialect可选值全部要小写。



## 四、模型定义

模型定义采用xml格式，扩展名必须是.model.xml，每个文件支持定义一个模型。

模型定义文件必须放在ModelPath指定的路径下，可以是子目录，不限层级。

模型中可定义任意多个属性。具体格式如下：

Org模型定义

```xml
<?xml version="1.0" encoding="utf-8" ?>
<model name="Org" table="t_org">
    <property name="Id" field="f_id" notNull="true" primary="true" autoIncrement="true" joinInsert="false" jojnUpdate="false" desc="主键"/>
    <property name="Code" field="f_code" notNull="true" length="32" uniqueGroup="uc_code" desc="机构编码" />
    <property name="Name" field="f_name" length="32" notNull="true" uniqueGroup="uc_name" joinInsert="true" joinUpdate="true" desc="机构名称" />
    <property name="CreateTime" type="DateTime" field="f_createtime" defaultValue="{{CurrentDateTime}}" joinUpdate="false" desc="创建时间" />
    <property name="UpdateTime" type="DateTime" field="f_updatetime" defaultValue="{{CurrentDateTime}}" desc="更新时间" />
</model>
```



User模型定义

```xml
<?xml version="1.0" encoding="utf-8" ?>
<model name="User" table="t_user">
    <property name="Id" field="f_id" notNull="True" primary="true" autoIncrement="true" joinInsert="false" jojnUpdate="false" desc="主键"/>
    <property name="Name" field="f_name" length="32" notNull="true" uniqueGroup="uc_name" joinInsert="true" joinUpdate="true" desc="名称" />
    <property name="Age" field="f_age" type="UInt16" unsigned="true" min="1" max="150" indexGroup="idx_age" joinInsert="true" joinUpdate="true" desc="年龄" />
    <property name="Birthday" type="DateTime" field="f_birthday" fieldType="Date" desc="出生日期" defaultValue="1980/01/01" />
    <property name="Org" field="f_org_code" type="Org" joinProp="Code" desc="所属机构" />
    <property name="Deposit" field="f_deposit" type="Decimal" precision="2" beforeSave="{{EncryptDeposit}}" afterQuery="{{DecryptDeposit}}" desc="银行存款" defaultValue="12345" />
    <property name="IsAdmin" field="f_is_admin" indexGroup="idx_age" type="Boolean" desc="是否超级管理员" defaultValue="false" />
    <property name="CreateTime" type="DateTime" field="f_createtime" defaultValue="{{CurrentDateTime}}" joinUpdate="false" desc="创建时间" />
    <property name="UpdateTime" type="DateTime" field="f_updatetime" beforeSave="{{CurrentDateTime}}" desc="更新时间" />
</model>
```

### 1. model属性：

###### name

指定模型名称，对当前模型的所有操作都将以该名称为标记。

###### table

对应模型的物理表名称，如果未设置，将采用name属性值；可选。

### 2. property属性：

###### name

属性的名称，针对该属性的所有操作都将以该名称为标记。

###### field

对应模型的物理表字段的名称，如果未设置，将采用name属性值；可选。

###### type

属性类型，对应.NetCore中的数据类型，默认String，可选。具体如下表：
| 类型     | 说明                                                         |
| -------- | ------------------------------------------------------------ |
| String   | 字符串                                                       |
| Boolean |布尔型|
| Byte     | 无符号整数，取值0~255                                        |
| SByte    | 有符号整数，取值-128~127                                     |
| Int16    | 有符号整数，取值-32768 ~32767                                |
| UInt16   | 无符号整数，取值0~65,535                                     |
| Int32    | 有符号整数，取值-2,147,483,648 ~2,147,483,647                |
| UInt32   | 无符号整数，取值0 ~ 4364967295                               |
| Int64    | 有符号整数，取值-9,223,372,036,854,775,808 ~9,223,372,036,854,775,807 |
| UInt64   | 无符号整数，取值0~18,446,744,073,709,551,615                 |
| Single   | 32位，单精度浮点数，精度 7 位，取值-3.4 × 10<sup>38</sup> ~ 3.4 × 10<sup>38</sup> |
| Double   | 64位，双精度浮点数，精度 15 - 16 位，取值±5.0 × 10<sup>-324</sup> ~ ±1.7 × 10<sup>308</sup> |
| Decimal  | 128位，高精度浮点数，精度 28 - 29 位，取值±1.0 × 10<sup>-28</sup> ~ ±7.9 × 10<sup>28</sup> |
| DateTime | 日期时间类型                                                 |

注：type属性值可以是其他模型，生成主从模型映射；如OrgCode属性。

###### joinProp

字符串，当type属性为其他关联模型时，此属性用于指定关联模型的关联属性名，默认为空；为空时使用关联模型的第1个主键属性作为关联属性。

###### fieldType

属性对应物理表字段的类型，默认根据type属性自动转换，可选。

可选格式内容：

type属性转换表：

| type属性 | fieldType属性 |
| -------- | ------------- |
| String   | String |
| Boolean |Boolean|
| Byte     | Byte |
| SByte    | SByte |
| Int16    | Int16 |
| UInt16   | UInt16 |
| Int32    | Int32 |
| UInt32   | UInt32 |
| Int64    | Int64 |
| UInt64   | UInt64 |
| Single   | Single |
| Double   | Double |
| Decimal  | Decimal |
| DateTime | DateTime |

###### length

整数型，指定属性的长度，默认0，即不指定，使用数据库默认设置，可选。

###### precision

整数型，指浮点数的小数位数，默认0，可选。

###### min

数值型，当type属性为数值时，该属性用来指定属性取值的下限，默认无；可选。

###### max

数值型，当type属性为数值时，该属性用来指定属性取值的上限，默认无；可选。

###### primary

布尔型，指定当前属性是否为对应物理表的主键，默认为False；可选。

###### autoIncrement

布尔型，指定当前属性是否为自增类型，属性type类型必须为整型，默认为False；可选。

###### notNull

布尔型，属性值是否不允许为空，默认为False；可选。

###### unsigned

布尔型，指定属性是否为无符号数，属性type类型必须为数值型，默认为False；可选。

###### uniqueGroup

字符串，指定模型的唯一约束，该属性值相同的属性将组合成一个唯一约束，可选。

###### desc

字符串，属性的详细说明。

###### defaultValue

字符串，属性默认值，新增模型时如未设置属性值，则使用该值填充。

默认值支持Processor写法，Processor具体用法参照下方的Processor说明。

###### beforeSave

字符串，属性保存前处理器，可选。

该值是一个Processor名称，Processor具体用法参照下方的Processor说明。

###### afterQuery

字符串，属性查询后处理器，可选。

该值是一个Processor名称，Processor具体用法参照下方的Processor说明。

###### joinInsert

布尔型，指示属性是否参与模型的插入操作，默认True；可选。

###### joinUpdate

布尔型，指示属性是否参与模型的修改操作，默认True；可选。



## 五、Processor说明

Processor是一种处理方法，该处理方法根据自己内在的处理逻辑计算处理并返回处理结果；根据不同的应用场景对返回值进行相应的使用。

Processor包含一些内置的系统Processor，也允许用户自定义进行扩展。



所有Processor都必须实现 IProcessor接口，接口定义如下：

```c#
namespace CodeM.Common.Orm
{
    public interface IProcessor
    {
        object Execute(Model model, string prop, dynamic obj);
    }
}
```

该接口只有一个Execute方法，方法需要3个输入参数：

model: 当前Model定义对象

prop: 当前属性的名称

obj: 当前Model的数据实例



具体的Processor只需要实现IProcessor的Execute方法即可，如取得当前时间的CurrentDateTime处理器：

```c#
using System;

namespace CodeM.Common.Orm.Processors
{
    public class CurrentDateTime: IProcessor
    {
        public object Execute(Model model, string prop, dynamic obj)
        {
            return DateTime.Now;
        }
    }
}
```

Execute需要一个object类型的返回值，根据属性类型不同而不同；一旦处理器发现Model定义或属性当前值不符合处理器的处理规则，无法进行处理和继续后续操作时，可以返回Undefined.Value，后续操作将忽略该Processor的存在。



有了Processor实现类，还必须进行注册才可以正常使用，注册使用RegisterProcessor方法，在API使用部分有详细说明：

```c#
OrmUtils.RegisterProcessor("CurrentDateTime", 
                           "CodeM.Common.Orm.Processors.CurrentDateTime");
```



此时，ORM中已经有了一个获取当前日期时间的Processor，名称为CurrentDateTime，所有处理器在模型定义中进行使用时都需要用双大括号包起来，如{{CurrentDateTime}}。



在Model定义中，有3个属性可以使用Processor写法，分别是defaultValue、beforeSave和afterQuery，3个属性中执行逻辑各自不同。

defaultValue: 当在defaultValue中使用Processor时，该值只在Model新建时起作用。在Model新建保存时，会对未设置属性值得属性用Processor处理器进行计算，如果计算结果为Undefined.Value，则放弃处理；否则，用计算结果为属性进行赋值，然后进行保存。

beforeSave: 在Model进行新建保存或修改保存前，调用当前设置的Processor处理器进行计算，如果计算结果为Undefined.Value，则放弃处理；否则，用计算结果为属性进行赋值，然后进行保存。

afterQuery: afterQuery是在查询数据之后，调用当前设置的Processor处理器进行计算，如果计算结果为Undefined.Value，则放弃处理；否则，用计算结果为属性进行赋值，然后进行返回。

举例说明，有如下Model定义：

```xml
<?xml version="1.0" encoding="utf-8" ?>
<model name="User" table="t_user">
    <property name="Id" field="f_id" notNull="True" primary="true" autoIncrement="true" joinInsert="false" jojnUpdate="false" desc="主键"/>
    <property name="Name" field="f_name" length="32" notNull="true" uniqueGroup="uc_name" joinInsert="true" joinUpdate="true" desc="名称" />
    <property name="Age" field="f_age" type="UInt16" unsigned="true" indexGroup="idx_age" joinInsert="true" joinUpdate="true" desc="年龄" />
    <property name="Deposit" field="f_deposit" type="Decimal" precision="2" beforeSave="{{EncryptDeposit}}" afterQuery="{{DecryptDeposit}}" desc="银行存款" defaultValue="12345" />
    <property name="CreateTime" type="DateTime" field="f_createtime" defaultValue="{{CurrentDateTime}}" joinUpdate="false" desc="创建时间" />
    <property name="UpdateTime" type="DateTime" field="f_updatetime" beforeSave="{{CurrentDateTime}}" desc="更新时间" />
</model>
```

Deposit属性设置了beforeSave值为{{EncryptDeposit}}对值进行加密操作，设置了afterQuery值为{{DecryptDeposit}}对返回值进行解密操作。

CreateTime属性设置defaultValue值为{{CurrentDateTime}}，在新建时会对CreateTime赋值为当前时间，而后续的修改操作CreateTime不会变化。

UpdateTime属性设置了beforeSave值为{{CurrentDateTime}}，因此在新建和修改时，系统都会对UpdateTime进行赋值操作，UpdateTime会及时更新为当前操作时间。



## 六、API使用

### 1. OrmUtils类

OrmUtils类是所有功能的入口，OrmUtils属于静态类，所有属性和方法可直接使用，简单方便。



### 2. OrmUtils类属性

###### ModelPath

模型定义文件存储目录，如果未设置，默认当前程序的根目录下的models子目录。



### 3. OrmUtils类方法

##### public static void RegisterProcessor(string name, string classname);

注册用户自定义的Processor。

###### 参数

name：Processor名称，保证唯一性。

classname：Processor实现类，必须是实现了IExecute接口的类。

```c#
OrmUtils.RegisterProcessor("CurrentDateTime", "CodeM.Common.Orm.Processor.Impl.CurrentDateTime");
```



##### public static void Load();

加载模型定义文件；后续所有方法的使用都必须先加载模型，属于初始化方法。

```c#
OrmUtils.Load();
```



##### public static void Refresh();

增量加载模型定义文件；只加载新增的模型定义文件或发生修改的模型定义文件。

```c#
OrmUtils.Refresh();
```



##### public static int GetTransaction()

获取模型根目录绑定数据连接的一个事务

###### 返回

事务对象的整型标识代码。



##### public static int GetTransaction(string path, IsolationLevel level=IsolationLevel.Unspecified)
获取指定模型目录绑定数据的一个事务，事务优先级为用户指定类型

###### 参数

path：模型目录。

level：事务优先级。

###### 返回

事务对象的整型标识代码。



##### public static bool CommitTransaction(int code)

提交指定标识代码对应的事务。

###### 参数

code：事务标识代码。

###### 返回

提交结果，成功返回true；否则，返回false。



##### public static bool RollbackTransaction(int code)

回滚指定标识代码对应的事务。

###### 参数

code：事务标识代码。

###### 返回

提交结果，成功返回true；否则，返回false。



##### public static bool IsDefind(string modelName);

判断指定的模型是否已定义。

###### 参数

modelName：模型名称。

###### 返回

布尔型，模型定义存在返回True；否则，返回False。

```c#
bool userDefined = OrmUtils.IsDefined("User");
if (userDefined)
{
	Console.WriteLine("User模型已定义。");
}
```



##### public static int ExecSql(string sql, string path = "/");

直接执行SQL语句。

###### 参数

sql：要执行的sql语句。

path：sql语句执行的目标数据库连接，默认为根数据库连接，可选。

###### 返回

执行sql语句影响的数据条数。

```c#
int count = OrmUtils.ExecSql("SELECT * FROM t_user WHERE Name='wangxm'");
if (count == 1)
{
    Console.WriteLine("用户wangxm已存在。");
}
```



##### public static int ExecSql(string sql, int transCode)

直接执行SQL语句；功能同上，不同之处是在事务中执行。

###### 参数

sql：要执行的sql语句。

transCode：指定事务的标识代码。

###### 返回

执行sql语句影响的数据条数。



##### public static bool CreateTables(bool force = false);

根据模型定义生成对应的所有数据库物理表。

###### 参数

force：是否覆盖已有物理表，重新创建，默认False。

###### 返回

无，异常时直接抛出异常并中断。

```c#
OrmUtils.CreateTables();
```



##### public static bool TryCreateTables(bool force = false);

尝试根据模型定义生成对应的所有数据库物理表。

###### 参数

force：是否覆盖已有物理表，重新创建，默认False。

###### 返回

创建成功返回True；否则，返回False。

```c#
OrmUtils.TryCreateTables();
```



##### public static void RemoveTables();

将模型定义对应的物理表和数据全部删除。

###### 返回

无，异常时直接抛出异常并中断。

```c#
OrmUtils.RemoveTables();
```



##### public static bool TryRemoveTables();

尝试将模型定义对应的物理表和数据全部删除。

###### 返回

删除成功返回True；否则，返回False。

```c#
OrmUtils.TryRemoveTables();
```



##### public static void TruncateTables();

清空所有物理表数据，同时重置自增ID。

###### 返回

无，异常时直接抛出异常并中断。

```c#
OrmUtils.TruncateTables();
```



##### public static bool TruncateTables();

尝试清空所有物理表数据，同时重置自增ID。

###### 返回

清空成功返回True；否则，返回False。

```c#
OrmUtils.TryTruncateTables();
```



##### public static Model Model(string modelName);

根据指定模型名称查找模型定义

###### 参数

modelName：模型名称，根目录下可使用短名称；否则，必须使用全名。

###### 返回

指定名称的模型定义；没有找到返回null。

```c#
Model user = OrmUtils.Model("User");
```



##### public static bool EnableVersionControl();

启动模型版本控制，版本控制只是对版本号进行管理，不对版本内容进行管理，用户需自行管理版本内容。

###### 返回

启动成功返回True；否则，返回False。

```c#
OrmUtils.EnableVersionControl();
```



##### public static int GetVersion();

获取当前模型版本号。

###### 返回

版本控制如果已启动返回最新版本号；否则，返回-1。

```c#
int version = OrmUtils.GetVersion();
Console.WriteLine("当前版本号：{0}", version);
```



##### public static bool SetVersion(int version);

设置最新的模型版本号。

###### 参数

version：最新的版本号；新版本号必须高于当前版本号，否则将设置失败。

###### 返回

设置成功返回True；否则，返回False。

```c#
OrmUtils.SetVersion(1);
```



## 七、模型用法

### 1. 模型的存储位置

模型定义文件必须存储在OrmUtils.ModelPath指定的模型目录中。

存储目录内可以新建子目录，不限层级，模型定义文件存储在子目录内也是有效的。



### 2. 模型名称

模型名称分为短名称和全名。

短名称：即在模型定义文件中的model标签上的name属性所设定的名称。

全名：模型定义文件所在存储目录+短名称即是模型全名。



例：

模型目录结构如下：

/models	//模型定义根目录

​	|-car.model.xml	//模型定义文件

​	|-custom	//子目录

​		|-dog.model.xml	//模型定义文件



car.model.xml内容：

```xml
<?xml version="1.0" encoding="utf-8" ?>
	<model name="Car" table="t_car">
</model>
```



dog.model.xml内容：

```xml
<?xml version="1.0" encoding="utf-8" ?>
	<model name="Dog" table="t_dog">
</model>
```



模型定义Car短名称：Car，全名 /Car。

模型定义Dog短名称：Dog，全名 /custom/Dog



### 3. 模型属性

###### Path

模型的目录路径，根目录为/，子目录为/子目录名，以此类推；此路径也即代表对应的数据库连接。

###### Name

模型短名称。

###### Table

模型对应的物理表名称。

###### PropertyCount

模型定义中属性的个数。

###### PrimaryKeyCount

模型定义中主键的个数。



### 4. 模型方法

####  行为操作方法

##### public static int GetTransaction()

获取模型所在目录绑定的数据连接的一个事务

###### 返回

事务对象的整型标识代码。



##### public void CreateTable(bool force = false)

创建模型的物理表

###### 参数

force：是否覆盖已有物理表，重新创建，默认False。

###### 返回

无，异常时直接抛出异常并中断。

```c#
OrmUtils.Model("User").CreateTable();	//创建模型User的物理表
```



##### public bool TryCreateTable(bool force = false)

尝试创建模型的物理表

###### 参数

force：是否覆盖已有物理表，重新创建，默认False。

###### 返回

成功返回True；否则返回False。

```c#
OrmUtils.Model("User").TryCreateTable();	//创建模型User的物理表
```



##### public void RemoveTable()

删除模型对应的物理表和数据。

###### 返回

无，异常时直接抛出异常并中断。

```c#
OrmUtils.Model("User").RemoveTable();	//删除模型User的物理表
```



##### public bool TryRemoveTable()

尝试删除模型对应的物理表和数据。

###### 返回

成功返回True；否则返回False。

```c#
OrmUtils.Model("User").TryRemoveTable();	//删除模型User的物理表
```



##### public void TruncateTable()

清空模型对应的物理表数据，同时重置自增ID。

###### 返回

无，异常时直接抛出异常并中断。

```c#
OrmUtils.Model("User").TruncateTable();		//清空模型User的物理表数据
```



##### public bool TryTruncateTable()

尝试清空模型对应的物理表数据，同时重置自增ID。

###### 返回

成功返回True；否则返回False。

```c#
OrmUtils.Model("User").TryTruncateTable();		//清空模型User的物理表数据
```



##### public bool TableExists()

判断当前模型对应的物理表是否存在。

###### 返回

存在返回True；否则返回False。

```c#
OrmUtils.Model("User").TableExists();
```



##### public bool Save(bool validate = false)

新增保存模型数据到物理表。

###### 参数

validate：保存前对数据根据定义规则进行校验，默认不校验；可选。

###### 返回

成功返回True；否则返回False。

```c#
OrmUtils.Model("User").SetValue("Name", "wangxm").SetValue("Age", 18).Save();	//新建一个名称为wangxm年龄为18的新模型并保存到物理表
```



##### public bool Save(int? transCode, bool validate = false)

新增保存模型数据到物理表，功能同上；不同之处是在指定事务中执行。

###### 参数

transCode：指定事务标识代码。

validate：保存前对数据根据定义规则进行校验，默认不校验；可选。

###### 返回

成功返回True；否则返回False。



##### public bool Update(bool updateAll = false)

更新模型数据到物理表。

###### 参数

updateAll：是否更新物理表中的所有数据，默认为False，必须通过查询条件设置方法限定更新范围，如果确实需要更新所有数据，可以设置成True，慎用。

###### 返回

成功返回True；否则返回False。

```c#
OrmUtils.Model("User").SetValue("Age", 20).Equals("Name", "wangxm").Update();	//修改用户wangxm的年龄为20
```



##### public bool Update(int? transCode, bool updateAll = false)

更新模型数据到物理表，功能同上；不同之处是在指定事务中执行。

###### 参数

transCode：指定事务标识代码。

updateAll：是否更新物理表中的所有数据，默认为False，必须通过查询条件设置方法限定更新范围，如果确实需要更新所有数据，可以设置成True，慎用。

###### 返回

成功返回True；否则返回False。



##### public bool Delete(bool deleteAll = false)

删除物理表中的模型数据。

###### 参数

deleteAll：是否删除物理表中的所有数据，默认为False，必须通过查询条件设置方法限定删除范围，如果确实需要删除所有数据，可以设置成True，慎用。

###### 返回

成功返回True；否则返回False。

```c#
OrmUtils.Model("User").Equals("Name", "wangxm").Delete();	//删除用户wangxm
```



##### public bool Delete(int? transCode, bool deleteAll = false)

删除物理表中的模型数据，功能同上；不同之处是在指定事务中执行。

###### 参数

transCode：指定事务标识代码。

deleteAll：是否删除物理表中的所有数据，默认为False，必须通过查询条件设置方法限定删除范围，如果确实需要删除所有数据，可以设置成True，慎用。

###### 返回

成功返回True；否则返回False。



##### public List<dynamic> Query(int? transCode=null)

查询并返回模型物理表中的数据，如果指定事务标识代码，则在事务中执行；否则，使用模型默认数据连接执行。

###### 参数

transCode：指定事务标识代码。

###### 返回

返回模型数据的列表，模型数据对象为dynamic对象，包含的属性由GetValue方法确定，如果为通过GetValue设置，则包括模型所有属性。

```c#
List<dynamic> result = OrmUtils.Model("User").Equals("Name", "wangxm").Query();
dynamic userWxm = result[0];
Console.WriteLine(userWxm.Age);
```



##### public dynamic QueryFirst(int? transCode=null)

查询符合条件的第一条记录，并返回其映射的动态对象，如果指定事务标识代码，则在事务中执行；否则，使用模型默认数据连接执行。

###### 参数

transCode：指定事务标识代码。

###### 返回

第一条记录映射的dynamic对象，包含的属性由GetValue方法确定，如果为通过GetValue设置，则包括模型所有属性。

```c#
dynamic user = OrmUtils.Model("User").Equals("Name", "wangxm").QueryFirst();
Console.WriteLine(user.Name);
```



##### public long Count(int? transCode=null)

计算物理表中的数据条数并返回，如果指定事务标识代码，则在事务中执行；否则，使用模型默认数据连接执行。

###### 参数

transCode：指定事务标识代码。

###### 返回

数据条数。

```c#
OrmUtils.Model("User").Count();		//计算用户表数据总数
OrmUtils.Model("User").Equals("Age", 20).Count();	//计算所有年龄为20的用户总数
```



##### public bool Exists(int? transCode=null)

判断物理表中的数据是否存在，如果指定事务标识代码，则在事务中执行；否则，使用模型默认数据连接执行。

###### 参数

transCode：指定事务标识代码。

###### 返回

存在返回True；否则返回False。

```c#
OrmUtils.Model("User").Equals("Name", "wangxm").Exists();	//判断名为为wangxm的用户是否存在
```



#### 设置属性值方法

##### public Model SetValue(string name, object value)

设置模型指定属性的内容，此处设置内容只是设置到内存中，只有调用Save方法或者Update方法后才会最终存储更新到物理表中。

###### 参数

name：属性名。

value：属性值。

###### 返回

当前Model模型。

```c#
OrmUtils.Model("User").SetValue("Name", "zhangsan").SetValue("Age", 18).Save();
```



##### public Model SetValues(ModelObject obj)

通过一个对象设置批量设置模型属性的内容，此处设置内容只是设置到内存中，只有调用Save方法或者Update方法后才会最终存储更新到物理表中。

###### 参数

obj：ModelObject对象。

###### 返回

当前Model模型。

```c#
dynamic newuser = ModelObject.New("User");
newuser.Name = "lisi";
newuser.Age = 20;
newuser.Birthday = new DateTime(1990, 7, 25);
newuser.Deposit = 123456789.00;
newuser.IsAdmin = true;
OrmUtils.Model("User").SetValues(newuser).Save();
```



#### 设置查询返回属性方法

##### public Model GetValue(params string[] names)

设置Query查询方法返回对象的属性，如果不设置，默认返回模型的所有属性。

###### 返回

当前Model模型。

```c#
List<dynamic> result = OrmUtils.Model("User").GetValue("Name", "Age").Query();
if (result.Count > 0)
{
    Console.WriteLine("{0} - {1}", result[0].Name, result[0].Age);
}
```

注：返回属性可以使用主从模型属性，返回格式按照模型自动嵌套，如或去用户所属机构的名称：

```c#
List<dynamic> result = OrmUtils.Model("User").GetValue("Name", "OrgId.Name").Query();
if (result.Count > 0)
{
    Console.WriteLine("所属机构名称：{0}", result[0].OrgId.Name);
}
```





#### 设置查询条件方法（支持链式调用）

##### public Model And(IFilter subCondition)

设置增加一个并条件的子查询，默认情况下，所有设置查询条件的方法都是隐含的并关系，除非使用了Or方法。

###### 参数

subCondition：子查询条件。

###### 返回

当前Model模型。

```c#
SubFilter subCond = new SubFilter();
subCond.Gt("Age", 18);
OrmUtils.Model("User").Equals("Name", "wangxm").And(subCond).Query();	//查询名称为wangxm并且年龄>18的用户

OrmUtils.Model("User").Equals("Name", "wangxm").Gt("Age", 18).Query();	//查询效果和上面相同，这里两个查询条件是隐含的并关系，无需特别指定
```



##### public Model Or(IFilter subCondition)

设置增加一个或条件的子查询。

###### 参数

subCondition：子查询条件。

###### 返回

当前Model模型。

```c#
SubFilter subCond = new SubFilter();
subCond.Equals("Name", "huxinyue");
OrmUtils.Model("User").Equals("Name", "wangxm").Or(subCond).Query();	//查询名称为wangxm或者名称为huxinyue的用户
```



##### public Model Equals(string name, object value)

设置指定属性的等于查询条件。

###### 参数

name：属性名。

value：比较的值。

###### 返回

当前Model模型。

```c#
OrmUtils.Model("User").Equals("Age", 18).Query();	//查询所有年龄=18的用户
```

注：查询属性名可以使用主从模型属性，如查询所属机构名称为XX科技的用户：

```c#
OrmUtils.Model("User").Equals("OrgId.Name", "XX科技").Query();
```



##### public Model NotEquals(string name, object value)

设置指定属性的不等于查询条件。

###### 参数

name：属性名。

value：比较的值。

###### 返回

当前Model模型。



##### public Model Gt(string name, object value)

设置指定属性的大于查询条件。

###### 参数

name：属性名。

value：比较的值。

###### 返回

当前Model模型。



##### public Model Gte(string name, object value)

设置指定属性的大于等于查询条件。

###### 参数

name：属性名。

value：比较的值。

###### 返回

当前Model模型。

```c#
OrmUtils.Model("User").Gte("Age", 18).Query();	//查询所有年龄>=18的用户
```



##### public Model Lt(string name, object value)

设置指定属性的小于查询条件。

###### 参数

name：属性名。

value：比较的值。

###### 返回

当前Model模型。



##### public Model Lte(string name, object value)

设置指定属性的小于等于查询条件。

###### 参数

name：属性名。

value：比较的值。

###### 返回

当前Model模型。



##### public Model Like(string name, string value)

设置指定属性的Like查询条件。

###### 参数

name：属性名。

value：比较的值。

###### 返回

当前Model模型。

```c#
OrmUtils.Model("User").Like("Name", "wang%").Query();	//查询所有名称以wang开头的用户
```



##### public Model NotLike(string name, string value)

设置指定属性的NotLike查询条件。

###### 参数

name：属性名。

value：比较的值。

###### 返回

当前Model模型。



##### public Model IsNull(string name)

设置指定属性为Null查询条件。

###### 参数

name：属性名。

value：比较的值。

###### 返回

当前Model模型。



##### public Model IsNotNull(string name)

设置指定属性不为Null查询条件。

###### 参数

name：属性名。

value：比较的值。

###### 返回

当前Model模型。



##### pblic Model Between(string name, object value, object value2)

设置指定属性介于两个值之间的查询条件。

###### 参数

name：属性名。

value：比较的值。

###### 返回

当前Model模型。

```c#
OrmUtils.Model("User").Between("Name", 18, 25).Query();	//查询年龄在18到25之间的用户
```



##### public Model In(string name, params object[] values)

设置指定属性在指定值之间的查询条件

###### 参数

name：属性名

values：指定值，数组类型，可指定任意多个值。

###### 返回

当前Model模型

```c#
OrmUtils.Model("User").In("Name", "User1", "User5", "User32").Query();	//查询用户名为User1、User5、User32的数据
```



##### public Model NotIn(string name, params object[] values)

设置指定属性不在指定值之间的查询条件

###### 参数

name：属性名

values：指定值，数组类型，可指定任意多个值。

###### 返回

当前Model模型



#### 设置分页方法（支持链式调用）

##### public Model PageSize(int size)

设置查询分页的大小，即每页多少条数据，默认100。

###### 参数

size：每页的数据条数。

###### 返回

当前Model模型。



##### public Model PageIndex(int index)

设置查询分页的索引，即第几页，默认第1页。

###### 参数

index：查询的分页页码，默认1，页码必须>=1。

###### 返回

当前Model模型。

```c#
OrmUtils.Model("User").PageSize(20).PageIndex(2).Query();	//按照20条数据每页进行分页查询第2页的数据进行返回
```



##### public Model Top(int num)

设置查询符合条件的头部数据。

###### 参数

num：设置返回多少条数据。

###### 返回

当前Model模型。

```c#
OrmUtils.Model("User").Equals("Age", 18).Top(10).Query();	//查询年龄为18的前10名用户
```



#### 设置排序方法（支持链式调用）

##### public Model AscendingSort(string name)

在查询时，对指定属性按照升序进行排序。

###### 参数

namge：属性名。

###### 返回

当前Model模型。

```c#
OrmUtils.Model("User").DescendingSort("Age").Query();	//对年龄按照降序进行排序并返回所有查询结果
```

注：可以使用主从模型属性进行排序，如按照所属机构名称对用户进行排序：

```c#
OrmUtils.Model("User").DescendingSort("OrgId.Name").Query();
```



##### public Model DescendingSort(string name)

在查询时，对指定属性按照降序进行排序。

###### 参数

namge：属性名。

###### 返回

当前Model模型。
