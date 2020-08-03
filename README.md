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

将模型定义对应的物理表和数据全部删除。

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

#### 模型定义相关方法

public Property GetProperty(string name)

public Property GetPropertyByField(string field)

public Property GetProperty(int index)

public Property GetPrimaryKey(int index)

public string ToString()



####  行为操作方法

##### public bool CreateTable(bool force = false)

创建模型的物理表

###### 参数

force：是否覆盖已有物理表，重新创建，默认False。

###### 返回

成功返回True；否则返回False。

```c#
OrmUtils.Model("User").CreateTable();	//创建模型User的物理表
```



##### public bool RemoveTable()

删除模型对应的物理表和数据。

###### 返回

成功返回True；否则返回False。

```c#
OrmUtils.Model("User").RemoveTable();	//删除模型User的物理表
```



##### public bool TruncateTable()

清空模型对应的物理表数据，同时重置自增ID。

###### 返回

成功返回True；否则返回False。

```c#
OrmUtils.Model("User").TruncateTable();		//清空模型User的物理表数据
```



##### public bool Save()

新增保存模型数据到物理表。

###### 返回

成功返回True；否则返回False。

```c#
OrmUtils.Model("User").SetValue("Name", "wangxm").SetValue("Age", 18).Save();	//新建一个名称为wangxm年龄为18的新模型并保存到物理表
```



##### public bool Update(bool updateAll = false)

更新模型数据到物理表。

###### 参数

updateAll：是否更新物理表中的所有数据，默认为False，必须通过查询条件设置方法限定更新范围，如果确实需要更新所有数据，可以设置成True，慎用。

###### 返回

成功返回True；否则返回False。

```c#
OrmUtils.Model("User").SetValue("Age", 20).Equals("Name", "wangxm").Update();	//修改用户wangxm的年龄为20
```



##### public bool Delete(bool deleteAll = false)

删除物理表中的模型数据。

###### 参数

deleteAll：是否删除物理表中的所有数据，默认为False，必须通过查询条件设置方法限定删除范围，如果确实需要删除所有数据，可以设置成True，慎用。

###### 返回

成功返回True；否则返回False。

```c#
OrmUtils.Model("User").Equals("Name", "wangxm").Delete();	//删除用户wangxm
```



##### public List<dynamic> Query()

查询并返回模型物理表中的数据。

###### 返回

返回模型数据的列表，模型数据对象为dynamic对象，包含的属性由GetValue方法确定，如果为通过GetValue设置，则包括模型所有属性。

```c#
List<dynamic> result = OrmUtils.Model("User").Equals("Name", "wangxm").Query();
dynamic userWxm = result[0];
Console.WriteLine(userWxm.Age);
```



##### public long Count()

计算物理表中的数据条数并返回。

###### 返回

数据条数。

```c#
OrmUtils.Model("User").Count();		//计算用户表数据总数
OrmUtils.Model("User").Equals("Age", 20).Count();	//计算所有年龄为20的用户总数
```



##### public bool Exists()

判断物理表中的数据是否存在。

###### 返回

存在返回True；否则返回False。

```c#
OrmUtils.Model("User").Equals("Name", "wangxm").Exists();	//判断名为为wangxm的用户是否存在
```



#### 属性设置方法

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



#### 设置返回属性方法

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



#### 查询条件设置方法（支持链式调用）

##### public Model And(IFilter subCondition)

##### public Model Or(IFilter subCondition)

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



#### 分页设置方法（支持链式调用）

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



#### 排序设置方法（支持链式调用）

##### public Model AscendingSort(string name)

在查询时，对指定属性按照升序进行排序。

###### 参数

namge：属性名。

###### 返回

当前Model模型。

```c#
OrmUtils.Model("User").DescendingSort("Age").Query();	//对年龄按照降序进行排序并返回所有查询结果
```



##### public Model DescendingSort(string name)

在查询时，对指定属性按照降序进行排序。

###### 参数

namge：属性名。

###### 返回

当前Model模型。