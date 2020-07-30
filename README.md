# netcoreORM
.NetCore轻量级ORM框架

## 一、概述

###### TODO



## 二、快速入门

###### TODO



## 三、数据库连接

###### TODO



## 四、模型定义

模型定义采用xml格式，扩展名必须是.model.xml，每个文件支持定义一个模型。

模型定义文件必须放在ModelPath指定的路径下，可以是子目录，不限层级。

模型中可定义任意多个属性。具体格式如下：

```xml
<?xml version="1.0" encoding="utf-8" ?>
<model name="User" table="t_user">
    <property name="Id" field="f_id" notNull="True" primary="true" autoIncrement="true" joinInsert="false" jojnUpdate="false" desc="主键"/>
    <property name="Name" field="f_name" length="32" notNull="true" uniqueGroup="uc_name" joinInsert="true" joinUpdate="true" desc="名称" />
    <property name="Age" field="f_age" type="UInt16" unsigned="true" joinInsert="true" joinUpdate="true" desc="年龄" />
    <property name="Birthday" type="DateTime" field="f_birthday" fieldType="Date" desc="出生日期" />
    <property name="Deposit" field="f_deposit" type="Decimal" precision="2" desc="银行存款" />
    <property name="IsAdmin" field="f_is_admin" type="Boolean" desc="是否超级管理员" />
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

###### fieldType

属性对应物理表字段的类型，

###### length

整数型，指定属性的长度，默认0，即不指定，使用数据库默认设置，可选。

###### precision

整数型，指浮点数的小数位数，默认0，可选。

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

###### joinInsert

布尔型，指示属性是否参与模型的插入操作，默认True；可选。

###### joinUpdate

布尔型，指示属性是否参与模型的修改操作，默认True；可选。



## 五、API使用

### 1. OrmUtils类

OrmUtils类是所有功能的入口，OrmUtils属于静态类，所有属性和方法可直接使用，简单方便。



### 2. OrmUtils类属性

###### ModelPath

模型定义文件存储目录，如果未设置，默认当前程序的根目录下的models子目录。



### 3. OrmUtils类方法

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



##### public static bool CreateTables(bool force = false);

根据模型定义生成对应的数据库物理表。

###### 参数

force：是否覆盖已有物理表，重新创建，默认False。

###### 返回

创建成功返回True；否则，返回False。

```c#
OrmUtils.CreateTables();
```



##### public static bool RemoveTables();

将模型定义对应的物理表全部删除。

###### 返回

删除成功返回True；否则，返回False。

```c#
OrmUtils.RemoveTables();
```



##### public static bool TruncateTables();

清空物理表数据，同时重置自增ID。

###### 返回

清空成功返回True；否则，返回False。

```c#
OrmUtils.TruncateTables();
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



## 六、模型用法

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

public Property GetProperty(string name)

public Property GetPropertyByField(string field)

public Property GetProperty(int index)

public Property GetPrimaryKey(int index)

public string ToString()